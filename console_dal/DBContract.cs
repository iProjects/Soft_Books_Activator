/*
* Created by: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
* Date: 03/23/2020
* Time: 02:55
*/
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using console_commonlib;
using console_dto;

namespace console_dal
{
    /// <summary>
    /// Description of DBContract.
    /// </summary>
    //Define the tables and columns of your local database
    public static class DBContract
    {

        public static String DATABASE_NAME = "tasks_tracking_db_2026_april";
        public static String SQLITE_DATABASE_NAME = "tasks_tracking_db_2026_april.sqlite3";

        public static String error = "error";
        public static String info = "info";
        public static String warn = "warn";

        public static String mssql = "mssql";
        public static String mysql = "mysql";
        public static String sqlite = "sqlite";
        public static String postgresql = "postgresql";
        public static String mongodb = "mongodb";

        public static DialogResult dialogresult = DialogResult.OK;

        //tasks
        public static class tasks_entity_table
        {
            public const String TABLE_NAME = "tbl_tasks";
            //Columns of the table
            public static String TASK_ID = "id";
            public static String TASK_NAME = "name";
            public static String TASK_DESCRIPTION = "description";
            public static String TASK_EXPECTED_COMPLETION_DATE = "expected_completion_date";
            public static String TASK_ACTUAL_COMPLETION_DATE = "actual_completion_date";
            public static String STATUS = "status";
            public static String CREATED_DATE = "created_date";

            public static String SELECT_ALL_QUERY = "SELECT * FROM " +
                                        DBContract.tasks_entity_table.TABLE_NAME;

            public static String SELECT_ALL_FILTER_QUERY = "SELECT * FROM " +
                                DBContract.tasks_entity_table.TABLE_NAME +
                                " WHERE " +
                                DBContract.tasks_entity_table.STATUS +
                                " = " +
                                "'Not Complete'";

        }

        //users table
        public static class users_entity_table
        {
            public static String TABLE_NAME = "tbl_users";
            //Columns of the table
            public static String ID = "id";
            public static String EMAIL = "email";
            public static String PASS_WORD = "pass_word";
            public static String FULL_NAMES = "full_names";
            public static String SECRET_WORD = "secret_word";
            public static String DOB = "dob";
            public static String YEAR = "year";
            public static String MONTH = "month";
            public static String DAY = "day";
            public static String PASSWORD_SALT = "password_salt";
            public static String PASSWORD_HASH = "password_hash";
            public static String GENDER = "gender";
            public static String STATUS = "status";
            public static String CREATED_DATE = "created_date";

            //query
            public static String SELECT_ALL_QUERY = "SELECT * FROM " +
                                        DBContract.users_entity_table.TABLE_NAME;

            public static String SELECT_ALL_FILTER_QUERY = "SELECT * FROM " +
                                DBContract.users_entity_table.TABLE_NAME +
                                " where " +
                                DBContract.users_entity_table.STATUS +
                                " = " +
                                "'active'";

        }

        //logs table
        public static class logs_entity_table
        {
            public static String TABLE_NAME = "tbl_logs";
            //Columns of the table
            public static String ID = "id";
            public static String MESSAGE = "message";
            public static String TIMESTAMP = "timestamp";
            public static String TAG = "tag";
            public static String STATUS = "status";
            public static String CREATED_DATE = "created_date";

            //users query
            public static String SELECT_ALL_QUERY = "SELECT * FROM " +
                                        DBContract.logs_entity_table.TABLE_NAME;

            public static String SELECT_ALL_FILTER_QUERY = "SELECT * FROM " +
                                DBContract.logs_entity_table.TABLE_NAME +
                                " where " +
                                DBContract.logs_entity_table.STATUS +
                                " = " +
                                "'active'";

        }

        public static string getdefaultmssqlconnectionstring()
        {
            mssqlconnectionstringdto _connectionstringdto = new mssqlconnectionstringdto();

            _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mssql_datasource"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mssql_database"];
            _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mssql_userid"];
            _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mssql_password"];
            _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mssql_port"];
            _connectionstringdto.update_schema_temp_db = System.Configuration.ConfigurationManager.AppSettings["update_schema_temp_db"];
            _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["update_schema_temp_db"];
            _connectionstringdto.update_schema_current_db = System.Configuration.ConfigurationManager.AppSettings["mssql_database"];

            string CONNECTION_STRING = buildmssqlconnectionstringfromobject(_connectionstringdto);

            return CONNECTION_STRING;
        }

        static string buildmssqlconnectionstringfromobject(mssqlconnectionstringdto _connectionstringdto)
        {
            string CONNECTION_STRING = @"Data Source=" + _connectionstringdto.datasource + ";" +
            "Database=" + _connectionstringdto.database + ";" +
            "User Id=" + _connectionstringdto.userid + ";" +
            "Password=" + _connectionstringdto.password;
            return CONNECTION_STRING;
        }

        public static string getdefaultmysqlconnectionstring()
        {
            mysqlconnectionstringdto _connectionstringdto = new mysqlconnectionstringdto();

            _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mysql_datasource"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mysql_database"];
            _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mysql_userid"];
            _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mysql_password"];
            _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mysql_port"];
            _connectionstringdto.update_schema_temp_db = System.Configuration.ConfigurationManager.AppSettings["update_schema_temp_db"];
            _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["update_schema_temp_db"];
            _connectionstringdto.update_schema_current_db = System.Configuration.ConfigurationManager.AppSettings["mysql_database"];

            string CONNECTION_STRING = buildmysqlconnectionstringfromobject(_connectionstringdto);

