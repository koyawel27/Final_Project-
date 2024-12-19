using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;  // Add this at the top


namespace Payroll
{
	public partial class Form8 : Form
	{
		private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;";
		private string tableName = "tbl_transaction_new";
		public Form8()
		{
			InitializeComponent();
		}

		private void Form8_Load(object sender, EventArgs e)
		{
            
            
				LoadData();
				CalculateTotalAmount();
				dateTimePicker1.ValueChanged += dateTimePicker1_ValueChanged;
				
			
		}

        


        private void PrintPage(object sender, PrintPageEventArgs e)
		{
			// Set the font and brush for printing
			Font font = new Font("Arial", 10);
			Brush brush = Brushes.Black;

			// Define starting position for printing
			float x = e.MarginBounds.Left;
			float y = e.MarginBounds.Top;
			float lineHeight = font.GetHeight();

			// Define spacing between columns
			float columnSpacing = 100f;  // Adjust the spacing to fit your content

			// Print the headers with your custom names
			e.Graphics.DrawString("Transaction ID", font, brush, x, y);
			e.Graphics.DrawString("Cashier", font, brush, x + columnSpacing, y);
			e.Graphics.DrawString("Product", font, brush, x + columnSpacing * 2, y);
			e.Graphics.DrawString("Quantity", font, brush, x + columnSpacing * 3, y);
			e.Graphics.DrawString("Price", font, brush, x + columnSpacing * 4, y);
			e.Graphics.DrawString("Amount", font, brush, x + columnSpacing * 5, y);
			e.Graphics.DrawString("Transaction Date", font, brush, x + columnSpacing * 6, y);
			e.Graphics.DrawString("Transaction Time", font, brush, x + columnSpacing * 7, y);

			y += lineHeight * 2; // Space between header and first row

			// Variable to store total sales
			decimal totalAmount = 0;

			// Loop through the DataGridView rows and print the data
			foreach (DataGridViewRow row in dataGridView1.Rows)
			{
				// Skip new rows or empty rows
				if (row.IsNewRow) continue;

				// Print TransactionID
				e.Graphics.DrawString(row.Cells["TransactionID"].Value.ToString(), font, brush, x, y);

				// Print Cashier (column name mapped to "Cashier")
				e.Graphics.DrawString(row.Cells["cashier_name"].Value.ToString(), font, brush, x + columnSpacing, y);

				// Print Product (column name mapped to "Product")
				e.Graphics.DrawString(row.Cells["prod_name"].Value.ToString(), font, brush, x + columnSpacing * 2, y);

				// Print Quantity
				e.Graphics.DrawString(row.Cells["quantity"].Value.ToString(), font, brush, x + columnSpacing * 3, y);

				// Print Price
				e.Graphics.DrawString(row.Cells["price"].Value.ToString(), font, brush, x + columnSpacing * 4, y);

				// Print Amount
				decimal amount = Convert.ToDecimal(row.Cells["amount"].Value);
				e.Graphics.DrawString(amount.ToString("N2"), font, brush, x + columnSpacing * 5, y);

				// Add the amount to totalAmount
				totalAmount += amount;

				// Print Transaction Date
				e.Graphics.DrawString(row.Cells["transact_date"].Value.ToString(), font, brush, x + columnSpacing * 6, y);

				// Print Transaction Time
				e.Graphics.DrawString(row.Cells["transact_time"].Value.ToString(), font, brush, x + columnSpacing * 7, y);

				y += lineHeight;
			}

			// Add the Total Sales (Gross Sales) at the bottom of the page
			y += lineHeight * 2;  // Space before the total
			e.Graphics.DrawString("Gross Sales (Total):", font, brush, x, y);
			e.Graphics.DrawString("₱" + totalAmount.ToString("N2"), font, brush, x + columnSpacing * 4, y);  // Adjust the position as needed
		}

		private void CalculateTotalAmount()
		{
			try
			{
				decimal totalAmount = 0;

				// Loop through all rows in the DataGridView
				foreach (DataGridViewRow row in dataGridView1.Rows)
				{
					// Ensure the row is not the "new row" (the blank row at the end)
					if (!row.IsNewRow)
					{
						// Check if the "amount" column has a valid value
						if (row.Cells["amount"].Value != null && decimal.TryParse(row.Cells["amount"].Value.ToString(), out decimal amount))
						{
							totalAmount += amount;
						}
					}
				}

				// Display the total amount in label2
				label2.Text = "₱" + totalAmount.ToString("N2");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error calculating total amount: " + ex.Message);
			}
		}


		private void LoadData()
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

		private void button1_Click(object sender, EventArgs e)
		{
			this.Hide();
			Form4 form4 = new Form4();
			form4.Show();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.Hide();
			Form7 form7 = new Form7();
			form7.Show(); // Show Form3
		}

