using CP.Data.Domain;
using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;

namespace CP.Data.Repositories.Implementations
{
    public class FriendRequestRepository : GenericRepository<FriendRequest> ,IFriendRequestRepository
    {
        private readonly CPDatabaseContext _dbContext;
        public FriendRequestRepository(CPDatabaseContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
