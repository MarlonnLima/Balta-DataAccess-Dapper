using System.Data;
using BaltaDataAcess.Models;
using Dapper;
using Microsoft.Data.SqlClient;

const string connectionString = "Server=127.0.0.1,1433;Database=balta;User ID=sa;Password=1q2w3e4r@#$;Trusted_Connection=False; TrustServerCertificate=True;";

using (var conn = new SqlConnection(connectionString))
{
    //ListCategories(conn);
    //CreateCategory(conn);
    //CreateManyCategory(conn);
    //UpdateCategory(conn);
    //DeleteCategory(conn);
    //ExecuteProcedure(conn);
    //ExecuteReadProcedure(conn);
    //ExecuteScalar(conn);
    //ReadView(conn);
    //OneToOne(conn);
    //OneToMany(conn);
    QueryMultiple(conn);
}

void ListCategories(SqlConnection connection)
{
    var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category]");
    foreach (var item in categories)
    {
        Console.WriteLine($"{item.Id} - {item.Title}");
    }
}

void CreateCategory(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Amazon AWS";
    category.Url = "amazon";
    category.Description = "Categoria destinada a serviços do AWS";
    category.Order = 8;
    category.Summary = "AWS Cloud";
    category.Featured = false;

    var insertSQL = @"INSERT INTO 
                [Category] 
                VALUES (
                @Id,
                @Title, 
                @Url, 
                @Summary, 
                @Order, 
                @Description, 
                @Featured)";

    var rows = connection.Execute(insertSQL, new
    {
        category.Id,
        category.Title,
        category.Url,
        category.Summary,
        category.Order,
        category.Description,
        category.Featured
    });

    Console.WriteLine($"{rows} linhas inseridas");
}

void UpdateCategory(SqlConnection connection)
{
    var updateQuery = "UPDATE [Category] SET [Title]=@Title WHERE [Id]=@Id";
    var rows = connection.Execute(updateQuery, new
    {
        Id = new Guid("af3407aa-11ae-4621-a2ef-2028b85507c4"),
        Title = "Frontend 2025"
    });

    Console.WriteLine($"{rows} registros foram atualizados");
}

void DeleteCategory(SqlConnection connection)
{
    var deleteQueryCarrerItem = "DELETE FROM [CareerItem] WHERE [CourseId] IN (SELECT [CourseId] FROM [Course] WHERE [CategoryId] = @Id)";
    connection.Execute(deleteQueryCarrerItem, new
    {
        Id = new Guid("B67F4EF0-958C-4104-A7D7-8814BD605472"),
    });

    var deleteQueryCourse = "DELETE FROM [Course] WHERE [CategoryId] = @Id";
    connection.Execute(deleteQueryCourse, new
    {
        Id = new Guid("B67F4EF0-958C-4104-A7D7-8814BD605472"),
    });

    var deleteQuery = "DELETE FROM [Category] WHERE [Id]=@Id";
    var rows = connection.Execute(deleteQuery, new
    {
        Id = new Guid("B67F4EF0-958C-4104-A7D7-8814BD605472"),
    });

    Console.WriteLine($"{rows} registros foram deletados");
}

void CreateManyCategory(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Amazon AWS";
    category.Url = "amazon";
    category.Description = "Categoria destinada a serviços do AWS";
    category.Order = 8;
    category.Summary = "AWS Cloud";
    category.Featured = false;

    var category2 = new Category();
    category2.Id = Guid.NewGuid();
    category2.Title = "Categoria Nova";
    category2.Url = "categoria-nova";
    category2.Description = "Categoria Nova";
    category2.Order = 9;
    category2.Summary = "Categoria";
    category2.Featured = true;

    var insertSQL = @"INSERT INTO 
                [Category] 
                VALUES (
                @Id,
                @Title, 
                @Url, 
                @Summary, 
                @Order, 
                @Description, 
                @Featured)";

    var rows = connection.Execute(insertSQL, new[]
    {
        new {
        category.Id,
        category.Title,
        category.Url,
        category.Summary,
        category.Order,
        category.Description,
        category.Featured
        },
        new {
        category2.Id,
        category2.Title,
        category2.Url,
        category2.Summary,
        category2.Order,
        category2.Description,
        category2.Featured
        }
    });

    Console.WriteLine($"{rows} linhas inseridas");
}

