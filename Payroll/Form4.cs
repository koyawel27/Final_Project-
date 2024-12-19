using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
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
            SetupDataGridView();
            // Populate the DataGridView with ingredients
            PopulateDataGridViewWithIngredients();
            dataGridViewIngredients.DataError += dataGridViewIngredients_DataError;
            dataGridViewIngredients.EditingControlShowing += dataGridViewIngredients_EditingControlShowing;  // Add this line to hook the event
        }

        

        
        private void dataGridViewIngredients_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Handle the data error here, showing the exception message
            MessageBox.Show($"Data error: {e.Exception.Message}");
        }

        private List<NumericUpDown> ingredientNumericUpDowns = new List<NumericUpDown>();  // To store dynamically created NumericUpDowns

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

                        // Suspend layout updates
                        dataGridView1.SuspendLayout();

                        // Clear existing data source
                        dataGridView1.DataSource = null;

                        // Set new data source
                        dataGridView1.DataSource = dataTable;

                        // Resume layout updates
                        dataGridView1.ResumeLayout();

                        // Optional: Auto-size columns for better display
                        dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void SetupDataGridView()
        {
            // Add Ingredient column (ComboBox)
            DataGridViewComboBoxColumn ingredientColumn = new DataGridViewComboBoxColumn();
            ingredientColumn.Name = "Ingredient";
            ingredientColumn.HeaderText = "Ingredient";

            // Fetch ingredients and bind them
            List<Ingredient> ingredients = GetIngredients();
            ingredientColumn.DataSource = ingredients;
            ingredientColumn.DisplayMember = "IngredientName";  // Display name of the ingredient
            ingredientColumn.ValueMember = "IngredientID";     // Set the value to the IngredientID
            dataGridViewIngredients.Columns.Add(ingredientColumn);

            // Add Unit column (TextBox)
            DataGridViewTextBoxColumn unitColumn = new DataGridViewTextBoxColumn();
            unitColumn.Name = "Unit";
            unitColumn.HeaderText = "Unit";
            dataGridViewIngredients.Columns.Add(unitColumn);

            // Add Quantity column (TextBox)
            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn();
            quantityColumn.Name = "Quantity";
            quantityColumn.HeaderText = "Quantity";
            dataGridViewIngredients.Columns.Add(quantityColumn);
        }



        private void PopulateDataGridViewWithIngredients()
        {
            List<Ingredient> ingredients = GetIngredients();

            foreach (var ingredient in ingredients)
            {
                // Add a new row with IngredientID (not IngredientName), leave Quantity as blank or allow user to input
                dataGridViewIngredients.Rows.Add(ingredient.IngredientID, ingredient.Unit, DBNull.Value);  // Use DBNull for empty value
            }
        }



        public class Ingredient
        {
            public int IngredientID { get; set; }  // Unique identifier for each ingredient
            public string IngredientName { get; set; }  // Name of the ingredient
            public string Unit { get; set; }  // Unit of measure (e.g., g, ml)
        }

        private List<Ingredient> GetIngredients()
        {
            List<Ingredient> ingredients = new List<Ingredient>();

            string query = "SELECT ItemID, Ingredient, [Unit of Measure] FROM tbl_ingredients";
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                OleDbCommand cmd = new OleDbCommand(query, conn);
                conn.Open();
                OleDbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    ingredients.Add(new Ingredient
                    {
                        IngredientID = Convert.ToInt32(reader["ItemID"]),
                        IngredientName = reader["Ingredient"].ToString(),
                        Unit = reader["Unit of Measure"].ToString() // Get Unit of Measure
                    });
                }
            }
            return ingredients;
        }



        private void dataGridViewIngredients_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridViewIngredients.CurrentCell.ColumnIndex == dataGridViewIngredients.Columns["Ingredient"].Index)
            {
                ComboBox comboBox = e.Control as ComboBox;
                if (comboBox != null)
                {
                    // Unsubscribe from previous event to avoid duplicate handlers
                    comboBox.SelectedIndexChanged -= ComboBox_SelectedIndexChanged;
                    // Subscribe to the event when the selection changes
                    comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
                }
            }
        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null && dataGridViewIngredients.CurrentCell != null)
            {
                // Get the selected ingredient directly since it's already an Ingredient object
                Ingredient selectedIngredient = comboBox.SelectedItem as Ingredient;

                // If the ingredient is found, update the Unit column
                if (selectedIngredient != null)
                {
                    dataGridViewIngredients.CurrentRow.Cells["Unit"].Value = selectedIngredient.Unit;
                }
            }
        }






        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

            // Populate textboxes
            textBox1.Text = row.Cells["Category"].Value?.ToString(); // Category (Product Type)
            textBox2.Text = row.Cells["Product Name"].Value?.ToString(); // Product Name
            textBox3.Text = row.Cells["Price"].Value?.ToString(); // Price

            // Load recipe ingredients for the selected product
            if (row.Cells["ProductID"].Value != null)
            {
                int productID = Convert.ToInt32(row.Cells["ProductID"].Value);
                LoadIngredients(productID); // Load the recipe ingredients into ingredients DataGridView
            }
        }

        private void LoadIngredients(int productID)
        {
            string query = "SELECT r.IngredientID, i.Ingredient, r.QuantityUsed, r.Unit " +
                           "FROM tbl_recipe r " +
                           "INNER JOIN tbl_ingredients i ON r.IngredientID = i.ItemID " +
                           "WHERE r.ProductID = @productID";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@productID", productID);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            // Clear existing rows but keep the columns intact
                            dataGridViewIngredients.Rows.Clear();

                            while (reader.Read())
                            {
                                // Add new row with ingredient data
                                dataGridViewIngredients.Rows.Add(
                                    reader["IngredientID"],
                                    reader["Ingredient"],
                                    reader["QuantityUsed"],
                                    reader["Unit"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading ingredients: " + ex.Message);
                }
            }
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

            // Insert the product into tbl_products
            string productQuery = "INSERT INTO tbl_products ([Category], [Product Name], [Price]) VALUES (@prodType, @prodName, @prodPrice)";
            int productId = 0;
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                using (OleDbCommand cmd = new OleDbCommand(productQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@prodType", prodType);
                    cmd.Parameters.AddWithValue("@prodName", prodName);
                    cmd.Parameters.AddWithValue("@prodPrice", prodPrice);
                    cmd.ExecuteNonQuery();

                    // Get the last inserted product's ID
                    using (OleDbCommand cmdGetID = new OleDbCommand("SELECT @@IDENTITY", conn))
                    {
                        productId = Convert.ToInt32(cmdGetID.ExecuteScalar());
                    }
                }
            }

            // Now, insert the ingredients for this product from the DataGridView
            foreach (DataGridViewRow row in dataGridViewIngredients.Rows)
            {
                if (row.IsNewRow) continue;

                int ingredientId = Convert.ToInt32(row.Cells["Ingredient"].Value);
                string unit = row.Cells["Unit"].Value.ToString();  // Get the unit value

                // Check if Quantity is valid (not null, empty, or zero)
                if (row.Cells["Quantity"].Value != DBNull.Value &&
                    !string.IsNullOrEmpty(row.Cells["Quantity"].Value.ToString()))
                {
                    decimal quantity;

                    // Only try to parse and insert if Quantity is provided
                    if (Decimal.TryParse(row.Cells["Quantity"].Value.ToString(), out quantity) && quantity > 0)
                    {
                        // Insert valid ingredient data into tbl_recipe
                        string recipeQuery = "INSERT INTO tbl_recipe (ProductID, IngredientID, QuantityUsed, Unit) VALUES (@prodID, @ingID, @quantity, @unit)";
                        using (OleDbConnection conn = new OleDbConnection(connectionString))
                        {
                            conn.Open();
                            using (OleDbCommand cmd = new OleDbCommand(recipeQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@prodID", productId);
                                cmd.Parameters.AddWithValue("@ingID", ingredientId);
                                cmd.Parameters.AddWithValue("@quantity", quantity);
                                cmd.Parameters.AddWithValue("@unit", unit);  // Pass unit as parameter
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    else
                    {
                        // Show error message if quantity is invalid or zero
                        string ingredientName = row.Cells["Ingredient"].Value.ToString();
                        MessageBox.Show($"Invalid quantity for ingredient: {ingredientName}. Please provide a valid quantity.");
                    }
                }
                // Skip rows where Quantity is empty for ingredients that are not used
            }

            MessageBox.Show("Product and ingredients added successfully!");
            LoadTable(); // Add this line to refresh the DataGridView
                         // Clear the form
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            dataGridViewIngredients.Rows.Clear(); // Clear the ingredients DataGridView
        }



        private void UpdateProduct()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a row to update.");
                return;
            }

            int productID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ProductID"].Value); // ProductID
            string prodType = textBox1.Text;
            string prodName = textBox2.Text;
            decimal prodPrice;

            if (!decimal.TryParse(textBox3.Text, out prodPrice))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }

            // Update the product details in tbl_products
            string updateProductQuery = "UPDATE tbl_products SET [Category] = @prodType, [Product Name] = @prodName, [Price] = @prodPrice WHERE ProductID = @productID";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (OleDbCommand cmd = new OleDbCommand(updateProductQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@prodType", prodType);
                        cmd.Parameters.AddWithValue("@prodName", prodName);
                        cmd.Parameters.AddWithValue("@prodPrice", prodPrice);
                        cmd.Parameters.AddWithValue("@productID", productID);

                        cmd.ExecuteNonQuery();
                    }

                    // Delete existing ingredients/recipe entries for the product in tbl_recipe
                    string deleteRecipeQuery = "DELETE FROM tbl_recipe WHERE ProductID = @prodID";
                    using (OleDbCommand deleteCmd = new OleDbCommand(deleteRecipeQuery, connection))
                    {
                        deleteCmd.Parameters.AddWithValue("@prodID", productID);
                        deleteCmd.ExecuteNonQuery();
                    }

                    // Insert the updated ingredients/recipe from the DataGridView
                    foreach (DataGridViewRow row in dataGridViewIngredients.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int ingredientId = Convert.ToInt32(row.Cells["Ingredient"].Value);
                        string unit = row.Cells["Unit"].Value?.ToString() ?? string.Empty;

                        // Check if Quantity is valid
                        if (row.Cells["Quantity"].Value != DBNull.Value &&
                            !string.IsNullOrEmpty(row.Cells["Quantity"].Value.ToString()))
                        {
                            decimal quantity;
                            if (decimal.TryParse(row.Cells["Quantity"].Value.ToString(), out quantity) && quantity > 0)
                            {
                                // Insert updated ingredient data into tbl_recipe
                                string recipeQuery = "INSERT INTO tbl_recipe (ProductID, IngredientID, QuantityUsed, Unit) VALUES (@prodID, @ingID, @quantity, @unit)";
                                using (OleDbCommand insertCmd = new OleDbCommand(recipeQuery, connection))
                                {
                                    insertCmd.Parameters.AddWithValue("@prodID", productID);
                                    insertCmd.Parameters.AddWithValue("@ingID", ingredientId);
                                    insertCmd.Parameters.AddWithValue("@quantity", quantity);
                                    insertCmd.Parameters.AddWithValue("@unit", unit);
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                string ingredientName = row.Cells["Ingredient"].Value.ToString();
                                MessageBox.Show($"Invalid quantity for ingredient: {ingredientName}. Please provide a valid quantity.");
                            }
                        }
                    }

                    MessageBox.Show("Product and ingredients updated successfully!");
                    LoadTable(); // Refresh DataGridView
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
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

            // Get the selected row ProductID (hidden column)
            int productID = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ProductID"].Value);

            // Ask for confirmation
            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?",
                                                  "Confirm Deletion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // First, archive the product before deleting it
                string loggedInUsername = LoggedInUser.Username;
                //ArchiveAndDeleteProduct(productID, loggedInUsername);

                // Proceed to delete the product from the tbl_products table
                string query = "DELETE FROM tbl_products WHERE ProductID = @productID";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (OleDbCommand cmd = new OleDbCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@productID", productID);
                            cmd.ExecuteNonQuery();

                            // Refresh the DataGridView to reflect the changes
                            LoadTable();

                            MessageBox.Show("Product deleted successfully!");

                            // Clear any input fields and reset the UI
                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();
                            dataGridViewIngredients.Rows.Clear(); // Clear ingredients list
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }



        private void button8_Click(object sender, EventArgs e)
        {
            DeleteProduct();
        }

        /*private void ArchiveAndDeleteProduct(int productId, string username)
        {
            // Connection string (replace with your actual connection string)
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();

                // Select the record from tbl_products
                string selectQuery = "SELECT * FROM tbl_products WHERE ProductID = @ProductID";
                using (OleDbCommand cmd = new OleDbCommand(selectQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) // If record found
                    {
                        // Insert the record into the archive table (tbl_products_archive)
                        string archiveQuery = @"
                    INSERT INTO tbl_products_archive 
                    (ProductID, Product Name, Price, Category, DeletedDate, DeletedBy) 
                    VALUES 
                    (@ProductID, @ProductName, @Price, @Category, @DeletedDate, @DeletedBy)";

                        using (OleDbCommand archiveCmd = new OleDbCommand(archiveQuery, connection))
                        {
                            archiveCmd.Parameters.AddWithValue("@ProductID", reader["ProductID"]);
                            archiveCmd.Parameters.AddWithValue("@ProductName", reader["Product Name"]);
                            archiveCmd.Parameters.AddWithValue("@Price", reader["Price"]);
                            archiveCmd.Parameters.AddWithValue("@Category", reader["Category"]);
                            archiveCmd.Parameters.AddWithValue("@DeletedDate", DateTime.Now);
                            archiveCmd.Parameters.AddWithValue("@DeletedBy", username);

                            // Execute the insert query to archive the record
                            archiveCmd.ExecuteNonQuery();
                        }

                        // Now delete the record from the original table (tbl_products)
                        string deleteQuery = "DELETE FROM tbl_products WHERE ProductID = @ProductID";
                        using (OleDbCommand deleteCmd = new OleDbCommand(deleteQuery, connection))
                        {
                            deleteCmd.Parameters.AddWithValue("@ProductID", productId);
                            deleteCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }*/


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
			this.Hide();
			Form8 form8 = new Form8();
			form8.Show();
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

        private void BackupRestore_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form10 form10 = new Form10();
            form10.Show();
        }
    }
}



