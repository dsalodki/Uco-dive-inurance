using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Uco.Infrastructure;
using Uco.Infrastructure.Repositories;
using Uco.Models;

namespace Uco.Controllers
{
    public class ShopCartController : BaseController
    {
        private const string fileName = "idive_payment_log2.txt";
        #region ShopCart
        [HttpGet]
        public ActionResult Index(int? ID, int? Quantity)
        {
            ShopCart cart = new ShopCart();

            string theme = CurrentSettings.Themes;
            if (string.IsNullOrEmpty(theme)) ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            else ViewBag.Layout = "~/Themes/" + theme + "/Views/Shared/_Layout.cshtml";

            if (ID == null || Quantity == null)
            {
                ViewBag.Items = SF.GetCart();
                return View(cart);
            }
            else
            {
                SF.AddItemToCart((int)ID, (int)Quantity);
                return RedirectToAction("Index");
            }
        }

        public ActionResult Delete(int ID)
        {
            SF.DeleteItemFromCart(ID);
            return RedirectToAction("Index");
        }

        public ActionResult Clear()
        {
            SF.ClearCart();
            return RedirectToAction("Index");
        }

        public ActionResult PayDone()
        {
            string theme = CurrentSettings.Themes;
            if (string.IsNullOrEmpty(theme)) ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            else ViewBag.Layout = "~/Themes/" + theme + "/Views/Shared/_Layout.cshtml";

            ViewBag.PayDoneText = RP.GetTextComponent("תודה שקנית");

            return View();
        }

        public ActionResult PayNotDone()
        {
            string theme = CurrentSettings.Themes;
            if (string.IsNullOrEmpty(theme)) ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            else ViewBag.Layout = "~/Themes/" + theme + "/Views/Shared/_Layout.cshtml";
            return View();
        }

