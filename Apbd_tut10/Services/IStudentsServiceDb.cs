using Apbd_tut10.Entities;
using Apbd_tut10.Models;
using Apbd_tut10.Models.Requests;
using Apbd_tut10.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apbd_tut10.Services
{
    public interface IStudentsServiceDb
    {
        List<GetStudentsResponse> GetStudents();

        InsertStudentResponse InsertStudent(InsertStudentRequest request);

        DeleteResponse DeleteStudent(string id);

        UpdateStudentResponse UpdateStudent(UpdateStudentRequest request);

        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);

        PromoteStudentResponse PromoteStudent(PromoteStudentRequest req);
    }
}
