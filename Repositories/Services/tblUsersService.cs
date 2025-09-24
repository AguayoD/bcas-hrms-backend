
using Models.DTOs.UsersDTO;
using Models.Models;
using Repositories.Repositories;

namespace Repositories.Services
{
    public class tblUsersService
    {
        private readonly tblUsersRepository _tblUsersRepository = new tblUsersRepository();
        private readonly AuthenticationService _authenticationService = new AuthenticationService();

        public async Task<IEnumerable<tblUsers>> GetAll()
        {
            return await _tblUsersRepository.GetAll();
        }

        public async Task<tblUsers> GetById(int id)
        {
            return await _tblUsersRepository.GetById(id);
        }

        public async Task<tblUsers> GetByUsername(string username)
        {
            return await _tblUsersRepository.GetByUsername(username);
        }
        public async new Task<tblUsers> Insert(UserInsertDTO userData)
        {
            try
            {
                var salt = _authenticationService.GenerateRandomSalt();
                var newPassword = _authenticationService.ToHash(userData.NewPassword, salt);

                var newUser = new tblUsers()
                {
                    EmployeeId = userData.EmployeeId,
                    RoleId = userData.RoleId,
                    UserName = userData.Username,
                    PasswordHash = newPassword,
                    Salt = Convert.ToBase64String(salt),    
                
                };

                var insertedUser = await _tblUsersRepository.Insert(newUser);

                if (insertedUser?.UserId == null)
                {
                    throw new Exception("Failed to insert user");
                }

                return insertedUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error inserting user with roles: {ex.Message}", ex);
            }
        }
        public async Task<UserRolesDTO> GetByIdWithRoles(int id)
        {
            return await _tblUsersRepository.GetByIdWithRoles(id);
        }

        public async Task<tblUsers> Update(tblUsers tblemployee)
        {
            return await _tblUsersRepository.Update(tblemployee);
        }

        public async Task<tblUsers> DeleteById(int id)
        {
            return await _tblUsersRepository.DeleteById(id);
        }

        public (string? hashedPassword, string? salt) GeneratePasswordHash(string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}
