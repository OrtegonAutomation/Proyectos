using System.Net;
using System.Net.Mail;
using FifoCleanup.Engine.Models;

namespace FifoCleanup.Engine.Services;

public static class EmailAlertHelper
{
    public static async Task<bool> TrySendCriticalAlertAsync(FifoConfiguration config, string subject, string body)
    {
        if (!config.EnableEmailAlerts)
            return false;

        if (string.IsNullOrWhiteSpace(config.AlertEmailTo) ||
            string.IsNullOrWhiteSpace(config.SmtpHost) ||
            string.IsNullOrWhiteSpace(config.SmtpFrom))
            return false;

        try
        {
            using var message = new MailMessage(config.SmtpFrom, config.AlertEmailTo)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            using var client = new SmtpClient(config.SmtpHost, config.SmtpPort)
            {
                EnableSsl = config.SmtpUseSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = string.IsNullOrWhiteSpace(config.SmtpUser)
                    ? CredentialCache.DefaultNetworkCredentials
                    : new NetworkCredential(config.SmtpUser, config.SmtpPassword)
            };

            await client.SendMailAsync(message);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