        [HttpPost]
        public ActionResult Index(ShopCart c, string InvisibleCaptchaValue)
        {
            if (!CaptchaController.IsInvisibleCaptchaValid(InvisibleCaptchaValue))
            {
                ModelState.AddModelError(string.Empty, "Captcha error.");
                return Content("Error: Captcha", "text/html");
            }

            decimal Total = SF.GetCartTotal();
            List<ShopCartItem> cil = SF.GetCart();

            if (ModelState.IsValid && cil.Count > 0)
            {
                ShopOrder o = new ShopOrder();

                //update ShopPayType
                if (c.ShopPayType == null) o.ShopPayType = ShopPayTypeEnum.NoPayment;
                else o.ShopPayType = (ShopPayTypeEnum)c.ShopPayType;

                //update DeliveryType
                if (c.ShopShippingType == null)
                {
                    o.ShopShippingType = ShopShippingTypeEnum.NoShipment;
                    o.ShopDeliveryDate = DateTime.Now;
                }
                else
                {
                    o.ShopShippingType = (ShopShippingTypeEnum)c.ShopShippingType;
                    o.ShopDeliveryDate = DateTime.Now.Date.AddDays(1).AddDays(RP.GetCurrentSettings().ShopShippingDays);

                    //add shipping price
                    if (c.ShopShippingType == 2)
                    {
                        o.ShopShippingPrice = RP.GetCurrentSettings().ShopShippingPrice;
                        foreach (ShopCartItem item in SF.GetCart())
                        {
                            o.ShopShippingPrice = o.ShopShippingPrice + item.ShopPriceShipping * item.ShopQuantity;
                        }
                        Total = Total + o.ShopShippingPrice;
                    }
                }

                //update data
                o.ShopFirstName = c.ShopFirstName;
                o.ShopLastName = c.ShopLastName;
                o.ShopCompanyName = c.ShopCompanyName;
                o.ShopPhone1 = c.ShopPhone1;
                o.ShopPhone2 = c.ShopPhone2;
                o.ShopEmail = c.ShopEmail;
                o.ShopAdressCity = c.ShopAdressCity;
                o.ShopAdressStreet = c.ShopAdressStreet;
                o.ShopOther = c.ShopOther;
                o.ShopApprovedDate = DateTime.Now;
                o.ShopStatus = ShopOrderStatusEnum.Placed;
                o.ShopSetDataToXML<ShopCartItem>(cil);
                o.ShopTotal = Total;
                o.OrderGuid = Guid.NewGuid();
                o.ShopDomainID = RP.GetCurrentSettings().ID;
                _db.ShopOrders.Add(o);
                _db.SaveChanges();

                //update log
                SF.AddToOrderLog(o.ID, "הזמנה נוצרה");

                //sign up for newsletter
                if (!string.IsNullOrEmpty(o.ShopEmail) && SF.isEmail(o.ShopEmail) && _db.Newsletters.Any(n => n.NewsletterEmail == o.ShopEmail) == false )
                {
                    _db.Newsletters.Add(new Newsletter() { NewsletterAccept = true, NewsletterEmail = o.ShopEmail, NewsletterName = o.ShopFirstName + " " + o.ShopLastName, NewsletterDate = DateTime.Now, NewsletterData = "ביצע רכישה בחנות" });
                    _db.SaveChanges();
                }

                //send mails
                if (o.ShopPayType == ShopPayTypeEnum.Phone)
                {
                    SF.SendOrderPayed(o);
                }

                //update log
                //SF.AddToOrderLog(o.ID, "מיילים נשלחו למנהל אתר וללקוח");


                // register to newsletter
                if (!string.IsNullOrEmpty(c.ShopEmail))
                {
                    SF.RegisterToNewsletter(c.ShopFirstName + " " + c.ShopLastName, c.ShopEmail, string.Empty, string.Empty);
                }

                //update analitics
                //foreach (Uco.Models.ShopCartItem item in SF.GetCart())
                //{
                //    SF.AddAnaliticsData(item.ID, false, false, false, true);
                //}



                //redirect to payment
                if (o.ShopPayType == ShopPayTypeEnum.CreditGuard) return RedirectToAction("PayCreditGuard", new { ID = o.ID });
                else if (o.ShopPayType == ShopPayTypeEnum.ZCredit) return RedirectToAction("PayZCredit", new { ID = o.ID });
                else if (o.ShopPayType == ShopPayTypeEnum.Paypal) return RedirectToAction("PayPayPal", new { ID = o.ID });
                SF.ClearCart();
                return RedirectToAction("PayDone");
            }

            string theme = CurrentSettings.Themes;
            if (string.IsNullOrEmpty(theme)) ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            else ViewBag.Layout = "~/Themes/" + theme + "/Views/Shared/_Layout.cshtml";
            ViewBag.Items = SF.GetCart();
            return View("Index", c);
        }

        [ChildActionOnly]
        public ActionResult _SmallCart()
        {
            ViewBag.ProductNumber = SF.GetCart().Count();
            ViewBag.TotalPrice = SF.GetCartTotal();
            return View();
        }

        [ChildActionOnly]
        public ActionResult _Cart()
        {
            ViewBag.ProductNumber = SF.GetCart().Count();
            ViewBag.TotalPrice = SF.GetCartTotal();
            return View(SF.GetCart());
        }

        public ActionResult Search(string ID)
        {
            string theme = CurrentSettings.Themes;
            if (string.IsNullOrEmpty(theme)) ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            else ViewBag.Layout = "~/Themes/" + theme + "/Views/Shared/_Layout.cshtml";

            List<ShopProductPage> l = _db.ShopProductPages
                .Where(r => r.DomainID == CurrentSettings.ID && r.Visible == true && r.ShowInMenu == true && (
                    r.ShopCategoryNames.Contains(ID)
                    || r.Title.Contains(ID)
                    || r.ShortDescription.Contains(ID)
                    || r.Text.Contains(ID)
                    || r.ShopBrand.Contains(ID)
                    || r.ShopSKU.Contains(ID)
                ))
                .OrderBy(r => r.Order)
                .ToList();

            ViewBag.Title = ID;

            return View(l);
        }

