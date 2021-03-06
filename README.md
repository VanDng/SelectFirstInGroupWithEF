# Introduction

I was working on a project using EF 3.1. It was a day that I was given a dataset and asked to select only a few rows in each grouped subset. 

There was nothing to clarify, I went straight forward to write LINQ, it should be something below
```
DatabaseContext.Objects.Where(w => ...)
                       .GroupBy(object => object.Key)
                       .Select(group => new {
                          Something = group.OrderBy(orderby => ...).First().Something
                       }
                       ...
```
and what I got was an exception saying "...[something] can not be translated...".

It turned out that LINQ obviously is an immediate language and gradually, it will be translated into a SQL query language. 
That is why there are cases LINQ still does not work, it is understandable.

Back to my problem, I had never dealt with this problem before so I had to search a lot to find a solution and to expand my knowledge surrounding it.

Here is the table showing ways to solve the problem.

| Approach                      | EF Core 3.1        | EF Core 6          |
| :---                          |    :----:          |    :----:          |
| Use `GroupBy` syntax          | :x:                | :heavy_check_mark: |
| Do not use `GroupBy` syntax   | :heavy_check_mark: | :heavy_check_mark: |
| Use `FromSqlRaw` syntax (*)   | :heavy_check_mark: | :heavy_check_mark: |

(*) [FromSqlRaw](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.relationalqueryableextensions.fromsqlraw?view=efcore-6.0)
is away we use EF to execute raw SQL queries.

This repository contains examples for each approach on each EF version. Enjoy ~

# Setup

1/ Prepare a file named `connectionstring.txt`, and put it in the folder at path `EFCore_3`. The `connectionstring.txt` must contain a connection string to a SQL Server.

2/ Run pre-created migration to create a sample table `Student` and data. Open a terminate in the folder `EFCore_3`, execute the command `dotnet ef database update`.

![image](https://user-images.githubusercontent.com/20492454/165210309-d638473b-06fa-4881-8969-2faa12643b81.png)

3/ Build and run the project EFCore_3 or EFCore_6 to experience.

# Reference

- https://docs.microsoft.com/en-us/ef/core/what-is-new/ef-core-6.0/whatsnew#improved-groupby-support
- https://stackoverflow.com/questions/59456026/how-to-select-top-n-rows-for-each-group-in-a-entity-framework-groupby-with-ef-3
- https://stackoverflow.com/questions/35631903/raw-sql-query-without-dbset-entity-framework-core
