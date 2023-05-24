using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите свои имя, фамилию и год рождения по следующему шаблону:");
            Console.WriteLine("Иван Иванов 03/24/2001\n");
            string input = ValidateInput();
            Person person = new Person(input);
            person.PrintPerson();
            person.BirthdayCountdown();
            person.PrintHappyBirthday();
            Console.ReadLine();

        }

        static string ValidateInput()
        {
            string input = Console.ReadLine();
            if (input.Length == 0 || input.Length > 40)
            {
                Console.WriteLine("Введённая строка должна быть от 1 до 40 символов");
                Environment.Exit(1);
            }
            return input;
        }
    }
    public class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime Birthday { get; set; }
        public Person(string input)
        {
            string[] splittingInput = Regex.Split(input, @"[^\w\d\/]");
            Name = splittingInput[0];
            Surname = splittingInput[1];
            Birthday = DateTime.ParseExact(splittingInput[2], "MM/dd/yyyy", new CultureInfo("en-US"));
        }

        public DateTime CalculateNextBirthday()
        {
            DateTime today = DateTime.Now;
            DateTime nextBirthday = Birthday.AddYears(today.Year - Birthday.Year);
            if (nextBirthday <= today)
            {
                nextBirthday = nextBirthday.AddYears(1);
            }
            return nextBirthday;
        }

        public int CalculateAge()
        {
            DateTime today = DateTime.Now;
            int age = today.Year - Birthday.Year;
            if (Birthday.AddYears(age) > today)
            {
                age--;
            }
            return age;
        }

        public string BirthdayStringBuilder()
        {
            return Birthday.ToString("dddd dd MMMM yyyy", new CultureInfo("en-US"));
        }

        public void PrintPerson()
        {
            Console.WriteLine("\nИмя: {0}", Name);
            Console.WriteLine("Фамилия: {0}", Surname);
            Console.WriteLine("Родился: {0}", BirthdayStringBuilder());
            Console.WriteLine("Количество полных лет: {0}", CalculateAge());
        }

        public void BirthdayCountdown()
        {
            DateTime nextBirthday = CalculateNextBirthday();
            TimeSpan timeUntilBirthday = nextBirthday - DateTime.Now;
            int secondsUntilBirthday = (int)timeUntilBirthday.TotalSeconds;

            for (int i = secondsUntilBirthday; i >= 0; i--)
            {
                Console.WriteLine("Следующий день рождения через: {0} days, {1} hours, {2} minutes, {3} seconds", timeUntilBirthday.Days, timeUntilBirthday.Hours, timeUntilBirthday.Minutes, timeUntilBirthday.Seconds);
                timeUntilBirthday = nextBirthday - DateTime.Now;
                System.Threading.Thread.Sleep(1000);
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, --currentLineCursor);
            }
        }

        public void PrintHappyBirthday()
        {
            Console.WriteLine("Поздравляем, вам исполнилось {0}", CalculateAge());
        }
    }
}
