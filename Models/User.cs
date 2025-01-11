using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchacksMacroManager.Models
{
    [Serializable]
    public class User
    {
        public string DailyCarbs { get; set; }
        public string DailyProtein { get; set; }
        public string DailyFat { get; set; }
        public List<Entry> Entries { get; set; }
        public string Name { get; set; }
        public User(string name)
        {
            Name = name;
            DailyCarbs = "0";
            DailyProtein = "0";
            DailyFat = "0";
            Entries = new List<Entry>();
        }
        public User() { }

    }
}
