using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace TestBase.Example.WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly ISimpleDataSource dataSource;

        public ValuesController(ISimpleDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        // GET api/values
        public IEnumerable<string> Get()
        {
            return dataSource.GetAll();
        }

        // GET api/values/5
        public string Get(int id)
        {
            return dataSource.Get(id);
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            dataSource.Add(value);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
            dataSource.Modify(id, value);
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
            dataSource.Delete(id);
        }
    }

    public interface ISimpleDataSource
    {
        IEnumerable<string> GetAll();
        void Add(string item);
        void Delete(int id);
        void Modify(int id, string newValue);
        string Get(int id);
    }

    public class ExampleDataSource : ISimpleDataSource
    {
        private static readonly object cachelock= new object();
        private static readonly Dictionary<int, string> DataSource = new Dictionary<int,string> { {1,"value1"}, {2,"value2"}, {3,"Three"}};

        public IEnumerable<string> GetAll()    { return DataSource.Values;} 

        public void Add(string item)
        {
            lock (cachelock){ DataSource.Add(DataSource.Keys.Max()+1, item); }
        }

        public void Delete(int id)
        {
            lock (cachelock){ DataSource.Remove(id); }
        }

        public void Modify(int id, string newValue)
        {
            lock (cachelock)
            {
                DataSource.Remove(id);
                DataSource.Add(id,newValue);
            }
        }

        public string Get(int id)
        {
            return DataSource[id];
        }
    }
}