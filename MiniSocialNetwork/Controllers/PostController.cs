using Microsoft.AspNet.Identity;
using MiniSocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniSocialNetwork.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Posts
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var posts = from post in db.Posts
                        select post;
            ViewBag.Posts = posts;

            return View();
        }

        public ActionResult Show(int id)
        {
            Post post = db.Posts.Find(id);
            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(Comment comm)
        {
            comm.CreatedAt = DateTime.Now;
            comm.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    db.Comments.Add(comm);
                    db.SaveChanges();
                    return Redirect("/Post/Show/" + comm.PostId);
                }

                else
                {
                    Post a = db.Posts.Find(comm.PostId);

                    SetAccessRights();

                    return View(a);
                }

            }

            catch (Exception e)
            {
                Post a = db.Posts.Find(comm.PostId);

                SetAccessRights();

                return View(a);
            }

        }
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Post pst)
        {
            var loggedUser = User.Identity.GetUserId();
            try
            {
                pst.UserId = loggedUser;
                pst.CreatedAt = DateTime.Now;
                db.Posts.Add(pst);
                db.SaveChanges();
                TempData["message"] = "Post added!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                System.Diagnostics.Debug.WriteLine(e.InnerException.ToString());
                return View(pst);
            }
        }

        public ActionResult Edit(int id)
        {
            Post post = db.Posts.Find(id);
     

            return View(post);
        }

        [HttpPut]
        public ActionResult Edit(int id, Post requestPost)
        {
            try
            {
                Post post = db.Posts.Find(id);
                if (TryUpdateModel(post))
                {
                    post.Content = requestPost.Content;
                    db.SaveChanges();
                    TempData["message"] = "Post edited!";
                    return RedirectToAction("Index");
                }

                return View(requestPost);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(requestPost);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Delete(int id)
        {
            var loggedUser = User.Identity.GetUserId();
           
            Post post = db.Posts.Find(id);

            if (loggedUser != post.UserId && !User.IsInRole("Admin"))
            {
                TempData["message"] = "You cannot delete this post";
                return RedirectToAction("Index");
            }
            db.Posts.Remove(post);
            TempData["message"] = "Post deleted!";
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        private void SetAccessRights()
        {
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }

            ViewBag.IsAdmin = User.IsInRole("Admin");
            ViewBag.loggedUser = User.Identity.GetUserId();
        }
    }
}