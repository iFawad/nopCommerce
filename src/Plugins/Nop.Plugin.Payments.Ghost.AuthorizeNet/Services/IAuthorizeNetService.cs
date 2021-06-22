using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthorizeNet.Api.Contracts.V1;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Ghost.PaymentAuthorizeNet.Services
{
    public interface IPaymentAuthorizeNetService
    {
        Task<ANetApiResponse> AuthorizeAndCaptureAsync(ProcessPaymentRequest paymentInfo);
    }
}
