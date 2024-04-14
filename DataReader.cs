using System;
using ExcelDataReader;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.IO;

namespace SchacksMacroManager
{
    public class DataReader
    {
        private string _excelPath;
        public DataReader(string excelPath) 
        {
            _excelPath = excelPath;
        }

        public List<Ingredient> ReadData()
        {
            List<Ingredient> ingredients = new List<Ingredient>();
            using (var stream = File.Open(_excelPath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet();
                    var tables = CreateDataTableArray(result);
                    foreach (var table in tables)
                    {
                        var rows = table.Rows;
                        rows.RemoveAt(0);
                        foreach (DataRow row in rows)
                            ingredients.Add(CreateIngredient(row));
                    }

                }
            }
            return ingredients;
        }

        private Ingredient CreateIngredient(DataRow dataRow)
        {
            var items = dataRow.ItemArray;
            return new Ingredient((string)items[0], (double)items[1], (double)items[2], (double)items[3]);
        }

        private DataTable[] CreateDataTableArray(DataSet dataSet)
        {
            var list = new List<DataTable>();
            var count = dataSet.Tables.Count;
            for (int i = 0; i < count; i++)
            {
                var table = dataSet.Tables[i];
                list.Add(table);
            }
            return list.ToArray();
        }


    }
}
