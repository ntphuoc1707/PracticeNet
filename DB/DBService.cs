using DB.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public class DBService : IDbService
    {
        private AppDbContext appDbContext;
        public DBService()
        {
            DropDatabase();
            appDbContext = new AppDbContext();
        }
        private void CreateDatabase()
        {
            appDbContext.Database.EnsureCreated();
        }
        private void DropDatabase()
        {
            appDbContext.Database.EnsureDeleted();
        }
    }
}
