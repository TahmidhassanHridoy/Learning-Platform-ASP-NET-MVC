﻿using AskNLearn.Models;
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
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User Model)
        {
            if (ModelState.IsValid)
            {
                var data = (from u in dbObj.Users
                            where u.username.Equals(Model.username) &&
                            u.password.Equals(Model.password)
                            select u).FirstOrDefault();

                //User data = dbObj.Users.Where(x => x.username == Model.username && x.password == Model.password).FirstOrDefault();

                //var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UsersLoginModel>());

                //var mapper = new Mapper(config);

                //var Data = mapper.Map<UsersLoginModel>(data);

                if (data != null)
                {
                    //FormsAuthentication.SetAuthCookie(data.Username, false);
                    //Session["Username"] = data.Username;
                    return RedirectToAction("../Dashboard/Dashboard");
                }
                else if (data == null)
                {
                    ViewBag.Message = "Your Username Or Password May Be Incorrect";
                }
            }
            else
            {
                ViewBag.Message = "Your model state is not correct";
            }
            return View();
        }



    }
}