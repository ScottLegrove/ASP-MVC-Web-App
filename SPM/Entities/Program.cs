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
    
    public partial class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Program()
        {
            this.Class_Assignements = new HashSet<Class_Assignements>();
        }
    
        public int id { get; set; }
        public int fk_school { get; set; }
        public string programs_name { get; set; }
        public Nullable<int> semesters { get; set; }
        public string course_code { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Class_Assignements> Class_Assignements { get; set; }
        public virtual School School { get; set; }
    }
}
