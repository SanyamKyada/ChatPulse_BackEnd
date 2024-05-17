namespace CP.Models.Models
{
    public class FriendRequestDto
    {
        public string UserId { get; set; }
        public string ReceiverUserId { get; set; }

        public bool HasWaved { get; set; }
    }
}
