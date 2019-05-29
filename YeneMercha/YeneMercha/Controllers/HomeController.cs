using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YeneMercha.Models;

namespace YeneMercha.Controllers
{
    public class HomeController : Controller
    {
        DBConnection db = new DBConnection();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            var data = db.Users.Where(a => a.username == user.username && a.password == user.password).ToList();
            if (data.Count() > 0)
            {
                Session["id"] = data[0].Id;
                Session["username"] = data[0].username;
                Session["role"] = data[0].role_id;
                
                FormsAuthentication.SetAuthCookie(data[0].username, false);
                if (data[0].role_id == 0)
                {
                    return RedirectToAction("../Admin/Home");
                }
                else if(data[0].role_id==1)
                {
                    return RedirectToAction("../User/Home");
                }
            }
            else
            {
                ViewBag.Message = "Incorrect username or password";
            }
            ModelState.Clear();
            return View();
        }

        public ActionResult Register()
        {
            List<Question> list = db.Questions.ToList();
            ViewBag.QuestionList = new SelectList(list,"id","question1");
            return View();
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
                user.role_id = 1;
                db.Users.Add(user);
                db.SaveChanges();
                ViewBag.WarningMessage = "Register successfully";
            }
            ModelState.Clear();
            return View();
        }

        public ActionResult ForgetPassword()
        {
            List<Question> list = db.Questions.ToList();
            ViewBag.QuestionList = new SelectList(list, "id", "question1");
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(User user)
        {
            List<Question> list = db.Questions.ToList();
            ViewBag.QuestionList = new SelectList(list, "id", "question1");

            var data = db.Users.Where(a => a.username == user.username && a.first_name == user.first_name && a.last_name == user.last_name && a.email == user.email && a.address == user.address && a.question_id==user.question_id && a.answer==user.answer).ToList();
            if (data.Count() > 0)
            {
                ViewBag.Message = "your password is " + data[0].password;
            }
            else
            {
                ViewBag.WarningMessage = "No user found.";
            }
            ModelState.Clear();
            return View();
        }

        public ActionResult Logout()
        {
            Session["Id"] = 0;
            Session["username"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}