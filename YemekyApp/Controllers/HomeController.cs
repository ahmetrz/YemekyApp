using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using YemekyApp.Models;
using YemekyApp.Filters;

namespace YemekyApp.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        

        YemekyAppEntities _db = new YemekyAppEntities();
        public ActionResult Index()
        {
            var model = new RecipeModel
            {
                Directions = _db.Directions.ToList(),
                Ingredients = _db.Ingredients.ToList(),
                Recipes = _db.Recipe.ToList(),
                Categories = _db.Category.ToList()
            };
            return View(model);
        }


        public ActionResult Recipe(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = _db.Recipe.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }

            return View(recipe);
        }

      
        [Route("recipeList/category/{id?}")]
        public JsonResult RecipeCategoryDetail(int? id)
        {

            List<RecipeCategoryViewModel> recipeModel;
            recipeModel = new List<RecipeCategoryViewModel>();

            var recipeList = _db.Recipe.Where(x => x.CategoryId == id).ToList();
            foreach (var item in recipeList.OrderByDescending(x=> x.ID))
            {
                
                var recipe = new RecipeCategoryViewModel
                {
                    RecipeId = item.ID,
                    RecipeName = item.RecipeName,
                    Image = item.Image
                };

         
                recipeModel.Add(recipe);
            }
            return Json(new { data = recipeModel }, JsonRequestBehavior.AllowGet);

        }


        [Route("recipeList/{id?}")]
        public JsonResult RecipeDetail(int? id)
        {

            List<RecipeViewModel> recipeModel;
            recipeModel = new List<RecipeViewModel>();

            var recipeList = _db.Recipe.Where(x => x.ID == id).ToList();
            foreach (var item in recipeList)
            {
                var directionsList = _db.Directions.Where(x => x.RecipeId == item.ID).ToList();
                var ingredientsList = _db.Ingredients.Where(x => x.RecipeId == item.ID).ToList();

                var recipe = new RecipeViewModel
                {
                    RecipeId = item.ID,
                    RecipeName = item.RecipeName,
                    RecipeTime = item.Time,
                    CategoryId = item.CategoryId,
                    CategoryName = _db.Category.FirstOrDefault(x => x.ID == item.CategoryId)?.CategoryName,
                    Difficulty = item.Difficulty,
                    Servings = item.Servings,
                    Description = item.Descripition,
                    CategoryIcon = _db.Category.FirstOrDefault(x => x.ID == item.CategoryId)?.CategoryIcon,
                    Image = item.Image
                };

                var directions = new List<DirectionViewModel>();
                foreach (var i in directionsList.OrderBy(x => x.ID))
                {
                    directions.Add(new DirectionViewModel
                    {
                        RecipeId = item.ID,
                        Description = i.Description
                    });
                }

                var ingredients = new List<IngredientsViewModel>();
                foreach (var j in ingredientsList.OrderBy(x => x.ID))
                {
                    ingredients.Add(new IngredientsViewModel
                    {
                        RecipeId = item.ID,
                        Description = j.Description,
                        Piece = j.Piece
                    });
                }

                recipe.Directions = directions;
                recipe.Ingredients = ingredients;
                recipeModel.Add(recipe);
            }
            return Json(new { data = recipeModel }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RecipeAdd(RecipeViewModel rcp)
        {
            var recipe = new Recipe()
            {
                CategoryId =  rcp.CategoryId,
                RecipeName = rcp.RecipeName,
                Time = rcp.RecipeTime,
                Difficulty = rcp.Difficulty,
                Servings = rcp.Servings,
                Descripition = rcp.Description,
                Image = rcp.Image
            };

            _db.Recipe.Add(recipe);
            _db.SaveChanges();
            if (rcp.Ingredients.Any())
            {
                foreach (var item in rcp.Ingredients)
                {
                    var id = recipe.ID;
                    var ingredientList = new Ingredients()
                    {
                        RecipeId = id,
                        Piece = item.Piece,
                        Description = item.Description
                    };
                    _db.Ingredients.Add(ingredientList);
                }
            }

            if (rcp.Directions.Any())
            {
                foreach (var item in rcp.Directions)
                {
                    var id = recipe.ID;
                    var directionList = new Directions()
                    {
                        RecipeId = id,
                        Description = item.Description
                    };
                    _db.Directions.Add(directionList);
                }
            }

            _db.SaveChanges();
            return Json(rcp, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CategoryAdd(Category model)
        {
            var category = new Category
            {
                CategoryIcon = model.CategoryIcon,
                CategoryName = model.CategoryName
            };

            _db.Category.Add(category);
            _db.SaveChanges();

            return Json(category, JsonRequestBehavior.AllowGet);
        }

        [Route("register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UserRegister(User userModel)
        {
            if (ModelState.IsValid)
            {
                var userFind = _db.User.FirstOrDefault(x => x.Email == userModel.Email || x.Username == userModel.Username);

                if (userFind != null)
                {
                    return Json("hata");
                }

                _db.User.Add(userModel);
                _db.SaveChanges();

            }
            return Json(userModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");//Kullanıcı girişi  başarılı ise Ana Sayfaya Yönelnediriyoruz
            }
            return View();
        }

        
        [HttpPost]
        public JsonResult Login(User us)
        {
            if (ModelState.IsValid)
            {
                var isValidate = _db.User.FirstOrDefault(x => x.Username == us.Username);
                if (isValidate != null)
                {
                    if (isValidate.Password != us.Password) return Json("Şifreniz yanlıştır.");
                    TempData["grs"] = "<script>iziToast.success({title: 'Başarılı', message: 'Yönlendiriliyorsunuz...'});</script>";
                    FormsAuthentication.SetAuthCookie(us.Username, false);

                    return Json("yes"); //Kullanıcı girişi  başarılı ise Ana Sayfaya Yönelnediriyoruz

                }
                else
                {
                    return Json("Kullanıcı adınız yanlıştır.");
                }
            }
            return Json("yes", JsonRequestBehavior.AllowGet);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Content/images/upload"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }


        #region Recipe

        // GET: Recipes
        public ActionResult Recipes()
        {
            var recipe = _db.Recipe.Include(r => r.Category);
            return View(recipe.ToList());
        }

        // GET: Recipes/Details/5
        public ActionResult RecipeDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = _db.Recipe.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // GET: Recipes/Create
        public ActionResult RecipeCreate()
        {
            ViewBag.CategoryId = new SelectList(_db.Category, "ID", "CategoryName");
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecipeCreate([Bind(Include = "ID,CategoryId,RecipeName,Time,Difficulty,Servings,Descripition,Image")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                _db.Recipe.Add(recipe);
                _db.SaveChanges();
                return RedirectToAction("Recipes");
            }

            ViewBag.CategoryId = new SelectList(_db.Category, "ID", "CategoryName", recipe.CategoryId);
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public ActionResult RecipeEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = _db.Recipe.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(_db.Category, "ID", "CategoryName", recipe.CategoryId);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecipeEdit([Bind(Include = "ID,CategoryId,RecipeName,Time,Difficulty,Servings,Descripition,Image")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(recipe).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Recipes");
            }
            ViewBag.CategoryId = new SelectList(_db.Category, "ID", "CategoryName", recipe.CategoryId);
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public ActionResult RecipeDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = _db.Recipe.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("RecipeDelete")]
        [ValidateAntiForgeryToken]
        public ActionResult RecipeDeleteConfirmed(int id)
        {
            Recipe recipe = _db.Recipe.Find(id);
            _db.Recipe.Remove(recipe);
            _db.SaveChanges();
            return RedirectToAction("Recipes");
        }

        #endregion

        #region    Select2 AJAX

        [HttpGet]
        [Route("categoriesList")]
        public JsonResult GetCategories(string q)
        {
            var list2 = _db.Category.Select(a => new
            {
                text = a.CategoryName,
                id = a.ID
            });

            if (!(string.IsNullOrEmpty(q) || string.IsNullOrWhiteSpace(q)))
            {
                list2 = list2.Where(x => x.text.ToLower().Contains(q.ToLower()));
            }
            return Json(new { items = list2 }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}