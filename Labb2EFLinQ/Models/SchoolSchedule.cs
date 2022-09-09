using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Labb2EFLinQ.Models
{
    public class SchoolSchedule
    {
        [Key]
        public int ScheduleId { get; set; }
        public int CourseId { get; set; }
        public Course _Course { get; set; }
        public int TeacherId { get; set; }
        public Teacher _Teacher { get; set; }
        public int StudentId { get; set; }
        public Student _Student { get; set; }
    }
}
