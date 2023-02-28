using PawFriends___Work.Data;
using PawFriends___Work.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PawFriends___Work.Controllers
{

    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;

        //PASUL 10

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;


        public PostsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //Oricine poate vedea postarile.
        //public IActionResult Index()
        //{
        //    var posts = db.Posts.Include("Category").Include("User");

        //    ViewBag.Posts = posts;

        //    if (TempData.ContainsKey("message"))
        //    {
        //        ViewBag.Message = TempData["message"];
        //    }

        //    return View();
        //}

        public IActionResult Index(string sortBy)
        {
            ViewBag.SortByTitle = "title";
            ViewBag.SortByTitleDesc = "title_desc";
            ViewBag.SortByDate = "date";
            ViewBag.SortByDateDesc = "date_desc";

            var posts = db.Posts.Include("Category").Include("User").AsQueryable();

            switch(sortBy)
            {
                case "title_desc": posts = posts.OrderByDescending(x => x.Title); break;
                //case "Date desc": posts = posts.OrderByDescending(x => x.Date); break;
                case "title": posts = posts.OrderBy(x => x.Title); break;
                case "date": posts = posts.OrderBy(x => x.Date); break;

                default: posts = posts.OrderByDescending(x => x.Date); break;
            }

            ViewBag.Posts = posts;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            // AFISARE PAGINATA

            // Alegem sa afisam 5 articole pe pagina
            int _perPage = 5;

            // Fiind un numar variabil de articole, verificam de fiecare data utilizand 
            // metoda Count()

            int totalItems = posts.Count();


            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Articles/Index?page=valoare

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3 
            // Asadar offsetul este egal cu numarul de articole care au fost deja afisate pe paginile anterioare
            var offset = 0;

            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            // Se preiau articolele corespunzatoare pentru fiecare pagina la care ne aflam 
            // in functie de offset
            var paginatedPosts = posts.Skip(offset).Take(_perPage);


            // Preluam numarul ultimei pagini

            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);

            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Posts = paginatedPosts;

            return View();
        }

        //Oricine poate vedea o postare.
        public IActionResult Show(int id)
        {
            Post post = db.Posts.Include("Category")
                                .Include("User")
                                .Include("Comments")
                                .Include("Comments.User")
                                .Where(p => p.Id == id)
                                .First();

            SetAccessRights();

            return View(post);
        }

        //Pot lasa comentarii doar persoanele care au cont
        //Adaugarea unui comentariu asociat unui articol in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            SetAccessRights();
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comment.PostId);
            }

            else
            {
                Post p = db.Posts.Include("Category")
                                         .Include("User")
                                         .Include("Comments")
                                         .Include("Comments.User")
                                         .Where(p => p.Id == comment.Id)
                                         .First();


                return View(p);
            }
        }

        //Nu poti posta daca nu ai cont.
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult New()
        {
            Post post = new Post();

            post.Categ = GetAllCategories();

            return View(post);
        }

        [Authorize(Roles = "User,Editor,Admin")]
        [HttpPost]
        public IActionResult New(Post post)
        {
            post.Date = DateTime.Now;
            post.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Posts.Add(post);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost adaugat";
                return RedirectToAction("Index");
            }
            else
            {
                post.Categ = GetAllCategories();
                return View(post);
            }
        }

        //Orice persoana cu cont isi poate edita propriile postari (privilegii speciale in cazul adminului).
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id)
        {
            Post post = db.Posts.Include("Category")
                                         .Where(p => p.Id == id)
                                         .First();

            post.Categ = GetAllCategories();

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(post);
            }

            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public IActionResult Edit(int id, Post requestPost)
        {
            Post post = db.Posts.Find(id);

            if (ModelState.IsValid)
            {
                if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Editor"))
                {
                    post.Title = requestPost.Title;
                    post.Content = requestPost.Content;
                    //??
                    post.Category = requestPost.Category;
                    TempData["message"] = "Articolul a fost modificat";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui articol care nu va apartine";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                requestPost.Categ = GetAllCategories();
                return View(requestPost);
            }
        }

        //Orice persoana cu cont isi poate sterge propriile postari (privilegii speciale in cazul adminului).
        [HttpPost]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Post post = db.Posts.Include("Comments")
                                         .Where(p => p.Id == id)
                                         .FirstOrDefault();

            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Editor"))
            {
                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["message"] = "Postarea a fost stearsa";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti o postare care nu va apartine";
                return RedirectToAction("Index");
            }
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var categories = from cat in db.Categories
                             select cat;

            // iteram prin categorii
            foreach (var category in categories)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            return selectList;
        }

        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("Editor"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }
    }
}

