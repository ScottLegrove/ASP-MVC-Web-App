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
    
    public partial class Project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            this.Project_Assignments = new HashSet<Project_Assignments>();
        }
    
        public int id { get; set; }
        public string projects_name { get; set; }
        public Nullable<double> grade { get; set; }
        public int grade_out_of { get; set; }
        public Nullable<int> grade_weight { get; set; }
        public System.DateTime due_date { get; set; }
        public Nullable<System.DateTime> created_at { get; set; }
        public byte[] last_modified { get; set; }
        public int fk_class_id { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Project_Assignments> Project_Assignments { get; set; }
    }
}