using System.Data;
using MySql.Data.Types;
using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;
using PrescripshunLib.Models.User.Profile;
using PrescripshunLib.Util.Faker;

namespace PrescripshunServer.Database.MySql
{
    internal class SqlDatabaseHandler : IDatabaseHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly SqlDatabase _sqlDatabase = new SqlDatabase();

        private async Task InitTables()
        {
            string usersTable = """
                                CREATE TABLE users (
                                    userKey CHAR(36) NOT NULL,      -- GUID for the user, represented as a CHAR(36)
                                    username VARCHAR(255) NOT NULL, -- Username, non-null string
                                    password VARCHAR(255) NOT NULL, -- Password, non-null string
                                    doctorKey CHAR(36),             -- Foreign key reference to a doctor (can be NULL for doctors)
                                    
                                    PRIMARY KEY (userKey),          -- Primary key constraint on userKey
                                    FOREIGN KEY (doctorKey) REFERENCES users(userKey)  -- Foreign key reference to another user's userKey (self-reference)
                                ) ENGINE=InnoDB;
                                """;

            await _sqlDatabase.ExecuteNonQueryAsync(usersTable);

            // Many-to-one relationship table.
            string doctorPatientsTable = """
                                         CREATE TABLE doctor_patient (
                                             doctorKey CHAR(36) NOT NULL,    -- GUID of the doctor
                                             patientKey CHAR(36) NOT NULL,   -- GUID of the patient
                                             
                                             PRIMARY KEY (doctorKey, patientKey), -- Composite primary key to ensure uniqueness of doctor-patient pairs
                                             FOREIGN KEY (doctorKey) REFERENCES users(userKey) ON DELETE CASCADE, -- Foreign key reference to users (doctor)
                                             FOREIGN KEY (patientKey) REFERENCES users(userKey) ON DELETE CASCADE -- Foreign key reference to users (patient)
                                         ) ENGINE=InnoDB;
                                         """;

            await _sqlDatabase.ExecuteNonQueryAsync(doctorPatientsTable);

            string chatMessagesTable = """
                                       CREATE TABLE chatmessages (
                                           id INT AUTO_INCREMENT PRIMARY KEY,  -- Unique ID for each message (auto-increment). This way we can reference the message if we ever tweak the chat system to allow editing.
                                           sender CHAR(36) NOT NULL,           -- GUID of the sender (references a user)
                                           recipient CHAR(36) NOT NULL,        -- GUID of the recipient (references a user)
                                           text TEXT NOT NULL,                 -- The message text, large enough to hold long messages
                                           time DATETIME NOT NULL,             -- The date and time when the message was sent
                                           
                                           FOREIGN KEY (sender) REFERENCES users(userKey) ON DELETE CASCADE,   -- Foreign key reference to the sender in the users table
                                           FOREIGN KEY (recipient) REFERENCES users(userKey) ON DELETE CASCADE -- Foreign key reference to the recipient in the users table
                                       ) ENGINE=InnoDB;
                                       """;

            await _sqlDatabase.ExecuteNonQueryAsync(chatMessagesTable);

            string profilesTable = """
                                   CREATE TABLE profiles (
                                       userKey CHAR(36) NOT NULL,          -- Foreign key linking to the users table (1-to-1 relationship)
                                       fullname VARCHAR(255) NOT NULL,     -- Full name of the user (required)
                                       birthdate DATETIME NOT NULL,        -- Birthdate of the user (required)
                                       profilepicture VARCHAR(2083),       -- URL to the profile picture (optional, can be NULL)
                                       
                                       PRIMARY KEY (userKey),              -- Primary key is the userKey (same as in users table)
                                       FOREIGN KEY (userKey) REFERENCES users(userKey) ON DELETE CASCADE -- Foreign key referencing users table
                                   ) ENGINE=InnoDB;
                                   """;

            await _sqlDatabase.ExecuteNonQueryAsync(profilesTable);

            string notesTable = """
                                CREATE TABLE notes (
                                    noteId INT AUTO_INCREMENT PRIMARY KEY,    -- Unique ID for each note (auto-increment)
                                    userKey CHAR(36) NOT NULL,                -- Reference to the user who created the note
                                    title VARCHAR(255) NOT NULL,              -- Title of the note (required)
                                    description TEXT NOT NULL,                -- Description of the note (required)
                                    datetime DATETIME NOT NULL,               -- Date and time when the note was created
                                    
                                    FOREIGN KEY (userKey) REFERENCES users(userKey) ON DELETE CASCADE  -- Foreign key reference to users table
                                ) ENGINE=InnoDB;
                                """;

            await _sqlDatabase.ExecuteNonQueryAsync(notesTable);

            string diagnosesTable = """
                                    CREATE TABLE diagnoses (
                                        diagnosisId INT AUTO_INCREMENT PRIMARY KEY, -- Unique ID for each diagnosis (auto-increment)
                                        userKey CHAR(36) NOT NULL,                  -- Reference to the user (patient) diagnosed
                                        title VARCHAR(255) NOT NULL,                -- Title of the diagnosis (required)
                                        description TEXT NOT NULL,                  -- Description of the diagnosis (required)
                                        datetime DATETIME NOT NULL,                 -- Date and time of the diagnosis
                                        
                                        FOREIGN KEY (userKey) REFERENCES users(userKey) ON DELETE CASCADE -- Foreign key reference to users table
                                    ) ENGINE=InnoDB;
                                    """;

            await _sqlDatabase.ExecuteNonQueryAsync(diagnosesTable);

            string medicationTable = """
                                     CREATE TABLE medication (
                                         medicationId INT AUTO_INCREMENT PRIMARY KEY, -- Unique ID for each medication (auto-increment)
                                         userKey CHAR(36) NOT NULL,                   -- Reference to the user (patient) prescribed the medication
                                         title VARCHAR(255) NOT NULL,                 -- Name or title of the medication
                                         description TEXT NOT NULL,                   -- Description of the medication
                                         startedUsingOn DATETIME NOT NULL,            -- Date when the medication started being used
                                         stoppedUsingOn DATETIME NULL,                -- Date when the medication stopped (nullable)
                                         
                                         FOREIGN KEY (userKey) REFERENCES users(userKey) ON DELETE CASCADE -- Foreign key reference to users table
                                     ) ENGINE=InnoDB;
                                     """;

            await _sqlDatabase.ExecuteNonQueryAsync(medicationTable);

            string appointments = """
                                  CREATE TABLE appointments (
                                      appointmentId INT AUTO_INCREMENT PRIMARY KEY, -- Unique ID for each appointment (auto-increment)
                                      userKey CHAR(36) NOT NULL,                    -- Reference to the user (patient) having the appointment
                                      doctorKey CHAR(36) NOT NULL,                  -- Reference to the doctor involved in the appointment
                                      title VARCHAR(255) NOT NULL,                  -- Title of the appointment (required)
                                      description TEXT NOT NULL,                    -- Description of the appointment (required)
                                      datetime DATETIME NOT NULL,                   -- Date and time of the appointment
                                      
                                      FOREIGN KEY (userKey) REFERENCES users(userKey) ON DELETE CASCADE,  -- Foreign key reference to patient (user)
                                      FOREIGN KEY (doctorKey) REFERENCES users(userKey) ON DELETE CASCADE -- Foreign key reference to doctor
                                  ) ENGINE=InnoDB;
                                  """;

            await _sqlDatabase.ExecuteNonQueryAsync(appointments);
        }

