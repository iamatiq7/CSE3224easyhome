using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using projectsd.Models;
using System.Web.Script.Serialization;

namespace projectsd.Controllers
{
    public class UserController : Controller
    {
        private dbf db = new dbf();
     
         Models.View.User uservm = new Models.View.User();
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        // GET: /User/Details/5
        public ActionResult Details(int? id)
        {
          

            Models.View.User user = new Models.View.User();
            if (id == null)
            {
                return Content("Invalid request");
            }
            //User user = db.Users.Find(id);
            if (user == null)
            {
                return Content("Data not avaliable");
            }
  Session["visit"] = id;
            var u = (from x in db.Users
                     where x.id == id
                     select x 
                     
                     ).FirstOrDefault();

            if (u.OwnerId != null)
            {
                user.isOwner = true;
            }
            if (u.Tenantid != null)
            {
                user.isTenant = true;
            }


            //find reviews
            var revs = (from i in db.userrevs
                        where i.userid == id
                        select i).ToList();


            foreach (var item in revs)
            {
                Models.View.Review v = new Models.View.Review();

                v.reviewid = (int?)item.id;
                v.reviewerid = (int?)item.reveiewerid;
                v.reviewtext = item.review;
                v.reviewerName = (from i in db.Users
                                  where i.id == item.reveiewerid
                                  select i.Name).FirstOrDefault();
                v.reviewerpic = (from i in db.Users
                                 where i.id == item.reveiewerid
                                 select i.pic).FirstOrDefault();

                user.revs.Add(v);

            }

            int? tid = (int?)Session["tid"];
            //find rents
            var rentts = (from i in db.Rentealseats
                          where i.TenantId != null
                        where i.TenantId == tid
                        where i.price != 0
                        select i).ToList();



            foreach (var item in rentts)
            {
                Models.View.Rent v = new Models.View.Rent();

                v.ID = item.id;
                v.startDate = item.startdate;
                v.pic = item.pic;

                user.rents.Add(v);

            }


            user.id = id;
            user.name = u.Name;
            user.email = u.Email;
            user.cell = u.Cell;
            user.address = u.Address;
            user.rating = u.Rating;
            user.gender = u.Gender;
            user.pic = u.pic;
            user.vid = u.VID;
            Session["vid"] = u.VID;
            return View(user);
        }

        // GET: /User/Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create( string name,string cell,string vid, string password, string gender, string email,string address, string propic,string start)
        {

            bool valid = cell.All(c => "0123456789".Contains(c));


            if (valid == true && email != "" && password != "" && vid != "")
            {

                uservm.email = email;
                uservm.cell = cell;
                uservm.gender = gender;
                uservm.pass = password;
                uservm.name = name;
                uservm.address = address;
                uservm.pic = propic;
                

                var tw = new Auth
                {
                    email = uservm.email,
                    pass = uservm.pass
                };

                db.Auths.Add(tw);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.error = "xyz";
                }


                var t = new User //Make sure you have a table called test in DB
                {
                    Name = uservm.name,
                    Rating = 0,
                    Address = uservm.address,
                    Cell = uservm.cell,
                    Gender = uservm.gender,
                    Email = uservm.email,
                    pic = uservm.pic,
                    VID = vid
                };

                db.Users.Add(t);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.error = "xyz";
                    return View();
                }



                return RedirectToAction("Login", "User");
            } else
            {
                ViewBag.error = "zyx";
            }

            if (!valid)
            {
                ViewBag.error = "zyxi2";
            }
            if (vid == "")
            {
                ViewBag.error = "zyxi";
            }
           



            //IFERROR
            return View(); 
        }





        [HttpPost]
        public ActionResult Details(int? rating,string review)
        {

            if (Session["log"] != "in")
            {
                return RedirectToAction("Login", "User");

            }

            var cc = (int?)Session["visit"];

            var up = (from i in db.Users
                      where i.id == cc
                      select i).First();
            int? pr = up.Rating;

            up.Rating = (int?)(pr + rating) / 2;
            db.SaveChanges();

            var newRev = new userrev
            {
                
                reveiewerid = (int?)Session["user"],
                review = review,
                userid = (int?)Session["visit"]
                
            };

            db.userrevs.Add(newRev);
            db.SaveChanges();


            return RedirectToAction("Details", "User", new { id = (int?)Session["visit"] });
        }










        public ActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public ActionResult Login(string email,string pass)
        {
            if (email == null)
            {
                return Content("Invalid request");
            
            }

            var result = (from item in db.Auths
                          where item.email == email
                         
                          select item).Count();
            if (result !=1)
            {
                ViewBag.error = "Invalid";
            }
            Session["log"] = "out";

            if (email != "" && pass != "")
            {
               
                   var x = (from p in db.Users
                          join e in db.Auths
                          on p.Email equals e.email
                          where e.email == email
                          select new{
                              id = p.id,
                              pass = e.pass,
                              pic = p.pic,
                              tid = p.Tenantid
                          }).FirstOrDefault();

                if(x != null){
                    
                   if(x.pass == pass){
                       Session["email"] = email;
                       Session["log"] = "in";
                       Session["pic"] = x.pic;
                       Session["tid"] = x.tid;
                        
                       Session["user"] = x.id;
                      // return Content(ViewBag.userid.ToString());
                       if (Session["need"] != "1")
                       {
                           return RedirectToAction("Index", "Rents"); ;
                       }
                       else
                       {
                           return RedirectToAction("Details", "Rents", new { id = (int?)Session["rentid"]}); ;
                       }
                   }
                   else
                   {
                       ViewBag.error = "Password";
                   }
                }
            }
            else
            {
                ViewBag.error = "empty";
            }

            return View();
        }




        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login","User");
        }



       
    }
}
