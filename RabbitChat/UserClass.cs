using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitChat
{
    internal class UserClass
    {
        public string UserName { get; set; }

        public List<Message> Messages { get; set; }

        public override string ToString()
        {
            return UserName.ToString();
        }
    }

    public class Message
    {
        public string message { get; set; }
        public string sender { get; set; }
        public string receiver { get; set; }
    }
}
