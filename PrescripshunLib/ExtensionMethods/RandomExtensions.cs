namespace PrescripshunLib.ExtensionMethods;

/// <summary>
/// Contains extension methods for <seealso cref="Random"/>.
/// </summary>
public static class RandomExtensions
{
    /// <summary>
    /// Returns a random <see cref="Boolean"/>, thus either returning <c>true</c> or <c>false</c>.
    /// </summary>
    /// <param name="random"><see cref="Random"/> instance to use.</param>
    /// <param name="invertedChance">Inverted chance of returning <c>true</c>. Meaning a value of <c>0.1</c> means it has a 90% chance of returning <c>true</c>.</param>
    /// <returns></returns>
    public static bool NextBool(this Random random, double invertedChance = 0.5) => random.NextDouble() >= invertedChance;
}