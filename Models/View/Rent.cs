using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectsd.Models.View
{
    public class Rent: Models.View.Rooms 
    {
        public int ID { get; set; }
        public int? price { get; set; }
        public DateTime? posted { get; set; }
        public string pic { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? expiryDate { get; set; }
        public Models.View.User tenant { get; set; }// object here
        public bool isAvaliable = true;
        public int totalReq { get; set; }
        public List<Models.View.Requests> reqs = new List<Models.View.Requests>();
        public List<Models.View.Review> revs = new List<Models.View.Review>();
        
    }
}