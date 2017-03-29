using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ZenithWebSite.Models
{
    public class Event
    {

            [Key]
            public int EventId { get; set; }
            public DateTime EventFromDateAndTime { get; set; }
            public DateTime EventToDateAndTime { get; set; }
            public string EnteredByUserName { get; set; }
            public DateTime CreationDate { get; set; }
            public bool IsActive { get; set; }


            public List<Activity> Activities { get; set; }
     }
}
