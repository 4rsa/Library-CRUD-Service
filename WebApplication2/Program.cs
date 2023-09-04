using System.Text.RegularExpressions;

// ��������� ������
List<Book> books = new List<Book>
{
    new() { Id = Guid.NewGuid().ToString(), Name = "Cathcher", Date = "12/03/1998" },
    new() { Id = Guid.NewGuid().ToString(), Name = "Bobby", Date = "12/03/1998" },
    new() { Id = Guid.NewGuid().ToString(), Name = "Samyel Jackson", Date = "12/03/1998" }
};

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var path = request.Path;
    //string expressionForNumber = "^/api/books/([0-9]+)$";   // ���� id ������������ �����

    // 2e752824-1657-4c7f-844b-6ec2e168e99c
    string expressionForGuid = @"^/api/books/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
    if (path == "/api/books" && request.Method == "GET")
    {
        await GetAllBooks(response);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
    {
        // �������� id �� ������ url
        string? id = path.Value?.Split("/")[3];
        await GetBook(id, response);
    }
    else if (path == "/api/books" && request.Method == "POST")
    {
        await CreateBook(response, request);
    }
    else if (path == "/api/books" && request.Method == "PUT")
    {
        await UpdateBook(response, request);
    }
    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "DELETE")
    {
        string? id = path.Value?.Split("/")[3];
        await DeleteBook(id, response);
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("html/index.html");
    }
});

app.Run();

// ��������� ���� ����
async Task GetAllBooks(HttpResponse response)
{
    await response.WriteAsJsonAsync(books);
}
// ��������� ����� ����� �� id
async Task GetBook(string? id, HttpResponse response)
{
    // �������� ����� �� id
    Book? book = books.FirstOrDefault((u) => u.Id == id);
    // ���� ����� �������, ���������� ���
    if (book != null)
        await response.WriteAsJsonAsync(book);
    // ���� �� ��������, ���������� ��������� ��� � ��������� �� ������
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "����� �� �������" });
    }
}

async Task DeleteBook(string? id, HttpResponse response)
{
    // �������� ������������ �� id
    Book? book = books.FirstOrDefault((u) => u.Id == id);
    // ���� ����� �������, ������� ���
    if (book != null)
    {
        books.Remove(book);
        await response.WriteAsJsonAsync(book);
    }
    // ���� �� �������, ���������� ��������� ��� � ��������� �� ������
    else
    {
        response.StatusCode = 404;
        await response.WriteAsJsonAsync(new { message = "����� �� �������" });
    }
}

async Task CreateBook(HttpResponse response, HttpRequest request)
{
    try
    {
        // �������� ������ ������������
        var book = await request.ReadFromJsonAsync<Book>();
        if (book != null)
        {
            // ������������� id ��� ������ ������������
            book.Id = Guid.NewGuid().ToString();
            // ��������� ������������ � ������
            books.Add(book);
            await response.WriteAsJsonAsync(book);
        }
        else
        {
            throw new Exception("������������ ������");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "������������ ������" });
    }
}

async Task UpdateBook(HttpResponse response, HttpRequest request)
{
    try
    {
        // �������� ������ �����
        Book? bookData = await request.ReadFromJsonAsync<Book>();
        if (bookData != null)
        {
            // �������� ����� �� id
            var book = books.FirstOrDefault(u => u.Id == bookData.Id);
            // ���� ����� �������, �������� ��� ������ � ���������� ������� �������
            if (book != null)
            {
                book.Date = bookData.Date;
                book.Name = bookData.Name;
                await response.WriteAsJsonAsync(book);
            }
            else
            {
                response.StatusCode = 404;
                await response.WriteAsJsonAsync(new { message = "����� �� �������" });
            }
        }
        else
        {
            throw new Exception("������������ ������");
        }
    }
    catch (Exception)
    {
        response.StatusCode = 400;
        await response.WriteAsJsonAsync(new { message = "������������ ������" });
    }
}
public class Book
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Date { get; set; }
}