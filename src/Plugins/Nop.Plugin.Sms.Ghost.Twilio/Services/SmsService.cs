using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
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
        private readonly IEventPublisher _eventPublisher;
        private readonly IAddressService _addressService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IQueuedEmailService _queuedEmailService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
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
            _eventPublisher = eventPublisher;
            _addressService = addressService;
            _genericAttributeService = genericAttributeService;
            _queuedEmailService = queuedEmailService;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
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
            var smsTwilioSettings = await _settingService.LoadSettingAsync<SmsTwilioSettings>();

            //Check if service is enabled.
            if (!smsTwilioSettings.Enabled)
            {
                //send base notification
                return await base.SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens,
                    toEmailAddress, toName, attachmentFilePath, attachmentFileName,
                    replyToEmailAddress, replyToName, fromEmail, fromName, subject);
            }

            if (messageTemplate == null)
                throw new ArgumentNullException(nameof(messageTemplate));

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var body = await _localizationService.GetLocalizedAsync(messageTemplate, mt => mt.Body, languageId);
            var bodyReplaced = _tokenizer.Replace(body, tokens, true);

            //Remove Html tags for Sms
            Regex regex = new Regex("\\<[^\\>]*\\>");
            bodyReplaced = regex.Replace(bodyReplaced, string.Empty);

            //extract Order.CustomerEmail token
            var customerEmailToken = tokens.Where(token =>
            token.Key == "Order.CustomerEmail").FirstOrDefault();

            if(customerEmailToken == null)
                throw new ArgumentNullException(nameof(customerEmailToken));

            //Customer email from CustomerEmail token
            var customerEmail = customerEmailToken.Value.ToString();

            //get Customer
            var customer = await _customerService.GetCustomerByEmailAsync(customerEmail);

            //get customer generic attributes for Phone number
            var keyGroup = customer.GetType().Name;
            var props = (await _genericAttributeService.GetAttributesForEntityAsync(customer.Id, keyGroup))
                .Where(x => x.StoreId == storeScope)
                .ToList();
            //Get phone number
            var phoneNumber = props.FirstOrDefault(prop =>
            prop.Key.ToLower() == "Phone".ToLower()).Value;

            //get customer Address for Phone number
            //var address = await _addressService.GetAddressByIdAsync((int)customer.ShippingAddressId);

            //try to send SMS
            await _twilioSmsManager.SendSMSAsync(phoneNumber, bodyReplaced);

            //send base notification
            return await base.SendNotificationAsync(messageTemplate, emailAccount, languageId, tokens,
                toEmailAddress, toName, attachmentFilePath, attachmentFileName,
                replyToEmailAddress, replyToName, fromEmail, fromName, subject);
        }

        #endregion


    }
}
