using CP.Data.Domain;
using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;

namespace CP.Data.Repositories.Implementations
{
    public class RefereshTokenRepository : GenericRepository<RefreshToken>, IRefereshTokenRepository
    {
        private readonly CPDatabaseContext _dbContext;
        public RefereshTokenRepository(CPDatabaseContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