		private void button4_Click(object sender, EventArgs e)
		{
			this.Hide();
			Form9 form9 = new Form9();
			form9.Show();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			this.Hide();
			Form8 form8 = new Form8();
			form8.Show();
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

		private void button10_Click(object sender, EventArgs e)
		{
			// Get the selected date range from the DateTimePickers
			string startDate = dateTimePicker1.Value.ToString("MMMM dd, yyyy");
			string endDate = dateTimePicker2.Value.ToString("MMMM dd, yyyy");

			// Query the database for transactions within the date range
			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				try
				{
					connection.Open();

					// Adjust the query to filter by date range
					string query = $"SELECT * FROM {tableName} WHERE transact_date BETWEEN @startDate AND @endDate";

					using (OleDbCommand command = new OleDbCommand(query, connection))
					{
						// Use parameters to prevent SQL injection
						command.Parameters.AddWithValue("@startDate", startDate);
						command.Parameters.AddWithValue("@endDate", endDate);

						// Execute the query and load results into a DataTable
						using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
						{
							DataTable dataTable = new DataTable();
							adapter.Fill(dataTable);

							// Bind the filtered data to the DataGridView
							dataGridView1.DataSource = dataTable;
							CalculateTotalAmount();
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error: " + ex.Message);
				}
				finally
				{
					connection.Close();
				}
			}
		}

		private void button11_Click(object sender, EventArgs e)
		{
			LoadData();
			CalculateTotalAmount();
		}

		private void button3_Click(object sender, EventArgs e)
		{
			// Get today's date in the desired format
			string todayDate = DateTime.Now.ToString("MMMM dd, yyyy");

			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				try
				{
					connection.Open();

					// Query to filter records where transact_date matches today's date
					string query = $"SELECT * FROM {tableName} WHERE transact_date = @todayDate";

					using (OleDbCommand command = new OleDbCommand(query, connection))
					{
						// Use parameter to specify today's date
						command.Parameters.AddWithValue("@todayDate", todayDate);

						// Execute the query and load results into a DataTable
						using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
						{
							DataTable dataTable = new DataTable();
							adapter.Fill(dataTable);

							// Bind the filtered data to the DataGridView
							dataGridView1.DataSource = dataTable;
							CalculateTotalAmount();
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error: " + ex.Message);
				}
				finally
				{
					connection.Close();
				}
			}
		}

		private void button7_Click(object sender, EventArgs e)
		{
			// Get the current date and calculate the start of the week (Monday)
			DateTime today = DateTime.Now;
			DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday); // Monday as the start of the week
			string startDate = startOfWeek.ToString("MMMM dd, yyyy");
			string endDate = today.ToString("MMMM dd, yyyy");

			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				try
				{
					connection.Open();

					// Query to filter records within the current week's range
					string query = $"SELECT * FROM {tableName} WHERE transact_date BETWEEN @startDate AND @endDate";

					using (OleDbCommand command = new OleDbCommand(query, connection))
					{
						// Use parameters for the date range
						command.Parameters.AddWithValue("@startDate", startDate);
						command.Parameters.AddWithValue("@endDate", endDate);

						// Execute the query and load results into a DataTable
						using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
						{
							DataTable dataTable = new DataTable();
							adapter.Fill(dataTable);

							// Bind the filtered data to the DataGridView
							dataGridView1.DataSource = dataTable;
							CalculateTotalAmount();
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error: " + ex.Message);
				}
				finally
				{
					connection.Close();
				}
			}
		}

		private void button8_Click(object sender, EventArgs e)
		{
			// Get the current date and calculate the first and last days of the month
			DateTime today = DateTime.Now;
			DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
			DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1); // Last day of the current month

			string startDate = firstDayOfMonth.ToString("MMMM dd, yyyy");
			string endDate = lastDayOfMonth.ToString("MMMM dd, yyyy");

			using (OleDbConnection connection = new OleDbConnection(connectionString))
			{
				try
				{
					connection.Open();

					// Query to filter records within the current month's range
					string query = $"SELECT * FROM {tableName} WHERE transact_date BETWEEN @startDate AND @endDate";

					using (OleDbCommand command = new OleDbCommand(query, connection))
					{
						// Use parameters for the date range
						command.Parameters.AddWithValue("@startDate", startDate);
						command.Parameters.AddWithValue("@endDate", endDate);

						// Execute the query and load results into a DataTable
						using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
						{
							DataTable dataTable = new DataTable();
							adapter.Fill(dataTable);

							// Bind the filtered data to the DataGridView
							dataGridView1.DataSource = dataTable;
							CalculateTotalAmount();
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show("Error: " + ex.Message);
				}
				finally
				{
					connection.Close();
				}
			}
		}

		private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
		{
			dateTimePicker2.MinDate = dateTimePicker1.Value;
		}

		private void btnPrintPreview_Click(object sender, EventArgs e)
		{
			PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
			PrintDocument printDocument = new PrintDocument();
			printDocument.PrintPage += new PrintPageEventHandler(PrintPage);

			// Set the printer settings if needed
			printPreviewDialog.Document = printDocument;

			// Show the print preview dialog
			printPreviewDialog.ShowDialog();
		}


		private void button12_Click(object sender, EventArgs e)
		{
			// Create a PrintDocument object
			PrintDocument printDoc = new PrintDocument();

			// Set up the event handler for the PrintPage event
			printDoc.PrintPage += new PrintPageEventHandler(PrintPage);

			// Show the PrintDialog to allow the user to select a printer
			PrintDialog printDialog = new PrintDialog();
			printDialog.Document = printDoc;

			if (printDialog.ShowDialog() == DialogResult.OK)
			{
				// Print the document
				printDoc.Print();
			}


		}


        private void FilterDataGridView()
        {
            string searchTerm = searchBox.Text.Trim();

            // Return early if search term is empty
            if (string.IsNullOrEmpty(searchTerm))
            {
                LoadData(); // Load the original data if no search term is entered
                return;
            }

            string query = "SELECT * FROM tbl_transaction_new WHERE cashier_name LIKE @searchTerm OR prod_name LIKE @searchTerm OR transact_date LIKE @searchTerm";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        // Add parameter to prevent SQL injection
                        command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");

                        using (OleDbDataAdapter adapter = new OleDbDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);

                            // Bind the filtered data to the DataGridView
                            dataGridView1.DataSource = dataTable;
                            CalculateTotalAmount(); // Optional: Recalculate total after filtering
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            FilterDataGridView();
        }

        private void BackupRestore_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form10 form10 = new Form10();
            form10.Show();
        }
    }
}
