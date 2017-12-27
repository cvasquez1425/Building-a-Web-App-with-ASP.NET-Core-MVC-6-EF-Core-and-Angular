using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Let's implement a service based on that interface IMailService.
/// </summary>
namespace TheWorld.Services
{
    public class DebugMailService : IMailService
    {
        public void SendMail(string to, string from, string subject, string body)
        {
            // $ dollar sign interpolation syntax to say to and from
            Debug.WriteLine($"Sending Mail: To: {to} from: {from} Subject: {subject}");
        }
    }
}
