using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DrapPanel
{
    public partial class Form1 : Form
    {
        //将被拖动的控件
        private GroupBox grp;
        private ListView lv;
        public int i = 0;
        public Dictionary<string, GroupBox> grpDict = new Dictionary<string, GroupBox>();
        public Form1()
        {
            InitializeComponent();
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormDrag_Paint);
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
            grp = new GroupBox();
            createGrp(grp);
            this.Controls.Add(grp);
       
           
            
        }

        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float Mo = 0;
            if (e.Delta > 0)
            {
                Mo = 8;
            }
            else
            {
                Mo = -8;
            }
          

            this.Invalidate();
        }
        private void createGrp(GroupBox grp)
        {
            grp.Name = "grp_" + i++.ToString();
            grp.Size = new Size(300, 200);
            grp.Text = grp.Name;
            grp.MouseDown += new MouseEventHandler(control_MouseDown);
            grp.MouseMove += new MouseEventHandler(control_MouseMove);
            grp.MouseUp += new MouseEventHandler(control_MouseUp);

            lv = new ListView();
            lv.View = View.Details;
            lv.FullRowSelect = true;
            lv.GridLines = true;
            lv.Dock = DockStyle.Fill;
            lv.Scrollable =true;
            lv.MultiSelect = false;
            lv.Name = "lv_" + i.ToString();
            lv.Parent = grp;
            lv.SelectedIndexChanged += new EventHandler(lv_SelectedIndexChanged);
            //lv.HoverSelection = true;
            #region add data
            lv.Columns.Add("列标题1", 100, HorizontalAlignment.Left);
            lv.Columns.Add("列标题2", 100, HorizontalAlignment.Left);
            lv.Columns.Add("列标题3", 100, HorizontalAlignment.Left);

            //add list
            lv.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度

            for (int j = 0; j < 20; j++)   //添加10行数据
            {
                ListViewItem lvi = new ListViewItem();

                // lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标

                lvi.Text = "subitem" + j;

                lvi.SubItems.Add("第2列,第" + j + "行");

                lvi.SubItems.Add("第3列,第" + j + "行");

                lv.Items.Add(lvi);
            }

            lv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            #endregion
            //lv.MouseClick+=new MouseEventHandler(listView1_MouseClick);
            lv.MouseDown+=new MouseEventHandler(listView1_MouseDown);
            lv.MouseMove+=new MouseEventHandler(listView1_MouseMove);
            lv.MouseUp+=new MouseEventHandler(listView1_MouseUp);
            lv.MouseEnter+=new EventHandler(listView1_MouseEnter);
        }

        void lv_SelectedIndexChanged(object sender, EventArgs e)
        {
             //Rectangle vrec = ((ListView)sender).GetItemRect(((ListView)sender).SelectedItems[0].Index);
             //   vrec.Offset(((ListView)sender).Location);
             //   vrec.Offset(((ListView)sender).Parent.Location);
             //   label2.Text = vrec.Location.ToString();
        }

        #region grpevent
        //鼠标按下坐标（control控件的相对坐标）
        Point mouseDownPoint = Point.Empty;
        //显示拖动效果的矩形
        Rectangle rect = Rectangle.Empty;
        //是否正在拖拽
        bool isDrag = false;
        void control_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint = e.Location;
                //记录控件的大小
                GroupBox g = (GroupBox)sender;
                g.Visible = false;
                rect = g.Bounds;
           
            }
        }
        void control_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
                //重新设置rect的位置，跟随鼠标移动
                rect.Location = getPointToForm(new Point(e.Location.X - mouseDownPoint.X, e.Location.Y - mouseDownPoint.Y));
                this.Refresh();
                label4.Text = rect.Location.ToString();
            }
        }
        void control_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (isDrag)
                {
                    GroupBox g = (GroupBox)sender;
                    
                    isDrag = false;
                    //移动control到放开鼠标的地方
                    g.Location = rect.Location;
                    g.Visible = true;
                    this.Refresh();
                }
                reset();
            }
        }
        //重置变量
        private void reset()
        {
            mouseDownPoint = Point.Empty;
            rect = Rectangle.Empty;
            isDrag = false;
        }
        //窗体重绘
        private void FormDrag_Paint(object sender, PaintEventArgs e)
        {
            if (rect != Rectangle.Empty)
            {
                if (isDrag)
                {//画一个和Control一样大小的黑框
                    e.Graphics.DrawRectangle(Pens.Black, rect);
                }
                else
                {
                    e.Graphics.DrawRectangle(new Pen(this.BackColor), rect);
                }
            }
        }
        //把相对与control控件的坐标，转换成相对于窗体的坐标。
        private Point getPointToForm(Point p)
        {
            return this.PointToClient(grp.PointToScreen(p));
        }

        #endregion
        List<string> contentList = new List<string>();
        private void Form1_Load(object sender, EventArgs e)
        {
            using (StreamReader sr=new StreamReader("1.txt",Encoding.UTF8))
            {
                string temp = "";
                while((temp=sr.ReadLine())!=null)
                {
                    contentList.Add(sr.ReadLine());
                }
            }
            groupBox1.MouseDown += new MouseEventHandler(control_MouseDown);
            groupBox1.MouseMove += new MouseEventHandler(control_MouseMove);
            groupBox1.MouseUp += new MouseEventHandler(control_MouseUp);
            #region
            listView1.Columns.Add("列标题1", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("列标题2", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("列标题3", 100, HorizontalAlignment.Left);

            //add list
            listView1.BeginUpdate();   //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度

            for (int j = 0; j < 10; j++)   //添加10行数据
            {
                ListViewItem lvi = new ListViewItem();

                // lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标

                lvi.Text = "subitem" + j;

                lvi.SubItems.Add("第2列,第" + j + "行");

                lvi.SubItems.Add("第3列,第" + j + "行");

                listView1.Items.Add(lvi);
            }

            listView1.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            #endregion
            //listview

           
        }
        private void paintPanel()
        { 
            
        }
        public bool isSelect = false;
        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (((ListView)sender).SelectedItems.Count > 0)
            {
                isSelect = true;
                Rectangle vrec = ((ListView)sender).GetItemRect(((ListView)sender).SelectedItems[0].Index);
                //vrec.Offset(((ListView)sender).Location);
                vrec.Offset(((ListView)sender).Parent.Location);
                textBox1.Bounds = vrec;
                textBox1.Size = vrec.Size;
                textBox1.BringToFront();
                textBox1.Text = vrec.Location.ToString();
                label2.Text = "a"+vrec.Location.ToString();
                label3.Text = ((ListView)sender).SelectedItems[0].Index.ToString();
            }
           
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (((ListView)sender).SelectedItems.Count > 0)
            {
                Rectangle vrec = ((ListView)sender).GetItemRect(((ListView)sender).SelectedItems[0].Index);
                vrec.Offset(((ListView)sender).Location);
                //vrec.Offset(((ListView)sender).Parent.Location);
                //label2.Text = vrec.Location.ToString();
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            isSelect = false;
        }

        private void listView1_MouseEnter(object sender, EventArgs e)
        {
            if (isSelect)
            {
                ListView s = (ListView)sender;
                label1.Text = s.Name;
            }
        }

        private void listView1_MouseLeave(object sender, EventArgs e)
        {
            if (isSelect)
            { //判断是否离开原listview，离开的话判断移动方向，在边界处开始描点
                
            }
        }
    }
}
