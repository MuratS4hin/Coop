using Coop.Interfaces;
using Coop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coop.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Loop { get; set; }
    public List<Animals> animals { get; set; }
    private ISimulation _simulation { get; set; }
    public ChartData ChartData { get; set; }
    public bool ShowForm { get; set; }

    public IndexModel(ISimulation simulation)
    {
        _simulation = simulation;
        animals = new List<Animals>
        {
            new Animals { Id = 1, Species = "Rabbit", Sex = "Male", Age = 1 , IsPragnent = false },
            new Animals { Id = 2, Species = "Rabbit", Sex = "Female", Age = 1, IsPragnent = false },
        };
        ChartData = new ChartData();
        ShowForm = true;
    }

    public IActionResult OnPost(string action)
    {        
        if(action == "LoopNumber")
        {
            TempData["LoopValue"] = Loop; //Preserves the Input Value for regenerating
            ShowForm = false;
            StartSimulation();
        }
        else if(action == "Regenerate")
        {
            //if (TempData["LoopValue"] is int loopValue)
            Loop = TempData["LoopValue"].ToString(); //Gets the Input Value
            TempData.Keep("LoopValue"); //Preserves the Input Value for regenerating
            ShowForm = false;
            StartSimulation();
        }
        else if (action == "EntDiffNum")
        {
            ShowForm = true;
        }

        return Page();
    }

    public void StartSimulation()
    {
        if (Loop == "0" || Loop == null)
            Loop = "1";
        int counter = 0;
        ChartData.Labels.Add(counter.ToString());
        ChartData.Values.Add(animals.Count());

        while (true)
        {
            IEnumerable<IGrouping<string, Animals>> groupedAnimals = animals.GroupBy(animal => animal.Species);
            animals = _simulation.Loop(animals, groupedAnimals);
            counter++;
            ChartData.Labels.Add(counter.ToString());
            ChartData.Values.Add(animals.Count());
            if (counter == Int32.Parse(Loop)) break;
        }
    }
}