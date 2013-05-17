using System.Collections.Generic;
using System.Data;
using Dapper;
using Ovineware.CodeSamples.DapperDemo.CSharp.Models;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Repositories
{
    public class HumanResourcesRepository : BaseRepository
    {
        public IEnumerable<Employee> SelectEmployees()
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string query = "SELECT e.EmployeeID AS Id, c.FirstName, c.MiddleName, c.LastName " +
                                     "FROM HumanResources.Employee e " +
                                     "INNER JOIN Person.Contact c ON e.ContactId = c.ContactId";
                return connection.Query<Employee>(query);
            }
        }

        public IEnumerable<Manager> SelectManagers(int employeeId)
        {
            using (IDbConnection connection = OpenConnection())
            {
                const string storedProcedure = "dbo.uspGetEmployeeManagers";
                return connection.Query<Manager>(storedProcedure, new { EmployeeID = employeeId }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}