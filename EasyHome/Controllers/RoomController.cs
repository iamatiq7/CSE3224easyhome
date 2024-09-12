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
   
    public class RoomController : Controller
    {
        private dbf db = new dbf();
     
        //
        // GET: /Room/

        int? rm ;
        

        public ActionResult Index()
        {

            var userid = (int?)Session["user"];

            var data = (from u in db.Users
                        where u.id == userid
                        select u
                            ).FirstOrDefault(); 

            List<Models.View.Rooms> room = new List<Models.View.Rooms>();
            var loc = (from c in db.Districts

                       select c.DistrictName

                          ).ToList();

            ViewBag.uiDist = loc;

            var fac = (from x in db.Facilities
                       select x).ToList();
            List<Models.View.Facilities> facobjl = new List<Models.View.Facilities>();

            foreach (var item in fac)
            {
                Models.View.Facilities facobj = new Models.View.Facilities();
                facobj.id = item.id;
                facobj.icon = item.icon;
                facobj.type = item.type;
                facobjl.Add(facobj);
            }

            ViewBag.fac = facobjl;
            if (data.OwnerId != null)
            {


                var oid = data.OwnerId;
                var rooms = (
                   from x in db.Rooms
                   where x.ownerid == oid
                   where x.sqft != 0
                   select x
               ).ToList();
                foreach (var item in rooms)
                {
                    Models.View.Rooms x = new Models.View.Rooms();
                    x.id = item.id;
                    x.totalrents = (from i in db.Rentealseats
                                    join j in db.Rooms on i.RoomId equals j.id
                                    where j.id == item.id
                                    where i.price != 0
                                    select i
                                        ).Count();
                    room.Add(x);
                }




                ViewBag.count = room.Count();
                

                return View(room);
            }
            ViewBag.count = 0;
            return View(room);
        }

        [HttpPost]
        public ActionResult Index(List<string> faco  ,string address,int? no,int? size,int? maxmem,int? floor,string district)
        {
            //for district list genaration
            List<Models.View.Rooms> room = new List<Models.View.Rooms>();
            var userid = (int?)Session["user"];
           
            var ownerCount = (from i in db.Users
                               where i.id == userid
                              select i
                                  ).FirstOrDefault();


            var fac = (from x in db.Facilities
                       select x).ToList();
            List<Models.View.Facilities> facobjl = new List<Models.View.Facilities>();

            foreach (var item in fac)
            {
                Models.View.Facilities facobj = new Models.View.Facilities();
                facobj.id = item.id;
                facobj.icon = item.icon;
                facobj.type = item.type;
                facobjl.Add(facobj);
            }

            ViewBag.fac = facobjl;



            var loc = (from c in db.Districts

                       select c.DistrictName

                      ).ToList();

            ViewBag.uiDist = loc;

            int? oid = ownerCount.OwnerId;
            
            
            try
            {   
                
                if (oid == null)
                {


                  var order = (from x in db.Owners
                               orderby x.id descending
                               select x
                                   ).First();
                    var newid = order.id + 1;

                  var w = new Owner { id = newid };
                    db.Owners.Add(w);
                    db.SaveChanges();
                  
                  var udata = (from u in db.Users
                                   where u.id == userid
                                   select u).First();
                  udata.OwnerId = newid;
                    db.SaveChanges();
                    oid = newid;
                    
                    
                }
                var t = new Room //Make sure you have a table called test in DB
                {
                    address = address,
                    maxmembers = maxmem,
                    noofrooms = no,
                    atfloor = floor,
                    sqft = size,
                    ownerid = oid


                };
                db.Rooms.Add(t);
                db.SaveChanges();

                foreach (var q in faco)
                {
                    var r = (from i in db.Facilities
                             where i.type == q
                             select i.id).FirstOrDefault();
                    
                     var f = new FacilitiesForRoom
                {
                    RoomId = t.id,
                    facilitiesId = r
                };

                     db.FacilitiesForRooms.Add(f);
                     db.SaveChanges();

                }

                var rooms = (
                   from x in db.Rooms
                   where x.ownerid == oid
                   where x.sqft != 0
                   select x
               ).ToList();
                foreach (var item in rooms)
                {
                    Models.View.Rooms x = new Models.View.Rooms();
                    x.id = item.id;
                    x.totalrents = (from i in db.Rentealseats
                                    join j in db.Rooms on i.RoomId equals j.id
                                    where j.id == item.id
                                    where i.price != 0
                                    select i
                                        ).Count();
                    room.Add(x);
                }

            }
            catch (Exception e)
            {
                ViewBag.adderr = 1;
                return View(room);
            }
            ViewBag.adderr = 0;
            return View(room);
        }


        //
        // GET: /Room/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return Content("Invalid request");
            }
            



            return View();
        }

        //
        // GET: /Room/Create
        
        
        public ActionResult Create()
        {
            return View();
        }





        public ActionResult RentList(int? roomId)
        {
            Session["room"] = roomId;
            rm = roomId;
            ViewBag.thisRoomId = roomId;
            var x = (from i in db.Rentealseats
                                join j in db.Rooms on i.RoomId equals j.id
                                where i.RoomId == roomId
                                where i.price != 0
                                select i.id
                     ).ToList();
            
            List<Models.View.Rent> rents = new List<Models.View.Rent>();
            foreach (var item in x)
            {

                Models.View.Rent objcvm = new Models.View.Rent(); // ViewModel

                objcvm.ID = item;

                objcvm.totalReq = (from i in db.Requests
                                   where i.rentid == item
                                   select i).Count();
                rents.Add(objcvm);
            }
            ViewBag.count = rents.Count();
            return View(rents);
        }



        [HttpPost]
        public ActionResult RentList(int? roomId,int? price, DateTime start, string propic)
        {
            ViewBag.thisRoomId = roomId;

            List<Models.View.Rent> rents = new List<Models.View.Rent>();
            

           

            
             var t = new Rentealseat //Make sure you have a table called test in DB
                {
                    price = price,
                    pic = propic,
                    startdate = start,
                    RoomId = roomId

                };
            
            try
            {
                db.Rentealseats.Add(t);
                db.SaveChanges();
                var x = (from i in db.Rentealseats
                         join j in db.Rooms on i.RoomId equals j.id
                         where i.RoomId == roomId
                         select i.id
                    ).ToList();
                foreach (var item in x)
                {

                    Models.View.Rent objcvm = new Models.View.Rent(); // ViewModel

                    objcvm.ID = item;
                    objcvm.totalReq = (from i in db.Requests
                                       join j in db.Rentealseats on i.rentid equals j.id

                                       select i).Count();
                    rents.Add(objcvm);
                }
            }
            catch (Exception e)
            {
                ViewBag.addrentListerr = 1;
                return View(rents);
            }

            ViewBag.addrentListerr = 0;
            ViewBag.count = rents.Count();
             
             
            return View(rents);
            
        }



        //
        // POST: /Room/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Room/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Room/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Room/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }



    }
}
