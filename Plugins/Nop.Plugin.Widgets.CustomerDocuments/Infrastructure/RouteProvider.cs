using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Widgets.CustomerDocuments.Infrastructure
{
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("UploadFileCustomerDocument", "Plugins/CustomerDocuments/UploadFileCustomerDocument",
                             new { controller = "CustomerDocuments", action = "UploadFileCustomerDocument" });
            endpointRouteBuilder.MapControllerRoute("UploadFileCustomerDocumentCompany", "Plugins/CustomerDocuments/UploadFileCustomerDocumentCompany",
                            new { controller = "CustomerDocuments", action = "UploadFileCustomerDocumentCompany" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
