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
            PopulateUnitOfMeasureComboBox(); // Populate the ComboBox with units of measure
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;

        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Check if the current column is the "Status" column
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Status")
            {
                // Check the value of the "Status" and set the cell color
                string status = e.Value?.ToString();

                if (status == "Out of Stock")
                {
                    // Set the background color to red for "Out of Stock"
                    e.CellStyle.BackColor = Color.Red;
                    e.CellStyle.ForeColor = Color.White; // Optional: change text color to white
                }
                else if (status == "Low Stock")
                {
                    // Set the background color to yellow for "Low Stock"
                    e.CellStyle.BackColor = Color.Yellow;
                    e.CellStyle.ForeColor = Color.Black; // Optional: change text color to black
                }
                else if (status == "In Stock")
                {
                    // Set the background color to green for "In Stock"
                    e.CellStyle.BackColor = Color.Green;
                    e.CellStyle.ForeColor = Color.White; // Optional: change text color to white
                }
            }
        }


        private void PopulateUnitOfMeasureComboBox()
        {
            // Clear any existing items in the ComboBox
            comboBox1.Items.Clear();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Query to get distinct units of measure
                    string query = "SELECT DISTINCT [Unit of Measure] FROM tbl_ingredients";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        OleDbDataReader reader = command.ExecuteReader();

                        // Loop through the results and add each item to the ComboBox
                        while (reader.Read())
                        {
                            string unit = reader["Unit of Measure"].ToString();
                            comboBox1.Items.Add(unit);
                        }
                    }
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

            // Populate textboxes and other controls
            textBox1.Text = row.Cells["Ingredient"].Value?.ToString(); // Ingredient
            comboBox1.Text = row.Cells["Unit of Measure"].Value?.ToString(); // Unit of Measure
            textBox3.Text = row.Cells["Quantity"].Value?.ToString(); // Quantity

            // Populate DateTimePickers with explicit conversion
            if (row.Cells["Date Added"].Value != null && DateTime.TryParse(row.Cells["Date Added"].Value.ToString(), out DateTime dateAdded))
            {
                dateTimePicker1.Value = dateAdded; // Date Added
            }
            else
            {
                dateTimePicker1.Value = DateTime.Now; // Default to current date if null or invalid
            }

            if (row.Cells["Expiration Date"].Value != null && DateTime.TryParse(row.Cells["Expiration Date"].Value.ToString(), out DateTime expirationDate))
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

                        foreach (DataRow row in dataTable.Rows)
                        {
                            int quantity = Convert.ToInt32(row["Quantity"]);
                            string status = "In Stock";

                            if (quantity == 0)
                            {
                                status = "Out of Stock";
                            }
                            else if (quantity < 500)
                            {
                                status = "Low Stock";
                            }

                            row["Status"] = status;
                        }

                        // Clear the existing DataSource
                        dataGridView1.DataSource = null;
                        // Set the new DataSource
                        dataGridView1.DataSource = dataTable;
                        // Refresh the view
                        dataGridView1.Refresh();
                    }
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
            string ingredient = textBox1.Text;
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

            // Set the stock status based on quantity
            string status = "In Stock";
            if (quantity == 0)
            {
                status = "Out of Stock";
            }
            else if (quantity < 500)
            {
                status = "Low Stock";
            }

            string query = "INSERT INTO tbl_ingredients (Ingredient, [Unit of Measure], Quantity, [Date Added], [Expiration Date], Status) " +
                           "VALUES (@Ingredient, @UnitOfMeasure, @Quantity, @DateAdded, @ExpirationDate, @Status)";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Ingredient", ingredient);
                        cmd.Parameters.AddWithValue("@UnitOfMeasure", unitOfMeasure);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@DateAdded", dateAdded);
                        cmd.Parameters.AddWithValue("@ExpirationDate", expirationDate);
                        cmd.Parameters.AddWithValue("@Status", status);

                        cmd.ExecuteNonQuery();
                        LoadIngredients();
                        Application.DoEvents();

                        MessageBox.Show("Ingredient added successfully!");
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

            // Get the selected row's ItemID
            int itemId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ItemID"].Value);

            // Get values from controls
            string ingredient = textBox1.Text;
            string unitOfMeasure = comboBox1.Text;
            int quantity;

            if (!int.TryParse(textBox3.Text, out quantity))
            {
                MessageBox.Show("Please enter a valid quantity.");
                return;
            }

            DateTime dateAdded = dateTimePicker1.Value;
            DateTime expirationDate = dateTimePicker2.Value;

            // Set the stock status based on quantity
            string status = "In Stock";
            if (quantity == 0)
            {
                status = "Out of Stock";
            }
            else if (quantity < 500)
            {
                status = "Low Stock";
            }

            // SQL query for updating
            string query = @"UPDATE tbl_ingredients 
                     SET Ingredient = @Ingredient, 
                         [Unit of Measure] = @UnitOfMeasure, 
                         Quantity = @Quantity, 
                         [Date Added] = @DateAdded, 
                         [Expiration Date] = @ExpirationDate,
                         Status = @Status
                     WHERE ItemID = @ItemID";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Ingredient", ingredient);
                        cmd.Parameters.AddWithValue("@UnitOfMeasure", unitOfMeasure);
                        cmd.Parameters.AddWithValue("@Quantity", quantity);
                        cmd.Parameters.AddWithValue("@DateAdded", dateAdded);
                        cmd.Parameters.AddWithValue("@ExpirationDate", expirationDate);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@ItemID", itemId);

                        cmd.ExecuteNonQuery();
                        LoadIngredients();
                        Application.DoEvents();

                        MessageBox.Show("Ingredient updated successfully!");
                        textBox1.Clear();
                        comboBox1.SelectedIndex = -1;
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

            // Get the selected row ItemID (hidden column)
            int itemId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ItemID"].Value);

            // Ask for confirmation
            DialogResult result = MessageBox.Show("Are you sure you want to delete this ingredient?",
                                                  "Confirm Deletion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM tbl_ingredients WHERE ItemID = @ItemID";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (OleDbCommand cmd = new OleDbCommand(query, connection))
                        {
                            // Bind the ItemID parameter
                            cmd.Parameters.AddWithValue("@ItemID", itemId);

                            // Execute the DELETE command
                            cmd.ExecuteNonQuery();
                            LoadIngredients(); // Refresh the table
                            Application.DoEvents();

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

        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form8 form8 = new Form8();
            form8.Show();
        }

        private void BackupRestore_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form10 form10 = new Form10();
            form10.Show();
        }
    }
}