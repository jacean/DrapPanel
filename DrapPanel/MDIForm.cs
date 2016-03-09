﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DrapPanel
{
    public partial class MDIForm : Form
    {
        public MDIForm()
        {
            InitializeComponent();
        }
        
        private void 新建ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        public void addTabPage()
        {
            MainForm mf = new MainForm();
            mf.MdiParent = this;
            mf.FormBorderStyle = FormBorderStyle.None;
            mf.Dock = DockStyle.Fill;
            this.MainTabControl.SelectedTab.Controls.Add(mf);
            this.MainTabControl.SelectedTab.Text = mf.Text;
            mf.Show();
            TabPage newTab = new TabPage();
            newTab.Text = "  New ";
            MainTabControl.TabPages.Add(newTab);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //清空控件
            //this.MainTabControl.TabPages.Clear();
            //绘制的方式OwnerDrawFixed表示由窗体绘制大小也一样
            this.MainTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.MainTabControl.Padding = new System.Drawing.Point(CLOSE_SIZE, 5);
            this.MainTabControl.DrawItem += new DrawItemEventHandler(this.MainTabControl_DrawItem);
            this.MainTabControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainTabControl_MouseDown);
            this.MainTabControl.Appearance = TabAppearance.Buttons;

            MainForm mf = new MainForm();
            mf.MdiParent = this;
            mf.FormBorderStyle = FormBorderStyle.None;
            mf.Dock = DockStyle.Fill;
            mf.Show();
            MainTabControl.TabPages[0].Controls.Add(mf);
            
        }
        const int CLOSE_SIZE = 15;
        //tabPage标签图片
        Bitmap image = new Bitmap(Properties.Resources.Image1);
        //绘制“Ｘ”号即关闭按钮
        private void MainTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            
            try
            {
                Rectangle myTabRect = this.MainTabControl.GetTabRect(e.Index);
                if (e.Index == this.MainTabControl.TabPages.Count - 1)
                {
                    e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text, this.Font, SystemBrushes.ControlText, myTabRect.X, myTabRect.Y+5);
                    return;
                }
                //先添加TabPage属性   
                e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text, this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 5);
 
                //再画一个矩形框
                using (Pen p = new Pen(Color.White))
                {
                    myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                    myTabRect.Width = CLOSE_SIZE;
                    myTabRect.Height = CLOSE_SIZE;
                    e.Graphics.DrawRectangle(p, myTabRect);
                }
 
                //填充矩形框
                Color recColor = e.State == DrawItemState.Selected ? Color.White : Color.White;
                using (Brush b = new SolidBrush(recColor))
                {
                    e.Graphics.FillRectangle(b, myTabRect);
                }
 
                //画关闭符号
                using (Pen objpen = new Pen(Color.Black))
                {
                    ////=============================================
                    //自己画X
                    ////"\"线
                    //Point p1 = new Point(myTabRect.X + 3, myTabRect.Y + 3);
                    //Point p2 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + myTabRect.Height - 3);
                    //e.Graphics.DrawLine(objpen, p1, p2);
                    ////"/"线
                    //Point p3 = new Point(myTabRect.X + 3, myTabRect.Y + myTabRect.Height - 3);
                    //Point p4 = new Point(myTabRect.X + myTabRect.Width - 3, myTabRect.Y + 3);
                    //e.Graphics.DrawLine(objpen, p3, p4);
 
                    ////=============================================
                    //使用图片
                    Bitmap bt = new Bitmap(image);
                    Point p5 = new Point(myTabRect.X, 4);
                    e.Graphics.DrawImage(bt, p5);
                    //e.Graphics.DrawString(this.MainTabControl.TabPages[e.Index].Text, this.Font, objpen.Brush, p5);
                }
                e.Graphics.Dispose();
            }
            catch (Exception)
            { }
        }
 
        //关闭按钮功能
        private void MainTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;
                //计算关闭区域   
                Rectangle myTabRect = this.MainTabControl.GetTabRect(this.MainTabControl.SelectedIndex);
 
                myTabRect.Offset(myTabRect.Width - (CLOSE_SIZE + 3), 2);
                myTabRect.Width = CLOSE_SIZE;
                myTabRect.Height = CLOSE_SIZE;
 
                //如果鼠标在区域内就关闭选项卡   
                bool isClose = x > myTabRect.X && x < myTabRect.Right && y > myTabRect.Y && y < myTabRect.Bottom;
                if (isClose == true)
                {
                    (this.MainTabControl.SelectedTab.Controls[0] as MainForm).Close();
                    this.MainTabControl.TabPages.Remove(this.MainTabControl.SelectedTab);

                }
            }

        }



        private void MainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainTabControl.SelectedIndex == MainTabControl.TabPages.Count-1)
            {
                addTabPage();
            }
        }    

        
    }

    
}
