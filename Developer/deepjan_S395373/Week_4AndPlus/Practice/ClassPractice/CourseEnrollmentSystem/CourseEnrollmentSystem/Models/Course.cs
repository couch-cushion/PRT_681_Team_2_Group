using CourseEnrollmentSystem.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseEnrollmentSystem.Models
{
    internal class Course:IEntity
    {
        public int Id { get; private set; }
        public string Title { get; private set; }
        public decimal BaseFee { get; private set; }

        public Course(int id, string title, decimal baseFee)
        {
            if(id <= 0)
            {
                throw new ArgumentException("Course Id cannot be smaller than 1.");
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title cannot be Empty");
            }
            if(baseFee < 0)
            {
                throw new("Base fee cannot be negative.");
            }

            Id = id;
            Title = title;
            BaseFee = baseFee;
        }

        public override string ToString()
        {
            return $"Course id: {Id}, Title: {Title}," +
                $"BaseFee: {BaseFee}";
        }
    }
}
