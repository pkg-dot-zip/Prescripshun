namespace PrescripshunLib.ExtensionMethods
{
    public static class RandomExtensions
    {
        public static bool NextBool(this Random random, double chance = 0.5) => random.NextDouble() >= chance;
    }
}
