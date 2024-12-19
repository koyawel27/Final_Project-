using System;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Payroll
{
    public partial class Form4 : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;";

        private string tableName = "tbl_products";

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadTable();
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void LoadTable()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = $"SELECT * FROM {tableName}";

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;

                        // Optional: Hide the ID column
                        //dataGridView1.Columns["id"].Visible = false;
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            // Populate textboxes
            textBox1.Text = row.Cells["prod_type"].Value?.ToString(); // Product Type
            textBox2.Text = row.Cells["prod_name"].Value?.ToString(); // Product Name
            textBox3.Text = row.Cells["prod_price"].Value?.ToString(); // Price
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddProduct();
        }

        private void AddProduct()
        {
            string prodType = textBox1.Text;
            string prodName = textBox2.Text;
            decimal prodPrice;

            if (!decimal.TryParse(textBox3.Text, out prodPrice))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }

            string query = "INSERT INTO tbl_products (prod_type, prod_name, prod_price) VALUES (@prodType, @prodName, @prodPrice)";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@prodType", prodType);
                        cmd.Parameters.AddWithValue("@prodName", prodName);
                        cmd.Parameters.AddWithValue("@prodPrice", prodPrice);

                        cmd.ExecuteNonQuery();
                        LoadTable();

                        MessageBox.Show("Product added successfully!");
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            UpdateProduct();
        }

        private void UpdateProduct()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a row to update.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            string prodType = textBox1.Text;
            string prodName = textBox2.Text;
            decimal prodPrice;

            if (!decimal.TryParse(textBox3.Text, out prodPrice))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }

            string query = "UPDATE tbl_products SET prod_type = @prodType, prod_name = @prodName, prod_price = @prodPrice WHERE id = @id";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@prodType", prodType);
                        cmd.Parameters.AddWithValue("@prodName", prodName);
                        cmd.Parameters.AddWithValue("@prodPrice", prodPrice);
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();
                        LoadTable();

                        MessageBox.Show("Product updated successfully!");
                        textBox1.Clear();
                        textBox2.Clear();
                        textBox3.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            UpdateProduct();
        }


        private void DeleteProduct()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }

            // Get the selected row ID (hidden column)
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);

            // Ask for confirmation
            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?",
                                                  "Confirm Deletion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM tbl_products WHERE id = @id";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (OleDbCommand cmd = new OleDbCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@id", id);

                            cmd.ExecuteNonQuery();
                            LoadTable(); // Refresh the table

                            MessageBox.Show("Product deleted successfully!");
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to logout?", "CONFIRMATION", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                this.Hide();
                Form1 form1 = new Form1();
                form1.Show();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DeleteProduct();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form7 form7 = new Form7();
            form7.Show(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Show(); 
        }


        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form9 form9 = new Form9();
            form9.Show();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

       
    }
}

