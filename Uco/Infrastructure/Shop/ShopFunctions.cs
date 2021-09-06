using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uco.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Web.Configuration;
using System.Dynamic;
using System.Web.Mvc;
using System.Xml;
using System.Text.RegularExpressions;
using System.Data;
using Uco.Infrastructure.Repositories;
using System.Security.Cryptography;
using System.Data.Entity;
using Kendo.Mvc.UI;

namespace Uco.Infrastructure
{
    public static partial class SF
    {

        #region ShopOther

        public static List<string> GetGoogleSuggestions(string text)
        {
            List<string> l = new List<string>();
            XmlDocument doc = new XmlDocument();
            string url = "http://google.co.il/complete/search?output=toolbar&q=" + text;

            String strResult;
            System.Net.WebResponse objResponse;

            System.Net.WebRequest objRequest = System.Net.HttpWebRequest.Create(url);

            //Pretend to be IE7
            ((System.Net.HttpWebRequest)objRequest).UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";
            ((System.Net.HttpWebRequest)objRequest).Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            System.Net.CookieContainer cc = new System.Net.CookieContainer();
            System.Net.Cookie cookie = new System.Net.Cookie("NID", "45=SXfhlhu1_OGt30xIS9k5u7RXI6lOAMDZT0M34XmrGDAhMKqqkuXrAYFDaWY-qcjg6ujFnarij4GpERCCCxa1g1ApeVyzmsKXDLdtaTLjKX6BmAL-32hTsT1fCNKkJCL8");
            cc.Add(new Uri("http://google.co.il"), cookie);
            cookie = new System.Net.Cookie("PREF", "ID=f49dd8f1e5dbf90f:U=4fea453c6fa4917d:FF=0:TM=1302338006:LM=1302338006:S=m9L0N4QmX1wxlLF0");
            cc.Add(new Uri("http://google.co.il"), cookie);
            cookie = new System.Net.Cookie("GALX", "Ncr7yxM0Zkc");
            cc.Add(new Uri("http://google.co.il"), cookie);
            ((System.Net.HttpWebRequest)objRequest).CookieContainer = cc;

            objResponse = objRequest.GetResponse();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(objResponse.GetResponseStream()))
            {
                strResult = sr.ReadToEnd();
                sr.Close();
            }

            doc.LoadXml(strResult);
            foreach (XmlNode node in doc.SelectNodes("//CompleteSuggestion"))
            {
                string value = node.SelectSingleNode("suggestion/@data").InnerText;
                l.Add(value);
            }
            return l;
        }

        public static string ProcessSearchMeta(string MetaString, string ID, bool Selected, string SelectedValue)
        {
            if (string.IsNullOrEmpty(MetaString)) return "";
            string OutString = "";
            if (Selected)
            {
                //replace [1] text by value
                OutString = MetaString.Replace("[" + ID + "]", SelectedValue);
                //remove wraper (<+1></+1>) <+1>in</+1> text when drop down selected
                OutString = RemovePlusWraper(OutString, ID);
                //remove all <-1>in</-1> text when drop down selected
                OutString = RemoveAllMinus(OutString, ID);
            }
            else
            {
                //remove [1] text
                OutString = MetaString.Replace("[" + ID + "]", "");
                //remove wraper (<-1></-1>) <-1>in</-1> text when drop down not selected
                OutString = RemoveMinusWraper(OutString, ID);
                //remove all <+1>in</+1> text when drop down not selected
                OutString = RemoveAllPlus(OutString, ID);

            }
            return OutString;
        }

        public static string ClearDoubleSpace(string s)
        {
            s = s.Replace("  ", " ");
            s = s.Replace("  ", " ");
            return s;
        }

        private static string RemovePlusWraper(string Text, string TextId)
        {
            Text = Text.Replace("<+" + TextId + ">", "");
            Text = Text.Replace("</+" + TextId + ">", "");
            return Text;
        }

        //remove wraper (<-1></-1>) <-1>in</-1> text
        private static string RemoveMinusWraper(string Text, string TextId)
        {
            Text = Text.Replace("<-" + TextId + ">", "");
            Text = Text.Replace("</-" + TextId + ">", "");
            return Text;
        }

        //remove all <+1>in</+1> text
        private static string RemoveAllPlus(string Text, string TextId)
        {
            int i = 8;
            while (Text.IndexOf("<+" + TextId + ">") > 0 && i != 0)
            {
                int StartIndex = Text.IndexOf("<+" + TextId + ">");
                int EndIndex = Text.IndexOf("</+" + TextId + ">");
                if (Text.Length > 0 && StartIndex >= 0 && EndIndex > 0 && EndIndex > StartIndex) Text = Text.Remove(StartIndex, EndIndex - StartIndex + 4 + TextId.Length);
                i--;
            }
            return Text;
        }

        //remove all <-1>in</-1> text
        private static string RemoveAllMinus(string Text, string TextId)
        {
            int i = 8;
            while (Text.IndexOf("<-" + TextId + ">") > 0 && i != 0)
            {
                int StartIndex = Text.IndexOf("<-" + TextId + ">");
                int EndIndex = Text.IndexOf("</-" + TextId + ">");
                if (Text.Length > 0 && StartIndex >= 0 && EndIndex > 0 && EndIndex > StartIndex) Text = Text.Remove(StartIndex, EndIndex - StartIndex + 4 + TextId.Length);
                i--;
            }
            return Text;
        }

        #endregion


















                #region Get/Clean Repository

        //public static List<SearchModel> GetSearchModelRepository()
        //{
        //    int ShopDomainID = SF.GetAdminCurrentSettingsRepository().ID;
        //    if (HttpContext.Current.Cache["SearchModel_" + ShopDomainID] == null)
        //    {
        //        using (Db _db = new Db())
        //        {
        //            HttpContext.Current.Cache["SearchModel_" + ShopDomainID] = _db.SearchModels.Where(r => r.ShopDomainID == ShopDomainID).ToList();
        //        }
        //    }
        //    return (List<SearchModel>)HttpContext.Current.Cache["SearchModel_" + ShopDomainID];
        //}

