﻿using AskNLearn.Models;
using AskNLearn.Models.Instructor;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AskNLearn.Controllers
{
    [Authorize]
    public class InstructorController : Controller
    {
        public ActionResult Dashboard()
        {
            if (Session["userType"].Equals("Instructor"))
            {
                AskNLearnEntities db = new AskNLearnEntities();
                InsDashModel model = new InsDashModel();
                int uid = (int)Session["uid"];
                var data = db.Users.ToList();
                var course = db.Courses.ToList();
                var enrolledusers = (from c in db.Courses
                                     join eu in db.EnrolledUsers on c.coid equals eu.coid
                                     where c.uid == uid
                                     select new
                                     {
                                         eu.eid
                                     }).Count();
                model.EnrolledLearners = enrolledusers;
                foreach (var item in course)
                {
                    if (item.uid.Equals(Session["uid"]))
                    {
                        model.PostedCourseCount++;
                    }
                }
                foreach (var item in data)
                {
                    if (item.userType.Equals("Learner"))
                    {
                        model.LernerCount++;
                    }
                }
                var posts = db.Posts.OrderByDescending(p=> p.pid).ToList();
                ViewBag.Posts = posts;
                return View(model);
            }
            else
            {
                ViewBag.Message = "You Are Authorized As " + Session["userType"] + " You Cannot Acces This Page";
                return RedirectToAction("Login", "Users");
            }
        }
        [HttpGet]
        public ActionResult Profile()
        {
            if (Session["userType"].Equals("Instructor"))
            {
                AskNLearnEntities dbObj = new AskNLearnEntities();
                InstructorProfile ip = new InstructorProfile();
                if (Session["uid"] != null)
                {
                    var uid = (int)Session["uid"];
                    var user = (from u in dbObj.Users
                                join ui in dbObj.UsersInfoes on u.uid equals ui.uid
                                where u.uid == uid
                                select new
                                {
                                    u.uid,
                                    u.name,
                                    u.username,
                                    u.email,
                                    u.password,
                                    u.dob,
                                    u.gender,
                                    u.proPic,
                                    u.dateTime,
                                    ui.eduInfo,
                                    ui.reputation,
                                    ui.currentPosition
                                }).ToList();
                    foreach (var item in user)
                    {
                        //if (item.uid.Equals(Session["uid"]))
                        //{
                            ip.uid = item.uid;
                            ip.name = item.name;
                            ip.username = item.username;
                            ip.email = item.email;
                            ip.password = item.password;
                            ip.dob = item.dob;
                            ip.gender = item.gender;
                            ip.proPic = item.proPic;
                            ip.dateTime = item.dateTime;
                            ip.eduInfo = item.eduInfo;
                            ip.reputation = item.reputation;
                            ip.currentPosition = item.currentPosition;
                        //}
                    }
                }
                return View(ip);
            }
            else
            {
                ViewBag.Message = "You Are Authorized As " + Session["userType"] + " You Cannot Acces This Page";
                return RedirectToAction("Login", "Users" , new { Message = ViewBag.Message });
            }
        }
        [HttpPost]
        public ActionResult Profile(InstructorProfile profile)
        {
            if (Session["userType"].Equals("Instructor"))
            {
                if (ModelState.IsValid)
                {
                    AskNLearnEntities db = new AskNLearnEntities();
                    var obj = db.Users.Where(value => value.uid == profile.uid).First();
                    obj.name = profile.name;
                    obj.username = profile.username;
                    obj.email = profile.email;
                    obj.password = profile.password;
                    obj.dob = profile.dob;
                    obj.gender = profile.gender;
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    var usdata = db.UsersInfoes.Where(value => value.uid == profile.uid).First();
                    usdata.eduInfo = profile.eduInfo;
                    usdata.currentPosition = profile.currentPosition;
                    db.Entry(usdata).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Profile");
                }
                return View(profile);
            }
            else
            {
                ViewBag.Message = "You Are Authorized As " + Session["userType"] + " You Cannot Acces This Page";
                return RedirectToAction("Login", "Users");
            }
        }
        [HttpPost]
        public ActionResult UpdateProPic(InstructorProfile profile)
        {
            if (Session["userType"].Equals("Instructor"))
            {
                if (profile.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(profile.ImageFile.FileName);
                    string extension = Path.GetExtension(profile.ImageFile.FileName);
                    fileName = fileName + "-" + profile.username + "-" + DateTime.Now.ToString("yyy-MM-d") + extension;
                    profile.proPic = "/Content/Instructor/ProPic/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Content/Instructor/ProPic/"), fileName);
                    profile.ImageFile.SaveAs(fileName);
                }
                else
                {
                    profile.proPic = "/Content/Instructor/ProPic/noPropic.jpg";
                }
                AskNLearnEntities db = new AskNLearnEntities();
                var obj = db.Users.Where(value => value.uid == profile.uid).First();
                obj.proPic = profile.proPic;
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Profile", "Instructor");
            }
            else
            {
                ViewBag.Message = "You Are Authorized As " + Session["userType"] + " You Cannot Acces This Page";
                return RedirectToAction("Login", "Users");
            }
        }
        [HttpGet]
        public ActionResult AddCourse()
        {
            if (Session["userType"].Equals("Instructor"))
            {
                return View();
        }
            else
            {
                ViewBag.Message = "You Are Authorized As " + Session["userType"] + " You Cannot Acces This Page";
                return RedirectToAction("Login", "Users");
    }
}
        [HttpPost]
        public ActionResult AddCourse(CourseModel c)
        {
            if (Session["userType"].Equals("Instructor"))
            {
                if (ModelState.IsValid)
                {
                    var uid = (int)Session["uid"];
                        if (c.ImageFile != null)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(c.ImageFile.FileName);
                            string extension = Path.GetExtension(c.ImageFile.FileName);
                            fileName = fileName + "-" + DateTime.Now.ToString("yyy-MM-d") + extension;
                            c.thumbnail = "Content/Instructor/Courses/Images/" + fileName;
                            fileName = Path.Combine(Server.MapPath("~/Content/Instructor/Courses/Images/"), fileName);
                            c.ImageFile.SaveAs(fileName);
                        }
                        else
                        {
                            c.thumbnail = "Content/Instructor/Courses/Images/noPropic.jpg";
                        }


                    AskNLearnEntities db = new AskNLearnEntities();
                    Cours cours = new Cours();
                    cours.uid = uid;
                    cours.title = c.title;
                    cours.details = c.details;
                    cours.price = c.price;
                    cours.thumbnail = c.thumbnail;
                    cours.upVote = 1;
                    cours.downVote = 0;
                    cours.dateTime = DateTime.Now;
                    db.Courses.Add(cours);
                    db.SaveChanges();
                    int x = cours.coid;
                    return RedirectToAction("UpdateCourse", new { id = x });
                }
                return View();
            }
            else
            {
                ViewBag.Message = "You Are Authorized As " + Session["userType"] + " You Cannot Acces This Page";
                return RedirectToAction("Login", "Users");
            }
        }
        [HttpGet]
        public ActionResult UpdateCourse(int id)
        {
            AskNLearnEntities db = new AskNLearnEntities();
            var check = (from d in db.Documents
                        where d.coid.Equals(id)
                        select d).FirstOrDefault();

            var quiz = (from q in db.Quizes
                        where q.coid.Equals(id)
                        select q).ToList();
            ViewBag.Quizes = quiz;
            if (check != null)
            {
                List<CourseViewModel> courseModel = new List<CourseViewModel>();
                var course = (from c in db.Courses
                              join d in db.Documents on c.coid equals d.coid
                              where c.coid == id
                              select new
                              {
                                  c.coid,
                                  c.uid,
                                  c.title,
                                  c.details,
                                  c.price,
                                  c.upVote,
                                  c.downVote,
                                  c.dateTime,
                                  c.thumbnail,
                                  d.imageTitle,
                                  d.image,
                                  d.videoTitle,
                                  d.videoLink,
                                  d.docTitle,
                                  d.docs
                              }).ToList();
                foreach (var item in course)
                {
                    CourseViewModel cm = new CourseViewModel();
                    cm.coid = item.coid;
                    cm.uid = item.uid;
                    cm.title = item.title;
                    cm.details = item.details;
                    cm.price = item.price;
                    cm.upVote = item.upVote;
                    cm.downVote = item.downVote;
                    cm.dateTime = item.dateTime;
                    cm.thumbnail = item.thumbnail;
                    cm.imageTitle = item.imageTitle;
                    cm.image = item.image;
                    cm.videoTitle = item.videoTitle;
                    cm.videoLink = item.videoLink;
                    cm.docTitle = item.docTitle;
                    cm.docs = item.docs;
                    courseModel.Add(cm);
                }
                ViewBag.CourseDoc = courseModel;
                return View();
            }
            else
            {
                var crs = db.Courses.Find(id);
                ViewBag.Course = crs;
                return View();
            }
        }
        [HttpPost]
        public ActionResult UpdateCourse(DocumentsModel d)
        {
            int x = d.coid;
            var videoLink = "";
            if (ModelState.IsValid)
            {
                if (d.videoLink!=null)
                {
                    const string pattern = @"(?:https?:\/\/)?(?:www\.)?(?:(?:(?:youtube.com\/watch\?[^?]*v=|youtu.be\/)([\w\-]+))(?:[^\s?]+)?)";
                    const string replacement = "https://www.youtube.com/embed/$1";
                    var rgx = new Regex(pattern);
                    videoLink = rgx.Replace(d.videoLink, replacement);
                }
                //For image Upload
                if (d.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(d.ImageFile.FileName);
                    string extension = Path.GetExtension(d.ImageFile.FileName);
                    fileName = d.imageTitle+ "-" + DateTime.Now.ToString("yyy-MM-d") + extension;
                    d.image = "Content/Instructor/Courses/Images/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Content/Instructor/Courses/Images/"), fileName);
                    d.ImageFile.SaveAs(fileName);
                }
                //For Documents Upload
                if (d.DocFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(d.DocFile.FileName);
                    string extension = Path.GetExtension(d.DocFile.FileName);
                    fileName = d.docTitle + "-" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + extension;
                    d.docs = "Content/Instructor/Courses/Documents/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Content/Instructor/Courses/Documents/"), fileName);
                    d.DocFile.SaveAs(fileName);
                }


                AskNLearnEntities db = new AskNLearnEntities();
                Document doc = new Document();
                doc.coid = d.coid;
                doc.videoTitle = d.videoTitle;
                if (!videoLink.Equals(""))
                {
                    doc.videoLink = videoLink;
                }
                doc.imageTitle = d.imageTitle;
                doc.image = d.image;
                doc.docTitle = d.docTitle;
                doc.docs = d.docs;
                db.Documents.Add(doc);
                db.SaveChanges();
                return RedirectToAction("UpdateCourse", new { id = x });
            }
            return RedirectToAction("UpdateCourse", new { id = x });
        }
        [AllowAnonymous]
        public ActionResult CourseView(int id)
        {
            AskNLearnEntities db = new AskNLearnEntities();
            if (Session["uid"] != null) {
            int uid = (int)Session["uid"];
            var quiz = (from q in db.Quizes
                            where q.coid.Equals(id)
                            select q).ToList();
            ViewBag.Quizes = quiz;
            var enrlchk = db.EnrolledUsers.Where(value => value.coid == id && value.uid == uid).FirstOrDefault();
            if (enrlchk != null || Session["userType"].Equals("Instructor"))
            {
                List<CourseViewModel> courseModel = new List<CourseViewModel>();
                var course = (from c in db.Courses
                              join d in db.Documents on c.coid equals d.coid
                              where c.coid == id
                              select new
                              {
                                  c.coid,
                                  c.uid,
                                  c.title,
                                  c.details,
                                  c.price,
                                  c.upVote,
                                  c.downVote,
                                  c.dateTime,
                                  d.imageTitle,
                                  d.image,
                                  d.videoTitle,
                                  d.videoLink,
                                  d.docTitle,
                                  d.docs
                              }).ToList();
                foreach (var item in course)
                {
                    CourseViewModel cm = new CourseViewModel();
                    cm.coid = item.coid;
                    cm.uid = item.uid;
                    cm.title = item.title;
                    cm.details = item.details;
                    cm.price = item.price;
                    cm.upVote = item.upVote;
                    cm.downVote = item.downVote;
                    cm.dateTime = item.dateTime;
                    cm.imageTitle = item.imageTitle;
                    cm.image = item.image;
                    cm.videoTitle = item.videoTitle;
                    cm.videoLink = item.videoLink;
                    cm.docTitle = item.docTitle;
                    cm.docs = item.docs;
                    courseModel.Add(cm);
                }
                return View(courseModel);
            }
            else
                {
                    ViewBag.Message = "You Are Authorized As " + Session["userType"] + " You Cannot Acces This Page";
                    return RedirectToAction("Login", "Users");
                }
            }
            else
            {
                ViewBag.Message = "You Are Authorized As " + Session["userType"] + " You Cannot Acces This Page";
                return RedirectToAction("Login", "Users");
            }
        }
        public ActionResult CourseList()
        {
            if (Session["userType"].Equals("Instructor"))
            {
                int uid = (int)Session["uid"];
                AskNLearnEntities db = new AskNLearnEntities();
                var courses = db.Courses.Where(value => value.uid == uid).ToList();

                return View(courses);
            }
            else
            {
                ViewBag.Message = "You Are Authorized As " + Session["userType"] + " You Cannot Acces This Page";
                return RedirectToAction("Login", "Users");
            }
        }

        public ActionResult DeleteCourse(int id)
        {
            AskNLearnEntities db = new AskNLearnEntities();
            var course = db.Courses.Find(id);
            var documents = db.Documents.Where(value => value.coid == id).ToList();
                foreach (var item in documents)
                {
                    db.Documents.Remove(item);
                    db.SaveChanges();
                }
            
                db.Courses.Remove(course);
                db.SaveChanges();
            
            return RedirectToAction("CourseList");
        }
        [AllowAnonymous]
        public ActionResult PdfPreview(string doc)
        {
            string path = Path.Combine(Server.MapPath("~/"), doc);
            return File(path, "application/pdf");
        }
        [AllowAnonymous]
        [HttpGet]
        public ActionResult CourseEnrolment(int id)
        {
                AskNLearnEntities db = new AskNLearnEntities();
                var data = db.Courses.Find(id);
                return View(data);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CourseEnrolment(EnrolledUser e)
        {
            if (Session["userType"] != null && Session["userType"].Equals("Learner"))
            {
                int uid = (int)Session["uid"];
            AskNLearnEntities db = new AskNLearnEntities();
            e.uid = uid;
            e.dateTime = DateTime.Now;
            db.EnrolledUsers.Add(e);
            db.SaveChanges();
            return RedirectToAction("CourseList");
            }
            else
            {
                ViewBag.Message = "You Must Be Authorized As Learner. Login First To Acces This Page";
                return RedirectToAction("Login", "Users");
            }
        }
        public ActionResult EnrolledUsersList()
        {
            int uid = (int)Session["uid"];
            AskNLearnEntities db = new AskNLearnEntities();
            List<EnrolledListModel> emlist = new List<EnrolledListModel>();
            var data = (from c in db.Courses
                        join e in db.EnrolledUsers on c.coid equals e.coid
                        join u in db.Users on e.uid equals u.uid
                        where c.uid == uid
                        select new
                        {
                            e.eid,
                            c.title,
                            u.name,
                            e.dateTime
                        }
                        ).ToList();
            foreach (var item in data)
            {
                EnrolledListModel em = new EnrolledListModel();
                em.eid = item.eid;
                em.courseTitle = item.title;
                em.UserName = item.name;
                em.dateTime = item.dateTime;
                emlist.Add(em);
            }
            return View(emlist);
        }
        public ActionResult DeleteEnrolledUsers(int id)
        {
            AskNLearnEntities db = new AskNLearnEntities();
            var eu = db.EnrolledUsers.Find(id);
            db.EnrolledUsers.Remove(eu);
            db.SaveChanges();
            return RedirectToAction("EnrolledUsersList");
        }
        public ActionResult PostList()
        {
            AskNLearnEntities db = new AskNLearnEntities();
            var data =  db.Posts.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult AddQuiz(Quize q)
        {
            AskNLearnEntities db = new AskNLearnEntities();
            db.Quizes.Add(q);
            db.SaveChanges();
            return RedirectToAction("UpdateCourse", new { id = q.coid });
        }
        [HttpGet]
        public ActionResult Comment(int id)
        {
            AskNLearnEntities db = new AskNLearnEntities();
            var post = (from p in db.Posts
                        join u in db.Users on p.uid equals u.uid
                        where p.pid == id
                        select new
                        {
                            p.pid,
                            p.uid,
                            p.title,
                            p.details,
                            p.upVote,
                            p.downVote,
                            p.dateTime,
                            u.name
                        }).FirstOrDefault();
            PostModel pm = new PostModel();
            pm.pid = post.pid;
            pm.PostedbyUid = post.uid;
            pm.title = post.title;
            pm.details = post.details;
            pm.Postedby = post.name;
            pm.upVote = post.upVote;
            pm.downVote = post.downVote;
            pm.dateTime = post.dateTime;
            var comments = db.Comments.ToList();
            ViewBag.Comments = comments;
            return View(pm);
        }
        [HttpPost]
        public ActionResult Comment(Comment c)
        {
            AskNLearnEntities db = new AskNLearnEntities();
            //int uid = (int)Session["uid"];
            //c.uid = uid;
            //c.upVote = 1;
            //c.downVote = 0;
            //c.dateTime = DateTime.Now;
            db.Comments.Add(c);
            db.SaveChanges();
            return RedirectToAction("Comment", new { id = c.pid });
        }
        }
}