using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.Ghost.PaymentAuthorizeNet.Services
{
    public class PaymentAuthorizeNetService : IPaymentAuthorizeNetService
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region ctor

        public PaymentAuthorizeNetService(ISettingService settingService,
            IStoreContext storeContext)
        {
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion


        #region Methods
        public async Task<ANetApiResponse> AuthorizeAndCaptureAsync(ProcessPaymentRequest paymentInfo, decimal amount)
        {
            //Console.WriteLine("Charge Credit Card Sample");

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var paymentAuthorizeNetPaymentSettings = await _settingService.LoadSettingAsync<PaymentAuthorizeNetPaymentSettings>(storeScope);
            
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = GetEnvironment(paymentAuthorizeNetPaymentSettings.Environment);

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = paymentAuthorizeNetPaymentSettings.ApiLoginId,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = paymentAuthorizeNetPaymentSettings.TransactionKey,
            };

            //Get Payment information
            var creditCard = new creditCardType
            {
                cardNumber = paymentInfo.CreditCardNumber,
                expirationDate = paymentInfo.CreditCardExpireMonth.ToString() + paymentInfo.CreditCardExpireYear.ToString(),
                cardCode = paymentInfo.CreditCardCvv2
            };

            var billingAddress = new customerAddressType
            {
                firstName = "John",
                lastName = "Doe",
                address = "123 My St",
                city = "OurTown",
                zip = "98004"
            };

            //standard api call to retrieve response
            var paymentType = new paymentType { Item = creditCard };

            // Add line Items
            var lineItems = new lineItemType[2];
            lineItems[0] = new lineItemType { itemId = "1", name = "t-shirt", quantity = 2, unitPrice = new Decimal(15.00) };
            lineItems[1] = new lineItemType { itemId = "2", name = "snowboard", quantity = 1, unitPrice = new Decimal(450.00) };

            var transactionRequest = new transactionRequestType
            {
                transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card

                amount = amount,
                payment = paymentType,
                billTo = billingAddress,
                lineItems = lineItems
            };

            var request = new createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            // validate response
            if (response != null)
            {
                if (response.messages.resultCode == messageTypeEnum.Ok)
                {
                    if (response.transactionResponse.messages != null)
                    {
                        Console.WriteLine("Successfully created transaction with Transaction ID: " + response.transactionResponse.transId);
                        Console.WriteLine("Response Code: " + response.transactionResponse.responseCode);
                        Console.WriteLine("Message Code: " + response.transactionResponse.messages[0].code);
                        Console.WriteLine("Description: " + response.transactionResponse.messages[0].description);
                        Console.WriteLine("Success, Auth Code : " + response.transactionResponse.authCode);
                    }
                    else
                    {
                        Console.WriteLine("Failed Transaction.");
                        if (response.transactionResponse.errors != null)
                        {
                            Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                            Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed Transaction.");
                    if (response.transactionResponse != null && response.transactionResponse.errors != null)
                    {
                        Console.WriteLine("Error Code: " + response.transactionResponse.errors[0].errorCode);
                        Console.WriteLine("Error message: " + response.transactionResponse.errors[0].errorText);
                    }
                    else
                    {
                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);
                    }
                }
            }
            else
            {
                Console.WriteLine("Null Response.");
            }

            return response;
        }

        private AuthorizeNet.Environment GetEnvironment(string environment)
        {
            AuthorizeNet.Environment result;
            switch (environment)
            {
                case "SANDBOX":
                    result = AuthorizeNet.Environment.SANDBOX;
                    break;
                case "PRODUCTION":
                    result = AuthorizeNet.Environment.PRODUCTION;
                    break;
                case "LOCAL_VM":
                    result = AuthorizeNet.Environment.LOCAL_VM;
                    break;
                case "HOSTED_VM":
                    result = AuthorizeNet.Environment.HOSTED_VM;
                    break;
                case "CUSTOM":
                    result = AuthorizeNet.Environment.CUSTOM;
                    break;
                default:
                    result = AuthorizeNet.Environment.SANDBOX;
                    break;
            }
            return result;
        }

        #endregion
    }
}
