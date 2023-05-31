using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Task2
{
    internal class CsvFileOpenService : IFileOpenService
    {
        public List<Movie> Open(string filename)
        {
            if (!filename.EndsWith(".csv"))
            {
                throw new ArgumentException("Неверное расширение файла");
            }

            List<Movie> movies = new List<Movie>();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = false,
            };

            try 
            {
                using (var reader = new StreamReader(filename))
                using (var csv = new CsvReader(reader, config))
                {
                    csv.Context.RegisterClassMap<MovieMap>();
                    movies = csv.GetRecords<Movie>().ToList();
                }
            }
            catch
            {
                throw new Exception("Не удалось распарсить файл, проверьте структуру файла или выберите другой");
            }
            return movies;
        }
    }
}
