using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Apbd_tut10.Models.Requests
{
    public class EnrollStudentRequest
    {
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }
     
        public string FirstName { get; set; }
 
        public string LastName { get; set; }
     
        public DateTime BirthDate { get; set; }
       
        public string Studies { get; set; }
    }
}
