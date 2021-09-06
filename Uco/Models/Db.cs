using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Uco.Infrastructure;
using Uco.Infrastructure.Repositories;

namespace Uco.Models
{
    public class Db : DbContext
    {
        public DbSet<AbstractPage> AbstractPages { get; set; }
        public DbSet<DomainPage> DomainPages { get; set; }
        public DbSet<LanguagePage> LanguagePages { get; set; }
        public DbSet<ContentPage> ContentPages { get; set; }
        public DbSet<SiteMapPage> SiteMapPages { get; set; }
        public DbSet<ArticleListPage> ArticleListPages { get; set; }
        public DbSet<ArticlePage> ArticlePages { get; set; }
        public DbSet<FormPage> FormPages { get; set; }
        public DbSet<GalleryListPage> GalleryListPages { get; set; }
        public DbSet<GalleryPage> GalleryPages { get; set; }
        public DbSet<NewsListPage> NewsListPages { get; set; }
        public DbSet<NewsPage> NewsPages { get; set; }

        public DbSet<MagazinesPage> MagazinesPages { get; set; }
        public DbSet<MagazinePage> MagazinePages { get; set; }
        public DbSet<MagazineArticlePage> MagazineArticlePages { get; set; }
        
        public DbSet<Error> Errors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Newsletter> Newsletters { get; set; }


        public DbSet<Settings> SettingsAll { get; set; }
        public DbSet<OutEmail> OutEmails { get; set; }
        public DbSet<TextComponent> TextComponents { get; set; }
        public DbSet<Translation> Translations { get; set; }

        public DbSet<MenuModel> Menus { get; set; } 


        public DbSet<ShopCategoryAllPage> ShopCategoryAllPages { get; set; }
        public DbSet<ShopCategoryPage> ShopCategoryPages { get; set; }
        public DbSet<ShopBrandAllPage> ShopBrandAllPages { get; set; }
        public DbSet<ShopBrandPage> ShopBrandPages { get; set; }
        public DbSet<ShopProductAllPage> ShopProductAllPages { get; set; }
        public DbSet<ShopProductPage> ShopProductPages { get; set; }
        public DbSet<ShopProductSalePage> ShopProductSalePages { get; set; }
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<ShopAnaliticsData> ShopAnaliticsDatas { get; set; }
        public DbSet<ShopProductOption> ShopProductOptions { get; set; }



        public DbSet<UrlRecord> UrlRecords { get; set; }

        public DbSet<MagazineBanner1> MagazineBanners1 { get; set; }
        public DbSet<MagazineBanner2> MagazineBanners2 { get; set; }


        public DbSet<Banner> Banners { get; set; }
        public DbSet<BannersStatistic> BannersStatistics { get; set; }

        
        public DbSet<HomeBanner> HomeBanners { get; set; }

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Insurance> Insurances { get; set; }
        public DbSet<Dive> Dives { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //register plugin models
            if (SF.UsePlugins())
            {
                
                //var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");
                //foreach (var type in RP.GetPlugingsEntityModels())
                //{
                //    entityMethod.MakeGenericMethod(type)
                //        .Invoke(modelBuilder, new object[] { });
                //}
            }
            base.OnModelCreating(modelBuilder);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}