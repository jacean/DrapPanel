using System;
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
    public partial class TablesForm : Form
    {
        public TablesForm()
        {
            InitializeComponent();
        }
        string sqltxt = "";
        sqlConn sqlconn = new sqlConn();

        private void addNewGroupBox(GroupBox grp, string name, bool isLoad)
        {
            foreach (GroupBox g in panel4.Controls)
            {
                if (g.Name == name)
                {
                    MessageBox.Show("该表已添加，请不要重复添加已造成混乱~");
                    return;
                }
            }
            DataTable tblFields = sqlconn.cn_Sql.GetSchema(SqlClientMetaDataCollectionNames.Columns);

            grp.Text = name;
            grp.Name = name;
            if (!isLoad)
            {
                grp.Width = 200;
                grp.Height = 100;
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
           //add label
            Label tablabel = new Label();
            tablabel.Font = new Font(tablabel.Font.FontFamily.Name, 20);
            tablabel.Text = name;
            tablabel.AutoSize = false;
            tablabel.TextAlign = ContentAlignment.MiddleCenter;

         
            panel4.Controls.Add(grp);
            grp.Controls.Add(panel);
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.Transparent;
            panel.Margin = new Padding(0);
            panel.Padding = new Padding(0);
            panel.Controls.Add(tablabel);
            tablabel.Margin = new Padding(0);

            tablabel.Width = panel.Width - 30;
            tablabel.Height = panel.Height - 30;
            tablabel.Location = new Point(15, 15);

            panel.BorderStyle = BorderStyle.Fixed3D;
            tablabel.BorderStyle = BorderStyle.Fixed3D;
        }

      

        private void Tables_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            panel4.Paint += new PaintEventHandler(panel4_Paint);
            panel4.MouseMove += new MouseEventHandler(panel4_MouseMove);
            panel4.MouseDown += new MouseEventHandler(panel4_MouseDown);
            panel4.MouseUp += new MouseEventHandler(panel4_MouseUp);
            panel4.MouseWheel += new MouseEventHandler(panel4_MouseWheel);



            //loadData();
            //updateListBox();
            //updateLines();

           // panel4.Height = this.ClientSize.Height - panel1.Height;
            
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

            isInline(x, y);

            if (e.Button == MouseButtons.Left)
            {
                if (inLine && drawingLine == null)//在已有的线上
                {//已屏蔽选中线条移动事件
                    if (isDelete)
                    {
                        lines.Remove(selectedLine);

                        this.Invalidate();
                        this.Refresh();
                    }
                    else
                    {//弹出窗体

                        //string showText = "";
                        //showText = "起点:" + selectedLine.srcg.Name + "->";

                        //foreach (Control cp in selectedLine.srcg.Controls)
                        //{
                        //    foreach (ListView lv in cp.Controls)
                        //    {
                        //        showText += lv.Items[selectedLine.srcg_itemIndex].Text + "\n";
                        //    }
                        //}
                        //showText += "终点:" + selectedLine.desg.Name + "->";
                        //foreach (Control cp in selectedLine.desg.Controls)
                        //{
                        //    foreach (ListView lv in cp.Controls)
                        //    {
                        //        showText += lv.Items[selectedLine.desg_itemIndex].Text + "\n";
                        //    }
                        //}

                        LinesForm lineForm = new LinesForm();
                        lineForm.ShowDialog();
                    }
                }
                else
                {//画面整体移动//怎么才能把控件画在画布上，这样调整画布的起始坐标就好了嘛
                    isMoveForm = true;
                    movestartPoint = e.Location;
                    foreach (Line l in lines)
                    {
                        l.endPointAdd = l.EndPoint;
                        l.startPointAdd = l.StartPoint;
                    }
                    pointList.Clear();
                    foreach (GroupBox gr in panel4.Controls)
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
                    showText += "起点:" + selectedLine.srcg.Name + "->";

                    foreach (Control cp in selectedLine.srcg.Controls)
                    {
                        foreach (ListView lv in cp.Controls)
                        {
                            showText += lv.Items[selectedLine.srcg_itemIndex].Text + "\n";
                        }
                    }
                    showText += "终点:" + selectedLine.desg.Name + "->";
                    foreach (Control cp in selectedLine.desg.Controls)
                    {
                        foreach (ListView lv in cp.Controls)
                        {
                            showText += lv.Items[selectedLine.desg_itemIndex].Text;
                        }
                    }
                  

                }
            }
            updateLines();
            inLine = false;
        }
        List<Point> pointList = new List<Point>();
        public bool isMoveForm = false;//是否在拖动画面
        public Point movestartPoint = Point.Empty;


        /// <summary>
        /// 在绘图区移动鼠标时，如果正在绘制新线条，就更新绘制面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void panel4_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = getPointToForm((Control)sender, e.Location).ToString();

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
                foreach (Line line in lines)
                {

                    //坐标转换  //只在确定移动的时候赋值，然后使用应该就可以了吧，就是在down时              

                    line.StartPoint.X = line.startPointAdd.X - movestartPoint.X + e.X;
                    line.StartPoint.Y = line.startPointAdd.Y - movestartPoint.Y + e.Y;

                    line.EndPoint.X = line.endPointAdd.X - movestartPoint.X + e.X;
                    line.EndPoint.Y = line.endPointAdd.Y - movestartPoint.Y + e.Y;

                }
               

                //移动容器,依靠的是系统对groupbox的遍历是一致的顺序
                int i = 0;
                foreach (GroupBox grp in panel4.Controls)
                {

                    grp.Left = pointList[i].X - movestartPoint.X + e.X;
                    grp.Top = pointList[i++].Y - movestartPoint.Y + e.Y;
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
                    Mo = 1.02f;


                }
                else if (e.Delta < 0)
                {
                    Mo = 0.98f;
                }
                #region 移动控件
                foreach (Control ct in this.panel4.Controls)
                {//看成是点的移动

                    if (ct.Width < 60 | ct.Height < 60)
                    {
                        if (Mo < 1)
                            return;
                    }

                    ct.Width += (int)((Mo - 1) * (float)ct.Width);
                    ct.Height += (int)((Mo - 1) * (float)ct.Height);
                    ct.Left += (int)((float)(ct.Left - e.X) * (Mo - 1));
                    ct.Top += (int)((float)(ct.Top - e.Y) * (Mo - 1));

                    /////////
                    foreach (Control cp in ct.Controls)
                    {
                        foreach (Control cl in cp.Controls)
                        {
                            cl.Width = cp.Width - 30;
                            cl.Height = cp.Height - 30;
                            cl.Location = new Point(15, 15);
                        }

                    }

                }

                #endregion
               // updateLines();

                //移动线条////////////////////

                foreach (Line line in lines)
                {

                    //线条的起始坐标转换,这个有误差，且越积累越大
                    //line.StartPoint.X += (int)((float)(line.StartPoint.X - e.X) * (Mo - 1));
                    //line.StartPoint.Y += (int)((float)(line.StartPoint.Y - e.Y) * (Mo - 1));
                    //line.EndPoint.X += (int)((float)(line.EndPoint.X - e.X) * (Mo - 1));
                    //line.EndPoint.Y += (int)((float)(line.EndPoint.Y - e.Y) * (Mo - 1));

                    //用相对坐标试下
                    //                
                    line.startPointtoSender.X += (int)((float)(line.startPointtoSender.X) * (Mo - 1));
                    line.startPointtoSender.Y += (int)((float)(line.startPointtoSender.Y) * (Mo - 1));
                    line.endPointtoSender.X += (int)((float)(line.endPointtoSender.X) * (Mo - 1));
                    line.endPointtoSender.Y += (int)((float)(line.endPointtoSender.Y) * (Mo - 1));
                    line.StartPoint = new Point(line.srcg.Location.X + line.startPointtoSender.X, line.srcg.Location.Y + line.startPointtoSender.Y);
                    line.EndPoint = new Point(line.desg.Location.X + line.endPointtoSender.X, line.desg.Location.Y + line.endPointtoSender.Y);


                }


                this.Invalidate();
                this.Refresh();
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
            foreach (Line line in lines)
            {
                if (line.src_isShow && line.des_isShow && line.isShow)
                {
                    if (line == drawingLine || (selectedLine != null && line == selectedLine))
                    {
                        p.Color = Color.Blue;

                    }
                    else
                    {
                        p.Color = line.color;
                    }
                    g.DrawLine(p, line.StartPoint, line.EndPoint);
                }
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

            updateListBox();


        }

        private void panel4_MouseEnter(object sender, EventArgs e)
        {

            this.panel4.Focus();
        }


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
                //g.Visible = false;
                rect = g.Bounds;

            }
        }


        void control_MouseMove(object sender, MouseEventArgs e)
        {
            label1.Text = getPointToForm((Control)sender, e.Location).ToString();
            if (e.Button == MouseButtons.Left)
            {
                isDrag = true;
                //重新设置rect的位置，跟随鼠标移动
                rect.Location = getPointToForm((Control)sender, new Point(e.Location.X - mouseDownPoint.X, e.Location.Y - mouseDownPoint.Y));

                //设置线条的跟随变化
                GroupBox g = (GroupBox)sender;
                g.Location = rect.Location;



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

        Rectangle rec = Rectangle.Empty;

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
                
                    endDrawingFunc(this.PointToClient(Control.MousePosition));//
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
                src = null;
                des = null;
             
            }
            this.Refresh();


           
        }


        void panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawingLine != null)
            {

                if (startPaint && location == 2)
                {


                    drawingLine.EndPoint = this.getPointToForm((Control)sender, e.Location);
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


                location = 2;
                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
            }
            if (startPaint && src == des)
            {
                count++;

                location = 1;
            }

          
          
        }

        private void panel_MouseLeave(object sender, EventArgs e)
        {

            if (mDown && sender == src && count == 1)
            { //开始化线，其实坐标为鼠标当前坐标

                location = 0;
                startPaint = true;
                //drawPanel_MouseDown((object)this, this.PointToClient(Control.MousePosition));
                
                startDrawingFunc(this.PointToClient(Control.MousePosition));
                drawingLine.srcg = (GroupBox)((Panel)sender).Parent;
                //
                drawingLine.startPointtoSender.X = this.PointToClient(Control.MousePosition).X - drawingLine.srcg.Location.X;
                drawingLine.startPointtoSender.Y = this.PointToClient(Control.MousePosition).Y - drawingLine.srcg.Location.Y;
                //
              
            }
            if (mDown && location == 1)
            {
                location = 0;

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
            public int ID = -1;
            public Point StartPoint = Point.Empty;
            public Point EndPoint = Point.Empty;
              //加附点以便在画面整体移动时定位
            public Point startPointAdd = Point.Empty;
            public Point endPointAdd = Point.Empty;

             public Point startPointtoSender = Point.Empty;
            public Point endPointtoSender = Point.Empty;

            public GroupBox srcg;
            public GroupBox desg;
            public int srcg_itemIndex = -1;
            public int desg_itemIndex = -1;
            public int dsX_src = 0;
            public int dsX_des = 0;
            public bool src_isShow = true;
            public bool des_isShow = true;
            public bool isShow = true;
            public Color color = Color.Black;
            public string tag = "添加标签";
           
             public Line(Point startPoint)
            {
                StartPoint = startPoint;
                startPointAdd = startPoint;//给附加坐标赋值，随之变化
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
        void endDrawingFunc(Point e)
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
        void startDrawingFunc(Point e)
        {
            //int x=e.Location.X;
            //int y=e.Location.Y;
            int x = e.X;
            int y = e.Y;

            drawingLine = new Line(e);

            lines.Add(drawingLine);

            //}


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

        private Point getPointToForm(Control control, Point p)
        {

            return this.PointToClient(control.PointToScreen(p));
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

           

            #endregion
            this.Invalidate();
            this.Refresh();

        }

        private void updateListBox()
        {
            if (lines.Count > 0 && drawingLine == null)
            {
                //listBox1.Items.Clear();
                //foreach (Line l in lines)
                //{

                //    string showText = "";
                //    showText = "[起点:" + l.srcg.Name + ".";

                //    foreach (Control cp in l.srcg.Controls)
                //    {
                //        foreach (ListView lv in cp.Controls)
                //        {
                //            showText += lv.Items[l.srcg_itemIndex].Text + "]+--+";
                //        }
                //    }
                //    showText += "[终点:" + l.desg.Name + ".";
                //    foreach (Control cp in l.desg.Controls)
                //    {
                //        foreach (ListView lv in cp.Controls)
                //        {
                //            showText += lv.Items[l.desg_itemIndex].Text + "]";
                //        }
                //    }

                //    if (l.src_isShow && l.des_isShow)
                //    {
                //        if (!l.isShow)
                //        {
                //            listBox1.Items.Add("setHide-" + showText);
                //        }
                //        else
                //            listBox1.Items.Add("Show-" + showText);
                //    }
                //    else
                //    {
                //        listBox1.Items.Add("Hide-" + showText);
                //    }
                //    l.ID = listBox1.Items.Count - 1;
                //}
                //listBox1.SelectedIndex = index;
            }
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
                    selectedLine = l;
                }
            }
            updateListBox();
            updateLines();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Data Source=ELAB-SQ252L;Initial Catalog=student;Persist Security Info=True;User ID=ta;Password=elab2013";
            sqltxt = textBox1.Text;
            //sqltxt="Data Source=ELAB-SQ252L;Initial Catalog=student;Persist Security Info=True;User ID=ta;Password=elab2013";

            try
            {
                sqlconn.sqlconn(sqltxt, "SQL");
            }
            catch (Exception ex)
            {
                label1.Text = "数据库未连接或连接失败。。。";
                return;
            }
            label1.Text = "数据库连接成功";

           
            DataTable dt = sqlconn.getVector("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
               
                checkedListBox1.Items.Add(dt.Rows[i][0].ToString());

            }
          

        }

       

        private void checkedListBox1_MouseClick(object sender, MouseEventArgs e)
        {
           
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int tableindex = e.Index;
            
            ////for (int i = 0; i < checkedListBox1.SelectedItems.Count;i++ )
            ////{

            if (checkedListBox1.GetItemCheckState(tableindex) == CheckState.Unchecked)
            {
                foreach (GroupBox g in panel4.Controls)
                {
                    if (g.Name == checkedListBox1.SelectedItem.ToString())
                    {
                        g.Visible = true;
                        return;
                    }
                }
                GroupBox grp = new GroupBox();
                addNewGroupBox(grp, checkedListBox1.Items[tableindex].ToString(), false);
            }
            if (checkedListBox1.GetItemCheckState(tableindex) == CheckState.Checked)
            {
                foreach (GroupBox g in panel4.Controls)
                {
                    if (g.Name == checkedListBox1.SelectedItem.ToString())
                    {
                        g.Visible = false;
                    }
                }
            }
            //}
           
        }
        private void isInline(int x, int y)
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

            #endregion

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
        
    }
}
