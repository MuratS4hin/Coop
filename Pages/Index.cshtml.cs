using Coop.Interfaces;
using Coop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coop.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Loop { get; set; }
    public List<Animals> Animals { get; set; }
    private ISimulation _simulation { get; set; }
    public ChartData ChartData { get; set; }
    public bool ShowForm { get; set; }

    public IndexModel(ISimulation simulation)
    {
        Animals = new List<Animals>
        {
            new Animals { Id = 1, Species = "Rabbit", Sex = "Male", Age = 1 , IsPragnent = false },
            new Animals { Id = 2, Species = "Rabbit", Sex = "Female", Age = 1, IsPragnent = false },
        };
        _simulation = simulation;
        ShowForm = true;

        #region Set ChartData
        int totalAnimalCount = Animals.Count();
        int maleAge = (int)Animals.FindAll(x => x.Sex == "Male")?.MaxBy(t => t.Age).Age;
        int femaleAge = (int)(Animals.FindAll(x => x.Sex == "Female")?.MaxBy(t => t.Age).Age);
        ChartData = new ChartData();
        ChartData.MaxPopulation = ChartData.MaxPopulation > totalAnimalCount ? ChartData.MaxPopulation : totalAnimalCount;
        ChartData.MaxMaleAge = ChartData.MaxMaleAge > maleAge ? ChartData.MaxMaleAge : maleAge;
        ChartData.MaxFemaleAge = ChartData.MaxFemaleAge > femaleAge ? ChartData.MaxFemaleAge : femaleAge;
        ChartData.TotalMaleNumber = Animals.FindAll(x => x.Sex == "Male").Count();
        ChartData.TotalFemaleNumber = Animals.FindAll(x => x.Sex == "Female").Count();
        #endregion        
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
        ChartData.Values.Add(Animals.Count());

        while (true)
        {
            IEnumerable<IGrouping<string, Animals>> groupedAnimals = Animals.GroupBy(animal => animal.Species);
            Animals = _simulation.Loop(Animals, groupedAnimals);
            counter++;

            #region Set ChartData
            int totalAnimalCount = Animals.Count();
            int maleAge = Animals?.FindAll(x => x.Sex == "Male").MaxBy(t => t.Age)?.Age ?? ChartData.MaxMaleAge;
            int femaleAge = Animals?.FindAll(x => x.Sex == "Female").MaxBy(t => t.Age)?.Age ?? ChartData.MaxFemaleAge;
            ChartData.Labels.Add(counter.ToString());
            ChartData.Values.Add(totalAnimalCount);
            ChartData.MaxPopulation = ChartData.MaxPopulation > totalAnimalCount ? ChartData.MaxPopulation : totalAnimalCount;
            ChartData.MaxMaleAge = ChartData.MaxMaleAge > maleAge ? ChartData.MaxMaleAge : maleAge;
            ChartData.MaxFemaleAge = ChartData.MaxFemaleAge > femaleAge ? ChartData.MaxFemaleAge : femaleAge;
            #endregion

            if (counter == Int32.Parse(Loop))
                break;
        }

        #region Set ChartData
        ChartData.TotalMaleNumber = Animals.FindAll(x => x.Sex == "Male").Count();
        ChartData.TotalFemaleNumber = Animals.FindAll(x => x.Sex == "Female").Count();
        #endregion
    }
}