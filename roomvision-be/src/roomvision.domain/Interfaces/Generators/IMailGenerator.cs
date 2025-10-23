using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace roomvision.domain.Interfaces.Generators
{
    /// <summary>
    /// Interface for generating email content.
    /// </summary>
    /// <remarks>
    /// This interface defines methods for generating various types of email content.
    /// </remarks>
    public interface IMailGenerator
    {
        /// <summary>
        /// Generates a welcome email asynchronously.
        /// </summary>
        /// <remarks>
        /// This method creates the content for a welcome email to be sent to new users.
        /// Using the provided email address and password, it constructs the email body.
        /// And uses SMTP settings from configuration for sending the email.
        /// </remarks>
        /// <param name="toAddress">The recipient's email address.</param>
        /// <param name="password">The password to include in the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task WelcomeEmailAsync(string toAddress, string password);
    }
}