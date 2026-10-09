/*
 * Created by SharpDevelop.
 * User: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 12/19/2018
 * Time: 12:06 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using console_commonlib;
using console_dal;
using console_dto;

namespace console_business
{
    /// <summary>
    /// Description of businesslayerapisingleton.
    /// </summary>
    public sealed class businesslayerapisingleton
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static businesslayerapisingleton singleInstance;

        public static businesslayerapisingleton getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new businesslayerapisingleton(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        private event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;

        DataTable mssql_dt = null;
        DataTable mysql_dt = null;
        DataTable sqlite_dt = null;
        DataTable postgresql_dt = null;
        DataTable mongodb_dt = null;
        DataTable dt = null;

        string TAG;
        public string _working_db = "";
        string query = "";

        private businesslayerapisingleton(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            _notificationmessageEventname = notificationmessageEventname;
            TAG = this.GetType().Name;
        }

        private businesslayerapisingleton()
        {

        }

        public List<responsedto> set_up_databases()
        {
            List<responsedto> _lstresponse = new List<responsedto>();

            responsedto _mssql_responsedto = mssqlapisingleton.getInstance(_notificationmessageEventname).set_up_database_on_load();

            _lstresponse.Add(_mssql_responsedto);

            responsedto _mysql_responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).set_up_database_on_load();

            _lstresponse.Add(_mysql_responsedto);

            responsedto _postgresql_responsedto = postgresqlapisingleton.getInstance(_notificationmessageEventname).set_up_database_on_load();

            _lstresponse.Add(_postgresql_responsedto);

            responsedto _sqlite_responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).set_up_database_on_load();

            _lstresponse.Add(_sqlite_responsedto);

            responsedto _mongodb_responsedto = mongodbapisingleton.getInstance(_notificationmessageEventname).set_up_database_on_load();

            _lstresponse.Add(_mongodb_responsedto);

            return _lstresponse;
        }

        #region "tasks"
        public DataTable get_tasks(bool showinactive = true, string query = "", string server = "")
        {
            sqlite_dt = get_tasks_from_sqlite(query);

            mysql_dt = get_tasks_from_mysql(query);

            mongodb_dt = get_tasks_from_mongodb(query);

            mssql_dt = get_tasks_from_mssql(query);

            postgresql_dt = get_tasks_from_postgresql(query);

            if (server == DBContract.mssql && mssql_dt != null)
            {
                dt = mssql_dt;
                _working_db = DBContract.mssql;
            }
            if (server == DBContract.mysql && mysql_dt != null)
            {
                dt = mysql_dt;
                _working_db = DBContract.mysql;
            }
            if (server == DBContract.postgresql && postgresql_dt != null)
            {
                dt = postgresql_dt;
                _working_db = DBContract.postgresql;
            }
            if (server == DBContract.sqlite && sqlite_dt != null)
            {
                dt = sqlite_dt;
                _working_db = DBContract.sqlite;
            }
            if (server == DBContract.mongodb && mongodb_dt != null)
            {
                dt = mongodb_dt;
                _working_db = DBContract.mongodb;
            }

            return dt;

        }

        private DataTable get_tasks_from_mssql(string query)
        {
            try
            {

                DataTable mssql_dt = mssqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (mssql_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mssql tasks count: " + mssql_dt.Rows.Count, TAG));
                }
                return mssql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        private DataTable get_tasks_from_mysql(string query)
        {
            try
            {

                DataTable mysql_dt = mysqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (mysql_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mysql tasks count: " + mysql_dt.Rows.Count, TAG));
                }
                return mysql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        private DataTable get_tasks_from_sqlite(string query)
        {
            try
            {

                DataTable sqlite_dt = sqliteapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (sqlite_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite tasks count: " + sqlite_dt.Rows.Count, TAG));
                }
                return sqlite_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        private DataTable get_tasks_from_postgresql(string query)
        {
            try
            {

                DataTable postgresql_dt = postgresqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (postgresql_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("postgresql tasks count: " + postgresql_dt.Rows.Count, TAG));
                }
                return postgresql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        private DataTable get_tasks_from_mongodb(string query)
        {
            try
            {

                DataTable mongodb_dt = mongodbapisingleton.getInstance(_notificationmessageEventname).get_all_tasks();
                if (mongodb_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mongodb tasks count: " + mongodb_dt.Rows.Count, TAG));
                }
                return mongodb_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        #endregion "tasks"

        #region "users"
        public DataTable get_users(bool showinactive = true, string query = "", string server = "")
        {
            sqlite_dt = get_users_from_sqlite(query);

            mongodb_dt = get_users_from_mongodb(query);

            mysql_dt = get_users_from_mysql(query);

            mssql_dt = get_users_from_mssql(query);

            postgresql_dt = get_users_from_postgresql(query);

            if (server == "mssql" && mssql_dt != null)
            {
                dt = mssql_dt;
                _working_db = DBContract.mssql;
            }
            if (server == "mysql" && mysql_dt != null)
            {
                dt = mysql_dt;
                _working_db = DBContract.mysql;
            }
            if (server == "postgresql" && postgresql_dt != null)
            {
                dt = postgresql_dt;
                _working_db = DBContract.postgresql;
            }
            if (server == "sqlite" && sqlite_dt != null)
            {
                dt = sqlite_dt;
                _working_db = DBContract.sqlite;
            }
            if (server == "mongodb" && mongodb_dt != null)
            {
                dt = mongodb_dt;
                _working_db = DBContract.mongodb;
            }

            return dt;

        }
        private DataTable get_users_from_mssql(string query)
        {
            try
            {
                DataTable mssql_dt = new DataTable();
                //mssql_dt = mssqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                //if (mssql_dt != null)
                //{
                //    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mssql missions count: " + mssql_dt.Rows.Count, TAG));
                //}
                return mssql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        private DataTable get_users_from_mysql(string query)
        {
            try
            {
                DataTable mysql_dt = new DataTable();
                //mysql_dt = mysqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                //if (mysql_dt != null)
                //{
                //    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mysql missions count: " + mysql_dt.Rows.Count, TAG));
                //}
                return mysql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        private DataTable get_users_from_sqlite(string query)
        {
            try
            {

                DataTable sqlite_dt = sqliteapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (sqlite_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite missions count: " + sqlite_dt.Rows.Count, TAG));
                }
                return sqlite_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        private DataTable get_users_from_postgresql(string query)
        {
            try
            {
                DataTable postgresql_dt = new DataTable();
                //postgresql_dt = postgresqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                //if (postgresql_dt != null)
                //{
                //    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("postgresql missions count: " + postgresql_dt.Rows.Count, TAG));
                //}
                return postgresql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        private DataTable get_users_from_mongodb(string query)
        {
            try
            {
                DataTable mongodb_dt = new DataTable();
                //mongodb_dt = mongodbapisingleton.getInstance(_notificationmessageEventname).get_all_missions();
                //if (mongodb_dt != null)
                //{
                //    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mongodb missions count: " + mongodb_dt.Rows.Count, TAG));
                //}
                return mongodb_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public List<responsedto> create_user_in_database(user_dto dto)
        {
            try
            {
                List<responsedto> _lstresponse = new List<responsedto>();

                responsedto _sqlite_responsedto = sqlite_api.getInstance(_notificationmessageEventname).save_user_in_sqlite(dto);

                _lstresponse.Add(_sqlite_responsedto);

                responsedto _mongodb_responsedto = mongodb_api.getInstance(_notificationmessageEventname).save_user_in_mongodb(dto);

                _lstresponse.Add(_mongodb_responsedto);

                responsedto _mssql_responsedto = mssql_api.getInstance(_notificationmessageEventname).save_user_in_mssql(dto);

                _lstresponse.Add(_mssql_responsedto);

                responsedto _mysql_responsedto = mysql_api.getInstance(_notificationmessageEventname).save_user_in_mysql(dto);

                _lstresponse.Add(_mysql_responsedto);

                responsedto _postgresql_responsedto = postgresql_api.getInstance(_notificationmessageEventname).save_user_in_postgresql(dto);

                _lstresponse.Add(_postgresql_responsedto);

                return _lstresponse;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public user_dto get_user_by_id(int id)
        {
            try
            {
                user_dto dto = new user_dto();
                dto = sqliteapisingleton.getInstance(_notificationmessageEventname).get_user_by_id(id);

                return dto;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public user_dto get_user_by_email(string email)
        {
            try
            {
                user_dto dto = new user_dto();
                dto = sqliteapisingleton.getInstance(_notificationmessageEventname).get_user_by_email(email);

                return dto;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public bool check_if_user_exists(string email)
        {
            try
            {
                return sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(email);
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return false;
            }
        }
        public responsedto update_user_in_database(user_dto dto)
        {
            try
            {
                responsedto _responsedto = new responsedto();
                _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).update_user_in_database(dto);

                return _responsedto;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public responsedto delete_user_in_database(user_dto dto)
        {
            try
            {
                responsedto _responsedto = new responsedto();
                _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).delete_user_in_database(dto);

                return _responsedto;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public DataTable search_users_in_database(user_dto dto)
        {
            try
            {
                DataTable sqlite_dt = sqliteapisingleton.getInstance(_notificationmessageEventname).search_users_in_database(dto);
                if (sqlite_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite missions count: " + sqlite_dt.Rows.Count, TAG));
                }
                return sqlite_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public responsedto login(user_dto dto)
        {
            try
            {
                responsedto _responsedto = new responsedto();
                _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).login(dto);

                return _responsedto;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        #endregion "users"

        #region "logs"
        public DataTable get_logs(bool showinactive = true, string query = "", string server = "")
        {
            sqlite_dt = get_logs_from_sqlite(query);

            mongodb_dt = get_logs_from_mongodb(query);

            mysql_dt = get_logs_from_mysql(query);

            mssql_dt = get_logs_from_mssql(query);

            postgresql_dt = get_logs_from_postgresql(query);

            if (server == DBContract.mssql && mssql_dt != null)
            {
                dt = mssql_dt;
                _working_db = DBContract.mssql;
            }
            if (server == DBContract.mysql && mysql_dt != null)
            {
                dt = mysql_dt;
                _working_db = DBContract.mysql;
            }
            if (server == DBContract.postgresql && postgresql_dt != null)
            {
                dt = postgresql_dt;
                _working_db = DBContract.postgresql;
            }
            if (server == DBContract.sqlite && sqlite_dt != null)
            {
                dt = sqlite_dt;
                _working_db = DBContract.sqlite;
            }
            if (server == DBContract.mongodb && mongodb_dt != null)
            {
                dt = mongodb_dt;
                _working_db = DBContract.mongodb;
            }

            return dt;

        }

        private DataTable get_logs_from_mssql(string query)
        {
            try
            {
                DataTable mssql_dt = new DataTable();
                mssql_dt = mssqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (mssql_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mssql logs count: " + mssql_dt.Rows.Count, TAG));
                }
                return mssql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        private DataTable get_logs_from_mysql(string query)
        {
            try
            {
                DataTable mysql_dt = new DataTable();
                mysql_dt = mysqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (mysql_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mysql logs count: " + mysql_dt.Rows.Count, TAG));
                }
                return mysql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        private DataTable get_logs_from_sqlite(string query)
        {
            try
            {

                DataTable sqlite_dt = sqliteapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (sqlite_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite logs count: " + sqlite_dt.Rows.Count, TAG));
                }
                return sqlite_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        private DataTable get_logs_from_postgresql(string query)
        {
            try
            {
                DataTable postgresql_dt = new DataTable();
                postgresql_dt = postgresqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (postgresql_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("postgresql logs count: " + postgresql_dt.Rows.Count, TAG));
                }
                return postgresql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        private DataTable get_logs_from_mongodb(string query)
        {
            try
            {
                DataTable mongodb_dt = new DataTable();
                mongodb_dt = mongodbapisingleton.getInstance(_notificationmessageEventname).get_all_logs();
                if (mongodb_dt != null)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mongodb logs count: " + mongodb_dt.Rows.Count, TAG));
                }
                return mongodb_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        public List<responsedto> create_log_in_database(log_dto dto)
        {
            try
            {
                List<responsedto> _lstresponse = new List<responsedto>();

                responsedto _sqlite_responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).create_log_in_database(dto);

                _lstresponse.Add(_sqlite_responsedto);

                responsedto _mongodb_responsedto = mongodbapisingleton.getInstance(_notificationmessageEventname).create_log_in_database(dto);

                _lstresponse.Add(_mongodb_responsedto);

                responsedto _mssql_responsedto = mssqlapisingleton.getInstance(_notificationmessageEventname).create_log_in_database(dto);

                _lstresponse.Add(_mssql_responsedto);

                responsedto _mysql_responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).create_log_in_database(dto);

                _lstresponse.Add(_mysql_responsedto);

                responsedto _postgresql_responsedto = postgresqlapisingleton.getInstance(_notificationmessageEventname).create_log_in_database(dto);

                _lstresponse.Add(_postgresql_responsedto);

                return _lstresponse;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }
        #endregion "logs"




    }
}
