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
    
    public partial class MetaTags
    {
        public int MetaTagID { get; set; }
        public int TypeID { get; set; }
        public int CategoryID { get; set; }
        public Nullable<int> ArticleID { get; set; }
        public string Content { get; set; }
    }
}
