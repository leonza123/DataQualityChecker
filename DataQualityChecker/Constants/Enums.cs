using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataQualityChecker.Constants
{
    public class Enums
    {
        public struct CellError
        {
            public const double Null = 0;

            //Type codes 
            //Order: Text, Boolean, Integer, Float, DayTime
            public const int WrongType = 1;

            public const double WrongTypeText = 1.1;
            public const double WrongTypeBoolean = 1.2;
            public const double WrongTypeInteger = 1.3;
            public const double WrongTypeFloat = 1.4;
            public const double WrongTypeDateTime = 1.5;
            public const double WrongTypeTime = 1.6;
            public const double WrongTypeChar = 1.7;

            public const double Duplicate = 2;

            public const int WrongFormat = 3;

            public const double FormatDateTime = 3.1;
            public const double FormatText = 3.2;

            public const double WrongOptions = 4;

            public const double WrongRanges = 5;
        }
    }
}
