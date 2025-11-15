using Microsoft.Extensions.Localization;
using System.Globalization;

namespace TrainingDay.Web.Services.Extensions
{
    public class StringLocalizerService : IStringLocalizer
    {
        Dictionary<string, Dictionary<string, string>> resources;
        // ключи ресурсов
        public static string WeightMessage = "WeightMessage";
        public static string WorkoutMessage = "WorkoutMessage";
        public static string AlarmMessageFormat = "AlarmMessageFormat";
        public static string NewsString = "NewsMessage";

        public StringLocalizerService()
        {
            // словарь для английского языка
            Dictionary<string, string> enDict = new Dictionary<string, string>
            {
                {WeightMessage, "Enter your new weight. Maybe something has changed." },
                {AlarmMessageFormat, "We remind you about the workout." },
                {WorkoutMessage, "You have not implemented a workout for a long time. Maybe today is that day?" },
                {NewsString, "News" }
            };
            // словарь для русского языка
            Dictionary<string, string> ruDict = new Dictionary<string, string>
            {
                {WeightMessage, "Введите свой новый вес. Возможно что-то изменилось." },
                {AlarmMessageFormat, "Напоминаем вам о тренировке." },
                {WorkoutMessage, "Вы давно не выполняли тренировку. Может сегодня тот день?" },
                {NewsString, "Новости" }
            };
            // словарь для немецкого языка
            Dictionary<string, string> deDict = new Dictionary<string, string>
            {
                {WeightMessage, "Willkommen" },
                {AlarmMessageFormat, "Willkommen" },
                {WorkoutMessage, "Hallo Welt!" }
            };
            // создаем словарь ресурсов
            resources = new Dictionary<string, Dictionary<string, string>>
            {
                {"en", enDict },
                {"ru", ruDict },
                {"de", deDict }
            };
        }
        // по ключу выбираем для текущей культуры нужный ресурс
        public LocalizedString this[string culture, string name]
        {
            get
            {
                string val = "";
                if (resources.ContainsKey(culture))
                {
                    if (resources[culture].ContainsKey(name))
                    {
                        val = resources[culture][name];
                    }
                }
                return new LocalizedString(name, val);
            }
        }

        public LocalizedString this[string name, params object[] arguments] => this[name, arguments[0].ToString()];

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this;
        }

        public LocalizedString this[string name] => new LocalizedString("", "");
    }
}