        //public static void CleanSearchModelRepository()
        //{
        //    foreach (Settings item in SF.GetAdminSettingsRepositoryList())
        //    {
        //        HttpContext.Current.Cache.Remove("SearchModel_" + item.ID);
        //    }
        //}

        #endregion

        #region Get/Set Repository Items

        //public static List<Telerik.Web.Mvc.UI.TreeViewItem> GetTreeViewItemModel(string Model)
        //{
        //    if (string.IsNullOrEmpty(Model) || !Model.Contains("|")) return new List<Telerik.Web.Mvc.UI.TreeViewItem>();
        //    string SearchSegmentName = Model.Split('|')[0];
        //    string SearchIDs = Model.Split('|')[1];
        //    List<int> CheckedNodesIDs = GetListIntFromString(Model);
        //    List<SearchModel> All = new List<SearchModel>();
        //    All = SF.GetSearchModelRepository().Where(r => r.SearchSegmentName == SearchSegmentName).ToList();

        //    List<TreeViewItem> l = new List<TreeViewItem>();
        //    foreach (SearchModel item in All.Where(r => r.ShopParentID == 0))
        //    {
        //        TreeViewItem tm = new TreeViewItem();
        //        tm.Value = item.ID.ToString();
        //        tm.ShopText = item.ShopProductTitle;
        //        tm.Enabled = true;
        //        //tm.Expanded = true;
        //        tm.Checked = CheckedNodesIDs.Contains(item.ID);
        //        foreach (TreeViewItem item2 in GetTreeViewItemModelChildItems(CheckedNodesIDs, All, item.ID))
        //        {
        //            tm.Items.Add(item2);
        //        }
        //        l.Add(tm);
        //    }
        //    return l;
        //}

        //public static string SetSearchTagsIDs(string Model, string SearchSegmentName)
        //{
        //    if (string.IsNullOrEmpty(Model)) return string.Empty;
        //    string OutString = "";

        //    List<SearchModel> All = new List<SearchModel>();
        //    string LangCode = SF.GetLangCode();
        //    All = SF.GetSearchModelRepository().Where(r => r.SearchSegmentName == SearchSegmentName).ToList();

        //    foreach (SearchModel item in All.Where(r => Model.Contains(r.ShopProductTitle)))
        //    {
        //        if (!OutString.Contains(item.ID.ToString())) OutString = OutString + "," + item.ID;
        //    }

        //    return OutString;
        //}

        //public static List<int> SetSearchTagsIDs(string Model, string SearchSegmentName, bool i)
        //{
        //    if (string.IsNullOrEmpty(Model)) return new List<int>();
        //    List<int> l = new List<int>();

        //    List<SearchModel> All = new List<SearchModel>();
        //    string LangCode = SF.GetLangCode();
        //    All = SF.GetSearchModelRepository().Where(r => r.SearchSegmentName == SearchSegmentName).ToList();

        //    foreach (SearchModel item in All.Where(r => Model.Contains(r.ShopProductTitle)))
        //    {
        //        if (!l.Contains(item.ID)) l.Add(item.ID);
        //    }

        //    return l;
        //}


        //public static string SetTreeViewItemModel(string Model)
        //{
        //    if (string.IsNullOrEmpty(Model) || Model.Split('|').Length != 2) return string.Empty;
        //    string SearchSegmentName = Model.Split('|')[0];
        //    string SearchIDs = Model.Split('|')[1];
        //    return SearchIDs;
        //}

        //public static string SetTreeViewItemModelNames(string Model)
        //{
        //    if (string.IsNullOrEmpty(Model) || Model.Split('|').Length != 2) return string.Empty;
        //    string SearchSegmentName = Model.Split('|')[0];
        //    string SearchIDs = Model.Split('|')[1];

        //    string OutString = ",";

        //    List<SearchModel> All = new List<SearchModel>();
        //    string LangCode = SF.GetLangCode();
        //    All = SF.GetSearchModelRepository().Where(r => r.SearchSegmentName == SearchSegmentName).ToList();

        //    List<int> CheckedNodesIDs = GetListIntFromString(SearchIDs);

        //    foreach (SearchModel item in All.Where(r => CheckedNodesIDs.Contains(r.ID)))
        //    {
        //        if (!OutString.Contains("," + item.ShopProductTitle + ",")) OutString = OutString + item.ShopProductTitle + ",";
        //        if (item.ShopParentID != 0)
        //        {
        //            SearchModel p = All.FirstOrDefault(r => r.ID == item.ShopParentID);
        //            if (p != null && !OutString.Contains("," + p.ShopProductTitle + ",")) OutString = OutString + p.ShopProductTitle + ",";
        //        }
        //    }

        //    return OutString;
        //}

        //public static string SetTreeViewItemModelIDs(string Model)
        //{
        //    string OutString = ",";
        //    if (string.IsNullOrEmpty(Model)) return OutString;

        //    List<SearchModel> All = SF.GetSearchModelRepository();
        //    List<string> l = Model.Split(',').ToList();

        //    foreach (string item in l)
        //    {
        //        SearchModel sm = All.FirstOrDefault(r => r.ShopProductTitle == item);
        //        if (sm == null) continue;

        //        if (!OutString.Contains("," + sm.ID + ",")) OutString = OutString + sm.ID + ",";
        //    }

        //    return OutString;
        //}

        //public static List<Telerik.Web.Mvc.UI.DropDownItem> GetDropDownItemModel(string Model)
        //{
        //    if (string.IsNullOrEmpty(Model) || !Model.Contains("|")) return new List<Telerik.Web.Mvc.UI.DropDownItem>();
        //    string SearchSegmentName = Model.Split('|')[0];
        //    string SearchID = Model.Split('|')[1];

