using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public ActionResult Recipe()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Route("recipeList")]
        public JsonResult RecipeDetail()
        {

            List<RecipeViewModel> recipeModel;
            recipeModel = new List<RecipeViewModel>();

            foreach (var item in _db.Recipe.ToList())
            {
                var recipe = new RecipeViewModel
                {
                    RecipeId = item.ID,
                    RecipeName = item.RecipeName,
                    RecipeTime = item.Time,
                    CategoryId = item.CategoryId,
                    CategoryName = _db.Category.First(x => x.ID == item.CategoryId)?.CategoryName,
                    Difficulty = item.Difficulty,
                    Servings = item.Servings,
                    Description = item.Descripition,
                    CategoryIcon = _db.Category.First(x => x.ID == item.CategoryId)?.CategoryIcon,
                    Image = item.Image
                };

                var directions = new List<DirectionViewModel>();
                var directionsList = _db.Directions.Where(x => x.RecipeId == item.ID).ToList();
                foreach (var i in directionsList)
                {
                    directions.Add(new DirectionViewModel
                    {
                        RecipeId = item.ID,
                        Description = i.Description
                    });
                }

                var ingredients = new List<IngredientsViewModel>();
                var ingredientsList = _db.Ingredients.Where(x => x.RecipeId == item.ID).ToList();
                foreach (var j in ingredientsList)
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
        public ActionResult Login(User us)
        {
            if (ModelState.IsValid)
            {
                if (_db.User.FirstOrDefault(x=> x.Username == us.Username && x.Password == us.Password) != null)
                {
                    TempData["grs"] = "<script>iziToast.success({title: 'Başarılı', message: 'Yönlendiriliyorsunuz...'});</script>";
                    FormsAuthentication.SetAuthCookie(us.Username, false);

                    return RedirectToAction("Index", "Home");//Kullanıcı girişi  başarılı ise Ana Sayfaya Yönelnediriyoruz
                }
                else
                {
                    TempData["grs"] = "<script>iziToast.warning({title: 'Başarısız', message: 'Email ya da şifrenizi kontrol ediniz.'});</script>";
                }
            }
            return View(us);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}