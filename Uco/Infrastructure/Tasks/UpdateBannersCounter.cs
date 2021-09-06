using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using Uco.Infrastructure.Livecycle;
using Uco.Infrastructure.Repositories;
using Uco.Models;

namespace Uco.Infrastructure.Tasks
{
    public class UpdateBannersCounter : ITask
    {
        private const int TimesToSendMax = 3;
        private const int TimeInterval = 3600;

        public string Title { get { return "UpdateBannersCounter"; } }
        public int StartSeconds { get { return 60; } }
        public int IntervalSecondsFrom { get { return (5 * 59); } }
        public int IntervalSecondsTo { get { return (5 * 61); } }

        public void Execute()
        {
            try
            {
                using (Db _db = new Db())
                {

                    List<BannersStatistic> list = MemoryCache.Default["BannerList"] as List<BannersStatistic>;

                    if (list != null && list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            item.Date = new DateTime(item.Date.Year, item.Date.Month, item.Date.Day, 0, 0, 0);
                            var itemlist = _db.BannersStatistics.FirstOrDefault(b => b.BannerID == item.BannerID && b.Date == item.Date);


                            if (itemlist != null)
                            {
                                itemlist.CountViews = itemlist.CountViews + item.CountViews;
                                itemlist.CountClicks = itemlist.CountClicks + item.CountClicks;
                                _db.Entry(itemlist).State = EntityState.Modified;
                            }

                            else
                            {
                                var newItem = new BannersStatistic()
                                {
                                    BannerID = item.BannerID,
                                    Date = item.Date,
                                    CountViews = item.CountViews,
                                    CountClicks = item.CountClicks,
                                };
                                _db.BannersStatistics.Add(newItem);
                            }
                        }

                        _db.SaveChanges();
                    }

                    MemoryCache.Default["BannerList"] = new List<BannersStatistic>();
                    // clear for garbage collector
                    list = null;
                }
            }
            catch(Exception e)
            { 

            }
        }

    }
}