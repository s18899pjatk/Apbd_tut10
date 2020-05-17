using System;
using System.Collections.Generic;

namespace Apbd_tut10.Entities
{
    public partial class Student
    {
        public string IndexNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int IdEnrollment { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public string PSalt { get; set; }

        public virtual Enrollment IdEnrollmentNavigation { get; set; }
    }
}
