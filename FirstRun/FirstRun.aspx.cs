using System;
using System.Web;
using _min.Common;
using CC = _min.Common.Constants;
using _min.Models;
using _min.Interfaces;
using System.Web.Security;
using System.IO;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.Configuration;
using System.Xml;
using System.Reflection;

namespace _min.FirstRun
{

    
    public class FirstRunData
    {

        public _min.Common.DbServer ServerType { get; set; }
    }

    public partial class FirstRun : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            Errors.Items.Clear();

            string serverType = ServerTypeDrop.SelectedValue;
            DbServer serverTypeParsed;
            if (!Enum.TryParse<DbServer>(serverType, out serverTypeParsed))
            {
                Errors.Items.Add("Please, choose the type of database engine you wish to use.");
                return;
            }

            IBaseDriver drv = null;
            switch (serverTypeParsed)
            {
                case DbServer.MySql:
                    drv = new BaseDriverMySql(SystemConnstringTextBox.Text);
                    break;
                case DbServer.MsSql:
                    drv = new BaseDriverMsSql(SystemConnstringTextBox.Text);
                    break;
            }

            

            try
            {
                drv.TestConnection();
                //drv.TestDatabaseIsEmpty();
            }
            catch (Exception ex)
            {
                Errors.Items.Add(ex.Message);
                return;
            }

            if (UsernameTextBox.Text == "")
            {
                Errors.Items.Add("Please, insert the initial user's name");
                return;
            }

            if (PasswordTextBox.Text.Length < 7)
            {
                Errors.Items.Add("The password must be at least 7 characters long.");
                return;
            }

            if (PasswordTextBox.Text != RetypePasswordTextBox.Text)
            {
                Errors.Items.Add("The passwords do not match.");
                return;
            }


            switch (serverTypeParsed)
            {
                case DbServer.MySql:
                    MySql.Data.MySqlClient.MySqlConnection connection = new MySql.Data.MySqlClient.MySqlConnection(SystemConnstringTextBox.Text);
                    try
                    {
                        MySql.Data.MySqlClient.MySqlScript script = new MySql.Data.MySqlClient.MySqlScript(connection);
                        string scriptText = File.ReadAllText(HttpContext.Current.Server.MapPath(CC.MYSQL_SCHEMA_FILE_PATH));
                        script.Query = scriptText;
                        script.Query = scriptText;
                        connection.Open();
                        script.Execute();
                        connection.Clone();
                    }
                    catch (Exception esql1)
                    {
                        Errors.Items.Add(esql1.Message);
                        connection.Close();
                        return;
                    }
                    break;
                case DbServer.MsSql:
                    SqlConnection conn = new SqlConnection(SystemConnstringTextBox.Text);
                    try
                    {
                        string query = File.ReadAllText(HttpContext.Current.Server.MapPath(CC.MSSQL_SCHEMA_FILE_PATH));
                        Microsoft.SqlServer.Management.Smo.Server sqlServer = new Server(new ServerConnection(conn));
                        conn.Open();
                        sqlServer.ConnectionContext.ExecuteNonQuery(query);
                        conn.Close();

                        SqlMembershipProvider mssqlProvider = new SqlMembershipProvider();
                    }
                    catch (Exception esql2)
                    {
                        Errors.Items.Add(esql2.Message);
                        conn.Close();
                        return;
                    }
                    break;
            }

            var configuration = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            foreach (string s in configuration.Sections.Keys)
                Errors.Items.Add(s);
            foreach (string s in configuration.SectionGroups.Keys)
                Errors.Items.Add(s);
            var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");

            System.Web.Security.MembershipProvider membership = null;

            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;
            string mail = "jankovic.rj@gmail.com";

            MembershipCreateStatus status;
                    

