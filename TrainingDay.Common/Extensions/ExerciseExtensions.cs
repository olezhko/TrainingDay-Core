using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Text;
using TrainingDay.Common.Models;

namespace TrainingDay.Common.Extensions;

public static class ExerciseExtensions
{
    public static string GetEnumDescription(object value, CultureInfo ci)
    {
        try
        {
            var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var c2 = enumMember.GetCustomAttributes().First();
            var attr = c2 as DescriptionAttribute;

            string attribute = ci.TwoLetterISOLanguageName switch
            {
                "ru" => attr.InfoRu,
                "de" => attr.InfoDe,
                _ => attr.InfoEn,
            };
            return attribute;
        }
        catch (Exception e)
        {
            return value.ToString();
        }
    }



    /// <summary>
    /// Convert from base to value
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<ExerciseTags> ConvertTagStringToList(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return [];
        }

        var tagsString = value.Split(',');
        List<ExerciseTags> enumLists = new();
        foreach (var tagStr in tagsString)
        {
            ExerciseTags res = (ExerciseTags)Enum.Parse(typeof(ExerciseTags), tagStr);
            enumLists.Add(res);
        }

        return enumLists;
    }

    /// <summary>
    /// Convert from value to model
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<ExerciseTags> ConvertTagIntToList(int value)
    {
        if (value == 0)
        {
            return new List<ExerciseTags>();
        }

        List<ExerciseTags> result = new List<ExerciseTags>();

        BitArray array = new BitArray(new[] { value });
        for (int i = 0; i < (int)ExerciseTags.Last; i++)
        {
            var flagValue = array.Get(i);
            if (flagValue)
            {
                result.Add((ExerciseTags)i);
            }
        }

        return result;
    }

    public static int ConvertTagListToInt(IEnumerable<ExerciseTags> tagsList)
    {
        if (tagsList.Count() == 0)
        {
            return 0;
        }

        BitArray array = new BitArray(32);
        int[] res = new int[1];
        foreach (var exerciseTags in tagsList)
        {
            array.Set((int)exerciseTags, true);
        }

        array.CopyTo(res, 0);
        return res[0];
    }

    public static string ConvertTagListToString(IEnumerable<ExerciseTags> tagsList)
    {
        if (!tagsList.Any())
        {
            return string.Empty;
        }

        return string.Join(",", tagsList);
    }

    public static List<MusclesEnum> ConvertMuscleStringToList(string value)
    {
        List<MusclesEnum> result = new List<MusclesEnum>();
        if (!string.IsNullOrEmpty(value))
        {
            string[] enums = value.Split(',');
            try
            {
                foreach (var type in enums)
                {
                    var res = Enum.TryParse(type, out MusclesEnum muscleValue);
                    if (res)
                    {
                        result.Add(muscleValue);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return result;
    }

    public static string ConvertFromMuscleListToString(List<MusclesEnum> array)
    {
        try
        {
            if (array == null || array.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder res = new StringBuilder();
            foreach (var muscleViewModel in array)
            {
                res.Append(muscleViewModel);
                res.Append(",");
            }

            res.Remove(res.Length - 1, 1);
            return res.ToString();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public static int ConvertTagStringToInt(string tags)
    {
        return ConvertTagListToInt(ConvertTagStringToList(tags));
    }
}