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
    public class RentsController : Controller
    {
        private dbf db = new dbf();
        List<Models.View.Rent> rents = new List<Models.View.Rent>();
        Models.View.Rent room = new Models.View.Rent();
        // GET: /Rents/

        public ActionResult Index()
        {

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


            ViewBag.userid = Session["user"];
            var lrents = (from p in db.Rentealseats
                          join x in db.Rooms on p.RoomId equals x.id
                          where p.TenantId == null
                          where p.price != 0
                          select new
                          {
                              cover = p.pic,
                              ID = p.id,
                              roomno = p.RoomId,
                              price = p.price,
                              location = x.address,
                              start = p.startdate
                          }
                          ).ToList();




            foreach (var item in lrents)
            {

                Models.View.Rent objcvm = new Models.View.Rent(); // ViewModel

                objcvm.ID = item.ID;
                objcvm.pic = item.cover;

                objcvm.posted = new DateTime();
                objcvm.startDate = item.start;
                objcvm.Location = item.location;

                objcvm.price = item.price;

                rents.Add(objcvm);

            }


            return View(rents);
        }

        // GET: /Rents/Details/5


        public ActionResult Details(int? id)
        {


            Session["rent"] = id;
            var uid = (int?)Session["user"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rentealseat rentdb = db.Rentealseats.Find(id);
            if (rentdb == null)
            {
                return HttpNotFound();//TO BE DONE: will show a error view
            }

            // for visiting user checking requests for this rent 
            var chek = (from i in db.Requests
                        join j in db.Rentealseats on i.rentid equals j.id
                        
                        where j.id == id && i.senderid == uid
                        select i).ToList();

            //if visiting userhas request of this rent
            if(chek.Count() == 1){
                ViewBag.hasReq = "has";
                ViewBag.dateReq = chek[0].date;
            }

            // picture of rent
            var coverpic = (from d in db.Rentealseats
                              where d.id == id
                            select  d.pic).FirstOrDefault();


            //every details available for this rent
            var roomDetail = (from d in db.Rentealseats
                              join c in db.Rooms on d.RoomId equals c.id
                              where d.id == id
                              select new
                              {
                                  tenantid = d.TenantId,
                                  rentid = d.id,
                                  start = d.startdate,
                                  roomno = c.id,
                                  noOfrooms = c.noofrooms,
                                  maxmember = c.maxmembers,
                                  price = d.price,
                                  size = c.sqft,
                                  at = c.address,
                                  oner = (from s in db.Users
                                         where s.OwnerId == c.ownerid
                                         select s).FirstOrDefault(),
                                  tenant = (from s in db.Users
                                            where s.Tenantid == d.TenantId
                                            select s).FirstOrDefault()
                                   

                              }).FirstOrDefault();


            // room no of rent
            int? roomno = roomDetail.roomno;



            var result = (from item in db.Rentealseats
                          where item.id == id
                          where item.TenantId != null
                          select item).Count();


            var fac = (from d in db.Rooms
                       join c in db.FacilitiesForRooms on d.id equals c.RoomId
                       join f in db.Facilities on c.id equals f.id
                       where d.id == roomno
                       select f
                      ).ToList();
           // return Content(id.ToString());
            var reqs = (from i in db.Requests
                        where i.rentid == id
                        where i.stat == "pending"
                        select i).ToList();

        // check if total requests == 0
            if(reqs.Count() <= 0){
                ViewBag.rentReqCnt = 0;
            }
            



         //   return Content(reqs.Count().ToString());
            //hierarchy

            if (roomDetail == null)
            {
                return Content("No data avaliable");
            }
            else
            {

                room.ID = roomDetail.rentid;
                room.startDate = roomDetail.start;
                room.maxmembers = roomDetail.maxmember;
                room.noofrooms = (int?)roomDetail.noOfrooms;
                room.needmembers = result;
                room.Location = roomDetail.at;
                room.size = roomDetail.size;
                room.pic = coverpic;
                room.price = roomDetail.price;
                if(roomDetail.tenantid != null ){room.isAvaliable = false;}
                
                Models.View.User tn = new Models.View.User();

                    tn.id = roomDetail.oner.id;

                    tn.name = roomDetail.oner.Name;

                    tn.pic = roomDetail.oner.pic;
                    tn.rating = roomDetail.oner.Rating;


                    Models.View.User tnt = new Models.View.User();

                    tnt.id = roomDetail.tenant.id;
                    tnt.tenantId = roomDetail.tenant.Tenantid;

                    tnt.name = roomDetail.tenant.Name;

                    tnt.pic = roomDetail.tenant.pic;
                    tnt.rating = roomDetail.tenant.Rating;


                    room.tenant = tnt;
                    room.owner = tn;

                    foreach (var item in reqs)
                    {
                        Models.View.Requests rq = new Models.View.Requests();

                        var sender  = (from i in db.Users
                                         where i.id == item.senderid
                                         select i).FirstOrDefault();



                        rq.reqid = item.id;
                        rq.sender = new Models.View.User();

                        rq.sender.id = sender.id;
                        rq.sender.pic = sender.pic;
                        rq.sender.name = sender.Name;
                        rq.date = item.date;

                        room.reqs.Add(rq);


                    }


                    //find reviews
                    var revs = (from i in db.rentrevs
                                where i.rentid == id
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

                        room.revs.Add(v);

                    }
                
                if ((int?)Session["user"] == roomDetail.oner.id)
                {
                   

                    ViewBag.userType = "owner";
                }
            }

            foreach (var items in fac)
            {
                Models.View.Facilities x = new Models.View.Facilities();
                x.icon = items.icon;
                x.type = items.type;
                room.facilities.Add(x);
            }



            return View(room);
        }


        [HttpPost]
        public ActionResult Index(int? minp,int? maxp,DateTime? start,int? minroom,int? maxroom,
                                  int? minmem, int? maxmem, int? minsize, int? maxsize,string district)
        {
            //intialize with zero
            ViewBag.Search = 0;

            //geting dataset from Facility table in database 
            var fac = (from x in db.Facilities
                       select x).ToList();

            // facilities data set to viewmodel insertion
            List<Models.View.Facilities> facobjl = new List<Models.View.Facilities>();

            foreach (var item in fac)
            {
                Models.View.Facilities facobj = new Models.View.Facilities();
                facobj.id = item.id;
                facobj.icon = item.icon;
                facobj.type = item.type;
                facobjl.Add(facobj);
            }

            //facilities list to viewbag
            ViewBag.fac = facobjl;


            // getting datasets of districts from database
            var loc = (from c in db.Districts

                       select c.DistrictName

                      ).ToList();

            //district list to viewbag
            ViewBag.uiDist = loc;

            //storing current user for future use
            ViewBag.userid = Session["user"];
            
            //checking fo NULL value
            if(minp != null && maxp != null && minmem != null && maxmem != null && maxroom != null && minroom != null && minsize != null && maxroom != null ){
               //getting filtered data sets
                var lrents = (from p in db.Rentealseats
                          join x in db.Rooms on p.RoomId equals x.id
                          where (p.price >= minp && p.price <= maxp) && p.startdate >= start && (x.sqft <= maxsize && x.sqft >= minsize) && (x.maxmembers <= maxmem && x.maxmembers >= minmem) && (x.noofrooms >= minroom && x.noofrooms <= maxroom) && x.upname == district
                          where p.TenantId == null
                          where p.price != 0
                          select new
                          {
                              cover = p.pic,
                              ID = p.id,
                              roomno = p.RoomId,
                              price = p.price,
                              location = x.address,
                              start = p.startdate
                          }
                          ).ToList();
                //total search results
                ViewBag.Search = lrents.Count();

                //assigning list values to viewmodel
               foreach (var item in lrents)
               {

                   Models.View.Rent objcvm = new Models.View.Rent(); // ViewModel

                   objcvm.ID = item.ID;
                   objcvm.pic = item.cover;

                   objcvm.startDate = item.start;

                   objcvm.Location = item.location;

                   objcvm.price = item.price;

                   rents.Add(objcvm);

               }

               return View(rents);
               

            }else{
                //setting conditional variable for search error in filter
               ViewBag.searchError = "yes";
                
                //getting datasets from database
                var lrents = (from p in db.Rentealseats
                          join x in db.Rooms on p.RoomId equals x.id
                          where p.price !=0
                          select new
                          {
                              cover = p.pic,
                              ID = p.id,
                              roomno = p.RoomId,
                              price = p.price,
                              location = x.address,
                              start = p.startdate
                          }
                          ).ToList();
                
                //assigning list values to viewmodel
                foreach (var item in lrents)
                {

                    Models.View.Rent objcvm = new Models.View.Rent(); // ViewModel

                    objcvm.ID = item.ID;
                    objcvm.pic = item.cover;

                    objcvm.startDate = item.start;

                    objcvm.Location = item.location;

                    objcvm.price = item.price;

                    rents.Add(objcvm);

                }


                return View(rents);


            }

            
            
        }



        // GET: /Rents/Create
        public ActionResult Create()
        {
            return View();
        }



        
        public ActionResult Requests(int? rentid){

            //storing rent id in session for future use
            Session["rentid"] = rentid;

            //retriving looged user id
            int? user = (int?)Session["user"];

            //if user not logged in then please log in
            if ((int?)Session["user"] == null)
            {
                //this are for error message showing
                ViewBag.error = "no";
                Session["need"] = "1";
               
                return RedirectToAction("Login", "User");

            }

            //check if user has tenant id
            var checkTenant = (from i in db.Users
                                   where i.id == user
                                   select i).First();




            // create a tenant data for current user because he has requested for a rent
            // this portion will add data to tenant table 
            if(checkTenant.Tenantid == null){
                var t = new Tenant
                {

                };
                db.Tenants.Add(t);

                db.SaveChanges();

                checkTenant.Tenantid = t.id;
            

            }
            //inserting request data in the database
            var req = new Request
            {
                rentid = rentid,
                senderid = user,
                stat = "pending",
                date = DateTime.Now

            };
           
                
            db.Requests.Add(req);
            db.SaveChanges();

            
            
           
            

            // redirect without any object[direct to method]
            return RedirectToAction("Details","Rents", new { id = rentid});

        }

        [HttpPost]
        public ActionResult Details(string rev)
        {

            if(Session["log"]!= "in"){
                return RedirectToAction("Login", "User");
        
            }

            var newRev = new rentrev
            {
                reveiewerid = (int?)Session["user"],
                review = rev,
                rentid = (int?)Session["rent"]
            };

            db.rentrevs.Add(newRev);
            db.SaveChanges();


            return RedirectToAction("Details", "Rents", new { id = (int?)Session["rent"] });
        }


    }

    

}
