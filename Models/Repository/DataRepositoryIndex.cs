using Microsoft.Extensions.DependencyInjection;
using TCU.English.Models.DataManager;

namespace TCU.English.Models.Repository
{
    public static class DataRepositoryIndex
    {
        // Khi khai báo mới một lớp DataManager kế thừa từ IDataRepository thì khai báo vào đây
        public static void AddDataRepositoryScope(this IServiceCollection services)
        {
            services.AddScoped<IDataRepository<User>, UserManager>();
            services.AddScoped<IDataRepository<UserType>, UserTypeManager>();
            services.AddScoped<IDataRepository<UserTypeUser>, UserTypeUserManager>();
            services.AddScoped<IDataRepository<TestCategory>, TestCategoryManager>();
            services.AddScoped<IDataRepository<ReadingPartOne>, ReadingPartOneManager>();
            services.AddScoped<IDataRepository<ReadingPartTwo>, ReadingPartTwoManager>();
            services.AddScoped<IDataRepository<ListeningMedia>, ListeningMediaManager>();
            services.AddScoped<IDataRepository<ListeningBaseQuestion>, ListeningBaseQuestionManager>();
            services.AddScoped<IDataRepository<WritingPartOne>, WritingPartOneManager>();
            services.AddScoped<IDataRepository<WritingPartTwo>, WritingPartTwoManager>();
            services.AddScoped<IDataRepository<SpeakingEmbed>, SpeakingEmbedManager>();
            services.AddScoped<IDataRepository<PieceOfTest>, PieceOfTestManager>();
            services.AddScoped<IDataRepository<Discussion>, DiscussionManager>();
            services.AddScoped<IDataRepository<DiscussionUser>, DiscussionUserManager>();
            services.AddScoped<IDataRepository<DiscussionUserMessage>, DiscussionUserMessageManager>();
        }
    }
}
