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


namespace Payroll
{
    public partial class Form5 : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;";

        private string tableName = "tbl_ingredients";
        public Form5(DataTable tableData)
        {
            InitializeComponent();

            // Bind the DataTable to the DataGridView
            dataGridView1.DataSource = tableData;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            LoadIngredientNames();
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowHeadersWidth = 20; // Set a smaller width
                                                //textBox1.Text = "₱.00"; // Default text
                                                // Make sure the ID column is hidden if it's not required

            // Set the width of each column
            dataGridView1.Columns[0].Width = 80; // First column width
            dataGridView1.Columns[1].Width = 200; // Second column width
            dataGridView1.Columns[2].Width = 50; // Third column width
            dataGridView1.Columns[3].Width = 100; // Fourth column width




            if (dataGridView1.Columns.Contains("id"))
            {
                dataGridView1.Columns["id"].Visible = false;
            }


            // Change header names for columns
            if (dataGridView1.Columns.Contains("prod_type"))
            {
                dataGridView1.Columns["prod_type"].HeaderText = "Product Type"; // Change column name to "Product Type"
            }

            if (dataGridView1.Columns.Contains("prod_name"))
            {
                dataGridView1.Columns["prod_name"].HeaderText = "Product Name"; // Change column name to "Product Name"
            }

            if (dataGridView1.Columns.Contains("amount"))
            {
                dataGridView1.Columns["amount"].HeaderText = "Amount"; // Change column name to "Amount"
            }

            if (dataGridView1.Columns.Contains("quantity"))
            {
                dataGridView1.Columns["quantity"].HeaderText = "Quantity"; // Change column name to "Amount"
            }



            if (dataGridView1.Columns.Contains("prod_price"))
            {
                dataGridView1.Columns["prod_price"].HeaderText = "Product Price"; // Change column name to "Amount"
            }

            // Calculate and display the total
            CalculateTotal();
        }