        private async Task InitTestData()
        {
            var fakeHandler = new FakeHandler();
            var doctorsList = fakeHandler.GetDoctors();
            var patientsList = fakeHandler.GetPatients(ref doctorsList);
            var medicalFiles = fakeHandler.GetMedicalFiles(ref patientsList);
            var chatMessagesList = fakeHandler.GetChats(ref patientsList);

            // PLEASE NOTE: ORDER MATTERS HERE!!! DON'T PLAY AROUND WITH THIS!
            foreach (var doctor in doctorsList) await AddDoctor(doctor);
            foreach (var patient in patientsList) await AddPatient(patient);
            foreach (var patient in patientsList) await AddDoctorPatientRelation(patient);
            foreach (var medicalFile in medicalFiles) await AddMedicalFile(medicalFile);
            foreach (var chat in chatMessagesList) await AddChat(chat);
        }

        public async Task Run()
        {
            await _sqlDatabase.ConnectAsync();

            await InitTables();
            await InitTestData();


            var patients = GetPatients();
            var patient = patients.First();

            var profile = patient.GetPatientProfile;
            Logger.Info($"PROFILE RECEIVED: {profile}");

            var medicalFile = GetMedicalFile(patient.UserKey);
            Logger.Info($"FILE RECEIVED: {medicalFile}");

            await _sqlDatabase.DisconnectAsync();
        }

