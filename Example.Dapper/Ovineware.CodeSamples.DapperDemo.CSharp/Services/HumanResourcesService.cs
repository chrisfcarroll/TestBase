using System.Collections.Generic;
using Ovineware.CodeSamples.DapperDemo.CSharp.Models;
using Ovineware.CodeSamples.DapperDemo.CSharp.Repositories;

namespace Ovineware.CodeSamples.DapperDemo.CSharp.Services
{
    public class HumanResourcesService
    {
        private HumanResourcesRepository humanResourcesRepository;

        public HumanResourcesService()
            : this(new HumanResourcesRepository())
        {
        }

        public HumanResourcesService(HumanResourcesRepository humanResourcesRepository)
        {
            this.humanResourcesRepository = humanResourcesRepository;
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return humanResourcesRepository.SelectEmployees();
        }

        public IEnumerable<Manager> GetManagers(int employeeId)
        {
            return humanResourcesRepository.SelectManagers(employeeId);
        }
    }
}