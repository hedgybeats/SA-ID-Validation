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

            // Get check digits to compare
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
            if (month > 12 || month < 1)
            {
                return false;
            }

            var day = int.Parse(idCharArray[4].ToString() + idCharArray[5].ToString());
            if (day > 31 || day < 1)
            {
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
