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
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }
        public Form5(string s,string d)
        {
            InitializeComponent();
            src = s;
            des = d;
        }
        public string src = "";
        public string des = "";

        private void Form5_Load(object sender, EventArgs e)
        {
            label1.Text = "源头:" + src;
            label2.Text = "目标:" + des;
        }



    }
}
