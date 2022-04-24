using System;
using System.Collections.Generic;
using System.Linq;
using EFCore_6.Model;
using Microsoft.EntityFrameworkCore;

namespace EFCore_6
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new StudentDbContext();

            // Givent a collection of student below
            // ID: 1, Name: A, Grade: 5.0
            // ID: 2, Name: B, Grade: 5.0
            // ID: 3, Name: C, Grade: 5.0
            // ID: 4, Name: D, Grade: 6.0
            // ID: 5, Name: E, Grade: 6.0
            // ID: 6, Name: F, Grade: 7.0

            // The expected result would be
            // Grade: 5, Name: A
            // Grade: 6, Name: D
            //SelectOneStudentInGradeGroupLessThanSeven(db.Students, orderByAsc: true);

            // The expected result would be
            // Grade: 5, Name: C
            // Grade: 6, Name: E
            SelectOneStudentInGradeGroupLessThanSeven(db, orderByAsc: false);

            db.Dispose();
        }

        static void SelectOneStudentInGradeGroupLessThanSeven(StudentDbContext db, bool orderByAsc)
        {
            Console.WriteLine("|*****************************|");
            Console.WriteLine("|*****************************|");
            if (orderByAsc)
            {
                Console.WriteLine("|**        ASCENDING        **|");
            }
            else
            {
                Console.WriteLine("|**        DESCENDING       **|");

            }
            Console.WriteLine("|*****************************|");
            Console.WriteLine("|*****************************|");
            Console.WriteLine();

            //
            // Approach 1
            //
            SelectOneStudentInGradeGroup_1(db.Students, orderByAsc);

            //
            // Approach 2: It works but not good because it fetches unnecessary data to the client side.
            //
            SelectOneStudentInGradeGroup_2(db.Students, orderByAsc);

            //
            // Approach 3
            //
            SelectOneStudentInGradeGroup_3(db, orderByAsc);
        }

        static void SelectOneStudentInGradeGroup_1(IQueryable<Student> students, bool orderByAsc)
        {
            Console.WriteLine("::::::::::::::::::");
            Console.WriteLine("::: Approach 1 :::");
            Console.WriteLine("::::::::::::::::::");

            IQueryable<dynamic> query = null;
            List<dynamic> queryResult = null;

            Exception exception = null;

            try
            {
                query = students.Where(student => student.Grade < 7)
                                .GroupBy(groupBy => groupBy.Grade)
                                .Select(gr => new
                                {
                                    Grade = gr.Key,
                                    Name = orderByAsc ? gr.OrderBy(e => e.Name).FirstOrDefault().Name :
                                                        gr.OrderByDescending(e => e.Name).FirstOrDefault().Name
                                });
         
                queryResult = query.ToList();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Console.WriteLine("SQL Query:");
            if (exception != null)
            {
                Console.WriteLine("Could not translate.");
            }
            else
            {
                Console.WriteLine($"{query.ToQueryString()}");
            }
            Console.WriteLine();

            Console.WriteLine("Result:");
            if (exception != null)
            {
                Console.WriteLine(exception.Message);
            }
            else
            {
                queryResult.ForEach(student =>
                {
                    Console.WriteLine($"Grade: {student.Grade}. Name: {student.Name}");
                });
            }
        }

        static void SelectOneStudentInGradeGroup_2(IQueryable<Student> students, bool orderByAsc)
        {
            Console.WriteLine("::::::::::::::::::");
            Console.WriteLine("::: Approach 2 :::");
            Console.WriteLine("::::::::::::::::::");

            IQueryable<Student> query = null;
            List<dynamic> queryResult = null;

            Exception exception = null;

            try
            {
                query = students.Where(student => student.Grade < 7)
                                .OrderBy(student => student.Grade);

                queryResult = query.ToList() // Execute the sql query
                                   .GroupBy(groupBy => groupBy.Grade)
                                   .Select(group => new
                                   {
                                       Grade = group.Key,
                                       Name = orderByAsc ? group.OrderBy(e => e.Name).FirstOrDefault().Name :
                                                           group.OrderByDescending(e => e.Name).FirstOrDefault().Name
                                   })
                                   .Cast<dynamic>().ToList();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Console.WriteLine("SQL Query:");
            if (exception != null)
            {
                Console.WriteLine("Could not translate.");
            }
            else
            {
                Console.WriteLine($"{query.ToQueryString()}");
            }
            Console.WriteLine();

            Console.WriteLine("Result:");
            if (exception != null)
            {
                Console.WriteLine(exception.Message);
            }
            else
            {
                queryResult.ForEach(student =>
                {
                    Console.WriteLine($"Grade: {student.Grade}. Name: {student.Name}");
                });
            }
        }

        static void SelectOneStudentInGradeGroup_3(StudentDbContext db, bool orderByAsc)
        {
            Console.WriteLine("::::::::::::::::::");
            Console.WriteLine("::: Approach 3 :::");
            Console.WriteLine("::::::::::::::::::");

            string query = string.Empty;
            List<StudentNoID> queryResult = null;
            Exception exception = null;

            try
            {
                query = "WITH GradeStatistic AS( " +
                            // There's an important point. The column ID is not selected !
                             "SELECT s.Name, " +
                                    "s.Grade, " +
                                   "ROW_NUMBER() OVER(PARTITION BY s.Grade " +
                                                     "ORDER BY s.Name " + (orderByAsc ? "ASC"  : "DESC") + ") AS rank " +
                              "FROM[Student] s " +

                              "WHERE s.Grade < 7) " +
                         "SELECT * " +
                         "FROM GradeStatistic " +
                         "WHERE rank = 1";

                queryResult = db.StudentNoID.FromSqlRaw(query).ToList();
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            Console.WriteLine("SQL Query:");
            Console.WriteLine($"{query}");
            Console.WriteLine();

            Console.WriteLine("Result:");
            if (exception != null)
            {
                Console.WriteLine(exception.Message);
            }
            else
            {
                queryResult.ForEach(student =>
                {
                    Console.WriteLine($"Grade: {student.Grade}. Name: {student.Name}");
                });
            }
        }
    }
}
