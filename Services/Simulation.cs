using Coop.Interfaces;
using Coop.Models;

namespace Coop.Services;

public class Simulation : ISimulation
{
    private static readonly Random random = new Random();

    public Simulation() { }

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
        if (animal.IsPragnent == true)
            return true;
        else
            return false;
    }    
}
