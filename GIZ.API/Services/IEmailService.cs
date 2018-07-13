using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GIZ.API.Services
{
    public class SendEmailOptions
    {
        public string Subject { get; set; }

        public IList<string> Destinations { get; set; }

        public string Content { get; set; }

        public bool IsHtml { get; set; }
    }

    public class SendTemplateOptions
    {
        public object Model { get; set; }

        public string Subject { get; set; }

        public IList<string> Destinations { get; set; }
    }


    public interface IEmailService
    {
        Task SendAsync(SendEmailOptions options);

        Task SendFileTemplate(string filename, SendTemplateOptions options);

        Task SendTemplate(string template, SendTemplateOptions options);
    }
}