using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRRecieptScanner
{
    static internal class DateParse
    {
        static DateTime MinDate = DateTime.Parse("7/14/2021");
        static DateTime MaxDate = DateTime.Parse("9/2/2021");

        internal static DateParseResult FindDate(string[] lines)
        {
            var dates = new List<DateTime>();
            var dirtyDates = new List<string>();
            foreach (var line in lines)
            {
                if (!DateTime.TryParse(line, out var date))
                {
                    var dateIndicatorIndex = line.IndexOf('/');

                    if (dateIndicatorIndex == -1)
                        dateIndicatorIndex = line.IndexOf('-');

                    if (dateIndicatorIndex == -1)
                        continue;

                    var f = FindBeginingDateIndex(line, dateIndicatorIndex);
                    var e = FindEndDateIndex(line, dateIndicatorIndex);

                    var dirtyDate = line.Substring(f, e - f).Trim();

                    if (!DateTime.TryParse(dirtyDate, out var cleanDate))
                    {
                        dirtyDates.Add(dirtyDate);
                    }
                    else
                    {
                        dates.Add(cleanDate);
                    }

                }
                else
                {
                    dates.Add(date);
                }
            }


            return new DateParseResult()
            {
                Dates = dates,
                DirtyDates = dirtyDates,
                PredictedDate = dates.FirstOrDefault(f => f > MinDate && f < MaxDate)

            };
        }

        private static int FindBeginingDateIndex(string line, int dateIndicatorIndex)
        {
            int beingIndex = dateIndicatorIndex;
            for (int i = dateIndicatorIndex; i > -1; i--)
            {
                if (line[i].IsNumeric() || line[i] == ' ')
                {
                    if (line[i] != ' ')
                        beingIndex = i;

                    else break;
                }

            }
            return beingIndex;
        }

        private static int FindEndDateIndex(string line, int dateIndicatorIndex)
        {

            int endIndex = dateIndicatorIndex;
            for (int i = dateIndicatorIndex; i < line.Length - 1; i++)
            {

                var c = line[i];

                if (line[i].IsNumeric() || line[i] == ' ' || line[i] == '/' || line[i] == '-' || line[i] == ':')
                {
                    if (line[i] != ' ')
                        endIndex = i;

                    else break;
                }
            }
            return endIndex;
        }
    }
}
