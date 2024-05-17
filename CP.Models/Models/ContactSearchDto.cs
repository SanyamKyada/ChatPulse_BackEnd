using CP.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CP.Models.Models
{
    public class ContactSearchDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeenTimestamp { get; set; }
        public bool IsRequestAlreadySent { get; set; }
    }
}
