using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DrapPanel
{
    class Line
    {
        public int ID = -1;
        public Point StartPoint = Point.Empty;
        public Point EndPoint = Point.Empty;
        public GroupBox srcg;
        public GroupBox desg;
        public int srcg_itemIndex = -1;
        public int desg_itemIndex = -1;       
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
}
