using CourseEnrollmentSystem.Models;

namespace CourseEnrollmentSystem.Interfaces
{
    internal interface IFeeCalculator
    {
        decimal CalculateFee(Course course, Student student);
    }
}