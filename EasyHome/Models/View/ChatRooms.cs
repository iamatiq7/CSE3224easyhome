using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace projectsd.Models.View
{
    public class ChatRooms
    {
        public int? chatNo { get; set; }
        public int? reciverid {get;set;}
        public int? senderid { get; set; }
        public string recieverpic{get;set;} 
        public string recieverName{get;set;}
        public string senderName { get; set; } 
        
        public string stat {get; set;}
        public DateTime? time {get;set;}
        public string text { get; set; }
    }
}