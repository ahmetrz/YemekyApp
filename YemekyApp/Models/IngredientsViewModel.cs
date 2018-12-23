using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YemekyApp.Models
{
    public class IngredientsViewModel
    {
        public int? RecipeId { get; set; }
        public string Piece { get; set; }
        public string Description { get; set; }
    }
}