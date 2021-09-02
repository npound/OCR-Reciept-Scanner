using System;
using System.IO;
using Tesseract;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace OCRRecieptScanner
{
    class Program
    {


        [STAThread]
        static void Main(string[] args)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            string[] files;
            var reciepts = new List<Reciept>();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                files = Directory.GetFiles(folderBrowserDialog.SelectedPath);
            }
            else
            {
                return;
            }

            var resultPath = Path.Combine(Environment.CurrentDirectory, "Reciepts");
            Directory.CreateDirectory(resultPath);

            try
            {
                using (var sw = new StreamWriter(Path.Combine(resultPath, "Reciepts.csv")))
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.TesseractAndLstm))
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            var picPath = Path.Combine(resultPath, Path.GetFileName(file));
                            using (var img = Pix.LoadFromFile(file))
                            {
                                using (var page = engine.Process(img))
                                {
                                    var text = page.GetText();

                                    var lines = text.Split(new char[] { '\n' });


                                    var reciept = new Reciept()
                                    {
                                        CompanyName = GetCompanyName(lines),
                                        DateParseResult = DateParse.FindDate(lines),
                                        Total = FindTotal(lines),
                                        LineItems = FindLineItems(lines),
                                        PictureName = Path.GetFileName(picPath)
                                    };

                                    sw.WriteLine($@"Date,Business Name,Total,Picture Name,Possible Dates");
                                    sw.WriteLine($@"{reciept.Date},{reciept.CompanyName},{reciept.Total},{reciept.PictureName}");
                                    sw.WriteLine($@"Price,Description");
                                    foreach (var li in reciept.LineItems)
                                        sw.WriteLine($@"{li.Price},{li.Item}");
                                    sw.WriteLine("End Reciept");
                                    sw.WriteLine("");
                                }
                            }
                            File.Copy(file, Path.Combine(resultPath, picPath));
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        }

                }
            }
            catch (Exception) { }
            Console.ReadLine();
        }

        private static string GetCompanyName(string[] lines)
        {
            var linesplits = new List<Tuple<string[], int>>();

            for (int i = 0; i < 3; i++)
                linesplits.Add(new Tuple<string[], int> (lines[i].Split(' '), i ) );

            var min = linesplits.Min(m => m.Item1.Length);

            return lines[linesplits.First(f => f.Item1.Length == min).Item2];
        }

        private static List<LineItem> FindLineItems(string[] lines)
        {
            var lineItems = new List<LineItem>();
            foreach(var line in lines)
            {
                var index = line.IndexOf('$');

                if (index != -1)
                {
                    lineItems.Add(new LineItem()
                    {
                        Item = line.Substring(0, index).Trim(),
                        Price = DecimalFindParse(line)
                    });
                }
            }
            return lineItems;
        }

        private static decimal FindTotal(string[] lines)
        {
            var amounts = new List<decimal>();
            for (int i = 0; i < lines.Length; i++)
            {
                amounts.Add(DecimalFindParse(lines[i]));

            }
            return amounts.Max();
        }

        private static decimal DecimalFindParse(string line)
        {
            var index = line.IndexOf('$');

            if (index != -1)
            {
                try
                {
                    var pstart = index + 1;
                    var pend = index + 1;

                    for (int j = index + 2; j < line.Length; j++)
                    {
                        if (line[j].IsNumeric() || line[j] == ' ' || line[j] == '.')
                        {
                            pend = j;
                        }
                    }
                    var dirtyAmount = line.Substring(pstart, pend - pstart).Replace(" ", "").Trim();

                    if (decimal.TryParse(dirtyAmount, out var amt))
                    {
                        return amt;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return default;
        }
    }
}
