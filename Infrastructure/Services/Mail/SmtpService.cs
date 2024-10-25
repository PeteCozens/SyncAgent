using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Text;

namespace Infrastructure.Services.Mail
{
    /// <summary>
    /// Represents the configuration of the SMTP Server connection, as defined in the appsettings*.json configuration file(s)
    /// </summary>
    public class SmtpServiceConfig 
    {
        /// <summary>
        /// Name or IP address of the SMTP Server
        /// </summary>
        public string Host { get; set; } = "localhost";

        /// <summary>
        /// Port number on which to connect to the SMTP Server
        /// </summary>
        public int Port { get; set; } = 25;

        /// <summary>
        /// Email address to send any emails from
        /// </summary>
        public string From { get; set; } = "do_not_reply@example.com";

        /// <summary>
        /// Array of email addresses to BCC every email to, in addition to any addresses that are specified on the message
        /// </summary>
        public string[] Bcc { get; set; } = [];

        /// <summary>
        /// Array of email addrsses to send emails to INSTEAD OF those that are specified on the message. This is intended solely for testing purposes
        /// </summary>
        public string[] OverrideRecipients { get; set; } = [];
    }

    /// <summary>
    /// This service sends emails via an SMTP Server. Be sure to include a section in your IConfiguration that
    /// provides the necessary properties to populate an SmtpConfig class.
    /// </summary>
    /// <param name="config">SmtpConfig from your IConfiguration</param>
    /// <exception cref="Exception"></exception>
    [ExcludeFromCodeCoverage]
    public class SmtpService(SmtpServiceConfig config) : IMailSendingService
    {
        /// <summary>
        /// Send an email synchronously
        /// </summary>
        /// <param name="msg"></param>
        public void Send(MailMessage msg)
        {
            SendAsync(msg).Wait();
        }

        /// <summary>
        /// Send an email Asynchronously
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task SendAsync(MailMessage msg, CancellationToken cancellationToken = default)
        {
            using var client = new SmtpClient(config.Host, config.Port);
            msg.From = new MailAddress(config.From);
            foreach (var address in config.Bcc)
                msg.Bcc.Add(address);

            if (config.OverrideRecipients != null && config.OverrideRecipients.Length > 0)
            {
                // Build up string to insert at the start of the email

                var sb = new StringBuilder();
                if (msg.IsBodyHtml)
                    sb.Append("<div><p>");
                sb.Append("An Email override is in place on this system. Ordinarily, this email would have been sent to the following:</p>");
                if (msg.IsBodyHtml)
                    sb.Append("</p><ul>");

                foreach (var email in msg.To)
                    sb.AppendLine(FormatAddress(msg.IsBodyHtml, "TO", email.DisplayName, email.Address));
                foreach (var email in msg.CC)
                    sb.AppendLine(FormatAddress(msg.IsBodyHtml, "CC", email.DisplayName, email.Address));
                foreach (var email in msg.Bcc)
                    sb.AppendLine(FormatAddress(msg.IsBodyHtml, "Bcc", email.DisplayName, email.Address));

                if (msg.IsBodyHtml)
                    sb.Append("</ul></div><hr style='margin:1em 0;' />");

                // Insert the generated string at the start of the email

                if (msg.IsBodyHtml)
                {
                    var i = msg.Body.IndexOf("<body", StringComparison.InvariantCultureIgnoreCase);
                    if (i < 0)
                    {
                        msg.Body = sb.ToString() + msg.Body;
                    }
                    else
                    {
                        i = msg.Body.IndexOf('>', i + 1);
                        msg.Body = msg.Body[..i] + sb.ToString() + msg.Body[(i + 1)..];
                    }
                }
                else
                {
                    msg.Body = sb.ToString() + "\n\n" + new string('-', 80) + "\n\n" + msg.Body;
                }

                // Remove ALL recipients

                msg.To.Clear();
                msg.CC.Clear();
                msg.Bcc.Clear();

                // Add the Override recipients

                foreach (var recipient in config.OverrideRecipients)
                    msg.To.Add(recipient);
            }

            await client.SendMailAsync(msg, cancellationToken);
        }

        private static string FormatAddress(bool isHtml, string type, string displayName, string address)
        {
            var v = string.IsNullOrEmpty(displayName) || displayName.Equals(address, StringComparison.InvariantCultureIgnoreCase)
                ? $"{type}: {address}"
                : $"{type}: {displayName} ({address})";

            return isHtml ? $"<li>{System.Net.WebUtility.HtmlEncode(v)}</li>" : v;
        }
    }
}
