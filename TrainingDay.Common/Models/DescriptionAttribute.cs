namespace TrainingDay.Common.Models;

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