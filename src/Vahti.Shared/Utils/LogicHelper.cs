using System;
using System.Globalization;
using Vahti.Shared.Enum;

namespace Vahti.Shared.Utils
{
    /// <summary>
    /// Helper class to provide common logic handling functionality
    /// </summary>
    public static class LogicHelper
    {
        public static bool Compare(string firstValue, string secondValue, OperatorType operatorType)
        {
            bool comparisonResult;
            switch (operatorType)
            {
                case OperatorType.IsLessThan:
                    comparisonResult = double.Parse(firstValue, CultureInfo.InvariantCulture) < double.Parse(secondValue, CultureInfo.InvariantCulture);
                    break;
                case OperatorType.IsGreaterThan:
                    comparisonResult = double.Parse(firstValue, CultureInfo.InvariantCulture) > double.Parse(secondValue, CultureInfo.InvariantCulture);
                    break;
                case OperatorType.IsEqualTo:
                    comparisonResult = int.Parse(firstValue, CultureInfo.InvariantCulture) == int.Parse(secondValue, CultureInfo.InvariantCulture);
                    break;
                default:
                    throw new InvalidOperationException($"Operator {operatorType} is not supported yet");
            }
            return comparisonResult;
        }
    }
}
