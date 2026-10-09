using MySql.Data.MySqlClient;
using Npgsql;
using SMOScripting;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using console_commonlib;
using console_dto;
using console_dal;

namespace console_business
{
    public class copy_data_utils
    {
        public string TAG;
        public List<notificationdto> _lstnotificationdto = new List<notificationdto>();

        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        public event EventHandler<progressBarNotificationEventArgs> _progressBarNotificationEventname;

        public copy_data_utils(EventHandler<notificationmessageEventArgs> notificationmessageEventname, EventHandler<progressBarNotificationEventArgs> progressBarNotificationEventname)
        {
            TAG = this.GetType().Name;

            //Subscribing to the event: 
            //Dynamically:
            //EventName += HandlerName;
            _notificationmessageEventname = notificationmessageEventname;
            _progressBarNotificationEventname = progressBarNotificationEventname;
        }
        public void sync_data_from_source_to_destination(dbsyncdto _dbsyncdto)
        {
            try
            {
                string _server_from = _dbsyncdto.serverfrom;
                string _server_to = _dbsyncdto.serverto;
                string _server_connectionstring_from = "";
                string _server_connectionstring_to = "";

                switch (_server_from)
                {
                    case "mssql":
                        try
                        {

                            //use selected db in object
                            mssqlconnectionstringdto _connectionstring_from_dto = getmssqlconnectionstringdto();
                            _connectionstring_from_dto.database = _dbsyncdto.databasefrom;
                            _connectionstring_from_dto.new_database_name = _dbsyncdto.databasefrom;

                            _server_connectionstring_from = buildmssqlconnectionstringfromobject(_connectionstring_from_dto);

                            switch (_server_to)
                            {
                                case "mssql":
                                    try
                                    {

                                        //use selected db in object
                                        mssqlconnectionstringdto _connectionstring_to_dto = getmssqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildmssqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "mysql":
                                    try
                                    {
                                        //use selected db in object
                                        mysqlconnectionstringdto _connectionstring_to_dto = getmysqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildmysqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "postgresql":
                                    try
                                    {
                                        //use selected db in object
                                        postgresqlconnectionstringdto _connectionstring_to_dto = getpostgresqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildpostgresqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "sqlite":
                                    try
                                    {
                                        //use selected db in object
                                        sqliteconnectionstringdto _connectionstring_to_dto = getsqliteconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildsqliteconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                            }

                        }
                        catch (Exception ex)
                        {
                            logtodbutils(ex.Message);
                        }
                        break;
                    case "mysql":
                        try
                        {

                            //use selected db in object
                            mysqlconnectionstringdto _connectionstring_from_dto = getmysqlconnectionstringdto();
                            _connectionstring_from_dto.database = _dbsyncdto.databasefrom;
                            _connectionstring_from_dto.new_database_name = _dbsyncdto.databasefrom;

                            _server_connectionstring_from = buildmysqlconnectionstringfromobject(_connectionstring_from_dto);

                            switch (_server_to)
                            {
                                case "mssql":
                                    try
                                    {

                                        //use selected db in object
                                        mssqlconnectionstringdto _connectionstring_to_dto = getmssqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildmssqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "mysql":
                                    try
                                    {
                                        //use selected db in object
                                        mysqlconnectionstringdto _connectionstring_to_dto = getmysqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildmysqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "postgresql":
                                    try
                                    {
                                        //use selected db in object
                                        postgresqlconnectionstringdto _connectionstring_to_dto = getpostgresqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildpostgresqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "sqlite":
                                    try
                                    {
                                        //use selected db in object
                                        sqliteconnectionstringdto _connectionstring_to_dto = getsqliteconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildsqliteconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                            }

                        }
                        catch (Exception ex)
                        {
                            logtodbutils(ex.Message);
                        }
                        break;
                    case "postgresql":
                        try
                        {

                            //use selected db in object
                            postgresqlconnectionstringdto _connectionstring_from_dto = getpostgresqlconnectionstringdto();
                            _connectionstring_from_dto.database = _dbsyncdto.databasefrom;
                            _connectionstring_from_dto.new_database_name = _dbsyncdto.databasefrom;

                            _server_connectionstring_from = buildpostgresqlconnectionstringfromobject(_connectionstring_from_dto);

                            switch (_server_to)
                            {
                                case "mssql":
                                    try
                                    {

                                        //use selected db in object
                                        mssqlconnectionstringdto _connectionstring_to_dto = getmssqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildmssqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "mysql":
                                    try
                                    {
                                        //use selected db in object
                                        mysqlconnectionstringdto _connectionstring_to_dto = getmysqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildmysqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "postgresql":
                                    try
                                    {
                                        //use selected db in object
                                        postgresqlconnectionstringdto _connectionstring_to_dto = getpostgresqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildpostgresqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "sqlite":
                                    try
                                    {
                                        //use selected db in object
                                        sqliteconnectionstringdto _connectionstring_to_dto = getsqliteconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildsqliteconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                            }

                        }
                        catch (Exception ex)
                        {
                            logtodbutils(ex.Message);
                        }
                        break;
                    case "sqlite":
                        try
                        {

                            //use selected db in object
                            sqliteconnectionstringdto _connectionstring_from_dto = getsqliteconnectionstringdto();
                            _connectionstring_from_dto.database = _dbsyncdto.databasefrom;
                            _connectionstring_from_dto.new_database_name = _dbsyncdto.databasefrom;

                            _server_connectionstring_from = buildsqliteconnectionstringfromobject(_connectionstring_from_dto);

                            switch (_server_to)
                            {
                                case "mssql":
                                    try
                                    {

                                        //use selected db in object
                                        mssqlconnectionstringdto _connectionstring_to_dto = getmssqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildmssqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "mysql":
                                    try
                                    {
                                        //use selected db in object
                                        mysqlconnectionstringdto _connectionstring_to_dto = getmysqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildmysqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "postgresql":
                                    try
                                    {
                                        //use selected db in object
                                        postgresqlconnectionstringdto _connectionstring_to_dto = getpostgresqlconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildpostgresqlconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                                case "sqlite":
                                    try
                                    {
                                        //use selected db in object
                                        sqliteconnectionstringdto _connectionstring_to_dto = getsqliteconnectionstringdto();
                                        _connectionstring_to_dto.database = _dbsyncdto.databaseto;
                                        _connectionstring_to_dto.new_database_name = _dbsyncdto.databaseto;

                                        _server_connectionstring_to = buildsqliteconnectionstringfromobject(_connectionstring_to_dto);

                                        fetch_data_to_sync_for_each_entity(_server_connectionstring_to, _server_to, _server_from);

                                    }
                                    catch (Exception ex)
                                    {
                                        logtodbutils(ex.Message);
                                    }
                                    break;
                            }

                        }
                        catch (Exception ex)
                        {
                            logtodbutils(ex.Message);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                logtodbutils(ex.Message);
            }
        }

        void fetch_data_to_sync_for_each_entity(string _server_connectionstring_to, string _server_to, string _server_from)
        {

            string[] _tables = DBContract.table_names_arr;
            int _entity_count = _tables.Length;
            Console.WriteLine("_entity_count [ " + _entity_count + " ]");
            int _task_count = 0;
            string query = "";
            DataTable dt;

            foreach (var _table in _tables)
            {

                switch (_table)
                {
                    case "tbl_tasks":
                        query = DBContract.tasks_entity_table.SELECT_ALL_QUERY;

                        dt = mssqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);

                        if (dt != null && dt.Rows.Count != 0)
                        {

                            sync_datastores_given_connectionstrings(dt, _server_connectionstring_to, _server_to, _server_from, DBContract.tasks_entity_table.TABLE_NAME);

                        }
                        _task_count++;
                        break;
                    case "tbl_logs":
                        query = DBContract.logs_entity_table.SELECT_ALL_QUERY;

                        dt = mssqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);

                        if (dt != null && dt.Rows.Count != 0)
                        {

                            sync_datastores_given_connectionstrings(dt, _server_connectionstring_to, _server_to, _server_from, DBContract.logs_entity_table.TABLE_NAME);

                        }
                        _task_count++;
                        break;
                    case "tbl_users":
                        query = DBContract.users_entity_table.SELECT_ALL_QUERY;

                        dt = mssqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);

                        if (dt != null && dt.Rows.Count != 0)
                        {

                            sync_datastores_given_connectionstrings(dt, _server_connectionstring_to, _server_to, _server_from, DBContract.users_entity_table.TABLE_NAME);

                        }
                        _task_count++;
                        break;
                }

            }

            Console.WriteLine("_task_count [ " + _task_count + " ]");

        }

        void sync_datastores_given_connectionstrings(DataTable dt, string connection_string_to, string _server_to, string _server_from, string _current_table)
        {
            try
            {

                switch (_server_to)
                {
                    case "mssql":
                        try
                        {
                            push_data_to_receiving_datastore(dt, _current_table, connection_string_to, _server_to, _server_from);
                        }
                        catch (Exception ex)
                        {
                            logtodbutils(ex.Message);
                        }
                        break;
                    case "mysql":
                        try
                        {
                            push_data_to_receiving_datastore(dt, _current_table, connection_string_to, _server_to, _server_from);
                        }
                        catch (Exception ex)
                        {
                            logtodbutils(ex.Message);
                        }
                        break;
                    case "postgresql":
                        try
                        {
                            push_data_to_receiving_datastore(dt, _current_table, connection_string_to, _server_to, _server_from);
                        }
                        catch (Exception ex)
                        {
                            logtodbutils(ex.Message);
                        }
                        break;
                    case "sqlite":
                        try
                        {
                            push_data_to_receiving_datastore(dt, _current_table, connection_string_to, _server_to, _server_from);
                        }
                        catch (Exception ex)
                        {
                            logtodbutils(ex.Message);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                logtodbutils(ex.Message);
            }
        }

        void push_data_to_receiving_datastore(DataTable dt, string _current_table, string connection_string_to, string _server_to, string _server_from)
        {

            try
            {
                responsedto _responsedto;
                string[] _tables = DBContract.table_names_arr;
                int _entity_count = _tables.Length;
                Console.WriteLine("_entity_count [ " + _entity_count + " ]");
                int _task_count = 0;

                int _recordscount = 0;
                int _insert_count = 0;
                int _exists_count = 0;

                string[] connection_string = connection_string_to.Split(';');

                bool _record_exists = false;

                switch (_current_table)
                {
                    case "tbl_tasks":

                        _recordscount = dt.Rows.Count;
                        _insert_count = 0;
                        _exists_count = 0;

                        for (int i = 0; i < _recordscount; i++)
                        {
                            task_dto _task_dto = new task_dto();
                            _task_dto.id = Convert.ToInt64(dt.Rows[i]["id"]);
                            _task_dto.name = Convert.ToString(dt.Rows[i]["name"]);
                            _task_dto.description = Convert.ToString(dt.Rows[i]["description"]);
                            _task_dto.expected_completion_date = Convert.ToString(dt.Rows[i]["expected_completion_date"]);
                            _task_dto.actual_completion_date = Convert.ToString(dt.Rows[i]["actual_completion_date"]);
                            _task_dto.status = Convert.ToString(dt.Rows[i]["status"]);
                            _task_dto.created_date = Convert.ToString(dt.Rows[i]["created_date"]);

                            switch (_server_to)
                            {
                                case "sqlite":
                                    if (_server_from == _server_to) return;

                                    _record_exists = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(_task_dto.name);

                                    if (_record_exists)
                                    {
                                        logtodbutils("task with name [ " + _task_dto.name + " ] exists in " + _server_to + ".");
                                        _exists_count++;
                                        continue;
                                    }
                                    else
                                    {
                                        _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).create_task_in_database(_task_dto);

                                        if (_responsedto.isresponseresultsuccessful)
                                        {
                                            logtodbutils("successfully inserted task in " + _server_to + " db { " +
                                            Environment.NewLine + "task id: " + _task_dto.id + "," +
                                            Environment.NewLine + "task name: " + _task_dto.name + "," +
                                            Environment.NewLine + "status: " + _task_dto.status + " }.");
                                        }

                                        _progressBarNotificationEventname.Invoke(this, new progressBarNotificationEventArgs(i, _recordscount));
                                        _insert_count++;
                                    }
                                    break;
                                case "postgresql":
                                    if (_server_from == _server_to) return;

                                    _record_exists = postgresqlapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(_task_dto.name);

                                    if (_record_exists)
                                    {
                                        logtodbutils("task with name [ " + _task_dto.name + " ] exists in " + _server_to + ".");
                                        _exists_count++;
                                        continue;
                                    }
                                    else
                                    {
                                        _responsedto = postgresqlapisingleton.getInstance(_notificationmessageEventname).create_task_in_database(_task_dto);

                                        if (_responsedto.isresponseresultsuccessful)
                                        {
                                            logtodbutils("successfully inserted task in " + _server_to + " db { " +
                                                Environment.NewLine + "task id: " + _task_dto.id + "," +
                                                Environment.NewLine + "task name: " + _task_dto.name + "," +
                                                Environment.NewLine + "status: " + _task_dto.status + " }.");
                                        }

                                        _progressBarNotificationEventname.Invoke(this, new progressBarNotificationEventArgs(i, _recordscount));
                                        _insert_count++;
                                    }
                                    break;
                                case "mysql":
                                    if (_server_from == _server_to) return;

                                    _record_exists = mysqlapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(_task_dto.name);

                                    if (_record_exists)
                                    {
                                        logtodbutils("task with name [ " + _task_dto.name + " ] exists in " + _server_to + ".");
                                        _exists_count++;
                                        continue;
                                    }
                                    else
                                    {
                                        _responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).create_task_in_database(_task_dto);

                                        if (_responsedto.isresponseresultsuccessful)
                                        {
                                            logtodbutils("successfully inserted task in " + _server_to + " db { " +
                                            Environment.NewLine + "task id: " + _task_dto.id + "," +
                                            Environment.NewLine + "task name: " + _task_dto.name + "," +
                                            Environment.NewLine + "status: " + _task_dto.status + " }.");
                                        }

                                        _progressBarNotificationEventname.Invoke(this, new progressBarNotificationEventArgs(i, _recordscount));
                                        _insert_count++;
                                    }
                                    break;
                                case "mssql":
                                    if (_server_from == _server_to) return;

                                    _record_exists = mssqlapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(_task_dto.name);

                                    if (_record_exists)
                                    {
                                        logtodbutils("task with name [ " + _task_dto.name + " ] exists in " + _server_to + ".");
                                        _exists_count++;
                                        continue;
                                    }
                                    else
                                    {

                                        _responsedto = mssqlapisingleton.getInstance(_notificationmessageEventname).create_task_in_database(_task_dto);

                                        if (_responsedto.isresponseresultsuccessful)
                                        {
                                            logtodbutils("successfully inserted task in " + _server_to + " db { " +
                                            Environment.NewLine + "task id: " + _task_dto.id + "," +
                                            Environment.NewLine + "task name: " + _task_dto.name + "," +
                                            Environment.NewLine + "status: " + _task_dto.status + " }.");
                                        }

                                        _progressBarNotificationEventname.Invoke(this, new progressBarNotificationEventArgs(i, _recordscount));
                                        _insert_count++;

                                    }
                                    break;
                            }

                        }

                        logtodbutils("[ " + _recordscount + " ] tasks found.");

                        logtodbutils("[ " + _exists_count + " ] tasks exists.");

                        logtodbutils("[ " + _insert_count + " ] tasks inserted.");

                        _task_count++;
                        break;
                }

                Console.WriteLine("_task_count [ " + _task_count + " ]");

            }
            catch (Exception ex)
            {
                logtodbutils(ex.Message);
            }
        }

        public mssqlconnectionstringdto getmssqlconnectionstringdto()
        {
            try
            {
                mssqlconnectionstringdto _connectionstringdto = new mssqlconnectionstringdto();

                _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mssql_datasource"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mssql_database"];
                _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mssql_userid"];
                _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mssql_password"];
                _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mssql_port"];

                return _connectionstringdto;

            }
            catch (Exception ex)
            {
                logtodbutils(ex.Message);
                return null;
            }
        }

        public string buildmssqlconnectionstringfromobject(mssqlconnectionstringdto _connectionstringdto)
        {
            string CONNECTION_STRING = @"Data Source=" + _connectionstringdto.datasource + ";" +
                                        "Database=" + _connectionstringdto.database + ";" +
                                        "User Id=" + _connectionstringdto.userid + ";" +
                                        "Password=" + _connectionstringdto.password;
            return CONNECTION_STRING;
        }

        public mysqlconnectionstringdto getmysqlconnectionstringdto()
        {
            try
            {
                mysqlconnectionstringdto _connectionstringdto = new mysqlconnectionstringdto();

                _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mysql_datasource"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mysql_database"];
                _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mysql_userid"];
                _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mysql_password"];
                _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mysql_port"];

                return _connectionstringdto;

            }
            catch (Exception ex)
            {
                logtodbutils(ex.Message);
                return null;
            }
        }
        public string buildmysqlconnectionstringfromobject(mysqlconnectionstringdto _connectionstringdto)
        {
            string CONNECTION_STRING = @"host=" + _connectionstringdto.datasource + ";" +
                "database=" + _connectionstringdto.database + ";" +
                "port=" + _connectionstringdto.port + ";" +
                "user=" + _connectionstringdto.userid + ";" +
                "password=" + _connectionstringdto.password;
            return CONNECTION_STRING;
        }

        public postgresqlconnectionstringdto getpostgresqlconnectionstringdto()
        {
            try
            {
                postgresqlconnectionstringdto _connectionstringdto = new postgresqlconnectionstringdto();

                _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["postgresql_datasource"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["postgresql_database"];
                _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["postgresql_userid"];
                _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["postgresql_password"];
                _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["postgresql_port"];

                return _connectionstringdto;

            }
            catch (Exception ex)
            {
                logtodbutils(ex.Message);
                return null;
            }
        }
        public string buildpostgresqlconnectionstringfromobject(postgresqlconnectionstringdto _connectionstringdto)
        {
            string CONNECTION_STRING = @"Server=" + _connectionstringdto.datasource + ";" +
                "Database=" + _connectionstringdto.database + ";" +
                "Port=" + _connectionstringdto.port + ";" +
                "User Id=" + _connectionstringdto.userid + ";" +
                "Password=" + _connectionstringdto.password;
            return CONNECTION_STRING;
        }
        public sqliteconnectionstringdto getsqliteconnectionstringdto()
        {
            try
            {
                sqliteconnectionstringdto _connectionstringdto = new sqliteconnectionstringdto();

                _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["APP_NAME"];
                _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
                _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
                _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
                _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

                return _connectionstringdto;

            }
            catch (Exception ex)
            {
                logtodbutils(ex.Message);
                return null;
            }
        }
        string DATABSE_WORKING_DIR = Path.GetTempPath();
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
        public string buildsqliteconnectionstringfromobject(sqliteconnectionstringdto _connectionstringdto)
        {

            string base_dir = DATABSE_WORKING_DIR;
            string database_dir = Path.Combine(base_dir, _connectionstringdto.sqlite_database_path);

            string plain_dbname = _connectionstringdto.database;
            string database_version = _connectionstringdto.sqlite_version;
            string db_extension = _connectionstringdto.sqlite_db_extension;
            string dbname = plain_dbname + "." + db_extension + database_version;

            if (!Directory.Exists(database_dir))
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + database_dir + " ] does not exist.", TAG));
            }
            else
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite datastore path with name [ " + _connectionstringdto.sqlite_database_path + " ] exist.", TAG));
            }

            string full_database_name_with_path = Path.Combine(database_dir, dbname);
            string _secure_path_name_response = dbname;

            if (!File.Exists(full_database_name_with_path))
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + _secure_path_name_response + " ] does not exist.", TAG));
            }
            else
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite database with name [ " + _secure_path_name_response + " ] exist.", TAG));
            }

            string CONNECTION_STRING = @"Data Source=" + full_database_name_with_path + ";" +
            "Version=" + _connectionstringdto.sqlite_version + ";" +
            "Pooling=" + _connectionstringdto.sqlite_pooling + ";" +
            "FailIfMissing=" + _connectionstringdto.sqlite_fail_if_missing;

            return CONNECTION_STRING;
        }
        public string getmysqlconnectionstring()
        {
            try
            {
                mysqlconnectionstringdto _connectionstringdto = new mysqlconnectionstringdto();

                _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mysql_datasource"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mysql_database"];
                _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mysql_userid"];
                _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mysql_password"];
                _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mysql_port"];

                string CONNECTION_STRING = buildmysqlconnectionstringfromobject(_connectionstringdto);

                return CONNECTION_STRING;

            }
            catch (Exception ex)
            {
                logtodbutils(ex.Message);
                return null;
            }
        }

        void logtodbutils(string message)
        {
            try
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(message, TAG));
            }
            catch (Exception ex)
            {
                logtodbutils(ex.Message);
            }
        }
        public DataTable getmysqldatabasesfromschema(string query, string CONNECTION_STRING)
        {
            if (string.IsNullOrEmpty(query.Trim())) return null;

            using (var con = new MySqlConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new MySqlCommand(query, con))
                {
                    var da = new MySqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }
        public string getpostgresqlconnectionstring()
        {
            try
            {
                postgresqlconnectionstringdto _connectionstringdto = new postgresqlconnectionstringdto();

                _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["postgresql_datasource"];
                _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["postgresql_database"];
                _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["postgresql_userid"];
                _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["postgresql_password"];
                _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["postgresql_port"];

                string CONNECTION_STRING = buildpostgresqlconnectionstringfromobject(_connectionstringdto);

                return CONNECTION_STRING;

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        public DataTable getpostgresqldatabasesfromschema(string query, string CONNECTION_STRING)
        {
            if (string.IsNullOrEmpty(query.Trim()))
                return null;
            using (var con = new NpgsqlConnection(CONNECTION_STRING))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(query, con))
                {
                    var da = new NpgsqlDataAdapter(cmd);
                    var dt = new DataTable();
                    da.Fill(dt);
                    da.Dispose();
                    return dt;
                }
            }
        }
        public List<string> get_mssql_databases()
        {
            smoapi _smoapi = new smoapi();
            List<string> server_databases = new List<string>();
            server_databases = _smoapi.getdatabases();
            return server_databases;
        }
        public List<string> get_mysql_databases()
        {
            List<string> server_databases = new List<string>();

            string CONNECTION_STRING = getmysqlconnectionstring();

            string query = "SHOW databases";

            DataTable dt = getmysqldatabasesfromschema(query, CONNECTION_STRING);

            var _recordscount = dt.Rows.Count;

            for (int i = 0; i < _recordscount; i++)
            {

                server_databases.Add(Convert.ToString(dt.Rows[i]["Database"]));
            }
            return server_databases;
        }
        public List<string> get_postgresql_databases()
        {
            List<string> server_databases = new List<string>();

            string CONNECTION_STRING = getpostgresqlconnectionstring();

            string query = "SELECT * FROM pg_catalog.pg_database";

            DataTable dt = getpostgresqldatabasesfromschema(query, CONNECTION_STRING);

            var _recordscount = dt.Rows.Count;

            for (int i = 0; i < _recordscount; i++)
            {

                server_databases.Add(Convert.ToString(dt.Rows[i]["Database"]));
            }
            return server_databases;
        }
        public List<string> get_sqlite_databases()
        {
            List<string> server_databases = new List<string>();

            string _base_dir = DATABSE_WORKING_DIR;

            Console.WriteLine("_base_dir " + _base_dir);

            string db_path = Path.Combine(_base_dir, "databases");

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




    }
}
