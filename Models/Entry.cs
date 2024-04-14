using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchacksMacroManager.Models
{
    [Serializable]
    public class Entry
    {
        public List<Ingredient> Ingredients { get; set; }
        public string Name { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Protein { get; set; }
        public double Kcal { get => Carbs*4 + Protein*4 + Fat*9; }
        public Entry(List<Ingredient> ingredients, string name)
        {
            Name = name;
            Ingredients = ingredients;
        }

        public Entry()
        {
            Name="";
            Ingredients=new List<Ingredient>();
        }

        public void CalculateMacros()
        {
            Carbs = 0;
            Fat = 0;
            Protein = 0;
            foreach (var ingredient in Ingredients)
            {
                var grams = ingredient.Grams;
                Carbs += ingredient.Carbs * grams * 0.01;
                Fat += ingredient.Fat * grams * 0.01;
                Protein += ingredient.Protein * grams * 0.01;
            }
        }

        public List<Ingredient> SyncIngredients(List<Ingredient> avaiableIngredients)
        {

            var ingredientsToRemove = new List<Ingredient>();
            if(avaiableIngredients == null)
                return ingredientsToRemove;
            foreach(var ingredient in Ingredients)
            {
                var matchingIngredient = avaiableIngredients.FirstOrDefault(ai => ai.Name == ingredient.Name);
                if(matchingIngredient == null)
                    ingredientsToRemove.Add(ingredient);
                else
                {
                    ingredient.Carbs = matchingIngredient.Carbs;
                    ingredient.Protein = matchingIngredient.Protein;
                    ingredient.Fat = matchingIngredient.Fat;
                }

            }
            ingredientsToRemove.ForEach(ingredient => Ingredients.Remove(ingredient));
            return ingredientsToRemove;
        }

    }
}
