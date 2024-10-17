using System.Reflection;
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
            Password = _faker.Internet.Password(memorable: true, length: passwordLength),
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

        var (title, desc) = DiagnosisFaker.GetDiagnosisForDisease(_random);
        return new Diagnosis()
        {
            Title = title,
            Description = desc,
            DateTime = _faker.Date.Between(patient.Profile.BirthDate, RefDateTime)
        };
    }

    public List<Chat> GetChats(ref List<User> patientsList, double defaultPatientDoctorChatChance = 0.1,
        double otherChatChance = 0.6)
    {
        // Create fake chat messages between doctors and patients.
        var chatMessagesList = new List<Chat>();
        foreach (var patient in patientsList)
        {
            if (patient.IsDoctor) throw new InvalidOperationException();
            if (_random.NextBool(defaultPatientDoctorChatChance)) continue;
            chatMessagesList.Add(GetDefaultChatForPatient(patient));
        }

        foreach (var patient in patientsList)
        {
            if (patient.IsDoctor) throw new InvalidOperationException();

            // If any chat message was already added for this user, we do not create a new one.
            if (chatMessagesList.Select(ch => ch.User1 == patient.UserKey || ch.User2 == patient.UserKey)
                .Any(b => b)) continue;

            if (_random.NextBool(otherChatChance)) continue;
            chatMessagesList.Add(GetLoremIpsumChatForPatient(patient));
        }

        return chatMessagesList;
    }

    /// <summary>
    /// Creates a chat history between a <paramref name="patient"/> and a doctor containing lorem ipsum nonsense.
    /// </summary>
    /// <param name="patient"></param>
    /// <param name="messageAmount">Amount of messages for this chat to contain</param>
    /// <param name="sentenceAmountMin">Minimum amount of sentences for each message.</param>
    /// <param name="sentenceAmountMax">Maximum amount of sentences for each message.</param>
    /// <param name="minMinuteDifference">Minimum amount of minutes that have to be passed for a new message.</param>
    /// <param name="maxMinuteDifference">Maximum amount of minutes that have to be passed for a new message.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private Chat GetLoremIpsumChatForPatient(User patient, int messageAmount = 10, int sentenceAmountMin = 1,
        int sentenceAmountMax = 3, int minMinuteDifference = 1,
        int maxMinuteDifference = 5)
    {
        if (patient.IsDoctor) throw new InvalidOperationException();

        var chatDate = _faker.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now);
        Chat chat = new Chat()
        {
            User1 = patient.DoctоrGuid ?? throw new InvalidOperationException(),
            User2 = patient.UserKey,
        };

        var patientIsSender = true;
        for (int i = 0; i < messageAmount; i++)
        {
            patientIsSender.Flip();
            chatDate = chatDate.AddMinutes(_random.Next(minMinuteDifference, maxMinuteDifference));
            chat.Messages.Add(new ChatMessage()
            {
                Sender =
                    patientIsSender ? patient.UserKey : patient.DoctоrGuid ?? throw new InvalidOperationException(),
                Recipient = !patientIsSender
                    ? patient.UserKey
                    : patient.DoctоrGuid ?? throw new InvalidOperationException(),
                Text = _faker.Lorem.Sentences(_random.Next(sentenceAmountMin, sentenceAmountMax)),
                Time = chatDate
            });
        }

        return chat;
    }

    /// <summary>
    /// Creates a default hardcoded chat history in Dutch between a <paramref name="patient"/> and doctor.
    /// </summary>
    /// <param name="patient"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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
                    Sender = patient.DoctоrGuid ?? throw new InvalidOperationException(),
                    Recipient = patient.UserKey,
                    Text = $"Hey {patient.Profile.FullName}, kom jij even langs?",
                    Time = chatDate
                },
                new ChatMessage()
                {
                    Recipient = patient.DoctоrGuid ?? throw new InvalidOperationException(),
                    Sender = patient.UserKey,
                    Text = "Maar natuurlijk dokter! :)",
                    Time = chatDate.AddMinutes(30)
                },
                new ChatMessage()
                {
                    Recipient = patient.DoctоrGuid ?? throw new InvalidOperationException(),
                    Sender = patient.UserKey,
                    Text = $"Tot zo, {patient.Profile.FullName.Split(" ")[0]}!",
                    Time = chatDate.AddMinutes(37)
                },
            ]
        };
    }
}