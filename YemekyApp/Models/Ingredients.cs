//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YemekyApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Ingredients
    {
        public int ID { get; set; }
        public Nullable<int> RecipeId { get; set; }
        public Nullable<int> Piece { get; set; }
        public string Description { get; set; }
    
        public virtual Recipe Recipe { get; set; }
    }
}
