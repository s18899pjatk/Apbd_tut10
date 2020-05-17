using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apbd_tut10.Models.Requests
{
    public class PromoteStudentRequest
    {
        public string Name { get; set; }

        public int Semester { get; set; }
    }
}
