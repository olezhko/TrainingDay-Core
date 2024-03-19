using System.Collections;
using System.Globalization;
using System.Reflection;
using TrainingDay.Common;

namespace TrainingDay.Web.Server.Extensions;

public static class ExerciseExtension
{
    public static List<MusclesEnum> ConvertMusclesToList(string value) // separator
    {
        List<MusclesEnum> muscle = new List<MusclesEnum>();

        if (!string.IsNullOrEmpty(value))
        {
            string[] enums = value.Split(',');
            try
            {
                for (int i = 0; i < enums.Length; i++)
                {
                    var res = Enum.Parse(typeof(MusclesEnum), enums[i]);
                    muscle.Add((MusclesEnum)res);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return muscle;
    }

    public static int ConvertTagsFromStringToInt(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return 0;
        }

        var tagsString = value.Split(',');
        List<ExerciseTags> enumLists = new List<ExerciseTags>();
        foreach (var tagStr in tagsString)
        {
            ExerciseTags res = (ExerciseTags)Enum.Parse(typeof(ExerciseTags), tagStr);
            enumLists.Add(res);
        }

        return ConvertTagsListToInt(enumLists);
    }

    public static int ConvertTagsListToInt(List<ExerciseTags> tagsList)
    {
        if (tagsList.Count == 0)
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

    public static List<ExerciseTags> ConvertTagsFromIntToList(int value)
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

    public static string GetEnumDescription(object value)
    {
        try
        {
            var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var c2 = enumMember.GetCustomAttributes().First();
            var attr = c2 as DescriptionAttribute;
            var ci = CultureInfo.CurrentUICulture;

            string res = ci.Name == "ru" || ci.Name == "ru-RU" ?
                attr.InfoRu :
                attr.InfoEn;
            return res;
        }
        catch (Exception e)
        {
            return value.ToString();
        }
    }
}