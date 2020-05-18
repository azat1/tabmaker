using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace FastImageTabMaker
{
    public partial class Form1 : Form
    {
        double zoom = 1;
        int pointcounter = 0;
        PointWList plist = new PointWList();
        BindingSource bs = new BindingSource();
        CoordConverter cnv;// = new CoordConverter(plist);
        string filename;
        int navx, navy, navw, navh;
        public Form1()
        {
            InitializeComponent();
          //  plist.Add(new PointW(0, 0, 0, 0, 0));
            ;
            bs.DataSource = plist;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = bs;
            cnv = new CoordConverter(plist);
            pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
        }

        void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            //throw new NotImplementedException();
            if (Control.ModifierKeys == Keys.Alt)
            {
               // splitContainer1.Panel1.HorizontalScroll.Value = splitContainer1.Panel1.HorizontalScroll.Value + 5;
            } else
                if (Control.ModifierKeys == Keys.Shift)
                {
                   // splitContainer1.Panel1.VerticalScroll.Value = splitContainer1.Panel1.VerticalScroll.Value + 5;  
                }
                else
                {
                    
//                    if (e.Delta > 0) zoom = zoom / 2;
//                    if (e.Delta < 0) zoom = zoom * 2;
//                    UpdateSize();
                }
            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                pointcounter = 0;
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
                filename = openFileDialog1.FileName;
                zoom = 1;
                pictureBox1.Width = pictureBox1.Image.Width;
                pictureBox1.Height = pictureBox1.Image.Height;
                DeletePoints();
                LoadNavigator();
            }
        }

        private void LoadNavigator()
        {
            pbNavigator.Image=pictureBox1.Image;
        }

        private void DeletePoints()
        {
            plist.Clear();
            bs.DataSource = null;
            bs.DataSource = plist;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            
            zoom = zoom * 2;
            UpdateSize();
        }

        private void UpdateSize()
        {
            pictureBox1.Width = (int) (pictureBox1.Image.Width*zoom);
            pictureBox1.Height = (int) (pictureBox1.Image.Height*zoom);
            splitContainer1_Panel1_Scroll(null, null);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            zoom = zoom / 2;
            UpdateSize();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            CalcCoords(e.X, e.Y);
        }

        private void CalcCoords(int x, int y)
        {
            int px=(int)(x/zoom);
            int py =(int) (y / zoom);
            toolStripTextBox1.Text = string.Format("{0} {1}", px, py);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            MakePoint(e.X, e.Y);
        }

        private void MakePoint(int x, int y)
        {
            int px = (int)(x / zoom);
            int py = (int)(y / zoom);
            AddPoint(px, py);
            CalcCoords();
            //int sz=30;
            //Graphics gr = Graphics.FromImage(pictureBox1.Image);
            //gr.DrawEllipse(new Pen(Color.Red),new Rectangle(px-sz,py-sz,sz*2,sz*2));
            //gr.Dispose();
            //pictureBox1.Refresh();
        }

        private void CalcCoords()
        {
            cnv.Calculate();
            textBox1.Text = string.Format("a1={0} b1={1} c1={2}\n a2={3} b2={4} c2={5}",
                cnv.a1,cnv.b1,cnv.c1,
                cnv.a2, cnv.b2, cnv.c2);
        }

        private void AddPoint(int px, int py)
        {
            double xx = 0, yy = 0;
            if (plist.Count == 2)
            {
                Calc2Point(px,py,out xx, out yy);
            }
            plist.Add(new PointW(++pointcounter,px,py,xx,yy));
           // dataGridView1.DataSource=null;
           // dataGridView1.DataSource = plist;
            dataGridView1.EndEdit();
            bs.DataSource = null;
            bs.DataSource = plist;
          //  dataGridView1.Refresh();
           // pointWListBindingSource.ResetBindings(true);

            DrawPoints(Graphics.FromHwnd(pictureBox1.Handle));
        }

        private void Calc2Point(int px,int py,out double xx, out double yy)
        {
            PointW p1=plist[0];
            PointW p2 = plist[1];
            int vx=p2.x-p1.x;
            int vy=p2.y-p1.y;
            double wx=p2.wx-p1.wx;
            double wy=p2.wy-p1.wy;
            PointW p3=new PointW(-1,p1.x-vy,p1.y+vx,p1.wx-wy,p1.wy+wx);
            PointWList plw=new PointWList();
            plw.Add(p1);
            plw.Add(p2);
            plw.Add(p3);
           
            CoordConverter cnv = new CoordConverter(plw);
            cnv.Calculate();
            cnv.ConvertPointD2W(px, py, out xx, out yy);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawPoints(e.Graphics);
        }

        private void DrawPoints(Graphics gr)
        {
            //Graphics gr=splitContainer1.Panel1.
            int sz=5;
            foreach (PointW p in plist)
            {

                int x = (int)(p.x * zoom);
                int y = (int)(p.y * zoom);
                //Graphics gr = Graphics.FromImage(pictureBox1.Image);
                gr.DrawEllipse(new Pen(Color.Red),new Rectangle(x-sz,y-sz,sz*2,sz*2));
                gr.DrawLine(new Pen(Color.Red), x - sz, y, x + sz, y);
                gr.DrawLine(new Pen(Color.Red), x, y-sz, x, y+sz);
                gr.DrawString(p.number.ToString(), new Font("Serif", 10), new SolidBrush(Color.Red), x + sz, y);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
        }

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CalcCoords();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (plist.Count < 2)
            {
                MessageBox.Show("Недостаточно точек!");
                return;
            }
            if (cnv.NoCalc())
            {
                MessageBox.Show("Неверный ввод чисел!");
                return;
            }
            SaveTab();
        }

        private void SaveTab()
        {
            string fname = filename.Substring(0, filename.LastIndexOf("."))+".tab";
            string rname=Path.GetFileName(filename);
            List<PointW> pwl = new List<PointW>();
            pwl.Add(MakeConvertedPoint(1,0, 0));
            pwl.Add(MakeConvertedPoint(2,pictureBox1.Image.Width, 0));
            pwl.Add(MakeConvertedPoint(3,pictureBox1.Image.Width, pictureBox1.Image.Height));
            if (File.Exists(fname))
            {
                if (MessageBox.Show("Уже имеется файл привязки! Если продолжить он будет стерт! Продолжить?", "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            }
            StreamWriter txt = new StreamWriter(fname, false, Encoding.Default);
            txt.WriteLine("!table");
            txt.WriteLine("!version 300");
            txt.WriteLine("!charset WindowsCyrillic");
            txt.WriteLine("");
            txt.WriteLine("Definition table");

            txt.WriteLine("File \""+rname+"\"");
            txt.WriteLine("Type \"RASTER\"");
            foreach (PointW p in pwl)
            {
                txt.WriteLine(string.Format(CultureInfo.InvariantCulture, "({0},{1}) ({2},{3}) Label \"Point {4}\" ",
                    p.wy, p.wx, p.x, p.y,p.number));//(2337518.350,556192.550) (8859,10428) Label "Point 1",
            }
            txt.WriteLine("CoordSys NonEarth Units \"m\"");
            txt.WriteLine("Units \"m\"");
            txt.Close();
            MessageBox.Show("Успешно создан файл привязки!");
        }

        private PointW MakeConvertedPoint(int n,int x, int y)
        {
            double wx = x * cnv.a1 + y * cnv.b1 + cnv.c1 ;
            double wy = x * cnv.a2 + y * cnv.b2 + cnv.c2;
            return new PointW(n, x, y, wx, wy);
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            ;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            //pictureBox1.Focus();
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            Point p1=splitContainer1.Panel1.AutoScrollPosition;
            UpdateNavigator(p1, splitContainer1.Panel1.Width, splitContainer1.Panel1.Height,pictureBox1.Width,pictureBox1.Height);
        }

        private void UpdateNavigator(Point scrlpoint, int pw, int ph,int picw,int pich)
        {
            //scale to small panel
            
            float sclw=(float) picw/panel1.Width;
            float sclh=(float) pich/panel1.Height;

            navx=(int)( (-scrlpoint.X)/sclw);
            navy =(int)( (-scrlpoint.Y) / sclh);
            navw = (int)(pw / sclw);
            navh = (int)(ph / sclh);
            panel1.Refresh();

        }

        private void pbNavigator_Paint(object sender, PaintEventArgs e)
        {
            DrawNavigator(e.Graphics);
        }

        private void DrawNavigator(Graphics gr)
        {
            gr.DrawRectangle(new Pen(Color.Red),new Rectangle(navx,navy,navw,navh));
        }

        private void splitContainer1_Panel1_Scroll(object sender, ScrollEventArgs e)
        {
            Point p1 = splitContainer1.Panel1.AutoScrollPosition;
            UpdateNavigator(p1, splitContainer1.Panel1.Width, splitContainer1.Panel1.Height, pictureBox1.Width, pictureBox1.Height);
        }

        private void pbNavigator_MouseClick(object sender, MouseEventArgs e)
        {
            NavigateTo(e.X,e.Y);
        }

        private void NavigateTo(int mx, int my)
        {
            float sclw = (float)pictureBox1.Width / panel1.Width;
            float sclh = (float)pictureBox1.Height / panel1.Height;
            int nx = (int) (mx * sclw)-splitContainer1.Panel1.Width/2; //new scroll pos
            int ny = (int)(my * sclh)-splitContainer1.Panel1.Height/2;
            if (nx < 0) nx = 0;
            if (ny < 0) ny = 0;
            //calc scroll pos
            splitContainer1.Panel1.AutoScrollPosition = new Point(nx, ny);
            splitContainer1_Panel1_Scroll(null, null);
        }
    }
}
