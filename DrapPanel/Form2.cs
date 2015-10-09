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
    public partial class Form2 : Form
    {
        //将被拖动的控件
        private GroupBox grp;
        private DataGridView dgv;
        public int i = 0;
        public Dictionary<string, GroupBox> grpDict = new Dictionary<string, GroupBox>();
        public Form2()
        {
           // this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormDrag_Paint);
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;//避免绘制时闪烁
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            //this.MouseDown += new MouseEventHandler(drawPanel_MouseDown);
            //this.MouseUp += new MouseEventHandler(drawPanel_MouseUp);
            //this.MouseMove += new MouseEventHandler(drawPanel_MouseMove);
            this.Paint += new PaintEventHandler(drawPanel_Paint);
            grp = new GroupBox();
            createGrp(grp);
            this.Controls.Add(grp);
            grp = new GroupBox();
            createGrp(grp);
            this.Controls.Add(grp);

        }
        #region 划线，移动
        class Line
        {
            public Point StartPoint = Point.Empty;
            public Point EndPoint = Point.Empty;

            public Line(Point startPoint)
            {
                StartPoint = startPoint;
                EndPoint = startPoint;
            }

        }
       
        /// <summary>
        /// 用来确定鼠标是不是在已有的线上
        /// </summary>
        bool inLine = false;
        /// <summary>
        /// 移动起始的点，用来计算新的坐标
        /// </summary>
        private Point moveStart = Point.Empty;
        /// <summary>
        /// 作为不变的起始线来计算移动线的坐标
        /// </summary>
        private Line tempLine = null;
        /// <summary>
        /// 可以被移动的正选中的线
        /// </summary>
        private  Line moveLine = null;
        /// <summary>
        /// 用于保存绘出线条的集合
        /// </summary>
        private List<Line> lines = new List<Line>();
        /// <summary>
        /// 用于保存当前正在绘制的线条
        /// </summary>
        private Line drawingLine = null;
        /// <summary>
        /// 用于显示绘图的面板组件
        /// </summary>
        //private DrawPanel drawPanel = new DrawPanel();
        private LineControl drawPanel = new LineControl();
        /// <summary>
        /// 在绘图区释放鼠标，结束当前线条绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       // void drawPanel_MouseUp(object sender, MouseEventArgs e)
         void drawPanel_MouseUp(object sender,Point e)
        {
            if (drawingLine == null && inLine) {
                inLine = false;
                moveLine = null; tempLine = null;
            }
            if(drawingLine==null)return;
            if (e == drawingLine.StartPoint)
            {
                drawingLine = null;
                lines.Remove(drawingLine);
            }
            else
            {
                drawingLine.EndPoint = e;
                
                drawingLine = null;
            }

            if (lines.Count > 0)
            {
                listBox1.Items.Clear();
                foreach (Line l in lines)
                {
                    listBox1.Items.Add(l.StartPoint.ToString() + "," + l.EndPoint.ToString());
                }
            }
        }
        /// <summary>
        /// 在绘图区按下鼠标，开始绘制新线条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       // void drawPanel_MouseDown(object sender, MouseEventArgs e)
        void drawPanel_MouseDown(object sender, Point p)
        {
            //int x=e.Location.X;
            //int y=e.Location.Y;
            int x = p.X;
            int y = p.Y;
            foreach(Line l in lines)
            {
                if((x==l.StartPoint.X&&y==l.StartPoint.Y)||(x==l.EndPoint.X&&y==l.EndPoint.Y))
                {//
                    inLine = true;
                    moveLine = l;
                    tempLine = new Line(l.StartPoint);//防止引用fuzhi是地址
                    tempLine.EndPoint = l.EndPoint;
                    lines.Remove(l);
                    lines.Add(moveLine);
                    break;
                }else if (l.StartPoint.X == l.EndPoint.X)
                { //x equal
                    if (x == l.StartPoint.X && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
                    {
                        inLine = true;
                        tempLine = new Line(l.StartPoint);
                        tempLine.EndPoint = l.EndPoint;
                        moveLine = l;  lines.Remove(l); lines.Add(moveLine);
                        break;
                    }
                }else if (l.StartPoint.Y == l.EndPoint.Y)
                {
                    if (y == l.StartPoint.Y && isBetween(l.StartPoint.X, l.EndPoint.X, x))
                    {
                        inLine = true; moveLine = l; tempLine = new Line(l.StartPoint);
                        tempLine.EndPoint = l.EndPoint; lines.Remove(l); lines.Add(moveLine); break;
                    }
                }else if( (l.EndPoint.Y-l.StartPoint.Y)/(l.EndPoint.X-l.StartPoint.X)==(y-l.StartPoint.Y)/(x-l.StartPoint.X)&&isBetween(l.StartPoint.X,l.EndPoint.X,x)&&isBetween(l.StartPoint.Y,l.EndPoint.Y,y))
               {
                   inLine = true; moveLine = l; tempLine = new Line(l.StartPoint);
                   tempLine.EndPoint = l.EndPoint; lines.Remove(l); lines.Add(moveLine);
                   break;
               }
            }
            if(inLine)//不在已有的线上
            {
                label2.Text = "inline";
                //move
                //moveStart = e.Location;
                moveStart = p;

            }
            else
            {
                label2.Text = "outLine";
            drawingLine = new Line(p);
            lines.Add(drawingLine);
            
            }

            
        }
        /// <summary>
        /// 在绘图区移动鼠标时，如果正在绘制新线条，就更新绘制面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //void drawPanel_MouseMove(object sender, MouseEventArgs e)
        void drawPanel_MouseMove(object sender, Point e)
        {
            if (drawingLine != null)
            {
                drawingLine.EndPoint = e;
                //drawPanel.Invalidate();
                this.Invalidate();
            }
            if (drawingLine == null && inLine)
            { 
                //moveLine的坐标转换
                moveLine.StartPoint.X = tempLine.StartPoint.X+e.X - moveStart.X;
                moveLine.EndPoint.X =tempLine.EndPoint.X+ e.X - moveStart.X;
                moveLine.StartPoint.Y = tempLine.StartPoint.Y + e.Y - moveStart.Y;
                moveLine.EndPoint.Y = tempLine.EndPoint.Y + e.Y - moveStart.Y;
                this.Invalidate();
            }
            label1.Text = this.PointToClient(Control.MousePosition).X.ToString() + "," + this.PointToClient(Control.MousePosition).Y.ToString();
        }
        /// <summary>
        /// 绘制效果到面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drawPanel_Paint(object sender, PaintEventArgs e)
        {
           // Bitmap bp = new Bitmap(drawPanel.Width, drawPanel.Height); // 用于缓冲输出的位图对象
            Bitmap bp = new Bitmap(this.Width, this.Height); // 用于缓冲输出的位图对象

            Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 消锯齿（可选项）
            Pen p = new Pen(Color.Black);
            foreach (Line line in lines)
            {
                if (line == drawingLine||line==moveLine)
                {
                    // 当前绘制的线条是正在鼠标定位的线条
                    p.Color = Color.Blue;
                }
                else
                {
                    p.Color = Color.Black;
                }
                g.DrawLine(p, line.StartPoint, line.EndPoint);
            }
            // 将缓冲位图绘制到输出
            e.Graphics.DrawImage(bp, Point.Empty);
            //移动容器
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

        private bool isBetween(int x, int y,int z)
        {
            if (x >= y)
            {
                if (z <= x && z >= y) return true;
                return false;
            }
            else
            {
                if (z <= y && z >=x) return true;
                return false;
            }
        }
        #endregion
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
            dgv.MouseDown += new MouseEventHandler(dgv_MouseDown);
            dgv.MouseMove += new MouseEventHandler(dgv_MouseMove);
            dgv.MouseUp += new MouseEventHandler(dgv_MouseUp);
            dgv.MouseEnter += new EventHandler(dgv_MouseEnter);
            dgv.MouseLeave += new EventHandler(dgv_MouseLeave);
            dgv.Dock = DockStyle.Fill;
            dgv.MultiSelect = false;
            dgv.ColumnCount = 3;
            dgv.ColumnHeadersVisible = true;
            dgv.RowHeadersVisible = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Columns[0].Name = "ID";
            dgv.Columns[1].Name = "Name";
            dgv.Columns[2].Name = "Type";
            //C:\Users\Public\Pictures\Sample Pictures\企鹅.jpg，考拉.jpg
            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "left_select";
            dgv.Columns.Insert(0, imgCol);
            imgCol = new DataGridViewImageColumn();
            imgCol.Name = "right_select";
            dgv.Columns.Insert(4, imgCol);
            List<string> list = new List<string>();
            using (StreamReader sr = new StreamReader("1.txt", Encoding.UTF8))
            {
                int j = 0;
                string t = "";
                while ((t = sr.ReadLine()) != null)
                {
                    list.Add(t);
                    dgv.Rows.Add();
                    dgv.Rows[j].Cells[1].Value = t.Split(',')[0];
                    dgv.Rows[j].Cells[2].Value = t.Split(',')[1];
                    dgv.Rows[j].Cells[3].Value = t.Split(',')[2];
                    j++;
                }
            }
        }


        #region datagridview上的操作
        private DataGridView src = new DataGridView();
        private DataGridView des = new DataGridView();
        //取值更换逻辑有问题
        private bool preDraw = false;
        private bool srcDgv = false;
        private bool desDgv = false;
        private bool leaveSrc = false;
        private bool isLeft = false;
        Point srcPoint = Point.Empty;
        Point desPoint = Point.Empty;

        void reSet()
        {
            preDraw = false;
            desDgv = false;
            leaveSrc = false;
            srcDgv = false;
            isLeft = false;
            srcPoint =Point.Empty;
            desPoint = Point.Empty;
        }
        void dgv_MouseLeave(object sender, EventArgs e)
        {//可以开始画了
            if (preDraw)
            {
               
                Rectangle vrec = ((DataGridView)sender).GetCellDisplayRectangle(((DataGridView)sender).SelectedCells[0].ColumnIndex, ((DataGridView)sender).SelectedCells[0].RowIndex, true);
                vrec.Offset(((DataGridView)sender).Location);
                vrec.Offset(((DataGridView)sender).Parent.Location);
                button1.Bounds = vrec;
                label3.Text = vrec.Location + ":" + vrec.Right + "," + vrec.Bottom;
                //根据左右位置决定起始点
                if (isLeft)
                {
                    srcPoint = new Point(vrec.Location.X, vrec.Location.Y + vrec.Height / 2);
                }
                else
                {
                    srcPoint = new Point(vrec.Right, vrec.Location.Y + vrec.Height / 2);
                }
                label4.Text = srcPoint.ToString();
                drawPanel_MouseDown(sender,srcPoint);
                leaveSrc = true;
                src = (DataGridView)sender;
            }
        }

        void dgv_MouseEnter(object sender, EventArgs e)
        {//进入目标所在地
            if (leaveSrc)
            {
                desDgv = true;
                des = (DataGridView)sender;
            }
        }

        void dgv_MouseUp(object sender, MouseEventArgs e)
        {//画完了
            if (desDgv)
            {//画完了，记录保存，否则取消这次划线 
                 Rectangle vrec = ((DataGridView)sender).GetCellDisplayRectangle(((DataGridView)sender).SelectedCells[0].ColumnIndex, ((DataGridView)sender).SelectedCells[0].RowIndex, true);
                vrec.Offset(((DataGridView)sender).Location);
                vrec.Offset(((DataGridView)sender).Parent.Location);
                button1.Bounds = vrec;
                label3.Text = vrec.Location + ":" + vrec.Right + "," + vrec.Bottom;
                //根据左右位置决定起始点
                if (isLeft)
                {
                    srcPoint = new Point(vrec.Location.X, vrec.Location.Y + vrec.Height / 2);
                }
                else
                {
                    srcPoint = new Point(vrec.Right, vrec.Location.Y + vrec.Height / 2);
                }
                //drawPanel_MouseUp(sender,)
                label4.Text = srcPoint.ToString();
                label4.Text += ((DataGridView)sender).Name;
                reSet();

            }
           
            
        }

        void dgv_MouseMove(object sender, MouseEventArgs e)
        {
            if (leaveSrc)
            { //可以预备画了,获得所在位置的坐标

                
            }
        }

        void dgv_MouseDown(object sender, MouseEventArgs e)
        {//预判启动
            if (((DataGridView)sender).SelectedCells[0].ColumnIndex == 0)
            {
                isLeft = true; preDraw = true; 
            }
            else if (((DataGridView)sender).SelectedCells[0].ColumnIndex == 4)
            {
                isLeft = false; preDraw = true;
            }
            else
            {
                return;
            }
           
            
        }
        #endregion
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
    }
}
