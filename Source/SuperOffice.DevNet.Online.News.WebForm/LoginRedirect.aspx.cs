using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using SuperOffice.DevNet.Online.Login;
using SuperOffice.Security.Principal;
using SuperOffice.DevNet.Online.Login.Extensions;

namespace SuperOffice.DevNet.Online.News.WebForm
{
    public partial class LoginRedirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string error = string.Empty;
            SetupVisibleParts();


            var requestType = Context.Request.RequestType;
            if (requestType.ToUpper() == "POST")
            {
                var callbackModel = new CallbackModel
                {
                    Saml = Context.Request["saml"],
                    Jwt = Context.Request["jwt"],
                };


                if (SuperOfficeAuthHelper.TryLogin(callbackModel, out error))
                {
                    var redirectUr = Context.Session["RedirectUrl"] as string;
                    Context.Session["RedirectUrl"] = "";

                    if (!String.IsNullOrEmpty(redirectUr))
                        Context.Response.Redirect(redirectUr);
                }
                else
                {
                    explanationText.Visible = true;
                    explanationText.InnerText = "Login unsuccessful, reason: " + error;
                }
            }
            else
            {
                var code = Context.Request["code"];
                var state = Context.Request["state"];

                var sessionState = Context.Session["state"] as string;

                if (sessionState.Equals(state))
                {
                    OidcModel oauthModel = SuperOfficeAuthHelper.GetOAuthModel(code);

                    if (SuperOfficeAuthHelper.TryLogin(oauthModel, out error))
                    {
                        var redirectUr = Context.Session["RedirectUrl"] as string;
                        Context.Session["RedirectUrl"] = "";

                        if (!String.IsNullOrEmpty(redirectUr))
                            Context.Response.Redirect(redirectUr);
                    }
                }
                else
                {
                    explanationText.Visible = true;
                    explanationText.InnerText = "Login unsuccessful, reason: " + error;
                }
            }
        }

        private void SetupVisibleParts()
        {
            var url = Context.Session["RedirectUrl"] as string;
            if (url.IsNullOrEmpty())
            {
                redirectLink.Visible = false;
                explanationText.Visible = false;
                redirectUrl.InnerText = "No redirect url is registered.";
            }
            else
            {
                redirectLink.HRef = url;
                redirectUrl.InnerText = url;
            }
        }
    }
}