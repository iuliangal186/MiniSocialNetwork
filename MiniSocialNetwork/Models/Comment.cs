using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniSocialNetwork.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "The content is required!")]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }

        public virtual Post Post { get; set; }
        public IEnumerable<SelectListItem> Posts { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}