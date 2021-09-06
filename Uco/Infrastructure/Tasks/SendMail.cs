using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Uco.Infrastructure.Livecycle;
using Uco.Models;

namespace Uco.Infrastructure.Tasks
{
    public class SendMail : ITask
    {
        private const int TimesToSendMax = 3;
        private const int TimeInterval = 3600;

        public string Title { get { return "SendMail"; } }
        public int StartSeconds { get { return 10; } }
        public int IntervalSecondsFrom { get { return 1; } }
        public int IntervalSecondsTo { get { return 20; } }

        public void Execute()
        {
            using(Db _db = new Db())
            {
                if (_db.OutEmails.Count() == 0) return;
                OutEmail om = _db.OutEmails.First();
                if (!SF.isEmail(om.MailTo))
                {
                    _db.OutEmails.Remove(om);
                    _db.SaveChanges();
                    return;
                }
                int sent = SendEmail(om);
                if (sent == -1) return;
                _db.OutEmails.Remove(om);
                _db.SaveChanges();
                if (sent != 0 && sent < TimesToSendMax)
                {
                    om.TimesSent = sent;
                    om.LastTry = DateTime.Now;
                    _db.OutEmails.Add(om);
                    _db.SaveChanges();
                }
            }
        }

        public int SendEmail(OutEmail om)
        {
            if (om.LastTry > DateTime.Now &&  DateTime.Now > om.LastTry.AddSeconds(TimeInterval)) return -1;
            SmtpClient client = new SmtpClient();
            string MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            MailMessage mm = new MailMessage(MailFrom, om.MailTo);
            mm.IsBodyHtml = true;
            mm.SubjectEncoding = Encoding.GetEncoding(1255);
            mm.BodyEncoding = Encoding.UTF8;
            if (mm.Subject != null) mm.Subject = om.Subject.Replace("\r\n", "");
            mm.Body = om.Body;
            try
            {
                client.Send(mm);
                return 0;
            }
            catch(Exception ex)
            {
                SF.LogError(ex);
                return om.TimesSent + 1;
            }
        }
    }
}