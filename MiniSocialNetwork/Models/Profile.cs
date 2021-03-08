using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MiniSocialNetwork.Models
{
    public class Profile
    {
        [Key]
        public int ProfileId { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Prenumele este obligatoriu!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Numele este obligatoriu!")]
        public string LastName { get; set; }
        public string FullName { get; set; }
        [DefaultValue("https://icon-library.com/images/default-user-icon/default-user-icon-13.jpg")]
        [RegularExpression(@"(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)")]
        public string ProfilePictureUrl { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        [DataType(DataType.MultilineText)]
        public string Biography { get; set; }
        public bool Private { get; set; } 
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
        
        public virtual ApplicationUser User { get; set; }
    }
}