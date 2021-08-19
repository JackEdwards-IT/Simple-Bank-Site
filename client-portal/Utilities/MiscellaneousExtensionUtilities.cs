using System;

namespace a2.Utilities
{
    public static class MiscellaneousExtensionUtilities
    {
        public static bool HasMoreThanNDecimalPlaces(this decimal value, int n) => decimal.Round(value, n) != value;
        public static bool HasMoreThanTwoDecimalPlaces(this decimal value) => value.HasMoreThanNDecimalPlaces(2);

        public static decimal RoundDecimal(decimal number)
        {
            return decimal.Round(number, 2, MidpointRounding.AwayFromZero);;
        }
    }
}
