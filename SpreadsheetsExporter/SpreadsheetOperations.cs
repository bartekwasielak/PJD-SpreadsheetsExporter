using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.Client;
using Google.GData.Spreadsheets;

namespace SpreadsheetsExporter
{
    public class SpreadsheetOperations
    {
        private OAuth2Parameters _parameters = new OAuth2Parameters();
        private SpreadsheetsService _service;

        public string GetAuthorizationUrl()
        {
            ////////////////////////////////////////////////////////////////////////////
            // STEP 1: Configure how to perform OAuth 2.0
            ////////////////////////////////////////////////////////////////////////////

            // TODO: Update the following information with that obtained from
            // https://code.google.com/apis/console. After registering
            // your application, these will be provided for you.

            string CLIENT_ID = "733817008598.apps.googleusercontent.com";

            // This is the OAuth 2.0 Client Secret retrieved
            // above.  Be sure to store this value securely.  Leaking this
            // value would enable others to act on behalf of your application!
            string CLIENT_SECRET = "M3sALwQnTkcnoOixH6o3o6Y-";

            // Space separated list of scopes for which to request access.
            string SCOPE = "https://spreadsheets.google.com/feeds https://docs.google.com/feeds";

            // This is the Redirect URI for installed applications.
            // If you are building a web application, you have to set your
            // Redirect URI at https://code.google.com/apis/console.
            string REDIRECT_URI = "urn:ietf:wg:oauth:2.0:oob";

            ////////////////////////////////////////////////////////////////////////////
            // STEP 2: Set up the OAuth 2.0 object
            ////////////////////////////////////////////////////////////////////////////           

            // Set your OAuth 2.0 Client Id (which you can register at
            // https://code.google.com/apis/console).
            _parameters.ClientId = CLIENT_ID;

            // Set your OAuth 2.0 Client Secret, which can be obtained at
            // https://code.google.com/apis/console.
            _parameters.ClientSecret = CLIENT_SECRET;

            // Set your Redirect URI, which can be registered at
            // https://code.google.com/apis/console.
            _parameters.RedirectUri = REDIRECT_URI;

            ////////////////////////////////////////////////////////////////////////////
            // STEP 3: Get the Authorization URL
            ////////////////////////////////////////////////////////////////////////////

            // Set the scope for this particular service.
            _parameters.Scope = SCOPE;

            // Get the authorization url.  The user of your application must visit
            // this url in order to authorize with Google.  If you are building a
            // browser-based application, you can redirect the user to the authorization
            // url.
            string authorizationUrl = OAuthUtil.CreateOAuth2AuthorizationUrl(_parameters);

            Console.WriteLine(authorizationUrl);
            Console.WriteLine("Please visit the URL above to authorize your OAuth "
              + "request token.  Once that is complete, type in your access code to "
              + "continue...");

            return authorizationUrl;
        }

        public void SetAccessCode(string accessCode)
        {
            _parameters.AccessCode = accessCode;
        }

        public void Connect()
        {
            if (_parameters.AccessCode == null)
            {
                throw new InvalidOperationException("No access code set.");
            }
                        

            ////////////////////////////////////////////////////////////////////////////
            // STEP 4: Get the Access Token
            ////////////////////////////////////////////////////////////////////////////

            // Once the user authorizes with Google, the request token can be exchanged
            // for a long-lived access token.  If you are building a browser-based
            // application, you should parse the incoming request token from the url and
            // set it in OAuthParameters before calling GetAccessToken().
            OAuthUtil.GetAccessToken(_parameters);
            string accessToken = _parameters.AccessToken;
            Console.WriteLine("OAuth Access Token: " + accessToken);

            ////////////////////////////////////////////////////////////////////////////
            // STEP 5: Make an OAuth authorized request to Google
            ////////////////////////////////////////////////////////////////////////////

            // Initialize the variables needed to make the request
            GOAuth2RequestFactory requestFactory =
                new GOAuth2RequestFactory(null, "MySpreadsheetIntegration-v1", _parameters);
            _service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
            _service.RequestFactory = requestFactory;

            // Make the request to Google
            // See other portions of this guide for code to put here...
        }

        public IEnumerable<SpreadsheetEntry> GetSpreadsheetList()
        {
            // Instantiate a SpreadsheetQuery object to retrieve spreadsheets.
            SpreadsheetQuery query = new SpreadsheetQuery();

            // Make a request to the API and get all spreadsheets.
            SpreadsheetFeed feed = _service.Query(query);

            return feed.Entries.Cast<SpreadsheetEntry>();
        }

        public WorksheetValues GetList(WorksheetEntry worksheet)
        {
            AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);
            ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
            ListFeed listFeed = _service.Query(listQuery);
            
            var values = listFeed.Entries.Select(row => ((ListEntry)row).Elements.Cast<ListEntry.Custom>().ToDictionary(el => el.LocalName, el => el.Value));

            return new WorksheetValues() { Values = values };
        }
    }
}
