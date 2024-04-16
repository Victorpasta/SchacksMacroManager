using Caliburn.Micro;
using SchacksMacroManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SchacksMacroManager.ViewModels
{
    public class NewIngredientViewModel : Screen
    {
        public Visibility SettingsButtonVisibility { get; }

        public bool NewIngredientButtonEnabled { get => Name != string.Empty && Name != null; }

        public int NameTextBoxMaxWidth { get => IsNew ? 99999 : 125; }
        public string ButtonCharacter { get;}
        public bool IsNew { get => ButtonCharacter == "+"; }
        public Ingredient Ingredient { get; set; }
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    Ingredient.Name = value;
                    NotifyOfPropertyChange(() => Name);
                    NotifyOfPropertyChange(() => NewIngredientButtonEnabled);
                }
            }
        }

        private string _carbs;
        public string Carbs
        {
            get => _carbs;
            set
            {
                if (value != _carbs)
                {
                    _carbs = value;
                    Ingredient.Carbs = MacroManager.StringToDouble(value);
                    NotifyOfPropertyChange(() => Carbs);
                }
            }
        }
        private string _protein;
        public string Protein
        {
            get => _protein;
            set
            {
                if (value != _protein)
                {
                    _protein = value;
                    Ingredient.Protein = MacroManager.StringToDouble(value);
                    NotifyOfPropertyChange(() => Protein);
                }
            }
        }

        private string _fat;
        public string Fat
        {
            get => _fat;
            set
            {
                if (value != _fat)
                {
                    _fat = value;
                    Ingredient.Fat = MacroManager.StringToDouble(value);
                    NotifyOfPropertyChange(() => Fat);
                }
            }
        }



        public MacrosViewModel ParentVm { get; }

        public NewIngredientViewModel(MacrosViewModel parentVm)
        {
            ParentVm = parentVm;
            ButtonCharacter = "+";
            Ingredient = new Ingredient();
            SettingsButtonVisibility = Visibility.Collapsed;

        }

        public NewIngredientViewModel(Ingredient ingredient, MacrosViewModel parentVm)
        {
            SettingsButtonVisibility = Visibility.Visible;
            ParentVm = parentVm;
            Ingredient = ingredient;
            Carbs = Ingredient.Carbs.ToString();
            Protein = Ingredient.Protein.ToString();
            Fat = Ingredient.Fat.ToString();
            Name = Ingredient.Name.ToString();
            ButtonCharacter = "X";
        }

        //NutrientScore determains carbs, protein or fat, crabs = 0, protein = 1, fat = 2
        public void IngredientKeyHandlerFunction(KeyEventArgs keyArgs, string s, int nutrientInt)
        {
            if (keyArgs.Key == Key.Enter && NewIngredientButtonEnabled)
            {
                DeleteOrAddNew();
                return;
            }
            if (nutrientInt >= 0)
            {
                string output = Regex.Replace(s, "[^0-9.,]", "");
                if (nutrientInt == 0)
                    Carbs = output;
                if (nutrientInt == 1)
                    Protein = output;
                if (nutrientInt == 2)
                    Fat = output;
            }
            else if (nutrientInt == -1)
                Name = s;
            
            Update();
        }

        public void Update()
        {
            NotifyOfPropertyChange(() => Carbs);
            NotifyOfPropertyChange(() => Protein);
            NotifyOfPropertyChange(() => Fat);
            ParentVm.Update();
        }

        public void DeleteOrAddNew()
        {
            //Delete
            if(ButtonCharacter == "X")
            {
                ParentVm.MacroManager.AvailableIngredients.Remove(Ingredient);
                ParentVm.AvailableIngredientVms.Remove(this);
            }
            //Add new
            if (ButtonCharacter == "+")
            {
                AddIngredientAlphabetically();
                Ingredient = new Ingredient();
                Name = "";
                Carbs = "";
                Protein = "";
                Fat = "";
            }
            Update();
        }

        public void OpenSettings()
        {
            var bootstrapper = ParentVm.Bootstrapper;
            var vm = new IngredientSettingsViewModel(Ingredient, ParentVm);
            bootstrapper.ShowDialog(vm);
        }
        private void AddIngredientAlphabetically()
        {
            ParentVm.MacroManager.AvailableIngredients.Add(Ingredient);
            ParentVm.SortAvailableIngredients();
            var addedNewIngredientVm = new NewIngredientViewModel(Ingredient, ParentVm);
            int index = ParentVm.MacroManager.AvailableIngredients.IndexOf(Ingredient);
            if (index == ParentVm.MacroManager.AvailableIngredients.Count)
                ParentVm.AvailableIngredientVms.Add(addedNewIngredientVm);
            else 
                ParentVm.AvailableIngredientVms.Insert(index, addedNewIngredientVm);
        }
    }
}
