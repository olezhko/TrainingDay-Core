namespace TrainingDay.Common.Models;

public sealed class DescriptionAttribute(string infoRu, string infoEn, string infoDe) : Attribute
{
    public string InfoRu = infoRu;
    public string InfoEn = infoEn;
    public string InfoDe = infoDe;
}