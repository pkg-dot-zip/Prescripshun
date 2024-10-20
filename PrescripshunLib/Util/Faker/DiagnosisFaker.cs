using PrescripshunLib.ExtensionMethods;

namespace PrescripshunLib.Util.Faker;

/// <summary>
/// Contains an array of disease names to grab data from for <seealso cref="FakeHandler"/> when generating diagnoses.
/// </summary>
internal static class DiagnosisFaker
{
    private static readonly string[] Diseases = new[]
    {
        "Diabetes Type I",
        "Diabetes Type II",
        "Astma",
        "Longontsteking",
        "Dengue", // Dengue fever is a mosquito-borne disease caused by dengue virus, prevalent in tropical and subtropical areas.
        "Tuberculose", // Tuberculosis (TB), also known colloquially as the "white death", or historically as consumption, is an infectious disease usually caused by Mycobacterium tuberculosis (MTB) bacteria. Tuberculosis generally affects the lungs, but it can also affect other parts of the body.
        "COVID-19",
        "Malaria",
        "Ziekte van Crohn", // Crohn's disease is a type of inflammatory bowel disease (IBD) that may affect any segment of the gastrointestinal tract.
        "Multiple sclerose", // Multiple sclerosis (MS) is an autoimmune disease in which the insulating covers of nerve cells in the brain and spinal cord are damaged.
        "HIV",
        "AIDS",
        "Reuma",
        "Cholera",
        "Hepatitis",
        "Bof",
        "Mazelen",
        "Kinkhoest"
    };

    /// <summary>
    /// Grabs a random disease from the <see cref="Diseases"/> and returns its title and a description.
    /// </summary>
    /// <param name="random"></param>
    /// <returns>A pair of a disease title and description.</returns>
    public static (string, string) GetDiagnosisForDisease(Random random)
    {
        var disease = Diseases[random.Next(0, Diseases.Length - 1)];

        // Either return desc 1 or 2; only implemented two descriptions.
        var description = random.NextBool()
            ? $"Na uitgebreid onderzoeken hebben wij bij de patient {disease} geconstateerd."
            : $"Vandaag hebben wij {disease} aangetroffen bij de patient.";

        return (disease, description);
    }
}