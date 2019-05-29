using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

using YeneMercha.Models;
using System.Data.SqlClient;

namespace YeneMercha.Controllers
{
    public class AdminController : Controller
    {
        DBConnection db = new DBConnection();
        string name;
        public ActionResult Home(Election e)
        {
            name =(string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login","Home");
            }
            else
            {
                var election = db.Elections.Include(x => x.Candidates);
                return View(election.ToList());
            }

        }

        public ActionResult AddElection()
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                List<Catagory> list = db.Catagories.ToList();
                ViewBag.CatagoryList = new SelectList(list, "id", "name");
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddElection(Election election)
        {

            List<Catagory> list = db.Catagories.ToList();
            ViewBag.CatagoryList = new SelectList(list, "id", "name");
            if (ModelState.IsValid)
            {
                if (db.Elections.Any(x => x.title == election.title && x.catagory_id == election.catagory_id && x.start_date==election.start_date && x.stop_date==election.stop_date))
                {
                    ViewBag.WarnigMessage = "Election Already Exist.";
                    ModelState.Clear();
                    return View();
                }
                ViewBag.Message = "Election Added.";
                db.Elections.Add(election);
                db.SaveChanges();
            }
            ModelState.Clear();
            return View();
        }

        public ActionResult ElectionDetails(int? id)
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Election election = db.Elections.Find(id);
                return View(election);
            }
        }

        public ActionResult EditElection(int? id)
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                List<Catagory> list = db.Catagories.ToList();
                ViewBag.CatagoryList = new SelectList(list, "id", "name");
                Election election = db.Elections.Find(id);
                return View(election);
            }
        }

        [HttpPost]
        public ActionResult EditElection(Election election)
        {
            List<Catagory> list = db.Catagories.ToList();
            ViewBag.CatagoryList = new SelectList(list, "id", "name");
            if (ModelState.IsValid)
            {
                db.Entry(election).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Home");
            }
            return View(election);
        }

        public ActionResult DeleteElection(int? id)
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Election election = db.Elections.Find(id);
                db.Elections.Remove(election);
                db.SaveChanges();
                return RedirectToAction("Home");
            }
        }

        public ActionResult AddCandidate(int id)
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var election = db.Elections.Where(x => x.Id == id).ToList();
                Candidate candidate = new Candidate();
                candidate.title = election[0].title;
                return View(candidate);
            }
        }

        [HttpPost]
        public ActionResult AddCandidate(int id,Candidate candidate, HttpPostedFileBase upload)
        {
            if (ModelState.IsValid)
            {
                
                candidate.election_id = id;
                db.Candidates.Add(candidate);
                db.SaveChanges();
            }
            ModelState.Clear();
            return View();
        }

        public ActionResult Image()
        {
            return View(db.Images.ToList());
        }

        public ActionResult ImageAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImageAdd(Image image,HttpPostedFileBase uplode)
        {
            string fileName = Path.GetFileName(uplode.FileName);
            string mapPath = Server.MapPath("~/images/"+fileName);
            uplode.SaveAs(mapPath);
            image.name = fileName;
            image.content = "";
            db.Images.Add(image);
            db.SaveChanges();
            return RedirectToAction("Image");
        }


        public ActionResult ImageDetail(int? id)
        {
            Image image= db.Images.Find(id);
            return View(image);
        }

        public ActionResult showCandidate()
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var can = db.Candidates.Include(x=>x.Election).ToList();
                return View(can);
            }
        }

        public ActionResult EditCandidate(int id)
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                    Candidate candidate = db.Candidates.Find(id);
                    
                    return View(candidate);
            }
        }

        [HttpPost]
        public ActionResult EditCandidate(Candidate candidate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(candidate).State = System.Data.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("showCandidate");
            }
            return View(candidate);   
        }

        public ActionResult DeleteCandidate(int? id)
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                Vote vote = db.Votes.SqlQuery("select * from Vote where candidate_id=@id",new SqlParameter("@id",id)).FirstOrDefault();
                db.Votes.Remove(vote);
                db.SaveChanges();
                Candidate candidate = db.Candidates.Find(id);
                db.Candidates.Remove(candidate);
                db.SaveChanges();
                return RedirectToAction("showCandidate");
            }
        }

        public ActionResult CandidateDetails(int? id)
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                    Candidate candidate = db.Candidates.Find(id);
                    return View(candidate);
            }
        }

        public ActionResult Register()
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                    List<Question> list = db.Questions.ToList();
                    ViewBag.QuestionList = new SelectList(list, "id", "question1");
                    return View();
            }
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            List<Question> list = db.Questions.ToList();
            ViewBag.QuestionList = new SelectList(list, "id", "question1");
            if (ModelState.IsValid)
            {
                if (db.Users.Any(x => x.username == user.username))
                {
                    ViewBag.WarningMessage = "Username taken use another username";
                    return View("Register", user);
                }
                user.role_id = 0;
                db.Users.Add(user);
                db.SaveChanges();
            }
            ModelState.Clear();
            return View();
        }

        public ActionResult Result()
        {
            name = (string)Session["username"];
            if (name == null)
            {
                return RedirectToAction("Login", "Home");
            }
            else
            {
                var data = db.Votes.Include(x => x.Candidate)
                    .Include(x => x.User).ToList();
                    return View(data);
            }

        }
    }
}