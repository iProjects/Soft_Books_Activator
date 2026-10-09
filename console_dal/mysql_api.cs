/*
 * Created by: "kevin mutugi, kevinmk30@gmail.com, +254717769329"
 * Date: 01/23/2020
 * Time: 02:55
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using console_commonlib;
using console_dto;

namespace console_dal
{
    public sealed class mysql_api
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static mysql_api singleInstance;
        public static mysql_api getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new mysql_api(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }
        public string TAG;

        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        public mysql_api(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {

            TAG = this.GetType().Name;

            _notificationmessageEventname = notificationmessageEventname;

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("initialized mysql_api...", TAG));
        }

        public responsedto save_task_in_mysql(task_dto _task_dto)
        {
            responsedto _responsedto = new responsedto();

            bool _exists_in_db = check_if_task_exists(_task_dto.name);

            if (_exists_in_db)
            {
                _responsedto.isresponseresultsuccessful = true;
                _responsedto.responsesuccessmessage = "task with name [ " + _task_dto.name + " ] exists in " + DBContract.mysql + ".";
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] exists in " + DBContract.mssql + ".", TAG));
            }
            else
            {
                string save_in_db = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("saveinmysql", "false");

                bool _save_in_db;
                bool _try_save_in_db = bool.TryParse(save_in_db, out _save_in_db);

                if (_save_in_db)
                {
                    _responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).create_task_in_database(_task_dto);

                }
            }
            return _responsedto;
        }
        public bool check_if_task_exists(string entity_name)
        {
            bool _exists_in_db = mysqlapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(entity_name);

            return _exists_in_db;

        }

        public responsedto save_user_in_mysql(user_dto _dto)
        {
            responsedto _responsedto = new responsedto();

            bool _exists_in_db = check_if_user_exists(_dto.email);

            if (_exists_in_db)
            {
                _responsedto.isresponseresultsuccessful = false;
                _responsedto.responsesuccessmessage = "user with email [ " + _dto.email + " ] exists in " + DBContract.mysql + ".";
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with email [ " + _dto.email + " ] exists in " + DBContract.mssql + ".", TAG));
            }
            else
            {
                string save_in_db = utilzsingleton.getInstance(_notificationmessageEventname).getappsettinggivenkey("saveinmysql", "false");

                bool _save_in_db;
                bool _try_save_in_db = bool.TryParse(save_in_db, out _save_in_db);

                if (_save_in_db)
                {
                    _responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).create_user_in_database(_dto);

                }
            }
            return _responsedto;
        }
        public bool check_if_user_exists(string entity_name)
        {
            bool _exists_in_db = mysqlapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(entity_name);

            return _exists_in_db;

        }







    }
}
