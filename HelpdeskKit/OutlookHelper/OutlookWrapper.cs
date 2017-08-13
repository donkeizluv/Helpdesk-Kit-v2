using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Outlook;

namespace HelpdeskKit.OutlookHelper
{
    public class NewEnailEventArgs : EventArgs
    {
        public NewEnailEventArgs(MailItem mail)
        {
            Email = mail;
        }

        public MailItem Email { get; }
        public bool ContainAd { get; set; }
        public string Ad { get; set; } = string.Empty;
    }

    public delegate void IncommingEmailHandler(object sender, NewEnailEventArgs e);

    public class OutlookWrapper : IDisposable
    {
        //problem with accented chars cuz some stupid fucks like to put accents
        //private readonly string ADRegex = @"[A-z]+\.[A-z]+\-[A-z0-9]+";
        private const string AD_REGEX_STRING = @"\b\w+\.\w+\-\w+\b";

        public const string HelpdeskEmail = "helpdesk@hdsaison.com.vn";
        private readonly Regex _idRegex = new Regex(AD_REGEX_STRING);

        private readonly Application _app;
        private MAPIFolder _folder;
        private Items _items;

        private NameSpace _space;

        public OutlookWrapper()
        {
            //_app = GetOutlook();
            //if (_app == null)
            //    throw new NullReferenceException("Cant get outlook instance, probly not running.");

            _app = GetOutlook() ?? throw new NullReferenceException("Cant get outlook instance, probly not running.");
        }

        public void Dispose()
        {
            ReleaseCom(_app);
        }

        public event IncommingEmailHandler IncommingEmail;

        protected virtual void OnIncommingEmail(NewEnailEventArgs e)
        {
            IncommingEmail?.Invoke(this, e);
        }

        public string GetAdInSelectedEmail()
        {
            MailItem mail = null;
            try
            {
                mail = GetSelectedMailItem();
                if (mail == null) return string.Empty;
                string match = ExtractAd(mail.Body);
                return match;
            }
            finally
            {
                ReleaseCom(mail);
            }
        }

        private string ExtractAd(string content)
        {
            var match = _idRegex.Match(content);
            if (match.Success)
                return match.Value;
            return string.Empty;
        }

        public void StopMonitorIncomingEmail()
        {
            if (_items != null)
                _items.ItemAdd -= Items_ItemAdd;
            ReleaseCom(_items);
            ReleaseCom(_folder);
            ReleaseCom(_space);
            _space = null;
            _folder = null;
            _items = null;
        }

        private bool GetFolderEntryByName(string name, NameSpace space, out string entry)
        {
            entry = string.Empty;
            foreach (MAPIFolder folder in space.Folders)
                if (string.Compare(folder.Name.Replace(" ", string.Empty), name.Replace(" ", string.Empty), true) == 0)
                {
                    entry = folder.EntryID;
                    return true;
                }
            return false;
        }

        public void MonitorIncomingEmail(string mailBoxName, string folderName)
        {
            _space = _app.GetNamespace("MAPI");

            //get root folder entry id
            if (!GetFolderEntryByName(mailBoxName, _space, out var rootEntry))
                throw new ArgumentException($"root folder {mailBoxName} cant be found");

            //get folder entry id
            if (!GetFolderEntryID(folderName, _space.GetFolderFromID(rootEntry), out var entryId))
                throw new ArgumentException($"folder {folderName} cant be found");

            _folder = _space.GetFolderFromID(entryId);
            if (_folder == null)
                throw new ArgumentException($"folder entry {entryId} cant be found");
            _items = _folder.Items;
            _items.ItemAdd += Items_ItemAdd;
        }

        public bool GetFolderEntryID(string folderName, MAPIFolder rootFolder, out string entryID)
        {
            bool found = false;
            string id = string.Empty;

            void searchFolderEntryID(string name, MAPIFolder folder)
            {
                if (string.Compare(folder.Name, name, true) == 0)
                {
                    id = folder.EntryID;
                    found = true;
                    return;
                }
                foreach (MAPIFolder f in folder.Folders)
                {
                    if (found) break;
                    searchFolderEntryID(name, f);
                }
            }

            searchFolderEntryID(folderName, rootFolder);
            entryID = id;
            return found;
        }

        private void Items_ItemAdd(object Item)
        {
            var mail = (MailItem) Item;
            if (Item == null) return;
            if (mail.MessageClass != "IPM.Note") return;
            var eventAgrs = new NewEnailEventArgs(mail);
            string ad = ExtractAd(mail.Body);
            if (ad != string.Empty)
            {
                eventAgrs.ContainAd = true;
                eventAgrs.Ad = ad;
            }
            OnIncommingEmail(eventAgrs);
            //raise event
        }


