
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Windows.Forms;
//using System.Data.SqlClient;
//using System.IO;
//namespace DrapPanel
//{
//    public partial class Form4 : Form
//    {
//        public Form4()
//        {
//            InitializeComponent();
//        }
//        sqlConn sqlconn = new sqlConn("Data Source=ELAB-SQ252L;Initial Catalog=student;Persist Security Info=True;User ID=ta;Password=elab2013", "SQL");

//        private void Form4_Load(object sender, EventArgs e)
//        {

//            panel4.Paint += new PaintEventHandler(panel4_Paint);
//            panel4.MouseMove += new MouseEventHandler(panel4_MouseMove);
//            panel4.MouseDown += new MouseEventHandler(panel4_MouseDown);
//            panel4.MouseUp += new MouseEventHandler(panel4_MouseUp);
//            panel4.MouseWheel += new MouseEventHandler(panel4_MouseWheel);

//            loadData();

//            this.DoubleBuffered = true;
//            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
//            DataTable dt = sqlconn.getVector("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name");
//            for (int i = 0; i < dt.Rows.Count; i++)
//            {
//                comboBox1.Items.Add(dt.Rows[i][0].ToString());

//            }
//            panel4.Height = this.ClientSize.Height - panel1.Height;


//        }

//        private void button1_Click(object sender, EventArgs e)
//        {

//            GroupBox grp = new GroupBox();
//            addNewGroupBox(grp, comboBox1.Text);

//        }


//        void panel4_MouseUp(object sender, MouseEventArgs e)
//        {
//            if (drawingLine == null && inLine)
//            {
//                inLine = false;
//                moveLine = null; tempLine = null;
//            }
//            if (lines.Count > 0)
//            {
//                listBox1.Items.Clear();
//                foreach (Line l in lines)
//                {
//                    listBox1.Items.Add(l.StartPoint.ToString() + "," + l.EndPoint.ToString());
//                }
//            }
//            if (isMoveForm)
//            {
//                isMoveForm = false;
//            }
//        }

//        void panel4_MouseDown(object sender, MouseEventArgs e)
//        {
//            int x = e.Location.X;
//            int y = e.Location.Y;

//            #region 判断鼠标是否选中线，如果选中的话inline=true，同时moveline被赋值和添加

//            foreach (Line l in lines)
//            {
//                if (l.StartPoint.X == l.EndPoint.X)
//                { //线是水平线的话，x的横坐标在不在两个断电之间
//                    if (x == l.StartPoint.X)
//                    {
//                        if (isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
//                        {
//                            inLine = true;
//                            tempLine = new Line(l.StartPoint);
//                            tempLine.EndPoint = l.EndPoint;
//                            moveLine = l; lines.Remove(l); lines.Add(moveLine);
//                            break;
//                        }
//                        else
//                        {
//                            inLine = false;
//                            continue;
//                        }
//                    }

