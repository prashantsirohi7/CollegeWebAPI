using StudentWebAPIProject.Models;

namespace StudentWebAPIProject.Services
{
    public interface IUserService
    {
        public Task<UserReadOnlyDTO> CreateUser(UserDTO userDTO);
        public Task<UserReadOnlyDTO> GetUserById(int id);
        public Task<List<UserReadOnlyDTO>> GetAllUsers();
        public Task<UserReadOnlyDTO> GetUserByName(string name);
        public Task<List<UserReadOnlyDTO>> GetUserByUserTypeId(int userTypeId);
        public Task<UserReadOnlyDTO> UpdateUser(UserReadOnlyDTO dto);
        public Task<bool> DeleteUserById(int id);
        public Task<bool> UpdatePassword(int id, string password);
        public (string PasswordHash, string PasswordSalt) GeneratePasswordHashWithSalt(string password);
    }
}
