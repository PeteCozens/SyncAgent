using System.Net.Mail;

namespace Infrastructure.Services.Mail
{
    internal interface IMailSendingService
    {
        public void Send(MailMessage msg);
        public Task SendAsync(MailMessage msg, CancellationToken cancellationToken);
    }
}
