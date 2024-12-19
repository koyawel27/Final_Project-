using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Payroll
{
    public partial class Form10 : Form
    {
        public Form10()
        {
            InitializeComponent();
        }


        private void Form10_Load(object sender, EventArgs e)
        {
            txtBackupFolder.Text = @"C:\Users\HP\Downloads\Nep\Payroll-20241207T131225Z-001\Payroll\Payroll\bin\Debug";
            
        }
        private void SearchDirectory(string directory)
        {
            try
            {
                string[] files = Directory.GetFiles(directory, "*.accdb");

                foreach (string file in files)
                {
                    string fileName = Path.GetFileName(file);
                    lstBackupFiles.Items.Add(fileName);
                }

                // Recursively search subdirectories
                string[] subDirectories = Directory.GetDirectories(directory);
                foreach (var subDirectory in subDirectories)
                {
                    SearchDirectory(subDirectory);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip directories that cannot be accessed
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }
        private void CreateBackup()
        {
            try
            {
                // Use Application.StartupPath to get the database in the bin\Debug folder
                string sourcePath = Path.Combine(Application.StartupPath, "inventory_db.accdb"); // Database file in bin\Debug

                // Open a folder dialog to select the backup destination
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string backupFolder = folderDialog.SelectedPath;  // Get the selected folder for backup

                    // Generate unique file name with timestamp
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    string backupFileName = $"backup_{timestamp}.accdb";
                    string destinationPath = Path.Combine(backupFolder, backupFileName);

                    // Copy database file to the selected backup folder
                    File.Copy(sourcePath, destinationPath, true);

                    // Add to listbox and notify user
                    lstBackupFiles.Items.Add(backupFileName);
                    MessageBox.Show("Backup created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnCreateBackup_Click(object sender, EventArgs e)
        {
            CreateBackup();
        }

        private void RestoreBackup()
        {
            try
            {
                // Let the user select a backup file
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Access Database Files (*.accdb)|*.accdb";
                openFileDialog.Title = "Select Backup File";

                // Show dialog to select backup file
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string backupPath = openFileDialog.FileName;

                    // Specify the full path to the database file, not just the directory
                    string currentDatabase = Path.Combine(Application.StartupPath, "inventory_db.accdb"); // Use the actual database file name

                    // Confirm restore
                    if (MessageBox.Show("This will overwrite your current database. Proceed?",
                                        "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        // Replace the current database file with the selected backup file
                        File.Copy(backupPath, currentDatabase, true);
                        MessageBox.Show("Database restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring backup: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnRestoreBackup_Click(object sender, EventArgs e)
        {
            RestoreBackup();
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            // Let the user select a folder to search
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFolder = folderDialog.SelectedPath;
                lstBackupFiles.Items.Clear();  // Clear existing items in ListBox

                try
                {
                    // Search for .accdb files in the selected folder and subfolders
                    string[] backupFiles = Directory.GetFiles(selectedFolder, "*.accdb", SearchOption.AllDirectories);

                    foreach (string backupFile in backupFiles)
                    {
                        string fileName = Path.GetFileName(backupFile);  // Get only the file name
                        lstBackupFiles.Items.Add(fileName);  // Add file name to ListBox
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Access to a folder was denied: {ex.Message}", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form7 form7 = new Form7();
            form7.Show(); // Show Form3
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Show(); // Show Form3
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form9 form9 = new Form9();
            form9.Show();
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Show(); // Show Form3
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form7 form7 = new Form7();
            form7.Show(); // Show Form3
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form9 form9 = new Form9();
            form9.Show();
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to logout?", "CONFIRMATION", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                this.Hide();
                Form1 form1 = new Form1();
                form1.Show();
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            Form8 form8 = new Form8();
            form8.Show();
        }

        private void txtBackupFolder_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
