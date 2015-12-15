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
    public partial class temp : Form
    {
        public temp()
        {
            InitializeComponent();
        }
        sqlConn sqlconn = new sqlConn();
        private void button1_Click(object sender, EventArgs e)
        {
            string sqltxt = textBox1.Text;
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
            dataGridView1.DataSource = dt.DefaultView;
            comboBox1.DataSource = dt;
            comboBox2.DataSource = dt.Copy();
            comboBox1.DisplayMember = "name";
            comboBox2.DisplayMember = "name";
            comboBox1.ValueMember = "name";
            comboBox2.ValueMember = "name";
            comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox2.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }
    }
}
