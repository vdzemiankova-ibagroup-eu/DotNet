using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace Task2
{
    internal class XmlFileSaveService : IFileSaveService
    {
        public void Save(string filename, List<Movie> moviesList)
        {
            if (!filename.EndsWith(".xml"))
            {
                filename += ".xml";
            }
            XmlSerializer xmlFormatter =
                new XmlSerializer(typeof(List<Movie>), new XmlRootAttribute("TestProgram"));
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                xmlFormatter.Serialize(fs, moviesList);
            }
        }
    }
}
