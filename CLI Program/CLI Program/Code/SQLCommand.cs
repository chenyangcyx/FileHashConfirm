using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI_Program.Code
{
    class SQLCommand
    {
        SqliteConnection cn;

        public SQLCommand()
        {
            OpenDataBase();
        }

        //打开数据库
        protected void OpenDataBase()
        {
            cn = new SqliteConnection($"data source={OverAllData.database_path}");
            cn.Open();
        }

        //关闭数据库
        public void CloseDataBase()
        {
            cn.Close();
        }

        //创建表
        public void CreateTable()
        {
            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = cn;
            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {OverAllData.table_name}({OverAllData.column_1_name} TEXT PRIMARY KEY,{OverAllData.column_2_name } TEXT)";
            cmd.ExecuteNonQuery();
        }

        //插入数据
        public void InsertData(string filename,string hashvalue)
        {
            try
            {
                SqliteCommand cmd = new SqliteCommand();
                cmd.Connection = cn;
                cmd.CommandText = $"INSERT INTO {OverAllData.table_name} VALUES('{filename.Replace("'", "''")}','{hashvalue}')";
                cmd.ExecuteNonQuery();
            }
            catch (Microsoft.Data.Sqlite.SqliteException)
            {
                Console.WriteLine("记录已经存在！");
            }
        }

        //获取表的所有数据
        public void GetAllDataFromDB()
        {
            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = cn;
            cmd.CommandText = $"SELECT * FROM {OverAllData.table_name}";
            SqliteDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Console.WriteLine($"filename: {dr.GetString(0)}\nhashvalue: {dr.GetString(1)}\n");
            }
        }

        //获取某一个filename的对应hashvalue
        public string GetHashValue(string filename)
        {
            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = cn;
            cmd.CommandText = $"SELECT * FROM {OverAllData.table_name} WHERE {OverAllData.column_1_name}='{filename.Replace("'","''")}'";
            SqliteDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
                return dr.GetString(1);
            else
                return null;
        }

        public string GetHashValue(FileInfo fi)
        {
            SqliteCommand cmd = new SqliteCommand();
            cmd.Connection = cn;
            cmd.CommandText = $"SELECT * FROM {OverAllData.table_name} WHERE {OverAllData.column_1_name}='{fi.Name.Replace("'", "''")}'";
            SqliteDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
                return dr.GetString(1);
            else
                return null;
        }
    }
}
