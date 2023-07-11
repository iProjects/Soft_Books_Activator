using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.EntityClient;
using System.Data.Sql;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;

namespace Soft_Books_Activator
{
    public partial class main_form : Form
    {
        SBPayrollDBEntities db;
        static string SBApplication = "SBPayroll";
        static string Metadata = "SBPayrollDBEntities";
        static string DatabaseVersionExtPropertyKey = SBApplication + "Version";
        List<server_databases> global_databases_list = new List<server_databases>();
        DateTime startDate = DateTime.Now;
        // Timers namespace rather than Threading
        System.Timers.Timer elapsed_timer = new System.Timers.Timer(); // Doesn't require any args
        int _TimeCounter = 0;
        DateTime _startDate = DateTime.Now;
        string TAG;
        List<notificationdto> _lstnotificationdto = new List<notificationdto>();
        //Event declaration:
        //event for publishing messages to output
        event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        /* to use a BackgroundWorker object to perform time-intensive operations in a background thread.
            You need to:
            1. Define a worker method that does the time-intensive work and call it from an event handler for the DoWork
            event of a BackgroundWorker.
            2. Start the execution with RunWorkerAsync. Any argument required by the worker method attached to DoWork
            can be passed in via the DoWorkEventArgs parameter to RunWorkerAsync.
            In addition to the DoWork event the BackgroundWorker class also defines two events that should be used for
            interacting with the user interface. These are optional.
            The RunWorkerCompleted event is triggered when the DoWork handlers have completed.
            The ProgressChanged event is triggered when the ReportProgress method is called. */
        BackgroundWorker bgWorker = new BackgroundWorker();
        string current_action = string.Empty;
        //countdown 
        Stopwatch stopWatch = new Stopwatch();
        //task start time
        DateTime _task_start_time = DateTime.Now;
        //task end time
        DateTime _task_end_time = DateTime.Now;
        string selected_system = string.Empty;
        SB_activator activation_info = null;
        string TRIAL_PERIOD = "370";
        string PAYROLL_SYSTEM_PATH = @"C:\Program Files (x86)\Software Providers\Soft Books Payroll";
        string SACCO_SYSTEM_PATH = @"C:\Program Files (x86)\Software Providers\Soft Books Sacco";
        string SCHOOL_SYSTEM_PATH = @"C:\Program Files (x86)\Software Providers\Soft Books School";
        string RUNTIME = "P";

        public main_form()
        {
            InitializeComponent();

            TAG = this.GetType().Name;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ThreadException);

            //Subscribing to the event: 
            //Dynamically:
            //EventName += HandlerName;
            _notificationmessageEventname += notificationmessageHandler;

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished main_form initialization", TAG));

        }

        private void main_form_Load(object sender, EventArgs e)
        {
            try
            {
                TRIAL_PERIOD = System.Configuration.ConfigurationManager.AppSettings["TRIAL_PERIOD"];
                PAYROLL_SYSTEM_PATH = System.Configuration.ConfigurationManager.AppSettings["PAYROLL_SYSTEM_PATH"];
                SACCO_SYSTEM_PATH = System.Configuration.ConfigurationManager.AppSettings["SACCO_SYSTEM_PATH"];
                SCHOOL_SYSTEM_PATH = System.Configuration.ConfigurationManager.AppSettings["SCHOOL_SYSTEM_PATH"];
                RUNTIME = System.Configuration.ConfigurationManager.AppSettings["RUNTIME"];

                var _systems = new BindingList<KeyValuePair<string, string>>();
                _systems.Add(new KeyValuePair<string, string>("Soft Books Payroll", "Soft Books Payroll"));
                _systems.Add(new KeyValuePair<string, string>("Soft Books Sacco", "Soft Books Sacco"));
                _systems.Add(new KeyValuePair<string, string>("Soft Books School", "Soft Books School"));
                cbosystem.DataSource = _systems;
                cbosystem.ValueMember = "Key";
                cbosystem.DisplayMember = "Value";

                toolStripProgressBar.Visible = false;

                //initialize current running time timer
                elapsed_timer.Interval = 1000;
                elapsed_timer.Elapsed += elapsed_timer_Elapsed; // Uses an event instead of a delegate
                elapsed_timer.Start(); // Start the timer

                //app version
                var _buid_version = Application.ProductVersion;
                groupBox1.Text = "build version - " + _buid_version;

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                populate_auto_complete_values();

                activation_info = Utils.get_license_activation_info(selected_system);

                _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished main_form load", TAG));
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            this._notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs(ex.Message, TAG));
        }

