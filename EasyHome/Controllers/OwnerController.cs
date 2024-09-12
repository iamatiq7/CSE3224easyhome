using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using projectsd.Models;

namespace projectsd.Controllers
{
    public class OwnerController : Controller
    {
        private dbf db = new dbf(); 
        Models.View.Owner me = new Models.View.Owner();
        // GET: /Owner/
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Owner/Details/5



        public ActionResult Requests()
        {
            return Content("");
        }

        public ActionResult Messages()
        {
            return Content("");
        }

        public ActionResult Posts(int? id)
        {


            if (id == null)
            {
                return Content("Invalid data request");
            }
            var rms = (from d in db.Rooms
                       join c in db.Owners on d.ownerid equals c.id
                       join f in db.Rentealseats on d.id equals f.RoomId

                       where c.id == id
                       select f.id
                      ).ToList();
            foreach (var items in rms)
            {
                me.myroomids.Add(items);
            }




            return View(me);
        }





    }
}