//                }
//                else if (l.StartPoint.Y == l.EndPoint.Y)
//                {//线是垂直线
//                    if (y == l.StartPoint.Y)
//                    {
//                        if (isBetween(l.StartPoint.X, l.EndPoint.X, x))
//                        {
//                            inLine = true; moveLine = l; tempLine = new Line(l.StartPoint);
//                            tempLine.EndPoint = l.EndPoint; lines.Remove(l); lines.Add(moveLine); break;
//                        }
//                        else
//                        {
//                            inLine = false;
//                            continue;//这条线已经没有再判断的必要了
//                        }
//                    }
//                }
//                else if (x == l.StartPoint.X)
//                {
//                    if (y == l.StartPoint.Y)
//                    {
//                        //点在线的两端点上
//                        inLine = true;
//                        moveLine = l;
//                        tempLine = new Line(l.StartPoint);//防止引用fuzhi是地址
//                        tempLine.EndPoint = l.EndPoint;
//                        lines.Remove(l);
//                        lines.Add(moveLine);
//                        break;
//                    }
//                    else
//                    {
//                        inLine = false;
//                        continue;
//                    }
//                }
//                else if (x == l.EndPoint.X)
//                {
//                    if (y == l.EndPoint.Y)
//                    {
//                        //点在线的两端点上
//                        inLine = true;
//                        moveLine = l;
//                        tempLine = new Line(l.StartPoint);//防止引用fuzhi是地址
//                        tempLine.EndPoint = l.EndPoint;
//                        lines.Remove(l);
//                        lines.Add(moveLine);
//                        break;
//                    }
//                    else
//                    {
//                        inLine = false;
//                        continue;
//                    }
//                }
//                else if ((l.EndPoint.Y - l.StartPoint.Y) / (l.EndPoint.X - l.StartPoint.X) == (y - l.StartPoint.Y) / (x - l.StartPoint.X) && isBetween(l.StartPoint.X, l.EndPoint.X, x) && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
//                //else if (((l.EndPoint.Y - l.StartPoint.Y) * (x - l.StartPoint.X)) == ((y - l.StartPoint.Y) * (l.EndPoint.X - l.StartPoint.X)) && isBetween(l.StartPoint.X, l.EndPoint.X, x) && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))//不知道为啥不好使
//                {
//                    inLine = true; moveLine = l; tempLine = new Line(l.StartPoint);
//                    tempLine.EndPoint = l.EndPoint; lines.Remove(l); lines.Add(moveLine);
//                    break;
//                }
//                else
//                {
//                    inLine = false;

//                }
//            }

//            #endregion

//            if (inLine)//在已有的线上
//            {//屏蔽选中线条移动事件
//                moveStart = e.Location;
//                label7.Text = moveLine.srcg.ToString();
//                label8.Text = moveLine.desg.ToString();
//                label9.Text = moveLine.startPointtoSender.ToString();
//                label10.Text = moveLine.endPointtoSender.ToString();
//                Form5 newform = new Form5(moveLine.srcg.Name.ToString(), moveLine.desg.Name.ToString());
//                newform.ShowDialog();
//                moveLine = null;/////////////防止弹出窗口后自身的up事件不执行
//            }
//            else
//            {//画面整体移动//怎么才能把控件画在画布上，这样调整画布的起始坐标就好了嘛
//                isMoveForm = true;
//                movestartPoint = e.Location;
//                //给附加坐标赋值
//                foreach (Line l in lines)
//                {
//                    l.endPointAdd = l.EndPoint;
//                    l.startPointAdd = l.StartPoint;
//                }
//                pointList.Clear();
//                foreach (GroupBox gr in panel4.Controls)
//                {
//                    pointList.Add(gr.Location);
//                }

//            }

//        }
//        List<Point> pointList = new List<Point>();
//        public bool isMoveForm = false;//是否在拖动画面
//        public Point movestartPoint = Point.Empty;


//        /// <summary>
//        /// 在绘图区移动鼠标时，如果正在绘制新线条，就更新绘制面板
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        void panel4_MouseMove(object sender, MouseEventArgs e)
//        {
//            label1.Text = sender.ToString() + "\n" + getPointToForm((Control)sender, e.Location).ToString();
//            if (startPaint && location == 0)
//            {

//                if (drawingLine != null)
//                {
//                    label3.Text = "endPoint" + e.Location.ToString();
//                    drawingLine.EndPoint = e.Location;

//                    //drawPanel.Invalidate();
//                    this.Invalidate();
//                    //splitContainer1.Panel1.Invalidate();
//                    this.Refresh();
//                }
//            }
//            else if (e.Button == MouseButtons.Left)
//            {

//                if (drawingLine == null && inLine)
//                {
//                    label1.Text = "选中线条";
//                    //moveLine的坐标转换
//                    //屏蔽选中线条移动事件
//                    //moveLine.StartPoint.X = tempLine.StartPoint.X + e.X - moveStart.X;
//                    //moveLine.EndPoint.X = tempLine.EndPoint.X + e.X - moveStart.X;
//                    //moveLine.StartPoint.Y = tempLine.StartPoint.Y + e.Y - moveStart.Y;
//                    //moveLine.EndPoint.Y = tempLine.EndPoint.Y + e.Y - moveStart.Y;

//                    this.Invalidate();
//                    this.Refresh();
//                    //splitContainer1.Panel1.Invalidate();
//                }

