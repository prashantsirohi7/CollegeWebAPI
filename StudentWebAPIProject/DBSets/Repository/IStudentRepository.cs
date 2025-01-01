using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets.Repository
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student> GetStudentByIdAsync(int id, bool asNoTracking = false);
        Task<Student> GetStudentByNameAsync(string name);
        Task<int> CreateStudentAsync(Student student);
        Task<int> UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(Student student);
    }
}
