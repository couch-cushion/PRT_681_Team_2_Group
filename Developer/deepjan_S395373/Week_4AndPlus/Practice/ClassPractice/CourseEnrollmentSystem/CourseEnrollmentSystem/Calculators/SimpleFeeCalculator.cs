using CourseEnrollmentSystem.Interfaces;
using CourseEnrollmentSystem.Models;

namespace CourseEnrollmentSystem.Calculators
{
    internal class SimpleFeeCalculator : IFeeCalculator
    {
        public decimal CalculateFee(Course course, Student student)
        {
            if (student.IsInternational)
            {
                return course.BaseFee * 1.20m;
            }

            return course.BaseFee;
        }
    }
}