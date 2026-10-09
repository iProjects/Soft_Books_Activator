/*
 * Created by SharpDevelop.
 * User: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 09/09/2018
 * Time: 20:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SMOScripting;
using console_commonlib;
using console_dto;
using System.Text;

namespace console_dal
{
    /// <summary>
    /// Description of mssqlapisingleton.
    /// </summary>
    public sealed class mssqlapisingleton
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static mssqlapisingleton singleInstance;

        public static mssqlapisingleton getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new mssqlapisingleton(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        private string CONNECTION_STRING = @"Data Source=.\SQLEXPRESS;Database=tasks_tracking_db_2026_april;User Id=sa;Password=123456789";
        private const string db_name = "tasks_tracking_db_2026_april";//DBContract.DATABASE_NAME;
        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        private string TAG;
        mssqlconnectionstringdto _connectionstringdto = new mssqlconnectionstringdto();

        private mssqlapisingleton(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            _notificationmessageEventname = notificationmessageEventname;
            try
            {
                TAG = this.GetType().Name;
                setconnectionstring();
                createdatabase();
                createtables();
                //updateexistingdbschema();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        private mssqlapisingleton()
        {

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

        private responsedto createdatabaseonfirstload()
        {
            _connectionstringdto = new mssqlconnectionstringdto();

            _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mssql_datasource"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mssql_database"];
            _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mssql_userid"];
            _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mssql_password"];
            _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mssql_port"];

            responsedto _responsedto = create_database_given_name(_connectionstringdto);
            return _responsedto;
        }

        private responsedto createtablesonfirstload()
        {
            _connectionstringdto = new mssqlconnectionstringdto();

            _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mssql_datasource"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mssql_database"];
            _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mssql_userid"];
            _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mssql_password"];
            _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mssql_port"];

            responsedto _responsedto = create_tables(_connectionstringdto);
            return _responsedto;
        }

        private void setconnectionstring()
        {
            try
            {
                _connectionstringdto = new mssqlconnectionstringdto();

                _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mssql_datasource"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mssql_database"];
                _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mssql_userid"];
                _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mssql_password"];
                _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mssql_port"];

                CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        string buildconnectionstringfromobject(mssqlconnectionstringdto _connectionstringdto)
        {
            string CONNECTION_STRING = @"Data Source=" + _connectionstringdto.datasource + ";" +
            "Database=" + _connectionstringdto.database + ";" +
            "User Id=" + _connectionstringdto.userid + ";" +
            "Password=" + _connectionstringdto.password;
            return CONNECTION_STRING;
        }

        private mssqlconnectionstringdto get_connection_string_dto()
        {
            try
            {
                _connectionstringdto = new mssqlconnectionstringdto();

                _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mssql_datasource"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mssql_database"];
                _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["mssql_database"];
                _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mssql_userid"];
                _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mssql_password"];
                _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mssql_port"];

                CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                return _connectionstringdto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public responsedto create_database_given_name(mssqlconnectionstringdto _connectionstringdto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                _connectionstringdto.new_database_name = _connectionstringdto.database;
                _connectionstringdto.database = "master";

                string CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                string query = "CREATE DATABASE " + _connectionstringdto.new_database_name + ";";

                using (var con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                        _responsedto.isresponseresultsuccessful = true;
                        _responsedto.responsesuccessmessage = "successfully created database [ " + _connectionstringdto.new_database_name + " ] in " + DBContract.mssql + ".";
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responsesuccessmessage, TAG));
                        _responsedto.responseresultobject = _connectionstringdto;
                        return _responsedto;
                    }
                }
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return _responsedto;
            }
        }
        public responsedto createdatabasegivenname(mssqlconnectionstringdto _connectionstringdto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                _connectionstringdto.database = "master";

                string CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                string query = "CREATE DATABASE " + _connectionstringdto.new_database_name + ";";

                using (var con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                        _responsedto.isresponseresultsuccessful = true;
                        _responsedto.responsesuccessmessage = "successfully created database [ " + _connectionstringdto.new_database_name + " ] in " + DBContract.mssql + ".";
                        _responsedto.responseresultobject = _connectionstringdto;
                        return _responsedto;
                    }
                }
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public responsedto createdatabasegivennamefromconsole(string new_database_name)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                _connectionstringdto = getmssqlconnectionstringdto();
                _connectionstringdto.database = "master";

                string CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                string query = "CREATE DATABASE " + new_database_name + ";";

                using (var con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                        _responsedto.isresponseresultsuccessful = true;
                        _responsedto.responsesuccessmessage = "successfully created database [ " + new_database_name + " ] in " + DBContract.mssql + ".";
                        _responsedto.responseresultobject = _connectionstringdto;
                        return _responsedto;
                    }
                }
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }

        public bool createdatabase()
        {
            try
            {
                _connectionstringdto = getmssqlconnectionstringdto();
                _connectionstringdto.database = "master";

                string CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                string _default_database_name = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("mssql_database", "tasks_tracking_db_2026_april");

                string query = "CREATE DATABASE " + _default_database_name + ";";

                using (var con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(query, con))
                    {
                        cmd.ExecuteNonQuery();
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("successfully created database [ " + _default_database_name + " ] in " + DBContract.mssql + ".", TAG));
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return false;
            }
        }

        public void createtables()
        {
            responsedto _responsedto = create_tables(get_connection_string_dto());
            //    if (_responsedto.isresponseresultsuccessful)
            //        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responsesuccessmessage, TAG));
            //    else
            //        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
        }

        public responsedto create_tables(mssqlconnectionstringdto _connectionstringdto)
        {
            responsedto _responsedto = new responsedto();
            responsedto _innerresponsedto = new responsedto();
            try
            {

                _connectionstringdto.database = _connectionstringdto.new_database_name;
                string CONNECTION_STRING = buildconnectionstringfromobject(_connectionstringdto);

                //Create the tasks table 
                string SQL_CREATE_TASK_TABLE = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + DBContract.tasks_entity_table.TABLE_NAME + "]') AND type in (N'U'))";
                SQL_CREATE_TASK_TABLE += " BEGIN ";
                SQL_CREATE_TASK_TABLE += " CREATE TABLE " +
                       DBContract.tasks_entity_table.TABLE_NAME + " (" +
                       DBContract.tasks_entity_table.TASK_ID + " INT IDENTITY(1,1) PRIMARY KEY NOT NULL, " +
                       DBContract.tasks_entity_table.TASK_NAME + " VARCHAR(MAX), " +
                       DBContract.tasks_entity_table.TASK_DESCRIPTION + " VARCHAR(MAX), " +
                       DBContract.tasks_entity_table.STATUS + " VARCHAR(MAX), " +
                       DBContract.tasks_entity_table.TASK_EXPECTED_COMPLETION_DATE + " VARCHAR(MAX), " +
                       DBContract.tasks_entity_table.TASK_ACTUAL_COMPLETION_DATE + " VARCHAR(MAX), " +
                       DBContract.tasks_entity_table.CREATED_DATE + " VARCHAR(MAX) " +
                      " )";
                SQL_CREATE_TASK_TABLE += "END";

                _innerresponsedto = createtable(SQL_CREATE_TASK_TABLE, CONNECTION_STRING, DBContract.tasks_entity_table.TABLE_NAME);
                if (_innerresponsedto.isresponseresultsuccessful)
                    _responsedto.responsesuccessmessage += _innerresponsedto.responsesuccessmessage;
                else
                    _responsedto.responseerrormessage += _innerresponsedto.responseerrormessage;

                //Create users table 
                string SQL_CREATE_USERS_TABLE = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + DBContract.users_entity_table.TABLE_NAME + "]') AND type in (N'U'))";
                SQL_CREATE_USERS_TABLE += " BEGIN ";
                SQL_CREATE_USERS_TABLE += " CREATE TABLE " + DBContract.users_entity_table.TABLE_NAME + " (" +
                      DBContract.users_entity_table.ID + " INT IDENTITY(1,1) PRIMARY KEY NOT NULL, " +
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
                SQL_CREATE_USERS_TABLE += "END";

                _innerresponsedto = createtable(SQL_CREATE_USERS_TABLE, CONNECTION_STRING, DBContract.users_entity_table.TABLE_NAME);
                if (_innerresponsedto.isresponseresultsuccessful)
                    _responsedto.responsesuccessmessage += _innerresponsedto.responsesuccessmessage;
                else
                    _responsedto.responseerrormessage += _innerresponsedto.responseerrormessage;

                //Create logs table 
                string SQL_CREATE_LOGS_TABLE = "IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + DBContract.logs_entity_table.TABLE_NAME + "]') AND type in (N'U'))";
                SQL_CREATE_LOGS_TABLE += " BEGIN ";
                SQL_CREATE_LOGS_TABLE += " CREATE TABLE " + DBContract.logs_entity_table.TABLE_NAME + " (" +
                      DBContract.logs_entity_table.ID + " INT IDENTITY(1,1) PRIMARY KEY NOT NULL, " +
                      DBContract.logs_entity_table.MESSAGE + " VARCHAR(1000), " +
                      DBContract.logs_entity_table.TIMESTAMP + " VARCHAR(1000), " +
                      DBContract.logs_entity_table.TAG + " VARCHAR(1000), " +
                      DBContract.logs_entity_table.STATUS + " VARCHAR(1000), " +
                      DBContract.logs_entity_table.CREATED_DATE + " VARCHAR(1000) " +
                       " ); ";
                SQL_CREATE_LOGS_TABLE += "END";

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
                //setup the connection to the database
                using (var con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();
                    //open a new command
                    using (var cmd = new SqlCommand(query, con))
                    {
                        //execute the query  
                        int _rows_affected = cmd.ExecuteNonQuery();
                        //Console.WriteLine("_rows_affected [ " + _rows_affected + " ]");

                        _responsedto.isresponseresultsuccessful = true;

                        _responsedto.responsesuccessmessage = "successfully created table [ " + table_name + " ] in [ " + DBContract.mssql + " ].";

                        return _responsedto;
                    }
                }
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                Exception _exception = ex.GetBaseException();
                //_responsedto.responseerrormessage += Environment.NewLine + "error executing query [ " + query + " ].";
                _responsedto.responseerrormessage += Environment.NewLine + Environment.NewLine + _exception.Message + Environment.NewLine;
                return _responsedto;
            }
        }

        bool checkiftableexists(string CONNECTION_STRING, string table_name)
        {
            try
            {
                string query = "SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ @tableName ]') AND type in (N'U')";

                using (var con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();
                    //open a new command
                    using (var cmd = new SqlCommand(query, con))
                    {
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
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return false;
            }
        }

        public bool isdbconnectionalive()
        {
            try
            {
                //setup the connection to the database
                var con = new SqlConnection(CONNECTION_STRING);
                con.Open();
                return true;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return false;
            }

        }

        public responsedto checkmssqlconnectionstate()
        {
            responsedto _responsedto = new responsedto();
            try
            {
                _connectionstringdto = getmssqlconnectionstringdto();

                _responsedto = checkconnectionasadmin(_connectionstringdto);

                return _responsedto;
            }
            catch (Exception ex)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responseerrormessage = ex.Message;
                return _responsedto;
            }
        }
        public responsedto checkconnectionasadmin(mssqlconnectionstringdto _connectionstringdto)
        {
            responsedto _responsedto = new responsedto();
            try
            {
                string CONNECTION_STRING = @"Data Source=" + _connectionstringdto.datasource + ";" +
                "Database=" + _connectionstringdto.database + ";" +
                "User Id=" + _connectionstringdto.userid + ";" +
                "Password=" + _connectionstringdto.password;

                string query = DBContract.logs_entity_table.SELECT_ALL_QUERY;

                int count = getrecordscountgiventable(DBContract.logs_entity_table.TABLE_NAME, CONNECTION_STRING);

                if (count != -1)
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "connection to mssql successfull. logs count [ " + count + " ]";
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responseerrormessage = "connection to mssql failed.";
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

        public int insertgeneric(string query, Dictionary<string, object> args)
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
        }

        public DataTable getallrecordsglobal(string query)
        {
            if (!isdbconnectionalive()) return null;

            if (string.IsNullOrEmpty(query.Trim()))
                return null;
            using (var con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }

        public DataTable getallrecordsglobal(string query, string CONNECTION_STRING)
        {
            if (!isdbconnectionalive(CONNECTION_STRING)) return null;

            if (string.IsNullOrEmpty(query.Trim()))
                return null;
            using (var con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }

        public int updategeneric(string query, Dictionary<string, object> args)
        {
            if (!isdbconnectionalive()) return 0;

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

        public bool isdbconnectionalive(string CONNECTION_STRING)
        {
            var con = new SqlConnection(CONNECTION_STRING);
            con.Open();
            return true;
        }

        private void updatedbschema(string query)
        {
            try
            {
                //setup the connection to the database
                using (var con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();
                    //open a new command
                    using (var cmd = new SqlCommand(query, con))
                    {
                        //execute the query  
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                //				msgboxform.Show(ex.Message, TAG, "OK", msgtype.error);	 
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        public void updateexistingdbschema()
        {
            try
            {

                /*//update the tables 
                string SQL_ALTER_CROPS_DISEASES_TABLE = "IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + DBContract.cropsdiseasesentitytable.CROPS_DISEASES_TABLE_NAME + "]') AND type in (N'U'))";
                SQL_ALTER_CROPS_DISEASES_TABLE += " BEGIN ";
                SQL_ALTER_CROPS_DISEASES_TABLE += " ALTER TABLE " + 
                  DBContract.cropsdiseasesentitytable.CROPS_DISEASES_TABLE_NAME +
                      " ADD " +
                      DBContract.cropsdiseasesentitytable.CROP_DISEASE_CATEGORY + 
                      " VARCHAR(MAX) ";
                SQL_ALTER_CROPS_DISEASES_TABLE += "END";
                updatedbschema(SQL_ALTER_CROPS_DISEASES_TABLE);*/

                //responsedto _responsedto = updatemssqlschemasingleton.getInstance(_notificationmessageEventname, _progressBarNotificationEventname).updateschemagivendatastore();

                //this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responsesuccessmessage, TAG));

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
            using (var con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    foreach (KeyValuePair<string, object> entry in args)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }

        private DataTable ExecuteRead(string query, Dictionary<string, object> args, string CONNECTION_STRING)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;
            using (var con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    foreach (KeyValuePair<string, object> entry in args)
                    {
                        cmd.Parameters.AddWithValue(entry.Key, entry.Value);
                    }
                    var da = new SqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
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

        public mssqlconnectionstringdto getmssqlconnectionstringdto()
        {
            try
            {
                _connectionstringdto = new mssqlconnectionstringdto();

                _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mssql_datasource"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mssql_database"];
                _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mssql_userid"];
                _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mssql_password"];
                _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mssql_port"];

                return _connectionstringdto;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        public List<string> get_mssql_databases()
        {
            List<string> server_databases = new List<string>();
            try
            {
                smoapi _smoapi = new smoapi();
                server_databases = _smoapi.getdatabases();
                return server_databases;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return server_databases;
            }
        }

        public bool check_if_mssql_database_exists(string database_name)
        {
            bool _exists = false;
            try
            {
                List<string> server_databases = get_mssql_databases();
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
            using (var con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new SqlCommand(query, con))
                {
                    var da = new SqlDataAdapter(cmd);
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
            using (var con = new SqlConnection(CONNECTION_STRING))
            {
                con.Open();
                //open a new command
                using (var cmd = new SqlCommand(query, con))
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
                    _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mssql;
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
                using (var con = new SqlConnection(CONNECTION_STRING))
                {
                    con.Open();

                    foreach (var table in DBContract.table_names_arr)
                    {
                        string query = "TRUNCATE TABLE " + table + ";";

                        //open a new command
                        using (var cmd = new SqlCommand(query, con))
                        {
                            //execute the query  
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "Tables successfully truncated in " + DBContract.mssql;

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
                    //_responsedto.responseerrormessage = "Record creation failed in " + DBContract.mssql + ".";
                    _responsedto.responseerrormessage = "Record creation failed.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    //_responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mssql + ".";
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
                    _responsedto.responseerrormessage = "Record creation failed in " + DBContract.mssql + ".";
                    //_responsedto.responseerrormessage = "Record creation failed.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "Record created successfully in " + DBContract.mssql + ".";
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
                    //_responsedto.responseerrormessage = "Record update failed in " + DBContract.mssql + ".";
                    _responsedto.responseerrormessage = "Record update failed.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    _responsedto.responsesuccessmessage = "Record updated successfully.";
                    //_responsedto.responsesuccessmessage = "Record updated successfully in " + DBContract.mssql + ".";
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
                    //_responsedto.responseerrormessage = "Record deletion failed in " + DBContract.mssql + ".";
                    _responsedto.responseerrormessage = "Record deletion failed.";
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_responsedto.responseerrormessage, TAG));
                    return _responsedto;
                }
                else
                {
                    _responsedto.isresponseresultsuccessful = true;
                    //_responsedto.responsesuccessmessage = "Record deleted successfully in " + DBContract.mssql + ".";
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
                    //_responsedto.responseerrormessage = "Record deletion failed in " + DBContract.mssql + ".";
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
                    //_responsedto.responsesuccessmessage = "Record deleted successfully in " + DBContract.mssql + ".";
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