            switch (serverTypeParsed)
            {
                case DbServer.MySql:
                    section.ConnectionStrings["MySqlServer"].ConnectionString = SystemConnstringTextBox.Text;
                    configuration.AppSettings.Settings["ServerType"].Value = "MySql";
                    configuration.Save();
                    SetDefaultMembershipProvider("MySqlMembershipProvider");
                    
                    //ConfigurationManager.RefreshSection("appSettings");
                    //ConfigurationManager.RefreshSection("connectionStrings");
                    
                    //MySql.Web.Security.MySQLMembershipProvider provider = new MySql.Web.Security.MySQLMembershipProvider();

                    //System.Collections.Specialized.NameValueCollection config = new System.Collections.Specialized.NameValueCollection();
                    
                /*
                    <add autogenerateschema="true" connectionStringName="MySqlServer" enablePasswordRetrieval="false" enablePasswordReset="true" 
                    requiresQuestionAndAnswer="false" applicationName="/" requiresUniqueEmail="false" passwordFormat="Clear" maxInvalidPasswordAttempts="5" 
                    minRequiredPasswordLength="7" minRequiredNonalphanumericCharacters="0" passwordAttemptWindow="10" passwordStrengthRegularExpression="" 
                    name="MySqlMembershipProvider" type="MySql.Web.Security.MySQLMembershipProvider, MySql.Web, Version=6.5.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d" />
                    */

                    /*
                    config.Add("actualConnectionString", SystemConnstringTextBox.Text);
                    config.Add("connectionStringName", "MySqlServer");
                    config.Add("enablePasswordRetrieval", "false");
                    config.Add("autogenerateschema", "false");
                    config.Add("enablePasswordReset", "true");
                                        config.Add("requiresQuestionAndAnswer", "false");
                                        config.Add("applicationName", "/");
                                        config.Add("requiresUniqueEmail", "false");
                                        config.Add("psswordFormat", "Clear");
                                        config.Add("maxInvalidPasswordAttempts", "5");
                                        config.Add("minRequiredPasswordLength", "7");
                                        config.Add("minRequiredNonalphanumericCharacters", "0");
                                        config.Add("passwordAttemptWindow", "10");
                                        config.Add("passwordStrengthRegularExpression", "");
                                                            config.Add("type", "MySql.Web.Security.MySQLMembershipProvider, MySql.Web, Version=6.5.4.0, Culture=neutral, PublicKeyToken=c5687fc88969c44d");
                                                            //string s = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;


                    provider.Initialize("MySqlMembershipProvider", config);
                    
                    Membership.Providers.Clear();
                        
                     Membership.Providers.Add(provider);
                    */

                    var settingsMy = ConfigurationManager.ConnectionStrings["MsSqlServer"];
                    var fiMy = typeof( ConfigurationElement ).GetField( "_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic );
                    fiMy.SetValue(settingsMy, false);
                    settingsMy.ConnectionString = SystemConnstringTextBox.Text;
                    
                    membership = Membership.Providers["MySqlMembershipProvider"];

                    membership.CreateUser(username, password, mail, "Dummy question", "Dummy answer", true, 1, out status);
                    break;
                case DbServer.MsSql:
                    section.ConnectionStrings["MsSqlServer"].ConnectionString = SystemConnstringTextBox.Text;
                    configuration.AppSettings.Settings["ServerType"].Value = "MsSql";
                    configuration.Save();
                    SetDefaultMembershipProvider("MsSqlMembershipProvider");
                    //ConfigurationManager.RefreshSection("appSettings");
                    //ConfigurationManager.ConnectionStrings["MsSqlServer"].ConnectionString = SystemConnstringTextBox.Text;
                    
                    var settings = ConfigurationManager.ConnectionStrings["MsSqlServer"];
                    var fi = typeof( ConfigurationElement ).GetField( "_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic );
                    fi.SetValue(settings, false);
                    settings.ConnectionString = SystemConnstringTextBox.Text;


                    membership = Membership.Providers["MsSqlMembershipProvider"];

                    Guid key = new Guid(1,2,3, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 });
                    ((SqlMembershipProvider)membership).CreateUser(username, password, mail, "Dummy question", "Dummy answer", true, key, out status);

                    break;
            }

            int totalUsers;
            MembershipUser user = membership.FindUsersByName(username, 0, 1, out totalUsers)[username];
            SystemDriver sysDriver = new SystemDriver(drv);
            sysDriver.SetUserRights((user.ProviderUserKey), null, 11110);

            //configuration.Save();

            /*
            // Add an Application Setting.
            config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings("SystemConnstring",
                SystemConnstringTextBox.Text));

            // Save the configuration file.
            config.Save(ConfigurationSaveMode.Modified);

            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
            */

            /*
            System.Configuration.Configuration conf = WebConfigurationManager.OpenWebConfiguration(Server.MapPath);
            conf.ConnectionStrings.ConnectionStrings["comp1"].ConnectionString = _connection_comp1;
            conf.ConnectionStrings.ConnectionStrings["comp2"].ConnectionString = _connection_comp2;
            conf.AppSettings.Settings["CompanyCode"].Value = _company_code;
            conf.Save();
            
            System.Configuration.ConfigurationManager.AppSettings["FirstRun"] = "False";
               */

            //SetFirstRunFalse();

            //configuration.AppSettings.Settings["FirstRun"].Value = "False";
            var config2 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            config2.AppSettings.Settings["FirstRun"].Value = "False";
            config2.Save();
            Response.RedirectToRoute("DefaultRoute");
            //Errors.Items.Add("Done");
        }


        private void SetDefaultMembershipProvider(string provider)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/Web.config"));
            XmlNode root = doc.DocumentElement;
            XmlNode myNode = root.SelectSingleNode("descendant::membership");
            XmlAttribute attr = doc.CreateAttribute("defaultProvider");
            attr.Value = provider;
            myNode.Attributes.Append(attr);
            doc.Save(Server.MapPath("~/Web.config"));
        }

    }
}