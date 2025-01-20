using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("students")]
public class StudentsController : ControllerBase
{
    private readonly string connectionString;

    public StudentsController()
    {
        connectionString = $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                           $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                           $"User={Environment.GetEnvironmentVariable("DB_USER")};" +
                           $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                           $"Port={Environment.GetEnvironmentVariable("DB_PORT")};";
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent([FromBody] Student student)
    {
        if (string.IsNullOrEmpty(student.Name) || string.IsNullOrEmpty(student.Email) || student.Age <= 0)
            return BadRequest("Invalid student data.");

        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "INSERT INTO students (name, email, age) VALUES (@Name, @Email, @Age)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", student.Name);
            command.Parameters.AddWithValue("@Email", student.Email);
            command.Parameters.AddWithValue("@Age", student.Age);

            await command.ExecuteNonQueryAsync();

            return Created("", student);
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, $"Database Error: {ex.Message}");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM students";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            var students = new List<Student>();
            while (await reader.ReadAsync())
            {
                students.Add(new Student
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Email = reader.GetString("email"),
                    Age = reader.GetInt32("age")
                });
            }

            return Ok(students);
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, $"Database Error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudentById(int id)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM students WHERE id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var student = new Student
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Email = reader.GetString("email"),
                    Age = reader.GetInt32("age")
                };
                return Ok(student);
            }
            return NotFound($"Student with ID {id} not found.");
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, $"Database Error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student student)
    {
        if (string.IsNullOrEmpty(student.Name) || string.IsNullOrEmpty(student.Email) || student.Age <= 0)
            return BadRequest("Invalid student data.");

        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "UPDATE students SET name = @Name, email = @Email, age = @Age WHERE id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", student.Name);
            command.Parameters.AddWithValue("@Email", student.Email);
            command.Parameters.AddWithValue("@Age", student.Age);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
                return NoContent();

            return NotFound($"Student with ID {id} not found.");
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, $"Database Error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        try
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM students WHERE id = @Id";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected > 0)
                return NoContent();

            return NotFound($"Student with ID {id} not found.");
        }
        catch (MySqlException ex)
        {
            return StatusCode(500, $"Database Error: {ex.Message}");
        }
    }
}

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Age { get; set; }
}
