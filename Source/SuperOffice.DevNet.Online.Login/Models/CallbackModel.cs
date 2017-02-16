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
}