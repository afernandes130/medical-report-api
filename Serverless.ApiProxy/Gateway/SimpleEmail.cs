using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using MimeKit;

namespace Serverless.ApiProxy.Gateway
{
    public class SimpleEmail : ISimpleEmail
    {
        private readonly IAmazonSimpleEmailService emailService;
        private readonly string fromName = "Caio Ragazzi";
        private readonly string from = "caio@caioragazzi.com";

        private readonly string ToName = "Adriano Fernandes";
        private readonly string to = "afernandes130@hotmail.com";
        private readonly string subject = "Teste Amazon SES";

        private readonly string htmlBody = @"<html>
            <head></head>
            <body>
              <h1>Teste Amazon SES via API </h1>
              <p>Esse email foi enviado com 
                <a href='https://aws.amazon.com/ses/'>Amazon SES</a> usando como referencia
                <a href='https://aws.amazon.com/sdk-for-net/'>
                  AWS SDK para .NET</a>.</p>
            </body>
            </html>";

        public SimpleEmail(IAmazonSimpleEmailService emailService)
        {
            this.emailService = emailService;
        }
        public Task SendEmailAsync()
        {

            var sendRequest = new SendEmailRequest
            {
                Source = from,
                Destination = new Destination
                {
                    ToAddresses =
                        new List<string> { to }
                },

                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = htmlBody
                        }
                    }
                }
            };

            var response = emailService.SendEmailAsync(sendRequest);
            return Task.FromResult(response);
        }

        public Task SendEmailAttachAsync()
        {

            using (var messageStream = new MemoryStream())
            {
                var message = new MimeMessage();
                var builder = new BodyBuilder() { HtmlBody = htmlBody };

                message.From.Add(new MailboxAddress(fromName, from));
                message.To.Add(new MailboxAddress(ToName, to));
                message.Subject = subject;

                //I'm using the stream method, but you don't have to.
                using (FileStream stream = File.Open(@"ExAttachment/LambdaSES_Atach.pdf", FileMode.Open)) builder.Attachments.Add("LambdaSES_Atach.pdf", stream);

                message.Body = builder.ToMessageBody();
                message.WriteTo(messageStream);

                var request = new SendRawEmailRequest()
                {
                    RawMessage = new RawMessage() { Data = messageStream }
                };

                var response = emailService.SendRawEmailAsync(request);
                return Task.FromResult(response);
            }
        }
    }
}
