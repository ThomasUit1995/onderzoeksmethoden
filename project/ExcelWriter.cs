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
        int trucksWritten = 0;
        Application oXL;
        _Workbook oWB;
        _Worksheet oSheet;
        const int outputWidth = 6;
        public ExcelWriter(string filepath)
        {
            this.filepath = filepath;
        }
        public void OpenWorksheet()
        {
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
            oSheet.get_Range("A2", "AZ2").Font.Bold = true;
            oSheet.get_Range("A2", "AZ2").HorizontalAlignment =
                 XlVAlign.xlVAlignCenter;
        }
        public void SaveSheet()
        {
            oXL.Visible = false;
            oXL.UserControl = false;
            string name = String.Format("Peak Visitors = {0}",Program.peakNewVisitors);
            oWB.SaveAs(filepath + name, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            oWB.Close();
        }
        public void WriteArrays(FoodTruck truck)
        {
            oSheet.Cells[1, 1 + trucksWritten * outputWidth] = String.Format("Truck At Position {0}", truck.location);
            oSheet.Cells[2,1 + trucksWritten * outputWidth] = "Current Time Frame";
            oSheet.Cells[2,2 + trucksWritten * outputWidth] = "Queue Size";
            oSheet.Cells[2, 3 + trucksWritten * outputWidth] = "Current Appeal";
            oSheet.Cells[2, 4 + trucksWritten * outputWidth] = "Visitors that got in line";
            oSheet.Cells[2, 5 + trucksWritten * outputWidth] = "Visitors that didn't get in line";
            //The timeframes
            for(int i = 0; i < Program.maxTimeFrame; i++)
            {
                oSheet.Cells[3 + i, 1 + trucksWritten * outputWidth] = i;
            }
            WriteArray(oSheet, truck.queueArray, null, 2);
            WriteArray(oSheet, null, truck.actualAppeal, 3);
            WriteArray(oSheet, truck.gotInLine, null, 4);
            WriteArray(oSheet, truck.didntGetInLine, null, 5);
            trucksWritten++;
        }
        public void WriteArray(_Worksheet oSheet, int[] intArray, double[] doubleArray, int xOffset)
        {
            if(intArray!=null)
                for(int i = 0; i < intArray.Length; i++)
                {
                    oSheet.Cells[3 + i, trucksWritten * outputWidth+xOffset] = intArray[i];
                }
            if (doubleArray != null)
                for (int i = 0; i < doubleArray.Length; i++)
                {
                    oSheet.Cells[3 + i, trucksWritten * outputWidth + xOffset] = doubleArray[i];
                }
        }
    }
}
