using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Shopping.Utilities
{
    public class CCValidator : ValidationAttribute
    {
        public CCValidator() : base()
        {
            ErrorMessage = "Please enter a valid credit card number";
        }

        public override bool IsValid(object value)
        {
            return IsValidCC(value.ToString());
        }

        private bool IsValidCC(string id)
        {
            int idLength = id.Length;
            int currentDigit;
            int idSum = 0;
            int currentProcNum = 0; //the current process number (to calc odd/even proc)

            for (int i = idLength - 1; i >= 0; i--)
            {
                //get the current rightmost digit from the string
                string idCurrentRightmostDigit = id.Substring(i, 1);

                //parse to int the current rightmost digit, if fail return false (not-valid id)
                if (!int.TryParse(idCurrentRightmostDigit, out currentDigit))
                    return false;

                //bouble value of every 2nd rightmost digit (odd)
                //if value 2 digits (can be 18 at the current case),
                //then sumarize the digits (made it easy the by remove 9)
                if (currentProcNum % 2 != 0)
                {
                    if ((currentDigit *= 2) > 9)
                        currentDigit -= 9;
                }
                currentProcNum++; //increase the proc number

                //summarize the processed digits
                idSum += currentDigit;
            }

            //if digits sum is exactly divisible by 10, return true (valid), else false (not-valid)
            return (idSum % 10 == 0);
        }
    }
}