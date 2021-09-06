using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Uco.Models;

namespace Uco.Infrastructure.Services
{
    public class ArticleService
    {
        private Db _db
        {
            get
            {
                if (_Context == null) { return null; }
                return _Context.EntityContext;
            }
        }
        private DBContextService _Context = null;
        public ArticleService(Db _db = null)
        {
            _Context = new DBContextService(_db);
        }


        public List<ArticlePage> GetReleatedArticlesByTitle(string[] title)
        {
            List<ArticlePage> list = new List<ArticlePage>();
            var data = _db.ArticlePages.AsQueryable();
            foreach (var t in title)
            {
                list.AddRange(data.Where(x => x.Title == t));
            }

            return list.ToList();
        }
    }
}