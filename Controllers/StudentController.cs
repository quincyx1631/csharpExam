using Microsoft.AspNetCore.Mvc;
using exam.api.Models;
using exam.api.Repositories;

namespace exam.api.Controllers;

[ApiController]
[Route("students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentRepository _repository;

    public StudentsController(IStudentRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        try
        {
            var students = await _repository.GetAllAsync();
            return Ok(students);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Database Error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudentById(int id)
    {
        try
        {
            var student = await _repository.GetByIdAsync(id);
            if (student is null)
                return NotFound($"Student with ID {id} not found.");

            return Ok(student);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Database Error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent([FromBody] Student student)
    {
        if (string.IsNullOrEmpty(student.Name) || string.IsNullOrEmpty(student.Email) || student.Age <= 0)
            return BadRequest("Invalid student data.");

        try
        {
            var success = await _repository.CreateAsync(student);
            if (success)
                return Created("", student);

            return StatusCode(500, "Failed to create student.");
        }
        catch (Exception ex)
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
            var success = await _repository.UpdateAsync(id, student);
            if (success)
                return NoContent();

            return NotFound($"Student with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Database Error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        try
        {
            var success = await _repository.DeleteAsync(id);
            if (success)
                return NoContent();

            return NotFound($"Student with ID {id} not found.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Database Error: {ex.Message}");
        }
    }
}