using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace GIZ.API.Services
{
    public class SendSMSOptions
    {
        public string Subject { get; set; }

        public IList<string> Destinations { get; set; }

        public string Message { get; set; }
    }

    public interface ISMSService
    {
        Task SendAsync(SendSMSOptions options);
    }
}