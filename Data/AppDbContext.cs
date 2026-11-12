using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<Level> Levels => Set<Level>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Word> Words => Set<Word>();
    public DbSet<WordExample> WordExamples => Set<WordExample>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
    public DbSet<QuizAnswer> QuizAnswers => Set<QuizAnswer>();
    public DbSet<FavoriteWord> FavoriteWords => Set<FavoriteWord>();
    public DbSet<UserWordProgress> UserWordProgress => Set<UserWordProgress>();
    public DbSet<LessonProgress> LessonProgress => Set<LessonProgress>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        b.Entity<User>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(256).IsRequired();
            e.Property(x => x.DisplayName).HasMaxLength(100).IsRequired();
            e.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.AvatarKey).HasMaxLength(50).HasDefaultValue("owl");
            e.Property(x => x.CompanionKey).HasMaxLength(50).HasDefaultValue("auto");
            e.Property(x => x.Bio).HasMaxLength(300);
            e.Property(x => x.DailyGoalXp).HasDefaultValue(50);
            e.Property(x => x.ExperiencePoints).HasDefaultValue(0);
            e.Property(x => x.TodayXp).HasDefaultValue(0);
            e.HasOne(x => x.PreferredLanguage).WithMany().HasForeignKey(x => x.PreferredLanguageId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<Language>(e =>
        {
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.Code).HasMaxLength(10).IsRequired();
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.Property(x => x.NativeName).HasMaxLength(50).IsRequired();
            e.Property(x => x.ProficiencySystem).HasMaxLength(20).IsRequired();
            e.Property(x => x.SpeechLocale).HasMaxLength(10).IsRequired();
        });

        b.Entity<Level>(e =>
        {
            e.HasIndex(x => new { x.LanguageId, x.Code }).IsUnique();
            e.Property(x => x.Code).HasMaxLength(20).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.HasOne(x => x.Language).WithMany(l => l.Levels).HasForeignKey(x => x.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Topic>(e =>
        {
            e.HasIndex(x => new { x.LanguageId, x.Name }).IsUnique();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Icon).HasMaxLength(20);
            e.HasOne(x => x.Language).WithMany(l => l.Topics).HasForeignKey(x => x.LanguageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Word>(e =>
        {
            e.HasIndex(x => new { x.LanguageId, x.Term });
            e.HasIndex(x => x.Meaning);
            e.Property(x => x.Term).HasMaxLength(200).IsRequired();
            e.Property(x => x.Reading).HasMaxLength(200);
            e.Property(x => x.Meaning).HasMaxLength(500).IsRequired();
            e.Property(x => x.PartOfSpeech).HasMaxLength(50);
            e.HasOne(x => x.Language).WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Level).WithMany().HasForeignKey(x => x.LevelId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Topic).WithMany().HasForeignKey(x => x.TopicId).OnDelete(DeleteBehavior.NoAction);
        });

        b.Entity<WordExample>(e =>
        {
            e.Property(x => x.Sentence).HasMaxLength(1000).IsRequired();
            e.Property(x => x.Reading).HasMaxLength(1000);
            e.Property(x => x.Translation).HasMaxLength(1000).IsRequired();
            e.HasOne(x => x.Word).WithMany(w => w.Examples).HasForeignKey(x => x.WordId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<Lesson>(e =>
        {
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Type).HasConversion<string>().HasMaxLength(20);
            e.HasOne(x => x.Language).WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Level).WithMany().HasForeignKey(x => x.LevelId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<Question>(e =>
        {
            e.Property(x => x.Text).HasMaxLength(1000).IsRequired();
            e.Property(x => x.Explanation).HasMaxLength(1000);
            e.HasOne(x => x.Language).WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Level).WithMany().HasForeignKey(x => x.LevelId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Lesson).WithMany(l => l.Questions).HasForeignKey(x => x.LessonId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        b.Entity<QuestionOption>(e =>
        {
            e.Property(x => x.Text).HasMaxLength(500).IsRequired();
            e.HasOne(x => x.Question).WithMany(q => q.Options).HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        b.Entity<FavoriteWord>(e =>
        {
            e.HasKey(x => new { x.UserId, x.WordId });
            e.HasOne(x => x.User).WithMany(u => u.Favorites).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Word).WithMany().HasForeignKey(x => x.WordId).OnDelete(DeleteBehavior.NoAction);
        });

        b.Entity<UserWordProgress>(e =>
        {
            e.HasKey(x => new { x.UserId, x.WordId });
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            e.ToTable(t => t.HasCheckConstraint("CK_UserWordProgress_Box", "[Box] BETWEEN 0 AND 5"));
            e.HasIndex(x => new { x.UserId, x.NextReviewAt });
            e.HasOne(x => x.User).WithMany(u => u.WordProgress).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Word).WithMany().HasForeignKey(x => x.WordId).OnDelete(DeleteBehavior.NoAction);
        });

        b.Entity<LessonProgress>(e =>
        {
            e.HasKey(x => new { x.UserId, x.LessonId });
            e.HasOne(x => x.User).WithMany(u => u.LessonProgress).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Lesson).WithMany().HasForeignKey(x => x.LessonId).OnDelete(DeleteBehavior.NoAction);
        });

        b.Entity<QuizAttempt>(e =>
        {
            e.ToTable(t => t.HasCheckConstraint("CK_QuizAttempt_Score", "[Score] >= 0 AND [Score] <= [TotalQuestions]"));
            e.HasIndex(x => new { x.UserId, x.CompletedAt });
            e.HasOne(x => x.User).WithMany(u => u.QuizAttempts).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Language).WithMany().HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Level).WithMany().HasForeignKey(x => x.LevelId).OnDelete(DeleteBehavior.Restrict);
        });

        b.Entity<QuizAnswer>(e =>
        {
            e.HasOne(x => x.QuizAttempt).WithMany(a => a.Answers).HasForeignKey(x => x.QuizAttemptId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Question).WithMany().HasForeignKey(x => x.QuestionId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.SelectedOption).WithMany().HasForeignKey(x => x.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        SeedData.Apply(b);
    }
}
