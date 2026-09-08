using CourseEnrollmentSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseEnrollmentSystem.Models
{
    internal class Student:IEntity
    {
        public  int Id{ get; private set; }
        public  string FullName { get; private set; }
        public bool IsInternational { get; private set; }

        public Student(int id, string fullName, bool isInternational)
        {
            if(id <= 0)
            {
                throw new ArgumentException("Id cannot be smaller than 1;");
            }

            if (String.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("Student's name cannot " +
                    "be negative.");
            }
            Id = id;
            FullName = fullName;
            IsInternational = isInternational;
        }

        public override string ToString()
        {
            if (IsInternational)
            {
                return $"Student id: {Id} -- Name: {FullName} -- " +
                $"Status: International student";
            } else
                return $"Student id: {Id } -- Name: {FullName} -- " +
                $"Status: Domestic Student";
        }
    }
}
