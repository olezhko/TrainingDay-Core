namespace TrainingDay.Common.Models
{
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
}
