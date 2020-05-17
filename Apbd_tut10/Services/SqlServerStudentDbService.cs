using Apbd_tut10.Entities;
using Apbd_tut10.Models;
using Apbd_tut10.Models.Requests;
using Apbd_tut10.Models.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apbd_tut10.Services
{
    public class SqlServerStudentDbService : IStudentsServiceDb
    {

        private readonly StudentContext _studentContext;

        public SqlServerStudentDbService(StudentContext studentContext)
        {
            this._studentContext = studentContext;
        }


        public List<GetStudentsResponse> GetStudents()
        {
            var students = _studentContext
                .Student
                .Include(s => s.IdEnrollmentNavigation)
                .ThenInclude(s => s.IdStudyNavigation)
                .Select(s => new GetStudentsResponse
                {
                    IndexNumber = s.IndexNumber,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    BirthDate = s.BirthDate.ToShortDateString(),
                    Semester = s.IdEnrollmentNavigation.Semester,
                    StudiesName = s.IdEnrollmentNavigation.IdStudyNavigation.Name
                }).ToList();
            return students;
        }

        public InsertStudentResponse InsertStudent(InsertStudentRequest request)
        {

            var exists = _studentContext.Student.Any(s => s.IndexNumber.Equals(request.IndexNumber));

            if (exists) { return null; }

            var student = new Student()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                IndexNumber = request.IndexNumber,
                IdEnrollment = request.IdEnrollment,
                Password = request.Password,
                PSalt = request.Salt,
                RefreshToken = request.RefreshToekn

            };
            _studentContext.Add(student);
            _studentContext.SaveChanges();

            return new InsertStudentResponse
            {
                IndexNumber = student.IndexNumber,
                LastName = student.LastName
            };
        }

        public UpdateStudentResponse UpdateStudent(UpdateStudentRequest request)
        {
            var exists = _studentContext.Student.Any(s => s.IndexNumber.Equals(request.IndexNumber));
            if (!exists) { return null; }

            var prevName = _studentContext.Student.Where(s => s.IndexNumber.Equals(request.IndexNumber))
                .Select(s => s.FirstName).FirstOrDefault();

            var s = new Student
            {
                IndexNumber = request.IndexNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IdEnrollment = request.IdEnrollment,
                BirthDate = request.BirthDate
            };

            _studentContext.Attach(s);
            _studentContext.Entry(s).State = EntityState.Modified;
            _studentContext.SaveChanges();
            return new UpdateStudentResponse
            {
                IndexNumber = s.IndexNumber,
                FirstName = s.FirstName,
                PreviousName = prevName
            };
        }

        public DeleteResponse DeleteStudent(string id)
        {
            var exists = _studentContext.Student.Any(s => s.IndexNumber.Equals(id));
            if (!exists) return null;

            var s = new Student
            {
                IndexNumber = id
            };
            _studentContext.Attach(s);
            //_studentContext.Entry(s).State = EntityState.Modified;
            _studentContext.Remove(s);
            _studentContext.SaveChanges();

            return new DeleteResponse
            {
                IndexNumber = s.IndexNumber,
            };
        }

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            var semester = 1;
            // checking if we have such idStudent
            var exists = _studentContext.Student.Any(s => s.IndexNumber.Equals(request.IndexNumber));
            if (exists) { return null; }

            // looking for the last Id enrollment
            var countOfRows = _studentContext.Enrollment.Count();
            var lastIdEnrollment = -1;

            if (countOfRows == 0)
            {
                lastIdEnrollment = 0;
            }
            else
            {
                lastIdEnrollment = _studentContext.Enrollment
                    .Skip(countOfRows - 1)
                    .FirstOrDefault().IdEnrollment;
            }

            //checking whether we have such studies or no
            var studyExists = _studentContext.Studies.Any(s => s.Name.Equals(request.Studies));
            if (!studyExists)
            {
                return null;
            }
            var idStudy = _studentContext.Studies
                .Where(s => s.Name.Equals(request.Studies))
                .Select(s => s.IdStudy)
                .FirstOrDefault();

            //cheking whether we have the records with semester 1
            var firstSemExists = _studentContext.Enrollment.Any(s => s.Semester.Equals(1));
            if (!firstSemExists)
            {
                var enr = new Enrollment()
                {
                    IdEnrollment = ++lastIdEnrollment,
                    Semester = semester,
                    IdStudy = idStudy,
                    StartDate = DateTime.Now,
                };

                _studentContext.Add(enr);
                _studentContext.SaveChanges();
            }

            //Insert into student
            var student = new Student()
            {
                IndexNumber = request.IndexNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                IdEnrollment = lastIdEnrollment
            };

            _studentContext.Add(student);
            _studentContext.SaveChanges();

            return new EnrollStudentResponse
            {
                LastName = request.LastName,
                Semester = semester
            };
        }


        public PromoteStudentResponse PromoteStudent(PromoteStudentRequest req)
        {
            // looking for id study of StudyName
            var idStudy = _studentContext
           .Studies.Where(s => s.Name.Equals(req.Name)).Select(s => s.IdStudy).FirstOrDefault();


            // finding such enrollment
            var nextIdEnrollment = _studentContext
                .Enrollment
                .Where(s => s.IdStudy.Equals(idStudy) && s.Semester == req.Semester + 1)
                .Select(s => s.IdEnrollment).FirstOrDefault();

            // last index of all enrollments
            var lastIdEnrollment = _studentContext
                .Enrollment
                .Max(e => e.IdEnrollment);

            if (nextIdEnrollment == 0)
            {


                var enrollmet = new Enrollment()
                {
                    IdEnrollment = lastIdEnrollment + 1,
                    Semester = req.Semester + 1,
                    IdStudy = idStudy,
                    StartDate = DateTime.Now.Date
                };
                nextIdEnrollment = lastIdEnrollment + 1;
                _studentContext.Add(enrollmet);
                _studentContext.SaveChanges();

            }
            // looking for the  enrollment id 

            var IdEnrollment = _studentContext
                .Enrollment.Where(e => e.Semester == req.Semester && e.IdStudy.Equals(idStudy))
                .Include(s => s.Student)
                .Select(s => s.IdEnrollment).Distinct().FirstOrDefault();

            // updating students (EASY VERSION)
            var student = _studentContext.Student.Where(s => s.IdEnrollment == IdEnrollment).ToList();
            student.ForEach(a => a.IdEnrollment = nextIdEnrollment);
            _studentContext.SaveChanges();

            //updating students (NOT REALLY EASY VERSION)
            /* // finding studnets with such id
             var students = _studentContext.Student.Where(s => s.IdEnrollment == IdEnrollment)
                 .AsNoTracking().ToList(); // for some reason change tracker detects that
                                           //another instance with same key value is tracked so I turned it off

             for (int i = 0; i < students.Count; i++)
             {
                     var s = new Student
                     {
                         IndexNumber = students[i].IndexNumber,
                         IdEnrollment = nextIdEnrollment,
                         FirstName = students[i].FirstName,
                         LastName = students[i].LastName,
                         BirthDate = students[i].BirthDate,
                         Password = students[i].Password,
                         PSalt = students[i].PSalt,
                         RefreshToken = students[i].RefreshToken,
                         IdEnrollmentNavigation = students[i].IdEnrollmentNavigation,

                     };

                     _studentContext.Attach(s);
                     _studentContext.Entry(s).State = EntityState.Modified;
                     _studentContext.SaveChanges();

             }*/
            return new PromoteStudentResponse
            {
                Name = req.Name,
                Semester = req.Semester + 1
            };
        }
    }
}
