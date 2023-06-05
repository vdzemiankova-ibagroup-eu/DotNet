using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DocumentFormat.OpenXml.Office2013.Word;
using Microsoft.IdentityModel.Tokens;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Task2
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private Movie selectedMovie;

        private int _PageSize = 5;

        private int _PageIndex = 1;
        public int PageIndex
        {
            get { return _PageIndex; }
            set
            {
                _PageIndex = value;
                OnPropertyChanged("PageIndex");
            }
        }

        private int _PageCount = 1;
        public int PageCount
        {
            get { return _PageCount; }
            set
            {
                _PageCount = value;
                OnPropertyChanged("PageCount");
            }
        }

        private bool _IsPreviousEnabled = false;
        public bool IsPreviousEnabled
        {
            get { return _IsPreviousEnabled; }
            set
            {
                _IsPreviousEnabled = value;
                OnPropertyChanged("IsPreviousEnabled");
            }
        }

        private bool _IsNextEnabled = false;
        public bool IsNextEnabled
        {
            get { return _IsNextEnabled; }
            set
            {
                _IsNextEnabled = value;
                OnPropertyChanged("IsNextEnabled");
            }
        }


        ApplicationContext db = new ApplicationContext();

        IFileOpenService fileOpenService;
        Dictionary<string, IFileSaveService> fileSaveServices;
        IDialogService dialogService;

        public ObservableCollection<Movie> Movies { get; set; }
        private List<Movie> allMovies;
        private List<Movie> filteredMovies;
        public Movie MovieFilter { get; set; } = new Movie();

        private void ViewPage() 
        {
            Movies.Clear();
            foreach (Movie fm in filteredMovies.Skip(_PageSize * (PageIndex - 1)).Take(_PageSize))
            {
                Movies.Add(fm);
            }

            IsPreviousEnabled = PageIndex != 1;
            IsNextEnabled = PageCount > PageIndex;

        }



        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ??
                  (saveCommand = new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.SaveFileDialog() == true)
                          {
                              fileSaveServices[(string)obj].Save(dialogService.FilePath, allMovies);
                              dialogService.ShowMessage($"Файл {dialogService.FilePath} сохранен");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage($"Файл не сохранён, произошла ошибка\n{ex}");
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.OpenFileDialog() == true)
                          {
                              var movies = fileOpenService.Open(dialogService.FilePath);

                              foreach (var m in movies)
                              {
                                  Validate(m);
                                  db.Movies.Add(m);
                              }
                              db.SaveChanges();

                              db.Movies.Load();
                              allMovies = db.Movies.ToList();
                              filteredMovies = allMovies;

                              PageIndex = 1;
                              PageCount = (int)Math.Ceiling((double)filteredMovies.Count / _PageSize);
                              PageCount = PageCount == 0 ? 1 : PageCount;
                              ViewPage();
                              
                              dialogService.ShowMessage($"Файл {dialogService.FilePath} открыт");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage($"{ex.Message}\nДанные не сохранены");
                      }
                  }));
            }
        }
        public Movie SelectedMovie
        {
            get { return selectedMovie; }
            set
            {
                selectedMovie = value;
                OnPropertyChanged("SelectedMovie");
            }
        }

        private void Validate(Movie movie)
        {
            string error = movie["MovieYear"] + movie["MovieRating"];
            if (!error.IsNullOrEmpty())
            {
                throw new Exception(error); 
            }
        }

        private RelayCommand moveToNextPage;
        public RelayCommand MoveToNextPage
        {
            get
            {
                return moveToNextPage ??
                  (moveToNextPage = new RelayCommand(obj =>
                  {
                      try
                      {
                          PageIndex += 1;
                          ViewPage();
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }

        private RelayCommand moveToPreviousPage;
        public RelayCommand MoveToPreviousPage
        {
            get
            {
                return moveToPreviousPage ??
                  (moveToPreviousPage = new RelayCommand(obj =>
                  {
                      try
                      {
                          PageIndex -= 1;
                          ViewPage();
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }

        private RelayCommand applyFilter;
        public RelayCommand ApplyFilter
        {
            get
            {
                return applyFilter ??
                  (applyFilter = new RelayCommand(obj =>
                  {
                      try
                      {
                          Validate(MovieFilter);

                          filteredMovies = (from movie in db.Movies
                                        where (movie.MovieName == MovieFilter.MovieName || MovieFilter.MovieName == null || MovieFilter.MovieName == "")
                                           && (movie.FirstName == MovieFilter.FirstName || MovieFilter.FirstName == null || MovieFilter.FirstName == "")
                                           && (movie.LastName == MovieFilter.LastName || MovieFilter.LastName == null || MovieFilter.LastName == "")
                                           && (movie.MovieYear == MovieFilter.MovieYear || MovieFilter.MovieYear == 0)
                                           && (movie.MovieRating == MovieFilter.MovieRating || MovieFilter.MovieRating == 0m)
                                        select movie).ToList();

                          PageIndex = 1;
                          PageCount = (int)Math.Ceiling((double)filteredMovies.Count / _PageSize);
                          PageCount = PageCount == 0 ? 1 : PageCount;
                          ViewPage();
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }

        public ApplicationViewModel(IDialogService dialogService, IFileOpenService fileOpenService, Dictionary<string, IFileSaveService> fileSaveServices)
        {
            this.dialogService = dialogService;
            this.fileOpenService = fileOpenService;
            this.fileSaveServices = fileSaveServices;

            db.Database.EnsureCreated();
            db.Movies.Load();
            allMovies = db.Movies.Local.ToList();
            filteredMovies = new List<Movie>(allMovies);
            Movies = new ObservableCollection<Movie>(filteredMovies.Skip(_PageSize * (PageIndex - 1)).Take(_PageSize));

            PageCount = (int)Math.Ceiling((double)filteredMovies.Count / _PageSize);
            PageCount = PageCount == 0 ? 1 : PageCount;
            IsNextEnabled = PageCount > PageIndex ? true : false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

