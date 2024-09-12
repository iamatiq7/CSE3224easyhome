using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectsd.Models.View
{
    public class Rooms
    {
        public List<Models.View.Facilities> facilities = new List<Models.View.Facilities>();
        public List<int> loc = new List<int>();

        public int? id { get; set; }
        public int? noofrooms { get; set; }
        public int? maxmembers { get; set; }
        public int? needmembers { get; set; }
        public int? size { get; set; }
        public string Location { get; set; }
        public Models.View.User owner { get; set; }// object here
        public int totalrents { get; set; }
        
    }
}