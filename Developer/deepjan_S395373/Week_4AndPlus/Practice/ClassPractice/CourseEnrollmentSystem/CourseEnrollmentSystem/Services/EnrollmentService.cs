using CourseEnrollmentSystem.Interfaces;
using CourseEnrollmentSystem.Models;
using CourseEnrollmentSystem.Repositories;

namespace CourseEnrollmentSystem.Services
{
    internal class EnrollmentService
    {
        private readonly InMemoryRepository<Student> _studentRepository;
        private readonly InMemoryRepository<Course> _courseRepository;
        private readonly IFeeCalculator _feeCalculator;

        public EnrollmentService(
            InMemoryRepository<Student> studentRepository,
            InMemoryRepository<Course> courseRepository,
            IFeeCalculator feeCalculator)
        {
            _studentRepository = studentRepository;
            _courseRepository = courseRepository;
            _feeCalculator = feeCalculator;
        }

        public void EnrolStudent(int studentId, int courseId)
        {
            Student? student = _studentRepository.GetById(studentId);

            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }

            Course? course = _courseRepository.GetById(courseId);

            if (course == null)
            {
                Console.WriteLine("Course not found.");
                return;
            }

            decimal finalFee = _feeCalculator.CalculateFee(course, student);

            Console.WriteLine("Student Enrollment Billing");
            Console.WriteLine($"Student: {student.FullName}");
            Console.WriteLine($"Course: {course.Title}");
            Console.WriteLine($"Student Type: {(student.IsInternational ? "International" : "Domestic")}");
            Console.WriteLine($"Final Fee: ${finalFee}");
        }
    }
}