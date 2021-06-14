using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Core.Events;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;

namespace Nop.Plugin.Sms.Ghost.Twilio.Services
{
    public class SmsService : WorkflowMessageService
    {
        #region Fields

        private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ISettingService _settingService;
        private readonly ICustomerService _customerService;
        private readonly ITokenizer _tokenizer;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly ILocalizationService _localizationService;
        private readonly TwilioSmsManager _twilioSmsManager;

        #endregion

        #region Ctor

        public SmsService(CommonSettings commonSettings,
            EmailAccountSettings emailAccountSettings,
            IAddressService addressService,
            IAffiliateService affiliateService,
            ICustomerService customerService,
            IEmailAccountService emailAccountService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMessageTemplateService messageTemplateService,
            IMessageTokenProvider messageTokenProvider,
            IOrderService orderService,
            IProductService productService,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IQueuedEmailService queuedEmailService,
            ITokenizer tokenizer,
            TwilioSmsManager twilioSmsManager)
            : base(commonSettings,
                emailAccountSettings,
                addressService,
                affiliateService,
                customerService,
                emailAccountService,
                eventPublisher,
                languageService,
                localizationService,
                messageTemplateService,
                messageTokenProvider,
                orderService,
                productService,
                queuedEmailService,
                storeContext,
                storeService,
                tokenizer)
        {
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _queuedEmailService = queuedEmailService;
            _settingService = settingService;
            _customerService = customerService;
            _tokenizer = tokenizer;
            _messageTokenProvider = messageTokenProvider;
            _localizationService = localizationService;
            _twilioSmsManager = twilioSmsManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="emailAccount">Email account</param>
        /// <param name="languageId">Language identifier</param>
        /// <param name="tokens">Tokens</param>
        /// <param name="toEmailAddress">Recipient email address</param>
        /// <param name="toName">Recipient name</param>
        /// <param name="attachmentFilePath">Attachment file path</param>
        /// <param name="attachmentFileName">Attachment file name</param>
        /// <param name="replyToEmailAddress">"Reply to" email</param>
        /// <param name="replyToName">"Reply to" name</param>
        /// <param name="fromEmail">Sender email. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="fromName">Sender name. If specified, then it overrides passed "emailAccount" details</param>
        /// <param name="subject">Subject. If specified, then it overrides subject of a message template</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the queued email identifier
        /// </returns>
        public override async Task<int> SendNotificationAsync(MessageTemplate messageTemplate, EmailAccount emailAccount, int languageId, IList<Token> tokens,
            string toEmailAddress, string toName, string attachmentFilePath = null, string attachmentFileName = null,
            string replyToEmailAddress = null, string replyToName = null, string fromEmail = null, string fromName = null,
            string subject = null)
        {
            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            var body = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Body, languageId);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //extract Customer email from token Order.CustomerEmail
            var customerToken = tokens.Where(token =>
            token.Key == "Order.CustomerEmail").FirstOrDefault();

            if(customerToken == null)
                throw new ArgumentNullException(nameof(customerToken));

            string customerEmail = customerToken.Value.ToString();

            //get Customer
            var customer = await _customerService.GetCustomerByEmailAsync(customerEmail);

            //get customer Phone number

            //tokens
            //var commonTokens = new List<Token>();
            //await _messageTokenProvider.AddCustomerTokensAsync(commonTokens, customer);

            //try to send SMS notification
            await SendSmsNotificationAsync("+9234424973033", bodyReplaced , tokens);


            //send base notification
            return await base.SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens,
                toEmailAddress, toName, attachmentFilePath, attachmentFileName,
                replyToEmailAddress, replyToName, fromEmail, fromName, subject);
        }

        /// <summary>
        /// Send SMS notification by message template
        /// </summary>
        /// <param name="messageTemplate">Message template</param>
        /// <param name="tokens">Tokens</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task SendSmsNotificationAsync(string toNumber, string messageText, IEnumerable<Token> tokens)
        {
            var text = messageText;

            //try to send SMS
            await _twilioSmsManager.SendSMSAsync(toNumber, text);
        }

        #endregion


    }
}
