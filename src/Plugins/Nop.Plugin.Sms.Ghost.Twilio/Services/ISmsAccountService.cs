using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Sms.Ghost.Twilio.Domain.Messages;

namespace Nop.Plugin.Sms.Ghost.Twilio.Services
{
    /// <summary>
    /// Sms account service
    /// </summary>
    public interface ISmsAccountService
    {
        /// <summary>
        /// Inserts an sms account
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertSmsAccountAsync(SmsAccount smsAccount);

        /// <summary>
        /// Updates an sms account
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateSmsAccountAsync(SmsAccount smsAccount);

        /// <summary>
        /// Deletes an sms account
        /// </summary>
        /// <param name="smsAccount">Sms account</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteSmsAccountAsync(SmsAccount smsAccount);

        /// <summary>
        /// Gets an sms account by identifier
        /// </summary>
        /// <param name="smsAccountId">The sms account identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sms account
        /// </returns>
        Task<SmsAccount> GetSmsAccountByIdAsync(int smsAccountId);

        /// <summary>
        /// Gets all sms accounts
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the sms accounts list
        /// </returns>
        Task<IList<SmsAccount>> GetAllSmsAccountsAsync();
    }
}
