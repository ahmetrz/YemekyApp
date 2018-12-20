using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YemekyApp.Models;

namespace YemekyApp.Controllers
{
    public class HomeController : Controller
    {

        YemekyAppEntities _db = new YemekyAppEntities();
        public ActionResult Index()
        {
            return View();
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
                    CategoryName = _db.Category.First(x=> x.ID == item.CategoryId)?.CategoryName,
                    Difficulty = item.Difficulty,
                    Servings = item.Servings,
                    Description = item.Descripition,
                    CategoryIcon = _db.Category.First(x=> x.ID== item.CategoryId)?.CategoryIcon,
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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}