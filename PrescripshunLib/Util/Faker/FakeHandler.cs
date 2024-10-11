using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User.Profile;
using PrescripshunLib.Models.User;
using Bogus;

namespace PrescripshunLib.Util.Faker;

public class FakeHandler(int seed = 0, string locale = "nl")
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
            for (int i = 0; i < _random.Next(1, 5); i++)
            {
                var appointment = new Appointment()
                {
                    Title = $"Beautiful appointment {i}", // TODO: Fake.
                    Description = "Basic description", // TODO: Fake.
                    DoctorToMeet = patient.DoctоrGuid ?? Guid.Empty,
                    DateTime = _faker.Date.Between(patient.Profile.BirthDate, new DateTime(2023, 12, 31))
                };

                appointmentsList.Add(appointment);
            }

            // Create fake notes.
            var notesList = new List<Note>();
            for (int i = 0; i < _random.Next(1, 5); i++)
            {
                var note = new Note()
                {
                    Title = $"Extraordinary note {i}", // TODO: Fake.
                    Description = "Basic description", // TODO: Fake.
                    DateTime = _faker.Date.Between(patient.Profile.BirthDate, new DateTime(2023, 12, 31))
                };

                notesList.Add(note);
            }

            // Create fake medication.
            var medicationList = new List<Medication>();
            for (int i = 0; i < _random.Next(1, 5); i++)
            {
                var medication = new Medication()
                {
                    Title = $"Amazingly complicated medication name {i}", // TODO: Fake.
                    Description = "Basic description", // TODO: Fake.
                    StartedUsingOn = _faker.Date.Between(patient.Profile.BirthDate, new DateTime(2023, 12, 31))
                };

                // Sometimes people are still using the meds, so we do not always add them.
                if (_random.NextBool())
                {
                    medication.StoppedUsingOn =
                        _faker.Date.Between(medication.StartedUsingOn, new DateTime(2023, 12, 31));
                }

                Logger.Trace($"Adding medication with stoppedUsingOn = {medication.StoppedUsingOn}");
                medicationList.Add(medication);
            }

            // Create fake diagnoses.
            var diagnosisList = new List<Diagnosis>();
            for (int i = 0; i < _random.Next(1, 5); i++)
            {
                var diagnosis = new Diagnosis()
                {
                    Title = $"Very long disease or disorder name {i}", // TODO: Fake.
                    Description = "Basic description", // TODO: Fake.
                    DateTime = _faker.Date.Between(patient.Profile.BirthDate, new DateTime(2023, 12, 31))
                };

                diagnosisList.Add(diagnosis);
            }

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