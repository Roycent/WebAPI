//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebAPI
{
    using System;
    using System.Collections.Generic;
    
    public partial class Patent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Patent()
        {
            this.Comment = new HashSet<Comment>();
            this.ExpertPatent = new HashSet<ExpertPatent>();
            this.Like = new HashSet<Like>();
            this.ManagePatent = new HashSet<ManagePatent>();
            this.Review = new HashSet<Review>();
        }
    
        public long PatentID { get; set; }
        public Nullable<long> UpID { get; set; }
        public string Title { get; set; }
        public Nullable<System.DateTime> UpDate { get; set; }
        public string IPC { get; set; }
        public string Abstract { get; set; }
        public Nullable<short> ApplyNum { get; set; }
        public Nullable<System.DateTime> ApplyDate { get; set; }
        public string Applicant { get; set; }
        public string ApplicantAddress { get; set; }
        public string SIPC { get; set; }
        public string State { get; set; }
        public string Agencies { get; set; }
        public string Agent { get; set; }
        public string PublicNum { get; set; }
        public Nullable<bool> IsPass { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExpertPatent> ExpertPatent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Like> Like { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ManagePatent> ManagePatent { get; set; }
        public virtual Users Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Review> Review { get; set; }
    }
}
