namespace DistChat.Node.Functionality.Database.Chat;

public interface IMessageDbService
{
    Task<Message> CreateAsync(Guid userId, Guid roomId, string content);

    Task<IReadOnlyList<Message>> GetMessagesAsync(
        Guid inquiringUserId,
        Guid roomId, 
        int limit, 
        Guid? before = null, 
        Guid? after = null, 
        bool newestFirst = false
    );
}