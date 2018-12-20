using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YemekyApp.Models
{
    public class RecipeModel
    {

        public IEnumerable<Recipe> Recipes { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Directions> Directions { get; set; }
        public IEnumerable<Ingredients> Ingredients { get; set; }

    }
}