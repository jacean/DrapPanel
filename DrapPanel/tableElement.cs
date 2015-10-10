using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DrapPanel
{
    [ToolboxItem(true)]
    public partial class tableElement : UserControl
    {
        public tableElement()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        public override string Text
        {
            get
            {
                return groupBox1.Text;
            }
            set
            {
                groupBox1.Text = value;
            }
        }
    }
}
