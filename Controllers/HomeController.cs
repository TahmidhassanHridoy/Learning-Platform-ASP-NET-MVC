﻿using AskNLearn.Models;
using AskNLearn.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AskNLearn.Controllers
{
    public class HomeController : Controller
    {
        AskNLearnEntities dbObj = new AskNLearnEntities();
        public ActionResult Index()
        {

            var users = dbObj.Users.ToList();

           




            var Courses = dbObj.Courses.ToList();
            List<CoursesModel> CourseList = new List<CoursesModel>();
            foreach(var course in Courses)
            {
                CoursesModel obj = new CoursesModel();
                obj.coid = course.coid;
                obj.uid = course.uid;
                obj.title = course.title;
                obj.details = course.details;
                obj.price = course.price;
                obj.upVote = course.upVote;
                obj.downVote = course.downVote;
                obj.dateTime = course.dateTime;
                obj.thumbnail = course.thumbnail;
                CourseList.Add(obj);
            }
            ViewBag.CourseList = CourseList;



            var UsersInfo = dbObj.Users.Where(u => u.userType.Equals("Instructor") && u.approval.Equals("active")).ToList();
            List<UsersProfile> Users = new List<UsersProfile>();
            foreach(var u in UsersInfo)
            {
                UsersProfile obj = new UsersProfile();
                obj.uid = u.uid;
                obj.username = u.username;
                obj.name = u.name;
                obj.email = u.email;
                obj.dob = u.dob;
                obj.gender = u.gender;
                obj.proPic = u.proPic;
                Users.Add(obj);

            }
            ViewBag.UserList = Users;


            List<UserPostModel> UsersPost = new List<UserPostModel>();

            var data = (from u in dbObj.Users
                        join up in dbObj.Posts on u.uid equals up.uid
                        select new { u.name, u.userType, u.username, u.proPic,up.title,up.details,up.upVote,up.downVote,up.dateTime }).ToList();


            foreach(var d in data)
            {
                UserPostModel obj = new UserPostModel();

                obj.name = d.name;
                obj.username = d.username;
                obj.userType = d.userType;
                obj.title = d.title;
                obj.details = d.details;
                obj.upVote = d.upVote;
                obj.downVote = d.downVote;
                obj.proPic = d.proPic;
                obj.dateTime = d.dateTime;
                UsersPost.Add(obj);
            }
            ViewBag.UserPostList = UsersPost;

            return View(users);
        }
        
        [HttpGet]
        public ActionResult UserDetails(int? iid)
        {

            ProfileUpdateModel ip = new ProfileUpdateModel();

            var user = (from u in dbObj.Users
                        join ui in dbObj.UsersInfoes on u.uid equals ui.uid
                        select new
                        {
                            u.uid,
                            u.name,
                            u.username,
                            u.email,
                            u.password,
                            u.dob,
                            u.gender,
                            u.userType,
                            u.proPic,
                            u.dateTime,
                            ui.eduInfo,
                            ui.reputation,
                            ui.currentPosition
                        }).ToList();


            foreach (var item in user)
            {
                if (item.uid.Equals(iid))
                {
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
                    ip.userType = item.userType;
                }

            }
            return View(ip);
        }


    }
}