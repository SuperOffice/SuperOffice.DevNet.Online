using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SuperOffice.CRM.Services;
using SuperOffice.Configuration;
using SuperOffice.Exceptions;
using SuperOffice.Security.Principal;
using SuperOffice.SuperID.Client.Tokens;
using SuperOffice.SuperId.WinClient;
using SuperOffice.SuperId.WinClient.Contracts;

namespace SuperOffice.DevNet.Online.WinForm
{
    public partial class Form1 : Form
    {
        private SuperIdToken _userToken;
        private SoSession _session;


        public Form1()
        {
            InitializeComponent();

            _applicationToken.Text = SuperOffice.Configuration.ConfigFile.Services.ApplicationToken;
            _applicationId.Text = ConfigurationManager.AppSettings["SoAppId"];
            btDoStuff.Enabled = false;
        }

        private void btLogin_Click(object sender, EventArgs e)
        {

            btDoStuff.Enabled = false;

            _netServerUrl.Text = string.Empty;
            _claims.Items.Clear();

            SuperOffice.Configuration.ConfigFile.Services.ApplicationToken = _applicationToken.Text;

            var login = new LoginHelper();
            var uri = new UriBuilder(_environmentLogin.Text).Uri;
            var response = login.TryFederatedLogin(uri, new AuthenticationRequest()
                {
                    ApplicationId = _applicationId.Text,
                     ApplicationTitle = "Testing win-forms login in demo app",
                     CustomerContext = string.Empty, // don't cara about which customer in this context
                });

            if (response.IsSuccessful)
            {
                var saml = GetClaim(response, "saml");
                //var jwt = GetClaim(response, "jwt");

                // Validate and parse saml with user authentication
                var userTokenHandler = new SuperIdTokenHandler();
                _userToken = userTokenHandler.ValidateToken(saml, SuperOffice.SuperID.Contracts.SystemUser.V1.TokenType.Saml);


                foreach (var claim in _userToken.Claims)
                {
                    var lvi = new ListViewItem(claim.ClaimType);
                    lvi.SubItems.Add(claim.Resource as string);
                    _claims.Items.Add(lvi);

                }

                _netServerUrl.Text = _userToken.NetserverUrl;
                ConfigFile.WebServices.RemoteBaseURL = _userToken.NetserverUrl;

                try
                {

                    _session = SoSession.Authenticate(new SoCredentials() {Ticket = _userToken.Ticket});
                }
                catch (Exception)
                {
                }

            }

            btDoStuff.Enabled = _session != null;

        }

        private static string GetClaim(AuthenticationResponse response, string name)
        {
            string netserver_url;
            netserver_url = (from c in response.Claims where c.ClaimType == name select c.Resource).FirstOrDefault() as string;
            return netserver_url;
        }

        private void btDoStuff_Click(object sender, EventArgs e)
        {
            _stuff.Text = "Running...";

            var myContactId = _session.Principal.ContactId;
            var contact = new ContactAgent().GetContact(myContactId);
            var sb = new StringBuilder();
            sb.AppendLine("Start: " + DateTime.Now.ToShortTimeString());
            sb.AppendFormat("Company: '{0}', User: '{1}'{2}", contact.FullName, SoContext.CurrentPrincipal.FullName, Environment.NewLine);

            // Try to access restricted agents
            
            try
            {
                var userInfo = new UserAgent().GetUserInfo(_session.Principal.AssociateId);
                sb.AppendLine("Successfully accessed UserAgent ant retrieved information for associate: " + userInfo.UserName);
            }
            catch (SoServerException ex)
            {
                sb.AppendLine("Failed to access User Agent: " + ex.ExceptionInfo.Message);
            }
            catch (Exception ex)
            {
                sb.AppendLine("Failed to access User Agent: " + ex.Message);
            }

            sb.AppendLine("End: " + DateTime.Now.ToShortTimeString());
            _stuff.Text = sb.ToString();

        }


    }
}
