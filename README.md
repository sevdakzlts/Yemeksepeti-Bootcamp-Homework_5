# Dapper

Dapper is an object-relational mapper (ORM) for .NET Dapper is a micro ORM or it is a simple object mapper framework that helps to map the native query output to a domain class or a C# class. It is a high-performance data access system built by the StackOverflow team. 



## Dependencies

-   [.Net5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
-   [Dapper2.0.78](https://www.nuget.org/packages/Dapper/)
-   [Swashbuckle.AspNetCore5.6.3](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Swagger/)



## Usage

After cloning this repository and installing  [Visual Studio](https://visualstudio.microsoft.com/tr/downloads/)  enter the project's folder through the command line and type the following code to run the program:  `dotnet run`

## CRUD in .Net using Dapper ORM

The Query() extension method in Dapper enables you to retrieve data from the database and populate data in your object model. The Execute() method of the Dapper framework can be used to insert, update, or delete data into a database. 
The Dapper framework also supports transactions, you can use transactional operations if needed. To do this, you can take advantage of the BeginTransaction() and EndTransaction() methods. You would then need to write your transactional statements inside the BeginTransaction and EndTransaction method calls.

The Dapper micro ORM is extremely lightweight and simple to use. It doesn’t generate your SQL for you but makes it easy to map the results of queries to your POCOs (plain old CLR objects).

## Stored procedures using Dapper ORM
To work with stored procedures using Dapper, you should mention the command type explicitly when calling the Query or the Execute methods. 

## Dapper - Result Multi-Mapping

Extension methods can be used to execute a query and map the result to a strongly typed list with relations.

The relation can be either:

-   [One to One](https://dapper-tutorial.net/result-multi-mapping#example---query-multi-mapping-one-to-one)
-   [One to Many](https://dapper-tutorial.net/result-multi-mapping#example---query-multi-mapping-one-to-many)

These extension methods can be called from any object of type IDbConnection.


### Query Multi-Mapping (One to One)[](https://dapper-tutorial.net/result-multi-mapping#example-query-multi-mapping-one-to-one)

Query method can execute a query and map the result to a strongly typed list with a one to one relation.

 

### Query Multi-Mapping (One to Many)[](https://dapper-tutorial.net/result-multi-mapping#example-query-multi-mapping-one-to-many)

Query method can execute a query and map the result to a strongly typed list with a one to many relations.

## Referances

-  https://www.infoworld.com/article/3025784/how-to-use-the-dapper-orm-in-c.html
- https://dapper-tutorial.net/dapper



