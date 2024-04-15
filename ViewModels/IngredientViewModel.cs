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

        private string _grams;
        public string Grams
        {
            get => _grams;
            set
            {
                int numericalValue;
                _grams = value;
                try
                {
                    numericalValue = int.Parse(value);
                }
                catch
                {
                    numericalValue = 0;
                }
                Ingredient.Grams = numericalValue;
                NotifyOfPropertyChange(() => Grams);
            }
        }
        public EntryViewModel ParentVm { get; }

        public IngredientViewModel(IngredientInstance ingredient, EntryViewModel parentVm)
        {
            Ingredient = ingredient;
            Grams = Ingredient.Grams.ToString();
            Name = ingredient.Ingredient.Name;
            ParentVm = parentVm;
        }

        public void KeyHandlerFunction(KeyEventArgs keyArgs, string s)
        {
            string output = Regex.Replace(s, "[^0-9]", "");
            Grams = output;
            ParentVm.Update();
        }

        public void Delete()
        {
            ParentVm.Ingredients.Remove(this);
            ParentVm.Entry.IngredientInstances.Remove(Ingredient);
            ParentVm.Update();
        }

    }
}
