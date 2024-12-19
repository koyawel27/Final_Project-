using System;
using System.Data;
using System.Windows.Forms;
using System.Data.OleDb;

namespace Payroll
{
    public partial class Form1 : Form
    {
        // Microsoft Access connection string
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Get the username and password from the textboxes
            string username = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            // Validate the credentials from the database
            if (ValidateUser(username, password, out string userType))
            {
                if (userType == "Admin")
                {
                    // Redirect to the admin form
                    Form4 adminForm = new Form4();
                    adminForm.Show();
                }
                else
                {
                    // Redirect to another form for non-admin users
                    Form3 userForm = new Form3();
                    userForm.Show();
                }
                this.Hide(); // Hide the login form
            }
            else
            {
                // Show an error message if the credentials are incorrect
                MessageBox.Show("Invalid username or password!", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateUser(string username, string password, out string userType)
        {
            userType = null; // Initialize the output parameter

            try
            {
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();

                    // SQL query to check the username and password
                    string query = "SELECT usertype FROM tbl_manage_users WHERE username = ? AND password = ?";

                    using (OleDbCommand command = new OleDbCommand(query, connection))
                    {
                        // Use parameters to prevent SQL injection
                        command.Parameters.AddWithValue("?", username);
                        command.Parameters.AddWithValue("?", password);

                        // Execute the query and get the usertype
                        object result = command.ExecuteScalar();

                        if (result != null)
                        {
                            userType = result.ToString(); // Assign the usertype
                            return true; // Credentials are valid
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                MessageBox.Show($"An error occurred while validating the user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false; // Credentials are invalid
        }
    }
}
