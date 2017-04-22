using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Msft.Gab.Api
{
    public class AuthenticationSettings
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AadInstance { get; set; }

        public string Domain  { get; set; }

        public string Audience { get; set; }

        public string TenantId { get; set; }

        public string AzureSqlResource { get; set; }

        public string ConnectionString { get; set; }
    }
}