        //    List<DropDownItem> l = new List<DropDownItem>();
        //    foreach (SearchModel item in SF.GetSearchModelRepository().Where(r => r.SearchSegmentName == SearchSegmentName && r.ShopParentID == 0))
        //    {
        //        DropDownItem tm = new DropDownItem();
        //        tm.Value = item.ID.ToString();
        //        tm.ShopText = item.ShopProductTitle;
        //        if (item.ID.ToString() == SearchID) tm.Selected = true;
        //        l.Add(tm);
        //    }
        //    return l;
        //}

        //public static string SetDropDownItemModel(string Model)
        //{
        //    int ID = 0;
        //    int.TryParse(Model, out ID);
        //    if (ID == 0) return "";
        //    SearchModel sm = SF.GetSearchModelRepository().FirstOrDefault(r => r.ID == ID);
        //    if (sm == null) return "|";
        //    else return sm.ID.ToString() + "|" + sm.ShopProductTitle;
        //}


        //public static string GetSearchTextByID(int ID)
        //{
        //    SearchModel sm = SF.GetSearchModelRepository().FirstOrDefault(r => r.ID == ID);
        //    if (sm == null) return string.Empty;
        //    else return sm.ShopProductTitle;
        //}

        #endregion

        #region PageModel

        //public static List<CustumMenuItem> GetSearchPageModelRepository(int CategoryAllPageID)
        //{
        //    List<CustumMenuItem> All = new List<CustumMenuItem>();
        //    All.AddRange(GetSearchPageModelRepositoryChild(CategoryAllPageID));
        //    return All;
        //}
        //private static List<CustumMenuItem> GetSearchPageModelRepositoryChild(int CategoryAllPageID)
        //{
        //    List<CustumMenuItem> All = new List<CustumMenuItem>();
        //    foreach (CustumMenuItem item in SF.GetAdminMenuRepository().Where(r => r.ShopParentID == CategoryAllPageID))
        //    {
        //        All.Add(item);
        //        All.AddRange(GetSearchPageModelRepositoryChild(item.ID));
        //    }
        //    return All;
        //}

        //public static List<Telerik.Web.Mvc.UI.TreeViewItem> GetTreeViewItemPageModel(string Model)
        //{
        //    CustumMenuItem parent = SF.GetAdminMenuRepository().FirstOrDefault(r => r.RouteUrl == "ktl");
        //    if (parent == null) return new List<Telerik.Web.Mvc.UI.TreeViewItem>();
        //    int CategoryAllPageID = parent.ID;
        //    List<int> CheckedNodesIDs = GetListIntFromString(Model);
        //    List<CustumMenuItem> All = SF.GetSearchPageModelRepository(CategoryAllPageID);

        //    List<TreeViewItem> l = new List<TreeViewItem>();
        //    foreach (CustumMenuItem item in All.Where(r => r.ShopParentID == CategoryAllPageID))
        //    {
        //        TreeViewItem tm = new TreeViewItem();
        //        tm.Value = item.ID.ToString();
        //        tm.ShopText = item.ShopProductTitle;
        //        tm.Enabled = true;
        //        //tm.Expanded = true;
        //        tm.Checked = CheckedNodesIDs.Contains(item.ID);
        //        foreach (TreeViewItem item2 in GetTreeViewItemPageModelChildItems(CheckedNodesIDs, All, item.ID))
        //        {
        //            tm.Items.Add(item2);
        //        }
        //        l.Add(tm);
        //    }
        //    return l;
        //}

        //public static List<TreeViewItem> GetTreeViewItemPageModelChildItems(List<int> CheckedNodesIDs, List<CustumMenuItem> All, int ShopParentID)
        //{
        //    List<TreeViewItem> items = new List<TreeViewItem>();
        //    foreach (CustumMenuItem item in All.Where(r => r.ShopParentID == ShopParentID))
        //    {
        //        TreeViewItem tm = new TreeViewItem();
        //        tm.Value = item.ID.ToString();
        //        tm.ShopText = item.ShopProductTitle;
        //        tm.Enabled = true;
        //        //tm.Expanded = true;
        //        tm.Checked = CheckedNodesIDs.Contains(item.ID);
        //        foreach (TreeViewItem item2 in GetTreeViewItemPageModelChildItems(CheckedNodesIDs, All, item.ID))
        //        {
        //            tm.Items.Add(item2);
        //        }
        //        items.Add(tm);
        //    }
        //    return items;
        //}

        //public static string SetTreeViewItemPageModelNames(string Model)
        //{
        //    string OutString = ",";
        //    if (string.IsNullOrEmpty(Model)) return OutString;

        //    CustumMenuItem parent = SF.GetAdminMenuRepository().FirstOrDefault(r => r.RouteUrl == "ktl");
        //    if (parent == null) return OutString;
        //    int CategoryAllPageID = parent.ID;

        //    List<CustumMenuItem> All = new List<CustumMenuItem>();
        //    All = GetSearchPageModelRepository(CategoryAllPageID);

        //    List<int> CheckedNodesIDs = GetListIntFromString(Model);

        //    foreach (CustumMenuItem item in All.Where(r => CheckedNodesIDs.Contains(r.ID)))
        //    {
        //        if (!OutString.Contains("," + item.ShopProductTitle + ",")) OutString = OutString + item.ShopProductTitle + ",";
        //        if (item.ShopParentID != 0)
        //        {
        //            CustumMenuItem p = All.FirstOrDefault(r => r.ID == item.ShopParentID);
        //            if (p != null && !OutString.Contains("," + p.ShopProductTitle + ",")) OutString = OutString + p.ShopProductTitle + ",";
        //        }
        //    }

        //    return OutString;
        //}

        //public static string SetTreeViewItemPageModelIDs(string Model)
        //{
        //    string OutString = ",";
        //    if (string.IsNullOrEmpty(Model)) return OutString;

