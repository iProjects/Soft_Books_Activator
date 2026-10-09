using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using console_commonlib;
using System.Data;
using console_dto;

namespace console_dal
{
    public sealed class mongodbapisingleton
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static mongodbapisingleton singleInstance;

        public static mongodbapisingleton getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new mongodbapisingleton(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        private string CONNECTION_STRING = @"mongodb://localhost:27017";
        // Sets the connection URI
        string connectionUri = "mongodb://localhost:27017";
        string app_name = System.Configuration.ConfigurationManager.AppSettings["APP_NAME"];
        string DB_NAME = ConfigurationManager.AppSettings["mongodb_database"];
        string host = ConfigurationManager.AppSettings["mongodb_datasource"];
        int port = int.Parse(ConfigurationManager.AppSettings["mongodb_port"]);
        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        private string TAG;

        MongoServerSettings server_settings;
        MongoServer server;
        MongoDatabase database;

        MongoCollection<task_dto> tasks;
        MongoCollection<log_dto> logs;
        MongoCollection<user_dto> users;

        private mongodbapisingleton(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {

            try
            {
                TAG = this.GetType().Name;
                _notificationmessageEventname = notificationmessageEventname;

                //setconnectionstring();
                //createdatabaseonfirstload();
                //createtablesonfirstload();

                connect_to_database();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private mongodbapisingleton()
        {

        }

        public responsedto set_up_database_on_load()
        {
            responsedto _responsedto = new responsedto();

            //responsedto _cloud_responsedto = connect_to_cloud_database();
            responsedto _local_responsedto = connect_to_local_database();

            //if (_cloud_responsedto.isresponseresultsuccessful)
            //{
            //    _responsedto.responsesuccessmessage += _cloud_responsedto.responsesuccessmessage + Environment.NewLine;
            //    _responsedto.isresponseresultsuccessful = true;
            //}
            //else
            //{
            //    _responsedto.responseerrormessage += _cloud_responsedto.responseerrormessage + Environment.NewLine;
            //    _responsedto.isresponseresultsuccessful = false;
            //}

            if (_local_responsedto.isresponseresultsuccessful)
            {
                _responsedto.responsesuccessmessage += _local_responsedto.responsesuccessmessage;
                _responsedto.isresponseresultsuccessful = true;
            }
            else
            {
                _responsedto.responseerrormessage += _local_responsedto.responseerrormessage;
                _responsedto.isresponseresultsuccessful = false;
            }

            return _responsedto;
        }

        public responsedto connect_to_cloud_database()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                var client = new MongoClient(@"mongodb://softwareproviders254_db_user:ObraVoke%4027809543@estatecluster.ch2oagw.mongodb.net/inpatient_management_db");
                server = client.GetServer();

                //const string connectionUri = "mongodb+srv://localhost:27017/?connectTimeoutMS=60000&tls=true";
                //var url = new MongoUrl(connectionUri);
                //var settings = MongoClientSettings.FromUrl(url);

                //server_settings = new MongoServerSettings();
                //server_settings.Server = new MongoServerAddress(host, port);
                //server = new MongoServer(server_settings);

                if (!server.DatabaseExists(DB_NAME))
                {
                    MongoDatabase.Create(server_settings, DB_NAME);
                }

                database = server.GetDatabase(DB_NAME);

                tasks = database.GetCollection<task_dto>("tasks");
                logs = database.GetCollection<log_dto>("logs");
                users = database.GetCollection<user_dto>("users");

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

                _responsedto.responsesuccessmessage = "Mongo DB Database Setup Successfull.";
                return _responsedto;

            }
            catch (Exception ex)
            {
                _responsedto.responseerrormessage = ex.ToString();
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _responsedto;
            }
        }

        public responsedto connect_to_local_database()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                server_settings = new MongoServerSettings();
                server_settings.Server = new MongoServerAddress(host, port);
                server = new MongoServer(server_settings);

                if (!server.DatabaseExists(DB_NAME))
                {
                    MongoDatabase.Create(server_settings, DB_NAME);
                }

                database = server.GetDatabase(DB_NAME);

                tasks = database.GetCollection<task_dto>("tasks");
                logs = database.GetCollection<log_dto>("logs");
                users = database.GetCollection<user_dto>("users");

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");


                _responsedto.responsesuccessmessage = "Mongo DB Database Setup Successfull.";
                return _responsedto;

            }
            catch (Exception ex)
            {
                _responsedto.responseerrormessage = ex.ToString();
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _responsedto;
            }
        }

