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
       
        public static string  currentTables="";
        public static void createTable(string table1, string table2)
        {
            sqlConn con = new sqlConn();
            con.sqlconn("Data Source=ELAB-36\\SQLEXPRESS;Initial Catalog=asj_DBR;Integrated Security=True;Pooling=False","SQL");
            
            string[] t = new string[] { table1, table2 };
            Array.Sort(t);
            string name = string.Join("_", t);
            name = "DBR_" + name;
            currentTables = name;
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
        public static void saveLine(Line l)
        {
            sqlConn con = new sqlConn();
            con.sqlconn("Data Source=ELAB-36\\SQLEXPRESS;Initial Catalog=asj_DBR;Integrated Security=True;Pooling=False", "SQL");
            string sql = "";
           // string p = l.dsX_src.ToString() + "\b" + l.dsX_des.ToString() + "\b" + l.srcg_itemIndex.ToString() + "\b" + l.desg_itemIndex.ToString() + "\b" + l.srcg.Name.ToString() + "\b" + l.desg.Name.ToString();
            sql = "INSERT INTO " + currentTables + " (src_table, src_field,des_table,des_field,property) VALUES ('" + l.srcg.Text + "', '" + l.srcg_itemIndex.ToString() + "', '" + l.desg.Text + "', '" + l.desg_itemIndex.ToString() + "', '属性暂定')";
            //我可能需要一个id来删除这一行
            con.executeUpdate(sql);
            con.Close();
            
        }
        public static void deleteLine(Line l)
        {
            sqlConn con = new sqlConn();
            con.sqlconn("Data Source=ELAB-36\\SQLEXPRESS;Initial Catalog=asj_DBR;Integrated Security=True;Pooling=False", "SQL");
            string sql = "";
            // string p = l.dsX_src.ToString() + "\b" + l.dsX_des.ToString() + "\b" + l.srcg_itemIndex.ToString() + "\b" + l.desg_itemIndex.ToString() + "\b" + l.srcg.Name.ToString() + "\b" + l.desg.Name.ToString();
            //DELETE FROM Person WHERE LastName = 'Wilson' 
            sql = "delete from " + currentTables + " where src_table='" + l.srcg.Text + "'and src_field='" + l.srcg_itemIndex.ToString() + "' and des_table='" + l.desg.Text + "' and des_field='" + l.desg_itemIndex.ToString() + "'";
            //我可能需要一个id来删除这一行
            con.executeUpdate(sql);
            con.Close();
        }

        public static List<string> readLine()
        {
            sqlConn con = new sqlConn();
            con.sqlconn("Data Source=ELAB-36\\SQLEXPRESS;Initial Catalog=asj_DBR;Integrated Security=True;Pooling=False", "SQL");
            string sql = "";
            sql = "select * from " + currentTables;
            DataTable dt=con.getVector(sql);
            List<string> lineList = new List<string>();
            string line = "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                line = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    line += dt.Rows[i][j].ToString()+"\b";
                }
                if (lineList.Contains(line)) continue;
                lineList.Add(line);

            }
            return lineList;
        }
        
    }
}
