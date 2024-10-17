namespace PrescripshunLib.Util.Faker;

internal class MedicationFaker
{
    private static readonly (string, string)[] Medication = new[]
    {
        ("Paracetamol",
            "Paracetamol wordt gebruikt voor het verlichten van pijn en het verlagen van koorts. Het is effectief bij hoofdpijn, spierpijn en koorts veroorzaakt door infecties."),
        ("Ibuprofen",
            "Ibuprofen is een ontstekingsremmer die wordt gebruikt voor pijn, zwelling en koorts. Het kan helpen bij artritis, menstruatiepijn en tandpijn."),
        ("Amoxicilline",
            "Amoxicilline is een antibioticum dat wordt voorgeschreven voor bacteriële infecties zoals longontsteking, oorontstekingen en huidinfecties."),
        ("Omeprazol",
            "Omeprazol vermindert de hoeveelheid zuur die door de maag wordt geproduceerd. Het wordt gebruikt bij brandend maagzuur, zure reflux en maagzweren."),
        ("Metformine",
            "Metformine wordt gebruikt voor de behandeling van type 2 diabetes. Het helpt de bloedsuikerspiegel te verlagen door de insulinegevoeligheid te verhogen."),
        ("Simvastatine",
            "Simvastatine is een cholesterolverlagend medicijn dat wordt voorgeschreven om het risico op hart- en vaatziekten te verminderen. Het verlaagt LDL-cholesterol en triglyceriden."),
        ("Salbutamol",
            "Salbutamol is een inhalatiemedicijn dat wordt gebruikt om kortademigheid en piepende ademhaling te verlichten bij astma en COPD."),
        ("Diclofenac",
            "Diclofenac is een ontstekingsremmende pijnstiller die wordt gebruikt bij gewrichts- en spierpijn. Het wordt vaak voorgeschreven bij reuma en artrose."),
        ("Hydrocortison",
            "Hydrocortison is een corticosteroïde die wordt gebruikt om ontstekingen en jeuk te verminderen. Het wordt vaak toegepast bij huidaandoeningen zoals eczeem."),
        ("Lorazepam",
            "Lorazepam is een kalmeringsmiddel dat wordt gebruikt bij angststoornissen en slaapproblemen. Het kan ook worden gebruikt om epileptische aanvallen te voorkomen."),
        ("Aspirine",
            "Aspirine wordt gebruikt voor pijnverlichting en ontstekingsremming. Het kan ook dagelijks worden ingenomen om hartaanvallen en beroertes te voorkomen."),
        ("Clopidogrel",
            "Clopidogrel is een bloedverdunner die wordt gebruikt om bloedstolsels te voorkomen bij mensen met een hoog risico op hartaanvallen en beroertes."),
        ("Citalopram",
            "Citalopram is een antidepressivum dat wordt gebruikt voor de behandeling van depressie en angststoornissen. Het verhoogt de hoeveelheid serotonine in de hersenen."),
        ("Lisinopril",
            "Lisinopril is een ACE-remmer die wordt voorgeschreven om hoge bloeddruk te verlagen en hartfalen te behandelen."),
        ("Warfarine",
            "Warfarine is een anticoagulans dat helpt bloedstolsels te voorkomen. Het wordt vaak voorgeschreven na operaties of bij aandoeningen zoals diepveneuze trombose."),
        ("Diazepam",
            "Diazepam wordt gebruikt om angst, spierkrampen en epileptische aanvallen te behandelen. Het werkt kalmerend op het zenuwstelsel."),
        ("Levothyroxine",
            "Levothyroxine is een synthetische vorm van het schildklierhormoon en wordt gebruikt voor de behandeling van hypothyreoïdie (trage schildklier)."),
        ("Zolpidem",
            "Zolpidem is een slaapmiddel dat wordt gebruikt voor de behandeling van slapeloosheid. Het helpt bij het sneller in slaap vallen."),
        ("Prednisolon",
            "Prednisolon is een corticosteroïde die ontstekingen in het lichaam onderdrukt. Het wordt gebruikt bij allergieën, reuma en astma."),
        ("Ciprofloxacine",
            "Ciprofloxacine is een breed-spectrum antibioticum dat wordt gebruikt voor de behandeling van verschillende bacteriële infecties, zoals blaasontstekingen en huidinfecties.")
    };


    public static (string, string) GetRandomMedication(Random random)
    {
        return Medication[random.Next(0, Medication.Length - 1)];
    }
}