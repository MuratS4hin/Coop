using Coop.Interfaces;
using Coop.Models;

namespace Coop.Services;

public class Simulation : ISimulation
{
    private static readonly Random random = new Random();
    private long LastId { get; set; }
    private int OptimalCageCapasity { get; set; }

    public Simulation() { }

    public List<Animals> Loop(List<Animals> animals, IEnumerable<IGrouping<string, Animals>> groupedAnimals, int optimalCageCapasity)
    {
        LastId = animals.Count > 0 ? (int)animals?.MaxBy(x => x.Id)?.Id : 0;
        OptimalCageCapasity = optimalCageCapasity;
        foreach (IGrouping<string, Animals> group in groupedAnimals)
        {
            foreach (Animals animal in group)
            {
                if (animal.Age >= 1 && animal.Sex == "Male")
                {
                    bool fertilityProbability = GenerateTrueWithProbability(0.7);
                    var female = animals.FirstOrDefault(x => x.Sex == "Female" && x.IsPregnant == false && fertilityProbability == true);
                    if (female != null)
                        animals.First(x => x.Id == female.Id).IsPregnant = true;
                }
                else if (animal.Sex == "Female" && CheckBirth(animal))
                {
                    animals.First(x => x.Id == animal.Id).IsPregnant = false;
                    int numberOfChild = NumberOfChild();
                    for (int i = 0; i < numberOfChild; i++)
                    {
                        LastId += 1;
                        bool isFemale = GenerateTrueWithProbability(0.7);
                        string Sex = "Male";
                        if (isFemale)
                            Sex = "Female";
                        animals.Add(new Animals() { Id = LastId, Species = animal.Species, Sex = Sex, Age = 0, IsPregnant = false, AverageDeathAge = animal.AverageDeathAge });
                    }
                }
                animals.First(x => x.Id == animal.Id).Age += 1;

                if (CheckDeath(animal, animals.Count))
                    animals.RemoveAll(x => x.Id == animal.Id);
            }
        }

        return animals;
    }

    public bool GenerateTrueWithProbability(double probability)
    {
        if (probability < 0.0 || probability > 1.0)
        {
            throw new ArgumentException("Probability must be between 0 and 1.");
        }

        double randomNumber = random.NextDouble();
        return randomNumber < probability;
    }

    public bool CheckBirth(Animals animal)
    {
        if (animal.IsPregnant == true)
            return true;
        else
            return false;
    }

    public bool CheckDeath(Animals animal, int numberOfAnimals)
    {
        if (animal.Age > animal.AverageDeathAge || numberOfAnimals > OptimalCageCapasity)
        {
            double probability = Math.Abs(animal.AverageDeathAge - animal.Age) * 0.3;
            if (numberOfAnimals > OptimalCageCapasity)
                probability = probability * numberOfAnimals / OptimalCageCapasity;
            if (probability > 1)
                probability = 1;
            if (GenerateTrueWithProbability(probability))
                return true;
        }
        else if (animal.Age < animal.AverageDeathAge && numberOfAnimals > 2)
        {
            double probability = 0.3;
            if (GenerateTrueWithProbability(probability))
                return true;
        }
        return false;
    }

    public int NumberOfChild()
    {
        double mean = 5.5; // The mean value between 1 and 10
        double stdDev = 2.0; // Adjust the standard deviation as needed
        double u1 = 1.0 - random.NextDouble(); // Uniform(0,1] random doubles
        double u2 = 1.0 - random.NextDouble();
        double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); // Box-Muller transform
        double randNormal = mean + stdDev * randStdNormal; // Adjust for desired mean and standard deviation
        randNormal = Math.Max(1, Math.Min(10, randNormal)); // Clip the values to be within the range [1, 10]

        return (int)Math.Round(randNormal);

    }
}
