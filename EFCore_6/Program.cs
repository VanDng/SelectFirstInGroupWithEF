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
            // ID: 2, Name: A, Grade: 6.0
            // ID: 3, Name: A, Grade: 7.0
            // ID: 4, Name: B, Grade: 8.0
            // ID: 5, Name: B, Grade: 9.0
            // ID: 6, Name: B, Grade: 10.0

            // The expected result would be
            // ID: 2, Name: A, Grade: 6
            // ID: 4, Name: B, Grade: 8
            //SelectFirstStudent(db.Students, orderByAsc: true);

            // The expected result would be
            // ID: 3, Name: B, Grade: 7
            // ID: 5, Name: B, Grade: 9
            SelectFirstStudent(db, orderByAsc: false);

            db.Dispose();
        }

        static void SelectFirstStudent(StudentDbContext db, bool orderByAsc)
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
            SelectFirstStudent_1(db.Students, orderByAsc);

            //
            // Approach 2:
            //
            SelectFirstStudent_2(db.Students, orderByAsc);

            //
            // Approach 3:
            //
            SelectFirstStudent_3(db, orderByAsc);
        }

        static void SelectFirstStudent_1(IQueryable<Student> students, bool orderByAsc)
        {
            Console.WriteLine("::::::::::::::::::");
            Console.WriteLine("::: Approach 1 :::");
            Console.WriteLine("::::::::::::::::::");

            IQueryable<dynamic> query = null;
            List<dynamic> queryResult = null;

            Exception exception = null;

            try
            {
                query = students.Where(student => student.Grade < 10 && student.Grade > 5)
                                .GroupBy(groupBy => groupBy.Name)
                                .Select(gr => new
                                {
                                    Student = orderByAsc ? gr.OrderBy(e => e.Grade).FirstOrDefault() :
                                                           gr.OrderByDescending(e => e.Grade).FirstOrDefault()
                                })

                                // It seems like a bug of EF. Don't understand why it does not work!
                                //.Select(student => new
                                //{
                                //    ID = student.Student.ID,
                                //    Name = student.Student.Name,
                                //    Grade = student.Student.Grade
                                //})
                                ;

                queryResult = query.AsEnumerable()
                                   .Select(student => new
                                   {
                                       ID = student.Student.ID,
                                       Name = student.Student.Name,
                                       Grade = student.Student.Grade
                                   })
                                   .Cast<dynamic>() // Yes, I can new a strong-typed object, instead. I just want to try a different way.
                                   .ToList();
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
                    Console.WriteLine($"ID:{student.ID}. Name:{student.Name}. Grade:{student.Grade}");
                });
            }
        }

        static void SelectFirstStudent_2(IQueryable<Student> students, bool orderByAsc)
        {
            Console.WriteLine("::::::::::::::::::");
            Console.WriteLine("::: Approach 2 :::");
            Console.WriteLine("::::::::::::::::::");

            IQueryable<dynamic> query = null;
            List<dynamic> queryResult = null;

            Exception exception = null;

            try
            {
                query = students.Where(student => student.Grade > 5 && student.Grade < 10)
                                .Select(student => student.Name).Distinct() // Similar to GroupBy
                                .Select(groupedName => new
                                {
                                    // Nested selection query
                                    Student = orderByAsc ? students.Where(s => s.Grade > 5 && s.Grade < 10)
                                                                   .Where(w => w.Name == groupedName)
                                                                   .OrderBy(s => s.Grade).First() 
                                                         :
                                                           students.Where(s => s.Grade > 5 && s.Grade < 10)
                                                                   .Where(w => w.Name == groupedName)
                                                                   .OrderByDescending(s => s.Grade).First()
                                })
                                .Select(student => new
                                {
                                    Id = student.Student.ID,
                                    Name = student.Student.Name,
                                    Grade = student.Student.Grade
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
                    Console.WriteLine($"ID:{student.Id}. Name:{student.Name}. Grade:{student.Grade}");
                });
            }
        }

        static void SelectFirstStudent_3(StudentDbContext db, bool orderByAsc)
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
                                   "ROW_NUMBER() OVER(PARTITION BY s.Name " +
                                                     "ORDER BY s.Grade " + (orderByAsc ? "ASC"  : "DESC") + ") AS rank " +
                              "FROM[Student] s " +

                              "WHERE s.Grade > 5 AND s.Grade < 10) " +
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
                    Console.WriteLine($"Name:{student.Name}. Grade:{student.Grade}");
                });
            }
        }
    }
}
