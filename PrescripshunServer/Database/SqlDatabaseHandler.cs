using PrescripshunLib.Models.Chat;
using PrescripshunLib.Models.MedicalFile;
using PrescripshunLib.Models.User;
using SqlKata;

namespace PrescripshunServer.Database
{
    internal class SqlDatabaseHandler : IDatabaseHandler
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly SqlDatabase _sqlDatabase = new SqlDatabase();

        private async Task TestInit()
        {
            string tableName = "TESTTABLE";
            await _sqlDatabase.ExecuteNonQueryAsync($"CREATE TABLE IF NOT EXISTS {tableName} (\r\n  ID INTEGER PRIMARY KEY,\r\n  Title VARCHAR(30),\r\n  Description VARCHAR(30)\r\n);");
            await _sqlDatabase.ExecuteNonQueryAsync($"INSERT INTO {tableName}\r\nVALUES (483, 'mooie titel', 'mooie desc');");
            await _sqlDatabase.ExecuteNonQueryAsync($"INSERT INTO {tableName}\r\nVALUES (821, 'mooie titel', 'andere desc');");
            await _sqlDatabase.ExecuteNonQueryAsync($"INSERT INTO {tableName}\r\nVALUES (129, 'titel ding', 'desc ding');");

            _sqlDatabase.ExecuteQuery($"SELECT * FROM `{tableName}` WHERE Title = 'mooie titel';\r\n", reader =>
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32("ID");
                    var title = reader.GetString("Title");
                    var desc = reader.GetString("Description");

                    Logger.Trace($"Retrieved test: {id} - {title} - {desc}");
                }
            });


            await _sqlDatabase.ExecuteNonQueryAsync($"DROP TABLE {tableName};");
        }

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

        public async Task Run()
        {
            await _sqlDatabase.ConnectAsync();
            // await _sqlDatabase.ExecuteNonQueryAsync("CREATE DATABASE MyDatabase()");

            await InitTables();

            await _sqlDatabase.DisconnectAsync();
        }

        public List<IUser> GetUsers()
        {
            throw new NotImplementedException();
        }

        public async Task AddDoctor(UserDoctor doctor)
        {
            await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                    INSERT INTO users (userKey, username, password)
                                                    VALUES ('{doctor.UserKey}', '{doctor.UserName}', '{doctor.Password}');
                                                    """);

            // Then we add all the patients to the patients table.
            foreach (var patient in doctor.Patients)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                        INSERT INTO doctor_patient (doctorKey, patientKey)
                                                        VALUES ('{doctor.UserKey}', '{patient}');
                                                        """);
            }
        }

        public List<UserDoctor> GetDoctors()
        {
            throw new NotImplementedException();
        }

        public async Task AddPatient(UserPatient patient)
        {
            await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                    INSERT INTO users (userKey, username, password, doctorKey)
                                                    VALUES ('{patient.UserKey}', '{patient.Password}', '{patient.Password}', '{patient.DoctоrGuid}');
                                                    """);
        }

        public List<UserPatient> GetPatients()
        {
            throw new NotImplementedException();
        }

        public IUser GetUser(Guid guid)
        {
            throw new NotImplementedException();
        }

        public UserDoctor GetDoctor(Guid guid)
        {
            throw new NotImplementedException();
        }

        public UserPatient GetPatient(Guid guid)
        {
            throw new NotImplementedException();
        }

        public async Task AddMedicalFile(IMedicalFile medicalFile)
        {
            // Notes.
            foreach (var note in medicalFile.Notes)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                         INSERT INTO notes (userKey, title, description, datetime)
                                                         VALUES ('{medicalFile.Patient}', '{note.Title}', '{note.Description}', '{note.DateTime}');
                                                         """);
            }

            // Diagnoses.
            foreach (var diagnosis in medicalFile.Diagnoses)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                         INSERT INTO diagnoses (userKey, title, description, datetime)
                                                         VALUES ('{medicalFile.Patient}', '{diagnosis.Title}', '{diagnosis.Description}', '{diagnosis.DateTime}');
                                                         """);
            }

            // Medication.
            foreach (var medication in medicalFile.Medication)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                        INSERT INTO medication (userKey, title, description, startedUsingOn, stoppedUsingOn)
                                                        VALUES ('{medicalFile.Patient}', '{medication.Title}', '{medication.Description}', '{medication.StartedUsingOn}', '{medication.StoppedUsingOn}');
                                                        """);
            }

            // Appointments.
            foreach (var appointment in medicalFile.Appointments)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                         INSERT INTO appointments (userKey, doctorKey, title, description, datetime)
                                                         VALUES ('{medicalFile.Patient}', '{appointment.DoctorToMeet}', '{appointment.Title}', '{appointment.Description}', '{appointment.DateTime}');
                                                         """);
            }
        }

        public IMedicalFile GetMedicalFile(Guid guid)
        {
            throw new NotImplementedException();
        }

        public async Task AddChat(IChat chat)
        {
            foreach (var message in chat.Messages)
            {
                await _sqlDatabase.ExecuteNonQueryAsync($"""
                                                        INSERT INTO chatmessages (sender, recipient, text, time)
                                                        VALUES ('{message.Sender}', '{message.Recipient}', '{message.Text}', '{message.Time}');
                                                        """);
            }
        }

        public IChat GetChat(Guid user1, Guid user2)
        {
            throw new NotImplementedException();
        }
    }
}
