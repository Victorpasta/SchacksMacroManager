using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchacksMacroManager.Models
{
    [Serializable]
    public class IngredientInstance
    {
        public int Grams { get; set; }
        public Ingredient Ingredient { get; set; }
        public IngredientInstance(Ingredient ingredient, int grams) 
        {
            Grams = grams;
            Ingredient = ingredient;
        }
        public IngredientInstance() { }


        
    }
}
