using AutoMapper.QueryableExtensions;

namespace API.Data;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public MessageRepository(DataContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;

    }

    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
       return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await _context.Groups
            .Include(g => g.Connections)
            .Where(g => g.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.Include(m => m.Sender).Include(m => m.Recipient).SingleOrDefaultAsync(x => x.Id == id );
    }

    public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
    {
        var query = _context.Messages
            .OrderByDescending(m => m.MessageSent)
            .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(u => u.RecipientUserName == messageParams.UserName && u.RecipientDeleted == false),
            "Outbox" => query.Where(u => u.SenderUserName == messageParams.UserName && u.SenderDeleted == false),
            _ => query.Where(u => u.RecipientUserName == messageParams.UserName && u.RecipientDeleted == false && u.DateRead == null),
        };

        return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.pageSize);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
       return await _context.Groups
        .Include(x => x.Connections)
        .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
    {
        var messages = await _context.Messages
                                    .Where(m => m.Recipient.UserName == currentUserName && m.RecipientDeleted == false
                                                && m.Sender.UserName == recipientUserName
                                                || m.Recipient.UserName == recipientUserName
                                                && m.Sender.UserName == currentUserName && m.SenderDeleted == false
                                    )
                                    .OrderBy(m => m.MessageSent)
                                    .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                                    .ToListAsync();

        var unreadMessages = messages.Where(m => m.DateRead == null
                                                && m.RecipientUserName == currentUserName).ToList();

        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.DateRead = DateTime.UtcNow;
            }
        }

        return messages;
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }
}