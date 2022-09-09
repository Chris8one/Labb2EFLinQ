using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Labb2EFLinQ.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int ClassId { get; set; }
        public Class _Class { get; set; }
        public virtual ICollection<SchoolSchedule> SchoolSchedules { get; set; }
    }
}
