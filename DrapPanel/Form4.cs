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
           
            panel4.Paint += new PaintEventHandler(panel4_Paint);
            panel4.MouseMove += new MouseEventHandler(panel4_MouseMove);
            panel4.MouseDown += new MouseEventHandler(panel4_MouseDown);
            panel4.MouseUp += new MouseEventHandler(panel4_MouseUp);
            panel4.MouseWheel += new MouseEventHandler(panel4_MouseWheel);
            foreach(Control c in this.panel4.Controls)
            {
                if (c.GetType().ToString() == "System.Windows.Forms.GroupBox")
                { 
                    c.MouseDown+=new MouseEventHandler(control_MouseDown);
                    c.MouseMove+=new MouseEventHandler(control_MouseMove);
                    c.MouseUp+=new MouseEventHandler(control_MouseUp);

                    foreach (Control cc in c.Controls)
                    {
                        if (cc.GetType().ToString() == "System.Windows.Forms.Panel")
                        {
                            cc.MouseEnter += new EventHandler(panel_MouseEnter);
                            cc.MouseLeave += new EventHandler(panel_MouseLeave);
                            cc.MouseMove += new MouseEventHandler(panel_MouseMove);
                            cc.MouseClick += new MouseEventHandler(panel_MouseClick);
                        }
                    }
                }
            }

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

        }

        void panel4_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawingLine == null && inLine)
            {
                inLine = false;
                moveLine = null; tempLine = null;
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

        void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;
           
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
                else if (x == l.StartPoint.X)
                {
                    if (y == l.StartPoint.Y)
                    {
                        //点在线的两端点上
                        inLine = true;
                        moveLine = l;
                        tempLine = new Line(l.StartPoint);//防止引用fuzhi是地址
                        tempLine.EndPoint = l.EndPoint;
                        lines.Remove(l);
                        lines.Add(moveLine);
                        break;
                    }
                    else
                    {
                        inLine = false;
                        continue;
                    }
                }
                else if (x == l.EndPoint.X)
                {
                    if (y == l.EndPoint.Y)
                    {
                        //点在线的两端点上
                        inLine = true;
                        moveLine = l;
                        tempLine = new Line(l.StartPoint);//防止引用fuzhi是地址
                        tempLine.EndPoint = l.EndPoint;
                        lines.Remove(l);
                        lines.Add(moveLine);
                        break;
                    }
                    else
                    {
                        inLine = false;
                        continue;
                    }
                }
                else if ((l.EndPoint.Y - l.StartPoint.Y) / (l.EndPoint.X - l.StartPoint.X) == (y - l.StartPoint.Y) / (x - l.StartPoint.X) && isBetween(l.StartPoint.X, l.EndPoint.X, x) && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
                //else if (((l.EndPoint.Y - l.StartPoint.Y) * (x - l.StartPoint.X)) == ((y - l.StartPoint.Y) * (l.EndPoint.X - l.StartPoint.X)) && isBetween(l.StartPoint.X, l.EndPoint.X, x) && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))//不知道为啥不好使
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
            if (inLine)//在已有的线上
            {
                moveStart = e.Location;
                label7.Text = moveLine.srcg.ToString();
                label8.Text = moveLine.desg.ToString();
                Form5 newform = new Form5();
                newform.ShowDialog();
            }
           
        }

        /// <summary>
        /// 在绘图区移动鼠标时，如果正在绘制新线条，就更新绘制面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = sender.ToString() + "\n" + getPointToForm((Control)sender, e.Location).ToString();
            if (startPaint && location == 0)
            {
                if (drawingLine != null)
                {
                    label3.Text = "endPoint" + e.Location.ToString();
                    drawingLine.EndPoint = e.Location;

                    //drawPanel.Invalidate();
                    this.Invalidate();
                    //splitContainer1.Panel1.Invalidate();
                    this.Refresh();
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (drawingLine == null && inLine)
                {
                    //moveLine的坐标转换
                    moveLine.StartPoint.X = tempLine.StartPoint.X + e.X - moveStart.X;
                    moveLine.EndPoint.X = tempLine.EndPoint.X + e.X - moveStart.X;
                    moveLine.StartPoint.Y = tempLine.StartPoint.Y + e.Y - moveStart.Y;
                    moveLine.EndPoint.Y = tempLine.EndPoint.Y + e.Y - moveStart.Y;
                    this.Invalidate();
                    this.Refresh();
                    //splitContainer1.Panel1.Invalidate();
                }

            }


        }

        void panel4_MouseWheel(object sender, MouseEventArgs e)
        {
            float Mo = 0;
           
            if (e.Delta > 0)
            {
                Mo = 1.02f;
              
               
            }
            else if (e.Delta < 0)
            {
                Mo = 0.98f;
            }

            foreach (Control ct in this.panel4.Controls)
            {//看成是点的移动
                ct.Width = (int)(Mo*ct.Width);
                ct.Height= (int)(Mo*ct.Height);

                //ct.Left -=  (int)((Mo-1)* ((float)ct.Width/2 ));//以自己中心为原点放大
                //ct.Left += (int)(((ct.Left + ct.Width/2) - e.X) * (float)(Mo - 1));//一鼠标为中心改变缩放偏移量
                //ct.Top -= (int)((Mo-1) * ((float)ct.Height/2));
                //ct.Top += (int)(((ct.Top + ct.Height/2) - e.Y) * (float)(Mo - 1));
                ct.Left += (int)((float)(ct.Left - e.X) * (Mo - 1));
                ct.Top += (int)((float)(ct.Top - e.Y) * (Mo - 1));
                //ct.Location = getPointToForm((Control)sender, new Point(e.Location.X - mouseDownPoint.X, e.Location.Y - mouseDownPoint.Y));
                
                foreach (Control cp in ct.Controls)
                {//这里之后改
                    listBox1.Width = cp.Width - 30;
                    listBox1.Height = cp.Height-30;
                    listBox2.Width = cp.Width-30;
                    listBox2.Height = cp.Height-30;
                }

            }
            ///////////////////////////////
            //移动线条////////////////////
           
            foreach (Line line in lines)
            {
              
                //线条的起始坐标转换
                line.StartPoint.X += (int)((float)(line.StartPoint.X - e.X) * (Mo - 1));
                line.StartPoint.Y += (int)((float)(line.StartPoint.Y - e.Y) * (Mo - 1));
                line.EndPoint.X += (int)((float)(line.EndPoint.X - e.X) * (Mo - 1));
                line.EndPoint.Y += (int)((float)(line.EndPoint.Y - e.Y) * (Mo - 1));

                //用相对坐标试下
                //
                //drawingLine.startPointtoSender.X = this.PointToClient(Control.MousePosition).X - drawingLine.srcg.Location.X;
                //drawingLine.startPointtoSender.Y = this.PointToClient(Control.MousePosition).Y - drawingLine.srcg.Location.Y;
                //
                //line.startPointtoSender.X += (int)((float)(line.startPointtoSender.X - e.X) * (Mo - 1));
                //line.startPointtoSender.Y += (int)((float)(line.startPointtoSender.Y - e.Y) * (Mo - 1));
                //line.endPointtoSender.X += (int)((float)(line.endPointtoSender.X - e.X) * (Mo - 1));
                //line.endPointtoSender.Y += (int)((float)(line.endPointtoSender.Y - e.Y) * (Mo - 1));

                //line.StartPoint = new Point(line.srcg.Location.X + line.startPointtoSender.X, line.srcg.Location.Y + line.startPointtoSender.Y);
                //line.EndPoint = new Point(line.desg.Location.X + line.endPointtoSender.X, line.desg.Location.Y + line.endPointtoSender.Y);

            }
           
            ///////////////////////////////


          
            this.Invalidate();
            this.Refresh();
        }

        /// <summary>
        /// 绘制效果到面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void panel4_Paint(object sender, PaintEventArgs e)
        {

            Bitmap bp = new Bitmap(panel4.Width, panel4.Height); // 用于缓冲输出的位图对象
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
            //e.Graphics.DrawImage(bp, Point.Empty);
            e.Graphics.DrawImage(bp, panel4.Location);
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

        private void panel4_MouseEnter(object sender, EventArgs e)
        {

            this.panel4.Focus();
        }

        private void panel4_MouseLeave(object sender, EventArgs e)
        {

        }


        bool mDown = false;
        object src;
        object des;
        bool startPaint = false;
        int location = 0;//0在form，1在src，2在des
        int count = 0;//只有count=1，才启动画新线
        void panel_MouseClick(object sender, MouseEventArgs e)
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

                    drawingLine.desg = (GroupBox)((Panel)sender).Parent;
                    //
                    drawingLine.endPointtoSender.X = this.PointToClient(Control.MousePosition).X - drawingLine.desg.Location.X;
                    drawingLine.endPointtoSender.Y = this.PointToClient(Control.MousePosition).Y - drawingLine.desg.Location.Y;
                    //
                    label6.Text = drawingLine.desg.ToString();
                    drawPanel_MouseUp((object)this, this.PointToClient(Control.MousePosition));

                }
                else
                {
                    if (drawingLine != null)
                    {
                        //清空之前的line
                        drawingLine.StartPoint = Point.Empty;
                        drawingLine.EndPoint = Point.Empty;
                        drawingLine = null;
                        lines.Remove(drawingLine);
                    }
                }
                    
                mDown = false;
                startPaint = false;
                location = 0;
                count = 0;
                src=null;
                des=null;
                label2.Text = "";
                label3.Text = "";
                label4.Text = "";
                label5.Text = "";

            }
            this.Refresh();
               
        }

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
                    //drawPanel_MouseUp((object)this, this.PointToClient(Control.MousePosition));
                    drawPanel_MouseUp((object)panel4, this.PointToClient(Control.MousePosition));
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
            if (drawingLine != null)
            {
                if (mDown && location == 1)
                {
                    label1.Text = "在src里徘徊，";
                    //label1.Text = sender.ToString()+"\n"+"开始画了"+getPointToForm((Control)sender, e.Location).ToString();
                }
                if (startPaint && location == 2)
                {
                    //if (drawingLine != null)
                    //{
                    label3.Text = "进入了des正在画endPoint" + e.Location.ToString();
                    drawingLine.EndPoint = this.getPointToForm((Control)sender, e.Location);

                    //drawPanel.Invalidate();

                    //splitContainer1.Panel1.Invalidate();
                    //}
                }
                this.Invalidate();
                this.Refresh();
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
                //drawPanel_MouseDown((object)this, this.PointToClient(Control.MousePosition));
                drawPanel_MouseDown((object)panel4, this.PointToClient(Control.MousePosition));
                drawingLine.srcg = (GroupBox)((Panel)sender).Parent;
                //
                drawingLine.startPointtoSender.X = this.PointToClient(Control.MousePosition).X - drawingLine.srcg.Location.X;
                drawingLine.startPointtoSender.Y = this.PointToClient(Control.MousePosition).Y - drawingLine.srcg.Location.Y;
                //
                label6.Text = drawingLine.srcg.ToString();
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
            public GroupBox srcg;
            public Point startPointtoSender = Point.Empty;
            public Point endPointtoSender = Point.Empty;
            public GroupBox desg;
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
        {
            if (drawingLine == null && inLine)
            {
                inLine = false;
                moveLine = null; tempLine = null;
            }
            if (drawingLine == null) return;
            if (e == drawingLine.StartPoint)
            {
                drawingLine.StartPoint = Point.Empty;
                drawingLine.EndPoint = Point.Empty;
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
                    this.Invalidate();
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
        //窗体重绘,该部分移动至画线那里,如果单独需的话把这部分加入控件事件里
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
