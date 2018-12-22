using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YemekyApp.Models
{
    public class RecipeModel
    {

        public List<Recipe> Recipes { get; set; }
        public List<Category> Categories { get; set; }
        public List<Directions> Directions { get; set; }
        public List<Ingredients> Ingredients { get; set; }

    }
}