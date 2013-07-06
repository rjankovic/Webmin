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
    /// <summary>
    /// Prompts the user for database credentials, tests database connection, creates the shema required by the Webmin system, adds an initial user with exclusive access rights
    /// and sends the user to the frontpage of a newly established instance of Webmin.
    /// </summary>
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

            // initial testing of the database connection before we attempt to create the main schema
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
                drv.TestDatabaseIsEmpty();
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

            try
            {
                System.Net.Mail.MailAddress address = new System.Net.Mail.MailAddress(MailTextBox.Text);
            }
            catch (FormatException fe)
            {
                Errors.Items.Add(fe.Message);
                return;
            }


            // run the schema dump script
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
            var section = (ConnectionStringsSection)configuration.GetSection("connectionStrings");

            System.Web.Security.MembershipProvider membership = null;

            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;
            string mail = MailTextBox.Text;

            MembershipCreateStatus status;
                    
            // rewrite the connection in the database and reload the connstring section, also set the defaultProvidder for the membership tag
            switch (serverTypeParsed)
            {
                case DbServer.MySql:
                    section.ConnectionStrings["MySqlServer"].ConnectionString = SystemConnstringTextBox.Text;
                    configuration.AppSettings.Settings["ServerType"].Value = "MySql";
                    configuration.Save();
                    SetDefaultMembershipProvider("MySqlMembershipProvider");
                    
                    // remove the readonly attribute of the connection string variable of the connfiguration
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

                    // remove the readonly attribute of the connection string variable of the connfiguration
                    var settings = ConfigurationManager.ConnectionStrings["MsSqlServer"];
                    var fi = typeof( ConfigurationElement ).GetField( "_bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic );
                    fi.SetValue(settings, false);
                    settings.ConnectionString = SystemConnstringTextBox.Text;

                    membership = Membership.Providers["MsSqlMembershipProvider"];

                    // generate a ProviderUserKey
                    Random rand = new Random();
                    Guid key = new Guid(rand.Next(), 2, 3, new byte[] { 10, 20, 30, 40, 50, 60, 70, 80 });
                    ((SqlMembershipProvider)membership).CreateUser(username, password, mail, "Dummy question", "Dummy answer", true, key, out status);
                    break;
            }

            int totalUsers;
            MembershipUser user = membership.FindUsersByName(username, 0, 1, out totalUsers)[username];
            SystemDriver sysDriver = new SystemDriver(drv);
            sysDriver.SetUserRights((user.ProviderUserKey), null, 11110);

            // Set FirstRun to false. This cannot be done by the first configuration object - it wil
            // not like the configuration file since it has been modified by SetDefaultMembershipProvider
            // in the meantime.
            var config2 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            config2.AppSettings.Settings["FirstRun"].Value = "False";
            config2.Save();
            Response.RedirectToRoute("DefaultRoute");
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