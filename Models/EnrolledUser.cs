//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AskNLearn.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class EnrolledUser
    {
        public int eid { get; set; }
        public int uid { get; set; }
        public int coid { get; set; }
        public System.DateTime dateTime { get; set; }
    
        public virtual Cours Cours { get; set; }
        public virtual User User { get; set; }
    }
}
