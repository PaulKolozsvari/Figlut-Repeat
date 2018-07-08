namespace Figlut.Repeat.Data
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    #endregion //Using Directives

    public class DataEditor
    {
        #region Methods

        public static string GetTrimmedCellPhoneNumber(string originalCellPhoneNumber, int trimLength)
        {
            if(string.IsNullOrEmpty(originalCellPhoneNumber))
            {
                return originalCellPhoneNumber;
            }
            if (originalCellPhoneNumber.Length < 10)
            {
                if (originalCellPhoneNumber.Length > 2)
                {
                    throw new ArgumentException(string.Format("Invalid Cell Phone number starting with {0}. Must be at least 10 digits.", originalCellPhoneNumber.Substring(0, 2)));
                }
                else
                {
                    throw new ArgumentException(string.Format("Invalid Cell Phone number. Must be at least 10 digits."));
                }
            }
            string result =
                originalCellPhoneNumber.Length > trimLength ?
                string.Format("{0} ...", originalCellPhoneNumber.Substring(0, trimLength)) :
                originalCellPhoneNumber;
            return result;
        }

        public static string GetTrimmedMessageContents(string originalMessageContents, int trimLength)
        {
            if (string.IsNullOrEmpty(originalMessageContents))
            {
                return originalMessageContents;
            }
            string result = 
                originalMessageContents.Length > trimLength ?
                string.Format("{0} ...", originalMessageContents.Substring(0, trimLength)) :
                originalMessageContents;
            return result;
        }

        public static string GetTrimmedSmsErrorMessage(string originalErrorMessage, int trimLength)
        {
            if (string.IsNullOrEmpty(originalErrorMessage))
            {
                return originalErrorMessage;
            }
            string result =
                originalErrorMessage.Length > trimLength ?
                string.Format("{0} ...", originalErrorMessage.Substring(0, trimLength)) : 
                originalErrorMessage;
            return result;
        }

        #endregion //Methods
    }
}