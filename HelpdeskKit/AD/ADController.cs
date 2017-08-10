using System;
using System.Collections;
using System.Data;
using System.DirectoryServices;
using System.Text;
using HelpdeskKit.Logger;

namespace HelpdeskKit.AD
{
    public class AdController : IDisposable
    {
        private DirectoryEntry _oDe;
        private DirectoryEntry _oDec;
        private DataSet _oDs;
        private DirectorySearcher _oDS;
        private DataSet _oDsUser;
        private DataRow _oNewCustomersRow;
        private SearchResultCollection _oResults;
        private DataRow _oRwResult;
        private DataRow _oRwUser;
        private DataTable _oTb;
        private ILogger _logger = LogManager.GetLogger(typeof(AdController));

        public AdController(string userName, string pwd)
        {
            sADUser = userName;
            sADPassword = pwd;
            var result = Login(userName, pwd);
            if (result != LoginResult.LOGIN_OK)
                throw new UnauthorizedAccessException("Fail to login, reason: " + result);
        }

        #region Private Variables

        private string sADPath = "LDAP://prd-vn-ad05.sgvf.sgcf";
        private readonly string sADPathPrefix = "";
        private string sADUser = string.Empty;
        private string sADPassword = string.Empty;
        private string sADServer = "prd-vn-ad05.sgvf.sgcf";
        private string sCharactersToTrim = string.Empty;

        #endregion Private Variables

        #region Enumerations

        public enum ADAccountOptions
        {
            UF_TEMP_DUPLICATE_ACCOUNT = 0x0100,
            UF_NORMAL_ACCOUNT = 0x0200,
            UF_INTERDOMAIN_TRUST_ACCOUNT = 0x0800,
            UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,
            UF_SERVER_TRUST_ACCOUNT = 0x2000,
            UF_DONT_EXPIRE_PASSWD = 0x10000,
            UF_SCRIPT = 0x0001,
            UF_ACCOUNTDISABLE = 0x0002,
            UF_HOMEDIR_REQUIRED = 0x0008,
            UF_LOCKOUT = 0x0010,
            UF_PASSWD_NOTREQD = 0x0020,
            UF_PASSWD_CANT_CHANGE = 0x0040,
            UF_ACCOUNT_LOCKOUT = 0X0010,
            UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0X0080,
            UF_EXPIRE_USER_PASSWORD = 0x800000
        }

        public enum GroupType : uint
        {
            UniversalGroup = 0x08,
            DomainLocalGroup = 0x04,
            GlobalGroup = 0x02,
            SecurityGroup = 0x80000000
        }

        public enum LoginResult
        {
            LOGIN_OK = 0,
            LOGIN_USER_DOESNT_EXIST,
            LOGIN_USER_ACCOUNT_INACTIVE
        }

        #endregion Enumerations

        #region Methods

        //Implement IDisposable.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (bDisposing)
            {
            }
            // Free your own state.
            // Set large fields to null.

            sADPath = null;
            sADUser = null;
            sADPassword = null;
            sADServer = null;
            sCharactersToTrim = null;

            _oDe = null;
            _oDec = null;
            _oDS = null;
            _oResults = null;
            _oDs = null;
            _oDsUser = null;
            _oTb = null;
            _oRwUser = null;
            _oRwResult = null;
            _oNewCustomersRow = null;
        }

        ~AdController()
        {
            //Simply call Dispose(false).
            Dispose(false);
        }

        #region Validate Methods

        public LoginResult Login(string sUserName, string sPassword)
        {
            //Check if the Logon exists Based on the Username and Password
            if (IsUserValid(sUserName, sPassword))
            {
                _oDe = GetUser(sUserName);
                if (_oDe != null)
                {
                    //Check the Account Status
                    int iUserAccountControl = Convert.ToInt32(_oDe.Properties["userAccountControl"][0]);
                    _oDe.Close();

                    //If the Disabled Item does not Exist then the Account is Active
                    if (!IsAccountActive(iUserAccountControl))
                        return LoginResult.LOGIN_USER_ACCOUNT_INACTIVE;
                    return LoginResult.LOGIN_OK;
                }
                return LoginResult.LOGIN_USER_DOESNT_EXIST;
            }
            return LoginResult.LOGIN_USER_DOESNT_EXIST;
        }

