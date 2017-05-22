using ChangePassWord.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MyTool.Valid
{
    public partial class ReadPassCode : Form
    {
        public ReadPassCode()
        {
            InitializeComponent();

            Load += ReadPassCode_Load;
        }

        void ReadPassCode_Load(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            SetPicture(ImageStr);

            //Thread th = new Thread(new ThreadStart(() =>
            //{
            //    ReadImage();

            //    Thread.Sleep(100);
                
            //    this.Invoke(new Action(() =>
            //    {
            //        button1_Click(null, null);
            //    }));
            //}));

            //th.Start();
        }

        //public void ReadImage()
        //{
        //    StringBuilder pCodeResult = new StringBuilder();

        //    Id = ss;

        //    var ran = new Random();

        //    var c = pCodeResult.ToString().ToCharArray();

        //    foreach (var current in c)
        //    {
        //        Thread.Sleep(100);

        //        Point currentPoint = new Point();

        //        int outValue=-1;

        //        int.TryParse(current.ToString(), out outValue);

        //        if (outValue == -1)
        //            continue;

        //        var index = Convert.ToInt32(outValue) - 1;

        //        var column = index / 4;

        //        var row = index % 4;

        //        currentPoint.X = (row) * 73 + 6 + ran.Next(10, 50);

        //        currentPoint.Y = 41 + column * (190 - 41) / 2 + ran.Next(10, 30); ;

        //        Invoke(new Action(() =>
        //        {
        //            CreatePicture(currentPoint);
        //        }));
        //    }
        //}


        public int Id { get; set; }

        Bitmap img = null;

        MemoryStream stream = null;

        public void SetPicture(string data)
        {
            try
            {
                stream = new MemoryStream(Convert.FromBase64String(data));

                img = new Bitmap(stream);

                this.BackgroundImage = img;
            }
            catch
            {
            }
        }
        List<Control> clicks = new List<Control>();

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.X > img.Width)
                return;

            if (e.Y > img.Height)
                return;

            CreatePicture(new Point(e.X, e.Y));
        }

        private void CreatePicture(Point e)
        {
            PictureBox box = new PictureBox();
            box.Image = Resources.hand1;
            box.Cursor = Cursors.Hand;
            box.Name = "p" + System.Environment.TickCount;
            box.Location = new Point(e.X, e.Y);
            box.SizeMode = PictureBoxSizeMode.AutoSize;
            box.Click += box_Click;
            clicks.Add(box);
            this.Controls.Add(box);
        }
        void box_Click(object sender, EventArgs e)
        {
            var box = sender as PictureBox;
            this.Controls.Remove(box);
            clicks.Remove(box);
        }

        public string ImageStr { get; set; }

        public event EventHandler<PassCodeEventArgs> ReadCodeCompleted;
        private void button1_Click(object sender, EventArgs e)
        {
            if (clicks.Count <= 0)
            {
                MessageBox.Show("识别验证码");
                return;
            }
            var location = new StringBuilder();
            foreach (PictureBox box in clicks)
            {
                int x = box.Location.X;
                int y = box.Location.Y - 30;
                location.Append(x + ",");
                location.Append(y + ",");
            }
            location.Remove(location.Length - 1, 1);
            if (ReadCodeCompleted != null)
            {
                ReadCodeCompleted(location.ToString(), new PassCodeEventArgs
                {
                    Result = location.ToString(),
                    ID = Id
                });
            }
            this.Close();
        }
    }

 

    public class PassCodeEventArgs : EventArgs
    {

        public string Result { get; set; }
        public int ID { get; set; }
    }

}
