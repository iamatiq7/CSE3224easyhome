using projectsd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projectsd.Controllers
{
    public class ButtonsController : Controller
    {

        private dbf db = new dbf();

        //
        // GET: /Buttons/



        public ActionResult ShowVid(string vid)
        {
            
            return View();
        }



        public ActionResult Terminate(int? rentid)
        {
            //setting null at tenantid
            var rent = (from i in db.Rentealseats
                        where i.id == rentid
                        select i).First();

            rent.TenantId = null;
            db.SaveChanges();




            return RedirectToAction("Details", "User", new { id = (int?)Session["user"]});
        }

        public ActionResult TerminateOwn(int? rentid)
        {
            //setting null at tenantid
            var rent = (from i in db.Rentealseats
                        where i.id == rentid
                        select i).First();

            rent.TenantId = null;
            db.SaveChanges();




            return RedirectToAction("Details", "Rents", new { id = rentid });
        }
        
        public ActionResult DeleteRent() 
        {

            int? rentid = (int?)Session["rent"];
            var allReqForCurrRent = (from i in db.Requests
                                    where i.rentid == rentid
                                    select i).ToList();

            foreach (var item in allReqForCurrRent)
            {
                db.Requests.Remove(item);

            }
            db.SaveChanges();


            var thisRentAtDb = (from i in db.Rentealseats
                                   where i.id == rentid
                                   select i).First();

            thisRentAtDb.price = 0;
            db.SaveChanges();


            return RedirectToAction("RentList", "Room", new { roomId = (int?)Session["room"] });    
        }
    

        public ActionResult DeleteApertment() {

            int? currentApertment = (int?)Session["room"];



           


            var allRentsInCurrAprtmnt = (from i in db.Rentealseats
                                    where i.RoomId == currentApertment
                                    select i).ToList();

            foreach (var item in allRentsInCurrAprtmnt)
            {
                item.price = 0;

            }
            db.SaveChanges();

            var thisAprtmntAtDb = (from i in db.Rooms
                                   where i.id == currentApertment
                                   select i).First()  ;

            thisAprtmntAtDb.sqft = 0;
            db.SaveChanges();

            return RedirectToAction("Index","Room");    

        }
    

        public ActionResult Chat()
        {
            if (Session["log"] != "in")
            {
                return RedirectToAction("Login","User");
            }
            
            int? guest = (int?)Session["visit"];
            int? user = (int?)Session["user"];
            
            //check chatroom exists or not
            var isroom = (from i in db.chrooms
                          where (i.p1 == guest && i.p2 == user) || (i.p2 == guest && i.p1 == user)
                          select i).FirstOrDefault();

            int? chid1;
            if (isroom == null)
            {
                var x = new chroom
                {
                    p1 = guest,
                    p2 = user,
                    time = DateTime.Now
                };

                db.chrooms.Add(x);
                db.SaveChanges();

                var y = new Message
                {

                    chroom = x.id,
                    recieverid = guest,
                    senderid = user,
                    text = "Hello, there",
                    time = DateTime.Now,
                    stat = "unseen"
                    
                };

                db.Messages.Add(y);
                db.SaveChanges();


                chid1 = x.id;
            }
            else
            {
                chid1 = (int?)isroom.id;
            }



            return RedirectToAction("Inbox", "Chat", new { chid = chid1});

        }




	}
}