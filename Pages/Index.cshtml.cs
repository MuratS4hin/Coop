using Coop.Interfaces;
using Coop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic.FileIO;

namespace Coop.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public string Loop { get; set; }
    public List<Animals> Animals { get; set; }
    private ISimulation _simulation { get; set; }
    public ChartData ChartData { get; set; }
    private int OptimalCageCapasity { get; set; }
    public bool ShowForm { get; set; }

    public IndexModel(ISimulation simulation)
    {
        Animals = ReadCsv();
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
            if (Loop == "0" || Loop == null)
                Loop = "1";
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
        int counter = 0;
        ChartData.Labels.Add(counter.ToString());
        ChartData.Values.Add(Animals.Count());

        while (true)
        {
            IEnumerable<IGrouping<string, Animals>> groupedAnimals = Animals.GroupBy(animal => animal.Species);
            Animals = _simulation.Loop(Animals, groupedAnimals, OptimalCageCapasity);
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

    public List<Animals> ReadCsv()
    {
        string filePath = "configuration.txt"; // Update the file path to your CSV file

        List<Animals> AnimalsCsv = new List<Animals>();
        Console.WriteLine("Current Directory: " + Directory.GetCurrentDirectory());

        using (TextFieldParser parser = new TextFieldParser(filePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            // Skip the header row if it exists
            if (!parser.EndOfData)
            {
                parser.ReadFields();
            }

            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if(fields.Length == 2 && fields[0] == "OptimalCageCapacity")
                {
                    OptimalCageCapasity = int.Parse(fields[1]);
                }
                else if (fields.Length == 6)
                {
                    Animals animal = new Animals
                    {
                        Id = int.Parse(fields[0]),
                        Species = fields[1],
                        Sex = fields[2],
                        Age = int.Parse(fields[3]),
                        AverageDeathAge = int.Parse(fields[4]),
                        IsPregnant = bool.Parse(fields[5])
                    };
                    AnimalsCsv.Add(animal);
                }
                else
                {
                    Console.WriteLine("Invalid data format in the CSV file.");
                }
            }
        }
        return AnimalsCsv;
    }
}