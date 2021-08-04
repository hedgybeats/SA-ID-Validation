using System;
using System.Linq;

namespace Application.Helpers
{

    public static class IdNumber
    {

        public enum Gender
        {
            Male,
            Female
        }

        public static bool Validate(ulong idNumber, Gender? expectedGender = null)
        {
            int[] idCharArray = idNumber.ToString().Select(x => Convert.ToInt32(x) - 48).ToArray();
            if (idCharArray.Length != 13)
            {
                return false;
            }

            if (!ValidateGender(idCharArray, expectedGender))
            {
                // Gender not as expected
                return false;
            }

            if (!ValidateDOB(idCharArray))
            {
                // Not a valid DOB
                return false;
            }

            // Get check digit to compare
            var (idCheckDigit, checkDigitVerificationValue) = GetCheckDigits(idCharArray);

            // Compare check digits
            return ValidateCheckDigits(idCheckDigit, checkDigitVerificationValue);

        }

        private static bool ValidateGender(int[] idCharArray, Gender? expectedGender)
        {
            if (!expectedGender.HasValue)
            {
                return true;
            }

            var gender = idCharArray[6] >= 5 ? Gender.Male : Gender.Female;
            return gender == expectedGender;
        }

        private static bool ValidateDOB(int[] idCharArray)
        {
            var month = int.Parse(idCharArray[2].ToString() + idCharArray[3].ToString());
            var day = int.Parse(idCharArray[4].ToString() + idCharArray[5].ToString());

            // Make sure day and month are valid
            if (day > 31 || day < 1 || month > 12 || month < 1)
            {
                return false;
            }

            //  Apr, Jun, Sep, Nov must have 30
            if ((month == 04 || month == 06 ||
                month == 09 || month == 11) && day !> 30)
            {
                return false;
            }

            //// Jan, Mar, May, July, Aug, Oct, Dec must have 31 days
            //if ((month == 01 || month == 03 || month == 05 ||
            //    month == 07 || month == 08 || month == 10 || month == 12) && day !<= 31)
            //{
            //    return false;
            //}

            // Feb must have 28 days or 29 if leap year
            if ((month == 02) && day !> 29)
            {
                return false;
            }

            // If Feb and day is 29, make sure is leap year
            if ((month == 02) && day == 29)
            {
                int year = int.Parse(idCharArray[0].ToString() + idCharArray[1].ToString());

                // Convert to 4 digit year
                year += year > (DateTime.Today.Year % 100) ? 1900 : 2000;

                // Check if is leap year
                if (year % 400 == 0 || (year % 4 == 0 && year % 100 != 0))
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        private static bool ValidateCheckDigits(int idCheckDigit, int checkDigitVerificationValue)
        {
            if (idCheckDigit == checkDigitVerificationValue)
            {
                return true;
            }

            return false;
        }

        private static (int, int) GetCheckDigits(int[] idCharArray)
        {
            int idCheckDigit = idCharArray[12];
            int checkDigitVerificationValue = (10 - ((idCharArray.Where((value, index) => index % 2 == 1).ToArray().Take(12).ToArray().Sum() + (int.Parse(string.Join("", idCharArray.Where((value, index) => index % 2 == 0).ToArray())) * 2).ToString().Select(x => Convert.ToInt32(x) - 48).ToArray().Sum()) % 10)) % 10;

            // If checkDigitVerificationValue has two digits, use the last digit
            if (checkDigitVerificationValue > 9)
            {
                checkDigitVerificationValue %= 10;
            }

            return (idCheckDigit, checkDigitVerificationValue);
        }
    }
}
