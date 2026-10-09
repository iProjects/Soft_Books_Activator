using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using console_dto;
using console_commonlib;
using System.Threading;
using System.IO;

namespace console_business
{
    public sealed class helper_utils
    {
        // Because the _instance member is made private, the only way to get the single
        // instance is via the static Instance property below. This can also be similarly
        // achieved with a GetInstance() method instead of the property.
        private static helper_utils singleInstance;

        public static helper_utils getInstance(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new helper_utils(notificationmessageEventname);
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        public static helper_utils getInstance()
        {
            // The first call will create the one and only instance.
            if (singleInstance == null)
                singleInstance = new helper_utils();
            // Every call afterwards will return the single instance created above.
            return singleInstance;
        }

        public helper_utils()
        {
            TAG = this.GetType().Name;

            AppDomain.CurrentDomain.UnhandledException += new
UnhandledExceptionEventHandler(UnhandledException);
            System.Windows.Forms.Application.ThreadException += new ThreadExceptionEventHandler(ThreadException);

            //Subscribing to the event: 
            //Dynamically:
            //EventName += HandlerName;
            _notificationmessageEventname += notificationmessageHandler;

            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs("finished helper_utils initialization", TAG));

        }

        public string TAG;
        //Event declaration:
        //event for publishing messages to output
        public event EventHandler<notificationmessageEventArgs> _notificationmessageEventname;
        //list to hold messages
        public List<notificationdto> _lstnotificationdto = new List<notificationdto>();

        public helper_utils(EventHandler<notificationmessageEventArgs> notificationmessageEventname)
        {
            _notificationmessageEventname = notificationmessageEventname;
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Log.Write_To_Log_File_web(ex);
            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
        }

        private void ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Log.Write_To_Log_File_web(ex);
            _notificationmessageEventname.Invoke(this, new notificationmessageEventArgs(ex.ToString(), TAG));
        }

        //Event handler declaration:
        private void notificationmessageHandler(object sender, notificationmessageEventArgs args)
        {
            try
            {
                /* Handler logic */
                notificationdto _notificationdto = new notificationdto();

                DateTime currentDate = DateTime.Now;
                String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

                String _logtext = Environment.NewLine + "[ " + dateTimenow + " ]   " + args.message;

                _notificationdto._notification_message = _logtext;
                _notificationdto._created_datetime = dateTimenow;
                _notificationdto.TAG = args.TAG;

                _lstnotificationdto.Add(_notificationdto);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void log_messages(string message, string TAG)
        {
            notificationdto _notificationdto = new notificationdto();
            DateTime currentDate = DateTime.Now;
            String dateTimenow = currentDate.ToString("dd-MM-yyyy HH:mm:ss tt");

            String _logtext = Environment.NewLine + "[ " + dateTimenow + " ]   " + message;

            _notificationdto._notification_message = _logtext;
            _notificationdto._created_datetime = dateTimenow;
            _notificationdto.TAG = TAG;

            create_log_in_database(_notificationdto);
        }
        private void create_log_in_database(notificationdto _notificationdto)
        {
            try
            {
                log_dto dto = new log_dto();
                dto.message = _notificationdto._notification_message;
                dto.timestamp = _notificationdto._created_datetime;
                dto.tag = _notificationdto.TAG;

                //save data in database.
                List<responsedto> _lstresponse = new List<responsedto>();
                _lstresponse = businesslayerapisingleton.getInstance(_notificationmessageEventname).create_log_in_database(dto);

                foreach (var response in _lstresponse)
                {
                    if (!string.IsNullOrEmpty(response.responsesuccessmessage))
                    {
                        //Console.WriteLine(response.responsesuccessmessage);
                    }
                    if (!string.IsNullOrEmpty(response.responseerrormessage))
                    {
                        //Console.WriteLine(response.responseerrormessage);
                    }
                }

                Write_To_Log_File_temp_dir(_notificationdto._notification_message);

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
            }
        }

        private void Write_To_Log_File_temp_dir(string msg)
        {
            try
            {
                Log.Write_To_Log_File_temp_dir(new Exception(msg));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void set_up_databases()
        {
            try
            {
                List<responsedto> _lstresponse = businesslayerapisingleton.getInstance(_notificationmessageEventname).set_up_databases();

                foreach (var response in _lstresponse)
                {
                    if (!string.IsNullOrEmpty(response.responsesuccessmessage))
                    {
                        //Console.WriteLine(response.responsesuccessmessage);
                        helper_utils.getInstance(_notificationmessageEventname).log_messages(response.responsesuccessmessage, TAG);
                    }
                    if (!string.IsNullOrEmpty(response.responseerrormessage))
                    {
                        //Console.WriteLine(response.responseerrormessage);
                        helper_utils.getInstance(_notificationmessageEventname).log_messages(response.responseerrormessage, TAG);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static bool Write_To_Log_File_temp_dir(Exception sourceException)
        {
            string temp_path = Path.GetTempPath();
            string app_name = System.Configuration.ConfigurationManager.AppSettings["APP_NAME"];
            string log_file_name = app_name + ".log";
            var _temp_file = Path.Combine(temp_path, log_file_name);

            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(_temp_file, FileMode.Append, FileAccess.Write);
                sw = new StreamWriter(fs);
                sw.WriteLine("==================================================================");
                sw.WriteLine("ERROR OCCOURED AT :" + DateTime.Now.ToString());
                sw.WriteLine("SOURCE:" + sourceException.Source);
                sw.WriteLine("MESSAGE:" + sourceException.Message);
                sw.WriteLine("Whole Exception:" + sourceException.ToString());
                sw.WriteLine("==================================================================");
                sw.WriteLine("");
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }

                if (fs != null)
                {
                    fs.Close();
                }
            }
        }




    }
}