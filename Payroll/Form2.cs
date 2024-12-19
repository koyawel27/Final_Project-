using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCrypt.Net;

namespace Payroll
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void btnSendOTP_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();

            // Validate username and email
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Please enter both username and email.", "Error");
                return;
            }

            // Check the last OTP request time
            string query = "SELECT LastOTPRequest FROM tbl_manage_users WHERE Username = @Username AND Email = @Email";
            using (OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;"))
            {
                connection.Open();
                OleDbCommand cmd = new OleDbCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Email", email);

                OleDbDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Check if LastOTPRequest is DBNull
                    DateTime lastRequest = reader["LastOTPRequest"] == DBNull.Value
                        ? DateTime.MinValue // Default value if no OTP has been requested before
                        : Convert.ToDateTime(reader["LastOTPRequest"]);

                    if ((DateTime.Now - lastRequest).TotalMinutes < 60) // 60 minutes = 1 hour
                    {
                        MessageBox.Show("You have already requested an OTP within the last hour. Please try again later.", "Error");
                        return; // Exit early if rate limit is exceeded
                    }
                }
            }

            // Proceed with sending OTP
            string otp = GenerateOTP();
            string updateQuery = "UPDATE tbl_manage_users SET OTPCode = ?, OTPTimestamp = ?, LastOTPRequest = ? WHERE Username = ?";

            using (OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;"))
            {
                connection.Open();
                OleDbCommand updateCmd = new OleDbCommand(updateQuery, connection);
                updateCmd.Parameters.Add("?", OleDbType.VarWChar).Value = otp;
                updateCmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now;
                updateCmd.Parameters.Add("?", OleDbType.Date).Value = DateTime.Now; // Set the last request time to now
                updateCmd.Parameters.Add("?", OleDbType.VarWChar).Value = username;

                updateCmd.ExecuteNonQuery();
            }

            // Send the OTP email
            SendOTPEmail(email, otp);
        }

        private string GenerateOTP()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }

        private void SendOTPEmail(string email, string otp)
        {
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("welkoya0@gmail.com", "dxqb qitr wnxc mfae"),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("your-email@example.com");
                mail.To.Add(email);
                mail.Subject = "Your OTP Code";
                mail.Body = $"Your OTP code is: {otp}";

                client.Send(mail);
                MessageBox.Show("OTP has been sent to your email.", "Success");
            }
            catch (SmtpException ex)
            {
                MessageBox.Show($"Error sending OTP: {ex.Message}", "Error");
            }
        }



        private void btnVerifyOTP_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string inputOTP = txtOTP.Text.Trim();

            string query = "SELECT OTPCode, OTPTimestamp FROM tbl_manage_users WHERE Username = @Username";
            using (OleDbConnection connection = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;"))
            {
                connection.Open();
                OleDbCommand cmd = new OleDbCommand(query, connection);
                cmd.Parameters.AddWithValue("@Username", username);

                OleDbDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string storedOTP = reader["OTPCode"].ToString();
                    DateTime otpTimestamp = Convert.ToDateTime(reader["OTPTimestamp"]);

                    // Check if OTP matches and is within validity
                    if (storedOTP == inputOTP && (DateTime.Now - otpTimestamp).TotalMinutes <= 1)
                    {
                        MessageBox.Show("OTP Verified. Redirecting to Reset Password.", "Success");
                        ResetPassword resetForm = new ResetPassword(username); // Pass the username here
                        resetForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Invalid or Expired OTP.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("No OTP found for this user.", "Error");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

          
            
                this.Hide();
                Form1 form1 = new Form1();
                form1.Show();
            
        }
    }
}