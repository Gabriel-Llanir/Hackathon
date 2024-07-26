using MongoDB.Driver;
using Moq;
using Moq.Language.Flow;
using System.Threading;
using System.Threading.Tasks;

namespace TECH_CHALLENGE_Tests.Helpers
{
    public static class MongoCollectionExtensions
    {
        public static ISetup<IMongoCollection<TDocument>, Task<IAsyncCursor<TDocument>>> SetupFindAsync<TDocument>(
            this Mock<IMongoCollection<TDocument>> mock,
            FilterDefinition<TDocument> filter,
            FindOptions<TDocument, TDocument> options = null,
            CancellationToken cancellationToken = default)
        {
            return mock.Setup(x => x.FindAsync(filter, options, cancellationToken));
        }
    }
}
