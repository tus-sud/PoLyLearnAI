using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PolyLearnAI.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProficiencySystem = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SpeechLocale = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Levels_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topics_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PreferredLanguageId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Languages_PreferredLanguageId",
                        column: x => x.PreferredLanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lessons_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    TopicId = table.Column<int>(type: "int", nullable: true),
                    Term = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Reading = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Meaning = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PartOfSpeech = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Words_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Words_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Words_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QuizAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAttempts", x => x.Id);
                    table.CheckConstraint("CK_QuizAttempt_Score", "[Score] >= 0 AND [Score] <= [TotalQuestions]");
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonProgress",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonProgress", x => new { x.UserId, x.LessonId });
                    table.ForeignKey(
                        name: "FK_LessonProgress_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<int>(type: "int", nullable: true),
                    Text = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Questions_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Questions_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteWords",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WordId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteWords", x => new { x.UserId, x.WordId });
                    table.ForeignKey(
                        name: "FK_FavoriteWords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoriteWords_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserWordProgress",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WordId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Box = table.Column<int>(type: "int", nullable: false),
                    ReviewCount = table.Column<int>(type: "int", nullable: false),
                    LastReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextReviewAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWordProgress", x => new { x.UserId, x.WordId });
                    table.CheckConstraint("CK_UserWordProgress_Box", "[Box] BETWEEN 0 AND 5");
                    table.ForeignKey(
                        name: "FK_UserWordProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWordProgress_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WordExamples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordId = table.Column<int>(type: "int", nullable: false),
                    Sentence = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Reading = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Translation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordExamples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WordExamples_Words_WordId",
                        column: x => x.WordId,
                        principalTable: "Words",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizAttemptId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedOptionId = table.Column<int>(type: "int", nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_QuestionOptions_SelectedOptionId",
                        column: x => x.SelectedOptionId,
                        principalTable: "QuestionOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizAnswers_QuizAttempts_QuizAttemptId",
                        column: x => x.QuizAttemptId,
                        principalTable: "QuizAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "Name", "NativeName", "ProficiencySystem", "SpeechLocale" },
                values: new object[,]
                {
                    { 1, "en", "Tiếng Anh", "English", "CEFR", "en-US" },
                    { 2, "zh", "Tiếng Trung", "中文", "HSK", "zh-CN" },
                    { 3, "ja", "Tiếng Nhật", "日本語", "JLPT", "ja-JP" },
                    { 4, "ko", "Tiếng Hàn", "한국어", "TOPIK", "ko-KR" }
                });

            migrationBuilder.InsertData(
                table: "Levels",
                columns: new[] { "Id", "Code", "LanguageId", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "A1", 1, "Sơ cấp A1", 1 },
                    { 2, "A2", 1, "Sơ cấp A2", 2 },
                    { 3, "HSK1", 2, "HSK 1", 1 },
                    { 4, "HSK2", 2, "HSK 2", 2 },
                    { 5, "N5", 3, "JLPT N5", 1 },
                    { 6, "N4", 3, "JLPT N4", 2 },
                    { 7, "TOPIK1", 4, "TOPIK I - Cấp 1", 1 },
                    { 8, "TOPIK2", 4, "TOPIK I - Cấp 2", 2 }
                });

            migrationBuilder.InsertData(
                table: "Topics",
                columns: new[] { "Id", "Description", "Icon", "LanguageId", "Name" },
                values: new object[,]
                {
                    { 1, "Câu chào và lịch sự cơ bản", "👋", 1, "Chào hỏi" },
                    { 2, "Đồ ăn thức uống", "🍎", 1, "Ẩm thực" },
                    { 3, "Di chuyển và đi lại", "✈️", 1, "Du lịch" },
                    { 4, "Câu chào và lịch sự cơ bản", "👋", 2, "Chào hỏi" },
                    { 5, "Đồ ăn thức uống", "🍎", 2, "Ẩm thực" },
                    { 6, "Di chuyển và đi lại", "✈️", 2, "Du lịch" },
                    { 7, "Câu chào và lịch sự cơ bản", "👋", 3, "Chào hỏi" },
                    { 8, "Đồ ăn thức uống", "🍎", 3, "Ẩm thực" },
                    { 9, "Di chuyển và đi lại", "✈️", 3, "Du lịch" },
                    { 10, "Câu chào và lịch sự cơ bản", "👋", 4, "Chào hỏi" },
                    { 11, "Đồ ăn thức uống", "🍎", 4, "Ẩm thực" },
                    { 12, "Di chuyển và đi lại", "✈️", 4, "Du lịch" }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "Content", "CreatedAt", "IsPublished", "LanguageId", "LevelId", "SortOrder", "Title", "Type" },
                values: new object[,]
                {
                    { 11, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, 1, 1, "Từ vựng cơ bản", "Vocabulary" },
                    { 12, "Tom goes to the airport. He buys a ticket and drinks some water.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, 1, 2, "Luyện nghe: Ở sân bay", "Listening" },
                    { 13, "Tom goes to the airport. He buys a ticket and drinks some water.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 1, 1, 3, "Đọc hiểu: Chuyến đi", "Reading" },
                    { 21, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, 3, 1, "Từ vựng cơ bản", "Vocabulary" },
                    { 22, "我去机场。我买了一张票，还喝了一杯水。", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, 3, 2, "Luyện nghe: Ở sân bay", "Listening" },
                    { 23, "我去机场。我买了一张票，还喝了一杯水。", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 2, 3, 3, "Đọc hiểu: Chuyến đi", "Reading" },
                    { 31, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, 5, 1, "Từ vựng cơ bản", "Vocabulary" },
                    { 32, "私は空港に行きます。切符を買って、水を飲みます。", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, 5, 2, "Luyện nghe: Ở sân bay", "Listening" },
                    { 33, "私は空港に行きます。切符を買って、水を飲みます。", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 3, 5, 3, "Đọc hiểu: Chuyến đi", "Reading" },
                    { 41, null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 4, 7, 1, "Từ vựng cơ bản", "Vocabulary" },
                    { 42, "저는 공항에 갑니다. 표를 사고 물을 마십니다.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 4, 7, 2, "Luyện nghe: Ở sân bay", "Listening" },
                    { 43, "저는 공항에 갑니다. 표를 사고 물을 마십니다.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 4, 7, 3, "Đọc hiểu: Chuyến đi", "Reading" }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Explanation", "LanguageId", "LessonId", "LevelId", "Text" },
                values: new object[,]
                {
                    { 1, "\"Hello\" nghĩa là \"xin chào\".", 1, null, 1, "Nghĩa của \"Hello\" là gì?" },
                    { 2, "\"Thank you\" nghĩa là \"cảm ơn\".", 1, null, 1, "Nghĩa của \"Thank you\" là gì?" },
                    { 3, "\"Apple\" nghĩa là \"quả táo\".", 1, null, 1, "Nghĩa của \"Apple\" là gì?" },
                    { 4, "\"Water\" nghĩa là \"nước\".", 1, null, 1, "Nghĩa của \"Water\" là gì?" },
                    { 5, "\"Airport\" nghĩa là \"sân bay\".", 1, null, 2, "Nghĩa của \"Airport\" là gì?" },
                    { 6, "\"Ticket\" nghĩa là \"vé\".", 1, null, 2, "Nghĩa của \"Ticket\" là gì?" },
                    { 7, "\"你好\" nghĩa là \"xin chào\".", 2, null, 3, "Nghĩa của \"你好\" là gì?" },
                    { 8, "\"谢谢\" nghĩa là \"cảm ơn\".", 2, null, 3, "Nghĩa của \"谢谢\" là gì?" },
                    { 9, "\"苹果\" nghĩa là \"quả táo\".", 2, null, 3, "Nghĩa của \"苹果\" là gì?" },
                    { 10, "\"水\" nghĩa là \"nước\".", 2, null, 3, "Nghĩa của \"水\" là gì?" },
                    { 11, "\"飞机\" nghĩa là \"máy bay\".", 2, null, 4, "Nghĩa của \"飞机\" là gì?" },
                    { 12, "\"火车\" nghĩa là \"tàu hỏa\".", 2, null, 4, "Nghĩa của \"火车\" là gì?" },
                    { 13, "\"こんにちは\" nghĩa là \"xin chào\".", 3, null, 5, "Nghĩa của \"こんにちは\" là gì?" },
                    { 14, "\"ありがとう\" nghĩa là \"cảm ơn\".", 3, null, 5, "Nghĩa của \"ありがとう\" là gì?" },
                    { 15, "\"りんご\" nghĩa là \"quả táo\".", 3, null, 5, "Nghĩa của \"りんご\" là gì?" },
                    { 16, "\"水\" nghĩa là \"nước\".", 3, null, 5, "Nghĩa của \"水\" là gì?" },
                    { 17, "\"空港\" nghĩa là \"sân bay\".", 3, null, 6, "Nghĩa của \"空港\" là gì?" },
                    { 18, "\"切符\" nghĩa là \"vé\".", 3, null, 6, "Nghĩa của \"切符\" là gì?" },
                    { 19, "\"안녕하세요\" nghĩa là \"xin chào\".", 4, null, 7, "Nghĩa của \"안녕하세요\" là gì?" },
                    { 20, "\"감사합니다\" nghĩa là \"cảm ơn\".", 4, null, 7, "Nghĩa của \"감사합니다\" là gì?" },
                    { 21, "\"사과\" nghĩa là \"quả táo\".", 4, null, 7, "Nghĩa của \"사과\" là gì?" },
                    { 22, "\"물\" nghĩa là \"nước\".", 4, null, 7, "Nghĩa của \"물\" là gì?" },
                    { 23, "\"공항\" nghĩa là \"sân bay\".", 4, null, 8, "Nghĩa của \"공항\" là gì?" },
                    { 24, "\"표\" nghĩa là \"vé\".", 4, null, 8, "Nghĩa của \"표\" là gì?" }
                });

            migrationBuilder.InsertData(
                table: "Words",
                columns: new[] { "Id", "CreatedAt", "LanguageId", "LevelId", "Meaning", "PartOfSpeech", "Reading", "Term", "TopicId" },
                values: new object[,]
                {
                    { 101, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, "xin chào", "interjection", "/həˈloʊ/", "Hello", 1 },
                    { 102, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, "cảm ơn", "phrase", "/θæŋk juː/", "Thank you", 1 },
                    { 103, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, "quả táo", "noun", "/ˈæp.əl/", "Apple", 2 },
                    { 104, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 1, "nước", "noun", "/ˈwɔː.tər/", "Water", 2 },
                    { 105, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, "sân bay", "noun", "/ˈeə.pɔːt/", "Airport", 3 },
                    { 106, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 2, "vé", "noun", "/ˈtɪk.ɪt/", "Ticket", 3 },
                    { 201, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3, "xin chào", "interjection", "nǐ hǎo", "你好", 4 },
                    { 202, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3, "cảm ơn", "verb", "xièxie", "谢谢", 4 },
                    { 203, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3, "quả táo", "noun", "píngguǒ", "苹果", 5 },
                    { 204, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 3, "nước", "noun", "shuǐ", "水", 5 },
                    { 205, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 4, "máy bay", "noun", "fēijī", "飞机", 6 },
                    { 206, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 4, "tàu hỏa", "noun", "huǒchē", "火车", 6 },
                    { 301, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 5, "xin chào", "interjection", "konnichiwa", "こんにちは", 7 },
                    { 302, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 5, "cảm ơn", "interjection", "arigatō", "ありがとう", 7 },
                    { 303, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 5, "quả táo", "noun", "ringo", "りんご", 8 },
                    { 304, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 5, "nước", "noun", "mizu", "水", 8 },
                    { 305, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 6, "sân bay", "noun", "kūkō", "空港", 9 },
                    { 306, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 6, "vé", "noun", "kippu", "切符", 9 },
                    { 401, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 7, "xin chào", "interjection", "annyeonghaseyo", "안녕하세요", 10 },
                    { 402, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 7, "cảm ơn", "verb", "gamsahamnida", "감사합니다", 10 },
                    { 403, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 7, "quả táo", "noun", "sagwa", "사과", 11 },
                    { 404, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 7, "nước", "noun", "mul", "물", 11 },
                    { 405, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 8, "sân bay", "noun", "gonghang", "공항", 12 },
                    { 406, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 8, "vé", "noun", "pyo", "표", 12 }
                });

            migrationBuilder.InsertData(
                table: "QuestionOptions",
                columns: new[] { "Id", "IsCorrect", "QuestionId", "Text" },
                values: new object[,]
                {
                    { 1, true, 1, "xin chào" },
                    { 2, false, 1, "cảm ơn" },
                    { 3, false, 1, "quả táo" },
                    { 4, false, 1, "nước" },
                    { 5, true, 2, "cảm ơn" },
                    { 6, false, 2, "quả táo" },
                    { 7, false, 2, "nước" },
                    { 8, false, 2, "sân bay" },
                    { 9, true, 3, "quả táo" },
                    { 10, false, 3, "nước" },
                    { 11, false, 3, "sân bay" },
                    { 12, false, 3, "vé" },
                    { 13, true, 4, "nước" },
                    { 14, false, 4, "sân bay" },
                    { 15, false, 4, "vé" },
                    { 16, false, 4, "xin chào" },
                    { 17, true, 5, "sân bay" },
                    { 18, false, 5, "vé" },
                    { 19, false, 5, "xin chào" },
                    { 20, false, 5, "cảm ơn" },
                    { 21, true, 6, "vé" },
                    { 22, false, 6, "xin chào" },
                    { 23, false, 6, "cảm ơn" },
                    { 24, false, 6, "quả táo" },
                    { 25, true, 7, "xin chào" },
                    { 26, false, 7, "cảm ơn" },
                    { 27, false, 7, "quả táo" },
                    { 28, false, 7, "nước" },
                    { 29, true, 8, "cảm ơn" },
                    { 30, false, 8, "quả táo" },
                    { 31, false, 8, "nước" },
                    { 32, false, 8, "máy bay" },
                    { 33, true, 9, "quả táo" },
                    { 34, false, 9, "nước" },
                    { 35, false, 9, "máy bay" },
                    { 36, false, 9, "tàu hỏa" },
                    { 37, true, 10, "nước" },
                    { 38, false, 10, "máy bay" },
                    { 39, false, 10, "tàu hỏa" },
                    { 40, false, 10, "xin chào" },
                    { 41, true, 11, "máy bay" },
                    { 42, false, 11, "tàu hỏa" },
                    { 43, false, 11, "xin chào" },
                    { 44, false, 11, "cảm ơn" },
                    { 45, true, 12, "tàu hỏa" },
                    { 46, false, 12, "xin chào" },
                    { 47, false, 12, "cảm ơn" },
                    { 48, false, 12, "quả táo" },
                    { 49, true, 13, "xin chào" },
                    { 50, false, 13, "cảm ơn" },
                    { 51, false, 13, "quả táo" },
                    { 52, false, 13, "nước" },
                    { 53, true, 14, "cảm ơn" },
                    { 54, false, 14, "quả táo" },
                    { 55, false, 14, "nước" },
                    { 56, false, 14, "sân bay" },
                    { 57, true, 15, "quả táo" },
                    { 58, false, 15, "nước" },
                    { 59, false, 15, "sân bay" },
                    { 60, false, 15, "vé" },
                    { 61, true, 16, "nước" },
                    { 62, false, 16, "sân bay" },
                    { 63, false, 16, "vé" },
                    { 64, false, 16, "xin chào" },
                    { 65, true, 17, "sân bay" },
                    { 66, false, 17, "vé" },
                    { 67, false, 17, "xin chào" },
                    { 68, false, 17, "cảm ơn" },
                    { 69, true, 18, "vé" },
                    { 70, false, 18, "xin chào" },
                    { 71, false, 18, "cảm ơn" },
                    { 72, false, 18, "quả táo" },
                    { 73, true, 19, "xin chào" },
                    { 74, false, 19, "cảm ơn" },
                    { 75, false, 19, "quả táo" },
                    { 76, false, 19, "nước" },
                    { 77, true, 20, "cảm ơn" },
                    { 78, false, 20, "quả táo" },
                    { 79, false, 20, "nước" },
                    { 80, false, 20, "sân bay" },
                    { 81, true, 21, "quả táo" },
                    { 82, false, 21, "nước" },
                    { 83, false, 21, "sân bay" },
                    { 84, false, 21, "vé" },
                    { 85, true, 22, "nước" },
                    { 86, false, 22, "sân bay" },
                    { 87, false, 22, "vé" },
                    { 88, false, 22, "xin chào" },
                    { 89, true, 23, "sân bay" },
                    { 90, false, 23, "vé" },
                    { 91, false, 23, "xin chào" },
                    { 92, false, 23, "cảm ơn" },
                    { 93, true, 24, "vé" },
                    { 94, false, 24, "xin chào" },
                    { 95, false, 24, "cảm ơn" },
                    { 96, false, 24, "quả táo" }
                });

            migrationBuilder.InsertData(
                table: "WordExamples",
                columns: new[] { "Id", "Reading", "Sentence", "Translation", "WordId" },
                values: new object[,]
                {
                    { 1, null, "Hello, how are you?", "Xin chào, bạn khỏe không?", 101 },
                    { 2, null, "Thank you very much.", "Cảm ơn bạn rất nhiều.", 102 },
                    { 3, null, "I eat an apple every day.", "Tôi ăn một quả táo mỗi ngày.", 103 },
                    { 4, null, "Can I have some water?", "Cho tôi xin ít nước được không?", 104 },
                    { 5, null, "The airport is far from here.", "Sân bay cách xa đây.", 105 },
                    { 6, null, "I need to buy a ticket.", "Tôi cần mua một tấm vé.", 106 },
                    { 7, "Nǐ hǎo, nǐ jiào shénme míngzi?", "你好，你叫什么名字？", "Xin chào, bạn tên là gì?", 201 },
                    { 8, "Xièxie nǐ de bāngzhù.", "谢谢你的帮助。", "Cảm ơn sự giúp đỡ của bạn.", 202 },
                    { 9, "Wǒ xǐhuan chī píngguǒ.", "我喜欢吃苹果。", "Tôi thích ăn táo.", 203 },
                    { 10, "Wǒ xiǎng hē shuǐ.", "我想喝水。", "Tôi muốn uống nước.", 204 },
                    { 11, "Wǒ zuò fēijī qù Běijīng.", "我坐飞机去北京。", "Tôi đi máy bay đến Bắc Kinh.", 205 },
                    { 12, "Huǒchēzhàn zài nǎlǐ?", "火车站在哪里？", "Ga tàu ở đâu?", 206 },
                    { 13, "Konnichiwa, ogenki desu ka.", "こんにちは、お元気ですか。", "Xin chào, bạn khỏe không?", 301 },
                    { 14, "Dōmo arigatō.", "どうもありがとう。", "Cảm ơn bạn rất nhiều.", 302 },
                    { 15, "Ringo o tabemasu.", "りんごを食べます。", "Tôi ăn táo.", 303 },
                    { 16, "Mizu o kudasai.", "水をください。", "Cho tôi xin nước.", 304 },
                    { 17, "Kūkō wa doko desu ka.", "空港はどこですか。", "Sân bay ở đâu?", 305 },
                    { 18, "Kippu o kaimasu.", "切符を買います。", "Tôi mua vé.", 306 },
                    { 19, "Annyeonghaseyo, mannaseo bangapseumnida.", "안녕하세요, 만나서 반갑습니다.", "Xin chào, rất vui được gặp bạn.", 401 },
                    { 20, "Dowajusyeoseo gamsahamnida.", "도와주셔서 감사합니다.", "Cảm ơn vì đã giúp đỡ.", 402 },
                    { 21, "Jeoneun sagwareul joahaeyo.", "저는 사과를 좋아해요.", "Tôi thích táo.", 403 },
                    { 22, "Mul jom juseyo.", "물 좀 주세요.", "Cho tôi xin nước.", 404 },
                    { 23, "Gonghange eotteoke gayo?", "공항에 어떻게 가요?", "Đi đến sân bay thế nào?", 405 },
                    { 24, "Pyoreul sago sipeoyo.", "표를 사고 싶어요.", "Tôi muốn mua vé.", 406 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteWords_WordId",
                table: "FavoriteWords",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Code",
                table: "Languages",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_LessonId",
                table: "LessonProgress",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_LanguageId",
                table: "Lessons",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_LevelId",
                table: "Lessons",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Levels_LanguageId_Code",
                table: "Levels",
                columns: new[] { "LanguageId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_QuestionId",
                table: "QuestionOptions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_LanguageId",
                table: "Questions",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_LessonId",
                table: "Questions",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_LevelId",
                table: "Questions",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_QuestionId",
                table: "QuizAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_QuizAttemptId",
                table: "QuizAnswers",
                column: "QuizAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAnswers_SelectedOptionId",
                table: "QuizAnswers",
                column: "SelectedOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_LanguageId",
                table: "QuizAttempts",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_LevelId",
                table: "QuizAttempts",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId_CompletedAt",
                table: "QuizAttempts",
                columns: new[] { "UserId", "CompletedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_LanguageId_Name",
                table: "Topics",
                columns: new[] { "LanguageId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PreferredLanguageId",
                table: "Users",
                column: "PreferredLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWordProgress_UserId_NextReviewAt",
                table: "UserWordProgress",
                columns: new[] { "UserId", "NextReviewAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserWordProgress_WordId",
                table: "UserWordProgress",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_WordExamples_WordId",
                table: "WordExamples",
                column: "WordId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_LanguageId_Term",
                table: "Words",
                columns: new[] { "LanguageId", "Term" });

            migrationBuilder.CreateIndex(
                name: "IX_Words_LevelId",
                table: "Words",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Words_Meaning",
                table: "Words",
                column: "Meaning");

            migrationBuilder.CreateIndex(
                name: "IX_Words_TopicId",
                table: "Words",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteWords");

            migrationBuilder.DropTable(
                name: "LessonProgress");

            migrationBuilder.DropTable(
                name: "QuizAnswers");

            migrationBuilder.DropTable(
                name: "UserWordProgress");

            migrationBuilder.DropTable(
                name: "WordExamples");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "QuizAttempts");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
