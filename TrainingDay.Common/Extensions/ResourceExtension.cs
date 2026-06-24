using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;

namespace TrainingDay.Common.Extensions
{
	public static class ResourceExtension
	{
        public static async Task<ObservableCollection<T>> LoadResourceAsync<T>(string category, string ci)
        {
            try
            {
                string filename = $"TrainingDay.Common.Resources.{category}_{ci}.json";
                var assembly = typeof(ExerciseExtensions).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(filename) ?? assembly.GetManifestResourceStream(@"TrainingDay.Common.Resources.{category}_en.json");

                if (stream == null)
                {
                    return [];
                }

                using var reader = new StreamReader(stream);
                var data = await reader.ReadToEndAsync();
                var collection = JsonSerializer.Deserialize<IEnumerable<T>>(data);

                return new ObservableCollection<T>(collection);
            }
            catch
            {
                return [];
            }
        }
    }
}