        //    CustumMenuItem parent = SF.GetAdminMenuRepository().FirstOrDefault(r => r.RouteUrl == "ktl");
        //    if (parent == null) return OutString;
        //    int CategoryAllPageID = parent.ID;

        //    List<CustumMenuItem> All = new List<CustumMenuItem>();
        //    All = GetSearchPageModelRepository(CategoryAllPageID);

        //    List<int> CheckedNodesIDs = GetListIntFromString(Model);

        //    foreach (CustumMenuItem item in All.Where(r => CheckedNodesIDs.Contains(r.ID)))
        //    {
        //        if (!OutString.Contains("," + item.ID + ",")) OutString = OutString + item.ID + ",";
        //        if (item.ShopParentID != 0)
        //        {
        //            CustumMenuItem p = All.FirstOrDefault(r => r.ID == item.ShopParentID);
        //            if (p != null && !OutString.Contains("," + p.ID + ",")) OutString = OutString + p.ID + ",";
        //        }
        //    }

        //    return OutString;
        //}

        #endregion

        #region ShopOther

        //public static List<string> GetGoogleSuggestions(string text)
        //{
        //    List<string> l = new List<string>();
        //    XmlDocument doc = new XmlDocument();
        //    string url = "http://google.co.il/complete/search?output=toolbar&q=" + text;

        //    String strResult;
        //    System.Net.WebResponse objResponse;

        //    System.Net.WebRequest objRequest = System.Net.HttpWebRequest.Create(url);

        //    //Pretend to be IE7
        //    ((System.Net.HttpWebRequest)objRequest).UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";
        //    ((System.Net.HttpWebRequest)objRequest).Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
        //    System.Net.CookieContainer cc = new System.Net.CookieContainer();
        //    System.Net.Cookie cookie = new System.Net.Cookie("NID", "45=SXfhlhu1_OGt30xIS9k5u7RXI6lOAMDZT0M34XmrGDAhMKqqkuXrAYFDaWY-qcjg6ujFnarij4GpERCCCxa1g1ApeVyzmsKXDLdtaTLjKX6BmAL-32hTsT1fCNKkJCL8");
        //    cc.Add(new Uri("http://google.co.il"), cookie);
        //    cookie = new System.Net.Cookie("PREF", "ID=f49dd8f1e5dbf90f:U=4fea453c6fa4917d:FF=0:TM=1302338006:LM=1302338006:S=m9L0N4QmX1wxlLF0");
        //    cc.Add(new Uri("http://google.co.il"), cookie);
        //    cookie = new System.Net.Cookie("GALX", "Ncr7yxM0Zkc");
        //    cc.Add(new Uri("http://google.co.il"), cookie);
        //    ((System.Net.HttpWebRequest)objRequest).CookieContainer = cc;

        //    objResponse = objRequest.GetResponse();
        //    using (System.IO.StreamReader sr = new System.IO.StreamReader(objResponse.GetResponseStream()))
        //    {
        //        strResult = sr.ReadToEnd();
        //        sr.Close();
        //    }

        //    doc.LoadXml(strResult);
        //    foreach (XmlNode node in doc.SelectNodes("//CompleteSuggestion"))
        //    {
        //        string value = node.SelectSingleNode("suggestion/@data").InnerText;
        //        l.Add(value);
        //    }
        //    return l;
        //}

        //public static string ProcessSearchMeta(string MetaString, string ID, bool Selected, string SelectedValue)
        //{
        //    if (string.IsNullOrEmpty(MetaString)) return "";
        //    string OutString = "";
        //    if (Selected)
        //    {
        //        //replace [1] text by value
        //        OutString = MetaString.Replace("[" + ID + "]", SelectedValue);
        //        //remove wraper (<+1></+1>) <+1>in</+1> text when drop down selected
        //        OutString = RemovePlusWraper(OutString, ID);
        //        //remove all <-1>in</-1> text when drop down selected
        //        OutString = RemoveAllMinus(OutString, ID);
        //    }
        //    else
        //    {
        //        //remove [1] text
        //        OutString = MetaString.Replace("[" + ID + "]", "");
        //        //remove wraper (<-1></-1>) <-1>in</-1> text when drop down not selected
        //        OutString = RemoveMinusWraper(OutString, ID);
        //        //remove all <+1>in</+1> text when drop down not selected
        //        OutString = RemoveAllPlus(OutString, ID);

        //    }
        //    return OutString;
        //}

        //public static string ClearDoubleSpace(string s)
        //{
        //    s = s.Replace("  ", " ");
        //    s = s.Replace("  ", " ");
        //    return s;
        //}

        #endregion

        #region Private

        //private static List<TreeViewItem> GetTreeViewItemModelChildItems(List<int> CheckedNodesIDs, List<SearchModel> All, int ShopParentID)
        //{
        //    List<TreeViewItem> items = new List<TreeViewItem>();
        //    foreach (SearchModel item in All.Where(r => r.ShopParentID == ShopParentID))
        //    {
        //        TreeViewItem tm = new TreeViewItem();
        //        tm.Value = item.ID.ToString();
        //        tm.ShopText = item.ShopProductTitle;
        //        tm.Enabled = true;
        //        //tm.Expanded = true;
        //        tm.Checked = CheckedNodesIDs.Contains(item.ID);
        //        foreach (TreeViewItem item2 in GetTreeViewItemModelChildItems(CheckedNodesIDs, All, item.ID))
        //        {
        //            tm.Items.Add(item2);
        //        }
        //        items.Add(tm);
        //    }
        //    return items;
        //}

        //remove wraper (<+1></+1>) <+1>in</+1> text
        //private static string RemovePlusWraper(string ShopText, string TextId)
        //{
        //    ShopText = ShopText.Replace("<+" + TextId + ">", "");
        //    ShopText = ShopText.Replace("</+" + TextId + ">", "");
        //    return ShopText;
        //}

