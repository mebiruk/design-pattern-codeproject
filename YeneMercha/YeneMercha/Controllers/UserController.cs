using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YeneMercha.Models;

namespace YeneMercha.Controllers
{
    public class UserController : Controller
    {
        DBConnection db = new DBConnection();
        string user;

        // GET: User
        public ActionResult Home()
        {
            user = (string)Session["username"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                    var election = db.Elections.Include(x => x.Catagory);
                    return View(election.ToList());
            }
        }

        public ActionResult Entertainment()
        {
            user = (string)Session["username"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
               int cid = 3;
               var entertainment = db.Elections.SqlQuery("select * from Election where catagory_id=@id", new SqlParameter("@id", cid)).ToList();
               return View(entertainment);
            }
        }

        public ActionResult Sport()
        {
            user = (string)Session["username"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                int cid = 4;
                var entertainment = db.Elections.SqlQuery("select * from Election where catagory_id=@id", new SqlParameter("@id", cid)).ToList();
                return View(entertainment);
            }
        }

        public ActionResult Science()
        {
            user = (string)Session["username"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                int cid = 1;
                var entertainment = db.Elections.SqlQuery("select * from Election where catagory_id=@id", new SqlParameter("@id", cid)).ToList();
                return View(entertainment);
            }
        }

        public ActionResult ElectionDetail(int? id)
        {
            user = (string)Session["username"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var detail = db.Candidates.SqlQuery("select * from Candidate where election_id=@id", new SqlParameter("@id", id)).ToList();
                return View(detail);
            }
        }

        public ActionResult vote(int? id,Vote vote)
        {
            user = (string)Session["username"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                vote.user_id =(int)Session["id"];
                vote.candidate_id = id;
                db.Votes.Add(vote);
                db.SaveChanges();
                return RedirectToAction("Home");
            }
        }

        public ActionResult Result()
        {
            user = (string)Session["username"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var result = db.Votes.Include(x=>x.Candidate).ToList();
                var data = db.Votes.SqlQuery("select count(*) from vote where candidate_id=@id",new SqlParameter("@id",result[0].candidate_id)).ToList();
                return View(result);
            }
        }

        public ActionResult MyVote(Vote vote)
        {
            user = (string)Session["username"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                string name = (string)Session["username"];
                var data = db.Users.Where(x => x.username == name).ToList();
                var id = data[0].Id;
                
                var my = db.Votes.SqlQuery("select * from Vote where user_id=@id", new SqlParameter("@id", id)).ToList();
                return View(my);
            }
        }

    }
}