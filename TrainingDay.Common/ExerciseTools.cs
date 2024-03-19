using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace TrainingDay.Common
{
    public struct Description
    {
        public string StartPosition { get; set; }
        public string Execution { get; set; }
        public string Advice { get; set; }
    }

    public enum ExerciseTags
    {
        [Description("Без Инвентаря", "No inventory", "No inventory")] CanDoAtHome,
        [Description("На Время", "By Time", "By Time")] ExerciseByTime,
        [Description("На Расстояние", "By Distance", "By Distance")] ExerciseByDistance,
        [Description("Подходы/Вес", "Repetitions And Weight", "Repetitions And Weight")] ExerciseByRepsAndWeight,
        [Description("Штанга", "Barbell", "Barbell")] BarbellExist,
        [Description("Гантеля", "Dumbbell", "Dumbbell")] DumbbellExist,
        [Description("Скамья", "Bench", "Bench")] BenchExist,
        [Description("Базовое", "Default", "Default")] DatabaseExercise,
        [Description("На Повторения", "Repetitions", "Wiederholungen")] ExerciseByReps,
        Last,
    }

    public enum MusclesEnum
    {
        [Description("Шея", "Neck", "Neck")] Neck,
        [Description("Трапеции", "Trapezium", "Trapezmuskel")] Trapezium,
        [Description("Передняя дельта", "Front Delta", "Vorderer Delta")] ShouldersFront,
        [Description("Задняя дельта", "Back Delta", "Hinterer Delta")] ShouldersBack,
        [Description("Средняя дельта", "Middle Delta", "Seitlicher Delta")] ShouldersMiddle,
        [Description("Широчайшие", "Widest", "Latissimus")] WidestBack,
        [Description("Спина", "Back", "Rücken")] MiddleBack,
        [Description("Мышцы позвоночника", "Spinal", "Spinal")] ErectorSpinae,
        [Description("Грудь", "Chest", "Core")] Chest,
        [Description("Пресс", "Abdominal", "Bauch")] Abdominal,
        [Description("Трицепс", "Triceps", "Trizeps")] Triceps,
        [Description("Бицепс", "Biceps", "Bizeps")] Biceps,
        [Description("Предплечье", "Forearm", "Oberarme")] Forearm,
        [Description("Квадрицепс", "Quadriceps", "Quadriceps")] Quadriceps,
        [Description("Икры", "Caviar", "Caviar")] Caviar,
        [Description("Камболовидная", "Camboloid", "Camboloid")] ShinCamboloid,
        [Description("Передняя голень", "Anterior tibialis", "Anterior tibialis")] ShinAnteriorTibialis,
        [Description("Бедра", "Thighs", "Oberschenkel")] Thighs,
        [Description("Ягодицы", "Glute", "Gesäß")] Buttocks,
        [Description("Кардио", "Cardio", "Cardio")] Cardio,
        [Description("Поясница", "Lower Back", "Kreuz")] LowerBack,
        [Description("Выберите", "Select", "Auswählen")] None,
    }

    public class DescriptionAttribute : Attribute
    {
        public string InfoRu;
        public string InfoEn;
        public string InfoDe;

        public DescriptionAttribute(string infoRu, string infoEn, string infoDe)
        {
            InfoEn = infoEn;
            InfoRu = infoRu;
            InfoDe = infoDe;
        }
    }
    public static class ExerciseTools
    {
        public static string GetEnumDescription(object value, CultureInfo ci)
        {
            try
            {
                var enumMember = value.GetType().GetMember(value.ToString()).FirstOrDefault();
                var c2 = enumMember.GetCustomAttributes().First();
                var attr = c2 as DescriptionAttribute;

                string attribute = string.Empty;
                switch (ci.Name)
                {
                    case "ru":
                    case "ru-RU":
                        attribute = attr.InfoRu;
                        break;
                    case "de":
                    case "de-DE":
                        attribute = attr.InfoDe;
                        break;
                    default:
                        attribute = attr.InfoEn;
                        break;
                }
                return attribute;
            }
            catch (Exception e)
            {
                return value.ToString();
            }
        }
        public static ObservableCollection<BaseExercise> InitExercises(string ci)
        {
            try
            {
                string filename = $"TrainingDay.Common.Resources.exercises_{ci}.json";
                var assembly = typeof(ExerciseTools).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream(filename) ?? assembly.GetManifestResourceStream(@"TrainingDay.Common.Resources.exercises_en.json");

                if (stream == null)
                {
                    return new ObservableCollection<BaseExercise>();
                }

                StreamReader reader = new StreamReader(stream);
                var data = reader.ReadToEnd();
                var exercises = JsonConvert.DeserializeObject<IEnumerable<BaseExercise>>(data);

                return new ObservableCollection<BaseExercise>(exercises);
            }
            catch (Exception e)
            {
                return new ObservableCollection<BaseExercise>();
            }
        }

        /// <summary>
        /// Convert from base to value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertFromTagStringToInt(string value)
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

            return ConvertTagListToInt(enumLists);
        }

        /// <summary>
        /// Convert from value to model
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static List<ExerciseTags> ConvertFromIntToTagList(int value)
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

        public static int ConvertTagListToInt(List<ExerciseTags> tagsList)
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
    }
}
