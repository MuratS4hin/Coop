using Coop.Interfaces;
using Coop.Models;
using Coop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Coop.Pages;

public class IndexModel : PageModel
{
    public List<Animals> Animals { get; set; }

    private int Loop { get; set; }
    private ISimulation _simulation { get; set; }
    private long LastId { get; set; }

    public IndexModel(ISimulation simulation)
    {
        Loop = 60;
        _simulation = simulation;
        LastId = 2;
        Animals = new List<Animals>()
        {
            new Animals() { Id = 1, Sex = "Male", Species = "Rabbit" },
            new Animals() { Id = 2, Sex = "Female", Species = "Rabbit" },
        };
    }

    public void OnGet()
    {
        List<Animals> animals = new List<Animals>
        {
            new Animals { Id = 1, Species = "Rabbit", Sex = "Male", Age = 5 , IsPragnent = false },
            new Animals { Id = 2, Species = "Rabbit", Sex = "Female", Age = 6, IsPragnent = false },
            //new Animals { Id = 2, Species = "Cat", Sex = "Female", Age = 2, IsPragnent = false },
            //new Animals { Id = 4, Species = "Cat", Sex = "Male", Age = 1, IsPragnent = false },
            //new Animals { Id = 5, Species = "Rabbit", Sex = "Male", Age = 5, IsPragnent = false }
        };

        int counter = 0;

        while (true)
        {
            var groupedAnimals = animals.GroupBy(animal => animal.Species);

            foreach (var group in groupedAnimals)
            {
                foreach (var animal in group)
                {
                    if (animal.Age >= 5 && animal.Age <= 20 && animal.Sex == "Male")
                    {
                        bool fertilityProbability = _simulation.GenerateTrueWithProbability(0.7);
                        var female = animals.FirstOrDefault(x => x.Sex == "Female"
                            && x.IsPragnent == false
                            && fertilityProbability == true
                            && animal.Age >= 5
                            && animal.Age <= 20
                            );

                        if (female != null)
                            animals.First(x => x.Id == female.Id).IsPragnent = true;

                    }
                    else if (animal.Sex == "Female" && _simulation.CheckBirth(animal))
                    {
                        animals.First(x => x.Id == animal.Id).IsPragnent = false;
                        bool isFemale = _simulation.GenerateTrueWithProbability(0.5);
                        string Sex = "Male";
                        LastId += 1;
                        if (isFemale)
                            Sex = "Female";

                        animals.Add(new Animals() { Id = LastId, Species = animal.Species, Sex = Sex, Age = 0, IsPragnent = false });
                    }
                    animals.First(x => x.Id == animal.Id).Age += 1;
                }
            }

            counter++;

            if (counter == Loop) break;
        }
    }
}