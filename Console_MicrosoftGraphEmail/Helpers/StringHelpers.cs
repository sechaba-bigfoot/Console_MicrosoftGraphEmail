using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Helpers
{
    public static class StringHelpers
    {
        /// <summary>
        /// method checks if all string values from listOne are present in listTwo.
        /// </summary>
        /// <param name="listOne"></param>
        /// <param name="listTwo"></param>
        /// <returns></returns>
        public static bool DoesStringListOneExistInStringListTwo(List<string> listOne, List<string> listTwo)
        {
            foreach (string email in listOne)
            {
                if (!listTwo.Contains(email))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Will return only string values that don't contain the critera in them.
        /// </summary>
        /// <param name="emails"></param>
        /// <returns></returns>
        public static List<string> RemoveStringValuesThatEndWithRuleOut(List<string> emails, string ruleOut)
        {
            List<string> updatedEmails = new List<string>();

            foreach (string email in emails)
            {
                if (!email.EndsWith(ruleOut))
                {
                    updatedEmails.Add(email);
                }
            }

            return updatedEmails;
        }
    }
}
