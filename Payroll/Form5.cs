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
			label11.Text = DateTime.Now.ToString("MMMM dd, yyyy");
			label12.Text = DateTime.Now.ToLongTimeString();

			/*LoadIngredientNames();*/
			dataGridView1.Columns["ProductID"].Visible = true;
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

            /*if (dataGridView1.Columns.Contains("prod_name"))
            {
                dataGridView1.Columns["prod_name"].HeaderText = "prod_name"; // Change column name to "Product Name"
            }*/

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
            // Confirm cancellation
            DialogResult result = MessageBox.Show("Are you sure you want to cancel the order?",
                                                  "Cancel Order",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // Clear any data or reset as necessary
                dataGridView1.DataSource = null;  // Unbind any data source
                dataGridView1.Rows.Clear();       // Remove all rows
                dataGridView1.Columns.Clear();    // Remove all columns

                // Optionally clear any other order-related controls here

                // Now redirect back to Form3 (POS menu)
                Form3 posMenu = new Form3();
                posMenu.Show();  // Show the POS menu
                this.Hide();     // Hide Form5
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to pay?", "PAYMENT", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                // Ensure textBox1 contains a valid numeric value
                if (!decimal.TryParse(textBox1.Text.Trim('₱'), out decimal enteredAmount))
                {
                    MessageBox.Show("Please enter a valid payment amount.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit if invalid input
                }

                decimal totalAmount = 0;

                // Ensure label4 text is not DBNull or invalid before parsing
                if (!decimal.TryParse(label4.Text.Trim('₱'), out totalAmount))
                {
                    MessageBox.Show("Invalid total amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit if invalid total amount
                }


                // Check if entered amount is insufficient
                if (enteredAmount < totalAmount)
                {
                    MessageBox.Show("The entered amount is insufficient. Please enter an amount greater than or equal to the total.",
                                    "Insufficient Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // Exit if insufficient amount
                }

                // Perform subtraction and update label8 for change
                decimal change = enteredAmount - totalAmount;
                label8.Text = $"₱{change:F2}"; // Display the change

                // Proceed with recording the transaction
                button13.Enabled = false;
                button14.Enabled = true;
                addTransaction();

                // Ensure textBox1 ends with .00
                textBox1.Text = $"₱{enteredAmount:F2}"; // Format with two decimal places
            }
            else if (dialogResult == DialogResult.No)
            {
                // Do something else if the user chooses "No"
            }
        }

        private void addTransaction()
        {
            string transact_date = label11.Text; // Transaction date from label
            string transact_time = label12.Text; // Transaction time from label
                                                 
            string cashier_name = LoggedInUser.Username;// Use the logged-in user's username as cashier_name
            if (string.IsNullOrEmpty(LoggedInUser.Username))
            {
                MessageBox.Show("Cashier name is not set. Please ensure you are logged in properly.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string transactionQuery = "INSERT INTO tbl_transaction_new ([cashier_name], [prod_name], [quantity], [price], " +
                                       "[amount], [transact_date], [transact_time]) " +
                                       "VALUES (@cashier_name, @prod_name, @quantity, @price, @amount, @transact_date, @transact_time)";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        // Skip rows that are new or incomplete
                        if (row.IsNewRow ||
                            row.Cells["Product Name"].Value == null ||
                            row.Cells["quantity"].Value == null ||
                            row.Cells["price"].Value == null ||
                            row.Cells["amount"].Value == null)
                        {
                            continue;
                        }

                        string productName = row.Cells["Product Name"].Value.ToString();
                        int quantitySold = Convert.ToInt32(row.Cells["quantity"].Value ?? 0);

                        // Insert transaction record
                        using (OleDbCommand transactionCmd = new OleDbCommand(transactionQuery, connection))
                        {
                            transactionCmd.Parameters.AddWithValue("@cashier_name", cashier_name ?? string.Empty);
                            transactionCmd.Parameters.AddWithValue("@prod_name", productName);
                            transactionCmd.Parameters.AddWithValue("@quantity", quantitySold);
                            transactionCmd.Parameters.AddWithValue("@price", Convert.ToDecimal(row.Cells["price"].Value ?? 0));
                            transactionCmd.Parameters.AddWithValue("@amount", Convert.ToDecimal(row.Cells["amount"].Value ?? 0));
                            transactionCmd.Parameters.AddWithValue("@transact_date", transact_date ?? string.Empty);
                            transactionCmd.Parameters.AddWithValue("@transact_time", transact_time ?? string.Empty);

                            transactionCmd.ExecuteNonQuery();
                        }

                        // Deduct ingredients based on the recipe
                        DeductIngredients(connection, productName, quantitySold);
                    }

                    MessageBox.Show("Transaction(s) added successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeductIngredients(OleDbConnection connection, string productName, int quantitySold)
        {
            try
            {
                // Step 1: Fetch the ProductID for the product name
                string productIdQuery = "SELECT ProductID FROM tbl_products WHERE [Product Name] = ?";
                int productId;

                using (OleDbCommand productCmd = new OleDbCommand(productIdQuery, connection))
                {
                    productCmd.Parameters.AddWithValue("?", productName);
                    productId = Convert.ToInt32(productCmd.ExecuteScalar());
                }

                // Step 2: Fetch the recipe for the product
                string recipeQuery = "SELECT IngredientID, QuantityUsed FROM tbl_recipe WHERE ProductID = ?";
                using (OleDbCommand recipeCmd = new OleDbCommand(recipeQuery, connection))
                {
                    recipeCmd.Parameters.AddWithValue("?", productId);

                    using (OleDbDataReader recipeReader = recipeCmd.ExecuteReader())
                    {
                        while (recipeReader.Read())
                        {
                            int ingredientId = Convert.ToInt32(recipeReader["IngredientID"]);
                            decimal quantityUsed = Convert.ToDecimal(recipeReader["QuantityUsed"]) * quantitySold;

                            // Step 3: Deduct from tbl_ingredients
                            string updateIngredientQuery = "UPDATE tbl_ingredients SET quantity = quantity - ? WHERE ItemID = ?";
                            using (OleDbCommand updateCmd = new OleDbCommand(updateIngredientQuery, connection))
                            {
                                updateCmd.Parameters.AddWithValue("?", quantityUsed);
                                updateCmd.Parameters.AddWithValue("?", ingredientId);
                                updateCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating ingredients: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
           /* string selectedIngredient = comboBox1.SelectedValue?.ToString();

            if (!string.IsNullOrEmpty(selectedIngredient))
            {
                LoadUnitOfMeasure(selectedIngredient);
            }*/
        }

        /*private void LoadUnitOfMeasure(string ingredientName)
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
        }*/

        private void button16_Click(object sender, EventArgs e)
        {
            
        }

        /*private void UpdateQuantity()
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
        }*/
        private string GetLatestTransactionID()
        {
            // Connect to your MS Access database
            using (var connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                // Query to get the latest transaction ID
                string query = "SELECT TOP 1 TransactionID FROM tbl_transaction_new ORDER BY TransactionID DESC";

                // Create and execute the query
                using (var command = new OleDbCommand(query, connection))
                {
                    var result = command.ExecuteScalar();
                    return result != null ? result.ToString() : "Unknown"; // Return the transaction ID or "Unknown" if not found
                }
            }
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Font settings
            Font headerFont = new Font("Arial", 14, FontStyle.Bold);
            Font regularFont = new Font("Arial", 10);
            Font itemFont = new Font("Arial", 10);

            // Starting positions
            float yPos = 50;
            int leftMargin = 50;
            int rightMargin = 300;

            // Store Header
            e.Graphics.DrawString("Kalye Luma", headerFont, Brushes.Black, leftMargin, yPos);
            yPos += 25;
            e.Graphics.DrawString("251 Governor Padilla Road, Banga 1", regularFont, Brushes.Black, leftMargin, yPos);
            yPos += 20;
            e.Graphics.DrawString("Plaridel, 3004 Bulacan", regularFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Transaction Info
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            e.Graphics.DrawString($"Date: {currentDate}", regularFont, Brushes.Black, leftMargin, yPos);
            yPos += 20;

            // Replacing "Cashier ID" with the current logged-in user's username
            e.Graphics.DrawString($"Cashier: {LoggedInUser.Username}", regularFont, Brushes.Black, leftMargin, yPos);
            yPos += 20;

            // Get the latest TransactionID from tbl_transaction_new and use it as Order Number
            string orderNum = GetLatestTransactionID();  // Fetch from the database
            string transNum = DateTime.Now.ToString("yyyyMMddHHmmssff");
            e.Graphics.DrawString($"Order Number: {orderNum}", regularFont, Brushes.Black, leftMargin, yPos);
            yPos += 20;
            e.Graphics.DrawString($"Transaction Number: {transNum}", regularFont, Brushes.Black, leftMargin, yPos);
            yPos += 30;

            // Headers for items
            e.Graphics.DrawString("Item", regularFont, Brushes.Black, leftMargin, yPos);
            e.Graphics.DrawString("Qty", regularFont, Brushes.Black, rightMargin - 50, yPos);
            yPos += 20;

            // Draw line separator
            e.Graphics.DrawLine(Pens.Black, leftMargin, yPos, rightMargin, yPos);
            yPos += 10;

            // Items from DataGridView
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value != null) // Check if row is not empty
                {
                    string item = row.Cells["Product Name"].Value.ToString();
                    string qty = row.Cells["quantity"].Value.ToString();
                    string price = row.Cells["Price"].Value.ToString(); // Get the price from the DataGridView
                    decimal amount = Convert.ToDecimal(price) * Convert.ToInt32(qty); // Calculate the amount (price * quantity)

                    // Print the item name
                    e.Graphics.DrawString(item, itemFont, Brushes.Black, leftMargin, yPos);

                    // Print the quantity
                    e.Graphics.DrawString(qty, itemFont, Brushes.Black, rightMargin - 50, yPos);

                    // Print the price
                    e.Graphics.DrawString($"₱{price}", itemFont, Brushes.Black, rightMargin - 100, yPos);

                    // Print the amount (price * quantity)
                    e.Graphics.DrawString($"₱{amount:F2}", itemFont, Brushes.Black, rightMargin - 50, yPos);

                    yPos += 20; // Move to the next line
                }
            }

            // Add space before the footer
            yPos += 20;

            // Footer section (total, amount given, change)
            e.Graphics.DrawLine(Pens.Black, leftMargin, yPos, rightMargin, yPos); // Line separator before the footer
            yPos += 10; // Space after the line

            // Print total sales
            decimal totalSales = 0;
            if (Decimal.TryParse(label4.Text.Replace("₱", "").Trim(), out totalSales)) // Remove ₱ symbol and parse as decimal
            {
                e.Graphics.DrawString($"Total Sales: ₱{totalSales:F2}", regularFont, Brushes.Black, leftMargin, yPos);
            }
            else
            {
                e.Graphics.DrawString("Total Sales: Invalid Amount", regularFont, Brushes.Black, leftMargin, yPos);
            }
            yPos += 20;

            // Print amount given
            decimal amountGiven = 0;
            if (Decimal.TryParse(textBox1.Text.Replace("₱", "").Trim(), out amountGiven)) // Remove ₱ symbol and parse as decimal
            {
                e.Graphics.DrawString($"Amount Given: ₱{amountGiven:F2}", regularFont, Brushes.Black, leftMargin, yPos);
            }
            else
            {
                e.Graphics.DrawString("Amount Given: Invalid Amount", regularFont, Brushes.Black, leftMargin, yPos);
            }
            yPos += 20;

            // Print change
            decimal change = 0;
            if (Decimal.TryParse(label8.Text.Replace("₱", "").Trim(), out change)) // Remove ₱ symbol and parse as decimal
            {
                e.Graphics.DrawString($"Change: ₱{change:F2}", regularFont, Brushes.Black, leftMargin, yPos);
            }
            else
            {
                e.Graphics.DrawString("Change: Invalid Amount", regularFont, Brushes.Black, leftMargin, yPos);
            }
            yPos += 20;

            // Optionally, print a thank you message or any additional footer info
            e.Graphics.DrawString("Thank you for your purchase!", regularFont, Brushes.Black, leftMargin, yPos);
            yPos += 20;

            this.Hide(); // Optionally hide the current form after printing
            Form3 form3 = new Form3();
            form3.Show(); // Show Form3
        }


        private void btnReturn_Click(object sender, EventArgs e)
		{

		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			label12.Text = DateTime.Now.ToLongTimeString();
            timer1.Start();
		}

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }

}