        private void LoadIngredientNames()
        {
            string query = "SELECT ingredient_name FROM tbl_ingredients";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            // Create a DataTable to hold the data
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            // Bind the DataTable to the ComboBox
                            comboBox1.DataSource = dt;
                            comboBox1.DisplayMember = "ingredient_name"; // Column to display
                            comboBox1.ValueMember = "ingredient_name";   // Column to use as value
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading ingredient names: " + ex.Message);
                }
            }
        }





        private void CalculateTotal()
        {
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["amount"].Value != null && decimal.TryParse(row.Cells["amount"].Value.ToString(), out decimal amount))
                {
                    totalAmount += amount;
                }
            }

            // Display the total in label4
            label4.Text = $"₱{totalAmount:F2}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateTextBox("7");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateTextBox("8");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateTextBox("9");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            UpdateTextBox("4");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            UpdateTextBox("5");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            UpdateTextBox("6");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            UpdateTextBox("1");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            UpdateTextBox("2");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            UpdateTextBox("3");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox1.Text += "0";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox1.Text += ".";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ClearLastDigit();
        }

        private void ClearLastDigit()
        {
            string currentText = textBox1.Text.StartsWith("₱")
                ? textBox1.Text.Substring(1).Replace(".00", "")
                : textBox1.Text.Replace(".00", "");

            if (!string.IsNullOrEmpty(currentText))
            {
                // Remove the last character
                currentText = currentText.Substring(0, currentText.Length - 1);

                // If the text becomes empty, default to ₱0.00
                if (string.IsNullOrEmpty(currentText))
                {
                    textBox1.Text = "₱0.00";
                }
                else if (decimal.TryParse(currentText, out decimal parsedValue))
                {
                    // If the value is a whole number, remove the decimal part
                    if (parsedValue % 1 == 0) // If it's a whole number
                    {
                        textBox1.Text = "₱" + parsedValue.ToString("F0"); // Format as a whole number
                    }
                    else
                    {
                        textBox1.Text = "₱" + parsedValue.ToString("F2"); // Format with 2 decimal places
                    }
                }
            }
            else
            {
                textBox1.Text = "₱0.00"; // Default value if TextBox is empty
            }

            // Set caret to the end of the text
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.SelectionLength = 0;
        }





        private void UpdateTextBox(string number)
        {
            // Remove any unwanted characters like multiple dots or '₱' symbols
            string currentText = textBox1.Text.StartsWith("₱")
                ? textBox1.Text.Substring(1).Replace(".00", "")
                : textBox1.Text.Replace(".00", "");

            // Append the new number
            currentText += number;

            // Parse the updated text as a decimal number
            if (decimal.TryParse(currentText, out decimal parsedValue))
            {
                // If the value is a whole number, remove the decimal part
                if (parsedValue % 1 == 0) // If it's a whole number
                {
                    textBox1.Text = "₱" + parsedValue.ToString("F0"); // Format as a whole number
                }
                else
                {
                    textBox1.Text = "₱" + parsedValue.ToString("F2"); // Format with 2 decimal places
                }
            }
            else
            {
                // If the input is invalid, show an error message
                MessageBox.Show("Invalid input!");
            }

            // Set caret to the end of the text
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.SelectionLength = 0;
        }





        private void button15_Click(object sender, EventArgs e)
        {
            //Form6 newForm = new Form6(dataGridView1);

            // Show the form
            //newForm.Show(); // Use Show() for non-blocking behavior
            this.Hide();
            dataGridView1.DataSource = null; // Unbind any data source
            dataGridView1.Rows.Clear(); // Remove all rows
            dataGridView1.Columns.Clear(); // Remove all columns

        }

        private void button13_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to pay?", "PAYMENT", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                button13.Enabled = false;
                button14.Enabled = true;

                // Ensure textBox1 ends with .00
                if (decimal.TryParse(textBox1.Text.Trim('₱'), out decimal textBoxValue))
                {
                    textBox1.Text = $"₱{textBoxValue:F2}"; // Format with two decimal places
                }
                else
                {
                    MessageBox.Show("Invalid numeric value in textBox1.");
                    return; // Exit if invalid input
                }

                // Ensure label4 value is valid and perform subtraction
                if (decimal.TryParse(label4.Text.Trim('₱'), out decimal label4Value))
                {
                    // Perform subtraction
                    decimal result = textBoxValue - label4Value;

                    // Display the result in label8 with currency formatting
                    label8.Text = $"₱{result:F2}";
                }
                else
                {
                    MessageBox.Show("Invalid numeric value in label4.");
                }
            }
            else if (dialogResult == DialogResult.No)
            {
                // Do something else if the user chooses "No"
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.PrintPreviewControl.Zoom = 1;
            printPreviewDialog1.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get the selected ingredient_name
            string selectedIngredient = comboBox1.SelectedValue?.ToString();

            if (!string.IsNullOrEmpty(selectedIngredient))
            {
                LoadUnitOfMeasure(selectedIngredient);
            }
        }

        private void LoadUnitOfMeasure(string ingredientName)
        {
            string query = "SELECT unit_of_measure FROM tbl_ingredients WHERE ingredient_name = @ingredient_name";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ingredient_name", ingredientName);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            // Create a DataTable to hold the data
                            DataTable dt = new DataTable();
                            dt.Load(reader);

                            // Bind the DataTable to comboBox2
                            comboBox2.DataSource = dt;
                            comboBox2.DisplayMember = "unit_of_measure"; // Column to display
                            comboBox2.ValueMember = "unit_of_measure";   // Column to use as value
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading unit of measure: " + ex.Message);
                }
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            UpdateQuantity();
        }

        private void UpdateQuantity()
        {
            // Get selected values
            string ingredientName = comboBox1.SelectedValue?.ToString();
            string unitOfMeasure = comboBox2.SelectedValue?.ToString();

            // Validate input
            if (string.IsNullOrEmpty(ingredientName) || string.IsNullOrEmpty(unitOfMeasure))
            {
                MessageBox.Show("Please select an ingredient and unit of measure.");
                return;
            }

            if (!int.TryParse(textBox2.Text, out int quantityToDeduct))
            {
                MessageBox.Show("Please enter a valid numeric quantity.");
                return;
            }

            string fetchQuery = "SELECT quantity FROM tbl_ingredients WHERE ingredient_name = @ingredient_name AND unit_of_measure = @unit_of_measure";
            string updateQuery = "UPDATE tbl_ingredients SET quantity = @new_quantity WHERE ingredient_name = @ingredient_name AND unit_of_measure = @unit_of_measure";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Step 1: Fetch the current quantity
                    int currentQuantity;
                    using (OleDbCommand fetchCmd = new OleDbCommand(fetchQuery, connection))
                    {
                        fetchCmd.Parameters.AddWithValue("@ingredient_name", ingredientName);
                        fetchCmd.Parameters.AddWithValue("@unit_of_measure", unitOfMeasure);

                        object result = fetchCmd.ExecuteScalar();
                        if (result == null || !int.TryParse(result.ToString(), out currentQuantity))
                        {
                            MessageBox.Show("Could not fetch current quantity. Please check your selection.");
                            return;
                        }
                    }

                    // Step 2: Deduct the entered quantity
                    int newQuantity = currentQuantity - quantityToDeduct;
                    if (newQuantity < 0)
                    {
                        MessageBox.Show("Entered quantity exceeds the available stock.");
                        return;
                    }

                    // Step 3: Update the database with the new quantity
                    using (OleDbCommand updateCmd = new OleDbCommand(updateQuery, connection))
                    {
                        updateCmd.Parameters.AddWithValue("@new_quantity", newQuantity);
                        updateCmd.Parameters.AddWithValue("@ingredient_name", ingredientName);
                        updateCmd.Parameters.AddWithValue("@unit_of_measure", unitOfMeasure);

                        updateCmd.ExecuteNonQuery();
                        MessageBox.Show("Quantity updated successfully!");
                        button13.Enabled = true;
                        button16.Enabled = false;

                        // Optionally reload data or reset UI
                        textBox2.Clear();
                        LoadIngredientNames(); // Assuming this refreshes your DataGridView or UI
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating quantity: " + ex.Message);
                }
            }
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Bitmap imagebmp = new Bitmap(dataGridView1.Width, dataGridView1.Height);
            dataGridView1.DrawToBitmap(imagebmp, new Rectangle(0, 0, dataGridView1.Width, dataGridView1.Height));
            e.Graphics.DrawImage(imagebmp, 160, 20);


            // Draw the TextBox text below the DataGridView image
            string label4text = "Total Amount: " + label4.Text; 
            Font font = new Font("Century Gothic", 16); 
            Brush brush = Brushes.Black; 
            e.Graphics.DrawString(label4text, font, brush, new PointF(400, 20 + imagebmp.Height + 20));

            
            string textBox1Box = "Cash: " + textBox1.Text; 
            Font font2 = new Font("Century Gothic", 16); 
            Brush brush2 = Brushes.Black; 
            e.Graphics.DrawString(textBox1Box, font2, brush2, new PointF(490, 20 + imagebmp.Height + 45));

            string label8text = "Change: " + label8.Text;
            Font font3 = new Font("Century Gothic", 16);
            Brush brush3 = Brushes.Black;
            e.Graphics.DrawString(label8text, font3, brush3, new PointF(470, 20 + imagebmp.Height + 70));


        }
    }

}
