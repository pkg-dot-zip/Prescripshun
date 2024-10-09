namespace PrescripshunLib.ExtensionMethods;

public static class RandomExtensions
{
    public static bool NextBool(this Random random, double invertedChance = 0.5) => random.NextDouble() >= invertedChance;
}