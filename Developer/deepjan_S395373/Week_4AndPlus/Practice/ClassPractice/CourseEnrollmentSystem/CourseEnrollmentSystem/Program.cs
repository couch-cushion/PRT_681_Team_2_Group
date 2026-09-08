using CourseEnrollmentSystem.Models;
using CourseEnrollmentSystem.Interfaces;
using CourseEnrollmentSystem.Repositories;
using CourseEnrollmentSystem.Services;
using CourseEnrollmentSystem.Calculators;
try
{
    InMemoryRepository<Student> studentData = new InMemoryRepository<Student>();
    InMemoryRepository<Course> courseData = new InMemoryRepository<Course>();
    studentData.Add(new Student(1, "Deepjan Thapaliya", true));
    studentData.Add(new Student(2, "Michel Jackson", false));
    studentData.Add(new Student(4, "Jonny Jackson", false));
    studentData.Add(new Student(3, "Swastika GodFather", true));

    studentData.PrintAll();
    Console.WriteLine();

    studentData.RemoveById(4);
    Console.WriteLine();

    studentData.PrintAll();
    Console.WriteLine();

    courseData.Add(new Course(1, "MIT", 20000m));
    courseData.Add(new Course(2, "BIT", 20000m));
    courseData.Add(new Course(3, "BBA", 18000m));

    //courseData.PrintAll();

    //courseData.RemoveById(4);

    //courseData.PrintAll();

    //EnrollmentService(studentData, CourseData);

    IFeeCalculator feeCalculator = new SimpleFeeCalculator();

    EnrollmentService enrolService = new EnrollmentService(studentData, courseData, feeCalculator);
    Console.WriteLine();

    enrolService.EnrolStudent(1, 1);
    Console.WriteLine();
    enrolService.EnrolStudent(2, 2);
    Console.WriteLine();
    enrolService.EnrolStudent(3, 3);
    Console.WriteLine();
    enrolService.EnrolStudent(1, 3);
    Console.WriteLine();
} catch(ArgumentException ex)
{
    Console.WriteLine($"Validtion Error: {ex.Message}");
}

