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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }
      
        string sqltxt = "";
        sqlConn sqlconn = new sqlConn();
        public ContextMenuStrip rM = new ContextMenuStrip();

        private void Form4_Load(object sender, EventArgs e)
        {
            label1.Text = "";
            label11.Text = "";
            label12.Text = "";
            panel4.Paint += new PaintEventHandler(panel4_Paint);
            panel4.MouseMove += new MouseEventHandler(panel4_MouseMove);
            panel4.MouseDown += new MouseEventHandler(panel4_MouseDown);
            panel4.MouseUp += new MouseEventHandler(panel4_MouseUp);
            panel4.MouseWheel += new MouseEventHandler(panel4_MouseWheel);
            
            
          
            loadData();
            updateListBox();
            updateLines();
            
            panel4.Height=this.ClientSize.Height-panel1.Height;

        }

       

        private void button4_Click(object sender, EventArgs e)
        {
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
            
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DataTable dt = sqlconn.getVector("SELECT Name FROM SysObjects Where XType='U' ORDER BY Name");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                comboBox1.Items.Add(dt.Rows[i][0].ToString());

            }
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (sqlconn.cn_Sql != null)
            {
                
                    GroupBox grp = new GroupBox();
                    addNewGroupBox(grp, comboBox1.Text,false);
                
            }
            else
            {
                MessageBox.Show("数据库尚未连接。。。");
            }
            
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

                        this.Invalidate();
                        this.Refresh();
                    }
                    else
                    {//弹出窗体

                        string showText = "";
                        showText = "起点:" + selectedLine.srcg.Name + "->";

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
                    rM.Show(panel4, e.Location);
                    rM.ShowImageMargin = false;
                    rM.Items.Clear();
                    rM.Items.Add(showText);
                    rM.Items.Add("颜色");
                    rM.Items.Add("隐藏");
                    ToolStripTextBox mtxt = new ToolStripTextBox();
                    mtxt.Text = selectedLine.tag;
                    mtxt.KeyDown += new KeyEventHandler(mtxt_KeyDown);
                    rM.Items.Add(mtxt);
                    rM.Items[0].Enabled = false;
                    rM.Items[1].Click += new EventHandler(item1_Click);
                    rM.Items[2].Click+=new EventHandler(item2_Click);
                    
                }
            }
            updateLines();
            inLine = false;
        }

        void mtxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedLine.tag = ((ToolStripTextBox)sender).Text;

            }
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

               
                //移动容器,依靠的是系统对groupbox的遍历是一致的顺序
                int i = 0;
                foreach (GroupBox grp in panel4.Controls)
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
                    Mo = 1.02f;


                }
                else if (e.Delta < 0)
                {
                    Mo = 0.98f;
                }
                #region 移动控件
                foreach (Control ct in this.panel4.Controls)
                {//看成是点的移动

                    if (ct.Width < 60 | ct.Height< 60)
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
                if (this.PointToClient(Control.MousePosition).X > gg.Location.X + gg.Width / 2)
                {
                    dsX = gg.Width;
                }
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
                        location = 1;
                        startPaint = true;

                        
                        startDrawingFunc(new Point(gg.Location.X+dsX, itemPoint.Y +height / 2+gg.Location.Y));
                        drawingLine.srcg = gg;
                        drawingLine.srcg_itemIndex = selectedIndex;
                        //
                        drawingLine.dsX_src= dsX;
                     
                       
                        label11.Text ="选中开始项:" + drawingLine.srcg_itemIndex.ToString();
                       
                      
                    }
                    else
                    {//结束panel
                        if (location == 2 && startPaint)
                        {
                            label12.Text = "结束点对应选中项为：" + selectedIndex.ToString();
                            //将end坐标设为鼠标纵坐标所属项的中间位置
                            drawingLine.desg = (GroupBox)(((Panel)sender).Parent);
                            drawingLine.desg_itemIndex = selectedIndex;
                            drawingLine.dsX_des = dsX;
                           
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
                        location = 0;
          
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
            
              
                location = 1;
            }
        }

        private void panel_MouseLeave(object sender, EventArgs e)
        {
            if (mDown&&location==1)
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
            public int ID =-1;
            public Point StartPoint = Point.Empty;
            public Point EndPoint = Point.Empty;
            public GroupBox srcg;
            public GroupBox desg;
            public int srcg_itemIndex=-1;
            public int desg_itemIndex=-1;
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
                rect.Location = getPointToForm((Control)sender,new Point(e.Location.X - mouseDownPoint.X, e.Location.Y - mouseDownPoint.Y));
                
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
        
        private Point getPointToForm(Control control, Point p)
        {
           
            return this.PointToClient(control.PointToScreen(p));
        }

        private void Form4_SizeChanged(object sender, EventArgs e)
        {
            panel4.Height = this.ClientSize.Height - panel1.Height;
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {//保存数据
            using (StreamWriter sw = new StreamWriter(@"Line.lst", false, Encoding.UTF8))
            {
                foreach (Line l in lines)
                {
                    sw.WriteLine(l.dsX_src.ToString()+"\b"+l.dsX_des.ToString()+"\b"+l.srcg_itemIndex.ToString()+"\b"+l.desg_itemIndex.ToString()+"\b"+l.srcg.Name.ToString()+"\b"+l.desg.Name.ToString());
                }
            }
            using (StreamWriter sw = new StreamWriter(@"Control.lst", false, Encoding.UTF8))
            {
                sw.WriteLine("sql:"+textBox1.Text);
                foreach (GroupBox grp in panel4.Controls)
                {
                    sw.WriteLine(grp.Name+"\b"+grp.Location+"\b"+grp.Width.ToString()+"\b"+grp.Height.ToString());
                }
            }
        }

        private void loadData()
        {
            if (File.Exists(@"Control.lst"))
            {
                using (StreamReader sr = new StreamReader(@"Control.lst", Encoding.UTF8))
                {
                    string l = "";
                    while ((l = sr.ReadLine()) != null)
                    {
                        if (l.StartsWith("sql:"))
                        {
                            textBox1.Text = l.Substring(4);
                            button4_Click(null,null);
                            continue;
                        }
                        if (sqlconn.cn_Sql==null|| sqlconn.cn_Sql.State==ConnectionState.Connecting)
                        {
                            return;
                        }
                        string[] ls = l.Split('\b');
                        GroupBox g = new GroupBox();
                        g.Location = getPoint(ls[1]);
                        g.Width = int.Parse(ls[2]);
                        g.Height = int.Parse(ls[3]);
                        addNewGroupBox(g, ls[0],true);
                        
                    }
                }
            }
            if (File.Exists(@"Line.lst"))
            {
                using (StreamReader sr = new StreamReader(@"Line.lst", Encoding.UTF8))
                {
                    string l = "";
                    Point startPoint = Point.Empty;
                    Point endPoint = Point.Empty;
                    Point itemstartPoint = Point.Empty;
                    Point itemendPoint = Point.Empty;
                    int startHeight = 0;
                    int endHeight = 0;
                    GroupBox srcg = null;
                    GroupBox desg = null;
                    int dsx_src = 0;
                    int dsx_des = 0;
                    int index_src = -1;
                    int index_des = -1;
                    bool src_isShow=true;
                    bool des_isShow=true;
                    while ((l = sr.ReadLine()) != null)
                    {
                        string[] ls = l.Split('\b');
                        dsx_src = int.Parse(ls[0]);
                        dsx_des = int.Parse(ls[1]);
                        index_src = int.Parse(ls[2]);
                        index_des = int.Parse(ls[3]);
                        
                       
                        foreach (GroupBox g in panel4.Controls)
                        {
                            if (g.Name == ls[4])
                            {
                                srcg = g;
                            }
                            if (g.Name == ls[5])
                            {
                                desg = g;
                            }
                        }

                        foreach (Control cp in srcg.Controls)
                        {
                            foreach (ListView lv in cp.Controls)
                            {

                                itemstartPoint = getItemLocation(lv, index_src);
                                startHeight = lv.GetItemRect(index_src).Height;
                                if (itemstartPoint.Y < 15+14+16 || itemstartPoint.Y > srcg.Height - 15)
                                {
                                    src_isShow = false;
                                }
                                else
                                {
                                    src_isShow = true;
                                }
                                startPoint.Y = srcg.Location.Y+ itemstartPoint.Y + startHeight / 2;

                            }

                        }

                        startPoint.X = srcg.Location.X + dsx_src;
                        foreach (Control cp in desg.Controls)
                        {
                            foreach (ListView lv in cp.Controls)
                            {

                                itemendPoint = getItemLocation(lv, index_des);
                                endHeight = lv.GetItemRect(index_des).Height;
                                if (itemendPoint.Y <  15+14+16 || itemendPoint.Y > desg.Height - 15)
                                {
                                   des_isShow = false;
                                }
                                else
                                {
                                    des_isShow = true;
                                }

                                endPoint.Y =desg.Location.Y+ itemendPoint.Y + endHeight / 2;

                            }

                        }
                        endPoint.X = desg.Location.X +dsx_des;
                     
                        Line line = new Line(startPoint);
                        line.EndPoint = endPoint;
                        line.srcg = srcg;
                        line.desg = desg;
                        line.dsX_src = dsx_src;
                        line.dsX_des = dsx_des;
                        line.srcg_itemIndex = index_src;
                        line.desg_itemIndex = index_des;
                        line.src_isShow = src_isShow;
                        line.des_isShow = des_isShow;
                        lines.Add(line);
                    }
                }
            }
            
        
        }
        private Point getPoint(string x)
        {
            x = x.TrimStart('{').TrimEnd('}');
            string[] xa = x.Split(',');
            return new Point(int.Parse(xa[0].TrimStart("X=".ToCharArray())),int.Parse(xa[1].TrimStart("Y=".ToCharArray())));
        }
        private void  addNewGroupBox(GroupBox grp,string name,bool isLoad)
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
                grp.Width = 150;
                grp.Height = 300;
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
            lv.HoverSelection = false;
            lv.Scroll += new ScrollEventHandler(lv_Scroll);
            

            #region add data
            lv.Columns.Add("字段名");
            lv.Columns.Add("字段类型");
            //lv.Columns.Add("列标题3", 100, HorizontalAlignment.Left);
            //add list
            lv.BeginUpdate();
            //数据更新，UI暂时挂起，直到EndUpdate绘制控件，可以有效避免闪烁并大大提高加载速度
            for (int i = 0; i < tblFields.Rows.Count; i++)
            {
                if (tblFields.Rows[i]["Table_Name"].ToString() ==name)
                {
                    ListViewItem lvi = new ListViewItem();
                    // lvi.ImageIndex = i;     //通过与imageList绑定，显示imageList中第i项图标

                    lvi.Text = tblFields.Rows[i]["Column_Name"].ToString();
                    lvi.SubItems.Add(tblFields.Rows[i]["DATA_TYPE"].ToString());
                    lv.Items.Add(lvi);

                }
            }
            lv.EndUpdate();  //结束数据处理，UI界面一次性绘制。
            #endregion
            panel4.Controls.Add(grp);
            grp.Controls.Add(panel);
            panel.Dock = DockStyle.Fill;
            panel.BackColor = Color.Transparent;
            panel.Margin = new Padding(0);
            panel.Padding = new Padding(0);
            panel.Controls.Add(lv);
            lv.Margin = new Padding(0);
            lv.Width = panel.Width - 30;
            lv.Height = panel.Height - 30;
            lv.Location = new Point(15, 15);
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

        private void button3_Click(object sender, EventArgs e)
        {
            
            if (comboBox1.Text != "")
            {
                bool isExist = false;
                string name = comboBox1.Text;
                foreach (GroupBox g in panel4.Controls)
                {
                    if (g.Name == name)
                    {
                        List<Line> templines = new List<Line>();
                        foreach (Line l in lines)
                        {
                            if (l.srcg == g||l.desg == g)
                            {
                                templines.Add(l);
                                continue;
                            }
                            
                        }
                       lines=lines.Except(templines).ToList<Line>();
                        isExist = true;
                        panel4.Controls.Remove(g);
                        this.Invalidate();
                        this.Refresh();
                    }
                }
                if (!isExist)
                {
                    MessageBox.Show("您要删除的这个表并没有在列表中，就不要枉费心机了哈哈哈哈哈");
                }
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
                    showText = "[起点:" + l.srcg.Name + ".";

                    foreach (Control cp in l.srcg.Controls)
                    {
                        foreach (ListView lv in cp.Controls)
                        {
                            showText += lv.Items[l.srcg_itemIndex].Text + "]+--+";
                        }
                    }
                    showText += "[终点:" + l.desg.Name + ".";
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

                if (line.desg != null)
                {
                    if (line.srcg.Left >= line.desg.Left)
                    {
                        line.dsX_src = 0;
                        line.dsX_des = line.desg.Width;

                    }
                    else
                    {
                        line.dsX_src = line.srcg.Width;
                        line.dsX_des = 0;
                    }

                }
                
                foreach (Control cp in srcg.Controls)
                {
                    foreach (ListView lv in cp.Controls)
                    {

                        itemstartPoint = getItemLocation(lv, line.srcg_itemIndex);
                        startHeight = lv.GetItemRect(line.srcg_itemIndex).Height;
                        if (itemstartPoint.Y < 15 + 14 + 16 || itemstartPoint.Y > srcg.Height - 15)
                        {
                            line.src_isShow = false;
                        }
                        else
                        {
                            line.src_isShow = true;
                        }
                        line.StartPoint.Y = srcg.Location.Y + itemstartPoint.Y + startHeight / 2;

                    }

                }

                line.StartPoint.X = line.srcg.Location.X + line.dsX_src;


                if (desg == null) return;

                foreach (Control cp in desg.Controls)
                {
                    foreach (ListView lv in cp.Controls)
                    {

                        itemendPoint = getItemLocation(lv, line.desg_itemIndex);
                        endHeight = lv.GetItemRect(line.desg_itemIndex).Height;
                        if (itemendPoint.Y < 15 + 14 + 16 || itemendPoint.Y > desg.Height - 15)
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
                line.EndPoint.X = line.desg.Location.X + line.dsX_des;


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

            #endregion

        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.SelectedIndex;
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
    }
}
