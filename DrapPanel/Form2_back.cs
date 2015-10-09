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
    public partial class Form2_back : Form
    {
        public Form2_back()
        {
            //drawPanel.BackColor = Color.White;
            //drawPanel.Cursor = Cursors.Cross;
            //drawPanel.Dock = DockStyle.Fill;
            //drawPanel.MouseDown += new MouseEventHandler(drawPanel_MouseDown);
            //drawPanel.MouseUp += new MouseEventHandler(drawPanel_MouseUp);
            //drawPanel.MouseMove += new MouseEventHandler(drawPanel_MouseMove);
            //drawPanel.Paint += new PaintEventHandler(drawPanel_Paint);
            //Controls.Add(drawPanel);
            
           
            InitializeComponent();
        }
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
        private void Form2_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;//避免绘制时闪烁
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            this.MouseDown += new MouseEventHandler(drawPanel_MouseDown);
            this.MouseUp += new MouseEventHandler(drawPanel_MouseUp);
            this.MouseMove += new MouseEventHandler(drawPanel_MouseMove);
            this.Paint += new PaintEventHandler(drawPanel_Paint);

            this.MouseWheel += new MouseEventHandler(Form2_back_MouseWheel);
           
        }

        void Form2_back_MouseWheel(object sender, MouseEventArgs e)
        {
            int Mo = 0;
            if (e.Delta > 0) Mo = 8;
            else Mo = -8;
      
       

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
        void drawPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawingLine == null && inLine) {
                inLine = false;
                moveLine = null; tempLine = null;
            }
            if(drawingLine==null)return;
            if (e.Location == drawingLine.StartPoint)
            {
                drawingLine = null;
                lines.Remove(drawingLine);
            }
            else
            {
                drawingLine.EndPoint = e.Location;
                
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
        void drawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            int x=e.Location.X;
            int y=e.Location.Y;
           
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
                moveStart = e.Location;
               

            }
            else
            {
                label2.Text = "outLine";
            drawingLine = new Line(e.Location);
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
            if (drawingLine != null)
            {
                drawingLine.EndPoint = e.Location;
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
    }
}