        ////remove wraper (<-1></-1>) <-1>in</-1> text
        //private static string RemoveMinusWraper(string ShopText, string TextId)
        //{
        //    ShopText = ShopText.Replace("<-" + TextId + ">", "");
        //    ShopText = ShopText.Replace("</-" + TextId + ">", "");
        //    return ShopText;
        //}

        //remove all <+1>in</+1> text
        //private static string RemoveAllPlus(string ShopText, string TextId)
        //{
        //    int i = 8;
        //    while (ShopText.IndexOf("<+" + TextId + ">") > 0 && i != 0)
        //    {
        //        int StartIndex = ShopText.IndexOf("<+" + TextId + ">");
        //        int EndIndex = ShopText.IndexOf("</+" + TextId + ">");
        //        if (ShopText.Length > 0 && StartIndex >= 0 && EndIndex > 0 && EndIndex > StartIndex) ShopText = ShopText.Remove(StartIndex, EndIndex - StartIndex + 4 + TextId.Length);
        //        i--;
        //    }
        //    return ShopText;
        //}

        //remove all <-1>in</-1> text
        //private static string RemoveAllMinus(string ShopText, string TextId)
        //{
        //    int i = 8;
        //    while (ShopText.IndexOf("<-" + TextId + ">") > 0 && i != 0)
        //    {
        //        int StartIndex = ShopText.IndexOf("<-" + TextId + ">");
        //        int EndIndex = ShopText.IndexOf("</-" + TextId + ">");
        //        if (ShopText.Length > 0 && StartIndex >= 0 && EndIndex > 0 && EndIndex > StartIndex) ShopText = ShopText.Remove(StartIndex, EndIndex - StartIndex + 4 + TextId.Length);
        //        i--;
        //    }
        //    return ShopText;
        //}

        #endregion














        public static List<ShopCartItem> GetCart()
        {
            if (HttpContext.Current.Session["MenuItems"] == null)
            {
                HttpContext.Current.Session["MenuItems"] = new List<ShopCartItem>();
            }
            return HttpContext.Current.Session["MenuItems"] as List<ShopCartItem>;
        }

        public static ShopProductPage GetProduct(int ID)
        {
            Settings s = RP.GetCurrentSettings();
            using (Db _db = new Db())
            {
                return _db.ShopProductPages.FirstOrDefault(r => r.ID == ID && r.DomainID == s.ID && r.ShopPrice > 0 && r.ShopShowInStock);
            }
        }

        public static void SetCart(List<ShopCartItem> cart)
        {
            HttpContext.Current.Session["MenuItems"] = cart;
        }

        public static void ClearCart()
        {
            HttpContext.Current.Session["MenuItems"] = null;
        }

        public static List<ShopCartItem> DeleteItemFromCart(int ID)
        {
            List<ShopCartItem> cart = GetCart();
            cart.RemoveAll(r => r.ID == ID);
            SetCart(cart);
            return cart;
        }

        public static List<ShopCartItem> AddItemToCart(int ID, int Quantity)
        {
            List<ShopCartItem> cart = GetCart();
            List<int> rl = new List<int>();
            Settings s = RP.GetCurrentSettings();

            using (Db _db = new Db())
            {
                ShopProductPage d = _db.ShopProductPages.FirstOrDefault(r => r.ID == ID && r.DomainID == s.ID && r.ShopPrice > 0 && r.ShopShowInStock);
                if (d == null) return cart;

                if (cart.Count(r => r.ID == ID) > 0)
                {
                    ShopCartItem ci = cart.FirstOrDefault(r => r.ID == ID);
                    ci.ShopQuantity = ci.ShopQuantity + Quantity;
                    if (ci.ShopQuantity <= 0) cart.RemoveAll(r => r.ID == ID);
                }
                else if (Quantity > 0)
                {
                    cart.Add(new ShopCartItem() { ID = d.ID, ShopPrice = d.ShopPrice, ShopQuantity = Quantity, ShopProductTitle = d.Title, ShopPic = d.Pic, ShopUrl = d.Url, ShopPriceShipping = d.ShopAddShippingPrice });
                }
                else return cart;
            }
            SetCart(cart);
            return cart;
        }

        public static List<ShopCartItem> UpdatePricesInCart()
        {
            List<ShopCartItem> cart = GetCart();
            List<int> rl = new List<int>();
            Settings s = RP.GetCurrentSettings();

            using (Db _db = new Db())
            {
                foreach (ShopCartItem item in cart)
                {
                    ShopProductPage d = _db.ShopProductPages.FirstOrDefault(r => r.ID == item.ID && r.DomainID == s.ID && r.ShopPrice > 0 && r.ShopShowInStock);
                    if (d == null)
                    {
                        rl.Add(item.ID);
                    }
                    item.ShopPrice = d.ShopPrice;
                    item.ShopProductTitle = d.Title;
                    item.ShopPriceShipping = d.ShopAddShippingPrice;
                }
                cart.RemoveAll(r => rl.Contains(r.ID));
            }
            SetCart(cart);
            return cart;
        }

        public static decimal GetCartTotal()
        {
            return SF.GetCart().Sum(r => r.ShopPrice * r.ShopQuantity);
        }

        public static int GetCartItemsNum()
        {
            return SF.GetCart().Count();
        }

        public static List<string> GetRelatedProducts()
        {
            int DomainID = RP.GetAdminCurrentSettingsRepository().ID;
            return _db.ShopProductPages.Where(r => r.DomainID == DomainID).Select(r => r.Title).OrderBy(r => r).ToList();
        }

        public static List<string> GetBrand()
        {
            int DomainID = RP.GetAdminCurrentSettingsRepository().ID;
            return _db.ShopBrandPages.Where(r => r.DomainID == DomainID).Select(r => r.Title).OrderBy(r => r).ToList();
        }

