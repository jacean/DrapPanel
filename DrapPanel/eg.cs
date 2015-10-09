//以下是完整代码，可以直接编译运行
//-------------------------------------------
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
namespace DrapPanel
{
    
    /// <summary>
    /// 线条对象
    /// </summary>
    class Line
    {
        /// <summary>
        /// 建立线条对象，并设置起点
        /// </summary>
        /// <param name="startPoint">此线条的起点</param>
        public Line(Point startPoint)
        {
            StartPoint = startPoint;
            EndPoint = startPoint;
        }
        public Point StartPoint = Point.Empty;
        public Point EndPoint = Point.Empty;
    }
    public class DrawPanel : Control
    {
        public DrawPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }
    }
    /// <summary>
    /// 窗口定义
    /// </summary>
    public class Form1 : Form
    {
        public Form1()
        {
            drawPanel.BackColor = Color.White;
            drawPanel.Cursor = Cursors.Cross;
            drawPanel.Dock = DockStyle.Fill;
            drawPanel.MouseDown += new MouseEventHandler(drawPanel_MouseDown);
            drawPanel.MouseUp += new MouseEventHandler(drawPanel_MouseUp);
            drawPanel.MouseMove += new MouseEventHandler(drawPanel_MouseMove);
            drawPanel.Paint += new PaintEventHandler(drawPanel_Paint);
            Controls.Add(drawPanel);
        }
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
        private DrawPanel drawPanel = new DrawPanel();
        /// <summary>
        /// 在绘图区释放鼠标，结束当前线条绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drawPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawingLine == null) return;
            drawingLine.EndPoint = e.Location;
            drawingLine = null;
        }
        /// <summary>
        /// 在绘图区按下鼠标，开始绘制新线条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            drawingLine = new Line(e.Location);
            lines.Add(drawingLine);
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
                drawPanel.Invalidate();
            }
        }
        /// <summary>
        /// 绘制效果到面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void drawPanel_Paint(object sender, PaintEventArgs e)
        {
            Bitmap bp = new Bitmap(drawPanel.Width, drawPanel.Height); // 用于缓冲输出的位图对象
            Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 消锯齿（可选项）
            Pen p = new Pen(Color.Black);
            foreach (Line line in lines)
            {
                if (line == drawingLine)
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
    }
}
