//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Errors
    {
        public int ID { get; set; }
        public int DomainID { get; set; }
        public System.DateTime Date { get; set; }
        public string Message { get; set; }
        public string Host { get; set; }
        public string UserName { get; set; }
        public string PhysicalPath { get; set; }
        public string UserAgent { get; set; }
        public string UserHostAddress { get; set; }
        public string Url { get; set; }
        public string UrlReferrer { get; set; }
        public string InnerException { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }
        public string TargetSite { get; set; }
    }
}
