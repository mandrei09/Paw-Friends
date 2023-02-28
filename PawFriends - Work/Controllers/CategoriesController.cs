using Microsoft.AspNetCore.Mvc;
using PawFriends___Work.Models;
using Microsoft.EntityFrameworkCore;
using PawFriends___Work.Data;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;

namespace PawFriends___Work.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CategoriesController(ApplicationDbContext context)
        {
            db = context;
        }


        // VIEW -----------------------------------------------------------------
        public ActionResult Index()
        {

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var categories = from category in db.Categories.Include("Posts")
                             select new Category()
                             {
                                 CategoryName = category.CategoryName,
                                 Id = category.Id,
                                 Descriere = category.Descriere,
                                 Icon = category.Icon,
                                 Posts = (from p in category.Posts
                                          select new Post()
                                          {
                                              Title = p.Title,
                                              Content = p.Content,
                                              Date = p.Date
                                          }).ToList()

                             };
            ViewBag.Categories = categories;
            return View();
        }

        // SHOW ------------------------------------------------------------------
        public ActionResult Show(int id)
        {
            
			Category category = db.Categories.Include("Posts")
								.Include("Posts.User")
								.Where(p => p.Id == id)
								.First();

			ViewBag.Category = category;
            SetAccessRights();

			return View();
		}

        // NEW -------------------------------------------------------------------
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult New(Category cat)
        {
            try
            {
                db.Categories.Add(cat);
                db.SaveChanges();
                TempData["message"] = "Category has been added!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(cat);
            }
        }

        // EDIT -----------------------------------------------------
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            SetAccessRights();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult Edit(int id, Category requestCategory)
        {
            SetAccessRights();
            Category category = db.Categories.Find(id);
            if (ModelState.IsValid)
            {

                category.CategoryName = requestCategory.CategoryName;
                category.Descriere = requestCategory.Descriere;
                category.Icon = requestCategory.Icon;
                db.SaveChanges();
                TempData["message"] = "Categoria a fost modificata!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestCategory);
            }
        }

        [Authorize(Roles = "Admin")]
        // DELETE ---------------------------------------------------
        [HttpPost]
        public ActionResult Delete(int id)
        {
            SetAccessRights();
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            TempData["message"] = "Categoria a fost stearsa";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Admin"))
            {
                ViewBag.AfisareButoane = true;
            }

        }

    }
}
