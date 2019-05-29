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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Candidate
    {
        public Candidate()
        {
            this.Votes = new HashSet<Vote>();
        }
    
        public int Id { get; set; }
        public Nullable<int> election_id { get; set; }

        [DisplayName("Candidate Name")]
        [Required(ErrorMessage ="Please Enter Candidate Name.")]
        public string name { get; set; }

        [DisplayName("About Candidate")]
        [Required(ErrorMessage = "Please Enter Description about Candidate.")]
        public string about { get; set; }
        public byte[] pic { get; set; }
        public Nullable<int> pic_id { get; set; }

        [DisplayName("Title")]
        [Required(ErrorMessage = "Please Enter Title Correctly.")]
        public string title { get; set; }
        public virtual ICollection<Vote> Votes { get; set; }
        public virtual Election Election { get; set; }
        public virtual Image Image { get; set; }
    }
}
