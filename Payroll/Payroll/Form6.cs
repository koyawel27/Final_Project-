using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Payroll
{
    public partial class Form6 : Form
    {
        private DataGridView dataGridView;
        private Form3 form3;

        public Form6(DataGridView passedDataGridView)
        {
            InitializeComponent();

            dataGridView = passedDataGridView;
        }

        private void button13_Click(object sender, EventArgs e)
        {
          

            // Show the form
            Hide();
            ResetDataGridView();

        }

        private void ResetDataGridView()
        {
            // Clear the DataSource of the passed DataGridView
            dataGridView.DataSource = null;
            dataGridView.Rows.Clear();
            dataGridView.Columns.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
