using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public interface IMessageService
    {
        bool SendCode(int code, string to);
        bool SendMessage(string message, string to);
    }
}
