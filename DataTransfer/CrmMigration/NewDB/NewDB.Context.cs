﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CrmMigration.NewDB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class NewDB : DbContext
    {
        public NewDB()
            : base("name=NewDB")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AbstractPages> AbstractPages { get; set; }
        public virtual DbSet<Agents> Agents { get; set; }
        public virtual DbSet<Banners> Banners { get; set; }
        public virtual DbSet<Contacts> Contacts { get; set; }
        public virtual DbSet<Errors> Errors { get; set; }
        public virtual DbSet<HomeBanners> HomeBanners { get; set; }
        public virtual DbSet<Newsletters> Newsletters { get; set; }
        public virtual DbSet<OutEmails> OutEmails { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<ShopAnaliticsDatas> ShopAnaliticsDatas { get; set; }
        public virtual DbSet<ShopOrders> ShopOrders { get; set; }
        public virtual DbSet<ShopProductOptions> ShopProductOptions { get; set; }
        public virtual DbSet<TextComponents> TextComponents { get; set; }
        public virtual DbSet<Translations> Translations { get; set; }
        public virtual DbSet<TravelInsurances> TravelInsurances { get; set; }
        public virtual DbSet<UrlRecords> UrlRecords { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}