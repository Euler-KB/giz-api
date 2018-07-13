using GIZ.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace GIZ.API
{
    public class SmtpEmailService : IEmailService
    {
        private string templatesRootDirectory;
        private System.Net.Mail.SmtpClient client;

        public SmtpEmailService(string source, string password, string hostAddress, int port, string defaultSubject = null, string templatesDir = null)
        {
            this.templatesRootDirectory = templatesDir;
            client = new System.Net.Mail.SmtpClient(hostAddress,port)
            {
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential(source,password)
            };
        }

        public Task SendAsync(SendEmailOptions options)
        {
            throw new NotImplementedException();
        }

        public Task SendFileTemplate(string filename, SendTemplateOptions options)
        {
            throw new NotImplementedException();
        }

        public Task SendTemplate(string template, SendTemplateOptions options)
        {
            throw new NotImplementedException();
        }
    }
}