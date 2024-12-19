using System;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Drawing;
using Org.BouncyCastle.Asn1.X509;
using System.Text;

namespace Payroll
{
    public partial class Form1 : Form
    {
        OleDbConnection conn;
        OleDbCommand cmd;
        OleDbDataAdapter adapter;
        private int loginAttempts = 0;
        private Timer sessionTimer;
        private const int SESSION_TIMEOUT_MINUTES = 30;
        public static string loggedInUser;
        public Form1()
        {
            InitializeComponent();
            SetupControls();
            // Clear fields and set focus to the username input when the form is loaded
            tbUsername.Clear();   // Clears the username field
            tbPassword.Clear();   // Clears the password field
            tbUsername.Focus();   // Sets the focus to the username field
        }

        private bool VerifyPassword(string inputPassword, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(inputPassword, hashedPassword);
            }
            catch
            {
                // If the stored hash isn't in BCrypt format, return false
                return false;
            }
        }


        private void LogError(string message, Exception ex)
        {
            string logFilePath = @"ErrorLog.txt"; // Update this path if needed
            string logMessage = $"[{DateTime.Now}] {message}\n{ex}\n";
            System.IO.File.AppendAllText(logFilePath, logMessage);
        }
        private void SetupControls()
        {
            // Add KeyPress event for Enter key
            tbPassword.KeyPress += (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    btnLogin.PerformClick();
                    e.Handled = true;
                }
            };



            // Add show/hide password toggle button
            Button togglePassword = new Button
            {
                Size = new Size(25, tbPassword.Height),
                Location = new Point(tbPassword.Right + 5, tbPassword.Top),
                Text = "👁",
                Cursor = Cursors.Hand
            };
            togglePassword.Click += (s, e) => {
                tbPassword.UseSystemPasswordChar = !tbPassword.UseSystemPasswordChar;
            };
            this.Controls.Add(togglePassword);

            // Initialize session timer
            sessionTimer = new Timer
            {
                Interval = SESSION_TIMEOUT_MINUTES * 60 * 1000 // Convert minutes to milliseconds
            };
            sessionTimer.Tick += SessionTimeout;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(tbUsername.Text) || string.IsNullOrWhiteSpace(tbPassword.Text))
            {
                MessageBox.Show("Username and password fields cannot be empty.",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Cursor = Cursors.Default;
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
                return;
            }

            // Show loading indicator
            Cursor = Cursors.WaitCursor;
            btnLogin.Enabled = false;
            btnLogin.Text = "Logging in...";

            conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;");
            string query = "SELECT ID, userType, username, password FROM tbl_manage_users WHERE username = ?";

            cmd = new OleDbCommand(query, conn);
            cmd.Parameters.AddWithValue("?", tbUsername.Text);

            try
            {
                conn.Open();
                OleDbDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string storedPassword = reader["password"].ToString();

                    if (VerifyPassword(tbPassword.Text, storedPassword)) // Removed the semicolon here
                    {
                        // Fetch user details
                        UserSession.UserID = reader["ID"] is DBNull ? 0 : Convert.ToInt32(reader["ID"]);
                        UserSession.FullName = reader["username"].ToString();
                        UserSession.UserType = reader["userType"].ToString();

                        // Set the logged-in username for tracking
                        LoggedInUser.Username = tbUsername.Text; // Store the logged-in username


                        Form nextForm;
                        if (UserSession.UserType == "Admin")
                        {
                            nextForm = new Form4();
                        }
                        else
                        {
                            nextForm = new Form3();
                        }

                        sessionTimer.Start();

                        // Add logout handler
                        nextForm.FormClosing += (s, args) => {
                            if (args.CloseReason == CloseReason.UserClosing)
                            {
                                args.Cancel = true;
                                LogoutUser();
                            }
                        };

                        this.Hide();
                        nextForm.Show();
                    }
                    else
                    {
                        loginAttempts++;
                        int remainingAttempts = 3 - loginAttempts;
                        MessageBox.Show($"Invalid username or password!\nRemaining attempts: {remainingAttempts}",
                            "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (loginAttempts >= 3)
                        {
                            MessageBox.Show("Maximum login attempts reached. Application will now close.",
                                "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            Application.Exit();
                        }
                    }
                }
                else
                {
                    loginAttempts++;
                    int remainingAttempts = 3 - loginAttempts;
                    MessageBox.Show($"Invalid username or password!\nRemaining attempts: {remainingAttempts}",
                        "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (loginAttempts >= 3)
                    {
                        MessageBox.Show("Maximum login attempts reached. Application will now close.",
                            "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Application.Exit();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("An error occurred during login.", ex);
                MessageBox.Show("Error: " + ex.Message, "Database Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
                Cursor = Cursors.Default;
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }



        private void SessionTimeout(object sender, EventArgs e)
        {
            MessageBox.Show("Your session has timed out due to inactivity.",
                "Session Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            LogoutUser();
        }

        public void LogoutUser()
        {
            sessionTimer.Stop();
            UserSession.ClearSession(); // Clear the session data

            foreach (Form form in Application.OpenForms)
            {
                if (form != this)
                {
                    form.Close();
                }
            }
            this.Show();
            tbUsername.Clear();
            tbPassword.Clear();
            tbUsername.Focus();
        }


        // Handle main form closing
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    Application.Exit();
                }
            }
        }

        // Reset session timer on user activity
        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEMOVE = 0x0200;
            const int WM_KEYDOWN = 0x0100;

            if (m.Msg == WM_MOUSEMOVE || m.Msg == WM_KEYDOWN)
            {
                if (sessionTimer.Enabled)
                {
                    sessionTimer.Stop();
                    sessionTimer.Start();
                }
            }
            base.WndProc(ref m);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create an instance of Form2
            Form2 forgotPasswordForm = new Form2();

            // Show Form2
            forgotPasswordForm.Show();

            // Optional: Hide the current form (Form1)
            this.Hide();

            // If you want to close Form1 entirely after opening Form2:
            // this.Close();
        }
    }
}