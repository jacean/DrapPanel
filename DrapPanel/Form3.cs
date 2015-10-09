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
    public partial class Form3 : Form
    {
        //将被拖动的控件
        private GroupBox grp;
        private DataGridView dgv;
        public int i = 0;
        public Dictionary<string, GroupBox> grpDict = new Dictionary<string, GroupBox>();
        public Form3()
        {
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormDrag_Paint);

            
            InitializeComponent();
        }
        private void createGrp(GroupBox grp)
        {
            grp.Name = "grp_" + i++.ToString();
            grp.Size = new Size(300, 200);
            grp.Text = grp.Name;
            
            grp.MouseDown += new MouseEventHandler(control_MouseDown);
            grp.MouseMove += new MouseEventHandler(control_MouseMove);
            grp.MouseUp += new MouseEventHandler(control_MouseUp);
            dgv = new DataGridView();
            dgv.Name = "dgv_" + i.ToString();
            dgv.Parent = grp;
            dgv.Dock = DockStyle.Fill;
            dgv.MultiSelect = false;
            dgv.ColumnCount = 3;
            dgv.ColumnHeadersVisible = true;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            //dgv.Columns[0].Name = "left_select";
            dgv.Columns[0].Name = "ID"; 
            dgv.Columns[1].Name = "Name"; 
            dgv.Columns[2].Name = "Type";
           // dgv.Columns[4].Name = "right_select";
            //C:\Users\Public\Pictures\Sample Pictures\企鹅.jpg，考拉.jpg
            DataGridViewImageColumn imgCol=new DataGridViewImageColumn();
            imgCol.Name="left_select";
            //imgCol.Width = 20;
            dgv.Columns.Insert(0,imgCol);
            imgCol = new DataGridViewImageColumn();
            imgCol.Name = "right_select";
            //imgCol.Width = 20;
            dgv.Columns.Insert(4, imgCol);
            List<string> list = new List<string>();
            using (StreamReader sr = new StreamReader("1.txt", Encoding.UTF8))
            {
                int j=0;
                string t = "";
                while ((t = sr.ReadLine()) != null)
                {
                    list.Add(t);
                    dgv.Rows.Add();
                    dgv.Rows[j].Cells[1].Value=t.Split(',')[0];
                    dgv.Rows[j].Cells[2].Value=t.Split(',')[1];
                    dgv.Rows[j].Cells[3].Value=t.Split(',')[2];
                    j++;
                }
            }
         
            

          

           
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
        private void Form3_Load(object sender, EventArgs e)
        {
            grp = new GroupBox();
            createGrp(grp);
            this.Controls.Add(grp);
            grp = new GroupBox();
            createGrp(grp);
            this.Controls.Add(grp);
        }
    }
}
