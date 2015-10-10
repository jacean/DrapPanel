using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DrapPanel
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            this.Paint+=new PaintEventHandler(drawPanel_Paint);
            this.MouseMove+=new MouseEventHandler(drawPanel_MouseMove);
            //s.MouseUp += new MouseEventHandler(drawPanel_MouseUp);

            foreach(Control c in this.Controls)
            {
                if (c.GetType().ToString() == "System.Windows.Forms.GroupBox")
                { 
                    

                    c.MouseDown+=new MouseEventHandler(control_MouseDown);
                    c.MouseMove+=new MouseEventHandler(control_MouseMove);
                    c.MouseUp+=new MouseEventHandler(control_MouseUp);
                    //////////////给listbox加事件

                    foreach (Control cc in c.Controls)
                    {
                        if (cc.GetType().ToString() == "System.Windows.Forms.Panel")
                        {
                            cc.MouseEnter += new EventHandler(panel_MouseEnter);
                            cc.MouseLeave += new EventHandler(panel_MouseLeave);
                            //cc.MouseDown += new MouseEventHandler(panel_MouseDown);
                            cc.MouseMove += new MouseEventHandler(panel_MouseMove);
                            //cc.MouseUp += new MouseEventHandler(panel_MouseUp);
                            cc.MouseClick += new MouseEventHandler(cc_MouseClick);
                        }
                    }


                }
            }

           

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);


           
        }

        void cc_MouseClick(object sender, MouseEventArgs e)
        {
            if (mDown == false)
            {//起始panel
                mDown = true;
                location = 1; 
                src = sender;
                count = 1;
            }
            else
            {//结束panel
                if (location == 2 && startPaint)
                {
                    drawPanel_MouseUp((object)this, this.PointToClient(Control.MousePosition));
                }
                mDown = false;
                startPaint = false;
                location = 0;
                count = 0;
                src=null;
                des=null;
            }
               
        }

        bool mDown = false;
        object src;
        object des;
        bool startPaint = false;
        int location = 0;//0在form，1在src，2在des
        int count = 0;//只有count=1，才启动画新线
        void panel_MouseDown(object sender, MouseEventArgs e)
        {
            if (mDown == false)
            {//起始panel
                mDown = true;
                location = 1;
            }
            else
            {//结束panel
                if (location == 2 && startPaint)
                {
                    drawPanel_MouseUp((object)this, this.PointToClient(Control.MousePosition));
                }
                mDown = false;
                startPaint = false;
                location = 0;
            }
               
            
        }
        void panel_MouseUp(object sender, MouseEventArgs e)
        {
            
        }

        void panel_MouseMove(object sender, MouseEventArgs e)
        {//这里不用做功能
            if (mDown&&location==1)
            {
                label1.Text = "在src里徘徊，";
                //label1.Text = sender.ToString()+"\n"+"开始画了"+getPointToForm((Control)sender, e.Location).ToString();
            }
            if(startPaint&&location==2)
            {
                    if (drawingLine != null)
                    {
                        label3.Text = "进入了des正在画endPoint" + e.Location.ToString();
                        //drawingLine.EndPoint = e.Location;

                        //drawPanel.Invalidate();
                        this.Invalidate();
                        //splitContainer1.Panel1.Invalidate();
                    }
            }
               
        }

       
       
        private void panel_MouseEnter(object sender, EventArgs e)
        {
            des = sender;
           
            if (startPaint && src != des)
            {
                
                label5.Text = "进入了des" + sender.ToString();
                location = 2;
                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
            }
            else if (startPaint && src == des)
            {
                count++;
                label5.Text = "回到了src" + sender.ToString();
                location = 1;
            }
        }

        private void panel_MouseLeave(object sender, EventArgs e)
        {
            if (mDown&&sender==src&&count==1)
            { //开始化线，其实坐标为鼠标当前坐标
                label4.Text = "离开src" + sender.ToString();
                location = 0;
                startPaint = true;
                drawPanel_MouseDown((object)this, this.PointToClient(Control.MousePosition));
            }
            if (startPaint && location == 2)
            {
                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
                location = 0;

            }
            if (startPaint && count > 1)
            {
                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
                location = 0;
            }
        }
        

        

        #region 划线，移动

        #region 定义线元素
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
        private Line moveLine = null;
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
        //private LineControl drawPanel = new LineControl();
        #endregion

        /// <summary>
        /// 在绘图区释放鼠标，结束当前线条绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drawPanel_MouseUp(object sender, Point e)
        //void drawPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawingLine == null && inLine)
            {
                inLine = false;
                moveLine = null; tempLine = null;
            }
            if (drawingLine == null) return;
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

            mDown = false;
        }
        /// <summary>
        /// 在绘图区按下鼠标，开始绘制新线条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drawPanel_MouseDown(object sender, Point e)
        //void drawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            //int x=e.Location.X;
            //int y=e.Location.Y;
            int x = e.X;
            int y = e.Y;
            foreach (Line l in lines)
            {
                if (l.StartPoint.X == l.EndPoint.X)
                { //线是水平线的话，x的横坐标在不在两个断电之间
                    if (x == l.StartPoint.X)
                    {
                        if (isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
                        {
                            inLine = true;
                            tempLine = new Line(l.StartPoint);
                            tempLine.EndPoint = l.EndPoint;
                            moveLine = l; lines.Remove(l); lines.Add(moveLine);
                            break;
                        }
                        else
                        {
                            inLine = false;
                            continue;
                        }
                    }

                }
                else if (l.StartPoint.Y == l.EndPoint.Y)
                {//线是垂直线
                    if (y == l.StartPoint.Y)
                    {
                        if (isBetween(l.StartPoint.X, l.EndPoint.X, x))
                        {
                            inLine = true; moveLine = l; tempLine = new Line(l.StartPoint);
                            tempLine.EndPoint = l.EndPoint; lines.Remove(l); lines.Add(moveLine); break;
                        }
                        else
                        {
                            inLine = false;
                            continue;//这条线已经没有再判断的必要了
                        }
                    }
                }

                else if ((x == l.StartPoint.X && y == l.StartPoint.Y) || (x == l.EndPoint.X && y == l.EndPoint.Y))
                {//点在线的两端点上
                    inLine = true;
                    moveLine = l;
                    tempLine = new Line(l.StartPoint);//防止引用fuzhi是地址
                    tempLine.EndPoint = l.EndPoint;
                    lines.Remove(l);
                    lines.Add(moveLine);
                    break;
                }
                //else if ((l.EndPoint.Y - l.StartPoint.Y) / (l.EndPoint.X - l.StartPoint.X) == (y - l.StartPoint.Y) / (x - l.StartPoint.X) && isBetween(l.StartPoint.X, l.EndPoint.X, x) && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
                else if ((l.EndPoint.Y - l.StartPoint.Y) * (x - l.StartPoint.X) == (y - l.StartPoint.Y) * (l.EndPoint.X - l.StartPoint.X) && isBetween(l.StartPoint.X, l.EndPoint.X, x) && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
                {
                    inLine = true; moveLine = l; tempLine = new Line(l.StartPoint);
                    tempLine.EndPoint = l.EndPoint; lines.Remove(l); lines.Add(moveLine);
                    break;
                }
                else
                {
                    inLine = false;

                }
            }
            if (inLine)//不在已有的线上
            {

                //move
                //moveStart = e.Location;
                moveStart = e;

            }
            else 
            {

                label2.Text = "startPoint" + e.ToString();
                drawingLine = new Line(e);
                lines.Add(drawingLine);

            }


        }
        /// <summary>
        /// 在绘图区移动鼠标时，如果正在绘制新线条，就更新绘制面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = sender.ToString()+"\n"+getPointToForm((Control)sender, e.Location).ToString();
            if (startPaint&&location==0)
            {
                if (drawingLine != null)
                {
                    label3.Text = "endPoint" + e.Location.ToString();
                    drawingLine.EndPoint = e.Location;

                    //drawPanel.Invalidate();
                    this.Invalidate();
                    //splitContainer1.Panel1.Invalidate();
                }
            }
            else if (e.Button==MouseButtons.Left)
            {
                if (drawingLine == null && inLine)
                {
                    //moveLine的坐标转换
                    moveLine.StartPoint.X = tempLine.StartPoint.X + e.X - moveStart.X;
                    moveLine.EndPoint.X = tempLine.EndPoint.X + e.X - moveStart.X;
                    moveLine.StartPoint.Y = tempLine.StartPoint.Y + e.Y - moveStart.Y;
                    moveLine.EndPoint.Y = tempLine.EndPoint.Y + e.Y - moveStart.Y;
                    this.Invalidate();
                    //splitContainer1.Panel1.Invalidate();
                }
                
            }
           
        }
        /// <summary>
        /// 绘制效果到面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drawPanel_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bp = new Bitmap(this.Width, this.Height); // 用于缓冲输出的位图对象
            //Bitmap bp = new Bitmap(this.Width, this.Height); // 用于缓冲输出的位图对象

            Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 消锯齿（可选项）
            Pen p = new Pen(Color.Black);
            foreach (Line line in lines)
            {
                if (line == drawingLine || line == moveLine)
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

        private bool isBetween(int x, int y, int z)
        {
            if (x >= y)
            {
                if (z <= x && z >= y) return true;
                return false;
            }
            else
            {
                if (z <= y && z >= x) return true;
                return false;
            }
        }
        #endregion

        #region controlevent 移动控件
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
            label1.Text = sender.ToString()+"\n"+getPointToForm((Control)sender, e.Location).ToString();
            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
                //重新设置rect的位置，跟随鼠标移动
                rect.Location = getPointToForm((Control)sender,new Point(e.Location.X - mouseDownPoint.X, e.Location.Y - mouseDownPoint.Y));
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
        //窗体重绘,该部分移动至画线那里
        //private void FormDrag_Paint(object sender, PaintEventArgs e)
        //{
        //    if (rect != Rectangle.Empty)
        //    {
        //        if (isDrag)
        //        {//画一个和Control一样大小的黑框
        //            e.Graphics.DrawRectangle(Pens.Black, rect);
        //        }
        //        else
        //        {
        //            e.Graphics.DrawRectangle(new Pen(this.BackColor), rect);
        //        }
        //    }
        //}
        //把相对与control控件的坐标，转换成相对于窗体的坐标。
        

        #endregion
        
        private Point getPointToForm(Control control, Point p)
        {
           
            return this.PointToClient(control.PointToScreen(p));
        }

       

    
    }
}
