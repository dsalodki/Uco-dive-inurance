using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uco.Infrastructure.Repositories;
using Uco.Models;
using System.Data.Entity;

namespace Uco.Infrastructure.Tasks
{
    public class ExpiredInternalInsurancesNotificator : ITask
    {
        public string Title { get { return "ExpiredInternalInsurancesNotificator"; } }

        public int StartSeconds { get { return 60; } }

        public int IntervalSecondsFrom { get { return 86400; } }

        public int IntervalSecondsTo { get { return 86400; } }

        public void Execute()
        {
            using (Db _db = new Db())
            {
                var subject = _db.Translations.FirstOrDefault(t => t.SystemName == "ExpiredInternalInsurancesNotificator.Subject")?.Text ?? string.Empty;
                var textComponentText = _db.TextComponents.FirstOrDefault(t => t.SystemName == "ExpiredInsuranceEmail")?.Text;

                if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(textComponentText))
                {
                    return;
                }

                var today = DateTime.UtcNow.Date;
                var insurances = _db.Insurances.Include(m => m.User).Where(x => x.ExternalInsurance && x.InsuranceEndDate < today && !x.SentNotificationExpired).ToArray();


                foreach (var insurance in insurances)
                {
                    var body = string.Concat("<div style='text-align: right; direction: rtl;'>",
                        textComponentText.Replace("{InsuranceStartDate}", insurance.InsuranceStartDate.ToString("dd/MM/yyyy")).Replace("{InsuranceEndDate}", insurance.InsuranceEndDate.ToString("dd/MM/yyyy")),
                        "</div>");

                    _db.OutEmails.Add(new OutEmail
                    {
                        MailTo = insurance.User.Email,
                        Subject = subject,
                        Body = body,
                        TimesSent = 0,
                        LastTry = DateTime.Now
                    });
                    insurance.SentNotificationExpired = true;
                }
                if (insurances.Count() > 0)
                {
                    _db.SaveChanges();
                }
            }
        }
    }
}