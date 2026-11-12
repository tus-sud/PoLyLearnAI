document.addEventListener('DOMContentLoaded', () => {
  // Global State
  const state = {
    currentView: 'lessons',
    activeLanguageId: API.getActiveLanguageId(),
    selectedMockLevelIndex: 3,
    quizTimerInterval: null,
    quizTimerRemaining: 0,
    activeMockExamSpec: null,
    lastSpokenQuestionId: null,
    user: API.getUser(),
    recentHarvestedWordIds: new Set(),
    quizSession: null,
    quizCurrentQuestionIndex: 0,
    quizAnswers: {},
    quizWrittenAnswers: {},
    chatHistory: [],
    lessonFilterType: 'all',
    lessonSearch: '',
    lessonLevelId: '',
    cachedLevels: [],
    currentLesson: null,
    wordFavoritesOnly: false,
    wordLevelFilter: '',
    wordsPage: 1,
    wordsTotalPages: 1,
    selectedWordId: null,
    spotlightWord: null,
    profileData: null,
    selectedAvatarKey: 'owl',
    selectedCompanionKey: 'auto',
    selectedDailyGoalXp: 50,
    languages: {
      1: {
        code: 'en',
        locale: 'en-US',
        name: 'Tiếng Anh',
        flag: '🇬🇧',
        standardLabel: '🇬🇧 Tiếng Anh • Chuẩn CEFR A1–C2',
        motto: '✨ "Practice makes progress — Luyện phản xạ tiếng Anh mỗi ngày!"',
        grammarSamples: [
          'She go to the office yesterday and forgetted her laptop.',
          'I am very interesting in learning artificial intelligence.',
          'If I know his number, I would called him earlier.'
        ],
        explainSamples: ['break the ice', 'resilient', 'ubiquitous', 'piece of cake'],
        starters: {
          casual_talk: ['Hi! How has your week been going so far?', 'What do you usually do to relax on weekends?'],
          restaurant: ['Could I see the menu and today’s specials, please?', 'I would like to book a table for two near the window.'],
          airport: ['Hello, I am checking in for my flight to London.', 'Where is boarding gate B12 located?'],
          shopping: ['Do you have this jacket in a medium size?', 'Is there any discount if I buy two items?'],
          job_interview: ['Hello! Thank you for inviting me to interview today.', 'Could you tell me more about the team culture?']
        }
      },
      2: {
        code: 'zh',
        locale: 'zh-CN',
        name: 'Tiếng Trung',
        flag: '🇨🇳',
        standardLabel: '🇨🇳 Tiếng Trung • Chuẩn HSK 1–6',
        motto: '🏮 "千里之行，始于足下 — Vạn dặm khởi đầu từ một bước chân!"',
        grammarSamples: [
          '我昨天去商店买了很多水果了。',
          '他比我很高一点儿。',
          '我已经吃饭在家里。'
        ],
        explainSamples: ['迫不及待', '入乡随俗', '效率', '人工智能'],
        starters: {
          casual_talk: ['你好！你周末一般喜欢做什么？', '最近工作忙不忙？'],
          restaurant: ['服务员，请给我看一下菜单！', '请问这家店有什么特色菜推荐？'],
          airport: ['你好，我想办理去北京航班的登机手续。', '请问托运行李在哪里办理？'],
          shopping: ['这件衣服可以试穿一下吗？', '太贵了，可以便宜一点儿吗？'],
          job_interview: ['您好！很高兴今天能参加贵公司的面试。', '请允许我简单介绍一下自己的工作经验。']
        }
      },
      3: {
        code: 'ja',
        locale: 'ja-JP',
        name: 'Tiếng Nhật',
        flag: '🇯🇵',
        standardLabel: '🇯🇵 Tiếng Nhật • Chuẩn JLPT N5–N1',
        motto: '🌸 "継続は力なり — Kiên trì bền bỉ chính là sức mạnh!"',
        grammarSamples: [
          '昨日、友達と一緒に映画を見るに行きました。',
          '日本語は難しいですが、とても楽しいだ。',
          '毎朝コーヒーを飲みてから会社へ行きます。'
        ],
        explainSamples: ['一期一会', '懐かしい', '臨機応変', 'お疲れ様です'],
        starters: {
          casual_talk: ['こんにちは！週末はどんなことをして過ごしましたか？', '最近ハマっている趣味はありますか？'],
          restaurant: ['すみません、おすすめの料理は何ですか？', 'お会計をお願いします。カードは使えますか？'],
          airport: ['東京行きの便にチェックインお願いします。', '窓側の席をお願いできますか？'],
          shopping: ['これのもう少し大きいサイズはありますか？', '免税の手続きはどこでできますか？'],
          job_interview: ['本日は面接の機会をいただき、ありがとうございます。', '御社のプロジェクトに大変興味があります。']
        }
      },
      4: {
        code: 'ko',
        locale: 'ko-KR',
        name: 'Tiếng Hàn',
        flag: '🇰🇷',
        standardLabel: '🇰🇷 Tiếng Hàn • Chuẩn TOPIK I–II',
        motto: '✨ "천리길도 한 걸음부터 — Đường vạn dặm bắt đầu từ bước đầu tiên!"',
        grammarSamples: [
          '어제 친구하고 같이 영화를 보러 가요.',
          '한국어를 공부하는 것이 너무 재미있어요지만 어려워요.',
          '내일 시간이 있으니까 같이 커피 마실래요?'
        ],
        explainSamples: ['소확행', '눈치가 빠르다', '일석이조', '수고하셨습니다'],
        starters: {
          casual_talk: ['안녕하세요! 요즘 어떻게 지내고 계세요?', '주말에 보통 뭐 하면서 시간을 보내세요?'],
          restaurant: ['저기요, 여기 인기 메뉴가 뭐예요?', '덜 맵게 해 주실 수 있나요?'],
          airport: ['서울행 비행기 탑승 수속을 하고 싶습니다.', '수하물은 몇 개까지 부칠 수 있나요?'],
          shopping: ['이 옷 다른 색상도 있나요?', '카드로 계산해도 될까요?'],
          job_interview: ['안녕하십니까! 오늘 면접 기회를 주셔서 감사합니다.', '제 강점과 직무 경험에 대해 말씀드리겠습니다.']
        }
      }
    }
  };

  // DOM Elements
  const navBtns = document.querySelectorAll('.nav-btn');
  const viewSections = document.querySelectorAll('.view-section');
  const globalLangSelect = document.getElementById('global-lang-select');
  const authSection = document.getElementById('auth-section');
  const btnToggleTheme = document.getElementById('btn-toggle-theme');
  const btnToggleSfx = document.getElementById('btn-toggle-sfx');
  const brandLogo = document.getElementById('brand-logo');

  // Theme & Language-Adaptive Palette Controller
  function applyTheme(theme) {
    const normalized = theme === 'dark' ? 'dark' : 'light';
    document.body.dataset.theme = normalized;
    localStorage.setItem('polylearn_theme', normalized);
    if (btnToggleTheme) {
      btnToggleTheme.textContent = normalized === 'dark' ? '☀️' : '🌙';
      btnToggleTheme.title = normalized === 'dark'
        ? 'Chuyển sang giao diện Sáng tươi mới ☀️'
        : 'Chuyển sang giao diện Tối dịu mắt 🌙';
    }
  }

  function applyLanguagePalette(langId) {
    document.body.dataset.lang = String(langId || 1);
  }

  applyTheme(localStorage.getItem('polylearn_theme') || 'light');
  applyLanguagePalette(state.activeLanguageId);

  if (btnToggleTheme) {
    btnToggleTheme.addEventListener('click', () => {
      const nextTheme = document.body.dataset.theme === 'dark' ? 'light' : 'dark';
      applyTheme(nextTheme);
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      showToast(
        nextTheme === 'light'
          ? 'Đã chuyển sang giao diện Sáng tươi mới ☀️'
          : 'Đã chuyển sang giao diện Tối dịu mắt 🌙',
        'info'
      );
    });
  }

  // Gamification & Mascot Helpers
  function getUserCompanionPref() {
    return state.user?.companionKey || 'auto';
  }

  function refreshMascots(mood = 'idle', customText = null) {
    if (typeof Gamification === 'undefined') return;
    const companionPref = getUserCompanionPref();
    Gamification.updateCompanionDock({
      companionPref,
      langId: state.activeLanguageId,
      mood,
      customText
    });

    const heroMascotEl = document.getElementById('hero-mascot-figure');
    if (heroMascotEl) {
      const comp = Gamification.getCompanion(companionPref, state.activeLanguageId);
      heroMascotEl.innerHTML = comp.svg;
    }
  }

  function syncRibbonMetrics({ streak, todayXp, goalXp } = {}) {
    if (typeof streak === 'number') {
      const el = document.getElementById('ribbon-streak-val');
      if (el) el.textContent = streak;
      const topStreak = document.getElementById('top-stat-streak');
      if (topStreak) topStreak.textContent = streak;
      const sideStreak = document.getElementById('side-streak-count');
      if (sideStreak) sideStreak.textContent = streak;
    }
    if (typeof todayXp === 'number' || typeof goalXp === 'number') {
      const tXp = typeof todayXp === 'number' ? todayXp : (state.user?.todayXp || 0);
      const gXp = typeof goalXp === 'number' ? goalXp : (state.user?.dailyGoalXp || 50);
      const el = document.getElementById('ribbon-xp-val');
      if (el) el.textContent = `${tXp}/${gXp}`;
    }
    syncShellUser();
  }

  // Sidebar user card + top stat chips (XP / level) from the cached user
  function syncShellUser() {
    const u = state.user;
    const setT = (id, v) => { const e = document.getElementById(id); if (e) e.textContent = v; };
    setT('top-stat-xp', u ? (u.experiencePoints || 0) : 0);
    setT('top-stat-level', u ? (u.level || 1) : 1);
    setT('side-user-name', u ? (u.displayName || 'Học viên') : 'Khách');
    setT('side-user-rank', u ? (u.rankTitle || `Cấp ${u.level || 1}`) : 'Đăng nhập để lưu tiến độ');
    const av = document.getElementById('side-user-avatar');
    if (av) {
      if (u && typeof Gamification !== 'undefined' && Gamification.getAvatarSvg) {
        av.innerHTML = Gamification.getAvatarSvg(u.avatarKey || 'owl');
      } else {
        av.textContent = u ? (u.displayName || '?').trim().charAt(0).toUpperCase() : '?';
      }
    }
    const sideAdmin = document.getElementById('side-nav-admin');
    if (sideAdmin) sideAdmin.style.display = u && u.role === 'Admin' ? 'flex' : 'none';
  }

  // 7-day dots in the sidebar streak card
  function renderSideWeek(activeDates = []) {
    const row = document.getElementById('side-week-row');
    if (!row) return;
    const labels = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7'];
    const activeSet = new Set((activeDates || []).map(d => String(d).slice(0, 10)));
    const now = new Date();
    let html = '';
    for (let i = 6; i >= 0; i--) {
      const d = new Date(Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate() - i));
      const key = d.toISOString().slice(0, 10);
      const on = activeSet.has(key);
      html += `<div class="side-day ${on ? 'on' : ''} ${i === 0 ? 'today' : ''}"><small>${labels[d.getUTCDay()]}</small><span>${on ? '✓' : ''}</span></div>`;
    }
    row.innerHTML = html;
  }

  function updateLanguageRibbon() {
    const langCfg = state.languages[state.activeLanguageId] || state.languages[1];
    const badgeEl = document.getElementById('ribbon-lang-badge');
    const dashTagEl = document.getElementById('dash-lang-standard-tag');
    const dashMottoEl = document.getElementById('dash-mastery-motto');
    const dateLabelEl = document.getElementById('dash-today-date-label');

    if (badgeEl) badgeEl.textContent = langCfg.standardLabel;
    if (dashTagEl) dashTagEl.textContent = langCfg.standardLabel;
    if (dashMottoEl) dashMottoEl.textContent = langCfg.motto;
    if (dateLabelEl) {
      dateLabelEl.textContent = new Date().toLocaleDateString('vi-VN', {
        weekday: 'long',
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
      });
    }
  }

  function handleXpGain(res, anchorEl = null) {
    if (!res || typeof Gamification === 'undefined') return;
    if (res.xpEarned > 0) {
      Gamification.spawnFloatingXp(res.xpEarned, anchorEl);
    }
    if (state.user) {
      if (typeof res.totalXp === 'number') state.user.experiencePoints = res.totalXp;
      if (typeof res.todayXp === 'number') state.user.todayXp = res.todayXp;
      if (typeof res.dailyGoalXp === 'number') state.user.dailyGoalXp = res.dailyGoalXp;
      if (typeof res.level === 'number') state.user.level = res.level;
      API.setUser(state.user);
      renderAuthStatus();
      syncRibbonMetrics({ todayXp: state.user.todayXp, goalXp: state.user.dailyGoalXp });
    }
    if (res.leveledUp && res.level) {
      Gamification.showLevelUpCelebration(res.level);
    }
  }

  // Sound Effects Toggle & Companion Dock Events
  if (btnToggleSfx && typeof Gamification !== 'undefined') {
    btnToggleSfx.textContent = Gamification.isSoundEnabled() ? '🔊' : '🔇';
    btnToggleSfx.addEventListener('click', () => {
      const enabled = Gamification.toggleSound();
      btnToggleSfx.textContent = enabled ? '🔊' : '🔇';
      showToast(enabled ? 'Đã bật âm thanh hiệu ứng 🔊' : 'Đã tắt âm thanh hiệu ứng 🔇', 'info');
    });
  }

  const companionDock = document.getElementById('companion-dock');
  const companionDockAvatar = document.getElementById('companion-dock-avatar');
  const btnCompanionMinimize = document.getElementById('btn-companion-minimize');
  const btnCompanionCustomize = document.getElementById('btn-companion-customize');
  const heroMascotFigure = document.getElementById('hero-mascot-figure');

  // On small mobile screens, default companion to minimized pill so it doesn't obstruct cards
  if (companionDock && window.innerWidth <= 768) {
    companionDock.classList.add('minimized');
    if (btnCompanionMinimize) btnCompanionMinimize.innerHTML = '💬';
  }

  if (companionDockAvatar) {
    companionDockAvatar.addEventListener('click', () => {
      if (companionDock) {
        if (companionDock.classList.contains('minimized')) {
          companionDock.classList.remove('minimized');
          if (btnCompanionMinimize) btnCompanionMinimize.innerHTML = '&minus;';
        } else if (window.innerWidth <= 768) {
          companionDock.classList.add('minimized');
          if (btnCompanionMinimize) btnCompanionMinimize.innerHTML = '💬';
        }
      }
      Gamification.playSound('pop');
      Gamification.spawnHeartBurst(companionDockAvatar);
      refreshMascots('happy');
    });
  }

  if (heroMascotFigure) {
    heroMascotFigure.addEventListener('click', () => {
      Gamification.playSound('pop');
      Gamification.spawnHeartBurst(heroMascotFigure);
      refreshMascots('happy');
    });
  }

  if (btnCompanionMinimize && companionDock) {
    btnCompanionMinimize.addEventListener('click', () => {
      const isMin = companionDock.classList.toggle('minimized');
      btnCompanionMinimize.innerHTML = isMin ? '💬' : '&minus;';
    });
  }

  if (btnCompanionCustomize) {
    btnCompanionCustomize.addEventListener('click', () => switchView('profile'));
  }

  const sideUserCard = document.getElementById('side-user-card');
  if (sideUserCard) {
    sideUserCard.addEventListener('click', () => {
      if (API.getToken()) switchView('profile');
      else openAuthModal();
    });
  }
  renderSideWeek([]);

  if (brandLogo) {
    brandLogo.addEventListener('click', (e) => {
      e.preventDefault();
      switchView('dashboard');
    });
  }

  const btnDashGoExam = document.getElementById('btn-dash-go-exam');
  const btnDashGoProfile = document.getElementById('btn-dash-go-profile');
  if (btnDashGoExam) btnDashGoExam.addEventListener('click', () => switchView('quizzes'));
  if (btnDashGoProfile) btnDashGoProfile.addEventListener('click', () => switchView('profile'));

  window.addEventListener('auth-changed', () => {
    renderAuthStatus();
  });

  // Modals
  const authModal = document.getElementById('auth-modal');
  const btnOpenLogin = document.getElementById('btn-open-login');
  const btnCloseModal = document.getElementById('btn-close-modal');
  const tabLogin = document.getElementById('tab-login');
  const tabRegister = document.getElementById('tab-register');
  const authForm = document.getElementById('auth-form');
  const authModalTitle = document.getElementById('auth-modal-title');
  const groupDisplayName = document.getElementById('group-displayname');
  const btnSubmitAuth = document.getElementById('btn-submit-auth');
  const btnQuickAdmin = document.getElementById('btn-quick-admin');
  let authMode = 'login'; // 'login' | 'register'

  // Toast Function
  function showToast(message, type = 'info') {
    const container = document.getElementById('toast-container');
    const toast = document.createElement('div');
    toast.className = 'toast';
    if (type === 'success') toast.style.borderColor = 'var(--success)';
    if (type === 'error') toast.style.borderColor = 'var(--danger)';
    toast.textContent = message;
    container.appendChild(toast);
    setTimeout(() => {
      toast.remove();
    }, 3500);
  }

  // TTS Voice
  function speak(text) {
    if (!('speechSynthesis' in window)) return;
    window.speechSynthesis.cancel();
    const langConfig = state.languages[state.activeLanguageId] || state.languages[1];
    const utterance = new SpeechSynthesisUtterance(text);
    utterance.lang = langConfig.locale;
    utterance.rate = 0.9;
    window.speechSynthesis.speak(utterance);
  }

  // Unified Subtab Controller inside #view-lessons (Gộp chung Lộ trình + Từ vựng + Câu/Từ làm sai)
  let currentUnifiedSubtab = 'all'; // 'all' | 'roadmap' | 'vocab' | 'mistakes'

  function switchUnifiedLessonsSubtab(subtab, scrollIntoTarget = false) {
    currentUnifiedSubtab = subtab || 'all';
    document.querySelectorAll('.unified-subtab-btn').forEach(btn => {
      btn.classList.toggle('active', btn.dataset.subtab === currentUnifiedSubtab);
    });

    const roadmapEl = document.getElementById('lessons-roadmap-subview');
    const vocabEl = document.getElementById('lessons-vocab-subview');
    const mistakesEl = document.getElementById('lessons-mistakes-subview');

    if (currentUnifiedSubtab === 'all') {
      if (roadmapEl) roadmapEl.style.display = 'block';
      if (vocabEl) vocabEl.style.display = 'block';
      if (mistakesEl) mistakesEl.style.display = 'block';
    } else if (currentUnifiedSubtab === 'roadmap') {
      if (roadmapEl) roadmapEl.style.display = 'block';
      if (vocabEl) vocabEl.style.display = 'none';
      if (mistakesEl) mistakesEl.style.display = 'none';
    } else if (currentUnifiedSubtab === 'vocab') {
      if (roadmapEl) roadmapEl.style.display = 'none';
      if (vocabEl) vocabEl.style.display = 'block';
      if (mistakesEl) mistakesEl.style.display = 'none';
    } else if (currentUnifiedSubtab === 'mistakes') {
      if (roadmapEl) roadmapEl.style.display = 'none';
      if (vocabEl) vocabEl.style.display = 'none';
      if (mistakesEl) mistakesEl.style.display = 'block';
    }

    if (typeof MistakeTracker !== 'undefined') {
      MistakeTracker.render();
    }

    if (scrollIntoTarget) {
      const targetEl = currentUnifiedSubtab === 'vocab' ? vocabEl
        : currentUnifiedSubtab === 'mistakes' ? mistakesEl
        : roadmapEl;
      if (targetEl) {
        setTimeout(() => targetEl.scrollIntoView({ behavior: 'smooth', block: 'start' }), 60);
      }
    }
  }

  document.querySelectorAll('.unified-subtab-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      const st = btn.dataset.subtab || 'all';
      switchUnifiedLessonsSubtab(st, false);
      if (st === 'vocab' || st === 'all') loadWords();
      if (st === 'roadmap' || st === 'all') loadLessons();
    });
  });

  const btnStageJumpVocab = document.getElementById('btn-stage-jump-vocab');
  if (btnStageJumpVocab) {
    btnStageJumpVocab.addEventListener('click', () => {
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      const vocabEl = document.getElementById('lessons-vocab-subview');
      if (vocabEl && vocabEl.style.display === 'none') {
        switchUnifiedLessonsSubtab('all', false);
      }
      if (vocabEl) {
        vocabEl.scrollIntoView({ behavior: 'smooth', block: 'start' });
      }
    });
  }

  // Navigation Switch
  function switchView(viewName) {
    const prevView = state.currentView;
    const isMergedLessonsView = (viewName === 'lessons' || viewName === 'words' || viewName === 'mistakes');
    const domSectionId = isMergedLessonsView ? 'view-lessons' : `view-${viewName}`;
    state.currentView = viewName;
    if (typeof Gamification !== 'undefined') Gamification.resetCombo();

    const topbarTitle = document.getElementById('app-topbar-title');
    if (topbarTitle) {
      const titles = {
        quizzes: 'Kiểm tra',
        dashboard: 'Trang chủ',
        ai: 'AI Lab',
        lessons: 'Lộ trình & Từ vựng',
        words: 'Kho Từ vựng',
        mistakes: 'Câu & Từ đã sai',
        profile: 'Hồ sơ',
        admin: 'Quản trị'
      };
      topbarTitle.textContent = titles[viewName] || 'Lộ trình & Từ vựng';
    }

    navBtns.forEach(btn => {
      if (viewName === 'words') {
        btn.classList.toggle('active', btn.dataset.view === 'lessons' || btn.dataset.view === 'words');
      } else {
        btn.classList.toggle('active', btn.dataset.view === viewName);
      }
    });
    let activeSection = null;
    viewSections.forEach(section => {
      const isMatch = section.id === domSectionId;
      section.classList.toggle('active', isMatch);
      if (isMatch) activeSection = section;
    });

    if (viewName !== 'quizzes') {
      document.body.classList.remove('in-exam');
    }
    window.scrollTo({ top: 0, behavior: 'smooth' });

    if (typeof Gamification !== 'undefined' && activeSection) {
      if (prevView !== viewName) Gamification.playSound('whoosh');
      Gamification.triggerEntranceCascade(activeSection);
    }

    if (viewName === 'dashboard') {
      loadDashboard();
      if (typeof MistakeTracker !== 'undefined') MistakeTracker.updateBadges();
    } else if (viewName === 'lessons') {
      switchUnifiedLessonsSubtab('all', false);
      loadLessons();
      loadWords();
      if (typeof MilestoneQuest !== 'undefined') MilestoneQuest.init();
      refreshMascots('idle', 'Lộ trình vượt ải và Kho từ vựng đã được gộp chung! Câu hoặc từ nào làm sai sẽ tự động lưu lại bên dưới nhé! 📖');
    } else if (viewName === 'words') {
      switchUnifiedLessonsSubtab('vocab', false);
      loadWords();
      if (typeof MilestoneQuest !== 'undefined') MilestoneQuest.init();
      refreshMascots('idle', 'Nhấp vào bất kỳ thẻ từ nào để mở bảng câu ví dụ song ngữ & phân tích AI! 📚');
    } else if (viewName === 'mistakes') {
      switchUnifiedLessonsSubtab('mistakes', false);
      if (typeof MistakeTracker !== 'undefined') MistakeTracker.render();
      refreshMascots('encourage', 'Ôn lại các câu hỏi và từ vựng bạn từng làm sai để biến điểm yếu thành điểm mạnh nhé! 💪');
    } else if (viewName === 'quizzes') {
      renderMockExamHub();
      loadQuizSidebarHistory();
      refreshMascots('idle', 'Chọn Đề thi chuẩn quốc tế 3 kỹ năng hoặc tùy chỉnh đề riêng nhé! 📝🎧✍️');
    } else if (viewName === 'ai') {
      renderAiLanguageSamples();
      refreshMascots('idle', 'Thử ngay 3 công cụ AI: Chấm ngữ pháp, Giải thích từ sâu và Nhập vai giao tiếp! 🤖');
    } else if (viewName === 'profile') {
      loadProfile();
      refreshMascots('happy', 'Chào mừng về nhà! Bạn muốn diện nhân vật mới nào hôm nay? ✨');
    } else if (viewName === 'admin') {
      loadAdminPanel();
    }
  }

  document.querySelectorAll('.nav-btn').forEach(btn => {
    btn.addEventListener('click', () => switchView(btn.dataset.view));
  });

  // Delegated [data-goto] & [data-exam-launch] quick-navigation clicks
  document.addEventListener('click', (e) => {
    const examEl = e.target.closest('[data-exam-launch]');
    if (examEl) {
      const fmt = examEl.dataset.examLaunch || 'combined';
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      switchView('quizzes');
      setExamFormatSelection(fmt);
      const qSetup = document.getElementById('quiz-setup-box');
      const qPlay = document.getElementById('quiz-playing-box');
      const qRes = document.getElementById('quiz-result-box');
      if (qSetup && qPlay && qRes) {
        qPlay.style.display = 'none';
        qRes.style.display = 'none';
        qSetup.style.display = 'block';
      }
      setQuizArenaMode(false);
      startQuizSession(examEl);
      return;
    }

    const gotoEl = e.target.closest('[data-goto]');
    if (!gotoEl) return;
    const targetView = gotoEl.dataset.goto;
    if (targetView) {
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      switchView(targetView);
    }
  });

  const ribbonBtnQuickWritten = document.getElementById('ribbon-btn-quick-written');
  const ribbonBtnQuickQuiz = document.getElementById('ribbon-btn-quick-quiz');
  if (ribbonBtnQuickWritten) {
    ribbonBtnQuickWritten.addEventListener('click', () => {
      switchView('quizzes');
      selectedQuizQty = 5;
      selectedQuizMode = 'vocab_random';
      setExamFormatSelection('written');
      document.querySelectorAll('.quiz-qty-btn').forEach(b => b.classList.toggle('active', b.dataset.qty === '5'));
      document.querySelectorAll('.quiz-mode-btn').forEach(b => b.classList.toggle('active', b.dataset.mode === 'vocab_random'));
      startQuizSession(ribbonBtnQuickWritten);
    });
  }
  if (ribbonBtnQuickQuiz) {
    ribbonBtnQuickQuiz.addEventListener('click', () => {
      switchView('quizzes');
      selectedQuizQty = 5;
      selectedQuizMode = 'vocab_random';
      setExamFormatSelection('combined');
      document.querySelectorAll('.quiz-qty-btn').forEach(b => b.classList.toggle('active', b.dataset.qty === '5'));
      document.querySelectorAll('.quiz-mode-btn').forEach(b => b.classList.toggle('active', b.dataset.mode === 'vocab_random'));
      startQuizSession(ribbonBtnQuickQuiz);
    });
  }

  // Language Change
  globalLangSelect.value = state.activeLanguageId;
  updateLanguageRibbon();
  globalLangSelect.addEventListener('change', (e) => {
    state.activeLanguageId = parseInt(e.target.value, 10);
    API.setActiveLanguageId(state.activeLanguageId);
    state.lessonLevelId = '';
    state.wordLevelFilter = '';
    state.wordsPage = 1;
    state.selectedWordId = null;
    applyLanguagePalette(state.activeLanguageId);
    if (typeof Gamification !== 'undefined' && Gamification.updateAmbientLanguage) {
      Gamification.updateAmbientLanguage(state.activeLanguageId);
    }
    updateLanguageRibbon();
    populateLessonLevels();
    renderAiLanguageSamples();
    if (typeof MilestoneQuest !== 'undefined') {
      MilestoneQuest.init();
    }
    const qSetup = document.getElementById('quiz-setup-box');
    const qPlay = document.getElementById('quiz-playing-box');
    const qRes = document.getElementById('quiz-result-box');
    const qNavCard = document.getElementById('quiz-nav-palette-card');
    const qPreCard = document.getElementById('quiz-quick-presets-card');
    if (qSetup && qPlay && qRes) {
      qPlay.style.display = 'none';
      qRes.style.display = 'none';
      qSetup.style.display = 'block';
    }
    if (qNavCard) qNavCard.style.display = 'none';
    if (qPreCard) qPreCard.style.display = 'block';
    if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
    showToast(`Đã chuyển sang học ${state.languages[state.activeLanguageId].name}`, 'info');
    refreshMascots('happy');
    switchView(state.currentView);
  });

  // Auth UI Rendering
  function renderAuthStatus() {
    state.user = API.getUser();
    syncShellUser();
    const navItemAdmin = document.getElementById('nav-item-admin');
    const navItemAdminTop = document.getElementById('nav-btn-admin-top');
    if (state.user && state.user.role === 'Admin') {
      if (navItemAdmin) navItemAdmin.style.display = 'block';
      if (navItemAdminTop) {
        navItemAdminTop.style.display = 'inline-flex';
        navItemAdminTop.onclick = () => switchView('admin');
      }
    } else {
      if (navItemAdmin) navItemAdmin.style.display = 'none';
      if (navItemAdminTop) navItemAdminTop.style.display = 'none';
      if (state.currentView === 'admin') switchView('quizzes');
    }

    if (state.user) {
      const avatarSvg = typeof Gamification !== 'undefined'
        ? Gamification.getAvatarSvg(state.user.avatarKey || 'owl')
        : '👤';
      const userLevel = state.user.level || 1;

      authSection.innerHTML = `
        <div style="display: flex; align-items: center; gap: 8px;">
          <button type="button" class="nav-profile-pill" id="btn-nav-profile-pill" title="Xem trang cá nhân &amp; linh vật">
            <span class="nav-avatar-circle">${avatarSvg}</span>
            <span style="font-size: 0.84rem; font-weight: 700;">${escapeHtml(state.user.displayName)}</span>
            <span class="nav-level-chip">Lv.${userLevel}</span>
          </button>
          <button class="app-icon-btn" id="btn-logout" title="Đăng xuất" style="width: 36px; height: 36px; font-size: 0.95rem;">🚪</button>
        </div>
      `;
      document.getElementById('btn-nav-profile-pill').addEventListener('click', () => {
        switchView('profile');
      });
      document.getElementById('btn-logout').addEventListener('click', () => {
        API.setToken(null);
        API.setUser(null);
        showToast('Đã đăng xuất.', 'info');
        renderAuthStatus();
        if (state.currentView === 'profile') loadProfile();
        else switchView('quizzes');
      });
    } else {
      authSection.innerHTML = `
        <button type="button" class="app-icon-btn bell-pill-btn" id="btn-topbar-account" title="Đăng nhập tài khoản">🔔</button>
        <button class="btn btn-primary" id="btn-open-login" style="padding: 6px 14px; font-size: 0.85rem;">Đăng nhập</button>
      `;
      document.getElementById('btn-open-login').addEventListener('click', openAuthModal);
      const bell = document.getElementById('btn-topbar-account');
      if (bell) bell.addEventListener('click', openAuthModal);
    }
  }

  function openAuthModal() {
    authModal.classList.add('active');
  }

  function closeAuthModal() {
    authModal.classList.remove('active');
  }

  if (btnOpenLogin) btnOpenLogin.addEventListener('click', openAuthModal);
  btnCloseModal.addEventListener('click', closeAuthModal);
  authModal.addEventListener('click', (e) => {
    if (e.target === authModal) closeAuthModal();
  });

  tabLogin.addEventListener('click', () => {
    authMode = 'login';
    tabLogin.classList.add('active');
    tabRegister.classList.remove('active');
    authModalTitle.textContent = 'Đăng Nhập';
    groupDisplayName.style.display = 'none';
    btnSubmitAuth.textContent = 'Đăng nhập';
  });

  tabRegister.addEventListener('click', () => {
    authMode = 'register';
    tabRegister.classList.add('active');
    tabLogin.classList.remove('active');
    authModalTitle.textContent = 'Tạo Tài Khoản';
    groupDisplayName.style.display = 'block';
    btnSubmitAuth.textContent = 'Đăng ký';
  });

  authForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const email = document.getElementById('auth-email').value.trim();
    const password = document.getElementById('auth-password').value;
    const displayName = document.getElementById('auth-displayname').value.trim();

    try {
      let res;
      if (authMode === 'login') {
        res = await API.login(email, password);
      } else {
        res = await API.register(email, password, displayName, state.activeLanguageId);
      }

      if (!res || !res.user) {
        throw new Error('Tài khoản chưa tồn tại hoặc sai thông tin. Nếu bạn chưa từng tạo tài khoản, hãy bấm vào tab "Đăng ký" bên cạnh để tạo nhé!');
      }

      const userName = res.user?.displayName || 'bạn';
      showToast(authMode === 'login' ? `Đăng nhập thành công! Chào ${userName}` : `Đăng ký thành công! Tặng bạn +10 XP khởi đầu 🎉`, 'success');
      API.setToken(res.token);
      API.setUser(res.user);
      closeAuthModal();
      renderAuthStatus();
      if (typeof Gamification !== 'undefined') {
        Gamification.playSound('fanfare');
        Gamification.launchConfetti(45);
      }
      refreshMascots('party', `Chào mừng ${userName}! Cùng chinh phục mục tiêu hôm nay nhé! 🚀`);
      switchView(state.currentView);
    } catch (err) {
      showToast(err.message || 'Đã có lỗi xảy ra khi xác thực.', 'error');
    }
  });

  btnQuickAdmin.addEventListener('click', async () => {
    try {
      const res = await API.login('admin@polylearn.ai', 'Admin@123456');
      if (!res || !res.user) {
        throw new Error('Không thể đăng nhập bằng tài khoản Quản trị.');
      }
      API.setToken(res.token);
      API.setUser(res.user);
      showToast('Đăng nhập với tài khoản Admin thành công!', 'success');
      closeAuthModal();
      renderAuthStatus();
      if (typeof Gamification !== 'undefined') Gamification.playSound('correct');
      const adminName = res.user?.displayName || 'Quản trị viên';
      refreshMascots('happy', `Chào Quản trị viên ${adminName}! ✨`);
      switchView(state.currentView);
    } catch (err) {
      showToast(err.message || 'Lỗi đăng nhập Admin.', 'error');
    }
  });

  // ---------- 1. DASHBOARD (BENTO COMMAND CENTER) ----------
  const btnSpotlightRefresh = document.getElementById('btn-spotlight-refresh');
  const btnSpotlightSpeak = document.getElementById('btn-spotlight-speak');

  if (btnSpotlightRefresh) {
    btnSpotlightRefresh.addEventListener('click', () => {
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      loadWordSpotlight();
    });
  }

  if (btnSpotlightSpeak) {
    btnSpotlightSpeak.addEventListener('click', () => {
      if (state.spotlightWord) speak(state.spotlightWord.term);
    });
  }

  async function loadWordSpotlight() {
    const spotlightEl = document.getElementById('dash-word-spotlight');
    const bankSubEl = document.getElementById('stat-bank-total-sub');
    if (!spotlightEl) return;

    try {
      const paged = await API.getWords({
        languageId: state.activeLanguageId,
        sort: 'random',
        pageSize: 1
      });
      if (bankSubEl && typeof paged.totalCount === 'number') {
        bankSubEl.textContent = `Kho vĩnh viễn: ${paged.totalCount} từ`;
      }

      const first = paged.items && paged.items[0];
      if (!first) {
        spotlightEl.innerHTML = `<div style="color: var(--text-muted); font-size: 0.9rem;">Chưa có từ vựng nào trong kho. Hãy sang mục Kho từ vựng để nạp ngay!</div>`;
        return;
      }

      const detail = await API.getWordDetail(first.id).catch(() => first);
      state.spotlightWord = detail;
      const ex = detail.examples && detail.examples[0];

      spotlightEl.innerHTML = `
        <div style="display: flex; align-items: center; gap: 8px; flex-wrap: wrap; margin-bottom: 8px;">
          <span class="card-tag">${escapeHtml(detail.levelCode || 'A1')}</span>
          ${detail.partOfSpeech ? `<span class="mini-stat-pill">${escapeHtml(detail.partOfSpeech)}</span>` : ''}
          ${detail.topicName ? `<span class="mini-stat-pill">🏷️ ${escapeHtml(detail.topicName)}</span>` : ''}
        </div>
        <div style="font-family: 'Outfit'; font-size: 2.2rem; font-weight: 800; color: var(--text-primary); line-height: 1.15;">
          ${escapeHtml(detail.term)}
        </div>
        <div style="color: var(--reading-color); font-size: 1.05rem; font-weight: 600; margin: 4px 0 8px;">
          ${escapeHtml(detail.reading || '')}
        </div>
        <div style="color: var(--meaning-color); font-size: 1.2rem; font-weight: 800; margin-bottom: 12px;">
          ${escapeHtml(detail.meaning)}
        </div>
        ${ex ? `
          <div style="padding: 10px 14px; border-radius: var(--radius-sm); background: var(--bg-surface); border: 1px solid var(--border-subtle); margin-bottom: 14px;">
            <div style="font-size: 0.9rem; font-weight: 600; color: var(--text-primary);">"${escapeHtml(ex.sentence)}"</div>
            <div style="font-size: 0.82rem; color: var(--text-secondary); margin-top: 2px;">${escapeHtml(ex.translation || '')}</div>
          </div>
        ` : ''}
        <div style="display: flex; gap: 8px; flex-wrap: wrap;">
          <button type="button" class="btn btn-primary" id="btn-spotlight-practice" style="padding: 8px 14px; font-size: 0.83rem;">✅ Đánh dấu Đã thuộc (+XP)</button>
          <button type="button" class="btn btn-secondary" id="btn-spotlight-open-dict" style="padding: 8px 14px; font-size: 0.83rem;">📚 Xem trong Kho từ</button>
        </div>
      `;

      const btnPrac = document.getElementById('btn-spotlight-practice');
      const btnDict = document.getElementById('btn-spotlight-open-dict');
      if (btnPrac) {
        btnPrac.addEventListener('click', async () => {
          if (!API.getToken()) {
            showToast('Vui lòng đăng nhập để lưu tiến độ từ vựng này!', 'info');
            openAuthModal();
            return;
          }
          try {
            const res = await API.reviewCard(detail.id, true);
            showToast(`✨ Đã ghi nhận học từ "${detail.term}" (+${res.xpEarned || 10} XP)!`, 'success');
            if (typeof Gamification !== 'undefined') Gamification.playSound('correct');
            handleXpGain(res, btnPrac);
            loadDashboard();
          } catch (err) {
            showToast(err.message, 'error');
          }
        });
      }
      if (btnDict) {
        btnDict.addEventListener('click', () => {
          state.selectedWordId = detail.id;
          switchView('words');
        });
      }
    } catch (e) {
      console.warn('Spotlight word error:', e);
    }
  }

  function renderDailyQuests({ todayXp = 0, goalXp = 50, trackedWords = 0, completedLessons = 0, totalLessons = 0, passedQuizzes = 0 } = {}) {
    const questsEl = document.getElementById('dash-daily-quests');
    const badgeEl = document.getElementById('dash-quests-badge');
    if (!questsEl) return;

    const quests = [
      {
        icon: '⚡',
        title: `Đạt mục tiêu ${goalXp} XP trong ngày`,
        progressText: `${Math.min(todayXp, goalXp)} / ${goalXp} XP`,
        pct: Math.min(100, Math.round((todayXp / Math.max(10, goalXp)) * 100)),
        done: todayXp >= goalXp,
        goto: 'quizzes',
        btnText: 'Kiếm XP'
      },
      {
        icon: '📚',
        title: 'Tích lũy ít nhất 5 từ vựng đã học qua Kiểm tra / Kho từ',
        progressText: `${trackedWords} từ đã ghi nhận`,
        pct: trackedWords >= 5 ? 100 : Math.min(90, trackedWords * 20),
        done: trackedWords >= 5,
        goto: 'words',
        btnText: 'Kho từ'
      },
      {
        icon: '📖',
        title: 'Hoàn thành bài học theo lộ trình (+25 XP)',
        progressText: `${completedLessons} / ${Math.max(1, totalLessons)} bài`,
        pct: totalLessons > 0 ? Math.min(100, Math.round((completedLessons / totalLessons) * 100)) : 0,
        done: completedLessons > 0,
        goto: 'lessons',
        btnText: 'Học bài'
      },
      {
        icon: '📝',
        title: 'Vượt qua bài Kiểm tra Trắc nghiệm & Tự luận (≥ 70%)',
        progressText: passedQuizzes > 0 ? `Đã đạt ${passedQuizzes} đề` : 'Chưa đạt đề nào',
        pct: passedQuizzes > 0 ? 100 : 0,
        done: passedQuizzes > 0,
        goto: 'quizzes',
        btnText: 'Kiểm tra'
      }
    ];

    const doneCount = quests.filter(q => q.done).length;
    if (badgeEl) badgeEl.textContent = `${doneCount}/${quests.length} Hoàn thành`;

    questsEl.innerHTML = quests.map(q => `
      <div class="quest-item ${q.done ? 'quest-done' : ''}">
        <div class="quest-icon">${q.done ? '✅' : q.icon}</div>
        <div style="flex: 1; min-width: 0;">
          <div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; margin-bottom: 4px;">
            <span style="font-weight: 700; font-size: 0.88rem; color: var(--text-primary);">${escapeHtml(q.title)}</span>
            <span style="font-size: 0.76rem; font-weight: 700; color: ${q.done ? 'var(--success-text)' : 'var(--text-muted)'}; white-space: nowrap;">${escapeHtml(q.progressText)}</span>
          </div>
          <div class="progress-bar-track" style="height: 6px;">
            <div class="progress-bar-fill" style="width: ${q.pct}%; ${q.done ? 'background: var(--success);' : ''}"></div>
          </div>
        </div>
        <button type="button" class="btn ${q.done ? 'btn-secondary' : 'btn-primary'}" data-goto="${q.goto}" style="padding: 6px 12px; font-size: 0.78rem; white-space: nowrap;">
          ${q.done ? 'Luyện thêm' : q.btnText}
        </button>
      </div>
    `).join('');
  }

  function renderDashActivityAndQuizzes(activeDates = [], recentAttempts = []) {
    renderSideWeek(activeDates);
    const streakRowEl = document.getElementById('dash-mini-streak');
    const recentQuizEl = document.getElementById('dash-recent-quizzes');

    if (streakRowEl) {
      const activeSet = new Set(activeDates || []);
      const cells = [];
      const now = new Date();
      for (let i = 13; i >= 0; i--) {
        const d = new Date(Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate() - i));
        const iso = d.toISOString().slice(0, 10);
        const dayNum = String(d.getUTCDate()).padStart(2, '0');
        const isActive = activeSet.has(iso);
        cells.push(`
          <div class="mini-streak-cell ${isActive ? 'active' : ''}" title="Ngày ${iso}">
            <div>${isActive ? '🔥' : '·'}</div>
            <div>${dayNum}</div>
          </div>
        `);
      }
      streakRowEl.innerHTML = cells.join('');
    }

    if (recentQuizEl) {
      if (!recentAttempts || recentAttempts.length === 0) {
        recentQuizEl.innerHTML = `
          <div class="compact-history-item" style="justify-content: center; color: var(--text-muted); padding: 14px;">
            Chưa có bài kiểm tra nào ở ngôn ngữ này. Bấm "Làm đề mới" để thử sức ngay!
          </div>
        `;
      } else {
        recentQuizEl.innerHTML = recentAttempts.slice(0, 4).map(a => {
          const pct = a.totalQuestions > 0 ? Math.round((a.score / a.totalQuestions) * 100) : 0;
          const passed = pct >= 70;
          const dateStr = a.completedAt ? new Date(a.completedAt).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit' }) : '';
          return `
            <div class="compact-history-item">
              <div>
                <div style="font-weight: 700; color: var(--text-primary);">Đề ${escapeHtml((a.languageCode || '').toUpperCase())} ${a.levelCode ? `• ${escapeHtml(a.levelCode)}` : '• Tổng hợp'}</div>
                <div style="font-size: 0.76rem; color: var(--text-muted);">Hoàn thành ${dateStr}</div>
              </div>
              <div style="text-align: right;">
                <span class="card-tag" style="background: ${passed ? 'rgba(16,185,129,0.14)' : 'rgba(239,68,68,0.12)'}; color: ${passed ? 'var(--success-text)' : 'var(--danger)'}; border-color: transparent;">
                  ${a.score}/${a.totalQuestions} (${pct}%)
                </span>
              </div>
            </div>
          `;
        }).join('');
      }
    }
  }

  async function loadDashboard() {
    refreshMascots('idle');
    updateLanguageRibbon();
    loadWordSpotlight();

    const setTxt = (id, val) => {
      const el = document.getElementById(id);
      if (el) el.textContent = val;
    };

    if (!API.getToken()) {
      setTxt('dash-greeting', 'Chào mừng bạn đến với PolyLearn AI!');
      setTxt('dash-streak-count', '0');
      setTxt('stat-words-tracked', '0');
      setTxt('stat-words-mastered', '0');
      setTxt('stat-exams-done', '0');
      setTxt('stat-quizzes-avg', '0%');
      setTxt('stat-lessons-done', '0');
      setTxt('stat-words-favs', '0');
      setTxt('dash-level-badge', '⭐ Cấp 1');
      setTxt('dash-rank-title', '🌱 Tân Binh Ngôn Ngữ');
      setTxt('dash-total-xp', '0 XP');
      setTxt('dash-today-xp', '0');
      setTxt('dash-goal-xp', '50');
      const gFill = document.getElementById('dash-goal-fill');
      if (gFill) gFill.style.width = '0%';
      setTxt('dash-goal-status', '🎯 Đăng nhập để lưu XP & kết quả kiểm tra');
      syncRibbonMetrics({ streak: 0, todayXp: 0, goalXp: 50 });
      renderDailyQuests({});
      renderDashActivityAndQuizzes([], []);
      renderDashVocabMasteryBar({});
      return;
    }

    try {
      const summary = await API.getDashboardSummary(state.activeLanguageId);
      const tracked = summary.vocab.totalWordsTracked || 0;
      const mastered = summary.vocab.masteredWords || 0;
      const learning = summary.vocab.learningWords || 0;
      const favs = summary.vocab.favoriteWords || 0;
      const lessonsCompleted = summary.lessons?.completedLessons || 0;
      const lessonsTotal = summary.lessons?.totalAvailableLessons || 0;
      const quizPassed = summary.quizzes?.passedAttempts || 0;
      const quizTotal = summary.quizzes?.totalAttempts || 0;

      setTxt('dash-greeting', `Chào ${summary.displayName}!`);
      setTxt('dash-streak-count', summary.streak.currentStreakDays);
      setTxt('stat-words-tracked', tracked);
      setTxt('stat-words-mastered', mastered);
      setTxt('stat-exams-done', quizTotal);
      setTxt('stat-exams-sub', quizTotal > 0 ? `Đạt chuẩn: ${quizPassed} đề` : 'Làm đề mới →');
      setTxt('stat-quizzes-avg', `${summary.quizzes.averagePercentage}%`);

      setTxt('stat-words-learning-sub', `Đang rèn luyện: ${learning} từ`);
      setTxt('stat-mastery-rate-sub', `Tỉ lệ thuộc: ${tracked > 0 ? Math.round((mastered / tracked) * 100) : 0}%`);
      setTxt('stat-lessons-done', lessonsCompleted);
      setTxt('stat-lessons-total-sub', `Lộ trình: ${lessonsCompleted}/${lessonsTotal} bài`);
      setTxt('stat-quizzes-passed-sub', `Đã đạt: ${quizPassed}/${quizTotal} đề`);
      setTxt('stat-words-favs', favs);
      const sideWordsBadge = document.getElementById('side-words-badge');
      if (sideWordsBadge) {
        sideWordsBadge.textContent = tracked;
        sideWordsBadge.style.display = tracked > 0 ? 'inline-flex' : 'none';
      }

      let todayXpVal = 0;
      let goalXpVal = 50;

      if (summary.gamification) {
        const g = summary.gamification;
        goalXpVal = Math.max(10, g.dailyGoalXp || 50);
        todayXpVal = g.todayXp || 0;
        const goalPct = Math.min(100, Math.round((todayXpVal / goalXpVal) * 100));

        document.getElementById('dash-level-badge').textContent = `⭐ Cấp ${g.level || 1}`;
        document.getElementById('dash-rank-title').textContent = g.rankTitle || '🌱 Tân Binh Ngôn Ngữ';
        document.getElementById('dash-total-xp').textContent = `${g.experiencePoints || 0} XP`;
        document.getElementById('dash-today-xp').textContent = todayXpVal;
        document.getElementById('dash-goal-xp').textContent = goalXpVal;
        document.getElementById('dash-goal-fill').style.width = `${goalPct}%`;
        document.getElementById('dash-goal-status').textContent =
          todayXpVal >= goalXpVal ? '🎉 Đã hoàn thành mục tiêu hôm nay!' : '🎯 Mục tiêu hôm nay';

        if (state.user) {
          state.user = {
            ...state.user,
            avatarKey: g.avatarKey || state.user.avatarKey,
            companionKey: g.companionKey || state.user.companionKey,
            bio: g.bio ?? state.user.bio,
            dailyGoalXp: goalXpVal,
            experiencePoints: g.experiencePoints || 0,
            todayXp: todayXpVal,
            level: g.level || 1,
            rankTitle: g.rankTitle || state.user.rankTitle
          };
          API.setUser(state.user);
          renderAuthStatus();
          refreshMascots('idle');
        }
      }

      syncRibbonMetrics({
        streak: summary.streak.currentStreakDays || 0,
        todayXp: todayXpVal,
        goalXp: goalXpVal
      });

      renderDailyQuests({
        todayXp: todayXpVal,
        goalXp: goalXpVal,
        trackedWords: tracked,
        completedLessons: lessonsCompleted,
        totalLessons: lessonsTotal,
        passedQuizzes: quizPassed
      });

      renderDashActivityAndQuizzes(
        summary.streak?.activeDatesInLastTwoWeeks || [],
        summary.quizzes?.recentAttempts || []
      );

      renderDashVocabMasteryBar({
        tracked,
        mastered,
        learning,
        avgScore: summary.quizzes?.averagePercentage || 0
      });
    } catch (err) {
      console.warn('Dashboard summary error:', err);
    }
  }

  function renderDashVocabMasteryBar({ tracked = 0, mastered = 0, learning = 0, avgScore = 0 } = {}) {
    const barBox = document.getElementById('dash-vocab-mastery-bar');
    if (!barBox) return;
    const masteryPct = tracked > 0 ? Math.min(100, Math.round((mastered / tracked) * 100)) : 0;
    barBox.innerHTML = `
      <div style="display: flex; justify-content: space-between; align-items: center; gap: 10px; flex-wrap: wrap; font-size: 0.82rem; margin-bottom: 6px;">
        <span style="font-weight: 700; color: var(--text-primary);">📈 Độ vững từ vựng qua Kiểm tra: <strong>${tracked}</strong> từ đã luyện (${learning} đang rèn • ${mastered} thành thạo)</span>
        <span style="font-weight: 800; color: var(--accent-primary);">Điểm TB: ${avgScore}%</span>
      </div>
      <div class="progress-bar-track" style="height: 7px;">
        <div class="progress-bar-fill" style="width: ${Math.max(tracked > 0 ? 12 : 0, masteryPct)}%;"></div>
      </div>
    `;
  }

  // ---------- 2. LESSONS (BÀI HỌC THEO LỘ TRÌNH) ----------
  const lessonsGrid = document.getElementById('lessons-grid');
  const lessonTypeTabs = document.querySelectorAll('.lesson-filter-btn');
  const lessonSearchInput = document.getElementById('lesson-search-input');
  const lessonLevelSelect = document.getElementById('lesson-level-select');
  const lessonStudyModal = document.getElementById('lesson-study-modal');
  const btnCloseLessonModal = document.getElementById('btn-close-lesson-modal');
  const btnStudyBack = document.getElementById('btn-study-back');
  const btnStudyComplete = document.getElementById('btn-study-complete');
  const btnStudySpeak = document.getElementById('btn-study-speak');
  const btnStudyAiExplain = document.getElementById('btn-study-ai-explain');
  const btnCloseAiExplain = document.getElementById('btn-close-ai-explain');
  const studyAiExplanationBox = document.getElementById('study-ai-explanation-box');
  const studyAiExplanationContent = document.getElementById('study-ai-explanation-content');

  // Filter listeners
  lessonTypeTabs.forEach(btn => {
    btn.addEventListener('click', () => {
      lessonTypeTabs.forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      state.lessonFilterType = btn.dataset.type;
      loadLessons();
    });
  });

  lessonSearchInput.addEventListener('input', debounce(() => {
    state.lessonSearch = lessonSearchInput.value.trim();
    loadLessons();
  }, 350));

  lessonLevelSelect.addEventListener('change', () => {
    state.lessonLevelId = lessonLevelSelect.value;
    loadLessons();
  });

  async function populateLessonLevels() {
    try {
      const lang = await API.getLanguage(state.activeLanguageId);
      const quizLvlSelect = document.getElementById('quiz-level-select');
      const wordLvlSelect = document.getElementById('word-level-filter');
      state.cachedLevels = (lang && lang.levels) ? lang.levels : [];

      if (lessonLevelSelect) lessonLevelSelect.innerHTML = '<option value="">Tất cả cấp độ</option>';
      if (quizLvlSelect) quizLvlSelect.innerHTML = '<option value="">Tất cả cấp độ</option>';
      if (wordLvlSelect) wordLvlSelect.innerHTML = '<option value="">Tất cả cấp độ</option>';

      state.cachedLevels.forEach(lvl => {
        const label = `${lvl.code} - ${lvl.name}`;
        if (lessonLevelSelect) {
          const opt = document.createElement('option');
          opt.value = lvl.id;
          opt.textContent = label;
          lessonLevelSelect.appendChild(opt);
        }
        if (quizLvlSelect) {
          const qOpt = document.createElement('option');
          qOpt.value = lvl.id;
          qOpt.textContent = label;
          quizLvlSelect.appendChild(qOpt);
        }
        if (wordLvlSelect) {
          const wOpt = document.createElement('option');
          wOpt.value = lvl.id;
          wOpt.textContent = label;
          wordLvlSelect.appendChild(wOpt);
        }
      });
    } catch (e) {
      console.warn('Cannot populate lesson levels:', e);
    }
  }

  function renderLessonRoadmapAndSpotlight(allLessons = []) {
    const stagePillsEl = document.getElementById('lesson-stage-pills');
    const btnStagePrev = document.getElementById('btn-stage-prev');
    const btnStageNext = document.getElementById('btn-stage-next');
    const levels = state.cachedLevels || [];

    // If no level selected yet, default to first uncompleted level, or first level
    if (!state.lessonLevelId && levels.length > 0) {
      const uncompletedLvl = levels.find(lvl => {
        const lvlLessons = allLessons.filter(l => l.levelId === lvl.id || l.levelCode === lvl.code);
        return lvlLessons.some(l => !l.isCompleted);
      });
      state.lessonLevelId = (uncompletedLvl || levels[0]).id;
      if (lessonLevelSelect) lessonLevelSelect.value = state.lessonLevelId;
    }

    // Render Stage pills (Ải 1, Ải 2, ...)
    if (stagePillsEl && levels.length > 0) {
      stagePillsEl.innerHTML = levels.map((lvl, idx) => {
        const isActive = String(state.lessonLevelId) === String(lvl.id);
        const lvlLessons = allLessons.filter(l => l.levelId === lvl.id || l.levelCode === lvl.code);
        const lvlDone = lvlLessons.filter(l => l.isCompleted).length;
        const isCleared = lvlLessons.length > 0 && lvlDone === lvlLessons.length;
        return `
          <button type="button" class="stage-pill ${isActive ? 'active' : ''} ${isCleared ? 'cleared' : ''}" data-lvl="${lvl.id}" title="${escapeHtml(lvl.code)} - ${escapeHtml(lvl.name)}">
            ${isCleared ? '✓ ' : ''}Ải ${idx + 1}
          </button>
        `;
      }).join('');

      stagePillsEl.querySelectorAll('.stage-pill').forEach(btn => {
        btn.addEventListener('click', () => {
          state.lessonLevelId = btn.dataset.lvl;
          if (lessonLevelSelect) lessonLevelSelect.value = state.lessonLevelId;
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          loadLessons();
        });
      });
    }

    // Cycle stage buttons
    if (btnStagePrev && levels.length > 0) {
      btnStagePrev.onclick = () => {
        const currIdx = levels.findIndex(l => String(l.id) === String(state.lessonLevelId));
        if (currIdx > 0) {
          state.lessonLevelId = levels[currIdx - 1].id;
          if (lessonLevelSelect) lessonLevelSelect.value = state.lessonLevelId;
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          loadLessons();
        }
      };
      const currIdx = levels.findIndex(l => String(l.id) === String(state.lessonLevelId));
      btnStagePrev.disabled = currIdx <= 0;
      btnStagePrev.style.opacity = currIdx <= 0 ? '0.5' : '1';
    }
    if (btnStageNext && levels.length > 0) {
      btnStageNext.onclick = () => {
        const currIdx = levels.findIndex(l => String(l.id) === String(state.lessonLevelId));
        if (currIdx < levels.length - 1 && currIdx >= 0) {
          state.lessonLevelId = levels[currIdx + 1].id;
          if (lessonLevelSelect) lessonLevelSelect.value = state.lessonLevelId;
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          loadLessons();
        }
      };
      const currIdx = levels.findIndex(l => String(l.id) === String(state.lessonLevelId));
      btnStageNext.disabled = currIdx >= levels.length - 1;
      btnStageNext.style.opacity = (currIdx >= levels.length - 1) ? '0.5' : '1';
    }

    // Active level calculations
    const activeLvlIdx = levels.findIndex(l => String(l.id) === String(state.lessonLevelId));
    const activeLvl = (activeLvlIdx >= 0) ? levels[activeLvlIdx] : (levels[0] || null);
    const lvlLessons = activeLvl ? allLessons.filter(l => l.levelId === activeLvl.id || l.levelCode === activeLvl.code) : allLessons;
    const lvlDone = lvlLessons.filter(l => l.isCompleted).length;
    const lvlTotal = lvlLessons.length;
    const lvlPct = lvlTotal > 0 ? Math.round((lvlDone / lvlTotal) * 100) : 0;
    const nextLessonInStage = lvlLessons.find(l => !l.isCompleted) || lvlLessons[0];
    const allStageDone = lvlTotal > 0 && lvlDone === lvlTotal;

    // Update Big Stage Hero Banner
    const stageHeroBadge = document.getElementById('stage-hero-badge');
    if (stageHeroBadge) {
      const stageNum = (activeLvlIdx >= 0 ? activeLvlIdx + 1 : 1);
      const totalStages = levels.length || 1;
      stageHeroBadge.textContent = allStageDone
        ? `⚔️ CỬA ẢI ${stageNum} / ${totalStages} • ĐÃ VƯỢT ẢI HOÀN TẤT`
        : `⚔️ CỬA ẢI ${stageNum} / ${totalStages} • ĐANG CHINH PHỤC CỬA ẢI`;
    }
    const stageHeroLessonsCount = document.getElementById('stage-hero-lessons-count');
    if (stageHeroLessonsCount) {
      stageHeroLessonsCount.textContent = `${lvlDone}/${lvlTotal} Bài học`;
    }
    const stageHeroTitle = document.getElementById('stage-hero-title');
    if (stageHeroTitle) {
      if (nextLessonInStage) {
        const lessonIdx = lvlLessons.indexOf(nextLessonInStage) + 1;
        stageHeroTitle.textContent = `${String(lessonIdx).padStart(2, '0')}. ${nextLessonInStage.title}`;
      } else {
        stageHeroTitle.textContent = activeLvl ? `01. ${activeLvl.name}` : 'Chưa có bài học trong ải';
      }
    }
    const stageHeroFill = document.getElementById('stage-hero-progress-fill');
    if (stageHeroFill) {
      stageHeroFill.style.width = `${lvlPct}%`;
    }
    const stageHeroText = document.getElementById('stage-hero-progress-text');
    if (stageHeroText) {
      stageHeroText.textContent = `Đã hoàn thành ${lvlDone} / ${lvlTotal} bài học (${lvlPct}%)`;
    }
    const stageHeroSpeech = document.getElementById('stage-hero-mascot-speech');
    if (stageHeroSpeech) {
      stageHeroSpeech.textContent = allStageDone
        ? 'Xuất sắc! Bạn đã vượt toàn bộ bài của ải này! Tiếp tục ải sau nhé! 🎉'
        : 'Cùng chinh phục cửa ải này nhé! 🚀';
    }
    const stageHeroMascot = document.getElementById('stage-hero-mascot-figure');
    if (stageHeroMascot && typeof Gamification !== 'undefined') {
      stageHeroMascot.innerHTML = Gamification.getAvatarSvg('ghost');
    }
    const lessonPageTitle = document.getElementById('lesson-page-title');
    if (lessonPageTitle) {
      const curLang = state.languages[state.activeLanguageId];
      lessonPageTitle.textContent = curLang ? `${curLang.name} - Chinh Phục Cửa Ải` : 'Lộ Trình Vượt Ải';
    }
    const btnLessonChangeLang = document.getElementById('btn-lesson-change-lang');
    if (btnLessonChangeLang) {
      btnLessonChangeLang.onclick = () => {
        if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
        // Cycle to next language
        const langIds = [1, 2, 3, 4];
        const nextId = langIds[(langIds.indexOf(state.activeLanguageId) + 1) % langIds.length];
        globalLangSelect.value = nextId;
        globalLangSelect.dispatchEvent(new Event('change'));
      };
    }
    const btnStageHeroAction = document.getElementById('btn-stage-hero-action');
    if (btnStageHeroAction) {
      if (nextLessonInStage) {
        btnStageHeroAction.textContent = allStageDone
          ? '↺ Ôn lại bài đầu tiên'
          : '⚔️ Bắt đầu bài học (+25 XP)';
        btnStageHeroAction.onclick = () => {
          openLessonStudy(nextLessonInStage.id);
        };
      }
    }
    const btnContinueQuest = document.getElementById('btn-lesson-continue-quest');
    if (btnContinueQuest && nextLessonInStage) {
      btnContinueQuest.onclick = () => {
        openLessonStudy(nextLessonInStage.id);
      };
    }

    const stationsHeading = document.getElementById('stage-stations-heading');
    if (stationsHeading) {
      stationsHeading.textContent = activeLvl
        ? `Cửa Ải: ${activeLvl.code} - ${activeLvl.name} (${lvlDone}/${lvlTotal} bài xong)`
        : `Tất cả bài học (${allLessons.filter(l => l.isCompleted).length}/${allLessons.length})`;
    }

    // Compatibility backward sync
    const setSafeTxt = (id, val) => { const el = document.getElementById(id); if (el) el.textContent = val; };
    setSafeTxt('lesson-progress-percent', `${lvlPct}%`);
    setSafeTxt('lesson-progress-text', `Đã hoàn thành ${lvlDone}/${lvlTotal} bài học`);
    const oldFill = document.getElementById('lesson-progress-fill');
    if (oldFill) oldFill.style.width = `${lvlPct}%`;
  }

  async function loadLessons() {
    lessonsGrid.innerHTML = '<div style="color: var(--text-muted); grid-column: 1/-1; text-align: center; padding: 40px;">⏳ Đang tải các mốc ải bài học...</div>';

    try {
      if (!state.cachedLevels || state.cachedLevels.length === 0) {
        await populateLessonLevels();
      }
      const allPaged = await API.getLessons({ languageId: state.activeLanguageId, pageSize: 100 });
      const allLessons = allPaged.items || [];
      renderLessonRoadmapAndSpotlight(allLessons);

      const params = {
        languageId: state.activeLanguageId,
        pageSize: 50
      };
      if (state.lessonFilterType !== 'all') {
        params.type = parseInt(state.lessonFilterType, 10);
      }
      if (state.lessonSearch) {
        params.search = state.lessonSearch;
      }
      if (state.lessonLevelId) {
        params.levelId = parseInt(state.lessonLevelId, 10);
      }

      const paged = await API.getLessons(params);
      const items = paged.items || [];

      if (items.length === 0) {
        lessonsGrid.innerHTML = `
          <div style="grid-column: 1/-1; text-align: center; padding: 50px 20px;" class="glass-card">
            <div style="font-size: 2.8rem; margin-bottom: 12px;">🗺️</div>
            <h3 style="font-family: 'Outfit'; font-size: 1.25rem; margin-bottom: 6px;">Không tìm thấy bài học nào trong ải này</h3>
            <p style="color: var(--text-secondary); font-size: 0.92rem;">Hãy chuyển sang ải khác trên thanh chọn ải bên trên hoặc chọn thể loại khác.</p>
          </div>
        `;
        return;
      }

      // Identify the next uncompleted lesson to mark as .current
      const nextUncompleted = items.find(l => !l.isCompleted);

      lessonsGrid.innerHTML = '';
      items.forEach((lesson, idx) => {
        const card = document.createElement('div');
        const isCompleted = lesson.isCompleted === true;
        const isCurrent = !isCompleted && nextUncompleted && nextUncompleted.id === lesson.id;

        card.className = `station-card ${isCompleted ? 'cleared' : (isCurrent ? 'current' : '')}`;

        let typeBadgeClass = 'lesson-type-badge-vocab';
        let typeIcon = '🟢';
        let typeText = 'Từ vựng';
        if (lesson.type === 1) {
          typeBadgeClass = 'lesson-type-badge-listen';
          typeIcon = '🟣';
          typeText = 'Luyện nghe';
        } else if (lesson.type === 2) {
          typeBadgeClass = 'lesson-type-badge-read';
          typeIcon = '🔵';
          typeText = 'Đọc hiểu';
        }

        const stationIndexStr = String(idx + 1).padStart(2, '0');

        card.innerHTML = `
          <div class="station-badge">
            ${isCompleted ? '✓' : stationIndexStr}
          </div>
          <div class="station-main">
            <div class="station-meta">
              <span class="station-type ${typeBadgeClass}">${typeIcon} ${typeText}</span>
              <span class="station-xp">${isCompleted ? '✓ Đã nhận +25 XP' : '⚡ +25 XP'}</span>
              <span style="font-size: 0.74rem; color: var(--text-muted);">${lesson.questionCount > 0 ? `📝 ${lesson.questionCount} bài tập` : '📖 Bài chuyên đề'}</span>
            </div>
            <h4 class="station-title">${escapeHtml(lesson.title)}</h4>
          </div>
          <button type="button" class="station-play-btn ${isCompleted ? 'done' : 'play'} btn-open-lesson">
            ${isCompleted ? '↺ Ôn lại' : '▶ Vượt ải'}
          </button>
        `;

        card.style.cursor = 'pointer';
        card.addEventListener('click', () => {
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          openLessonStudy(lesson.id);
        });

        card.querySelector('.btn-open-lesson').addEventListener('click', (e) => {
          e.stopPropagation();
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          openLessonStudy(lesson.id);
        });

        lessonsGrid.appendChild(card);
      });
    } catch (err) {
      lessonsGrid.innerHTML = `<div style="color: var(--danger); grid-column: 1/-1; text-align: center; padding: 40px;">Lỗi khi tải bài học: ${escapeHtml(err.message)}</div>`;
    }
  }

  // Study Modal Handling
  async function openLessonStudy(lessonId) {
    lessonStudyModal.classList.add('active');
    document.getElementById('study-lesson-title').textContent = 'Đang tải bài học...';
    document.getElementById('study-lesson-content').innerHTML = '<div style="color: var(--text-muted); text-align: center; padding: 30px;">⏳ Đang nạp nội dung bài học...</div>';
    document.getElementById('study-questions-section').style.display = 'none';
    studyAiExplanationBox.style.display = 'none';

    try {
      const lesson = await API.getLessonDetail(lessonId);
      state.currentLesson = lesson;

      // Badges
      let typeBadgeClass = 'lesson-type-badge-vocab';
      let typeText = 'Từ vựng';
      if (lesson.type === 1) {
        typeBadgeClass = 'lesson-type-badge-listen';
        typeText = 'Luyện nghe';
      } else if (lesson.type === 2) {
        typeBadgeClass = 'lesson-type-badge-read';
        typeText = 'Đọc hiểu';
      }

      const typeBadge = document.getElementById('study-lesson-type-badge');
      typeBadge.className = `card-tag ${typeBadgeClass}`;
      typeBadge.textContent = typeText;

      const levelBadge = document.getElementById('study-lesson-level-badge');
      levelBadge.textContent = lesson.levelCode || 'A1';

      const statusBadge = document.getElementById('study-lesson-status-badge');
      if (lesson.isCompleted) {
        statusBadge.style.display = 'inline-block';
        btnStudyComplete.textContent = '✓ Đã hoàn thành (Cập nhật lại)';
      } else {
        statusBadge.style.display = 'none';
        btnStudyComplete.textContent = '✅ Đánh dấu hoàn thành bài học';
      }

      document.getElementById('study-lesson-title').textContent = lesson.title;

      // Render Content & Sentences
      const readerBox = document.getElementById('study-lesson-content');
      if (lesson.content) {
        document.getElementById('study-audio-bar').style.display = 'flex';
        const rawSentences = lesson.content.match(/[^.!?。！？\n]+[.!?。！？\n]*/g) || [lesson.content];
        readerBox.innerHTML = '';
        
        rawSentences.forEach(s => {
          const trimmed = s.trim();
          if (!trimmed) return;
          const span = document.createElement('span');
          span.className = 'lesson-sentence-span';
          span.textContent = trimmed + ' ';
          span.title = 'Nhấp để nghe đọc câu này';
          span.addEventListener('click', () => {
            speak(trimmed);
          });
          readerBox.appendChild(span);
        });
      } else {
        document.getElementById('study-audio-bar').style.display = 'none';
        readerBox.innerHTML = `
          <div style="text-align: center; color: var(--text-secondary); padding: 20px;">
            <p style="margin-bottom: 10px; font-size: 1.1rem;">Bài học từ vựng và bài tập củng cố.</p>
            <p style="font-size: 0.95rem; color: var(--text-muted);">Hãy làm các bài tập bên dưới để ghi nhớ và thực hành phản xạ!</p>
          </div>
        `;
      }

      // Render Practice Questions
      const questionsSection = document.getElementById('study-questions-section');
      const questionsList = document.getElementById('study-questions-list');
      if (lesson.questions && lesson.questions.length > 0) {
        questionsSection.style.display = 'block';
        questionsList.innerHTML = '';

        lesson.questions.forEach((q, qIndex) => {
          const qBox = document.createElement('div');
          qBox.className = 'lesson-practice-item';

          qBox.innerHTML = `
            <div style="font-weight: 700; font-size: 1.05rem; margin-bottom: 12px; color: var(--text-primary);">
              Câu ${qIndex + 1}: ${escapeHtml(q.text)}
            </div>
            <div class="lesson-options-container" id="l-opt-container-${q.id}"></div>
            <div class="lesson-q-explanation" id="l-exp-${q.id}" style="display: none; margin-top: 12px; padding: 10px 14px; background: var(--accent-soft); border-left: 3px solid var(--accent-primary); border-radius: 6px; font-size: 0.9rem; color: var(--text-primary);"></div>
          `;

          const optContainer = qBox.querySelector(`#l-opt-container-${q.id}`);
          const expBox = qBox.querySelector(`#l-exp-${q.id}`);

          q.options.forEach(opt => {
            const optBtn = document.createElement('button');
            optBtn.className = 'lesson-option-choice';
            optBtn.textContent = opt.text;

            optBtn.addEventListener('click', () => {
              optContainer.querySelectorAll('.lesson-option-choice').forEach(b => b.disabled = true);

              if (opt.isCorrect) {
                optBtn.classList.add('correct');
                optBtn.textContent = `✓ ${opt.text}`;
                if (typeof Gamification !== 'undefined') {
                  Gamification.playSound('correct');
                  Gamification.recordCombo(true);
                  Gamification.spawnHeartBurst(optBtn);
                }
                refreshMascots('happy');
              } else {
                optBtn.classList.add('wrong');
                optBtn.textContent = `✗ ${opt.text}`;
                const correctOpt = (q.options || []).find(o => o.isCorrect);
                optContainer.querySelectorAll('.lesson-option-choice').forEach((b, idx) => {
                  if (q.options[idx].isCorrect) {
                    b.classList.add('correct');
                    b.textContent = `✓ ${b.textContent}`;
                  }
                });
                if (typeof MistakeTracker !== 'undefined') {
                  MistakeTracker.recordQuestionMistake({
                    id: `lesson_q_${q.id}`,
                    questionText: q.text,
                    wrongAnswer: opt.text,
                    correctAnswer: correctOpt ? correctOpt.text : '',
                    explanation: q.explanation || '',
                    options: (q.options || []).map(o => o.text),
                    source: `Bài học: ${lesson.title || ''}`
                  });
                }
                showToast('⚠️ Đã lưu câu làm sai vào mục "Câu & Từ Đã Sai" để ôn lại!', 'info');
                if (typeof Gamification !== 'undefined') {
                  Gamification.playSound('wrong');
                  Gamification.recordCombo(false);
                }
                refreshMascots('encourage');
              }

              expBox.style.display = 'block';
              expBox.innerHTML = `${q.explanation ? `<div><strong>💡 Giải thích:</strong> ${escapeHtml(q.explanation)}</div>` : ''}${!opt.isCorrect ? `<div style="margin-top:6px; font-size:0.82rem; font-weight:700; color:#dc2626;">📌 Đã tự động lưu câu này vào Sổ Tay Câu &amp; Từ Đã Sai!</div>` : ''}`;
              if (!q.explanation && opt.isCorrect) {
                expBox.style.display = 'none';
              }
            });

            optContainer.appendChild(optBtn);
          });

          questionsList.appendChild(qBox);
        });
      } else {
        questionsSection.style.display = 'none';
      }

    } catch (err) {
      showToast(`Lỗi nạp bài học: ${err.message}`, 'error');
      closeLessonStudyModal();
    }
  }

  function closeLessonStudyModal() {
    lessonStudyModal.classList.remove('active');
    state.currentLesson = null;
    window.speechSynthesis?.cancel();
  }

  btnCloseLessonModal.addEventListener('click', closeLessonStudyModal);
  btnStudyBack.addEventListener('click', closeLessonStudyModal);

  // Audio Play All
  btnStudySpeak.addEventListener('click', () => {
    if (state.currentLesson && state.currentLesson.content) {
      speak(state.currentLesson.content);
    }
  });

  // AI Explain
  btnStudyAiExplain.addEventListener('click', async () => {
    if (!state.currentLesson) return;
    const textToExplain = state.currentLesson.content || state.currentLesson.title;

    studyAiExplanationBox.style.display = 'block';
    studyAiExplanationContent.innerHTML = '<em>🤖 AI đang phân tích bài học, từ vựng và cấu trúc ngữ pháp quan trọng...</em>';
    btnStudyAiExplain.disabled = true;

    try {
      const langCode = state.languages[state.activeLanguageId]?.code || 'en';
      const result = await API.checkGrammar(textToExplain, langCode);

      studyAiExplanationContent.innerHTML = `
        <div style="margin-bottom: 8px;"><strong>✓ Phân tích:</strong> ${escapeHtml(result.explanation || 'Bài đọc sử dụng từ vựng và ngữ cảnh chuẩn tự nhiên.')}</div>
        ${result.suggestions && result.suggestions.length > 0 ? `
          <div style="margin-top: 8px;">
            <strong>💡 Cách diễn đạt hoặc từ vựng tương đương:</strong>
            <ul style="padding-left: 20px; margin-top: 4px;">
              ${result.suggestions.map(s => `<li>${escapeHtml(s)}</li>`).join('')}
            </ul>
          </div>
        ` : ''}
      `;
    } catch (err) {
      studyAiExplanationContent.innerHTML = `<span style="color: var(--danger);">Không thể kết nối AI: ${escapeHtml(err.message)}</span>`;
    } finally {
      btnStudyAiExplain.disabled = false;
    }
  });

  btnCloseAiExplain.addEventListener('click', () => {
    studyAiExplanationBox.style.display = 'none';
  });

  // Mark Completed Button
  btnStudyComplete.addEventListener('click', async () => {
    if (!state.currentLesson) return;

    if (!API.getToken()) {
      showToast('Vui lòng đăng nhập để lưu tiến độ hoàn thành bài học!', 'info');
      openAuthModal();
      return;
    }

    btnStudyComplete.disabled = true;
    try {
      const res = await API.completeLesson(state.currentLesson.id);
      showToast(`🎉 Hoàn thành bài học! (+${res.xpEarned || 25} XP)`, 'success');
      state.currentLesson.isCompleted = true;
      document.getElementById('study-lesson-status-badge').style.display = 'inline-block';
      btnStudyComplete.textContent = '✓ Đã hoàn thành (Cập nhật lại)';

      if (typeof Gamification !== 'undefined') {
        Gamification.playSound('fanfare');
        Gamification.launchConfetti(65);
      }
      handleXpGain(res, btnStudyComplete);
      refreshMascots('party', `Xuất sắc! Bạn vừa chinh phục bài học và bỏ túi +${res.xpEarned || 25} XP! 🎉`);

      loadLessons();
      loadDashboard();
    } catch (err) {
      showToast(err.message, 'error');
    } finally {
      btnStudyComplete.disabled = false;
    }
  });

  // ---------- 3. EXAM ARENA (TRẮC NGHIỆM & TỰ LUẬN AI + PERMANENT HARVEST) ----------
  const quizSetupBox = document.getElementById('quiz-setup-box');
  const quizPlayingBox = document.getElementById('quiz-playing-box');
  const quizResultBox = document.getElementById('quiz-result-box');
  const btnStartQuiz = document.getElementById('btn-start-quiz');
  const btnQuitQuiz = document.getElementById('btn-quit-quiz');
  const btnPrevQuestion = document.getElementById('btn-prev-question');
  const btnNextQuestion = document.getElementById('btn-next-question');
  const btnSidebarSubmitQuiz = document.getElementById('btn-sidebar-submit-quiz');
  const btnRestartQuiz = document.getElementById('btn-restart-quiz');
  const btnQuickRematchQuiz = document.getElementById('btn-quick-rematch-quiz');
  const quizHarvestToggle = document.getElementById('quiz-harvest-toggle');
  const quizHarvestOptions = document.getElementById('quiz-harvest-options');
  const quizTopicSelect = document.getElementById('quiz-topic-select');
  const quizNewWordsCount = document.getElementById('quiz-new-words-count');
  const quizCustomTopicWrap = document.getElementById('quiz-custom-topic-wrap');
  const quizCustomTopic = document.getElementById('quiz-custom-topic');
  const quizLevelSelect = document.getElementById('quiz-level-select');
  const quizPlayingHarvestBanner = document.getElementById('quiz-playing-harvest-banner');
  const quizResultHarvestShowcase = document.getElementById('quiz-result-harvest-showcase');
  const quizNavPaletteCard = document.getElementById('quiz-nav-palette-card');
  const quizQuickPresetsCard = document.getElementById('quiz-quick-presets-card');

  const quizQuestionTypeBadge = document.getElementById('quiz-question-type-badge');
  const quizWrittenWorkspace = document.getElementById('quiz-written-workspace');
  const quizWrittenTargetChip = document.getElementById('quiz-written-target-chip');
  const quizWrittenTargetWord = document.getElementById('quiz-written-target-word');
  const btnQuizSpeakTarget = document.getElementById('btn-quiz-speak-target');
  const quizWrittenInput = document.getElementById('quiz-written-input');
  const quizWrittenTextarea = document.getElementById('quiz-written-textarea');
  const btnQuizToggleHint = document.getElementById('btn-quiz-toggle-hint');
  const quizWrittenCounter = document.getElementById('quiz-written-counter');
  const quizWrittenHintBox = document.getElementById('quiz-written-hint-box');
  const quizKbdGuide = document.getElementById('quiz-kbd-guide');
  const quizResultBreakdown = document.getElementById('quiz-result-breakdown');

  // Official Exam Profiles per Language & Level (Level 3 specs match screenshot: 65 min • 40 Nghe • 30 Đọc • 10 Viết)
  const OFFICIAL_EXAM_PROFILES = {
    // 1: English (CEFR)
    1: {
      datasetLabel: 'Oxford v2.0',
      levels: [
        { index: 1, code: 'A1', name: 'CEFR A1', minutes: 35, listening: 20, reading: 20, writing: 0 },
        { index: 2, code: 'A2', name: 'CEFR A2', minutes: 50, listening: 30, reading: 25, writing: 5 },
        { index: 3, code: 'B1', name: 'CEFR B1', minutes: 65, listening: 40, reading: 30, writing: 10 },
        { index: 4, code: 'B2', name: 'CEFR B2', minutes: 80, listening: 40, reading: 35, writing: 15 },
        { index: 5, code: 'C1', name: 'CEFR C1', minutes: 95, listening: 45, reading: 40, writing: 15 }
      ],
      defaultLevelIndex: 3
    },
    // 2: Chinese (HSK) - Matches user screenshot exactly!
    2: {
      datasetLabel: 'Hanban v2.0',
      levels: [
        { index: 1, code: 'HSK 1', name: 'HSK 1', minutes: 35, listening: 20, reading: 20, writing: 0 },
        { index: 2, code: 'HSK 2', name: 'HSK 2', minutes: 50, listening: 35, reading: 25, writing: 0 },
        { index: 3, code: 'HSK 3', name: 'HSK 3', minutes: 65, listening: 40, reading: 30, writing: 10 },
        { index: 4, code: 'HSK 4', name: 'HSK 4', minutes: 100, listening: 45, reading: 40, writing: 15 },
        { index: 5, code: 'HSK 5', name: 'HSK 5', minutes: 120, listening: 45, reading: 45, writing: 10 },
        { index: 6, code: 'HSK 6', name: 'HSK 6', minutes: 135, listening: 50, reading: 50, writing: 1 }
      ],
      defaultLevelIndex: 3
    },
    // 3: Japanese (JLPT)
    3: {
      datasetLabel: 'JLPT v2.0',
      levels: [
        { index: 1, code: 'N5', name: 'JLPT N5', minutes: 40, listening: 20, reading: 20, writing: 0 },
        { index: 2, code: 'N4', name: 'JLPT N4', minutes: 50, listening: 30, reading: 25, writing: 5 },
        { index: 3, code: 'N3', name: 'JLPT N3', minutes: 65, listening: 40, reading: 30, writing: 10 },
        { index: 4, code: 'N2', name: 'JLPT N2', minutes: 90, listening: 40, reading: 35, writing: 10 },
        { index: 5, code: 'N1', name: 'JLPT N1', minutes: 110, listening: 45, reading: 40, writing: 10 }
      ],
      defaultLevelIndex: 3
    },
    // 4: Korean (TOPIK)
    4: {
      datasetLabel: 'TOPIK v2.0',
      levels: [
        { index: 1, code: 'TOPIK 1', name: 'TOPIK 1', minutes: 40, listening: 25, reading: 25, writing: 0 },
        { index: 2, code: 'TOPIK 2', name: 'TOPIK 2', minutes: 55, listening: 30, reading: 30, writing: 0 },
        { index: 3, code: 'TOPIK 3', name: 'TOPIK 3', minutes: 65, listening: 40, reading: 30, writing: 10 },
        { index: 4, code: 'TOPIK 4', name: 'TOPIK 4', minutes: 85, listening: 40, reading: 35, writing: 10 },
        { index: 5, code: 'TOPIK 5', name: 'TOPIK 5', minutes: 105, listening: 45, reading: 40, writing: 10 }
      ],
      defaultLevelIndex: 3
    }
  };

  function speakAudio(text, rate = 1.0) {
    if (!text || !('speechSynthesis' in window)) return;
    try {
      window.speechSynthesis.cancel();
      const utterance = new SpeechSynthesisUtterance(text);
      const langInfo = state.languages[state.activeLanguageId];
      utterance.lang = langInfo ? langInfo.locale : 'zh-CN';
      utterance.rate = rate;
      window.speechSynthesis.speak(utterance);
    } catch (e) {
      console.warn('TTS error:', e);
    }
  }

  function renderExamLevelPills() {
    const strip = document.getElementById('exam-level-pills');
    if (!strip) return;

    const profile = OFFICIAL_EXAM_PROFILES[state.activeLanguageId] || OFFICIAL_EXAM_PROFILES[2];
    const isAll = state.selectedMockLevelIndex === null || state.selectedMockLevelIndex === undefined;

    let html = `<button type="button" class="level-pill-btn pill-all ${isAll ? 'active' : ''}" data-lvl-idx="all">${isAll ? '✓ ' : ''}Tất cả</button>`;

    profile.levels.forEach(lv => {
      const isActive = !isAll && state.selectedMockLevelIndex === lv.index;
      const classMap = { 1: 'pill-lvl1', 2: 'pill-lvl2', 3: 'pill-lvl3', 4: 'pill-lvl4', 5: 'pill-lvl5', 6: 'pill-lvl6' };
      const pillClass = classMap[lv.index] || 'pill-lvl3';
      html += `<button type="button" class="level-pill-btn ${pillClass} ${isActive ? 'active' : ''}" data-lvl-idx="${lv.index}">${isActive ? '✓ ' : ''}${escapeHtml(lv.name)}</button>`;
    });

    strip.innerHTML = html;

    strip.querySelectorAll('.level-pill-btn').forEach(btn => {
      btn.addEventListener('click', () => {
        const idx = btn.dataset.lvlIdx;
        state.selectedMockLevelIndex = idx === 'all' ? null : parseInt(idx, 10);
        if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
        renderExamLevelPills();
        renderMockExamsGrid();
      });
    });
  }

  function renderMockExamsGrid() {
    const container = document.getElementById('mock-exams-container');
    const heading = document.getElementById('mock-exam-level-heading');
    if (!container) return;

    const profile = OFFICIAL_EXAM_PROFILES[state.activeLanguageId] || OFFICIAL_EXAM_PROFILES[2];
    const levelSpec = state.selectedMockLevelIndex !== null && state.selectedMockLevelIndex !== undefined
      ? profile.levels.find(l => l.index === state.selectedMockLevelIndex) || profile.levels[0]
      : (profile.levels.find(l => l.index === profile.defaultLevelIndex) || profile.levels[0]);

    if (heading) {
      heading.textContent = state.selectedMockLevelIndex === null
        ? `Tất cả đề thi (${profile.levels.map(l => l.name).join(' • ')})`
        : levelSpec.name;
    }

    let cardsHtml = '';
    for (let examNo = 1; examNo <= 10; examNo++) {
      const storageKey = `polylearn_mock_${state.activeLanguageId}_${levelSpec.index}_${examNo}`;
      const saved = JSON.parse(localStorage.getItem(storageKey) || 'null');
      const scoreBadge = saved
        ? `<div class="mock-exam-score-badge">✓ Đã làm: ${saved.score}/${saved.total} (${saved.pct}%)</div>`
        : '';

      cardsHtml += `
        <div class="mock-exam-card">
          <div class="mock-exam-card-left">
            <span class="mock-exam-num-pill">Đề ${examNo}</span>
            <div class="mock-exam-sub-label">${escapeHtml(profile.datasetLabel)}</div>
            <button type="button" class="mock-exam-start-btn" data-exam-no="${examNo}" data-lvl-idx="${levelSpec.index}">
              Bắt đầu
            </button>
            ${scoreBadge}
          </div>
          <div class="mock-exam-card-right">
            <div class="mock-exam-spec-row"><span class="spec-icon">⏱️</span> ${levelSpec.minutes} min</div>
            <div class="mock-exam-spec-row"><span class="spec-icon">🎧</span> - ${levelSpec.listening} Qs</div>
            <div class="mock-exam-spec-row"><span class="spec-icon">📖</span> - ${levelSpec.reading} Qs</div>
            <div class="mock-exam-spec-row"><span class="spec-icon">✏️</span> - ${levelSpec.writing} Qs</div>
          </div>
        </div>
      `;
    }

    container.innerHTML = cardsHtml;

    container.querySelectorAll('.mock-exam-start-btn').forEach(btn => {
      btn.addEventListener('click', () => {
        const examNo = parseInt(btn.dataset.examNo, 10) || 1;
        const lvlIdx = parseInt(btn.dataset.lvlIdx, 10) || levelSpec.index;
        const spec = profile.levels.find(l => l.index === lvlIdx) || levelSpec;
        startOfficialMockExam(examNo, spec, btn);
      });
    });
  }

  function startExamCountdownTimer(minutes) {
    stopExamCountdownTimer();
    state.quizTimerRemaining = minutes * 60;
    updateExamTimerBadge();

    state.quizTimerInterval = setInterval(() => {
      state.quizTimerRemaining--;
      updateExamTimerBadge();

      if (state.quizTimerRemaining <= 0) {
        stopExamCountdownTimer();
        showToast('⏱️ Đã hết thời gian làm bài! Hệ thống đang tự động nộp bài...', 'warning');
        submitActiveQuiz();
      }
    }, 1000);
  }

  function stopExamCountdownTimer() {
    if (state.quizTimerInterval) {
      clearInterval(state.quizTimerInterval);
      state.quizTimerInterval = null;
    }
  }

  function updateExamTimerBadge() {
    const badge = document.getElementById('quiz-timer-badge');
    if (!badge) return;

    const rem = Math.max(0, state.quizTimerRemaining);
    const m = Math.floor(rem / 60);
    const s = rem % 60;
    const timeStr = `${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')}`;
    badge.textContent = `⏱️ ${timeStr}`;
    badge.classList.toggle('timer-warning', rem <= 300);
  }

  function jumpToQuizSection(secName) {
    const session = state.quizSession;
    if (!session) return;
    const targetIdx = session.questions.findIndex(q => {
      if (secName === 'listening') return q.section === 'listening';
      if (secName === 'writing') return q.section === 'writing' || q.questionType === 'written';
      return q.section === 'reading' || (!q.section && q.questionType === 'mcq');
    });

    if (targetIdx !== -1) {
      state.quizCurrentQuestionIndex = targetIdx;
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      renderQuizQuestion();
    }
  }

  async function startOfficialMockExam(examNo, spec, triggerBtn) {
    if (!API.getToken()) {
      showToast('Vui lòng đăng nhập để bắt đầu làm bài thi chính thức!', 'info');
      openAuthModal();
      return;
    }

    const originalText = triggerBtn ? triggerBtn.innerHTML : '';
    if (triggerBtn) {
      triggerBtn.disabled = true;
      triggerBtn.textContent = '⏳ Khởi tạo...';
    }

    try {
      const totalQs = spec.listening + spec.reading + spec.writing;

      let dbLevelId = null;
      if (state.cachedLevels && state.cachedLevels.length > 0) {
        const matched = state.cachedLevels.find(l =>
          l.code.toLowerCase() === spec.code.toLowerCase() ||
          l.name.toLowerCase().includes(spec.name.toLowerCase()) ||
          l.sortOrder === spec.index
        );
        if (matched) dbLevelId = matched.id;
      }

      state.activeMockExamSpec = {
        examNo,
        levelIndex: spec.index,
        spec
      };

      const session = await API.generateQuiz({
        languageId: state.activeLanguageId,
        levelId: dbLevelId,
        questionCount: totalQs,
        mode: 'vocab_random',
        examFormat: 'official_mock',
        examNumber: examNo,
        durationMinutes: spec.minutes,
        listeningCount: spec.listening,
        readingCount: spec.reading,
        writingCount: spec.writing,
        harvestNewWords: false
      });

      state.quizSession = session;
      state.quizCurrentQuestionIndex = 0;
      state.quizAnswers = {};
      state.quizWrittenAnswers = {};
      state.lastSpokenQuestionId = null;

      // Setup Section Tabs
      const tabs = document.getElementById('quiz-section-tabs');
      if (tabs) {
        const lCount = session.listeningCount || spec.listening;
        const rCount = session.readingCount || spec.reading;
        const wCount = session.writingCount || spec.writing;
        tabs.innerHTML = `
          <button type="button" class="exam-sec-tab active" data-sec="listening">🎧 Nghe (1–${lCount})</button>
          <button type="button" class="exam-sec-tab" data-sec="reading">📖 Đọc (${lCount + 1}–${lCount + rCount})</button>
          <button type="button" class="exam-sec-tab" data-sec="writing">✏️ Viết (${lCount + rCount + 1}–${lCount + rCount + wCount})</button>
        `;
        tabs.querySelectorAll('.exam-sec-tab').forEach(t => {
          t.addEventListener('click', () => jumpToQuizSection(t.dataset.sec));
        });
      }

      startExamCountdownTimer(spec.minutes || 65);

      const setupBox = document.getElementById('quiz-setup-box');
      const playingBox = document.getElementById('quiz-playing-box');
      const resultBox = document.getElementById('quiz-result-box');

      if (setupBox) setupBox.style.display = 'none';
      if (resultBox) resultBox.style.display = 'none';
      if (playingBox) playingBox.style.display = 'block';
      setQuizArenaMode(true);

      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      refreshMascots('happy', `Đề ${examNo} (${spec.name} • ${spec.minutes} phút) đã bắt đầu! Chúc bạn làm bài thật tốt! 🎯`);

      renderQuizQuestion();
    } catch (err) {
      showToast(err.message, 'error');
    } finally {
      if (triggerBtn) {
        triggerBtn.disabled = false;
        triggerBtn.innerHTML = originalText;
      }
    }
  }

  function renderMockExamHub() {
    renderExamLevelPills();
    renderMockExamsGrid();
  }

  async function topbarQuickHarvest() {
    if (!API.getToken()) {
      showToast('Vui lòng đăng nhập để nạp từ mới vào kho vĩnh viễn!', 'info');
      openAuthModal();
      return;
    }
    const btn = document.getElementById('btn-topbar-harvest');
    if (btn) btn.disabled = true;
    showToast('🌐 Đang săn +8 từ vựng mới và lưu vĩnh viễn vào CSDL...', 'info');

    try {
      const res = await API.harvestWords({
        languageId: state.activeLanguageId,
        count: 8,
        topicHint: 'random'
      });
      if (typeof Gamification !== 'undefined') {
        Gamification.playSound('success');
        Gamification.launchConfetti(45);
      }
      showToast(`✨ Đã nạp thành công +${res.insertedCount} từ vựng mới vào kho vĩnh viễn! (Tổng kho: ${res.totalWordsInLanguage} từ)`, 'success');
      refreshMascots('happy', `Vừa thu hoạch thêm +${res.insertedCount} từ vựng mới vào kho! Vào làm đề ngay nhé! ✨`);
    } catch (err) {
      showToast(err.message, 'error');
    } finally {
      if (btn) btn.disabled = false;
    }
  }

  async function openExamHistoryModal() {
    const modal = document.getElementById('exam-history-modal');
    const list = document.getElementById('exam-history-modal-list');
    if (!modal || !list) return;

    modal.classList.add('active');
    list.innerHTML = `<div style="text-align: center; padding: 24px; color: var(--text-muted);">⏳ Đang tải lịch sử thi...</div>`;

    if (!API.getToken()) {
      list.innerHTML = `<div style="text-align: center; padding: 24px; color: var(--text-muted);">Vui lòng đăng nhập để xem lịch sử làm bài thi của bạn.</div>`;
      return;
    }

    try {
      const paged = await API.getQuizHistory(1, 25);
      const items = paged.items || [];
      if (items.length === 0) {
        list.innerHTML = `<div style="text-align: center; padding: 24px; color: var(--text-muted);">Chưa có lịch sử làm bài. Hãy bắt đầu Đề 1 ngay nhé!</div>`;
        return;
      }

      list.innerHTML = items.map((a, i) => {
        const pct = a.totalQuestions > 0 ? Math.round((a.score / a.totalQuestions) * 100) : 0;
        const passed = pct >= 70;
        const dt = a.completedAt ? new Date(a.completedAt).toLocaleString('vi-VN') : '';
        return `
          <div class="glass-card" style="padding: 14px 18px; display: flex; justify-content: space-between; align-items: center; margin-bottom: 8px;">
            <div>
              <div style="font-weight: 700; color: var(--text-primary); font-size: 0.95rem;">
                ${escapeHtml((a.languageCode || '').toUpperCase())} ${a.levelCode ? `• ${escapeHtml(a.levelCode)}` : ''} • Lượt ${a.attemptId}
              </div>
              <div style="font-size: 0.78rem; color: var(--text-muted);">${dt}</div>
            </div>
            <div style="display: flex; align-items: center; gap: 10px;">
              <span class="card-tag" style="background: ${passed ? 'rgba(16,185,129,0.14)' : 'rgba(239,68,68,0.12)'}; color: ${passed ? 'var(--success-text)' : 'var(--danger)'}; border-color: transparent;">
                ${a.score}/${a.totalQuestions} (${pct}%)
              </span>
              <button type="button" class="btn btn-secondary btn-review-attempt" data-attempt-id="${a.attemptId}" style="padding: 5px 12px; font-size: 0.8rem;">
                Xem lại
              </button>
            </div>
          </div>
        `;
      }).join('');

      list.querySelectorAll('.btn-review-attempt').forEach(btn => {
        btn.addEventListener('click', async () => {
          modal.classList.remove('active');
          const attId = parseInt(btn.dataset.attemptId, 10);
          try {
            const res = await API.getQuizResult(attId);
            showQuizResult(res);
            switchView('quizzes');
          } catch (e) {
            showToast(e.message, 'error');
          }
        });
      });
    } catch (err) {
      list.innerHTML = `<div style="text-align: center; padding: 24px; color: var(--danger);">Lỗi tải lịch sử: ${escapeHtml(err.message)}</div>`;
    }
  }

  let selectedQuizQty = 10;
  let selectedQuizMode = 'vocab_random';
  let selectedExamFormat = 'combined';
  let isSubmittingQuiz = false;

  function setExamFormatSelection(fmt) {
    const normalized = ['combined', 'mcq', 'written'].includes(fmt) ? fmt : 'combined';
    selectedExamFormat = normalized;
    document.querySelectorAll('.exam-format-card').forEach(card => {
      card.classList.toggle('active', card.dataset.format === normalized);
    });
  }

  function setQuizArenaMode(isPlaying) {
    if (quizNavPaletteCard) quizNavPaletteCard.style.display = isPlaying ? 'block' : 'none';
    if (quizQuickPresetsCard) quizQuickPresetsCard.style.display = isPlaying ? 'none' : 'block';
    if (isPlaying) {
      document.body.classList.add('in-exam');
    } else {
      document.body.classList.remove('in-exam');
    }
  }

  function isQuestionAnswered(q) {
    if (!q) return false;
    if (q.questionType === 'written') {
      return Boolean((state.quizWrittenAnswers[q.questionId] || '').trim());
    }
    return state.quizAnswers[q.questionId] !== undefined;
  }

  async function loadQuizSidebarHistory() {
    const listEl = document.getElementById('quiz-sidebar-history-list');
    const pillEl = document.getElementById('quiz-history-total-pill');
    if (!listEl) return;

    if (!API.getToken()) {
      if (pillEl) pillEl.textContent = 'Khách';
      listEl.innerHTML = `<div class="compact-history-item" style="justify-content: center; color: var(--text-muted);">Đăng nhập để lưu lịch sử các bài kiểm tra của bạn.</div>`;
      return;
    }

    try {
      const paged = await API.getQuizHistory(1);
      const items = paged.items || [];
      if (pillEl) pillEl.textContent = `${paged.totalCount ?? items.length} đề`;

      if (items.length === 0) {
        listEl.innerHTML = `<div class="compact-history-item" style="justify-content: center; color: var(--text-muted);">Chưa có lịch sử kiểm tra. Hãy tạo đề đầu tiên ngay!</div>`;
        return;
      }

      listEl.innerHTML = items.slice(0, 6).map(a => {
        const pct = a.totalQuestions > 0 ? Math.round((a.score / a.totalQuestions) * 100) : 0;
        const passed = pct >= 70;
        const dt = a.completedAt ? new Date(a.completedAt).toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' }) : '';
        return `
          <div class="compact-history-item">
            <div>
              <div style="font-weight: 700; color: var(--text-primary);">${escapeHtml((a.languageCode || '').toUpperCase())} ${a.levelCode ? `• ${escapeHtml(a.levelCode)}` : '• Đề kiểm tra'}</div>
              <div style="font-size: 0.75rem; color: var(--text-muted);">${dt}</div>
            </div>
            <span class="card-tag" style="background: ${passed ? 'rgba(16,185,129,0.14)' : 'rgba(239,68,68,0.12)'}; color: ${passed ? 'var(--success-text)' : 'var(--danger)'}; border-color: transparent;">
              ${a.score}/${a.totalQuestions} (${pct}%)
            </span>
          </div>
        `;
      }).join('');
    } catch (e) {
      console.warn('Quiz sidebar history error:', e);
    }
  }

  document.querySelectorAll('.exam-format-card').forEach(card => {
    card.addEventListener('click', () => {
      setExamFormatSelection(card.dataset.format || 'combined');
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
    });
  });

  document.querySelectorAll('.quiz-qty-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.quiz-qty-btn').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      selectedQuizQty = parseInt(btn.dataset.qty, 10) || 10;
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
    });
  });

  document.querySelectorAll('.quiz-mode-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.quiz-mode-btn').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      selectedQuizMode = btn.dataset.mode || 'vocab_random';
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
    });
  });

  // 1-Click Quick Preset Exam Buttons
  document.querySelectorAll('.preset-quiz-item').forEach(btn => {
    btn.addEventListener('click', () => {
      const qty = parseInt(btn.dataset.qty, 10) || 10;
      const newCnt = parseInt(btn.dataset.new, 10) || 5;
      const topic = btn.dataset.topic || 'random';
      const mode = btn.dataset.mode || 'vocab_random';
      const fmt = btn.dataset.format || 'combined';

      selectedQuizQty = qty;
      selectedQuizMode = mode;
      setExamFormatSelection(fmt);
      document.querySelectorAll('.quiz-qty-btn').forEach(b => b.classList.toggle('active', parseInt(b.dataset.qty, 10) === qty));
      document.querySelectorAll('.quiz-mode-btn').forEach(b => b.classList.toggle('active', b.dataset.mode === mode));
      if (quizHarvestToggle) {
        quizHarvestToggle.checked = true;
        if (quizHarvestOptions) quizHarvestOptions.style.display = 'grid';
      }
      if (quizNewWordsCount) quizNewWordsCount.value = String(newCnt);
      if (quizTopicSelect) {
        quizTopicSelect.value = topic;
        if (quizCustomTopicWrap) quizCustomTopicWrap.style.display = 'none';
      }
      startQuizSession(btn);
    });
  });

  if (quizHarvestToggle && quizHarvestOptions) {
    quizHarvestToggle.addEventListener('change', () => {
      quizHarvestOptions.style.display = quizHarvestToggle.checked ? 'grid' : 'none';
    });
  }

  if (quizTopicSelect && quizCustomTopicWrap) {
    quizTopicSelect.addEventListener('change', () => {
      quizCustomTopicWrap.style.display = quizTopicSelect.value === 'custom' ? 'block' : 'none';
      if (quizTopicSelect.value === 'custom' && quizCustomTopic) {
        quizCustomTopic.focus();
      }
    });
  }

  function renderQuizPalette() {
    const paletteEl = document.getElementById('quiz-palette-grid');
    const badgeEl = document.getElementById('quiz-answered-count-badge');
    const session = state.quizSession;
    if (!paletteEl || !session) return;

    const answeredCount = session.questions.filter(q => isQuestionAnswered(q)).length;
    if (badgeEl) badgeEl.textContent = `Đã làm ${answeredCount}/${session.questions.length}`;

    paletteEl.innerHTML = session.questions.map((q, idx) => {
      const isAns = isQuestionAnswered(q);
      const isCur = idx === state.quizCurrentQuestionIndex;
      const isWritten = q.questionType === 'written';
      const label = isWritten ? `✍️${idx + 1}` : `${idx + 1}`;
      return `<button type="button" class="palette-cell ${isWritten ? 'written-cell' : ''} ${isAns ? 'answered' : ''} ${isCur ? 'current' : ''}" data-qidx="${idx}" title="Câu ${idx + 1}: ${isWritten ? 'Tự luận' : 'Trắc nghiệm'}">${label}</button>`;
    }).join('');

    paletteEl.querySelectorAll('.palette-cell').forEach(cell => {
      cell.addEventListener('click', () => {
        state.quizCurrentQuestionIndex = parseInt(cell.dataset.qidx, 10) || 0;
        if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
        renderQuizQuestion();
      });
    });
  }

  async function startQuizSession(triggerBtn = btnStartQuiz) {
    if (!API.getToken()) {
      showToast('Vui lòng đăng nhập để tạo đề kiểm tra và lưu từ vựng vĩnh viễn!', 'info');
      openAuthModal();
      return;
    }

    const harvestNewWords = quizHarvestToggle ? quizHarvestToggle.checked : true;
    const newWordsCount = parseInt(quizNewWordsCount?.value || '5', 10) || 5;
    const rawTopic = quizTopicSelect?.value || 'random';
    let topicHint = null;
    if (rawTopic === 'custom') {
      topicHint = quizCustomTopic?.value.trim() || null;
    } else if (rawTopic !== 'random') {
      topicHint = rawTopic;
    }
    const levelId = quizLevelSelect?.value ? parseInt(quizLevelSelect.value, 10) : null;

    const originalText = triggerBtn ? triggerBtn.innerHTML : '';
    if (triggerBtn) {
      triggerBtn.disabled = true;
      if (triggerBtn === btnStartQuiz || triggerBtn === btnQuickRematchQuiz) {
        triggerBtn.textContent = harvestNewWords
          ? '⏳ Đang săn từ mới & biên soạn đề kiểm tra...'
          : '⏳ Đang biên soạn đề kiểm tra...';
      }
    }

    try {
      const session = await API.generateQuiz({
        languageId: state.activeLanguageId,
        levelId,
        questionCount: selectedQuizQty,
        mode: selectedQuizMode,
        examFormat: selectedExamFormat,
        harvestNewWords,
        newWordsCount,
        topicHint
      });

      state.quizSession = session;
      state.quizCurrentQuestionIndex = 0;
      state.quizAnswers = {};
      state.quizWrittenAnswers = {};

      (session.newlyHarvestedWords || []).forEach(w => state.recentHarvestedWordIds.add(w.id));

      if (quizPlayingHarvestBanner) {
        const compositionParts = [];
        if ((session.mcqCount || 0) > 0) compositionParts.push(`<strong>${session.mcqCount} Trắc nghiệm</strong>`);
        if ((session.writtenCount || 0) > 0) compositionParts.push(`<strong>${session.writtenCount} Tự luận</strong>`);
        const compositionStr = compositionParts.length > 0
          ? `📋 Cấu trúc đề: ${compositionParts.join(' + ')}`
          : `📋 Tổng số: <strong>${session.questions.length} câu</strong>`;

        quizPlayingHarvestBanner.style.display = 'block';
        if (session.newWordsHarvestedCount > 0) {
          quizPlayingHarvestBanner.innerHTML = `${compositionStr} • ✨ Đã lưu vĩnh viễn <strong>+${session.newWordsHarvestedCount} từ mới</strong> (${escapeHtml(session.harvestSource || 'Kho mở rộng')}) • Tổng kho: <strong>${session.totalWordsInBank || 0} từ</strong>`;
        } else if (session.totalWordsInBank > 0) {
          quizPlayingHarvestBanner.innerHTML = `${compositionStr} • 🎲 Rút ngẫu nhiên từ kho <strong>${session.totalWordsInBank} từ vựng chuẩn</strong> trong CSDL`;
        } else {
          quizPlayingHarvestBanner.innerHTML = compositionStr;
        }
      }

      quizSetupBox.style.display = 'none';
      quizResultBox.style.display = 'none';
      quizPlayingBox.style.display = 'block';
      setQuizArenaMode(true);

      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      refreshMascots(
        'happy',
        session.writtenCount > 0 && session.mcqCount > 0
          ? `Đề kết hợp (${session.mcqCount} TN + ${session.writtenCount} Tự luận) đã sẵn sàng! Chiến thôi! 🚀`
          : session.writtenCount > 0
            ? `Đề Tự luận ${session.writtenCount} câu đã sẵn sàng! Chú ý gõ đúng chính tả và ngữ pháp nhé! ✍️`
            : `Đề Trắc nghiệm ${session.questions.length} câu đã sẵn sàng! Tự tin giành điểm tối đa nào! 💪`
      );
      renderQuizQuestion();
    } catch (err) {
      showToast(err.message, 'error');
    } finally {
      if (triggerBtn) {
        triggerBtn.disabled = false;
        triggerBtn.innerHTML = originalText;
      }
    }
  }

  btnStartQuiz.addEventListener('click', () => startQuizSession(btnStartQuiz));
  if (btnQuickRematchQuiz) {
    btnQuickRematchQuiz.addEventListener('click', () => startQuizSession(btnQuickRematchQuiz));
  }

  btnQuitQuiz.addEventListener('click', () => {
    if (confirm('Bạn có chắc muốn hủy bài kiểm tra này? (Các từ mới lấy về vẫn đã được lưu vĩnh viễn trong kho!)')) {
      quizPlayingBox.style.display = 'none';
      quizSetupBox.style.display = 'block';
      setQuizArenaMode(false);
    }
  });

  function syncActiveWrittenAnswer(rawVal) {
    const session = state.quizSession;
    if (!session) return;
    const q = session.questions[state.quizCurrentQuestionIndex];
    if (!q || q.questionType !== 'written') return;

    state.quizWrittenAnswers[q.questionId] = rawVal;
    if (quizWrittenCounter) {
      quizWrittenCounter.textContent = `${rawVal.trim().length} ký tự`;
    }
    renderQuizPalette();
  }

  if (quizWrittenInput) {
    quizWrittenInput.addEventListener('input', (e) => {
      syncActiveWrittenAnswer(e.target.value);
    });
    quizWrittenInput.addEventListener('keydown', (e) => {
      if (e.key === 'Enter') {
        e.preventDefault();
        btnNextQuestion.click();
      }
    });
  }

  if (quizWrittenTextarea) {
    quizWrittenTextarea.addEventListener('input', (e) => {
      syncActiveWrittenAnswer(e.target.value);
    });
    quizWrittenTextarea.addEventListener('keydown', (e) => {
      if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) {
        e.preventDefault();
        btnNextQuestion.click();
      }
    });
  }

  if (btnQuizToggleHint) {
    btnQuizToggleHint.addEventListener('click', () => {
      const session = state.quizSession;
      if (!session || !quizWrittenHintBox) return;
      const q = session.questions[state.quizCurrentQuestionIndex];
      if (!q) return;

      const isHidden = quizWrittenHintBox.style.display === 'none';
      if (isHidden) {
        quizWrittenHintBox.style.display = 'block';
        quizWrittenHintBox.textContent = `💡 ${q.hint || 'Hãy nhập câu trả lời bằng ngôn ngữ đang học.'}`;
        btnQuizToggleHint.textContent = '🙈 Ẩn gợi ý';
      } else {
        quizWrittenHintBox.style.display = 'none';
        btnQuizToggleHint.textContent = '💡 Xem gợi ý đáp án';
      }
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
    });
  }

  if (btnQuizSpeakTarget) {
    btnQuizSpeakTarget.addEventListener('click', () => {
      const session = state.quizSession;
      if (!session) return;
      const q = session.questions[state.quizCurrentQuestionIndex];
      if (q && q.targetTerm) speak(q.targetTerm);
    });
  }

  function renderQuizQuestion() {
    const session = state.quizSession;
    const qIndex = state.quizCurrentQuestionIndex;
    const q = session.questions[qIndex];

    document.getElementById('quiz-progress-text').textContent = `Câu hỏi ${qIndex + 1} / ${session.questions.length}`;
    const qBar = document.getElementById('quiz-progress-bar-fill');
    if (qBar) {
      qBar.style.width = `${Math.round(((qIndex + 1) / session.questions.length) * 100)}%`;
    }
    if (btnPrevQuestion) {
      btnPrevQuestion.disabled = qIndex === 0;
    }

    document.getElementById('quiz-question-text').textContent = q.text;
    const optionsList = document.getElementById('quiz-options-list');
    const listeningBox = document.getElementById('quiz-listening-box');
    const isListening = q.section === 'listening';

    if (listeningBox) {
      listeningBox.style.display = isListening ? 'flex' : 'none';
      if (isListening) {
        const audioText = q.audioText || q.text;
        const btnPlay = document.getElementById('btn-quiz-play-audio');
        const btnSlow = document.getElementById('btn-quiz-play-slow');
        if (btnPlay) {
          btnPlay.onclick = () => speakAudio(audioText, 1.0);
        }
        if (btnSlow) {
          btnSlow.onclick = () => speakAudio(audioText, 0.75);
        }
        if (state.lastSpokenQuestionId !== q.questionId) {
          state.lastSpokenQuestionId = q.questionId;
          speakAudio(audioText, 1.0);
        }
      }
    }

    const currentSec = q.section || (q.questionType === 'written' ? 'writing' : 'reading');
    document.querySelectorAll('.exam-sec-tab').forEach(tab => {
      tab.classList.toggle('active', tab.dataset.sec === currentSec);
    });

    if (q.questionType === 'written') {
      if (optionsList) {
        optionsList.style.display = 'none';
        optionsList.innerHTML = '';
      }
      if (quizWrittenWorkspace) quizWrittenWorkspace.style.display = 'block';

      if (quizQuestionTypeBadge) {
        if (q.writtenSubtype === 'make_sentence') {
          quizQuestionTypeBadge.textContent = '✏️ Viết • Đặt câu (AI chấm ngữ pháp)';
        } else if (q.writtenSubtype === 'cloze') {
          quizQuestionTypeBadge.textContent = '✏️ Viết • Điền từ vào câu';
        } else {
          quizQuestionTypeBadge.textContent = '✏️ Viết • Viết từ vựng';
        }
      }

      if (quizWrittenTargetChip && quizWrittenTargetWord) {
        if (q.targetTerm) {
          quizWrittenTargetChip.style.display = 'flex';
          quizWrittenTargetWord.textContent = q.targetTerm;
        } else {
          quizWrittenTargetChip.style.display = 'none';
          quizWrittenTargetWord.textContent = '';
        }
      }

      const isSentence = q.writtenSubtype === 'make_sentence';
      const savedAnswer = state.quizWrittenAnswers[q.questionId] || '';

      if (quizWrittenInput && quizWrittenTextarea) {
        if (isSentence) {
          quizWrittenInput.style.display = 'none';
          quizWrittenTextarea.style.display = 'block';
          quizWrittenTextarea.value = savedAnswer;
          requestAnimationFrame(() => quizWrittenTextarea.focus());
        } else {
          quizWrittenTextarea.style.display = 'none';
          quizWrittenInput.style.display = 'block';
          quizWrittenInput.value = savedAnswer;
          quizWrittenInput.placeholder = q.writtenSubtype === 'cloze'
            ? 'Nhập từ vựng còn thiếu vào chỗ trống...'
            : 'Nhập chính xác từ vựng bằng ngôn ngữ đang học...';
          requestAnimationFrame(() => quizWrittenInput.focus());
        }
      }

      if (quizWrittenCounter) {
        quizWrittenCounter.textContent = `${savedAnswer.trim().length} ký tự`;
      }
      if (quizWrittenHintBox) {
        quizWrittenHintBox.style.display = 'none';
        quizWrittenHintBox.textContent = '';
      }
      if (btnQuizToggleHint) {
        btnQuizToggleHint.style.display = q.hint ? 'inline-flex' : 'none';
        btnQuizToggleHint.textContent = '💡 Xem gợi ý đáp án';
      }
      if (quizKbdGuide) {
        quizKbdGuide.innerHTML = isSentence
          ? 'Nhấn <code>Ctrl + Enter</code> để sang câu tiếp theo'
          : 'Nhấn <code>Enter</code> để sang câu tiếp theo';
      }
    } else {
      if (quizWrittenWorkspace) quizWrittenWorkspace.style.display = 'none';
      if (optionsList) {
        optionsList.style.display = 'block';
        optionsList.innerHTML = '';
      }
      if (quizQuestionTypeBadge) {
        quizQuestionTypeBadge.textContent = isListening
          ? '🎧 Phần I • Nghe hiểu'
          : '📖 Phần II • Đọc hiểu';
      }
      if (quizKbdGuide) {
        quizKbdGuide.innerHTML = 'Phím <code>1–4</code> chọn đáp án • <code>Enter</code> tiếp tục';
      }

      const optionLabels = ['A', 'B', 'C', 'D', 'E', 'F'];
      (q.options || []).forEach((opt, idx) => {
        const optEl = document.createElement('div');
        optEl.className = 'quiz-option';
        if (state.quizAnswers[q.questionId] === opt.id) {
          optEl.classList.add('selected');
        }
        optEl.innerHTML = `
          <span style="font-weight: 800; color: var(--accent-primary); min-width: 24px;">${optionLabels[idx] || '●'}.</span>
          <span>${escapeHtml(opt.text)}</span>
        `;
        optEl.addEventListener('click', () => {
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          state.quizAnswers[q.questionId] = opt.id;
          document.querySelectorAll('.quiz-option').forEach(o => o.classList.remove('selected'));
          optEl.classList.add('selected');
          renderQuizPalette();
        });
        optionsList.appendChild(optEl);
      });
    }

    if (qIndex === session.questions.length - 1) {
      btnNextQuestion.textContent = '✓ Nộp bài & Chấm điểm';
    } else {
      btnNextQuestion.textContent = 'Câu tiếp theo →';
    }
    renderQuizPalette();
  }

  if (btnPrevQuestion) {
    btnPrevQuestion.addEventListener('click', () => {
      if (state.quizSession && state.quizCurrentQuestionIndex > 0) {
        state.quizCurrentQuestionIndex--;
        if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
        renderQuizQuestion();
      }
    });
  }

  async function submitActiveQuiz() {
    const session = state.quizSession;
    if (!session || isSubmittingQuiz) return;

    isSubmittingQuiz = true;
    const prevNextLabel = btnNextQuestion ? btnNextQuestion.textContent : '';
    const prevSidebarLabel = btnSidebarSubmitQuiz ? btnSidebarSubmitQuiz.textContent : '';
    if (btnNextQuestion) {
      btnNextQuestion.disabled = true;
      btnNextQuestion.textContent = '⏳ Đang chấm bài & AI phân tích...';
    }
    if (btnSidebarSubmitQuiz) {
      btnSidebarSubmitQuiz.disabled = true;
      btnSidebarSubmitQuiz.textContent = '⏳ Đang chấm điểm...';
    }

    const answersPayload = session.questions.map(q => ({
      questionId: q.questionId,
      selectedOptionId: q.questionType === 'mcq' ? (state.quizAnswers[q.questionId] ?? null) : null,
      writtenAnswer: q.questionType === 'written' ? (state.quizWrittenAnswers[q.questionId] ?? '') : null
    }));

    try {
      const result = await API.submitQuiz(session.attemptId, answersPayload);
      showQuizResult(result);
      loadQuizSidebarHistory();
    } catch (err) {
      showToast(err.message, 'error');
    } finally {
      isSubmittingQuiz = false;
      if (btnNextQuestion) {
        btnNextQuestion.disabled = false;
        btnNextQuestion.textContent = prevNextLabel || '✓ Nộp bài & Chấm điểm';
      }
      if (btnSidebarSubmitQuiz) {
        btnSidebarSubmitQuiz.disabled = false;
        btnSidebarSubmitQuiz.textContent = prevSidebarLabel || '✓ Nộp bài & Chấm điểm ngay';
      }
    }
  }

  btnNextQuestion.addEventListener('click', async () => {
    const session = state.quizSession;
    if (!session) return;
    if (state.quizCurrentQuestionIndex < session.questions.length - 1) {
      state.quizCurrentQuestionIndex++;
      renderQuizQuestion();
    } else {
      await submitActiveQuiz();
    }
  });

  if (btnSidebarSubmitQuiz) {
    btnSidebarSubmitQuiz.addEventListener('click', () => submitActiveQuiz());
  }

  function showQuizResult(res) {
    stopExamCountdownTimer();
    quizPlayingBox.style.display = 'none';
    quizResultBox.style.display = 'block';
    const setupBox = document.getElementById('quiz-setup-box');
    if (setupBox) setupBox.style.display = 'none';
    setQuizArenaMode(false);

    document.getElementById('quiz-result-score').textContent = `${res.score} / ${res.totalQuestions}`;
    document.getElementById('quiz-result-percent').textContent =
      `Tỉ lệ chính xác: ${res.percentage}% (${res.isPassed ? 'Đạt yêu cầu' : 'Cần cố gắng thêm'}) • +${res.xpEarned || 0} XP`;
    document.getElementById('quiz-result-percent').style.color = res.isPassed ? 'var(--success-text)' : 'var(--danger)';

    if (quizResultBreakdown) {
      const pills = [];
      if ((res.listeningTotal || 0) > 0) {
        pills.push(`<span class="result-pill">🎧 Nghe: <strong>${res.listeningCorrect || 0}/${res.listeningTotal}</strong></span>`);
      }
      if ((res.readingTotal || 0) > 0) {
        pills.push(`<span class="result-pill">📖 Đọc: <strong>${res.readingCorrect || 0}/${res.readingTotal}</strong></span>`);
      }
      if ((res.writtenTotal || 0) > 0) {
        pills.push(`<span class="result-pill">✏️ Viết: <strong>${res.writtenCorrect || 0}/${res.writtenTotal}</strong></span>`);
      }
      if (pills.length === 0) {
        if ((res.mcqTotal || 0) > 0) {
          pills.push(`<span class="result-pill">📝 Trắc nghiệm: <strong>${res.mcqCorrect || 0}/${res.mcqTotal}</strong></span>`);
        }
        if ((res.writtenTotal || 0) > 0) {
          pills.push(`<span class="result-pill">✍️ Tự luận: <strong>${res.writtenCorrect || 0}/${res.writtenTotal}</strong></span>`);
        }
      }
      pills.push(`<span class="result-pill">⚡ Điểm thưởng: <strong>+${res.xpEarned || 0} XP</strong></span>`);
      quizResultBreakdown.innerHTML = pills.join('');
    }

    // Save best score to localStorage for mock exam card badges
    if (state.activeMockExamSpec) {
      const key = `polylearn_mock_${state.activeLanguageId}_${state.activeMockExamSpec.levelIndex}_${state.activeMockExamSpec.examNo}`;
      const prev = JSON.parse(localStorage.getItem(key) || 'null');
      if (!prev || res.score > prev.score) {
        localStorage.setItem(key, JSON.stringify({ score: res.score, total: res.totalQuestions, pct: res.percentage }));
      }
      renderMockExamsGrid();
    }

    const newlyWords = state.quizSession?.newlyHarvestedWords || [];
    if (quizResultHarvestShowcase) {
      if (newlyWords.length > 0) {
        quizResultHarvestShowcase.style.display = 'block';
        quizResultHarvestShowcase.innerHTML = `
          <div style="font-weight: 700; color: var(--text-primary); font-size: 0.95rem;">
            🌐 Đã lưu vĩnh viễn +${newlyWords.length} từ vựng mới vào Cơ sở dữ liệu từ đề kiểm tra này (Tổng kho: ${state.quizSession?.totalWordsInBank || ''} từ):
          </div>
          <div class="harvested-words-chips">
            ${newlyWords.map(w => `
              <div class="harvested-word-chip">
                <div class="harvested-word-chip-main">
                  <div class="harvested-word-chip-term">
                    <span>${escapeHtml(w.term)}</span>
                    ${w.reading ? `<span class="harvested-word-chip-reading">${escapeHtml(w.reading)}</span>` : ''}
                  </div>
                  <div class="harvested-word-chip-meaning">${escapeHtml(w.meaning)}</div>
                </div>
                <button type="button" class="speaker-btn chip-speak-btn" data-term="${escapeHtml(w.term)}" title="Nghe phát âm" style="width: 30px; height: 30px; font-size: 0.82rem; flex-shrink: 0;">🔊</button>
              </div>
            `).join('')}
          </div>
        `;
        quizResultHarvestShowcase.querySelectorAll('.chip-speak-btn').forEach(btn => {
          btn.addEventListener('click', () => speak(btn.dataset.term));
        });
      } else {
        quizResultHarvestShowcase.style.display = 'none';
      }
    }

    handleXpGain(res, quizResultBox);
    if (res.isPassed) {
      if (typeof Gamification !== 'undefined') {
        Gamification.playSound('fanfare');
        Gamification.launchConfetti(85);
      }
      refreshMascots('party', `🏆 Xuất sắc đạt ${res.percentage}%! Bạn nhận được +${res.xpEarned || 0} XP và tăng mức thành thạo từ vựng!`);
    } else {
      if (typeof Gamification !== 'undefined') Gamification.playSound('wrong');
      refreshMascots('encourage', `Đạt ${res.percentage}% (+${res.xpEarned || 0} XP khích lệ). Xem kỹ lời giải & nhận xét AI bên dưới rồi thử sức đề mới nhé!`);
    }

    const reviewEl = document.getElementById('quiz-answers-review');
    reviewEl.innerHTML = '';

    let wrongCountInExam = 0;
    const sessionQuestions = state.quizSession?.questions || [];

    (res.details || []).forEach((d, idx) => {
      const isWritten = d.questionType === 'written';
      const origQ = sessionQuestions.find(q => q.questionId === d.questionId) || sessionQuestions[idx] || {};

      if (!d.isCorrect) {
        wrongCountInExam++;
        if (typeof MistakeTracker !== 'undefined') {
          const userWrongVal = isWritten
            ? (d.userWrittenAnswer && d.userWrittenAnswer.trim() ? d.userWrittenAnswer.trim() : '(Bỏ trống)')
            : (d.selectedOptionText || '(Chưa chọn)');

          MistakeTracker.recordQuestionMistake({
            id: `quiz_q_${d.questionId || idx}_${(d.questionText || '').slice(0, 24)}`,
            questionText: d.questionText,
            wrongAnswer: userWrongVal,
            correctAnswer: d.correctOptionText || origQ.targetTerm || '',
            explanation: d.explanation || d.aiFeedback || '',
            options: (origQ.options || []).map(o => o.text),
            questionType: d.questionType || 'mcq',
            source: state.activeMockExamSpec
              ? `Đề thi chuẩn #${state.activeMockExamSpec.examNo}`
              : 'Đề kiểm tra'
          });

          if (origQ.targetTerm) {
            MistakeTracker.recordWordMistake({
              id: origQ.wordId || `term_${origQ.targetTerm}`,
              term: origQ.targetTerm,
              reading: origQ.reading || '',
              meaning: d.correctOptionText && d.correctOptionText !== origQ.targetTerm
                ? d.correctOptionText
                : (origQ.hint || d.explanation || 'Từ mục tiêu trong đề thi'),
              levelCode: ''
            }, userWrongVal, 'Từ làm sai trong Đề kiểm tra');
          }
        }
      }

      const item = document.createElement('div');
      item.style.marginBottom = '14px';
      item.style.padding = '14px 18px';
      item.style.background = 'var(--bg-subtle)';
      item.style.borderRadius = 'var(--radius-md)';
      item.style.borderLeft = d.isCorrect ? '4px solid var(--success)' : '4px solid var(--danger)';

      const userAnsDisplay = isWritten
        ? (d.userWrittenAnswer && d.userWrittenAnswer.trim() ? `"${escapeHtml(d.userWrittenAnswer)}"` : '(Bỏ trống)')
        : escapeHtml(d.selectedOptionText || '(Chưa chọn)');

      const secLabel = d.section === 'listening' ? '🎧 Nghe' : isWritten ? '✏️ Viết' : '📖 Đọc';

      item.innerHTML = `
        <div style="display: flex; align-items: center; justify-content: space-between; gap: 8px; flex-wrap: wrap; margin-bottom: 6px;">
          <span style="font-weight: 700; color: var(--text-primary);">Câu ${idx + 1}: ${escapeHtml(d.questionText)}</span>
          <span class="mini-stat-pill" style="font-size: 0.72rem;">${secLabel}</span>
        </div>
        <div style="font-size: 0.94rem; font-weight: 600; color: ${d.isCorrect ? 'var(--success-text)' : 'var(--danger)'};">
          ${isWritten ? 'Bài làm của bạn:' : 'Đáp án đã chọn:'} ${userAnsDisplay} ${d.isCorrect ? '✓ Chính xác' : '✗ Chưa chính xác (📌 Đã lưu vào Câu & Từ Đã Sai)'}
        </div>
        ${!d.isCorrect && d.correctOptionText ? `<div style="font-size: 0.92rem; font-weight: 700; color: var(--success-text); margin-top: 3px;">${isWritten ? 'Đáp án chuẩn / Từ mục tiêu:' : 'Đáp án đúng:'} ${escapeHtml(d.correctOptionText)}</div>` : ''}
        ${d.explanation ? `<div style="font-size: 0.85rem; color: var(--text-muted); margin-top: 4px;">📖 ${escapeHtml(d.explanation)}</div>` : ''}
        ${d.aiFeedback ? `<div class="ai-grading-feedback">🤖 <strong>Nhận xét &amp; Chữa bài:</strong> ${escapeHtml(d.aiFeedback)}</div>` : ''}
      `;
      reviewEl.appendChild(item);
    });

    const mistakesBannerEl = document.getElementById('quiz-result-mistakes-banner');
    if (mistakesBannerEl) {
      if (wrongCountInExam > 0) {
        mistakesBannerEl.style.display = 'flex';
        mistakesBannerEl.style.justifyContent = 'space-between';
        mistakesBannerEl.style.alignItems = 'center';
        mistakesBannerEl.style.flexWrap = 'wrap';
        mistakesBannerEl.style.gap = '12px';
        mistakesBannerEl.innerHTML = `
          <div>
            <strong style="color: #dc2626; font-size: 0.95rem;">⚠️ Đã tự động lưu ${wrongCountInExam} câu làm sai vào "Sổ Tay Câu &amp; Từ Đã Sai"!</strong>
            <div style="font-size: 0.84rem; color: var(--text-secondary); margin-top: 2px;">Bạn có thể mở mục Lộ trình &amp; Từ vựng để ôn tập và làm lại riêng các câu sai này bất cứ lúc nào.</div>
          </div>
          <button type="button" class="btn btn-primary" id="btn-banner-practice-mistakes-now" style="background: linear-gradient(135deg, #ef4444 0%, #ea580c 100%); padding: 8px 16px; font-size: 0.85rem;">
            🔥 Ôn lại ${wrongCountInExam} câu sai ngay
          </button>
        `;
        const bNow = document.getElementById('btn-banner-practice-mistakes-now');
        if (bNow) {
          bNow.addEventListener('click', () => {
            switchView('mistakes');
            if (typeof MistakeTracker !== 'undefined') MistakeTracker.startPractice();
          });
        }
      } else {
        mistakesBannerEl.style.display = 'none';
      }
    }
  }

  const btnQuizResultGoMistakes = document.getElementById('btn-quiz-result-go-mistakes');
  if (btnQuizResultGoMistakes) {
    btnQuizResultGoMistakes.addEventListener('click', () => {
      switchView('mistakes');
    });
  }

  btnRestartQuiz.addEventListener('click', () => {
    stopExamCountdownTimer();
    quizResultBox.style.display = 'none';
    const setupBox = document.getElementById('quiz-setup-box');
    if (setupBox) setupBox.style.display = 'block';
    setQuizArenaMode(false);
    renderMockExamHub();
  });

  // Top Bar & Floating Drawer Action Triggers
  const btnOpenExamHistory = document.getElementById('btn-open-exam-history');
  const btnCloseExamHistoryModal = document.getElementById('btn-close-exam-history-modal');
  const btnCancelExamHistory = document.getElementById('btn-cancel-exam-history');
  const examHistoryModal = document.getElementById('exam-history-modal');

  if (btnOpenExamHistory) {
    btnOpenExamHistory.addEventListener('click', () => openExamHistoryModal());
  }
  if (btnCloseExamHistoryModal && examHistoryModal) {
    btnCloseExamHistoryModal.addEventListener('click', () => examHistoryModal.classList.remove('active'));
  }
  if (btnCancelExamHistory && examHistoryModal) {
    btnCancelExamHistory.addEventListener('click', () => examHistoryModal.classList.remove('active'));
  }

  const btnTopbarHarvest = document.getElementById('btn-topbar-harvest');
  if (btnTopbarHarvest) {
    btnTopbarHarvest.addEventListener('click', () => topbarQuickHarvest());
  }

  const btnTopbarAccount = document.getElementById('btn-topbar-account');
  if (btnTopbarAccount) {
    btnTopbarAccount.addEventListener('click', () => {
      if (API.getToken()) {
        switchView('profile');
      } else {
        openAuthModal();
      }
    });
  }

  const btnFloatingCustomExam = document.getElementById('btn-floating-custom-exam');
  const btnToggleCustomDrawer = document.getElementById('btn-toggle-custom-drawer');
  const btnCloseCustomDrawer = document.getElementById('btn-close-custom-drawer');
  const quizCustomDrawer = document.getElementById('quiz-custom-drawer');

  function toggleCustomDrawer() {
    if (!quizCustomDrawer) return;
    const isHidden = quizCustomDrawer.style.display === 'none';
    quizCustomDrawer.style.display = isHidden ? 'block' : 'none';
    if (isHidden) {
      quizCustomDrawer.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
  }

  if (btnFloatingCustomExam) {
    btnFloatingCustomExam.addEventListener('click', () => {
      switchView('quizzes');
      toggleCustomDrawer();
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
    });
  }
  if (btnToggleCustomDrawer) {
    btnToggleCustomDrawer.addEventListener('click', () => toggleCustomDrawer());
  }
  if (btnCloseCustomDrawer && quizCustomDrawer) {
    btnCloseCustomDrawer.addEventListener('click', () => {
      quizCustomDrawer.style.display = 'none';
    });
  }

  // ---------- 5. WORDS EXPLORER (MASTER-DETAIL & AI INSPECTOR) ----------
  const btnFilterFavs = document.getElementById('btn-filter-favs');
  const wordsTotalBadge = document.getElementById('words-total-badge');
  const wordsHarvestTopic = document.getElementById('words-harvest-topic');
  const btnWordsHarvest = document.getElementById('btn-words-harvest');
  const btnWordsQuickQuiz = document.getElementById('btn-words-quick-quiz');
  const wordSortSelect = document.getElementById('word-sort-select');
  const wordLevelFilter = document.getElementById('word-level-filter');
  const btnWordsPrev = document.getElementById('btn-words-prev');
  const btnWordsNext = document.getElementById('btn-words-next');
  const wordsPageInfo = document.getElementById('words-page-info');
  const wordInspectorContent = document.getElementById('word-inspector-content');

  if (btnFilterFavs) {
    btnFilterFavs.addEventListener('click', () => {
      if (!API.getToken()) {
        showToast('Vui lòng đăng nhập để xem danh sách từ đã lưu!', 'info');
        openAuthModal();
        return;
      }
      state.wordFavoritesOnly = !state.wordFavoritesOnly;
      state.wordsPage = 1;
      btnFilterFavs.classList.toggle('active', state.wordFavoritesOnly);
      btnFilterFavs.style.borderColor = state.wordFavoritesOnly ? 'var(--warning)' : '';
      btnFilterFavs.style.color = state.wordFavoritesOnly ? 'var(--warning-text)' : '';
      loadWords();
    });
  }

  if (wordSortSelect) {
    wordSortSelect.addEventListener('change', () => {
      state.wordsPage = 1;
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      loadWords();
    });
  }

  if (wordLevelFilter) {
    wordLevelFilter.addEventListener('change', () => {
      state.wordLevelFilter = wordLevelFilter.value;
      state.wordsPage = 1;
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      loadWords();
    });
  }

  if (btnWordsPrev) {
    btnWordsPrev.addEventListener('click', () => {
      if (state.wordsPage > 1) {
        state.wordsPage--;
        loadWords();
      }
    });
  }

  if (btnWordsNext) {
    btnWordsNext.addEventListener('click', () => {
      if (state.wordsPage < state.wordsTotalPages) {
        state.wordsPage++;
        loadWords();
      }
    });
  }

  async function inspectWord(wordId, fallbackWord = null) {
    if (!wordInspectorContent) return;
    state.selectedWordId = wordId;
    document.querySelectorAll('.word-card-selectable').forEach(el => {
      el.classList.toggle('selected-word', parseInt(el.dataset.wordId, 10) === wordId);
    });

    wordInspectorContent.innerHTML = `<div style="color: var(--text-muted); padding: 24px; text-align: center;">⏳ Đang tải chi tiết từ vựng...</div>`;

    try {
      const detail = await API.getWordDetail(wordId).catch(() => fallbackWord);
      if (!detail) return;

      const examples = detail.examples || [];
      wordInspectorContent.innerHTML = `
        <div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; margin-bottom: 10px;">
          <div style="display: flex; gap: 6px; flex-wrap: wrap;">
            <span class="card-tag">${escapeHtml(detail.levelCode || 'A1')}</span>
            ${detail.partOfSpeech ? `<span class="mini-stat-pill">${escapeHtml(detail.partOfSpeech)}</span>` : ''}
            ${detail.topicName ? `<span class="mini-stat-pill">🏷️ ${escapeHtml(detail.topicName)}</span>` : ''}
          </div>
          <button type="button" class="speaker-btn" id="btn-insp-speak" title="Nghe phát âm chuẩn">🔊</button>
        </div>

        <div style="font-family: 'Outfit'; font-size: 2.25rem; font-weight: 800; color: var(--text-primary); line-height: 1.15;">
          ${escapeHtml(detail.term)}
        </div>
        <div style="color: var(--reading-color); font-size: 1.08rem; font-weight: 600; margin: 4px 0 8px;">
          ${escapeHtml(detail.reading || '')}
        </div>
        <div style="color: var(--meaning-color); font-size: 1.25rem; font-weight: 800; margin-bottom: 16px;">
          ${escapeHtml(detail.meaning)}
        </div>

        <div style="display: flex; gap: 8px; flex-wrap: wrap; margin-bottom: 18px;">
          <button type="button" class="btn btn-primary" id="btn-insp-mastered" style="flex: 1; padding: 9px 12px; font-size: 0.84rem;">✅ Đánh dấu Đã thuộc (+XP)</button>
          <button type="button" class="btn btn-secondary" id="btn-insp-ai" style="padding: 9px 12px; font-size: 0.84rem;">🤖 Phân tích AI</button>
        </div>

        <div style="border-top: 1px solid var(--border-subtle); padding-top: 14px;">
          <div style="font-size: 0.82rem; font-weight: 800; color: var(--text-muted); text-transform: uppercase; letter-spacing: 0.04em; margin-bottom: 10px;">
            📖 Câu ví dụ song ngữ (${examples.length})
          </div>
          ${examples.length === 0 ? `
            <div style="color: var(--text-muted); font-size: 0.86rem; padding: 10px 0;">Bấm "🤖 Phân tích AI" ở trên để tạo câu ví dụ và phân tích ngữ cảnh sử dụng chi tiết.</div>
          ` : examples.map(ex => `
            <div style="padding: 10px 12px; border-radius: var(--radius-sm); background: var(--bg-surface-elevated); border: 1px solid var(--border-subtle); margin-bottom: 8px;">
              <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 8px;">
                <div style="font-weight: 600; font-size: 0.9rem; color: var(--text-primary);">"${escapeHtml(ex.sentence)}"</div>
                <button type="button" class="speaker-btn btn-ex-speak" data-sent="${escapeHtml(ex.sentence)}" style="width: 26px; height: 26px; font-size: 0.75rem; flex-shrink: 0;">🔊</button>
              </div>
              ${ex.reading ? `<div style="font-size: 0.8rem; color: var(--reading-color); margin-top: 2px;">${escapeHtml(ex.reading)}</div>` : ''}
              <div style="font-size: 0.84rem; color: var(--success-text); font-weight: 600; margin-top: 3px;">${escapeHtml(ex.translation || '')}</div>
            </div>
          `).join('')}
        </div>

        <div id="insp-ai-box" style="display: none; margin-top: 14px; padding: 14px; border-radius: var(--radius-md); background: var(--accent-soft); border: 1px solid var(--border-highlight);"></div>
      `;

      document.getElementById('btn-insp-speak').addEventListener('click', () => speak(detail.term));
      wordInspectorContent.querySelectorAll('.btn-ex-speak').forEach(b => {
        b.addEventListener('click', () => speak(b.dataset.sent));
      });

      document.getElementById('btn-insp-mastered').addEventListener('click', async (e) => {
        const btn = e.currentTarget;
        if (!API.getToken()) {
          showToast('Vui lòng đăng nhập để lưu tiến độ thành thạo từ vựng!', 'info');
          openAuthModal();
          return;
        }
        try {
          const res = await API.reviewCard(detail.id, true);
          showToast(`✅ Đã ghi nhận mức độ thuộc từ "${detail.term}" (Cấp ${res.newBox}/5 • +${res.xpEarned || 10} XP)!`, 'success');
          if (typeof Gamification !== 'undefined') Gamification.playSound('correct');
          handleXpGain(res, btn);
        } catch (err) {
          showToast(err.message, 'error');
        }
      });

      document.getElementById('btn-insp-ai').addEventListener('click', async (e) => {
        const btn = e.currentTarget;
        const aiBox = document.getElementById('insp-ai-box');
        if (!API.getToken()) {
          showToast('Vui lòng đăng nhập để dùng Trợ lý AI phân tích từ vựng!', 'info');
          openAuthModal();
          return;
        }
        btn.disabled = true;
        btn.textContent = '⏳ Đang phân tích...';
        aiBox.style.display = 'block';
        aiBox.innerHTML = `<div style="font-size: 0.85rem; color: var(--text-secondary);">🤖 AI đang phân tích ngữ cảnh và cụm từ đi kèm...</div>`;
        try {
          const langCode = state.languages[state.activeLanguageId]?.code || 'en';
          const aiRes = await API.explainWord(detail.term, langCode);
          aiBox.innerHTML = renderWordExplainHtml(aiRes);
        } catch (err) {
          aiBox.innerHTML = `<div style="color: var(--danger); font-size: 0.85rem;">${escapeHtml(err.message)}</div>`;
        } finally {
          btn.disabled = false;
          btn.textContent = '🤖 Phân tích AI';
        }
      });
    } catch (e) {
      console.warn('Inspect word error:', e);
    }
  }

  if (btnWordsHarvest) {
    btnWordsHarvest.addEventListener('click', async () => {
      if (!API.getToken()) {
        showToast('Vui lòng đăng nhập để lấy và lưu vĩnh viễn từ mới vào kho!', 'info');
        openAuthModal();
        return;
      }

      const originalLabel = btnWordsHarvest.innerHTML;
      btnWordsHarvest.disabled = true;
      btnWordsHarvest.textContent = '⏳ Đang lấy +8 từ mới...';

      try {
        const topicVal = wordsHarvestTopic?.value || 'random';
        const topicHint = topicVal === 'random' ? null : topicVal;
        const res = await API.harvestWords({
          languageId: state.activeLanguageId,
          count: 8,
          topicHint
        });

        (res.words || []).forEach(w => state.recentHarvestedWordIds.add(w.id));
        if (res.words && res.words[0]) {
          state.selectedWordId = res.words[0].id;
        }
        showToast(res.message, 'success');
        handleXpGain(res, btnWordsHarvest);

        if (res.insertedCount > 0 && typeof Gamification !== 'undefined') {
          Gamification.playSound('fanfare');
          Gamification.launchConfetti(55);
          refreshMascots('party', `🎉 Đã lưu vĩnh viễn +${res.insertedCount} từ mới (${res.source})! Kho hiện có ${res.totalWordsInLanguage} từ!`);
        } else if (typeof Gamification !== 'undefined') {
          Gamification.playSound('pop');
        }

        if (state.wordFavoritesOnly) {
          state.wordFavoritesOnly = false;
          if (btnFilterFavs) {
            btnFilterFavs.classList.remove('active');
            btnFilterFavs.style.borderColor = '';
            btnFilterFavs.style.color = '';
          }
        }
        state.wordsPage = 1;
        if (wordSortSelect) wordSortSelect.value = 'newest';
        await loadWords();
        if (typeof MilestoneQuest !== 'undefined') {
          await MilestoneQuest.fetchWordsPool();
          MilestoneQuest.buildMilestones();
          MilestoneQuest.renderRoadmap();
        }
      } catch (err) {
        showToast(err.message, 'error');
      } finally {
        btnWordsHarvest.disabled = false;
        btnWordsHarvest.innerHTML = originalLabel;
      }
    });
  }

  if (btnWordsQuickQuiz) {
    btnWordsQuickQuiz.addEventListener('click', () => {
      quizPlayingBox.style.display = 'none';
      quizResultBox.style.display = 'none';
      quizSetupBox.style.display = 'block';
      setQuizArenaMode(false);
      switchView('quizzes');
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      showToast('📝 Chọn hình thức Trắc nghiệm / Tự luận và bấm Bắt đầu làm bài ngay!', 'info');
    });
  }

  async function loadWords() {
    const searchVal = document.getElementById('word-search-input').value.trim();
    const sortVal = wordSortSelect ? wordSortSelect.value : 'newest';
    const wordsGrid = document.getElementById('words-grid');
    wordsGrid.innerHTML = '<div style="color: var(--text-muted);">Đang tải dữ liệu...</div>';

    try {
      let items = [];
      if (state.wordFavoritesOnly && API.getToken()) {
        const favPaged = await API.getFavorites(state.wordsPage, 24);
        items = (favPaged.items || []).filter(w => {
          if (w.languageId && w.languageId !== state.activeLanguageId) return false;
          if (!searchVal) return true;
          const q = searchVal.toLowerCase();
          return (w.term && w.term.toLowerCase().includes(q)) ||
                 (w.meaning && w.meaning.toLowerCase().includes(q));
        });
        state.wordsTotalPages = Math.max(1, favPaged.totalPages || 1);
        if (wordsTotalBadge) {
          wordsTotalBadge.textContent = `⭐ Yêu thích: ${items.length} từ`;
        }
      } else {
        const queryParams = {
          languageId: state.activeLanguageId,
          search: searchVal,
          sort: sortVal,
          pageNumber: state.wordsPage,
          pageSize: 24
        };
        if (state.wordLevelFilter) {
          queryParams.levelId = state.wordLevelFilter;
        }
        const paged = await API.getWords(queryParams);
        items = paged.items || [];
        state.wordsTotalPages = Math.max(1, paged.totalPages || Math.ceil((paged.totalCount || 1) / 24));
        if (wordsTotalBadge) {
          wordsTotalBadge.textContent = `📦 Kho vĩnh viễn: ${paged.totalCount ?? items.length} từ`;
        }
        const sideWordsBadge = document.getElementById('side-words-badge');
        if (sideWordsBadge) {
          sideWordsBadge.textContent = paged.totalCount ?? items.length;
        }
      }

      if (wordsPageInfo) {
        wordsPageInfo.textContent = `Trang ${state.wordsPage} / ${state.wordsTotalPages}`;
      }
      if (btnWordsPrev) btnWordsPrev.disabled = state.wordsPage <= 1;
      if (btnWordsNext) btnWordsNext.disabled = state.wordsPage >= state.wordsTotalPages;

      wordsGrid.innerHTML = '';
      if (items.length === 0) {
        wordsGrid.innerHTML = `<div style="color: var(--text-muted); grid-column: 1/-1; padding: 32px; text-align: center;" class="glass-card">${
          state.wordFavoritesOnly ? 'Chưa có từ vựng yêu thích nào trong ngôn ngữ này.' : 'Không tìm thấy từ vựng phù hợp bộ lọc.'
        }</div>`;
        return;
      }

      items.forEach(w => {
        const card = document.createElement('div');
        card.className = `glass-card word-card-selectable ${state.selectedWordId === w.id ? 'selected-word' : ''}`;
        card.dataset.wordId = w.id;
        card.style.display = 'flex';
        card.style.flexDirection = 'column';
        card.style.justifyContent = 'space-between';

        const isFavMode = state.wordFavoritesOnly;
        const isRecentlySaved = state.recentHarvestedWordIds.has(w.id);
        card.innerHTML = `
          <div>
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 6px; gap: 6px;">
              <div style="display: flex; align-items: center; gap: 6px; flex-wrap: wrap;">
                <span class="card-tag">${escapeHtml(w.levelCode)}</span>
                ${isRecentlySaved ? '<span class="word-new-badge">✨ Mới lưu</span>' : ''}
              </div>
              <button class="speaker-btn" data-term="${escapeHtml(w.term)}" style="width: 30px; height: 30px; font-size: 0.85rem;">🔊</button>
            </div>
            <div style="font-size: 1.45rem; font-weight: 800; font-family: 'Outfit'; color: var(--text-primary);">${escapeHtml(w.term)}</div>
            <div style="color: var(--reading-color); font-size: 0.9rem; font-weight: 500; margin-bottom: 4px;">${escapeHtml(w.reading || '')}</div>
            <div style="color: var(--meaning-color); font-weight: 700; font-size: 1rem;">${escapeHtml(w.meaning)}</div>
          </div>
          <div style="display: flex; justify-content: space-between; align-items: center; margin-top: 12px; padding-top: 8px; border-top: 1px solid var(--border-subtle); font-size: 0.78rem; color: var(--text-muted); gap: 4px; flex-wrap: wrap;">
            <span>${escapeHtml(w.partOfSpeech || 'Từ')}${w.examplesCount > 0 ? ` • ${w.examplesCount} ví dụ` : ''}</span>
            <div style="display: flex; gap: 4px;">
              <button class="btn btn-secondary btn-mark-hard" data-id="${w.id}" title="Lưu từ này vào Sổ tay Câu & Từ đã sai để ôn tập" style="padding: 3px 8px; font-size: 0.73rem; color: #dc2626; border-color: rgba(239,68,68,0.3);">
                ⚠️ Ôn sai
              </button>
              <button class="btn btn-secondary btn-fav" data-id="${w.id}" style="padding: 3px 9px; font-size: 0.75rem; ${isFavMode ? 'color: var(--warning-text); border-color: rgba(245,158,11,0.45);' : ''}">
                ${isFavMode ? '⭐ Bỏ lưu' : '⭐ Ghim'}
              </button>
            </div>
          </div>
        `;

        card.addEventListener('click', (e) => {
          if (e.target.closest('.speaker-btn') || e.target.closest('.btn-fav') || e.target.closest('.btn-mark-hard')) return;
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          inspectWord(w.id, w);
        });

        card.querySelector('.speaker-btn').addEventListener('click', () => speak(w.term));
        card.querySelector('.btn-mark-hard').addEventListener('click', () => {
          if (typeof MistakeTracker !== 'undefined') {
            MistakeTracker.recordWordMistake(w, 'Đánh dấu cần ôn kỹ', 'Kho từ vựng');
            if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
            showToast(`⚠️ Đã lưu từ "${w.term}" vào Sổ Tay Câu & Từ Đã Sai!`, 'info');
          }
        });
        card.querySelector('.btn-fav').addEventListener('click', async (e) => {
          const btnEl = e.currentTarget;
          if (!API.getToken()) {
            showToast('Vui lòng đăng nhập để lưu từ yêu thích!', 'info');
            openAuthModal();
            return;
          }
          try {
            if (isFavMode) {
              await API.toggleFavorite(w.id, false);
              showToast(`Đã bỏ lưu "${w.term}".`, 'info');
              loadWords();
            } else {
              await API.toggleFavorite(w.id, true);
              btnEl.style.color = 'var(--warning-text)';
              if (typeof Gamification !== 'undefined') {
                Gamification.playSound('pop');
                Gamification.spawnHeartBurst(btnEl);
              }
              showToast(`Đã lưu "${w.term}" vào bộ sưu tập yêu thích! ⭐`, 'success');
            }
          } catch (err) {
            showToast(err.message, 'error');
          }
        });

        wordsGrid.appendChild(card);
      });

      const toInspect = items.find(i => i.id === state.selectedWordId) || items[0];
      if (toInspect) {
        inspectWord(toInspect.id, toInspect);
      }
    } catch (err) {
      wordsGrid.innerHTML = `<div style="color: var(--danger);">${err.message}</div>`;
    }
  }

  document.getElementById('word-search-input').addEventListener('input', debounce(() => {
    state.wordsPage = 1;
    loadWords();
  }, 400));

  // =========================================================================
  // 5B. GAMIFIED MILESTONE QUEST ROADMAP & ARENA ENGINE
  // =========================================================================
  const MilestoneQuest = (() => {
    let questWordsPool = [];
    let activeMilestones = [];
    let currentMilestoneIndex = 1;
    let arenaTimer = null;
    let arenaTimeRemaining = 10;
    let arenaState = {
      mIndex: 1,
      questions: [],
      currentIndex: 0,
      hearts: 3,
      combo: 0,
      maxCombo: 0,
      wrongWords: [],
      isBoss: false,
      bossMaxHp: 100,
      bossCurrentHp: 100
    };

    const MILESTONE_THEMES = [
      { name: 'Khởi Động Phản Xạ', icon: '🎯', type: 'mcq' },
      { name: 'Thính Giác Thần Tốc', icon: '🎧', type: 'audio' },
      { name: 'Nối Từ Tốc Chiến', icon: '⚡', type: 'match' },
      { name: 'Điền Chữ Ngữ Cảnh', icon: '🏹', type: 'context' },
      { name: 'ĐẠI CHIẾN HẮC MA', icon: '👑', type: 'boss', isBoss: true },
      { name: 'Tăng Tốc Siêu Cấp', icon: '🚀', type: 'mcq' },
      { name: 'Siêu Thính Vượt Bão', icon: '💎', type: 'audio' },
      { name: 'Cơn Lốc Nối Cặp', icon: '🌪️', type: 'match' },
      { name: 'Giải Mã Cụm Từ', icon: '🔮', type: 'context' },
      { name: 'ĐẠI CHIẾN BẠO LONG', icon: '🐉', type: 'boss', isBoss: true },
      { name: 'Chiến Binh Đa Ngữ', icon: '🛡️', type: 'mcq' },
      { name: 'Âm Vang Xuyên Lục Địa', icon: '🌟', type: 'audio' }
    ];

    function getStorageKey() {
      return `polylearn_quest_progress_${state.activeLanguageId}`;
    }

    function getProgress() {
      const raw = localStorage.getItem(getStorageKey());
      if (!raw) {
        return {
          unlockedMilestone: 1,
          stars: {},
          streak: 0,
          clearedCount: 0,
          openedChests: []
        };
      }
      try {
        return JSON.parse(raw);
      } catch {
        return { unlockedMilestone: 1, stars: {}, streak: 0, clearedCount: 0, openedChests: [] };
      }
    }

    function saveProgress(prog) {
      localStorage.setItem(getStorageKey(), JSON.stringify(prog));
      updateHudStats(prog);
    }

    function updateHudStats(prog = getProgress()) {
      const totalStars = Object.values(prog.stars || {}).reduce((a, b) => a + b, 0);
      const starEl = document.getElementById('quest-total-stars');
      const clearedEl = document.getElementById('quest-cleared-count');
      const totalMEl = document.getElementById('quest-total-milestones');
      const streakEl = document.getElementById('quest-streak-count');
      const rankEl = document.getElementById('quest-hero-rank');
      const chestSubEl = document.getElementById('quest-chest-progress');

      if (starEl) starEl.textContent = totalStars;
      if (clearedEl) clearedEl.textContent = Object.keys(prog.stars || {}).filter(k => prog.stars[k] > 0).length;
      if (totalMEl) totalMEl.textContent = Math.max(10, activeMilestones.length || 10);
      if (streakEl) streakEl.textContent = prog.streak || 0;

      if (rankEl) {
        if (totalStars >= 30) rankEl.textContent = '👑 Đại Sư Đa Ngữ • Cấp 5';
        else if (totalStars >= 20) rankEl.textContent = '💎 Bậc Thầy Phản Xạ • Cấp 4';
        else if (totalStars >= 12) rankEl.textContent = '🥇 Chiến Binh Tinh Anh • Cấp 3';
        else if (totalStars >= 5) rankEl.textContent = '🥈 Thợ Săn Từ Vựng • Cấp 2';
        else rankEl.textContent = '🏅 Tân Binh Vượt Ải • Cấp 1';
      }

      if (chestSubEl) {
        const openedCount = (prog.openedChests || []).length;
        const required = (openedCount + 1) * 6;
        if (totalStars >= required) {
          chestSubEl.textContent = '🎉 Đã sẵn sàng mở rương!';
          chestSubEl.style.color = '#059669';
        } else {
          chestSubEl.textContent = `Tích ${required - totalStars} ⭐ nữa để mở`;
          chestSubEl.style.color = '#b45309';
        }
      }
    }

    function updateHeroAvatar() {
      const avatarWrap = document.getElementById('quest-hero-avatar');
      const mascotSpeech = document.getElementById('quest-mascot-speech');
      if (!avatarWrap) return;

      const mascotIcons = { 1: '🦉', 2: '🐼', 3: '🐱', 4: '🐯' };
      const mascotNames = { 1: 'Giáo sư Oliver', 2: 'Võ sư BaoBao', 3: 'Mèo Mochi', 4: 'Hổ Horangi' };
      const icon = mascotIcons[state.activeLanguageId] || '🦉';
      const name = mascotNames[state.activeLanguageId] || 'Linh vật dẫn đường';

      avatarWrap.innerHTML = `<div class="quest-hero-avatar-circle">${icon}</div>`;
      if (mascotSpeech) {
        mascotSpeech.textContent = `"${name} đang đồng hành cùng bạn! Hãy vượt từng ải để tích lũy sao ⭐, mở rương báu và trở thành Bậc Thầy Đa Ngữ!"`;
      }
    }

    async function fetchWordsPool() {
      try {
        const res = await API.getWords({
          languageId: state.activeLanguageId,
          pageSize: 120,
          sort: 'random'
        });
        questWordsPool = res.items || [];
      } catch (e) {
        console.warn('Fetch quest words error:', e);
        questWordsPool = [];
      }
    }

    function buildMilestones() {
      const wordsPerMilestone = 5;
      const count = Math.max(10, Math.ceil((questWordsPool.length || 10) / wordsPerMilestone));
      activeMilestones = [];

      for (let i = 1; i <= count; i++) {
        const themeIndex = (i - 1) % MILESTONE_THEMES.length;
        const theme = MILESTONE_THEMES[themeIndex];
        const start = ((i - 1) * wordsPerMilestone) % Math.max(1, questWordsPool.length);
        let chunk = questWordsPool.slice(start, start + wordsPerMilestone);
        if (chunk.length < wordsPerMilestone && questWordsPool.length > 0) {
          chunk = chunk.concat(questWordsPool.slice(0, wordsPerMilestone - chunk.length));
        }

        activeMilestones.push({
          index: i,
          name: theme.name,
          icon: theme.icon,
          type: theme.type,
          isBoss: !!theme.isBoss,
          words: chunk
        });
      }
    }

    function renderRoadmap() {
      const track = document.getElementById('quest-roadmap-track');
      if (!track) return;

      const prog = getProgress();
      const currentUnlocked = prog.unlockedMilestone || 1;
      updateHudStats(prog);
      updateHeroAvatar();

      track.innerHTML = '';

      const posPattern = ['pos-center', 'pos-left', 'pos-center', 'pos-right'];

      activeMilestones.forEach((m, idx) => {
        const posClass = posPattern[idx % posPattern.length];
        const stars = prog.stars && prog.stars[m.index] ? prog.stars[m.index] : 0;
        const isCleared = stars > 0;
        const isActive = m.index === currentUnlocked;
        const isLocked = m.index > currentUnlocked;

        let statusClass = 'status-locked';
        if (isCleared) statusClass = 'status-cleared';
        else if (isActive) statusClass = 'status-active';

        const row = document.createElement('div');
        row.className = `quest-milestone-row ${posClass}`;

        const mascotIcons = { 1: '🦉 Oliver', 2: '🐼 BaoBao', 3: '🐱 Mochi', 4: '🐯 Horangi' };
        const activeMascotTag = isActive
          ? `<div class="quest-active-mascot-pill">${mascotIcons[state.activeLanguageId] || '🦉'}: Chiến ngay!</div>`
          : '';

        const starHtml = `
          <div class="milestone-stars-row">
            <span class="${stars >= 1 ? '' : 'star-dim'}">⭐</span>
            <span class="${stars >= 2 ? '' : 'star-dim'}">⭐</span>
            <span class="${stars >= 3 ? '' : 'star-dim'}">⭐</span>
          </div>
        `;

        row.innerHTML = `
          ${activeMascotTag}
          <button type="button" class="milestone-btn ${statusClass} ${m.isBoss ? 'is-boss' : ''}" data-mindex="${m.index}">
            <span class="milestone-inner-icon">${isLocked ? '🔒' : m.icon}</span>
          </button>
          <div class="milestone-meta">
            <div class="milestone-title-pill">Mốc ${m.index} • ${escapeHtml(m.name)}</div>
            ${starHtml}
          </div>
        `;

        const btn = row.querySelector('.milestone-btn');
        btn.addEventListener('click', () => {
          if (isLocked) {
            if (typeof Gamification !== 'undefined') Gamification.playSound('wrong');
            showToast(`🔒 Mốc ${m.index} đang bị khóa! Bạn cần vượt qua Mốc ${currentUnlocked} trước nhé!`, 'info');
            return;
          }
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          startMilestone(m.index);
        });

        track.appendChild(row);
      });
    }

    // ---------- ARENA GAMEPLAY ENGINE ----------
    function startMilestone(mIndex) {
      const milestone = activeMilestones.find(m => m.index === mIndex);
      if (!milestone) return;

      const words = (milestone.words && milestone.words.length >= 3)
        ? milestone.words
        : questWordsPool.slice(0, 5);

      if (words.length === 0) {
        showToast('Chưa có đủ từ vựng để tạo ải! Bấm "+8 từ mới vĩnh viễn" để nạp ngay nhé!', 'warning');
        return;
      }

      arenaState = {
        mIndex: mIndex,
        type: milestone.type,
        isBoss: milestone.isBoss,
        words: words,
        currentIndex: 0,
        hearts: 3,
        combo: 0,
        maxCombo: 0,
        wrongWords: [],
        bossMaxHp: 100,
        bossCurrentHp: 100,
        totalQuestions: milestone.isBoss ? 8 : 5
      };

      const modal = document.getElementById('quest-game-modal');
      const tag = document.getElementById('arena-milestone-tag');
      if (tag) tag.textContent = `${milestone.icon} MỐC ${mIndex}: ${milestone.name}`;
      if (modal) modal.style.display = 'flex';

      updateArenaHUD();
      renderCurrentQuestion();
    }

    function updateArenaHUD() {
      const heartsEl = document.getElementById('arena-hearts');
      const comboEl = document.getElementById('arena-combo-badge');
      const fillEl = document.getElementById('arena-progress-fill');
      const stepEl = document.getElementById('arena-progress-step');

      if (heartsEl) {
        const heartsArr = [];
        for (let i = 0; i < 3; i++) {
          heartsArr.push(i < arenaState.hearts ? '❤️' : '🖤');
        }
        heartsEl.textContent = heartsArr.join('');
      }

      if (comboEl) {
        if (arenaState.combo >= 2) {
          comboEl.style.display = 'inline-block';
          comboEl.textContent = `🔥 Combo x${arenaState.combo}`;
        } else {
          comboEl.style.display = 'none';
        }
      }

      const total = arenaState.totalQuestions;
      const cur = arenaState.currentIndex + 1;
      const pct = Math.min(100, Math.round((cur / total) * 100));
      if (fillEl) fillEl.style.width = `${pct}%`;
      if (stepEl) stepEl.textContent = `Ải ${Math.min(cur, total)} / ${total}`;
    }

    function startTimer(duration = 10, onTimeout) {
      clearInterval(arenaTimer);
      arenaTimeRemaining = duration;
      const timerEl = document.getElementById('arena-timer');
      if (timerEl) {
        timerEl.textContent = `⏱️ ${arenaTimeRemaining}s`;
        timerEl.style.color = 'var(--accent-primary)';
      }

      arenaTimer = setInterval(() => {
        arenaTimeRemaining--;
        if (timerEl) {
          timerEl.textContent = `⏱️ ${arenaTimeRemaining}s`;
          if (arenaTimeRemaining <= 3) {
            timerEl.style.color = 'var(--danger)';
          }
        }
        if (arenaTimeRemaining <= 0) {
          clearInterval(arenaTimer);
          if (onTimeout) onTimeout();
        }
      }, 1000);
    }

    function stopTimer() {
      clearInterval(arenaTimer);
    }

    function renderCurrentQuestion() {
      const body = document.getElementById('arena-body');
      if (!body) return;

      if (arenaState.currentIndex >= arenaState.totalQuestions) {
        finishMilestone(true);
        return;
      }

      updateArenaHUD();

      // Pick target word
      const targetWord = arenaState.words[arenaState.currentIndex % arenaState.words.length];
      const otherWords = questWordsPool.filter(w => w.id !== targetWord.id);

      // Handle Matching type milestone
      if (arenaState.type === 'match' && arenaState.currentIndex === 0) {
        renderSpeedMatchArena(body);
        return;
      }

      // Default Question Types: MCQ, Audio Blitz, Context, or Boss Battle
      const isAudioType = arenaState.type === 'audio' || (arenaState.isBoss && arenaState.currentIndex % 2 === 1);
      const isContextType = arenaState.type === 'context' && targetWord.examples && targetWord.examples.length > 0;

      // Prepare 4 choices
      let distractors = otherWords
        .map(w => w.meaning)
        .filter(m => m !== targetWord.meaning)
        .sort(() => 0.5 - Math.random())
        .slice(0, 3);

      if (distractors.length < 3) {
        distractors = ['Đang cập nhật...', 'Chính xác', 'Cần ôn thêm'];
      }

      const choices = [targetWord.meaning, ...distractors].sort(() => 0.5 - Math.random());

      // Auto speak audio for active language
      if (isAudioType || arenaState.currentIndex === 0) {
        setTimeout(() => speak(targetWord.term), 300);
      }

      let bossHeaderHtml = '';
      if (arenaState.isBoss) {
        const hpPct = Math.max(0, Math.round((arenaState.bossCurrentHp / arenaState.bossMaxHp) * 100));
        bossHeaderHtml = `
          <div class="boss-arena-header">
            <div class="boss-avatar-box">👹</div>
            <div class="boss-name">Trùm Hắc Ma Trừu Tượng</div>
            <div class="boss-hp-track">
              <div class="boss-hp-fill" style="width: ${hpPct}%;"></div>
            </div>
            <div style="font-size: 0.78rem; font-weight: 700; color: #dc2626;">HP: ${arenaState.bossCurrentHp} / ${arenaState.bossMaxHp}</div>
          </div>
        `;
      }

      let promptHtml = '';
      if (isAudioType) {
        promptHtml = `
          <div class="arena-question-card">
            <div style="font-size: 0.88rem; font-weight: 700; color: var(--text-muted); margin-bottom: 8px;">🎧 NGHE VÀ CHỌN NGHĨA CHUẨN XÁC:</div>
            <button type="button" class="arena-audio-prompt-btn" id="btn-arena-repeat-audio">
              🔊 Nghe phát âm lại
            </button>
            <div style="font-size: 0.84rem; color: var(--reading-color); font-weight: 600;">[ ${escapeHtml(targetWord.reading || targetWord.partOfSpeech || 'Từ vựng')} ]</div>
          </div>
        `;
      } else if (isContextType) {
        const ex = targetWord.examples[0];
        const maskedSentence = ex.sentence.replace(new RegExp(targetWord.term, 'gi'), '_______');
        promptHtml = `
          <div class="arena-question-card">
            <div style="font-size: 0.88rem; font-weight: 700; color: var(--text-muted); margin-bottom: 8px;">🏹 ĐIỀN TỪ CÒN THIẾU TRONG CÂU:</div>
            <div style="font-size: 1.35rem; font-weight: 700; color: var(--text-primary); margin-bottom: 6px;">"${escapeHtml(maskedSentence)}"</div>
            <div style="font-size: 0.9rem; color: var(--text-secondary); margin-bottom: 8px;">👉 ${escapeHtml(ex.translation || targetWord.meaning)}</div>
          </div>
        `;
      } else {
        promptHtml = `
          <div class="arena-question-card">
            <div style="font-size: 0.84rem; font-weight: 700; color: var(--text-muted); text-transform: uppercase; margin-bottom: 6px;">
              Từ vựng ${escapeHtml(targetWord.levelCode || 'A1')} • ${escapeHtml(targetWord.partOfSpeech || 'Từ')}
            </div>
            <div class="arena-term-display">${escapeHtml(targetWord.term)}</div>
            ${targetWord.reading ? `<div class="arena-reading-display">${escapeHtml(targetWord.reading)}</div>` : ''}
            <button type="button" class="speaker-btn" id="btn-arena-word-speak" style="width: 32px; height: 32px; font-size: 0.88rem; margin: 4px auto 8px;" title="Nghe phát âm">🔊</button>
          </div>
        `;
      }

      body.innerHTML = `
        ${bossHeaderHtml}
        ${promptHtml}
        <div class="arena-options-grid" id="arena-options-grid">
          ${choices.map((c, i) => `
            <button type="button" class="arena-option-btn" data-choice="${escapeHtml(c)}">
              <span>${escapeHtml(c)}</span>
              <span style="font-size: 0.8rem; opacity: 0.5;">[${i + 1}]</span>
            </button>
          `).join('')}
        </div>
      `;

      const repeatBtn = document.getElementById('btn-arena-repeat-audio');
      if (repeatBtn) repeatBtn.addEventListener('click', () => speak(targetWord.term));

      const wordSpeakBtn = document.getElementById('btn-arena-word-speak');
      if (wordSpeakBtn) wordSpeakBtn.addEventListener('click', () => speak(targetWord.term));

      // Start Countdown Timer
      startTimer(10, () => {
        handleAnswer(null, targetWord, choices);
      });

      // Bind Choice Clicks
      document.querySelectorAll('.arena-option-btn').forEach(btn => {
        btn.addEventListener('click', (e) => {
          stopTimer();
          const chosen = e.currentTarget.dataset.choice;
          handleAnswer(chosen, targetWord, choices);
        });
      });
    }

    function handleAnswer(chosen, targetWord, choices) {
      stopTimer();
      const isCorrect = chosen === targetWord.meaning;
      const optionBtns = document.querySelectorAll('.arena-option-btn');

      optionBtns.forEach(b => {
        b.disabled = true;
        if (b.dataset.choice === targetWord.meaning) {
          b.classList.add('correct');
        } else if (b.dataset.choice === chosen) {
          b.classList.add('wrong');
        }
      });

      if (isCorrect) {
        if (typeof Gamification !== 'undefined') Gamification.playSound('correct');
        arenaState.combo++;
        if (arenaState.combo > arenaState.maxCombo) {
          arenaState.maxCombo = arenaState.combo;
        }

        if (arenaState.isBoss) {
          const dmg = Math.ceil(arenaState.bossMaxHp / arenaState.totalQuestions);
          arenaState.bossCurrentHp = Math.max(0, arenaState.bossCurrentHp - dmg);
        }

        setTimeout(() => {
          arenaState.currentIndex++;
          renderCurrentQuestion();
        }, 750);
      } else {
        if (typeof Gamification !== 'undefined') Gamification.playSound('wrong');
        arenaState.combo = 0;
        arenaState.hearts--;
        arenaState.wrongWords.push(targetWord);
        if (typeof MistakeTracker !== 'undefined') {
          MistakeTracker.recordWordMistake(targetWord, chosen || '(Hết giờ)', `Đấu trường Mốc ${arenaState.mIndex}`);
        }
        updateArenaHUD();

        if (arenaState.hearts <= 0) {
          setTimeout(() => finishMilestone(false), 800);
        } else {
          setTimeout(() => {
            arenaState.currentIndex++;
            renderCurrentQuestion();
          }, 1100);
        }
      }
    }

    // ---------- SPECIAL MINI-GAME: SPEED MATCH ARENA ----------
    function renderSpeedMatchArena(body) {
      stopTimer();
      const matchWords = arenaState.words.slice(0, 4);
      let selectedTerm = null;
      let selectedMeaning = null;
      let matchedCount = 0;

      const terms = matchWords.map(w => ({ id: w.id, text: w.term })).sort(() => 0.5 - Math.random());
      const meanings = matchWords.map(w => ({ id: w.id, text: w.meaning })).sort(() => 0.5 - Math.random());

      body.innerHTML = `
        <div class="arena-question-card">
          <div style="font-size: 0.88rem; font-weight: 700; color: var(--text-muted); margin-bottom: 6px;">⚡ TỐC CHIẾN GHÉP CẶP:</div>
          <p style="font-size: 0.9rem; color: var(--text-secondary); margin: 0 0 16px;">Nhấp 1 từ bên trái và 1 nghĩa bên phải tương ứng để nối cặp thành công!</p>
        </div>
        <div class="arena-matching-wrap">
          <div class="matching-col" id="match-col-terms">
            ${terms.map(t => `<div class="matching-tile" data-side="term" data-id="${t.id}">${escapeHtml(t.text)}</div>`).join('')}
          </div>
          <div class="matching-col" id="match-col-meanings">
            ${meanings.map(m => `<div class="matching-tile" data-side="meaning" data-id="${m.id}">${escapeHtml(m.text)}</div>`).join('')}
          </div>
        </div>
      `;

      startTimer(20, () => {
        arenaState.hearts--;
        if (typeof Gamification !== 'undefined') Gamification.playSound('wrong');
        updateArenaHUD();
        if (arenaState.hearts <= 0) finishMilestone(false);
        else {
          arenaState.currentIndex = arenaState.totalQuestions;
          finishMilestone(true);
        }
      });

      const termTiles = body.querySelectorAll('[data-side="term"]');
      const meanTiles = body.querySelectorAll('[data-side="meaning"]');

      function checkMatch() {
        if (!selectedTerm || !selectedMeaning) return;
        const termId = selectedTerm.dataset.id;
        const meanId = selectedMeaning.dataset.id;

        if (termId === meanId) {
          if (typeof Gamification !== 'undefined') Gamification.playSound('correct');
          selectedTerm.classList.remove('selected');
          selectedTerm.classList.add('matched');
          selectedMeaning.classList.remove('selected');
          selectedMeaning.classList.add('matched');
          matchedCount++;

          const wordObj = matchWords.find(w => w.id === parseInt(termId, 10));
          if (wordObj) speak(wordObj.term);

          selectedTerm = null;
          selectedMeaning = null;

          if (matchedCount >= matchWords.length) {
            stopTimer();
            setTimeout(() => {
              arenaState.currentIndex = arenaState.totalQuestions;
              finishMilestone(true);
            }, 600);
          }
        } else {
          if (typeof Gamification !== 'undefined') Gamification.playSound('wrong');
          selectedTerm.classList.add('wrong');
          selectedMeaning.classList.add('wrong');
          arenaState.hearts--;
          const missedWordObj = matchWords.find(w => w.id === parseInt(termId, 10));
          if (missedWordObj) {
            arenaState.wrongWords.push(missedWordObj);
            if (typeof MistakeTracker !== 'undefined') {
              MistakeTracker.recordWordMistake(missedWordObj, selectedMeaning.textContent || 'Nối nhầm nghĩa', `Đấu trường Ghép Cặp Mốc ${arenaState.mIndex}`);
            }
          }
          updateArenaHUD();

          setTimeout(() => {
            selectedTerm.classList.remove('selected', 'wrong');
            selectedMeaning.classList.remove('selected', 'wrong');
            selectedTerm = null;
            selectedMeaning = null;
            if (arenaState.hearts <= 0) {
              stopTimer();
              finishMilestone(false);
            }
          }, 500);
        }
      }

      termTiles.forEach(tile => {
        tile.addEventListener('click', () => {
          if (tile.classList.contains('matched')) return;
          termTiles.forEach(t => t.classList.remove('selected'));
          tile.classList.add('selected');
          selectedTerm = tile;
          checkMatch();
        });
      });

      meanTiles.forEach(tile => {
        tile.addEventListener('click', () => {
          if (tile.classList.contains('matched')) return;
          meanTiles.forEach(t => t.classList.remove('selected'));
          tile.classList.add('selected');
          selectedMeaning = tile;
          checkMatch();
        });
      });
    }

    // ---------- END OF MILESTONE (VICTORY / DEFEAT) ----------
    function finishMilestone(isWin) {
      stopTimer();
      const body = document.getElementById('arena-body');
      if (!body) return;

      const prog = getProgress();
      const mIdx = arenaState.mIndex;

      if (isWin) {
        let stars = 1;
        if (arenaState.hearts === 3) stars = 3;
        else if (arenaState.hearts === 2) stars = 2;

        prog.stars = prog.stars || {};
        const oldStars = prog.stars[mIdx] || 0;
        if (stars > oldStars) prog.stars[mIdx] = stars;

        if (mIdx === prog.unlockedMilestone) {
          prog.unlockedMilestone = mIdx + 1;
        }

        prog.streak = (prog.streak || 0) + 1;
        saveProgress(prog);

        const xpGained = arenaState.isBoss ? 150 : (40 + stars * 15 + arenaState.maxCombo * 5);

        if (typeof Gamification !== 'undefined') {
          Gamification.playSound('fanfare');
          Gamification.launchConfetti(75);
          refreshMascots('cheer', `🎉 Xuất sắc! Bạn đã vượt Mốc ${mIdx} và đạt ${stars} ⭐ (+${xpGained} XP)!`);
        }

        let bossLootBtn = '';
        if (arenaState.isBoss) {
          bossLootBtn = `
            <button type="button" class="btn btn-primary" id="btn-arena-boss-harvest" style="background: linear-gradient(135deg, #f59e0b 0%, #ea580c 100%);">
              🎁 Mở Rương Trùm: Thu Hoạch +5 Từ Mới Vĩnh Viễn!
            </button>
          `;
        }

        body.innerHTML = `
          <div class="arena-end-box">
            <div class="arena-end-icon">🏆</div>
            <h3 class="arena-end-title win">VƯỢT MỐC THÀNH CÔNG!</h3>
            <p style="color: var(--text-secondary); font-size: 0.95rem; margin-bottom: 8px;">
              Bạn đã hoàn thành xuất sắc thử thách <strong>Mốc ${mIdx}</strong>!
            </p>
            <div class="arena-stars-earned">
              ${stars >= 1 ? '⭐' : '☆'}${stars >= 2 ? '⭐' : '☆'}${stars >= 3 ? '⭐' : '☆'}
            </div>
            <div>
              <span class="arena-xp-reward-tag">⚡ +${xpGained} XP Thưởng</span>
            </div>
            ${bossLootBtn}
            <div class="arena-end-actions" style="margin-top: 14px;">
              <button type="button" class="btn btn-primary" id="btn-arena-next-milestone">
                🚀 Vượt Mốc Tiếp Theo ➔
              </button>
              <button type="button" class="btn btn-secondary" id="btn-arena-replay">
                🔄 Chơi Lại Cày 3 Sao
              </button>
              <button type="button" class="btn btn-secondary" id="btn-arena-back-map">
                🗺️ Về Bản Đồ
              </button>
            </div>
          </div>
        `;

        const nextBtn = document.getElementById('btn-arena-next-milestone');
        if (nextBtn) {
          nextBtn.addEventListener('click', () => {
            closeArena();
            renderRoadmap();
            startMilestone(mIdx + 1);
          });
        }

        const replayBtn = document.getElementById('btn-arena-replay');
        if (replayBtn) {
          replayBtn.addEventListener('click', () => startMilestone(mIdx));
        }

        const backBtn = document.getElementById('btn-arena-back-map');
        if (backBtn) {
          backBtn.addEventListener('click', () => {
            closeArena();
            renderRoadmap();
          });
        }

        const bossHarvestBtn = document.getElementById('btn-arena-boss-harvest');
        if (bossHarvestBtn) {
          bossHarvestBtn.addEventListener('click', async () => {
            bossHarvestBtn.disabled = true;
            bossHarvestBtn.textContent = '⏳ Đang mở rương báu...';
            try {
              const res = await API.harvestWords({
                languageId: state.activeLanguageId,
                count: 5,
                topicHint: 'random'
              });
              showToast(res.message, 'success');
              if (typeof Gamification !== 'undefined') Gamification.launchConfetti(85);
              await fetchWordsPool();
              buildMilestones();
              renderRoadmap();
              closeArena();
            } catch (e) {
              showToast('Lỗi khi mở rương: ' + e.message, 'error');
            }
          });
        }
      } else {
        // Defeat
        prog.streak = 0;
        saveProgress(prog);

        if (typeof Gamification !== 'undefined') {
          Gamification.playSound('wrong');
          refreshMascots('worried', '💔 Hết tim rồi! Đừng nản lòng, ôn lại từ và phục thù ngay nhé!');
        }

        const wrongItemsHtml = (arenaState.wrongWords || []).map(w => `
          <div style="display: flex; justify-content: space-between; align-items: center; padding: 8px 12px; border-radius: 8px; background: var(--bg-surface-elevated); margin-bottom: 6px; text-align: left; border: 1px solid var(--border-subtle);">
            <div>
              <strong style="color: var(--text-primary); font-family: 'Outfit';">${escapeHtml(w.term)}</strong>
              <span style="color: var(--reading-color); font-size: 0.85rem; margin-left: 6px;">${escapeHtml(w.reading || '')}</span>
            </div>
            <span style="color: var(--meaning-color); font-weight: 700; font-size: 0.9rem;">${escapeHtml(w.meaning)}</span>
          </div>
        `).join('');

        body.innerHTML = `
          <div class="arena-end-box">
            <div class="arena-end-icon">💔</div>
            <h3 class="arena-end-title lose">HẾT TIM RỒI!</h3>
            <p style="color: var(--text-secondary); font-size: 0.95rem; margin-bottom: 12px;">
              Đừng lo! Hãy xem lại các từ bạn vừa chưa nhớ chính xác dưới đây:
            </p>
            <div style="max-height: 180px; overflow-y: auto; margin-bottom: 18px;">
              ${wrongItemsHtml || '<div style="color: var(--text-muted);">Cần phản xạ nhanh hơn trong 10 giây!</div>'}
            </div>
            <div class="arena-end-actions">
              <button type="button" class="btn btn-primary" id="btn-arena-retry-full">
                ❤️ Hồi 3 Tim &amp; Phục Thù Ngay!
              </button>
              <button type="button" class="btn btn-secondary" id="btn-arena-exit-map">
                🗺️ Về Bản Đồ
              </button>
            </div>
          </div>
        `;

        const retryBtn = document.getElementById('btn-arena-retry-full');
        if (retryBtn) retryBtn.addEventListener('click', () => startMilestone(mIdx));

        const exitBtn = document.getElementById('btn-arena-exit-map');
        if (exitBtn) {
          exitBtn.addEventListener('click', () => {
            closeArena();
            renderRoadmap();
          });
        }
      }
    }

    function closeArena() {
      stopTimer();
      const modal = document.getElementById('quest-game-modal');
      if (modal) modal.style.display = 'none';
    }

    // Bind Close & Loot Chest Buttons
    const closeBtn = document.getElementById('btn-close-arena');
    if (closeBtn) closeBtn.addEventListener('click', () => closeArena());

    const lootChestBtn = document.getElementById('btn-quest-loot-chest');
    if (lootChestBtn) {
      lootChestBtn.addEventListener('click', () => {
        const prog = getProgress();
        const totalStars = Object.values(prog.stars || {}).reduce((a, b) => a + b, 0);
        const opened = prog.openedChests || [];
        const required = (opened.length + 1) * 6;

        if (totalStars >= required) {
          opened.push(required);
          prog.openedChests = opened;
          saveProgress(prog);
          if (typeof Gamification !== 'undefined') {
            Gamification.playSound('fanfare');
            Gamification.launchConfetti(90);
          }
          showToast(`🎉 CHÚC MỪNG! Bạn đã mở Rương Báu Mốc thành công và nhận +100 XP cùng danh hiệu dũng sĩ!`, 'success');
        } else {
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          showToast(`🎁 Cần tích thêm ${required - totalStars} ⭐ nữa để mở Rương Báu Mốc này! Hãy vượt tiếp các mốc nhé!`, 'info');
        }
      });
    }

    async function init() {
      await fetchWordsPool();
      buildMilestones();
      renderRoadmap();
    }

    return {
      init,
      renderRoadmap,
      fetchWordsPool,
      buildMilestones,
      startMilestone,
      closeArena
    };
  })();

  // Mode Switcher Listeners (Quest Game Mode vs Classic Grid Mode)
  const btnModeQuest = document.getElementById('btn-mode-quest');
  const btnModeGrid = document.getElementById('btn-mode-grid');
  const questContainer = document.getElementById('words-quest-container');
  const gridContainer = document.getElementById('words-grid-container');

  if (btnModeQuest && btnModeGrid) {
    btnModeQuest.addEventListener('click', () => {
      btnModeQuest.classList.add('active');
      btnModeGrid.classList.remove('active');
      if (questContainer) questContainer.style.display = 'block';
      if (gridContainer) gridContainer.style.display = 'none';
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      MilestoneQuest.renderRoadmap();
    });

    btnModeGrid.addEventListener('click', () => {
      btnModeGrid.classList.add('active');
      btnModeQuest.classList.remove('active');
      if (questContainer) questContainer.style.display = 'none';
      if (gridContainer) gridContainer.style.display = 'block';
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      loadWords();
    });
  }

  // Auto initialize Quest Mode & Vocabulary when view loads or language switches
  MilestoneQuest.init();
  loadWords();

  // =========================================================================
  // 5C. AUTOMATIC MISTAKE TRACKER (SỔ TAY CÂU HỎI & TỪ VỰNG ĐÃ LÀM SAI)
  // =========================================================================
  const MistakeTracker = (() => {
    let activeFilter = 'all'; // 'all' | 'word' | 'question'
    let practiceQueue = [];
    let practiceIndex = 0;
    let practiceFixedCount = 0;

    function getStorageKey() {
      const uid = state.user?.id || 'guest';
      return `polylearn_mistakes_${uid}_lang_${state.activeLanguageId}`;
    }

    function getMistakes() {
      try {
        const raw = localStorage.getItem(getStorageKey());
        if (!raw) return [];
        const parsed = JSON.parse(raw);
        return Array.isArray(parsed) ? parsed : [];
      } catch {
        return [];
      }
    }

    function saveMistakes(list) {
      localStorage.setItem(getStorageKey(), JSON.stringify(list));
      updateBadges(list);
    }

    function recordWordMistake(wordObj, wrongAnswer = '', source = 'Bài tập từ vựng') {
      if (!wordObj || !wordObj.term) return;
      const list = getMistakes();
      const key = `word_${(wordObj.term || '').trim().toLowerCase()}`;
      const existing = list.find(i => i.key === key);
      const nowIso = new Date().toISOString();

      if (existing) {
        existing.wrongCount = (existing.wrongCount || 1) + 1;
        existing.lastWrongAt = nowIso;
        if (wrongAnswer) existing.wrongAnswer = wrongAnswer;
        if (source) existing.source = source;
        if (wordObj.meaning) existing.meaning = wordObj.meaning;
        if (wordObj.reading) existing.reading = wordObj.reading;
      } else {
        list.unshift({
          key,
          type: 'word',
          wordId: typeof wordObj.id === 'number' ? wordObj.id : null,
          term: wordObj.term,
          reading: wordObj.reading || '',
          meaning: wordObj.meaning || '',
          levelCode: wordObj.levelCode || '',
          partOfSpeech: wordObj.partOfSpeech || '',
          wrongAnswer: wrongAnswer || '(Chưa nhớ nghĩa)',
          correctAnswer: wordObj.meaning || '',
          source: source || 'Luyện từ vựng',
          wrongCount: 1,
          lastWrongAt: nowIso
        });
      }

      if (typeof wordObj.id === 'number' && API.getToken()) {
        API.reviewCard(wordObj.id, false).catch(() => {});
      }

      saveMistakes(list);
      render();
    }

    function recordQuestionMistake(qData) {
      if (!qData || !qData.questionText) return;
      const list = getMistakes();
      const normalizedPrompt = qData.questionText.trim().toLowerCase();
      const key = `question_${normalizedPrompt.slice(0, 80)}`;
      const existing = list.find(i => i.key === key);
      const nowIso = new Date().toISOString();

      if (existing) {
        existing.wrongCount = (existing.wrongCount || 1) + 1;
        existing.lastWrongAt = nowIso;
        if (qData.wrongAnswer) existing.wrongAnswer = qData.wrongAnswer;
        if (qData.correctAnswer) existing.correctAnswer = qData.correctAnswer;
        if (qData.explanation) existing.explanation = qData.explanation;
        if (qData.options && qData.options.length > 0) existing.options = qData.options;
        if (qData.source) existing.source = qData.source;
      } else {
        list.unshift({
          key,
          type: 'question',
          questionText: qData.questionText,
          wrongAnswer: qData.wrongAnswer || '(Chưa chọn đúng)',
          correctAnswer: qData.correctAnswer || '',
          explanation: qData.explanation || '',
          options: Array.isArray(qData.options) ? qData.options : [],
          questionType: qData.questionType || 'mcq',
          source: qData.source || 'Bài tập / Kiểm tra',
          wrongCount: 1,
          lastWrongAt: nowIso
        });
      }

      saveMistakes(list);
      render();
    }

    function removeMistake(key, silent = false) {
      const list = getMistakes().filter(i => i.key !== key);
      saveMistakes(list);
      render();
      if (!silent) {
        if (typeof Gamification !== 'undefined') Gamification.playSound('correct');
        showToast('✅ Đã đánh dấu thuộc và xóa khỏi danh sách câu/từ làm sai!', 'success');
      }
    }

    function clearAllMistakes() {
      const list = getMistakes();
      if (list.length === 0) {
        showToast('Danh sách câu & từ làm sai đang trống!', 'info');
        return;
      }
      saveMistakes([]);
      const practiceBox = document.getElementById('mistake-practice-box');
      if (practiceBox) practiceBox.style.display = 'none';
      render();
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
      showToast('🗑️ Đã xóa toàn bộ danh sách câu & từ làm sai!', 'info');
    }

    function updateBadges(list = getMistakes()) {
      const total = list.length;
      const wordCount = list.filter(i => i.type === 'word').length;
      const questionCount = list.filter(i => i.type === 'question').length;

      const tabBadge = document.getElementById('mistakes-tab-badge');
      const sideBadge = document.getElementById('side-mistakes-badge');
      const dockBadge = document.getElementById('dock-mistakes-badge');
      const dashTileCount = document.getElementById('dash-mistakes-tile-count');
      const summaryBadge = document.getElementById('mistakes-summary-badge');
      const mAll = document.getElementById('mcount-all');
      const mWord = document.getElementById('mcount-word');
      const mQues = document.getElementById('mcount-question');

      if (tabBadge) tabBadge.textContent = total;
      if (sideBadge) sideBadge.textContent = total;
      if (dockBadge) dockBadge.textContent = total;
      if (dashTileCount) dashTileCount.textContent = total;
      if (summaryBadge) summaryBadge.textContent = `${total} mục cần ôn lại (${wordCount} từ • ${questionCount} câu)`;
      if (mAll) mAll.textContent = total;
      if (mWord) mWord.textContent = wordCount;
      if (mQues) mQues.textContent = questionCount;

      const alertBanner = document.getElementById('roadmap-mistakes-alert');
      const alertTitle = document.getElementById('roadmap-mistakes-alert-title');
      if (alertBanner) {
        if (total > 0 && currentUnifiedSubtab !== 'mistakes') {
          alertBanner.style.display = 'flex';
          if (alertTitle) {
            alertTitle.innerHTML = `Bạn đang có <strong style="color:#dc2626;">${total} mục làm sai</strong> (${wordCount} từ vựng, ${questionCount} câu hỏi) đã được lưu tự động!`;
          }
        } else {
          alertBanner.style.display = 'none';
        }
      }
    }

    function render() {
      const list = getMistakes();
      updateBadges(list);

      const grid = document.getElementById('mistakes-list-grid');
      if (!grid) return;

      const filtered = activeFilter === 'all'
        ? list
        : list.filter(i => i.type === activeFilter);

      if (filtered.length === 0) {
        grid.innerHTML = `
          <div class="glass-card" style="grid-column: 1 / -1; text-align: center; padding: 36px 20px;">
            <div style="font-size: 2.8rem; margin-bottom: 10px;">🎉</div>
            <div style="font-family: 'Outfit'; font-size: 1.25rem; font-weight: 800; color: var(--text-primary); margin-bottom: 6px;">
              ${list.length === 0 ? 'Tuyệt vời! Bạn chưa có câu hỏi hay từ vựng nào làm sai!' : 'Không có mục nào trong bộ lọc này.'}
            </div>
            <p style="font-size: 0.9rem; color: var(--text-secondary); max-width: 560px; margin: 0 auto;">
              Khi bạn làm bài trong <strong>Lộ trình vượt ải</strong>, <strong>Đề kiểm tra</strong> hoặc <strong>Đấu trường từ vựng</strong> mà trả lời sai câu nào hay từ nào, hệ thống sẽ tự động lưu vào đây để bạn ôn lại!
            </p>
          </div>
        `;
        return;
      }

      grid.innerHTML = '';
      filtered.forEach(item => {
        const card = document.createElement('div');
        card.className = `mistake-card-item ${item.type === 'word' ? 'is-word' : 'is-question'}`;

        const isWord = item.type === 'word';
        const timeStr = item.lastWrongAt
          ? new Date(item.lastWrongAt).toLocaleString('vi-VN', { hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit' })
          : '';

        if (isWord) {
          card.innerHTML = `
            <div>
              <div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; margin-bottom: 8px; flex-wrap: wrap;">
                <div style="display: flex; align-items: center; gap: 6px; flex-wrap: wrap;">
                  <span class="card-tag" style="background: rgba(245, 158, 11, 0.16); color: #d97706;">🔤 Từ vựng sai</span>
                  <span class="mini-stat-pill" style="color: #dc2626; font-weight: 800;">Sai ${item.wrongCount || 1} lần</span>
                  ${item.source ? `<span class="mini-stat-pill">${escapeHtml(item.source)}</span>` : ''}
                </div>
                <button type="button" class="speaker-btn btn-mistake-speak" data-text="${escapeHtml(item.term)}" style="width: 32px; height: 32px; font-size: 0.85rem;" title="Nghe phát âm">🔊</button>
              </div>

              <div style="font-family: 'Outfit'; font-size: 1.65rem; font-weight: 800; color: var(--text-primary); line-height: 1.2;">
                ${escapeHtml(item.term)}
              </div>
              ${item.reading ? `<div style="color: var(--reading-color); font-size: 0.92rem; font-weight: 600; margin-top: 2px;">${escapeHtml(item.reading)}</div>` : ''}
              <div style="margin-top: 8px; padding: 8px 12px; border-radius: 10px; background: rgba(16, 185, 129, 0.1); border: 1px solid rgba(16, 185, 129, 0.25);">
                <span style="font-size: 0.78rem; font-weight: 700; color: var(--text-muted); text-transform: uppercase;">✓ Nghĩa chuẩn:</span>
                <div style="font-size: 1.05rem; font-weight: 800; color: var(--success-text); margin-top: 2px;">${escapeHtml(item.meaning || item.correctAnswer)}</div>
              </div>
              ${item.wrongAnswer ? `
                <div style="font-size: 0.83rem; color: #dc2626; font-weight: 600; margin-top: 6px;">
                  ✗ Lần trước chọn nhầm: ${escapeHtml(item.wrongAnswer)}
                </div>
              ` : ''}
            </div>

            <div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; margin-top: 14px; padding-top: 10px; border-top: 1px solid var(--border-subtle); flex-wrap: wrap;">
              <span style="font-size: 0.75rem; color: var(--text-muted);">🕒 ${escapeHtml(timeStr)}</span>
              <div style="display: flex; gap: 6px;">
                <button type="button" class="btn btn-secondary btn-practice-single" data-key="${escapeHtml(item.key)}" style="padding: 5px 11px; font-size: 0.78rem; font-weight: 700;">
                  🎯 Test lại
                </button>
                <button type="button" class="btn btn-primary btn-resolve-mistake" data-key="${escapeHtml(item.key)}" style="padding: 5px 11px; font-size: 0.78rem; background: var(--success);">
                  ✅ Đã thuộc
                </button>
              </div>
            </div>
          `;
        } else {
          card.innerHTML = `
            <div>
              <div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; margin-bottom: 8px; flex-wrap: wrap;">
                <div style="display: flex; align-items: center; gap: 6px; flex-wrap: wrap;">
                  <span class="card-tag" style="background: rgba(239, 68, 68, 0.14); color: #dc2626;">❓ Câu hỏi sai</span>
                  <span class="mini-stat-pill" style="color: #dc2626; font-weight: 800;">Sai ${item.wrongCount || 1} lần</span>
                  ${item.source ? `<span class="mini-stat-pill">${escapeHtml(item.source)}</span>` : ''}
                </div>
                <button type="button" class="speaker-btn btn-mistake-speak" data-text="${escapeHtml(item.correctAnswer || item.questionText)}" style="width: 32px; height: 32px; font-size: 0.85rem;" title="Nghe đọc">🔊</button>
              </div>

              <div style="font-weight: 800; font-size: 1.02rem; color: var(--text-primary); line-height: 1.4; margin-bottom: 8px;">
                ${escapeHtml(item.questionText)}
              </div>

              <div style="font-size: 0.86rem; color: #dc2626; font-weight: 700; margin-bottom: 4px;">
                ✗ Bạn đã trả lời: ${escapeHtml(item.wrongAnswer || '(Chưa chọn)')}
              </div>

              <div style="padding: 8px 12px; border-radius: 10px; background: rgba(16, 185, 129, 0.1); border: 1px solid rgba(16, 185, 129, 0.25); font-size: 0.92rem; font-weight: 800; color: var(--success-text);">
                ✓ Đáp án đúng: ${escapeHtml(item.correctAnswer || '(Xem giải thích)')}
              </div>

              ${item.explanation ? `
                <div style="font-size: 0.82rem; color: var(--text-secondary); margin-top: 8px; line-height: 1.4;">
                  💡 ${escapeHtml(item.explanation)}
                </div>
              ` : ''}
            </div>

            <div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; margin-top: 14px; padding-top: 10px; border-top: 1px solid var(--border-subtle); flex-wrap: wrap;">
              <span style="font-size: 0.75rem; color: var(--text-muted);">🕒 ${escapeHtml(timeStr)}</span>
              <div style="display: flex; gap: 6px;">
                <button type="button" class="btn btn-secondary btn-practice-single" data-key="${escapeHtml(item.key)}" style="padding: 5px 11px; font-size: 0.78rem; font-weight: 700;">
                  🎯 Làm lại câu này
                </button>
                <button type="button" class="btn btn-primary btn-resolve-mistake" data-key="${escapeHtml(item.key)}" style="padding: 5px 11px; font-size: 0.78rem; background: var(--success);">
                  ✅ Đã thuộc
                </button>
              </div>
            </div>
          `;
        }

        const speakBtn = card.querySelector('.btn-mistake-speak');
        if (speakBtn) {
          speakBtn.addEventListener('click', () => speak(speakBtn.dataset.text));
        }

        const practiceSingleBtn = card.querySelector('.btn-practice-single');
        if (practiceSingleBtn) {
          practiceSingleBtn.addEventListener('click', () => startPractice(item.key));
        }

        const resolveBtn = card.querySelector('.btn-resolve-mistake');
        if (resolveBtn) {
          resolveBtn.addEventListener('click', () => removeMistake(item.key));
        }

        grid.appendChild(card);
      });
    }

    function startPractice(singleKey = null) {
      const all = getMistakes();
      if (all.length === 0) {
        showToast('🎉 Bạn không có câu hỏi hay từ vựng nào đang làm sai!', 'success');
        return;
      }

      if (singleKey) {
        const target = all.find(i => i.key === singleKey);
        practiceQueue = target ? [target] : [...all];
      } else {
        practiceQueue = activeFilter === 'all'
          ? [...all]
          : all.filter(i => i.type === activeFilter);
        if (practiceQueue.length === 0) practiceQueue = [...all];
      }

      practiceIndex = 0;
      practiceFixedCount = 0;

      const practiceBox = document.getElementById('mistake-practice-box');
      if (!practiceBox) return;
      practiceBox.style.display = 'block';
      practiceBox.scrollIntoView({ behavior: 'smooth', block: 'center' });
      renderPracticeStep();
    }

    function renderPracticeStep() {
      const practiceBox = document.getElementById('mistake-practice-box');
      if (!practiceBox) return;

      if (practiceIndex >= practiceQueue.length) {
        if (typeof Gamification !== 'undefined') {
          Gamification.playSound('fanfare');
          Gamification.launchConfetti(60);
        }
        practiceBox.innerHTML = `
          <div style="text-align: center; padding: 12px;">
            <div style="font-size: 2.6rem; margin-bottom: 6px;">🏆</div>
            <h3 style="font-family: 'Outfit'; font-size: 1.45rem; margin: 0 0 6px; color: var(--text-primary);">
              Hoàn Thành Ôn Lại Lỗi Sai!
            </h3>
            <p style="color: var(--text-secondary); font-size: 0.92rem; margin-bottom: 14px;">
              Bạn đã trả lời đúng <strong>${practiceFixedCount} / ${practiceQueue.length}</strong> mục trong lượt ôn này!
            </p>
            <div style="display: flex; justify-content: center; gap: 10px; flex-wrap: wrap;">
              ${getMistakes().length > 0 ? `<button type="button" class="btn btn-primary" id="btn-retry-remaining-mistakes">🔄 Ôn tiếp các mục còn lại (${getMistakes().length})</button>` : ''}
              <button type="button" class="btn btn-secondary" id="btn-close-mistake-practice">Đóng khung luyện tập</button>
            </div>
          </div>
        `;
        const retryRem = document.getElementById('btn-retry-remaining-mistakes');
        if (retryRem) retryRem.addEventListener('click', () => startPractice());
        const closeBtn = document.getElementById('btn-close-mistake-practice');
        if (closeBtn) closeBtn.addEventListener('click', () => { practiceBox.style.display = 'none'; });
        return;
      }

      const item = practiceQueue[practiceIndex];
      const allMistakes = getMistakes();
      const isWord = item.type === 'word';

      // Build 4 multiple-choice options for fast, engaging review
      const correctAns = (isWord ? item.meaning : item.correctAnswer) || '';
      let optionsPool = [];

      if (!isWord && Array.isArray(item.options) && item.options.length >= 2) {
        optionsPool = [...item.options];
      } else {
        const otherAnswers = allMistakes
          .map(m => m.type === 'word' ? m.meaning : m.correctAnswer)
          .filter(a => a && a !== correctAns);
        const fallbackDistractors = [
          item.wrongAnswer && item.wrongAnswer !== correctAns && !item.wrongAnswer.startsWith('(') ? item.wrongAnswer : null,
          ...otherAnswers,
          'Xin chào / Giao tiếp cơ bản',
          'Cảm ơn / Lịch sự',
          'Thời gian & Địa điểm',
          'Công việc & Học tập'
        ].filter(Boolean);

        const uniqueDistractors = [...new Set(fallbackDistractors)].filter(x => x !== correctAns).slice(0, 3);
        optionsPool = [correctAns, ...uniqueDistractors].sort(() => 0.5 - Math.random());
      }

      practiceBox.innerHTML = `
        <div style="display: flex; justify-content: space-between; align-items: center; gap: 8px; margin-bottom: 12px; flex-wrap: wrap;">
          <span class="card-tag" style="background: rgba(239, 68, 68, 0.15); color: #dc2626; font-weight: 800;">
            🔥 Ôn Lại Lỗi Sai • Mục ${practiceIndex + 1} / ${practiceQueue.length}
          </span>
          <button type="button" class="btn btn-secondary" id="btn-quit-mistake-practice" style="padding: 4px 10px; font-size: 0.78rem;">✕ Đóng</button>
        </div>

        <div style="margin-bottom: 14px;">
          ${isWord ? `
            <div style="font-size: 0.82rem; font-weight: 700; color: var(--text-muted); text-transform: uppercase;">Chọn nghĩa đúng của từ vựng:</div>
            <div style="display: flex; align-items: center; gap: 10px; margin-top: 4px;">
              <span style="font-family: 'Outfit'; font-size: 2rem; font-weight: 800; color: var(--text-primary);">${escapeHtml(item.term)}</span>
              ${item.reading ? `<span style="color: var(--reading-color); font-weight: 600;">[${escapeHtml(item.reading)}]</span>` : ''}
              <button type="button" class="speaker-btn" id="btn-practice-speak" style="width: 32px; height: 32px; font-size: 0.85rem;">🔊</button>
            </div>
          ` : `
            <div style="font-size: 0.82rem; font-weight: 700; color: var(--text-muted); text-transform: uppercase;">Trả lời lại câu hỏi bạn từng làm sai:</div>
            <div style="font-size: 1.15rem; font-weight: 800; color: var(--text-primary); margin-top: 4px;">${escapeHtml(item.questionText)}</div>
          `}
        </div>

        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(220px, 1fr)); gap: 10px;" id="mistake-practice-options">
          ${optionsPool.map(opt => `
            <button type="button" class="lesson-option-choice mistake-opt-btn" data-val="${escapeHtml(opt)}" style="text-align: left; padding: 12px 14px; font-weight: 700;">
              ${escapeHtml(opt)}
            </button>
          `).join('')}
        </div>

        <div id="mistake-practice-feedback" style="display: none; margin-top: 14px; padding: 12px 14px; border-radius: 10px;"></div>
      `;

      const quitBtn = document.getElementById('btn-quit-mistake-practice');
      if (quitBtn) quitBtn.addEventListener('click', () => { practiceBox.style.display = 'none'; });

      const spkBtn = document.getElementById('btn-practice-speak');
      if (spkBtn && item.term) {
        spkBtn.addEventListener('click', () => speak(item.term));
        speak(item.term);
      }

      const autoRemoveEl = document.getElementById('mistake-auto-remove-toggle');
      const shouldAutoRemove = autoRemoveEl ? autoRemoveEl.checked : true;

      practiceBox.querySelectorAll('.mistake-opt-btn').forEach(btn => {
        btn.addEventListener('click', () => {
          practiceBox.querySelectorAll('.mistake-opt-btn').forEach(b => b.disabled = true);
          const chosen = btn.dataset.val;
          const isRight = chosen === correctAns;
          const fb = document.getElementById('mistake-practice-feedback');

          if (isRight) {
            btn.classList.add('correct');
            practiceFixedCount++;
            if (typeof Gamification !== 'undefined') {
              Gamification.playSound('correct');
              Gamification.spawnFloatingXp(10, btn);
            }
            if (shouldAutoRemove) {
              removeMistake(item.key, true);
            }
            if (fb) {
              fb.style.display = 'block';
              fb.style.background = 'rgba(16, 185, 129, 0.12)';
              fb.style.border = '1px solid rgba(16, 185, 129, 0.35)';
              fb.innerHTML = `
                <div style="font-weight: 800; color: var(--success-text);">
                  🎉 Chính xác! +10 XP ${shouldAutoRemove ? '• Đã xóa mục này khỏi Sổ tay lỗi sai!' : ''}
                </div>
                ${item.explanation ? `<div style="font-size: 0.85rem; color: var(--text-secondary); margin-top: 4px;">💡 ${escapeHtml(item.explanation)}</div>` : ''}
                <div style="margin-top: 10px;">
                  <button type="button" class="btn btn-primary" id="btn-next-mistake-step" style="padding: 7px 16px; font-size: 0.85rem;">Tiếp tục ➔</button>
                </div>
              `;
            }
          } else {
            btn.classList.add('wrong');
            practiceBox.querySelectorAll('.mistake-opt-btn').forEach(b => {
              if (b.dataset.val === correctAns) b.classList.add('correct');
            });
            if (typeof Gamification !== 'undefined') Gamification.playSound('wrong');
            if (fb) {
              fb.style.display = 'block';
              fb.style.background = 'rgba(239, 68, 68, 0.1)';
              fb.style.border = '1px solid rgba(239, 68, 68, 0.35)';
              fb.innerHTML = `
                <div style="font-weight: 800; color: #dc2626;">
                  ✗ Chưa chính xác! Đáp án đúng là: "${escapeHtml(correctAns)}"
                </div>
                ${item.explanation ? `<div style="font-size: 0.85rem; color: var(--text-secondary); margin-top: 4px;">💡 ${escapeHtml(item.explanation)}</div>` : ''}
                <div style="margin-top: 10px;">
                  <button type="button" class="btn btn-primary" id="btn-next-mistake-step" style="padding: 7px 16px; font-size: 0.85rem;">Câu tiếp theo ➔</button>
                </div>
              `;
            }
          }

          const nextStepBtn = document.getElementById('btn-next-mistake-step');
          if (nextStepBtn) {
            nextStepBtn.addEventListener('click', () => {
              practiceIndex++;
              renderPracticeStep();
            });
          }
        });
      });
    }

    function init() {
      document.querySelectorAll('.mistake-filter-btn').forEach(btn => {
        btn.addEventListener('click', () => {
          document.querySelectorAll('.mistake-filter-btn').forEach(b => b.classList.remove('active'));
          btn.classList.add('active');
          activeFilter = btn.dataset.mfilter || 'all';
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          render();
        });
      });

      const btnStartPractice = document.getElementById('btn-start-mistake-practice');
      if (btnStartPractice) {
        btnStartPractice.addEventListener('click', () => {
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          startPractice();
        });
      }

      const btnClearAll = document.getElementById('btn-clear-all-mistakes');
      if (btnClearAll) {
        btnClearAll.addEventListener('click', () => clearAllMistakes());
      }

      const btnAlertPractice = document.getElementById('btn-alert-practice-mistakes');
      if (btnAlertPractice) {
        btnAlertPractice.addEventListener('click', () => {
          switchUnifiedLessonsSubtab('mistakes', true);
          startPractice();
        });
      }

      const btnAlertView = document.getElementById('btn-alert-view-mistakes');
      if (btnAlertView) {
        btnAlertView.addEventListener('click', () => {
          switchUnifiedLessonsSubtab('mistakes', true);
        });
      }

      render();
    }

    return {
      init,
      render,
      updateBadges,
      getMistakes,
      recordWordMistake,
      recordQuestionMistake,
      removeMistake,
      clearAllMistakes,
      startPractice
    };
  })();

  MistakeTracker.init();

  // ---------- 6. AI IMMERSION STUDIO ----------
  const btnCheckGrammar = document.getElementById('btn-check-grammar');
  const aiGrammarInput = document.getElementById('ai-grammar-input');
  const aiGrammarResult = document.getElementById('ai-grammar-result');
  const aiGrammarSamples = document.getElementById('ai-grammar-samples');
  const aiExplainTermInput = document.getElementById('ai-explain-term-input');
  const btnAiExplainTerm = document.getElementById('btn-ai-explain-term');
  const aiExplainTermResult = document.getElementById('ai-explain-term-result');
  const aiExplainSamples = document.getElementById('ai-explain-samples');
  const btnSendChat = document.getElementById('btn-send-chat');
  const chatUserInput = document.getElementById('chat-user-input');
  const chatMessagesContainer = document.getElementById('chat-messages-container');
  const aiScenarioSelect = document.getElementById('ai-scenario-select');
  const aiScenarioMission = document.getElementById('ai-scenario-mission');
  const aiStarterChips = document.getElementById('ai-starter-chips');
  const btnResetChat = document.getElementById('btn-reset-chat');

  const SCENARIO_MISSIONS = {
    casual_talk: '🎯 <strong>Nhiệm vụ tình huống:</strong> Trò chuyện tự nhiên về sở thích, công việc và kế hoạch cuối tuần.',
    restaurant: '🎯 <strong>Nhiệm vụ tình huống:</strong> Đặt bàn, hỏi món đặc sản, yêu cầu khẩu vị riêng và thanh toán hóa đơn.',
    airport: '🎯 <strong>Nhiệm vụ tình huống:</strong> Làm thủ tục check-in chuyến bay, ký gửi hành lý và hỏi cổng lên máy bay.',
    shopping: '🎯 <strong>Nhiệm vụ tình huống:</strong> Hỏi kích cỡ, màu sắc, thử đồ và thương lượng ưu đãi giảm giá.',
    job_interview: '🎯 <strong>Nhiệm vụ tình huống:</strong> Giới thiệu kinh nghiệm bản thân, điểm mạnh và đặt câu hỏi cho nhà tuyển dụng.'
  };

  function renderWordExplainHtml(res) {
    return `
      <div>
        <div style="display: flex; align-items: baseline; gap: 8px; flex-wrap: wrap; margin-bottom: 4px;">
          <span style="font-family: 'Outfit'; font-weight: 800; font-size: 1.15rem; color: var(--text-primary);">${escapeHtml(res.term)}</span>
          ${res.reading ? `<span style="color: var(--reading-color); font-size: 0.88rem; font-weight: 600;">${escapeHtml(res.reading)}</span>` : ''}
          <span style="color: var(--meaning-color); font-weight: 700; font-size: 0.95rem;">— ${escapeHtml(res.meaning)}</span>
        </div>
        <p style="font-size: 0.88rem; color: var(--text-secondary); margin-bottom: 10px;">${escapeHtml(res.detailedExplanation || '')}</p>
        ${res.commonCollocations && res.commonCollocations.length > 0 ? `
          <div style="display: flex; gap: 6px; flex-wrap: wrap; margin-bottom: 10px;">
            ${res.commonCollocations.map(c => `<span class="mini-stat-pill">💡 ${escapeHtml(c)}</span>`).join('')}
          </div>
        ` : ''}
        ${res.sampleSentences && res.sampleSentences.length > 0 ? `
          <div style="display: flex; flex-direction: column; gap: 6px;">
            ${res.sampleSentences.slice(0, 3).map(s => `
              <div style="padding: 8px 10px; border-radius: 8px; background: var(--bg-surface); border: 1px solid var(--border-subtle); font-size: 0.84rem;">
                <div style="font-weight: 600; color: var(--text-primary);">"${escapeHtml(s.sentence)}"</div>
                ${s.reading ? `<div style="color: var(--reading-color); font-size: 0.78rem;">${escapeHtml(s.reading)}</div>` : ''}
                <div style="color: var(--success-text); font-weight: 600;">${escapeHtml(s.translation || '')}</div>
              </div>
            `).join('')}
          </div>
        ` : ''}
      </div>
    `;
  }

  function renderAiLanguageSamples() {
    const langCfg = state.languages[state.activeLanguageId] || state.languages[1];
    const scenario = aiScenarioSelect ? aiScenarioSelect.value : 'casual_talk';

    if (aiGrammarSamples) {
      aiGrammarSamples.innerHTML = (langCfg.grammarSamples || []).map((s, idx) => `
        <button type="button" class="sample-chip" data-sample="${escapeHtml(s)}">Mẫu ${idx + 1}: ${escapeHtml(s.slice(0, 28))}${s.length > 28 ? '…' : ''}</button>
      `).join('');
      aiGrammarSamples.querySelectorAll('.sample-chip').forEach(chip => {
        chip.addEventListener('click', () => {
          aiGrammarInput.value = chip.dataset.sample;
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
        });
      });
    }

    if (aiExplainSamples) {
      aiExplainSamples.innerHTML = (langCfg.explainSamples || []).map(term => `
        <button type="button" class="sample-chip" data-term="${escapeHtml(term)}">✨ ${escapeHtml(term)}</button>
      `).join('');
      aiExplainSamples.querySelectorAll('.sample-chip').forEach(chip => {
        chip.addEventListener('click', () => {
          aiExplainTermInput.value = chip.dataset.term;
          runAiTermExplain();
        });
      });
    }

    if (aiScenarioMission) {
      aiScenarioMission.innerHTML = SCENARIO_MISSIONS[scenario] || SCENARIO_MISSIONS.casual_talk;
    }

    if (aiStarterChips) {
      const starters = (langCfg.starters && langCfg.starters[scenario]) || (langCfg.starters && langCfg.starters.casual_talk) || [];
      aiStarterChips.innerHTML = starters.map(st => `
        <button type="button" class="sample-chip" data-starter="${escapeHtml(st)}">💬 "${escapeHtml(st)}"</button>
      `).join('');
      aiStarterChips.querySelectorAll('.sample-chip').forEach(chip => {
        chip.addEventListener('click', () => {
          chatUserInput.value = chip.dataset.starter;
          sendChatMessage();
        });
      });
    }
  }

  if (aiScenarioSelect) {
    aiScenarioSelect.addEventListener('change', () => {
      renderAiLanguageSamples();
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
    });
  }

  if (btnResetChat) {
    btnResetChat.addEventListener('click', () => {
      state.chatHistory = [];
      chatMessagesContainer.innerHTML = `
        <div class="chat-msg assistant">
          Đã làm mới tình huống hội thoại! Hãy bấm một câu gợi ý phía trên hoặc nhập lời chào của bạn nhé! ✨
        </div>
      `;
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
    });
  }

  async function runAiTermExplain() {
    const term = aiExplainTermInput ? aiExplainTermInput.value.trim() : '';
    if (!term) {
      showToast('Vui lòng nhập từ hoặc cụm từ cần giải thích!', 'error');
      return;
    }
    if (!API.getToken()) {
      showToast('Vui lòng đăng nhập để sử dụng Trợ lý AI!', 'info');
      openAuthModal();
      return;
    }

    btnAiExplainTerm.disabled = true;
    btnAiExplainTerm.textContent = '⏳ Đang tra...';
    aiExplainTermResult.style.display = 'block';
    aiExplainTermResult.innerHTML = `<div style="color: var(--text-muted); font-size: 0.88rem;">🤖 Đang phân tích sắc thái và ví dụ cho "${escapeHtml(term)}"...</div>`;

    try {
      const langCode = state.languages[state.activeLanguageId]?.code || 'en';
      const res = await API.explainWord(term, langCode);
      aiExplainTermResult.innerHTML = `
        <div style="background: var(--bg-surface-elevated); border: 1px solid var(--border-highlight); border-radius: var(--radius-md); padding: 14px 16px;">
          ${renderWordExplainHtml(res)}
        </div>
      `;
    } catch (err) {
      aiExplainTermResult.innerHTML = `<div style="color: var(--danger);">${escapeHtml(err.message)}</div>`;
    } finally {
      btnAiExplainTerm.disabled = false;
      btnAiExplainTerm.textContent = '🔍 Phân tích';
    }
  }

  if (btnAiExplainTerm) {
    btnAiExplainTerm.addEventListener('click', runAiTermExplain);
  }
  if (aiExplainTermInput) {
    aiExplainTermInput.addEventListener('keydown', (e) => {
      if (e.key === 'Enter') runAiTermExplain();
    });
  }

  btnCheckGrammar.addEventListener('click', async () => {
    const text = aiGrammarInput.value.trim();
    if (!text) {
      showToast('Vui lòng nhập câu cần kiểm tra', 'error');
      return;
    }

    if (!API.getToken()) {
      showToast('Vui lòng đăng nhập để sử dụng Trợ lý AI!', 'info');
      openAuthModal();
      return;
    }

    btnCheckGrammar.disabled = true;
    btnCheckGrammar.textContent = '⏳ Đang phân tích...';
    aiGrammarResult.style.display = 'none';

    try {
      const langCode = state.languages[state.activeLanguageId]?.code || 'en';
      const res = await API.checkGrammar(text, langCode);

      aiGrammarResult.style.display = 'block';
      aiGrammarResult.innerHTML = `
        <div style="background: var(--bg-surface-elevated); border: 1px solid var(--border-highlight); border-radius: var(--radius-md); padding: 16px;">
          <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 6px;">
            <span style="font-weight: 700; color: ${res.isCorrect ? 'var(--success-text)' : 'var(--meaning-color)'}; font-size: 1.05rem;">
              ${res.isCorrect ? '✓ Câu chuẩn ngữ pháp tự nhiên' : '💡 Câu gợi ý sửa chuẩn:'}
            </span>
            <button type="button" class="speaker-btn" id="btn-speak-corrected" style="width: 30px; height: 30px; font-size: 0.85rem;">🔊</button>
          </div>
          <div style="font-size: 1.18rem; font-weight: 700; color: var(--text-primary); margin-bottom: 8px;">
            "${escapeHtml(res.correctedText)}"
          </div>
          <p style="color: var(--text-secondary); font-size: 0.92rem; margin-bottom: 10px;">${escapeHtml(res.explanation)}</p>
          ${res.grammarPoints && res.grammarPoints.length > 0 ? `
            <div style="display: flex; gap: 6px; flex-wrap: wrap; margin-bottom: 8px;">
              ${res.grammarPoints.map(gp => `<span class="mini-stat-pill">📌 ${escapeHtml(gp)}</span>`).join('')}
            </div>
          ` : ''}
          ${res.suggestions && res.suggestions.length > 0 ? `
            <div style="font-weight: 700; font-size: 0.84rem; color: var(--accent-primary); margin-top: 8px;">Cách diễn đạt tự nhiên khác:</div>
            <ul style="padding-left: 20px; font-size: 0.88rem; color: var(--text-secondary); margin-top: 4px;">
              ${res.suggestions.map(s => `<li>${escapeHtml(s)}</li>`).join('')}
            </ul>
          ` : ''}
        </div>
      `;
      const spkBtn = document.getElementById('btn-speak-corrected');
      if (spkBtn) spkBtn.addEventListener('click', () => speak(res.correctedText));
    } catch (err) {
      showToast(err.message, 'error');
    } finally {
      btnCheckGrammar.disabled = false;
      btnCheckGrammar.textContent = '✨ Kiểm tra & Sửa ngữ pháp';
    }
  });

  async function sendChatMessage() {
    const text = chatUserInput.value.trim();
    if (!text) return;

    if (!API.getToken()) {
      showToast('Vui lòng đăng nhập để trò chuyện cùng AI!', 'info');
      openAuthModal();
      return;
    }

    state.chatHistory.push({ role: 'user', content: text });
    appendChatMessage('user', text);
    chatUserInput.value = '';

    btnSendChat.disabled = true;

    try {
      const langCode = state.languages[state.activeLanguageId]?.code || 'en';
      const scenario = aiScenarioSelect.value;
      const res = await API.chatAi(state.chatHistory, langCode, scenario);

      state.chatHistory.push({ role: 'assistant', content: res.reply });
      appendChatMessage('assistant', res.reply, res.reading, res.vietnameseTranslation, res.suggestedReplies);
    } catch (err) {
      showToast(err.message, 'error');
    } finally {
      btnSendChat.disabled = false;
    }
  }

  function appendChatMessage(role, text, reading = null, translation = null, suggestedReplies = []) {
    const msg = document.createElement('div');
    msg.className = `chat-msg ${role}`;

    if (role === 'user') {
      msg.textContent = text;
    } else {
      let html = `
        <div style="display: flex; justify-content: space-between; align-items: flex-start; gap: 10px;">
          <div style="font-weight: 600; font-size: 1.02rem; flex: 1;">${escapeHtml(text)}</div>
          <button type="button" class="speaker-btn btn-chat-speak" style="width: 28px; height: 28px; font-size: 0.8rem; flex-shrink: 0;" title="Nghe câu thoại AI">🔊</button>
        </div>
      `;
      if (reading) html += `<div style="color: var(--reading-color); font-size: 0.84rem; margin-top: 2px;">${escapeHtml(reading)}</div>`;
      if (translation) html += `<div style="color: var(--text-muted); font-size: 0.84rem; margin-top: 4px;">(${escapeHtml(translation)})</div>`;

      if (suggestedReplies && suggestedReplies.length > 0) {
        html += `<div style="display: flex; gap: 6px; flex-wrap: wrap; margin-top: 10px;">`;
        suggestedReplies.forEach(r => {
          html += `<button class="btn btn-secondary btn-reply-suggestion" style="padding: 4px 10px; font-size: 0.75rem;">${escapeHtml(r)}</button>`;
        });
        html += `</div>`;
      }
      msg.innerHTML = html;

      const spk = msg.querySelector('.btn-chat-speak');
      if (spk) spk.addEventListener('click', () => speak(text));

      msg.querySelectorAll('.btn-reply-suggestion').forEach(b => {
        b.addEventListener('click', () => {
          chatUserInput.value = b.textContent;
          sendChatMessage();
        });
      });
    }

    chatMessagesContainer.appendChild(msg);
    chatMessagesContainer.scrollTop = chatMessagesContainer.scrollHeight;
  }

  btnSendChat.addEventListener('click', sendChatMessage);
  chatUserInput.addEventListener('keydown', (e) => {
    if (e.key === 'Enter') sendChatMessage();
  });
  renderAiLanguageSamples();

  // Global Keyboard Shortcuts for Exam Arena
  document.addEventListener('keydown', (e) => {
    const activeTag = document.activeElement?.tagName;
    if (activeTag === 'INPUT' || activeTag === 'TEXTAREA' || activeTag === 'SELECT') return;
    if (document.querySelector('.modal-overlay.active')) return;

    if (state.currentView === 'quizzes' && quizPlayingBox.style.display === 'block') {
      const curQ = state.quizSession?.questions?.[state.quizCurrentQuestionIndex];
      if (!curQ || curQ.questionType === 'written') return;

      const idx = ['1', '2', '3', '4'].indexOf(e.key);
      if (idx >= 0) {
        const optEls = document.querySelectorAll('#quiz-options-list .quiz-option');
        if (optEls[idx]) optEls[idx].click();
      } else if (e.key === 'Enter') {
        btnNextQuestion.click();
      }
    }
  });

  // ---------- 5.5 PERSONAL PROFILE (TRANG CÁ NHÂN) ----------
  const profileGuestBox = document.getElementById('profile-guest-box');
  const profileContentBox = document.getElementById('profile-content-box');
  const btnProfileOpenAuth = document.getElementById('btn-profile-open-auth');
  const btnSaveCharacter = document.getElementById('btn-save-character');
  const profileForm = document.getElementById('profile-form');
  const profilePasswordForm = document.getElementById('profile-password-form');

  if (btnProfileOpenAuth) {
    btnProfileOpenAuth.addEventListener('click', openAuthModal);
  }

  document.querySelectorAll('#profile-goal-presets .goal-preset-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('#profile-goal-presets .goal-preset-btn').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      state.selectedDailyGoalXp = parseInt(btn.dataset.goal, 10) || 50;
      if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
    });
  });

  function renderProfileBannerAndStats(profile) {
    if (!profile) return;

    const bannerAvatar = document.getElementById('profile-banner-avatar');
    if (bannerAvatar && typeof Gamification !== 'undefined') {
      bannerAvatar.innerHTML = Gamification.getAvatarSvg(profile.avatarKey || 'owl');
    }

    document.getElementById('profile-banner-level').textContent = `Lv.${profile.level || 1}`;
    document.getElementById('profile-banner-name').textContent = profile.displayName || 'Học viên';
    document.getElementById('profile-banner-rank').textContent = profile.rankTitle || '🌱 Tân Binh Ngôn Ngữ';
    document.getElementById('profile-banner-bio').textContent =
      profile.bio ? `"${profile.bio}"` : '"Mỗi ngày tiến bộ 1% cùng PolyLearn AI!"';

    const joinedStr = profile.createdAt ? new Date(profile.createdAt).toLocaleDateString('vi-VN') : '--/--/----';
    const prefLang = profile.preferredLanguageName || 'Đa ngôn ngữ';
    document.getElementById('profile-banner-meta').innerHTML = `
      <span>📧 ${escapeHtml(profile.email)}</span>
      <span>🌍 Ưu tiên: ${escapeHtml(prefLang)}</span>
      <span>📅 Tham gia: ${joinedStr}</span>
    `;

    const curLvlXp = profile.currentLevelXp || 0;
    const nextLvlXp = profile.nextLevelXp || 100;
    const lvlPct = Math.min(100, Math.round((curLvlXp / Math.max(1, nextLvlXp)) * 100));
    document.getElementById('profile-xp-text').textContent = `${curLvlXp} / ${nextLvlXp} XP`;
    document.getElementById('profile-xp-fill').style.width = `${lvlPct}%`;
    document.getElementById('profile-today-goal-text').textContent =
      `🎯 Hôm nay: ${profile.todayXp || 0}/${profile.dailyGoalXp || 50} XP`;
    document.getElementById('profile-total-xp-badge').textContent =
      `Tổng: ${profile.experiencePoints || 0} XP`;

    document.getElementById('prof-stat-words').textContent = profile.totalWordsLearned || 0;
    document.getElementById('prof-stat-mastered').textContent = profile.totalWordsMastered || 0;
    document.getElementById('prof-stat-lessons').textContent = profile.totalLessonsCompleted || 0;
    document.getElementById('prof-stat-quizzes').textContent = profile.totalQuizzesCompleted || 0;
  }

  function getUnlockedAvatars() {
    const uId = state.user?.id || 'guest';
    try {
      const raw = localStorage.getItem('polylearn_unlocked_avatars_' + uId);
      const list = raw ? JSON.parse(raw) : ['owl', 'panda'];
      if (state.user?.avatarKey && !list.includes(state.user.avatarKey)) {
        list.push(state.user.avatarKey);
      }
      return list;
    } catch {
      return ['owl', 'panda'];
    }
  }

  function saveUnlockedAvatar(key) {
    const uId = state.user?.id || 'guest';
    const list = getUnlockedAvatars();
    if (!list.includes(key)) list.push(key);
    localStorage.setItem('polylearn_unlocked_avatars_' + uId, JSON.stringify(list));
  }

  function getShopGems() {
    const uId = state.user?.id || 'guest';
    const baseXp = state.user?.experiencePoints || 0;
    const starterBonus = 500;
    const spent = parseInt(localStorage.getItem('polylearn_spent_gems_' + uId) || '0', 10) || 0;
    return Math.max(0, baseXp + starterBonus - spent);
  }

  function spendShopGems(amount) {
    const uId = state.user?.id || 'guest';
    const spent = parseInt(localStorage.getItem('polylearn_spent_gems_' + uId) || '0', 10) || 0;
    localStorage.setItem('polylearn_spent_gems_' + uId, String(spent + amount));
  }

  function renderProfilePickers() {
    if (typeof Gamification === 'undefined') return;

    // Sync shop gems pill
    const userGems = getShopGems();
    const gemsPill = document.getElementById('shop-user-gems');
    if (gemsPill) gemsPill.textContent = userGems;

    // Filter tabs
    state.avatarShopFilter = state.avatarShopFilter || 'all';
    document.querySelectorAll('.avatar-shop-tab').forEach(tab => {
      tab.classList.toggle('active', tab.dataset.filter === state.avatarShopFilter);
      if (!tab.dataset.bound) {
        tab.dataset.bound = 'true';
        tab.addEventListener('click', () => {
          state.avatarShopFilter = tab.dataset.filter;
          if (typeof Gamification !== 'undefined') Gamification.playSound('pop');
          renderProfilePickers();
        });
      }
    });

    // Custom File upload handler
    const customFileInput = document.getElementById('input-custom-avatar-file');
    if (customFileInput && !customFileInput.dataset.bound) {
      customFileInput.dataset.bound = 'true';
      customFileInput.addEventListener('change', (e) => {
        const file = e.target.files?.[0];
        if (!file) return;
        if (file.size > 2 * 1024 * 1024) {
          showToast('Vui lòng chọn ảnh dung lượng dưới 2MB.', 'error');
          return;
        }
        const reader = new FileReader();
        reader.onload = async (evt) => {
          const dataUrl = evt.target.result;
          const uId = state.user?.id || 'guest';
          localStorage.setItem('polylearn_custom_avatar_' + uId, dataUrl);
          state.selectedAvatarKey = 'custom';
          if (state.user) state.user.avatarKey = 'custom';
          saveUnlockedAvatar('custom');
          Gamification.playSound('fanfare');
          Gamification.launchConfetti(80);
          showToast('🎉 Đã tải lên và cài ảnh đại diện cá nhân!', 'success');
          const bannerAvatar = document.getElementById('profile-banner-avatar');
          if (bannerAvatar) bannerAvatar.innerHTML = Gamification.getAvatarSvg('custom');
          syncShellUser();
          renderProfilePickers();
          if (state.user && API.getToken()) {
            try {
              await API.updateProfile({ avatarKey: 'custom' });
            } catch {}
          }
        };
        reader.readAsDataURL(file);
      });
    }

    const unlockedList = getUnlockedAvatars();
    const avatarGrid = document.getElementById('profile-avatar-picker');

    if (avatarGrid) {
      avatarGrid.innerHTML = '';
      const catalog = Gamification.AVATAR_CATALOG || [];

      // Filter catalog
      const itemsToRender = catalog.filter(item => {
        const isUnlocked = unlockedList.includes(item.key) || item.price === 0;
        if (state.avatarShopFilter === 'owned') return isUnlocked;
        if (state.avatarShopFilter === 'shop') return !isUnlocked;
        return true;
      });

      if (itemsToRender.length === 0) {
        avatarGrid.innerHTML = `
          <div style="grid-column: 1/-1; text-align: center; padding: 30px; color: var(--text-muted);">
            Không có ảnh đại diện nào trong danh mục này.
          </div>
        `;
      }

      const rarityLabels = {
        free: 'Miễn phí',
        rare: '✨ Hiếm',
        epic: '🔥 Sử thi',
        legendary: '👑 Huyền thoại',
        mythic: '⚡ Thần thoại'
      };

      itemsToRender.forEach(item => {
        const isUnlocked = unlockedList.includes(item.key) || item.price === 0;
        const isCurrent = state.selectedAvatarKey === item.key;

        const card = document.createElement('div');
        card.className = `avatar-shop-item ${isCurrent ? 'active-avatar' : ''} ${!isUnlocked ? 'locked' : ''}`;

        card.innerHTML = `
          <span class="avatar-shop-rarity ${item.rarity || 'free'}">${rarityLabels[item.rarity] || 'Thường'}</span>
          <div class="avatar-shop-svg-wrap">
            ${Gamification.getAvatarSvg(item.key)}
          </div>
          <div class="avatar-shop-name" title="${escapeHtml(item.label)}">${escapeHtml(item.label)}</div>
          <div class="avatar-shop-desc">${escapeHtml(item.desc || '')}</div>
          <div style="width: 100%; margin-top: auto;">
            ${isCurrent
              ? `<button type="button" class="avatar-shop-action-btn btn-active">★ Đang dùng</button>`
              : isUnlocked
                ? `<button type="button" class="avatar-shop-action-btn btn-equip btn-equip-avatar">✨ Sử dụng</button>`
                : `<button type="button" class="avatar-shop-action-btn btn-buy btn-buy-avatar">🛒 Mua (💎 ${item.price})</button>`
            }
          </div>
        `;

        const equipAvatar = async () => {
          state.selectedAvatarKey = item.key;
          if (state.user) state.user.avatarKey = item.key;
          Gamification.playSound('pop');
          const bannerAvatar = document.getElementById('profile-banner-avatar');
          if (bannerAvatar) bannerAvatar.innerHTML = Gamification.getAvatarSvg(item.key);
          syncShellUser();
          renderProfilePickers();
          showToast(`Đã chọn ảnh đại diện ${item.label}!`, 'info');
          if (state.user && API.getToken()) {
            try {
              await API.updateProfile({ avatarKey: item.key });
            } catch {}
          }
        };

        const buyAvatar = async () => {
          const curGems = getShopGems();
          if (curGems < item.price) {
            Gamification.playSound('wrong');
            showToast(`Bạn cần thêm ${item.price - curGems} 💎 Kim cương để mua ảnh này! Hãy làm bài kiểm tra hoặc vượt ải để tích lũy thêm nhé!`, 'warning');
            return;
          }

          spendShopGems(item.price);
          saveUnlockedAvatar(item.key);
          state.selectedAvatarKey = item.key;
          if (state.user) state.user.avatarKey = item.key;

          Gamification.playSound('fanfare');
          Gamification.launchConfetti(85);
          showToast(`🎉 Mua thành công (-${item.price} 💎)! Bạn đã mở khóa và trang bị "${item.label}"!`, 'success');

          const bannerAvatar = document.getElementById('profile-banner-avatar');
          if (bannerAvatar) bannerAvatar.innerHTML = Gamification.getAvatarSvg(item.key);
          syncShellUser();
          renderProfilePickers();

          if (state.user && API.getToken()) {
            try {
              await API.updateProfile({ avatarKey: item.key });
            } catch {}
          }
        };

        card.style.cursor = 'pointer';
        card.addEventListener('click', () => {
          if (isCurrent) return;
          if (isUnlocked) equipAvatar();
          else buyAvatar();
        });

        const btnEquip = card.querySelector('.btn-equip-avatar');
        if (btnEquip) {
          btnEquip.addEventListener('click', (e) => {
            e.stopPropagation();
            equipAvatar();
          });
        }

        const btnBuy = card.querySelector('.btn-buy-avatar');
        if (btnBuy) {
          btnBuy.addEventListener('click', (e) => {
            e.stopPropagation();
            buyAvatar();
          });
        }

        avatarGrid.appendChild(card);
      });
    }

    const companionGrid = document.getElementById('profile-companion-picker');
    if (companionGrid) {
      companionGrid.innerHTML = '';
      Gamification.COMPANION_CATALOG.forEach(item => {
        const card = document.createElement('div');
        card.className = `companion-option-card ${state.selectedCompanionKey === item.key ? 'selected' : ''}`;
        card.innerHTML = `
          <div style="font-weight: 700; font-size: 0.88rem; color: var(--text-primary); margin-bottom: 3px;">${escapeHtml(item.label)}</div>
          <div style="font-size: 0.76rem; color: var(--text-muted); line-height: 1.35;">${escapeHtml(item.desc)}</div>
        `;
        card.addEventListener('click', () => {
          state.selectedCompanionKey = item.key;
          Gamification.playSound('pop');
          renderProfilePickers();
        });
        companionGrid.appendChild(card);
      });
    }
  }

  function renderProfileStreakAndBadges(profile) {
    document.getElementById('profile-streak-count').textContent = profile.currentStreakDays || 0;
    const activeSet = new Set(profile.activeDatesInLastTwoWeeks || []);
    const strip = document.getElementById('profile-streak-strip');
    if (strip) {
      strip.innerHTML = '';
      const now = new Date();
      for (let i = 13; i >= 0; i--) {
        const d = new Date(Date.UTC(now.getUTCFullYear(), now.getUTCMonth(), now.getUTCDate() - i));
        const iso = d.toISOString().slice(0, 10);
        const dayLabel = `${String(d.getUTCDate()).padStart(2, '0')}/${String(d.getUTCMonth() + 1).padStart(2, '0')}`;
        const isActive = activeSet.has(iso);

        const cell = document.createElement('div');
        cell.className = `streak-day-cell ${isActive ? 'active' : ''}`;
        cell.innerHTML = `
          <div style="font-size: 1.1rem; margin-bottom: 3px;">${isActive ? '🔥' : '⚪'}</div>
          <div>${i === 0 ? 'H.nay' : dayLabel}</div>
        `;
        strip.appendChild(cell);
      }
    }

    const badges = profile.badges || [];
    const unlockedCount = badges.filter(b => b.isUnlocked).length;
    document.getElementById('profile-badges-summary').textContent = `Đã mở khóa ${unlockedCount}/${badges.length}`;

    const badgesGrid = document.getElementById('profile-badges-grid');
    if (badgesGrid) {
      badgesGrid.innerHTML = '';
      badges.forEach(b => {
        const card = document.createElement('div');
        card.className = `badge-card ${b.isUnlocked ? '' : 'locked'}`;
        card.innerHTML = `
          <div class="badge-icon-orb">${b.icon || '🏅'}</div>
          <div style="flex: 1;">
            <div style="font-weight: 700; font-size: 0.95rem; color: var(--text-primary); margin-bottom: 3px;">${escapeHtml(b.name)}</div>
            <div style="font-size: 0.8rem; color: var(--text-secondary); margin-bottom: 6px; line-height: 1.4;">${escapeHtml(b.description)}</div>
            <span class="card-tag" style="font-size: 0.72rem; padding: 2px 8px; background: ${b.isUnlocked ? 'rgba(16,185,129,0.16)' : 'var(--bg-subtle)'}; color: ${b.isUnlocked ? 'var(--success-text)' : 'var(--text-muted)'}; border-color: ${b.isUnlocked ? 'rgba(16,185,129,0.35)' : 'var(--border-subtle)'};">
              ${b.isUnlocked ? `✓ Hoàn thành (${escapeHtml(b.progressText)})` : `Tiến độ: ${escapeHtml(b.progressText)}`}
            </span>
          </div>
        `;
        badgesGrid.appendChild(card);
      });
    }
  }

  function renderProfileQuizHistory(items = []) {
    const container = document.getElementById('profile-quiz-history');
    if (!container) return;

    if (!items || items.length === 0) {
      container.innerHTML = `
        <div style="text-align: center; padding: 24px; color: var(--text-muted);">
          Bạn chưa làm bài kiểm tra nào. Hãy sang mục <strong>📝 Kiểm tra</strong> (Trắc nghiệm &amp; Tự luận) để thử sức nhé!
        </div>
      `;
      return;
    }

    const langNames = { en: '🇬🇧 Tiếng Anh', zh: '🇨🇳 Tiếng Trung', ja: '🇯🇵 Tiếng Nhật', ko: '🇰🇷 Tiếng Hàn' };
    container.innerHTML = items.slice(0, 6).map(q => {
      const dateStr = new Date(q.completedAt || q.startedAt).toLocaleString('vi-VN');
      const langBadge = langNames[q.languageCode] || q.languageCode.toUpperCase();
      return `
        <div style="display: flex; justify-content: space-between; align-items: center; padding: 12px 16px; border-radius: var(--radius-md); background: var(--bg-surface-elevated); border: 1px solid var(--border-subtle); margin-bottom: 10px; flex-wrap: wrap; gap: 10px;">
          <div>
            <div style="font-weight: 700; color: var(--text-primary);">${langBadge} ${q.levelCode ? `• ${escapeHtml(q.levelCode)}` : ''}</div>
            <div style="font-size: 0.8rem; color: var(--text-muted);">${dateStr}</div>
          </div>
          <div style="display: flex; align-items: center; gap: 12px;">
            <span style="font-family: 'Outfit'; font-weight: 800; font-size: 1.1rem; color: ${q.isPassed ? 'var(--success-text)' : 'var(--danger)'};">
              ${q.score}/${q.totalQuestions} (${q.percentage}%)
            </span>
            <span class="card-tag" style="background: ${q.isPassed ? 'rgba(16,185,129,0.15)' : 'rgba(239,68,68,0.14)'}; color: ${q.isPassed ? 'var(--success-text)' : 'var(--danger)'}; border-color: ${q.isPassed ? 'rgba(16,185,129,0.35)' : 'rgba(239,68,68,0.35)'}; font-size: 0.75rem;">
              ${q.isPassed ? '✓ Đạt' : 'Chưa đạt'}
            </span>
          </div>
        </div>
      `;
    }).join('');
  }

  async function loadProfile() {
    if (!API.getToken()) {
      if (profileGuestBox) profileGuestBox.style.display = 'block';
      if (profileContentBox) profileContentBox.style.display = 'none';
      const guestMascot = document.getElementById('profile-guest-mascot');
      if (guestMascot && typeof Gamification !== 'undefined') {
        guestMascot.innerHTML = Gamification.getCompanion('auto', state.activeLanguageId).svg;
      }
      return;
    }

    if (profileGuestBox) profileGuestBox.style.display = 'none';
    if (profileContentBox) profileContentBox.style.display = 'block';

    try {
      const [profile, quizHist] = await Promise.all([
        API.getMe(),
        API.getQuizHistory(1).catch(() => ({ items: [] }))
      ]);

      state.profileData = profile;
      state.selectedAvatarKey = profile.avatarKey || 'owl';
      state.selectedCompanionKey = profile.companionKey || 'auto';
      state.selectedDailyGoalXp = profile.dailyGoalXp || 50;

      // Sync stored user
      state.user = {
        ...(state.user || {}),
        id: profile.id,
        email: profile.email,
        displayName: profile.displayName,
        role: profile.role,
        preferredLanguageId: profile.preferredLanguageId,
        avatarKey: profile.avatarKey,
        companionKey: profile.companionKey,
        bio: profile.bio,
        dailyGoalXp: profile.dailyGoalXp,
        experiencePoints: profile.experiencePoints,
        todayXp: profile.todayXp,
        level: profile.level,
        rankTitle: profile.rankTitle
      };
      API.setUser(state.user);
      renderAuthStatus();

      renderProfileBannerAndStats(profile);
      renderProfilePickers();
      renderProfileStreakAndBadges(profile);
      renderProfileQuizHistory(quizHist.items || []);

      // Populate form fields
      document.getElementById('profile-input-name').value = profile.displayName || '';
      document.getElementById('profile-input-bio').value = profile.bio || '';
      document.getElementById('profile-input-lang').value = profile.preferredLanguageId || state.activeLanguageId || 1;

      const presets = document.querySelectorAll('#profile-goal-presets .goal-preset-btn');
      let matchedPreset = false;
      presets.forEach(btn => {
        const isMatch = parseInt(btn.dataset.goal, 10) === state.selectedDailyGoalXp;
        btn.classList.toggle('active', isMatch);
        if (isMatch) matchedPreset = true;
      });
      if (!matchedPreset && presets[1]) presets[1].classList.add('active');
    } catch (err) {
      showToast(`Không thể tải hồ sơ cá nhân: ${err.message}`, 'error');
    }
  }

  async function saveProfileUpdates(triggerEl) {
    if (!API.getToken()) {
      openAuthModal();
      return;
    }

    const displayName = document.getElementById('profile-input-name').value.trim() || state.user?.displayName || 'Học viên';
    const bio = document.getElementById('profile-input-bio').value.trim();
    const preferredLanguageId = parseInt(document.getElementById('profile-input-lang').value, 10) || 1;

    try {
      const updated = await API.updateProfile({
        displayName,
        preferredLanguageId,
        avatarKey: state.selectedAvatarKey,
        companionKey: state.selectedCompanionKey,
        bio,
        dailyGoalXp: state.selectedDailyGoalXp
      });

      state.profileData = updated;
      state.user = {
        ...(state.user || {}),
        displayName: updated.displayName,
        preferredLanguageId: updated.preferredLanguageId,
        avatarKey: updated.avatarKey,
        companionKey: updated.companionKey,
        bio: updated.bio,
        dailyGoalXp: updated.dailyGoalXp,
        experiencePoints: updated.experiencePoints,
        todayXp: updated.todayXp,
        level: updated.level,
        rankTitle: updated.rankTitle
      };
      API.setUser(state.user);
      renderAuthStatus();
      renderProfileBannerAndStats(updated);
      renderProfileStreakAndBadges(updated);

      if (typeof Gamification !== 'undefined') {
        Gamification.playSound('fanfare');
        Gamification.launchConfetti(45);
        if (triggerEl) Gamification.spawnHeartBurst(triggerEl);
      }
      refreshMascots('party', `Tuyệt vời ${updated.displayName}! Diện mạo và mục tiêu mới đã sẵn sàng! ✨`);
      showToast('Đã cập nhật trang cá nhân & linh vật thành công!', 'success');
    } catch (err) {
      showToast(err.message, 'error');
    }
  }

  if (btnSaveCharacter) {
    btnSaveCharacter.addEventListener('click', () => saveProfileUpdates(btnSaveCharacter));
  }

  if (profileForm) {
    profileForm.addEventListener('submit', async (e) => {
      e.preventDefault();
      await saveProfileUpdates(document.getElementById('btn-save-profile'));
    });
  }

  if (profilePasswordForm) {
    profilePasswordForm.addEventListener('submit', async (e) => {
      e.preventDefault();
      const curPwd = document.getElementById('profile-cur-pwd').value;
      const newPwd = document.getElementById('profile-new-pwd').value;
      if (!curPwd || !newPwd) return;

      const btn = document.getElementById('btn-change-pwd');
      btn.disabled = true;
      try {
        const res = await API.changePassword(curPwd, newPwd);
        profilePasswordForm.reset();
        if (typeof Gamification !== 'undefined') Gamification.playSound('correct');
        showToast(res?.message || 'Đổi mật khẩu thành công!', 'success');
      } catch (err) {
        showToast(err.message, 'error');
      } finally {
        btn.disabled = false;
      }
    });
  }

  // ---------- 6. ADMIN PORTAL (QUẢN TRỊ VIÊN) ----------
  let adminActiveTab = 'words';
  let adminEditingWordId = null;
  let adminEditingLessonId = null;
  let adminEditingQuestionId = null;
  let adminParsedCsvWords = [];

  // Subtabs switching
  document.querySelectorAll('.admin-subtab-btn').forEach(btn => {
    btn.addEventListener('click', () => {
      document.querySelectorAll('.admin-subtab-btn').forEach(b => b.classList.remove('active'));
      btn.classList.add('active');
      adminActiveTab = btn.dataset.panel;

      document.querySelectorAll('.admin-panel').forEach(p => {
        p.style.display = p.id === `admin-panel-${adminActiveTab}` ? 'block' : 'none';
      });

      if (adminActiveTab === 'words') loadAdminWords();
      else if (adminActiveTab === 'lessons') loadAdminLessons();
      else if (adminActiveTab === 'questions') loadAdminQuestions();
      else if (adminActiveTab === 'users') loadAdminUsers();
    });
  });

  function loadAdminPanel() {
    if (!state.user || state.user.role !== 'Admin') {
      showToast('Bạn không có quyền truy cập trang quản trị!', 'error');
      switchView('dashboard');
      return;
    }

    if (adminActiveTab === 'words') loadAdminWords();
    else if (adminActiveTab === 'lessons') loadAdminLessons();
    else if (adminActiveTab === 'questions') loadAdminQuestions();
    else if (adminActiveTab === 'users') loadAdminUsers();
  }

  // Helper to populate levels for an admin select
  async function populateAdminLevels(langId, selectEl) {
    try {
      const lang = await API.getLanguage(langId);
      selectEl.innerHTML = '';
      if (lang && lang.levels) {
        lang.levels.forEach(lvl => {
          const opt = document.createElement('option');
          opt.value = lvl.id;
          opt.textContent = `${lvl.code} - ${lvl.name}`;
          selectEl.appendChild(opt);
        });
      }
    } catch (e) {
      console.warn('Cannot populate admin levels:', e);
    }
  }

  // ----- A. Admin Words Management -----
  const adminWordsLangSelect = document.getElementById('admin-words-lang-select');
  const adminWordsSearch = document.getElementById('admin-words-search');
  const adminWordsTbody = document.getElementById('admin-words-tbody');
  const btnAdminAddWord = document.getElementById('btn-admin-add-word');
  const btnAdminExportWordsCsv = document.getElementById('btn-admin-export-words-csv');
  const btnAdminImportWordsCsv = document.getElementById('btn-admin-import-words-csv');
  const adminWordModal = document.getElementById('admin-word-modal');
  const adminWordForm = document.getElementById('admin-word-form');
  const adminWordLangSelect = document.getElementById('admin-word-lang-select');
  const adminWordLevelSelect = document.getElementById('admin-word-level-select');
  const btnCloseWordModal = document.getElementById('btn-close-word-modal');
  const btnCancelWordModal = document.getElementById('btn-cancel-word-modal');

  // CSV Modal Elements
  const adminCsvModal = document.getElementById('admin-csv-modal');
  const btnCloseCsvModal = document.getElementById('btn-close-csv-modal');
  const btnCancelCsv = document.getElementById('btn-cancel-csv');
  const adminCsvFileInput = document.getElementById('admin-csv-file-input');
  const adminCsvPreviewArea = document.getElementById('admin-csv-preview-area');
  const btnSubmitCsvImport = document.getElementById('btn-submit-csv-import');

  adminWordsLangSelect.addEventListener('change', loadAdminWords);
  adminWordsSearch.addEventListener('input', debounce(loadAdminWords, 350));

  async function loadAdminWords() {
    adminWordsTbody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-muted); padding: 30px;">⏳ Đang nạp danh sách từ vựng...</td></tr>';
    try {
      const res = await API.getWords({
        languageId: parseInt(adminWordsLangSelect.value, 10),
        search: adminWordsSearch.value.trim(),
        pageSize: 100
      });

      const words = res.items || [];
      if (words.length === 0) {
        adminWordsTbody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-muted); padding: 30px;">Chưa có từ vựng nào trong danh mục này.</td></tr>';
        return;
      }

      adminWordsTbody.innerHTML = '';
      words.forEach(w => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
          <td style="font-weight: 700; color: var(--text-primary);">${escapeHtml(w.term)}</td>
          <td style="color: var(--reading-color); font-weight: 500;">${escapeHtml(w.reading || '-')}</td>
          <td>${escapeHtml(w.meaning)}</td>
          <td><span class="card-tag" style="background: var(--bg-subtle); color: var(--text-secondary); border-color: var(--border-subtle); font-size: 0.75rem;">${escapeHtml(w.partOfSpeech || '-')}</span></td>
          <td><span class="card-tag" style="background: var(--accent-soft); color: var(--accent-primary); font-size: 0.75rem;">${escapeHtml(w.levelCode || 'A1')}</span></td>
          <td style="text-align: right; white-space: nowrap;">
            <button class="btn-action-sm btn-action-edit btn-edit-word" data-id="${w.id}">✏️ Sửa</button>
            <button class="btn-action-sm btn-action-delete btn-del-word" data-id="${w.id}" style="margin-left: 6px;">🗑️ Xóa</button>
          </td>
        `;

        tr.querySelector('.btn-edit-word').addEventListener('click', () => editWord(w.id));
        tr.querySelector('.btn-del-word').addEventListener('click', () => deleteWord(w.id, w.term));

        adminWordsTbody.appendChild(tr);
      });
    } catch (err) {
      adminWordsTbody.innerHTML = `<tr><td colspan="6" style="color: var(--danger); text-align: center; padding: 20px;">Lỗi: ${escapeHtml(err.message)}</td></tr>`;
    }
  }

  adminWordLangSelect.addEventListener('change', () => {
    populateAdminLevels(adminWordLangSelect.value, adminWordLevelSelect);
  });

  btnAdminAddWord.addEventListener('click', async () => {
    adminEditingWordId = null;
    document.getElementById('admin-word-modal-title').textContent = 'Thêm Từ Vựng Mới';
    adminWordForm.reset();
    adminWordLangSelect.value = adminWordsLangSelect.value;
    await populateAdminLevels(adminWordLangSelect.value, adminWordLevelSelect);
    adminWordModal.classList.add('active');
  });

  async function editWord(id) {
    try {
      const w = await API.getWordDetail(id);
      adminEditingWordId = id;
      document.getElementById('admin-word-modal-title').textContent = `Sửa Từ Vựng: ${w.term}`;
      adminWordLangSelect.value = w.languageId;
      await populateAdminLevels(w.languageId, adminWordLevelSelect);
      adminWordLevelSelect.value = w.levelId;

      document.getElementById('admin-word-term').value = w.term;
      document.getElementById('admin-word-reading').value = w.reading || '';
      document.getElementById('admin-word-meaning').value = w.meaning;
      document.getElementById('admin-word-pos').value = w.partOfSpeech || '';

      const ex = w.examples && w.examples[0] ? w.examples[0] : null;
      document.getElementById('admin-word-ex-sentence').value = ex ? ex.sentence : '';
      document.getElementById('admin-word-ex-trans').value = ex ? ex.translation : '';

      adminWordModal.classList.add('active');
    } catch (err) {
      showToast(`Lỗi: ${err.message}`, 'error');
    }
  }

  async function deleteWord(id, term) {
    if (!confirm(`Bạn có chắc muốn xóa từ vựng "${term}" (ID: ${id})?`)) return;
    try {
      await API.deleteAdminWord(id);
      showToast(`Đã xóa từ vựng "${term}" thành công!`, 'success');
      loadAdminWords();
    } catch (err) {
      showToast(err.message, 'error');
    }
  }

  function closeAdminWordModal() {
    adminWordModal.classList.remove('active');
    adminEditingWordId = null;
  }
  btnCloseWordModal.addEventListener('click', closeAdminWordModal);
  btnCancelWordModal.addEventListener('click', closeAdminWordModal);

  adminWordForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const langId = parseInt(adminWordLangSelect.value, 10);
    const levelId = parseInt(adminWordLevelSelect.value, 10);
    const term = document.getElementById('admin-word-term').value.trim();
    const reading = document.getElementById('admin-word-reading').value.trim();
    const meaning = document.getElementById('admin-word-meaning').value.trim();
    const partOfSpeech = document.getElementById('admin-word-pos').value.trim();
    const sentence = document.getElementById('admin-word-ex-sentence').value.trim();
    const translation = document.getElementById('admin-word-ex-trans').value.trim();

    const examples = sentence ? [{ sentence, reading: null, translation: translation || sentence }] : [];

    try {
      if (adminEditingWordId) {
        await API.updateAdminWord(adminEditingWordId, {
          levelId,
          topicId: null,
          term,
          reading,
          meaning,
          partOfSpeech,
          examples
        });
        showToast(`Đã cập nhật từ "${term}" thành công!`, 'success');
      } else {
        await API.createAdminWord({
          languageId: langId,
          levelId,
          topicId: null,
          term,
          reading,
          meaning,
          partOfSpeech,
          examples
        });
        showToast(`Đã tạo từ vựng "${term}" thành công!`, 'success');
      }

      closeAdminWordModal();
      loadAdminWords();
      loadWords(); // update library view
    } catch (err) {
      showToast(err.message, 'error');
    }
  });

  // Export Words CSV
  btnAdminExportWordsCsv.addEventListener('click', async () => {
    try {
      const res = await API.getWords({
        languageId: parseInt(adminWordsLangSelect.value, 10),
        pageSize: 1000
      });
      const words = res.items || [];
      if (words.length === 0) {
        showToast('Không có dữ liệu từ vựng để xuất.', 'info');
        return;
      }

      let csv = '\uFEFFTerm,Reading,Meaning,PartOfSpeech,Level\n';
      words.forEach(w => {
        csv += `"${(w.term || '').replace(/"/g, '""')}","${(w.reading || '').replace(/"/g, '""')}","${(w.meaning || '').replace(/"/g, '""')}","${(w.partOfSpeech || '').replace(/"/g, '""')}","${(w.levelCode || '').replace(/"/g, '""')}"\n`;
      });

      const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = `PolyLearn_Words_Lang${adminWordsLangSelect.value}.csv`;
      link.click();
      URL.revokeObjectURL(url);
      showToast('Đã xuất file CSV thành công!', 'success');
    } catch (err) {
      showToast(err.message, 'error');
    }
  });

  // Import Words CSV
  btnAdminImportWordsCsv.addEventListener('click', () => {
    adminParsedCsvWords = [];
    adminCsvFileInput.value = '';
    adminCsvPreviewArea.style.display = 'none';
    adminCsvPreviewArea.innerHTML = '';
    btnSubmitCsvImport.disabled = true;
    adminCsvModal.classList.add('active');
  });

  function closeAdminCsvModal() {
    adminCsvModal.classList.remove('active');
    adminParsedCsvWords = [];
  }
  btnCloseCsvModal.addEventListener('click', closeAdminCsvModal);
  btnCancelCsv.addEventListener('click', closeAdminCsvModal);

  adminCsvFileInput.addEventListener('change', (e) => {
    const file = e.target.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = async (evt) => {
      const text = evt.target.result;
      const lines = text.split(/\r?\n/).map(l => l.trim()).filter(l => l.length > 0);
      if (lines.length < 2) {
        showToast('File CSV không có đủ dữ liệu!', 'error');
        return;
      }

      const langId = parseInt(adminWordsLangSelect.value, 10);
      const lang = await API.getLanguage(langId);
      const defaultLevelId = lang && lang.levels && lang.levels[0] ? lang.levels[0].id : 1;

      const splitCsvRow = (row) => (
        row.match(/(?:^|,)(?:"([^"]*(?:""[^"]*)*)"|([^,]*))/g)?.map(val => {
          let s = val.startsWith(',') ? val.slice(1) : val;
          if (s.startsWith('"') && s.endsWith('"')) s = s.slice(1, -1).replace(/""/g, '"');
          return s.replace(/^\uFEFF/, '').trim();
        }) || []
      );

      const headerCols = splitCsvRow(lines[0]).map(h => h.toLowerCase());
      const findCol = (keywords) => headerCols.findIndex(h => keywords.some(k => h === k || h.includes(k)));

      let idxTerm = findCol(['term', 'word', 'vocab', 'từ vựng', 'từ']);
      let idxReading = findCol(['reading', 'phonetic', 'ipa', 'pronunciation', 'pinyin', 'romaji', 'phiên âm']);
      let idxMeaning = findCol(['meaning', 'vietnamese', 'nghĩa', 'translation', 'definition', 'dịch']);
      let idxPos = findCol(['partofspeech', 'pos', 'word class', 'type', 'từ loại']);
      let idxEx = findCol(['example', 'sentence', 'ví dụ']);
      let idxExTrans = findCol(['exampletranslation', 'example_vi', 'dịch ví dụ']);

      const invalidPosCodes = new Set([
        'n', 'v', 'adj', 'adv', 'prep', 'pron', 'conj', 'det', 'interj', 'num', 'aux', 'art',
        'noun', 'verb', 'adjective', 'adverb', 'preposition', 'pronoun', 'conjunction',
        'a1', 'a2', 'b1', 'b2', 'c1', 'c2', 'hsk1', 'hsk2', 'hsk3', 'hsk4', 'n5', 'n4', 'n3', 'n2', 'n1'
      ]);

      // Parse lines (skipping header)
      adminParsedCsvWords = [];
      let skippedInvalid = 0;
      for (let i = 1; i < lines.length; i++) {
        const parts = splitCsvRow(lines[i]);
        if (parts.length < 2) continue;

        // Nếu cột đầu tiên là số thứ tự (STT/Index: "1", "32", "36") và chưa khớp header rõ ràng -> tự động dịch sang cột tiếp theo
        const offset = (idxTerm < 0 && /^\d+$/.test(parts[0])) ? 1 : 0;
        const term = (idxTerm >= 0 ? parts[idxTerm] : parts[offset]) || '';
        const reading = (idxReading >= 0 ? parts[idxReading] : parts[offset + 1]) || '';
        const meaning = (idxMeaning >= 0 ? parts[idxMeaning] : parts[offset + 2]) || '';
        const pos = (idxPos >= 0 ? parts[idxPos] : parts[offset + 3]) || 'noun';
        const exSentence = (idxEx >= 0 ? parts[idxEx] : parts[offset + 4]) || '';
        const exTrans = (idxExTrans >= 0 ? parts[idxExTrans] : parts[offset + 5]) || '';

        if (!term || !meaning || /^\d+/.test(term) || /^[\d\W_]+$/.test(meaning) || invalidPosCodes.has(meaning.toLowerCase())) {
          skippedInvalid++;
          continue;
        }

        adminParsedCsvWords.push({
          languageId: langId,
          levelId: defaultLevelId,
          term,
          reading,
          meaning,
          partOfSpeech: pos,
          examples: exSentence ? [{ sentence: exSentence, reading: null, translation: exTrans || exSentence }] : []
        });
      }

      if (adminParsedCsvWords.length > 0) {
        adminCsvPreviewArea.style.display = 'block';
        adminCsvPreviewArea.innerHTML = `
          <strong>Đã phát hiện ${adminParsedCsvWords.length} từ vựng chuẩn${skippedInvalid > 0 ? ` (đã tự lọc bỏ ${skippedInvalid} dòng số thứ tự / thiếu nghĩa tiếng Việt)` : ''}:</strong>
          <ul style="padding-left: 20px; margin-top: 6px;">
            ${adminParsedCsvWords.slice(0, 5).map(w => `<li><strong>${escapeHtml(w.term)}</strong> ${w.reading ? `(${escapeHtml(w.reading)})` : ''}: ${escapeHtml(w.meaning)}</li>`).join('')}
            ${adminParsedCsvWords.length > 5 ? `<li><em>... và ${adminParsedCsvWords.length - 5} từ vựng khác</em></li>` : ''}
          </ul>
        `;
        btnSubmitCsvImport.disabled = false;
      } else {
        showToast('File CSV không có cột nghĩa tiếng Việt hợp lệ (hoặc bị lệch cột số thứ tự / chỉ chứa mã từ loại n, v, adv)!', 'error');
      }
    };
    reader.readAsText(file);
  });

  btnSubmitCsvImport.addEventListener('click', async () => {
    if (adminParsedCsvWords.length === 0) return;
    btnSubmitCsvImport.disabled = true;
    btnSubmitCsvImport.textContent = 'Đang nhập dữ liệu...';

    try {
      const res = await API.batchCreateAdminWords(adminParsedCsvWords);
      showToast(res.message || 'Đã nhập CSV thành công!', 'success');
      closeAdminCsvModal();
      loadAdminWords();
    } catch (err) {
      showToast(err.message, 'error');
    } finally {
      btnSubmitCsvImport.disabled = false;
      btnSubmitCsvImport.textContent = 'Bắt đầu nhập dữ liệu';
    }
  });

  // ----- External Dictionary Lookup for Words -----
  const btnAdminLookupExternal = document.getElementById('btn-admin-lookup-external');
  if (btnAdminLookupExternal) {
    btnAdminLookupExternal.addEventListener('click', async () => {
      const termInput = document.getElementById('admin-word-term');
      const term = termInput.value.trim();
      if (!term) {
        showToast('Vui lòng nhập từ vựng cần tra trước!', 'warning');
        return;
      }
      const langId = parseInt(adminWordLangSelect.value, 10);
      btnAdminLookupExternal.disabled = true;
      btnAdminLookupExternal.textContent = '⏳ Đang tra...';
      try {
        if (langId === 1) { // English via Free Dictionary API
          const resp = await fetch(`https://api.dictionaryapi.dev/api/v2/entries/en/${encodeURIComponent(term)}`);
          if (!resp.ok) throw new Error(`Không tìm thấy "${term}" trên từ điển trực tuyến.`);
          const data = await resp.json();
          const entry = data[0];
          if (entry) {
            const phonetic = entry.phonetic || (entry.phonetics && entry.phonetics.find(p => p.text)?.text) || '';
            if (phonetic) document.getElementById('admin-word-reading').value = phonetic;

            if (entry.meanings && entry.meanings.length > 0) {
              const m = entry.meanings[0];
              if (m.partOfSpeech) document.getElementById('admin-word-pos').value = m.partOfSpeech;
              if (m.definitions && m.definitions.length > 0) {
                const def = m.definitions[0];
                document.getElementById('admin-word-meaning').value = def.definition || '';
                const exDef = m.definitions.find(d => d.example);
                if (exDef && exDef.example) {
                  document.getElementById('admin-word-ex-sentence').value = exDef.example;
                  document.getElementById('admin-word-ex-trans').value = exDef.example;
                }
              }
            }
            showToast(`Đã tự động lấy dữ liệu từ điển cho "${term}"!`, 'success');
          }
        } else {
          showToast('Tra từ trực tuyến hỗ trợ nhanh cho Tiếng Anh. Để nạp hàng loạt theo chuẩn, vui lòng dùng "🌐 Nạp từ vựng ngoài (Oxford/HSK/JLPT/TOPIK)"!', 'info');
        }
      } catch (err) {
        showToast(err.message || 'Lỗi khi tra từ vựng ngoài!', 'error');
      } finally {
        btnAdminLookupExternal.disabled = false;
        btnAdminLookupExternal.textContent = '🔍 Tra từ ngoài';
      }
    });
  }

  // ----- International Standard Presets Modal -----
  const adminPresetsModal = document.getElementById('admin-presets-modal');
  const btnAdminOpenPresets = document.getElementById('btn-admin-open-presets');
  const btnClosePresetsModal = document.getElementById('btn-close-presets-modal');
  const btnCancelPresets = document.getElementById('btn-cancel-presets');
  const btnImportAllPresets = document.getElementById('btn-import-all-presets');

  function openAdminPresetsModal() {
    if (adminPresetsModal) adminPresetsModal.classList.add('active');
  }

  function closeAdminPresetsModal() {
    if (adminPresetsModal) adminPresetsModal.classList.remove('active');
  }

  if (btnAdminOpenPresets) btnAdminOpenPresets.addEventListener('click', openAdminPresetsModal);
  if (btnClosePresetsModal) btnClosePresetsModal.addEventListener('click', closeAdminPresetsModal);
  if (btnCancelPresets) btnCancelPresets.addEventListener('click', closeAdminPresetsModal);

  // Single preset import
  document.querySelectorAll('.btn-import-preset-single').forEach(btn => {
    btn.addEventListener('click', async () => {
      const datasetKey = btn.dataset.key;
      const originalText = btn.textContent;
      btn.disabled = true;
      btn.textContent = '⏳ Đang nạp...';
      try {
        const res = await API.importAdminPreset(datasetKey);
        showToast(res.message || 'Nạp bộ từ vựng chuẩn thành công!', 'success');
        closeAdminPresetsModal();
        loadAdminWords();
      } catch (err) {
        showToast(err.message, 'error');
      } finally {
        btn.disabled = false;
        btn.textContent = originalText;
      }
    });
  });

  // Import all presets
  if (btnImportAllPresets) {
    btnImportAllPresets.addEventListener('click', async () => {
      btnImportAllPresets.disabled = true;
      const originalText = btnImportAllPresets.innerHTML;
      btnImportAllPresets.innerHTML = '⏳ Đang nạp toàn bộ 100 từ vựng và 100 câu hỏi...';
      try {
        const res = await API.importAdminPreset('all');
        showToast(res.message || 'Nạp toàn bộ 4 bộ từ vựng chuẩn quốc tế thành công!', 'success');
        closeAdminPresetsModal();
        loadAdminWords();
      } catch (err) {
        showToast(err.message, 'error');
      } finally {
        btnImportAllPresets.disabled = false;
        btnImportAllPresets.innerHTML = originalText;
      }
    });
  }

  // ----- B. Admin Lessons Management -----
  const adminLessonsLangSelect = document.getElementById('admin-lessons-lang-select');
  const adminLessonsSearch = document.getElementById('admin-lessons-search');
  const adminLessonsTbody = document.getElementById('admin-lessons-tbody');
  const btnAdminAddLesson = document.getElementById('btn-admin-add-lesson');
  const adminLessonModal = document.getElementById('admin-lesson-modal');
  const adminLessonForm = document.getElementById('admin-lesson-form');
  const adminLessonLangSelect = document.getElementById('admin-lesson-lang-select');
  const adminLessonLevelSelect = document.getElementById('admin-lesson-level-select');
  const btnCloseLessonAdminModal = document.getElementById('btn-close-lesson-admin-modal');
  const btnCancelLessonAdminModal = document.getElementById('btn-cancel-lesson-admin-modal');

  adminLessonsLangSelect.addEventListener('change', loadAdminLessons);
  adminLessonsSearch.addEventListener('input', debounce(loadAdminLessons, 350));

  async function loadAdminLessons() {
    adminLessonsTbody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-muted); padding: 30px;">⏳ Đang nạp danh sách bài học...</td></tr>';
    try {
      const res = await API.getLessons({
        languageId: parseInt(adminLessonsLangSelect.value, 10),
        search: adminLessonsSearch.value.trim(),
        pageSize: 100
      });

      const lessons = res.items || [];
      if (lessons.length === 0) {
        adminLessonsTbody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-muted); padding: 30px;">Chưa có bài học nào.</td></tr>';
        return;
      }

      adminLessonsTbody.innerHTML = '';
      lessons.forEach(l => {
        const tr = document.createElement('tr');
        const typeNames = ['🟢 Từ vựng', '🟣 Luyện nghe', '🔵 Đọc hiểu'];
        tr.innerHTML = `
          <td style="font-weight: 700; color: var(--text-primary);">${escapeHtml(l.title)}</td>
          <td><span class="card-tag" style="font-size: 0.75rem;">${typeNames[l.type] || l.typeName}</span></td>
          <td><span class="card-tag" style="background: var(--accent-soft); color: var(--accent-primary); font-size: 0.75rem;">${escapeHtml(l.levelCode || 'A1')}</span></td>
          <td>${l.sortOrder}</td>
          <td><span class="card-tag" style="background: rgba(16, 185, 129, 0.15); color: var(--success-text); border-color: rgba(16, 185, 129, 0.35); font-size: 0.75rem;">Công khai</span></td>
          <td style="text-align: right; white-space: nowrap;">
            <button class="btn-action-sm btn-action-edit btn-edit-lesson" data-id="${l.id}">✏️ Sửa</button>
            <button class="btn-action-sm btn-action-delete btn-del-lesson" data-id="${l.id}" style="margin-left: 6px;">🗑️ Xóa</button>
          </td>
        `;

        tr.querySelector('.btn-edit-lesson').addEventListener('click', () => editLesson(l.id));
        tr.querySelector('.btn-del-lesson').addEventListener('click', () => deleteLesson(l.id, l.title));

        adminLessonsTbody.appendChild(tr);
      });
    } catch (err) {
      adminLessonsTbody.innerHTML = `<tr><td colspan="6" style="color: var(--danger); text-align: center; padding: 20px;">Lỗi: ${escapeHtml(err.message)}</td></tr>`;
    }
  }

  adminLessonLangSelect.addEventListener('change', () => {
    populateAdminLevels(adminLessonLangSelect.value, adminLessonLevelSelect);
  });

  btnAdminAddLesson.addEventListener('click', async () => {
    adminEditingLessonId = null;
    document.getElementById('admin-lesson-modal-title').textContent = 'Thêm Bài Học Mới';
    adminLessonForm.reset();
    adminLessonLangSelect.value = adminLessonsLangSelect.value;
    await populateAdminLevels(adminLessonLangSelect.value, adminLessonLevelSelect);
    adminLessonModal.classList.add('active');
  });

  async function editLesson(id) {
    try {
      const l = await API.getLessonDetail(id);
      adminEditingLessonId = id;
      document.getElementById('admin-lesson-modal-title').textContent = `Sửa Bài Học: ${l.title}`;
      adminLessonLangSelect.value = l.languageId;
      await populateAdminLevels(l.languageId, adminLessonLevelSelect);
      adminLessonLevelSelect.value = l.levelId;

      document.getElementById('admin-lesson-type-select').value = l.type;
      document.getElementById('admin-lesson-title').value = l.title;
      document.getElementById('admin-lesson-content').value = l.content || '';
      document.getElementById('admin-lesson-sort').value = l.sortOrder;
      document.getElementById('admin-lesson-published').checked = true;

      adminLessonModal.classList.add('active');
    } catch (err) {
      showToast(err.message, 'error');
    }
  }

  async function deleteLesson(id, title) {
    if (!confirm(`Bạn có chắc muốn xóa bài học "${title}"?`)) return;
    try {
      await API.deleteAdminLesson(id);
      showToast(`Đã xóa bài học "${title}" thành công!`, 'success');
      loadAdminLessons();
      loadLessons();
    } catch (err) {
      showToast(err.message, 'error');
    }
  }

  function closeAdminLessonModal() {
    adminLessonModal.classList.remove('active');
    adminEditingLessonId = null;
  }
  btnCloseLessonAdminModal.addEventListener('click', closeAdminLessonModal);
  btnCancelLessonAdminModal.addEventListener('click', closeAdminLessonModal);

  adminLessonForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const langId = parseInt(adminLessonLangSelect.value, 10);
    const levelId = parseInt(adminLessonLevelSelect.value, 10);
    const type = parseInt(document.getElementById('admin-lesson-type-select').value, 10);
    const title = document.getElementById('admin-lesson-title').value.trim();
    const content = document.getElementById('admin-lesson-content').value.trim();
    const sortOrder = parseInt(document.getElementById('admin-lesson-sort').value, 10) || 1;
    const isPublished = document.getElementById('admin-lesson-published').checked;

    try {
      if (adminEditingLessonId) {
        await API.updateAdminLesson(adminEditingLessonId, {
          levelId,
          title,
          type,
          content,
          sortOrder,
          isPublished
        });
        showToast('Cập nhật bài học thành công!', 'success');
      } else {
        await API.createAdminLesson({
          languageId: langId,
          levelId,
          title,
          type,
          content,
          sortOrder,
          isPublished
        });
        showToast('Tạo bài học mới thành công!', 'success');
      }

      closeAdminLessonModal();
      loadAdminLessons();
      loadLessons();
    } catch (err) {
      showToast(err.message, 'error');
    }
  });

  // ----- C. Admin Questions Management -----
  const adminQuestionsLangSelect = document.getElementById('admin-questions-lang-select');
  const adminQuestionsSearch = document.getElementById('admin-questions-search');
  const adminQuestionsTbody = document.getElementById('admin-questions-tbody');
  const btnAdminAddQuestion = document.getElementById('btn-admin-add-question');
  const btnAdminExportQuestionsCsv = document.getElementById('btn-admin-export-questions-csv');
  const adminQuestionModal = document.getElementById('admin-question-modal');
  const adminQuestionForm = document.getElementById('admin-question-form');
  const adminQuestionLangSelect = document.getElementById('admin-question-lang-select');
  const adminQuestionLevelSelect = document.getElementById('admin-question-level-select');
  const btnCloseQuestionModal = document.getElementById('btn-close-question-modal');
  const btnCancelQuestionModal = document.getElementById('btn-cancel-question-modal');

  adminQuestionsLangSelect.addEventListener('change', loadAdminQuestions);
  adminQuestionsSearch.addEventListener('input', debounce(loadAdminQuestions, 350));

  let adminLoadedQuestions = [];
  async function loadAdminQuestions() {
    adminQuestionsTbody.innerHTML = '<tr><td colspan="5" style="text-align: center; color: var(--text-muted); padding: 30px;">⏳ Đang nạp danh sách câu hỏi...</td></tr>';
    try {
      const res = await API.getAdminQuestions({
        languageId: parseInt(adminQuestionsLangSelect.value, 10),
        search: adminQuestionsSearch.value.trim(),
        pageSize: 100
      });

      adminLoadedQuestions = res.items || [];
      if (adminLoadedQuestions.length === 0) {
        adminQuestionsTbody.innerHTML = '<tr><td colspan="5" style="text-align: center; color: var(--text-muted); padding: 30px;">Chưa có câu hỏi trắc nghiệm nào.</td></tr>';
        return;
      }

      adminQuestionsTbody.innerHTML = '';
      adminLoadedQuestions.forEach(q => {
        const correctOpt = q.options.find(o => o.isCorrect);
        const tr = document.createElement('tr');
        tr.innerHTML = `
          <td style="font-weight: 600; color: var(--text-primary);">${escapeHtml(q.text)}</td>
          <td><span class="card-tag" style="background: var(--bg-subtle); color: var(--text-secondary); border-color: var(--border-subtle); font-size: 0.75rem;">${q.options.length} đáp án</span></td>
          <td style="color: var(--success-text); font-weight: 600;">${escapeHtml(correctOpt ? correctOpt.text : '-')}</td>
          <td style="color: var(--text-secondary); font-size: 0.85rem;">${escapeHtml(q.explanation || '-')}</td>
          <td style="text-align: right; white-space: nowrap;">
            <button class="btn-action-sm btn-action-edit btn-edit-question" data-id="${q.id}">✏️ Sửa</button>
            <button class="btn-action-sm btn-action-delete btn-del-question" data-id="${q.id}" style="margin-left: 6px;">🗑️ Xóa</button>
          </td>
        `;

        tr.querySelector('.btn-edit-question').addEventListener('click', () => editQuestion(q.id));
        tr.querySelector('.btn-del-question').addEventListener('click', () => deleteQuestion(q.id, q.text));

        adminQuestionsTbody.appendChild(tr);
      });
    } catch (err) {
      adminQuestionsTbody.innerHTML = `<tr><td colspan="5" style="color: var(--danger); text-align: center; padding: 20px;">Lỗi: ${escapeHtml(err.message)}</td></tr>`;
    }
  }

  adminQuestionLangSelect.addEventListener('change', () => {
    populateAdminLevels(adminQuestionLangSelect.value, adminQuestionLevelSelect);
  });

  btnAdminAddQuestion.addEventListener('click', async () => {
    adminEditingQuestionId = null;
    document.getElementById('admin-question-modal-title').textContent = 'Thêm Câu Hỏi Mới';
    adminQuestionForm.reset();
    adminQuestionLangSelect.value = adminQuestionsLangSelect.value;
    await populateAdminLevels(adminQuestionLangSelect.value, adminQuestionLevelSelect);
    adminQuestionModal.classList.add('active');
  });

  async function editQuestion(id) {
    const q = adminLoadedQuestions.find(item => item.id === id);
    if (!q) return;

    adminEditingQuestionId = id;
    document.getElementById('admin-question-modal-title').textContent = 'Sửa Câu Hỏi Trắc Nghiệm';
    adminQuestionLangSelect.value = adminQuestionsLangSelect.value;
    await populateAdminLevels(adminQuestionLangSelect.value, adminQuestionLevelSelect);

    document.getElementById('admin-question-text').value = q.text;
    document.getElementById('admin-question-explanation').value = q.explanation || '';

    const optInputs = adminQuestionForm.querySelectorAll('.opt-text-input');
    const radioInputs = adminQuestionForm.querySelectorAll('input[name="admin-correct-opt"]');

    q.options.forEach((opt, idx) => {
      if (optInputs[idx]) optInputs[idx].value = opt.text;
      if (opt.isCorrect && radioInputs[idx]) radioInputs[idx].checked = true;
    });

    adminQuestionModal.classList.add('active');
  }

  async function deleteQuestion(id, text) {
    if (!confirm(`Bạn có chắc muốn xóa câu hỏi "${text}"?`)) return;
    try {
      await API.deleteAdminQuestion(id);
      showToast('Đã xóa câu hỏi thành công!', 'success');
      loadAdminQuestions();
    } catch (err) {
      showToast(err.message, 'error');
    }
  }

  function closeAdminQuestionModal() {
    adminQuestionModal.classList.remove('active');
    adminEditingQuestionId = null;
  }
  btnCloseQuestionModal.addEventListener('click', closeAdminQuestionModal);
  btnCancelQuestionModal.addEventListener('click', closeAdminQuestionModal);

  adminQuestionForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    const langId = parseInt(adminQuestionLangSelect.value, 10);
    const levelId = parseInt(adminQuestionLevelSelect.value, 10);
    const text = document.getElementById('admin-question-text').value.trim();
    const explanation = document.getElementById('admin-question-explanation').value.trim();

    const optInputs = adminQuestionForm.querySelectorAll('.opt-text-input');
    const checkedRadio = adminQuestionForm.querySelector('input[name="admin-correct-opt"]:checked');
    const correctIdx = checkedRadio ? parseInt(checkedRadio.value, 10) : 0;

    const options = [];
    optInputs.forEach((inp, idx) => {
      const val = inp.value.trim();
      if (val) {
        options.push({
          text: val,
          isCorrect: idx === correctIdx
        });
      }
    });

    if (options.length < 2) {
      showToast('Cần điền ít nhất 2 đáp án lựa chọn!', 'error');
      return;
    }

    try {
      if (adminEditingQuestionId) {
        await API.updateAdminQuestion(adminEditingQuestionId, {
          levelId,
          lessonId: null,
          text,
          explanation,
          options
        });
        showToast('Cập nhật câu hỏi thành công!', 'success');
      } else {
        await API.createAdminQuestion({
          languageId: langId,
          levelId,
          lessonId: null,
          text,
          explanation,
          options
        });
        showToast('Tạo câu hỏi trắc nghiệm mới thành công!', 'success');
      }

      closeAdminQuestionModal();
      loadAdminQuestions();
    } catch (err) {
      showToast(err.message, 'error');
    }
  });

  // Export Questions CSV
  btnAdminExportQuestionsCsv.addEventListener('click', () => {
    if (adminLoadedQuestions.length === 0) {
      showToast('Chưa có câu hỏi nào để xuất.', 'info');
      return;
    }

    let csv = '\uFEFFQuestion,CorrectOption,Option2,Option3,Option4,Explanation\n';
    adminLoadedQuestions.forEach(q => {
      const correct = q.options.find(o => o.isCorrect)?.text || '';
      const wrong = q.options.filter(o => !o.isCorrect).map(o => o.text);
      csv += `"${q.text.replace(/"/g, '""')}","${correct.replace(/"/g, '""')}","${(wrong[0] || '').replace(/"/g, '""')}","${(wrong[1] || '').replace(/"/g, '""')}","${(wrong[2] || '').replace(/"/g, '""')}","${(q.explanation || '').replace(/"/g, '""')}"\n`;
    });

    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `PolyLearn_Questions_Lang${adminQuestionsLangSelect.value}.csv`;
    link.click();
    URL.revokeObjectURL(url);
    showToast('Đã xuất ngân hàng đề thi thành công!', 'success');
  });

  // ----- D. Admin Users Management -----
  const adminUsersSearch = document.getElementById('admin-users-search');
  const adminUsersTbody = document.getElementById('admin-users-tbody');
  const btnAdminRefreshUsers = document.getElementById('btn-admin-refresh-users');

  adminUsersSearch.addEventListener('input', debounce(loadAdminUsers, 350));
  btnAdminRefreshUsers.addEventListener('click', loadAdminUsers);

  async function loadAdminUsers() {
    adminUsersTbody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-muted); padding: 30px;">⏳ Đang nạp danh sách người dùng...</td></tr>';
    try {
      const res = await API.getAdminUsers({
        search: adminUsersSearch.value.trim(),
        pageSize: 50
      });

      const users = res.items || [];
      if (users.length === 0) {
        adminUsersTbody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-muted); padding: 30px;">Không tìm thấy người dùng nào.</td></tr>';
        return;
      }

      adminUsersTbody.innerHTML = '';
      users.forEach(u => {
        const isAdmin = u.role === 'Admin';
        const isActive = u.isActive;
        const createdDate = new Date(u.createdAt).toLocaleDateString('vi-VN');

        const tr = document.createElement('tr');
        tr.innerHTML = `
          <td style="font-weight: 700; color: var(--text-primary);">${escapeHtml(u.displayName)}</td>
          <td style="color: var(--text-secondary);">${escapeHtml(u.email)}</td>
          <td>
            <span class="card-tag" style="background: ${isAdmin ? 'rgba(236,72,153,0.15)' : 'var(--accent-soft)'}; color: ${isAdmin ? '#db2777' : 'var(--accent-primary)'}; border-color: ${isAdmin ? 'rgba(236,72,153,0.35)' : 'var(--border-highlight)'}; font-size: 0.75rem;">
              ${isAdmin ? '👑 Admin' : '👤 Học viên'}
            </span>
          </td>
          <td>
            <span class="card-tag" style="background: ${isActive ? 'rgba(16,185,129,0.15)' : 'rgba(239,68,68,0.14)'}; color: ${isActive ? 'var(--success-text)' : 'var(--danger)'}; border-color: ${isActive ? 'rgba(16,185,129,0.35)' : 'rgba(239,68,68,0.35)'}; font-size: 0.75rem;">
              ${isActive ? '● Đang hoạt động' : '🔒 Đã bị khóa'}
            </span>
          </td>
          <td style="color: var(--text-muted); font-size: 0.85rem;">${createdDate}</td>
          <td style="text-align: right; white-space: nowrap;">
            <button class="btn-action-sm btn-toggle-status" data-id="${u.id}" data-active="${isActive}">
              ${isActive ? '🔒 Khóa' : '🔓 Mở khóa'}
            </button>
            <button class="btn-action-sm btn-toggle-role" data-id="${u.id}" data-role="${u.role}" style="margin-left: 6px;">
              ${isAdmin ? 'Hạ quyền User' : 'Nâng quyền Admin'}
            </button>
          </td>
        `;

        tr.querySelector('.btn-toggle-status').addEventListener('click', async () => {
          try {
            await API.updateUserStatus(u.id, !isActive);
            showToast(`Đã ${!isActive ? 'mở khóa' : 'khóa'} tài khoản ${u.email}!`, 'success');
            loadAdminUsers();
          } catch (err) {
            showToast(err.message, 'error');
          }
        });

        tr.querySelector('.btn-toggle-role').addEventListener('click', async () => {
          const newRole = isAdmin ? 0 : 1; // 0 = User, 1 = Admin
          try {
            await API.updateUserRole(u.id, newRole);
            showToast(`Đã thay đổi quyền cho ${u.email}!`, 'success');
            loadAdminUsers();
          } catch (err) {
            showToast(err.message, 'error');
          }
        });

        adminUsersTbody.appendChild(tr);
      });
    } catch (err) {
      adminUsersTbody.innerHTML = `<tr><td colspan="6" style="color: var(--danger); text-align: center; padding: 20px;">Lỗi: ${escapeHtml(err.message)}</td></tr>`;
    }
  }

  // Helpers
  function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
  }

  function debounce(fn, delay) {
    let timer = null;
    return function (...args) {
      clearTimeout(timer);
      timer = setTimeout(() => fn.apply(this, args), delay);
    };
  }

  // Initial Load
  renderAuthStatus();
  populateLessonLevels().finally(() => {
    switchView('lessons');
  });
  // Prefill top stats + sidebar streak even when the dashboard isn't open
  if (API.getToken()) {
    API.getDashboardSummary(state.activeLanguageId).then(summary => {
      if (!summary) return;
      syncRibbonMetrics({ streak: summary.streak?.currentStreakDays || 0 });
      renderSideWeek(summary.streak?.activeDatesInLastTwoWeeks || []);
    }).catch(() => {});
  }
  if (typeof Gamification !== 'undefined' && Gamification.initInteractiveEffects) {
    Gamification.initInteractiveEffects(state.activeLanguageId);
  }
});