        public bool IsAccountActive(int iUserAccountControl)
        {
            int iUserAccountControl_Disabled = Convert.ToInt32(ADAccountOptions.UF_ACCOUNTDISABLE);
            int iFlagExists = iUserAccountControl & iUserAccountControl_Disabled;

            //If a Match is Found, then the Disabled Flag Exists within the Control Flags
            if (iFlagExists > 0)
                return false;
            return true;
        }

        public bool IsAccountActive(string sUserName)
        {
            _oDe = GetUser(sUserName);
            if (_oDe != null)
            {
                //to check of the Disabled option exists.
                int iUserAccountControl = Convert.ToInt32(_oDe.Properties["userAccountControl"][0]);
                _oDe.Close();

                //Check if the Disabled Item does not Exist then the Account is Active
                if (!IsAccountActive(iUserAccountControl))
                    return false;
                return true;
            }
            return false;
        }

        public bool IsUserValid(string sUserName, string sPassword)
        {
            try
            {
                _oDe = GetUser(sUserName, sPassword);
                _oDe.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion Validate Methods

        #region Search Methods

        public DirectoryEntry GetUserByHRCode(string hrCode)
        {
            _oDe = GetDirectoryObject();
            _oDS = new DirectorySearcher();
            _oDS.SearchRoot = _oDe;
            _oDS.Filter = string.Format("(&(objectClass=user)(description=*{0}))", hrCode);
            _oDS.SearchScope = SearchScope.Subtree;
            _oDS.PageSize = 10000;
            //debug
            //var allResult = oDS.FindAll();
            //_logger.Log(allResult.Count.ToString());

            var oResults = _oDS.FindOne();

            if (oResults != null)
            {
                _oDe = new DirectoryEntry(oResults.Path, sADUser, sADPassword, AuthenticationTypes.Secure);
                return _oDe;
            }
            return null;
        }

        public DirectoryEntry GetUser(string sUserName)
        {
            //Create an Instance of the DirectoryEntry
            _oDe = GetDirectoryObject();

            //Create Instance fo the Direcory Searcher
            _oDS = new DirectorySearcher();

            _oDS.SearchRoot = _oDe;
            //Set the Search Filter
            _oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
            _oDS.SearchScope = SearchScope.Subtree;
            _oDS.PageSize = 10000;

            //Find the First Instance
            var oResults = _oDS.FindOne();

            //If found then Return Directory Object, otherwise return Null
            if (oResults != null)
            {
                _oDe = new DirectoryEntry(oResults.Path, sADUser, sADPassword, AuthenticationTypes.Secure);
                return _oDe;
            }
            return null;
        }

        public DirectoryEntry GetUser(string sUserName, string sPassword)
        {
            //Create an Instance of the DirectoryEntry
            _oDe = GetDirectoryObject(sUserName, sPassword);

            //Create Instance fo the Direcory Searcher
            _oDS = new DirectorySearcher();
            _oDS.SearchRoot = _oDe;

            //Set the Search Filter
            _oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
            _oDS.SearchScope = SearchScope.Subtree;
            _oDS.PageSize = 10000;

            //Find the First Instance
            var oResults = _oDS.FindOne();

            //If a Match is Found, Return Directory Object, Otherwise return Null
            if (oResults != null)
            {
                _oDe = new DirectoryEntry(oResults.Path, sADUser, sADPassword, AuthenticationTypes.Secure);
                return _oDe;
            }
            return null;
        }

        public DataSet GetUserDataSet(string sUserName)
        {
            _oDe = GetDirectoryObject();

            //Create Instance fo the Direcory Searcher
            _oDS = new DirectorySearcher();
            _oDS.SearchRoot = _oDe;

            //Set the Search Filter
            _oDS.Filter = "(&(objectClass=user)(sAMAccountName=" + sUserName + "))";
            _oDS.SearchScope = SearchScope.Subtree;
            _oDS.PageSize = 10000;

            //Find the First Instance
            var oResults = _oDS.FindOne();

            //Create Empty User Dataset
            _oDsUser = CreateUserDataSet();

            //If Record is not Null, Then Populate DataSet
            if (oResults != null)
            {
                _oNewCustomersRow = _oDsUser.Tables["User"].NewRow();
                _oNewCustomersRow = PopulateUserDataSet(oResults, _oDsUser.Tables["User"]);

                _oDsUser.Tables["User"].Rows.Add(_oNewCustomersRow);
            }
            _oDe.Close();

            return _oDsUser;
        }

        public DataSet GetUsersDataSet(string sCriteria)
        {
            _oDe = GetDirectoryObject();

            //Create Instance fo the Direcory Searcher
            _oDS = new DirectorySearcher();
            _oDS.SearchRoot = _oDe;

            //Set the Search Filter
            _oDS.Filter = "(&(objectClass=user)(objectCategory=person)(" + sCriteria + "))";
            _oDS.SearchScope = SearchScope.Subtree;
            _oDS.PageSize = 10000;

            //Find the First Instance
            _oResults = _oDS.FindAll();

            //Create Empty User Dataset
            _oDsUser = CreateUserDataSet();

            //If Record is not Null, Then Populate DataSet
            try
            {
                if (_oResults.Count > 0)
                    foreach (SearchResult oResult in _oResults)
                        _oDsUser.Tables["User"].Rows.Add(PopulateUserDataSet(oResult, _oDsUser.Tables["User"]));
            }
            catch
            {
            }

            _oDe.Close();
            return _oDsUser;
        }

        #endregion Search Methods

        #region User Account Methods

        public void SetUserPassword(DirectoryEntry oDE, string sPassword, out string sMessage)
        {
            try
            {
                //Set The new Password
                oDE.Invoke("SetPassword", sPassword);
                sMessage = "";

                oDE.CommitChanges();
                oDE.Close();
            }
            catch (Exception ex)
            {
                sMessage = ex.Message;
                sMessage += ex.InnerException?.Message ?? string.Empty;
            }
        }

        public void EnableUserAccount(string sUserName)
        {
            //Get the Directory Entry fot the User and Enable the Password
            EnableUserAccount(GetUser(sUserName));
        }

        public void EnableUserAccount(DirectoryEntry oDE)
        {
            oDE.Properties["userAccountControl"][0] = ADAccountOptions.UF_NORMAL_ACCOUNT;
            oDE.CommitChanges();
            oDE.Close();
        }

        public void ExpireUserPassword(DirectoryEntry oDE)
        {
            //Set the Password Last Set to 0, this will Expire the Password
            oDE.Properties["pwdLastSet"][0] = 0;
            oDE.CommitChanges();
            oDE.Close();
        }

        public void DisableUserAccount(string sUserName)
        {
            DisableUserAccount(GetUser(sUserName));
        }

        public void DisableUserAccount(DirectoryEntry oDE)
        {
            //oDE.Properties["userAccountControl"][0] = ADAccountOptions.UF_NORMAL_ACCOUNT |
            //                                          ADAccountOptions.UF_DONT_EXPIRE_PASSWD |
            //                                          ADAccountOptions.UF_ACCOUNTDISABLE;
            oDE.Properties["userAccountControl"][0] = ADAccountOptions.UF_NORMAL_ACCOUNT |
                                                      ADAccountOptions.UF_ACCOUNTDISABLE;
            oDE.CommitChanges();
            oDE.Close();
        }

        public void MoveUserAccount(DirectoryEntry oDE, string sNewOUPath)
        {
            DirectoryEntry myNewPath = null;
            //Define the new Path
            myNewPath = new DirectoryEntry("LDAP://" + sADServer + "/" + sNewOUPath, sADUser, sADPassword,
                AuthenticationTypes.Secure);

            oDE.MoveTo(myNewPath);
            oDE.CommitChanges();
            oDE.Close();
        }

        public DateTime GetPasswordExpirationDate(DirectoryEntry oDE)
        {
            return (DateTime)oDE.InvokeGet("PasswordExpirationDate");
        }

        public bool IsAccountLocked(DirectoryEntry oDE)
        {
            return Convert.ToBoolean(oDE.InvokeGet("IsAccountLocked"));
        }

        public bool UnlockUserAccount(DirectoryEntry oDE)
        {
            SetProperty(oDE, "lockoutTime", "0", out string ex);
            return ex == string.Empty;
        }

        public bool IsUserExpired(DirectoryEntry oDE)
        {
            int iDecimalValue = int.Parse(GetProperty(oDE, "userAccountControl"));
            string sBinaryValue = Convert.ToString(iDecimalValue, 2);

            //Reverse the Binary Value to get the Location for all 1's
            var str = sBinaryValue.ToCharArray();
            Array.Reverse(str);
            string sBinaryValueReversed = new string(str);

            //24th 1 is the Switch for the Expired Account
            if (sBinaryValueReversed.Length >= 24)
            {
                if (sBinaryValueReversed.Substring(24, 1) == "1")
                    return true;
                return false;
            }
            return false;
        }

        public DirectoryEntry CreateNewUser(string sCN)
        {
            //Set the LDAP Path so that the user will be Created under the Users Container
            string LDAPDomain = "/CN=Users," + GetLDAPDomain();

            _oDe = GetDirectoryObject();
            _oDec = _oDe.Children.Add("CN=" + sCN, "user");
            _oDe.Close();
            return _oDec;
        }

        public DirectoryEntry CreateNewUser(string sUserName, string sLDAPDomain)
        {
            //Set the LDAP qualification so that the user will be Created under the Users Container
            string LDAPDomain = "/CN=Users," + sLDAPDomain;
            _oDe = new DirectoryEntry("LDAP://" + sADServer + "/" + sLDAPDomain, sADUser, sADPassword,
                AuthenticationTypes.Secure);

            _oDec = _oDe.Children.Add("CN=" + sUserName, "user");
            _oDe.Close();
            return _oDec;
        }

        public bool DeleteUser(string sUserName)
        {
            string sParentPath = GetUser(sUserName).Parent.Path;
            return DeleteUser(sUserName, sParentPath);
        }

        public bool DeleteUser(string sUserName, string sParentPath)
        {
            try
            {
                _oDe = new DirectoryEntry(sParentPath, sADUser, sADPassword, AuthenticationTypes.Secure);

                _oDe.Children.Remove(GetUser(sUserName));

                _oDe.CommitChanges();
                _oDe.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion User Account Methods

        #region Group Methods

        public DirectoryEntry CreateNewGroup(string sOULocation, string sGroupName, string sDescription,
            GroupType oGroupTypeInput, bool bSecurityGroup)
        {
            GroupType oGroupType;

            _oDe = new DirectoryEntry("LDAP://" + sADServer + "/" + sOULocation, sADUser, sADPassword,
                AuthenticationTypes.Secure);

            //Check if the Requested group is a Security Group or Distribution Group
            if (bSecurityGroup)
                oGroupType = oGroupTypeInput | GroupType.SecurityGroup;
            else
                oGroupType = oGroupTypeInput;
            int typeNum = (int)oGroupType;

            //Add Properties to the Group
            var myGroup = _oDe.Children.Add("cn=" + sGroupName, "group");
            myGroup.Properties["sAMAccountName"].Add(sGroupName);
            myGroup.Properties["description"].Add(sDescription);
            myGroup.Properties["groupType"].Add(typeNum);
            myGroup.CommitChanges();

            return myGroup;
        }

        public void AddUserToGroup(string sDN, string sGroupDN)
        {
            _oDe = new DirectoryEntry("LDAP://" + sADServer + "/" + sGroupDN, sADUser, sADPassword,
                AuthenticationTypes.Secure);

            //Adds the User to the Group
            _oDe.Properties["member"].Add(sDN);
            _oDe.CommitChanges();
            _oDe.Close();
        }

        public void RemoveUserFromGroup(string sDN, string sGroupDN)
        {
            _oDe = new DirectoryEntry("LDAP://" + sADServer + "/" + sGroupDN, sADUser, sADPassword,
                AuthenticationTypes.Secure);

            //Removes the User to the Group
            _oDe.Properties["member"].Remove(sDN);
            _oDe.CommitChanges();
            _oDe.Close();
        }

        public bool IsUserGroupMember(string sDN, string sGroupDN)
        {
            _oDe = new DirectoryEntry("LDAP://" + sADServer + "/" + sDN, sADUser, sADPassword, AuthenticationTypes.Secure);

            string sUserName = GetProperty(_oDe, "sAMAccountName");

            var oUserGroups = GetUserGroups(sUserName);
            int iGroupsCount = oUserGroups.Count;

            if (iGroupsCount != 0)
            {
                for (int i = 0; i < iGroupsCount; i++)
                    if (sGroupDN == oUserGroups[i].ToString())
                        return true;
                return false;
            }
            return false;
        }

        public ArrayList GetUserGroups(string sUserName)
        {
            var oGroupMemberships = new ArrayList();
            return AttributeValuesMultiString("memberOf", sUserName, oGroupMemberships);
        }

        #endregion Group Methods

        #region Helper Methods

        public string GetProperty(DirectoryEntry oDE, string sPropertyName)
        {
            if (oDE.Properties.Contains(sPropertyName))
                return oDE.Properties[sPropertyName][0].ToString();
            return string.Empty;
        }

        public string GetUserName(DirectoryEntry oDE)
        {
            return oDE.Properties["sAMAccountName"].Value.ToString();
        }

        public ArrayList GetProperty_Array(DirectoryEntry oDE, string sPropertyName)
        {
            var myItems = new ArrayList();
            if (oDE.Properties.Contains(sPropertyName))
            {
                for (int i = 0; i < oDE.Properties[sPropertyName].Count; i++)
                    myItems.Add(oDE.Properties[sPropertyName][i].ToString());
                return myItems;
            }
            return myItems;
        }

        public byte[] GetProperty_Byte(DirectoryEntry oDE, string sPropertyName)
        {
            if (oDE.Properties.Contains(sPropertyName))
                return (byte[])oDE.Properties[sPropertyName].Value;
            return null;
        }

        public string GetProperty(SearchResult oSearchResult, string sPropertyName)
        {
            if (oSearchResult.Properties.Contains(sPropertyName))
                return oSearchResult.Properties[sPropertyName][0].ToString();
            return string.Empty;
        }

        public bool ClearProperty(DirectoryEntry oDE, string sPropertyName, out string ex)
        {
            ex = string.Empty;
            try
            {
                if (oDE.Properties.Contains(sPropertyName))
                {
                    oDE.Properties[sPropertyName].Clear();
                    oDE.CommitChanges();
                    oDE.Close();
                    return true;
                }
            }
            catch (Exception e)
            {
                ex = e.Message;
                return false;
            }
            return false;
        }

        public bool SetProperty(DirectoryEntry oDE, string sPropertyName, string sPropertyValue, out string ex)
        {
            //Check if the Value is Valid
            ex = string.Empty;
            if (sPropertyValue != string.Empty)
                try
                {
                    if (oDE.Properties.Contains(sPropertyName))
                    {
                        oDE.Properties[sPropertyName].Value = sPropertyValue;
                        oDE.CommitChanges();
                        oDE.Close();
                    }
                    else
                    {
                        oDE.Properties[sPropertyName].Add(sPropertyValue);
                        oDE.CommitChanges();
                        oDE.Close();
                    }
                    return true;
                }
                catch (Exception e)
                {
                    ex = e.Message;
                    return false;
                }
            return ClearProperty(oDE, sPropertyName, out ex);
        }

        public void SetProperty(DirectoryEntry oDE, string sPropertyName, byte[] bPropertyValue)
        {
            //Clear Binary Data if Exists
            oDE.Properties[sPropertyName].Clear();

            //Update Attribute with Binary Data from File
            oDE.Properties[sPropertyName].Add(bPropertyValue);
            oDE.CommitChanges();
            oDE.Dispose();
        }

        public void SetProperty(DirectoryEntry oDE, string sPropertyName, ArrayList aPropertyValue)
        {
            //Check if the Value is Valid
            if (aPropertyValue.Count != 0)
                foreach (string sPropertyValue in aPropertyValue)
                {
                    oDE.Properties[sPropertyName].Add(sPropertyValue);
                    oDE.CommitChanges();
                    oDE.Close();
                }
        }

        public void ClearProperty(DirectoryEntry oDE, string sPropertyName)
        {
            //Check if the Property Exists
            if (oDE.Properties.Contains(sPropertyName))
            {
                oDE.Properties[sPropertyName].Clear();
                oDE.CommitChanges();
                oDE.Close();
            }
        }

        private DirectoryEntry GetDirectoryObject()
        {
            _oDe = new DirectoryEntry(sADPath, sADUser, sADPassword, AuthenticationTypes.Secure);
            return _oDe;
        }

        private DirectoryEntry GetDirectoryObject(string sUserName, string sPassword)
        {
            _oDe = new DirectoryEntry(sADPath, sUserName, sPassword, AuthenticationTypes.Secure);
            return _oDe;
        }

        private DirectoryEntry GetDirectoryObject(string sDomainReference)
        {
            _oDe = new DirectoryEntry(sADPath + sDomainReference, sADUser, sADPassword, AuthenticationTypes.Secure);
            return _oDe;
        }

        public DirectoryEntry GetDirectoryObject_ByPath(string sPath)
        {
            _oDe = new DirectoryEntry(sADPathPrefix + sPath, sADUser, sADPassword, AuthenticationTypes.Secure);
            return _oDe;
        }

        private DirectoryEntry GetDirectoryObject(string sDomainReference, string sUserName, string sPassword)
        {
            _oDe = new DirectoryEntry(sADPath + sDomainReference, sUserName, sPassword, AuthenticationTypes.Secure);
            return _oDe;
        }

        public string GetDistinguishedName(DirectoryEntry oDE)
        {
            if (oDE.Properties.Contains("distinguishedName"))
                return oDE.Properties["distinguishedName"][0].ToString();
            return string.Empty;
        }

        public string GetDistinguishedName(string sUserName)
        {
            _oDe = GetUser(sUserName);

            if (_oDe.Properties.Contains("distinguishedName"))
                return _oDe.Properties["distinguishedName"][0].ToString();
            return string.Empty;
        }

        public ArrayList AttributeValuesMultiString(string sAttributeName, string sUserName, ArrayList oValuesCollection)
        {
            _oDe = GetUser(sUserName);

            var oValueCollection = _oDe.Properties[sAttributeName];
            var oIEn = oValueCollection.GetEnumerator();

            while (oIEn.MoveNext())
                if (oIEn.Current != null)
                    if (!oValuesCollection.Contains(oIEn.Current.ToString()))
                        oValuesCollection.Add(oIEn.Current.ToString());
            _oDe.Close();
            _oDe.Dispose();
            return oValuesCollection;
        }

        #endregion Helper Methods

        #region Internal Methods

        private string GetLDAPDomain()
        {
            var LDAPDomain = new StringBuilder();
            var LDAPDC = sADServer.Split('.');

            for (int i = 0; i < LDAPDC.GetUpperBound(0) + 1; i++)
            {
                LDAPDomain.Append("DC=" + LDAPDC[i]);
                if (i < LDAPDC.GetUpperBound(0))
                    LDAPDomain.Append(",");
            }
            return LDAPDomain.ToString();
        }

        private DataSet CreateUserDataSet()
        {
            _oDs = new DataSet();

            _oTb = _oDs.Tables.Add("User");

            //Create All the Columns
            _oTb.Columns.Add("company");
            _oTb.Columns.Add("department");
            _oTb.Columns.Add("description");
            _oTb.Columns.Add("displayName");
            _oTb.Columns.Add("facsimileTelephoneNumber");
            _oTb.Columns.Add("givenName");
            _oTb.Columns.Add("homePhone");
            _oTb.Columns.Add("employeeNumber");
            _oTb.Columns.Add("initials");
            _oTb.Columns.Add("ipPhone");
            _oTb.Columns.Add("l");
            _oTb.Columns.Add("mail");
            _oTb.Columns.Add("manager");
            _oTb.Columns.Add("mobile");
            _oTb.Columns.Add("name");
            _oTb.Columns.Add("pager");
            _oTb.Columns.Add("physicalDeliveryOfficeName");
            _oTb.Columns.Add("postalAddress");
            _oTb.Columns.Add("postalCode");
            _oTb.Columns.Add("postOfficeBox");
            _oTb.Columns.Add("sAMAccountName");
            _oTb.Columns.Add("sn");
            _oTb.Columns.Add("st");
            _oTb.Columns.Add("street");
            _oTb.Columns.Add("streetAddress");
            _oTb.Columns.Add("telephoneNumber");
            _oTb.Columns.Add("title");
            _oTb.Columns.Add("userPrincipalName");
            _oTb.Columns.Add("wWWHomePage");
            _oTb.Columns.Add("whenCreated");
            _oTb.Columns.Add("whenChanged");
            _oTb.Columns.Add("distinguishedName");
            _oTb.Columns.Add("info");

            return _oDs;
        }

        private DataSet CreateGroupDataSet(string sTableName)
        {
            _oDs = new DataSet();

            _oTb = _oDs.Tables.Add(sTableName);

            //Create all the Columns
            _oTb.Columns.Add("distinguishedName");
            _oTb.Columns.Add("name");
            _oTb.Columns.Add("friendlyname");
            _oTb.Columns.Add("description");
            _oTb.Columns.Add("domainType");
            _oTb.Columns.Add("groupType");
            _oTb.Columns.Add("groupTypeDesc");

            return _oDs;
        }

        private DataRow PopulateUserDataSet(SearchResult oUserSearchResult, DataTable oUserTable)
        {
            //Sets a New Empty Row
            _oRwUser = oUserTable.NewRow();

            _oRwUser["company"] = GetProperty(oUserSearchResult, "company");
            _oRwUser["department"] = GetProperty(oUserSearchResult, "department");
            _oRwUser["description"] = GetProperty(oUserSearchResult, "description");
            _oRwUser["displayName"] = GetProperty(oUserSearchResult, "displayName");
            _oRwUser["facsimileTelephoneNumber"] = GetProperty(oUserSearchResult, "facsimileTelephoneNumber");
            _oRwUser["givenName"] = GetProperty(oUserSearchResult, "givenName");
            _oRwUser["homePhone"] = GetProperty(oUserSearchResult, "homePhone");
            _oRwUser["employeeNumber"] = GetProperty(oUserSearchResult, "employeeNumber");
            _oRwUser["initials"] = GetProperty(oUserSearchResult, "initials");
            _oRwUser["ipPhone"] = GetProperty(oUserSearchResult, "ipPhone");
            _oRwUser["l"] = GetProperty(oUserSearchResult, "l");
            _oRwUser["mail"] = GetProperty(oUserSearchResult, "mail");
            _oRwUser["manager"] = GetProperty(oUserSearchResult, "manager");
            _oRwUser["mobile"] = GetProperty(oUserSearchResult, "mobile");
            _oRwUser["name"] = GetProperty(oUserSearchResult, "name");
            _oRwUser["pager"] = GetProperty(oUserSearchResult, "pager");
            _oRwUser["physicalDeliveryOfficeName"] = GetProperty(oUserSearchResult, "physicalDeliveryOfficeName");
            _oRwUser["postalAddress"] = GetProperty(oUserSearchResult, "postalAddress");
            _oRwUser["postalCode"] = GetProperty(oUserSearchResult, "postalCode");
            _oRwUser["postOfficeBox"] = GetProperty(oUserSearchResult, "postOfficeBox");
            _oRwUser["sAMAccountName"] = GetProperty(oUserSearchResult, "sAMAccountName");
            _oRwUser["sn"] = GetProperty(oUserSearchResult, "sn");
            _oRwUser["st"] = GetProperty(oUserSearchResult, "st");
            _oRwUser["street"] = GetProperty(oUserSearchResult, "street");
            _oRwUser["streetAddress"] = GetProperty(oUserSearchResult, "streetAddress");
            _oRwUser["telephoneNumber"] = GetProperty(oUserSearchResult, "telephoneNumber");
            _oRwUser["title"] = GetProperty(oUserSearchResult, "title");
            _oRwUser["userPrincipalName"] = GetProperty(oUserSearchResult, "userPrincipalName");
            _oRwUser["wWWHomePage"] = GetProperty(oUserSearchResult, "wWWHomePage");
            _oRwUser["whenCreated"] = GetProperty(oUserSearchResult, "whenCreated");
            _oRwUser["whenChanged"] = GetProperty(oUserSearchResult, "whenChanged");
            _oRwUser["distinguishedName"] = GetProperty(oUserSearchResult, "distinguishedName");
            _oRwUser["info"] = GetProperty(oUserSearchResult, "info");

            return _oRwUser;
        }

        private DataRow PopulateGroupDataSet(SearchResult oSearchResult, DataTable oTable)
        {
            //Sets a New Empty Row
            _oRwResult = oTable.NewRow();

            string sFullOU = GetProperty(oSearchResult, "distinguishedName");
            var splita = sCharactersToTrim.Split(';');
            foreach (string sa in splita)
                sFullOU = sFullOU.Replace(sa, "");

            string sDisplayName = "";
            string sRawString = "";
            var split1 = sFullOU.Split(',');
            foreach (string s1 in split1)
            {
                sRawString = s1;
                sRawString = sRawString.Replace("OU=", "");
                sRawString = sRawString.Replace("DC=", "");
                sRawString = sRawString.Replace("CN=", "");
                sDisplayName = sRawString + "/" + sDisplayName;
            }

            _oRwResult["distinguishedName"] = GetProperty(oSearchResult, "distinguishedName");
            _oRwResult["name"] = GetProperty(oSearchResult, "name");
            _oRwResult["friendlyname"] = sDisplayName.Substring(0, sDisplayName.Length - 1);
            ;
            _oRwResult["description"] = GetProperty(oSearchResult, "description");
            _oRwResult["domainType"] = sADServer;

            string sGroupType = GetProperty(oSearchResult, "groupType");
            _oRwResult["groupType"] = sGroupType;

            switch (sGroupType)
            {
                case "2":
                    _oRwResult["groupTypeDesc"] = "Global, Distribution";
                    break;

                case "4":
                    _oRwResult["groupTypeDesc"] = "Domain, Distribution";
                    break;

                case "8":
                    _oRwResult["groupTypeDesc"] = "Universal, Distribution";
                    break;

                case "-2147483640":
                    _oRwResult["groupTypeDesc"] = "Universal, Security";
                    break;

                case "-2147483646":
                    _oRwResult["groupTypeDesc"] = "Global, Security";
                    break;

                case "-2147483644":
                    _oRwResult["groupTypeDesc"] = "Domain, Security";
                    break;

                default:
                    _oRwResult["groupTypeDesc"] = "";
                    break;
            }

            return _oRwResult;
        }

        #endregion Internal Methods

        #endregion Methods
    }
}