﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WebAPIEntities : DbContext
    {
        public WebAPIEntities()
            : base("name=WebAPIEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Administrator> Administrator { get; set; }
        public virtual DbSet<Attention> Attention { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Download> Download { get; set; }
        public virtual DbSet<ExpertInfo> ExpertInfo { get; set; }
        public virtual DbSet<ExpertPaper> ExpertPaper { get; set; }
        public virtual DbSet<ExpertPatent> ExpertPatent { get; set; }
        public virtual DbSet<Like> Like { get; set; }
        public virtual DbSet<ManageExpert> ManageExpert { get; set; }
        public virtual DbSet<ManagePaper> ManagePaper { get; set; }
        public virtual DbSet<ManagePatent> ManagePatent { get; set; }
        public virtual DbSet<ManageUser> ManageUser { get; set; }
        public virtual DbSet<Paper> Paper { get; set; }
        public virtual DbSet<Patent> Patent { get; set; }
        public virtual DbSet<Reviewer> Reviewer { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}