using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Uco.Infrastructure;
using Uco.Models;

namespace Uco.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    public class MagazineController : BaseAdminController
    {
        public ActionResult GetMagazineArticleMappings(int ID)
        {
            var result = new List<MagazineArticleGroupMapping>();
            var articles = _db.MagazineArticlePages.Where(x => x.ParentID == ID).ToList();
            //foreach (var item in articles)
            //{
            //    result.Add(new MagazineArticleGroupMapping { Group = item.MagazineArticleGroup, ID = item.ID, Title = item.Title });
            //}
            return this.Json(new { mappings = result, success = true });
        }

        public ActionResult SaveMagazineArticleMappings(string mappingsJson, int id)
        {
            var mappings = new JavaScriptSerializer().Deserialize<MagazineArticleGroupMapping[]>(mappingsJson);
            var articles = _db.MagazineArticlePages.Where(x => x.ParentID == id).ToList();
            foreach (var item in mappings)
            {
                var foundArticle = articles.FirstOrDefault(x => x.ID == item.ID);
                //if (foundArticle != null && foundArticle.MagazineArticleGroup != item.Group)
                //{
                //    foundArticle.MagazineArticleGroup = item.Group;
                //    _db.Entry(foundArticle).State = EntityState.Modified;
                //}
            }

            try
            {
                _db.SaveChanges();
            }
            catch (DataException error)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator. Error:" + error.Message + ". Data:" + error.Data);
                return Json(new { success = false });
            }

            return Json(new { success = true });
        }
    }

    public class MagazineArticleGroupMappingViewModel
    {
        public List<MagazineArticleGroupMapping> Mappings { get; set; }

        public int ID { get; set; }
    }

    public class MagazineArticleGroupMapping
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Group { get; set; }
    }
}