using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Sms.Ghost.Twilio.Domain.Messages;

namespace Nop.Plugin.Sms.Ghost.Twilio.Services
{
    public class SmsAccountService :  ISmsAccountService
    {
        #region Fields

        private readonly IRepository<SmsAccount> _smsAccountRepository;

        #endregion

        #region Ctor

        public SmsAccountService(IRepository<SmsAccount> smsAccountRepository)
        {
            _smsAccountRepository = smsAccountRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts an sms account
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertSmsAccountAsync(SmsAccount smsAccount)
        {
            if (smsAccount == null)
                throw new ArgumentNullException(nameof(smsAccount));

            smsAccount.AccountSid = CommonHelper.EnsureNotNull(smsAccount.AccountSid);
            smsAccount.DisplayName = CommonHelper.EnsureNotNull(smsAccount.DisplayName);
            smsAccount.AuthToken = CommonHelper.EnsureNotNull(smsAccount.AuthToken);
            smsAccount.TwilioPhoneNumber = CommonHelper.EnsureNotNull(smsAccount.TwilioPhoneNumber);

            smsAccount.AccountSid = smsAccount.AccountSid.Trim();
            smsAccount.DisplayName = smsAccount.DisplayName.Trim();
            smsAccount.AuthToken = smsAccount.AuthToken.Trim();
            smsAccount.TwilioPhoneNumber = smsAccount.TwilioPhoneNumber.Trim();

            smsAccount.AccountSid = CommonHelper.EnsureMaximumLength(smsAccount.AccountSid, 255);
            smsAccount.DisplayName = CommonHelper.EnsureMaximumLength(smsAccount.DisplayName, 255);
            smsAccount.AuthToken = CommonHelper.EnsureMaximumLength(smsAccount.AuthToken, 255);
            smsAccount.TwilioPhoneNumber = CommonHelper.EnsureMaximumLength(smsAccount.TwilioPhoneNumber, 255);

            await _smsAccountRepository.InsertAsync(smsAccount);
        }

        /// <summary>
        /// Updates an sms account
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateSmsAccountAsync(SmsAccount smsAccount)
        {
            if (smsAccount == null)
                throw new ArgumentNullException(nameof(smsAccount));

            smsAccount.AccountSid = CommonHelper.EnsureNotNull(smsAccount.AccountSid);
            smsAccount.DisplayName = CommonHelper.EnsureNotNull(smsAccount.DisplayName);
            smsAccount.AuthToken = CommonHelper.EnsureNotNull(smsAccount.AuthToken);
            smsAccount.TwilioPhoneNumber = CommonHelper.EnsureNotNull(smsAccount.TwilioPhoneNumber);

            smsAccount.AccountSid = smsAccount.AccountSid.Trim();
            smsAccount.DisplayName = smsAccount.DisplayName.Trim();
            smsAccount.AuthToken = smsAccount.AuthToken.Trim();
            smsAccount.TwilioPhoneNumber = smsAccount.TwilioPhoneNumber.Trim();

            smsAccount.AccountSid = CommonHelper.EnsureMaximumLength(smsAccount.AccountSid, 255);
            smsAccount.DisplayName = CommonHelper.EnsureMaximumLength(smsAccount.DisplayName, 255);
            smsAccount.AuthToken = CommonHelper.EnsureMaximumLength(smsAccount.AuthToken, 255);
            smsAccount.TwilioPhoneNumber = CommonHelper.EnsureMaximumLength(smsAccount.TwilioPhoneNumber, 255);

            await _smsAccountRepository.UpdateAsync(smsAccount);
        }

        /// <summary>
        /// Deletes an sms account
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteSmsAccountAsync(SmsAccount smsAccount)
        {
            if (smsAccount == null)
                throw new ArgumentNullException(nameof(smsAccount));

            if ((await GetAllSmsAccountsAsync()).Count == 1)
                throw new NopException("You cannot delete this sms account. At least one account is required.");

            await _smsAccountRepository.DeleteAsync(smsAccount);
        }

        /// <summary>
        /// Gets an sms account by identifier
        /// </summary>
        /// <param name="smsAccountId">The sms account identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sms account
        /// </returns>
        public virtual async Task<SmsAccount> GetSmsAccountByIdAsync(int smsAccountId)
        {
            return await _smsAccountRepository.GetByIdAsync(smsAccountId, cache => default);
        }

        /// <summary>
        /// Gets all sms accounts
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sms accounts list
        /// </returns>
        public virtual async Task<IList<SmsAccount>> GetAllSmsAccountsAsync()
        {
            var smsAccounts = await _smsAccountRepository.GetAllAsync(query =>
            {
                return from ea in query
                       orderby ea.Id
                       select ea;
            }, cache => default);

            return smsAccounts;
        }

        #endregion
    }
}
