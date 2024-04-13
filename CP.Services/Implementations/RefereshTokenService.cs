using CP.Data.Repositories.Interfaces;
using CP.Models.Entities;
using CP.Services.Interfaces;
using System.Security.Cryptography;

namespace CP.Services.Implementations
{
    public class RefereshTokenService : IRefereshTokenService
    {
        private readonly IRefereshTokenRepository _refereshTokenRepository;
        public RefereshTokenService(IRefereshTokenRepository refereshTokenRepository)
        {
            _refereshTokenRepository = refereshTokenRepository;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomnumber = new byte[32];
            using (var ramdomnumbergenerator = RandomNumberGenerator.Create())
            {
                ramdomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);
                var token = _refereshTokenRepository.GetFilteredData(x => x.UserId == username) as RefreshToken;
                if (token != null)
                {
                    token.Refresh_Token = refreshtoken;
                }
                else
                {
                    _refereshTokenRepository.Insert(new RefreshToken()
                    {
                        UserId = username,
                        TokenId = new Random().Next().ToString(),
                        Refresh_Token = refreshtoken,
                        IsActive = true
                    });
                }
                _refereshTokenRepository.Save();

                return refreshtoken;
            }
        }

        public bool TokenExists(string userId, string refreshtoken)
        {
            var user = _refereshTokenRepository.GetFilteredData(x => x.UserId == userId && x.Refresh_Token == refreshtoken);

            return user.Any();
        }
    }
}
