namespace SuperOffice.DevNet.Online.Login
{
	/// <summary>
	/// Data as posted back from the login page
	/// </summary>
	public class CallbackModel
	{
		/// <summary>
		///  SAML Token
		/// </summary>
		public string Saml { get; set; }

		/// <summary>
		/// JSON Web Token
		/// </summary>
		public string Jwt { get; set; }
    }

    public class OidcModel
    {
        public string IdToken { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}