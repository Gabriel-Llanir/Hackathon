using MongoDB.Driver;
using Moq;
using Moq.Language.Flow;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationTests.Helpers
{
    public static class MongoCollectionExtensions
    {
        #region | Primary Test DataBase 
        public static string Get_Test_DB_URL()
        {
            return Environment.GetEnvironmentVariable("TestDataBase") ?? GetSecondary_DataBase_URL();
        }
        #endregion

        #region | Secondary Test DataBase
        public static ISetup<IMongoCollection<TDocument>, Task<IAsyncCursor<TDocument>>> SetupFindAsync<TDocument>(
            this Mock<IMongoCollection<TDocument>> mock,
            FilterDefinition<TDocument> filter,
            FindOptions<TDocument, TDocument> options = null,
            CancellationToken cancellationToken = default)
        {
            return mock.Setup(x => x.FindAsync(filter, options, cancellationToken));
        }

        public static string GetSecondary_DataBase_URL()
        {
            return "";
        }
        #endregion
    }
}