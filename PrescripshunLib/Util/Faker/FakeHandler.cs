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

    public List<User> GetDoctors(int amount = 3)
    {
        var doctorsList = new List<User>();
        for (var i = 0; i < amount; i++)
        {
            var fullName = _faker.Name.FullName();
            doctorsList.Add(new User()
            {
                UserKey = Guid.NewGuid(),
                UserName = _faker.Internet.UserName(fullName.Split(' ', 2)[0], fullName.Split(' ', 2)[1]),
                Password = _faker.Internet.Password(memorable: true),
                Profile = new Profile()
                {
                    BirthDate = _faker.Date.Past(30, DateTime.Now.AddYears(-10)),
                    FullName = fullName,
                    ProfilePicture = new ProfilePicture(_faker.Image.PlaceholderUrl(360, 360)),
                }
            });
        }

        return doctorsList;
    }

    public List<User> GetPatients(ref List<User> doctorsList)
    {
        var patientsList = new List<User>();
        for (var i = 0; i < doctorsList.Count * 10; i++)
        {
            var fullName = _faker.Name.FullName();
            patientsList.Add(new User()
            {
                UserKey = Guid.NewGuid(),
                UserName = _faker.Internet.UserName(fullName.Split(' ', 2)[0], fullName.Split(' ', 2)[1]),
                Password = _faker.Internet.Password(memorable: true),
                DoctоrGuid = doctorsList[i % doctorsList.Count].UserKey,
                Profile = new Profile()
                {
                    BirthDate = _faker.Date.Past(30, new DateTime(2023, 12, 31)),
                    FullName = fullName,
                    ProfilePicture = new ProfilePicture(_faker.Image.PlaceholderUrl(360, 360)),
                }
            });
        }

        return patientsList;
    }

    public List<MedicalFile> GetMedicalFiles(ref List<User> patientsList)
    {
        var medicalFileList = new List<MedicalFile>();
        foreach (var patient in patientsList)
        {
            // Create fake appointments.
            var appointmentsList = new List<Appointment>();
            _random.Next(1, 5).DoFor(() => appointmentsList.Add(GetAppointment(patient)));

            // Create fake notes.
            var notesList = new List<Note>();
            _random.Next(1, 5).DoFor(() => notesList.Add(GetNote(patient)));

            // Create fake medication.
            var medicationList = new List<Medication>();
            _random.Next(1, 5).DoFor(() => medicationList.Add(GetMedication(patient)));

            // Create fake diagnoses.
            var diagnosisList = new List<Diagnosis>();
            _random.Next(1, 5).DoFor(() => diagnosisList.Add(GetDiagnosis(patient)));

            var medicalFile = new MedicalFile()
            {
                Patient = patient.UserKey,
                Appointments = appointmentsList,
                Notes = notesList,
                Medication = medicationList,
                Diagnoses = diagnosisList,
            };
            medicalFileList.Add(medicalFile);
        }

        return medicalFileList;
    }

    private Appointment GetAppointment(User patient)
    {
        return new Appointment()
        {
            Title = $"Beautiful appointment", // TODO: Fake.
            Description = "Basic description", // TODO: Fake.
            DoctorToMeet = patient.DoctоrGuid ?? Guid.Empty,
            DateTime = _faker.Date.Between(patient.Profile.BirthDate, new DateTime(2023, 12, 31))
        };
    }

    private Note GetNote(User patient)
    {
        return new Note()
        {
            Title = $"Extraordinary note", // TODO: Fake.
            Description = "Basic description", // TODO: Fake.
            DateTime = _faker.Date.Between(patient.Profile.BirthDate, new DateTime(2023, 12, 31))
        };
    }

    private Medication GetMedication(User patient)
    {
        var medication = new Medication()
        {
            Title = $"Amazingly complicated medication name", // TODO: Fake.
            Description = "Basic description", // TODO: Fake.
            StartedUsingOn = _faker.Date.Between(patient.Profile.BirthDate, new DateTime(2023, 12, 31))
        };

        // Sometimes people are still using the meds, so we do not always add them.
        if (_random.NextBool())
        {
            medication.StoppedUsingOn =
                _faker.Date.Between(medication.StartedUsingOn, new DateTime(2023, 12, 31));
        }

        return medication;
    }

    private Diagnosis GetDiagnosis(User patient)
    {
        return new Diagnosis()
        {
            Title = $"Very long disease or disorder name", // TODO: Fake.
            Description = "Basic description", // TODO: Fake.
            DateTime = _faker.Date.Between(patient.Profile.BirthDate, new DateTime(2023, 12, 31))
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

            var chatDate = _faker.Date.Between(DateTime.Now.AddYears(-1), DateTime.Now);
            var chat = new Chat()
            {
                User1 = patient.DoctоrGuid ?? Guid.Empty,
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

            chatMessagesList.Add(chat);
        }

        return chatMessagesList;
    }
}