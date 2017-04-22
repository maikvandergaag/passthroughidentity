using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace Msft.Gab.Api.Controllers {
    [Authorize]
    [Route("api/[controller]")]
    public class ProductController : Controller {

        private IHttpContextAccessor httpContextAccessor;

        private AuthenticationSettings authSettings { get; set; }

        public ProductController(IHttpContextAccessor httpContextAcc, IOptions<AuthenticationSettings> settings) {
            httpContextAccessor = httpContextAcc;
            authSettings = settings.Value;
        }

        // GET: api/product
        [HttpGet]
        public JsonResult Get() {
            JsonResult retVal = null;

            AuthenticationResult authResult = AuthenticationHelper.GetAuthenticationResult(httpContextAccessor, authSettings);

            if (authResult != null) {
                string queryString = "SELECT * FROM SalesLT.Product";

                using (SqlConnection connection = new SqlConnection(authSettings.ConnectionString)) {
                    connection.AccessToken = authResult.AccessToken;
                    try {
                        connection.Open();
                        SqlCommand command = new SqlCommand(queryString, connection);
                        SqlDataAdapter adapter = new SqlDataAdapter(command);

                        DataTable table = new DataTable();  
                        adapter.Fill(table);

                        retVal = new JsonResult(table);

                    } catch (SqlException ex) {
                    }
                }
            }
            return retVal;
        }
    }
}
