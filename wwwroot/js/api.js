const API = {
  baseUrl: window.location.origin,

  getToken() {
    return localStorage.getItem('polylearn_token') || '';
  },

  setToken(token) {
    if (token) localStorage.setItem('polylearn_token', token);
    else localStorage.removeItem('polylearn_token');
  },

  getUser() {
    try {
      const raw = localStorage.getItem('polylearn_user');
      if (!raw || raw === 'null' || raw === 'undefined') return null;
      return JSON.parse(raw);
    } catch {
      return null;
    }
  },

  setUser(user) {
    if (user) localStorage.setItem('polylearn_user', JSON.stringify(user));
    else localStorage.removeItem('polylearn_user');
  },

  getActiveLanguageId() {
    return parseInt(localStorage.getItem('polylearn_lang_id') || '1', 10);
  },

  setActiveLanguageId(id) {
    localStorage.setItem('polylearn_lang_id', id.toString());
  },

  async request(endpoint, options = {}) {
    const headers = {
      'Content-Type': 'application/json',
      'ngrok-skip-browser-warning': 'true',
      ...(options.headers || {})
    };

    const token = this.getToken();
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    try {
      const res = await fetch(`${this.baseUrl}${endpoint}`, {
        ...options,
        headers
      });

      if (res.status === 401) {
        this.setToken(null);
        this.setUser(null);
        window.dispatchEvent(new CustomEvent('auth-changed'));
        const errData = await res.json().catch(() => null);
        throw new Error(errData?.message || (endpoint.includes('/auth/login') ? 'Email hoặc mật khẩu không chính xác.' : 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.'));
      }

      const data = await res.json().catch(() => null);

      if (!res.ok) {
        const errorMsg = data?.message || data?.detail || `Lỗi yêu cầu (${res.status})`;
        throw new Error(errorMsg);
      }

      if (data === null && res.ok) {
        throw new Error('Không nhận được dữ liệu phản hồi từ máy chủ.');
      }

      return data;
    } catch (err) {
      console.error(`API Error [${endpoint}]:`, err);
      throw err;
    }
  },

  // Auth
  login(email, password) {
    return this.request('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password })
    });
  },

  register(email, password, displayName, preferredLanguageId) {
    return this.request('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify({ email, password, displayName, preferredLanguageId })
    });
  },

  getMe() {
    return this.request('/api/auth/me');
  },

  updateProfile(data) {
    return this.request('/api/auth/profile', {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  },

  changePassword(currentPassword, newPassword) {
    return this.request('/api/auth/change-password', {
      method: 'PUT',
      body: JSON.stringify({ currentPassword, newPassword })
    });
  },

  // Languages
  getLanguages() {
    return this.request('/api/languages');
  },

  getLanguage(id) {
    return this.request(`/api/languages/${id}`);
  },

  // Dashboard
  getDashboardSummary(langId) {
    const q = langId ? `?languageId=${langId}` : '';
    return this.request(`/api/dashboard/summary${q}`);
  },

  // Words
  getWords(params = {}) {
    const q = new URLSearchParams(params).toString();
    return this.request(`/api/words?${q}`);
  },

  getWordDetail(id) {
    return this.request(`/api/words/${id}`);
  },

  toggleFavorite(id, isFav) {
    return this.request(`/api/words/${id}/favorite`, {
      method: isFav ? 'POST' : 'DELETE'
    });
  },

  getFavorites(page = 1, pageSize = 30) {
    return this.request(`/api/words/favorites?pageNumber=${page}&pageSize=${pageSize}`);
  },

  harvestWords({ languageId, count = 8, topicHint = null, levelId = null } = {}) {
    return this.request('/api/words/harvest', {
      method: 'POST',
      body: JSON.stringify({ languageId, count, topicHint, levelId })
    });
  },

  // Flashcards Leitner & Endless Random
  getDueFlashcards(languageId, limit = 20) {
    return this.request(`/api/flashcards/due?languageId=${languageId}&limit=${limit}`);
  },

  getRandomFlashcards({ languageId, limit = 15, levelId = null, harvestNew = false, newCount = 8, topicHint = null } = {}) {
    const params = new URLSearchParams({ languageId, limit, harvestNew, newCount });
    if (levelId) params.set('levelId', levelId);
    if (topicHint) params.set('topicHint', topicHint);
    return this.request(`/api/flashcards/random?${params.toString()}`);
  },

  reviewCard(wordId, isRemembered) {
    return this.request('/api/flashcards/review', {
      method: 'POST',
      body: JSON.stringify({ wordId, isRemembered })
    });
  },

  getFlashcardStats(languageId) {
    return this.request(`/api/flashcards/stats?languageId=${languageId}`);
  },

  // Quizzes
  generateQuiz(languageIdOrPayload, questionCount = 10, levelId = null) {
    const payload = typeof languageIdOrPayload === 'object' && languageIdOrPayload !== null
      ? languageIdOrPayload
      : { languageId: languageIdOrPayload, questionCount, levelId };
    return this.request('/api/quizzes/generate', {
      method: 'POST',
      body: JSON.stringify(payload)
    });
  },

  submitQuiz(attemptId, answers) {
    return this.request(`/api/quizzes/${attemptId}/submit`, {
      method: 'POST',
      body: JSON.stringify({ answers })
    });
  },

  getQuizHistory(page = 1) {
    return this.request(`/api/quizzes/history?pageNumber=${page}&pageSize=10`);
  },

  // Lessons
  getLessons(params = {}) {
    const q = new URLSearchParams(params).toString();
    return this.request(`/api/lessons?${q}`);
  },

  getLessonDetail(id) {
    return this.request(`/api/lessons/${id}`);
  },

  completeLesson(id) {
    return this.request(`/api/lessons/${id}/complete`, { method: 'POST' });
  },

  // AI Assistant
  checkGrammar(text, languageCode) {
    return this.request('/api/ai/check-grammar', {
      method: 'POST',
      body: JSON.stringify({ text, languageCode })
    });
  },

  explainWord(term, languageCode) {
    return this.request('/api/ai/explain-word', {
      method: 'POST',
      body: JSON.stringify({ term, languageCode })
    });
  },

  chatAi(messages, languageCode, scenario = 'casual_talk') {
    return this.request('/api/ai/chat', {
      method: 'POST',
      body: JSON.stringify({ messages, languageCode, scenario })
    });
  },

  // ---------- Admin APIs ----------
  // Words
  createAdminWord(data) {
    return this.request('/api/admin/words', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  },

  batchCreateAdminWords(requests) {
    return this.request('/api/admin/words/batch', {
      method: 'POST',
      body: JSON.stringify(requests)
    });
  },

  updateAdminWord(id, data) {
    return this.request(`/api/admin/words/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  },

  deleteAdminWord(id) {
    return this.request(`/api/admin/words/${id}`, { method: 'DELETE' });
  },

  getAdminPresets() {
    return this.request('/api/admin/words/presets');
  },

  importAdminPreset(datasetKey) {
    return this.request(`/api/admin/words/presets/import?datasetKey=${encodeURIComponent(datasetKey)}`, {
      method: 'POST'
    });
  },

  // Lessons
  createAdminLesson(data) {
    return this.request('/api/admin/lessons', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  },

  updateAdminLesson(id, data) {
    return this.request(`/api/admin/lessons/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  },

  deleteAdminLesson(id) {
    return this.request(`/api/admin/lessons/${id}`, { method: 'DELETE' });
  },

  // Questions
  getAdminQuestions(params = {}) {
    const q = new URLSearchParams(params).toString();
    return this.request(`/api/admin/questions?${q}`);
  },

  createAdminQuestion(data) {
    return this.request('/api/admin/questions', {
      method: 'POST',
      body: JSON.stringify(data)
    });
  },

  updateAdminQuestion(id, data) {
    return this.request(`/api/admin/questions/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data)
    });
  },

  deleteAdminQuestion(id) {
    return this.request(`/api/admin/questions/${id}`, { method: 'DELETE' });
  },

  // Users
  getAdminUsers(params = {}) {
    const q = new URLSearchParams(params).toString();
    return this.request(`/api/admin/users?${q}`);
  },

  updateUserStatus(id, isActive) {
    return this.request(`/api/admin/users/${id}/status`, {
      method: 'PUT',
      body: JSON.stringify({ isActive })
    });
  },

  updateUserRole(id, role) {
    return this.request(`/api/admin/users/${id}/role`, {
      method: 'PUT',
      body: JSON.stringify({ role })
    });
  }
};
