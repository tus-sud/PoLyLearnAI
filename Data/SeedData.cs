using Microsoft.EntityFrameworkCore;
using PolyLearnAI.Api.Models.Entities;

namespace PolyLearnAI.Api.Data;

/// <summary>
/// Dữ liệu mẫu tĩnh (ID cố định để migration ổn định). Tài khoản admin KHÔNG seed ở đây
/// vì cần băm mật khẩu từ cấu hình; sẽ tạo lúc chạy ở giai đoạn xác thực.
/// </summary>
public static class SeedData
{
    private record W(string Term, string? Reading, string Meaning, string Pos, int Topic, int LevelIdx,
        string Ex, string? ExReading, string ExVi);

    public static void Apply(ModelBuilder b)
    {
        var languages = new[]
        {
            new Language { Id = 1, Code = "en", Name = "Tiếng Anh", NativeName = "English", ProficiencySystem = "CEFR", SpeechLocale = "en-US" },
            new Language { Id = 2, Code = "zh", Name = "Tiếng Trung", NativeName = "中文", ProficiencySystem = "HSK", SpeechLocale = "zh-CN" },
            new Language { Id = 3, Code = "ja", Name = "Tiếng Nhật", NativeName = "日本語", ProficiencySystem = "JLPT", SpeechLocale = "ja-JP" },
            new Language { Id = 4, Code = "ko", Name = "Tiếng Hàn", NativeName = "한국어", ProficiencySystem = "TOPIK", SpeechLocale = "ko-KR" },
        };
        b.Entity<Language>().HasData(languages);

        // Levels: 2 cấp mỗi ngôn ngữ (ID 1..8)
        var levelDefs = new (int Lang, string Code, string Name)[]
        {
            (1, "A1", "Sơ cấp A1"), (1, "A2", "Sơ cấp A2"),
            (2, "HSK1", "HSK 1"), (2, "HSK2", "HSK 2"),
            (3, "N5", "JLPT N5"), (3, "N4", "JLPT N4"),
            (4, "TOPIK1", "TOPIK I - Cấp 1"), (4, "TOPIK2", "TOPIK I - Cấp 2"),
        };
        var levels = levelDefs.Select((d, i) => new Level
        {
            Id = i + 1, LanguageId = d.Lang, Code = d.Code, Name = d.Name, SortOrder = i % 2 + 1
        }).ToArray();
        b.Entity<Level>().HasData(levels);

        // Topics: 3 chủ đề mỗi ngôn ngữ, ID = (lang-1)*3 + k
        var topicNames = new (string Name, string Icon, string Desc)[]
        {
            ("Chào hỏi", "👋", "Câu chào và lịch sự cơ bản"),
            ("Ẩm thực", "🍎", "Đồ ăn thức uống"),
            ("Du lịch", "✈️", "Di chuyển và đi lại"),
        };
        var topics = languages.SelectMany(l => topicNames.Select((t, k) => new Topic
        {
            Id = (l.Id - 1) * 3 + k + 1, LanguageId = l.Id, Name = t.Name, Icon = t.Icon, Description = t.Desc
        })).ToArray();
        b.Entity<Topic>().HasData(topics);

        // Words: 20 từ mỗi ngôn ngữ. Topic: 1 chào hỏi, 2 ẩm thực, 3 du lịch
        var data = new Dictionary<int, W[]>
        {
            [1] = // English (CEFR A1 / A2)
            [
                new("Hello", "/həˈloʊ/", "xin chào", "interjection", 1, 0, "Hello, how are you?", null, "Xin chào, bạn khỏe không?"),
                new("Thank you", "/θæŋk juː/", "cảm ơn", "phrase", 1, 0, "Thank you very much.", null, "Cảm ơn bạn rất nhiều."),
                new("Goodbye", "/ɡʊdˈbaɪ/", "tạm biệt", "interjection", 1, 0, "Goodbye, see you tomorrow!", null, "Tạm biệt, hẹn gặp lại ngày mai!"),
                new("Please", "/pliːz/", "xin vui lòng / làm ơn", "adverb", 1, 0, "Please sit down.", null, "Xin vui lòng ngồi xuống."),
                new("Sorry", "/ˈsɒr.i/", "xin lỗi", "adjective", 1, 0, "I am so sorry for being late.", null, "Tôi rất xin lỗi vì đến muộn."),
                new("Excuse me", "/ɪkˈskjuːz miː/", "xin thứ lỗi / xin phép", "phrase", 1, 0, "Excuse me, where is the station?", null, "Xin lỗi, ga tàu ở đâu ạ?"),
                new("Friend", "/frend/", "bạn bè", "noun", 1, 0, "She is my best friend.", null, "Cô ấy là bạn thân nhất của tôi."),
                new("Apple", "/ˈæp.əl/", "quả táo", "noun", 2, 0, "I eat an apple every day.", null, "Tôi ăn một quả táo mỗi ngày."),
                new("Water", "/ˈwɔː.tər/", "nước", "noun", 2, 0, "Can I have some water?", null, "Cho tôi xin ít nước được không?"),
                new("Coffee", "/ˈkɒf.i/", "cà phê", "noun", 2, 0, "I drink black coffee in the morning.", null, "Tôi uống cà phê đen vào buổi sáng."),
                new("Bread", "/bred/", "bánh mì", "noun", 2, 0, "I usually eat bread for breakfast.", null, "Tôi thường ăn bánh mì cho bữa sáng."),
                new("Rice", "/raɪs/", "cơm / gạo", "noun", 2, 0, "Do you want steamed rice?", null, "Bạn có muốn ăn cơm trắng không?"),
                new("Tea", "/tiː/", "trà", "noun", 2, 0, "Would you like a cup of green tea?", null, "Bạn có muốn một tách trà xanh không?"),
                new("Delicious", "/dɪˈlɪʃ.əs/", "ngon miệng", "adjective", 2, 1, "This soup is very delicious.", null, "Món súp này rất là ngon."),
                new("Airport", "/ˈeə.pɔːt/", "sân bay", "noun", 3, 1, "The airport is far from here.", null, "Sân bay cách xa đây."),
                new("Ticket", "/ˈtɪk.ɪt/", "vé", "noun", 3, 1, "I need to buy a ticket.", null, "Tôi cần mua một tấm vé."),
                new("Hotel", "/həʊˈtel/", "khách sạn", "noun", 3, 1, "We stayed at a five-star hotel.", null, "Chúng tôi đã ở một khách sạn năm sao."),
                new("Train", "/treɪn/", "tàu hỏa", "noun", 3, 1, "The train arrives at 8 AM.", null, "Tàu hỏa sẽ đến vào lúc 8 giờ sáng."),
                new("Hospital", "/ˈhɒs.pɪ.təl/", "bệnh viện", "noun", 3, 1, "Where is the nearest hospital?", null, "Bệnh viện gần nhất ở đâu?"),
                new("Passport", "/ˈpɑːs.pɔːt/", "hộ chiếu", "noun", 3, 1, "Please show me your passport.", null, "Vui lòng xuất trình hộ chiếu của bạn.")
            ],
            [2] = // Chinese (HSK 1 / HSK 2)
            [
                new("你好", "nǐ hǎo", "xin chào", "interjection", 1, 0, "你好，你叫什么名字？", "Nǐ hǎo, nǐ jiào shénme míngzi?", "Xin chào, bạn tên là gì?"),
                new("谢谢", "xièxie", "cảm ơn", "verb", 1, 0, "谢谢你的帮助。", "Xièxie nǐ de bāngzhù.", "Cảm ơn sự giúp đỡ của bạn."),
                new("再见", "zàijiàn", "tạm biệt", "verb", 1, 0, "明天见，再见！", "Míngtiān jiàn, zàijiàn!", "Mai gặp lại, tạm biệt!"),
                new("请", "qǐng", "xin vui lòng / mời", "verb", 1, 0, "请进，请坐。", "Qǐng jìn, qǐng zuò.", "Mời vào, mời ngồi."),
                new("对不起", "duìbuqǐ", "xin lỗi", "verb", 1, 0, "对不起，我来晚了。", "Duìbuqǐ, wǒ lái wǎn le.", "Xin lỗi, tôi đến muộn."),
                new("没关系", "méi guānxi", "không sao đâu", "phrase", 1, 0, "没关系，不要担心。", "Méi guānxi, bú yào dānxīn.", "Không sao đâu, đừng lo lắng."),
                new("朋友", "péngyou", "bạn bè", "noun", 1, 0, "他是我的好朋友。", "Tā shì wǒ de hǎo péngyou.", "Anh ấy là bạn tốt của tôi."),
                new("苹果", "píngguǒ", "quả táo", "noun", 2, 0, "我喜欢吃苹果。", "Wǒ xǐhuan chī píngguǒ.", "Tôi thích ăn táo."),
                new("水", "shuǐ", "nước", "noun", 2, 0, "我想喝水。", "Wǒ xiǎng hē shuǐ.", "Tôi muốn uống nước."),
                new("咖啡", "kāfēi", "cà phê", "noun", 2, 0, "你要喝咖啡吗？", "Nǐ yào hē kāfēi ma?", "Bạn muốn uống cà phê không?"),
                new("面包", "miànbāo", "bánh mì", "noun", 2, 0, "早餐我吃面包。", "Zǎocān wǒ chī miànbāo.", "Bữa sáng tôi ăn bánh mì."),
                new("米饭", "mǐfàn", "cơm", "noun", 2, 0, "中国人喜欢吃米饭。", "Zhōngguó rén xǐhuan chī mǐfàn.", "Người Trung Quốc thích ăn cơm."),
                new("茶", "chá", "trà", "noun", 2, 0, "请喝一杯热茶。", "Qǐng hē yì bēi rè chá.", "Mời uống một ly trà nóng."),
                new("好吃", "hǎochī", "ngon", "adjective", 2, 1, "中国菜很好吃。", "Zhōngguó cài hěn hǎochī.", "Món ăn Trung Quốc rất ngon."),
                new("飞机", "fēijī", "máy bay", "noun", 3, 1, "我坐飞机去北京。", "Wǒ zuò fēijī qù Běijīng.", "Tôi đi máy bay đến Bắc Kinh."),
                new("火车", "huǒchē", "tàu hỏa", "noun", 3, 1, "火车站在哪里？", "Huǒchēzhàn zài nǎlǐ?", "Ga tàu ở đâu?"),
                new("酒店", "jiǔdiàn", "khách sạn", "noun", 3, 1, "这家酒店很舒服。", "Zhè jiā jiǔdiàn hěn shūfu.", "Khách sạn này rất thoải mái."),
                new("车站", "chēzhàn", "trạm xe / bến xe", "noun", 3, 1, "我们在车站集合。", "Wǒmen zài chēzhàn jíhé.", "Chúng ta tập trung ở trạm xe."),
                new("医院", "yīyuàn", "bệnh viện", "noun", 3, 1, "他身体不舒服去医院了。", "Tā shēntǐ bù shūfu qù yīyuàn le.", "Anh ấy thấy không khỏe nên đến bệnh viện rồi."),
                new("行李", "xíngli", "hành lý", "noun", 3, 1, "这是你的行李吗？", "Zhè shì nǐ de xíngli ma?", "Đây là hành lý của bạn phải không?")
            ],
            [3] = // Japanese (JLPT N5 / N4)
            [
                new("こんにちは", "konnichiwa", "xin chào", "interjection", 1, 0, "こんにちは、お元気ですか。", "Konnichiwa, ogenki desu ka.", "Xin chào, bạn khỏe không?"),
                new("ありがとう", "arigatō", "cảm ơn", "interjection", 1, 0, "どうもありがとう。", "Dōmo arigatō.", "Cảm ơn bạn rất nhiều."),
                new("さようなら", "sayōnara", "tạm biệt", "interjection", 1, 0, "先生、さようなら。", "Sensei, sayōnara.", "Chào thầy, em xin phép về ạ."),
                new("すみません", "sumimasen", "xin lỗi / xin phép", "phrase", 1, 0, "すみません、トイレはどこですか。", "Sumimasen, toire wa doko desu ka.", "Xin lỗi, nhà vệ sinh ở đâu ạ?"),
                new("お願いします", "onegaishimasu", "xin vui lòng / làm ơn", "phrase", 1, 0, "これをお願いします。", "Kore o onegaishimasu.", "Làm ơn cho tôi cái này."),
                new("友達", "tomodachi", "bạn bè", "noun", 1, 0, "明日、友達に会います。", "Ashita, tomodachi ni aimasu.", "Ngày mai tôi sẽ gặp bạn."),
                new("はい", "hai", "vâng / dạ", "interjection", 1, 0, "はい、分かりました。", "Hai, wakarimashita.", "Vâng, tôi hiểu rồi."),
                new("りんご", "ringo", "quả táo", "noun", 2, 0, "りんごを食べます。", "Ringo o tabemasu.", "Tôi ăn táo."),
                new("水", "mizu", "nước", "noun", 2, 0, "水をください。", "Mizu o kudasai.", "Cho tôi xin nước."),
                new("コーヒー", "kōhī", "cà phê", "noun", 2, 0, "朝コーヒーを飲みます。", "Asa kōhī o nomimasu.", "Buổi sáng tôi uống cà phê."),
                new("パン", "pan", "bánh mì", "noun", 2, 0, "朝ごはんはパンです。", "Asagohan wa pan desu.", "Bữa sáng là bánh mì."),
                new("ご飯", "gohan", "cơm", "noun", 2, 0, "白いご飯が好きです。", "Shiroi gohan ga suki desu.", "Tôi thích cơm trắng."),
                new("お茶", "ocha", "trà", "noun", 2, 0, "熱いお茶を一杯どうぞ。", "Atsui ocha o ippai dōzo.", "Xin mời một tách trà nóng."),
                new("美味しい", "oishii", "ngon", "adjective", 2, 1, "このラーメンはとても美味しいです。", "Kono rāmen wa totemo oishii desu.", "Món mì ramen này rất ngon."),
                new("空港", "kūkō", "sân bay", "noun", 3, 1, "空港はどこですか。", "Kūkō wa doko desu ka.", "Sân bay ở đâu?"),
                new("切符", "kippu", "vé", "noun", 3, 1, "切符を買います。", "Kippu o kaimasu.", "Tôi mua vé."),
                new("ホテル", "hoteru", "khách sạn", "noun", 3, 1, "駅の近くのホテルに泊まります。", "Eki no chikaku no hoteru ni tomarimasu.", "Tôi ở khách sạn gần nhà ga."),
                new("電車", "densha", "tàu điện", "noun", 3, 1, "電車で会社に行きます。", "Densha de kaisha ni ikimasu.", "Tôi đi làm bằng tàu điện."),
                new("病院", "byōin", "bệnh viện", "noun", 3, 1, "熱があるので病院に行きます。", "Netsu ga aru node byōin ni ikimasu.", "Vì bị sốt nên tôi đi bệnh viện."),
                new("パスポート", "pasupōto", "hộ chiếu", "noun", 3, 1, "パスポートを見せてください。", "Pasupōto o misete kudasai.", "Xin vui lòng cho xem hộ chiếu.")
            ],
            [4] = // Korean (TOPIK I - Level 1 / Level 2)
            [
                new("안녕하세요", "annyeonghaseyo", "xin chào", "interjection", 1, 0, "안녕하세요, 만나서 반갑습니다.", "Annyeonghaseyo, mannaseo bangapseumnida.", "Xin chào, rất vui được gặp bạn."),
                new("감사합니다", "gamsahamnida", "cảm ơn", "verb", 1, 0, "도와주셔서 감사합니다.", "Dowajusyeoseo gamsahamnida.", "Cảm ơn vì đã giúp đỡ."),
                new("안녕히 계세요", "annyeonghi gyeseyo", "tạm biệt (người ở lại)", "phrase", 1, 0, "먼저 가겠습니다. 안녕히 계세요.", "Meonjeo gagesseumnida. Annyeonghi gyeseyo.", "Tôi xin phép về trước. Tạm biệt quý vị."),
                new("죄송합니다", "joesonghamnida", "xin lỗi", "verb", 1, 0, "늦어서 정말 죄송합니다.", "Neujeoseo jeongmal joesonghamnida.", "Thành thật xin lỗi vì đã đến muộn."),
                new("부탁합니다", "butakamnida", "làm ơn / nhờ bạn", "verb", 1, 0, "잘 부탁합니다.", "Jal butakamnida.", "Rất mong được bạn giúp đỡ."),
                new("친구", "chingu", "bạn bè", "noun", 1, 0, "내일 친구를 만나요.", "Naeil chingureul mannayo.", "Ngày mai tôi gặp bạn."),
                new("네", "ne", "vâng / dạ", "interjection", 1, 0, "네, 알겠습니다.", "Ne, algesseumnida.", "Vâng, tôi đã hiểu rồi ạ."),
                new("사과", "sagwa", "quả táo", "noun", 2, 0, "저는 사과를 좋아해요.", "Jeoneun sagwareul joahaeyo.", "Tôi thích táo."),
                new("물", "mul", "nước", "noun", 2, 0, "물 좀 주세요.", "Mul jom juseyo.", "Cho tôi xin nước."),
                new("커피", "keopi", "cà phê", "noun", 2, 0, "아이스 커피 한 잔 주세요.", "Aiseu keopi han jan juseyo.", "Cho tôi một ly cà phê đá."),
                new("빵", "ppang", "bánh mì", "noun", 2, 0, "아침에 빵을 먹었어요.", "Achime ppangeul meogeosseoyo.", "Buổi sáng tôi đã ăn bánh mì."),
                new("밥", "bap", "cơm / bữa ăn", "noun", 2, 0, "밥 먹었어요?", "Bap meogeosseoyo?", "Bạn đã ăn cơm chưa?"),
                new("차", "cha", "trà", "noun", 2, 0, "따뜻한 녹차를 마셔요.", "Ttatteutan nokchareul masyeoyo.", "Uống một tách trà xanh ấm."),
                new("맛있어요", "masisseoyo", "ngon", "adjective", 2, 1, "한국 음식이 정말 맛있어요.", "Hanguk eumsigi jeongmal masisseoyo.", "Món ăn Hàn Quốc thực sự rất ngon."),
                new("공항", "gonghang", "sân bay", "noun", 3, 1, "공항에 어떻게 가요?", "Gonghange eotteoke gayo?", "Đi đến sân bay thế nào?"),
                new("표", "pyo", "vé", "noun", 3, 1, "표를 사고 싶어요.", "Pyoreul sago sipeoyo.", "Tôi muốn mua vé."),
                new("호텔", "hotel", "khách sạn", "noun", 3, 1, "호텔을 예약했어요.", "Hoteleul yeyakaesseoyo.", "Tôi đã đặt phòng khách sạn."),
                new("기차", "gicha", "tàu hỏa", "noun", 3, 1, "기차표를 예매했어요.", "Gichapyoreul yemaehaesseoyo.", "Tôi đã mua vé tàu hỏa rồi."),
                new("병원", "byeong-won", "bệnh viện", "noun", 3, 1, "아파서 병원에 가요.", "Apaseo byeong-wone gayo.", "Vì bị ốm nên tôi đi bệnh viện."),
                new("여권", "yeogwon", "hộ chiếu", "noun", 3, 1, "여권을 잊지 마세요.", "Yeogwoneul ijji maseyo.", "Đừng quên mang theo hộ chiếu nhé.")
            ],
        };

        var words = new List<Word>();
        var examples = new List<WordExample>();
        var questions = new List<Question>();
        var options = new List<QuestionOption>();
        int exId = 1, qId = 1, oId = 1;

        foreach (var lang in languages)
        {
            var list = data[lang.Id];
            var lvlBase = (lang.Id - 1) * 2 + 1; // level id đầu tiên của ngôn ngữ
            for (int i = 0; i < list.Length; i++)
            {
                var w = list[i];
                var wordId = lang.Id * 100 + i + 1;
                var levelId = lvlBase + w.LevelIdx;
                words.Add(new Word
                {
                    Id = wordId, LanguageId = lang.Id, LevelId = levelId,
                    TopicId = (lang.Id - 1) * 3 + w.Topic,
                    Term = w.Term, Reading = w.Reading, Meaning = w.Meaning, PartOfSpeech = w.Pos,
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                });
                examples.Add(new WordExample
                {
                    Id = exId++, WordId = wordId, Sentence = w.Ex, Reading = w.ExReading, Translation = w.ExVi
                });

                // Câu hỏi trắc nghiệm: "Nghĩa của 'X' là gì?" + 3 đáp án nhiễu (các từ kế tiếp, vòng tròn)
                var q = qId++;
                questions.Add(new Question
                {
                    Id = q, LanguageId = lang.Id, LevelId = levelId,
                    Text = $"Nghĩa của \"{w.Term}\" là gì?",
                    Explanation = $"\"{w.Term}\" nghĩa là \"{w.Meaning}\"."
                });
                options.Add(new QuestionOption { Id = oId++, QuestionId = q, Text = w.Meaning, IsCorrect = true });
                for (int d = 1; d <= 3; d++)
                {
                    var wrong = list[(i + d) % list.Length].Meaning;
                    if (wrong == w.Meaning) continue;
                    options.Add(new QuestionOption { Id = oId++, QuestionId = q, Text = wrong, IsCorrect = false });
                }
            }
        }

        b.Entity<Word>().HasData(words);
        b.Entity<WordExample>().HasData(examples);
        b.Entity<Question>().HasData(questions);
        b.Entity<QuestionOption>().HasData(options);

        // Bài học mẫu: đọc hiểu + nghe mỗi ngôn ngữ
        var lessons = new List<Lesson>();
        var content = new Dictionary<int, string>
        {
            [1] = "Tom goes to the airport. He buys a ticket and drinks some water.",
            [2] = "我去机场。我买了一张票，还喝了一杯水。",
            [3] = "私は空港に行きます。切符を買って、水を飲みます。",
            [4] = "저는 공항에 갑니다. 표를 사고 물을 마십니다.",
        };
        foreach (var lang in languages)
        {
            var lvl = (lang.Id - 1) * 2 + 1;
            lessons.Add(new Lesson { Id = lang.Id * 10 + 1, LanguageId = lang.Id, LevelId = lvl, Title = "Từ vựng cơ bản", Type = LessonType.Vocabulary, SortOrder = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            lessons.Add(new Lesson { Id = lang.Id * 10 + 2, LanguageId = lang.Id, LevelId = lvl, Title = "Luyện nghe: Ở sân bay", Type = LessonType.Listening, Content = content[lang.Id], SortOrder = 2, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            lessons.Add(new Lesson { Id = lang.Id * 10 + 3, LanguageId = lang.Id, LevelId = lvl, Title = "Đọc hiểu: Chuyến đi", Type = LessonType.Reading, Content = content[lang.Id], SortOrder = 3, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
        }
        b.Entity<Lesson>().HasData(lessons);
    }
}
