using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Lemon.Extensions;
using Lemon;
using System.Configuration;

namespace ChessServer.Game.DAL
{
    public class DBBase
    {
        public static string ConnectStr = ConfigurationManager.ConnectionStrings["mysqlconnect"].ConnectionString;

        static DBBase()
        {
        }

        public static int ExcuteCustom(string sqlstr)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectStr))
            {
                return conn.Execute(sqlstr);
            }
        }

        public static object Create(object obj)
        {
            if (obj == null)
                return null;
            using (MySqlConnection conn = new MySqlConnection(ConnectStr))
            {
                string insertSql = MySqlConverter.ToInsertSQL(obj);
                insertSql += ";SELECT @@IDENTITY as id;";
                var newrow = conn.Query(insertSql).FirstOrDefault() as IDictionary<string, object>;
                int newid = newrow["id"].ToInt();
                obj.SetProperty("ID", newid);
                return obj;
            }
        }
        public static object Change(object obj, string wherestr = null)
        {
            if (obj == null)
                return null;
            int id = obj.GetProperty("ID").ToInt();
            using (MySqlConnection conn = new MySqlConnection(ConnectStr))
            {
                string sql = MySqlConverter.ToUpdateSQL(obj, wherestr);
                int ret = conn.Execute(sql);
                if (ret > 0)
                {
                    return obj;
                }
                else
                    return null;
            }
        }
        public static bool Delete<T>(int key, string wherestr = null)
        {
            return Delete(typeof(T), key, wherestr);
        }
        public static bool Delete(Type type, int key, string wherestr = null)
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectStr))
            {
                string sql = MySqlConverter.ToDeleteSQL(type, key, wherestr);
                int ret = conn.Execute(sql);
                if (ret > 0)
                {
                    return true;
                }
                else
                    return false;
            }
        }
        public static List<T> Query<T>(string wherestr = null) where T : class
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectStr))
            {
                string sql = MySqlConverter.ToSelectSQL(typeof(T), wherestr);
                return conn.Query<T>(sql).ToList();
            }
        }
        public static List<T> QueryCustom<T>(string sqlstr) where T : class
        {
            using (MySqlConnection conn = new MySqlConnection(ConnectStr))
            {
                return conn.Query<T>(sqlstr).ToList();
            }
        }
        public static T Get<T>(int id)
        {
            if (id <= 0)
                return default(T);
            using (MySqlConnection conn = new MySqlConnection(ConnectStr))
            {
                string sql = MySqlConverter.ToSelectSQL(typeof(T), "ID=" + id);
                return conn.Query<T>(sql).FirstOrDefault();
            }
        }
    }
}
