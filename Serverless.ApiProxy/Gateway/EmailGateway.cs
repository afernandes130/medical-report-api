using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using MimeKit;
using Serverless.ApiProxy.Models;

namespace Serverless.ApiProxy.Gateway
{
    public class EmailGateway : IEmailGateway
    {
        private readonly IAmazonSimpleEmailService emailService;
        private readonly string fromName = "Medical Report";
        private readonly string from = "noreply@medicalreport.ga";

        private readonly string ToName = "Adriano Fernandes";
        private readonly string to = "afernandes130@hotmail.com";
        private readonly string subject = "Medical Report - Dados de acesso";

        private readonly string htmlBody = @"<html>
            <head></head>
            <body>
              <p>Você está recebendo os dados de acesso para acompanhamento do paciente <i>{0}</i></p>
              <p> Url: {1} </p>
              <p> Protocolo: {2} </p>
              <p> Senha: {3} </p>
            </body>
            </html>";

        public EmailGateway(IAmazonSimpleEmailService emailService)
        {
            this.emailService = emailService;
        }
        public Task SendEmailAsync(EmailModel model)
        {
            var body = string.Format(htmlBody, model.NamePatient,model.Url , model.LoginData.Protocol, model.LoginData.Password);
            var sendRequest = new SendEmailRequest
            {
                Source = from,
                Destination = new Destination
                {
                    ToAddresses =
                        new List<string> { model.To }
                },

                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = body
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