        // ---------------------- ZCredit ---------------------------

        public ActionResult PayZCredit(int ID)
        {
            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ID == ID);
            if (o == null) return Content("Can't find order");

            string sURL = "https://pci.zcredit.co.il/WebControl/RequestToken.aspx";

            NameValueCollection formData = new NameValueCollection();

            formData.Add("TerminalNumber", RP.GetCurrentSettings().ShopPaymentZCreditTerminalNumber);
            formData.Add("UserName", RP.GetCurrentSettings().ShopPaymentZCreditUserName);
            formData.Add("PaymentSum", o.ShopTotal.ToString());
            formData.Add("Currency", "1");
            formData.Add("Lang", "he-IL");
            formData.Add("HideCustomer", "1");
            formData.Add("ShowHolderID", "0");
            formData.Add("UniqueID", ID.ToString());
            formData.Add("NotifyLink", "http://" + Request.ServerVariables["HTTP_HOST"] + "/cart/PayZCreditIPN");
            formData.Add("RedirectLink", "http://" + Request.ServerVariables["HTTP_HOST"] + "/cart/PayDone");

            WebClient webClient = new WebClient();
            byte[] responseBytes = webClient.UploadValues(sURL, formData);
            string response = Encoding.UTF8.GetString(responseBytes);

            if (response.Contains("Error"))
            {
                return Content("Error: " + response);
            }

            if (!string.IsNullOrEmpty(response))
            {
                string[] r = response.Split('\n');
                string GUID = r[0];
                string DataPackage = r[1];

                o.ShopApprovedGuid = GUID;
                _db.SaveChanges();
                return Redirect("https://pci.zcredit.co.il/WebControl/Transaction.aspx?GUID=" + GUID.Trim() + "&DataPackage=" + DataPackage.Trim());
            }
            else
            {
                return Content("Invalid response");
            }
        }
        public ActionResult PayZCreditIPN(int? UniqueID, string ApprovalNumber, decimal? Sum, string GUID)
        {
            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ID == UniqueID);
            string error = "";
            if (o == null) error = "Can't find order";
            if (o.ShopApprovedGuid != GUID) error = "o.ShopApprovedGuid != GUID";
            if (o.ShopTotal != Sum) error = "o.ShopTotal != Sum";

            if (!string.IsNullOrEmpty(error))
            {
                LogZCreditData(DateTime.Now + " - " + error + ". IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n");
                return Content(error);
            }

            o.ShopStatus = ShopOrderStatusEnum.Payed;
            o.ShopApprovedToken = ApprovalNumber;
            o.ShopApprovedDate = DateTime.Now;

            o.ShopLog = o.ShopLog + DateTime.Now + " - תשלום התקבל דרך ZCredit. IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";

            _db.Entry(o).State = EntityState.Modified;
            _db.SaveChanges();

