using MyTool.Properties;
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
using YDMCSDemo;

namespace MyTool.Valid
{
    public partial class ReadPassCode : Form
    {

        public DateTime StartReadTime { get; set; }
        public ReadPassCode()
        {
            InitializeComponent();

            Load += ReadPassCode_Load;

            UIPostion.Current.Ui.Add(this);

            this.Opacity = ReadPassengerPage.Current != null ? ReadPassengerPage.Current.ShowOP : 1;

            if (this.Opacity < 1)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            StartReadTime = DateTime.Now;
        }

        void ReadPassCode_Load(object sender, EventArgs e)
        {
            label1.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (SetPicture(ImageStr))
            {
                this.Location = UIPostion.Current.GetPoint();

                Thread th = new Thread(new ThreadStart(() =>
                {
                    ReadImage();

                    Thread.Sleep(100);

                    var timeTween = DateTime.Now - StartReadTime;

                    if (timeTween.TotalSeconds < 4)
                    {
                        Thread.Sleep(Convert.ToInt32((4 - timeTween.TotalSeconds) * 1000));
                    }

                    this.Invoke(new Action(() =>
                   {
                       label2.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                   }));

                    Thread.Sleep(500);

                    this.Invoke(new Action(() =>
                    {
                        button1_Click(null, null);
                    }));
                }));

                th.Start();
            }
            else 
            {
                Thread th = new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(2000);
                    button1_Click(null, null);
                }));
                th.Start();
            }
        }

        public void ReadImage()
        {
            StringBuilder pCodeResult = new StringBuilder();

            var ss = YDMWrapper.YDM_DecodeByBytes(stream.ToArray(), (int)stream.Length, 6701, pCodeResult);
            if (ss ==-1001)
            {
                Program.Login();
                ss = YDMWrapper.YDM_DecodeByBytes(stream.ToArray(), (int)stream.Length, 6701, pCodeResult);
            }
            Id = ss;

            var ran = new Random();

            var c = pCodeResult.ToString().ToCharArray();

            foreach (var current in c)
            {
                Thread.Sleep(100);

                Point currentPoint = new Point();

                int outValue=-1;

                int.TryParse(current.ToString(), out outValue);

                if (outValue == -1)
                    continue;

                var index = Convert.ToInt32(outValue) - 1;

                var column = index / 4;

                var row = index % 4;

                currentPoint.X = (row) * 73 + 6 + ran.Next(10, 50);

                currentPoint.Y = 41 + column * (190 - 41) / 2 + ran.Next(10, 30); ;

                Invoke(new Action(() =>
                {
                    CreatePicture(currentPoint);
                }));
            }
        }


        public int Id { get; set; }

        Bitmap img = null;

        MemoryStream stream = null;

        public bool SetPicture(string data)
        {
            try
            {
                stream = new MemoryStream(Convert.FromBase64String(data));

                img = new Bitmap(stream);

                this.BackgroundImage = img;
                return true;
            }
            catch
            {
                return false;
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
            var location = new StringBuilder();
            foreach (PictureBox box in clicks)
            {
                int x = box.Location.X;
                int y = box.Location.Y - 30;
                location.Append(x + ",");
                location.Append(y + ",");
            }
            if (location.Length > 0)
            {
                location.Remove(location.Length - 1, 1);
            }
            else
            {
                location.Append("212,222");
            }
            if (ReadCodeCompleted != null)
            {
                ReadCodeCompleted(location.ToString(), new PassCodeEventArgs
                {
                    Result = location.ToString(),
                    ID = Id
                });
            }
            this.Close();
            UIPostion.Current.Ui.Remove(this); 
        }
    }

    public class UIPostion
    {
        private static UIPostion current = null;
        public static UIPostion Current
        {
            get
            {

                if (current == null)
                {
                    current = new UIPostion();
                }
                return current;
            }
        }

        private List<ReadPassCode> ui = new List<ReadPassCode> { };

        public List<ReadPassCode> Ui
        {
            get { return ui; }
        }
        int i = 0;
        public Point GetPoint()
        {
            Point ss = new Point();
            if (ReadPassengerPage.Current != null)
            {
                var task = ReadPassengerPage.Current.Task;// Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["readPassengerNumber"]);

                var row = i / 3;

                var column = i % 3;

                ss.X = column * 400 + Screen.PrimaryScreen.Bounds.Width / 6;

                ss.Y = row * 300 + Screen.PrimaryScreen.Bounds.Height / 4;

                i++;

                if (i >= task)
                {
                    i = 0;
                }
            }
            else {
                ss.X = 400;
                ss.Y = 300;
            }
            return ss;

        }
    }

    public class PassCodeEventArgs : EventArgs
    {

        public string Result { get; set; }
        public int ID { get; set; }
    }

}
