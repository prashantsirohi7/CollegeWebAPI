using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets.Repository
{
    public interface IStudentRepository : ICollegeRepository<Student>
    {
        Task<List<Student>> GetAllFeesStructure();
    }
}