            SF.SendInvoice(o);
            SF.SendOrderPayed(o);
            SF.ClearCart();
            return Content("Done");
        }
        private void LogZCreditData(string LogText)
        {
            System.IO.File.AppendAllText(Server.MapPath("~/App_Data/zcreditlog.txt"), LogText);
        }

        // ---------------------- CreditGuard --------------------------

        public ActionResult PayCreditGuard(int ID)
        {
            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ID == ID);
            if (o == null) return Content("Can't find order");

            o.OrderGuid = Guid.NewGuid();
            _db.Entry(o).State = EntityState.Modified;
            _db.SaveChanges();

            string returnUrl = "http://" + Request.ServerVariables["HTTP_HOST"] + "/shopcart/PayCreditGuardOK";
            string cancelReturnUrl = "http://" + Request.ServerVariables["HTTP_HOST"] + "/shopcart/PayNotDone";

            // _logger("return url: " + returnUrl, fileName);
            // _logger("cancel return url: " + cancelReturnUrl, fileName);

            string url = "";

            try
            {

                // this is what we are sending
                string xml_data = "<ashrait>" +
                        "<request>" +
                            "<version>1001</version>" +
                            "<language>HEB</language>" +
                            "<dateTime/>" +
                            "<command>doDeal</command>" +
                            "<requestid/>" +
                                "<doDeal>" +
                                "<successUrl>" + returnUrl + "</successUrl>" +
                                "<errorUrl>" + cancelReturnUrl + "</errorUrl>" +
                                    "<terminalNumber>" + CurrentSettings.PaymentCreditGuardID1 + "</terminalNumber>" +
                                    "<cardNo>CGMPI</cardNo>" +
                                    "<total>" + Math.Round((double)o.ShopTotal * 100, 0).ToString("0", CultureInfo.InvariantCulture) + "</total>" +
                                    "<transactionType>Debit</transactionType>" +
                                    "<creditType>RegularCredit</creditType>" +
                                    "<currency>ILS</currency>" +
                                    "<transactionCode>Phone</transactionCode>" +
                                    "<validation>TxnSetup</validation>" +
                                    "<firstPayment></firstPayment>" +
                                    "<periodicalPayment></periodicalPayment>" +
                                    "<numberOfPayments></numberOfPayments>" +
                                    "<user>" + o.ShopFirstName + " " + o.ShopLastName + "</user>" +
                                    "<eci></eci>" +
                                    "<mid>" + CurrentSettings.PaymentCreditGuardID2 + "</mid>" + // ========================================================================================
                    // "<supplierNumber>" + CurrentSettings.PaymentCreditGuardID2 + "</supplierNumber>" +
                                    "<uniqueid>" + o.OrderGuid.ToString() + "</uniqueid>" +
                                    "<mpiValidation>AutoComm</mpiValidation>" +
                                    "<description>Idive Shop</description>" +
                                    "<email>" + o.ShopEmail + "</email>" +
                                    "<customerData>" +
                                        "<userData1/>" +
                                        "<userData2/>" +
                                        "<userData3/>" +
                                        "<userData4/>" +
                                        "<userData5/>" +
                                        "<userData6/>" +
                                        "<userData7/>" +
                                        "<userData8/>" +
                                        "<userData9/>" +
                                        "<userData10/>" +
                                    "</customerData>" +
                                "</doDeal>" +
                            "</request>" +
                        "</ashrait>";

                string post_data = "user=" + CurrentSettings.PaymentCreditGuardID3 + "&password=" + CurrentSettings.PaymentCreditGuardID4 + "&int_in=" + xml_data;

                // this is where we will send it
                string uri = CurrentSettings.PaymentCreditGuardUrl;



                // create a request
                HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(uri);
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.Method = "POST";

                // turn our request string into a byte stream
                //byte[] postBytes = Encoding.ASCII.GetBytes(post_data);
                byte[] postBytes = Encoding.GetEncoding("UTF-8").GetBytes(post_data);

                // this is important - make sure you specify type this way
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postBytes.Length;
                Stream requestStream = request.GetRequestStream();

                // now send it
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                // grab te response and print it out to the console along with the status code
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string responseString = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8")).ReadToEnd();

                // _logger("response string: " + responseString, fileName);

                if (!responseString.Contains("https://"))
                    return Content("Can't get ShopUrl " + responseString);

                int urlStart = responseString.ToLower().IndexOf("<mpihostedpageurl>");
                int urlEnd = responseString.ToLower().IndexOf("</mpihostedpageurl>");
                int responseStringLength = responseString.Length;

                if (urlStart == -1 || urlStart > responseStringLength || urlEnd == -1 || urlEnd > responseStringLength || urlEnd < urlStart)
                {
                    return Content("Error urlStart urlEnd " + responseString);
                }

                url = responseString.Substring(urlStart + 18, urlEnd - urlStart - 18);
                url = url.Trim();

                // _logger("redirect url: " + url, fileName);

                if (string.IsNullOrEmpty(url))
                {
                    return Content("Error IsNullOrEmpty(url) " + responseString);
                }

            }
            catch (Exception ex)
            {
                _logger(ex.Message, "log4.txt");
            }

            return Redirect(url);
        }

        public ActionResult PayCreditGuardOK(Guid? uniqueID, string txId, string authNumber)
        {
            // _logger("PayCreditGuardOK entered", fileName);

            //ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ID == uniqueID); // ===================================================================================
            ShopOrder o = _db.ShopOrders.FirstOrDefault(x => x.OrderGuid == uniqueID);

            string error = "";
            if (o == null) error = "Can't find order";

            if (!string.IsNullOrEmpty(error))
            {
                LogCreditGuardData(DateTime.Now + " - " + error + ". IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n");
                return Content(error);
            }

            o.ShopStatus = ShopOrderStatusEnum.Payed;
            o.ShopApprovedGuid = txId;
            o.ShopApprovedToken = authNumber;
            o.ShopApprovedDate = DateTime.Now;

            o.ShopLog = o.ShopLog + DateTime.Now + " - תשלום התקבל דרך CreditGuard. IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";

            _db.Entry(o).State = EntityState.Modified;
            _db.SaveChanges();
            SF.ClearCart();
            SF.SendInvoice(o);
            SF.SendOrderPayed(o);

            // _logger("PayCreditGuardOK before exit", fileName);

            return RedirectToAction("PayDone");
        }
        public ActionResult PayCreditGuardNotOK(int? uniqueID, string txId, string authNumber, string ErrorText, string ErrorCode)
        {
            string theme = CurrentSettings.Themes;
            if (string.IsNullOrEmpty(theme)) ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            else ViewBag.Layout = "~/Themes/" + theme + "/Views/Shared/_Layout.cshtml";

            LogCreditGuardData(DateTime.Now + " - " + ErrorCode + " " + ErrorText + ". IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n");

            _logger("PayCreditGuardNotOK", fileName);

            return RedirectToAction("PayNotDone");
        }
        private void LogCreditGuardData(string LogText)
        {
            System.IO.File.AppendAllText(Server.MapPath("~/App_Data/creditguardlog.txt"), LogText);
        }

        // ---------------------- Paypal ---------------------------

        public ActionResult PayPaypal(int ID)
        {

            ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ID == ID);
            if (o == null) return Content("can't find ShopOrder");

            string theme = CurrentSettings.Themes;
            if (string.IsNullOrEmpty(theme)) ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
            else ViewBag.Layout = "~/Themes/" + theme + "/Views/Shared/_Layout.cshtml";

            return View(o);
        }

        public ActionResult PayPaypalIPN()
        {
            string LogText = "";

            string strLive = "https://www.paypal.com/cgi-bin/webscr";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(strLive);

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            byte[] param = Request.BinaryRead(HttpContext.Request.ContentLength);
            string strRequest = Encoding.ASCII.GetString(param);
            strRequest += "&cmd=_notify-validate&lc=he_IL";
            req.ContentLength = strRequest.Length;

            //for proxy
            //WebProxy proxy = new WebProxy(new Uri("http://url:port#"));
            //req.Proxy = proxy;

            //Send the request to PayPal and get the response
            StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
            streamOut.Write(strRequest);
            streamOut.Close();
            StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
            string strResponse = streamIn.ReadToEnd();
            streamIn.Close();

            if (strResponse == "VERIFIED")
            {
                string mc_gross_string = Request["mc_gross"];

                decimal mc_gross_decimal = 0;
                decimal.TryParse(mc_gross_string, out mc_gross_decimal);

                string item_name = Request["item_name"];

                string receiver_email = Request["receiver_email"];
                string receiver_id = Request["receiver_id"];
                string test_ipn = Request["test_ipn"];
                string txn_id = Request["txn_id"];
                string txn_type = Request["txn_type"];
                string payer_email = Request["payer_email"];
                string payer_id = Request["payer_id"];
                string payer_status = Request["payer_status"];
                string first_name = Request["first_name"];
                string last_name = Request["last_name"];
                string address_city = Request["address_city"];
                string address_country = Request["address_country"];
                string address_country_code = Request["address_country_code"];
                string address_name = Request["address_name"];
                string address_state = Request["address_state"];
                string address_status = Request["address_status"];
                string address_street = Request["address_street"];
                string address_zip = Request["address_zip"];

                string handling_amount = Request["handling_amount"];
                string item_number = Request["item_number"];
                string mc_currency = Request["mc_currency"];
                string mc_fee = Request["mc_fee"];
                string mc_gross = Request["mc_gross"];
                string payment_date = Request["payment_date"];
                string payment_fee = Request["payment_fee"];
                string payment_gross = Request["payment_gross"];

                string payment_status = Request["payment_status"];
                string payment_type = Request["payment_type"];
                string protection_eligibility = Request["protection_eligibility"];
                string quantity = Request["quantity"];
                string shipping = Request["shipping "];
                string tax = Request["tax "];
                string notify_version = Request["notify_version"];
                string charset = Request["charset"];
                string verify_sign = Request["verify_sign"];

                int custom = 0;
                int.TryParse(Request["custom"], out custom);

                string AllData = "receiver_email: " + receiver_email + "\r\n"
                    + "receiver_id: " + receiver_id + "\r\n"
                    + "txn_type: " + txn_type + "\r\n"
                    + "txn_id: " + txn_id + "\r\n"
                    + "payer_email: " + payer_email + "\r\n"
                    + "payer_id: " + payer_id + "\r\n"
                    + "payer_status: " + payer_status + "\r\n"
                    + "first_name: " + first_name + "\r\n"
                    + "last_name: " + last_name + "\r\n"
                    + "address_city: " + address_city + "\r\n"
                    + "address_country: " + address_country + "\r\n"
                    + "address_country_code: " + address_country_code + "\r\n"
                    + "address_name: " + address_name + "\r\n"
                    + "address_state: " + address_state + "\r\n"
                    + "address_status: " + address_status + "\r\n"
                    + "address_street: " + address_street + "\r\n"
                    + "address_zip: " + address_zip + "\r\n"
                    + "handling_amount: " + handling_amount + "\r\n"
                    + "item_number: " + item_number + "\r\n"
                    + "mc_currency: " + mc_currency + "\r\n"
                    + "mc_fee: " + mc_fee + "\r\n"
                    + "mc_gross: " + mc_gross + "\r\n"
                    + "payment_fee: " + payment_fee + "\r\n"
                    + "payment_gross: " + payment_gross + "\r\n"
                    + "payment_status: " + payment_status + "\r\n"
                    + "payment_type: " + payment_type + "\r\n"
                    + "protection_eligibility: " + protection_eligibility + "\r\n"
                    + "quantity: " + quantity + "\r\n"
                    + "shipping: " + shipping + "\r\n"
                    + "tax: " + tax + "\r\n"
                    + "notify_version: " + notify_version + "\r\n"
                    + "charset: " + charset + "\r\n"
                    + "verify_sign: " + verify_sign + "\r\n"
                    + "custom: " + custom + "\r\n____________________________________\r\n";

                LogPaypalData(DateTime.Now + " " + AllData);

                ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ID == custom);
                if (o == null)
                {
                    LogPaypalData(DateTime.Now + " - IPN של PAYPAL לא תקין. can't find ShopOrder " + custom + ". IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n");
                    return Content("can't find ShopOrder");
                }

                //check the payment_status is Completed
                if (payment_status.Trim().ToLower() != "completed")
                {
                    o.ShopLog = o.ShopLog + DateTime.Now + " - IPN של PAYPAL לא תקין. payment_status = " + payment_status + ". IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";
                    _db.Entry(o).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Content("payment_status error");
                }
                //check that txn_id has not been previously processed
                if (o.ShopStatus == ShopOrderStatusEnum.Payed)
                {
                    o.ShopLog = o.ShopLog + DateTime.Now + " - IPN של PAYPAL לא תקין. txn_id has not been previously processed. IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";
                    _db.Entry(o).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Content("txn_id has not been previously processed error");
                }
                //check that receiver_email is your Primary PayPal email
                //if(receiver_email.Trim().ToLower() != PayPalEmail)
                //{
                //    o.OrderLog = o.OrderLog + DateTime.Now + " - IPN של PAYPAL לא תקין. receiver_email is your Primary PayPal email error. IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";
                //    _db.Entry(o).State = EntityState.Modified;
                //    _db.SaveChanges();
                //    return Content("receiver_email is your Primary PayPal email error");
                //}

                //check that payment_amount/payment_currency are correct
                if (mc_currency.Trim().ToLower() != "ILS".ToLower())
                {
                    o.ShopLog = o.ShopLog + DateTime.Now + " - IPN של PAYPAL לא תקין. payment_currency error " + mc_gross_decimal + " " + mc_currency + ". IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";
                    _db.Entry(o).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Content("payment_currency error");
                }

                if (mc_gross_decimal != o.ShopTotal)
                {
                    o.ShopLog = o.ShopLog + DateTime.Now + " - IPN של PAYPAL לא תקין. payment_amount " + mc_gross_decimal + " " + mc_currency + ". IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";
                    _db.Entry(o).State = EntityState.Modified;
                    _db.SaveChanges();
                    return Content("payment_amount error");
                }

                //process payment
                o.ShopStatus = ShopOrderStatusEnum.Payed;
                o.ShopApprovedDate = DateTime.Now;
                o.ShopPayType = ShopPayTypeEnum.Paypal;

                o.ShopLog = o.ShopLog + DateTime.Now + " - תשלום התקבל דרך PAYPAL. " + strResponse + " IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";
                _db.Entry(o).State = EntityState.Modified;
                _db.SaveChanges();

                SF.SendInvoice(o);
                SF.SendOrderPayed(o);

            }
            else if (strResponse == "INVALID")
            {
                LogText = LogText + DateTime.Now + " - IPN error. " + strResponse + " IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";
                LogPaypalData(LogText);
            }
            else
            {
                LogText = LogText + DateTime.Now + " - IPN error. " + strResponse + " IP - " + Request.ServerVariables["LOCAL_ADDR"] + "\r\n";
                LogPaypalData(LogText);
            }

            return Content("Done");
        }
        private void LogPaypalData(string LogText)
        {
            System.IO.File.AppendAllText(Server.MapPath("~/App_Data/paypallog.txt"), LogText);
        }

        private void _logger(string message, string fileName)
        {
            using (StreamWriter streamWriter = new StreamWriter(@"C:\Sites\idive.co.il\content\log\" + fileName, true))
            {
                streamWriter.WriteLine(string.Format("{0:dd/MM/yyyy hh:mm:ss tt}", DateTime.Now) + " " + message);
            }
        }

        // ===== http://www.idive.co.il/shopcart/PayCreditGuardOK?uniqueID=d999facb-36e8-4ebd-b51f-0d6e276739d1&lang=HE&authNumber=6531818&responseMac=ef2709a2903ec81b185dc9f809b10de62cfb0c469bc3429d1aeb8b97b05e798c&cardToken=1094006948606357&cardExp=0218&personalId=&cardMask=532611******6357&txId=870d0861-f86b-416d-b1cb-e3595a29d657&numberOfPayments=&firstPayment=&periodicalPayment=

        #endregion
    }
}