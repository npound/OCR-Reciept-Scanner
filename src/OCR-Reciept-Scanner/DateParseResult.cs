using System;
using System.Collections.Generic;

namespace OCRRecieptScanner
{
    internal class DateParseResult
    {
        public DateTime? PredictedDate { get; set; }
        public List<DateTime> Dates { get; set; }
        public List<string> DirtyDates { get; set; }
        }
}