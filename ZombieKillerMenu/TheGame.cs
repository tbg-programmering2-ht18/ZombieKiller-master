using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ZombieKillerMenu
{

    public partial class TheGame : Form
    {
        //↓↓ Variabledeclarator ↓↓
        bool goUp; // Used for the player to go up the screen
        bool goDown; // Used for the player to go down the screen
        bool goLeft; // Used for the player to go left on the screen
        bool goRight; // Used for the player to go right on the screen
        string facing = "up"; // Used for directin bullets
        double playerHealth = 100; // Represents the players health in the game
        int speed = 10; // Represents the players moving speed in the game
        int ammo = 10; // Represents the players amount of bullets at the start of the game
        int zombieSpeed = 1; // Represents the moving speed of zombies in the game
        int score = 0; // Represents the players achived score troughout the game
        bool gameOver = false; // Is game finsihed or not
        Random rnd = new Random();

        private TcpClient client; // Provides client connection for TCP network service.
                                  // Network service = application that is running on the network application layer.
        public StreamWriter STW;
        public StreamReader STR;
        public string recieveText;
        public string sendText;

        public Records GameName;
        //↑↑ Variabledeclarator ↑↑

        public TheGame()
        {
            InitializeComponent();

        }

        //↓↓ Function 1: A key is being pressed down ↓↓
        private void keyisdown(object sender, KeyEventArgs e)
        {
            timer1.Enabled = true;

            if (gameOver == true) return; // None of the below are nessicary if the game has ended already

            // If left key is pressed,   
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
                facing = "left";
                Player.Image = Properties.Resources.left;
            }

            // If right key is pressed,
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                Player.Image = Properties.Resources.right;
            }

            // If the up key is pressed,
            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                Player.Image = Properties.Resources.up;
            }
            // If the down key is pressed,
            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                Player.Image = Properties.Resources.down;
            }
        }
        //↑↑ Function 1: A key is being pressed down ↑↑


        //↓↓ Function 2: A key is being relesed ↓↓
        private void keyisup(object sender, KeyEventArgs e)
        {
            if (gameOver == true) return; // None of the below are needed if the game has ended already

            // Left key is released
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            // Right key is released
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            // Up key is released
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            // Down key is released
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

            // Space key is released
            if (e.KeyCode == Keys.Space && ammo > 0) // In other words, when the trigger has been pulled and we still have ammo 
            {
                ammo--; // Reduce ammo when fireing
                Shoot(facing); // Invoke the shooting funktion

                if (ammo < 1) // If we are out of ammo (less then 1)
                {
                    DropAmmo(); // Invoke the drop ammo function
                }

            }
        }
        //↑↑ Function 2: A key is being relesed ↑↑


        //↓↓ Function 3: Engine that handles movement, interactions and it runs on a timer ↓↓
        private void gameEngine(object sender, EventArgs e)
        {
            // Player is still alive
            if (playerHealth > 1) 
            {
                progressBar1.Value = Convert.ToInt32(playerHealth); // Assign the progress bar to players health integer 
                
            }

            // Player is dead (PlayerHealth < 1)
            else
            {
                sendText = "You have won the game";
                Player.Image = Properties.Resources.dead;
                timer1.Stop();
                gameOver = true;
            }

            label1.Text = "Ammo: " + ammo;
            label2.Text = "Kills: " + score;

            
            if (playerHealth < 20)
            {
                progressBar1.ForeColor = System.Drawing.Color.Red; // Player HP bar becomes red
            }

            if (goLeft && Player.Left > 0)
            {
                Player.Left -= speed;
            }

            if (goRight && Player.Left + Player.Width < 930)
            {
                Player.Left += speed;
            }

            if (goUp && Player.Top > 60)
            {
                Player.Top -= speed;
            }

            if (goDown && Player.Top + Player.Height < 700)
            {
                Player.Top += speed;
            }

            // x is a Control and we will search for all controls in this loop
            foreach (Control x in this.Controls)
            {   
                // x is a picture and has the tag ammo
                if (x is PictureBox && x.Tag == "ammo")
                {
                    // Check if x is hitting the player picture box
                    // In other words, player is picking up ammo
                    if (((PictureBox)x).Bounds.IntersectsWith(Player.Bounds))
                    {
                        // Add the ammo and remove the in-game ammo picture  
                        this.Controls.Remove(((PictureBox)x)); 
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                // x is a picture and has the tag of bullet
                if (x is PictureBox && x.Tag == "bullet")
                {
                    if (((PictureBox)x).Left < 1 || ((PictureBox)x).Left > 930 || ((PictureBox)x).Top < 10 || ((PictureBox)x).Top > 700)
                    {
                        this.Controls.Remove(((PictureBox)x)); // Remove the bullet from the display
                        ((PictureBox)x).Dispose(); // Dispose the bullet from the program.
                    }
                }

                // If the player gets hit by a zombie
                if (x is PictureBox && x.Tag == "zombie")
                {
                    // If the bounds of the player intersects with the bounds of the zombie
                    if (((PictureBox)x).Bounds.IntersectsWith(Player.Bounds))
                    {
                        playerHealth -= 1;
                    }

                    // Move zombie towards player picture box
                    if (((PictureBox)x).Left > Player.Left)
                    {
                        ((PictureBox)x).Left -= zombieSpeed; // Move zombie towards the left of the player
                        ((PictureBox)x).Image = Properties.Resources.zleft; // Change the zombie picture
                    }

                    if (((PictureBox)x).Top > Player.Top)
                    {
                        ((PictureBox)x).Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                    }

                    if (((PictureBox)x).Left < Player.Left)
                    {
                        ((PictureBox)x).Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                    }

                    if (((PictureBox)x).Top < Player.Top)
                    {
                        ((PictureBox)x).Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown; 
                    }

                }

                foreach(Control j in this.Controls)
                // Below is the section that identifies the bullet and the zombie
                {
                    if ((j is PictureBox && j.Tag == "bullet") && (x is PictureBox && x.Tag == "zombie"))
                    {
                        // If bullet hits the zombie 
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;
                            this.Controls.Remove(j);
                            j.Dispose();
                            this.Controls.Remove(x);
                            x.Dispose(); // KILL IT!!!, in other words, remove this zombie figure
                            MakeZombies();

                            if (score == 5) // After you have killed five zombies
                            {
                                sendText = " Your opponent has killed " + score + " zombies";
                            }
                        }
                    }
                }
            }
        }
        //↑↑ Function 3: Engine that handles movement, interactions and it runs a timer ↑↑


        //↓↓ Function 4: When the player is out of ammontion ↓↓
        private void DropAmmo() // This funktion is being called from Function 2 "KeyIsUp" 
        {
            PictureBox ammo = new PictureBox(); // New instance for the picture box 
            ammo.Image = Properties.Resources.ammo_Image; // Assign the ammo image to the Picture box
            ammo.SizeMode = PictureBoxSizeMode.AutoSize; // Set the size to autosize
            ammo.Left = rnd.Next(10, 890); // Set the location to random left
            ammo.Top = rnd.Next(50, 600); // Set the location to random top
            ammo.Tag = "ammo";
            this.Controls.Add(ammo); // Add the ammo picture to the screen
            ammo.BringToFront(); // Bring it to front
            Player.BringToFront(); // Bring the player to front
            redtHistory.AppendText("Dropped Ammo" + "\n"); 
        }
        //↑↑ Function 4: When the player is out of ammontion ↑↑


        //↓↓ Function 5: When the player is out of ammontion ↓↓
        private void Shoot(string direct) // This function is being called from Function 2 "KeyIsUp       
        {                                 //Paramenter "direct" is the facing varible, it comes from Function 1 "KeyIsDown"          
            Bullet shoot = new Bullet();// New instance for the picture box
            shoot.direction = direct; 
            shoot.bulletLeft = Player.Left + (Player.Width / 2);
            shoot.bulletTop = Player.Top + (Player.Height / 2);
            shoot.mkBullet(this); // Run the mkBullet (Make bullets) from the bullet class;
        }
        //↑↑ Function 5: When the player is out of ammontion ↑↑



        //↓↓ Function 6: When zombie has been killed, generate a new one ↓↓
        private void MakeZombies() // This function is being called from 
        {
            PictureBox zombie = new PictureBox();
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown; // Default picture for the zombies is down
            zombie.Left = rnd.Next(0, 900);
            zombie.Top = rnd.Next(0, 800);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize;
            this.Controls.Add(zombie);
            Player.BringToFront();
        }
        //↑↑ Function 6: When zombie has been killed, generate a new one ↑↑


        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieveText = STR.ReadLine();
                    this.redtHistory.Invoke(new MethodInvoker(delegate ()
                    {
                        redtHistory.AppendText("You:" + recieveText + "\n");
                    }));
                    recieveText = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(sendText);
                this.redtHistory.Invoke(new MethodInvoker(delegate ()
                {
                    redtHistory.AppendText("Me:" + sendText + "\n");
                }));
            }
            else
            {
                MessageBox.Show("Sending failed");
            }
            backgroundWorker2.CancelAsync();

        }

        private void TheGame_Load(object sender, EventArgs e)
        {

        }

        private void TheGame_Enter(object sender, EventArgs e)
        {
        }
    }
}
