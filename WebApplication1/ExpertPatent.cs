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
    
    public partial class ExpertPatent
    {
        public Nullable<long> PatentID { get; set; }
        public Nullable<long> UserID { get; set; }
        public int ExpertPatentID { get; set; }
    
        public virtual ExpertInfo ExpertInfo { get; set; }
        public virtual Patent Patent { get; set; }
    }
}
