using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchacksMacroManager
{
    public class Dish
    {
        private double MeanCarbs { get; set; }
        private double MeanFat { get; set; }
        private double MeanProtein { get; set; }
        public double Carbs { get => MeanCarbs * Grams; }
        public double Fat { get => MeanFat * Grams; }
        public double Protein { get => MeanProtein * Grams; }
        public int TotKcal { get => (int) ((MeanCarbs*4 + MeanFat*9 + MeanProtein * 4) * Grams); }

        public int Grams { get; set; }
        public Dish(Ingredient[] ingredients, int grams) 
        {
            Grams = grams;
            Cook(ingredients);
        }

        private void Cook(Ingredient[] ingredients)
        {
            double totCarbs = 0;
            double totFat = 0;
            double totProtein = 0;
            int totGrams = 0;
            foreach(Ingredient ingredient in ingredients)
            {
                totCarbs += ingredient.Carbs * ingredient.Grams;
                totFat += ingredient.Fat * ingredient.Grams;
                totProtein += ingredient.Protein * ingredient.Grams;
                totGrams += ingredient.Grams;
            }
            MeanCarbs = totCarbs / totGrams;
            MeanFat = totFat / totGrams;
            MeanProtein = totProtein / totGrams;

        }
    }
}