//            }

//            if (isMoveForm)
//            {

//                foreach (Line line in lines)
//                {

//                    //坐标转换  //只在确定移动的时候赋值，然后使用应该就可以了吧，就是在down时              

//                    line.StartPoint.X = line.startPointAdd.X - movestartPoint.X + e.X;
//                    line.StartPoint.Y = line.startPointAdd.Y - movestartPoint.Y + e.Y;

//                    line.EndPoint.X = line.endPointAdd.X - movestartPoint.X + e.X;
//                    line.EndPoint.Y = line.endPointAdd.Y - movestartPoint.Y + e.Y;

//                }

//                //移动容器,依靠的是系统对groupbox的遍历是一致的顺序
//                int i = 0;
//                foreach (GroupBox grp in panel4.Controls)
//                {

//                    grp.Left = pointList[i].X - movestartPoint.X + e.X;
//                    grp.Top = pointList[i++].Y - movestartPoint.Y + e.Y;
//                }
//                this.Invalidate();
//                this.Refresh();
//            }


//        }

//        void panel4_MouseWheel(object sender, MouseEventArgs e)
//        {
//            if (drawingLine != null || moveLine != null)
//            {
//                return;
//            }
//            float Mo = 0;

//            if (e.Delta > 0)
//            {
//                Mo = 1.02f;


//            }
//            else if (e.Delta < 0)
//            {
//                Mo = 0.98f;
//            }

//            foreach (Control ct in this.panel4.Controls)
//            {//看成是点的移动
//                ct.Width += (int)((Mo - 1) * (float)ct.Width);
//                ct.Height += (int)((Mo - 1) * (float)ct.Height);

//                //ct.Left -=  (int)((Mo-1)* ((float)ct.Width/2 ));//以自己中心为原点放大
//                //ct.Left += (int)(((ct.Left + ct.Width/2) - e.X) * (float)(Mo - 1));//一鼠标为中心改变缩放偏移量
//                //ct.Top -= (int)((Mo-1) * ((float)ct.Height/2));
//                //ct.Top += (int)(((ct.Top + ct.Height/2) - e.Y) * (float)(Mo - 1));
//                ct.Left += (int)((float)(ct.Left - e.X) * (Mo - 1));
//                ct.Top += (int)((float)(ct.Top - e.Y) * (Mo - 1));

//                /////////
//                foreach (Control cp in ct.Controls)
//                {//这里之后改
//                    foreach (Control cl in cp.Controls)
//                    {
//                        cl.Width = cp.Width - 30;
//                        cl.Height = cp.Height - 30;

//                    }

//                }

//            }
//            ///////////////////////////////
//            //移动线条////////////////////

//            foreach (Line line in lines)
//            {

//                //线条的起始坐标转换,这个有误差，且越积累越大
//                //line.StartPoint.X += (int)((float)(line.StartPoint.X - e.X) * (Mo - 1));
//                //line.StartPoint.Y += (int)((float)(line.StartPoint.Y - e.Y) * (Mo - 1));
//                //line.EndPoint.X += (int)((float)(line.EndPoint.X - e.X) * (Mo - 1));
//                //line.EndPoint.Y += (int)((float)(line.EndPoint.Y - e.Y) * (Mo - 1));

//                //用相对坐标试下
//                //                
//                line.startPointtoSender.X += (int)((float)(line.startPointtoSender.X) * (Mo - 1));
//                line.startPointtoSender.Y += (int)((float)(line.startPointtoSender.Y) * (Mo - 1));
//                line.endPointtoSender.X += (int)((float)(line.endPointtoSender.X) * (Mo - 1));
//                line.endPointtoSender.Y += (int)((float)(line.endPointtoSender.Y) * (Mo - 1));
//                line.StartPoint = new Point(line.srcg.Location.X + line.startPointtoSender.X, line.srcg.Location.Y + line.startPointtoSender.Y);
//                line.EndPoint = new Point(line.desg.Location.X + line.endPointtoSender.X, line.desg.Location.Y + line.endPointtoSender.Y);


//            }


//            this.Invalidate();
//            this.Refresh();
//        }