        public List<IUser> GetUsers()
        {
            var patients = GetPatients().ToList<IUser>();
            var doctors = GetDoctors().ToList<IUser>();
            patients.AddRange(doctors);
            return patients;
        }

        private async Task AddDoctorPatientRelation(UserPatient patient)
        {
            await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                     INSERT INTO doctor_patient (doctorKey, patientKey)
                                                     VALUES ('{patient.DoctоrGuid}', '{patient.UserKey}');
                                                     """);
        }

        public async Task AddDoctor(UserDoctor doctor)
        {
            await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                     INSERT INTO users (userKey, username, password)
                                                     VALUES ('{doctor.UserKey}', '{doctor.UserName}', '{doctor.Password}');
                                                     """);

            // Then we add the profile of the doctor. // TODO: STORE PROFILE PICTURE.
            await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                     INSERT INTO profiles (userKey, fullname, birthdate, profilepicture)
                                                     VALUES ('{doctor.UserKey}', '{doctor.Profile.FullName}', '{doctor.Profile.BirthDate.GetSqlString()}', NULL);
                                                     """);
        }

        public List<UserDoctor> GetDoctors()
        {
            var doctors = new List<UserDoctor>();
            _sqlDatabase.ExecuteQuery("""
                                      SELECT u.*, p.fullname, p.birthdate, p.profilepicture
                                      FROM users u
                                      JOIN profiles p ON u.userKey = p.userKey
                                      WHERE u.doctorKey IS NULL;
                                      """, reader =>
            {
                while (reader.Read())
                {
                    doctors.Add(new UserDoctor()
                    {
                        // TODO: Retrieve patient list?
                        UserKey = reader.GetGuid("userKey"),
                        UserName = reader.GetString("username"),
                        Password = reader.GetString("password"),
                        Profile = new PatientProfile()
                        {
                            BirthDate = reader.GetDateTime("birthdate"),
                            FullName = reader.GetString("fullname"),
                            // TODO: Retrieve profile picture.
                            // TODO: DO NOT retrieve the medical file here
                        }
                    });
                }
            });

            return doctors;
        }

        public async Task AddPatient(UserPatient patient)
        {
            await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                     INSERT INTO users (userKey, username, password, doctorKey)
                                                     VALUES ('{patient.UserKey}', '{patient.UserName}', '{patient.Password}', '{patient.DoctоrGuid}');
                                                     """);


            // Then we add the profile of the doctor. // TODO: STORE PROFILE PICTURE.
            await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                     INSERT INTO profiles (userKey, fullname, birthdate, profilepicture)
                                                     VALUES ('{patient.UserKey}', '{patient.Profile.FullName}', '{patient.Profile.BirthDate.GetSqlString()}', NULL);
                                                     """);
        }

        public List<UserPatient> GetPatients()
        {
            var patients = new List<UserPatient>();
            _sqlDatabase.ExecuteQuery("""
                                      SELECT u.*, p.fullname, p.birthdate, p.profilepicture
                                      FROM users u
                                      JOIN profiles p ON u.userKey = p.userKey
                                      WHERE u.doctorKey IS NOT NULL;
                                      """, reader =>
            {
                while (reader.Read())
                {
                    patients.Add(new UserPatient()
                    {
                        UserKey = reader.GetGuid("userKey"),
                        DoctоrGuid = reader.GetGuid("doctorKey"),
                        UserName = reader.GetString("username"),
                        Password = reader.GetString("password"),
                        Profile = new PatientProfile()
                        {
                            BirthDate = reader.GetDateTime("birthdate"),
                            FullName = reader.GetString("fullname"),
                            // TODO: Retrieve profile picture.
                            // TODO: DO NOT retrieve the medical file here
                        }
                    });
                }
            });

            return patients;
        }

        public IUser GetUser(Guid guid)
        {
            try
            {
                return GetPatient(guid);
            }
            catch
            {
                return GetDoctor(guid);
            }
        }

        public UserDoctor GetDoctor(Guid guid)
        {
            UserDoctor? doctor = null;

            _sqlDatabase.ExecuteQuery($"""
                                       SELECT u.*, p.fullname, p.birthdate, p.profilepicture
                                       FROM users u
                                       JOIN profiles p ON u.userKey = p.userKey
                                       WHERE u.doctorKey IS NULL
                                       AND u.userKey = '{guid}';
                                       """, reader =>
            {
                while (reader.Read())
                {
                    doctor = new UserDoctor()
                    {
                        // TODO: Retrieve patient list?
                        UserKey = reader.GetGuid("userKey"),
                        UserName = reader.GetString("username"),
                        Password = reader.GetString("password"),
                        Profile = new PatientProfile()
                        {
                            BirthDate = reader.GetDateTime("birthdate"),
                            FullName = reader.GetString("fullname"),
                            // TODO: Retrieve profile picture.
                            // TODO: DO NOT retrieve the medical file here
                        }
                    };
                }
            });

            return doctor ?? throw new InvalidOperationException();
        }

        public UserPatient GetPatient(Guid guid)
        {
            UserPatient? patient = null;

            _sqlDatabase.ExecuteQuery($"""
                                      SELECT u.*, p.fullname, p.birthdate, p.profilepicture
                                      FROM users u
                                      JOIN profiles p ON u.userKey = p.userKey
                                      WHERE u.doctorKey IS NOT NULL
                                      AND u.userKey = '{guid}';
                                      """, reader =>
            {
                while (reader.Read())
                {
                    patient = new UserPatient()
                    {
                        UserKey = reader.GetGuid("userKey"),
                        DoctоrGuid = reader.GetGuid("doctorKey"),
                        UserName = reader.GetString("username"),
                        Password = reader.GetString("password"),
                        Profile = new PatientProfile()
                        {
                            BirthDate = reader.GetDateTime("birthdate"),
                            FullName = reader.GetString("fullname"),
                            // TODO: Retrieve profile picture.
                            // TODO: DO NOT retrieve the medical file here
                        }
                    };
                }
            });

            return patient ?? throw new InvalidOperationException();
        }

        public async Task AddMedicalFile(IMedicalFile medicalFile)
        {
            // Notes.
            foreach (var note in medicalFile.Notes)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                         INSERT INTO notes (userKey, title, description, datetime)
                                                         VALUES ('{medicalFile.Patient}', '{note.Title}', '{note.Description}', '{note.DateTime.GetSqlString()}');
                                                         """);
            }

            // Diagnoses.
            foreach (var diagnosis in medicalFile.Diagnoses)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                         INSERT INTO diagnoses (userKey, title, description, datetime)
                                                         VALUES ('{medicalFile.Patient}', '{diagnosis.Title}', '{diagnosis.Description}', '{diagnosis.DateTime.GetSqlString()}');
                                                         """);
            }

            // Medication.
            foreach (var medication in medicalFile.Medication)
            {
                string stoppedUsingString = medication.StoppedUsingOn is null ? "NULL" : $"'{medication.StoppedUsingOn?.GetSqlString()}'";

                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                         INSERT INTO medication (userKey, title, description, startedUsingOn, stoppedUsingOn)
                                                         VALUES ('{medicalFile.Patient}', '{medication.Title}', '{medication.Description}', '{medication.StartedUsingOn.GetSqlString()}', {stoppedUsingString});
                                                         """);
            }

            // Appointments.
            foreach (var appointment in medicalFile.Appointments)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                         INSERT INTO appointments (userKey, doctorKey, title, description, datetime)
                                                         VALUES ('{medicalFile.Patient}', '{appointment.DoctorToMeet}', '{appointment.Title}', '{appointment.Description}', '{appointment.DateTime.GetSqlString()}');
                                                         """);
            }
        }

        public IMedicalFile GetMedicalFile(Guid guid)
        {
            var notesList = new List<Note>();
            var appointmentList = new List<Appointment>();
            var medicationList = new List<Medication>();
            var diagnosisList = new List<Diagnosis>();

            _sqlDatabase.ExecuteQuery($"""
                                      SELECT *
                                      FROM notes
                                      WHERE userKey = '{guid}';
                                      """, reader =>
            {
                while (reader.Read())
                {
                    notesList.Add(new Note()
                    {
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        DateTime = reader.GetDateTime("datetime")
                    });
                }
            });

            _sqlDatabase.ExecuteQuery($"""
                                       SELECT *
                                       FROM appointments
                                       WHERE userKey = '{guid}';
                                       """, reader =>
            {
                while (reader.Read())
                {
                    appointmentList.Add(new Appointment()
                    {
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        DateTime = reader.GetDateTime("datetime"),
                        DoctorToMeet = reader.GetGuid("doctorKey")
                    });
                }
            });

            _sqlDatabase.ExecuteQuery($"""
                                       SELECT *
                                       FROM medication
                                       WHERE userKey = '{guid}';
                                       """, reader =>
            {
                while (reader.Read())
                {
                    try
                    {
                        medicationList.Add(new Medication()
                        {
                            Title = reader.GetString("title"),
                            Description = reader.GetString("description"),
                            StartedUsingOn = reader.GetDateTime("startedUsingOn"),
                        });

                        if (!reader.IsDBNull("stoppedUsingOn")) // Can not parse NULL as a date time, so this is needed.
                        {
                            medicationList.Last().StoppedUsingOn = reader.GetDateTime("stoppedUsingOn");
                        }
                    }
                    catch (MySqlConversionException ex)
                    {
                        // TODO: Fix.
                        Logger.Error(ex, "Still have an issue with stoppedUsingOn. Ignoring for now.");
                    }
                }
            });

            _sqlDatabase.ExecuteQuery($"""
                                       SELECT *
                                       FROM diagnoses
                                       WHERE userKey = '{guid}';
                                       """, reader =>
            {
                while (reader.Read())
                {
                    diagnosisList.Add(new Diagnosis()
                    {
                        Title = reader.GetString("title"),
                        Description = reader.GetString("description"),
                        DateTime = reader.GetDateTime("datetime")
                    });
                }
            });

            return new MedicalFile()
            {
                Patient = guid,
                Appointments = appointmentList,
                Diagnoses = diagnosisList,
                Medication = medicationList,
                Notes = notesList,
            };
        }

        public async Task AddChat(IChat chat)
        {
            foreach (var message in chat.Messages)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                         INSERT INTO chatmessages (sender, recipient, text, time)
                                                         VALUES ('{message.Sender}', '{message.Recipient}', '{message.Text}', '{message.Time.GetSqlString()}');
                                                         """);
            }
        }

        public IChat GetChat(Guid user1, Guid user2)
        {
            var messages = new List<IChatMessage>();

            _sqlDatabase.ExecuteQuery($"""
                                      SELECT *
                                      FROM chatmessages
                                      WHERE (sender = '{user1}' AND recipient = '{user2}')
                                         OR (sender = '{user2}' AND recipient = '{user1}');
                                      """, reader =>
            {
                while (reader.Read())
                {
                    messages.Add(new ChatMessage()
                    {
                        Recipient = reader.GetGuid("recipient"),
                        Sender = reader.GetGuid("sender"),
                        Text = reader.GetString("text"),
                        Time = reader.GetDateTime("time"),
                    });
                }
            });

            return new Chat()
            {
                User1 = user1,
                User2 = user2,
                Messages = messages,
            };
        }
    }
}