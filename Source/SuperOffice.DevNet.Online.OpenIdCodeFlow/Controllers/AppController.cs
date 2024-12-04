using MvcTest.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MvcTest.Controllers
{

    public class ODataResponse
    {
        public List<ContactModel> value { get; set; }
    }


    public class AppController : Controller
    {
        RestClient _client = new RestClient();

        // GET: App - must be logged in to access this
        public ActionResult Index()
        {
            string error = null;
            // Are we logged in?
            if (Session["LoggedIn"] == null)
                return RedirectToAction("Index", "Home");

            // Do we need to refresh access token?
            DateTime expiryDate = (DateTime)Session["Expires"];
            if (expiryDate < DateTime.Now)
                error = RefreshAcessToken();

            string accessToken = Session["LoggedIn"] as string;
            string baseUrl = Session["WebAPI_url"] as string; // https://xxx.yyy/api

            var model = GetDataFromSuperOffice(baseUrl, accessToken);
            model.Error = error;
            model.TimeLeft = expiryDate - DateTime.Now;

            return View(model);
        }

        private string RefreshAcessToken()
        {
            try
            {
                string refresh_token = Session["RefreshToken"] as string;
                OAuthHelper.GetRefreshToken(Server, Session, refresh_token);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message; 
            }
        }

        private AppModel GetDataFromSuperOffice(string baseUrl, string accessToken)
        {
            var model = new AppModel();
            model.BaseUrl = baseUrl;
            model.AccessToken = accessToken;

            _client.BaseUrl = new Uri(baseUrl);
            _client.AddDefaultHeader("Authorization", string.Format("Bearer {0}", accessToken));

            // We don't want XML but JSON response, so force JSON serializer
            _client.ClearHandlers();
            _client.AddHandler("application/json", new RestSharp.Serialization.Json.JsonSerializer());

            var request = new RestRequest("v1/Contact", Method.GET);
            // request.AddParameter("$filter", "registeredDate thisYear");
            request.AddParameter("$select", "contactId,nameDepartment,category,business,number,registeredDate");

            var response = _client.Execute<ODataResponse>(request);
            if (response.IsSuccessful)
            {
                model.Contacts = response.Data.value.ToArray();
            }
            else
            {
                model.Error = response.ErrorMessage;
            }

            return model;
        }

    }
}