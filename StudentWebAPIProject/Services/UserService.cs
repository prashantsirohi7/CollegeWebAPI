using AutoMapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using StudentWebAPIProject.DBSets.Repository;
using StudentWebAPIProject.Models;
using System.Security.Cryptography;

namespace StudentWebAPIProject.Services
{
    public class UserService : IUserService
    {
        private readonly ICollegeRepository<User> _repository;
        private readonly IMapper _mapper;
        public UserService(ICollegeRepository<User> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public (string PasswordHash, string PasswordSalt) GeneratePasswordHashWithSalt(string password)
        {
            // Create the salt
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            //Create Password Hash
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));

            return (hash, Convert.ToBase64String(salt));
        }

        public async Task<UserReadOnlyDTO> CreateUser(UserDTO userDTO)
        {
            ArgumentNullException.ThrowIfNull(userDTO, nameof(userDTO));

            if(string.IsNullOrEmpty(userDTO.Username) || string.IsNullOrEmpty(userDTO.Password))
                throw new ArgumentNullException($"Username - {userDTO.Username} or Password - {userDTO.Password} is empty");

            if((await _repository.GetByFilterAsync(user => user.Username == userDTO.Username)) != null)
                throw new ArgumentException($"User with username - {userDTO.Username} already exists");

            var user = _mapper.Map<User>(userDTO);
            user.IsDeleted = false;
            user.CreatedDate = DateTime.Now;
            user.ModifiedDate = DateTime.Now;

            var pwdhash = GeneratePasswordHashWithSalt(userDTO.Password);
            user.Password = pwdhash.PasswordHash;
            user.PasswordSalt = pwdhash.PasswordSalt;

            await _repository.CreateAsync(user);
            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<UserReadOnlyDTO> GetUserById(int id)
        {
            if (id <= 0)
                throw new ArgumentException($"Invalid User Id - {id}");
            
            var user = await _repository.GetByFilterAsync(user => user.Id == id);

            if (user == null)
                throw new ArgumentException($"No User found with Id - {id}");

            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<List<UserReadOnlyDTO>> GetAllUsers()
        {
            var users = await _repository.GetAllAsync();
            return _mapper.Map<List<UserReadOnlyDTO>>(users);
        }

        public async Task<UserReadOnlyDTO> GetUserByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException($"Invalid User name - {name}");

            var user = await _repository.GetByFilterAsync(user => user.Username == name);

            if (user == null)
                throw new ArgumentException($"No User found with name - {name}");

            return _mapper.Map<UserReadOnlyDTO>(user);
        }

        public async Task<List<UserReadOnlyDTO>> GetUserByUserTypeId(int userTypeId)
        {
            if (userTypeId <= 0)
                throw new ArgumentException($"Invalid UserType Id - {userTypeId}");

            var user = await _repository.GetAllByFilterAsync(user => user.UserTypeId == userTypeId);

            if (user == null)
                throw new ArgumentException($"No User found with UserType Id - {userTypeId}");

            return _mapper.Map<List<UserReadOnlyDTO>>(user);
        }

        public async Task<UserReadOnlyDTO> UpdateUser(UserReadOnlyDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));

            if (dto.Id <= 0)
                throw new ArgumentException($"Invalid User Id - {dto.Id}");

            var user = await _repository.GetByFilterAsync(user => user.Id == dto.Id, true);

            if (user == null)
                throw new ArgumentException($"No User found with Id - {dto.Id}");

            var newUser = _mapper.Map<User>(dto);
            newUser.ModifiedDate = DateTime.Now;
            newUser.Password = user.Password;
            newUser.PasswordSalt = user.PasswordSalt;
            await _repository.UpdateAsync(newUser);

            return _mapper.Map<UserReadOnlyDTO>(newUser);
        }

        public async Task<bool> DeleteUserById(int id)
        {
            if (id <= 0)
                throw new ArgumentException($"Invalid User Id - {id}");

            var user = await _repository.GetByFilterAsync(user => user.Id == id);

            if (user == null)
                throw new ArgumentException($"No User found with Id - {id}");

            await _repository.DeleteAsync(user);

            return true;
        }

        public async Task<bool> UpdatePassword(int id, string password)
        {
            if (id <= 0)
                throw new ArgumentException($"Invalid User Id - {id}");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentException($"Invalid password - {password}");

            var user = await _repository.GetByFilterAsync(user => user.Id == id, true);

            if (user == null)
                throw new ArgumentException($"No User found with Id - {id}");

            //var newUser = _mapper.Map<User>(dto);
            user.ModifiedDate = DateTime.Now;

            var pwdhash = GeneratePasswordHashWithSalt(password);
            user.Password = pwdhash.PasswordHash;
            user.PasswordSalt = pwdhash.PasswordSalt;

            await _repository.UpdateAsync(user);
            return true;
        }
    }
}
