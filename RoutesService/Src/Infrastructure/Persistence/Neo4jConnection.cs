using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;
namespace RoutesService.Src.Infrastructure.Persistence
{
    public class Neo4jConnection : IDisposable
    {
       
        public IDriver Driver { get; }

        
        public Neo4jConnection(IConfiguration configuration)
        {
            var uri = configuration["Neo4j:Uri"];
            var user = configuration["Neo4j:User"];
            var password = configuration["Neo4j:Password"];

            
            Driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        
        public void Dispose()
        {
            Driver?.Dispose();
        }
    }
}