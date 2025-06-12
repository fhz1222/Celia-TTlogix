using Microsoft.Extensions.Configuration;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.PetaPoco
{
    public interface IPPDbContextFactory
    {
        Database GetInstance();
    }

    public class PetaPocoDbContextFactory : IPPDbContextFactory
    {
        private readonly string connectionString;
        private readonly string dbProviderName;

        public PetaPocoDbContextFactory(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("Database");
            dbProviderName = "System.Data.SqlClient";
        }

        private Database? dbContext;
        public Database GetInstance()
        {
            if (dbContext == null)
                dbContext = new Database(connectionString, dbProviderName);
            return dbContext;
        }
    }
}
