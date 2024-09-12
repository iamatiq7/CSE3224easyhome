using projectsd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace projectsd.Controllers
{
    public class ChatController : Controller
    {
        private dbf db = new dbf();
       
        //
        // GET: /Chat/
        public ActionResult Index()
        {
            //decleraing a view model list of chatroom
            List<Models.View.ChatRooms> cl = new List<Models.View.ChatRooms>();

            //retriving current user
            int? user = (int?)Session["user"];

            //get all distintct chats


            var chatList1 = (from i in db.chrooms
                            
                            where i.p1 == user || i.p2 == user
                            orderby i.time descending
                            select new
                            {
                                chatno = i.id
                            }
                                ).ToList();
            
            
            
            
            
            
            
            
            
            
            
            
            //storing distinct chat count
            ViewBag.msg = chatList1.Count();
            //getting last message
            
            //storing distinct chats
            foreach (var item in chatList1)
            {
             var lastmsg = (from  j in db.Messages
                            where j.chroom == item.chatno
             orderby j.time descending
                            select new
                            {
                                reciverid = j.recieverid,
                                senderid = j.senderid,
                                recieverpic = (from k in db.Users
                                               where k.id == j.recieverid
                                               select k.pic).FirstOrDefault(),
                                recieverName = (from l in db.Users
                                                where l.id == j.recieverid
                                                select l.Name).FirstOrDefault(),

                                senderpic = (from k in db.Users
                                             where k.id == j.senderid
                                             select k.pic).FirstOrDefault(),
                                senderName = (from l in db.Users
                                              where l.id == j.senderid
                                              select l.Name).FirstOrDefault(),
                                
                                stat = j.stat,
                                time = j.time,
                                text = j.text

                            }
                               ).FirstOrDefault();

                Models.View.ChatRooms ch = new Models.View.ChatRooms();

                if (lastmsg.senderid == user)
                {
                     ch.reciverid = lastmsg.reciverid;
                     ch.recieverpic = lastmsg.recieverpic;
                     ch.senderid = lastmsg.senderid;
                     ch.senderName = "You";
                }
                else
                {
                    ch.reciverid = lastmsg.senderid;
                    ch.recieverpic = lastmsg.senderpic;
                    ch.senderid = lastmsg.senderid;
                    ch.senderName = lastmsg.senderName;


                }

                ch.chatNo = (int?)item.chatno;
                ch.stat = lastmsg.stat;
                ch.text = lastmsg.text;
                ch.time = lastmsg.time;
                cl.Add(ch);
            }

            return View(cl);
        }








        public ActionResult Inbox(int? chid)
        {
          //  return Content(chid.ToString());
            //check null
            if (chid == null)
            {
                return Content("Invalid request");
            }

            Session["chatroom"] = chid;

            //storing looged user as sender
            int? sender = (int?)Session["user"];


           
                //changing last message status to "seen"
                var last = (from i in db.Messages
                            where i.chroom == chid
                            orderby i.time descending
                            select i).First();

                if (last != null)
                {
                    last.stat = "seen";

                    db.SaveChanges();
                }

            

            
            
            //getting messages
            var msgs = (from i in db.Messages
                        where i.chroom == chid
                        orderby i.time descending
                        select i).ToList();

            int? reciever;

            List<Models.View.Messages> msl = new List<Models.View.Messages>();

            int? user = (int?)Session["user"];

            if(msgs[0].senderid != user){
                reciever = msgs[0].senderid;

            }
            else
            {
                reciever = msgs[0].recieverid;

            }
           // return Content(reciever.ToString());

            Session["rec"] = reciever;
            foreach(var items in msgs){
                //sender information
                var to = (from j in db.Users
                          where j.id == reciever
                          select j).FirstOrDefault();

                Models.View.User x = new Models.View.User();

                x.id = reciever;
                x.name = to.Name;
                x.pic = to.pic;

                //msg information
                Models.View.Messages m = new Models.View.Messages();

                if (items.senderid == to.id)
                {
                    m.sender = x;
                }
                else
                {
                    x.name = "me";
                    m.sender = x;
                }

                Session["chatName"] = to.Name;

                m.status = items.stat;
                m.text = items.text;
                m.time = items.time;

                msl.Add(m);
            }
            //return Content(msl[0].);

            return View(msl);
        }



        public ActionResult GetPartialViewData()
        {



            //getting messages
            var msgs = (from i in db.Messages
                        where i.chroom == 3
                        orderby i.time descending
                        select i).ToList();

            int? reciever;

            List<Models.View.Messages> msl = new List<Models.View.Messages>();

            int? user = (int?)Session["user"];

            if (msgs[0].senderid != user)
            {
                reciever = msgs[0].senderid;

            }
            else
            {
                reciever = msgs[0].recieverid;

            }
            // return Content(reciever.ToString());

            Session["rec"] = reciever;
            foreach (var items in msgs)
            {
                //sender information
                var to = (from j in db.Users
                          where j.id == reciever
                          select j).FirstOrDefault();

                Models.View.User x = new Models.View.User();

                x.id = reciever;
                x.name = to.Name;
                x.pic = to.pic;

                //msg information
                Models.View.Messages m = new Models.View.Messages();

                if (items.senderid == to.id)
                {
                    m.sender = x;
                }
                else
                {
                    x.name = "me";
                    m.sender = x;
                }

                Session["chatName"] = to.Name;

                m.status = items.stat;
                m.text = items.text;
                m.time = items.time;

                msl.Add(m);
            }
            


            return Json(new { html = PartialView("_partial", msl).ToString() });
        }





        [HttpPost]
        public ActionResult Inbox(string msg)
        {

            //check null
            if (msg == "")
            {
                return RedirectToAction("Inbox", "Chat", new { chid = (int?)Session["chatroom"] });
            }

            //insert new data set of message in database
            var newMsg = new Message
            {
                time = DateTime.Now,
                senderid = (int?)Session["user"],
                recieverid = (int?)Session["rec"],
                text = msg,
                stat = "unseen",
                chroom = (int?)Session["chatroom"]

            };

            //insert dataset to database
            db.Messages.Add(newMsg);
            db.SaveChanges();


            //return to inbox by redirectAction()
            return RedirectToAction("Inbox", "Chat", new { chid = (int?)Session["chatroom"] });
          
        }



	}
}