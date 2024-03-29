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
    
    public partial class ExpertInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExpertInfo()
        {
            this.Attention = new HashSet<Attention>();
            this.ExpertPatent = new HashSet<ExpertPatent>();
            this.Like = new HashSet<Like>();
            this.ManageExpert = new HashSet<ManageExpert>();
            this.UserExpert = new HashSet<UserExpert>();
            this.ExpertPaper = new HashSet<ExpertPaper>();
        }
    
        public long ExpertID { get; set; }
        public string Name { get; set; }
        public string Workstation { get; set; }
        public Nullable<bool> IsPass { get; set; }
        public Nullable<int> TimesCited { get; set; }
        public Nullable<int> Results { get; set; }
        public Nullable<int> PUJournals { get; set; }
        public Nullable<int> CSCDJournals { get; set; }
        public Nullable<int> CTJournals { get; set; }
        public Nullable<int> SCIJournals { get; set; }
        public Nullable<int> EIJournals { get; set; }
        public Nullable<int> SCIEJournals { get; set; }
        public Nullable<int> SSCIJournals { get; set; }
        public Nullable<int> OtherJournals { get; set; }
        public Nullable<int> ConferencePapers { get; set; }
        public string Field { get; set; }
        public Nullable<int> Books { get; set; }
        public Nullable<int> Others { get; set; }
        public string BaiduID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Attention> Attention { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExpertPatent> ExpertPatent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Like> Like { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ManageExpert> ManageExpert { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserExpert> UserExpert { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExpertPaper> ExpertPaper { get; set; }
    }
}
