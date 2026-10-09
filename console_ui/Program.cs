using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using console_business;
using console_commonlib;
using console_dal;
using console_dto;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using SMOScripting;
using System.Reflection;


namespace console_ui
{
    public class Program
    {
        public string TAG;
        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        public event EventHandler<progressBarNotificationEventArgs> _progressBarNotificationEventname;
        public string _apppath;
        public string _xmlpathfolder;
        public string _txtpathfolder;
        public string _xml_loga_file;
        public string _xml_loga_file_fluentsyntax;
        public string _txt_loga_file;
        public List<notificationdto> _lstnotificationdto = new List<notificationdto>();
        public string _working_db;
        copy_data_utils data_utils;

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_MAXIMIZE = 3;

        static void Main(string[] args)
        {
            Program _Program = new Program();
        }

        void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Console.ForegroundColor = ConsoleColor.Red;
            this._notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs(ex.Message, TAG));
            Console.ForegroundColor = ConsoleColor.Green;
        }

        void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Console.ForegroundColor = ConsoleColor.Red;
            this._notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs(ex.Message, TAG));
            Console.ForegroundColor = ConsoleColor.Green;
        }

        //Event handler declaration:
        void notificationmessageHandler(object sender, notificationmessageEventArgs args)
        {
            /* Handler logic */
            //notificationdto _notificationdto = new notificationdto();
            DateTime currentDate = DateTime.Now;
            String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss");
            String _logtext = "[ " + dateTimenow + " ]   " + args.message + "\n";

            global_write_to_console(_logtext, dateTimenow);
        }

        void global_write_to_console(string _stringtoprint, string str_default = "")
        {
            try
            {
                if (string.IsNullOrEmpty(str_default))
                {
                    DateTime currentDate = DateTime.Now;
                    String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss");
                    String _logtext = "[ " + dateTimenow + " ]   " + _stringtoprint + "\n";

                    Console.WriteLine(_logtext);
                    Console.Beep();

                    addnotification(_stringtoprint, TAG);
                    persistconsolecontenttofile();
                }
                else
                {
                    Console.WriteLine(_stringtoprint);
                    Console.Beep();
                    addnotification(_stringtoprint, TAG);
                    persistconsolecontenttofile();
                }

                helper_utils.getInstance().log_messages(_stringtoprint, TAG);

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void addnotification(string message, string TAG)
        {
            notificationdto _notificationdto = new notificationdto();
            DateTime currentDate = DateTime.Now;
            String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss");
            //String _logtext = "[ " + dateTimenow + " ]   " + message + "\n";
            _notificationdto._notification_message = message;
            _notificationdto._created_datetime = dateTimenow;
            _notificationdto.TAG = TAG;
            _lstnotificationdto.Add(_notificationdto);

            Log.Write_To_Log_File_temp_dir(new Exception(message));
        }

        void progressbarHandler(object sender, progressBarNotificationEventArgs args)
        {
            /* Handler logic */
            DateTime currentDate = DateTime.Now;
            String dateTimenow = currentDate.Ticks.ToString();
            String _logtext = "" + currentDate + " " + "\n";
            Console.Beep();

            global_write_to_console(_logtext, dateTimenow);
        }

        public Program()
        {
            TAG = this.GetType().Name;

            Console.Title = "Soft Books Activator Management App Command Line Interface";

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

            IntPtr handle = GetConsoleWindow();
            ShowWindow(handle, SW_MAXIMIZE);

            _notificationmessageEventname += notificationmessageHandler;
            _progressBarNotificationEventname += progressbarHandler;

            data_utils = new copy_data_utils(_notificationmessageEventname, _progressBarNotificationEventname);

            DateTime currentDate = DateTime.Now;
            String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss");

            _apppath = Application.StartupPath;
            _xmlpathfolder = _apppath + @"\xmlloga\";
            _txtpathfolder = _apppath + @"\txtloga\";
            _xml_loga_file = _xmlpathfolder + "xmllogautilz.xml";
            _xml_loga_file_fluentsyntax = _xmlpathfolder + "xmllogautilzfluentsyntax.xml";
            _txt_loga_file = _txtpathfolder + "txtlogautilz.txt";

            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Soft Books Activator Management App Command Line Interface started.", TAG));

            showversioninfocliutils();

            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Type a command to do a task. Following is a list of commands.", TAG));

            helper_utils.getInstance().set_up_databases();

            show_prompt();

        }

        void checkcommands()
        {
            string _command = Console.ReadLine();
            switch (_command)
            {
                case commandsutilsclass.createdatabase:
                    create_database_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.createdatabase_shortcut:
                    create_database_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.backupdatabase:
                    backup_database_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.backupdatabase_shortcut:
                    backup_database_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.script:
                    show_script_form();
                    checkcommands();
                    break;
                case commandsutilsclass.script_shortcut:
                    show_script_form();
                    checkcommands();
                    break;
                case commandsutilsclass.truncatedatabase:
                    truncate_database_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.truncatedatabase_shortcut:
                    truncate_database_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.copydata:
                    copy_data();
                    checkcommands();
                    break;
                case commandsutilsclass.copydata_shortcut:
                    copy_data();
                    checkcommands();
                    break;
                case commandsutilsclass.connections:
                    showconnectionsinfocliutils();
                    checkcommands();
                    break;
                case commandsutilsclass.connections_shortcut:
                    showconnectionsinfocliutils();
                    checkcommands();
                    break;
                case commandsutilsclass.version:
                    showversioninfocliutils();
                    checkcommands();
                    break;
                case commandsutilsclass.version_shortcut:
                    showversioninfocliutils();
                    checkcommands();
                    break;
                case commandsutilsclass.start:
                    startnewconsolecliutils();
                    checkcommands();
                    break;
                case commandsutilsclass.start_shortcut:
                    startnewconsolecliutils();
                    checkcommands();
                    break;
                case commandsutilsclass.help:
                    showcommands();
                    break;
                case commandsutilsclass.help_shortcut:
                    showcommands();
                    break;
                case commandsutilsclass.exit:
                    exit_system();
                    break;
                case commandsutilsclass.exit_shortcut:
                    exit_system();
                    break;

                case commandsutilsclass.listall:
                    listallcliutils();
                    checkcommands();
                    break;
                case commandsutilsclass.listall_shortcut:
                    listallcliutils();
                    checkcommands();
                    break;

                case commandsutilsclass.listlogs:
                    list_logs_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.listlogs_shortcut:
                    list_logs_cli_utils();
                    checkcommands();
                    break;

                case commandsutilsclass.create_user:
                    create_user_cli_utils();
                    list_users_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.create_user_shortcut:
                    create_user_cli_utils();
                    list_users_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.list_users:
                    list_users_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.list_users_shortcut:
                    list_users_cli_utils();
                    checkcommands();
                    break;

                case commandsutilsclass.create_task:
                    create_task_cli_utils();
                    list_tasks_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.create_task_shortcut:
                    create_task_cli_utils();
                    list_tasks_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.list_tasks:
                    list_tasks_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.list_tasks_shortcut:
                    list_tasks_cli_utils();
                    checkcommands();
                    break;

                case commandsutilsclass.fetch_servers:
                    fetch_servers_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.fetch_servers_shortcut:
                    fetch_servers_cli_utils();
                    checkcommands();
                    break;

                case commandsutilsclass.fetch_databases:
                    fetch_databases_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.fetch_databases_shortcut:
                    fetch_databases_cli_utils();
                    checkcommands();
                    break;

                case commandsutilsclass.upload_users:
                    upload_users_cli_utils();
                    checkcommands();
                    break;
                case commandsutilsclass.upload_users_shortcut:
                    upload_users_cli_utils();
                    checkcommands();
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Unrecognised Command [ " + _command + " ]. \r\n Type help or h to see a list of available commands.", TAG));
                    Console.ForegroundColor = ConsoleColor.Green;
                    show_prompt();
                    persistconsolecontenttofile();
                    break;
            }
        }

        void showcommands()
        {
            try
            {
                string _strcommands = "\n\n Command \t  -\t Description \n\n";
                _strcommands += "createdatabase\\db \t  -\t Create database. \n";
                _strcommands += "backupdatabase\\bd \t  -\t Back Up database. \n";
                _strcommands += "script\\sc \t  -\t Script mssql databases. \n";
                _strcommands += "truncatedatabase\\td \t  -\t Truncate databases. \n";
                _strcommands += "copydata\\cd \t  -\t Show datastores connection states. \n";
                _strcommands += "connections\\c \t  -\t Show datastores connection states. \n";
                _strcommands += "version\\v \t  -\t Display the app version. \n";
                _strcommands += "start\\s \t  -\t Start a new window to run commands. \n";
                _strcommands += "help\\h \t  -\t Show a list of commands. \n";
                _strcommands += "exit\\x \t  -\t Exit the system. \n";

                _strcommands += "listall\\all \t  -\t Show a list of all records. \n";

                _strcommands += "listlogs\\ll \t  -\t Show a list of Logs. \n";

                _strcommands += "createuser\\cu \t  -\t Create User. \n";
                _strcommands += "listusers\\lu \t  -\t Show a list of Users. \n";

                _strcommands += "createtask\\ct \t  -\t Create Task. \n";
                _strcommands += "listtasks\\lt \t  -\t Show a list of Tasks. \n";

                _strcommands += "fetchservers\\fs \t  -\t Show a list of Servers. \n";

                _strcommands += "fetchdatabases\\fd \t  -\t Show a list of Databases. \n";

                _strcommands += "uploadusers\\uu \t  -\t Upload a list of Users. \n";

                Console.ForegroundColor = ConsoleColor.Cyan;
                global_write_to_console(_strcommands);
                Console.ForegroundColor = ConsoleColor.Green;

                checkcommands();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void showversioninfocliutils()
        {
            try
            {
                print_table_list_separator();

                StringBuilder sb = new StringBuilder();
                sb.Append("\tProduct Name [ " + Application.ProductName + " ]");
                sb.Append(Environment.NewLine);
                sb.Append("\tCopyright @ [ 2025 - " + DateTime.Now.Year + " ]");
                sb.Append(Environment.NewLine);
                sb.Append("\tProduct Version [ " + Application.ProductVersion + " ]");
                sb.Append(Environment.NewLine);
                sb.Append("\tCompany Name [ " + Application.CompanyName + " ]");

                global_write_to_console(sb.ToString());

                print_table_list_separator();

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void print_table_list_separator()
        {

            global_write_to_console("**********************************************************************************************************");
        }

        void persistconsolecontenttofile()
        {
            try
            {
                savetotxt();
                savetoxml();
                savetoxmlfluentsyntax();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void savetotxt()
        {
            try
            {
                string temp_path = Path.GetTempPath();
                string app_name = System.Configuration.ConfigurationManager.AppSettings["APP_NAME"];
                string log_file_name = app_name + "_text_file" + ".txt";
                var _temp_file = Path.Combine(temp_path, log_file_name);

                if (!Directory.Exists(temp_path))
                {
                    Directory.CreateDirectory(temp_path);
                    return;
                }
                if (!File.Exists(_temp_file))
                    File.Create(_temp_file).Close();

                using (FileStream _fileStream = new FileStream(_temp_file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {

                    StreamWriter sw = new StreamWriter(_fileStream);

                    foreach (var _log in _lstnotificationdto)
                    {

                        sw.WriteLine("..............................");
                        sw.WriteLine("TAG: " + _log.TAG);
                        sw.WriteLine("NOTIFICATION_MESSAGE: " + _log._notification_message);
                        sw.WriteLine("..............................\n");

                    }

                    //sw.Close(); This will close ms and when we try to use ms later it will cause an exception
                    sw.Flush();
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }

        }

        void savetoxml()
        {
            try
            {
                string temp_path = Path.GetTempPath();
                string app_name = System.Configuration.ConfigurationManager.AppSettings["APP_NAME"];
                string log_file_name = app_name + "_xml_file" + ".xml";
                var _temp_file = Path.Combine(temp_path, log_file_name);

                if (!Directory.Exists(temp_path))
                {
                    Directory.CreateDirectory(temp_path);
                    return;
                }
                if (!File.Exists(_temp_file))
                    File.Create(_temp_file).Close();

                using (FileStream _fileStream = new FileStream(_temp_file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                { // you can write to the fileStream

                    var xmlDoc = new XmlDocument();
                    //			xml.Load(_fileStream);
                    xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                    var root = xmlDoc.CreateElement("com.tech.nyax");
                    // Creates an attribute, so the element will now be "<element attribute='value' />"
                    root.SetAttribute("APPLICATION_NAME", Application.ProductName);
                    root.SetAttribute("APPLICATION_VERSION", Application.ProductVersion);
                    // All XML documents must have one, and only one, root element
                    xmlDoc.AppendChild(root);
                    // Adding data to an XML document
                    foreach (var _log in _lstnotificationdto)
                    {
                        var child = xmlDoc.CreateElement("LOG");
                        child.SetAttribute("TAG", _log.TAG);
                        child.SetAttribute("CREATED_DATE", _log._created_datetime);
                        child.SetAttribute("NOTIFICATION_MESSAGE", _log._notification_message);
                        //child.InnerText = _log._notification_message;
                        // Don't forget to add the new value to the current document!
                        root.AppendChild(child);
                    }

                    // Displays the XML document in the screen; 
                    // optionally can be saved to a file
                    xmlDoc.Save(_fileStream);

                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }

        }

        void savetoxmlfluentsyntax()
        {
            try
            {

                string temp_path = Path.GetTempPath();
                string app_name = System.Configuration.ConfigurationManager.AppSettings["APP_NAME"];
                string log_file_name = app_name + "_xml_fluent_syntax_file" + ".xml";
                var _temp_file = Path.Combine(temp_path, log_file_name);

                if (!Directory.Exists(temp_path))
                {
                    Directory.CreateDirectory(temp_path);
                    return;
                }
                if (!File.Exists(_temp_file))
                    File.Create(_temp_file).Close();

                using (FileStream _fileStream = new FileStream(_temp_file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                { // you can write to the fileStream

                    XNamespace xns = "http://com.tech.nyax";
                    XDeclaration xDeclaration = new XDeclaration("1.0", "utf-8", "yes");
                    XDocument xDoc = new XDocument(xDeclaration);
                    XElement xRoot = new XElement(xns + "com.tech.nyax");
                    xDoc.Add(xRoot);

                    foreach (var _log in _lstnotificationdto)
                    {
                        XElement xelparent = new XElement(xns + "LOG");
                        XAttribute xAttribute1 = new XAttribute("APPLICATION_NAME", Application.ProductName);
                        XAttribute xAttribute2 = new XAttribute("APPLICATION_VERSION", Application.ProductVersion);
                        xelparent.Add(xAttribute1);
                        xelparent.Add(xAttribute2);
                        XElement xelchild1 = new XElement(xns + "TAG", _log.TAG);
                        XElement xelchild2 = new XElement(xns + "CREATED_DATE", _log._created_datetime);
                        XElement xelchild3 = new XElement(xns + "NOTIFICATION_MESSAGE", _log._notification_message);
                        xelparent.Add(xelchild1);
                        xelparent.Add(xelchild2);
                        xelparent.Add(xelchild3);
                        xRoot.Add(xelparent);
                    }

                    xDoc.Save(_fileStream);

                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void exit_system()
        {
            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("committing transactions...", TAG));
            Thread.Sleep(500);
            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("closing connections...", TAG));
            Thread.Sleep(500);
            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("cleaning temporary objects...", TAG));
            persistconsolecontenttofile();
            Thread.Sleep(500);
            Environment.Exit(0);
        }

        void show_prompt()
        {
            showcommands();
            checkcommands();
        }

        void show_script_form()
        {
            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("calling Script Form...", TAG));
            ScriptForm _frmtenants = new ScriptForm();
            _frmtenants.Show();
        }

        void truncate_database_cli_utils()
        {
            try
            {
                global_write_to_console("Are you sure you want to delete all the Records in the databases? Y/N");
                string answer = Console.ReadLine();

                while (String.IsNullOrEmpty(answer))
                {
                    global_write_to_console("Are you sure you want to delete all the Records in the databases? Y/N");
                    answer = Console.ReadLine();
                }

                while (!String.IsNullOrEmpty(answer))
                {
                    if (answer == "Y" || answer == "y")
                    {
                        break;
                    }
                    else if (answer == "N" || answer == "n")
                    {
                        break;
                    }

                    global_write_to_console("Are you sure you want to delete all the Records in the databases? Y/N");
                    answer = Console.ReadLine();
                }

                if (answer == "N" || answer == "n")
                {
                    show_prompt();
                }

                if (answer == "Y" || answer == "y")
                {
                    List<responsedto> _lstresponse = new List<responsedto>();
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Truncating " + DBContract.mssql + " database.", TAG));
                    responsedto _mssql_responsedto = mssqlapisingleton.getInstance(_notificationmessageEventname).truncate_database();
                    _lstresponse.Add(_mssql_responsedto);
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Truncating " + DBContract.mongodb + " database.", TAG));
                    responsedto _mongodb_responsedto = mongodbapisingleton.getInstance(_notificationmessageEventname).truncate_database();
                    _lstresponse.Add(_mongodb_responsedto);
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Truncating " + DBContract.mysql + " database.", TAG));
                    responsedto _mysql_responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).truncate_database();
                    _lstresponse.Add(_mysql_responsedto);
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Truncating " + DBContract.sqlite + " database.", TAG));
                    responsedto _sqlite_responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).truncate_database();
                    _lstresponse.Add(_sqlite_responsedto);
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Truncating " + DBContract.postgresql + " database.", TAG));
                    responsedto _postgresql_responsedto = postgresqlapisingleton.getInstance(_notificationmessageEventname).truncate_database();
                    _lstresponse.Add(_postgresql_responsedto);

                    foreach (var _dto in _lstresponse)
                    {
                        if (_dto.isresponseresultsuccessful)
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_dto.responsesuccessmessage, TAG));
                        }
                        else
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_dto.responseerrormessage, TAG));
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void create_database_cli_utils()
        {
            try
            {
                helper_utils.getInstance().set_up_databases();

                string _database_name = "";
                string _system = "";

                global_write_to_console("");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("loading create database interface...", TAG));

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide detail for given fields...", TAG));

                global_write_to_console("database name:");
                _database_name = Console.ReadLine();

                while (String.IsNullOrEmpty(_database_name))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database name cannot be null.", TAG));
                    global_write_to_console("database name:");
                    _database_name = Console.ReadLine();
                }

                //global_write_to_console(String.Format("\nsystem: [ {0}, {1}, {2}, {3} ]", DBContract.mssql, DBContract.mysql, DBContract.postgresql, DBContract.sqlite));
                //_system = Console.ReadLine();

                //while (String.IsNullOrEmpty(_system))
                //{
                //    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("system cannot be null.", TAG));
                //    global_write_to_console(String.Format("\nsystem: [ {0}, {1}, {2}, {3} ]", DBContract.mssql, DBContract.mysql, DBContract.postgresql, DBContract.sqlite));
                //    _system = Console.ReadLine();
                //}

                //while (!String.IsNullOrEmpty(_system))

                foreach (var _sys in DBContract._entities.ToList())
                {
                    _system = _sys;

                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("server [ " + _sys + " ]...", TAG));

                    if (_system == "mssql")
                    {
                        global_write_to_console("");

                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("create database task execution running against server [ " + DBContract.mssql + " ]...", TAG));

                        bool _exists_in_mssql = mssqlapisingleton.getInstance(_notificationmessageEventname).check_if_mssql_database_exists(_database_name);

                        if (!_exists_in_mssql)
                        {

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database existence check complete.", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] does not exists in " + DBContract.mssql + ".", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("commencing with the database creation...", TAG));

                            responsedto _responsedto = mssqlapisingleton.getInstance(_notificationmessageEventname).createdatabasegivennamefromconsole(_database_name);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                            mssqlconnectionstringdto _connectionstringdto = mssqlapisingleton.getInstance(_notificationmessageEventname).getmssqlconnectionstringdto();
                            _connectionstringdto.database = _database_name;
                            _connectionstringdto.new_database_name = _database_name;

                            _responsedto = mssqlapisingleton.getInstance(_notificationmessageEventname).create_tables(_connectionstringdto);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                        }
                        else
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] exists in " + DBContract.mssql + ".", TAG));
                        }
                        //Indicates that the iteration has ended
                        //break;
                    }
                    else if (_system == "mysql")
                    {
                        global_write_to_console("");

                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("create database task execution running against server [ " + DBContract.mysql + " ]...", TAG));

                        bool _exists_in_mysql = mysqlapisingleton.getInstance(_notificationmessageEventname).check_if_mysql_database_exists(_database_name);

                        if (!_exists_in_mysql)
                        {

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database existence check complete.", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] does not exists in " + DBContract.mysql + ".", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("commencing with the database creation...", TAG));

                            responsedto _responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).createdatabasegivennamefromconsole(_database_name);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                            mysqlconnectionstringdto _connectionstringdto = mysqlapisingleton.getInstance(_notificationmessageEventname).getmysqlconnectionstringdto();
                            _connectionstringdto.database = _database_name;
                            _connectionstringdto.new_database_name = _database_name;

                            _responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).create_tables(_connectionstringdto);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                        }
                        else
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] exists in " + DBContract.mysql + ".", TAG));
                        }
                        //Indicates that the iteration has ended
                        //break;
                    }
                    else if (_system == "postgresql")
                    {
                        global_write_to_console("");

                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("create database task execution running against server [ " + DBContract.postgresql + " ]...", TAG));

                        bool _exists_in_postgresql = postgresqlapisingleton.getInstance(_notificationmessageEventname).check_if_postgresql_database_exists(_database_name);

                        if (!_exists_in_postgresql)
                        {

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database existence check complete.", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] does not exists in " + DBContract.postgresql + ".", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("commencing with the database creation...", TAG));

                            responsedto _responsedto = postgresqlapisingleton.getInstance(_notificationmessageEventname).createdatabasegivennamefromconsole(_database_name);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                            postgresqlconnectionstringdto _connectionstringdto = postgresqlapisingleton.getInstance(_notificationmessageEventname).getpostgresqlconnectionstringdto();
                            _connectionstringdto.database = _database_name;
                            _connectionstringdto.new_database_name = _database_name;

                            _responsedto = postgresqlapisingleton.getInstance(_notificationmessageEventname).create_tables(_connectionstringdto);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                        }
                        else
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] exists in " + DBContract.postgresql + ".", TAG));
                        }
                        //Indicates that the iteration has ended
                        //break;
                    }
                    else if (_system == "sqlite")
                    {
                        global_write_to_console("");

                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("create database task execution running against server [ " + DBContract.sqlite + " ]...", TAG));

                        bool _exists_in_sqlite = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_sqlite_database_exists(_database_name);

                        if (!_exists_in_sqlite)
                        {

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database existence check complete.", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] does not exists in " + DBContract.sqlite + ".", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("commencing with the database creation...", TAG));

                            responsedto _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).createdatabasegivennamefromconsole(_database_name);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                            sqliteconnectionstringdto _connectionstringdto = sqliteapisingleton.getInstance(_notificationmessageEventname).getsqliteconnectionstringdto();
                            _connectionstringdto.database = _database_name;
                            _connectionstringdto.new_database_name = _database_name;

                            _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).createtables(_connectionstringdto);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                        }
                        else
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] exists in " + DBContract.sqlite + ".", TAG));
                        }
                        //Indicates that the iteration has ended
                        //break;
                    }
                    else if (_system == "mongodb")
                    {
                        global_write_to_console("");

                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("create database task execution running against server [ " + DBContract.sqlite + " ]...", TAG));

                        bool _exists_in_mongodb = mongodbapisingleton.getInstance(_notificationmessageEventname).check_if_mongodb_database_exists(_database_name);

                        if (!_exists_in_mongodb)
                        {

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database existence check complete.", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] does not exists in " + DBContract.sqlite + ".", TAG));

                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("commencing with the database creation...", TAG));

                            responsedto _responsedto = mongodbapisingleton.getInstance(_notificationmessageEventname).createdatabasegivennamefromconsole(_database_name);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                            mongodbconnectionstringdto _connectionstringdto = mongodbapisingleton.getInstance(_notificationmessageEventname).getmongodbconnectionstringdto();
                            _connectionstringdto.database = _database_name;
                            _connectionstringdto.new_database_name = _database_name;

                            _responsedto = mongodbapisingleton.getInstance(_notificationmessageEventname).create_tables(_connectionstringdto);

                            global_write_to_console(_responsedto.responsesuccessmessage);
                            global_write_to_console(_responsedto.responseerrormessage);

                        }
                        else
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("database with name [ " + _database_name + " ] exists in " + DBContract.mongodb + ".", TAG));
                        }
                        //Indicates that the iteration has ended
                        //break;
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("\nsystem value must either be: [ {0}, {1}, {2}, {3} ]", DBContract.mssql, DBContract.mysql, DBContract.postgresql, DBContract.sqlite), TAG));
                        global_write_to_console(String.Format("\nsystem value must either be: [ {0}, {1}, {2}, {3} ]", DBContract.mssql, DBContract.mysql, DBContract.postgresql, DBContract.sqlite));
                        _system = Console.ReadLine();
                        //continue;
                    }

                }

                show_prompt();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }

        DataTable getallrecordsfrommssql(string query)
        {
            try
            {

                DataTable mssql_dt = mssqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (mssql_dt != null && mssql_dt.Rows.Count != 0)
                {
                    //_notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mssql records count: " + mssql_dt.Rows.Count, TAG));
                }
                return mssql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        DataTable getallrecordsfrommysql(string query)
        {
            try
            {

                DataTable mysql_dt = mysqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (mysql_dt != null && mysql_dt.Rows.Count != 0)
                {
                    //_notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("mysql records count: " + mysql_dt.Rows.Count, TAG));
                }
                return mysql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        DataTable getallrecordsfromsqlite(string query)
        {
            try
            {

                DataTable sqlite_dt = sqliteapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (sqlite_dt != null && sqlite_dt.Rows.Count != 0)
                {
                    //_notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("sqlite records count: " + sqlite_dt.Rows.Count, TAG));
                }
                return sqlite_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        DataTable getallrecordsfrompostgresql(string query)
        {
            try
            {

                DataTable postgresql_dt = postgresqlapisingleton.getInstance(_notificationmessageEventname).getallrecordsglobal(query);
                if (postgresql_dt != null && postgresql_dt.Rows.Count != 0)
                {
                    //_notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("postgresql records count: " + postgresql_dt.Rows.Count, TAG));
                }
                return postgresql_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        DataTable get_all_logs_from_mongodb(string query)
        {
            try
            {

                DataTable mongodb_dt = mongodbapisingleton.getInstance(_notificationmessageEventname).get_all_logs();
                if (mongodb_dt != null && mongodb_dt.Rows.Count != 0)
                {
                    //_notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("postgresql records count: " + postgresql_dt.Rows.Count, TAG));
                }
                return mongodb_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        DataTable get_all_tasks_from_mongodb(string query)
        {
            try
            {

                DataTable mongodb_dt = mongodbapisingleton.getInstance(_notificationmessageEventname).get_all_tasks();
                if (mongodb_dt != null && mongodb_dt.Rows.Count != 0)
                {
                    //_notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("postgresql records count: " + postgresql_dt.Rows.Count, TAG));
                }
                return mongodb_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        DataTable get_all_users_from_mongodb(string query)
        {
            try
            {

                DataTable mongodb_dt = mongodbapisingleton.getInstance(_notificationmessageEventname).get_all_users();
                if (mongodb_dt != null && mongodb_dt.Rows.Count != 0)
                {
                    //_notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("postgresql records count: " + postgresql_dt.Rows.Count, TAG));
                }
                return mongodb_dt;

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                return null;
            }
        }

        void listallcliutils()
        {
            try
            {
                long _start = DateTime.Now.Ticks;
                DateTime _begin = DateTime.Now;

                list_tasks_cli_utils();
                list_logs_cli_utils();
                list_users_cli_utils();

                long _end = DateTime.Now.Ticks;
                DateTime _terminate = DateTime.Now;
                long _duration = _end - _start;
                TimeSpan _iduration = _terminate - _begin;

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Task took [ " + _iduration + " ] seconds.", TAG));

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void list_logs_cli_utils()
        {
            try
            {
                global_write_to_console("");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("fetch logs list task running...", TAG));

                string query = DBContract.logs_entity_table.SELECT_ALL_QUERY;

                DataTable mssql_dt = null;
                DataTable mysql_dt = null;
                DataTable sqlite_dt = null;
                DataTable postgresql_dt = null;
                DataTable mongodb_dt = null;

                black_bg_cyan_fg();

                mssql_dt = getallrecordsfrommssql(query);
                mysql_dt = getallrecordsfrommysql(query);
                sqlite_dt = getallrecordsfromsqlite(query);
                postgresql_dt = getallrecordsfrompostgresql(query);
                mongodb_dt = get_all_logs_from_mongodb(query);

                if (mssql_dt != null && mssql_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_logs(mssql_dt, DBContract.mssql);
                }
                if (mysql_dt != null && mysql_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_logs(mysql_dt, DBContract.mysql);
                }
                if (sqlite_dt != null && sqlite_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_logs(sqlite_dt, DBContract.sqlite);
                }
                if (postgresql_dt != null && postgresql_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_logs(postgresql_dt, DBContract.postgresql);
                }
                if (mongodb_dt != null && mongodb_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_logs(mongodb_dt, DBContract.mongodb);
                }

                black_bg_green_fg();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void print_logs(DataTable dt, string _datastore)
        {
            try
            {

                global_write_to_console("");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("begin printing records from [ " + _datastore + " ]...", TAG));

                var _recordscount = dt.Rows.Count;
                for (int i = 0; i < _recordscount; i++)
                {

                    log_dto _log_dto = new log_dto();
                    _log_dto.id = Convert.ToInt64(dt.Rows[i]["id"]);
                    _log_dto.message = Convert.ToString(dt.Rows[i]["message"]);
                    _log_dto.tag = Convert.ToString(dt.Rows[i]["tag"]);
                    _log_dto.timestamp = Convert.ToString(dt.Rows[i]["timestamp"]);
                    _log_dto.status = Convert.ToString(dt.Rows[i]["status"]);
                    _log_dto.created_date = Convert.ToString(dt.Rows[i]["created_date"]);

                    string _strlstdto = "id: " + String.Format("{0}", _log_dto.id) + "\t";
                    _strlstdto += "message: " + _log_dto.message + "\t";
                    _strlstdto += "tag: " + _log_dto.tag + "\t";
                    _strlstdto += "created date: " + _log_dto.created_date + "\n";

                    global_write_to_console(_strlstdto);
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("fetch logs list task finished. " + _datastore + " records count [ " + _recordscount + " ].", TAG));

                print_table_list_separator();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void create_user_cli_utils()
        {
            try
            {
                user_dto _user_dto = new user_dto();

                global_write_to_console("");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Loading create user interface...", TAG));

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Provide detail for given fields...", TAG));

                //name
                global_write_to_console("Name:");
                _user_dto.full_names = Console.ReadLine();

                while (String.IsNullOrEmpty(_user_dto.full_names))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Name cannot be null.", TAG));
                    global_write_to_console("Name:");
                    _user_dto.full_names = Console.ReadLine();
                }

                //email
                global_write_to_console("Email:");
                _user_dto.email = Console.ReadLine();

                while (String.IsNullOrEmpty(_user_dto.email))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Email cannot be null.", TAG));
                    global_write_to_console("Email:");
                    _user_dto.email = Console.ReadLine();
                }

                //password
                global_write_to_console("Password:");
                _user_dto.pass_word = Console.ReadLine();

                while (String.IsNullOrEmpty(_user_dto.pass_word))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Password cannot be null.", TAG));
                    global_write_to_console("Password:");
                    _user_dto.pass_word = Console.ReadLine();
                }

                string salt = Utils.create_random_salt();
                string salted_password = salt + _user_dto.pass_word;
                string password_salt_hash = Utils.get_SHA512_hash(salted_password);

                _user_dto.password_hash = password_salt_hash;
                _user_dto.password_salt = salt;

                _user_dto.pass_word = Utils.encrypt_string(_user_dto.pass_word);

                //secret word
                global_write_to_console("Secret Word:");
                _user_dto.secret_word = Console.ReadLine();

                while (String.IsNullOrEmpty(_user_dto.secret_word))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Secret Word cannot be null.", TAG));
                    global_write_to_console("Secret Word:");
                    _user_dto.secret_word = Console.ReadLine();
                }

                //gender
                global_write_to_console("\nGender: { 1: Male, 2: Female, 3: Prefer Not To Say  }");
                _user_dto.gender = Console.ReadLine();

                while (String.IsNullOrEmpty(_user_dto.gender))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Gender cannot be null.", TAG));
                    global_write_to_console("\n Gender: { 1: Male, 2: Female, 3: Prefer Not To Say }");
                    _user_dto.gender = Console.ReadLine();
                }

                while (!String.IsNullOrEmpty(_user_dto.gender))
                {
                    if (_user_dto.gender == "1")
                    {
                        _user_dto.gender = "Male";
                        //Indicates that the iteration has ended
                        break;
                    }
                    else if (_user_dto.gender == "2")
                    {
                        _user_dto.gender = "Female";
                        //Indicates that the iteration has ended
                        break;
                    }
                    else if (_user_dto.gender == "3")
                    {
                        _user_dto.gender = "Prefer Not To Say";
                        //Indicates that the iteration has ended
                        break;
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Gender value must either be 1, 2 or 3.", TAG));
                        global_write_to_console("\n Gender: { 1: Male, 2: Female, 3: Prefer Not To Say }");
                        _user_dto.gender = Console.ReadLine();
                        continue;
                    }
                }

                //date of birth
                global_write_to_console("\nDate of birth: format: dd-MM-yyyy");
                _user_dto.dob = Console.ReadLine();

                while (String.IsNullOrEmpty(_user_dto.dob))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Date of birth cannot be null.", TAG));
                    global_write_to_console("\n Date of birth: format: dd-MM-yyyy");
                    _user_dto.gender = Console.ReadLine();
                }

                while (!String.IsNullOrEmpty(_user_dto.dob))
                {
                    DateTime _dob = new DateTime();
                    bool is_datetime = DateTime.TryParse(_user_dto.dob, out _dob);

                    if (is_datetime)
                    {
                        _user_dto.year = _dob.Year.ToString();
                        _user_dto.month = _dob.Month.ToString();
                        _user_dto.day = _dob.Day.ToString();
                        //Indicates that the iteration has ended
                        break;
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Date of birth: format: dd-MM-yyyy.", TAG));
                        global_write_to_console("\n Date of birth: format: dd-MM-yyyy");
                        _user_dto.gender = Console.ReadLine();
                        continue;
                    }
                }

                //status
                _user_dto.status = "Active";
                //created date
                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");
                _user_dto.created_date = dateTimenow;

                global_write_to_console("");

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("insert record execution running....", TAG));

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.mssql + "....", TAG));

                bool _exists_in_mssql = mssqlapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_mssql)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.mssql + "\nproceeding with insertion....", TAG));

                    mssql_api.getInstance(_notificationmessageEventname).save_user_in_mssql(_user_dto);
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.mssql + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.sqlite + "....", TAG));

                bool _exists_in_sqlite = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_sqlite)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.sqlite + ",\nproceeding with insertion....", TAG));

                    sqlite_api.getInstance(_notificationmessageEventname).save_user_in_sqlite(_user_dto);
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.sqlite + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.mysql + "....", TAG));

                bool _exists_in_mysql = mysqlapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_mysql)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.mysql + ",\nproceeding with insertion....", TAG));

                    mysql_api.getInstance(_notificationmessageEventname).save_user_in_mysql(_user_dto);
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.mysql + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.postgresql + "....", TAG));

                bool _exists_in_postgresql = postgresqlapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_postgresql)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.postgresql + ",\nproceeding with insertion....", TAG));

                    postgresql_api.getInstance(_notificationmessageEventname).save_user_in_postgresql(_user_dto);

                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.postgresql + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.mongodb + "....", TAG));

                bool _exists_in_mongodb = mongodbapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_mongodb)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.mongodb + ",\nproceeding with insertion....", TAG));

                    mongodb_api.getInstance(_notificationmessageEventname).save_user_in_mongodb(_user_dto);

                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.postgresql + ".", TAG));
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void list_users_cli_utils()
        {
            try
            {
                global_write_to_console("");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("fetch users list task running...", TAG));

                string query = DBContract.users_entity_table.SELECT_ALL_QUERY;

                DataTable mssql_dt = null;
                DataTable mysql_dt = null;
                DataTable sqlite_dt = null;
                DataTable postgresql_dt = null;
                DataTable mongodb_dt = null;

                black_bg_cyan_fg();

                mssql_dt = getallrecordsfrommssql(query);
                mysql_dt = getallrecordsfrommysql(query);
                sqlite_dt = getallrecordsfromsqlite(query);
                postgresql_dt = getallrecordsfrompostgresql(query);
                mongodb_dt = get_all_users_from_mongodb(query);

                if (mssql_dt != null && mssql_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_users(mssql_dt, DBContract.mssql);
                }
                if (mysql_dt != null && mysql_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_users(mysql_dt, DBContract.mysql);
                }
                if (sqlite_dt != null && sqlite_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_users(sqlite_dt, DBContract.sqlite);
                }
                if (postgresql_dt != null && postgresql_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_users(postgresql_dt, DBContract.postgresql);
                }
                if (mongodb_dt != null && mongodb_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_users(mongodb_dt, DBContract.mongodb);
                }

                black_bg_green_fg();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void print_users(DataTable dt, string _datastore)
        {
            try
            {

                global_write_to_console("");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("begin printing records from [ " + _datastore + " ]...", TAG));

                var _recordscount = dt.Rows.Count;
                for (int i = 0; i < _recordscount; i++)
                {
                    user_dto _dto = new user_dto();
                    _dto.id = Convert.ToInt64(dt.Rows[i]["id"]);
                    _dto.email = Convert.ToString(dt.Rows[i]["email"]);
                    _dto.pass_word = Convert.ToString(dt.Rows[i]["pass_word"]);
                    _dto.full_names = Convert.ToString(dt.Rows[i]["full_names"]);
                    _dto.year = Convert.ToString(dt.Rows[i]["year"]);
                    _dto.month = Convert.ToString(dt.Rows[i]["month"]);
                    _dto.day = Convert.ToString(dt.Rows[i]["day"]);
                    _dto.gender = Convert.ToString(dt.Rows[i]["gender"]);
                    _dto.status = Convert.ToString(dt.Rows[i]["status"]);
                    _dto.created_date = Convert.ToString(dt.Rows[i]["created_date"]);

                    string _strlstdto = "id: " + String.Format("{0}", _dto.id) + "\t";
                    _strlstdto += "name: " + _dto.full_names + "\t";
                    _strlstdto += "email: " + _dto.email + "\t";
                    _strlstdto += "created date: " + _dto.created_date + "\n";

                    global_write_to_console(_strlstdto);
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("fetch tasks list task finished. " + _datastore + " records count [ " + _recordscount + " ].", TAG));

                print_table_list_separator();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void create_task_cli_utils()
        {
            try
            {
                task_dto _task_dto = new task_dto();

                global_write_to_console("");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Loading create task interface...", TAG));

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Provide detail for given fields...", TAG));

                global_write_to_console("name:");
                _task_dto.name = Console.ReadLine();

                while (String.IsNullOrEmpty(_task_dto.name))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("name cannot be null.", TAG));
                    global_write_to_console("name:");
                    _task_dto.name = Console.ReadLine();
                }

                //global_write_to_console("description:");
                //_task_dto.description = Console.ReadLine();

                //while (String.IsNullOrEmpty(_task_dto.description))
                //{
                //    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("description cannot be null.", TAG));
                //    global_write_to_console("description:");
                //    _task_dto.description = Console.ReadLine();
                //}

                //global_write_to_console("expected completion date:");
                //_task_dto.expected_completion_date = Console.ReadLine();

                //while (String.IsNullOrEmpty(_task_dto.expected_completion_date))
                //{
                //    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("expected completion date cannot be null.", TAG));
                //    global_write_to_console("expected completion date:");
                //    _task_dto.expected_completion_date = Console.ReadLine();
                //}

                _task_dto.description = _task_dto.name;
                _task_dto.expected_completion_date = DateTime.Now.AddMonths(4).ToString("dd-MM-yyyy");
                _task_dto.actual_completion_date = "";

                _task_dto.status = "active";

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");
                _task_dto.created_date = dateTimenow;

                global_write_to_console("");

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("insert record execution running....", TAG));

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if task with name [ " + _task_dto.name + " ] exists in " + DBContract.mssql + "....", TAG));

                bool _exists_in_mssql = mssqlapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(_task_dto.name);

                if (!_exists_in_mssql)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] does not exists in " + DBContract.mssql + "\nproceeding with insertion....", TAG));

                    mssql_api.getInstance(_notificationmessageEventname).save_task_in_mssql(_task_dto);
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] exists in " + DBContract.mssql + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if task with name [ " + _task_dto.name + " ] exists in " + DBContract.sqlite + "....", TAG));

                bool _exists_in_sqlite = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(_task_dto.name);

                if (!_exists_in_sqlite)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] does not exists in " + DBContract.sqlite + ",\nproceeding with insertion....", TAG));

                    sqlite_api.getInstance(_notificationmessageEventname).save_task_in_sqlite(_task_dto);
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] exists in " + DBContract.sqlite + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if task with name [ " + _task_dto.name + " ] exists in " + DBContract.mysql + "....", TAG));

                bool _exists_in_mysql = mysqlapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(_task_dto.name);

                if (!_exists_in_mysql)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] does not exists in " + DBContract.mysql + ",\nproceeding with insertion....", TAG));

                    mysql_api.getInstance(_notificationmessageEventname).save_task_in_mysql(_task_dto);
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] exists in " + DBContract.mysql + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if task with name [ " + _task_dto.name + " ] exists in " + DBContract.postgresql + "....", TAG));

                bool _exists_in_postgresql = postgresqlapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(_task_dto.name);

                if (!_exists_in_postgresql)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] does not exists in " + DBContract.postgresql + ",\nproceeding with insertion....", TAG));

                    postgresql_api.getInstance(_notificationmessageEventname).save_task_in_postgresql(_task_dto);

                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] exists in " + DBContract.postgresql + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if task with name [ " + _task_dto.name + " ] exists in " + DBContract.mongodb + "....", TAG));

                bool _exists_in_mongodb = mongodbapisingleton.getInstance(_notificationmessageEventname).check_if_task_exists(_task_dto.name);

                if (!_exists_in_mongodb)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] does not exists in " + DBContract.mongodb + ",\nproceeding with insertion....", TAG));

                    mongodb_api.getInstance(_notificationmessageEventname).save_task_in_mongodb(_task_dto);

                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("task with name [ " + _task_dto.name + " ] exists in " + DBContract.postgresql + ".", TAG));
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void list_tasks_cli_utils()
        {
            try
            {
                global_write_to_console("");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Fetch tasks list task running...", TAG));

                string query = DBContract.tasks_entity_table.SELECT_ALL_QUERY;

                DataTable mssql_dt = null;
                DataTable mysql_dt = null;
                DataTable sqlite_dt = null;
                DataTable postgresql_dt = null;
                DataTable mongodb_dt = null;

                black_bg_cyan_fg();

                mssql_dt = getallrecordsfrommssql(query);
                mysql_dt = getallrecordsfrommysql(query);
                sqlite_dt = getallrecordsfromsqlite(query);
                postgresql_dt = getallrecordsfrompostgresql(query);
                mongodb_dt = get_all_tasks_from_mongodb(query);

                if (mssql_dt != null && mssql_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_tasks(mssql_dt, DBContract.mssql);
                }
                if (mysql_dt != null && mysql_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_tasks(mysql_dt, DBContract.mysql);
                }
                if (sqlite_dt != null && sqlite_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_tasks(sqlite_dt, DBContract.sqlite);
                }
                if (postgresql_dt != null && postgresql_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_tasks(postgresql_dt, DBContract.postgresql);
                }
                if (mongodb_dt != null && mongodb_dt.Rows.Count != 0)
                {
                    print_table_list_separator();
                    print_tasks(mongodb_dt, DBContract.mongodb);
                }

                black_bg_green_fg();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void print_tasks(DataTable dt, string _datastore)
        {
            try
            {

                global_write_to_console("");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Begin printing records from [ " + _datastore + " ]...", TAG));

                var _recordscount = dt.Rows.Count;
                for (int i = 0; i < _recordscount; i++)
                {
                    task_dto _dto = new task_dto();
                    _dto.id = Convert.ToInt64(dt.Rows[i]["id"]);
                    _dto.name = Convert.ToString(dt.Rows[i]["name"]);
                    _dto.description = Convert.ToString(dt.Rows[i]["description"]);
                    _dto.expected_completion_date = Convert.ToString(dt.Rows[i]["expected_completion_date"]);
                    _dto.actual_completion_date = Convert.ToString(dt.Rows[i]["actual_completion_date"]);
                    _dto.status = Convert.ToString(dt.Rows[i]["status"]);
                    _dto.created_date = Convert.ToString(dt.Rows[i]["created_date"]);

                    string _strlstdto = "id: " + String.Format("{0}", _dto.id) + "\t";
                    _strlstdto += "name: " + _dto.name + "\t";
                    _strlstdto += "description: " + _dto.description + "\t";
                    _strlstdto += "expected completion date: " + _dto.expected_completion_date + "\t";
                    _strlstdto += "created date: " + _dto.created_date + "\n";

                    global_write_to_console(_strlstdto);
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Fetch tasks list task finished. " + _datastore + " records count [ " + _recordscount + " ].", TAG));

                print_table_list_separator();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void startnewconsolecliutils()
        {
            try
            {
                string base_dir = Environment.CurrentDirectory;
                string process_name = System.Configuration.ConfigurationManager.AppSettings["CONSOLE_APP_NAME"];
                string full_process_name_with_path = base_dir + @"\" + process_name;

                string _process_path = @full_process_name_with_path;

                var _process_info = Process.Start(_process_path);

                global_write_to_console(String.Format("Successfully started process [ {0} ] with id [ {1} ] at [ {2} ] took [ {3} ] seconds.", _process_info.StartInfo.FileName, _process_info.Id, _process_info.StartTime, _process_info.TotalProcessorTime));

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void showconnectionsinfocliutils()
        {
            try
            {
                checkmssqlconnectionstate();
                checkmysqlconnectionstate();
                checkpostgresqlconnectionstate();
                checksqliteconnectionstate();
                checkmongodbconnectionstate();
            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void checkmssqlconnectionstate()
        {
            try
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking mssql connection state..."));

                responsedto _responsedto = mssqlapisingleton.getInstance(_notificationmessageEventname).checkmssqlconnectionstate();

                if (_responsedto.isresponseresultsuccessful)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responsesuccessmessage)));

                }
                else
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responseerrormessage)));

                }

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void checkmysqlconnectionstate()
        {
            try
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking mysql connection state..."));

                responsedto _responsedto = mysqlapisingleton.getInstance(_notificationmessageEventname).checkmysqlconnectionstate();

                if (_responsedto.isresponseresultsuccessful)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responsesuccessmessage)));

                }
                else
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responseerrormessage)));

                }

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void checkpostgresqlconnectionstate()
        {
            try
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking postgresql connection state..."));

                responsedto _responsedto = postgresqlapisingleton.getInstance(_notificationmessageEventname).checkpostgresqlconnectionstate();

                if (_responsedto.isresponseresultsuccessful)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responsesuccessmessage)));

                }
                else
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responseerrormessage)));

                }

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void checksqliteconnectionstate()
        {
            try
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking sqlite connection state..."));

                responsedto _responsedto = sqliteapisingleton.getInstance(_notificationmessageEventname).checksqliteconnectionstate();

                if (_responsedto.isresponseresultsuccessful)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responsesuccessmessage)));

                }
                else
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responseerrormessage)));

                }

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void checkmongodbconnectionstate()
        {
            try
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking mongodb connection state..."));

                responsedto _responsedto = mongodbapisingleton.getInstance(_notificationmessageEventname).checkmongodbconnectionstate();

                if (_responsedto.isresponseresultsuccessful)
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responsesuccessmessage)));

                }
                else
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("{0}", _responsedto.responseerrormessage)));

                }

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
            }
        }

        void changeconfigurations()
        {
            try
            {
                string _base_dir = Environment.CurrentDirectory;
                string _base_dir_with_filename = Application.ExecutablePath;
                string[] _path_levels_arr = _base_dir_with_filename.Split(Path.DirectorySeparatorChar);
                int _file_name_index_in_path = _path_levels_arr.Length - 1;
                string _exe_file_name = _path_levels_arr[_file_name_index_in_path];

                System.Configuration.Configuration _Configuration = System.Configuration.ConfigurationManager.OpenExeConfiguration(_base_dir_with_filename);

                bool _isuserdetailsvalid = true;
                string _errormsg = "";

                //if (String.IsNullOrEmpty(txtsysmssqldatasource.Text))
                //{
                //    _isuserdetailsvalid = false;
                //    _errormsg += "server name cannot be null.";
                //    _notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs("server name cannot be null.", TAG));
                //    errorProvider.SetError(txtsysmssqldatasource, "server name cannot be null.");
                //}
                //if (String.IsNullOrEmpty(txtsysmssqldatabase.Text))
                //{
                //    _isuserdetailsvalid = false;
                //    _errormsg += Environment.NewLine + "database name cannot be null.";
                //    _notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs("database name cannot be null.", TAG));
                //    errorProvider.SetError(txtsysmssqldatabase, "database name cannot be null.");
                //}
                //if (String.IsNullOrEmpty(txtsysmssqlusername.Text))
                //{
                //    _isuserdetailsvalid = false;
                //    _errormsg += Environment.NewLine + "user name cannot be null.";
                //    _notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs("user name cannot be null.", TAG));
                //    errorProvider.SetError(txtsysmssqlusername, "user name cannot be null.");
                //}
                //if (String.IsNullOrEmpty(txtsysmssqlpassword.Text))
                //{
                //    _isuserdetailsvalid = false;
                //    _errormsg += Environment.NewLine + "password cannot be null.";
                //    _notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs("password cannot be null.", TAG));
                //    errorProvider.SetError(txtsysmssqlpassword, "password cannot be null.");
                //}
                //if (String.IsNullOrEmpty(txtsysmssqlport.Text))
                //{
                //    _isuserdetailsvalid = false;
                //    _errormsg += Environment.NewLine + "port cannot be null.";
                //    _notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs("port cannot be null.", TAG));
                //    errorProvider.SetError(txtsysmssqlport, "port cannot be null.");
                //}

                if (_isuserdetailsvalid)
                {

                    mssqlconnectionstringdto _connectionstringdto = new mssqlconnectionstringdto();
                    //_connectionstringdto.datasource = txtsysmssqldatasource.Text;
                    //_connectionstringdto.database = txtsysmssqldatabase.Text;
                    //_connectionstringdto.userid = txtsysmssqlusername.Text;
                    //_connectionstringdto.password = txtsysmssqlpassword.Text;
                    //_connectionstringdto.port = txtsysmssqlport.Text;

                    _Configuration.AppSettings.Settings.Remove("mssql_datasource");
                    _Configuration.AppSettings.Settings.Remove("mssql_database");
                    _Configuration.AppSettings.Settings.Remove("mssql_userid");
                    _Configuration.AppSettings.Settings.Remove("mssql_password");
                    _Configuration.AppSettings.Settings.Remove("mssql_port");

                    _Configuration.AppSettings.Settings.Add("mssql_datasource", _connectionstringdto.datasource);
                    _Configuration.AppSettings.Settings.Add("mssql_database", _connectionstringdto.database);
                    _Configuration.AppSettings.Settings.Add("mssql_userid", _connectionstringdto.userid);
                    _Configuration.AppSettings.Settings.Add("mssql_password", _connectionstringdto.password);
                    _Configuration.AppSettings.Settings.Add("mssql_port", _connectionstringdto.port);

                    _Configuration.Save();

                    System.Configuration.ConfigurationManager.RefreshSection("AppSettings");

                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(DBContract.mssql + " configurations persisted successfully."));

                }
                else
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(_errormsg));
                }

            }
            catch (Exception ex)
            {
                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message));
            }
        }

        void copy_data()
        {
            try
            {
                long _start = DateTime.Now.Ticks;
                DateTime _begin = DateTime.Now;

                string serverfrom = string.Empty;
                string _database_name = string.Empty;
                string serverto = string.Empty;
                string databaseto = string.Empty;
                bool _database_exists = false;

                //string _database_name = "";
                //string _system = "";

                global_write_to_console("");
                global_write_to_console("Loading Copy Data Interface...");

                global_write_to_console("Provide Detail for Given Fields...");

                //let the user select the server to copy from.
                global_write_to_console(String.Format("\nServer From: [ {0}, {1}, {2}, {3} ]", DBContract.mssql, DBContract.mysql, DBContract.postgresql, DBContract.sqlite));
                serverfrom = Console.ReadLine();

                //loop until the user provides a server
                while (String.IsNullOrEmpty(serverfrom))
                {
                    global_write_to_console("System cannot be null.");
                    global_write_to_console(String.Format("\nServer From: [ {0}, {1}, {2}, {3} ]", DBContract.mssql, DBContract.mysql, DBContract.postgresql, DBContract.sqlite));
                    serverfrom = Console.ReadLine();
                }

                while (!String.IsNullOrEmpty(serverfrom))
                {
                    //check the selected server
                    if (serverfrom == "mssql")
                    {
                        global_write_to_console("");
                        global_write_to_console("fetching a list of databases...");
                        global_write_to_console("select from the folowing databases.");
                        List<string> databases = data_utils.get_mssql_databases();
                        global_write_to_console("");
                        foreach (string db in databases)
                        {
                            //list all the available table for user to choose from.
                            global_write_to_console(db);
                        }
                        //get the database
                        global_write_to_console("database name:");
                        global_write_to_console("");
                        //wait for the user to press the enter key.
                        _database_name = Console.ReadLine();

                        //loop until the user gives a database.
                        while (String.IsNullOrEmpty(_database_name))
                        {
                            global_write_to_console("database name cannot be null.");

                            global_write_to_console("");
                            global_write_to_console("fetching a list of databases...");
                            global_write_to_console("select from the folowing databases.");
                            global_write_to_console("");
                            foreach (string db in databases)
                            {
                                //list all the available table for user to choose from.
                                global_write_to_console(db);
                            }

                            global_write_to_console("database name:");
                            _database_name = Console.ReadLine();
                        }

                        global_write_to_console("checking if the database exists...");

                        //check if the provided database exists.
                        _database_exists = mssqlapisingleton.getInstance(_notificationmessageEventname).check_if_mssql_database_exists(_database_name);

                        while (!_database_exists)
                        {
                            global_write_to_console("database with name [ " + _database_name + " ] does not exists in " + DBContract.mssql + ".");

                            global_write_to_console("");
                            global_write_to_console("fetching a list of databases...");
                            global_write_to_console("select from the folowing databases.");
                            global_write_to_console("");
                            foreach (string db in databases)
                            {
                                //list all the available table for user to choose from.
                                global_write_to_console(db);
                            }

                            //get the database
                            global_write_to_console("database name:");
                            _database_name = Console.ReadLine();

                            global_write_to_console("checking if the database exists...");

                            //check if the provided database exists.
                            _database_exists = mssqlapisingleton.getInstance(_notificationmessageEventname).check_if_mssql_database_exists(_database_name);
                        }

                        //Indicates that the iteration has ended
                        global_write_to_console("database selected [ " + _database_name + " ]");
                        break;
                    }
                    else if (serverfrom == "mysql")
                    {
                        global_write_to_console("");
                        global_write_to_console("fetching a list of databases...");
                        global_write_to_console("select from the folowing databases.");
                        List<string> databases = data_utils.get_mysql_databases();
                        global_write_to_console("");
                        foreach (string db in databases)
                        {
                            //list all the available table for user to choose from.
                            global_write_to_console(db);
                        }
                        //get the database
                        global_write_to_console("database name:");
                        global_write_to_console("");
                        //wait for the user to press the enter key.
                        _database_name = Console.ReadLine();

                        //loop until the user gives a database.
                        while (String.IsNullOrEmpty(_database_name))
                        {
                            global_write_to_console("database name cannot be null.");

                            global_write_to_console("");
                            global_write_to_console("fetching a list of databases...");
                            global_write_to_console("select from the folowing databases.");
                            global_write_to_console("");
                            foreach (string db in databases)
                            {
                                //list all the available table for user to choose from.
                                global_write_to_console(db);
                            }

                            global_write_to_console("database name:");
                            _database_name = Console.ReadLine();
                        }

                        global_write_to_console("checking if the database exists...");

                        //check if the provided database exists.
                        _database_exists = mysqlapisingleton.getInstance(_notificationmessageEventname).check_if_mysql_database_exists(_database_name);

                        while (!_database_exists)
                        {
                            global_write_to_console("database with name [ " + _database_name + " ] does not exists in " + DBContract.mysql + ".");

                            global_write_to_console("");
                            global_write_to_console("fetching a list of databases...");
                            global_write_to_console("select from the folowing databases.");
                            global_write_to_console("");
                            foreach (string db in databases)
                            {
                                //list all the available table for user to choose from.
                                global_write_to_console(db);
                            }

                            //get the database
                            global_write_to_console("database name:");
                            _database_name = Console.ReadLine();

                            global_write_to_console("checking if the database exists...");

                            //check if the provided database exists.
                            _database_exists = mysqlapisingleton.getInstance(_notificationmessageEventname).check_if_mysql_database_exists(_database_name);
                        }

                        //Indicates that the iteration has ended
                        global_write_to_console("database selected [ " + _database_name + " ]");
                        break;
                    }
                    else if (serverfrom == "postgresql")
                    {
                        global_write_to_console("");
                        global_write_to_console("fetching a list of databases...");
                        global_write_to_console("select from the folowing databases.");
                        List<string> databases = data_utils.get_postgresql_databases();
                        global_write_to_console("");
                        foreach (string db in databases)
                        {
                            //list all the available table for user to choose from.
                            global_write_to_console(db);
                        }
                        //get the database
                        global_write_to_console("database name:");
                        global_write_to_console("");
                        //wait for the user to press the enter key.
                        _database_name = Console.ReadLine();

                        //loop until the user gives a database.
                        while (String.IsNullOrEmpty(_database_name))
                        {
                            global_write_to_console("database name cannot be null.");

                            global_write_to_console("");
                            global_write_to_console("fetching a list of databases...");
                            global_write_to_console("select from the folowing databases.");
                            global_write_to_console("");
                            foreach (string db in databases)
                            {
                                //list all the available table for user to choose from.
                                global_write_to_console(db);
                            }

                            global_write_to_console("database name:");
                            _database_name = Console.ReadLine();
                        }

                        global_write_to_console("checking if the database exists...");

                        //check if the provided database exists.
                        _database_exists = postgresqlapisingleton.getInstance(_notificationmessageEventname).check_if_postgresql_database_exists(_database_name);

                        while (!_database_exists)
                        {
                            global_write_to_console("database with name [ " + _database_name + " ] does not exists in " + DBContract.postgresql + ".");

                            global_write_to_console("");
                            global_write_to_console("fetching a list of databases...");
                            global_write_to_console("select from the folowing databases.");
                            global_write_to_console("");
                            foreach (string db in databases)
                            {
                                //list all the available table for user to choose from.
                                global_write_to_console(db);
                            }

                            //get the database
                            global_write_to_console("database name:");
                            _database_name = Console.ReadLine();

                            global_write_to_console("checking if the database exists...");

                            //check if the provided database exists.
                            _database_exists = postgresqlapisingleton.getInstance(_notificationmessageEventname).check_if_postgresql_database_exists(_database_name);
                        }

                        //Indicates that the iteration has ended
                        global_write_to_console("database selected [ " + _database_name + " ]");
                        break;
                    }
                    else if (serverfrom == "sqlite")
                    {
                        global_write_to_console("");
                        global_write_to_console("fetching a list of databases...");
                        global_write_to_console("select from the folowing databases.");
                        List<string> databases = data_utils.get_sqlite_databases();
                        global_write_to_console("");
                        foreach (string db in databases)
                        {
                            //list all the available table for user to choose from.
                            global_write_to_console(db);
                        }
                        //get the database
                        global_write_to_console("database name:");
                        global_write_to_console("");
                        //wait for the user to press the enter key.
                        _database_name = Console.ReadLine();

                        //loop until the user gives a database.
                        while (String.IsNullOrEmpty(_database_name))
                        {
                            global_write_to_console("database name cannot be null.");

                            global_write_to_console("");
                            global_write_to_console("fetching a list of databases...");
                            global_write_to_console("select from the folowing databases.");
                            global_write_to_console("");
                            foreach (string db in databases)
                            {
                                //list all the available table for user to choose from.
                                global_write_to_console(db);
                            }

                            global_write_to_console("database name:");
                            _database_name = Console.ReadLine();
                        }

                        global_write_to_console("checking if the database exists...");

                        //check if the provided database exists.
                        _database_exists = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_sqlite_database_exists(_database_name);

                        while (!_database_exists)
                        {
                            global_write_to_console("database with name [ " + _database_name + " ] does not exists in " + DBContract.sqlite + ".");

                            global_write_to_console("");
                            global_write_to_console("fetching a list of databases...");
                            global_write_to_console("select from the folowing databases.");
                            global_write_to_console("");
                            foreach (string db in databases)
                            {
                                //list all the available table for user to choose from.
                                global_write_to_console(db);
                            }

                            //get the database
                            global_write_to_console("database name:");
                            _database_name = Console.ReadLine();

                            global_write_to_console("checking if the database exists...");

                            //check if the provided database exists.
                            _database_exists = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_sqlite_database_exists(_database_name);
                        }

                        //Indicates that the iteration has ended
                        global_write_to_console("database selected [ " + _database_name + " ]");
                        break;
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("\nsystem value must either be: [ {0}, {1}, {2}, {3} ]", DBContract.mssql, DBContract.mysql, DBContract.postgresql, DBContract.sqlite), TAG));
                        global_write_to_console(String.Format("\nsystem value must either be: [ {0}, {1}, {2}, {3} ]", DBContract.mssql, DBContract.mysql, DBContract.postgresql, DBContract.sqlite));
                        serverfrom = Console.ReadLine();
                        continue;
                    }

                }

                dbsyncdto _dbsyncdto = new dbsyncdto();
                _dbsyncdto.serverfrom = serverfrom.Trim();
                _dbsyncdto.databasefrom = _database_name.Trim();
                _dbsyncdto.serverto = serverto.Trim();
                _dbsyncdto.databaseto = _database_name.Trim();

                copy_data_utils datautils = new copy_data_utils(_notificationmessageEventname, _progressBarNotificationEventname);
                datautils.sync_data_from_source_to_destination(_dbsyncdto);

                long _end = DateTime.Now.Ticks;
                DateTime _terminate = DateTime.Now;
                long _duration = _end - _start;
                TimeSpan _iduration = _terminate - _begin;

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Task took [ " + _iduration + " ] seconds.", TAG));

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                Console.ForegroundColor = ConsoleColor.Green;
            }
        }

        void backup_database_cli_utils()
        {
            try
            {
                bool use_integrated_security = false;
                string user_name = "";//txtusername.Text;
                string password = "";// txtpassword.Text;
                //string server = "";// cboserver.Text;
                string master_database = "master";
                string CONNECTION_STRING = string.Empty;
                string DatabaseName = "";

                long _start = DateTime.Now.Ticks;
                DateTime _begin = DateTime.Now;

                string serverfrom = string.Empty;
                string _database_name = string.Empty;
                string _server_name = string.Empty;
                string serverto = string.Empty;
                string databaseto = string.Empty;
                bool _database_exists = false;
                bool _server_connected = false;

                global_write_to_console("");
                global_write_to_console("Loading Backup Database Interface...");

                global_write_to_console("Provide Detail for Given Fields...");

                global_write_to_console("");
                global_write_to_console("Select from the following servers.");
                global_write_to_console("Fetching a list of servers...");

                List<string> servers = get_mssql_server_names();

                global_write_to_console("");

                foreach (string srv in servers)
                {
                    //list all the available servers for user to choose from.
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    global_write_to_console(srv);
                    Console.ForegroundColor = ConsoleColor.Green;
                }

                //get the server
                global_write_to_console("Server name:");
                global_write_to_console("");

                //wait for the user to press the enter key.
                _server_name = Console.ReadLine();

                //loop until the user gives a server.
                while (String.IsNullOrEmpty(_server_name))
                {
                    global_write_to_console("Server name cannot be null.");

                    global_write_to_console("");
                    global_write_to_console("Select from the following servers.");
                    global_write_to_console("Fetching a list of servers...");
                    global_write_to_console("");

                    foreach (string srv in servers)
                    {
                        //list all the available server for user to choose from.
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        global_write_to_console(srv);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    global_write_to_console("Server name:");
                    _server_name = Console.ReadLine();
                }

                global_write_to_console("Connecting to the server...");

                //email
                global_write_to_console("User Name:");
                user_name = Console.ReadLine();

                while (String.IsNullOrEmpty(user_name))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("User Name cannot be null.", TAG));
                    global_write_to_console("User Name:");
                    user_name = Console.ReadLine();
                }

                //password
                global_write_to_console("Password:");
                password = Console.ReadLine();

                while (String.IsNullOrEmpty(password))
                {
                    _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Password cannot be null.", TAG));
                    global_write_to_console("Password:");
                    password = Console.ReadLine();
                }

                //connect to the provided server.
                _server_connected = Connect_To_mssql_Server(_server_name, user_name, password, false);

                while (!_server_connected)
                {
                    global_write_to_console("Cannot connect to server [ " + _server_name + " ].");

                    global_write_to_console("");
                    global_write_to_console("Select from the following servers.");
                    global_write_to_console("");

                    foreach (string srv in servers)
                    {
                        //list all the available server for user to choose from.                        
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        global_write_to_console(srv);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    //get the server
                    global_write_to_console("server name:");
                    _server_name = Console.ReadLine();

                    global_write_to_console("Connecting to the server...");

                    //email
                    global_write_to_console("User Name:");
                    user_name = Console.ReadLine();

                    while (String.IsNullOrEmpty(user_name))
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("User Name cannot be null.", TAG));
                        global_write_to_console("User Name:");
                        user_name = Console.ReadLine();
                    }

                    //password
                    global_write_to_console("Password:");
                    password = Console.ReadLine();

                    while (String.IsNullOrEmpty(password))
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Password cannot be null.", TAG));
                        global_write_to_console("Password:");
                        password = Console.ReadLine();
                    }

                    //connect to the provided server.
                    _server_connected = Connect_To_mssql_Server(_server_name, user_name, password, false);

                }

                //Indicates that the iteration has ended
                global_write_to_console("Server selected [ " + _server_name + " ]");



                /**database**/
                global_write_to_console("");
                global_write_to_console("Select from the following databases.");
                global_write_to_console("Fetching a list of databases...");
                global_write_to_console("");

                List<string> databases = data_utils.get_mssql_databases();

                foreach (string db in databases)
                {
                    //list all the available table for user to choose from.
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    global_write_to_console(db);
                    Console.ForegroundColor = ConsoleColor.Green;

                }

                global_write_to_console("");

                //get the database
                global_write_to_console("Database name:");
                global_write_to_console("");
                //wait for the user to press the enter key.
                _database_name = Console.ReadLine();

                //loop until the user gives a database.
                while (String.IsNullOrEmpty(_database_name))
                {
                    global_write_to_console("Database name cannot be null.");

                    global_write_to_console("");
                    global_write_to_console("Select from the following databases.");
                    global_write_to_console("Fetching a list of databases...");
                    global_write_to_console("");

                    foreach (string db in databases)
                    {
                        //list all the available table for user to choose from.
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        global_write_to_console(db);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    global_write_to_console("Database name:");
                    _database_name = Console.ReadLine();
                }

                global_write_to_console("Checking if the database exists...");

                //check if the provided database exists.
                _database_exists = mssqlapisingleton.getInstance(_notificationmessageEventname).check_if_mssql_database_exists(_database_name);

                while (!_database_exists)
                {
                    global_write_to_console("Database with name [ " + _database_name + " ] does not exists in " + DBContract.mssql + ".");

                    global_write_to_console("");
                    global_write_to_console("Fetching a list of databases...");
                    global_write_to_console("Select from the following databases.");
                    global_write_to_console("");

                    foreach (string db in databases)
                    {
                        //list all the available table for user to choose from.
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        global_write_to_console(db);
                        Console.ForegroundColor = ConsoleColor.Green;
                    }

                    //get the database
                    global_write_to_console("Database name:");
                    _database_name = Console.ReadLine();

                    global_write_to_console("Checking if the database exists...");

                    //check if the provided database exists.
                    _database_exists = mssqlapisingleton.getInstance(_notificationmessageEventname).check_if_mssql_database_exists(_database_name);
                }

                //Indicates that the iteration has ended
                global_write_to_console("Database selected [ " + _database_name + " ]");



                // Connection 
                if (use_integrated_security)
                    CONNECTION_STRING = @"Data Source=" + _server_name + ";" +
                                        "Database=" + master_database + ";" +
                                        "Integrated Security=True";
                else
                    CONNECTION_STRING = @"Data Source=" + _server_name + ";" +
                                        "Database=" + master_database + ";" +
                                        "User Id=" + user_name + ";" +
                                        "Password=" + password;


                SqlConnection con = new SqlConnection(CONNECTION_STRING);
                con.Open(); //OPEN ACCESS TO DATABASE
                SqlCommand cmd = new SqlCommand(CONNECTION_STRING, con);

                DatabaseName = _database_name;

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("yyyy_MM_dd_HH_mm_ss");
                string formated_backup_db_name = DatabaseName + "_" + dateTimenow;
                string backup_db_name = formated_backup_db_name + ".BAK";
                string backup_folder = @"C:\database_backups\online_ip_telephony";
                string backup_file = Path.Combine(backup_folder, backup_db_name);

                //CREATE DIRECTORY IF NOT EXIST
                if (!Directory.Exists(backup_folder))
                {
                    Directory.CreateDirectory(backup_folder);
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "BACKUP DATABASE " + DatabaseName + " TO DISK = '" + backup_file + "'";
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Database backup successfull. ");
                    Console.WriteLine("Backup Database [ " + DatabaseName + " ].");
                    Console.WriteLine("Backup File [ " + backup_file + " ].");
                    //MessageBox.Show(@"Database backup successfull." + Environment.NewLine + backup_file, "Backup Database - " + DatabaseName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "BACKUP DATABASE " + DatabaseName + " TO DISK = '" + backup_file + "'";
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();

                    Console.WriteLine("Database backup successfull. ");
                    Console.WriteLine("Backup Database [ " + DatabaseName + " ].");
                    Console.WriteLine("Backup File [ " + backup_file + " ].");
                    //MessageBox.Show(@"Database backup successfull." + Environment.NewLine + backup_file, "Backup Database - " + DatabaseName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (Directory.Exists(backup_folder))
                {
                    string _filetoSelect = Path.Combine(backup_folder, backup_db_name);
                    // opens the folder in explorer and selects the displayed file
                    string args = string.Format("/Select, {0}", _filetoSelect);
                    ProcessStartInfo pfi = new ProcessStartInfo("Explorer.exe", args);
                    System.Diagnostics.Process.Start(pfi);
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = ConsoleColor.Green;
                Utils.LogEventViewer(ex);
                Log.Write_To_Log_File_temp_dir(ex);
                //MessageBox.Show(ex.Message + Environment.NewLine + "Please contact the developer", " Database Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        List<string> get_mssql_server_names()
        {
            List<string> srvNames = new List<string>();

            DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string InstanceName = dr["InstanceName"].ToString();
                    string ServerName = dr["ServerName"].ToString();

                    if (string.IsNullOrEmpty(InstanceName) || string.IsNullOrEmpty(ServerName))
                    {
                        srvNames.Add(ServerName);
                    }

                    if (!string.IsNullOrEmpty(InstanceName) && !string.IsNullOrEmpty(ServerName))
                    {
                        ServerName = ServerName + "\\" + InstanceName;
                        srvNames.Add(ServerName);

                    }
                }

                int count = srvNames.Count;
                Console.WriteLine("servers: " + count);
            }
            else
            {
                string computer_name = Utils.get_computer_name();

                srvNames.Add(computer_name);
                srvNames.Add(computer_name + "\\sqlexpress");
                srvNames.Add(".");
                srvNames.Add(".\\sqlexpress");

            }
            return srvNames;
        }

        List<string> enumerate_mssql_Server_Databases(Microsoft.SqlServer.Management.Smo.Server server)
        {
            List<string> databases = new List<string>();
            try
            {
                global_write_to_console("Enumerating available Databases in the Server...");
                global_write_to_console("");

                foreach (Microsoft.SqlServer.Management.Smo.Database dtb in server.Databases)
                {
                    if (!dtb.IsSystemObject && dtb.IsAccessible)
                    {
                        databases.Add(dtb.Name);

                        string msg = "Name: " + dtb.Name + "   Size: " + dtb.Size + " MB" + "    Owner: " + dtb.Owner + "   Tables count: " + dtb.Tables.Count;

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.BackgroundColor = ConsoleColor.White;
                        global_write_to_console(msg);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }

                global_write_to_console("");
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.Message, TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
            return databases;
        }

        bool Connect_To_mssql_Server(string srv, string user, string pwd, bool intsec)
        {
            try
            {
                Server server = new Server(srv);
                server.ConnectionContext.LoginSecure = intsec;
                if (!intsec)
                {
                    server.ConnectionContext.Login = user;
                    server.ConnectionContext.Password = pwd;
                }
                server.ConnectionContext.Connect();

                global_write_to_console("Connection to Server Succesfull...");

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.BackgroundColor = ConsoleColor.White;

                global_write_to_console("");
                global_write_to_console("BuildClrVersionString: " + server.BuildClrVersionString);
                global_write_to_console("BrowserServiceAccount: " + server.BrowserServiceAccount.ToString());
                global_write_to_console("ComputerNamePhysicalNetBIOS: " + server.ComputerNamePhysicalNetBIOS.ToString());
                global_write_to_console("Collation: " + server.Collation.ToString());
                global_write_to_console("Edition: " + server.Edition.ToString());
                global_write_to_console("EngineEdition: " + server.EngineEdition.ToString());
                global_write_to_console("FilestreamShareName.: " + server.FilestreamShareName.ToString());
                global_write_to_console("Name: " + server.Name.ToString());
                global_write_to_console("Platform: " + server.Platform.ToString());
                global_write_to_console("Product: " + server.Product.ToString());
                global_write_to_console("PhysicalMemory: " + server.PhysicalMemory.ToString());
                global_write_to_console("PhysicalMemoryUsageInKB: " + server.PhysicalMemoryUsageInKB.ToString());
                global_write_to_console("Processors: " + server.Processors.ToString());
                global_write_to_console("OSVersion: " + server.OSVersion.ToString());
                global_write_to_console("Version: " + server.Version.ToString());
                global_write_to_console("");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.BackgroundColor = ConsoleColor.Black;

                enumerate_mssql_Server_Databases(server);

                return true;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null)
                {
                    msg += "\n" + ex.InnerException.Message;
                }
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return false;
            }
        }

        void fetch_servers_cli_utils()
        {
            List<string> lst_found_servers = fetch_servers();
        }

        void fetch_databases_cli_utils()
        {

        }

        private List<string> fetch_servers()
        {
            try
            {
                List<string> lst_servers = new List<string>();
                List<string> found_servers = new List<string>();

                List<string> sql_data_source_servers = fetch_servers_using_sql_data_source_enumerator();
                List<string> smo_application_servers = fetch_servers_using_smo_application();
                List<string> registry_servers = fetch_servers_using_registry();

                if (sql_data_source_servers != null)
                    found_servers = found_servers.Concat(sql_data_source_servers).ToList();

                if (smo_application_servers != null)
                    found_servers = found_servers.Concat(smo_application_servers).ToList();

                if (registry_servers != null)
                    found_servers = found_servers.Concat(registry_servers).ToList();

                foreach (string server in found_servers)
                {
                    Console.WriteLine(server);

                    if (!lst_servers.Contains(server))
                    {
                        lst_servers.Add(server);
                    }
                }

                return lst_servers;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }

        private List<string> fetch_servers_using_sql_data_source_enumerator()
        {
            try
            {
                call_delegate("Enumerating servers using sql data source enumerator.");

                List<string> lst_servers = new List<string>();
                string servername = Environment.MachineName;
                DataTable sql_servers_table = SqlDataSourceEnumerator.Instance.GetDataSources();
                lst_servers.Add(servername);

                foreach (DataRow row in sql_servers_table.Rows)
                {
                    string name = row[1] + @"\" + (row[2]).ToString();
                    Console.WriteLine(name);

                    call_delegate(name);

                    lst_servers.Add(name);
                }

                return lst_servers;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }

        private List<string> fetch_servers_using_smo_application()
        {
            try
            {
                call_delegate("Enumerating servers using smo application.");

                List<string> lst_servers = new List<string>();
                string servername = Environment.MachineName;
                DataTable sql_servers_table = SmoApplication.EnumAvailableSqlServers(false);
                lst_servers.Add(servername);

                foreach (DataRow row in sql_servers_table.Rows)
                {
                    string name = row[1] + @"\" + (row[2]).ToString();
                    Console.WriteLine(name);

                    call_delegate(name);

                    lst_servers.Add(name);
                }

                return lst_servers;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }

        private List<string> fetch_servers_using_registry()
        {
            try
            {
                call_delegate("Enumerating servers using registry.");

                List<string> lst_servers = new List<string>();
                string servername = Environment.MachineName;
                lst_servers.Add(servername);

                RegistryView regview = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;
                using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, regview))
                {
                    RegistryKey instancekey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL", false);
                    if (instancekey != null)
                    {
                        foreach (var instancename in instancekey.GetValueNames())
                        {
                            string name = servername + @"\" + (instancename).ToString();
                            Console.WriteLine(name);

                            call_delegate(name);

                            lst_servers.Add(name);
                        }
                    }
                }
                return lst_servers;
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }

        void call_delegate(string msg)
        {
            Action myAction = () =>
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
            };

            // Invoke the action
            myAction.Invoke();
        }

        void black_bg_green_fg()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        void black_bg_blue_fg()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        void black_bg_cyan_fg()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        void black_bg_yellow_fg()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.BackgroundColor = ConsoleColor.Black;
        }

        void upload_users_cli_utils()
        {
            try
            {
                string base_directory = AppDomain.CurrentDomain.BaseDirectory;
                string data_path = Path.Combine(base_directory, "data");
                string file_name = "users.txt";

                var _data_file = Path.Combine(data_path, file_name);

                // Read all lines into a string array
                string[] lines = File.ReadAllLines(_data_file);

                // Loop through the array using a traditional for loop
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Trim().Length > 0)
                    {
                        Console.WriteLine(string.Format("Line {0}: {1}", i + 1, lines[i]));
                        upload_users_to_database(lines[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
            }
        }

        void upload_users_to_database(string line)
        {
            try
            {
                string[] fields = line.Split(',');

                user_dto _user_dto = new user_dto();

                if (fields.Length > 0)
                {
                    if (!string.IsNullOrEmpty(fields[0]))
                    {
                        _user_dto.full_names = fields[0].Trim();
                    }
                    else
                    {
                        while (String.IsNullOrEmpty(_user_dto.full_names))
                        {
                            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user full names cannot be null.", TAG));
                            global_write_to_console("full names:");
                            _user_dto.full_names = Console.ReadLine().Trim();
                        }
                    }
                }
                else
                {
                    while (String.IsNullOrEmpty(_user_dto.full_names))
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user full names cannot be null.", TAG));
                        global_write_to_console("full names:");
                        _user_dto.full_names = Console.ReadLine().Trim();
                    }
                }

                if (fields.Length > 1)
                {
                    if (!string.IsNullOrEmpty(fields[1]))
                    {
                        _user_dto.email = fields[1].Trim();
                    }
                    else
                    {
                        while (String.IsNullOrEmpty(_user_dto.email))
                        {
                            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide an email for user [ " + _user_dto.full_names + ".", TAG));
                            global_write_to_console("email:");
                            _user_dto.email = Console.ReadLine().Trim();
                        }
                    }
                }
                else
                {
                    while (String.IsNullOrEmpty(_user_dto.email))
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide an email for user [ " + _user_dto.full_names + ".", TAG));
                        global_write_to_console("email:");
                        _user_dto.email = Console.ReadLine().Trim();
                    }
                }

                if (fields.Length > 2)
                {
                    if (!string.IsNullOrEmpty(fields[2]))
                    {
                        _user_dto.pass_word = fields[2].Trim();
                    }
                    else
                    {
                        while (String.IsNullOrEmpty(_user_dto.pass_word))
                        {
                            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide a password for user [ " + _user_dto.full_names + ".", TAG));
                            global_write_to_console("password:");
                            _user_dto.pass_word = Console.ReadLine().Trim();
                        }
                    }
                }
                else
                {
                    while (String.IsNullOrEmpty(_user_dto.pass_word))
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide a password for user [ " + _user_dto.full_names + ".", TAG));
                        global_write_to_console("password:");
                        _user_dto.pass_word = Console.ReadLine().Trim();
                    }
                }

                string salt = Utils.create_random_salt();
                string salted_password = salt + _user_dto.pass_word;
                string password_salt_hash = Utils.get_SHA512_hash(salted_password);

                _user_dto.password_hash = password_salt_hash;
                _user_dto.password_salt = salt;

                _user_dto.pass_word = Utils.encrypt_string(_user_dto.pass_word);

                if (fields.Length > 3)
                {
                    if (!string.IsNullOrEmpty(fields[3]))
                    {
                        _user_dto.secret_word = fields[3].Trim();
                    }
                    else
                    {
                        while (String.IsNullOrEmpty(_user_dto.secret_word))
                        {
                            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide a Secret Word for user [ " + _user_dto.full_names + ".", TAG));
                            global_write_to_console("Secret Word:");
                            _user_dto.secret_word = Console.ReadLine().Trim();
                        }
                    }
                }
                else
                {
                    while (String.IsNullOrEmpty(_user_dto.secret_word))
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide a Secret Word for user [ " + _user_dto.full_names + ".", TAG));
                        global_write_to_console("Secret Word:");
                        _user_dto.secret_word = Console.ReadLine().Trim();
                    }
                }

                if (fields.Length > 4)
                {
                    if (!string.IsNullOrEmpty(fields[4]))
                    {
                        _user_dto.gender = fields[4].Trim();
                    }
                    else
                    {
                        while (String.IsNullOrEmpty(_user_dto.gender))
                        {
                            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide a Gender for user [ " + _user_dto.full_names + ".", TAG));
                            global_write_to_console("Gender:");
                            _user_dto.gender = Console.ReadLine().Trim();
                        }
                    }
                }
                else
                {
                    while (String.IsNullOrEmpty(_user_dto.gender))
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide a Gender for user [ " + _user_dto.full_names + ".", TAG));
                        global_write_to_console("Gender: { Male, Female, Prefer Not To Say }:");
                        _user_dto.gender = Console.ReadLine().Trim();
                    }
                }

                while (!String.IsNullOrEmpty(_user_dto.gender))
                {
                    if (_user_dto.gender.Equals("Male", StringComparison.OrdinalIgnoreCase))
                    {
                        //Indicates that the iteration has ended
                        break;
                    }
                    else if (_user_dto.gender.Equals("Female", StringComparison.OrdinalIgnoreCase))
                    {
                        //Indicates that the iteration has ended
                        break;
                    }
                    else if (_user_dto.gender.Equals("Prefer Not To Say", StringComparison.OrdinalIgnoreCase))
                    {
                        //Indicates that the iteration has ended
                        break;
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Gender value must either be 'Male', 'Female' or 'Prefer Not To Say'.", TAG));
                        global_write_to_console("\n Gender: { Male, Female, Prefer Not To Say }");
                        _user_dto.gender = Console.ReadLine();
                        continue;
                    }
                }

                if (fields.Length > 5)
                {
                    if (!string.IsNullOrEmpty(fields[5]))
                    {
                        _user_dto.dob = fields[5].Trim();
                    }
                    else
                    {
                        while (String.IsNullOrEmpty(_user_dto.dob))
                        {
                            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide a Date of birth for user [ " + _user_dto.full_names + ".", TAG));
                            global_write_to_console("Date of birth: format: dd-MM-yyyy:");
                            _user_dto.dob = Console.ReadLine().Trim();
                        }
                    }
                }
                else
                {
                    while (String.IsNullOrEmpty(_user_dto.dob))
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("provide a Date of birth for user [ " + _user_dto.full_names + ".", TAG));
                        global_write_to_console("Date of birth: format: dd-MM-yyyy:");
                        _user_dto.dob = Console.ReadLine().Trim();
                    }
                }

                while (!String.IsNullOrEmpty(_user_dto.dob))
                {
                    DateTime _dob = new DateTime();
                    bool is_datetime = DateTime.TryParse(_user_dto.dob, out _dob);

                    if (is_datetime)
                    {
                        _user_dto.year = _dob.Year.ToString();
                        _user_dto.month = _dob.Month.ToString();
                        _user_dto.day = _dob.Day.ToString();
                        //Indicates that the iteration has ended
                        break;
                    }
                    else
                    {
                        _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Date of birth: format: dd-MM-yyyy.", TAG));
                        global_write_to_console("\n Date of birth: format: dd-MM-yyyy");
                        _user_dto.gender = Console.ReadLine();
                        continue;
                    }
                }



                //status
                _user_dto.status = "Active";
                //created date
                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");
                _user_dto.created_date = dateTimenow;

                global_write_to_console("");

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("insert record execution running....", TAG));

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.mssql + "....", TAG));

                bool _exists_in_mssql = mssqlapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_mssql)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.mssql + "\nproceeding with insertion....", TAG));

                    mssql_api.getInstance(_notificationmessageEventname).save_user_in_mssql(_user_dto);
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.mssql + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.sqlite + "....", TAG));

                bool _exists_in_sqlite = sqliteapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_sqlite)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.sqlite + ",\nproceeding with insertion....", TAG));

                    sqlite_api.getInstance(_notificationmessageEventname).save_user_in_sqlite(_user_dto);
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.sqlite + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.mysql + "....", TAG));

                bool _exists_in_mysql = mysqlapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_mysql)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.mysql + ",\nproceeding with insertion....", TAG));

                    mysql_api.getInstance(_notificationmessageEventname).save_user_in_mysql(_user_dto);
                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.mysql + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.postgresql + "....", TAG));

                bool _exists_in_postgresql = postgresqlapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_postgresql)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.postgresql + ",\nproceeding with insertion....", TAG));

                    postgresql_api.getInstance(_notificationmessageEventname).save_user_in_postgresql(_user_dto);

                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.postgresql + ".", TAG));
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("checking if user with name [ " + _user_dto.email + " ] exists in " + DBContract.mongodb + "....", TAG));

                bool _exists_in_mongodb = mongodbapisingleton.getInstance(_notificationmessageEventname).check_if_user_exists(_user_dto.email);

                if (!_exists_in_mongodb)
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] does not exists in " + DBContract.mongodb + ",\nproceeding with insertion....", TAG));

                    mongodb_api.getInstance(_notificationmessageEventname).save_user_in_mongodb(_user_dto);

                }
                else
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("user with name [ " + _user_dto.email + " ] exists in " + DBContract.postgresql + ".", TAG));
                }

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
            }
        }
















































    }
}