//        /// <summary>
//        /// 绘制效果到面板
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        void panel4_Paint(object sender, PaintEventArgs e)
//        {

//            Bitmap bp = new Bitmap(panel4.Width, panel4.Height); // 用于缓冲输出的位图对象
//            //Bitmap bp = new Bitmap(this.Width, this.Height); // 用于缓冲输出的位图对象

//            Graphics g = Graphics.FromImage(bp);
//            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 消锯齿（可选项）
//            Pen p = new Pen(Color.Black);
//            foreach (Line line in lines)
//            {
//                if (line == drawingLine || line == moveLine)
//                {
//                    // 当前绘制的线条是正在鼠标定位的线条
//                    p.Color = Color.Blue;
//                }
//                else
//                {
//                    p.Color = Color.Black;
//                }
//                g.DrawLine(p, line.StartPoint, line.EndPoint);
//            }
//            // 将缓冲位图绘制到输出
//            //e.Graphics.DrawImage(bp, Point.Empty);
//            e.Graphics.DrawImage(bp, panel4.Location);
//            //移动容器
//            if (rect != Rectangle.Empty)
//            {
//                if (isDrag)
//                {//画一个和Control一样大小的黑框
//                    e.Graphics.DrawRectangle(Pens.Black, rect);
//                }
//                else
//                {
//                    e.Graphics.DrawRectangle(new Pen(this.BackColor), rect);
//                }
//            }

//        }

//        private void panel4_MouseEnter(object sender, EventArgs e)
//        {

//            this.panel4.Focus();
//        }

//        private void panel4_MouseLeave(object sender, EventArgs e)
//        {

//        }


//        bool mDown = false;
//        object src;
//        object des;
//        bool startPaint = false;
//        int location = 0;//0在form，1在src，2在des
//        int count = 0;//只有count=1，才启动画新线
//        void panel_MouseClick(object sender, MouseEventArgs e)
//        {
//            if (mDown == false)
//            {//起始panel
//                mDown = true;
//                location = 1;
//                src = sender;
//                count = 1;
//            }
//            else
//            {//结束panel
//                if (location == 2 && startPaint)
//                {

//                    drawingLine.desg = (GroupBox)((Panel)sender).Parent;
//                    //
//                    drawingLine.endPointtoSender.X = this.PointToClient(Control.MousePosition).X - drawingLine.desg.Location.X;
//                    drawingLine.endPointtoSender.Y = this.PointToClient(Control.MousePosition).Y - drawingLine.desg.Location.Y;
//                    //
//                    label6.Text = drawingLine.desg.ToString();
//                    drawPanel_MouseUp((object)this, this.PointToClient(Control.MousePosition));

//                }
//                else
//                {
//                    if (drawingLine != null)
//                    {
//                        //清空之前的line
//                        drawingLine.StartPoint = Point.Empty;
//                        drawingLine.EndPoint = Point.Empty;
//                        drawingLine = null;
//                        lines.Remove(drawingLine);
//                    }
//                }

//                mDown = false;
//                startPaint = false;
//                location = 0;
//                count = 0;
//                src = null;
//                des = null;
//                label2.Text = "";
//                label3.Text = "";
//                label4.Text = "";
//                label5.Text = "";

//            }
//            this.Refresh();

//        }

//        void panel_MouseDown(object sender, MouseEventArgs e)
//        {
//            if (mDown == false)
//            {//起始panel
//                mDown = true;
//                location = 1;

//            }
//            else
//            {//结束panel
//                if (location == 2 && startPaint)
//                {
//                    //drawPanel_MouseUp((object)this, this.PointToClient(Control.MousePosition));
//                    drawPanel_MouseUp((object)panel4, this.PointToClient(Control.MousePosition));
//                }
//                mDown = false;
//                startPaint = false;
//                location = 0;
//            }


//        }

//        void panel_MouseUp(object sender, MouseEventArgs e)
//        {

//        }

