namespace TrainingDay.Common
{
    public class SelectItem<T>
    {
        public string Name { get; set; }

        public SelectItem(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public T Value { get; set; }
    }
}
