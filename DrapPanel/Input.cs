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
    public partial class Input : Form
    {
        private string comment = "";
        bool isOK = false;
        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
        public Input()
        {
            InitializeComponent();
        }
        public Input(string label,string file)
        {
            InitializeComponent();
            this.label1.Text = label;
            //this.label2.Text = file;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                new sqlConn().sqlconn(textBox1.Text.Trim(), "SQL");
                function.sqlcon = textBox1.Text.Trim();
                isOK = true;
                this.Dispose();
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("数据库连接失败，请重试\n"+ex.ToString());
                isOK = false;
            }
            
        }

        private void Input_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode== Keys.Enter)
            {
                button1_Click(null,null);
            }
        }

        private void Input_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isOK)
            { }
            else
            {
                Application.Exit();
            }
        }
    }
}
