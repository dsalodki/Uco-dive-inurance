//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CrmMigration.OldDB
{
    using System;
    using System.Collections.Generic;
    
    public partial class dbsArticlesAttached
    {
        public int ArticlesAttachedID { get; set; }
        public string ArticlesAttachedTitle { get; set; }
        public string AttchedFile { get; set; }
        public int ArticleID { get; set; }
        public Nullable<int> OrderNo { get; set; }
        public int ParentType { get; set; }
        public int UserID { get; set; }
    }
}
