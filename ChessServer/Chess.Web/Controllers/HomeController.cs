using Chess.Entity;
using ChessServer.Game.DAL;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dapper;

namespace Chess.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            string idnum = Request.Params["idnum"];
            if (!String.IsNullOrEmpty(idnum))
            {
                using (MySqlConnection conn = new MySqlConnection(DBBase.ConnectStr))
                {
                    string sql = "SELECT b.BuyTime as BuyTime,s.Description as GoodsName,s.Price as Cost,b.CostDiamond as Diamon from buyrecord b LEFT JOIN store s on b.GoodsID=s.ID where b.BuyerID=" + idnum + " ORDER BY BuyTime DESC";
                    ViewBag.Data = conn.Query<UserPayLog>(sql).ToList();
                }
            }
            else
                ViewBag.Data = null;
            return View();
        }

        public ActionResult AnalysisGoods()
        {
            string goodsid = Request.Params["goodsid"];
            if (!String.IsNullOrEmpty(goodsid))
            {
                using (MySqlConnection conn = new MySqlConnection(DBBase.ConnectStr))
                {
                    string sql = "SELECT a.NickName,b.CostDiamond,b.NowDiamond,b.BuyTime from buyrecord b LEFT JOIN account a on b.BuyerID=a.ID where b.GoodsID=" + goodsid + " ORDER BY BuyTime DESC";
                    ViewBag.Data = conn.Query<GoodsPayLog>(sql).ToList();
                }
            }
            else
            {
                ViewBag.Data = null;
            }
            using (MySqlConnection conn = new MySqlConnection(DBBase.ConnectStr))
            {
                string sql = "SELECT ID,Description from store";
                ViewBag.Goods = conn.Query<EStore>(sql).ToList();
            }
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Download()
        {
            Response.Redirect("http://www.kawumei.com/Download/chess.apk");
            return View();
        }

        public ActionResult DataAnalysis()
        {
            TotalNum statistics = new TotalNum();
            using (MySqlConnection conn = new MySqlConnection(DBBase.ConnectStr))
            {
                string sql = "SELECT count(*) as ID from account";
                IEnumerable<EStore> list = conn.Query<EStore>(sql);
                if (list.Count() > 0)
                {
                    statistics.TotalRegistUser = list.FirstOrDefault().ID;
                }

                string sql2 = string.Format("SELECT count(*) as ID from account where CreateTime>='{0}' and CreateTime<'{1}'", DateTime.Now.Date, DateTime.Now.Date.AddDays(1));
                IEnumerable<EStore> list2 = conn.Query<EStore>(sql2);
                if (list2.Count() > 0)
                {
                    statistics.TodayRegistUser = list2.FirstOrDefault().ID;
                }

                string sql3 = "SELECT SUM(s.Price) as ID from buyrecord b LEFT JOIN store s on b.GoodsID=s.ID and s.ID>=9 and s.ID<20";
                IEnumerable<EStore> list3 = conn.Query<EStore>(sql3);
                if (list3.Count() > 0)
                {
                    statistics.TotalRecharge = list3.FirstOrDefault().ID;
                }

                string sql4 = string.Format("SELECT SUM(s.Price) as ID from buyrecord b LEFT JOIN store s on b.GoodsID=s.ID and s.ID>=9 and s.ID<20 and b.BuyTime>='{0}' and b.BuyTime<'{1}'", DateTime.Now.Date, DateTime.Now.Date.AddDays(1)); ;
                IEnumerable<EStore> list4 = conn.Query<EStore>(sql4);
                if (list4.Count() > 0)
                {
                    statistics.TodayRecharge = list4.FirstOrDefault().ID;
                }
            }
            ViewBag.Data = statistics;
            return View();
        }
    }

    public class TotalNum
    {
        public int TotalRegistUser { get; set; }
        public int TodayRegistUser { get; set; }
        public int TotalRecharge { get; set; }
        public int TodayRecharge { get; set; }
    }

    public class UserPayLog
    {
        public DateTime BuyTime { get; set; }
        public string GoodsName { get; set; }
        public int Cost { get; set; }
        public int Diamon { get; set; }
    }

    public class GoodsPayLog
    {
        public string NickName { get; set; }
        public int CostDiamond { get; set; }
        public int NowDiamond { get; set; }
        public DateTime BuyTime { get; set; }
    }
}