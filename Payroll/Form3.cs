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
using static Payroll.Form4;

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
            // Initialize the list of custom ingredients
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
            // You can specify the default category to load on form load (e.g., "Coffee")
            LoadTable("Coffee", flowLayoutPanel1); // Default category: Coffee, panel: flowLayoutPanel1
            LoadCategories(); // This will dynamically add tabs for each category
            isFormLoading = false; // Set the flag to false once the form is fully loaded
                                   // Simulate selecting the default tab (Coffee)
            tabControl1.SelectedIndex = 0; // Select the first tab (Coffee) by default
                                           // Manually call the method to load products for the selected category (Coffee)
            LoadTable(tabControl1.SelectedTab.Text, flowLayoutPanel1); // Assuming flowLayoutPanel1 is for Coffee
            InitializeDataGridView();
            InitializeFlowLayoutPanel();
            //DisplayCashierName();
            
        }
        public void ResetDataGridView()
        {
            //dataGridView1.DataSource = null; // Clear the DataSource
            dataGridView1.Rows.Clear(); // Optionally clear rows
            //dataGridView1.Columns.Clear(); // Optionally clear columns
        }

        private void Form3_Shown(object sender, EventArgs e)
        {
            ResetDataGridView(); // Reset the DataGridView when Form3 is shown
            ReloadProductButtons(); // Reload product buttons in the FlowLayoutPanel
        }

        private void ReloadProductButtons()
        {
            // Example: Reload Coffee products into flowLayoutPanel1
            LoadTable("Coffee", flowLayoutPanel1);

            // You can similarly load Tea or Drinks if needed:
            // LoadTable("Tea", flowLayoutPanel2);
            // LoadTable("Drinks", flowLayoutPanel3);
        }

        // Helper method to check stock levels using Status
        // Helper method to check stock levels using Status
        private bool GetIngredientStockStatus(int itemID, decimal quantityUsed)
        {
            bool isInStock = false;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT [Quantity] FROM tbl_ingredients WHERE [ItemID] = @ItemID";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ItemID", itemID);

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal currentStock = reader["Quantity"] != DBNull.Value ? Convert.ToDecimal(reader["Quantity"]) : 0m;

                                // Check if the current stock is greater than or equal to the quantity used
                                isInStock = currentStock >= quantityUsed;
                            }
                        }
                    }
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return isInStock;
        }


        // Helper method to get ingredients for a given product
        private List<Ingredient> GetIngredientsForProduct(int productID)
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"SELECT r.[IngredientID], i.[Ingredient], i.[Unit of Measure], r.[QuantityUsed]
                             FROM [tbl_recipe] AS r 
                             INNER JOIN [tbl_ingredients] AS i ON r.[IngredientID] = i.[ItemID]
                             WHERE r.[ProductID] = @ProductID";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductID", productID);

                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Safely retrieve values and handle DBNull
                                int itemId = reader["IngredientID"] != DBNull.Value ? Convert.ToInt32(reader["IngredientID"]) : 0;
                                string ingredientName = reader["Ingredient"] != DBNull.Value ? reader["Ingredient"].ToString() : string.Empty;
                                string unitOfMeasure = reader["Unit of Measure"] != DBNull.Value ? reader["Unit of Measure"].ToString() : string.Empty;
                                decimal quantityUsed = reader["QuantityUsed"] != DBNull.Value ? Convert.ToDecimal(reader["QuantityUsed"]) : 0m;

                                // Add the ingredient to the list
                                ingredients.Add(new Ingredient
                                {
                                    ItemID = itemId,
                                    IngredientName = ingredientName,
                                    UnitOfMeasure = unitOfMeasure,
                                    QuantityUsed = quantityUsed
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}\n\nStack Trace:\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return ingredients;
        }


        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Skip event processing if the form is still loading
            if (isFormLoading)
            {
                return;
            }

            // Ensure that a valid tab is selected
            if (tabControl1.SelectedTab != null)
            {
                // Get the selected tab's text (category name)
                string selectedCategory = tabControl1.SelectedTab.Text;

                // Determine the corresponding FlowLayoutPanel
                FlowLayoutPanel selectedPanel = null;

                foreach (TabPage tabPage in tabControl1.TabPages)
                {
                    if (tabPage.Text == selectedCategory)
                    {
                        selectedPanel = (FlowLayoutPanel)tabPage.Controls[0]; // Get the first control (FlowLayoutPanel)
                        break;
                    }
                }

                if (selectedPanel != null)
                {
                    // Load products based on the selected category
                    LoadTable(selectedCategory, selectedPanel);
                }
                else
                {
                    MessageBox.Show("No FlowLayoutPanel found for the selected category.");
                }
            }
            else
            {
                MessageBox.Show("No tab is selected.");
            }
        }

        private bool isFormLoading = true; // Flag to track if the form is still loading


        private void LoadCategories()
        {
            // Clear previous tabs if any
            tabControl1.TabPages.Clear();

            // Use OleDbConnection to fetch categories from the database
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Query to fetch unique categories
                    string query = "SELECT DISTINCT [Category] FROM tbl_products";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        using (OleDbDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string category = reader["Category"].ToString();
                                TabPage newTab = new TabPage(category);

                                // Create a new FlowLayoutPanel for each category
                                FlowLayoutPanel newPanel = new FlowLayoutPanel
                                {
                                    Dock = DockStyle.Fill,
                                    AutoScroll = true
                                };
                                newTab.Controls.Add(newPanel);
                                tabControl1.TabPages.Add(newTab);
                            }
                        }
                    }
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadTable(string category, FlowLayoutPanel flowLayoutPanel)
        {
            // Clear previous controls (buttons) before adding new ones
            flowLayoutPanel.Controls.Clear();

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Query to fetch products by category
                    string query = "SELECT [Product Name], [Price], [ProductID] FROM tbl_products WHERE [Category] = @Category";
                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Category", category);

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            // Iterate through each row and create a button for each product
                            foreach (DataRow row in dataTable.Rows)
                            {
                                int productID = Convert.ToInt32(row["ProductID"]);
                                string productName = row["Product Name"].ToString();
                                decimal productPrice = Convert.ToDecimal(row["Price"]);

                                // Create a new button for the product
                                Button btnProduct = new Button
                                {
                                    Text = $"{productName} - ₱{productPrice}",
                                    Width = 185,
                                    Height = 100,
                                    Tag = productID // Store ProductID for later use
                                };
                                // Define updated darker red for out of stock
                                Color pastelGreen = ColorTranslator.FromHtml("#B4D7A8"); // In Stock
                                Color pastelYellow = ColorTranslator.FromHtml("#FFF4B1"); // Low Stock
                                Color brickRed = ColorTranslator.FromHtml("#B74D4D");   // Out of Stock

                                // Add event handler for button click
                                btnProduct.Click += (s, ev) =>
                                {
                                    // Check if the button is red (out of stock)
                                    if (btnProduct.BackColor == brickRed)
                                    {
                                        MessageBox.Show(
                                            $"Cannot be added because one or more ingredients are out of stock.",
                                            "Out of Stock",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Warning
                                        );
                                        return; // Prevent adding the product to the cart
                                    }
                                    AddProductToGrid(row);
                                };

                                // Check ingredients for the product and set the button color
                                var ingredients = GetIngredientsForProduct(productID);
                                bool allInStock = true;
                                bool someLowStock = false;

                                // Check ingredients for the product and set the button color
                                foreach (var ingredient in ingredients)
                                {
                                    bool isInStock = GetIngredientStockStatus(ingredient.ItemID, ingredient.QuantityUsed);

                                    if (!isInStock)
                                    {
                                        allInStock = false;
                                        break;
                                    }
                                    else if (isInStock && !someLowStock)
                                    {
                                        someLowStock = true;
                                    }
                                }

                                
                                // Set button color based on stock availability
                                if (allInStock)
                                {
                                    btnProduct.BackColor = pastelGreen; // In Stock
                                }
                                else if (someLowStock)
                                {
                                    btnProduct.BackColor = pastelYellow; // Low Stock
                                }
                                else
                                {
                                    btnProduct.BackColor = brickRed;   // Out of Stock
                                }

                                // Add the button to the FlowLayoutPanel
                                flowLayoutPanel.Controls.Add(btnProduct);
                            }
                        }
                    }
                }
                catch (OleDbException ex)
                {
                    MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            string prodName = row["Product Name"].ToString();
            decimal prodPrice = Convert.ToDecimal(row["Price"]);
            int quantity = 1; // Default quantity for a new product

            // Check if the product already exists in the DataGridView
            foreach (DataGridViewRow dgvRow in dataGridView1.Rows)
            {
                if (dgvRow.Cells["Product Name"].Value?.ToString() == prodName)
                {
                    // If the product exists, update the quantity and amount
                    int currentQuantity = Convert.ToInt32(dgvRow.Cells["quantity"].Value);
                    dgvRow.Cells["quantity"].Value = currentQuantity + 1;
                    dgvRow.Cells["amount"].Value = (currentQuantity + 1) * prodPrice;

                    // Exit the method once the product is updated
                    return;
                }
            }

            // If the product doesn't exist, add a new row
            dataGridView1.Rows.Add(null, prodName, quantity, prodPrice, quantity * prodPrice);
        }

        private void InitializeDataGridView()
        {
            if (dataGridView1.Columns.Count == 0) // Initialize columns only once
            {
                // Clear existing columns (optional)
                dataGridView1.Columns.Clear();

                // Add required columns
                dataGridView1.Columns.Add("ProductID", "ProductID"); // Optional, can be hidden or removed
                dataGridView1.Columns.Add("Product Name", "Product Name");
                dataGridView1.Columns.Add("quantity", "Quantity");
                dataGridView1.Columns.Add("Price", "Price");
                dataGridView1.Columns.Add("amount", "Amount");

                // Make sure the ID column is hidden if it's not required
                dataGridView1.Columns["ProductID"].Visible = false;

                // Add the Delete button column
                DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
                deleteButtonColumn.Name = "delete";
                deleteButtonColumn.HeaderText = "Delete";
                deleteButtonColumn.Text = "Delete";
                deleteButtonColumn.UseColumnTextForButtonValue = true; // Use the same text for all buttons
                dataGridView1.Columns.Add(deleteButtonColumn);

                // Optionally, make other adjustments (like column widths)
                dataGridView1.CellFormatting += dataGridView1_CellFormatting;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AllowUserToAddRows = false; // Disables the option for users to add rows directly
            }
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
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Step 1: Validate ingredient stocks for all products in the cart
                    bool hasStockIssues = false;
                    StringBuilder stockIssuesMessage = new StringBuilder();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow || row.Cells["ProductID"].Value == null) continue;

                        int productID = Convert.ToInt32(row.Cells["ProductID"].Value);
                        decimal productQuantity = Convert.ToDecimal(row.Cells["Quantity"].Value);

                        // Fetch ingredients for this product
                        var ingredients = GetIngredientsForProduct(productID);

                        foreach (var ingredient in ingredients)
                        {
                            decimal requiredQuantity = ingredient.QuantityUsed * productQuantity;

                            // Check stock for each ingredient
                            if (!HasSufficientStock(ingredient.ItemID, requiredQuantity, connection))
                            {
                                hasStockIssues = true;
                                stockIssuesMessage.AppendLine($"Product: {row.Cells["Product Name"].Value} - Ingredient: {ingredient.IngredientName} is out of stock.");
                            }
                        }
                    }

                    // Step 2: Show error message if there are stock issues and stop the process
                    if (hasStockIssues)
                    {
                        MessageBox.Show(stockIssuesMessage.ToString(), "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Stop the order process
                    }

                    // Step 3: Deduct stock for all ingredients in the cart
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow || row.Cells["ProductID"].Value == null) continue;

                        int productID = Convert.ToInt32(row.Cells["ProductID"].Value);
                        decimal productQuantity = Convert.ToDecimal(row.Cells["Quantity"].Value);

                        // Fetch ingredients for this product
                        var ingredients = GetIngredientsForProduct(productID);

                        foreach (var ingredient in ingredients)
                        {
                            decimal requiredQuantity = ingredient.QuantityUsed * productQuantity;
                            DeductIngredientStock(ingredient.ItemID, requiredQuantity, connection);
                        }
                    }

                    // Step 4: Create a DataTable to pass to Form5
                    DataTable tableData = new DataTable();

                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        if (column.Name != "delete") // Skip the delete button column
                        {
                            tableData.Columns.Add(column.Name, typeof(string));
                        }
                    }

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

                    // Step 5: Pass the DataTable to Form5
                    if (tableData.Rows.Count > 0)
                    {
                        Form5 receiptForm = new Form5(tableData);
                        receiptForm.ShowDialog();

                        // Reset Form3
                        ResetDataGridView();  // Reset the DataGridView in Form3
                        ReloadProductButtons(); // Re-add product buttons after resetting
                    }
                    else
                    {
                        MessageBox.Show("No data available to transfer.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool HasSufficientStock(int itemID, decimal requiredQuantity, OleDbConnection connection)
        {
            string query = "SELECT [Quantity] FROM tbl_ingredients WHERE [ItemID] = @ItemID";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ItemID", itemID);

                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal currentStock = reader["Quantity"] != DBNull.Value ? Convert.ToDecimal(reader["Quantity"]) : 0m;
                        return currentStock >= requiredQuantity;
                    }
                }
            }
            return false; // Default to false if no stock data found
        }
        private void DeductIngredientStock(int itemID, decimal quantity, OleDbConnection connection)
        {
            string query = "UPDATE tbl_ingredients SET [Quantity] = [Quantity] - @Quantity WHERE [ItemID] = @ItemID";
            using (OleDbCommand command = new OleDbCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@ItemID", itemID);

                command.ExecuteNonQuery();
            }
        }

        private void btn_Logout_Click(object sender, EventArgs e)
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