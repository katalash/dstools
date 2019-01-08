using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowDSIO.Exceptions.DSRead
{
    public class ConsumeValueCheckFailedException<TVal> : DSReadException
    {
        private static string GetMsg(string valueName, TVal expectedValue, TVal consumedValue)
        {
            return $"Consumed a value different than what was expected." +
                $"\nExpected Value: {expectedValue}" +
                $"\nConsumed Value: {consumedValue}";
        }

        public ConsumeValueCheckFailedException(DSBinaryReader bin, string valueName, TVal expectedValue, TVal consumedValue) 
            : base(bin, GetMsg(valueName, expectedValue, consumedValue))
        {

        }
    }
}
