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
        public List<IngredientInstance> IngredientInstances { get; set; }
        public string Name { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Protein { get; set; }
        public double Kcal { get => Carbs*4 + Protein*4 + Fat*9; }
        public Entry(List<IngredientInstance> ingredients, string name)
        {
            Name = name;
            IngredientInstances = ingredients;
        }

        public Entry()
        {

        }

        public void CalculateMacros()
        {
            Carbs = 0;
            Fat = 0;
            Protein = 0;
            foreach (var ingredient in IngredientInstances)
            {
                var grams = ingredient.Grams;
                Carbs += ingredient.Ingredient.Carbs * grams * 0.01;
                Fat += ingredient.Ingredient.Fat * grams * 0.01;
                Protein += ingredient.Ingredient.Protein * grams * 0.01;
            }
        }

        public List<IngredientInstance> SyncIngredients(List<Ingredient> avaiableIngredients)
        {

            var ingredientsToRemove = new List<IngredientInstance>();
            if(avaiableIngredients == null)
                return ingredientsToRemove;
            foreach(var ingredient in IngredientInstances)
            {
                var matchingIngredient = avaiableIngredients.FirstOrDefault(ai => ai.Name == ingredient.Ingredient.Name);
                if(matchingIngredient == null)
                    ingredientsToRemove.Add(ingredient);
                else
                {
                    ingredient.Ingredient = matchingIngredient;
                    ingredient.UpdateCount();
                    //ingredient.Ingredient.Carbs = matchingIngredient.Carbs;
                    //ingredient.Ingredient.Protein = matchingIngredient.Protein;
                    //ingredient.Ingredient.Fat = matchingIngredient.Fat;
                }

            }
            ingredientsToRemove.ForEach(ingredient => IngredientInstances.Remove(ingredient));
            return ingredientsToRemove;
        }

    }
}