        public static ShopCategoryAllPage ShopGetCategoryAllPage()
        {
            int DomainID = RP.GetAdminCurrentSettingsRepository().ID;
            int DomainPageID = RP.GetAdminCurrentSettingsRepository().DomainPageID;
            return ShopGetCategoryAllPage(DomainID, DomainPageID);
        }

        public static ShopCategoryAllPage ShopGetCategoryAllPage(int DomainID, int DomainPageID)
        {
            using (Db _db = new Db())
            {
                ShopCategoryAllPage ap = _db.ShopCategoryAllPages.FirstOrDefault(r => r.DomainID == DomainID);
                if (ap != null) return ap;

                ap = new ShopCategoryAllPage();
                ap.ParentID = DomainPageID;
                ap.DomainID = DomainID;
                ap.SeoUrlName = "קטגוריות";
                ap.Title = "קטגוריות";

                if (_db.AbstractPages.Count(r => r.DomainID == ap.DomainID && r.RouteUrl == ap.RouteUrl && r.SeoUrlName == ap.SeoUrlName) != 0)
                {
                    ap.SeoUrlName = (_db.AbstractPages.Max(r => r.ID) + 1).ToString();
                }
                ap.CreateTime = DateTime.Now;
                if (_db.AbstractPages.Where(r => r.DomainID == ap.DomainID && r.ParentID == ap.ParentID).Count() == 0) ap.Order = 1;
                else ap.Order = _db.AbstractPages.Where(r => r.DomainID == ap.DomainID && r.ParentID == ap.ParentID).Max(r => r.Order) + 1;

                _db.ShopCategoryAllPages.Add(ap);
                _db.SaveChanges();

                return ap;
            }
        }

        public static AbstractPage ShopGetProductAllPage()
        {
            int DomainID = RP.GetAdminCurrentSettingsRepository().ID;
            int DomainPageID = RP.GetAdminCurrentSettingsRepository().DomainPageID;
            return ShopGetProductAllPage(DomainID, DomainPageID);
        }

        public static AbstractPage ShopGetProductAllPage(int DomainID, int DomainPageID)
        {
            using (Db _db = new Db())
            {
                ShopProductAllPage ap = _db.ShopProductAllPages.FirstOrDefault(r => r.DomainID == DomainID);
                if (ap != null) return ap;

                ap = new ShopProductAllPage();
                ap.ParentID = DomainPageID;
                ap.DomainID = DomainID;
                ap.SeoUrlName = "מוצרים";
                ap.Title = "מוצרים";

                if (_db.AbstractPages.Count(r => r.DomainID == ap.DomainID && r.RouteUrl == ap.RouteUrl && r.SeoUrlName == ap.SeoUrlName) != 0)
                {
                    ap.SeoUrlName = (_db.AbstractPages.Max(r => r.ID) + 1).ToString();
                }
                ap.CreateTime = DateTime.Now;
                if (_db.AbstractPages.Where(r => r.DomainID == ap.DomainID && r.ParentID == ap.ParentID).Count() == 0) ap.Order = 1;
                else ap.Order = _db.AbstractPages.Where(r => r.DomainID == ap.DomainID && r.ParentID == ap.ParentID).Max(r => r.Order) + 1;

                _db.ShopProductAllPages.Add(ap);
                _db.SaveChanges();

                return ap;
            }
        }

        public static AbstractPage ShopGetBrandAllPage()
        {
            int DomainID = RP.GetAdminCurrentSettingsRepository().ID;
            int DomainPageID = RP.GetAdminCurrentSettingsRepository().DomainPageID;
            return ShopGetBrandAllPage(DomainID, DomainPageID);
        }

        public static AbstractPage ShopGetBrandAllPage(int DomainID, int DomainPageID)
        {
            using (Db _db = new Db())
            {
                ShopBrandAllPage ap = _db.ShopBrandAllPages.FirstOrDefault(r => r.DomainID == DomainID);
                if (ap != null) return ap;

                ap = new ShopBrandAllPage();
                ap.ParentID = DomainPageID;
                ap.DomainID = DomainID;
                ap.SeoUrlName = "מותגים";
                ap.Title = "מותגים";

                if (_db.AbstractPages.Count(r => r.DomainID == ap.DomainID && r.RouteUrl == ap.RouteUrl && r.SeoUrlName == ap.SeoUrlName) != 0)
                {
                    ap.SeoUrlName = (_db.AbstractPages.Max(r => r.ID) + 1).ToString();
                }
                ap.CreateTime = DateTime.Now;
                if (_db.AbstractPages.Where(r => r.DomainID == ap.DomainID && r.ParentID == ap.ParentID).Count() == 0) ap.Order = 1;
                else ap.Order = _db.AbstractPages.Where(r => r.DomainID == ap.DomainID && r.ParentID == ap.ParentID).Max(r => r.Order) + 1;

                _db.ShopBrandAllPages.Add(ap);
                _db.SaveChanges();

                return ap;
            }
        }

