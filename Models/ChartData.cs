namespace Coop.Models;

public class ChartData
{
    public List<string> Labels { get; set; }
    public List<int> Values { get; set; }

    public ChartData()
    {
        Labels = new List<string> {};
        Values = new List<int> {};
    }

}
