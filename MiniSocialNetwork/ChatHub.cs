using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using MiniSocialNetwork.Models;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace MiniSocialNetwork
{
    [Authorize]
    public class ChatHub : Hub
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Send(string room, string profilepic, string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            var loggedUser = Context.User.Identity.GetUserId();
            Message msg = new Message
            {
                CreatedAt = DateTime.Now,
                GroupId = Int32.Parse(room),
                UserId = loggedUser,
                PictureUrl = profilepic,
                Nickname = name,
                Content = message
            };
            db.Messages.Add(msg);
            db.SaveChanges();
            //Clients.Group(room).addMessageToGroup(loggedUser, name, message);
            Clients.Group(room).addMessageToGroup(room, loggedUser, profilepic, name, message);
        }
        public override Task OnConnected()
        {
            // Retrieve user.
            var loggedUser = Context.User.Identity.GetUserId();
            var rooms = from groups in db.GroupUsers
                        where groups.UserId == loggedUser
                        select groups.GroupId;

            // If user does not exist in database, must add.
            if (rooms != null)
            {
                // Add to each assigned group.
                foreach (int room in rooms)
                {
                    Groups.Add(Context.ConnectionId, room.ToString());
                }
            }
            return base.OnConnected();
        }

        public void AddToRoom(string roomparam)
        {
            // Retrieve room.
            var loggedUser = Context.User.Identity.GetUserId();
            var room = db.Groups.Find(roomparam);
            var connection = (from users in db.GroupUsers
                              where room.GroupId == users.GroupId && users.UserId == loggedUser
                              select users).SingleOrDefault();

            if (connection != null)
            {
                Groups.Add(Context.ConnectionId, room.GroupId.ToString());
            }
        }

        public void RemoveFromRoom(string roomparam)
        {
            {
                // Retrieve room.
                var room = db.Groups.Find(roomparam);
                if (room != null)
                {
                    Groups.Remove(Context.ConnectionId, room.GroupId.ToString());
                }
            }
        }
    }
}
