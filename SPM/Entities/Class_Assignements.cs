//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SPM.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Class_Assignements
    {
        public int id { get; set; }
        public int fk_program { get; set; }
        public int fk_class { get; set; }
        public int fk_school { get; set; }
        public int fk_student { get; set; }
        public Nullable<int> fk_teacher { get; set; }
    
        public virtual Class Class { get; set; }
        public virtual School School { get; set; }
        public virtual SPM_Users SPM_Users { get; set; }
        public virtual Program Program { get; set; }
    }
}
