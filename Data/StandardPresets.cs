namespace PolyLearnAI.Api.Data;

public static class StandardPresets
{
    public record PresetWord(
        string Term,
        string? Reading,
        string Meaning,
        string PartOfSpeech,
        int TopicOffset, // 0 = Chào hỏi/Đời sống, 1 = Ẩm thực, 2 = Du lịch/Xã hội
        int LevelOffset, // 0 = Level 1, 1 = Level 2
        string Example,
        string? ExampleReading,
        string ExampleVi
    );

    public static readonly Dictionary<string, (string Name, string Description, int LanguageId, IReadOnlyList<PresetWord> Words)> Datasets = new()
    {
        ["oxford"] = (
            "Oxford 3000 / CEFR (Tiếng Anh A1 - A2)",
            "Bộ từ vựng cốt lõi thiết yếu theo khung chuẩn Châu Âu CEFR do Đại học Oxford khuyến nghị.",
            1,
            new List<PresetWord>
            {
                new("Book", "/bʊk/", "quyển sách", "noun", 0, 0, "I am reading an interesting book.", null, "Tôi đang đọc một cuốn sách rất thú vị."),
                new("Family", "/ˈfæm.əl.i/", "gia đình", "noun", 0, 0, "I love spending time with my family.", null, "Tôi thích dành thời gian cho gia đình mình."),
                new("Happy", "/ˈhæp.i/", "hạnh phúc, vui vẻ", "adjective", 0, 0, "They look so happy together.", null, "Họ trông thật hạnh phúc bên nhau."),
                new("Morning", "/ˈmɔː.nɪŋ/", "buổi sáng", "noun", 0, 0, "Good morning, everyone!", null, "Chào buổi sáng mọi người!"),
                new("School", "/skuːl/", "trường học", "noun", 0, 0, "The children walk to school every day.", null, "Lũ trẻ đi bộ đến trường mỗi ngày."),
                new("Student", "/ˈstjuː.dənt/", "học sinh, sinh viên", "noun", 0, 0, "He is a medical student.", null, "Anh ấy là một sinh viên y khoa."),
                new("Teacher", "/ˈtiː.tʃər/", "giáo viên", "noun", 0, 0, "Our English teacher is very kind.", null, "Giáo viên tiếng Anh của chúng tôi rất tốt bụng."),
                new("Time", "/taɪm/", "thời gian, giờ", "noun", 0, 0, "Do you have time for a chat?", null, "Bạn có thời gian trò chuyện một chút không?"),
                new("Money", "/ˈmʌn.i/", "tiền bạc", "noun", 0, 0, "How much money do you need?", null, "Bạn cần bao nhiêu tiền?"),
                new("House", "/haʊs/", "ngôi nhà", "noun", 0, 0, "They live in a beautiful house.", null, "Họ sống trong một ngôi nhà xinh đẹp."),
                new("Music", "/ˈmjuː.zɪk/", "âm nhạc", "noun", 0, 0, "I listen to music while working.", null, "Tôi nghe nhạc trong khi làm việc."),
                new("Computer", "/kəmˈpjuː.tər/", "máy vi tính", "noun", 0, 1, "I work on my computer all day.", null, "Tôi làm việc trên máy tính cả ngày."),
                new("Doctor", "/ˈdɒk.tər/", "bác sĩ", "noun", 0, 1, "You should see a doctor soon.", null, "Bạn nên đi gặp bác sĩ sớm."),
                new("Weather", "/ˈweð.ər/", "thời tiết", "noun", 0, 1, "The weather is lovely today.", null, "Thời tiết hôm nay thật đẹp."),
                new("Language", "/ˈlæŋ.ɡwɪdʒ/", "ngôn ngữ", "noun", 0, 1, "Learning a new language opens many doors.", null, "Học một ngôn ngữ mới mở ra nhiều cơ hội."),
                new("Breakfast", "/ˈbrek.fəst/", "bữa sáng", "noun", 1, 0, "We had eggs and toast for breakfast.", null, "Chúng tôi ăn trứng và bánh mì nướng cho bữa sáng."),
                new("Dinner", "/ˈdɪn.ər/", "bữa tối", "noun", 1, 0, "Let's have dinner together tonight.", null, "Tối nay chúng ta cùng ăn bữa tối nhé."),
                new("Fruit", "/fruːt/", "trái cây, hoa quả", "noun", 1, 0, "Fresh fruit is good for your health.", null, "Trái cây tươi rất tốt cho sức khỏe của bạn."),
                new("Meat", "/miːt/", "thịt", "noun", 1, 0, "She does not eat red meat.", null, "Cô ấy không ăn thịt đỏ."),
                new("Restaurant", "/ˈres.trɒnt/", "nhà hàng, quán ăn", "noun", 1, 1, "This is my favorite Italian restaurant.", null, "Đây là nhà hàng Ý yêu thích của tôi."),
                new("Bus", "/bʌs/", "xe buýt", "noun", 2, 0, "I take the bus to work every morning.", null, "Tôi đi xe buýt đi làm mỗi sáng."),
                new("Station", "/ˈsteɪ.ʃən/", "nhà ga, trạm", "noun", 2, 0, "The train station is right across the street.", null, "Ga tàu hỏa ở ngay bên kia đường."),
                new("Luggage", "/ˈlʌɡ.ɪdʒ/", "hành lý", "noun", 2, 1, "Where can I collect my luggage?", null, "Tôi có thể lấy hành lý ở đâu?"),
                new("Taxi", "/ˈtæk.si/", "xe taxi", "noun", 2, 0, "We took a taxi to the museum.", null, "Chúng tôi đã đón taxi đến bảo tàng."),
                new("Map", "/mæp/", "bản đồ", "noun", 2, 0, "Can you show me on the map?", null, "Bạn có thể chỉ cho tôi trên bản đồ không?")
            }
        ),
        ["hsk"] = (
            "HSK Standard Course (Tiếng Trung HSK 1 - 2)",
            "Bộ từ vựng Hán ngữ đối ngoại chính thức của Bộ Giáo Dục Trung Quốc (Hán Ban).",
            2,
            new List<PresetWord>
            {
                new("学习", "xuéxí", "học tập", "verb", 0, 0, "我们都在努力学习汉语。", "Wǒmen dōu zài nǔlì xuéxí hànyǔ.", "Tất cả chúng tôi đều đang nỗ lực học tiếng Trung."),
                new("老师", "lǎoshī", "thầy cô giáo", "noun", 0, 0, "王老师是我们的班主任。", "Wáng lǎoshī shì wǒmen de bānzhǔrèn.", "Thầy Vương là giáo viên chủ nhiệm của chúng tôi."),
                new("学生", "xuésheng", "học sinh, sinh viên", "noun", 0, 0, "这个学校有两千个学生。", "Zhège xuéxiào yǒu liǎngqiān ge xuésheng.", "Trường này có hai nghìn học sinh."),
                new("书", "shū", "quyển sách", "noun", 0, 0, "桌子上有一本好书。", "Zhuōzi shàng yǒu yì běn hǎo shū.", "Trên bàn có một quyển sách hay."),
                new("家", "jiā", "nhà, gia đình", "noun", 0, 0, "我的家在北京。", "Wǒ de jiā zài Běijīng.", "Nhà của tôi ở Bắc Kinh."),
                new("钱", "qián", "tiền", "noun", 0, 0, "这本书多少钱？", "Zhè běn shū duōshǎo qián?", "Cuốn sách này bao nhiêu tiền?"),
                new("时间", "shíjiān", "thời gian", "noun", 0, 0, "你现在有时间吗？", "Nǐ xiànzài yǒu shíjiān ma?", "Bây giờ bạn có thời gian không?"),
                new("高兴", "gāoxìng", "vui mừng, vui vẻ", "adjective", 0, 0, "认识你我很高兴。", "Rènshi nǐ wǒ hěn gāoxìng.", "Rất vui được làm quen với bạn."),
                new("早上", "zǎoshang", "buổi sáng", "noun", 0, 0, "我早上七点起床。", "Wǒ zǎoshang qī diǎn qǐchuáng.", "Tôi thức dậy lúc 7 giờ sáng."),
                new("晚上", "wǎnshang", "buổi tối", "noun", 0, 0, "晚上我们一起看电影吧。", "Wǎnshang wǒmen yìqǐ kàn diànyǐng ba.", "Buổi tối chúng mình cùng xem phim nhé."),
                new("电脑", "diànnǎo", "máy vi tính", "noun", 0, 1, "他在电脑前工作了一整天。", "Tā zài diànnǎo qián gōngzuò le yì zhěng tiān.", "Anh ấy làm việc trước máy tính cả ngày."),
                new("手机", "shǒujī", "điện thoại di động", "noun", 0, 1, "我的手机没电了。", "Wǒ de shǒujī méi diàn le.", "Điện thoại của tôi hết pin rồi."),
                new("医生", "yīshēng", "bác sĩ", "noun", 0, 1, "他是这家医院最好的医生。", "Tā shì zhè jiā yīyuàn zuì hǎo de yīshēng.", "Ông ấy là bác sĩ giỏi nhất bệnh viện này."),
                new("天气", "tiānqì", "thời tiết", "noun", 0, 1, "今天天气非常晴朗。", "Jīntiān tiānqì fēicháng qínglǎng.", "Thời tiết hôm nay rất quang đãng."),
                new("汉语", "hànyǔ", "tiếng Hán, tiếng Trung", "noun", 0, 0, "说汉语很有意思。", "Shuō hànyǔ hěn yǒu yìsi.", "Nói tiếng Trung rất thú vị."),
                new("米饭", "mǐfàn", "cơm trắng", "noun", 1, 0, "请给我一碗热米饭。", "Qǐng gěi wǒ yì wǎn rè mǐfàn.", "Cho tôi xin một bát cơm nóng."),
                new("茶", "chá", "trà", "noun", 1, 0, "中国人很喜欢喝绿茶。", "Zhōngguó rén hěn xǐhuan hē lǜchá.", "Người Trung Quốc rất thích uống trà xanh."),
                new("菜", "cài", "món ăn, thức ăn, rau", "noun", 1, 0, "今天的菜做得真好吃！", "Jīntiān de cài zuò de zhēn hǎochī!", "Món ăn hôm nay nấu thật ngon!"),
                new("水果", "shuǐguǒ", "trái cây, hoa quả", "noun", 1, 0, "多吃水果对健康有好处。", "Duō chī shuǐguǒ duì jiànkāng yǒu hǎochu.", "Ăn nhiều hoa quả tốt cho sức khỏe."),
                new("饭馆", "fànguǎn", "quán ăn, nhà hàng", "noun", 1, 1, "学校旁边有一家好饭馆。", "Xuéxiào pángbiān yǒu yì jiā hǎo fànguǎn.", "Cạnh trường học có một quán ăn rất ngon."),
                new("公共汽车", "gōnggòng qìchē", "xe buýt", "noun", 2, 1, "我坐公共汽车去上班。", "Wǒ zuò gōnggòng qìchē qù shàngbān.", "Tôi đi làm bằng xe buýt."),
                new("出租车", "chūzūchē", "xe taxi", "noun", 2, 1, "外面下雨了，我们坐出租车吧。", "Wàimiàn xiàyǔ le, wǒmen zuò chūzūchē ba.", "Bên ngoài trời mưa rồi, chúng ta đi taxi nhé."),
                new("车站", "chēzhàn", "trạm xe, bến xe", "noun", 2, 1, "汽车站在学校对面。", "Qìchēzhàn zài xuéxiào duìmiàn.", "Bến xe buýt ở đối diện trường học."),
                new("买东西", "mǎi dōngxi", "mua sắm đồ đạc", "phrase", 2, 0, "周末妈妈常去超市买东西。", "Zhōumò māma cháng qù chāoshì mǎi dōngxi.", "Cuối tuần mẹ thường đi siêu thị mua sắm."),
                new("衣服", "yīfu", "quần áo, y phục", "noun", 2, 0, "这件衣服不仅漂亮而且便宜。", "Zhè jiàn yīfu bùjǐn piàoliang érqiě piányi.", "Bộ quần áo này vừa đẹp lại vừa rẻ.")
            }
        ),
        ["jlpt"] = (
            "JLPT Official Vocabulary (Tiếng Nhật N5 - N4)",
            "Bộ từ vựng chuẩn kỳ thi Năng lực Nhật ngữ quốc tế JLPT (Japan Foundation & JEES).",
            3,
            new List<PresetWord>
            {
                new("勉強", "べんきょう (benkyō)", "học tập", "noun", 0, 0, "毎晩、日本語を勉強します。", "Maiban, nihongo o benkyō shimasu.", "Mỗi tối tôi đều học tiếng Nhật."),
                new("先生", "せんせい (sensei)", "thầy cô giáo", "noun", 0, 0, "先生、質問があります。", "Sensei, shitsumon ga arimasu.", "Thưa thầy, em có một câu hỏi ạ."),
                new("学生", "がくせい (gakusei)", "học sinh, sinh viên", "noun", 0, 0, "彼は大学の学生です。", "Kare wa daigaku no gakusei desu.", "Anh ấy là sinh viên đại học."),
                new("本", "ほん (hon)", "quyển sách", "noun", 0, 0, "図書館で本を借りました。", "Toshokan de hon o karimashita.", "Tôi đã mượn sách ở thư viện."),
                new("家族", "かぞく (kazoku)", "gia đình", "noun", 0, 0, "家族と一緒に住んでいます。", "Kazoku to issho ni sunde imasu.", "Tôi đang sống cùng với gia đình."),
                new("お金", "おかね (okane)", "tiền bạc", "noun", 0, 0, "財布にお金がありません。", "Saifu ni okane ga arimasen.", "Trong ví tôi không có tiền."),
                new("時間", "じかん (jikan)", "thời gian", "noun", 0, 0, "今は時間がありません。", "Ima wa jikan ga arimasen.", "Bây giờ tôi không có thời gian."),
                new("嬉しい", "うれしい (ureshii)", "vui mừng, hạnh phúc", "adjective", 0, 0, "プレゼントをもらって嬉しいです。", "Purezento o moratte ureshii desu.", "Nhận được quà tôi rất vui mừng."),
                new("朝", "あさ (asa)", "buổi sáng", "noun", 0, 0, "朝早く起きて散歩します。", "Asa hayaku okite sanpo shimasu.", "Buổi sáng tôi dậy sớm đi dạo."),
                new("夜", "よる (yoru)", "buổi tối, ban đêm", "noun", 0, 0, "夜は静かで落ち着きます。", "Yoru wa shizuka de ochitsukimasu.", "Ban đêm yên tĩnh và dễ chịu."),
                new("パソコン", "パソコン (pasokon)", "máy tính cá nhân", "noun", 0, 1, "新しいパソコンを買いました。", "Atarashii pasokon o kaimashita.", "Tôi đã mua một chiếc máy tính mới."),
                new("携帯", "けいたい (keitai)", "điện thoại di động", "noun", 0, 1, "携帯電話を忘れました。", "Keitai denwa o wasuremashita.", "Tôi đã để quên điện thoại di động rồi."),
                new("医者", "いしゃ (isha)", "bác sĩ", "noun", 0, 1, "医者に診てもらいました。", "Isha ni mite moraimashita.", "Tôi đã được bác sĩ khám bệnh."),
                new("天気", "てんき (tenki)", "thời tiết", "noun", 0, 1, "明日の天気はどうですか。", "Ashita no tenki wa dō desu ka.", "Thời tiết ngày mai thế nào?"),
                new("日本語", "にほんご (nihongo)", "tiếng Nhật", "noun", 0, 0, "日本語を上手に話したいです。", "Nihongo o jōzu ni hanashitai desu.", "Tôi muốn nói tiếng Nhật thật lưu loát."),
                new("ご飯", "ごはん (gohan)", "cơm, bữa ăn", "noun", 1, 0, "お昼ご飯を一緒に食べましょう。", "Ohiru gohan o issho ni tabemashō.", "Chúng ta cùng ăn cơm trưa nhé."),
                new("お茶", "おちゃ (ocha)", "trà xanh", "noun", 1, 0, "温かいお茶をどうぞ。", "Atatakai ocha o dōzo.", "Xin mời dùng trà nóng."),
                new("肉", "にく (niku)", "thịt", "noun", 1, 0, "牛肉と豚肉のどちらが好きですか。", "Gyūniku to butaniku no dochira ga suki desu ka.", "Bạn thích thịt bò hay thịt lợn hơn?"),
                new("魚", "さかな (sakana)", "cá", "noun", 1, 0, "日本人は魚をよく食べます。", "Nihonjin wa sakana o yoku tabemasu.", "Người Nhật rất thường ăn cá."),
                new("レストラン", "レストラン (resutoran)", "nhà hàng quán ăn", "noun", 1, 1, "駅前のレストランで夕食をとりました。", "Ekimae no resutoran de yūshoku o torimashita.", "Tôi đã ăn tối ở nhà hàng trước ga."),
                new("バス", "バス (basu)", "xe buýt", "noun", 2, 0, "バスに乗って出かけます。", "Basu ni notte dekakemasu.", "Tôi đón xe buýt đi ra ngoài."),
                new("駅", "えき (eki)", "nhà ga", "noun", 2, 0, "次の駅で降ります。", "Tsugi no eki de orimasu.", "Tôi sẽ xuống ở nhà ga tiếp theo."),
                new("荷物", "にもつ (nimotsu)", "hành lý, đồ đạc", "noun", 2, 1, "重い荷物を持っています。", "Omoi nimotsu o motte imasu.", "Tôi đang xách hành lý nặng."),
                new("タクシー", "タクシー (takushī)", "xe taxi", "noun", 2, 0, "急いでいるのでタクシーを呼びます。", "Isoide iru node takushī o yobimasu.", "Vì đang vội nên tôi sẽ gọi taxi."),
                new("服", "ふく (fuku)", "quần áo, trang phục", "noun", 2, 0, "新しい服を買いに行きます。", "Atarashii fuku o kai ni ikimasu.", "Tôi đi mua quần áo mới.")
            }
        ),
        ["topik"] = (
            "TOPIK I Official Core (Tiếng Hàn Cấp 1 - 2)",
            "Bộ từ vựng chuẩn kỳ thi Năng lực tiếng Hàn TOPIK do Viện Giáo dục Quốc tế NIIED Hàn Quốc ban hành.",
            4,
            new List<PresetWord>
            {
                new("공부", "gongbu", "học tập", "noun", 0, 0, "매일 도서관에서 한국어 공부를 해요.", "Maeil doseogwaneseo hangugeo gongbureul haeyo.", "Mỗi ngày tôi đều học tiếng Hàn ở thư viện."),
                new("선생님", "seonsaengnim", "thầy cô giáo", "noun", 0, 0, "선생님, 질문이 있습니다.", "Seonsaengnim, jilmuni itseumnida.", "Thưa thầy, em có câu hỏi ạ."),
                new("학생", "haksaeng", "học sinh, sinh viên", "noun", 0, 0, "저는 서울대학교 학생입니다.", "Jeoneun seouldaehakgyo haksaeng-imnida.", "Tôi là sinh viên trường Đại học Seoul."),
                new("책", "chaek", "quyển sách", "noun", 0, 0, "시간이 있을 때 책을 읽어요.", "Sigani isseul ttae chaegeul ilgeoyo.", "Khi có thời gian tôi thường đọc sách."),
                new("가족", "gajok", "gia đình", "noun", 0, 0, "우리 가족은 모두 네 명이에요.", "Uri gajogeun modu ne myeong-ieyo.", "Gia đình tôi có tất cả 4 người."),
                new("돈", "don", "tiền bạc", "noun", 0, 0, "돈을 아껴 써야 해요.", "Doneul akkyeo sseoya haeyo.", "Phải biết chi tiêu tiết kiệm tiền."),
                new("시간", "sigan", "thời gian", "noun", 0, 0, "약속 시간이 언제예요?", "Yaksok sigani eonjeyeyo?", "Thời gian hẹn là khi nào vậy?"),
                new("행복", "haengbok", "hạnh phúc", "noun", 0, 0, "가족과 함께 있을 때 가장 행복해요.", "Gajokgwa hamkke isseul ttae gajang haengbokaeyo.", "Ở bên gia đình là lúc tôi hạnh phúc nhất."),
                new("아침", "achim", "buổi sáng / bữa sáng", "noun", 0, 0, "아침 일찍 운동을 시작했어요.", "Achim iljjik undong-eul sijakaesseoyo.", "Buổi sáng tôi dậy sớm bắt đầu tập thể dục."),
                new("저녁", "jeonyeok", "buổi tối / bữa tối", "noun", 0, 0, "오늘 저녁에 같이 밥 먹을까요?", "Oneul jeonyeoge gachi bap meogeulkkayo?", "Tối nay chúng mình cùng đi ăn cơm nhé?"),
                new("컴퓨터", "keompyuteo", "máy tính", "noun", 0, 1, "컴퓨터로 이메일을 보냈어요.", "Keompyuteoro imeireul bonaesseoyo.", "Tôi đã gửi email bằng máy vi tính."),
                new("핸드폰", "haendeupon", "điện thoại di động", "noun", 0, 1, "새 핸드폰을 샀어요.", "Sae haendeuponeul sasseoyo.", "Tôi đã mua một chiếc điện thoại mới."),
                new("의사", "uisa", "bác sĩ", "noun", 0, 1, "몸이 아파서 의사 선생님을 만났어요.", "Momi apaseo uisa seonsaengnimeul mannasseoyo.", "Vì người mệt nên tôi đã đến gặp bác sĩ."),
                new("날씨", "nalssi", "thời tiết", "noun", 0, 1, "오늘 서울 날씨가 참 맑아요.", "Oneul seoul nalssiga cham malgayo.", "Thời tiết Seoul hôm nay rất trong xanh."),
                new("한국어", "hangugeo", "tiếng Hàn", "noun", 0, 0, "한국어 발음이 조금 어려워요.", "Hangugeo bareumi jogeum eoryeowoyo.", "Phát âm tiếng Hàn hơi khó một chút."),
                new("밥", "bap", "cơm, bữa cơm", "noun", 1, 0, "한국 사람은 밥과 김치를 먹어요.", "Hanguk sarameun bapgwa gimchireul meogeoyo.", "Người Hàn Quốc ăn cơm cùng kim chi."),
                new("차", "cha", "trà", "noun", 1, 0, "식사 후에 따뜻한 차를 마셔요.", "Siksa hue ttatteutan chareul masyeoyo.", "Sau bữa ăn tôi uống một tách trà ấm."),
                new("고기", "gogi", "thịt", "noun", 1, 0, "불고기는 달콤하고 맛있어요.", "Bulgogineun dalkomhago masisseoyo.", "Thịt nướng Bulgogi ngọt và rất ngon."),
                new("과일", "gwail", "trái cây, hoa quả", "noun", 1, 0, "시장에는 신선한 과일이 많아요.", "Sijang-eneun sinseonhan gwairi manayo.", "Ở chợ có rất nhiều trái cây tươi ngon."),
                new("식당", "sikdang", "nhà hàng, quán ăn", "noun", 1, 1, "이 식당은 김치찌개가 유명해요.", "I sikdangeun gimchijjigaeyo yumyeonghaeyo.", "Quán ăn này nổi tiếng với món canh kim chi."),
                new("버스", "beoseu", "xe buýt", "noun", 2, 0, "버스를 타고 시내에 가요.", "Beoseureul tago sinae-e gayo.", "Tôi đón xe buýt đi vào trung tâm thành phố."),
                new("지하철", "jihacheol", "tàu điện ngầm", "noun", 2, 0, "지하철이 빠르고 편리해요.", "Jihacheori ppareugo pyeonrihaeyo.", "Tàu điện ngầm chạy nhanh và rất thuận tiện."),
                new("짐", "jim", "hành lý, đồ đạc", "noun", 2, 1, "여행 짐을 미리 싸 두었어요.", "Yeohaeng jimeul miri ssa dueosseoyo.", "Tôi đã chuẩn bị sẵn hành lý du lịch từ trước."),
                new("택시", "taeksi", "xe taxi", "noun", 2, 0, "늦을 것 같아서 택시를 탔어요.", "Neujeul geot gat-aseo taeksireul tasseoyo.", "Sợ bị muộn nên tôi đã bắt taxi đi."),
                new("옷", "ot", "quần áo, trang phục", "noun", 2, 0, "백화점에서 예쁜 옷을 구경했어요.", "Baekhwajeomeseo yeppeun oseul gugyeonghaesseoyo.", "Tôi đã ngắm nhìn những bộ quần áo đẹp trong trung tâm thương mại.")
            }
        )
    };
}
