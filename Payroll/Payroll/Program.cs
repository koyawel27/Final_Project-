using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Payroll
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Create a DataTable (you should fill this with your actual data)
            DataTable tableData = new DataTable();
            // Optionally, add some columns and rows to the DataTable
           // tableData.Columns.Add("Column1");
           // tableData.Rows.Add("Row1");

            // Initialize the form with the DataTable
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Pass DataTable to Form3
        }
    }
}
