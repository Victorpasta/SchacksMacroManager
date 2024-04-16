using Caliburn.Micro;
using SchacksMacroManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchacksMacroManager.ViewModels
{
    public class IngredientViewModel : Screen
    {
        public string GramsOrCountLabel { get => Ingredient.Ingredient.UseCount ? "Count" : "Grams"; }
        public IngredientInstance Ingredient { get; set; }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    NotifyOfPropertyChange(() => Name);
                }
            }
        }

        private string _gramsOrCount;
        public string GramsOrCount
        {
            get => _gramsOrCount;
            set
            {
                int numericalValue;
                _gramsOrCount = value;
                try
                {
                    numericalValue = int.Parse(value);
                }
                catch
                {
                    numericalValue = 0;
                }
                if (Ingredient.Ingredient.UseCount)
                {
                    Ingredient.Count = numericalValue;
                    Ingredient.Grams = (int)(numericalValue * Ingredient.Ingredient.GramsPerCount);
                }
                else
                {
                    Ingredient.Grams = numericalValue;
                    Ingredient.UpdateCount();
                }
                
            }
        }
        public EntryViewModel ParentVm { get; }

        public IngredientViewModel(IngredientInstance ingredient, EntryViewModel parentVm)
        {
            Ingredient = ingredient;
            GramsOrCount = Ingredient.Ingredient.UseCount ? Ingredient.Count.ToString() : Ingredient.Grams.ToString();
            Name = ingredient.Ingredient.Name;
            ParentVm = parentVm;
        }

        public void KeyHandlerFunction(KeyEventArgs keyArgs, string s)
        {
            string output = s;
            if (keyArgs.Key == Key.Up || keyArgs.Key == Key.Down)
            {
                int value;
                var key = keyArgs.Key;
                try
                {
                    value = int.Parse(output);
                } catch
                {
                    value = 0;
                }
                value = key == Key.Up ? value + 1 : value - 1;
                if(value < 1)
                    value = 1;
                output = value.ToString();
            }
            output = Regex.Replace(output, "[^0-9]", "");
            GramsOrCount = output;
            ParentVm.Update();
        }

        public void Delete()
        {
            ParentVm.Ingredients.Remove(this);
            ParentVm.Entry.IngredientInstances.Remove(Ingredient);
            ParentVm.Update();
        }

        public void Update()
        {
            GramsOrCount = Ingredient.Ingredient.UseCount ? Ingredient.Count.ToString() : Ingredient.Grams.ToString();
            NotifyOfPropertyChange(() => GramsOrCount);
            NotifyOfPropertyChange(() => GramsOrCountLabel);
        }

    }
}
