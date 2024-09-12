using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using projectsd.Models;
using System.Web;
using System.Web.Mvc;

namespace projectsd.Controllers
{
    public class PasswordController : Controller
    {
        //
        // GET: /Password/

        private dbf db = new dbf();


        public ActionResult Page1()
        {



            return View();
        }

        [HttpPost]
        public ActionResult Page1(string email)
        {
            if (email != "")
            {

                int i = 0;
                Session["remail"] = email;
                
                
                    sendMail(email);

            }
            else
            {
                ViewBag.email = "";
                return View("Page1");
            }

            return View("Page2");
        }





        public ActionResult Page2()
        {



            return View();
        }

        [HttpPost]
        public ActionResult Page2(string code)
        {

            bool valid = false;

            string xcode = Session["otp"].ToString();

            if(code.Equals(xcode)){
                return Content("Abbe sala");
                valid  = true;
            }

            if(valid){
                return Content(valid.ToString());
                //return View("Page3");
            }else{
                
                ViewBag.email = ":";
                return View("Page1");
            }
        }

        public ActionResult Page3()
        {



            return View();
        }

        [HttpPost]
        public ActionResult Page3(string newpass)
        {
            //update database
            var getEmail = (from i in db.Auths
                            where i.email == Session["remail"].ToString()
                            select i
                                ).First();

            if(getEmail == null){
                ViewBag.email = ";";
                return View("Page1");
            }

            getEmail.pass = newpass;
            db.SaveChanges();

            ViewBag.error = "::";
            return View("Login","User");
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        int flag = 0;
        public void sendMail(string email)
        {

            var x = RandomString(6);
            
            Session["otp"] = x;

            var smtpClient = new SmtpClient("smtp-relay.sendinblue.com")
            {
                Port = 587,
                Credentials = new System.Net.NetworkCredential("muhaiminulkabir32@gmail.com", "zRNWhX1fnI4yOc2L"),
                EnableSsl = true,
            };

            smtpClient.Send("noreply@easyrent.com", email, "Reset password", x);
            smtpClient.Dispose();
        }



	}
}