        public static void AddToOrderLog(int OrderID, string Log)
        {
            using (Db _db = new Db())
            {
                ShopOrder o = _db.ShopOrders.FirstOrDefault(r => r.ID == OrderID);
                if (o != null)
                {
                    o.ShopLog = new StringBuilder().AppendFormat("{0}\n{1} - {2}", o.ShopLog, DateTime.Now.ToString(), Log).ToString();
                    _db.Entry(o).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
        }

        public static string GetStatusName(ShopOrderStatusEnum en)
        {
            if (en == ShopOrderStatusEnum.Placed) return "נרשמה";
            if (en == ShopOrderStatusEnum.Payed) return "שולמה";
            if (en == ShopOrderStatusEnum.Shipped) return "נשלחה";
            if (en == ShopOrderStatusEnum.Received) return "התקבלה";
            if (en == ShopOrderStatusEnum.Canceled) return "בוטלה";
            return "";
        }

        public static string GetPayTypeName(ShopPayTypeEnum en)
        {
            if (en == ShopPayTypeEnum.CreditGuard) return "אשראי CreditGuard";
            if (en == ShopPayTypeEnum.Instore) return "בחנות";
            if (en == ShopPayTypeEnum.NoPayment) return "לא שולם";
            if (en == ShopPayTypeEnum.Paypal) return "Paypal";
            if (en == ShopPayTypeEnum.Phone) return "טלפון";
            if (en == ShopPayTypeEnum.ZCredit) return "אשראי ZCredit";
            return "";
        }

        public static string GetShippingTypeName(ShopShippingTypeEnum en)
        {
            if (en == ShopShippingTypeEnum.NoShipment) return "ללא משלוח";
            if (en == ShopShippingTypeEnum.Self) return "איסוף אצמי";
            if (en == ShopShippingTypeEnum.Shipment) return "משלוח";
            return "";
        }

        public static string HMACSHA256HashString(string stringToHash)
        {
            string intercomApiSecret = "MxQ1kEZavs1d8lSC5a3l6UJQtasdvcOSs3VWu3a0";
            byte[] secretKey = Encoding.UTF8.GetBytes(intercomApiSecret);
            byte[] bytes = Encoding.UTF8.GetBytes(stringToHash);
            using (var hmac = new HMACSHA256(secretKey))
            {
                hmac.ComputeHash(bytes);
                byte[] data = hmac.Hash;

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }



















        public static string SendInvoice(ShopOrder o)
        {
            return string.Empty;

            using (Db _db = new Db())
            {
                Uco.Models.Settings s = _db.SettingsAll.FirstOrDefault();
                if (!s.ShopInvoiceUse || string.IsNullOrEmpty(s.ShopInvoiceUser) || string.IsNullOrEmpty(s.ShopInvoiceKey)) return "לא ניתן לשלוח חשבונית. בדוק הגדרות.";
                if (!o.ShopInvoiceSent)
                {
                    //string Product = "מוצרים";
                    //if (HttpContext.Current != null)
                    //{
                    //    Product = "מוצרים - " + HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"];
                    //}

                    //invoice4u.account.w_invoicereceipt invoiceService = new invoice4u.account.w_invoicereceipt();
                    //DataSet d0 = invoiceService.CREATE102(
                    //    "",
                    //    s.ShopInvoiceUser,
                    //    s.ShopInvoiceKey,
                    //    Product,
                    //    "0",
                    //    "0",
                    //    o.ID.ToString(),
                    //    Product,
                    //    "1",
                    //    (o.ShopTotal).ToString(),
                    //    "",
                    //    "",
                    //    "99999",
                    //    (string.IsNullOrEmpty(o.ShopCompanyName) ? o.ShopFirstName + " " + o.ShopLastName : o.ShopCompanyName),
                    //    o.ShopEmail,
                    //    "1");

                    //string ResponseCode = "";
                    //string DocNumber = "";
                    //string DocURL = "";

                    //for (int d2i = 0; d2i <= d0.Tables[0].Rows.Count - 1; d2i++)
                    //{
                    //    if (d0.Tables[0].Rows[d2i][0].ToString() == "ResponseCode") ResponseCode = d0.Tables[0].Rows[d2i][1].ToString();
                    //    if (d0.Tables[0].Rows[d2i][0].ToString() == "DocNumber") DocNumber = d0.Tables[0].Rows[d2i][1].ToString();
                    //    if (d0.Tables[0].Rows[d2i][0].ToString() == "DocURL") DocURL = d0.Tables[0].Rows[d2i][1].ToString();
                    //}

                    //if (ResponseCode != "100")
                    //{
                    //    o.ShopLog = o.ShopLog + DateTime.Now + " - Error sending invoice: " + ResponseCode + "\r\n";
                    //    _db.Entry(s).State = EntityState.Modified;
                    //    _db.SaveChanges();
                    //    return "חשבונית לא נשלחה. שגיאה מספר " + ResponseCode + ". הסבר - http://www.invoice4u.co.il/site/documentation/documentation.aspx?responsecodes.asp";
                    //}
                    //else
                    //{
                    //    o.ShopLog = o.ShopLog + DateTime.Now + " - חשבונית נשלחה\r\n";
                    //    o.ShopInvoiceSent = true;
                    //    o.ShopInvoiceID = DocNumber;
                    //    o.ShopInvoiceURL = DocURL;
                    //    _db.Entry(o).State = EntityState.Modified;
                    //    _db.SaveChanges();
                    //}

                    //return "חשבונית נשלחה";
                }
                else
                {
                    //invoice4u.account.w_invoicereceipt invoiceService = new invoice4u.account.w_invoicereceipt();
                    //DataSet d0 = invoiceService.SEND(
                    //    s.ShopInvoiceUser,
                    //    s.ShopInvoiceKey,
                    //    o.ShopInvoiceID,
                    //    o.ShopEmail);


                    //DataRowCollection dtr = d0.Tables[0].Rows;
                    //object ReaponceCodeOb = dtr[0]["Value"];
                    //int ReaponceCode = int.Parse(ReaponceCodeOb.ToString());

                    //if (ReaponceCode == 100)
                    //{
                    //    o.ShopLog = o.ShopLog + DateTime.Now + " - Sending invoice: " + ReaponceCode + "\r\n";
                    //    _db.Entry(s).State = EntityState.Modified;
                    //    _db.SaveChanges();
                    //}
                    //else
                    //{
                    //    o.ShopLog = o.ShopLog + DateTime.Now + " - Error sending invoice: " + ReaponceCode + "\r\n";
                    //    _db.Entry(s).State = EntityState.Modified;
                    //    _db.SaveChanges();
                    //    return "חשבונית לא נשלחה. שגיאה מספר " + ReaponceCode + ". הסבר - http://www.invoice4u.co.il/site/documentation/documentation.aspx?responsecodes.asp";
                    //}


                    //return "חשבונית נשלחה בעבר. שלחנו שוב.";
                }
            }
        }

        public static List<Kendo.Mvc.UI.TreeViewItemModel> GetTreeViewItemModel(string Model)
        {
            if (string.IsNullOrEmpty(Model) || !Model.Contains("|")) return new List<Kendo.Mvc.UI.TreeViewItemModel>();
            string SearchSegmentName = Model.Split('|')[0];
            string SearchIDs = Model.Split('|')[1];
            List<int> CheckedNodesIDs = SF.GetListIntFromString(Model);
            List<ShopCategoryPage> All = new List<ShopCategoryPage>();
            All = RP.GetCategoryPageReprository().Where(r => r.Visible).ToList();

            List<TreeViewItemModel> l = new List<TreeViewItemModel>();
            foreach (ShopCategoryPage item in All.Where(x => !All.Exists(k => x.ParentID == k.ID))) //.Where(r => r.ParentID == 0)
            {
                TreeViewItemModel tm = new TreeViewItemModel();
                tm.Id = item.ID.ToString();
                tm.Text = item.Title;
                tm.Enabled = true;
                //tm.Expanded = true;
                tm.Checked = CheckedNodesIDs.Contains(item.ID);
                foreach (TreeViewItemModel item2 in GetTreeViewItemModelChildItems(CheckedNodesIDs, All, item.ID))
                {
                    tm.Items.Add(item2);
                }
                l.Add(tm);
            }
            return l;
        }

       

        private static List<TreeViewItemModel> GetTreeViewItemModelChildItems(List<int> CheckedNodesIDs, List<ShopCategoryPage> All, int ParentID)
        {
            List<TreeViewItemModel> items = new List<TreeViewItemModel>();
            foreach (ShopCategoryPage item in All.Where(r => r.ParentID == ParentID))
            {
                TreeViewItemModel tm = new TreeViewItemModel();
                tm.Id = item.ID.ToString();
                tm.Text = item.Title;
                tm.Enabled = true;
                //tm.Expanded = true;
                tm.Checked = CheckedNodesIDs.Contains(item.ID);
                foreach (TreeViewItemModel item2 in GetTreeViewItemModelChildItems(CheckedNodesIDs, All, item.ID))
                {
                    tm.Items.Add(item2);
                }
                items.Add(tm);
            }
            return items;
        }

        public static string SetTreeViewItemModel(string Model)
        {
            if (string.IsNullOrEmpty(Model) || Model.Split('|').Length != 2) return string.Empty;
            string SearchSegmentName = Model.Split('|')[0];
            string SearchIDs = Model.Split('|')[1];

            string OutString = SearchIDs;
            List<ShopCategoryPage> All = new List<ShopCategoryPage>();
            All = RP.GetCategoryPageReprository().Where(r => r.Visible).ToList();

            List<int> CheckedNodesIDs = SF.GetListIntFromString(SearchIDs);

            foreach (ShopCategoryPage item in All.Where(r => CheckedNodesIDs.Contains(r.ID)))
            {

                if (!OutString.Contains("," + item.ID + ",")) OutString = OutString + item.ID + ",";
                if (item.ParentID != 0)
                {
                    OutString = RecursiveMenuItemModelIDs(item, OutString, All);
                }
            }

            return OutString;
        }

       

        private static string RecursiveMenuItemModelIDs(ShopCategoryPage item, string OutString, List<ShopCategoryPage> All)
        {
            ShopCategoryPage p = All.FirstOrDefault(r => r.ID == item.ParentID);
            if (p != null && !OutString.Contains("," + p.ID + ",")) OutString = OutString + p.ID + ",";
            if (p == null) return OutString;
            if (p.ParentID != 0 && !OutString.Contains("," + p.ParentID + ",")) //stack overflow, recursive check!!
            {
                OutString = RecursiveMenuItemModelIDs(p, OutString, All);
            }
            return OutString;
        }

        public static string SetTreeViewItemModelNames(string Model)
        {
            if (string.IsNullOrEmpty(Model) || Model.Split('|').Length != 2) return string.Empty;
            string SearchSegmentName = Model.Split('|')[0];
            string SearchIDs = Model.Split('|')[1];

            string OutString = ",";

            List<ShopCategoryPage> All = new List<ShopCategoryPage>();
            string LangCode = Uco.Infrastructure.SF.GetLangCodeThreading();
            All = RP.GetCategoryPageReprository().Where(r => r.Visible).ToList();

            List<int> CheckedNodesIDs = new List<int>();
            var arr = SearchIDs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr != null)
            {
                foreach (var inn in arr)
                {
                    if (!string.IsNullOrEmpty(inn))
                    {
                        int testI = 0;
                        //Don`t change!! in admin editor we get bug, and result return int, but need string list
                        var update = inn;
                        if (int.TryParse(inn, out testI))
                        {
                            var testS = All.FirstOrDefault(x => x.ID == testI);
                            if (testS != null)
                            {
                                update = testS.Title;
                            }
                        }

                        if (!OutString.Contains("," + update + ",")) OutString = OutString + inn + ",";


                    }
                }
            }
            //foreach (SearchModel item in All.Where(r => CheckedNodesIDs.Contains(r.ID)))
            //{
            //    if (!OutString.Contains("," + item.Title + ",")) OutString = OutString + item.Title + ",";
            //    if (item.ParentID != 0)
            //    {
            //        OutString = RecursiveMenuItemModelNames(item, OutString, All);
            //    }
            //    //if (item.ParentID != 0)
            //    //{
            //    //    SearchModel p = All.FirstOrDefault(r => r.ID == item.ParentID);
            //    //    if (p != null && !OutString.Contains("," + p.Title + ",")) OutString = OutString + p.Title + ",";
            //    //}
            //}

            return OutString;
        }
    }
}