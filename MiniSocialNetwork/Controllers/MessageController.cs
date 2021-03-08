using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using System.Web;
using System.Web.Mvc;
using MiniSocialNetwork.Models;

namespace MiniSocialNetwork.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Message
        public ActionResult Index()
        {
            var messages = from message in db.Messages
                         select message;
            ViewBag.Messages = messages;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }
        
        public void New(int id, Message message)
        {
            var loggedUser = User.Identity.GetUserId();
            var groups = getGroups();
            Profile profile = (from profilex in db.Profiles
                              where profilex.UserId == loggedUser
                              select profilex).SingleOrDefault();
            if (!groups.Contains(id))
            {
                TempData["message"] = "You are not part of this group!";
            } else 
            {
                // Socket.io implementation
                if (ModelState.IsValid)
                {
                    message.CreatedAt = DateTime.Now;
                    message.GroupId = id;
                    message.UserId = loggedUser;
                    if (profile.ProfilePictureUrl == null) {
                        message.PictureUrl = "https://icon-library.com/images/default-user-icon/default-user-icon-13.jpg";
                    } else
                    {
                        message.PictureUrl = profile.ProfilePictureUrl;
                    }
                    message.Nickname = profile.FullName;
                    db.Messages.Add(message);
                    db.SaveChanges();
                } else
                {
                    TempData["message"] = "Couldn't send message";
                }

            }
        }

        [NonAction]
        private IEnumerable<int> getGroups()
        {
            var loggedUser = User.Identity.GetUserId();
            var joinedGroups = from groups in db.GroupUsers
                               where groups.UserId == loggedUser
                               select groups.GroupId;
            return joinedGroups.ToList();
        }
    }
}