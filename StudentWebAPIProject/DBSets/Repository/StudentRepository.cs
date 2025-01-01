using Microsoft.EntityFrameworkCore;
using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.DBSets.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly CollegeDBContext _dBContext;
        public StudentRepository(CollegeDBContext dBContext)
        {
            _dBContext = dBContext;
        }
        public async Task<int> CreateStudentAsync(Student student)
        {
            await _dBContext.Students.AddAsync(student);
            await _dBContext.SaveChangesAsync();
            return student.Id;
        }

        public async Task<bool> DeleteStudentAsync(Student student)
        {
            _dBContext.Remove(student);
            await _dBContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _dBContext.Students.ToListAsync();
        }

        public async Task<Student> GetStudentByIdAsync(int id, bool asNoTracking = false)
        {
            if(asNoTracking)
                return await _dBContext.Students.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
            else
                return await _dBContext.Students.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Student> GetStudentByNameAsync(string name)
        {
            return await _dBContext.Students.Where(x => x.Name == name).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateStudentAsync(Student student)
        {
            _dBContext.Students.Update(student);
            await _dBContext.SaveChangesAsync();
            return student.Id;
        }
    }
}
