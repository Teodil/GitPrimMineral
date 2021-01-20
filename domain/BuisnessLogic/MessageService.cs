using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic
{
    public class MessageService : IMessageService
    {

        // отправитель - устанавливаем адрес и отображаемое в письме имя
        MailAddress from = new MailAddress("primmineralsinfo@gmail.com", "Администрация PrimMinerals");

        public bool SendCode(int code, string to)
        {
            try
            {
                // кому отправляем
                MailAddress _to = new MailAddress(to);
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, _to);
                // тема письма
                m.Subject = "Код подтверждения";
                // текст письма
                m.Body = $"<h2>Ваш код подтверждения {code.ToString()}</h2>";
                // письмо представляет код html
                m.IsBodyHtml = true;
                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                // логин и пароль
                smtp.Credentials = new NetworkCredential("primmineralsinfo@gmail.com", "TheKillerOfKingsMyNigga1337");
                smtp.EnableSsl = true;
                smtp.Send(m);
                return true;
            }
            catch(Exception ex)
            {
                var _ex = ex.Message;
                return false;
            }
        }

        public bool SendMessage(string message, string to)
        {
            throw new NotImplementedException();
        }
    }
}