        private MailItem GetSelectedMailItem()
        {
            foreach (var item in _app.ActiveExplorer().Selection)
                return (MailItem) item;
            throw new NullReferenceException("Cant get selected email.");
        }

        private MailItem ReplyAll(MailItem mail, string content)
        {
            try
            {
                var reply = mail.ReplyAll();
                switch (reply.BodyFormat)
                {
                    //case OlBodyFormat.olFormatPlain:
                    //    reply.Body = GetTemplate(mail.SenderName, content) + reply.Body;
                    //    break;

                    case OlBodyFormat.olFormatHTML:
                        reply.HTMLBody = GetTemplateHTML(mail.SenderName, content) + reply.HTMLBody;
                        break;

                    //case OlBodyFormat.olFormatRichText:
                    //    reply.HTMLBody = GetTemplateHTML(mail.SenderName, content) + reply.HTMLBody;
                    //    break;

                    default:
                        reply.Body = GetTemplate(mail.SenderName, content) + reply.Body;
                        break;
                }
                reply.SentOnBehalfOfName = HelpdeskEmail;
                return reply;
            }
            finally
            {
                ReleaseCom(mail);
            }
        }

        private static string GetTemplate(string senderName, string insertion)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format(
                "Dear {0}, \n {1} \n Regards, \n Helpdesk \n Tel : (08) 35516101 / 54137400 ext: 71111 \n Email: helpdesk@hdsaison.com.vn",
                senderName, insertion));
            return sb.ToString();
        }

        private static string GetTemplateHTML(string senderName, string insertion)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("<p>Dear {0},</p><br><p>{1}</p><br>", senderName, insertion));
            sb.Append("<p>Regards,</p>");
            sb.Append("<br><p>Helpdesk</p>");
            sb.Append("<p>Tel : (08) 35516101 / 54137400 ext: 71111</p>");
            sb.Append("<p>Email : helpdesk@hdsaison.com.vn</b></p>");
            return sb.ToString();
        }

        private void HandleReply(MailItem mail, bool send)
        {
            try
            {
                if (send)
                    mail.Send();
                else
                    mail.Display();
            }
            finally
            {
                ReleaseCom(mail);
                mail = null;
            }
        }

        public void Reply_NotActive(MailItem mail)
        {
            var reply = ReplyAll(mail, "Account nay da vo hieu hoa. Ban vui long lien he BDS, HR de xac nhan lai.");
            HandleReply(reply, true);
        }

        public void Reply_NotActive(bool send)
        {
            var mail = GetSelectedMailItem();
            var reply = ReplyAll(mail, "Account nay da vo hieu hoa. Ban vui long lien he BDS, HR de xac nhan lai.");
            HandleReply(reply, send);
        }

        public void Reply_UnlockOK(MailItem mail)
        {
            var reply = ReplyAll(mail, "Account da unlock, ban dang nhap lai can than.");
            HandleReply(reply, true);
        }

        public void Reply_UnlockOK(bool send)
        {
            var mail = GetSelectedMailItem();
            var reply = ReplyAll(mail, "Account da unlock, ban dang nhap lai can than.");
            HandleReply(reply, send);
        }

        public void Reply_NoLock(bool send)
        {
            var mail = GetSelectedMailItem();
            var reply = ReplyAll(mail, "Account khong bi lock, chup man hinh loi de duoc ho tro.");
            HandleReply(reply, send);
        }

        public void Reply_PwdExpired(bool send)
        {
            var mail = GetSelectedMailItem();
            var reply = ReplyAll(mail, "Mat khau da het han, ban vui long doi mat khau moi.");
            HandleReply(reply, send);
        }

        public void Reply_PwdExpired(MailItem mail)
        {
            var reply = ReplyAll(mail, "Mat khau da het han, ban vui long doi mat khau moi.");
            HandleReply(reply, true);
        }

        public void Reply_UserNotFound(bool send)
        {
            var mail = GetSelectedMailItem();
            var reply = ReplyAll(mail, "Khong tim thay user nay tren he thong, ban check lai giup.");
            HandleReply(reply, send);
        }

        public bool IsComInstanceValid()
        {
            try
            {
                var test = _app.ActiveExplorer();
                //ReleaseCom(test); //this will close outlook
                test = null;
                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }

        private static Application GetOutlook()
        {
            try
            {
                //outlook needs to be clicked on (foreground) once before this script works....weird :/
                //not reliable way to check
                var ol = (Application) Marshal.GetActiveObject("Outlook.Application");
                return ol;
            }
            catch (COMException ex) when (ex.HResult == -2147221021) //operation invalid
            {
                return null;
            }
        }

        private static void ReleaseCom(object obj)
        {
            if (obj != null && Marshal.IsComObject(obj))
                Marshal.ReleaseComObject(obj);
            obj = null;
        }
    }
}