//        void panel_MouseMove(object sender, MouseEventArgs e)
//        {//这里不用做功能
//            if (drawingLine != null)
//            {
//                if (mDown && location == 1)
//                {
//                    label1.Text = "在src里徘徊，";
//                    //label1.Text = sender.ToString()+"\n"+"开始画了"+getPointToForm((Control)sender, e.Location).ToString();
//                }
//                if (startPaint && location == 2)
//                {
//                    //if (drawingLine != null)
//                    //{
//                    label3.Text = "进入了des正在画endPoint" + e.Location.ToString();
//                    drawingLine.EndPoint = this.getPointToForm((Control)sender, e.Location);

//                    //drawPanel.Invalidate();

//                    //splitContainer1.Panel1.Invalidate();
//                    //}
//                }
//                this.Invalidate();
//                this.Refresh();
//            }
//        }

//        private void panel_MouseEnter(object sender, EventArgs e)
//        {
//            des = sender;

//            if (startPaint && src != des)
//            {

//                label5.Text = "进入了des" + sender.ToString();
//                location = 2;
//                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
//            }
//            else if (startPaint && src == des)
//            {
//                count++;
//                label5.Text = "回到了src" + sender.ToString();
//                location = 1;
//            }
//        }

//        private void panel_MouseLeave(object sender, EventArgs e)
//        {
//            if (mDown && sender == src && count == 1)
//            { //开始化线，其实坐标为鼠标当前坐标
//                label4.Text = "离开src" + sender.ToString();
//                location = 0;
//                startPaint = true;
//                //drawPanel_MouseDown((object)this, this.PointToClient(Control.MousePosition));
//                drawPanel_MouseDown((object)panel4, this.PointToClient(Control.MousePosition));
//                drawingLine.srcg = (GroupBox)((Panel)sender).Parent;
//                //
//                drawingLine.startPointtoSender.X = this.PointToClient(Control.MousePosition).X - drawingLine.srcg.Location.X;
//                drawingLine.startPointtoSender.Y = this.PointToClient(Control.MousePosition).Y - drawingLine.srcg.Location.Y;
//                //
//                label6.Text = drawingLine.srcg.ToString();
//            }
//            if (startPaint && location == 2)
//            {
//                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
//                location = 0;

//            }
//            if (startPaint && count > 1)
//            {
//                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
//                location = 0;
//            }
//        }


//        #region 划线，移动

//        #region 定义线元素
//        class Line
//        {
//            public Point StartPoint = Point.Empty;
//            public Point EndPoint = Point.Empty;
//            //加附点以便在画面整体移动时定位
//            public Point startPointAdd = Point.Empty;
//            public Point endPointAdd = Point.Empty;
//            public GroupBox srcg;
//            public Point startPointtoSender = Point.Empty;
//            public Point endPointtoSender = Point.Empty;
//            public GroupBox desg;
//            public Line(Point startPoint)
//            {
//                StartPoint = startPoint;
//                startPointAdd = startPoint;//给附加坐标赋值，随之变化
//                EndPoint = startPoint;
//            }

//        }

//        /// <summary>
//        /// 用来确定鼠标是不是在已有的线上
//        /// </summary>
//        bool inLine = false;
//        /// <summary>
//        /// 移动起始的点，用来计算新的坐标
//        /// </summary>
//        private Point moveStart = Point.Empty;
//        /// <summary>
//        /// 作为不变的起始线来计算移动线的坐标
//        /// </summary>
//        private Line tempLine = null;
//        /// <summary>
//        /// 可以被移动的正选中的线
//        /// </summary>
//        private Line moveLine = null;
//        /// <summary>
//        /// 用于保存绘出线条的集合
//        /// </summary>
//        private List<Line> lines = new List<Line>();
//        /// <summary>
//        /// 用于保存当前正在绘制的线条
//        /// </summary>
//        private Line drawingLine = null;
//        /// <summary>
//        /// 用于显示绘图的面板组件
//        /// </summary>
//        //private DrawPanel drawPanel = new DrawPanel();
//        //private LineControl drawPanel = new LineControl();
//        #endregion


//        #region 变身为化线等坐标的处理函数
//        /// <summary>
//        /// 在绘图区释放鼠标，结束当前线条绘制
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        void drawPanel_MouseUp(object sender, Point e)
//        {
//            if (drawingLine == null && inLine)
//            {

