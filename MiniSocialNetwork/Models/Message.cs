using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MiniSocialNetwork.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }
        public int GroupId { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Nickname is required!")]
        public string Nickname { get; set; }
        [DefaultValue("https://icon-library.com/images/default-user-icon/default-user-icon-13.jpg")]
        [RegularExpression(@"(http(s)?:\/\/.)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#?&//=]*)")]
        public string PictureUrl { get; set; }
        [Required(ErrorMessage = "You cannot send an empty message")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Group Group { get; set; }
    }
}