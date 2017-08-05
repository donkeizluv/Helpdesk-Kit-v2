using System.Globalization;
using System.Windows.Controls;

namespace HelpdeskKit.Rules
{
    public class VadidateSearchString : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string input = (value ?? "").ToString().Trim();
            if (string.IsNullOrEmpty(input)) return ValidationResult.ValidResult;
            //check for invalid char in input
            if (true)
            {
                return new ValidationResult(true, "User found");
            }
            else
            {
                return new ValidationResult(false, "User not found");
            }
        }
    }
}