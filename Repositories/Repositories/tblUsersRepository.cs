
using Dapper;
using Models.DTOs.UsersDTO;
using Models.Enums;
using Models.Models;
using System.Data;

namespace Repositories.Repositories
{
    public class tblUsersRepository : tblGenericRepository<tblUsers>
    {
  
        public async Task<tblUsers> GetByUsername(string username)
        {
            string procedureName = StoredProcedures.tblUsers_GetByUsername.ToString();
            return await _connection.QueryFirstOrDefaultAsync<tblUsers>
                  (procedureName, new { Username = username }, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
        }
        public async Task<UserRolesDTO> GetByIdWithRoles(int id)
        {
            tblUserRoleRepository _userRefRoleRepo = new tblUserRoleRepository();
            var user = await GetById(id);
            var roles = await _userRefRoleRepo.GetByUserId(id);
            var result = new UserRolesDTO
            {
                UserId = id,
                EmployeeId = user?.EmployeeId,
                RoleId = user?.RoleId,
                Username = user?.UserName,
                Roles = roles,
            };
            return result;
        }
    }
}
