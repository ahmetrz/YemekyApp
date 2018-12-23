using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YemekyApp.Models
{
    public class RecipeViewModel
    {
        public int?  RecipeId { get; set; }
        public int? CategoryId { get; set; }
        public string RecipeName { get; set; }
        public string CategoryName { get; set; }
        public string RecipeTime { get; set; }
        public string  Difficulty { get; set; }
        public int? Servings { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string CategoryIcon { get; set; }

        public List<DirectionViewModel> Directions { get; set; }
        public List<IngredientsViewModel> Ingredients { get; set; }
    }

    public class RecipeCategoryViewModel
    {
        public int? RecipeId { get; set; }
        public string RecipeName { get; set; }
        public string Image { get; set; }
    }
}