//                inLine = false;
//                moveLine = null; tempLine = null;
//            }
//            if (drawingLine == null) return;
//            if (e == drawingLine.StartPoint)
//            {
//                drawingLine.StartPoint = Point.Empty;
//                drawingLine.EndPoint = Point.Empty;
//                drawingLine = null;
//                lines.Remove(drawingLine);
//            }
//            else
//            {
//                drawingLine.EndPoint = e;

//                drawingLine = null;
//            }

//            if (lines.Count > 0)
//            {
//                listBox1.Items.Clear();
//                foreach (Line l in lines)
//                {
//                    listBox1.Items.Add(l.StartPoint.ToString() + "," + l.EndPoint.ToString());
//                }
//            }

//            mDown = false;
//        }
//        /// <summary>
//        /// 在绘图区按下鼠标，开始绘制新线条
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        void drawPanel_MouseDown(object sender, Point e)
//        {
//            //int x=e.Location.X;
//            //int y=e.Location.Y;
//            int x = e.X;
//            int y = e.Y;
//            foreach (Line l in lines)
//            {
//                if (l.StartPoint.X == l.EndPoint.X)
//                { //线是水平线的话，x的横坐标在不在两个断电之间
//                    if (x == l.StartPoint.X)
//                    {
//                        if (isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
//                        {
//                            inLine = true;
//                            tempLine = new Line(l.StartPoint);
//                            tempLine.EndPoint = l.EndPoint;
//                            moveLine = l; lines.Remove(l); lines.Add(moveLine);
//                            break;
//                        }
//                        else
//                        {
//                            inLine = false;
//                            continue;
//                        }
//                    }

//                }
//                else if (l.StartPoint.Y == l.EndPoint.Y)
//                {//线是垂直线
//                    if (y == l.StartPoint.Y)
//                    {
//                        if (isBetween(l.StartPoint.X, l.EndPoint.X, x))
//                        {
//                            inLine = true; moveLine = l; tempLine = new Line(l.StartPoint);
//                            tempLine.EndPoint = l.EndPoint; lines.Remove(l); lines.Add(moveLine); break;
//                        }
//                        else
//                        {
//                            inLine = false;
//                            continue;//这条线已经没有再判断的必要了
//                        }
//                    }
//                }

//                else if ((x == l.StartPoint.X && y == l.StartPoint.Y) || (x == l.EndPoint.X && y == l.EndPoint.Y))
//                {//点在线的两端点上
//                    inLine = true;
//                    moveLine = l;
//                    tempLine = new Line(l.StartPoint);//防止引用fuzhi是地址
//                    tempLine.EndPoint = l.EndPoint;
//                    lines.Remove(l);
//                    lines.Add(moveLine);
//                    break;
//                }
//                //else if ((l.EndPoint.Y - l.StartPoint.Y) / (l.EndPoint.X - l.StartPoint.X) == (y - l.StartPoint.Y) / (x - l.StartPoint.X) && isBetween(l.StartPoint.X, l.EndPoint.X, x) && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
//                else if ((l.EndPoint.Y - l.StartPoint.Y) * (x - l.StartPoint.X) == (y - l.StartPoint.Y) * (l.EndPoint.X - l.StartPoint.X) && isBetween(l.StartPoint.X, l.EndPoint.X, x) && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
//                {
//                    inLine = true; moveLine = l; tempLine = new Line(l.StartPoint);
//                    tempLine.EndPoint = l.EndPoint; lines.Remove(l); lines.Add(moveLine);
//                    break;
//                }
//                else
//                {
//                    inLine = false;

//                }
//            }
//            if (inLine)//不在已有的线上
//            {
//                moveStart = e;

//            }
//            else
//            {

//                label2.Text = "startPoint" + e.ToString();
//                drawingLine = new Line(e);

//                lines.Add(drawingLine);

//            }


//        }

//        #endregion

//        private bool isBetween(int x, int y, int z)
//        {
//            if (x >= y)
//            {
//                if (z <= x && z >= y) return true;
//                return false;
//            }
//            else
//            {
//                if (z <= y && z >= x) return true;
//                return false;
//            }
//        }

//        #endregion

