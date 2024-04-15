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
        public double Carbs { get => Entry.Carbs; }
        public double Protein { get => Entry.Protein; }
        public double Fat { get => Entry.Fat; }
        public double Kcal { get => Entry.Kcal; }

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
            vm.Grams = "";
            Ingredients.Add(vm);
            Entry.IngredientInstances.Add(ingredientIntance);
        }
        private void AddIngredientVMFromIngredientInEntry(IngredientInstance ingredient)
        {
            if (!Entry.IngredientInstances.Contains(ingredient))
                return;
            var vm = new IngredientViewModel(ingredient, this);
            vm.Grams = ingredient.Grams.ToString();
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
    }
}
