using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore_3
{
    [Table("Student")]
    class Student
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public double Grade { get; set; }
    }
}