//        #region controlevent 移动控件
//        //鼠标按下坐标（control控件的相对坐标）
//        Point mouseDownPoint = Point.Empty;
//        //显示拖动效果的矩形
//        Rectangle rect = Rectangle.Empty;
//        //是否正在拖拽
//        bool isDrag = false;

//        void control_MouseDown(object sender, MouseEventArgs e)
//        {
//            if (e.Button == MouseButtons.Left)
//            {
//                mouseDownPoint = e.Location;
//                //记录控件的大小
//                GroupBox g = (GroupBox)sender;
//                //g.Visible = false;
//                rect = g.Bounds;

//            }
//        }
//        void control_MouseMove(object sender, MouseEventArgs e)
//        {
//            label1.Text = sender.ToString() + "\n" + getPointToForm((Control)sender, e.Location).ToString();
//            if (e.Button == MouseButtons.Left)
//            {
//                isDrag = true;
//                //重新设置rect的位置，跟随鼠标移动
//                rect.Location = getPointToForm((Control)sender, new Point(e.Location.X - mouseDownPoint.X, e.Location.Y - mouseDownPoint.Y));

//                //设置线条的跟随变化
//                GroupBox g = (GroupBox)sender;
//                g.Location = rect.Location;
//                g.Visible = false;
//                foreach (Line l in lines)
//                {
//                    if (l.srcg == g)
//                    {
//                        l.StartPoint.X = g.Location.X + l.startPointtoSender.X;
//                        l.StartPoint.Y = g.Location.Y + l.startPointtoSender.Y;
//                    }
//                    if (l.desg == g)
//                    {
//                        l.EndPoint.X = g.Location.X + l.endPointtoSender.X;
//                        l.EndPoint.Y = g.Location.Y + l.endPointtoSender.Y;
//                    }
//                }


//                ////////////////////////
//                this.Refresh();

//            }
//        }
//        void control_MouseUp(object sender, MouseEventArgs e)
//        {
//            if (e.Button == MouseButtons.Left)
//            {
//                if (isDrag)
//                {
//                    GroupBox g = (GroupBox)sender;

//                    isDrag = false;
//                    //移动control到放开鼠标的地方
//                    g.Location = rect.Location;
//                    g.Visible = true;
//                    this.Invalidate();
//                    this.Refresh();
//                }
//                reset();
//            }
//        }
//        //重置变量
//        private void reset()
//        {
//            mouseDownPoint = Point.Empty;
//            rect = Rectangle.Empty;
//            isDrag = false;
//        }
//        //窗体重绘,该部分移动至画线那里,如果单独需的话把这部分加入控件事件里
//        //private void FormDrag_Paint(object sender, PaintEventArgs e)
//        //{
//        //    if (rect != Rectangle.Empty)
//        //    {
//        //        if (isDrag)
//        //        {//画一个和Control一样大小的黑框
//        //            e.Graphics.DrawRectangle(Pens.Black, rect);
//        //        }
//        //        else
//        //        {
//        //            e.Graphics.DrawRectangle(new Pen(this.BackColor), rect);
//        //        }
//        //    }
//        //}
//        //把相对与control控件的坐标，转换成相对于窗体的坐标。


//        #endregion

//        private Point getPointToForm(Control control, Point p)
//        {

//            return this.PointToClient(control.PointToScreen(p));
//        }

//        private void Form4_SizeChanged(object sender, EventArgs e)
//        {
//            panel4.Height = this.ClientSize.Height - panel1.Height;
//        }

//        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
//        {//保存数据
//            using (StreamWriter sw = new StreamWriter(@"Line.lst", false, Encoding.UTF8))
//            {
//                foreach (Line l in lines)
//                {
//                    sw.WriteLine(l.StartPoint.ToString() + "\b" + l.EndPoint.ToString() + "\b" + l.startPointtoSender.ToString() + "\b" + l.endPointtoSender.ToString() + "\b" + l.srcg.Name.ToString() + "\b" + l.desg.Name.ToString());
//                }
//            }
//            using (StreamWriter sw = new StreamWriter(@"Control.lst", false, Encoding.UTF8))
//            {
//                foreach (GroupBox grp in panel4.Controls)
//                {
//                    sw.WriteLine(grp.Name + "\b" + grp.Location);
//                }
//            }
//        }

