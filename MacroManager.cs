using SchacksMacroManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SchacksMacroManager
{
    [Serializable]
    public class MacroManager
    {
       
        public List<User> Users { get; set; }
        public User ActiveUser { get; set; }
        public List<Entry> Entries 
        { 
            get 
            {

                return ActiveUser?.Entries ?? new List<Entry>();
            } 
            set 
            {
                ActiveUser.Entries = value;
            } 
        }

        public List<Ingredient> AvailableIngredients { get; set; }
        public void SerializeMacroManager(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MacroManager));
            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, this);
            }
        }

        public static MacroManager DeserializeMacroManager(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(MacroManager));
            using (TextReader reader = new StreamReader(filePath))
            {
                return (MacroManager)serializer.Deserialize(reader);
            }
        }

        public static double StringToDouble(string s)
        {
            try
            {
                return double.Parse(s);
            }
            catch 
            {
                if (s.Contains(","))
                    s = s.Replace(",", ".");
                if (s.Contains("."))
                    s = s.Replace(".", ",");
                try
                {
                    return double.Parse(s);
                } catch 
                {
                    return 0;
                }
            }
        }
    }
}
