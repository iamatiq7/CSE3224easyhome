using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectsd.Models.View
{
    public class User
    {
        public int? id { get; set; }
        public int? tenantId { get; set; }
        
        public List<Models.View.Review> revs = new List<Models.View.Review>();
        public List<Models.View.Rent> rents = new List<Models.View.Rent>();
        public string name { get; set; }
        public string email { get; set; }
        public string cell { get; set; }
        public string vid { get; set; }
        public string gender { get; set; }
        public int? age { get; set; }
        public int? rating { get; set; }
        public string pass { get; set; }
        public string address { get; set; }
        public string pic { get; set; }
        public bool isOwner = false;
        public bool isTenant = false;








    }
}