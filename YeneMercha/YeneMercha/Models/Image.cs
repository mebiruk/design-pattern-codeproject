//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace YeneMercha.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Image
    {
        public Image()
        {
            this.Candidates = new HashSet<Candidate>();
        }
    
        public int Id { get; set; }
        public string name { get; set; }
        public string content { get; set; }
    
        public virtual ICollection<Candidate> Candidates { get; set; }
    }
}
