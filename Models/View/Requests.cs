using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectsd.Models.View
{
    public class Requests : Models.View.Rent
    {
        public int? reqid { get; set; }
        public Models.View.User sender{get;set;}
        public string senderName { get; set; }
        public DateTime? date{get;set;}
        public string status = "pending";



    }
}