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
    
    public partial class CompetitorVotes
    {
        public int CompetitorVoteID { get; set; }
        public string VoteTitle { get; set; }
        public Nullable<int> CustomerID { get; set; }
        public Nullable<int> CompetitorsID { get; set; }
        public Nullable<int> CompetitionID { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public int UserID { get; set; }
    }
}