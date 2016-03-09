﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
namespace DrapPanel
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
        public MainForm(string name,string sqltxt)
        {
            InitializeComponent();
            this.sqltxt = sqltxt;
            textBox1.Text = sqltxt;
            this.Text = name;
            tableName = name;
           
        }
        public string sqltxt = "";
        public string tableName = "";
        sqlConn sqlconn = new sqlConn();
        bool loadEnd = false;
        public ContextMenuStrip rM = new ContextMenuStrip();
        public static int LV_DEFAULT_ROWHEIGHT = 25;        
        public static int LV_TO_GRP_DEFAULT_LEFT = 10;        
        public static int LV_TO_GRP_DEFAULT_TOP = 10;        
        public static int GRP_DEFAULT_WIDTH = 150;        
        public static int GRP_DEFAULT_HEIGHT = 300;

        public static int LV_NOW_ROWHEIGHT = LV_DEFAULT_ROWHEIGHT;
        //public static int LV_TO_GRP_NOW_LEFT = LV_TO_GRP_DEFAULT_LEFT;
        //public static int LV_TO_GRP_NOW_TOP = LV_TO_GRP_DEFAULT_TOP;
        public static int GRP_NOW_WIDTH = GRP_DEFAULT_WIDTH;
        public static int GRP_NOW_HEIGHT = GRP_DEFAULT_HEIGHT;

        public static int MIN_GRP_WIDTH = 60;
        public static int MIN_GRP_HEIGHT = 60;
        public static float MOUSE_WHEEL_UP = 1.02f;
        public static float MOUSE_WHEEL_DOWN = 0.98f;
        public enum Direct : int { isSrc = 0, isDes = 1, isBoth = 2 };
        public enum PointStatus : int { InForm = 0, InSrc = 1, InDes = 2 };


        private void Form4_Load(object sender, EventArgs e)
        {
            this.Text += "  waiting...";
            this.Parent.Text = this.Text;
            label1.Text = "";
            label11.Text = "";
            label12.Text = "";
            panel4.Paint += new PaintEventHandler(panel4_Paint);
            panel4.MouseMove += new MouseEventHandler(panel4_MouseMove);
            panel4.MouseDown += new MouseEventHandler(panel4_MouseDown);
            panel4.MouseUp += new MouseEventHandler(panel4_MouseUp);
            panel4.MouseWheel += new MouseEventHandler(panel4_MouseWheel);



            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
           
            
           
            
            panel4.Height=this.ClientSize.Height-panel1.Height;

            if (sqltxt != "")
            {
              
                button5_Click(null, null);

                for (int i = 0; i < cb1.Items.Count; i++)
                {
                    if (cb1.Items[i].ToString() == tableName.Split(new[] { "__" }, StringSplitOptions.None)[0]) cb1.SelectedIndex = i;
                }
                for (int i = 0; i < cb2.Items.Count; i++)
                {
                    if (cb2.Items[i].ToString() == tableName.Split(new[] { "__" }, StringSplitOptions.None)[1]) cb2.SelectedIndex = i;
                }
                //cb1.SelectedText = tableName.Split(new[] { "__" }, StringSplitOptions.None)[0];
                //cb2.SelectedText = tableName.Split(new[] { "__" }, StringSplitOptions.None)[1];
                //foreach (GroupBox gr in panel4.Controls.OfType<GroupBox>())
                //{
                //    if (gr.Name == "table1") cb1.SelectedText = gr.Text;
                //    if (gr.Name == "table2") cb2.Text = gr.Text;
                //}

            }
            updateListBox();
            updateLines();
        }   

   
        void panel4_MouseUp(object sender, MouseEventArgs e)
        {
           
            updateListBox();
            if (isMoveForm)
            {
                isMoveForm = false;
            }
        }

        void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;

            isInline(x,y);

            if (e.Button == MouseButtons.Left)
            {
                if (inLine && drawingLine == null)//在已有的线上
                {//已屏蔽选中线条移动事件
                    if (isDelete)
                    {
                        lines.Remove(selectedLine);
                        //在数据库里删除线
                        function.deleteLine(selectedLine,tableName);
                        this.Invalidate();
                        this.Refresh();
                    }
                    else
                    {//弹出窗体

                        string showText = "";
                        showText = "起点:" + selectedLine.srcg.Text + "->";

                        foreach (Control cp in selectedLine.srcg.Controls)
                        {
                            foreach (ListView lv in cp.Controls)
                            {
                                showText += lv.Items[selectedLine.srcg_itemIndex].Text + "\n";
                            }
                        }
                        showText += "终点:" + selectedLine.desg.Text + "->";
                        foreach (Control cp in selectedLine.desg.Controls)
                        {
                            foreach (ListView lv in cp.Controls)
                            {
                                showText += lv.Items[selectedLine.desg_itemIndex].Text + "\n";
                            }
                        }

                    }
                }
                else
                {//画面整体移动//怎么才能把控件画在画布上，这样调整画布的起始坐标就好了嘛
                    isMoveForm = true;
                    movestartPoint = e.Location;
                    pointList.Clear();
                    foreach (GroupBox gr in panel4.Controls.OfType<GroupBox>())
                    {
                        pointList.Add(gr.Location);
                    }

                }
            }
            if (e.Button == MouseButtons.Right)
            {
                if (inLine)
                {
                     string showText = "选中线:\n";
                        showText += "起点:" + selectedLine.srcg.Text + "->";

                        foreach (Control cp in selectedLine.srcg.Controls)
                        {
                            foreach (ListView lv in cp.Controls)
                            {
                                showText += lv.Items[selectedLine.srcg_itemIndex].Text + "\n";
                            }
                        }
                        showText += "终点:" + selectedLine.desg.Text + "->";
                        foreach (Control cp in selectedLine.desg.Controls)
                        {
                            foreach (ListView lv in cp.Controls)
                            {
                                showText += lv.Items[selectedLine.desg_itemIndex].Text;
                            }
                        }
                    rM.Show(panel4, e.Location);
                    rM.ShowImageMargin = false;
                    rM.Items.Clear();
                    rM.Items.Add(showText);
                    rM.Items.Add("颜色");
                    rM.Items.Add("隐藏");         
                    rM.Items[0].Enabled = false;
                    rM.Items[1].Click += new EventHandler(item1_Click);
                    rM.Items[2].Click+=new EventHandler(item2_Click);
                    
                }
            }
            updateLines();
            inLine = false;
        }

        void tagTxt_KeyDown(object sender, KeyEventArgs e)
        {

        }
        private void tagTxt_Leave(object sender, EventArgs e)
        {
            if (selectedLine == null) return;
            selectedLine.tag = tagTxt.Text;
            function.updateTag(selectedLine,tableName);
        }
        

        void item1_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            
            if (cd.ShowDialog() == DialogResult.OK)
            {
                selectedLine.color = cd.Color;
                selectedLine = null;
            }
          
            this.Refresh();
        }
        void item2_Click(object sender, EventArgs e)
        {
            selectedLine.isShow = false;
            selectedLine = null;
            this.Refresh();
        }
        List<Point> pointList = new List<Point>();
        public bool isMoveForm= false;//是否在拖动画面
        public Point movestartPoint = Point.Empty;
        

        /// <summary>
        /// 在绘图区移动鼠标时，如果正在绘制新线条，就更新绘制面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void panel4_MouseMove(object sender, MouseEventArgs e)
        {

            int x = e.Location.X;
            int y = e.Location.Y;

            isInline(x, y);

            label1.Text = "在panle4中移动"+getPointToForm().ToString();
           
            if (startPaint && location == 0)
            {                
                if (drawingLine != null)
                {                   
                    drawingLine.EndPoint = e.Location;              
                    this.Invalidate();                 
                    this.Refresh();
                }
            }


            if (isMoveForm)
            {
                //移动容器,依靠的是系统对groupbox的遍历是一致的顺序
                int i = 0;
                foreach (GroupBox grp in panel4.Controls.OfType<GroupBox>())
                {

                    grp.Left = pointList[i].X-movestartPoint.X+e.X;
                    grp.Top = pointList[i++].Y-movestartPoint.Y+e.Y;
                }
                updateLines();                
            }

          
        }

        void panel4_MouseWheel(object sender, MouseEventArgs e)
        {
            if (panel4.Focus())
            {
                if (drawingLine != null)
                {
                    return;
                }
                float Mo = 0;

                if (e.Delta > 0)
                {
                    Mo = MOUSE_WHEEL_UP;


                }
                else if (e.Delta < 0)
                {
                    Mo = MOUSE_WHEEL_DOWN;
                }
                #region 移动控件
                foreach (Control ct in this.panel4.Controls.OfType<GroupBox>())
                {//看成是点的移动

                    if (ct.Width < MIN_GRP_WIDTH | ct.Height< MIN_GRP_HEIGHT)
                    {
                        if (Mo < 1)
                            return;
                    }

                    ct.Width += (int)((Mo - 1) * (float)ct.Width);                    
                    ct.Height += (int)((Mo - 1) * (float)ct.Height);                    
                    ct.Left += (int)((float)(ct.Left - e.X) * (Mo - 1));                    
                    ct.Top += (int)((float)(ct.Top - e.Y) * (Mo - 1));

                    GRP_NOW_WIDTH = ct.Width;
                    GRP_NOW_HEIGHT = ct.Height;
                    
                    /////////
                    foreach (Control cp in ct.Controls)
                    {
                        foreach (Control cl in cp.Controls)
                        {
                            cl.Width = cp.Width - 2*LV_TO_GRP_DEFAULT_LEFT;
                            cl.Height = cp.Height - 2*LV_TO_GRP_DEFAULT_TOP;
                            cl.Location = new Point(LV_TO_GRP_DEFAULT_LEFT, LV_TO_GRP_DEFAULT_TOP);

                            LV_NOW_ROWHEIGHT += (int)((float)LV_NOW_ROWHEIGHT * (Mo - 1));
                            ImageList imgList = new ImageList();
                            imgList.ImageSize = new Size(1,LV_NOW_ROWHEIGHT);
                           (cl as ListView).SmallImageList = imgList;
                           (cl as ListView).Columns[0].Width = cl.Width / 2;
                           (cl as ListView).Columns[1].Width = cl.Width / 2;                           
                        }

                    }

                }

                #endregion
                updateLines();
            }
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
            System.Drawing.Drawing2D.AdjustableArrowCap lineCap = new System.Drawing.Drawing2D.AdjustableArrowCap(6, 6, true);
            p.CustomEndCap = lineCap;
            foreach (Line line in lines)
            {
                if (line.src_isShow && line.des_isShow&&line.isShow)
                {
                    if (line == drawingLine||(selectedLine!=null&&line==selectedLine))
                    {
                        p.Color = Color.Blue;

                    }
                    else
                    {
                        p.Color = line.color;
                    }
                    //通过两个表的位置来决定画在左边还是右边,注意正在画的线是没有desg的\
                    if (line != drawingLine)
                    {

                        if (line.srcg.Location.X > line.desg.Location.X)
                        {
                            line.StartPoint.X = line.srcg.Location.X;
                            line.EndPoint.X = line.desg.Location.X + line.desg.Width;
                        }
                        else
                        {
                            line.StartPoint.X = line.srcg.Location.X + line.srcg.Width;
                            line.EndPoint.X = line.desg.Location.X;
                        }
                    }
                    if (line.direct == (int)Direct.isBoth)
                    {
                        p.CustomStartCap = lineCap;
                    }
                    else
                    {
                   
                        p.StartCap = System.Drawing.Drawing2D.LineCap.NoAnchor; }
                g.DrawLine(p, line.StartPoint, line.EndPoint);
                }
            }
            // 将缓冲位图绘制到输出
            //e.Graphics.DrawImage(bp, Point.Empty);
            e.Graphics.DrawImage(bp,new Point(0,0));//设置画布坐标,相对于panel4的
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

            updateListBox();


        }

        private void panel4_MouseEnter(object sender, EventArgs e)
        {

            this.panel4.Focus();
        }


        Rectangle rec = Rectangle.Empty;

        bool mDown = false;
        object src;
        object des;
        bool startPaint = false;
        int location = 0;//0在form，1在src，2在des
        int count = 0;//只有count=1，才启动画新线
       
        void panel_MouseClick(object sender, MouseEventArgs e)
        {

            //////////////////////判断是否选中了项///////////
                Point itemPoint = Point.Empty;
                Point itemNextPoint = Point.Empty;
                int selectedIndex = -1;
                int height = 0;
                int dsX = 0;//用来判断在左还是右
                GroupBox gg = (GroupBox)(((Panel)sender).Parent);
                if ( getPointToForm().X > gg.Location.X + gg.Width / 2)
                {
                    dsX = gg.Width;
                }
                label3.Text = gg.Location.ToString();
                foreach (ListView lv in ((Panel)sender).Controls)
                {
                     
                    for (int i = 0; i < lv.Items.Count; i++)
                    {
                        itemPoint = getItemLocation(lv, i);
                        height = lv.GetItemRect(i).Height;

                        if ((gg.PointToClient(Control.MousePosition)).Y > itemPoint.Y && (gg.PointToClient(Control.MousePosition)).Y < itemPoint.Y+height)
                        {
                            selectedIndex = i;
                            lv.Items[i].Selected = true;
                            break;
                        }
                      
                    }
                   
                }
                //如果所选点并没有选项则不开始画点或结束
                if (selectedIndex != -1)
                {
                   
                    if (mDown == false)
                    {//起始panel
                        mDown = true;
                        src = sender;
                        location = (int)PointStatus.InSrc;
                        startPaint = true;

                        label3.Text=(new Point(gg.Location.X + dsX, itemPoint.Y + height / 2 + gg.Location.Y)).ToString();
                        startDrawingFunc(new Point(gg.Location.X+dsX, itemPoint.Y +height / 2+gg.Location.Y));
                        drawingLine.srcg = gg;
                        drawingLine.srcg_itemIndex = selectedIndex;
                        label11.Text ="选中开始项:" + drawingLine.srcg_itemIndex.ToString();
                       
                      
                    }
                    else
                    {//结束panel
                        if (location == (int)PointStatus.InDes && startPaint)
                        {
                            label12.Text = "结束点对应选中项为：" + selectedIndex.ToString();
                            //将end坐标设为鼠标纵坐标所属项的中间位置
                            drawingLine.desg = (GroupBox)(((Panel)sender).Parent);
                            drawingLine.desg_itemIndex = selectedIndex;
                            label3.Text = (new Point(gg.Location.X + dsX, itemPoint.Y + height / 2 + gg.Location.Y)).ToString();
                            endDrawingFunc(new Point(gg.Location.X+dsX, (itemPoint.Y + height / 2+gg.Location.Y)));//
                           
                           
                        }
                        else
                        {
                            if (drawingLine != null)
                            {
                                //清空之前的line
                                drawingLine.StartPoint = Point.Empty;
                                drawingLine.EndPoint = Point.Empty;
                                lines.Remove(drawingLine);
                                drawingLine = null;
                                
                            }
                        }

                        mDown = false;
                        startPaint = false;
                        location = (int)PointStatus.InForm;
          
                        src = null;
                        des = null;
                      
                    }
                    this.Refresh();
                }
            
        }


        void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawingLine != null)
            {
               
                if (startPaint && location == 2)
                {
                    drawingLine.EndPoint = getPointToForm();
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
                
             
                location = (int)PointStatus.InDes;
                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
            }
            if (startPaint && src == des)
            {
            
              
                location = (int)PointStatus.InSrc;
            }
        }

        private void panel_MouseLeave(object sender, EventArgs e)
        {
            if (mDown && location == (int)PointStatus.InSrc)
            {
                location = (int)PointStatus.InForm;
              
            }
            if (startPaint && location == (int)PointStatus.InDes)
            {
                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
                location = (int)PointStatus.InForm;

            }
            if (startPaint && count > 1)
            {
                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
                location = (int)PointStatus.InForm;
            }
        }
        

        #region 划线，移动

        #region 定义线元素       

        /// <summary>
        /// 用来确定鼠标是不是在已有的线上
        /// </summary>
        bool inLine = false;
        /// <summary>
        /// 移动起始的点，用来计算新的坐标
        /// </summary>
        private Point moveStart = Point.Empty;
        /// <summary>
        /// 可以被移动的正选中的线
        /// </summary>
        private Line selectedLine = null;
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
       
        #endregion


        #region 变身为化线等坐标的处理函数
        /// <summary>
        /// 在绘图区释放鼠标，结束当前线条绘制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void endDrawingFunc( Point e)
        {
            
            if (drawingLine == null) return;


            if (e == drawingLine.StartPoint)
            {//现在好像也没用了这段
                drawingLine.StartPoint = Point.Empty;
                drawingLine.EndPoint = Point.Empty;
                drawingLine = null;
                lines.Remove(drawingLine);
            }
            else
            {
                drawingLine.EndPoint = e;
                //把新线存入数据库
                function.saveLine(drawingLine,tableName);
                drawingLine = null;
               
            }

            updateListBox();

            mDown = false;
        }
        /// <summary>
        /// 在绘图区按下鼠标，开始绘制新线条
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void startDrawingFunc( Point e)
        {
            drawingLine = new Line(e);      
                lines.Add(drawingLine);
        }

        #endregion

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
                label3.Text ="中间点"+ mouseDownPoint.ToString();
                //记录控件的大小
                GroupBox g = (GroupBox)sender;
                //g.Visible = false;
                rect = g.Bounds;
            }
        }


        void control_MouseMove(object sender, MouseEventArgs e)
        {
           
            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
                //重新设置rect的位置，跟随鼠标移动
                Point p = getPointToForm();
                rect.Location = new Point(p.X - mouseDownPoint.X, p.Y - mouseDownPoint.Y);
                label3.Text = "中间点" + mouseDownPoint.ToString();
                //设置线条的跟随变化
                GroupBox g = (GroupBox)sender;
                g.Location = rect.Location;
                label1.Text = g.Location.ToString();
               
                
                updateLines();
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
     

        #endregion
        
        private Point getPointToForm()
        {
            return panel4.PointToClient(Control.MousePosition);
            //return this.PointToClient(control.PointToScreen(p));
        }

        private void Form4_SizeChanged(object sender, EventArgs e)
        {
            panel4.Height = this.ClientSize.Height - panel1.Height;
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {//保存数据
        
        }

        private void loadData()
        {
                    Point startPoint = Point.Empty;
                    Point endPoint = Point.Empty;
                    Point itemstartPoint = Point.Empty;
                    Point itemendPoint = Point.Empty;
                    int startHeight = 0;
                    int endHeight = 0;
                    GroupBox srcg = null;
                    GroupBox desg = null;                    
                    int index_src = -1;
                    int index_des = -1;
                    bool src_isShow=true;
                    bool des_isShow=true;
                    int direct = 0;
            foreach (string  l in function.readLine(tableName))
	        {
        		        string[] ls = l.Split('\b');
                        if (ls[6] == "0")
                        {
                            index_src = int.Parse(ls[2]);
                            index_des = int.Parse(ls[4]);
                            foreach (GroupBox g in panel4.Controls.OfType<GroupBox>())
                            {
                                if (g.Text == ls[1])
                                {
                                    srcg = g;
                                }
                                if (g.Text == ls[3])
                                {
                                    desg = g;
                                }
                            }
                            direct =(int)Direct.isSrc;
                        }
                        if (ls[6] == "1")
                        {
                            index_src = int.Parse(ls[4]);
                            index_des = int.Parse(ls[2]);
                            foreach (GroupBox g in panel4.Controls.OfType<GroupBox>())
                            {
                                if (g.Text == ls[3])
                                {
                                    srcg = g;
                                }
                                if (g.Text == ls[1])
                                {
                                    desg = g;
                                }
                            }
                            direct =(int)Direct.isDes;
                        }
                        if (ls[6] == "2")
                        {
                            index_src = int.Parse(ls[2]);
                            index_des = int.Parse(ls[4]);
                            foreach (GroupBox g in panel4.Controls.OfType<GroupBox>())
                            {
                                if (g.Text == ls[1])
                                {
                                    srcg = g;
                                }
                                if (g.Text == ls[3])
                                {
                                    desg = g;
                                }
                            }
                            direct = (int)Direct.isBoth;
                        }




                        if (srcg != null)
                        {
                            foreach (Control cp in srcg.Controls)
                            {
                                foreach (ListView lv in cp.Controls)
                                {

                                    itemstartPoint = getItemLocation(lv, index_src);
                                    startHeight = lv.GetItemRect(index_src).Height;
                                    if (itemstartPoint.Y < 5 + 14 + 16 || itemstartPoint.Y > srcg.Height - 5)
                                    {
                                        src_isShow = false;
                                    }
                                    else
                                    {
                                        src_isShow = true;
                                    }
                                    startPoint.Y = srcg.Location.Y + itemstartPoint.Y + startHeight / 2;

                                }

                            }
                        }

                        if (desg != null)
                        {
                            foreach (Control cp in desg.Controls)
                            {
                                foreach (ListView lv in cp.Controls)
                                {

                                    itemendPoint = getItemLocation(lv, index_des);
                                    endHeight = lv.GetItemRect(index_des).Height;
                                    if (itemendPoint.Y < 5 + 14 + 16 || itemendPoint.Y > desg.Height - 5)
                                    {
                                        des_isShow = false;
                                    }
                                    else
                                    {
                                        des_isShow = true;
                                    }

                                    endPoint.Y = desg.Location.Y + itemendPoint.Y + endHeight / 2;

                                }

                            }
                        }
                     
                        Line line = new Line(startPoint);
                        line.EndPoint = endPoint;
                        line.srcg = srcg;
                        line.desg = desg;                       
                        line.srcg_itemIndex = index_src;
                        line.desg_itemIndex = index_des;
                        line.src_isShow = src_isShow;
                        line.des_isShow = des_isShow;
                        line.direct = direct;
                        lines.Add(line);                    
            }
            this.Refresh();

        
        }
        private Point getPoint(string x)
        {
            x = x.TrimStart('{').TrimEnd('}');
            string[] xa = x.Split(',');
            return new Point(int.Parse(xa[0].TrimStart("X=".ToCharArray())),int.Parse(xa[1].TrimStart("Y=".ToCharArray())));
        }
        private void  addNewGroupBox(GroupBox grp,string name,string text,bool isLoad)
        {
            foreach (GroupBox g in panel4.Controls.OfType<GroupBox>())
            {
                if (g.Text == text)
                {
                    MessageBox.Show("该表已添加，请不要重复添加已造成混乱~");
                    return;
                }
            }

            DataTable tblFields = sqlconn.cn_Sql.GetSchema("Columns");
         
            grp.Text = text;
            grp.Name = name;
            //确定选择项新出现的位置
            if (name == "table1") grp.Location = new Point((int)(panel4.Width * 0.2),(int)(panel4.Height*0.2));
            else { grp.Location = new Point((int)(panel4.Width * 0.6), (int)(panel4.Height * 0.2)); }
            if (!isLoad)
            {
                grp.Width = GRP_NOW_WIDTH;
                grp.Height = GRP_NOW_HEIGHT;
            }
            grp.Margin = new Padding(0);
            grp.Padding = new Padding(0);
            grp.MouseDown += new MouseEventHandler(control_MouseDown);
            grp.MouseMove += new MouseEventHandler(control_MouseMove);
            grp.MouseUp += new MouseEventHandler(control_MouseUp);
            Panel panel = new Panel();

            panel.MouseEnter += new EventHandler(panel_MouseEnter);
            panel.MouseLeave += new EventHandler(panel_MouseLeave);
            panel.MouseMove += new MouseEventHandler(panel_MouseMove);
            panel.MouseClick += new MouseEventHandler(panel_MouseClick);
            //ListView lv = new ListView();
            myListview lv = new myListview();
            lv.View = View.Details;
            lv.FullRowSelect = true;
            lv.GridLines = true;
            lv.Scrollable = true;
            lv.MultiSelect = false;
            lv.HoverSelection = true;
            lv.Scroll += new ScrollEventHandler(lv_Scroll);
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, LV_NOW_ROWHEIGHT);
            lv.SmallImageList = imgList;
            //lv.CheckBoxes = true;            
            panel4.Controls.Add(grp);
            grp.Controls.Add(panel);
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.Transparent;
            panel.Margin = new Padding(0);
            panel.Padding = new Padding(0);
            panel.Controls.Add(lv);
            lv.Margin = new Padding(0);
            lv.Width = panel.Width - 2*LV_TO_GRP_DEFAULT_LEFT;
            lv.Height = panel.Height - 2*LV_TO_GRP_DEFAULT_TOP;
            lv.Location = new Point(LV_TO_GRP_DEFAULT_LEFT, LV_TO_GRP_DEFAULT_TOP);
            grp.BringToFront();

            #region add data
            lv.Columns.Add("字段名",lv.Width/2);//自适应列宽
            lv.Columns.Add("字段类型",lv.Width/2);
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            //lv.Columns.Add("列标题3", 100, HorizontalAlignment.Left);
            //add list
            lv.BeginUpdate();
            //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
            for (int i = 0; i < tblFields.Rows.Count; i++)
            {
                if (tblFields.Rows[i]["Table_Name"].ToString() == text)
                {
                    ListViewItem lvi = new ListViewItem();
                    // lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标
                    if (i % 2 == 0) lvi.BackColor = Color.PapayaWhip;
                    else lvi.BackColor = Color.Wheat;
                    lvi.Text = tblFields.Rows[i]["Column_Name"].ToString();
                    lvi.SubItems.Add(tblFields.Rows[i]["DATA_TYPE"].ToString());
                    lv.Items.Add(lvi);

                }
            }
            lv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            #endregion
        }

        void lv_Scroll(object sender, ScrollEventArgs e)
        {
            updateLines();
        }


        public bool isDelete = false;
        private void button2_Click(object sender, EventArgs e)
        {
            if (isDelete == false)
            {
                isDelete = true;
                button2.Text = "选中线条以删除";
            }
            else
            {
                isDelete = false;
                button2.Text = "delete(Line)";
            }
        }

       

        public Point getItemLocation(ListView sender,int index)
        {
            Rectangle vrec = sender.GetItemRect(index);//相对listview的坐标
            vrec.Offset(((ListView)sender).Location);//相对panel的坐标
            vrec.Offset((sender.Parent).Location);//相对groupBox的坐标
        
            return vrec.Location;
        }

        private void updateListBox()
        {
            if (lines.Count > 0&&drawingLine==null)
            {
                listBox1.Items.Clear();
                foreach (Line l in lines)
                {

                    string showText = "";
                    showText = "[起点:" + l.srcg.Text + ".";

                    foreach (Control cp in l.srcg.Controls)
                    {
                        foreach (ListView lv in cp.Controls)
                        {
                            showText += lv.Items[l.srcg_itemIndex].Text + "]+--+";
                        }
                    }
                    showText += "[终点:" + l.desg.Text + ".";
                    foreach (Control cp in l.desg.Controls)
                    {
                        foreach (ListView lv in cp.Controls)
                        {
                            showText += lv.Items[l.desg_itemIndex].Text + "]";
                        }
                    }

                    if (l.src_isShow && l.des_isShow)
                    {
                        if (!l.isShow)
                        {
                            listBox1.Items.Add("setHide-" + showText);
                        }
                        else 
                            listBox1.Items.Add("Show-" + showText);
                    }
                    else
                    {
                        listBox1.Items.Add("Hide-" +showText);
                    }
                    l.ID = listBox1.Items.Count - 1;
                }
                listBox1.SelectedIndex = index;
            }
        }

        private void updateLines()
        {

            

            #region 刷新线

            Point itemstartPoint = Point.Empty;
            Point itemendPoint = Point.Empty;
            int startHeight = 0;
            int endHeight = 0;
            GroupBox srcg = null;
            GroupBox desg = null;

            foreach (Line line in lines)
            {
                

                
                desg = (GroupBox)line.desg;
                srcg = (GroupBox)line.srcg;               
                
                foreach (Control cp in srcg.Controls)
                {
                    foreach (ListView lv in cp.Controls)
                    {

                        itemstartPoint = getItemLocation(lv, line.srcg_itemIndex);
                        startHeight = lv.GetItemRect(line.srcg_itemIndex).Height;
                        if (itemstartPoint.Y < LV_TO_GRP_DEFAULT_TOP + 14 + 16 || itemstartPoint.Y > srcg.Height - LV_TO_GRP_DEFAULT_TOP)
                        {//除去groupbox的空白部分的距离
                            line.src_isShow = false;
                        }
                        else
                        {
                            line.src_isShow = true;
                        }
                        line.StartPoint.Y = srcg.Location.Y + itemstartPoint.Y + startHeight / 2;

                    }

                }
                if (desg == null) return;

                foreach (Control cp in desg.Controls)
                {
                    foreach (ListView lv in cp.Controls)
                    {

                        itemendPoint = getItemLocation(lv, line.desg_itemIndex);
                        endHeight = lv.GetItemRect(line.desg_itemIndex).Height;
                        if (itemendPoint.Y < LV_TO_GRP_DEFAULT_TOP + 14 + 16 || itemendPoint.Y > desg.Height - LV_TO_GRP_DEFAULT_TOP)
                        {
                            line.des_isShow = false;
                        }
                        else
                        {
                            line.des_isShow = true;
                        }

                        line.EndPoint.Y = desg.Location.Y + itemendPoint.Y + endHeight / 2;

                    }

                }
            }

            #endregion
            this.Invalidate();
            this.Refresh();

        }

        private void isInline(int x,int y)
        {
            #region 判断鼠标是否选中线，如果选中的话inline=true，同时moveline被赋值和添加

            foreach (Line l in lines)
            {
                if (l.StartPoint.X == l.EndPoint.X)
                { //线是水平线的话，x的横坐标在不在两个断电之间
                    if (x == l.StartPoint.X)
                    {
                        if (isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
                        {
                            inLine = true;
                            selectedLine = l;
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
                            inLine = true;
                            selectedLine = l;

                            break;
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
                        selectedLine = l;

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
                        selectedLine = l;

                        break;
                    }
                    else
                    {
                        inLine = false;
                        continue;
                    }
                }
                else if ((l.EndPoint.Y - l.StartPoint.Y) / (l.EndPoint.X - l.StartPoint.X) == (y - l.StartPoint.Y) / (x - l.StartPoint.X) && isBetween(l.StartPoint.X, l.EndPoint.X, x) && isBetween(l.StartPoint.Y, l.EndPoint.Y, y))
                {
                    inLine = true;
                    selectedLine = l;
                    break;
                }
                else
                {
                    inLine = false;

                }
            }
            if (inLine)
            {
                this.Cursor = Cursors.Hand; 
                this.Refresh();
                this.Invalidate();
            }
            else this.Cursor = Cursors.Default;
            if(selectedLine!=null){
                tagTxt.Text = selectedLine.tag;                
            }
            #endregion

        }

        public int index = -1;
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
             index = listBox1.SelectedIndex;
            foreach (Line l in lines)
            {
                if (l.ID == index)
                {
                    if (l.isShow == true) l.isShow = false;
                    else { l.isShow = true; selectedLine = l; }
                }
            }
            updateListBox();
            updateLines();
            
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
             index = listBox1.SelectedIndex;
            foreach (Line l in lines)
            {
                if (l.ID == index)
                {
                    selectedLine=l;
                }
            }
            updateListBox();
            updateLines();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            sqltxt = textBox1.Text;
            //Data Source=ELAB-36\SQLEXPRESS;Initial Catalog=asj_DBR;Integrated Security=True;Pooling=False
            try
            {
                sqlconn.sqlconn(sqltxt, "SQL");
            }
            catch (Exception ex)
            {
                label1.Text = "数据库未连接或连接失败。。。";

                return;
            }
            
            DataTable dt = sqlconn.getVector("SELECT Name,crdate FROM SysObjects Where XType='U' ORDER BY Name");

            if (dt != null)
            {
                dataGridView1.DataSource = dt.DefaultView;
                cb1.DataSource = dt;
                cb2.DataSource = dt.Copy();
                cb1.DisplayMember = "name";
                cb2.DisplayMember = "name";
                cb1.ValueMember = "name";
                cb2.ValueMember = "name";
                cb1.AutoCompleteSource = AutoCompleteSource.ListItems;
                cb1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cb2.AutoCompleteSource = AutoCompleteSource.ListItems;
                cb2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                loadEnd = true;
                cb1.SelectedIndex = -1;
                cb2.SelectedIndex = -1;
                if(dt.Rows.Count>0)cb1.SelectedIndex = 0;
            }
        }

        private void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            string tablename = "";
            if (cb.Name == "cb1") tablename = "table1";
            if (cb.Name == "cb2") tablename = "table2";
            if (!loadEnd) return;
            if (cb.SelectedIndex < 0) return;
            bool isExist = false;
            if (sqlconn.cn_Sql != null)
            {
                foreach (GroupBox g in panel4.Controls.OfType<GroupBox>())
                {
                    if (g.Name == tablename)
                    {//调整table1或table2，如果cb1调整table1，则继续
                        isExist = true;
                        if (g.Text != cb.Text)
                        {//表不同的话就删除旧的加新的，相同则啥都不做
                            string name = cb.Text;
                            List<Line> templines = new List<Line>();
                            foreach (Line l in lines)
                            {
                                if (l.srcg == g || l.desg == g)
                                {
                                    templines.Add(l);
                                    continue;
                                }
                            }
                            lines = lines.Except(templines).ToList<Line>();

                            panel4.Controls.Remove(g);
                            GroupBox grp = new GroupBox();
                            addNewGroupBox(grp, tablename, cb.Text, false);
                            this.Invalidate();
                            this.Refresh();
                        }
                    }
                    else
                    {//发现不是cb1对应的table1，直接看table2是不是和要加的一样，如果一样则不加了
                        if (g.Text == cb.Text)
                        {
                            MessageBox.Show("该表已存在，请不要重复添加！");
                            return;
                        }
                    }
                }
                if (!isExist)
                {
                    GroupBox grp = new GroupBox();
                    addNewGroupBox(grp, tablename, cb.Text, false);
                }

            }
            else
            {
                MessageBox.Show("数据库尚未连接。。。");
            }

            if (isOK())
            {
                string[] t = new string[] { cb1.Text, cb2.Text };
                Array.Sort(t);
                tableName= function.createName(cb1.Text, cb2.Text);
                this.Text = tableName;
                function.createTable(tableName);
                //读取数据，因为每画一条线就保存一次，所以不需要再有保存了在切换之前
                loadData();
                label2.Text = "已建立联系表";
            }
            else
            {
                this.Text = cb.Text+"__等待连接";
            }
            if(this.Parent!=null)this.Parent.Text = this.Text;

        }
        
        private bool isOK()
        {
            int num=0;            
            foreach (GroupBox g in panel4.Controls.OfType<GroupBox>())
            {
                if(g.Name=="table1"){ num++;}
                if (g.Name == "table2"){num++;}                
            }
            if (num == 2) {                
                return true; }
            else return false;
        }

        
      
    

    }
}