        private void ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            this._notificationmessageEventname.Invoke(sender, new notificationmessageEventArgs(ex.Message, TAG));
        }

        //Event handler declaration:
        private void notificationmessageHandler(object sender, notificationmessageEventArgs args)
        {
            try
            {
                /* Handler logic */

                //Invoke(new Action(() =>
                //{

                notificationdto _notificationdto = new notificationdto();

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

                String _logtext = Environment.NewLine + "[ " + dateTimenow + " ]   " + args.message;

                _notificationdto._notification_message = _logtext;
                _notificationdto._created_datetime = dateTimenow;
                _notificationdto.TAG = args.TAG;

                _lstnotificationdto.Add(_notificationdto);
                Console.WriteLine(args.message);

                var _lstmsgdto = from msgdto in _lstnotificationdto
                                 orderby msgdto._created_datetime descending
                                 select msgdto._notification_message;

                String[] _logflippedlines = _lstmsgdto.ToArray();

                txtlog.Lines = _logflippedlines;
                txtlog.ScrollToCaret();

                //}));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void elapsed_timer_Elapsed(object sender, EventArgs e)
        {
            try
            {
                _TimeCounter++;
                DateTime nowDate = DateTime.Now;
                TimeSpan t = nowDate - _startDate;

                lbltimelapsed.Owner.Invoke(new Action(() =>
                {
                    lbltimelapsed.Text = string.Format("{0} : {1} : {2} : {3}", t.Days, t.Hours, t.Minutes, t.Seconds);
                }));

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

                lblrunningtime.Owner.Invoke(new Action(() =>
                {
                    lblrunningtime.Text = dateTimenow;
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void btnconnect_Click(object sender, EventArgs e)
        {
            try
            {
                current_action = "connect";

                _task_start_time = DateTime.Now;

                btnconnect.Text = "connecting...";

                disable_controls();

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                if (!bgWorker.IsBusy)
                    bgWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void btnactivate_Click(object sender, EventArgs e)
        {
            try
            {
                current_action = "activate";

                _task_start_time = DateTime.Now;

                btnactivate.Text = "activating...";

                disable_controls();

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                if (!bgWorker.IsBusy)
                    bgWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void btnclose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private string get_payroll_Entity_Connection(string server, string database, string userId, string Pwd, bool IntegratedSec)
        {
            // Specify the provider name, server and database.
            string providerName = "System.Data.SqlClient";
            string serverName = server;
            string databaseName = database;

            // Initialize the connection string builder for the
            // underlying provider.
            SqlConnectionStringBuilder sqlBuilder =
            new SqlConnectionStringBuilder();

            // Set the properties for the data source.
            sqlBuilder.DataSource = serverName;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.IntegratedSecurity = IntegratedSec;
            sqlBuilder.UserID = userId;
            sqlBuilder.Password = Pwd;

            // Build the SqlConnection connection string.
            string providerString = sqlBuilder.ToString();

            // Initialize the EntityConnectionStringBuilder.
            EntityConnectionStringBuilder entityBuilder =
            new EntityConnectionStringBuilder();

            //Set the provider name.
            entityBuilder.Provider = providerName;

            // Set the provider-specific connection string.
            entityBuilder.ProviderConnectionString = providerString;

            // Set the Metadata location.
            string metadata = @"res://*/SBPayrollDBEntities.csdl|res://*/SBPayrollDBEntities.ssdl|res://*/SBPayrollDBEntities.msl";
            //metadata.Replace("SBPayrollDBEntities", system.Metadata);

            entityBuilder.Metadata = metadata;

            return entityBuilder.ToString();

        }

        private bool is_valid()
        {
            bool noerror = true;
            if (cbosystem.SelectedIndex == -1)
            {
                errorProvider.SetError(cbosystem, "select system!");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("select system!", TAG));
                noerror = false;
            }
            if (string.IsNullOrEmpty(cboserver.Text))
            {
                errorProvider.SetError(cboserver, "select server!");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("select server!", TAG));
                noerror = false;
            }
            if (string.IsNullOrEmpty(cbodatabase.Text))
            {
                errorProvider.SetError(cbodatabase, "select database!");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("select database!", TAG));
                noerror = false;
            }
            if (!chkIntegratedSecurity.Checked)
            {
                if (string.IsNullOrEmpty(txtusername.Text))
                {
                    errorProvider.SetError(txtusername, "username cannot be null!");
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("username cannot be null!", TAG));
                    noerror = false;
                }
                if (string.IsNullOrEmpty(txtpassword.Text))
                {
                    errorProvider.SetError(txtpassword, "password cannot be null!");
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("password cannot be null!", TAG));
                    noerror = false;
                }
            }
            return noerror;
        }

        private bool is_database_connection_valid()
        {
            bool noerror = true;
            if (string.IsNullOrEmpty(cboserver.Text))
            {
                errorProvider.SetError(cboserver, "select server!");
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("select server!", TAG));
                noerror = false;
            }
            if (!chkIntegratedSecurity.Checked)
            {
                if (string.IsNullOrEmpty(txtusername.Text))
                {
                    errorProvider.SetError(txtusername, "username cannot be null!");
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("username cannot be null!", TAG));
                    noerror = false;
                }
                if (string.IsNullOrEmpty(txtpassword.Text))
                {
                    errorProvider.SetError(txtpassword, "password cannot be null!");
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("password cannot be null!", TAG));
                    noerror = false;
                }
            }
            return noerror;
        }

        private void btnfetchservers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                errorProvider.Clear();
                if (cbosystem.SelectedIndex == -1)
                {
                    errorProvider.SetError(cbosystem, "select system!");
                    return;
                }

                disable_controls();

                current_action = "server";

                _task_start_time = DateTime.Now;

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("calling backgund worker...", TAG));

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                if (!bgWorker.IsBusy)
                    bgWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //this is the method that the backgroundworker will perform on in the background thread without blocking the UI.
            /* One thing to note! A try catch is not necessary as any exceptions will terminate the
            backgroundWorker and report
            the error to the "RunWorkerCompleted" event */

            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("running backgund worker...", TAG));

            toolStripProgressBar.Owner.Invoke(new Action(() =>
            {
                toolStripProgressBar.Visible = true;
            }));

            if (current_action.Equals("server"))
            {
                try
                {
                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("enumerating available servers...", TAG));
                    }));

                    List<string> srvNames = new List<string>();

                    //string machine_name = Environment.MachineName;
                    //srvNames.Add(machine_name);

                    cboserver.Invoke(new Action(() =>
                    {
                        cboserver.DataSource = srvNames;
                        cboserver.Text = string.Empty;
                    }));

                    //DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();

                    //if (dt.Rows.Count > 0)
                    //{
                    //    foreach (DataRow dr in dt.Rows)
                    //    {
                    //        string InstanceName = dr["InstanceName"].ToString();
                    //        string ServerName = dr["ServerName"].ToString();

                    //        if (string.IsNullOrEmpty(InstanceName) || string.IsNullOrEmpty(ServerName))
                    //        {
                    //            srvNames.Add(ServerName);

                    //            Invoke(new Action(() =>
                    //            {
                    //                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ServerName, TAG));
                    //            }));
                    //        }

                    //        if (!string.IsNullOrEmpty(InstanceName) && !string.IsNullOrEmpty(ServerName))
                    //        {
                    //            ServerName = ServerName + @"\" + InstanceName;
                    //            srvNames.Add(ServerName);

                    //            Invoke(new Action(() =>
                    //            {
                    //                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ServerName, TAG));

                    //            }));
                    //        }
                    //    }

                    //    int count = srvNames.Count;

                    //    Debug.WriteLine("servers: " + count);

                    //    Invoke(new Action(() =>
                    //    {
                    //        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("found [ " + count + " ] servers.", TAG));

                    //    }));

                    //    cboserver.Invoke(new Action(() =>
                    //    {
                    //        cboserver.DataSource = srvNames;
                    //    }));
                    //}
                    //else
                    //{
                    //    cboserver.Invoke(new Action(() =>
                    //    {
                    //        cboserver.DataSource = srvNames;
                    //    }));
                    //}

                    List<string> lst_found_servers = fetch_servers();

                    cboserver.Invoke(new Action(() =>
                    {
                        cboserver.DataSource = lst_found_servers;
                    }));

                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                    }));
                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                }
            }
            else if (current_action.Equals("database"))
            {
                try
                {
                    errorProvider.Clear();
                    bool isvalid = false;

                    List<string> databases = new List<string>();

                    cbodatabase.Invoke(new Action(() =>
                    {
                        cbodatabase.DataSource = databases;
                        cbodatabase.Text = string.Empty;
                    }));

                    Invoke(new Action(() =>
                    {
                        isvalid = is_database_connection_valid();
                    }));

                    if (isvalid)
                    {
                        string serverName = string.Empty;
                        string userName = string.Empty;
                        string password = string.Empty;
                        bool IntegratedSecurity = false;

                        Invoke(new Action(() =>
                        {
                            serverName = cboserver.Text.Trim();
                            userName = txtusername.Text.Trim();
                            password = txtpassword.Text.Trim();
                            IntegratedSecurity = chkIntegratedSecurity.Checked;
                        }));

                        Invoke(new Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Establishing connection to the server.", TAG));
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("Using Parameters [ Server [ {0} ] Username [ {1} ] Integrated Security [ {2} ] ].", serverName, userName, IntegratedSecurity), TAG));
                        }));

                        Microsoft.SqlServer.Management.Smo.Server server = ConnectToServer(serverName, userName, password, IntegratedSecurity);

                        if (server == null)
                        {
                            Invoke(new Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Failed to connect to server.", TAG));
                            }));
                        }
                        else
                        {
                            if (server.ConnectionContext.IsOpen)
                            {
                                string msg = "Edition: " + server.Edition + Environment.NewLine +
                                    "IsClustered: " + server.IsClustered + Environment.NewLine +
                                    "Build: " + server.BuildNumber + Environment.NewLine +
                                    "Net Name: " + server.NetName + Environment.NewLine +
                                    "Instance: " + server.InstanceName + Environment.NewLine +
                                    "Physical Memory: " + server.PhysicalMemory.ToString() + Environment.NewLine +
                                    "Platform: " + server.Platform + Environment.NewLine +
                                    "Processors: " + server.Processors + Environment.NewLine +
                                    "Type: " + server.ServerType + Environment.NewLine +
                                    "Service Id: " + server.ServiceInstanceId + Environment.NewLine +
                                    "Start Mode: " + server.ServiceStartMode.ToString() + Environment.NewLine +
                                    "State: " + server.State;

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Connected to Server successfully.", TAG));

                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                                }));

                                NotifyMessage("Connected to Server successfully.", msg);

                                databases = GetServerDatabases(server);

                                int count = databases.Count;

                                Debug.WriteLine("databases: " + count);

                                cbodatabase.Invoke(new Action(() =>
                                {
                                    cbodatabase.DataSource = databases;
                                }));

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("found [ " + count + " ] databases.", TAG));
                                }));

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                    }));
                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                }
            }
            else if (current_action.Equals("connect"))
            {
                try
                {
                    errorProvider.Clear();
                    bool isvalid = false;

                    Invoke(new Action(() =>
                    {
                        isvalid = is_valid();
                    }));

                    if (isvalid)
                    {
                        string system = string.Empty;
                        string serverName = string.Empty;
                        string databaseName = string.Empty;
                        string userName = string.Empty;
                        string password = string.Empty;
                        bool IntegratedSecurity = false;

                        Invoke(new Action(() =>
                        {
                            system = cbosystem.SelectedValue.ToString();
                            serverName = cboserver.Text.Trim();
                            databaseName = cbodatabase.Text.Trim();
                            userName = txtusername.Text.Trim();
                            password = txtpassword.Text.Trim();
                            IntegratedSecurity = chkIntegratedSecurity.Checked;
                        }));

                        Invoke(new Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Establishing connection to the server.", TAG));
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("Using Parameters [ Server [ {0} ] Username [ {1} ] Integrated Security [ {2} ] ].", serverName, userName, IntegratedSecurity), TAG));
                        }));

                        Microsoft.SqlServer.Management.Smo.Server server = ConnectToServer(serverName, userName, password, IntegratedSecurity);

                        if (server == null)
                        {
                            Invoke(new Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Failed to connect to server.", TAG));
                            }));
                        }
                        else
                        {
                            if (server.ConnectionContext.IsOpen)
                            {
                                string msg = "Edition: " + server.Edition + Environment.NewLine +
                                    "IsClustered: " + server.IsClustered + Environment.NewLine +
                                    "Build: " + server.BuildNumber + Environment.NewLine +
                                    "Net Name: " + server.NetName + Environment.NewLine +
                                    "Instance: " + server.InstanceName + Environment.NewLine +
                                    "Physical Memory: " + server.PhysicalMemory.ToString() + Environment.NewLine +
                                    "Platform: " + server.Platform + Environment.NewLine +
                                    "Processors: " + server.Processors + Environment.NewLine +
                                    "Type: " + server.ServerType + Environment.NewLine +
                                    "Service Id: " + server.ServiceInstanceId + Environment.NewLine +
                                    "Start Mode: " + server.ServiceStartMode.ToString() + Environment.NewLine +
                                    "State: " + server.State;

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Connected to Server successfully.", TAG));

                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                                }));

                                NotifyMessage("Connected to Server successfully.", msg);

                                List<string> databases = new List<string>();

                                databases = GetServerDatabases(server);

                                int count = databases.Count;

                                Debug.WriteLine("databases: " + count);

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("found [ " + count + " ] databases.", TAG));
                                }));

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Establishing connection to the database.", TAG));
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("Using Parameters [ Server [ {0} ] Username [ {1} ] Integrated Security [ {2} ] Database [ {3} ] ].", serverName, userName, IntegratedSecurity, databaseName), TAG));
                                }));

                                db = null;

                                EntityConnection conn = new EntityConnection(
                                    get_payroll_Entity_Connection(
                                        serverName,
                                        databaseName,
                                        userName,
                                        password,
                                        IntegratedSecurity
                                    ));

                                //overwrite the default context with this one
                                db = new SBPayrollDBEntities(conn);

                                try
                                {
                                    var Settings = db.Settings.FirstOrDefault();

                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("connection successfull.", TAG));
                                    Utils.ShowError(new Exception("connection successfull."));

                                    Invoke(new MethodInvoker(delegate()
                                    {
                                        save_auto_complete_login();
                                    }));

                                }
                                catch (Exception ex)
                                {
                                    Invoke(new Action(() =>
                                    {
                                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                                    }));
                                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                                }

                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                    }));
                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                }
            }
            else if (current_action.Equals("activate"))
            {
                try
                {
                    errorProvider.Clear();
                    bool isvalid = false;

                    Invoke(new Action(() =>
                    {
                        isvalid = is_valid();
                    }));

                    if (isvalid)
                    {
                        string system = string.Empty;
                        string serverName = string.Empty;
                        string databaseName = string.Empty;
                        string userName = string.Empty;
                        string password = string.Empty;
                        bool IntegratedSecurity = false;

                        Invoke(new Action(() =>
                        {
                            system = cbosystem.SelectedValue.ToString();
                            serverName = cboserver.Text.Trim();
                            databaseName = cbodatabase.Text.Trim();
                            userName = txtusername.Text.Trim();
                            password = txtpassword.Text.Trim();
                            IntegratedSecurity = chkIntegratedSecurity.Checked;
                        }));

                        Invoke(new Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Establishing connection to the server.", TAG));
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("Using Parameters [ Server [ {0}]  Username [ {1} ] Integrated Security [ {2} ] Database [ {3} ] ].", serverName, userName, IntegratedSecurity, databaseName), TAG));
                        }));

                        Microsoft.SqlServer.Management.Smo.Server server = ConnectToServer(serverName, userName, password, IntegratedSecurity);

                        if (server == null)
                        {
                            Invoke(new Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Failed to connect to server.", TAG));
                            }));
                        }
                        else
                        {
                            if (server.ConnectionContext.IsOpen)
                            {
                                string msg = "Edition: " + server.Edition + Environment.NewLine +
                                    "IsClustered: " + server.IsClustered + Environment.NewLine +
                                    "Build: " + server.BuildNumber + Environment.NewLine +
                                    "Net Name: " + server.NetName + Environment.NewLine +
                                    "Instance: " + server.InstanceName + Environment.NewLine +
                                    "Physical Memory: " + server.PhysicalMemory.ToString() + Environment.NewLine +
                                    "Platform: " + server.Platform + Environment.NewLine +
                                    "Processors: " + server.Processors + Environment.NewLine +
                                    "Type: " + server.ServerType + Environment.NewLine +
                                    "Service Id: " + server.ServiceInstanceId + Environment.NewLine +
                                    "Start Mode: " + server.ServiceStartMode.ToString() + Environment.NewLine +
                                    "State: " + server.State;

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Connected to Server successfully.", TAG));

                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                                }));

                                NotifyMessage("Connected to Server successfully.", msg);

                                List<string> databases = new List<string>();

                                databases = GetServerDatabases(server);

                                int count = databases.Count;

                                Debug.WriteLine("databases: " + count);

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("found [ " + count + " ] databases.", TAG));
                                }));

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Establishing connection to the database.", TAG));
                                }));

                                db = null;

                                EntityConnection conn = new EntityConnection(
                                    get_payroll_Entity_Connection(
                                        serverName,
                                        databaseName,
                                        userName,
                                        password,
                                        IntegratedSecurity
                                    ));

                                //overwrite the default context with this one
                                db = new SBPayrollDBEntities(conn);

                                try
                                {
                                    var Settings = db.Settings.FirstOrDefault();

                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("connection successfull.", TAG));

                                    Invoke(new MethodInvoker(delegate()
                                    {
                                        save_auto_complete_login();
                                    }));

                                    Invoke(new MethodInvoker(delegate()
                                    {
                                        activation_info = Utils.get_license_activation_info(selected_system);

                                        activate_from_database();
                                        activate_from_registry();
                                        activate_from_xml();
                                    }));

                                }
                                catch (Exception ex)
                                {
                                    Invoke(new Action(() =>
                                    {
                                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                                    }));
                                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                                }

                            }
                        }

                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                    }));
                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                }
            }
            else if (current_action.Equals("system"))
            {
                try
                {
                    errorProvider.Clear();
                    bool isvalid = false;

                    Invoke(new Action(() =>
                    {
                        isvalid = is_database_connection_valid();
                    }));

                    if (isvalid)
                    {
                        string serverName = string.Empty;
                        string userName = string.Empty;
                        string password = string.Empty;
                        bool IntegratedSecurity = false;

                        Invoke(new Action(() =>
                        {
                            serverName = cboserver.Text.Trim();
                            userName = txtusername.Text.Trim();
                            password = txtpassword.Text.Trim();
                            IntegratedSecurity = chkIntegratedSecurity.Checked;
                        }));

                        List<string> databases = new List<string>();

                        cbodatabase.Invoke(new Action(() =>
                        {
                            cbodatabase.DataSource = databases;
                            cbodatabase.Text = string.Empty;
                        }));

                        Invoke(new Action(() =>
                        {
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Establishing connection to the server.", TAG));
                            this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(String.Format("Using Parameters [ Server [ {0} ] Username [ {1} ] Integrated Security [ {2} ] ].", serverName, userName, IntegratedSecurity), TAG));
                        }));

                        Microsoft.SqlServer.Management.Smo.Server server = ConnectToServer(serverName, userName, password, IntegratedSecurity);

                        if (server == null)
                        {
                            Invoke(new Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Failed to connect to server.", TAG));
                            }));
                        }
                        else
                        {
                            if (server.ConnectionContext.IsOpen)
                            {
                                string msg = "Edition: " + server.Edition + Environment.NewLine +
                                    "IsClustered: " + server.IsClustered + Environment.NewLine +
                                    "Build: " + server.BuildNumber + Environment.NewLine +
                                    "Net Name: " + server.NetName + Environment.NewLine +
                                    "Instance: " + server.InstanceName + Environment.NewLine +
                                    "Physical Memory: " + server.PhysicalMemory.ToString() + Environment.NewLine +
                                    "Platform: " + server.Platform + Environment.NewLine +
                                    "Processors: " + server.Processors + Environment.NewLine +
                                    "Type: " + server.ServerType + Environment.NewLine +
                                    "Service Id: " + server.ServiceInstanceId + Environment.NewLine +
                                    "Start Mode: " + server.ServiceStartMode.ToString() + Environment.NewLine +
                                    "State: " + server.State;

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Connected to Server successfully.", TAG));

                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));
                                }));

                                NotifyMessage("Connected to Server successfully.", msg);

                                databases = GetServerDatabases(server);

                                int count = databases.Count;

                                Debug.WriteLine("databases: " + count);

                                cbodatabase.Invoke(new Action(() =>
                                {
                                    cbodatabase.DataSource = databases;
                                }));

                                Invoke(new Action(() =>
                                {
                                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("found [ " + count + " ] databases.", TAG));
                                }));

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                    }));
                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                }
            }
        }

        /*This is how the ProgressChanged event method signature looks like:*/
        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Things to be done when a progress change has been reported
            /* The ProgressChangedEventArgs gives access to a percentage,
            allowing for easy reporting of how far along a process is*/
            int progress = e.ProgressPercentage;
        }

        private void bgWorker_WorkComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("disconnecting background worker...", TAG));

                bgWorker.DoWork -= bgWorker_DoWork;
                bgWorker.RunWorkerCompleted -= bgWorker_WorkComplete;
                bgWorker.ProgressChanged -= bgWorker_ProgressChanged;

                _task_end_time = DateTime.Now;

                var _time_lapsed = _task_end_time.Subtract(_task_start_time);

                string msg = "Task Complete!" + Environment.NewLine + "Task took [ " + _time_lapsed + " ]";

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));

                btnconnect.Text = "connect";
                btnactivate.Text = "activate";

                enable_controls();

                toolStripProgressBar.Owner.Invoke(new Action(() =>
                {
                    toolStripProgressBar.Visible = false;
                }));

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void btnfetchdatabases_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                errorProvider.Clear();
                if (string.IsNullOrEmpty(cboserver.Text))
                {
                    errorProvider.SetError(cboserver, "select server!");
                    return;
                }

                disable_controls();

                current_action = "database";

                _task_start_time = DateTime.Now;

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                if (!bgWorker.IsBusy)
                    bgWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private Microsoft.SqlServer.Management.Smo.Server ConnectToServer(string srv, string user, string pwd, bool intsec)
        {
            try
            {
                Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(srv);
                server.ConnectionContext.LoginSecure = intsec;
                if (!intsec)
                {
                    server.ConnectionContext.Login = user;
                    server.ConnectionContext.Password = pwd;
                }
                server.ConnectionContext.Connect();
                return server;
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
                return null;
            }
        }
        private List<string> GetServerDatabases(Microsoft.SqlServer.Management.Smo.Server server)
        {
            List<string> databases = new List<string>();
            try
            {
                Invoke(new Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("enumerating available databases...", TAG));
                }));

                foreach (Microsoft.SqlServer.Management.Smo.Database dtb in server.Databases)
                {
                    if (!dtb.IsSystemObject && dtb.IsAccessible)
                    {
                        var sbverion = dtb.ExtendedProperties[DatabaseVersionExtPropertyKey];
                        if (sbverion != null)
                        {
                            databases.Add(dtb.Name);

                            string msg = "Name: " + dtb.Name + Environment.NewLine + "Size: " + dtb.Size + " MB" + Environment.NewLine + "Owner: " + dtb.Owner + Environment.NewLine + "Tables count: " + dtb.Tables.Count;

                            server_databases server_db = new server_databases(dtb.Name, dtb.Size.ToString(), dtb.Owner, selected_system);

                            global_databases_list.Add(server_db);

                            Invoke(new Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(msg, TAG));

                            }));

                            Console.WriteLine(msg);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
            return databases;
        }
        private bool check_if_db_is_for_the_selected_system()
        {
            try
            {
                string database = string.Empty;

                cbodatabase.Invoke(new Action(() =>
                {
                    database = cbodatabase.Text.Trim().ToString();
                }));

                Invoke(new Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Checking if the selected database is for the selected system.", TAG));
                }));

                if (global_databases_list.Count == 0)
                {
                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Failed to retrieve database information. Try to connect first to the database.", TAG));
                    }));
                    return false;
                }

                server_databases server_db = global_databases_list.Where(i => i.Name.Equals(database)).FirstOrDefault();

                if (server_db == null)
                {
                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Failed to retrieve database information. Try to connect first to the database.", TAG));
                    }));
                    return false;
                }

                if (server_db.system.Equals(selected_system))
                {
                    return true;
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("The selected database does not belong to the selected system. Please fetch a list of databases that belong to the selected system.", TAG));
                    }));
                    return false;
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return false;
            }
        }
        private bool NotifyMessage(string _Title, string _Text)
        {
            try
            {
                appNotifyIcon.Text = Utils.APP_NAME;
                appNotifyIcon.Icon = new Icon("Resources/Dollar.ico");
                appNotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                appNotifyIcon.BalloonTipTitle = _Title;
                appNotifyIcon.BalloonTipText = _Text;
                appNotifyIcon.Visible = true;
                appNotifyIcon.ShowBalloonTip(900000);

                return true;
            }
            catch (Exception ex)
            {
                Utils.LogEventViewer(ex);
                return false;
            }
        }

        private void save_auto_complete_login()
        {
            try
            {
                string auto_complete_login_filename = Utils.AUTO_COMPLETE_LOGIN_FILENAME;

                string system = cbosystem.SelectedValue.ToString();
                string serverName = cboserver.Text.Trim();
                string databaseName = cbodatabase.Text.Trim();
                string userName = txtusername.Text.Trim();
                string password = txtpassword.Text.Trim();
                password = Utils.encrypt_string(password);
                bool IntegratedSecurity = chkIntegratedSecurity.Checked;
                bool remember = chkremember.Checked;

                if (File.Exists(auto_complete_login_filename))
                {
                    //List<SB_Login> successfully_logged_users = GetDataFromSB_LoginXML(auto_complete_login_filename);

                    //var exists = successfully_logged_users.Where(i => i.userName.Equals(userName) && i.databaseName.Equals(databaseName) && i.serverName.Equals(serverName) && i.system.Equals(system)).FirstOrDefault();

                    DataSet ds = new DataSet();

                    ds.ReadXml(auto_complete_login_filename);

                    if (ds.Tables.Count == 0)
                    {
                        XDocument doc = XDocument.Load(auto_complete_login_filename);
                        doc.Element("Systems").Add(
                            new XElement("System",
                            new XAttribute("system", system),
                            new XAttribute("serverName", serverName),
                            new XAttribute("databaseName", databaseName),
                            new XAttribute("userName", userName),
                            new XAttribute("password", password),
                            new XAttribute("IntegratedSecurity", IntegratedSecurity.ToString()),
                            new XAttribute("remember", remember.ToString())
                            ));

                        doc.Save(auto_complete_login_filename);
                    }
                    else
                    {
                        int count = ds.Tables[0].Rows.Count;

                        for (int i = 0; i < count; i++)
                        {
                            ds.Tables[0].DefaultView.RowFilter = "userName = '" + userName + "' and databaseName = '" + databaseName + "' and serverName = '" + serverName + "' and system = '" + system + "'";

                            DataTable dt = (ds.Tables[0].DefaultView).ToTable();

                            if (dt.Rows.Count > 0)
                            {
                                try
                                {
                                    ds.Tables[0].Rows[i].Delete();
                                }
                                catch (Exception ex)
                                {
                                    Log.WriteToErrorLogFile_and_EventViewer(ex);
                                }
                            }
                        }

                        //get data
                        string xmlData = ds.GetXml();

                        //save to file
                        ds.WriteXml(auto_complete_login_filename);

                        XDocument doc = XDocument.Load(auto_complete_login_filename);
                        doc.Element("Systems").Add(
                            new XElement("System",
                            new XAttribute("system", system),
                            new XAttribute("serverName", serverName),
                            new XAttribute("databaseName", databaseName),
                            new XAttribute("userName", userName),
                            new XAttribute("password", password),
                            new XAttribute("IntegratedSecurity", IntegratedSecurity.ToString()),
                            new XAttribute("remember", remember.ToString())
                            ));

                        doc.Save(auto_complete_login_filename);

                    }
                }

                if (!File.Exists(auto_complete_login_filename))
                {
                    List<SB_Login> systems = new List<SB_Login>() { 
                        new SB_Login(
                           system, 
                           serverName,
                           databaseName,
                           userName,
                           password,
                           IntegratedSecurity.ToString(),
                           remember.ToString()                            
                            )};

                    var xml = new XElement("Systems", systems.Select(x =>
                            new XElement("System",
                            new XAttribute("system", x.system),
                            new XAttribute("serverName", x.serverName),
                            new XAttribute("databaseName", x.databaseName),
                            new XAttribute("userName", x.userName),
                            new XAttribute("password", x.password),
                            new XAttribute("IntegratedSecurity", x.IntegratedSecurity),
                            new XAttribute("remember", x.remember)
                                           )));

                    xml.Save(auto_complete_login_filename);
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private List<SB_Login> GetDataFromSB_LoginXML(string filename)
        {
            using (XmlReader xmlRdr = new XmlTextReader(filename))
            {
                return new List<SB_Login>(
                   (from sysElem in XDocument.Load(xmlRdr).
                        Element("Systems").
                        Elements("System")
                    select new SB_Login(
                   (string)sysElem.Attribute("system"),
                   (string)sysElem.Attribute("serverName"),
                   (string)sysElem.Attribute("databaseName"),
                   (string)sysElem.Attribute("userName"),
                   (string)sysElem.Attribute("password"),
                   (string)sysElem.Attribute("IntegratedSecurity"),
                   (string)sysElem.Attribute("remember")
                            )).ToList());
            }
        }
        private void populate_auto_complete_values()
        {
            try
            {
                string auto_complete_login_filename = Utils.AUTO_COMPLETE_LOGIN_FILENAME;

                if (!File.Exists(auto_complete_login_filename))
                {
                    return;
                }

                List<SB_Login> auto_complete_from_xml = GetDataFromSB_LoginXML(auto_complete_login_filename);

                List<string> saved_servers = new List<string>();
                List<string> saved_databases = new List<string>();

                SB_Login last_record = auto_complete_from_xml.Last();

                if (last_record == null)
                {
                    return;
                }

                foreach (var item in auto_complete_from_xml)
                {
                    if (!saved_servers.Contains(item.serverName))
                    {
                        saved_servers.Add(item.serverName);
                    }
                    if (!saved_databases.Contains(item.databaseName))
                    {
                        saved_databases.Add(item.databaseName);
                    }
                }

                cbosystem.SelectedValue = last_record.system;

                set_system_path();

                set_database_properties();

                cboserver.DataSource = saved_servers;
                cboserver.SelectedItem = last_record.serverName;
                cbodatabase.DataSource = saved_databases;
                cbodatabase.SelectedItem = last_record.databaseName;
                txtusername.Text = last_record.userName;

                if (bool.Parse(last_record.remember))
                {
                    string password = Utils.decrypt_string(last_record.password);
                    txtpassword.Text = password;
                }

                chkIntegratedSecurity.Checked = bool.Parse(last_record.IntegratedSecurity);
                chkremember.Checked = bool.Parse(last_record.remember);

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void activate_from_database()
        {
            try
            {
                if (activation_info == null)
                {
                    activation_info = Utils.get_license_activation_info(selected_system);
                }

                string _mac_address = Utils.GetMACAddress();

                tbl_LAS license_activation_store = db.tbl_LAS.Where(i => i.mac_address.Equals(_mac_address)).FirstOrDefault();

                if (license_activation_store == null)
                {
                    //activate

                    tbl_LAS _license_activation_store = new tbl_LAS();

                    _license_activation_store.mac_address = activation_info.mac_address;
                    _license_activation_store.computer_name = activation_info.computer_name;
                    _license_activation_store.activation_key = activation_info.activation_key;
                    _license_activation_store.date_activated = activation_info.date_activated;
                    _license_activation_store.next_expiry_date = activation_info.next_expiry_date;
                    _license_activation_store.days_for_expiry = activation_info.days_for_expiry;

                    db.tbl_LAS.AddObject(_license_activation_store);
                    db.SaveChanges();

                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("License activation from db successfull.", TAG));
                }
                else
                {
                    license_activation_store.mac_address = activation_info.mac_address;
                    license_activation_store.computer_name = activation_info.computer_name;
                    license_activation_store.activation_key = activation_info.activation_key;
                    license_activation_store.date_activated = activation_info.date_activated;
                    license_activation_store.next_expiry_date = activation_info.next_expiry_date;
                    license_activation_store.days_for_expiry = activation_info.days_for_expiry;

                    db.SaveChanges();

                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("License activation from db successfull.", TAG));
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void activate_from_xml()
        {
            try
            {
                string activator_file = "activator.xml";

                string selected_system = cbosystem.SelectedValue.ToString();

                string base_directory = string.Empty;

                switch (selected_system)
                {
                    case "Soft Books Payroll":
                        base_directory = PAYROLL_SYSTEM_PATH;
                        break;
                    case "Soft Books Sacco":
                        base_directory = SACCO_SYSTEM_PATH;
                        break;
                    case "Soft Books School":
                        base_directory = SCHOOL_SYSTEM_PATH;
                        break;
                }

                string resource_path = Path.Combine(base_directory, "Resources");
                string activator_file_path = Path.Combine(resource_path, activator_file);

                string activator_filename = activator_file_path;

                if (activation_info == null)
                {
                    activation_info = Utils.get_license_activation_info(selected_system);
                }

                if (File.Exists(activator_filename))
                {
                    DataSet ds = new DataSet();

                    ds.ReadXml(activator_filename);

                    if (ds.Tables.Count == 0)
                    {
                        //activate 
                        XDocument doc = XDocument.Load(activator_filename);

                        doc.Element("Systems").Add(
                            new XElement("System",
                            new XAttribute("mac_address", activation_info.mac_address),
                            new XAttribute("computer_name", activation_info.computer_name),
                            new XAttribute("activation_key", activation_info.activation_key),
                            new XAttribute("date_activated", activation_info.date_activated),
                            new XAttribute("next_expiry_date", activation_info.next_expiry_date),
                            new XAttribute("days_for_expiry", activation_info.days_for_expiry)
                            ));

                        doc.Save(activator_filename);

                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("License activation from xml successfull.", TAG));
                    }
                    else
                    {
                        int count = ds.Tables[0].Rows.Count;

                        for (int i = 0; i < count; i++)
                        {
                            //ds.Tables[0].DefaultView.RowFilter = "userName = '" + userName + "' and databaseName = '" + databaseName + "' and serverName = '" + serverName + "' and system = '" + system + "'";

                            DataTable dt = (ds.Tables[0].DefaultView).ToTable();

                            if (dt.Rows.Count > 0)
                            {
                                ds.Tables[0].Rows[i].Delete();
                            }
                        }

                        //get data
                        string xmlData = ds.GetXml();

                        //save to file
                        ds.WriteXml(activator_filename);

                        XDocument doc = XDocument.Load(activator_filename);
                        doc.Element("Systems").Add(
                            new XElement("System",
                            new XAttribute("mac_address", activation_info.mac_address),
                            new XAttribute("computer_name", activation_info.computer_name),
                            new XAttribute("activation_key", activation_info.activation_key),
                            new XAttribute("date_activated", activation_info.date_activated),
                            new XAttribute("next_expiry_date", activation_info.next_expiry_date),
                            new XAttribute("days_for_expiry", activation_info.days_for_expiry)
                            ));

                        doc.Save(activator_filename);

                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("License activation from xml successfull.", TAG));
                    }
                }

                if (!File.Exists(activator_filename))
                {
                    List<SB_activator> systems = new List<SB_activator>() { 
                        new SB_activator(
                           activation_info.mac_address, 
                           activation_info.computer_name,
                           activation_info.activation_key,
                           activation_info.date_activated,
                           activation_info.next_expiry_date,
                           activation_info.days_for_expiry
                            )};

                    var xml = new XElement("Systems", systems.Select(x =>
                            new XElement("System",
                            new XAttribute("mac_address", x.mac_address),
                            new XAttribute("computer_name", x.computer_name),
                            new XAttribute("activation_key", x.activation_key),
                            new XAttribute("date_activated", x.date_activated),
                            new XAttribute("next_expiry_date", x.next_expiry_date),
                            new XAttribute("days_for_expiry", x.days_for_expiry)
                                           )));

                    xml.Save(activator_filename);

                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("License activation from xml successfull.", TAG));

                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void activate_from_registry()
        {
            try
            {
                if (activation_info == null)
                {
                    activation_info = Utils.get_license_activation_info(selected_system);
                }

                string registryPath = "SOFTWARE\\" + "Activator" + "\\" + Application.CompanyName + "\\" + selected_system;

                string keyvaluedata = string.Empty;

                using (Microsoft.Win32.RegistryKey MyReg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryPath, Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                {
                    if (MyReg != null)
                    {
                        keyvaluedata = string.Format("{0}", MyReg.GetValue("activation_key", "").ToString());
                    }
                    else
                    {
                        Microsoft.Win32.RegistryKey My_Reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registryPath);

                        My_Reg.SetValue("mac_address", activation_info.mac_address);
                        My_Reg.SetValue("computer_name", activation_info.computer_name);
                        My_Reg.SetValue("activation_key", activation_info.activation_key);
                        My_Reg.SetValue("date_activated", activation_info.date_activated);
                        My_Reg.SetValue("next_expiry_date", activation_info.next_expiry_date);
                        My_Reg.SetValue("days_for_expiry", activation_info.days_for_expiry);

                        My_Reg.Close();

                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("License activation from registry successfull.", TAG));

                    }
                }

                if (keyvaluedata.Length == 0)
                {
                    Microsoft.Win32.RegistryKey MyReg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registryPath);

                    MyReg.SetValue("mac_address", activation_info.mac_address);
                    MyReg.SetValue("computer_name", activation_info.computer_name);
                    MyReg.SetValue("activation_key", activation_info.activation_key);
                    MyReg.SetValue("date_activated", activation_info.date_activated);
                    MyReg.SetValue("next_expiry_date", activation_info.next_expiry_date);
                    MyReg.SetValue("days_for_expiry", activation_info.days_for_expiry);

                    MyReg.Close();

                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("License activation from registry successfull.", TAG));

                }
                else
                {
                    Microsoft.Win32.RegistryKey My_Reg = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(registryPath);

                    My_Reg.SetValue("mac_address", activation_info.mac_address);
                    My_Reg.SetValue("computer_name", activation_info.computer_name);
                    My_Reg.SetValue("activation_key", activation_info.activation_key);
                    My_Reg.SetValue("date_activated", activation_info.date_activated);
                    My_Reg.SetValue("next_expiry_date", activation_info.next_expiry_date);
                    My_Reg.SetValue("days_for_expiry", activation_info.days_for_expiry);

                    My_Reg.Close();

                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("License activation from registry successfull.", TAG));

                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void cbosystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                selected_system = cbosystem.SelectedValue.ToString();

                set_system_path();

                set_database_properties();

                disable_controls();

                current_action = "system";

                _task_start_time = DateTime.Now;

                //This allows the BackgroundWorker to be cancelled in between tasks
                bgWorker.WorkerSupportsCancellation = true;
                //This allows the worker to report progress between completion of tasks...
                //this must also be used in conjunction with the ProgressChanged event
                bgWorker.WorkerReportsProgress = true;

                //this assigns event handlers for the backgroundWorker
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.RunWorkerCompleted += bgWorker_WorkComplete;
                /* When you wish to have something occur when a change in progress
                    occurs, (like the completion of a specific task) the "ProgressChanged"
                    event handler is used. Note that ProgressChanged events may be invoked
                    by calls to bgWorker.ReportProgress(...) only if bgWorker.WorkerReportsProgress
                    is set to true. */
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;

                //tell the backgroundWorker to raise the "DoWork" event, thus starting it.
                //Check to make sure the background worker is not already running.
                if (!bgWorker.IsBusy)
                    bgWorker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void disable_controls()
        {
            btnfetchdatabases.Enabled = false;
            btnactivate.Enabled = false;
            btnconnect.Enabled = false;
            btnfetchservers.Enabled = false;

            cbosystem.Enabled = false;
            cboserver.Enabled = false;
            txtusername.Enabled = false;
            txtpassword.Enabled = false;
            cbodatabase.Enabled = false;

            chkIntegratedSecurity.Enabled = false;
            chkremember.Enabled = false;
        }
        private void enable_controls()
        {
            btnfetchdatabases.Enabled = true;
            btnactivate.Enabled = true;
            btnconnect.Enabled = true;
            btnfetchservers.Enabled = true;

            cbosystem.Enabled = true;
            cboserver.Enabled = true;
            txtusername.Enabled = true;
            txtpassword.Enabled = true;
            cbodatabase.Enabled = true;

            chkIntegratedSecurity.Enabled = true;
            chkremember.Enabled = true;
        }
        private void set_system_path()
        {
            try
            {
                string selected_system = cbosystem.SelectedValue.ToString();

                switch (RUNTIME)
                {
                    case "D":
                        switch (selected_system)
                        {
                            case "Soft Books Payroll":
                                PAYROLL_SYSTEM_PATH = @"D:\wakxpx\csharp\visualstudio\2010\SBPayroll\Build\Debug";
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("PAYROLL_SYSTEM_PATH [ " + PAYROLL_SYSTEM_PATH + " ] ", TAG));
                                break;
                            case "Soft Books Sacco":
                                SACCO_SYSTEM_PATH = @"D:\wakxpx\csharp\visualstudio\2010\SBSacco\Build\Debug";
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("SACCO_SYSTEM_PATH [ " + SACCO_SYSTEM_PATH + " ] ", TAG));
                                break;
                            case "Soft Books School":
                                SCHOOL_SYSTEM_PATH = @"D:\wakxpx\csharp\visualstudio\2010\SBSchool\Build\Debug";
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("SCHOOL_SYSTEM_PATH [ " + SCHOOL_SYSTEM_PATH + " ] ", TAG));
                                break;
                        }
                        break;
                    case "P":
                        switch (selected_system)
                        {
                            case "Soft Books Payroll":
                                PAYROLL_SYSTEM_PATH = @"C:\Program Files (x86)\Software Providers\Soft Books Payroll";
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("PAYROLL_SYSTEM_PATH [ " + PAYROLL_SYSTEM_PATH + " ] ", TAG));
                                break;
                            case "Soft Books Sacco":
                                SACCO_SYSTEM_PATH = @"C:\Program Files (x86)\Software Providers\Soft Books Sacco";
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("SACCO_SYSTEM_PATH [ " + SACCO_SYSTEM_PATH + " ] ", TAG));
                                break;
                            case "Soft Books School":
                                SCHOOL_SYSTEM_PATH = @"C:\Program Files (x86)\Software Providers\Soft Books School";
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("SCHOOL_SYSTEM_PATH [ " + SCHOOL_SYSTEM_PATH + " ] ", TAG));
                                break;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void set_database_properties()
        {
            try
            {
                string selected_system = cbosystem.SelectedValue.ToString();

                switch (selected_system)
                {
                    case "Soft Books Payroll":
                        SBApplication = "SBPayroll";
                        Metadata = "SBPayrollDBEntities";
                        DatabaseVersionExtPropertyKey = SBApplication + "Version";
                        break;
                    case "Soft Books Sacco":
                        SBApplication = "SBSacco";
                        Metadata = "SBSaccoDBEntities";
                        DatabaseVersionExtPropertyKey = SBApplication + "Version";
                        break;
                    case "Soft Books School":
                        SBApplication = "SBSchool";
                        Metadata = "SBSchoolDBEntities";
                        DatabaseVersionExtPropertyKey = SBApplication + "Version";
                        break;
                }

                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("SBApplication [ " + SBApplication + " ] ", TAG));
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Metadata [ " + Metadata + " ] ", TAG));
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("DatabaseVersionExtPropertyKey [ " + DatabaseVersionExtPropertyKey + " ] ", TAG));
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }

        private void btn_systems_paths_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                systems_paths_form spf = new systems_paths_form();
                spf.ShowDialog();
            }
            catch (Exception ex)
            {
                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
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
                Invoke(new Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Enumerating servers using sql data source enumerator.", TAG));
                }));

                List<string> lst_servers = new List<string>();
                string servername = Environment.MachineName;
                DataTable sql_servers_table = SqlDataSourceEnumerator.Instance.GetDataSources();
                lst_servers.Add(servername);

                foreach (DataRow row in sql_servers_table.Rows)
                {
                    string name = row[1] + @"\" + (row[2]).ToString();
                    Console.WriteLine(name);

                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(name, TAG));
                    }));

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
                Invoke(new Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Enumerating servers using smo application.", TAG));
                }));

                List<string> lst_servers = new List<string>();
                string servername = Environment.MachineName;
                DataTable sql_servers_table = SmoApplication.EnumAvailableSqlServers(false);
                lst_servers.Add(servername);

                foreach (DataRow row in sql_servers_table.Rows)
                {
                    string name = row[1] + @"\" + (row[2]).ToString();
                    Console.WriteLine(name);

                    Invoke(new Action(() =>
                    {
                        this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(name, TAG));
                    }));

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
                Invoke(new Action(() =>
                {
                    this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("Enumerating servers using registry.", TAG));
                }));

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

                            Invoke(new Action(() =>
                            {
                                this._notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(name, TAG));
                            }));

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



    }
}
