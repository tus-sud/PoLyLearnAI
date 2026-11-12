namespace PolyLearnAI.Api.Data;

public static class EndlessVocabBank
{
    public record VaultWord(
        int LanguageId,
        string Category, // technology | travel | business | food | health | culture
        string Term,
        string? Reading,
        string Meaning,
        string PartOfSpeech,
        int LevelOffset, // 0, 1, 2
        int TopicOffset, // 0, 1, 2
        string Example,
        string? ExampleReading,
        string ExampleVi
    );

    public static readonly IReadOnlyList<VaultWord> Words =
    [
        // ==================== 1. ENGLISH (LanguageId = 1) ====================
        // Technology & AI
        new(1, "technology", "Algorithm", "/ˈæl.ɡə.rɪ.ðəm/", "thuật toán", "noun", 1, 0, "The search engine uses a smart algorithm.", null, "Công cụ tìm kiếm sử dụng một thuật toán thông minh."),
        new(1, "technology", "Database", "/ˈdeɪ.tə.beɪs/", "cơ sở dữ liệu", "noun", 1, 0, "All vocabulary is saved permanently in the database.", null, "Toàn bộ từ vựng được lưu vĩnh viễn trong cơ sở dữ liệu."),
        new(1, "technology", "Artificial", "/ˌɑː.tɪˈfɪʃ.əl/", "nhân tạo", "adjective", 1, 0, "Artificial intelligence helps people learn faster.", null, "Trí tuệ nhân tạo giúp mọi người học nhanh hơn."),
        new(1, "technology", "Software", "/ˈsɒft.weər/", "phần mềm", "noun", 0, 0, "Please update the software to the latest version.", null, "Vui lòng cập nhật phần mềm lên phiên bản mới nhất."),
        new(1, "technology", "Network", "/ˈnet.wɜːk/", "mạng lưới, kết nối mạng", "noun", 0, 0, "The wireless network here is very stable.", null, "Mạng không dây ở đây rất ổn định."),
        new(1, "technology", "Security", "/sɪˈkjʊə.rə.ti/", "bảo mật, an ninh", "noun", 1, 0, "Strong passwords improve account security.", null, "Mật khẩu mạnh giúp tăng cường bảo mật tài khoản."),
        new(1, "technology", "Device", "/dɪˈvaɪs/", "thiết bị", "noun", 0, 0, "You can sync your lessons across every device.", null, "Bạn có thể đồng bộ bài học trên mọi thiết bị."),
        new(1, "technology", "Innovation", "/ˌɪn.əˈveɪ.ʃən/", "sự đổi mới sáng tạo", "noun", 1, 0, "Technological innovation changes how we work.", null, "Đổi mới công nghệ thay đổi cách chúng ta làm việc."),

        // Business & Career
        new(1, "business", "Opportunity", "/ˌɒp.əˈtjuː.nə.ti/", "cơ hội", "noun", 1, 2, "Fluency in languages creates career opportunities.", null, "Thành thạo ngoại ngữ tạo ra nhiều cơ hội nghề nghiệp."),
        new(1, "business", "Schedule", "/ˈʃedʒ.uːl/", "lịch trình, thời gian biểu", "noun", 0, 0, "Let me check my meeting schedule for tomorrow.", null, "Để tôi kiểm tra lịch họp ngày mai của mình."),
        new(1, "business", "Interview", "/ˈɪn.tə.vjuː/", "buổi phỏng vấn", "noun", 0, 2, "She prepared carefully for the job interview.", null, "Cô ấy đã chuẩn bị kỹ lưỡng cho buổi phỏng vấn xin việc."),
        new(1, "business", "Colleague", "/ˈkɒl.iːɡ/", "đồng nghiệp", "noun", 1, 2, "My colleagues are friendly and supportive.", null, "Các đồng nghiệp của tôi rất thân thiện và hay giúp đỡ."),
        new(1, "business", "Strategy", "/ˈstræt.ə.dʒi/", "chiến lược", "noun", 1, 2, "Spaced repetition is an effective study strategy.", null, "Lặp lại ngắt quãng là một chiến lược học tập hiệu quả."),
        new(1, "business", "Negotiate", "/nɪˈɡəʊ.ʃi.eɪt/", "đàm phán, thương lượng", "verb", 1, 2, "They met to negotiate the new contract.", null, "Họ gặp nhau để đàm phán hợp đồng mới."),
        new(1, "business", "Deadline", "/ˈded.laɪn/", "hạn chót", "noun", 1, 0, "We finished the project before the deadline.", null, "Chúng tôi đã hoàn thành dự án trước hạn chót."),
        new(1, "business", "Experience", "/ɪkˈspɪə.ri.əns/", "kinh nghiệm, trải nghiệm", "noun", 0, 2, "Traveling abroad is an unforgettable experience.", null, "Du lịch nước ngoài là một trải nghiệm khó quên."),

        // Travel & Exploration
        new(1, "travel", "Destination", "/ˌdes.tɪˈneɪ.ʃən/", "điểm đến", "noun", 1, 2, "Kyoto is a popular tourist destination.", null, "Kyoto là một điểm đến du lịch nổi tiếng."),
        new(1, "travel", "Departure", "/dɪˈpɑː.tʃər/", "sự khởi hành", "noun", 1, 2, "Please arrive two hours before departure.", null, "Vui lòng có mặt hai tiếng trước giờ khởi hành."),
        new(1, "travel", "Accommodation", "/əˌkɒm.əˈdeɪ.ʃən/", "chỗ ở, nơi lưu trú", "noun", 1, 2, "We booked affordable accommodation near the beach.", null, "Chúng tôi đã đặt chỗ ở giá hợp lý gần bãi biển."),
        new(1, "travel", "Souvenir", "/ˌsuː.vəˈnɪər/", "quà lưu niệm", "noun", 0, 2, "I bought a handmade keychain as a souvenir.", null, "Tôi đã mua một chiếc móc khóa thủ công làm quà lưu niệm."),
        new(1, "travel", "Adventure", "/ədˈven.tʃər/", "cuộc phiêu lưu", "noun", 0, 2, "Learning four languages is an exciting adventure.", null, "Học bốn ngôn ngữ là một cuộc phiêu lưu thú vị."),
        new(1, "travel", "Subway", "/ˈsʌb.weɪ/", "tàu điện ngầm", "noun", 0, 2, "Taking the subway saves time during rush hour.", null, "Đi tàu điện ngầm giúp tiết kiệm thời gian vào giờ cao điểm."),
        new(1, "travel", "Currency", "/ˈkʌr.ən.si/", "tiền tệ, ngoại tệ", "noun", 1, 2, "Where can I exchange foreign currency?", null, "Tôi có thể đổi ngoại tệ ở đâu?"),

        // Food & Lifestyle
        new(1, "food", "Ingredient", "/ɪnˈɡriː.di.ənt/", "nguyên liệu, thành phần", "noun", 1, 1, "Fresh herbs are the secret ingredient in this soup.", null, "Thảo mộc tươi là nguyên liệu bí quyết trong món súp này."),
        new(1, "food", "Delicious", "/dɪˈlɪʃ.əs/", "ngon miệng", "adjective", 0, 1, "The street food in Hanoi is delicious.", null, "Ẩm thực đường phố ở Hà Nội thật ngon miệng."),
        new(1, "food", "Beverage", "/ˈbev.ər.ɪdʒ/", "đồ uống, thức uống", "noun", 1, 1, "Hot beverages are served on the flight.", null, "Đồ uống nóng được phục vụ trên chuyến bay."),
        new(1, "food", "Appetite", "/ˈæp.ə.taɪt/", "sự ngon miệng, khẩu vị", "noun", 1, 1, "A morning walk always gives me a good appetite.", null, "Đi bộ buổi sáng luôn giúp tôi ăn ngon miệng hơn."),
        new(1, "food", "Recipe", "/ˈres.ɪ.pi/", "công thức nấu ăn", "noun", 0, 1, "My grandmother taught me this cake recipe.", null, "Bà tôi đã dạy tôi công thức làm bánh này."),
        new(1, "food", "Vegetarian", "/ˌvedʒ.ɪˈteə.ri.ən/", "ăn chay, món chay", "adjective", 1, 1, "Does this café have vegetarian options?", null, "Quán cà phê này có thực đơn chay không?"),

        // Health & Emotions
        new(1, "health", "Confident", "/ˈkɒn.fɪ.dənt/", "tự tin", "adjective", 0, 0, "Daily practice makes you confident in speaking.", null, "Luyện tập hằng ngày giúp bạn tự tin khi giao tiếp."),
        new(1, "health", "Curious", "/ˈkjʊə.ri.əs/", "tò mò, ham học hỏi", "adjective", 1, 0, "Stay curious and keep exploring new words.", null, "Hãy luôn ham học hỏi và tiếp tục khám phá từ mới."),
        new(1, "health", "Habit", "/ˈhæb.ɪt/", "thói quen", "noun", 0, 0, "Reading ten minutes a day is a great habit.", null, "Đọc sách mười phút mỗi ngày là một thói quen tuyệt vời."),
        new(1, "health", "Energy", "/ˈen.ə.dʒi/", "năng lượng", "noun", 0, 0, "Good sleep gives your brain fresh energy.", null, "Giấc ngủ ngon mang lại năng lượng tươi mới cho não bộ."),
        new(1, "health", "Patience", "/ˈpeɪ.ʃəns/", "sự kiên nhẫn", "noun", 1, 0, "Mastering pronunciation requires patience.", null, "Làm chủ phát âm đòi hỏi sự kiên nhẫn."),
        new(1, "health", "Inspire", "/ɪnˈspaɪər/", "truyền cảm hứng", "verb", 1, 0, "Great stories inspire us to dream bigger.", null, "Những câu chuyện hay truyền cảm hứng để ta ước mơ lớn hơn."),

        // Culture & Society
        new(1, "culture", "Tradition", "/trəˈdɪʃ.ən/", "truyền thống", "noun", 1, 2, "Each country celebrates New Year with unique traditions.", null, "Mỗi quốc gia đón năm mới với những truyền thống độc đáo."),
        new(1, "culture", "Festival", "/ˈfes.tɪ.vəl/", "lễ hội", "noun", 0, 2, "Lanterns lit up the entire autumn festival.", null, "Những chiếc đèn lồng thắp sáng cả lễ hội mùa thu."),
        new(1, "culture", "Literature", "/ˈlɪt.rə.tʃər/", "văn học", "noun", 1, 2, "She enjoys reading modern world literature.", null, "Cô ấy thích đọc văn học thế giới hiện đại."),
        new(1, "culture", "Exhibition", "/ˌek.sɪˈbɪʃ.ən/", "triển lãm", "noun", 1, 2, "We visited an art exhibition downtown.", null, "Chúng tôi đã ghé thăm một triển lãm nghệ thuật ở trung tâm."),
        new(1, "culture", "Harmony", "/ˈhɑː.mə.ni/", "sự hài hòa, hòa hợp", "noun", 1, 2, "Living in harmony with nature brings peace.", null, "Sống hài hòa với thiên nhiên mang lại sự bình yên."),

        // ==================== 2. CHINESE (LanguageId = 2) ====================
        // Technology
        new(2, "technology", "互联网", "hùliánwǎng", "mạng Internet", "noun", 1, 0, "互联网让学习外语变得非常方便。", "Hùliánwǎng ràng xuéxí wàiyǔ biàn de fēicháng fāngbiàn.", "Internet giúp việc học ngoại ngữ trở nên vô cùng thuận tiện."),
        new(2, "technology", "软件", "ruǎnjiàn", "phần mềm, ứng dụng", "noun", 1, 0, "这个学习软件有很多有趣的单词。", "Zhège xuéxí ruǎnjiàn yǒu hěn duō yǒuqù de dāncí.", "Phần mềm học tập này có rất nhiều từ vựng thú vị."),
        new(2, "technology", "数据", "shùjù", "dữ liệu", "noun", 1, 0, "系统会自动保存你的学习数据。", "Xìtǒng huì zìdòng bǎocún nǐ de xuéxí shùjù.", "Hệ thống sẽ tự động lưu trữ dữ liệu học tập của bạn."),
        new(2, "technology", "密码", "mìmǎ", "mật khẩu", "noun", 0, 0, "请不要把密码告诉别人。", "Qǐng búyào bǎ mìmǎ gàosu biérén.", "Xin đừng nói mật khẩu cho người khác biết."),
        new(2, "technology", "网站", "wǎngzhàn", "trang web", "noun", 0, 0, "我每天都在这个网站练习听力。", "Wǒ měitiān dōu zài zhège wǎngzhàn liànxí tīnglì.", "Mỗi ngày tôi đều luyện nghe trên trang web này."),
        new(2, "technology", "人工智能", "réngōng zhìnéng", "trí tuệ nhân tạo (AI)", "noun", 1, 0, "人工智能可以帮我们修改语法错误。", "Réngōng zhìnéng kěyǐ bāng wǒmen xiūgǎi yǔfǎ cuòwù.", "Trí tuệ nhân tạo có thể giúp chúng ta sửa lỗi ngữ pháp."),
        new(2, "technology", "充电", "chōngdiàn", "sạc pin", "verb", 0, 0, "出门前别忘了给手机充电。", "Chūmén qián bié wàng le gěi shǒujī chōngdiàn.", "Trước khi ra ngoài đừng quên sạc pin điện thoại nhé."),

        // Business & Study
        new(2, "business", "经验", "jīngyàn", "kinh nghiệm", "noun", 1, 2, "他在国际贸易方面很有经验。", "Tā zài guójì màoyì fāngmiàn hěn yǒu jīngyàn.", "Anh ấy rất có kinh nghiệm trong lĩnh vực thương mại quốc tế."),
        new(2, "business", "机会", "jīhuì", "cơ hội", "noun", 1, 2, "学好外语能带来更多工作机会。", "Xuéhǎo wàiyǔ néng dàilái gèng duō gōngzuò jīhuì.", "Học tốt ngoại ngữ mang lại nhiều cơ hội việc làm hơn."),
        new(2, "business", "会议", "huìyì", "cuộc họp, hội nghị", "noun", 1, 2, "下午三点我们有一个重要会议。", "Xiàwǔ sān diǎn wǒmen yǒu yí ge zhòngyào huìyì.", "Ba giờ chiều chúng tôi có một cuộc họp quan trọng."),
        new(2, "business", "计划", "jìhuà", "kế hoạch", "noun", 1, 0, "你今年有什么新的学习计划？", "Nǐ jīnnián yǒu shénme xīn de xuéxí jìhuà?", "Năm nay bạn có kế hoạch học tập mới nào không?"),
        new(2, "business", "成功", "chénggōng", "thành công", "adjective", 1, 2, "坚持每天复习是成功的关键。", "Jiānchí měitiān fùxí shì chénggōng de guānjiàn.", "Kiên trì ôn tập mỗi ngày là chìa khóa của thành công."),
        new(2, "business", "同事", "tóngshì", "đồng nghiệp", "noun", 0, 2, "新来的同事非常热情。", "Xīn lái de tóngshì fēicháng rèqíng.", "Người đồng nghiệp mới đến vô cùng nhiệt tình."),
        new(2, "business", "合作", "hézuò", "hợp tác", "verb", 1, 2, "希望我们以后能愉快地合作。", "Xīwàng wǒmen yǐhòu néng yúkuài de hézuò.", "Hy vọng sau này chúng ta có thể hợp tác vui vẻ."),

        // Travel
        new(2, "travel", "护照", "hùzhào", "hộ chiếu", "noun", 0, 2, "去国外旅行一定要带好护照。", "Qù guówài lǚxíng yídìng yào dài hǎo hùzhào.", "Đi du lịch nước ngoài nhất định phải mang theo hộ chiếu."),
        new(2, "travel", "地铁", "dìtiě", "tàu điện ngầm", "noun", 0, 2, "坐地铁去机场既快又便宜。", "Zuò dìtiě qù jīchǎng jì kuài yòu piányi.", "Đi tàu điện ngầm ra sân bay vừa nhanh vừa rẻ."),
        new(2, "travel", "风景", "fēngjǐng", "phong cảnh", "noun", 1, 2, "山上的风景美极了。", "Shānshàng de fēngjǐng měi jí le.", "Phong cảnh trên núi đẹp vô cùng."),
        new(2, "travel", "行李", "xíngli", "hành lý", "noun", 1, 2, "请帮我照看一下行李，谢谢。", "Qǐng bāng wǒ zhàokàn yíxià xíngli, xièxie.", "Làm ơn trông giúp tôi hành lý một lát, cảm ơn bạn."),
        new(2, "travel", "地图", "dìtú", "bản đồ", "noun", 0, 2, "手机上的电子地图非常准确。", "Shǒujī shàng de diànzǐ dìtú fēicháng zhǔnquè.", "Bản đồ điện tử trên điện thoại rất chính xác."),
        new(2, "travel", "导游", "dǎoyóu", "hướng dẫn viên du lịch", "noun", 1, 2, "这位导游会说三种语言。", "Zhè wèi dǎoyóu huì shuō sān zhǒng yǔyán.", "Vị hướng dẫn viên này biết nói ba thứ tiếng."),

        // Food
        new(2, "food", "菜单", "càidān", "thực đơn (menu)", "noun", 0, 1, "服务员，请给我看一下菜单。", "Fúwùyuán, qǐng gěi wǒ kàn yíxià càidān.", "Phục vụ ơi, cho tôi xem thực đơn một chút."),
        new(2, "food", "面条", "miàntiáo", "mì sợi", "noun", 0, 1, "这家店的手工面条特别香。", "Zhè jiā diàn de shǒugōng miàntiáo tèbié xiāng.", "Món mì sợi làm thủ công của quán này thơm đặc biệt."),
        new(2, "food", "饺子", "jiǎozi", "sủi cảo, bánh chẻo", "noun", 0, 1, "春节的时候大家一起包饺子。", "Chūnjié de shíhou dàjiā yìqǐ bāo jiǎozi.", "Vào dịp Tết mọi người cùng nhau gói sủi cảo."),
        new(2, "food", "咖啡", "kāfēi", "cà phê", "noun", 0, 1, "早上喝一杯咖啡很有精神。", "Zǎoshang hē yì bēi kāfēi hěn yǒu jīngshen.", "Buổi sáng uống một tách cà phê rất tỉnh táo."),
        new(2, "food", "甜点", "tiándiǎn", "món tráng miệng, đồ ngọt", "noun", 1, 1, "吃完晚饭后想来一点甜点吗？", "Chīwán wǎnfàn hòu xiǎng lái yìdiǎn tiándiǎn ma?", "Ăn tối xong bạn có muốn dùng chút tráng miệng không?"),
        new(2, "food", "味道", "wèidào", "hương vị, mùi vị", "noun", 1, 1, "这道汤的味道刚刚好。", "Zhè dào tāng de wèidào gānggāng hǎo.", "Hương vị món canh này vừa vặn lắm."),

        // Health & Emotions
        new(2, "health", "健康", "jiànkāng", "khỏe mạnh, sức khỏe", "adjective", 0, 0, "每天运动让人保持健康。", "Měitiān yùndòng ràng rén bǎochí jiànkāng.", "Vận động mỗi ngày giúp con người giữ gìn sức khỏe."),
        new(2, "health", "习惯", "xíguàn", "thói quen, quen với", "noun", 1, 0, "我已经习惯早起背单词了。", "Wǒ yǐjīng xíguàn zǎoqǐ bèi dāncí le.", "Tôi đã quen với việc dậy sớm học từ vựng rồi."),
        new(2, "health", "信心", "xìnxīn", "sự tự tin, niềm tin", "noun", 1, 0, "我对这次考试充满信心。", "Wǒ duì zhè cì kǎoshì chōngmǎn xìnxīn.", "Tôi tràn đầy tự tin vào kỳ thi lần này."),
        new(2, "health", "耐心", "nàixīn", "sự kiên nhẫn", "noun", 1, 0, "老师总是很有耐心地回答问题。", "Lǎoshī zǒngshì hěn yǒu nàixīn de huídá wèntí.", "Cô giáo luôn kiên nhẫn trả lời các câu hỏi."),
        new(2, "health", "休息", "xiūxi", "nghỉ ngơi", "verb", 0, 0, "学了一个小时，稍微休息一下吧。", "Xué le yí ge xiǎoshí, shāowēi xiūxi yíxià ba.", "Học một tiếng rồi, nghỉ ngơi một chút nhé."),

        // Culture
        new(2, "culture", "文化", "wénhuà", "văn hóa", "noun", 1, 2, "学习语言也是在了解一种文化。", "Xuéxí yǔyán yě shì zài liǎojiě yì zhǒng wénhuà.", "Học ngôn ngữ cũng chính là tìm hiểu một nền văn hóa."),
        new(2, "culture", "节日", "jiérì", "ngày lễ, lễ hội", "noun", 0, 2, "中秋节是团圆的传统节日。", "Zhōngqiūjié shì tuányuán de chuántǒng jiérì.", "Tết Trung thu là ngày lễ đoàn viên truyền thống."),
        new(2, "culture", "音乐", "yīnyuè", "âm nhạc", "noun", 0, 2, "听轻柔的音乐可以放松心情。", "Tīng qīngróu de yīnyuè kěyǐ fàngsōng xīnqíng.", "Nghe nhạc nhẹ nhàng có thể giúp thư giãn tâm trạng."),
        new(2, "culture", "历史", "lìshǐ", "lịch sử", "noun", 1, 2, "这座城市有悠久的历史。", "Zhè zuò chéngshì yǒu yōujiǔ de lìshǐ.", "Thành phố này có lịch sử lâu đời."),

        // ==================== 3. JAPANESE (LanguageId = 3) ====================
        // Technology
        new(3, "technology", "パソコン", "pasokon", "máy tính cá nhân", "noun", 0, 0, "新しいパソコンでレポートを書きます。", "Atarashii pasokon de repōto o kakimasu.", "Tôi viết báo cáo bằng chiếc máy tính mới."),
        new(3, "technology", "アプリ", "apuri", "ứng dụng (app)", "noun", 0, 0, "この単語アプリはとても使いやすいです。", "Kono tango apuri wa totemo tsukaiyasui desu.", "Ứng dụng từ vựng này rất dễ sử dụng."),
        new(3, "technology", "データ", "dēta", "dữ liệu", "noun", 1, 0, "大切なデータをクラウドに保存しました。", "Taisetsu na dēta o kuraudo ni hozon shimashita.", "Tôi đã lưu dữ liệu quan trọng lên đám mây."),
        new(3, "technology", "検索", "けんさく (kensaku)", "tìm kiếm, tra cứu", "noun", 1, 0, "わからない言葉はすぐ検索します。", "Wakaranai kotoba wa sugu kensaku shimasu.", "Từ nào không biết tôi sẽ tra cứu ngay."),
        new(3, "technology", "便利", "べんり (benri)", "tiện lợi", "adjective", 0, 0, "音声機能はとても便利です。", "Onsei kinō wa totemo benri desu.", "Tính năng giọng nói vô cùng tiện lợi."),
        new(3, "technology", "技術", "ぎじゅつ (gijutsu)", "kỹ thuật, công nghệ", "noun", 1, 0, "日本の科学技術は世界中で有名です。", "Nihon no kagaku gijutsu wa sekaijū de yūmei desu.", "Khoa học công nghệ Nhật Bản nổi tiếng khắp thế giới."),

        // Business & Study
        new(3, "business", "会議", "かいぎ (kaigi)", "cuộc họp", "noun", 0, 2, "来週の月曜日にオンライン会議があります。", "Raishū no getsuyōbi ni onrain kaigi ga arimasu.", "Thứ Hai tuần sau có một cuộc họp trực tuyến."),
        new(3, "business", "約束", "やくそく (yakusoku)", "lời hứa, cuộc hẹn", "noun", 0, 0, "友達との約束を忘れないでください。", "Tomodachi to no yakusoku o wasurenaide kudasai.", "Xin đừng quên cuộc hẹn với bạn bè nhé."),
        new(3, "business", "経験", "けいけん (keiken)", "kinh nghiệm, trải nghiệm", "noun", 1, 2, "留学はとてもいい経験になりました。", "Ryūgaku wa totemo ii keiken ni narimashita.", "Du học đã trở thành một trải nghiệm rất tuyệt vời."),
        new(3, "business", "準備", "じゅんび (junbi)", "sự chuẩn bị", "noun", 0, 0, "試験の準備はもうできましたか。", "Shiken no junbi wa mō dekimashita ka.", "Bạn đã chuẩn bị xong cho kỳ thi chưa?"),
        new(3, "business", "目標", "もくひょう (mokuhyō)", "mục tiêu", "noun", 1, 0, "毎日の目標を達成しましょう。", "Mainichi no mokuhyō o tassei shimashō.", "Hãy cùng hoàn thành mục tiêu mỗi ngày nhé."),
        new(3, "business", "相談", "そうだん (sōdan)", "thảo luận, trao đổi, hỏi ý kiến", "noun", 1, 2, "先生に将来のことを相談しました。", "Sensei ni shōrai no koto o sōdan shimashita.", "Tôi đã trao đổi với thầy giáo về chuyện tương lai."),

        // Travel
        new(3, "travel", "新幹線", "しんかんせん (shinkansen)", "tàu siêu tốc Shinkansen", "noun", 0, 2, "新幹線で東京から京都へ行きます。", "Shinkansen de Tōkyō kara Kyōto e ikimasu.", "Tôi đi từ Tokyo đến Kyoto bằng tàu Shinkansen."),
        new(3, "travel", "空港", "くうこう (kūkō)", "sân bay", "noun", 0, 2, "空港までバスで三十分かかります。", "Kūkō made basu de sanjuppun kakarimasu.", "Đi xe buýt đến sân bay mất ba mươi phút."),
        new(3, "travel", "予約", "よやく (yoyaku)", "đặt trước (phòng, vé)", "noun", 1, 2, "ホテルの部屋をインターネットで予約しました。", "Hoteru no heya o intānetto de yoyaku shimashita.", "Tôi đã đặt trước phòng khách sạn qua mạng."),
        new(3, "travel", "お土産", "おみやげ (omiyage)", "quà lưu niệm, quà đi xa về", "noun", 0, 2, "家族に抹茶のお菓子をお土産に買いました。", "Kazoku ni matcha no okashi o omiyage ni kaimashita.", "Tôi đã mua bánh kẹo trà xanh làm quà cho gia đình."),
        new(3, "travel", "景色", "けしき (keshiki)", "phong cảnh", "noun", 1, 2, "窓から見える山の景色がきれいです。", "Mado kara mieru yama no keshiki ga kirei desu.", "Phong cảnh núi non nhìn từ cửa sổ thật đẹp."),
        new(3, "travel", "温泉", "おんせん (onsen)", "suối nước nóng", "noun", 0, 2, "冬は暖かい温泉に入りたいです。", "Fuyu wa atatakai onsen ni hairitai desu.", "Vào mùa đông tôi muốn ngâm mình trong suối nước nóng."),

        // Food
        new(3, "food", "お弁当", "おべんとう (obentō)", "cơm hộp Bento", "noun", 0, 1, "母が作ってくれたお弁当はおいしいです。", "Haha ga tsukutte kureta obentō wa oishii desu.", "Hộp cơm mẹ làm cho rất ngon."),
        new(3, "food", "抹茶", "まっちゃ (matcha)", "trà xanh Matcha", "noun", 0, 1, "京都の抹茶アイスクリームが大好きです。", "Kyōto no matcha aisukurīmu ga daisuki desu.", "Tôi rất thích kem trà xanh của Kyoto."),
        new(3, "food", "料理", "りょうり (ryōri)", "món ăn, việc nấu nướng", "noun", 0, 1, "週末は家で日本料理を作ります。", "Shūmatsu wa ie de Nihon ryōri o tsukurimasu.", "Cuối tuần tôi nấu món Nhật ở nhà."),
        new(3, "food", "野菜", "やさい (yasai)", "rau củ", "noun", 0, 1, "健康のために毎日新鮮な野菜を食べます。", "Kenkō no tame ni mainichi shinsen na yasai o tabemasu.", "Vì sức khỏe nên mỗi ngày tôi đều ăn rau củ tươi."),
        new(3, "food", "味", "あじ (aji)", "hương vị, vị", "noun", 1, 1, "このスープは優しい味がします。", "Kono sūpu wa yasashii aji ga shimasu.", "Món súp này có hương vị thật thanh dịu."),

        // Health & Emotions
        new(3, "health", "元気", "げんき (genki)", "khỏe mạnh, tràn đầy năng lượng", "adjective", 0, 0, "今日も元気に日本語を勉強しましょう！", "Kyō mo genki ni Nihongo o benkyō shimashō!", "Hôm nay cũng tràn đầy năng lượng học tiếng Nhật nhé!"),
        new(3, "health", "安心", "あんしん (anshin)", "yên tâm, an tâm", "noun", 1, 0, "合格の知らせを聞いて安心しました。", "Gōkaku no shirase o kiite anshin shimashita.", "Nghe tin đỗ đạt tôi đã yên tâm rồi."),
        new(3, "health", "習慣", "しゅうかん (shūkan)", "thói quen, tập quán", "noun", 1, 0, "毎朝十分間読書する習慣があります。", "Maiasa jippunkan dokusho suru shūkan ga arimasu.", "Tôi có thói quen đọc sách mười phút mỗi sáng."),
        new(3, "health", "笑顔", "えがお (egao)", "nụ cười, gương mặt tươi cười", "noun", 1, 0, "彼女の笑顔を見ると元気が出ます。", "Kanojo no egao o miru to genki ga demasu.", "Nhìn thấy nụ cười của cô ấy là tôi có thêm động lực."),
        new(3, "health", "夢", "ゆめ (yume)", "ước mơ, giấc mơ", "noun", 0, 0, "通訳になるのが私の夢です。", "Tsūyaku ni naru no ga watashi no yume desu.", "Trở thành phiên dịch viên là ước mơ của tôi."),

        // Culture
        new(3, "culture", "桜", "さくら (sakura)", "hoa anh đào", "noun", 0, 2, "春になると公園の桜が咲きます。", "Haru ni naru to kōen no sakura ga sakimasu.", "Khi mùa xuân đến, hoa anh đào trong công viên nở rộ."),
        new(3, "culture", "祭り", "まつり (matsuri)", "lễ hội", "noun", 0, 2, "夏祭りでたくさんの花火を見ました。", "Natsumatsuri de takusan no hanabi o mimashita.", "Tôi đã xem rất nhiều pháo hoa ở lễ hội mùa hè."),
        new(3, "culture", "伝統", "でんとう (dentō)", "truyền thống", "noun", 1, 2, "着物は日本の美しい伝統文化です。", "Kimono wa Nihon no utsukushii dentō bunka desu.", "Kimono là nét văn hóa truyền thống tươi đẹp của Nhật Bản."),
        new(3, "culture", "アニメ", "anime", "phim hoạt hình Nhật Bản", "noun", 0, 2, "アニメを見て自然な表現を覚えました。", "Anime o mite shizen na hyōgen o oboemashita.", "Tôi đã học được nhiều cách diễn đạt tự nhiên nhờ xem anime."),

        // ==================== 4. KOREAN (LanguageId = 4) ====================
        // Technology
        new(4, "technology", "컴퓨터", "keompyuteo", "máy vi tính", "noun", 0, 0, "새 컴퓨터로 온라인 수업을 들어요.", "Sae keompyuteoro onlain sueobeul deureoyo.", "Tôi học trực tuyến bằng chiếc máy tính mới."),
        new(4, "technology", "인터넷", "inteonet", "mạng Internet", "noun", 0, 0, "한국은 인터넷 속도가 아주 빨라요.", "Hangugeun inteonet sokdoga aju ppallayo.", "Tốc độ Internet ở Hàn Quốc cực kỳ nhanh."),
        new(4, "technology", "인공지능", "ingongjineung", "trí tuệ nhân tạo (AI)", "noun", 1, 0, "인공지능 덕분에 발음 연습이 쉬워졌어요.", "Ingongjineung deokbune bareum yeonseubi swiwojyeosseoyo.", "Nhờ trí tuệ nhân tạo mà việc luyện phát âm trở nên dễ dàng hơn."),
        new(4, "technology", "비밀번호", "bimilbeonho", "mật khẩu", "noun", 0, 0, "안전한 비밀번호를 설정하세요.", "Anjeonhan bimilbeonhoreul seoljeonghaseyo.", "Hãy thiết lập một mật khẩu an toàn."),
        new(4, "technology", "검색", "geomsaek", "tìm kiếm, tra cứu", "noun", 1, 0, "모르는 단어는 사전에서 바로 검색해요.", "Moreuneun daneoneun sajeoneseo baro geomsaekaeyo.", "Từ nào không biết tôi tra ngay trong từ điển."),
        new(4, "technology", "데이터", "deiteo", "dữ liệu", "noun", 1, 0, "모든 단어 데이터가 영구적으로 저장됩니다.", "Modeun daneo deiteoga yeonggujeogeuro jeojangdoemnida.", "Toàn bộ dữ liệu từ vựng được lưu trữ vĩnh viễn."),

        // Business & Study
        new(4, "business", "회의", "hoeui", "cuộc họp", "noun", 1, 2, "오전 열 시에 팀 회의가 있어요.", "Ojeon yeol sie tim hoeuiga isseoyo.", "Mười giờ sáng có cuộc họp nhóm."),
        new(4, "business", "목표", "mokpyo", "mục tiêu", "noun", 1, 0, "올해 목표는 토픽 시험에 합격하는 거예요.", "Olhae mokpyoneun Topik siheome hapgyeokaneun geoyeyo.", "Mục tiêu năm nay của tôi là thi đỗ kỳ thi TOPIK."),
        new(4, "business", "경험", "gyeongheom", "kinh nghiệm, trải nghiệm", "noun", 1, 2, "다양한 경험이 인생을 풍부하게 만들어요.", "Dayanghan gyeongheomi insaengeul pungbuhage mandeureoyo.", "Nhiều trải nghiệm đa dạng giúp cuộc sống thêm phong phú."),
        new(4, "business", "약속", "yaksok", "cuộc hẹn, lời hứa", "noun", 0, 0, "주말에 친구와 저녁 약속이 있어요.", "Jumare chinguwa jeonyeok yaksogi isseoyo.", "Cuối tuần tôi có hẹn ăn tối với bạn."),
        new(4, "business", "준비", "junbi", "sự chuẩn bị", "noun", 0, 0, "발표 준비를 완벽하게 끝냈어요.", "Balpyo junbireul wanbyeokage kkeunnaesseoyo.", "Tôi đã hoàn tất việc chuẩn bị thuyết trình một cách hoàn hảo."),
        new(4, "business", "성공", "seonggong", "thành công", "noun", 1, 2, "꾸준한 노력이 성공을 가져옵니다.", "Kkujunhan noryeogi seonggongeul gajyeoomnida.", "Nỗ lực bền bỉ sẽ mang lại thành công."),

        // Travel
        new(4, "travel", "지하철", "jihacheol", "tàu điện ngầm", "noun", 0, 2, "서울 지하철은 깨끗하고 편리해요.", "Seoul jihacheoreun kkaekkeutago pyeollihaeyo.", "Tàu điện ngầm Seoul sạch sẽ và tiện lợi."),
        new(4, "travel", "공항", "gonghang", "sân bay", "noun", 0, 2, "인천공항에서 비행기를 탔어요.", "Incheongonghang-eseo bihaenggireul tasseoyo.", "Tôi đã lên máy bay tại sân bay Incheon."),
        new(4, "travel", "여행", "yeohaeng", "chuyến du lịch", "noun", 0, 2, "여름 휴가 때 제주도로 여행을 갈 거예요.", "Yeoreum hyuga ttae Jejudoro yeohaengeul gal geoyeyo.", "Kỳ nghỉ hè này tôi sẽ đi du lịch đảo Jeju."),
        new(4, "travel", "예약", "yeyak", "đặt trước", "noun", 1, 2, "식당 자리를 미리 예약했어요.", "Sikdang jarireul miri yeyakaesseoyo.", "Tôi đã đặt trước bàn ở nhà hàng rồi."),
        new(4, "travel", "여권", "yeogwon", "hộ chiếu", "noun", 0, 2, "출국하기 전에 여권을 꼭 확인하세요.", "Chulgukagi jeone yeogwoneul kkok hwaginhaseyo.", "Trước khi xuất cảnh nhất định phải kiểm tra hộ chiếu nhé."),
        new(4, "travel", "기념품", "ginyeompum", "quà lưu niệm", "noun", 1, 2, "명동에서 예쁜 기념품을 샀어요.", "Myeongdong-eseo yeppeun ginyeompumeul sasseoyo.", "Tôi đã mua quà lưu niệm xinh xắn ở Myeongdong."),

        // Food
        new(4, "food", "비빔밥", "bibimbap", "cơm trộn Hàn Quốc", "noun", 0, 1, "야채가 많이 들어간 비빔밥을 좋아해요.", "Yachaega mani deureogan bibimbabeul joahaeyo.", "Tôi thích món cơm trộn có nhiều rau củ."),
        new(4, "food", "김치", "gimchi", "kim chi", "noun", 0, 1, "한국 식탁에는 김치가 빠지지 않아요.", "Hanguk siktageneun gimchiga ppajiji anayo.", "Trên bàn ăn Hàn Quốc không thể thiếu món kim chi."),
        new(4, "food", "떡볶이", "tteokbokki", "bánh gạo cay Tteokbokki", "noun", 0, 1, "학교 앞 분식집 떡볶이가 정말 맛있어요.", "Hakgyo ap bunsikjip tteokbokkiga jeongmal masisseoyo.", "Bánh gạo cay ở quán ăn vặt trước trường thật sự rất ngon."),
        new(4, "food", "커피", "keopi", "cà phê", "noun", 0, 1, "점심 식사 후에 아이스 커피를 마셔요.", "Jeomsim siksa hue aiseu keopireul masyeoyo.", "Sau bữa trưa tôi uống một ly cà phê đá."),
        new(4, "food", "메뉴", "menyu", "thực đơn (menu)", "noun", 0, 1, "오늘의 추천 메뉴는 무엇인가요?", "Oneurui chucheon menyuneun mueosingayo?", "Thực đơn gợi ý hôm nay là món gì vậy ạ?"),

        // Health & Emotions
        new(4, "health", "행복", "haengbok", "hạnh phúc", "noun", 0, 0, "작은 습관이 큰 행복을 만들어요.", "Jageun seupgwani keun haengbogeul mandeureoyo.", "Thói quen nhỏ tạo nên niềm hạnh phúc lớn."),
        new(4, "health", "건강", "geongang", "sức khỏe", "noun", 0, 0, "규칙적인 운동은 건강에 좋아요.", "Gyuchikjeogin undongeun geongang-e joayo.", "Tập thể dục đều đặn rất tốt cho sức khỏe."),
        new(4, "health", "자신감", "jasingam", "sự tự tin", "noun", 1, 0, "큰 소리로 읽으면 자신감이 생겨요.", "Keun soriro ilgeumyeon jasingami saenggyeoyo.", "Đọc to thành tiếng sẽ giúp bạn có thêm sự tự tin."),
        new(4, "health", "습관", "seupgwan", "thói quen", "noun", 1, 0, "매일 단어를 복습하는 좋은 습관을 길러요.", "Maeil daneoreul bokseupaneun joeun seupgwaneul gilleoyo.", "Hãy rèn thói quen tốt ôn tập từ vựng mỗi ngày."),
        new(4, "health", "응원", "eungwon", "sự cổ vũ, ủng hộ", "noun", 1, 0, "항상 당신의 도전을 진심으로 응원해요!", "Hangsang dangsinui dojeoneul jinsimeuro eungwonhaeyo!", "Mình luôn chân thành cổ vũ cho thử thách của bạn!"),

        // Culture
        new(4, "culture", "문화", "munhwa", "văn hóa", "noun", 0, 2, "드라마를 통해 한국 문화를 배웠어요.", "Deuramareul tonghae Hanguk munhwareul baewosseoyo.", "Tôi đã tìm hiểu văn hóa Hàn Quốc qua phim truyền hình."),
        new(4, "culture", "축제", "chukje", "lễ hội", "noun", 1, 2, "가을에는 불꽃놀이 축제가 열려요.", "Gaeureneun bulkkonnori chukjega yeollyeoyo.", "Vào mùa thu lễ hội pháo hoa được tổ chức."),
        new(4, "culture", "음악", "eumak", "âm nhạc", "noun", 0, 2, "신나는 케이팝 음악을 들으며 공부해요.", "Sinnaneun K-pop eumageul deureumyeo gongbuhaeyo.", "Tôi vừa nghe nhạc K-pop sôi động vừa học bài."),
        new(4, "culture", "전통", "jeontong", "truyền thống", "noun", 1, 2, "한복은 한국의 아름다운 전통 의상이에요.", "Hanbogeun Hangugui areumdaun jeontong uisang-ieyo.", "Hanbok là trang phục truyền thống tươi đẹp của Hàn Quốc.")
    ];
}

