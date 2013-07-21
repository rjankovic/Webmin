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
    /// Creates the initial user (after asking for neccessary info) 
    /// and gives them full administrative rights (there can be only one oser of this kind in the application).
    /// </summary>
    public partial class FirstRunMono : System.Web.UI.Page
    {

        MinMaster mm;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            mm = (MinMaster)Master;

            Errors.Items.Clear();

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

            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;
            string mail = MailTextBox.Text;
            
            MembershipCreateStatus status;
            Membership.CreateUser(username, password, mail, "Dummy question", "Dummy answer", true, 1, out status);
                      
            int totalUsers;
            MembershipUser user = Membership.FindUsersByName(username, 0, 1, out totalUsers)[username];
            mm.SysDriver.SetUserRights((user.ProviderUserKey), null, 11110);


            var config2 = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            config2.AppSettings.Settings["FirstRunMono"].Value = "False";
            System.Web.Configuration.WebConfigurationManager.AppSettings["FirstRunMono"] = "False";
            config2.Save();

            Errors.Items.Add("Done.");
            Response.RedirectToRoute("DefaultRoute");
        }
    }
}