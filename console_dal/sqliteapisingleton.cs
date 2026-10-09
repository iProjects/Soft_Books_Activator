/*
 * Created by SharpDevelop.
 * User: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 09/23/2018
 * Time: 08:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using console_commonlib;
using console_dto;
using System.Text;

namespace console_dal
{
    /// <summary>
    /// Description of sqliteapisingleton.
    /// </summary>
    public sealed class sqliteapisingleton
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static sqliteapisingleton singleInstance;
        string DATABSE_WORKING_DIR = AppDomain.CurrentDomain.BaseDirectory;
        string DATABASE_NAME;

        public static sqliteapisingleton getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new sqliteapisingleton(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        private string CONNECTION_STRING = @"Data Source=tasks_tracking_db_2026_april.sqlite3;Pooling=true;FailIfMissing=false";

        // Holds our connection with the database
        SQLiteConnection m_dbConnection;

        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        private event EventHandler<notificationmessageEventArgs> _databaseutilsnotificationeventname;
        private string TAG;
        sqliteconnectionstringdto _connectionstringdto = new sqliteconnectionstringdto();

        private sqliteapisingleton(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            _notificationmessageEventname = notificationmessageEventname;
            try
            {
                TAG = this.GetType().Name;
                set_database_working_directory();
                set_database_name();
                createdatabaseonfirstload();
                createtablesonfirstload();
                createconnectionstring();
                setconnectionstring();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private sqliteapisingleton()
        {

        }
        private void set_database_working_directory()
        {
            try
            {
                //string base_dir = AppDomain.CurrentDomain.BaseDirectory;
                string base_dir = Path.GetTempPath();

                DATABSE_WORKING_DIR = base_dir;


            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }
        private void set_database_name()
        {
            try
            {
                string app_name = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
                string db_file_name = app_name + ".sqlite3";

                DATABASE_NAME = db_file_name;


            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        public responsedto set_up_database_on_load()
        {
            responsedto _responsedto = new responsedto();

            responsedto _database_responsedto = createdatabaseonfirstload();
            responsedto _tables_responsedto = createtablesonfirstload();

            if (!string.IsNullOrEmpty(_database_responsedto.responsesuccessmessage))
            {
                _responsedto.responsesuccessmessage += _database_responsedto.responsesuccessmessage + Environment.NewLine;
            }
            if (!string.IsNullOrEmpty(_database_responsedto.responseerrormessage))
            {
                _responsedto.responseerrormessage += _database_responsedto.responseerrormessage + Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(_tables_responsedto.responsesuccessmessage))
            {
                _responsedto.responsesuccessmessage += _tables_responsedto.responsesuccessmessage;
            }
            if (!string.IsNullOrEmpty(_tables_responsedto.responseerrormessage))
            {
                _responsedto.responseerrormessage += _tables_responsedto.responseerrormessage;
            }

            return _responsedto;
        }

        public responsedto setup_database()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                responsedto _db_responsedto = createdatabaseonfirstload();
                responsedto _table_responsedto = createtablesonfirstload();
                createconnectionstring();
                setconnectionstring();

                if (!string.IsNullOrEmpty(_db_responsedto.responsesuccessmessage))
                {
                    _responsedto.responsesuccessmessage += (Environment.NewLine + _db_responsedto.responsesuccessmessage);
                }
                if (!string.IsNullOrEmpty(_db_responsedto.responseerrormessage))
                {
                    _responsedto.responseerrormessage += (Environment.NewLine + _db_responsedto.responseerrormessage);
                }

                if (!string.IsNullOrEmpty(_table_responsedto.responsesuccessmessage))
                {
                    _responsedto.responsesuccessmessage += (Environment.NewLine + _table_responsedto.responsesuccessmessage);
                }
                if (!string.IsNullOrEmpty(_table_responsedto.responseerrormessage))
                {
                    _responsedto.responseerrormessage += (Environment.NewLine + _table_responsedto.responseerrormessage);
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage += ex.ToString();
            }

            return _responsedto;
        }
        private void createconnectionstring()
        {
            try
            {
                CONNECTION_STRING = setconnectionstring();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }


        private string setconnectionstring()
        {
            try
            {
                _connectionstringdto = new sqliteconnectionstringdto();

                _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
                _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
                _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
                _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
                _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

                CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                return CONNECTION_STRING;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return "";
            }
        }

        string buildconnectionstringfromobject(sqliteconnectionstringdto _connectionstringdto)
        {
            string base_dir = DATABSE_WORKING_DIR;
            string database_dir = Path.Combine(base_dir, _connectionstringdto.sqlite_database_path);

            string plain_dbname = _connectionstringdto.database;
            string database_version = _connectionstringdto.sqlite_version;
            string db_extension = _connectionstringdto.sqlite_db_extension;
            string dbname = plain_dbname + "." + db_extension + database_version;

            if (!Directory.Exists(database_dir))
            {
                //this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + database_dir + " ] does not exist.", TAG));
            }
            else
            {
                //this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + database_dir + " ] exist.", TAG));
            }

            string full_database_name_with_path = Path.Combine(database_dir, dbname);
            string _secure_path_name_response = dbname;

            if (!File.Exists(full_database_name_with_path))
            {
                //this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + full_database_name_with_path + " ] does not exist.", TAG));
            }
            else
            {
                //this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + full_database_name_with_path + " ] exist.", TAG));
            }

            string CONNECTION_STRING = @"Data Source=" + full_database_name_with_path + ";" +
            "Version=" + _connectionstringdto.sqlite_version + ";" +
            "Pooling=" + _connectionstringdto.sqlite_pooling + ";" +
            "FailIfMissing=" + _connectionstringdto.sqlite_fail_if_missing;

            return CONNECTION_STRING;
        }

        // Creates a connection with our database file.
        public void connectToDatabase()
        {
            try
            {
                CONNECTION_STRING = setconnectionstring();
                m_dbConnection = new SQLiteConnection(CONNECTION_STRING);
                m_dbConnection.Open();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private responsedto createdatabaseonfirstload()
        {
            _connectionstringdto = new sqliteconnectionstringdto();

            _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
            _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
            _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
            _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
            _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
            _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

            responsedto _responsedto = createdatabasegivenname(_connectionstringdto);
            return _responsedto;
        }
        private responsedto createtablesonfirstload()
        {
            _connectionstringdto = new sqliteconnectionstringdto();

            _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
            _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
            _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
            _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
            _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
            _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

            responsedto _responsedto = createtables(_connectionstringdto);
            return _responsedto;
        }
        public responsedto createdatabasegivenname(sqliteconnectionstringdto _connectionstringdto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                string base_dir = DATABSE_WORKING_DIR;
                string database_dir = Path.Combine(base_dir, _connectionstringdto.sqlite_database_path);

                string new_database_name = _connectionstringdto.new_database_name;
                string database_version = _connectionstringdto.sqlite_version;
                string db_extension = _connectionstringdto.sqlite_db_extension;
                string dbname = new_database_name + "." + db_extension + database_version;

                if (!Directory.Exists(database_dir))
                {
                    _responsedto.responsesuccessmessage += "\nsqlite datastore path with name [ " + database_dir + " ] does not exist.";
                    _responsedto.responsesuccessmessage += "\n creating path...";

                    Directory.CreateDirectory(database_dir);

                    _responsedto.responsesuccessmessage += "\ncreated sqlite datastore path with name [ " + database_dir + " ].";
                }
                else
                {
                    _responsedto.responsesuccessmessage += "\nsqlite datastore path with name [ " + database_dir + " ] exist.";
                }

                string full_database_name_with_path = Path.Combine(database_dir, dbname);
                string _secure_path_name_response = dbname;

                if (!File.Exists(full_database_name_with_path))
                {
                    _responsedto.responsesuccessmessage += "\nsqlite database with name [ " + full_database_name_with_path + " ] does not exist.";
                    _responsedto.responsesuccessmessage += "\n creating database...";

                    SQLiteConnection.CreateFile(full_database_name_with_path);

                    _responsedto.responsesuccessmessage += "\nsuccessfully created database [ " + full_database_name_with_path + " ] in sqlite.";
                }
                else
                {
                    _responsedto.responsesuccessmessage += "\nsqlite database with name [ " + full_database_name_with_path + " ] exist.";
                }

                _responsedto.isresponseresultsuccessful = true;
                return _responsedto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public responsedto createdatabase()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                string _default_db_path = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("sqlite_database_path", @"\databases\");
                string dbname = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("sqlite_database", "tasks_tracking_db_2026_april");
                string database_version = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("sqlite_version", "3");
                string db_extension = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("sqlite_db_extension", "sqlite");
                dbname = dbname + "." + db_extension + database_version;

                string base_dir = DATABSE_WORKING_DIR;

                string database_dir = Path.Combine(base_dir, _default_db_path);


                if (!Directory.Exists(database_dir))
                {
                    Directory.CreateDirectory(database_dir);
                }

                string full_database_name_with_path = Path.Combine(database_dir, dbname);
                string _secure_path_name_response = dbname;

                if (!File.Exists(full_database_name_with_path))
                {
                    SQLiteConnection.CreateFile(full_database_name_with_path);
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "successfully created database [ " + full_database_name_with_path + " ] in sqlite.";
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "sqlite datastore with name [ " + full_database_name_with_path + " ] exists.";
                    return _responsedto;
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public void initializedb()
        {
            try
            {
                createconnectionstring();
                createdatabase();
                //createtables(); 
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        public responsedto createdatabasegivennamefromconsole(string _new_database_name)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                _connectionstringdto = getsqliteconnectionstringdto();
                _connectionstringdto.database = _new_database_name;
                _connectionstringdto.new_database_name = _new_database_name;

                string base_dir = DATABSE_WORKING_DIR;

                string database_dir = Path.Combine(base_dir, _connectionstringdto.sqlite_database_path);

                string dbname = _connectionstringdto.new_database_name;
                string database_version = _connectionstringdto.sqlite_version;
                string db_extension = _connectionstringdto.sqlite_db_extension;
                dbname = dbname + "." + db_extension + database_version;

                if (!Directory.Exists(database_dir))
                {
                    _responsedto.responsesuccessmessage += "\nsqlite datastore path with name [ " + database_dir + " ] does not exist.";
                    _responsedto.responsesuccessmessage += "\n creating path...";
                    Directory.CreateDirectory(database_dir);
                    _responsedto.responsesuccessmessage += "\ncreated sqlite datastore path with name [ " + database_dir + " ].";
                }
                else
                {
                    _responsedto.responsesuccessmessage += "\nsqlite datastore path with name [ " + database_dir + " ] exist.";
                }

                string full_database_name_with_path = Path.Combine(database_dir, dbname);
                string _secure_path_name_response = dbname;

                if (!File.Exists(full_database_name_with_path))
                {
                    _responsedto.responsesuccessmessage += "\nsqlite database with name [ " + full_database_name_with_path + " ] does not exist.";
                    _responsedto.responsesuccessmessage += "\n creating database...";
                    SQLiteConnection.CreateFile(full_database_name_with_path);
                    _responsedto.responsesuccessmessage += "\nsuccessfully created database [ " + full_database_name_with_path + " ] in sqlite.";
                }
                else
                {
                    _responsedto.responsesuccessmessage += "\nsqlite database with name [ " + full_database_name_with_path + " ] exist.";
                }

                _responsedto.isresponseresultsuccessful = true;
                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }

        public responsedto createtable(string query, string CONNECTION_STRING)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                using (var con = new SQLiteConnection(CONNECTION_STRING))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                        _responsedto.isresponseresultsuccessful = true;
                        string[] _conn_arr = CONNECTION_STRING.Split(new char[] { ';' });
                        _conn_arr.SetValue("", _conn_arr.Length - 1);
                        _conn_arr.SetValue("", _conn_arr.Length - 2);
                        string _sanitized_conn_arr = _conn_arr[0] + ";" + _conn_arr[1];
                        _responsedto.responsesuccessmessage += "successfully executed query [ " + query + " ] against connection [ " + _sanitized_conn_arr + " ]." + Environment.NewLine;
                        return _responsedto;
                    }
                }
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                Exception _exception = ex.GetBaseException();
                _responsedto.responseerrormessage += Environment.NewLine + "error executing query [ " + query + " ].";
                _responsedto.responseerrormessage += Environment.NewLine + Environment.NewLine + _exception.Message + Environment.NewLine;
                return _responsedto;
            }
        }

        public responsedto createtables(sqliteconnectionstringdto _connectionstringdto)
        {
            responsedto _responsedto = new responsedto();
            responsedto _innerresponsedto = new responsedto();
            try
            {

                _connectionstringdto.database = _connectionstringdto.new_database_name;
                string CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                //Create the tasks table
                string SQL_CREATE_TASK_TABLE = "CREATE TABLE IF NOT EXISTS " +
                       DBContract.tasks_entity_table.TABLE_NAME + " (" +
                       DBContract.tasks_entity_table.TASK_ID + " INTEGER PRIMARY KEY, " +
                       DBContract.tasks_entity_table.TASK_NAME + " VARCHAR(500), " +
                       DBContract.tasks_entity_table.TASK_DESCRIPTION + " VARCHAR(500), " +
                       DBContract.tasks_entity_table.STATUS + " VARCHAR(500), " +
                       DBContract.tasks_entity_table.TASK_EXPECTED_COMPLETION_DATE + " VARCHAR(500), " +
                       DBContract.tasks_entity_table.TASK_ACTUAL_COMPLETION_DATE + " VARCHAR(500), " +
                       DBContract.tasks_entity_table.CREATED_DATE + " VARCHAR(500) " +
                       " ); ";

                _innerresponsedto = createtable(SQL_CREATE_TASK_TABLE, CONNECTION_STRING);
                if (_innerresponsedto.isresponseresultsuccessful)
                    _responsedto.responsesuccessmessage += _innerresponsedto.responsesuccessmessage;
                else
                    _responsedto.responseerrormessage += _innerresponsedto.responseerrormessage;

                //Create users table 
                string SQL_CREATE_USERS_TABLE = " CREATE TABLE IF NOT EXISTS " + DBContract.users_entity_table.TABLE_NAME + " (" +
                      DBContract.users_entity_table.ID + " INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                      DBContract.users_entity_table.EMAIL + " VARCHAR(1000), " +
                      DBContract.users_entity_table.PASS_WORD + " VARCHAR(1000), " +
                      DBContract.users_entity_table.PASSWORD_SALT + " VARCHAR(1000), " +
                      DBContract.users_entity_table.PASSWORD_HASH + " VARCHAR(1000), " +
                      DBContract.users_entity_table.FULL_NAMES + " VARCHAR(1000), " +
                      DBContract.users_entity_table.YEAR + " VARCHAR(1000), " +
                      DBContract.users_entity_table.MONTH + " VARCHAR(1000), " +
                      DBContract.users_entity_table.DAY + " VARCHAR(1000), " +
                      DBContract.users_entity_table.GENDER + " VARCHAR(1000), " +
                      DBContract.users_entity_table.STATUS + " VARCHAR(1000), " +
                      DBContract.users_entity_table.CREATED_DATE + " VARCHAR(1000) " +
                       " ); ";

                _innerresponsedto = createtable(SQL_CREATE_USERS_TABLE, CONNECTION_STRING, DBContract.users_entity_table.TABLE_NAME);
                if (_innerresponsedto.isresponseresultsuccessful)
                    _responsedto.responsesuccessmessage += _innerresponsedto.responsesuccessmessage;
                else
                    _responsedto.responseerrormessage += _innerresponsedto.responseerrormessage;

                //Create logs table 
                string SQL_CREATE_LOGS_TABLE = " CREATE TABLE IF NOT EXISTS " + DBContract.logs_entity_table.TABLE_NAME + " (" +
                      DBContract.logs_entity_table.ID + " INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                      DBContract.logs_entity_table.MESSAGE + " VARCHAR(1000), " +
                      DBContract.logs_entity_table.TIMESTAMP + " VARCHAR(1000), " +
                      DBContract.logs_entity_table.TAG + " VARCHAR(1000), " +
                      DBContract.logs_entity_table.STATUS + " VARCHAR(1000), " +
                      DBContract.logs_entity_table.CREATED_DATE + " VARCHAR(1000) " +
                       " ); ";

                _innerresponsedto = createtable(SQL_CREATE_LOGS_TABLE, CONNECTION_STRING, DBContract.logs_entity_table.TABLE_NAME);
                if (_innerresponsedto.isresponseresultsuccessful)
                    _responsedto.responsesuccessmessage += _innerresponsedto.responsesuccessmessage;
                else
                    _responsedto.responseerrormessage += _innerresponsedto.responseerrormessage;

                _responsedto.isresponseresultsuccessful = true;

                return _responsedto;

            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage += Environment.NewLine + ex.Message;
                return _responsedto;
            }
        }

        public responsedto createtable(string query, string CONNECTION_STRING, string table_name)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                using (var con = new SQLiteConnection(CONNECTION_STRING))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                        _responsedto.isresponseresultsuccessful = true;

                        _responsedto.responsesuccessmessage = "successfully created table [ " + table_name + " ] in [ " + DBContract.sqlite + " ].";

                        return _responsedto;
                    }
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                _responsedto.responseerrormessage = ex.Message;
                _responsedto.isresponseresultsuccessful = false;
                return _responsedto;
            }
        }

        bool checkiftableexists(string CONNECTION_STRING, string table_name)
        {
            try
            {
                string query = "SELECT EXISTS (SELECT name FROM sqlite_master WHERE type = @type AND name = @tableName)";

                using (var con = new SQLiteConnection(CONNECTION_STRING))
                {
                    con.Open();
                    //open a new command
                    using (var cmd = new SQLiteCommand(query, con))
                    {

                        cmd.Parameters.AddWithValue("type", "table");
                        cmd.Parameters.AddWithValue("tableName", table_name);

                        //execute the query  
                        int _rows_affected = cmd.ExecuteNonQuery();
                        //Console.WriteLine("_rows_affected [ " + _rows_affected + " ]");

                        if (_rows_affected < 0)
                        {
                            return false;
                        }

                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                return false;
            }
        }

        public int insertgeneric(string query, Dictionary<string, object> args, string CONNECTION_STRING)
        {
            int numberOfRowsAffected;
            if (CONNECTION_STRING == null)
            {
                numberOfRowsAffected = insertgeneric(query, args);
                return numberOfRowsAffected;
            }
            else if (String.IsNullOrEmpty(CONNECTION_STRING))
            {
                numberOfRowsAffected = insertgeneric(query, args);
                return numberOfRowsAffected;
            }
            else
            {

                //setup the connection to the database
                using (var con = new SQLiteConnection(CONNECTION_STRING))
                {
                    con.Open();
                    //open a new command
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        //set the arguments given in the query
                        foreach (var pair in args)
                        {
                            cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                        }
                        //execute the query and get the number of row affected
                        numberOfRowsAffected = cmd.ExecuteNonQuery();
                    }
                    return numberOfRowsAffected;
                }
            }
        }

        public int insertgeneric(string query, Dictionary<string, object> args)
        {
            int numberOfRowsAffected;
            //setup the connection to the database
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                //open a new command
                using (var cmd = new SQLiteCommand(query, con))
                {
                    //set the arguments given in the query
                    foreach (var pair in args)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }
                    //execute the query and get the number of row affected
                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }
                return numberOfRowsAffected;
            }
        }

        public int updategeneric(string query, Dictionary<string, object> args)
        {
            int numberOfRowsAffected;
            //setup the connection to the database
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                //open a new command
                using (var cmd = new SQLiteCommand(query, con))
                {
                    //set the arguments given in the query
                    foreach (var pair in args)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }
                    //execute the query and get the number of row affected
                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }
                return numberOfRowsAffected;
            }
        }

        public int deletegeneric(string query, Dictionary<string, object> args)
        {
            int numberOfRowsAffected;
            //setup the connection to the database
            using (var con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();
                //open a new command
                using (var cmd = new SqlCommand(query, con))
                {
                    //set the arguments given in the query
                    foreach (var pair in args)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }
                    //execute the query and get the number of row affected
                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }
                return numberOfRowsAffected;
            }
        }

        public DataTable getallrecordsglobal(string query)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    var da = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }

        public DataTable getallrecordsglobal(string query, string CONNECTION_STRING)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    var da = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }

        private void updatedbschema(string query)
        {
            try
            {
                //setup the connection to the database
                using (var con = new SQLiteConnection(CONNECTION_STRING))
                {
                    con.Open();
                    //open a new command
                    using (var cmd = new SQLiteCommand(query, con))
                    {
                        //execute the query  
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        public void updateexistingdbschema()
        {
            try
            {

                //update the cropsdiseases table  
                //string SQL_ALTER_CROPS_DISEASES_TABLE = " ALTER TABLE " +
                //  DBContract.cropsdiseasesentitytable.CROPS_DISEASES_TABLE_NAME +
                //      " ADD " +
                //      DBContract.cropsdiseasesentitytable.CROP_DISEASE_CATEGORY +
                //      " VARCHAR(500) ";
                //updatedbschema(SQL_ALTER_CROPS_DISEASES_TABLE);

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private DataTable ExecuteRead(string query, Dictionary<string, object> args)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    foreach (KeyValuePair<string, object> entry in args)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }
                    var da = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }
        public responsedto checksqliteconnectionstate()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                _connectionstringdto = getsqliteconnectionstringdto();

                _responsedto = checkconnectionasadmin(_connectionstringdto, _databaseutilsnotificationeventname);

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public responsedto checkconnectionasadmin(sqliteconnectionstringdto _connectionstringdto, EventHandler<notificationmessageEventArgs> databaseutilsnotificationeventname)
        {
            _databaseutilsnotificationeventname = databaseutilsnotificationeventname;
            responsedto _responsedto = new responsedto();
            try
            {
                //string base_dir = Environment.CurrentDirectory;
                //string database_dir = base_dir + _connectionstringdto.sqlite_database_path;

                string base_dir = DATABSE_WORKING_DIR;
                string database_dir = Path.Combine(base_dir, _connectionstringdto.sqlite_database_path);

                string dbname = _connectionstringdto.database;
                string database_version = _connectionstringdto.sqlite_version;
                string db_extension = _connectionstringdto.sqlite_db_extension;
                dbname = dbname + "." + db_extension + database_version;

                if (!Directory.Exists(database_dir))
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "sqlite datastore path with name [ " + database_dir + " ] does not exist.";
                    return _responsedto;
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + database_dir + " ] exist.", TAG));

                    //_databaseutilsnotificationeventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + database_dir + " ] exist.", TAG));
                }

                string full_database_name_with_path = Path.Combine(database_dir, dbname);
                string _secure_path_name_response = _connectionstringdto.sqlite_database_path + dbname;

                if (!File.Exists(full_database_name_with_path))
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "sqlite database with name [ " + full_database_name_with_path + " ] does not exist.";
                    return _responsedto;
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + full_database_name_with_path + " ] exist.", TAG));

                    //_databaseutilsnotificationeventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + full_database_name_with_path+ " ] exist.", TAG));
                }

                string CONNECTION_STRING = @"Data Source=" + full_database_name_with_path + ";" +
                "Version=" + _connectionstringdto.sqlite_version + ";" +
                "Pooling=" + _connectionstringdto.sqlite_pooling + ";" +
                "FailIfMissing=" + _connectionstringdto.sqlite_fail_if_missing;

                string query = DBContract.logs_entity_table.SELECT_ALL_QUERY;

                int count = getrecordscountgiventable(DBContract.logs_entity_table.TABLE_NAME, CONNECTION_STRING);

                if (count != -1)
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "connection to sqlite successfull. logs count [ " + count + " ].";
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "connection to sqlite failed.";
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

        public int getrecordscountgiventable(string tablename, string CONNECTION_STRING)
        {
            string query = "SELECT * FROM " + tablename;
            DataTable dt = getallrecordsglobal(query, CONNECTION_STRING);
            if (dt != null)
                return dt.Rows.Count;
            else
                return -1;
        }

        public sqliteconnectionstringdto getsqliteconnectionstringdto()
        {
            try
            {
                _connectionstringdto = new sqliteconnectionstringdto();

                _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
                _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
                _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
                _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
                _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
                _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

                return _connectionstringdto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        public List<string> get_sqlite_databases()
        {
            List<string> server_databases = new List<string>();
            try
            {
                string base_dir = DATABSE_WORKING_DIR;
                Console.WriteLine("_base_dir " + base_dir);

                //string db_path = base_dir + Path.DirectorySeparatorChar + "databases" + Path.DirectorySeparatorChar;
                string db_path = Path.Combine(base_dir, "databases");
                Console.WriteLine("db_path " + db_path);

                var dbfileNames = Directory.GetFiles(db_path, "*.sqlite3").Select(name => new FileInfo(name).FullName).ToArray();

                foreach (var db_name in dbfileNames)
                {

                    string[] _path_levels_arr = db_name.Split(Path.DirectorySeparatorChar);
                    int _file_name_index_in_path = _path_levels_arr.Length - 1;
                    Console.WriteLine("_file_name_index_in_path " + _file_name_index_in_path);
                    string _file_name_with_extension = _path_levels_arr[_file_name_index_in_path];
                    Console.WriteLine("_file_name_with_extension " + _file_name_with_extension);

                    string[] file_arr = _file_name_with_extension.Split('.');

                    string _file_name_without_extension = file_arr[0];
                    Console.WriteLine("_file_name_without_extension " + _file_name_without_extension);

                    server_databases.Add(_file_name_without_extension);

                }
                return server_databases;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return server_databases;
            }
        }

        public bool check_if_sqlite_database_exists(string database_name)
        {
            bool _exists = false;
            try
            {
                List<string> server_databases = get_sqlite_databases();
                var _recordscount = server_databases.Count;

                for (int i = 0; i < _recordscount; i++)
                {

                    var _record_from_server = server_databases[i];

                    if (database_name == _record_from_server)
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

        public DataTable execute_select_query(string query, string CONNECTION_STRING)
        {
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    var da = new SQLiteDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }

        public int execute_data_manipulation_query(string query, string CONNECTION_STRING)
        {
            int numberOfRowsAffected;
            //setup the connection to the database
            using (var con = new SQLiteConnection(CONNECTION_STRING))
            {
                con.Open();
                //open a new command
                using (var cmd = new SQLiteCommand(query, con))
                {
                    //execute the query and get the number of row affected
                    numberOfRowsAffected = cmd.ExecuteNonQuery();
                }
                return numberOfRowsAffected;
            }
        }

        public responsedto create_task_in_database(task_dto _task_dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {

                string query = "INSERT INTO " +
                 DBContract.tasks_entity_table.TABLE_NAME
                 + "(" +
                 DBContract.tasks_entity_table.TASK_NAME
                 + "," +
                 DBContract.tasks_entity_table.TASK_DESCRIPTION
                 + "," +
                 DBContract.tasks_entity_table.STATUS
                 + "," +
                 DBContract.tasks_entity_table.TASK_EXPECTED_COMPLETION_DATE
                 + "," +
                 DBContract.tasks_entity_table.TASK_ACTUAL_COMPLETION_DATE
                 + "," +
                 DBContract.tasks_entity_table.CREATED_DATE
                 + ") VALUES(@task_name, @task_description, @task_status, @task_expected_completion_date, @task_actual_completion_date, @created_date)";

                //here we are setting the parameter values that will be actually replaced in the query in Execute method
                var args = new Dictionary<string, object>
			    {
				    {"@task_name", _task_dto.name},
                    {"@task_description", _task_dto.description}, 
                    {"@task_status", _task_dto.status},
                    {"@task_expected_completion_date", _task_dto.expected_completion_date},
                    {"@task_actual_completion_date", _task_dto.actual_completion_date},
				    {"@created_date", _task_dto.created_date}
			    };

                int numberOfRowsAffected = insertgeneric(query, args, CONNECTION_STRING);
                if (numberOfRowsAffected != 1)
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "Record creation failed.";
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.sqlite;
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

        public bool check_if_task_exists(string entity_name)
        {
            bool _exists = false;
            try
            {

                string query = DBContract.tasks_entity_table.SELECT_ALL_QUERY;

                DataTable dt = getallrecordsglobal(query, CONNECTION_STRING);

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
                _exists = false;
                return _exists;
            }
        }

        public responsedto truncate_database()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                //setup the connection to the database
                using (var con = new SQLiteConnection(CONNECTION_STRING))
                {
                    con.Open();

                    foreach (var table in DBContract.table_names_arr)
                    {
                        string query = "DELETE FROM " + table + ";";

                        //open a new command
                        using (var cmd = new SQLiteCommand(query, con))
                        {
                            //execute the query  
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Tables successfully truncated in " + DBContract.sqlite;

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
        public responsedto create_log_in_database(log_dto _dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                string query = "INSERT INTO " +
                DBContract.logs_entity_table.TABLE_NAME +
                " ( " +
                DBContract.logs_entity_table.MESSAGE + ", " +
                DBContract.logs_entity_table.TIMESTAMP + ", " +
                DBContract.logs_entity_table.TAG + ", " +
                DBContract.logs_entity_table.STATUS + ", " +
                DBContract.logs_entity_table.CREATED_DATE +
                " ) VALUES(@message, @timestamp, @tag, @status, @created_date)";

                //here we are setting the parameter values that will be actually replaced in the query in Execute method
                var args = new Dictionary<string, object>
			    {
				    {"@message", _dto.message},
				    {"@timestamp", _dto.timestamp},
                    {"@tag", _dto.tag},  
                    {"@status", "active"},
				    {"@created_date", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt")}
			    };

                int numberOfRowsAffected = insertgeneric(query, args, CONNECTION_STRING);
                if (numberOfRowsAffected != 1)
                {
                    _responsedto.isresponseresultsuccessful = false;
                    //_responsedto.responseerrormessage = "Record creation failed in " + DBContract.sqlite + ".";
                    _responsedto.responseerrormessage = "Record creation failed.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    //_responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.sqlite + ".";
                    _responsedto.responsesuccessmessage = "Record created successfully.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responsesuccessmessage, TAG));
                    return _responsedto;
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        #endregion "logs"

        #region "users"
        public responsedto create_user_in_database(user_dto _dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                string query = "INSERT INTO " +
                DBContract.users_entity_table.TABLE_NAME +
                " ( " +
                DBContract.users_entity_table.EMAIL + ", " +
                DBContract.users_entity_table.PASS_WORD + ", " +
                DBContract.users_entity_table.PASSWORD_SALT + ", " +
                DBContract.users_entity_table.PASSWORD_HASH + ", " +
                DBContract.users_entity_table.FULL_NAMES + ", " +
                DBContract.users_entity_table.YEAR + ", " +
                DBContract.users_entity_table.MONTH + ", " +
                DBContract.users_entity_table.DAY + ", " +
                DBContract.users_entity_table.GENDER + ", " +
                DBContract.users_entity_table.STATUS + ", " +
                DBContract.users_entity_table.CREATED_DATE +
                " ) VALUES(@email, @password, @password_salt, @password_hash, @fullnames, @year, @month, @day, @gender, @status, @created_date)";

                //here we are setting the parameter values that will be actually replaced in the query in Execute method
                var args = new Dictionary<string, object>
			    {
				    {"@email", _dto.email},
				    {"@password", _dto.pass_word},
                    {"@password_salt", _dto.password_salt},
                    {"@password_hash", _dto.password_hash},
                    {"@fullnames", _dto.full_names},
                    {"@year", _dto.year},
                    {"@month", _dto.month},
                    {"@day", _dto.day},
                    {"@gender", _dto.gender}, 
                    {"@status", "active"},
				    {"@created_date", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss tt")}
			    };

                int numberOfRowsAffected = insertgeneric(query, args, CONNECTION_STRING);
                if (numberOfRowsAffected != 1)
                {
                    _responsedto.isresponseresultsuccessful = false;
                    _responsedto.responseerrormessage = "Record creation failed in " + DBContract.sqlite + ".";
                    //_responsedto.responseerrormessage = "Record creation failed.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.sqlite + ".";
                    //_responsedto.responsesuccessmessage = "Record created successfully.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responsesuccessmessage, TAG));
                    return _responsedto;
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public bool check_if_user_exists(string entity_name)
        {
            bool _exists = false;
            try
            {

                string query = DBContract.users_entity_table.SELECT_ALL_QUERY;

                DataTable dt = getallrecordsglobal(query, CONNECTION_STRING);

                var _recordscount = dt.Rows.Count;

                for (int i = 0; i < _recordscount; i++)
                {

                    var _record_from_server = Convert.ToString(dt.Rows[i][DBContract.users_entity_table.EMAIL]);

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
                _exists = false;
                return _exists;
            }
        }
        public user_dto get_user_by_id(int id)
        {
            try
            {
                var query = "SELECT * FROM " +
                    DBContract.users_entity_table.TABLE_NAME +
                    " WHERE " +
                    DBContract.users_entity_table.ID +
                    " = " +
                    "@id";

                var args = new Dictionary<string, object>
				{
					{"@id", id}
				};

                DataTable dt = ExecuteRead(query, args);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }

                user_dto _dto = new user_dto();
                _dto.id = Convert.ToInt64(dt.Rows[0]["id"]);
                _dto.email = Convert.ToString(dt.Rows[0]["email"]);
                _dto.pass_word = Convert.ToString(dt.Rows[0]["password"]);
                _dto.full_names = Convert.ToString(dt.Rows[0]["fullnames"]);
                _dto.year = Convert.ToString(dt.Rows[0]["year"]);
                _dto.month = Convert.ToString(dt.Rows[0]["month"]);
                _dto.day = Convert.ToString(dt.Rows[0]["day"]);
                _dto.gender = Convert.ToString(dt.Rows[0]["gender"]);
                _dto.status = Convert.ToString(dt.Rows[0]["status"]);
                _dto.created_date = Convert.ToString(dt.Rows[0]["created_date"]);

                return _dto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public user_dto get_user_by_email(string email)
        {
            try
            {

                var query = "SELECT * FROM " +
                    DBContract.users_entity_table.TABLE_NAME +
                    " WHERE " +
                    DBContract.users_entity_table.EMAIL +
                    " = " +
                    "@email";

                var args = new Dictionary<string, object>
				{
					{"@email", email}
				};

                DataTable dt = ExecuteRead(query, args);
                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }

                user_dto _dto = new user_dto();
                _dto.id = Convert.ToInt64(dt.Rows[0]["id"]);
                _dto.email = Convert.ToString(dt.Rows[0]["email"]);
                _dto.pass_word = Convert.ToString(dt.Rows[0]["password"]);
                _dto.full_names = Convert.ToString(dt.Rows[0]["fullnames"]);
                _dto.year = Convert.ToString(dt.Rows[0]["year"]);
                _dto.month = Convert.ToString(dt.Rows[0]["month"]);
                _dto.day = Convert.ToString(dt.Rows[0]["day"]);
                _dto.gender = Convert.ToString(dt.Rows[0]["gender"]);
                _dto.status = Convert.ToString(dt.Rows[0]["status"]);
                _dto.created_date = Convert.ToString(dt.Rows[0]["created_date"]);

                return _dto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public responsedto update_user_in_database(user_dto _dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {

                string query = "UPDATE " +
                DBContract.users_entity_table.TABLE_NAME +
                " SET " +
                "fullnames = @fullnames, " +
                "year = @year, " +
                "month = @month, " +
                "day = @day, " +
                "gender = @gender " +
                "WHERE id = @id";

                //here we are setting the parameter values that will be actually replaced in the query in Execute method
                var args = new Dictionary<string, object>
			    { 
                    {"@id", _dto.id},
                    {"@fullnames", _dto.full_names},
                    {"@year", _dto.year},
                    {"@month", _dto.month},
                    {"@day", _dto.day},
                    {"@gender", _dto.gender} 
			    };

                int numberOfRowsAffected = updategeneric(query, args);
                if (numberOfRowsAffected != 1)
                {
                    _responsedto.isresponseresultsuccessful = false;
                    //_responsedto.responseerrormessage = "Record update failed in " + DBContract.sqlite + ".";
                    _responsedto.responseerrormessage = "Record update failed.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "Record updated successfully.";
                    //_responsedto.responsesuccessmessage = "Record updated successfully in " + DBContract.sqlite + ".";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responsesuccessmessage, TAG));
                    return _responsedto;
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public responsedto delete_user_in_database(user_dto _dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                string query = "DELETE FROM " +
                        DBContract.users_entity_table.TABLE_NAME +
                        " WHERE " +
                        DBContract.users_entity_table.ID +
                        " = " +
                        "@id";

                //here we are setting the parameter values that will be actually replaced in the query in Execute method
                var args = new Dictionary<string, object>
				{
					{"@id", _dto.id}  
				};

                int numberOfRowsAffected = deletegeneric(query, args);
                if (numberOfRowsAffected != 1)
                {
                    _responsedto.isresponseresultsuccessful = false;
                    //_responsedto.responseerrormessage = "Record deletion failed in " + DBContract.sqlite + ".";
                    _responsedto.responseerrormessage = "Record deletion failed.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    //_responsedto.responsesuccessmessage = "Record deleted successfully in " + DBContract.sqlite + ".";
                    _responsedto.responsesuccessmessage = "Record deleted successfully.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responsesuccessmessage, TAG));
                    return _responsedto;
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public DataTable search_users_in_database(user_dto _dto)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("SELECT * FROM ");
                sb.Append(DBContract.users_entity_table.TABLE_NAME);

                //no field specified
                if (string.IsNullOrEmpty(_dto.email) && string.IsNullOrEmpty(_dto.full_names))
                {

                }
                //email only
                if (!string.IsNullOrEmpty(_dto.email) && string.IsNullOrEmpty(_dto.full_names))
                {
                    sb.Append(" WHERE ");
                    sb.Append(DBContract.users_entity_table.EMAIL);
                    sb.Append(" LIKE ");
                    sb.Append(" @email ");
                }
                //fullnames only
                if (string.IsNullOrEmpty(_dto.email) && !string.IsNullOrEmpty(_dto.full_names))
                {
                    sb.Append(" WHERE ");
                    sb.Append(DBContract.users_entity_table.FULL_NAMES);
                    sb.Append(" LIKE ");
                    sb.Append(" @fullnames ");
                }
                //email and fullnames
                if (!string.IsNullOrEmpty(_dto.email) && !string.IsNullOrEmpty(_dto.full_names))
                {
                    sb.Append(" WHERE ");
                    sb.Append(DBContract.users_entity_table.EMAIL);
                    sb.Append(" LIKE ");
                    sb.Append(" @email ");
                    sb.Append(" AND ");
                    sb.Append(DBContract.users_entity_table.FULL_NAMES);
                    sb.Append(" LIKE ");
                    sb.Append(" @fullnames ");
                }

                var query = sb.ToString();

                var args = new Dictionary<string, object>
				{
                    {"@email", _dto.email},
					{"@fullnames", _dto.full_names} 
				};

                DataTable dt = ExecuteRead(query, args);

                if (dt == null || dt.Rows.Count == 0)
                {
                    return null;
                }

                return dt;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public responsedto login(user_dto dto)
        {
            responsedto _responsedto = new responsedto();
            try
            {

                var query = "SELECT * FROM " +
                    DBContract.users_entity_table.TABLE_NAME +
                    " WHERE " +
                    DBContract.users_entity_table.EMAIL +
                    " = " +
                    "@email" +
                    " AND " +
                    DBContract.users_entity_table.PASS_WORD +
                    " = " +
                    "@password";

                var args = new Dictionary<string, object>
				{
					{"@email", dto.email},
                    {"@password", dto.pass_word}
				};

                DataTable dt = ExecuteRead(query, args);

                if (dt == null || dt.Rows.Count == 0)
                {
                    _responsedto.isresponseresultsuccessful = false;
                    //_responsedto.responseerrormessage = "Record deletion failed in " + DBContract.sqlite + ".";
                    _responsedto.responseerrormessage = "Log in failed.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    user_dto _dto = new user_dto();
                    _dto.id = Convert.ToInt64(dt.Rows[0]["id"]);
                    _dto.email = Convert.ToString(dt.Rows[0]["email"]);
                    _dto.pass_word = Convert.ToString(dt.Rows[0]["password"]);
                    _dto.full_names = Convert.ToString(dt.Rows[0]["fullnames"]);
                    _dto.year = Convert.ToString(dt.Rows[0]["year"]);
                    _dto.month = Convert.ToString(dt.Rows[0]["month"]);
                    _dto.day = Convert.ToString(dt.Rows[0]["day"]);
                    _dto.gender = Convert.ToString(dt.Rows[0]["gender"]);
                    _dto.status = Convert.ToString(dt.Rows[0]["status"]);
                    _dto.created_date = Convert.ToString(dt.Rows[0]["created_date"]);

                    _responsedto.responseresultobject = _dto;

                    _responsedto.isresponseresultsuccessful = true;
                    //_responsedto.responsesuccessmessage = "Record deleted successfully in " + DBContract.sqlite + ".";
                    _responsedto.responsesuccessmessage = "Logged in  successfully.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responsesuccessmessage, TAG));
                    return _responsedto;
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        #endregion "users"



    }
}





