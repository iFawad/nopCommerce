using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Payments.Ghost.PaymentAuthorizeNet.Services;
using Nop.Services.Messages;

namespace Nop.Plugin.Payments.Ghost.PaymentAuthorizeNet.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        /// <summary>
        /// Gets order of this dependency registrar implementation
        /// </summary>
        public int Order => 1003;

        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="appSettings">App settings</param>
        public virtual void Register(IServiceCollection services, ITypeFinder typeFinder, AppSettings appSettings)
        {
            //register custom services
            //services.AddScoped<PaymentAuthorizeNetManager>();

            //override service
            //services.AddScoped<IWorkflowMessageService, PaymentAuthorizeNetService>();

            services.AddScoped<IPaymentAuthorizeNetService, PaymentAuthorizeNetService>();
        }
    }
}
