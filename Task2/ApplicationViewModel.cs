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
        private int pageSize = 5;
        private int pageIndex = 1;
        public int PageIndex
        {
            get { return pageIndex; }
            set
            {
                pageIndex = value;
                OnPropertyChanged("PageIndex");
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
                              allMovies = (from movie in db.Movies select movie).ToList();
                              filteredMovies = allMovies;
                              PageIndex = 1;
                              Movies.Clear();
                              foreach (Movie fm in filteredMovies.Skip(pageSize * (PageIndex - 1)).Take(pageSize))
                              {
                                  Movies.Add(fm);
                              }
                              
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
                          int count = filteredMovies.Count;
                          if (count - pageSize * PageIndex > 0)
                          {
                              PageIndex += 1;
                              Movies.Clear();
                              foreach (Movie fm in filteredMovies.Skip(pageSize * (PageIndex - 1)).Take(pageSize))
                              {
                                  Movies.Add(fm);
                              }
                          }
                          else
                          {
                              dialogService.ShowMessage("Достигнута последняя страница");
                          }
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
                          if (PageIndex != 1)
                          {
                              PageIndex -= 1;
                              Movies.Clear();
                              foreach (Movie fm in filteredMovies.Skip(pageSize * (PageIndex - 1)).Take(pageSize))
                              {
                                  Movies.Add(fm);
                              }
                          }
                          else
                          {
                              dialogService.ShowMessage("Достигнута первая страница");
                          }
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
                          Movies.Clear();
                          foreach (Movie fm in filteredMovies.Skip(pageSize * (PageIndex - 1)).Take(pageSize))
                          {
                              Movies.Add(fm);
                          }
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
            Movies = new ObservableCollection<Movie>(filteredMovies.Skip(pageSize * (PageIndex - 1)).Take(pageSize));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}

