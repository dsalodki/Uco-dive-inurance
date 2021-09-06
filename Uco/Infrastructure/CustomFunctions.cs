using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;
using Uco.Models;

namespace Uco.Infrastructure
{
    public static partial class SF
    {

        public static string GetImageUrl(string Pic, int Width, int Height, bool ExactSize, bool ToCacheFile)
        {
            var path = "~/Image?img=" + HttpUtility.UrlEncode(Pic);
            if (!System.IO.File.Exists(HttpContext.Current.Server.MapPath("~" + Pic)))
            {
                path = "~/Image?img=" + "/Content/DesignFiles/default.png";
            }
            var url = path + "&w=" + Width + "&h=" + Height + "&t=" + (ExactSize ? "1" : "0") + "&c=" + (ToCacheFile ? "1" : "0");
            return url;
        }

        public static string CleanUrl(string UrlToClean)
        {
            if (UrlToClean == null) return "";
            UrlToClean = UrlToClean.Trim();
            UrlToClean = UrlToClean.Replace(" ", "-");
            UrlToClean = UrlToClean.Replace(".", "-");
            UrlToClean = UrlToClean.Replace(",", "-");
            UrlToClean = UrlToClean.Replace("\"", "-");
            UrlToClean = UrlToClean.Replace("'", "-");
            UrlToClean = UrlToClean.Replace(",", "-");
            UrlToClean = UrlToClean.Replace("/", "-");
            UrlToClean = UrlToClean.Replace("\\", "-");
            UrlToClean = UrlToClean.Replace("!", "-");
            UrlToClean = UrlToClean.Replace("@", "-");
            UrlToClean = UrlToClean.Replace("#", "-");
            UrlToClean = UrlToClean.Replace("%", "-");
            UrlToClean = UrlToClean.Replace("^", "-");
            UrlToClean = UrlToClean.Replace("&", "-");
            UrlToClean = UrlToClean.Replace("*", "-");
            UrlToClean = UrlToClean.Replace("(", "-");
            UrlToClean = UrlToClean.Replace(")", "-");
            UrlToClean = UrlToClean.Replace("-", "-");
            UrlToClean = UrlToClean.Replace("+", "-");
            UrlToClean = UrlToClean.Replace("=", "-");
            UrlToClean = UrlToClean.Replace("?", "-");
            UrlToClean = UrlToClean.Replace("<", "-");
            UrlToClean = UrlToClean.Replace(">", "-");
            UrlToClean = UrlToClean.Replace("₪", "-");
            UrlToClean = UrlToClean.Replace("|", "-");

            UrlToClean = UrlToClean.Replace("-----", "-");
            UrlToClean = UrlToClean.Replace("----", "-");
            UrlToClean = UrlToClean.Replace("---", "-");
            UrlToClean = UrlToClean.Replace("--", "-");

            return UrlToClean;
        }

        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public static bool isEmail(string inputEmail)
        {
            inputEmail = inputEmail ?? string.Empty;
            string strRegex = @"^[\w-]+(\.[\w-]+)*@([A-Za-z0-9-]+(\.[A-Za-z0-9-]+)*?\.[A-Za-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        public class AbstractPageInfo
        {
           /* public AbstractPageInfo()
            { }
            public AbstractPageInfo(int ID, int ParentID, string Text, string Title, int Order)
            {
                this.ID = ID;
                this.ParentID = ParentID;
                this.Text = Text;
                this.Title = Title;
                this.Order = Order;
            }*/
            public int ID;
            public int ParentID;
            public string Text;
            public int Order;
            public int Level;
            public string Title;

        }

        public static List<Kendo.Mvc.UI.TreeViewItemModel> GetTreeViewItemModelForAllPages(string Model)
        {
            Settings s = RP.GetAdminCurrentSettingsRepository();

            string Token = "TreeAllPages_" + s.ID;
            if (LS.Cache[Token] != null)
            {
                return (List<Kendo.Mvc.UI.TreeViewItemModel>)LS.Cache[Token];
            }
                
            
            if (string.IsNullOrEmpty(Model) || !Model.Contains("|")) return new List<Kendo.Mvc.UI.TreeViewItemModel>();
            string SearchSegmentName = Model.Split('|')[0];
            string SearchIDs = Model.Split('|')[1];
            List<int> CheckedNodesIDs = SF.GetListIntFromString(Model);
            //Settings s = RP.GetCurrentSettings();
            var All = _db.AbstractPages.Where(r => r.DomainID == s.ID && r.ShowInAdminMenu == true)
                .OrderBy(r => r.Order).ThenBy(r => r.CreateTime)
                /*.Select(x =>  new 
                        {
                            ID = x.ID,
                            Order = x.Order,
                            ParentID = x.ParentID,
                            Text = x.Text,
                            Title = x.Title
                        }).ToList()*/
                 //new Abstrac)
                .Select(x =>
                 new AbstractPageInfo()
                        {
                            ID = x.ID,
                            Order = x.Order,
                            ParentID = x.ParentID,
                            Text = x.Text,
                            Title = x.Title,
                            Level = ((x.RouteUrl == "d" || x.RouteUrl == "l") ? 0 : -1),
                        }
                 //new AbstractPageInfo(x.ID,x.ParentID,x.Text,x.Title,x.Order)
                )
               .ToList();

            List<TreeViewItemModel> l = new List<TreeViewItemModel>();

            AbstractPageInfo domainPage = All.FirstOrDefault(r => r.Level == 0);
            if (domainPage != null)
            {
                domainPage.Level = 0;
                TreeViewItemModel tm = new TreeViewItemModel();
                SetMenuLevel(domainPage, All, CheckedNodesIDs,tm);
                
                tm.Id = domainPage.ID.ToString();
                tm.Text = domainPage.Title;
                tm.Enabled = true;
                tm.Checked = CheckedNodesIDs.Contains(domainPage.ID);

                l.Add(tm);
            }

            All.RemoveAll(r => r.Level == -1);

            /*
            foreach (var item in All.Where(x => !All.Exists(k => x.ParentID == k.ID))) //.Where(r => r.ParentID == 0)
            {
                TreeViewItemModel tm = new TreeViewItemModel();
                tm.Id = item.ID.ToString();
                tm.Text = item.Title;
                tm.Enabled = true;
                tm.Checked = CheckedNodesIDs.Contains(item.ID);
                foreach (TreeViewItemModel item2 in GetTreeViewItemModelForAllPagesChildItems(CheckedNodesIDs, All, item.ID))
                {
                    tm.Items.Add(item2);
                }
                l.Add(tm);
            }*/

            LS.Cache[Token] = l;
            return l;
        }

        private static void SetMenuLevel(AbstractPageInfo item1, List<AbstractPageInfo> All, List<int> CheckedNodesIDs,TreeViewItemModel parentNode)
        {
            foreach (var item in All.Where(r => r.ParentID == item1.ID).OrderBy(r => r.ID))
            {

                TreeViewItemModel tm = new TreeViewItemModel();
                tm.Id = item.ID.ToString();
                tm.Text = item.Title;
                tm.Enabled = true;
                tm.Checked = CheckedNodesIDs.Contains(item.ID);
                //foreach (TreeViewItemModel item2 in GetTreeViewItemModelForAllPagesChildItems(CheckedNodesIDs, All, item.ID))
               // {
                 //   tm.Items.Add(item2);
                //}
                //l.Add(tm);
                item.Level = item1.Level + 1;
                SetMenuLevel(item, All, CheckedNodesIDs, tm);
                parentNode.Items.Add(tm);
            }
        }

        /*

        private static List<TreeViewItemModel> GetTreeViewItemModelForAllPagesChildItems(List<int> CheckedNodesIDs, List<AbstractPageInfo> All, int ParentID)
        {
            List<TreeViewItemModel> items = new List<TreeViewItemModel>();
            foreach (var item in All.Where(r => r.ParentID == ParentID))
            {
                TreeViewItemModel tm = new TreeViewItemModel();
                tm.Id = item.ID.ToString();
                tm.Text = item.Title;
                tm.Enabled = true;
                //tm.Expanded = true;
                tm.Checked = CheckedNodesIDs.Contains(item.ID);
                foreach (TreeViewItemModel item2 in GetTreeViewItemModelForAllPagesChildItems(CheckedNodesIDs, All, item.ID))
                {
                    tm.Items.Add(item2);
                }
                items.Add(tm);
            }
            return items;
        }*/

        public static string SetTreeViewItemModelForAllPages(string Model)
        {
            if (string.IsNullOrEmpty(Model) || Model.Split('|').Length != 2) return string.Empty;
            string SearchSegmentName = Model.Split('|')[0];
            string SearchIDs = Model.Split('|')[1];

            string OutString = SearchIDs;
            var All = new List<AbstractPage>();
            All = RP.GetAllPageReprository().ToList();

            List<int> CheckedNodesIDs = SF.GetListIntFromString(SearchIDs);

            foreach (var item in All.Where(r => CheckedNodesIDs.Contains(r.ID)))
            {

                if (!OutString.Contains("," + item.ID + ",")) OutString = OutString + item.ID + ",";
                if (item.ParentID != 0)
                {
                    OutString = RecursiveMenuItemModelIDs(item, OutString, All);
                }
            }

            return OutString;
        }



        private static string RecursiveMenuItemModelIDs(AbstractPage item, string OutString, List<AbstractPage> All)
        {
            var p = All.FirstOrDefault(r => r.ID == item.ParentID);
            if (p != null && !OutString.Contains("," + p.ID + ",")) OutString = OutString + p.ID + ",";
            if (p == null) return OutString;
            if (p.ParentID != 0 && !OutString.Contains("," + p.ParentID + ",")) //stack overflow, recursive check!!
            {
                OutString = RecursiveMenuItemModelIDs(p, OutString, All);
            }
            return OutString;
        }
    }
}