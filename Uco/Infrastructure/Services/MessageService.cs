using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;
using Uco.Models;
using System.Linq;
using System.Web.WebPages.Html;
using Uco.Models.Overview;
using Uco.Infrastructure.Services;
using System.Text;
using System.Reflection;
using System.Web.Mvc;

namespace Uco.Infrastructure
{
    public class MessageService
    {
        public MessageService(Db _db = null)
        {
            _Context = new DBContextService(_db);
        }
        private DBContextService _Context = null;
        private string _SMSGateway = "@sms.inforu.co.il";

        #region Util
        public void CreateAllMissingMessageTemplates()
        {
            MethodInfo[] methodInfos = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var m in methodInfos)
            {
                if (m.ReturnParameter != null && m.ReturnParameter.ParameterType.Name == "Int32")
                {
                    try
                    {
                        var pinfo = m.GetParameters();
                        var objs = new List<object>();
                        foreach (var pi in pinfo)
                        {
                            var o = Activator.CreateInstance(pi.ParameterType);
                            objs.Add(o);
                        }
                        m.Invoke(this, objs.ToArray());
                    }
                    catch (Exception e)
                    {

                    }

                }
            }

        }
        #endregion

        private int Tokenize(string SystemName, string ToEmail, IList<object> models)
        {
            var LanguageCode = SF.GetLangCodeThreading();
            var mailTemplate = _Context.EntityContext.MessageTemplates.FirstOrDefault(x => x.SystemName == SystemName && x.LanguageCode == LanguageCode);
            if (mailTemplate != null && mailTemplate.Active && !string.IsNullOrEmpty(ToEmail))
            {
                StringBuilder Subject = new StringBuilder(mailTemplate.Subject);
                StringBuilder Body = new StringBuilder(mailTemplate.Body);


                foreach (var o in models)
                {
                    var t = o.GetType();
                    var properties = t.GetProperties();
                    foreach (var p in properties)
                    {
                        string repl = "";
                        var value = p.GetValue(o);
                        if (value != null)
                        {
                            repl = value.ToString();
                        }
                        Subject.Replace(string.Format("%{0}.{1}%", t.Name, p.Name), repl); // %Order.Total% -> $345.34
                        Body.Replace(string.Format("%{0}.{1}%", t.Name, p.Name), repl); // %Order.Total% -> $345.34
                    }
                }

                var email = new OutEmail()
               {
                   Body = Body.ToString(),
                   Subject = Subject.ToString(),
                   MailTo = ToEmail,

               };

                _Context.EntityContext.OutEmails.Add(email);
                _Context.EntityContext.SaveChanges();
                return email.ID;

            }
            if (mailTemplate == null)
            {
                //add to log or create automatic
                var template = new MessageTemplate()
                {
                    Active = false,
                    Body = "Auto generated, please change",
                    LanguageCode = SF.GetLangCodeThreading(),
                    Subject = "Auto generated",
                    SystemName = SystemName
                };
                _Context.EntityContext.MessageTemplates.Add(template);
                _Context.EntityContext.SaveChanges();
            }
            return 0;
        }
        public int SendUserRegisterEmailToUser(User user)
        {
            var res = Tokenize("User.Register.EmailToUser", user.Email, new List<object>() {user});
            return res;
        }
        public int SendUserPasswordEmailToUser(User user)
        {
            var res = Tokenize("User.SendPassword.EmailToUser", user.Email, new List<object>() { user });
            return res;
        }
        public int OrderQuestionedToMember(Order order, Shop shop, List<PollAnswer> Answers)
        {
            var mailparams = new List<object>() {
            order,
            shop
            };
            mailparams.AddRange(Answers);
            var res = Tokenize("Order.Questioned.EmailToMember", shop.Email, mailparams);
            return res;
        }
        public int SendQuestionsDeliveredToUser(Order order, Shop shop)
        {
            //http://localhost:23416/poll/index?OrderID=30&ShopID=3
            PollPageModel model = new PollPageModel();
            var domain = _Context.EntityContext.SettingsAll.Select(x => x.Domain).FirstOrDefault();
            model.PollUrl = string.Format("{0}://{1}/poll/index?OrderID={2}&ShopID={3}",
                "http",
                domain,
                order.ID.ToString(),
                shop.ID.ToString());
            var res = Tokenize("Poll.OrderShop.EmailToUser", order.Email, new List<object>() {
            order,
            shop,
            model
            });
            return res;
        }
        public int SendOrderPayedEmailToUser(Order order, Shop shop)
        {
            var res = Tokenize("Order.Payed.EmailToUser", order.Email, new List<object>() {
            order,
            shop
            });
            return res;
        }
        public int SendOrderPayedEmailToMember(Order order, Shop shop)
        {
            var res = Tokenize("Order.Payed.EmailToMember", shop.Email, new List<object>() {
            order,
            shop
            });
            return res;
        }
        public int SendNewOrderEmailToUser(Order order, Shop shop)
        {
            var res = Tokenize("Order.New.EmailToUser", order.Email, new List<object>() {
            order,
            shop
            });
            return res;
        }
        public int SendNewOrderSMSToUser(Order order, Shop shop,User user)
        {
            if (user != null && user.Phone != null)
            {
                var res = Tokenize("Order.New.SmsToUser", user.Phone + _SMSGateway, new List<object>() {
            order,
            shop
            });
                return res;
            }
            return 0;
        }
        public int SendNewOrderEmailToMember(Order order, Shop shop)
        {
            var res = Tokenize("Order.New.EmailToMember", shop.Email, new List<object>() {
            order,
            shop
            });
            return res;
        }
        public int SendNewOrderSMSToMember(Order order, Shop shop)
        {
            if (shop.Phone != null)
            {
                var res = Tokenize("Order.New.SmsToMember", shop.Phone+_SMSGateway, new List<object>() {
            order,
            shop
            });
                return res;
            } 
            return 0;
        }
        public int SendOrderChangedEmailToUser(Order order)
        {
            var res = Tokenize("Order.Changed.ToUser", order.Email, new List<object>() {
            order
            });
            return res;
        }
        public int SendOrderChangedSmsToUser(Order order,User user)
        {
            if (user != null && user.Phone != null)
            {
                var res = Tokenize("Order.Changed.SmsToUser", user.Phone+_SMSGateway, new List<object>() {
            order
            });
                return res;
            }
            return 0;
        }
        public int CheckoutSmsConfirmSmsToUser(CheckoutData checkooutData, User user)
        {
            if (user != null && user.Phone != null)
            {
                var res = Tokenize("Checkout.SmsConfirm.SmsToUser", user.Phone + _SMSGateway, new List<object>() {
                    user,
            checkooutData
            });
                return res;
            }
            return 0;
        }

        public int OrderCanceledSmsToUser(Order order, User user)
        {
             if (user != null && user.Phone != null)
            {
                var res = Tokenize("Order.Canceled.SmsToUser", user.Phone+_SMSGateway, new List<object>() {
            order
            });
            return res;
            }
             return 0;
        }
        public int OrderNotSendedToAdmin(Order order)
        {
            var domain = _Context.EntityContext.SettingsAll.Select(x => x.AdminEmail).FirstOrDefault();
            var res = Tokenize("Order.NotSended.EmailToAdmin", domain, new List<object>() {
            order           
            });
            return res;
        }
        public int OrderCanceledEmailToUser(Order order)
        {
            var res = Tokenize("Order.Canceled.EmailToUser", order.Email, new List<object>() {
            order
            });
            return res;
        }
        public int SendOrderUserCantPayToUser(Order order)
        {
            var res = Tokenize("Order.UserCantPay.ToUser", order.Email, new List<object>() {
            order
            });
            return res;

        }
        public int SendOrderUserCantPayToMember(Order order, Shop shop)
        {
            var res = Tokenize("Order.UserCantPay.ToMember", shop.Email, new List<object>() {
            order
            });
            return res;

        }
    }

}