using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectsd.Models.View
{
    public class Review
    {
        public int? reviewid { get; set; }
        public string reviewerpic { get; set; }
        public int? reviewerid { get; set; }
        public string reviewerName { get; set; }

        public string reviewtext { get; set; }
    }
}