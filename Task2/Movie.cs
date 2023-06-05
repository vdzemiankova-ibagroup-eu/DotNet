using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Xml.Serialization;
using Azure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Task2
{
    [Serializable]
    [XmlType(TypeName = "Record")]
    public class Movie : INotifyPropertyChanged, IDataErrorInfo
    {
        private int _Id;
        private string _FirstName;
        private string _LastName;
        private string _MovieName;
        private int? _MovieYear;
        private decimal? _MovieRating;

        [XmlAttribute("id")]
        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged("Id");
            }
        }
        public string FirstName
        {
            get { return _FirstName; }
            set
            {
                _FirstName = value;
                OnPropertyChanged("FirstName");
            }
        }
        public string LastName
        {
            get { return _LastName; }
            set
            {
                _LastName = value;
                OnPropertyChanged("LastName");
            }
        }
        public string MovieName
        {
            get { return _MovieName; }
            set
            {
                _MovieName = value;
                OnPropertyChanged("MovieName");
            }
        }

        public int? MovieYear
        {
            get { return _MovieYear.GetValueOrDefault(0); }
            set
            {
                _MovieYear = value.GetValueOrDefault(0);
                OnPropertyChanged("MovieYear");
            }
        }
        public decimal? MovieRating
        {
            get { return _MovieRating.GetValueOrDefault(0.0m); }
            set
            {
                _MovieRating = value.GetValueOrDefault(0.0m);
                OnPropertyChanged("MovieRating");
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "MovieYear":
                        if (((MovieYear < 1895) || (MovieYear > 2030)) && (MovieYear != 0))
                        {
                            error = "Год выпуска фильма должен быть от 1895 до 2030";
                        }
                        break;
                    case "MovieRating":
                        if ((MovieRating < 0m) || (MovieRating > 10m))
                        {
                            error = "Рейтинг фильма должен быть десятичным числом от 0 до 10";
                        }
                        break;
                }
                return error;
            }
        }

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
                string error = this[prop];
                if (!string.IsNullOrEmpty(error))
                {
                    MessageBox.Show(error);
                    PropertyChanged(this, new PropertyChangedEventArgs(prop));
                }
            }
        }
    }
}
