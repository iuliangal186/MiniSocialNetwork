using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniSocialNetwork.Models
{
    public class GroupUsers
    {
        [Key]
        public int GroupUsersId { get; set; }
        public string UserId { get; set; }
        public int GroupId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Group Group { get; set; }
    }
}