            return CONNECTION_STRING;
        }

        static string buildmysqlconnectionstringfromobject(mysqlconnectionstringdto _connectionstringdto)
        {
            string CONNECTION_STRING = @"host=" + _connectionstringdto.datasource + ";" +
                "database=" + _connectionstringdto.database + ";" +
                "port=" + _connectionstringdto.port + ";" +
                "user=" + _connectionstringdto.userid + ";" +
                "password=" + _connectionstringdto.password;
            return CONNECTION_STRING;
        }

        public static string getdefaultpostgresqlconnectionstring()
        {
            postgresqlconnectionstringdto _connectionstringdto = new postgresqlconnectionstringdto();

            _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["postgresql_datasource"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["postgresql_database"];
            _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["postgresql_userid"];
            _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["postgresql_password"];
            _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["postgresql_port"];
            _connectionstringdto.update_schema_temp_db = System.Configuration.ConfigurationManager.AppSettings["update_schema_temp_db"];
            _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["update_schema_temp_db"];
            _connectionstringdto.update_schema_current_db = System.Configuration.ConfigurationManager.AppSettings["postgresql_database"];

            string CONNECTION_STRING = buildpostgresqlconnectionstringfromobject(_connectionstringdto);

            return CONNECTION_STRING;
        }

        static string buildpostgresqlconnectionstringfromobject(postgresqlconnectionstringdto _connectionstringdto)
        {
            string CONNECTION_STRING = @"Server=" + _connectionstringdto.datasource + ";" +
                "Database=" + _connectionstringdto.database + ";" +
                "Port=" + _connectionstringdto.port + ";" +
                "User Id=" + _connectionstringdto.userid + ";" +
                "Password=" + _connectionstringdto.password;
            return CONNECTION_STRING;
        }

        public static string getdefaultsqliteconnectionstring()
        {
            sqliteconnectionstringdto _connectionstringdto = new sqliteconnectionstringdto();

            _connectionstringdto.sqlite_database_path = System.Configuration.ConfigurationManager.AppSettings["sqlite_database_path"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["sqlite_database"];
            _connectionstringdto.sqlite_db_extension = System.Configuration.ConfigurationManager.AppSettings["sqlite_db_extension"];
            _connectionstringdto.sqlite_version = System.Configuration.ConfigurationManager.AppSettings["sqlite_version"];
            _connectionstringdto.sqlite_pooling = System.Configuration.ConfigurationManager.AppSettings["sqlite_pooling"];
            _connectionstringdto.sqlite_fail_if_missing = System.Configuration.ConfigurationManager.AppSettings["sqlite_fail_if_missing"];

            string CONNECTION_STRING = buildsqliteconnectionstringfromobject(_connectionstringdto);

            return CONNECTION_STRING;

        }

        static string buildsqliteconnectionstringfromobject(sqliteconnectionstringdto _connectionstringdto)
        {
            string base_dir = Path.GetTempPath();

            string database_dir = Path.Combine(base_dir, _connectionstringdto.sqlite_database_path);

            string dbname = _connectionstringdto.new_database_name;
            string database_version = _connectionstringdto.sqlite_version;
            string db_extension = _connectionstringdto.sqlite_db_extension;
            dbname = dbname + "." + db_extension + database_version;

            if (!Directory.Exists(database_dir))
            {
                return null;
            }
            else
            {

            }

            string full_database_name_with_path = Path.Combine(database_dir, dbname);
            string _secure_path_name_response = dbname;

            if (!File.Exists(full_database_name_with_path))
            {
                return null;
            }
            else
            {

            }

            string CONNECTION_STRING = @"Data Source=" + full_database_name_with_path + ";" +
            "Version=" + _connectionstringdto.sqlite_version + ";" +
            "Pooling=" + _connectionstringdto.sqlite_pooling + ";" +
            "FailIfMissing=" + _connectionstringdto.sqlite_fail_if_missing;

            return CONNECTION_STRING;
        }

        public static string getdefaultmongodbconnectionstring()
        {
            mongodbconnectionstringdto _connectionstringdto = new mongodbconnectionstringdto();

            _connectionstringdto.datasource = System.Configuration.ConfigurationManager.AppSettings["mongodb_datasource"];
            _connectionstringdto.database = System.Configuration.ConfigurationManager.AppSettings["mongodb_database"];
            _connectionstringdto.userid = System.Configuration.ConfigurationManager.AppSettings["mongodb_userid"];
            _connectionstringdto.password = System.Configuration.ConfigurationManager.AppSettings["mongodb_password"];
            _connectionstringdto.port = System.Configuration.ConfigurationManager.AppSettings["mongodb_port"];
            _connectionstringdto.update_schema_temp_db = System.Configuration.ConfigurationManager.AppSettings["update_schema_temp_db"];
            _connectionstringdto.new_database_name = System.Configuration.ConfigurationManager.AppSettings["update_schema_temp_db"];
            _connectionstringdto.update_schema_current_db = System.Configuration.ConfigurationManager.AppSettings["postgresql_database"];

            string CONNECTION_STRING = buildmongodbconnectionstringfromobject(_connectionstringdto);

            return CONNECTION_STRING;
        }

        static string buildmongodbconnectionstringfromobject(mongodbconnectionstringdto _connectionstringdto)
        {
            string CONNECTION_STRING = @"Server=" + _connectionstringdto.datasource + ";" +
                "Database=" + _connectionstringdto.database + ";" +
                "Port=" + _connectionstringdto.port + ";" +
                "User Id=" + _connectionstringdto.userid + ";" +
                "Password=" + _connectionstringdto.password;
            return CONNECTION_STRING;
        }

        public static string[] table_names_arr = {
		"tbl_tasks",
        "tbl_logs"
	};

        public static string[] _entities = {
			"mssql", 
			"mysql",
			"postgresql", 
			"sqlite",
            "mongodb"
		};





    }






}



