using Microsoft.Office.Interop.Excel;
using System;

namespace ExcelTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Application excelApp = new Application();
            Workbook wbk = excelApp.Workbooks.Open(Filename:args[0],Password: args[1]);
            Worksheet wsht = wbk.Worksheets["Specification"];
            try
            {
                Shape shp = wsht.Shapes.Item(1);
            }
            catch
            {
                wbk.Close(false);
                excelApp.Quit();
            }
            finally { }

            Range rng =  wsht.Range["SpecificationTable"];
            try
            {
                for (int i = 1; i <= rng.Rows.Count; i++)
                {
                    Console.WriteLine("Column:\t{0}\tRule:\t{1}", 
                        Convert.ToString(rng.Cells[i, 4].Value2), 
                        Convert.ToString(rng.Cells[i, 7].Value2));
                }
            }
            catch (Exception e)
            { }
            finally
            {
                wbk.Close(false);
                excelApp.Quit();
            }
        }
    }
}