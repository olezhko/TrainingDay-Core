using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;

namespace TrainingDay.Common.Extensions;

public static class ResourceExtension
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public static async Task<ObservableCollection<T>> LoadResourceAsync<T>(string category, string ci)
    {
        var assembly = typeof(ExerciseExtensions).GetTypeInfo().Assembly;
        Stream? stream = assembly.GetManifestResourceStream($"TrainingDay.Common.Resources.{category}_{ci}.json") ?? assembly.GetManifestResourceStream($"TrainingDay.Common.Resources.{category}_en.json");
        return await DeserializeAsync<T>(stream);
    }

    /// <summary>Loads a language-neutral resource <c>Resources/{name}.json</c>.</summary>
    public static async Task<ObservableCollection<T>> LoadResourceAsync<T>(string name)
    {
        var assembly = typeof(ExerciseExtensions).GetTypeInfo().Assembly;
        return await DeserializeAsync<T>(assembly.GetManifestResourceStream($"TrainingDay.Common.Resources.{name}.json"));
    }

    private static async Task<ObservableCollection<T>> DeserializeAsync<T>(Stream? stream)
    {
        try
        {
            if (stream == null)
            {
                return [];
            }

            using var reader = new StreamReader(stream);
            var data = await reader.ReadToEndAsync();
            var collection = JsonSerializer.Deserialize<IEnumerable<T>>(data, SerializerOptions);

            return new ObservableCollection<T>(collection ?? []);
        }
        catch
        {
            return [];
        }
    }
}
