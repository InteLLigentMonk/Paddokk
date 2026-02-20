using Paddokk.Core.Interfaces;

namespace Paddokk.Data.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly PaddokkDbContext _db;

    public CommentRepository(PaddokkDbContext db)
    {
        _db = db;
    }


}
