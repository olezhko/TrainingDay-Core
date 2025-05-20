using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Reflection;

namespace TrainingDay.Common.Extensions
{
	public static class ResourceExtension
	{
        public static async Task<ObservableCollection<T>> LoadResource<T>(string category, string ci)
        {
            try
            {
                string filename = $"TrainingDay.Common.Resources.{category}_{ci}.json";
                var assembly = typeof(ExerciseExtensions).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(filename) ?? assembly.GetManifestResourceStream(@"TrainingDay.Common.Resources.{category}_en.json");

                if (stream == null)
                {
                    return new ObservableCollection<T>();
                }

                StreamReader reader = new StreamReader(stream);
                var data = await reader.ReadToEndAsync();
                var collection = JsonConvert.DeserializeObject<IEnumerable<T>>(data);

                return new ObservableCollection<T>(collection);
            }
            catch (Exception e)
            {
                return new ObservableCollection<T>();
            }
        }
    }
}

