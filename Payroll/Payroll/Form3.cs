using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Payroll
{
    public partial class Form3 : Form
    {
        // Connection string details
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;";
        public DataGridView dataGridView1new;

        // Table name
        private string tableName = "tbl_products";

        public Form3()
        {
            InitializeComponent();

            dataGridView1new = new DataGridView();
            this.Controls.Add(dataGridView1new); // Add DataGridView to the form
        }

        private void label2_Click(object sender, EventArgs e)
        {


        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            LoadTable();
            InitializeDataGridView();
            InitializeFlowLayoutPanel();
        }
        public void ResetDataGridView()
        {
            // Clear the DataSource
            dataGridView1.DataSource = null;

            // Optionally clear rows and columns if needed
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
        }

        private void LoadTable()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Select all data from tbl_products
                    string query = $"SELECT prod_name, prod_price FROM tbl_products";

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);



                        // Clear previous buttons if any
                        flowLayoutPanel1.Controls.Clear();

                        // Iterate through each row and create a button for each product
                        foreach (DataRow row in dataTable.Rows)
                        {
                            // Create a new button
                            Button btnProduct = new Button();
                            btnProduct.Text = $"{row["prod_name"]} - ₱{row["prod_price"]}";
                            btnProduct.Width = 185; // Adjust button width
                            btnProduct.Height = 100; // Adjust button height
                            btnProduct.Tag = row["prod_name"]; // Store prod_name as Tag to access later if needed

                            // Optionally, add an event handler to handle button click
                            btnProduct.Click += (s, ev) =>
                            {
                                // Use the captured 'row' variable here
                                AddProductToGrid(row);
                            };

                            // Add the button to the panel
                            flowLayoutPanel1.Controls.Add(btnProduct);
                        }
                    }

                    // Close the connection
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void InitializeFlowLayoutPanel()
        {
            // Enable auto-scrolling for vertical scrolling
            flowLayoutPanel1.AutoScroll = true;

            // Set flow direction to LeftToRight to maintain old design
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;

            // Allow wrapping of controls to the next row
            flowLayoutPanel1.WrapContents = true;

            // Keep the size fixed to match your design
            flowLayoutPanel1.Width = 600; // Adjust width as per UI layout
           // flowLayoutPanel1.Height = 400; // Adjust height for vertical scroll area

        }

        // Method to add product details to the DataGridView
        private void AddProductToGrid(DataRow row)
        {
            string prodName = row["prod_name"].ToString();
            decimal prodPrice = Convert.ToDecimal(row["prod_price"]);
            int quantity = 1; // Default quantity for a new product

            // Check if the product already exists in the DataGridView
            foreach (DataGridViewRow dgvRow in dataGridView1.Rows)
            {
                if (dgvRow.Cells["prod_name"].Value?.ToString() == prodName)
                {
                    // If the product exists, update the quantity and amount
                    int currentQuantity = Convert.ToInt32(dgvRow.Cells["quantity"].Value);
                    dgvRow.Cells["quantity"].Value = currentQuantity + 1;
                    dgvRow.Cells["amount"].Value = (currentQuantity + 1) * prodPrice;
                    return;
                }
            }

            // If the product doesn't exist, add a new row
            dataGridView1.Rows.Add(null, prodName, quantity, prodPrice, prodPrice);
        }

        private void InitializeDataGridView()
        {
            // Clear existing columns
            dataGridView1.Columns.Clear();

            // Add required columns
            dataGridView1.Columns.Add("id", "ID"); // Optional, can be hidden or removed
            dataGridView1.Columns.Add("prod_name", "Product Name");
            dataGridView1.Columns.Add("quantity", "Quantity");
            dataGridView1.Columns.Add("prod_price", "Price");
            dataGridView1.Columns.Add("amount", "Amount");
            

            // Make sure the ID column is hidden if it's not required
            dataGridView1.Columns["id"].Visible = false;

            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
            deleteButtonColumn.Name = "delete";
            deleteButtonColumn.HeaderText = "Delete";
            deleteButtonColumn.Text = "Delete";
            deleteButtonColumn.UseColumnTextForButtonValue = true; // Use the same text for all buttons
            dataGridView1.Columns.Add(deleteButtonColumn);

            // Optionally, make other adjustments (like column widths)
            // Enable custom formatting
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;

        }





        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click is on the Delete button column
            if (e.ColumnIndex == dataGridView1.Columns["delete"].Index && e.RowIndex >= 0)
            {
                // Confirm the deletion (optional)
                var result = MessageBox.Show("Are you sure you want to delete this row?", "Delete Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    // Remove the row
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                }
            }
        }


        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "delete" && e.RowIndex >= 0)
            {
                // Set the red background and white text for the delete button
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.BackColor = Color.Red;
                style.ForeColor = Color.White;
                style.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                e.CellStyle = style;
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Create a DataTable to hold the data
            DataTable tableData = new DataTable();
            
            // Add columns to the DataTable based on DataGridView columns
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (column.Name != "delete") // Skip the delete button column
                {
                    tableData.Columns.Add(column.Name, typeof(string));
                }
            }

            // Add rows to the DataTable
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow) // Skip the new row placeholder
                {
                    DataRow newRow = tableData.NewRow();
                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        if (column.Name != "delete") // Skip the delete button column
                        {
                            newRow[column.Name] = row.Cells[column.Name].Value ?? DBNull.Value;
                        }
                    }
                    tableData.Rows.Add(newRow);
                }
            }

            // Check if there's data to transfer
            if (tableData.Rows.Count == 0)
            {
                MessageBox.Show("No data available to transfer.");
                return;
            }

            // Pass the DataTable to Form5
            Form5 receiptForm = new Form5(tableData);

            // Show Form5 as a dialog or normal window
            receiptForm.ShowDialog();
        }
    }
}
