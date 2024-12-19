using System.Data.OleDb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Payroll
{
    public partial class Form7 : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;";
        private string tableName = "tbl_ingredients";
        public Form7()
        {
            InitializeComponent();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            LoadIngredients();
            dataGridView1.CellClick += DataGridView1_CellClick;
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "yyyy-MM-dd";
            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = "yyyy-MM-dd";

        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            // Populate textboxes and other controls
            textBox1.Text = row.Cells["ingredient_name"].Value?.ToString(); // Ingredient Name
            comboBox1.Text = row.Cells["unit_of_measure"].Value?.ToString(); // Unit of Measure
            textBox3.Text = row.Cells["quantity"].Value?.ToString(); // Quantity

            // Populate DateTimePickers with explicit conversion
            if (row.Cells["date_added"].Value != null && DateTime.TryParse(row.Cells["date_added"].Value.ToString(), out DateTime dateAdded))
            {
                dateTimePicker1.Value = dateAdded; // Date Added
            }
            else
            {
                dateTimePicker1.Value = DateTime.Now; // Default to current date if null or invalid
            }

            if (row.Cells["expiration_date"].Value != null && DateTime.TryParse(row.Cells["expiration_date"].Value.ToString(), out DateTime expirationDate))
            {
                dateTimePicker2.Value = expirationDate; // Expiration Date
            }
            else
            {
                dateTimePicker2.Value = DateTime.Now; // Default to current date if null or invalid
            }
        }



        private void LoadIngredients()
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

        private void button3_Click(object sender, EventArgs e)
        {
            AddIngredients();
        }

        private void AddIngredients()
        {
            string ingredient_name = textBox1.Text;
            string unitOfMeasure = comboBox1.Text;

            // Parse quantity from textBox3
            int quantity;
            if (!int.TryParse(textBox3.Text, out quantity))
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            // Retrieve only the date part
            DateTime dateAdded = dateTimePicker1.Value.Date;
            DateTime expirationDate = dateTimePicker2.Value.Date;

            string query = "INSERT INTO tbl_ingredients (ingredient_name, unit_of_measure, quantity, date_added, expiration_date) " +
                           "VALUES (@ingredient_name, @unit_of_measure, @quantity, @date_added, @expiration_date)";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ingredient_name", ingredient_name);
                        cmd.Parameters.AddWithValue("@unit_of_measure", unitOfMeasure);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@date_added", dateAdded);
                        cmd.Parameters.AddWithValue("@expiration_date", expirationDate);

                        cmd.ExecuteNonQuery();
                        LoadIngredients();

                        MessageBox.Show("Product added successfully!");
                        textBox1.Clear();
                        comboBox1.SelectedIndex = -1;
                        textBox3.Clear();
                        dateTimePicker1.Value = DateTime.Now;
                        dateTimePicker2.Value = DateTime.Now;
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
            UpdateIngredients();
        }
        private void UpdateIngredients()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a row to update.");
                return;
            }

            // Get the selected row's ID
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);

            // Get values from controls
            string ingredientName = textBox1.Text;
            string unitOfMeasure = comboBox1.Text;
            int quantity;

            if (!int.TryParse(textBox3.Text, out quantity))
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            DateTime dateAdded = dateTimePicker1.Value;
            DateTime expirationDate = dateTimePicker2.Value;

            // SQL query for updating
            string query = @"UPDATE tbl_ingredients 
                     SET ingredient_name = @ingredient_name, 
                         unit_of_measure = @unit_of_measure, 
                         quantity = @quantity, 
                         date_added = @date_added, 
                         expiration_date = @expiration_date 
                     WHERE id = @id";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        // Bind parameters to the SQL query
                        cmd.Parameters.AddWithValue("@ingredient_name", ingredientName);
                        cmd.Parameters.AddWithValue("@unit_of_measure", unitOfMeasure);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@date_added", dateAdded);
                        cmd.Parameters.AddWithValue("@expiration_date", expirationDate);
                        cmd.Parameters.AddWithValue("@id", id);

                        // Execute the query
                        cmd.ExecuteNonQuery();
                        LoadIngredients();

                        MessageBox.Show("Ingredient updated successfully!");
                        // Clear input fields
                        textBox1.Clear();
                        comboBox1.SelectedIndex = -1; // Reset ComboBox
                        textBox3.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            DeleteIngredient();
        }

        private void DeleteIngredient()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }

            // Get the selected row ID (hidden column)
            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);

            // Ask for confirmation
            DialogResult result = MessageBox.Show("Are you sure you want to delete this ingredient?",
                                                  "Confirm Deletion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM tbl_ingredients WHERE id = @id";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (OleDbCommand cmd = new OleDbCommand(query, connection))
                        {
                            // Bind the ID parameter
                            cmd.Parameters.AddWithValue("@id", id);

                            // Execute the DELETE command
                            cmd.ExecuteNonQuery();
                            LoadIngredients(); // Refresh the table

                            MessageBox.Show("Ingredient deleted successfully!");

                            // Clear input fields
                            textBox1.Clear();
                            comboBox1.SelectedIndex = -1; // Reset ComboBox
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form7 form7 = new Form7();
            form7.Show(); // Show Form3
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Show(); // Show Form3
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form9 form9 = new Form9();
            form9.Show();
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
    }
}
