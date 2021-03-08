using MiniSocialNetwork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MiniSocialNetwork.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include("Post");
            ViewBag.Comments = comments;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            return View();
        }

        public ActionResult Show(int id)
        {
            Comment comment = db.Comments.Find(id);
            
            return View(comment);

        }

        public ActionResult New()
        {
            Comment comment = new Comment();
            comment.Posts = GetAllPosts();
            
            return View(comment);
        }
        [NonAction]
        private IEnumerable<SelectListItem> GetAllPosts()
        {
            var selectList = new List<SelectListItem>();
            
            var posts = from pst in db.Posts select pst;
            
            foreach (var post in posts)
            {
                
                selectList.Add(new SelectListItem
                {
                    Value = post.PostId.ToString(),
                    Text = post.Content.ToString()
                });
            }
            
            return selectList;
        }

        [HttpPost]
        public ActionResult New(Comment comment)
        {
            try
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                TempData["message"] = "Comment added!";
                return Redirect("/Post/Show/" + comment.PostId);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View(comment);
            }
        }

        public ActionResult Edit(int id)
        {

            Comment comment= db.Comments.Find(id);
            comment.Posts = GetAllPosts();
            return View(comment);
        }

        

        [HttpPut]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comment = db.Comments.Find(id);
                if (TryUpdateModel(comment))
                {
                    comment.Content = requestComment.Content;
                    comment.CreatedAt = requestComment.CreatedAt;
                    db.SaveChanges();
                    TempData["message"] = "Comment edited!";
                    return Redirect("/Post/Show/" + comment.PostId);
                }
                return View(requestComment);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Source + e.Message);
                return View();
            }
        }


        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            TempData["message"] = "Comment deleted!";
            return Redirect("/Post/Show/" + comment.PostId);
        }

    }
}