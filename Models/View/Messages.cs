using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectsd.Models.View
{
    public class Messages
    {
        public Models.View.User sender { get; set; }
        public string text { get; set; }
        public string status { get; set; }
        public DateTime? time { get; set; }
    }
}