using System;
using System.Collections.Generic;

namespace OCRRecieptScanner
{
    internal class Reciept
    {
        public string CompanyName { get; set; }
        public decimal Total { get; set; }
        public List<LineItem> LineItems { get; set; }
        public DateTime Date => DateParseResult.PredictedDate ?? default;
        public string PictureName { get; set; }

        public DateParseResult DateParseResult { get; set; } = new DateParseResult();

    }
}