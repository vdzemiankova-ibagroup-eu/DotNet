using System.Globalization;

Console.WriteLine("Введите свои имя, фамилию и год рождения по следующему шаблону:");
Console.WriteLine("Иван Иванов 03/24/2001\n");
string input = Console.ReadLine();
if (input.Length == 0 || input.Length > 40) 
{ 
    Console.WriteLine("Введённая строка должна быть от 1 до 40 символов");
    return;
}
string[] splittingInput = input.Split(' ');
string name = splittingInput[0];
string surname = splittingInput[1];
DateTime birthday = DateTime.ParseExact(splittingInput[2], "MM/dd/yyyy", new CultureInfo("en-US"));

DateTime today = DateTime.Now;
string birthdayString = birthday.ToString("dddd dd MMMM yyyy", new CultureInfo("en-US"));
DateTime nextBirthday = birthday.AddYears(today.Year - birthday.Year);
int fullYears = today.Year - birthday.Year;
if (nextBirthday < today)
    nextBirthday = nextBirthday.AddYears(1);
else
    fullYears--;
TimeSpan timeUntilBirthday = nextBirthday - today;
int secondsUntilBirthday = (int)timeUntilBirthday.TotalSeconds;

if (nextBirthday < today)
    nextBirthday = nextBirthday.AddYears(1);

Console.WriteLine("\nИмя: {0}\nФамилия: {1}\nРодился: {2}\nКоличество полных лет: {3}", name, surname, birthdayString, fullYears);
for (int i = secondsUntilBirthday; i >= 0; i--)
{
    Console.WriteLine("Следующий день рождения через: {0} days, {1} hours, {2} minutes, {3} seconds", timeUntilBirthday.Days, timeUntilBirthday.Hours, timeUntilBirthday.Minutes, timeUntilBirthday.Seconds);
    timeUntilBirthday = nextBirthday - DateTime.Now;
    Thread.Sleep(1000);
    int currentLineCursor = Console.CursorTop; 
    Console.SetCursorPosition(0, Console.CursorTop - 1); 
    Console.Write(new string(' ', Console.WindowWidth)); 
    Console.SetCursorPosition(0, --currentLineCursor); 
}
Console.WriteLine("Поздравляем, вам исполнилось {0} + 1", fullYears);
Console.ReadLine();


