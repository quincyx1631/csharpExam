using MySqlConnector;
using exam.api.Data;
using exam.api.Models;

namespace exam.api.Repositories;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<bool> CreateAsync(Student student);
    Task<bool> UpdateAsync(int id, Student student);
    Task<bool> DeleteAsync(int id);
}

public class StudentRepository : IStudentRepository
{
    private readonly DatabaseContext _context;

    public StudentRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        using var connection = await _context.CreateConnectionAsync();
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

        return students;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        using var connection = await _context.CreateConnectionAsync();
        var query = "SELECT * FROM students WHERE id = @Id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Student
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Email = reader.GetString("email"),
                Age = reader.GetInt32("age")
            };
        }

        return null;
    }

    public async Task<bool> CreateAsync(Student student)
    {
        using var connection = await _context.CreateConnectionAsync();
        var query = "INSERT INTO students (name, email, age) VALUES (@Name, @Email, @Age)";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Name", student.Name);
        command.Parameters.AddWithValue("@Email", student.Email);
        command.Parameters.AddWithValue("@Age", student.Age);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> UpdateAsync(int id, Student student)
    {
        using var connection = await _context.CreateConnectionAsync();
        var query = "UPDATE students SET name = @Name, email = @Email, age = @Age WHERE id = @Id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Name", student.Name);
        command.Parameters.AddWithValue("@Email", student.Email);
        command.Parameters.AddWithValue("@Age", student.Age);

        return await command.ExecuteNonQueryAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = await _context.CreateConnectionAsync();
        var query = "DELETE FROM students WHERE id = @Id";
        using var command = new MySqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);

        return await command.ExecuteNonQueryAsync() > 0;
    }
}