//        private void loadData()
//        {
//            if (File.Exists(@"Control.lst"))
//            {
//                using (StreamReader sr = new StreamReader(@"Control.lst", Encoding.UTF8))
//                {
//                    string l = "";
//                    while ((l = sr.ReadLine()) != null)
//                    {
//                        string[] ls = l.Split('\b');
//                        GroupBox g = new GroupBox();
//                        g.Location = getPoint(ls[1]);
//                        addNewGroupBox(g, ls[0]);
//                    }
//                }
//            }
//            if (File.Exists(@"Line.lst"))
//            {
//                using (StreamReader sr = new StreamReader(@"Line.lst", Encoding.UTF8))
//                {
//                    string l = "";
//                    while ((l = sr.ReadLine()) != null)
//                    {
//                        string[] ls = l.Split('\b');
//                        Line line = new Line(getPoint(ls[0]));
//                        line.EndPoint = getPoint(ls[1]);
//                        line.startPointtoSender = getPoint(ls[2]);
//                        line.endPointtoSender = getPoint(ls[3]);
//                        foreach (GroupBox g in panel4.Controls)
//                        {
//                            if (g.Name == ls[4])
//                            {
//                                line.srcg = g;
//                            }
//                            if (g.Name == ls[5])
//                            {
//                                line.desg = g;
//                            }
//                        }
//                        lines.Add(line);
//                    }
//                }
//            }


//        }
//        private Point getPoint(string x)
//        {
//            x = x.TrimStart('{').TrimEnd('}');
//            string[] xa = x.Split(',');
//            return new Point(int.Parse(xa[0].TrimStart("X=".ToCharArray())), int.Parse(xa[1].TrimStart("Y=".ToCharArray())));
//        }
//        private void addNewGroupBox(GroupBox grp, string name)
//        {
//            DataTable tblFields = sqlconn.cn_Sql.GetSchema(SqlClientMetaDataCollectionNames.Columns);
//            //GroupBox grp = new GroupBox();
//            grp.Text = name;
//            grp.Name = name;
//            grp.Width = 150;
//            grp.Height = 300;
//            grp.MouseDown += new MouseEventHandler(control_MouseDown);
//            grp.MouseMove += new MouseEventHandler(control_MouseMove);
//            grp.MouseUp += new MouseEventHandler(control_MouseUp);
//            Panel panel = new Panel();

//            panel.MouseEnter += new EventHandler(panel_MouseEnter);
//            panel.MouseLeave += new EventHandler(panel_MouseLeave);
//            panel.MouseMove += new MouseEventHandler(panel_MouseMove);
//            panel.MouseClick += new MouseEventHandler(panel_MouseClick);
//            ListView lv = new ListView();
//            lv.View = View.Details;
//            lv.FullRowSelect = true;
//            lv.GridLines = true;
//            lv.Scrollable = true;
//            lv.MultiSelect = false;
//            lv.HoverSelection = true;

//            #region add data
//            lv.Columns.Add("字段名");
//            lv.Columns.Add("字段类型");
//            //lv.Columns.Add("列标题3", 100, HorizontalAlignment.Left);
//            //add list
//            lv.BeginUpdate();
//            //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
//            for (int i = 0; i < tblFields.Rows.Count; i++)
//            {
//                if (tblFields.Rows[i]["Table_Name"].ToString() == name)
//                {
//                    ListViewItem lvi = new ListViewItem();
//                    // lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标

//                    lvi.Text = tblFields.Rows[i]["Column_Name"].ToString();
//                    lvi.SubItems.Add(tblFields.Rows[i]["DATA_TYPE"].ToString());
//                    lv.Items.Add(lvi);

//                }
//            }
//            lv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
//            #endregion
//            panel4.Controls.Add(grp);
//            grp.Controls.Add(panel);
//            panel.Dock = DockStyle.Fill;
//            panel.Controls.Add(lv);
//            lv.Width = panel.Width - 30;
//            lv.Height = panel.Height - 30;
//            lv.Location = new Point(15, 15);
//        }


//    }
//}
