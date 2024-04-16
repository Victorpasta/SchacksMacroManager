using Caliburn.Micro;
using SchacksMacroManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SchacksMacroManager.ViewModels
{
    public class IngredientSettingsViewModel : Screen
    {
        private MacrosViewModel _parentVm;
        private double _numericalGramsPerCount;
        private string _gramsPerCount;
        public string GramsPerCount
        {
            get => _gramsPerCount;
            set
            {
                if (value != _gramsPerCount)
                {
                    _gramsPerCount = value;
                    _numericalGramsPerCount = MacroManager.StringToDouble(GramsPerCount);
                    NotifyOfPropertyChange(() => GramsPerCount);
                }
            }
        }

        private bool _useCount;
        public bool UseCount
        {
            get => _useCount;
            set
            {
                if (value != _useCount)
                {
                    _useCount = value;
                    NotifyOfPropertyChange(() => UseCount);
                }
            }
        }

        public Ingredient Ingredient { get; set; }
        public IngredientSettingsViewModel(Ingredient ingredient, MacrosViewModel parentVM)
        {
            Ingredient = ingredient;
            UseCount = Ingredient.UseCount;
            GramsPerCount = Ingredient.GramsPerCount.ToString();
            _parentVm = parentVM;
        }

        public async Task SaveAndClose()
        {
            Ingredient.UseCount = UseCount;
            if (_numericalGramsPerCount == 0 && UseCount)
            {
                MessageBox.Show("Choose a valid number for grams per count");
                return;
            }
            Ingredient.GramsPerCount = _numericalGramsPerCount;
            _parentVm.Update();
            await TryCloseAsync();
        }
    }
}
