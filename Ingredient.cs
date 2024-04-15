using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchacksMacroManager
{
    [Serializable]
    public class Ingredient
    {
        public string Name { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Protein {  get; set; }
        public Ingredient(string name, double carbs, double fat, double protein) 
        {
            Name = name;
            Carbs = carbs;
            Fat = fat;
            Protein = protein;
        }
        public Ingredient()
        {

        }

        public int CalculateKcalPerGram() 
        {
            var fatKcal = Fat * 9;
            var protienKcal = Protein * 4;
            var carbsKcal = Carbs * 4;
            return (int)((fatKcal + carbsKcal + protienKcal));
        }
    }
}
