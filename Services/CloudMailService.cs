using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService    
    {
        private readonly IConfiguration _configuration;

        // Using Constructor Injection to require our appsettings.json file for mail config
        public CloudMailService(IConfiguration configuration)
        {
            // null check
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public void Send(string subject, string message)
        {
            // send mail - output to debug window mimicing sending email
            Debug.WriteLine($"Mail from {_configuration["mailSettings:mailFromAddress"]} to {_configuration["mailSettings:mailToAddress"]}, with LocalMailService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}
