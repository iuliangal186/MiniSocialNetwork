using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MiniSocialNetwork.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        public string CreatorId { get; set; }
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Details is required!")]
        public string Details { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }

        
        public virtual ICollection<GroupUsers> GroupUsers { get; set; }
        
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ApplicationUser Creator { get; set; }
    }
}