using MiniSocialNetwork.Models;
using System;
using System.Linq;
using System.Collections;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Collections.Generic;

namespace MiniSocialNetwork.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private int _perPage = 10;

        // GET: Profile
        public ActionResult Index()
        {
            //var profiles = (from user in db.Users
            //               join profile in db.Profiles on user.Id equals profile.UserId
            //               select new {
            //                   profile.ProfileId,
            //                   profile.ProfilePictureUrl,
            //                   profile.Status,
            //                   profile.FirstName,
            //                   profile.LastName,
            //                   profile.Location,
            //                   profile.Biography,
            //                   profile.Private,
            //                   profile.BirthDate,
            //                   user.UserName,
            //                   user.Email,
            //                   user.PhoneNumber,
            //               }).ToList();
            var profiles = from profile in db.Profiles
                           orderby profile.FullName
                           select profile;

            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var totalItems = profiles.Count();

            var currentProfile = Convert.ToInt32(Request.Params.Get("profile"));
            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedProfiles = profiles.Skip(offset).Take(this._perPage);

            ViewBag.Total = totalItems;
            ViewBag.LastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.Profiles = paginatedProfiles;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        [ActionName("Search")]
        public ActionResult SearchProfile()
        {
            var currentPage = Convert.ToInt32(Request.Params.Get("page"));
            var searchedString = "";
            if (Request.Params.Get("search") != null)
            {
                searchedString = Request.Params.Get("search").Trim();
                //System.Diagnostics.Debug.WriteLine(searchedString);
            }
            var query = (from profile in db.Profiles
                         join user in db.Users on profile.UserId equals user.Id
                         where (profile.FullName.ToLower().Contains(searchedString)
                                 || user.Email.ToLower().Contains(searchedString))
                             && profile.Private == false
                         orderby profile.FullName
                         select profile);

            var totalItems = query.Count();

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * this._perPage;
            }

            var paginatedProfiles = query.Skip(offset).Take(this._perPage);

            ViewBag.Total = totalItems;
            ViewBag.LastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
            ViewBag.SearchedString = searchedString;
            ViewBag.SearchedProfiles = paginatedProfiles;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [ActionName("View")]
        public ActionResult ViewProfile(int id)
        {
            Profile profileUser = (from profile in db.Profiles
                                   where profile.ProfileId == id
                                   select profile).SingleOrDefault();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            if (profileUser == null)
            {
                TempData["message"] = "Profile doesn't exist";
                return RedirectToAction("Index");
            }
            
            return View(profileUser);
        }

        [ActionName("New")]
        public ActionResult CreateProfile()
        {
            var loggedUser = User.Identity.GetUserId();
            Profile profileUser = (from profile in db.Profiles
                                   where profile.UserId == loggedUser
                                   select profile).SingleOrDefault();
            if (profileUser != null)
            {
                TempData["message"] = "You already have a profile!";
                return RedirectToAction("Edit");
            }
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }

        [ActionName("New")]
        [HttpPost]
        public ActionResult CreateProfile(Profile profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loggedUser = User.Identity.GetUserId();
                    profile.UserId = loggedUser;
                    profile.FullName = profile.FirstName + ' ' + profile.LastName;
                    if (profile.ProfilePictureUrl == null)
                    {
                        profile.ProfilePictureUrl = "https://icon-library.com/images/default-user-icon/default-user-icon-13.jpg";
                    }
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                    TempData["message"] = "You successfully created the profile!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(profile);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(profile);
            }
        }

        [ActionName("Edit")]
        public ActionResult EditProfile()
        {
            var loggedUser = User.Identity.GetUserId();
            Profile profileUser = (from profile in db.Profiles
                                   where profile.UserId == loggedUser
                                   select profile).SingleOrDefault();
            if (profileUser == null)
            {
                TempData["message"] = "You need to first create a profile!";
                return RedirectToAction("New");
            }
            ViewBag.Profile = profileUser;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View(profileUser);
        }

        [ActionName("Edit")]
        [HttpPut]
        public ActionResult EditProfile(Profile profile)
        {
            try
            {
                var loggedUser = User.Identity.GetUserId();
                Profile profileUser = (from profilex in db.Profiles
                                       where profilex.UserId == loggedUser
                                       select profilex).SingleOrDefault();
                if (profileUser == null)
                {
                    TempData["message"] = "You need to first create a profile!";
                    return RedirectToAction("New");
                }
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(profileUser))
                    {
                        profileUser.FirstName = profile.FirstName;
                        profileUser.LastName = profile.LastName;
                        profileUser.FullName = profile.FirstName + ' ' + profile.LastName;
                        profileUser.ProfilePictureUrl = profile.ProfilePictureUrl;
                        profileUser.Status = profile.Status;
                        profileUser.Location = profile.Location;
                        profileUser.Biography = profile.Biography;
                        profileUser.Private = profile.Private;
                        profileUser.BirthDate = profile.BirthDate;
                        if (profile.ProfilePictureUrl == null)
                        {
                            profileUser.ProfilePictureUrl = "https://icon-library.com/images/default-user-icon/default-user-icon-13.jpg";
                        }
                        db.SaveChanges();
                        TempData["message"] = "You successfully updated your profile!";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Cannot update your profile";
                    return View(profile);
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(profile);
            }
        }

        [ActionName("Friend")]
        public ActionResult ViewFriends()
        {
            string loggedUser = User.Identity.GetUserId();
            var friends = from friend in db.Friends
                          where friend.Accepted == true && (friend.Receiver == loggedUser || friend.Sender == loggedUser)
                          orderby friend.CreatedAt
                          select friend;

            IEnumerable<Profile> profiles = null;
            foreach (Friend frnd in friends)
            {
                var profil = (from profile in db.Profiles
                              where profile.UserId != loggedUser && (profile.UserId == frnd.Sender || profile.UserId == frnd.Receiver)
                              select profile);
                if (profil != null)
                {
                    if (profiles == null)
                    {
                        profiles = profil.AsEnumerable<Profile>();
                    } else
                    {
                        profiles = profiles.Concat(profil.AsEnumerable<Profile>());
                    }
                }
            }

            ViewBag.Friends = profiles;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            return View();
        }


        [ActionName("IncomingFriends")]
        public ActionResult IncomingFriendRequests()
        {
            string loggedUser = User.Identity.GetUserId();
            var friendsIds = from friendsx in db.Friends
                             join profile in db.Profiles on friendsx.Receiver equals loggedUser
                             select friendsx.Sender;

            var friends = from profile in db.Profiles
                          where friendsIds.Contains(profile.UserId)
                          select profile;

            ViewBag.Friends = friends;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        [ActionName("AcceptFriend")]
        [HttpPut]
        public ActionResult AcceptFriendRequest(int id)
        {
            try
            {
                string loggedUser = User.Identity.GetUserId();

                string senderId = (from profile in db.Profiles
                                   where profile.ProfileId == id
                                   select profile.UserId).FirstOrDefault();

                var updatedFriend = (from friend in db.Friends
                                    where friend.Sender == senderId && friend.Receiver == loggedUser
                                    select friend).FirstOrDefault();

                

                if (updatedFriend == null)
                {
                    TempData["message"] = "You didn't receive this friend request";
                    return RedirectToAction("Index");
                }
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(updatedFriend))
                    {
                        updatedFriend.Sender = updatedFriend.Sender;
                        updatedFriend.Receiver = updatedFriend.Receiver;
                        updatedFriend.CreatedAt = updatedFriend.CreatedAt;
                        updatedFriend.Accepted = true;
                        db.SaveChanges();
                        TempData["message"] = "You successfully accepted the friend request!";
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Cannot accept the friend request";
                    return RedirectToAction("IncomingFriends");
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return RedirectToAction("IncomingFriends");
            }
        }

        [ActionName("DeleteFriend")]
        [HttpDelete]
        public ActionResult DeleteFriendRequest(int id)
        {
            string loggedUser = User.Identity.GetUserId();

            string senderId = (from profile in db.Profiles
                               where profile.ProfileId == id
                               select profile.UserId).FirstOrDefault();

            var updatedFriend = (from friend in db.Friends
                                where friend.Sender == senderId && friend.Receiver == loggedUser
                                select friend).FirstOrDefault();
            db.Friends.Remove(updatedFriend);
            TempData["message"] = "Friend request deleted!";
            db.SaveChanges();
            return RedirectToAction("IncomingFriends");
        }

        [ActionName("AddFriend")]
        [HttpPost]
        public ActionResult AddFriend(FormCollection formData)
        {
            string currentUser = User.Identity.GetUserId();
            string friendToAdd = formData.Get("UserId");

            var alreadyAdded = (from friend in db.Friends
                               where friend.Sender == currentUser && friend.Receiver == friendToAdd 
                               select friend).FirstOrDefault();
            var receivedFriend = (from friend in db.Friends
                               where friend.Sender == friendToAdd && friend.Receiver == currentUser
                               select friend).FirstOrDefault();
            var AcceptedFriend = (from friend in db.Friends
                                  where friend.Accepted == true && ((friend.Sender == currentUser && friend.Receiver == friendToAdd) || (friend.Sender == friendToAdd && friend.Receiver == currentUser))
                                  select friend).FirstOrDefault();
      
            if(AcceptedFriend != null)
            {
                TempData["message"] = "Already friends";
                return RedirectToAction("Index");
            }
            if (currentUser == friendToAdd)
            {
                TempData["message"] = "You cannot add yourself as friend!";
                return RedirectToAction("Index");
            }

            if (alreadyAdded != null)
            {
                TempData["message"] = "You already send a friend request";
                return RedirectToAction("Index");
            }

            if (receivedFriend != null)
            {
                var profileId = (from profile in db.Profiles
                                 where profile.UserId == friendToAdd
                                 select profile).FirstOrDefault();
                return RedirectToAction("AcceptFriend", new { id = profileId});
            }


            try
            {
                Friend friendship = new Friend();
                friendship.Sender = currentUser;
                friendship.Receiver = friendToAdd;
                friendship.Accepted = false;
                friendship.CreatedAt = DateTime.Now;

                db.Friends.Add(friendship);
                db.SaveChanges();
                TempData["message"] = "You successfully sent your friend request!";

                return RedirectToAction("Index");

            }
            catch (Exception e)
            {
                TempData["message"] = "Cannot add your friend!";
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return RedirectToAction("Index");
            }

        }

    }
}