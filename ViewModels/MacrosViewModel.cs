using Caliburn.Micro;
using SchacksMacroManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace SchacksMacroManager.ViewModels
{
    public class MacrosViewModel : Screen
    {
        private string _filePath;
        private Timer _saveTimer;
        public MacroManager MacroManager { get; private set; }
        public SolidColorBrush TextColor { get => DetermineColor(); }
        public string TotCarbs { get => CalculateTotalCarbs(); }
        public string TotProtein { get => CalculateTotalProtein(); }
        public string TotFat { get => CalculateTotalFat(); }

        public string TotKcal
        {
            get => (MacroManager.StringToDouble(TotCarbs) * 4 + MacroManager.StringToDouble(TotProtein) * 4 + MacroManager.StringToDouble(TotFat) * 9
                ).ToString();
        }
        public string DailyKcal
        {
            get => (MacroManager.StringToDouble(DailyCarbs) * 4 + MacroManager.StringToDouble(DailyProtein) * 4 + MacroManager.StringToDouble(DailyFat) * 9
                ).ToString();
        }
        public string RemainingCarbs { get => CalculateRemainingCarbs(); }
        public string RemainingProtein { get => CalculateRemainingProtein(); }
        public string RemainingFat { get => CalculateRemainingFat(); }
        public string RemainingKcal { get => Math.Round(
            (
            MacroManager.StringToDouble(RemainingCarbs) * 4 
            + MacroManager.StringToDouble(RemainingProtein) * 4 
            + MacroManager.StringToDouble(RemainingFat) * 9
            ),2).ToString(); }

        public List<Ingredient> AvailableIngredients;
        public BindableCollection<NewIngredientViewModel> AvailableIngredientVms { get; set; }

        public NewIngredientViewModel NextNewIngredient { get; set; }
        public BindableCollection<EntryViewModel> Entries { get; set; }
        private string _dailyCarbs;
        public string DailyCarbs
        {
            get => _dailyCarbs;
            set
            {
                if (value != _dailyCarbs)
                {
                    _dailyCarbs = value;
                    NotifyOfPropertyChange(() => DailyCarbs);
                }
            }
        }

        private string _dailyProtein;
        public string DailyProtein
        {
            get => _dailyProtein;
            set
            {
                if (value != _dailyProtein)
                {
                    _dailyProtein = value;
                    NotifyOfPropertyChange(() => DailyProtein);
                }
            }
        }

        private string _dailyFat;
        public string DailyFat
        {
            get => _dailyFat;
            set
            {
                if (value != _dailyFat)
                {
                    _dailyFat = value;
                    NotifyOfPropertyChange(() => DailyFat);
                }
            }
        }
        public CaliburnBootstrapper Bootstrapper { get; set; }
        public MacrosViewModel(CaliburnBootstrapper bootstrapper) 
        {
            Bootstrapper = bootstrapper;
            DateTime today = DateTime.Now;
            AvailableIngredientVms = new BindableCollection<NewIngredientViewModel>();
            string formattedDate = today.ToString("yyMMdd");
            var saveFile = Directory.GetCurrentDirectory() + "\\Saves\\SaveFile" + formattedDate + ".xml";
            Entries = new BindableCollection<EntryViewModel>();
            MacroManager macroManager;
            _filePath = saveFile;
            if (File.Exists(saveFile))
            {
                macroManager = MacroManager.DeserializeMacroManager(saveFile);
                var entries = macroManager.Entries;
                var availableIngredients = macroManager.AvailableIngredients;
                if(availableIngredients == null || availableIngredients.Count < 1)
                {
                    InitializeFromPreviousDays();
                    macroManager.SerializeMacroManager(saveFile);
                }
                MacroManager = macroManager;
                SortAvailableIngredients();
                Load();
            }
            else
            {
                macroManager = new MacroManager();
                macroManager.AvailableIngredients = AvailableIngredients;
                macroManager.Entries = new List<Entry>();
                InitializeFromPreviousDays();
                MacroManager = macroManager;
                Save();
            }
            if(AvailableIngredients.Count > 0)
                AvailableIngredients.ForEach(ai => AvailableIngredientVms.Add(new NewIngredientViewModel(ai, this)));
            NextNewIngredient = new NewIngredientViewModel(this);
            //_saveTimer = new Timer(5000); // Set the interval to 5000 milliseconds (5 seconds)
            //_saveTimer.Elapsed += OnTimedSave; // Subscribe to the Elapsed event
            //_saveTimer.AutoReset = true; // Ensures the timer resets and ticks again
            //_saveTimer.Enabled = true; // Start the timer
        }

        public void AddNewEntry()
        {
            var entry = new Entry(new List<IngredientInstance>(), "");
            var vm = new EntryViewModel(entry, MacroManager, this);
            Entries.Add(vm);
        }

        private string CalculateTotalCarbs()
        {
            double totalCarbs = 0;
            foreach (var entry in Entries)
            {
                totalCarbs += entry.Carbs;
            }
            return Math.Round(totalCarbs,2).ToString();
        }

        private string CalculateTotalFat()
        {
            double totalFat = 0;
            foreach (var entry in Entries)
            {
                totalFat += entry.Fat;
            }
            return Math.Round(totalFat, 2).ToString();
        }

        private string CalculateTotalProtein()
        {
            double totalProtein = 0;
            foreach (var entry in Entries)
            {
                totalProtein += entry.Protein;
            }
            return Math.Round(totalProtein, 2).ToString();
        }

        private string CalculateRemainingCarbs()
        {
            var totalCarbs = double.Parse(TotCarbs);
            var dailyCarbs = double.Parse(DailyCarbs);
            var remainingCarbs = dailyCarbs - totalCarbs;
            return Math.Round(remainingCarbs, 2).ToString();
        }
        private string CalculateRemainingProtein()
        {
            var totalProtein = double.Parse(TotProtein);
            var dailyProtein = double.Parse(DailyProtein);
            var remainingProtein = dailyProtein - totalProtein;
            return Math.Round(remainingProtein, 2).ToString();
        }
        private string CalculateRemainingFat()
        {
            var totalFat = double.Parse(TotFat);
            var dailyFat = double.Parse(DailyFat);
            var remainingFat = dailyFat - totalFat;
            return Math.Round(remainingFat, 2).ToString();
        }

        public void Update()
        {
            NotifyAll();
            foreach (var item in Entries)
                item.Update(false);
            NotifyAll();
        }

        private void NotifyAll()
        {
            NotifyOfPropertyChange(() => TotCarbs);
            NotifyOfPropertyChange(() => TotKcal);
            NotifyOfPropertyChange(() => TotProtein);
            NotifyOfPropertyChange(() => TotFat);
            NotifyOfPropertyChange(() => DailyCarbs);
            NotifyOfPropertyChange(() => DailyProtein);
            NotifyOfPropertyChange(() => DailyFat);
            NotifyOfPropertyChange(() => DailyKcal);
            NotifyOfPropertyChange(() => RemainingCarbs);
            NotifyOfPropertyChange(() => RemainingKcal);
            NotifyOfPropertyChange(() => RemainingProtein);
            NotifyOfPropertyChange(() => RemainingFat);
            NotifyOfPropertyChange(() => TextColor);
        }


        public void KeyHandlerFunction(KeyEventArgs keyArgs, string s, int macro)
        {
            string output = Regex.Replace(s, "[^0-9]", "");
            if(macro == 1)
                DailyCarbs = output;
            if (macro == 2)
                DailyProtein = output;
            if (macro == 3)
                DailyFat = output;
            Update();
        }

        private void OnTimedSave(Object source, ElapsedEventArgs e) => Save();
        public void Save()
        {
            var entries = new List<Entry>();
            foreach (var entryVM in Entries)
                entries.Add(entryVM.Entry);
            MacroManager.Entries = entries;
            MacroManager.AvailableIngredients = AvailableIngredients;
            MacroManager.DailyCarbs = DailyCarbs;
            MacroManager.DailyProtein = DailyProtein;
            MacroManager.DailyFat = DailyFat;
            MacroManager.SerializeMacroManager(_filePath);
        }

        private void Load()
        {
            Entries.Clear();
            var entries = MacroManager.Entries;
            var availableIngredients = MacroManager.AvailableIngredients;
            foreach (var entry in entries)
                Entries.Add(new EntryViewModel(entry, MacroManager, this));
            AvailableIngredients = availableIngredients;
            SetDailyValuesFromMacroManager();
        }

        private void InitializeFromPreviousDays()
        {
            List<string> fileDates = new List<string>();
            var saveFolder = Path.GetDirectoryName(_filePath);
            if(Directory.GetFiles(saveFolder).Count() < 1)
            {
                DailyCarbs = "450";
                DailyProtein = "200";
                DailyFat = "135";
                AvailableIngredients = new List<Ingredient>();
                MacroManager = new MacroManager();
                Save();
                return;
            }
            var newsetFile = GetMostRecentFilePath(saveFolder);
            MacroManager macroManager = MacroManager.DeserializeMacroManager(newsetFile);
            SetDailyValuesFromMacroManager(macroManager);
            SetAvailableIngredientsFromMacroManager(macroManager);
            MacroManager = macroManager;
            Save();
        }


        private void SetDailyValuesFromMacroManager(MacroManager macroManager = null)
        {
            if (macroManager == null)
                macroManager = MacroManager;
            DailyCarbs = macroManager.DailyCarbs;
            DailyProtein = macroManager.DailyProtein;
            DailyFat = macroManager.DailyFat;

        }

        private void SetAvailableIngredientsFromMacroManager(MacroManager macroManager = null)
        {
            if (macroManager == null)
                macroManager = MacroManager;
            AvailableIngredients = macroManager.AvailableIngredients;
        }

        private string GetMostRecentFilePath(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            var files = directoryInfo.GetFiles();
            var mostRecentFile = files.OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
            return mostRecentFile?.FullName;
        }

        private SolidColorBrush DetermineColor()
        {
            bool isOver = false;
            try
            {
                var dailyCalories = double.Parse(DailyCarbs) * 4 + double.Parse(DailyProtein) * 4 + double.Parse(DailyFat) * 9;
                var totalCalories = double.Parse(TotCarbs) * 4 + double.Parse(TotProtein) * 4 + double.Parse(TotFat) * 9;
                isOver = dailyCalories <= totalCalories && double.Parse(DailyProtein) <= double.Parse(TotProtein);
            } catch { isOver = false; }
            return isOver ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Black);
        }

        public void LoadPreviousDay()
        {
            try
            {
                string fileToLoad;
                var fileDiag = new OpenFileDialog();
                fileDiag.InitialDirectory = Directory.GetCurrentDirectory() + "\\Saves";
                fileDiag.Filter = "Text Files (*.xml)|*.xml|All Files (*.*)|*.*";
                fileDiag.FilterIndex = 1;
                fileDiag.Multiselect = false;
                if (fileDiag.ShowDialog() == true)
                {
                    fileToLoad = fileDiag.FileName; // Return the selected file path
                    MacroManager = MacroManager.DeserializeMacroManager(fileToLoad);
                }
                Load();
            } catch { }
            
        }

        public void SortAvailableIngredients()
        {
            MacroManager.AvailableIngredients.Sort((i1, i2) => i1.Name.CompareTo(i2.Name));
        }
    }
}
