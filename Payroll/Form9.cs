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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Payroll
{
    public partial class Form9 : Form
    {
        private string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\inventory_db.accdb;";
        private string tableName = "tbl_manage_users";
        public Form9()
        {
            InitializeComponent();
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            LoadData();
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.CellFormatting += DataGridView1_CellFormatting;
        }

        // New helper method to clear form fields
        private void ClearFormFields()
        {
            comboBox1.Text = "";
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            comboBox2.Text = "";
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Check if the column index corresponds to the password column
            if (e.ColumnIndex == 3 && e.Value != null) // Assuming the first column (index 0) is the password column
            {
                // Replace the value with asterisks
                e.Value = new string('*', e.Value.ToString().Length);
                e.FormattingApplied = true;
            }
        }

        private void LoadData()
        {
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM tbl_manage_users";

                    using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        // Clear existing data source first
                        dataGridView1.DataSource = null;
                        dataGridView1.Rows.Clear();
                        dataGridView1.DataSource = dataTable;

                        // Optional: Hide the ID column
                        //dataGridView1.Columns["id"].Visible = false;
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading data: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AddData();
        }


        private void AddData()
        {
            string usertype = comboBox1.Text;
            string username = textBox2.Text;
            string password = textBox3.Text;
            string status = comboBox2.Text;
            string fullName = textBox4.Text;
            string email = textBox1.Text;
            string phone = textBox5.Text;

            string query = "INSERT INTO tbl_manage_users ([usertype], [username], [password], [Full Name], [email], [phone], [status]) " +
                          "VALUES (@usertype, @username, @password, @fullName, @email, @phone, @status)";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@usertype", usertype);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@fullName", fullName);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@status", status);

                        cmd.ExecuteNonQuery();

                        // Clear the form fields
                        ClearFormFields();

                        // Refresh the DataGridView
                        LoadData();

                        MessageBox.Show("User added successfully!");
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
            UpdateData();
        }
        private void UpdateData()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a row to update.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);
            string usertype = comboBox1.Text;
            string username = textBox2.Text;
            string password = textBox3.Text;
            string status = comboBox2.Text;
            string fullName = textBox4.Text;
            string email = textBox1.Text;
            string phone = textBox5.Text;

            string query = "UPDATE tbl_manage_users SET usertype = @usertype, username = @username, password = @password, " +
                          "[Full Name] = @fullName, email = @email, phone = @phone, status = @status WHERE id = @id";

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    using (OleDbCommand cmd = new OleDbCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@usertype", usertype);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.Parameters.AddWithValue("@fullName", fullName);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@phone", phone);
                        cmd.Parameters.AddWithValue("@id", id);

                        cmd.ExecuteNonQuery();

                        // Clear the form fields
                        ClearFormFields();

                        // Refresh the DataGridView
                        LoadData();

                        MessageBox.Show("User updated successfully!");
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

            comboBox1.Text = row.Cells["usertype"].Value?.ToString();
            textBox2.Text = row.Cells["username"].Value?.ToString();
            textBox3.Text = row.Cells["password"].Value?.ToString();
            comboBox2.Text = row.Cells["status"].Value?.ToString();
            textBox4.Text = row.Cells["Full Name"].Value?.ToString(); // Use the original column name
            textBox1.Text = row.Cells["email"].Value?.ToString();
            textBox5.Text = row.Cells["phone"].Value?.ToString();
        }


        private void button8_Click(object sender, EventArgs e)
        {
            DeleteData();
        }

        private void DeleteData()
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a row to delete.");
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);

            DialogResult result = MessageBox.Show("Are you sure you want to delete this user?",
                                                "Confirm Deletion",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                string query = "DELETE FROM tbl_manage_users WHERE id = @id";

                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    try
                    {
                        connection.Open();

                        using (OleDbCommand cmd = new OleDbCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@id", id);
                            cmd.ExecuteNonQuery();

                            // Clear the form fields
                            ClearFormFields();

                            // Refresh the DataGridView
                            LoadData();

                            MessageBox.Show("User deleted successfully!");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }



        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form9 form9 = new Form9();
            form9.Show();
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
