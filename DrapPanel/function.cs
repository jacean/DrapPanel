using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace DrapPanel
{
    class function
    {
       
    
        static string table1;
        static string table2;
        static string index1;
        static string index2;
        static string direct;
        //static string sqlcon="Data Source=ELAB-36\\SQLEXPRESS;Initial Catalog=asj_DBR;Integrated Security=True;Pooling=False";
        public static string sqlconDBR = "";

        public static string createName(string table1,string table2)
        {
            string[] t = new string[] { table1, table2 };
            Array.Sort(t);
            string name = string.Join("__", t);
            name = "DB_" + name;           
            return name;
        }
        public static void createTable(string name)
        {
            sqlConn con = new sqlConn();
            con.sqlconn(sqlconDBR,"SQL"); 
           
            string l = "";
            string sql = "";
            using (StreamReader sr = new StreamReader("sqlcontent.sql", Encoding.UTF8))
            {
                int c = 0;
                while ((l = sr.ReadLine()) != null)
                {
                    if (c == 0)
                    {
                        l += " " + name;
                    }
                    c++;
                    sql += l;
                }
            }
            try
            {
                con.executeUpdate(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            con.Close();

        }
        public static void saveLine(Line l,string currentTables)
        {
            int i=0;
            string olddirect = "";
            if ((i=checkrepeat(l,currentTables))>-1)//checkrepeat已经执行了sort
            {//重了的话 ,判断指向
                olddirect = i.ToString();
                if (l.srcg.Text == table1) { direct = "0"; }
                if (l.desg.Text == table1) { direct = "1"; }
                if (direct == olddirect) { return; }
                else {
                    direct = "2";
                    updatedirect(direct,currentTables);
                }
                
            }
            else
            {
                sqlConn con = new sqlConn();
                con.sqlconn(sqlconDBR, "SQL");
                string sql = "";
                sort(l);
                if (l.srcg.Text == table1) { direct = "0"; }
                if (l.desg.Text == table1) { direct = "1"; }
                // string p = l.dsX_src.ToString() + "\b" + l.dsX_des.ToString() + "\b" + l.srcg_itemIndex.ToString() + "\b" + l.desg_itemIndex.ToString() + "\b" + l.srcg.Name.ToString() + "\b" + l.desg.Name.ToString();
                sql = "INSERT INTO " + currentTables + " (table1, field1,table2,field2,tag,direct) VALUES ('" + table1 + "', '" + index1 + "', '" + table2 + "', '" + index2 + "', '" + l.tag.ToString() + "','"+direct+"')";
                //我可能需要一个id来删除这一行
                con.executeUpdate(sql);
                con.Close();
            }
        }
        public static void updateTag(Line l, string currentTables)
        {
            sqlConn con = new sqlConn();
            con.sqlconn(sqlconDBR, "SQL");
            string sql = "";
            sort(l);
            // string p = l.dsX_src.ToString() + "\b" + l.dsX_des.ToString() + "\b" + l.srcg_itemIndex.ToString() + "\b" + l.desg_itemIndex.ToString() + "\b" + l.srcg.Name.ToString() + "\b" + l.desg.Name.ToString();
            sql = "update " + currentTables + " set tag='" + l.tag.ToString() + "' where table1='" + table1 + "'and field1='" + index1 + "' and table2='" + table2 + "' and field2='" + index2 + "'";
            //我可能需要一个id来删除这一行
            con.executeUpdate(sql);
            con.Close();
        }
        public static void updatedirect(string d, string currentTables)
        {
            sqlConn con = new sqlConn();
            con.sqlconn(sqlconDBR, "SQL");
            string sql = "";            
            // string p = l.dsX_src.ToString() + "\b" + l.dsX_des.ToString() + "\b" + l.srcg_itemIndex.ToString() + "\b" + l.desg_itemIndex.ToString() + "\b" + l.srcg.Name.ToString() + "\b" + l.desg.Name.ToString();
            sql = "update " + currentTables + " set direct='" + d + "' where table1='" + table1 + "'and field1='" + index1 + "' and table2='" + table2 + "' and field2='" + index2 + "'";
            //我可能需要一个id来删除这一行
            con.executeUpdate(sql);
            con.Close();
        }
        public static void deleteLine(Line l, string currentTables)
        {
            sqlConn con = new sqlConn();
            con.sqlconn(sqlconDBR, "SQL");
            string sql = "";
            sort(l);
            // string p = l.dsX_src.ToString() + "\b" + l.dsX_des.ToString() + "\b" + l.srcg_itemIndex.ToString() + "\b" + l.desg_itemIndex.ToString() + "\b" + l.srcg.Name.ToString() + "\b" + l.desg.Name.ToString();
            //DELETE FROM Person WHERE LastName = 'Wilson' 
            sql = "delete from " + currentTables + " where table1='" + table1 + "'and field1='" + index1 + "' and table2='" + table2 + "' and field2='" +index2 + "'";
            //我可能需要一个id来删除这一行
            con.executeUpdate(sql);
            con.Close();
        }

        public static List<string> readLine(string currentTables)
        {
            sqlConn con = new sqlConn();
            con.sqlconn(sqlconDBR, "SQL");
            string sql = "";
            sql = "select * from " + currentTables;
            DataTable dt=con.getVector(sql);
            List<string> lineList = new List<string>();
            string line = "";
            if (dt!=null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    line = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        line += dt.Rows[i][j].ToString() + "\b";
                    }
                    if (lineList.Contains(line)) continue;
                    lineList.Add(line);

                }
            }
            return lineList;
        }
        public static void sort(Line l)
        {
            if (string.Compare(l.srcg.Text, l.desg.Text) > 0)
            {
                table1 = l.srcg.Text;
                table2 = l.desg.Text;
                index1 = l.srcg_itemIndex.ToString();
                index2 = l.desg_itemIndex.ToString();
            }
            else
            {
                table1 = l.desg.Text;
                table2 = l.srcg.Text;
                index1 = l.desg_itemIndex.ToString();
                index2 = l.srcg_itemIndex.ToString();
            }
        }
        /// <summary>
        /// 返回-1是没重复。大于-1的则是存在的线的指向
        /// </summary>
        /// <param name="l"></param>
        /// <returns></returns>
        public static int checkrepeat(Line l, string currentTables)
        {
            sqlConn con = new sqlConn();
            con.sqlconn(sqlconDBR, "SQL");
            string sql = "";
            sort(l);
            // string p = l.dsX_src.ToString() + "\b" + l.dsX_des.ToString() + "\b" + l.srcg_itemIndex.ToString() + "\b" + l.desg_itemIndex.ToString() + "\b" + l.srcg.Name.ToString() + "\b" + l.desg.Name.ToString();
            //DELETE FROM Person WHERE LastName = 'Wilson' 
            sql = "select * from " + currentTables + " where table1='" + table1 + "'and field1='" + index1 + "' and table2='" + table2 + "' and field2='" + index2 + "'";
            //我可能需要一个id来删除这一行
            DataTable dt=con.getVector(sql);
            con.Close();
            if (dt.Rows.Count > 0)
            {
                return int.Parse(dt.Rows[0]["direct"].ToString());
            }
            else return -1;
        }

        
    }
}
