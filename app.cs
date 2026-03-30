using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace AuthSystem
{
    public partial class LoginForm : Form
    {
        // Connection string - replace ... with your database path
        string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=...\\AuthDB.mdf;Integrated Security=True;Connect Timeout=30";
        
        public LoginForm()
        {
            InitializeComponent();
        }
        
        // ============================================================================
        // SIGN UP BUTTON
        // ============================================================================
        private void btnSignUp_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("SignUpUser", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            
            sqlConnection.Open();
            command.ExecuteNonQuery();
            sqlConnection.Close();
            
            MessageBox.Show("Account created!");
            txtUsername.Clear();
            txtPassword.Clear();
        }
        
        // ============================================================================
        // LOG IN BUTTON
        // ============================================================================
        private void btnLogIn_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("LogInUser", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            string username = txtUsername.Text;
            string password = txtPassword.Text;
            
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            
            // Output parameter to check if login was successful
            SqlParameter resultParam = new SqlParameter("@Result", SqlDbType.Int);
            resultParam.Direction = ParameterDirection.Output;
            command.Parameters.Add(resultParam);
            
            sqlConnection.Open();
            command.ExecuteNonQuery();
            int result = (int)resultParam.Value;
            sqlConnection.Close();
            
            if (result > 0) // result is the UserID if login successful
            {
                MessageBox.Show("Login successful!");
                
                // Open main form, pass the UserID
                MainForm mainForm = new MainForm(result, connectionString);
                this.Hide();
                mainForm.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password!");
            }
        }
    }
    
    // ============================================================================
    // MAIN FORM (After Login)
    // ============================================================================
    public partial class MainForm : Form
    {
        private int currentUserID;
        private string connectionString;
        
        public MainForm(int userID, string connString)
        {
            InitializeComponent();
            currentUserID = userID;
            connectionString = connString;
            LoadSettings();
        }
        
        // ------------------------------------------------------------------------
        // LOAD SETTINGS WHEN FORM OPENS
        // ------------------------------------------------------------------------
        private void LoadSettings()
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("GetUserSettings", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            command.Parameters.AddWithValue("@UserID", currentUserID);
            
            sqlConnection.Open();
            SqlDataReader reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                string settingName = reader["SettingName"].ToString();
                bool settingValue = (bool)reader["SettingValue"];
                
                // Apply settings to checkboxes
                if (settingName == "DarkMode")
                    chkDarkMode.Checked = settingValue;
                else if (settingName == "Notifications")
                    chkNotifications.Checked = settingValue;
                else if (settingName == "AutoSave")
                    chkAutoSave.Checked = settingValue;
            }
            
            reader.Close();
            sqlConnection.Close();
        }
        
        // ------------------------------------------------------------------------
        // SAVE SETTING (Called when checkbox changes)
        // ------------------------------------------------------------------------
        private void SaveSetting(string settingName, bool settingValue)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("SaveUserSetting", sqlConnection);
            command.CommandType = CommandType.StoredProcedure;
            
            command.Parameters.AddWithValue("@UserID", currentUserID);
            command.Parameters.AddWithValue("@SettingName", settingName);
            command.Parameters.AddWithValue("@SettingValue", settingValue);
            
            sqlConnection.Open();
            command.ExecuteNonQuery();
            sqlConnection.Close();
        }
        
        // ------------------------------------------------------------------------
        // CHECKBOX EVENTS
        // ------------------------------------------------------------------------
        private void chkDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            SaveSetting("DarkMode", chkDarkMode.Checked);
        }
        
        private void chkNotifications_CheckedChanged(object sender, EventArgs e)
        {
            SaveSetting("Notifications", chkNotifications.Checked);
        }
        
        private void chkAutoSave_CheckedChanged(object sender, EventArgs e)
        {
            SaveSetting("AutoSave", chkAutoSave.Checked);
        }
        
        // ------------------------------------------------------------------------
        // SIGN OUT BUTTON
        // ------------------------------------------------------------------------
        private void btnSignOut_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Signed out!");
            
            LoginForm loginForm = new LoginForm();
            this.Hide();
            loginForm.ShowDialog();
            this.Close();
        }
    }
}
