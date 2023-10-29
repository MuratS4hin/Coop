namespace Coop.Models;

public class ChartData
{
    public List<string> Labels { get; set; }
    public List<int> Values { get; set; }
    public int MaxPopulation { get; set; }
    public int TotalMaleNumber { get; set; }
    public int MaxMaleAge { get; set; }
    public int TotalFemaleNumber { get; set; }
    public int MaxFemaleAge { get; set; }

    public ChartData()
    {
        Labels = new List<string> {};
        Values = new List<int> {};
    }

}
