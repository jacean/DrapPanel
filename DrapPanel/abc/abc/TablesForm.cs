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
        public bool isCon = false;

        

      

        private void Tables_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);

            panel4.Paint += new PaintEventHandler(panel4_Paint);
            panel4.MouseMove += new MouseEventHandler(panel4_MouseMove);
            panel4.MouseDown += new MouseEventHandler(panel4_MouseDown);
            panel4.MouseUp += new MouseEventHandler(panel4_MouseUp);
            panel4.MouseWheel += new MouseEventHandler(panel4_MouseWheel);
            label1.Text = "";
            label2.Text = "";
            loadData();

            
        }
        private void TablesForm_SizeChanged(object sender, EventArgs e)
        {
            panel4.Height = this.ClientSize.Height - panel1.Height;
        }
        private void TablesForm_FormClosed(object sender, FormClosedEventArgs e)
        {//保存数据
            if (!Directory.Exists(Application.StartupPath + "Work")) Directory.CreateDirectory(Application.StartupPath + "\\Work");
            using (StreamWriter sw = new StreamWriter(@"Work\\Connection.lst", false, Encoding.UTF8))
            {
                foreach (Line l in lines)
                {
                    sw.WriteLine(l.StartPoint.ToString() + "\b" + l.EndPoint.ToString() + "\b" + l.startPointtoSender.ToString() + "\b" + l.endPointtoSender.ToString() + "\b" + l.srcg.Name.ToString() + "\b" + l.desg.Name.ToString());
                }
            }
            using (StreamWriter sw = new StreamWriter(@"Work\\Location.lst", false, Encoding.UTF8))
            {
                sw.WriteLine("sql:" + textBox1.Text);
                foreach (GroupBox grp in panel4.Controls)
                {
                    sw.WriteLine(grp.Name + "\b" + grp.Location + "\b" + grp.Width.ToString() + "\b" + grp.Height.ToString());
                }
            }
        }


        List<Point> pointList = new List<Point>();
        public bool isMoveForm = false;//是否在拖动画面
        public Point movestartPoint = Point.Empty;
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
                    string[] text = { selectedLine.srcg.Name, selectedLine.desg.Name };
                    Array.Sort(text);

                    if (isDelete)
                    {
                        lines.Remove(selectedLine);
                        DirectoryInfo folder = new DirectoryInfo(Application.StartupPath+"\\Work");
                        foreach (FileInfo file in folder.GetFiles())
                        {
                            if (file.Name.StartsWith(text[0] + "_" + text[1]))
                            {
                                file.Delete();
                            }
                        }
                        this.Invalidate();
                        this.Refresh();
                    }
                    else
                    {//弹出窗体
                        LinesForm linesForm = new LinesForm(sqltxt);
                        
                        linesForm.Text = text[0]+ "&"+ text[1];
                        linesForm.Show();
                    }
                }
                else
                {//画面整体移动
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

            updateLines();
            inLine = false;
        }       
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
                    //坐标转换          

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

                //移动线条////////////////////

                foreach (Line line in lines)
                {
                    //用相对坐标试下
                    line.startPointtoSender.X += (int)((float)(line.startPointtoSender.X) * (Mo - 1));
                    line.startPointtoSender.Y += (int)((float)(line.startPointtoSender.Y) * (Mo - 1));
                    line.endPointtoSender.X += (int)((float)(line.endPointtoSender.X) * (Mo - 1));
                    line.endPointtoSender.Y += (int)((float)(line.endPointtoSender.Y) * (Mo - 1));
                    line.StartPoint = new Point(line.srcg.Location.X + line.startPointtoSender.X, line.srcg.Location.Y + line.startPointtoSender.Y);
                    line.EndPoint = new Point(line.desg.Location.X + line.endPointtoSender.X, line.desg.Location.Y + line.endPointtoSender.Y);
                }

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
        
            Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 消锯齿（可选项）
            Pen p = new Pen(Color.Black);
            foreach (Line line in lines)
            {
                if (line == drawingLine || (selectedLine != null && line == selectedLine))
                {
                    p.Color = Color.Blue;
                    g.DrawLine(p, line.StartPoint, line.EndPoint);
                }
                else
                {
                    if (line.srcg.Visible && line.desg.Visible)
                    {
                        p.Color = line.color;
                        g.DrawLine(p, line.StartPoint, line.EndPoint);
                    }                                                      
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

                foreach (Line l in lines)
                {
                    if (l.srcg == g)
                    {
                        l.StartPoint.X = g.Location.X + l.startPointtoSender.X;
                        l.StartPoint.Y = g.Location.Y + l.startPointtoSender.Y;
                    }
                    if (l.desg == g)
                    {
                        l.EndPoint.X = g.Location.X + l.endPointtoSender.X;
                        l.EndPoint.Y = g.Location.Y + l.endPointtoSender.Y;
                    }
                }


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
                label2.Text = "正在画线";
            }
            else
            {//结束panel
                if (location == 2 && startPaint)
                {

                    drawingLine.desg = (GroupBox)((Panel)sender).Parent;
                    bool isHave = false;
                    foreach (Line l in lines)
                    {
                        if (l != drawingLine && ((l.srcg == drawingLine.srcg && l.desg == drawingLine.desg)||(l.srcg==drawingLine.desg&&l.desg==drawingLine.srcg)))
                        {
                            label2.Text = "这两个表已经有了联系，不必重新建立关联";
                            isHave = true;

                        }
                    }
                    if (isHave)
                    {
                        drawingLine.StartPoint = Point.Empty;
                        drawingLine.EndPoint = Point.Empty;
                        lines.Remove(drawingLine);
                        drawingLine = null;
                    }
                    else
                    {
                        drawingLine.endPointtoSender.X = this.PointToClient(Control.MousePosition).X - drawingLine.desg.Location.X;
                        drawingLine.endPointtoSender.Y = this.PointToClient(Control.MousePosition).Y - drawingLine.desg.Location.Y;
                        endDrawingFunc(this.PointToClient(Control.MousePosition));//
                        label2.Text = "画线成功";
                    }
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
                        label2.Text = "不能在一张表上建立自身关联";
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
          
            if (startPaint && location == 2)
            {
                drawingLine.EndPoint = this.PointToClient(Control.MousePosition);
                location = 0;

            }
            if (startPaint && location==1 &&count > 1)
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
   
            int x = e.X;
            int y = e.Y;

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

       

        public int index = -1;
        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            index = listBox1.SelectedIndex;
            foreach (Line l in lines)
            {
                if (l.ID == index)
                {
                    LinesForm linesForm = new LinesForm(sqltxt);
                    linesForm.Text = selectedLine.srcg.Name + "&" + selectedLine.desg.Name;
                    linesForm.Show();
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
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int tableindex = e.Index;
            if (checkedListBox1.GetItemCheckState(tableindex) == CheckState.Unchecked)
            {
                foreach (GroupBox g in panel4.Controls)
                {
                    if (g.Name == checkedListBox1.SelectedItem.ToString())
                    {
                        g.Visible = true;
                        updateLines();
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
                button2.Text = "要删除点我";
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            //textBox1.Text = "Data Source=ELAB-SQ252L;Initial Catalog=student;Persist Security Info=True;User ID=ta;Password=elab2013";
            sqltxt = textBox1.Text;
            try
            {
                sqlconn.sqlconn(sqltxt, "SQL");
            }
            catch (Exception ex)
            {
                label2.Text = "数据库未连接或连接失败。。。";
                isCon = false;
                return;
            }
            label2.Text = "数据库连接成功";
            isCon = true;

           
            DataTable dt = sqlconn.getVector("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
               
                checkedListBox1.Items.Add(dt.Rows[i][0].ToString());

            }
          

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
        private void loadData()
        {
            if (File.Exists(@"Work\\Location.lst"))
            {
                using (StreamReader sr = new StreamReader(@"Work\\Location.lst", Encoding.UTF8))
                {
                    string l = "";
                    while ((l = sr.ReadLine()) != null)
                    {if (l.StartsWith("sql:"))
                        {
                            textBox1.Text = l.Substring(4);
                            button4_Click(null,null);
                        if(isCon)
                            continue;
                        else
                        {
                            textBox1.Text = "请输入连接字符串";
                            return;
                        }
                        }
                       
                        string[] ls = l.Split('\b');
                        GroupBox g = new GroupBox();
                        g.Location = getPoint(ls[1]);
                        g.Width = int.Parse(ls[2]);
                        g.Height = int.Parse(ls[3]);
                        addNewGroupBox(g, ls[0], true);
                        if (checkedListBox1.Items.Count > 0)
                        {
                            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                            {
                                if (checkedListBox1.Items[i].ToString() == ls[0])
                                {
                                    checkedListBox1.SelectedIndex = i;
                                    checkedListBox1.SetItemChecked(i, true);                                  
                                }
                            }
                        }
                    }
                }
            }
            if (File.Exists(@"Work\\Connection.lst"))
            {
                using (StreamReader sr = new StreamReader(@"Work\\Connection.lst", Encoding.UTF8))
                {
                    string l = "";
                    while ((l = sr.ReadLine()) != null)
                    {
                        string[] ls = l.Split('\b');
                        Line line = new Line(getPoint(ls[0]));
                        line.EndPoint = getPoint(ls[1]);
                        line.startPointtoSender = getPoint(ls[2]);
                        line.endPointtoSender = getPoint(ls[3]);
                        foreach (GroupBox g in panel4.Controls)
                        {
                            if (g.Name == ls[4])
                            {
                                line.srcg = g;
                            }
                            if (g.Name == ls[5])
                            {
                                line.desg = g;
                            }
                        }
                        lines.Add(line);
                    }
                }
            }
        }
        private Point getPoint(string x)
        {
            x = x.TrimStart('{').TrimEnd('}');
            string[] xa = x.Split(',');
            return new Point(int.Parse(xa[0].TrimStart("X=".ToCharArray())), int.Parse(xa[1].TrimStart("Y=".ToCharArray())));
        }
        private Point getPointToForm(Control control, Point p)
        {

            return this.PointToClient(control.PointToScreen(p));
        }
        private void updateLines()
        {

            Point itemstartPoint = Point.Empty;
            Point itemendPoint = Point.Empty;

            this.Invalidate();
            this.Refresh();

        }
        private void updateListBox()
        {
            if (lines.Count > 0 && drawingLine == null)
            {
                listBox1.Items.Clear();
                foreach (Line l in lines)
                {

                    string showText = "";
                    showText = "[起点:" + l.srcg.Name + "]+--+";


                    showText += "[终点:" + l.desg.Name + "]";
                    foreach (Control cp in l.desg.Controls)


                        if (l.srcg.Visible && l.desg.Visible)
                        {

                            listBox1.Items.Add("Show-" + showText);
                        }
                        else
                        {
                            listBox1.Items.Add("Hide-" + showText);
                        }
                    l.ID = listBox1.Items.Count - 1;
                }
                listBox1.SelectedIndex = index;
            }
        }
    }
}
