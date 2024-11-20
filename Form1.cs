using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ขอร้องคุณน้า
{
    public partial class Form1 : Form
    {

        bool goLeft, goRight, jumping, hasKey;

        bool isGameOver = false;

        int jumpSpeed = 10;
        int force = 10;
        int score = 0;

        int hearts = 4; // จำนวนหัวใจเริ่มต้น

        int playerSpeed = 7;
        int backgroundSpeed = 8;


        Timer howTimer = new Timer();


        public Form1()
        {
            InitializeComponent();

            Label makeLabel = new Label();
            makeLabel.Tag = "make";
            makeLabel.BringToFront();

            // นำหัวใจมาอยู่ด้านหน้าสุด
            PictureBox[] heartsArray = { pictureBox96, pictureBox97, pictureBox98, pictureBox99 };
            foreach (var heart in heartsArray)
            {
                heart.Tag = "heart";
                heart.BringToFront();
            }

            // ตั้งค่า Timer
            Timer mainTimer = new Timer();
            mainTimer.Interval = 20;// Interval ระหว่างแต่ละครั้งที่ Timer จะทำงาน (20ms)
            mainTimer.Tick += MainTimerEvent;
            mainTimer.Start();

            // ตั้งค่าการจับคีย์
            this.KeyDown += new KeyEventHandler(KeyIsDown);
            this.KeyUp += new KeyEventHandler(KeyIsUp);          
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            txtScore.Text = "Score: " + score;
            txtScore.Left = 536;
            player.Top += jumpSpeed;


            //ทำให้อยุ่หน้า
           

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "sco")
                {
                    x.BringToFront(); // นำหัวใจมาไว้ด้านหน้าสุด
                }
            }

            foreach (Control control in this.Controls)
            {
                if (control is Label && (string)control.Tag == "sco")
                {
                    control.Left = (this.ClientSize.Width / 2) - (control.Width / 1); // คำนวณให้อยู่ตรงกลาง
                    
                }
            }

            foreach (Control control in this.Controls)
            {
                if (control is Label && (string)control.Tag == "make")
                {
                    control.Left = (this.ClientSize.Width / 2) - (control.Width / 2); // คำนวณให้อยู่ตรงกลาง

                }
            }

           


            if (goLeft == true && player.Left > 60)
            {
                player.Left -= playerSpeed;
            }
            if (goRight == true && player.Left + (player.Width + 60) < this.ClientSize.Width)
            {
                player.Left += playerSpeed;
            }



            if (goLeft == true && background.Left < 0)
            {
                background.Left += backgroundSpeed;
                MoveGameElaments("forward");
            }
            if(goRight == true && background.Left > -1372)
            {
                background.Left -= backgroundSpeed;
                MoveGameElaments("back");
            }

            if (jumping == true)
            {
                jumpSpeed = -10;
                force -= 1;
            }
            else
            {
                jumpSpeed = 10;
            }


            if (jumping == true && force < 0)
            {
                jumping = false;
            }


            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "plaform")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds) && jumping == false)
                    {
                        force = 8;
                        player.Top = x.Top - player.Height;
                        jumpSpeed = 0;
                    }

                    x.BringToFront();

                }



                if (x is PictureBox && (string)x.Tag == "coin")
                {

                    if (player.Bounds.IntersectsWith(x.Bounds) && x.Visible == true)
                    {
                        x.Visible = false;
                        score += 1;
                    }
                }
            }

            if (player.Bounds.IntersectsWith(key.Bounds))
            {
                key.Visible = false;
                hasKey = true;
            }

            if (player.Bounds.IntersectsWith(door.Bounds) && hasKey == true && !isGameOver)
            {
                isGameOver = true; // ตั้งค่าว่าเกมจบแล้ว
                door.Image = Properties.Resources.door_open;
                GameTimer.Stop();
                DialogResult result = MessageBox.Show("Well done, your Journey is complete!" + Environment.NewLine + "Click OK to exit.", "Victory", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    Application.Exit(); // ปิดโปรแกรมเมื่อผู้เล่นชนะ
                }
            }

            if (player.Top + player.Height > this.ClientSize.Height && !isGameOver)
            {
                isGameOver = true; // ตั้งค่าว่าเกมจบแล้ว
                GameTimer.Stop();
                MessageBox.Show("You Died!" + Environment.NewLine + "Click OK to try again.", "Game Over");
                RestartGame(); // รีเซ็ตเกมใหม่
            }

        }


        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Space && jumping == false)
            {
                jumping = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (jumping == true)
            {
                jumping = false;
            }
        }

        private void CloseGame(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }



        private void UpdateHearts()
        {
            int heartsToHide = 4 - hearts;
            int count = 0;

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "heart")
                {
                    if (count < heartsToHide)
                    {
                        x.Visible = false; // ซ่อนหัวใจ
                        count++;
                    }
                    else
                    {
                        x.BringToFront(); // นำหัวใจที่เหลืออยู่มาด้านหน้า
                    }
                }
            }
        }


        private void RestartGame()
        {
            isGameOver = false;
            hasKey = false;
            score = 0;


            // ลดหัวใจเมื่อผู้เล่นตาย
            hearts--;
            UpdateHearts();

            // ปิดเกมทันทีเมื่อหัวใจหมด
            if (hearts <= 0)
            {
                Application.Exit(); // ปิดเกมโดยไม่แสดงข้อความแจ้งเตือน
            }

            player.Left = 0;
            player.Top = 300;
            key.Visible = true;
            key.Left = 400;
            door.Image = Properties.Resources.door_closed;


            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "coin")
                {
                    x.Visible = true;
                }
            }

            GameTimer.Start();
        }

        private void MoveGameElaments(string direction)

        {
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "plaform" || x is PictureBox && (string)x.Tag == "coin" || x is PictureBox && (string)x.Tag == "key" || x is PictureBox && (string)x.Tag == "door")
                {
                    if (direction == "back")
                    {
                        x.Left -= backgroundSpeed;
                    }
                    if (direction == "forward")
                    {
                        x.Left += backgroundSpeed;
                    }
                }
            }
        }
    }
}
