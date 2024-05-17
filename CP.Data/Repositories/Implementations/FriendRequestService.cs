using CP.Data.Domain;
using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;

namespace CP.Data.Repositories.Implementations
{
    public class FriendRequestService : GenericRepository<FriendRequest> ,IFriendRequestService
    {
        private readonly CPDatabaseContext _dbContext;
        public FriendRequestService(CPDatabaseContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
