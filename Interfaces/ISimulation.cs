using Coop.Models;

namespace Coop.Interfaces;

public interface ISimulation
{
    public bool GenerateTrueWithProbability(double probability);
    public bool CheckBirth(Animals animal);
}
