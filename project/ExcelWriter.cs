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
        public int trucksWritten = 0;
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
            string name = String.Format("Chance of entering = {0} Simulations = {1}", Program.chanceOfEntering,Program.amountOfSimulations);
            oWB.SaveAs(filepath + name, XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                false, false, XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            oWB.Close();
        }
        public void WriteArrays(FoodTruck truck)
        {
            int simulationOffset = Program.currentsimulation * 485;
            oSheet.Cells[1 + simulationOffset, 1 + trucksWritten * outputWidth] = String.Format("Truck At Position {0}", truck.location);
            oSheet.Cells[2 + simulationOffset, 1 + trucksWritten * outputWidth] = "Current Time Frame";
            oSheet.Cells[2 + simulationOffset, 2 + trucksWritten * outputWidth] = "Queue Size";
            oSheet.Cells[2 + simulationOffset, 3 + trucksWritten * outputWidth] = "Current Appeal";
            oSheet.Cells[2 + simulationOffset, 4 + trucksWritten * outputWidth] = "Visitors that got in line";
            oSheet.Cells[2 + simulationOffset, 5 + trucksWritten * outputWidth] = "Visitors that didn't get in line";
            //The timeframes
            for (int i = 0; i < Program.maxTimeFrame; i++)
            {
                oSheet.Cells[3 + i + simulationOffset, 1 + trucksWritten * outputWidth] = i;
            }
            WriteArray(truck.queueArray, null, 2, simulationOffset);
            WriteArray(null, truck.actualAppeal, 3, simulationOffset);
            WriteArray (truck.gotInLine, null, 4, simulationOffset);
            WriteArray(truck.didntGetInLine, null, 5, simulationOffset);
            trucksWritten++;
            if (trucksWritten == Program.numberOfFoodtrucks && Program.currentsimulation == Program.amountOfSimulations - 1)
            {
                trucksWritten = 0;
                for (int i = 0; i < Program.numberOfFoodtrucks; i++)
                {
                    int simOffSet = simulationOffset + 485;
                    oSheet.Cells[1 + simOffSet, 1 + trucksWritten * outputWidth] = String.Format("Average for Truck At Position {0}", trucksWritten);
                    oSheet.Cells[2 + simOffSet, 1 + trucksWritten * outputWidth] = "Current Time Frame";
                    oSheet.Cells[2 + simOffSet, 2 + trucksWritten * outputWidth] = "Queue Size";
                    oSheet.Cells[2 + simOffSet, 3 + trucksWritten * outputWidth] = "Current Appeal";
                    //The timeframes
                    for (int j = 0; j < Program.maxTimeFrame; j++)
                    {
                        oSheet.Cells[3 + j + simOffSet, 1 + trucksWritten * outputWidth] = j;
                    }
                    double[] averageQueues = getAverages(2);
                    double[] averageAppeal = getAverages(3);
                    WriteArray(null, averageQueues, 2, simOffSet);
                    WriteArray(null, averageAppeal, 3, simOffSet);
                    trucksWritten++;
                }
            }
        }
        public void WriteArray( int[] intArray, double[] doubleArray, int xOffset, int yOffset)
        {
            if (intArray != null)
                for (int i = 0; i < intArray.Length; i++)
                {
                    oSheet.Cells[3 + i + yOffset, trucksWritten * outputWidth + xOffset] = intArray[i];
                }
            if (doubleArray != null)
                for (int i = 0; i < doubleArray.Length; i++)
                {
                    oSheet.Cells[3 + i + yOffset, trucksWritten * outputWidth + xOffset] = doubleArray[i];
                }
        }
        public double[] getAverages(int column)
        {
            double[] averages = new double[Program.maxTimeFrame];
            for (int j = 0; j < Program.maxTimeFrame; j++)
            {
                double sum = 0;
                for (int i = 0; i < Program.amountOfSimulations; i++)
                {
                    string test = oSheet.Cells[3 + j + (i * 485), trucksWritten * outputWidth+column].Value.ToString();
                    sum += double.Parse(test);
                }
                averages[j] = sum / (double)Program.amountOfSimulations;
            }
            return averages;
        }
    }
}