        public bool check_if_mongodb_database_exists(string _database_name)
        {
            if (server.DatabaseExists(_database_name))
            {
                return true;
            }
            return false;
        }

        public responsedto createdatabasegivennamefromconsole(string dbname)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                server_settings = new MongoServerSettings();
                server_settings.Server = new MongoServerAddress(host, port);
                server = new MongoServer(server_settings);

                DB_NAME = dbname;

                if (!server.DatabaseExists(DB_NAME))
                {
                    MongoDatabase.Create(server_settings, DB_NAME);

                    Console.WriteLine("successfully created database [ " + dbname + " ] in " + DBContract.mongodb + ".");
                }
                else
                {
                    Console.WriteLine("Database [ " + dbname + " ] exists in " + DBContract.mongodb + ".");
                }

                database = server.GetDatabase(DB_NAME);

                tasks = database.GetCollection<task_dto>("tasks");
                Console.WriteLine("successfully created table [ tasks ] in " + DBContract.mongodb + ".");
                logs = database.GetCollection<log_dto>("logs");
                Console.WriteLine("successfully created table [ logs ] in " + DBContract.mongodb + ".");
                users = database.GetCollection<user_dto>("users");
                Console.WriteLine("successfully created table [ users ] in " + DBContract.mongodb + ".");

