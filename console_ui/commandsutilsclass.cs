/*
 * Created by SharpDevelop.
 * User: USER
 * Date: 10/18/2018
 * Time: 14:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data;

namespace console_ui
{
    public class commandsutilsclass
    {

        public const string createdatabase = "createdatabase";
        public const string createdatabase_shortcut = "db";
        public const string backupdatabase = "backupdatabase";
        public const string backupdatabase_shortcut = "bd";
        public const string script = "script";
        public const string script_shortcut = "sc";
        public const string truncatedatabase = "truncatedatabase";
        public const string truncatedatabase_shortcut = "td";
        public const string copydata = "copydata";
        public const string copydata_shortcut = "cd";
        public const string connections = "connections";
        public const string connections_shortcut = "c";
        public const string version = "version";
        public const string version_shortcut = "v";
        public const string start = "start";
        public const string start_shortcut = "s";
        public const string help = "help";
        public const string help_shortcut = "h";
        public const string exit = "exit";
        public const string exit_shortcut = "x";

        public const string listall = "listall";
        public const string listall_shortcut = "all";

        public const string listlogs = "listlogs";
        public const string listlogs_shortcut = "ll";

        public const string create_user = "createuser";
        public const string create_user_shortcut = "cu";
        public const string list_users = "listusers";
        public const string list_users_shortcut = "lu";

        public const string create_task = "createtask";
        public const string create_task_shortcut = "ct";
        public const string list_tasks = "listtasks";
        public const string list_tasks_shortcut = "lt";

        public const string fetch_servers = "fetchservers";
        public const string fetch_servers_shortcut = "fs";

        public const string fetch_databases = "fetchdatabases";
        public const string fetch_databases_shortcut = "fd";

        public const string upload_users = "uploadusers";
        public const string upload_users_shortcut = "uu";

    }


}