void ExecuteProcedure(SqlConnection connection)
{
    var procedure = "[spDeleteStudent]";
    var param = new { StudentId = "C207504D-2F7A-460E-BECE-1A133A04C7EE" };

    var affectedRows = connection.Execute(
        procedure,
        param,
        commandType: CommandType.StoredProcedure
    );

    Console.WriteLine($"{affectedRows} linhas foram afetadas");
}

void ExecuteReadProcedure(SqlConnection connection)
{
    var procedure = "[spGetCoursesByCategory]";
    var param = new { CategoryId = "09CE0B7B-CFCA-497B-92C0-3290AD9D5142" };

    var courses = connection.Query(
        procedure,
        param,
        commandType: CommandType.StoredProcedure
    );

    foreach (var item in courses)
    {
        Console.WriteLine(item.Id + " - " + item.Title);
    }
}

void ExecuteScalar(SqlConnection connection)
{
    var category = new Category();
    category.Title = "Amazon AWS";
    category.Url = "amazon";
    category.Description = "Categoria destinada a serviços do AWS";
    category.Order = 8;
    category.Summary = "AWS Cloud";
    category.Featured = false;

    var insertSQL = @"INSERT INTO 
                [Category] 
                OUTPUT inserted.[Id]
                VALUES (
                NEWID(),
                @Title, 
                @Url, 
                @Summary, 
                @Order, 
                @Description, 
                @Featured)";

    var id = connection.ExecuteScalar<Guid>(insertSQL, new
    {
        category.Title,
        category.Url,
        category.Summary,
        category.Order,
        category.Description,
        category.Featured
    });

    Console.WriteLine($"novo id {id}");
}

void ReadView(SqlConnection connection)
{
    var courses = connection.Query("SELECT * from [vwCourses]");
    foreach (var item in courses)
    {
        Console.WriteLine($"{item.Id} - {item.Title}");
    }
}

void OneToOne(SqlConnection connection)
{
    var sql = @"
        SELECT
            * 
        FROM
            [CareerItem]
        INNER JOIN
            [Course] ON [CareerItem].[CourseId] = [Course].[Id]
    ";

    var items = connection.Query<CareerItem, Course, CareerItem>(
        sql,
        (careerItem, course) =>
        {
            careerItem.Course = course;
            return careerItem;
        }, splitOn: "Id");

    foreach (var item in items)
    {
        Console.WriteLine($"{item.Title} - Curso: {item.Course.Title}");
    }
}

void OneToMany(SqlConnection connection)
{
    var sql = @"
        SELECT
            [Career].[Id],
            [Career].[Title],
            [CareerItem].[CareerId],
            [CareerItem].[Title]
        FROM
            [Career]
        INNER JOIN
            [CareerItem] ON [CareerItem].[CareerId]	 = [Career].[Id]
        ORDER BY
            [Career].[Title]
    ";

    var careers = new List<Career>();
    var items = connection.Query<Career, CareerItem, Career>(
        sql,
        (career, careerItem) =>
        {
            var car = careers.Where(x => x.Id == career.Id).FirstOrDefault();
            if (car == null)
            {
                car = career;
                car.Items.Add(careerItem);
                careers.Add(car);
            }
            else
            {
                car.Items.Add(careerItem);
            }
            return career;
        }, splitOn: "CareerId");

    foreach (var career in careers)
    {
        Console.WriteLine($"{career.Title}");
        foreach (var item in career.Items)
        {
            Console.WriteLine($" - {item.Title}");
        }
    }
}

void QueryMultiple(SqlConnection connection)
{
    var query = "SELECT * FROM [Category]; SELECT * FROM [Course]";

    using (var multi = connection.QueryMultiple(query))
    {
        var categories = multi.Read<Category>();
        var courses = multi.Read<Course>();

        foreach (var item in categories)
        {
            Console.WriteLine(item.Title);
        }

        foreach (var item in courses)
        {
            Console.WriteLine(item.Title);
        }
    }
}