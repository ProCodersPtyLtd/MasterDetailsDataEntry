using System;
using System.Collections.Generic;

namespace MasterDetailsDataEntry.Demo.Database.School
{
    public partial class Department
    {
        public Department()
        {
            Course = new HashSet<Course>();
        }

        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public int? Administrator { get; set; }

        public virtual ICollection<Course> Course { get; set; }
    }
}
