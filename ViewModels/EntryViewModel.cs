using Caliburn.Micro;
using SchacksMacroManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchacksMacroManager.ViewModels
{
    public class EntryViewModel : Screen
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    Entry.Name = value;
                    NotifyOfPropertyChange(() => Name);
                }
            }
        }
        public double Carbs { get => Math.Round(Entry.Carbs, 2); }
        public double Protein { get => Math.Round(Entry.Protein,2); }
        public double Fat { get => Math.Round(Entry.Fat, 2); }
        public double Kcal { get => Math.Round(Entry.Kcal, 2); }

        public bool NewIngredientButtonEnabled { get => AvailableIngredientsNames.Contains(NextIngredientName); }

        private ObservableCollection<string> _availableIngredientsNames = new ObservableCollection<string>();
        public ObservableCollection<string> AvailableIngredientsNames
        {
            get
            {
                var ingrList = new ObservableCollection<string>();
                if(AvailableIngredients == null)
                    return ingrList;
                AvailableIngredients.ForEach(ai=> ingrList.Add(ai.Name));
                return ingrList;
            }
        }
        public MacrosViewModel ParentVm { get; set; }
        public List<Ingredient> AvailableIngredients { get; set; }
        public Ingredient NextIngredient { get => GetIngredientFromName(NextIngredientName); }
        private string _nextIngredientName;
        public string NextIngredientName
        {
            get => _nextIngredientName;
            set
            {
                if (value != _nextIngredientName)
                {
                    _nextIngredientName = value;
                    NotifyOfPropertyChange(() => NextIngredientName);
                    NotifyOfPropertyChange(() => NewIngredientButtonEnabled);
                }
            }
        }

        public BindableCollection<IngredientViewModel> Ingredients { get; set; }
        public Entry Entry { get; set; }
        public EntryViewModel(Entry entry, MacroManager manager, MacrosViewModel parentVm)
        {
            ParentVm = parentVm;
            Ingredients = new BindableCollection<IngredientViewModel>();
            AvailableIngredients = manager.AvailableIngredients;
            AvailableIngredients.ForEach(x => AvailableIngredientsNames.Add(x.Name));
            Entry = entry;
            Name = Entry.Name;
            foreach (var ingredient in entry.IngredientInstances)
            {
                AddIngredientVMFromIngredientInEntry(ingredient);
            }
            NotifyOfPropertyChange(() => AvailableIngredientsNames);
            NotifyOfPropertyChange(() => NextIngredientName);

            Update();
        }

        public void AddNewIngredient()
        {
            var ingredientIntance = new IngredientInstance(NextIngredient, 0);
            var vm = new IngredientViewModel(ingredientIntance, this);
            vm.GramsOrCount = "";
            Ingredients.Add(vm);
            Entry.IngredientInstances.Add(ingredientIntance);
        }
        private void AddIngredientVMFromIngredientInEntry(IngredientInstance ingredient)
        {
            if (!Entry.IngredientInstances.Contains(ingredient))
                return;
            var vm = new IngredientViewModel(ingredient, this);
            vm.GramsOrCount = ingredient.Ingredient.UseCount ? ingredient.Count.ToString() : ingredient.Grams.ToString();
            Ingredients.Add(vm);
        }

        private Ingredient GetIngredientFromName(string name)
        {
            return AvailableIngredients.FirstOrDefault(x => x.Name == name);
        }
        public void Update(bool updateParent = true)
        {
            AvailableIngredients = ParentVm.AvailableIngredients;
            var ingredientsToRemove = Entry.SyncIngredients(AvailableIngredients);
            RemoveVmFromIngredient(ingredientsToRemove);
            Entry.CalculateMacros();
            NotifyOfPropertyChange(() => AvailableIngredientsNames);
            NotifyOfPropertyChange(() => Carbs);
            NotifyOfPropertyChange(() => Protein);
            NotifyOfPropertyChange(() => Fat);
            NotifyOfPropertyChange(() => Kcal);
            foreach(var vm in Ingredients)
            {
                vm.Update();
            }
            if (updateParent)
                ParentVm.Update();
        }

        public void RemoveVmFromIngredient(List<IngredientInstance> ingredientsToRemove)
        {
            var vmsToRemove = new List<IngredientViewModel>();
            foreach (var vm in Ingredients)
            {
                if (ingredientsToRemove.Contains(vm.Ingredient))
                    vmsToRemove.Add(vm);
            }
            vmsToRemove.ForEach(vm => Ingredients.Remove(vm));
        }
        public void Delete()
        {
            ParentVm.Entries.Remove(this);
            ParentVm.Update();
        }

        public void MakeIngredient()
        {
            int totalGrams = 0;
            foreach(var ingredient in Entry.IngredientInstances)
                totalGrams += ingredient.Grams;
            double n100Grams = totalGrams * 0.01;
            double carbsPer100Grams = Math.Round(Carbs / n100Grams, 1);
            double proteinPer100Grams = Math.Round(Protein / n100Grams, 1);
            double fatPer100Grams = Math.Round(Fat / n100Grams, 1);
            var newIngredient = new Ingredient(Name, carbsPer100Grams, fatPer100Grams, proteinPer100Grams);
            ParentVm.MacroManager.AvailableIngredients.Add(newIngredient);
            ParentVm.SortAvailableIngredients();
            var newIngredientVm = new NewIngredientViewModel(newIngredient, ParentVm);
            int index = ParentVm.MacroManager.AvailableIngredients.IndexOf(newIngredient);
            if (index == ParentVm.MacroManager.AvailableIngredients.Count)
                ParentVm.AvailableIngredientVms.Add(newIngredientVm);
            else
                ParentVm.AvailableIngredientVms.Insert(index, newIngredientVm);
        }
    }
}
