using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.Excel;
using DocumentFormat.OpenXml.Office2013.Word;

namespace Task2
{
    internal class ExcelFileService : IFileSaveService
    {
        public void Save(string filename, List<Movie> moviesList)
        {
            if (!filename.EndsWith(".xlsx") && !filename.EndsWith(".xls"))
            {
                filename += ".xlsx";
            }
            using (var writer = new ExcelWriter(filename, CultureInfo.InvariantCulture))
            {
                writer.Context.RegisterClassMap<MovieMap>();
                writer.WriteRecords(moviesList);
            }

        }
    }
}
