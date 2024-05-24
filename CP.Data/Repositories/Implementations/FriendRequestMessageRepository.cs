using CP.Data.Domain;
using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;

namespace CP.Data.Repositories.Implementations
{
    public class FriendRequestMessageRepository: GenericRepository<FriendRequestMessage>, IFriendRequestMessageRepository
    {
        public FriendRequestMessageRepository(CPDatabaseContext dbContext) : base(dbContext)
        {
            
        }
    }
}
