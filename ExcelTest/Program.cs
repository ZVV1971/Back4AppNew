using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace ExcelTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Application excelApp = new Application();
            Workbook wbk = excelApp.Workbooks.Open(Filename:args[0],Password: args[1]);
            Worksheet wsht = wbk.Worksheets["Specification"];
            Range rng =  wsht.Range["SpecificationTable"];
            try
            {
                foreach (Range row in rng.Rows)
                {
                    Console.WriteLine("Column:\t{0}\tRule:\t{1}", row[0, 3], row[0, 6]);
                }
            }
            catch { }
            finally
            {
                wbk.Close();
                excelApp.Quit();
            }
        }
    }
}
