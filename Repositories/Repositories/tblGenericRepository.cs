using Dapper;
using Models.Enums;
using Repositories.Context;
using System.Data;

namespace Repositories.Repositories
{
    public class tblGenericRepository<T> where T : class
    {
        public IDbConnection _connection;
        public int _commandTimeout;
        public string tableName;

        public tblGenericRepository(string connectionString = "DefaultSqlConnection")
        {
            _connection = new ApplicationContext(connectionString).CreateConnection();
            _commandTimeout = 120;
            tableName = typeof(T).Name;
        }

        private string ProcedureName(ProcedureTypes procedureType)
        {
            return $"{tableName}_{procedureType.ToString()}";
        }

        public virtual async Task<IEnumerable<T>> GetAll()
        {
            var procedureName = ProcedureName(ProcedureTypes.GetAll);
            var result = await _connection.QueryAsync<T>(procedureName, commandTimeout: _commandTimeout,
            commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        public virtual async Task<T?> GetById(int id)
        {
            var procedureName = ProcedureName(ProcedureTypes.GetById);
            return await _connection.QueryFirstOrDefaultAsync<T>
                  (procedureName.ToString(), new { Id = id }, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
        }
        public virtual async Task<T?> Insert(T parameters)
        {
            var procedureName = ProcedureName(ProcedureTypes.Insert);
            return await _connection.QueryFirstOrDefaultAsync<T>
                  (procedureName.ToString(), parameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
        }
        public virtual async Task<IEnumerable<T>> InsertMany(IEnumerable<T> parameters)
        {
            List<T> results = new List<T>();
            foreach (var parameter in parameters)
            {
                var newData = await Insert(parameter);
                results.Add(newData);
            }
            return results;
        }
        public virtual async Task<T?> Update(T parameters)
        {
            var procedureName = ProcedureName(ProcedureTypes.Update);
            return await _connection.QueryFirstOrDefaultAsync<T>
                  (procedureName.ToString(), parameters, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
        }
        public virtual async Task<T?> DeleteById(int id)
        {
            var deletedData = await GetById(id);
            var procedureName = ProcedureName(ProcedureTypes.DeleteById);
            _connection.Execute(procedureName.ToString(), new { Id = id }, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
            return deletedData;
        }
        public virtual async Task<T?> InsertOrUpdate(int? id, T data)
        {
            if (id == null || id == 0) return await Insert(data);
            return await Update(data);
        }
        public async Task<DataTable> InsertManyDT(DataTable dt)
        {
            var procedureName = ProcedureName(ProcedureTypes.InsertMany);
            await _connection.ExecuteAsync(procedureName.ToString(), new { TVP = dt.AsTableValuedParameter($"TVP_{tableName}") }, commandType: CommandType.StoredProcedure);
            return dt;
        }
        public async Task<DataTable> UpdateManyDT(DataTable dt)
        {
            var procedureName = ProcedureName(ProcedureTypes.UpdateMany);
            await _connection.ExecuteAsync(procedureName.ToString(), new { TVP = dt.AsTableValuedParameter($"TVP_{tableName}") }, commandType: CommandType.StoredProcedure);
            return dt;
        }
    }
}
