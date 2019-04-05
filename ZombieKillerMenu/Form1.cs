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
using Newtonsoft.Json;



namespace ZombieKillerMenu
{
    public partial class usrForm : Form
    {
        Menu FrmMenu = new Menu();
        static Dictionary<string, string> userPasswordDict = new Dictionary<string, string>();
        static Dictionary<string, Records> userRecordsDict = new Dictionary<string, Records>();
        String UserFilename;

        public int checkForbtnclick = 0;

        public usrForm()
        {
            InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            String Path = @"C:\ZombieData\"; // Here all data for the game exist
            // If this Path does not exist, then create it
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }

            UserFilename = Path + "user_login.txt";// Read Login data for the user

            // If the login file does not exist, also means there is no user, then create
            if (!File.Exists(UserFilename))
            {
                FileStream f = File.Create(UserFilename);
                f.Close();
                lblErrorMessage.Text = "You have no user please fill in the sign up"; // this will automatically happen the first time
            }

            // Deserialize prevous login data with JSON
            else
            {
                string json = File.ReadAllText(UserFilename);
                userPasswordDict = JsonConvert.DeserializeObject<Dictionary<String, String>>(json);
            }

            // Read the users saved data (Records)
            String UserRecordFilename = Path + "user_records.txt";
            // if this file does not exist
            if (!File.Exists(UserRecordFilename))
            {
                FileStream f = File.Create(UserRecordFilename);
                f.Close();
            }
            // Deserialize prevous user data with JSON
            else
            {
                string json = File.ReadAllText(UserRecordFilename);
                userRecordsDict = JsonConvert.DeserializeObject<Dictionary<string, Records>>(json);
            }


        }


        private void btnSignUp_Click(object sender, EventArgs e)
        {
            // Login data
            string username = createUsrName.Text;
            string password = createUsrPassword.Text;

            // In-game data (Player records)
            string gamename = gameNametxt.Text;
            int highscore = 0;
            int survivaltime = 0;

            // A user with a password is created with specifik attributes, these attributes are visible for other players (Record)
            Records userRecords = new Records(gamename, highscore, survivaltime);
            userPasswordDict.Add(username, password);
            userRecordsDict.Add(username, userRecords);
            string errorMessage = "";

            // If error message is none then there are no issues what-so-ever..., therefore, create user!
            if (errorMessage == "") 
            {
                this.Hide();
                string jsonUserPasswd = JsonConvert.SerializeObject(userPasswordDict, Formatting.Indented); // Create persitant storing with JSON
                File.WriteAllText(UserFilename, jsonUserPasswd); // Store the JSON on the file created in the "setup" function
            }
        }




        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private string Login(string username, string password)
        {
            string registredPassword = "";
            // Given the key "user" get value(registred Password) for that user.
            // If the Key does not exist in Dictionary then false will be returned 
            bool userExist = userPasswordDict.TryGetValue(username, out registredPassword);//?
            if (userExist)
            {
                if (password.CompareTo(registredPassword) == 0)
                {
                    // Logged in
                    lblErrorMessage.Text = "";
                }
                // Therefore, if user does not exit
                else
                {
                    // Not logged in
                    lblErrorMessage.Text = "Invalid username or password or does not exist";
                }
            }

            else
            {
                lblErrorMessage.Text = string.Format("The user {0} is not found", username);
            }
            return lblErrorMessage.Text;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string errorMessage = Login(txtUsername.Text, txtPassword.Text);
            lblErrorMessage.Text = errorMessage; 

            if (errorMessage == "") //error message is none
            {
                this.Hide();
                if (FrmMenu.ShowDialog() == DialogResult.OK)
                {
                    // Clear the textboxes (Looks and feels good) 
                    lblErrorMessage.Text = "";
                    txtPassword.Text = "";
                    this.Show();
                }
            }

        }
    }
}