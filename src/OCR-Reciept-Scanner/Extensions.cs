using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRRecieptScanner
{
    public static class Extensions
    {
        public static char[] Numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static bool IsNumeric(this char c)
        {
            return Numbers.Contains(c);
        }
    }
}
