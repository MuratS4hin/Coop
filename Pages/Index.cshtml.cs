using Coop.Interfaces;
using Coop.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coop.Pages;

public class IndexModel : PageModel
{
    public List<Animals> animals { get; set; }
    private int Loop { get; set; }
    private ISimulation _simulation { get; set; }
    public ChartData ChartData { get; set; }

    public IndexModel(ISimulation simulation)
    {
        Loop = 60;
        _simulation = simulation;
        animals = new List<Animals>
        {
            new Animals { Id = 1, Species = "Rabbit", Sex = "Male", Age = 1 , IsPragnent = false },
            new Animals { Id = 2, Species = "Rabbit", Sex = "Female", Age = 1, IsPragnent = false },
        };
        ChartData = new ChartData();
    }

    public void OnGet()
    {
        int counter = 0;

        while (true)
        {
            IEnumerable<IGrouping<string, Animals>> groupedAnimals = animals.GroupBy(animal => animal.Species);
            animals = _simulation.Loop(animals, groupedAnimals);
            counter++;
            ChartData.Labels.Add(counter.ToString());
            ChartData.Values.Add(animals.Count());
            if (counter == Loop) break;
        }
    }
}