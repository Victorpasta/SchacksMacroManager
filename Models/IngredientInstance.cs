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
        public int Count { get; set; }
        public int Grams { get; set; }
        public Ingredient Ingredient { get; set; }
        public IngredientInstance(Ingredient ingredient, int grams)
        {
            Grams = grams;
            Ingredient = ingredient;
        }
        public IngredientInstance() { }


        public void UpdateCount()
        {
            if (Ingredient.GramsPerCount <= 0)
                return;
            Count = (int)Math.Round(Grams / Ingredient.GramsPerCount);
        }


    }
}
