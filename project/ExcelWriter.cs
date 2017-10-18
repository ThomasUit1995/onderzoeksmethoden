using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace project
{
    class ExcelWriter
    {
        string filepath;
        public ExcelWriter(string filepath)
        {
            this.filepath = filepath;
        }
        public void WriteStuff()
        {
            Application oXL;
            _Workbook oWB;
            _Worksheet oSheet;
            Range oRng;
            object misvalue = System.Reflection.Missing.Value;

            // try
            /// {
            //Start Excel and get Application object.
            oXL = new Application();
            oXL.Visible = true;

            //Get a new workbook.
            oWB = (_Workbook)(oXL.Workbooks.Add(""));
            oSheet = (_Worksheet)oWB.ActiveSheet;

            //Add table headers going cell by cell.
            oSheet.Cells[1, 1] = "Sudoku Size";
            oSheet.Cells[1, 2] = "Runtime (milliseconds)";
            oSheet.Cells[1, 3] = "S value";
            oSheet.Cells[1, 4] = "Random Walk Threshhold";
            //Format A1:D1 as bold, vertical alignment = center.
            oSheet.get_Range("A1", "D1").Font.Bold = true;
            oSheet.get_Range("A1", "D1").VerticalAlignment =
                 XlVAlign.xlVAlignCenter;

           
            // oSheet.Unprotect();


            oXL.Visible = false;
            oXL.UserControl = false;
            string name = "kek";
            oWB.SaveAs(filepath + name, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            oWB.Close();
            //  }
            //  catch
            // {
            //     Console.WriteLine("error writing to excel file");
            // }
        }
    }
}
