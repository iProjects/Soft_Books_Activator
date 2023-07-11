using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Soft_Books_Activator
{
    public class SBSystem
    {
        public string Name { get; set; }
        public string Application { get; set; }
        public string Database { get; set; }
        public string Server { get; set; }
        public string AttachDB { get; set; }
        public string Metadata { get; set; }
        public string Version { get; set; }
        public bool Default { get; set; }

        public SBSystem(string name, string app, string database, string server, string attach, string metadata, string ver, bool def)
        {
            this.Name = name;
            this.Application = app;
            this.Database = database;
            this.Server = server;
            this.AttachDB = attach;
            this.Metadata = metadata;
            this.Version = ver;
            this.Default = def;
        }
    }

    public class SBSystem_Exp
    {
        public string Name { get; set; }
        public string Application { get; set; }

        public SBSystem_Exp(string name, string app)
        {
            this.Name = name;
            this.Application = app;
        }
    }

    public class SBSystem_DTO
    {
        public string Name { get; set; }
        public string Application { get; set; }

        public SBSystem_DTO(string name, string app)
        {
            this.Name = name;
            this.Application = app;
        }
    }

    public class SBSystem_DCP_DTO
    {
        public string Name { get; set; }
        public string Application { get; set; }
        public string Database { get; set; }
        public bool Defaultsn { get; set; }
        public bool Defaultun { get; set; }
        public bool Defaultpd { get; set; }

        public SBSystem_DCP_DTO(string name, string app, string database, bool defaultsn, bool defaultun, bool defaultpd)
        {
            this.Name = name;
            this.Application = app;
            this.Database = database;
            this.Defaultsn = defaultsn;
            this.Defaultun = defaultun;
            this.Defaultpd = defaultpd;
        }
    }

    public class ServerDatabase
    {
        public SBSystem System;
        public double Size;
        public ServerDatabase(SBSystem system, double sz)
        {
            this.System = system;
            this.Size = sz;
        }
    }

    public class SB_Login
    {
        public string system { get; set; }
        public string serverName { get; set; }
        public string databaseName { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string IntegratedSecurity { get; set; }
        public string remember { get; set; }

        public SB_Login(string system, string serverName, string databaseName, string userName, string password, string IntegratedSecurity, string remember)
        {
            this.system = system;
            this.serverName = serverName;
            this.databaseName = databaseName;
            this.userName = userName;
            this.password = password;
            this.IntegratedSecurity = IntegratedSecurity;
            this.remember = remember;
        }
    }

    public class SB_activator
    {
        public string mac_address { get; set; }
        public string computer_name { get; set; }
        public string activation_key { get; set; }
        public string date_activated { get; set; }
        public string next_expiry_date { get; set; }
        public string days_for_expiry { get; set; }

        public SB_activator(string mac_address, string computer_name, string activation_key, string date_activated, string next_expiry_date, string days_for_expiry)
        {
            this.mac_address = mac_address;
            this.computer_name = computer_name;
            this.activation_key = activation_key;
            this.date_activated = date_activated;
            this.next_expiry_date = next_expiry_date;
            this.days_for_expiry = days_for_expiry;
        }
    }
    public class server_databases
    {
        public string Name { get; set; }
        public string Size { get; set; }
        public string Owner { get; set; }
        public string system { get; set; }

        public server_databases(string Name, string Size, string Owner, string system)
        {
            this.Name = Name;
            this.Size = Size;
            this.Owner = Owner;
            this.system = system;
        }
    }
    public class systems_paths
    {
        public string path { get; set; }
        public string sys { get; set; }

        public systems_paths(string _path, string _sys)
        {
            this.path = _path;
            this.sys = _sys;
        }
    }




}