                return _responsedto;

            }
            catch (Exception ex)
            {
                _responsedto.responseerrormessage = ex.ToString();
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _responsedto;
            }
        }

        public responsedto create_tables(mongodbconnectionstringdto _connectionstringdto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                server_settings = new MongoServerSettings();
                server_settings.Server = new MongoServerAddress(host, port);
                server = new MongoServer(server_settings);

                DB_NAME = _connectionstringdto.new_database_name;

                if (!server.DatabaseExists(DB_NAME))
                {
                    MongoDatabase.Create(server_settings, DB_NAME);
                }

                database = server.GetDatabase(DB_NAME);

                tasks = database.GetCollection<task_dto>("tasks");
                Console.WriteLine("successfully created table [ tasks ] in " + DBContract.mongodb + ".");
                logs = database.GetCollection<log_dto>("logs");
                Console.WriteLine("successfully created table [ logs ] in " + DBContract.mongodb + ".");
                users = database.GetCollection<user_dto>("users");
                Console.WriteLine("successfully created table [ users ] in " + DBContract.mongodb + ".");

                _responsedto.isresponseresultsuccessful = true;

                return _responsedto;

            }
            catch (Exception ex)
            {
                _responsedto.responseerrormessage = ex.ToString();
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _responsedto;
            }
        }

        public mongodbconnectionstringdto getmongodbconnectionstringdto()
        {
            try
            {
                mongodbconnectionstringdto _connectionstringdto = new mongodbconnectionstringdto();

                _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mongodb_datasource"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mongodb_database"];
                _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mongodb_userid"];
                _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mongodb_password"];
                _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mongodb_port"];

                return _connectionstringdto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        public responsedto checkmongodbconnectionstate()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                _responsedto = checkconnectionasadmin();

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }

        public responsedto checkconnectionasadmin()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                int count = getrecordscountgiventable();

                if (count != -1)
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "connection to mongodb successfull. logs count [ " + count + " ].";
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "connection to mongodb failed.";
                    return _responsedto;
                }
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }

        public int getrecordscountgiventable()
        {
            DataTable dt = get_all_logs();
            if (dt != null)
                return dt.Rows.Count;
            else
                return -1;
        }

        public void connect_to_database()
        {
            try
            {
                server_settings = new MongoServerSettings();
                server_settings.Server = new MongoServerAddress(host, port);
                server = new MongoServer(server_settings);

                if (!server.DatabaseExists(DB_NAME))
                {
                    MongoDatabase.Create(server_settings, DB_NAME);
                }

                database = server.GetDatabase(DB_NAME);

                tasks = database.GetCollection<task_dto>("tasks");
                logs = database.GetCollection<log_dto>("logs");
                users = database.GetCollection<user_dto>("users");

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        public responsedto create_task_in_database(task_dto _task_dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                MongoCollection<task_dto> tasks = database.GetCollection<task_dto>("tasks");

                task_dto task_dto = new task_dto();
                task_dto.mongodb_id = ObjectId.GenerateNewId();
                task_dto.id = get_next_task_id();
                task_dto.name = _task_dto.name;
                task_dto.description = _task_dto.description;
                task_dto.status = _task_dto.status;
                task_dto.expected_completion_date = _task_dto.expected_completion_date;
                task_dto.actual_completion_date = _task_dto.actual_completion_date;
                task_dto.created_date = _task_dto.created_date;

                MongoInsertOptions opts = new MongoInsertOptions();
                opts.Flags = InsertFlags.ContinueOnError;

                tasks.Insert(task_dto, opts);

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mongodb;

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }

        private long get_next_task_id()
        {
            try
            {
                long next_id;
                MongoCollection<task_dto> tasks = database.GetCollection<task_dto>("tasks");
                List<task_dto> lst_tasks = tasks.FindAll().AsQueryable().OrderByDescending(a => a.id).Take(20).ToList();
                var last_doc = lst_tasks.FirstOrDefault();
                if (last_doc != null)
                {
                    long previous_id = last_doc.id;
                    next_id = previous_id + 1;
                }
                else
                {
                    next_id = 1;
                }
                return next_id;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return 0;
            }
        }

        public bool check_if_task_exists(string entity_name)
        {
            bool _exists = false;
            try
            {
                DataTable dt = new DataTable();
                dt = get_all_tasks();

                var _recordscount = dt.Rows.Count;

                for (int i = 0; i < _recordscount; i++)
                {
                    var _record_from_server = Convert.ToString(dt.Rows[i]["task_name"]);

                    if (entity_name == _record_from_server)
                    {
                        _exists = true;
                        return _exists;
                    }
                }

                return _exists;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _exists;
            }
        }

        public DataTable get_all_tasks()
        {
            MongoCollection<task_dto> tasks = database.GetCollection<task_dto>("tasks");
            DataTable dt = new DataTable();
            var count = tasks.FindAll().Count();
            List<task_dto> lst_dto = new List<task_dto>();

            foreach (var dto in tasks.FindAll())
            {
                lst_dto.Add(dto);
            }

            dt = utilzsingleton.getInstance(_notificationmessageEventname).Convert_List_To_Datatable(lst_dto);

            return dt;

        }

        public responsedto truncate_database()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                MongoCollection<task_dto> tasks = database.GetCollection<task_dto>("tasks");
                DataTable dt = new DataTable();
                var count = tasks.FindAll().Count();
                tasks.RemoveAll();

                MongoCollection<log_dto> logs = database.GetCollection<log_dto>("logs");
                dt = new DataTable();
                count = logs.FindAll().Count();
                logs.RemoveAll();

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Tables successfully truncated in " + DBContract.mongodb;

                return _responsedto;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }

        #region "logs"
        public responsedto create_log_in_database(log_dto dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                MongoCollection<log_dto> records = database.GetCollection<log_dto>("logs");

                log_dto _dto = new log_dto();
                _dto.mongodb_id = ObjectId.GenerateNewId();
                _dto.id = get_next_log_id();
                _dto.message = dto.message;
                _dto.timestamp = dto.timestamp;
                _dto.tag = dto.tag;
                _dto.status = "active";
                _dto.created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt");

                MongoInsertOptions opts = new MongoInsertOptions();
                opts.Flags = InsertFlags.ContinueOnError;

                records.Insert(_dto, opts);

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mongodb + ".";

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        private long get_next_log_id()
        {
            try
            {
                long next_id;
                MongoCollection<log_dto> records = database.GetCollection<log_dto>("logs");
                List<log_dto> lst_dto = records.FindAll().AsQueryable().OrderByDescending(a => a.id).Take(20).ToList();
                var last_doc = lst_dto.FirstOrDefault();
                if (last_doc != null)
                {
                    long previous_id = last_doc.id;
                    next_id = previous_id + 1;
                }
                else
                {
                    next_id = 1;
                }
                return next_id;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return 0;
            }
        }
        public bool check_if_log_exists(string entity_name)
        {
            bool _exists = false;
            try
            {
                DataTable dt = new DataTable();
                dt = get_all_logs();

                var _recordscount = dt.Rows.Count;

                for (int i = 0; i < _recordscount; i++)
                {
                    var _record_from_server = Convert.ToString(dt.Rows[i]["message"]);

                    if (entity_name == _record_from_server)
                    {
                        _exists = true;
                        return _exists;
                    }
                }

                return _exists;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _exists;
            }
        }

        public DataTable get_all_logs()
        {
            MongoCollection<log_dto> records = database.GetCollection<log_dto>("logs");
            DataTable dt = new DataTable();
            var count = records.FindAll().Count();
            List<log_dto> lst_dto = new List<log_dto>();

            foreach (var dto in records.FindAll())
            {
                lst_dto.Add(dto);
            }

            dt = utilzsingleton.getInstance(_notificationmessageEventname).Convert_List_To_Datatable(lst_dto);

            return dt;

        }
        #endregion "log"

        #region "users"
        public responsedto create_user_in_database(user_dto dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                MongoCollection<user_dto> records = database.GetCollection<user_dto>("users");

                user_dto _dto = new user_dto();
                _dto.mongodb_id = ObjectId.GenerateNewId();
                _dto.id = get_next_log_id();
                _dto.email = dto.email;
                _dto.pass_word = dto.pass_word;
                _dto.password_salt = dto.password_salt;
                _dto.password_hash = dto.password_hash;
                _dto.full_names = dto.full_names;
                _dto.gender = dto.gender;
                _dto.dob = dto.dob;
                _dto.year = dto.year;
                _dto.month = dto.month;
                _dto.day = dto.day;
                _dto.status = "active";
                _dto.created_date = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt");

                MongoInsertOptions opts = new MongoInsertOptions();
                opts.Flags = InsertFlags.ContinueOnError;

                records.Insert(_dto, opts);

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mongodb + ".";

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        private long get_next_user_id()
        {
            try
            {
                long next_id;
                MongoCollection<user_dto> records = database.GetCollection<user_dto>("users");
                List<user_dto> lst_dto = records.FindAll().AsQueryable().OrderByDescending(a => a.id).Take(20).ToList();
                var last_doc = lst_dto.FirstOrDefault();
                if (last_doc != null)
                {
                    long previous_id = last_doc.id;
                    next_id = previous_id + 1;
                }
                else
                {
                    next_id = 1;
                }
                return next_id;
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return 0;
            }
        }
        public bool check_if_user_exists(string entity_name)
        {
            bool _exists = false;
            try
            {
                DataTable dt = new DataTable();

                dt = get_all_users();

                var _recordscount = dt.Rows.Count;

                for (int i = 0; i < _recordscount; i++)
                {
                    var _record_from_server = Convert.ToString(dt.Rows[i]["email"]);

                    if (entity_name == _record_from_server)
                    {
                        _exists = true;
                        return _exists;
                    }
                }

                return _exists;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _exists;
            }
        }

        public DataTable get_all_users()
        {
            MongoCollection<user_dto> records = database.GetCollection<user_dto>("users");
            DataTable dt = new DataTable();
            var count = records.FindAll().Count();
            List<user_dto> lst_dto = new List<user_dto>();

            foreach (var dto in records.FindAll())
            {
                lst_dto.Add(dto);
            }

            dt = utilzsingleton.getInstance(_notificationmessageEventname).Convert_List_To_Datatable(lst_dto);

            return dt;

        }
        #endregion "users"























    }
}
