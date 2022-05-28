﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Infrastructure.Extensions;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Framework.SignalR.Hubs;

namespace Nop.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring common features and middleware on application startup
    /// </summary>
    public class NopCommonStartup : INopStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //compression
            services.AddResponseCompression();

            //add options feature
            services.AddOptions();
            
            //add distributed memory cache
            services.AddDistributedMemoryCache();

            //add HTTP sesion state feature
            services.AddHttpSession();

            //add default HTTP clients
            services.AddNopHttpClients();

            //add anti-forgery
            services.AddAntiForgery();

            //add localization
            services.AddLocalization();

            //add theme support
            services.AddThemes();

            //add routing
            services.AddRouting(options =>
            {
                //add constraint key for language
                options.ConstraintMap["lang"] = typeof(LanguageParameterTransformer);
            });
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //use response compression
            application.UseNopResponseCompression();

            //use static files feature
            application.UseNopStaticFiles();

            //check whether requested page is keep alive page
            application.UseKeepAlive();

            //check whether database is installed
            application.UseInstallUrl();

            //use HTTP session
            application.UseSession();

            //use request localization
            application.UseNopRequestLocalization();

            //set request culture
            application.UseCulture();

            
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 100; //common services should be loaded after error handlers
    }
}