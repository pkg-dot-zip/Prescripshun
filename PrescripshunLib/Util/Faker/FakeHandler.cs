using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User.Profile;
using PrescripshunLib.Models.User;
using Bogus;
using PhilosopherPanda.ExtensionMethods.DataTypes;

namespace PrescripshunLib.Util.Faker;

/// <summary>
/// Creates 'fake' data we use to fill our database with test data since we do not have access to actual data used by
/// general practitioners.
/// </summary>
/// <param name="seed">Seed to use for our <seealso cref="Bogus.Faker.Random"/> and our own <seealso cref="Random"/> instance.</param>
/// <param name="locale">Language setting to use. Right now the default is 'nl', which is Dutch. Note that Flemish is also supported but not used.</param>
public class FakeHandler(int seed = 0, string locale = "nl") // Note: Flemish locale has boring names. Use 'nl'. :(
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly Bogus.Faker _faker = new(locale)
    {
        Random = new Randomizer(seed)
    };

    private readonly Random _random = new(seed);

    // Used as base date for random dates instead of today.
    private static readonly DateTime RefDateTime = new DateTime(2023, 12, 31);

    /// <summary>
    /// Creates new user. If a <paramref name="doctorKey"/> is specified the user is a <b>patient</b>. If not, it is a <b>doctor</b>.
    /// </summary>
    /// <param name="doctorKey">Guid of the patients doctor. If not specified, the user itself is a doctor.</param>
    /// <param name="passwordLength">Length of the password in characters.</param>
    /// <returns></returns>
    private User GetUser(Guid? doctorKey = null, int passwordLength = 10)
    {
        var fullName = _faker.Name.FullName();

        return new User()
        {
            UserKey = Guid.NewGuid(),
            UserName = _faker.Internet.UserName(fullName.Split(' ', 2)[0], fullName.Split(' ', 2)[1]),
            Password = _faker.Internet.Password(memorable: true, length:passwordLength),
            DoctоrGuid = doctorKey,
            Profile = GetProfile(fullName)
        };
    }

    public List<User> GetDoctors(int amount = 3)
    {
        var doctorsList = new List<User>();
        amount.DoFor(() => doctorsList.Add(GetUser()));
        return doctorsList;
    }

    public List<User> GetPatients(ref List<User> doctorsList)
    {
        var patientsList = new List<User>();
        for (var i = 0; i < doctorsList.Count * 10; i++)
        {
            patientsList.Add(GetUser(doctorsList[i % doctorsList.Count].UserKey));
        }

        return patientsList;
    }

    private Profile GetProfile(string fullName)
    {
        return new Profile()
        {
            BirthDate = _faker.Date.Past(30, RefDateTime),
            FullName = fullName,
            ProfilePicture = new ProfilePicture(_faker.Image.PlaceholderUrl(360, 360)),
        };
    }

    public List<MedicalFile> GetMedicalFiles(ref List<User> patientsList)
    {
        var medicalFileList = new List<MedicalFile>();
        foreach (var patient in patientsList)
        {
            medicalFileList.Add(GetMedicalFile(patient));
        }

        return medicalFileList;
    }

    private MedicalFile GetMedicalFile(User patient)
    {
        if (patient.IsDoctor) throw new InvalidOperationException();

        var appointments = new List<Appointment>();
        var notes = new List<Note>();
        var medication = new List<Medication>();
        var diagnoses = new List<Diagnosis>();

        _random.Next(1, 5).DoFor(() => appointments.Add(GetAppointment(patient)));
        _random.Next(1, 5).DoFor(() => notes.Add(GetNote(patient)));
        _random.Next(1, 5).DoFor(() => medication.Add(GetMedication(patient)));
        _random.Next(1, 5).DoFor(() => diagnoses.Add(GetDiagnosis(patient)));

        return new MedicalFile()
        {
            Patient = patient.UserKey,
            Appointments = appointments,
            Notes = notes,
            Medication = medication,
            Diagnoses = diagnoses,
        };
    }

    private Appointment GetAppointment(User patient)
    {
        if (patient.IsDoctor) throw new InvalidOperationException();

        return new Appointment()
        {
            Title = $"Beautiful appointment", // TODO: Fake.
            Description = "Basic description", // TODO: Fake.
            DoctorToMeet = patient.DoctоrGuid ?? Guid.Empty,
            DateTime = _faker.Date.Between(patient.Profile.BirthDate, RefDateTime)
        };
    }

    private Note GetNote(User patient)
    {
        if (patient.IsDoctor) throw new InvalidOperationException();

        return new Note()
        {
            Title = $"Extraordinary note", // TODO: Fake.
            Description = "Basic description", // TODO: Fake.
            DateTime = _faker.Date.Between(patient.Profile.BirthDate, RefDateTime)
        };
    }

    private Medication GetMedication(User patient)
    {
        if (patient.IsDoctor) throw new InvalidOperationException();

        var medication = new Medication()
        {
            Title = $"Amazingly complicated medication name", // TODO: Fake.
            Description = "Basic description", // TODO: Fake.
            StartedUsingOn = _faker.Date.Between(patient.Profile.BirthDate, RefDateTime)
        };

        // Sometimes people are still using the meds, so we do not always add them.
        if (_random.NextBool())
        {
            medication.StoppedUsingOn =
                _faker.Date.Between(medication.StartedUsingOn, RefDateTime);
        }

        return medication;
    }

    private Diagnosis GetDiagnosis(User patient)
    {
        if (patient.IsDoctor) throw new InvalidOperationException();
        
        return new Diagnosis()
        {
            Title = $"Very long disease or disorder name", // TODO: Fake.
            Description = "Basic description", // TODO: Fake.
            DateTime = _faker.Date.Between(patient.Profile.BirthDate, RefDateTime)
        };
    }

    public List<Chat> GetChats(ref List<User> patientsList)
    {
        // Create fake chat messages between doctors and patients.
        var chatMessagesList = new List<Chat>();
        foreach (var patient in patientsList)
        {
            // Patients seem to be not very talkative. :P
            if (_random.NextBool(0.1)) continue;
            chatMessagesList.Add(GetDefaultChatForPatient(patient));
        }

        return chatMessagesList;
    }

    private Chat GetDefaultChatForPatient(User patient)
    {
        if (patient.IsDoctor) throw new InvalidOperationException();

        var chatDate = _faker.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now);
        return new Chat()
        {
            User1 = patient.DoctоrGuid ?? throw new InvalidOperationException(),
            User2 = patient.UserKey,
            Messages =
            [
                new ChatMessage()
                {
                    Sender = patient.DoctоrGuid ?? Guid.Empty,
                    Recipient = patient.UserKey,
                    Text = $"Hey {patient.Profile.FullName}, kom jij even langs?",
                    Time = chatDate
                },
                new ChatMessage()
                {
                    Recipient = patient.DoctоrGuid ?? Guid.Empty,
                    Sender = patient.UserKey,
                    Text = "Maar natuurlijk dokter! :)",
                    Time = chatDate.AddMinutes(30)
                },
                new ChatMessage()
                {
                    Recipient = patient.DoctоrGuid ?? Guid.Empty,
                    Sender = patient.UserKey,
                    Text = $"Tot zo, {patient.Profile.FullName.Split(" ")[0]}!",
                    Time = chatDate.AddMinutes(37)
                },
            ]
        };
    }
}