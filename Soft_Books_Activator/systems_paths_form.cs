using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using System.ComponentModel;

namespace Soft_Books_Activator
{

    public class systems_paths_form : System.Windows.Forms.Form
    {

        public string TAG;

        /*Default values */
        const string APPLICATION = "SBPayroll";
        const string VERSION = "1.0.0.0";
        const string METADATA = "SBPayrollDBEntities";
        DataSet ds;
        BindingSource bs;
        private Button btndelete;
        private DataGridView dataGridView_systems_paths;
        private BindingSource bindingSource_systems_paths;
        private TextBox txt_systems_paths;
        private Button btnadd_system_path_to_activate;
        private Button btnrefresh;
        private ErrorProvider errorProvider;
        private Label label2;
        private ComboBox cbosystem;
        private DataGridViewTextBoxColumn Columnsys;
        private DataGridViewTextBoxColumn Columnpath;
        List<systems_paths> lst_systems_paths_from_xml = new List<systems_paths>();

        public systems_paths_form()
        {
            InitializeComponent();

            TAG = this.GetType().Name;

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(ThreadException);

        }

        private void systems_paths_form_Load(object sender, System.EventArgs e)
        {
            try
            {
                bs = new BindingSource(); //Private Variable class level 
                ds = new DataSet();

                var _systems = new BindingList<KeyValuePair<string, string>>();
                _systems.Add(new KeyValuePair<string, string>("Soft Books Payroll", "Soft Books Payroll"));
                _systems.Add(new KeyValuePair<string, string>("Soft Books Sacco", "Soft Books Sacco"));
                _systems.Add(new KeyValuePair<string, string>("Soft Books School", "Soft Books School"));
                cbosystem.DataSource = _systems;
                cbosystem.ValueMember = "Key";
                cbosystem.DisplayMember = "Value";

                lst_systems_paths_from_xml = get_saved_directories();
                bindingSource_systems_paths.DataSource = lst_systems_paths_from_xml;

                this.dataGridView_systems_paths.AutoGenerateColumns = false;
                this.dataGridView_systems_paths.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                this.dataGridView_systems_paths.DataSource = bindingSource_systems_paths;
                groupBox2.Text = lst_systems_paths_from_xml.Count.ToString();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteToErrorLogFile_and_EventViewer(ex);

                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "\n" + ex.InnerException.Message;
                MessageBox.Show(msg, Utils.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Console.WriteLine(ex.ToString());
        }

        private void ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            Console.WriteLine(ex.ToString());
        }

        private List<systems_paths> get_saved_directories()
        {
            try
            {
                string _filename = Utils.get_system_paths_file();

                if (File.Exists(_filename))
                {
                    DataSet ds = new DataSet();

                    ds.ReadXml(_filename);

                    if (ds.Tables.Count > 0)
                    {
                        int count = ds.Tables[0].Rows.Count;
                        Console.WriteLine(count);

                        DataTable dt = (ds.Tables[0].DefaultView).ToTable();

                        for (int i = 0; i < count; i++)
                        {
                            string system_path = ds.Tables[0].Rows[i].ItemArray[0].ToString();
                            string sys = ds.Tables[0].Rows[i].ItemArray[1].ToString();
                            lst_systems_paths_from_xml.Add(new systems_paths(system_path, sys));
                            Console.WriteLine(system_path);
                        }
                    }
                    Console.WriteLine("file for saved directories exists.");
                }
                else
                {
                    Console.WriteLine("file for saved directories does not exist.");
                }
                return lst_systems_paths_from_xml;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteToErrorLogFile_and_EventViewer(ex);
                return null;
            }
        }
        private void refreshgrid()
        {
            try
            {
                bindingSource_systems_paths.DataSource = null;
                this.dataGridView_systems_paths.DataSource = null;
                lst_systems_paths_from_xml.Clear();

                lst_systems_paths_from_xml = get_saved_directories();
                bindingSource_systems_paths.DataSource = lst_systems_paths_from_xml;

                this.dataGridView_systems_paths.DataSource = bindingSource_systems_paths;
                groupBox2.Text = lst_systems_paths_from_xml.Count.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void btndelete_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (dataGridView_systems_paths.SelectedRows.Count != 0)
                {
                    if (msgboxform.Show(String.Format("Are you sure you want to delete the selected record?", ""), TAG, msgtype.error) == DialogResult.OK)
                    {
                        systems_paths dir = (systems_paths)bindingSource_systems_paths.Current;

                        string _filename = Utils.get_system_paths_file();

                        if (File.Exists(_filename))
                        {
                            DataSet ds = new DataSet();

                            ds.ReadXml(_filename);

                            if (ds.Tables.Count > 0)
                            {
                                //if tables count is greater than zero we have records in xml

                                int count = ds.Tables[0].Rows.Count;

                                for (int i = 0; i < count; i++)
                                {
                                    ds.Tables[0].DefaultView.RowFilter = "systems_paths = '" + dir.path + "'";

                                    DataTable dt = (ds.Tables[0].DefaultView).ToTable();

                                    int counta = dt.Rows.Count;

                                    if (counta > 0)
                                    {
                                        //we have a matching recording between what has been selected and what is in the xml
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
                                ds.WriteXml(_filename);
                            }
                        }
                        else
                        {
                            Console.WriteLine("file for systems paths does not exist.");
                        }

                    }
                }

                refreshgrid();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.WriteToErrorLogFile_and_EventViewer(ex);

                string msg = ex.Message;
                if (ex.InnerException != null)
                    msg += "\n" + ex.InnerException.Message;
                MessageBox.Show(msg, Utils.APP_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void dataGridView_saved_directories_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                e.ThrowException = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnadd_systems_paths_Click(object sender, System.EventArgs e)
        {
            try
            {
                errorProvider.Clear();
                var selected_system = cbosystem.SelectedValue.ToString();
                if (string.IsNullOrEmpty(txt_systems_paths.Text))
                {
                    errorProvider.SetError(txt_systems_paths, "system directory cannot be null.");
                    Console.WriteLine("system directory cannot be null.");
                }
                else
                {
                    save_systems_paths(txt_systems_paths.Text, selected_system);
                    refreshgrid();
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void save_systems_paths(string systems_paths, string selected_system)
        {
            try
            {
                if (!Directory.Exists(systems_paths))
                {
                    errorProvider.SetError(txt_systems_paths, "directory [ " + systems_paths + " ] does not exist.");
                    Console.WriteLine("directory [ " + systems_paths + " ] does not exist.");
                }
                else
                {
                    save_in_xml(systems_paths, selected_system);
                    //save_in_sqlite(systems_paths);
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void save_in_xml(string systems_paths, string selected_system)
        {
            try
            {
                string _filename = Utils.get_system_paths_file();
                bool exists = false;

                if (File.Exists(_filename))
                {
                    lst_systems_paths_from_xml = new List<systems_paths>();
                    lst_systems_paths_from_xml = get_saved_directories();

                    foreach (systems_paths dir in lst_systems_paths_from_xml)
                    {
                        if (dir.path.Equals(systems_paths))
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        XDocument doc = XDocument.Load(_filename);

                        doc.Element("Systems").Add(
                            new XElement("System",
                            new XAttribute("systems_paths", systems_paths),
                            new XAttribute("sys", selected_system)
                            ));

                        doc.Save(_filename);

                        Console.WriteLine("directory saved in xml successfully.");
                        refreshgrid();
                    }
                    else if (exists)
                    {
                        Console.WriteLine("directory [ " + systems_paths + " ] exists.");
                        errorProvider.SetError(txt_systems_paths, "directory [ " + systems_paths + " ] exists.");
                    }
                }
                else if (!File.Exists(_filename))
                {
                    List<String> systems = new List<String>() { 
                        systems_paths
                     };

                    var xml = new XElement("Systems", systems.Select(x =>
                            new XElement("System",
                            new XAttribute("systems_paths", systems_paths),
                            new XAttribute("sys", selected_system)
                            )));

                    xml.Save(_filename);

                    Console.WriteLine("directory saved in xml successfully.");
                    refreshgrid();
                }
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }
        private void btnrefresh_Click(object sender, System.EventArgs e)
        {
            try
            {
                refreshgrid();
            }
            catch (Exception ex)
            {
                Log.WriteToErrorLogFile_and_EventViewer(ex);
            }
        }


        #region "Windows Form Designer generated code"
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(systems_paths_form));
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnadd_system_path_to_activate = new System.Windows.Forms.Button();
            this.txt_systems_paths = new System.Windows.Forms.TextBox();
            this.btndelete = new System.Windows.Forms.Button();
            this.btnrefresh = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dataGridView_systems_paths = new System.Windows.Forms.DataGridView();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.bindingSource_systems_paths = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.cbosystem = new System.Windows.Forms.ComboBox();
            this.Columnsys = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Columnpath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_systems_paths)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource_systems_paths)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(767, 71);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 24);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbosystem);
            this.groupBox1.Controls.Add(this.btnadd_system_path_to_activate);
            this.groupBox1.Controls.Add(this.txt_systems_paths);
            this.groupBox1.Controls.Add(this.btndelete);
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnrefresh);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 311);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(853, 124);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "systems paths";
            // 
            // btnadd_system_path_to_activate
            // 
            this.btnadd_system_path_to_activate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnadd_system_path_to_activate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnadd_system_path_to_activate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnadd_system_path_to_activate.Location = new System.Drawing.Point(425, 72);
            this.btnadd_system_path_to_activate.Name = "btnadd_system_path_to_activate";
            this.btnadd_system_path_to_activate.Size = new System.Drawing.Size(99, 23);
            this.btnadd_system_path_to_activate.TabIndex = 6;
            this.btnadd_system_path_to_activate.Text = "save directory";
            this.btnadd_system_path_to_activate.UseVisualStyleBackColor = false;
            this.btnadd_system_path_to_activate.Click += new System.EventHandler(this.btnadd_systems_paths_Click);
            // 
            // txt_systems_paths
            // 
            this.txt_systems_paths.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_systems_paths.Dock = System.Windows.Forms.DockStyle.Left;
            this.txt_systems_paths.Location = new System.Drawing.Point(3, 16);
            this.txt_systems_paths.Multiline = true;
            this.txt_systems_paths.Name = "txt_systems_paths";
            this.txt_systems_paths.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_systems_paths.Size = new System.Drawing.Size(389, 105);
            this.txt_systems_paths.TabIndex = 5;
            // 
            // btndelete
            // 
            this.btndelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btndelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btndelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btndelete.Location = new System.Drawing.Point(644, 71);
            this.btndelete.Name = "btndelete";
            this.btndelete.Size = new System.Drawing.Size(105, 24);
            this.btndelete.TabIndex = 4;
            this.btndelete.Text = "delete selected";
            this.btndelete.UseVisualStyleBackColor = false;
            this.btndelete.Click += new System.EventHandler(this.btndelete_Click);
            // 
            // btnrefresh
            // 
            this.btnrefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnrefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnrefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnrefresh.Location = new System.Drawing.Point(542, 71);
            this.btnrefresh.Name = "btnrefresh";
            this.btnrefresh.Size = new System.Drawing.Size(84, 24);
            this.btnrefresh.TabIndex = 2;
            this.btnrefresh.Text = "reload";
            this.btnrefresh.UseVisualStyleBackColor = false;
            this.btnrefresh.Click += new System.EventHandler(this.btnrefresh_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dataGridView_systems_paths);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(853, 311);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            // 
            // dataGridView_systems_paths
            // 
            this.dataGridView_systems_paths.AllowUserToAddRows = false;
            this.dataGridView_systems_paths.AllowUserToDeleteRows = false;
            this.dataGridView_systems_paths.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_systems_paths.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Columnsys,
            this.Columnpath});
            this.dataGridView_systems_paths.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_systems_paths.Location = new System.Drawing.Point(3, 16);
            this.dataGridView_systems_paths.MultiSelect = false;
            this.dataGridView_systems_paths.Name = "dataGridView_systems_paths";
            this.dataGridView_systems_paths.ReadOnly = true;
            this.dataGridView_systems_paths.Size = new System.Drawing.Size(847, 292);
            this.dataGridView_systems_paths.TabIndex = 0;
            this.dataGridView_systems_paths.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView_saved_directories_DataError);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(431, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "system*";
            // 
            // cbosystem
            // 
            this.cbosystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbosystem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbosystem.FormattingEnabled = true;
            this.cbosystem.Location = new System.Drawing.Point(477, 19);
            this.cbosystem.Name = "cbosystem";
            this.cbosystem.Size = new System.Drawing.Size(290, 21);
            this.cbosystem.TabIndex = 9;
            // 
            // Columnsys
            // 
            this.Columnsys.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Columnsys.DataPropertyName = "sys";
            this.Columnsys.HeaderText = "System";
            this.Columnsys.Name = "Columnsys";
            this.Columnsys.ReadOnly = true;
            this.Columnsys.Width = 150;
            // 
            // Columnpath
            // 
            this.Columnpath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Columnpath.DataPropertyName = "path";
            this.Columnpath.HeaderText = "System Path";
            this.Columnpath.Name = "Columnpath";
            this.Columnpath.ReadOnly = true;
            // 
            // systems_paths_form
            // 
            this.AcceptButton = this.btnrefresh;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(853, 435);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "systems_paths_form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "systems paths";
            this.Load += new System.EventHandler(this.systems_paths_form_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_systems_paths)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource_systems_paths)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.ComponentModel.IContainer components;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion "Windows Form Designer generated code"









    }
}
