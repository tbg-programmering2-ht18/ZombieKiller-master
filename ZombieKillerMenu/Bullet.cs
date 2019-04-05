using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace ZombieKillerMenu
{
    class Bullet
    {
        //↓↓ Varibledeclaration ↓↓
        public string direction;
        public int speed = 20;
        PictureBox bullet = new PictureBox();
        Timer tm = new Timer();

        public int bulletLeft;
        public int bulletTop;
        //↑↑ Varibledeclaration ↑↑

        public void mkBullet(Form form)
        {
            bullet.BackColor = System.Drawing.Color.White;
            bullet.Size = new Size(5, 5);
            bullet.Tag = "bullet";
            bullet.Top = bulletTop;
            bullet.Left = bulletLeft;
            bullet.BringToFront();
            form.Controls.Add(bullet);

            tm.Interval = speed;
            tm.Tick += new EventHandler(tm_Tick);
            tm.Start();

        }

        public void tm_Tick(object sneder, EventArgs e)
        {
            if (direction == "left")
            {
                bullet.Left -= speed;
            }

            if (direction == "right")
            {
                bullet.Left += speed;
            }

            if (direction == "up")
            {
                bullet.Top -= speed;
            }

            if (direction == "down")
            {
                bullet.Top += speed;
            }

            if (bullet.Left < 16 || bullet.Left > 860 || bullet.Top < 10 || bullet.Top > 616)
            {
                tm.Stop();
                tm.Dispose();
                bullet.Dispose();
                tm = null;
                bullet = null;
            }

        }
    }
}
