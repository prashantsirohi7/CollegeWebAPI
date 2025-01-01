using Microsoft.EntityFrameworkCore;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets.Repository
{
    public class StudentRepository : CollegeRepository<Student>, IStudentRepository
    {
        private readonly CollegeDBContext _dBContext;
        public StudentRepository(CollegeDBContext dBContext) : base(dBContext)
        {
            _dBContext = dBContext;
        }

        public Task<List<Student>> GetAllFeesStructure()
        {
            return _dBContext.Students.Include(s => s.Id).ToListAsync();
        }
    }
}
