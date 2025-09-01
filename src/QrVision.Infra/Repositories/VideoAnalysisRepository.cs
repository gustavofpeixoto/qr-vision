using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using QrVision.Domain.Entities;
using QrVision.Infra.Settings;
using MongoDB.Driver;
using QrVision.Domain.Interfaces.Repositories;

namespace QrVision.Infra.Repositories
{
    public class VideoAnalysisRepository : IVideoAnalysisRepository
    {
        private readonly IMongoCollection<VideoAnalysis> _videoAnalysisCollection;

        static VideoAnalysisRepository()
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            BsonClassMap.RegisterClassMap<VideoAnalysis>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdProperty(p => p.Id);
            });

            BsonClassMap.RegisterClassMap<QrCodeResult>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

        public VideoAnalysisRepository(MongoDbSettings mongoDbSettings)
        {
            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);

            _videoAnalysisCollection = mongoDatabase.GetCollection<VideoAnalysis>("video-analysis");
        }

        public Task AddAsync(VideoAnalysis videoAnalysis) => _videoAnalysisCollection.InsertOneAsync(videoAnalysis);

        public async Task<VideoAnalysis> GetByIdAsync(Guid id)
            => await _videoAnalysisCollection.Find(va => va.Id == id).SingleOrDefaultAsync();

        public async Task UpdateAsync(VideoAnalysis videoAnalysis)
            => await _videoAnalysisCollection.ReplaceOneAsync(va => va.Id == videoAnalysis.Id, videoAnalysis);
    }
}
