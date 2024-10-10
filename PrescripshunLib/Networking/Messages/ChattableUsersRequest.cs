using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrescripshunLib.Networking.Messages
{
    public class ChattableUsersRequest : IMessage
    {
        public Guid UserKey { get; set; } = Guid.Empty;
    }
}