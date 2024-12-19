using Org.BouncyCastle.Crypto.Generators;
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
using BCrypt.Net;

namespace Payroll
{
    public partial class ResetPassword : Form
    {
        private readonly string username;

        public ResetPassword(string username)
        {
            InitializeComponent();
            this.username = username;
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (!ValidatePasswords())
                return;

            if (UpdatePasswordInDatabase())
            {
                NavigateToLogin();
            }
        }

        private bool ValidatePasswords()
        {
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            if (newPassword.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.", "Error");
                return false;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error");
                return false;
            }

            return true;
        }

        private bool UpdatePasswordInDatabase()
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(txtNewPassword.Text.Trim());
            string updateQuery = "UPDATE tbl_manage_users SET [Password] = ? WHERE [Username] = ?";

            using (OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;"))
            {
                try
                {
                    connection.Open();
                    using (OleDbCommand cmd = new OleDbCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("?", hashedPassword);
                        cmd.Parameters.AddWithValue("?", username);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password Reset Successfully.");
                            return true;
                        }

                        MessageBox.Show("No rows affected. Password reset failed.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return false;
                }
            }
        }

        private void NavigateToLogin()
        {
            Form1 loginForm = new Form1();
            loginForm.Show();
            this.Hide();
        }
    }
}
