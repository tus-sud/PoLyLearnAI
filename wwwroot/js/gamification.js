// =========================================================================
// PolyLearn AI - Mascots, Avatars & Gamification Visual/Audio Effects Engine
// =========================================================================

const Gamification = (() => {
  // ---------- 1. Custom Expressive SVG Characters ----------
  const MASCOT_SVGS = {
    owl: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <defs>
          <linearGradient id="owlBody" x1="0" y1="0" x2="1" y2="1">
            <stop offset="0%" stop-color="#818cf8"/>
            <stop offset="100%" stop-color="#4f46e5"/>
          </linearGradient>
        </defs>
        <!-- Wings -->
        <ellipse cx="22" cy="72" rx="12" ry="22" fill="#4338ca" class="mascot-wing-left"/>
        <ellipse cx="98" cy="72" rx="12" ry="22" fill="#4338ca" class="mascot-wing-right"/>
        <!-- Body -->
        <ellipse cx="60" cy="68" rx="38" ry="40" fill="url(#owlBody)"/>
        <ellipse cx="60" cy="78" rx="25" ry="26" fill="#e0e7ff"/>
        <!-- Belly feathers -->
        <path d="M48 72 Q52 77 56 72 M64 72 Q68 77 72 72 M54 83 Q60 89 66 83" stroke="#818cf8" stroke-width="2.5" fill="none" stroke-linecap="round"/>
        <!-- Ear tufts -->
        <polygon points="28,38 36,20 46,35" fill="#4f46e5"/>
        <polygon points="92,38 84,20 74,35" fill="#4f46e5"/>
        <!-- Eyes & Glasses -->
        <circle cx="45" cy="52" r="14" fill="#ffffff"/>
        <circle cx="75" cy="52" r="14" fill="#ffffff"/>
        <g class="mascot-pupils">
          <circle cx="46" cy="52" r="7" fill="#1e1b4b"/>
          <circle cx="74" cy="52" r="7" fill="#1e1b4b"/>
          <circle cx="48" cy="49" r="2.5" fill="#ffffff"/>
          <circle cx="76" cy="49" r="2.5" fill="#ffffff"/>
        </g>
        <circle cx="45" cy="52" r="14.5" fill="none" stroke="#fbbf24" stroke-width="2.8"/>
        <circle cx="75" cy="52" r="14.5" fill="none" stroke="#fbbf24" stroke-width="2.8"/>
        <line x1="59" y1="52" x2="61" y2="52" stroke="#fbbf24" stroke-width="2.8"/>
        <!-- Beak & Cheeks -->
        <polygon points="55,58 65,58 60,67" fill="#f59e0b"/>
        <ellipse cx="32" cy="62" rx="5" ry="3" fill="#f472b6" opacity="0.65"/>
        <ellipse cx="88" cy="62" rx="5" ry="3" fill="#f472b6" opacity="0.65"/>
        <!-- Graduation Cap -->
        <polygon points="60,10 26,24 60,36 94,24" fill="#1e293b" stroke="#fbbf24" stroke-width="1.5"/>
        <line x1="88" y1="25" x2="92" y2="40" stroke="#fbbf24" stroke-width="2.5"/>
        <circle cx="92" cy="41" r="3" fill="#fbbf24"/>
        <!-- Feet -->
        <ellipse cx="47" cy="108" rx="8" ry="4" fill="#f59e0b"/>
        <ellipse cx="73" cy="108" rx="8" ry="4" fill="#f59e0b"/>
      </svg>`,

    panda: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <!-- Ears -->
        <circle cx="30" cy="28" r="14" fill="#1f2937"/>
        <circle cx="90" cy="28" r="14" fill="#1f2937"/>
        <!-- Arms -->
        <ellipse cx="22" cy="76" rx="11" ry="18" fill="#1f2937" class="mascot-wing-left"/>
        <ellipse cx="98" cy="76" rx="11" ry="18" fill="#1f2937" class="mascot-wing-right"/>
        <!-- Body & Head -->
        <ellipse cx="60" cy="84" rx="34" ry="26" fill="#f8fafc"/>
        <circle cx="60" cy="54" r="36" fill="#ffffff" stroke="#e2e8f0" stroke-width="1.5"/>
        <!-- Red HSK Headband -->
        <path d="M25 42 Q60 32 95 42 L94 49 Q60 39 26 49 Z" fill="#ef4444"/>
        <circle cx="60" cy="40" r="4.5" fill="#fbbf24"/>
        <!-- Eye patches -->
        <ellipse cx="44" cy="56" rx="11" ry="13" transform="rotate(-12 44 56)" fill="#1f2937"/>
        <ellipse cx="76" cy="56" rx="11" ry="13" transform="rotate(12 76 56)" fill="#1f2937"/>
        <g class="mascot-pupils">
          <circle cx="45" cy="55" r="5" fill="#ffffff"/>
          <circle cx="75" cy="55" r="5" fill="#ffffff"/>
          <circle cx="46" cy="55" r="2.6" fill="#0f172a"/>
          <circle cx="74" cy="55" r="2.6" fill="#0f172a"/>
        </g>
        <!-- Nose & Smile -->
        <ellipse cx="60" cy="65" rx="5" ry="3.5" fill="#0f172a"/>
        <path d="M53 71 Q60 77 67 71" stroke="#0f172a" stroke-width="2.5" fill="none" stroke-linecap="round"/>
        <!-- Rosy cheeks -->
        <ellipse cx="32" cy="67" rx="6" ry="3.5" fill="#fb7185" opacity="0.65"/>
        <ellipse cx="88" cy="67" rx="6" ry="3.5" fill="#fb7185" opacity="0.65"/>
        <!-- Bamboo leaf accent -->
        <path d="M95 66 Q108 58 106 72 Q98 74 95 66 Z" fill="#10b981"/>
        <!-- Feet -->
        <ellipse cx="44" cy="106" rx="10" ry="6" fill="#1f2937"/>
        <ellipse cx="76" cy="106" rx="10" ry="6" fill="#1f2937"/>
      </svg>`,

    neko: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <!-- Cat Ears -->
        <polygon points="26,50 28,16 52,34" fill="#fffbeb" stroke="#fde68a" stroke-width="2"/>
        <polygon points="31,44 32,23 47,35" fill="#fda4af"/>
        <polygon points="94,50 92,16 68,34" fill="#fffbeb" stroke="#fde68a" stroke-width="2"/>
        <polygon points="89,44 88,23 73,35" fill="#fda4af"/>
        <!-- Body -->
        <ellipse cx="60" cy="84" rx="33" ry="26" fill="#fffbeb"/>
        <!-- Waving Paw (Maneki) -->
        <ellipse cx="23" cy="64" rx="10" ry="15" fill="#fffbeb" stroke="#fde68a" stroke-width="2" class="mascot-wing-left"/>
        <circle cx="23" cy="58" r="4" fill="#fda4af" class="mascot-wing-left"/>
        <ellipse cx="95" cy="78" rx="9" ry="14" fill="#fffbeb" stroke="#fde68a" stroke-width="2" class="mascot-wing-right"/>
        <!-- Head -->
        <ellipse cx="60" cy="56" rx="36" ry="31" fill="#fffbeb" stroke="#fde68a" stroke-width="1.8"/>
        <!-- Forehead markings -->
        <ellipse cx="60" cy="33" rx="6" ry="3" fill="#f59e0b" opacity="0.7"/>
        <!-- Eyes -->
        <g class="mascot-pupils">
          <circle cx="44" cy="55" r="6" fill="#1e293b"/>
          <circle cx="76" cy="55" r="6" fill="#1e293b"/>
          <circle cx="46" cy="53" r="2.2" fill="#ffffff"/>
          <circle cx="78" cy="53" r="2.2" fill="#ffffff"/>
        </g>
        <!-- Nose, Mouth & Whiskers -->
        <polygon points="57,62 63,62 60,66" fill="#fb7185"/>
        <path d="M52 68 Q56 73 60 68 Q64 73 68 68" stroke="#1e293b" stroke-width="2.2" fill="none" stroke-linecap="round"/>
        <line x1="18" y1="58" x2="31" y2="60" stroke="#94a3b8" stroke-width="1.8" stroke-linecap="round"/>
        <line x1="18" y1="65" x2="31" y2="65" stroke="#94a3b8" stroke-width="1.8" stroke-linecap="round"/>
        <line x1="89" y1="60" x2="102" y2="58" stroke="#94a3b8" stroke-width="1.8" stroke-linecap="round"/>
        <line x1="89" y1="65" x2="102" y2="65" stroke="#94a3b8" stroke-width="1.8" stroke-linecap="round"/>
        <ellipse cx="34" cy="64" rx="5.5" ry="3" fill="#fda4af" opacity="0.75"/>
        <ellipse cx="86" cy="64" rx="5.5" ry="3" fill="#fda4af" opacity="0.75"/>
        <!-- Red Collar & Golden Bell -->
        <path d="M34 80 Q60 88 86 80" stroke="#ef4444" stroke-width="6" fill="none" stroke-linecap="round"/>
        <circle cx="60" cy="86" r="7" fill="#fbbf24" stroke="#d97706" stroke-width="1.5"/>
        <circle cx="60" cy="88" r="1.8" fill="#78350f"/>
      </svg>`,

    tiger: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <!-- Ears -->
        <circle cx="30" cy="28" r="13" fill="#f97316"/>
        <circle cx="30" cy="28" r="7" fill="#fed7aa"/>
        <circle cx="90" cy="28" r="13" fill="#f97316"/>
        <circle cx="90" cy="28" r="7" fill="#fed7aa"/>
        <!-- Arms -->
        <ellipse cx="22" cy="76" rx="10" ry="17" fill="#f97316" class="mascot-wing-left"/>
        <ellipse cx="98" cy="76" rx="10" ry="17" fill="#f97316" class="mascot-wing-right"/>
        <!-- Body & Head -->
        <ellipse cx="60" cy="84" rx="33" ry="25" fill="#fb923c"/>
        <ellipse cx="60" cy="86" rx="20" ry="18" fill="#ffedd5"/>
        <ellipse cx="60" cy="56" rx="37" ry="32" fill="#fb923c"/>
        <!-- Tiger Stripes -->
        <path d="M52 28 L68 28 M55 34 L65 34 M60 25 L60 38" stroke="#7c2d12" stroke-width="3.2" stroke-linecap="round"/>
        <path d="M24 52 L32 54 M25 60 L33 60 M96 52 L88 54 M95 60 L87 60" stroke="#7c2d12" stroke-width="3" stroke-linecap="round"/>
        <!-- Muzzle -->
        <ellipse cx="60" cy="66" rx="18" ry="12" fill="#ffedd5"/>
        <!-- Eyes -->
        <g class="mascot-pupils">
          <circle cx="44" cy="53" r="6" fill="#1e293b"/>
          <circle cx="76" cy="53" r="6" fill="#1e293b"/>
          <circle cx="46" cy="51" r="2.2" fill="#ffffff"/>
          <circle cx="78" cy="51" r="2.2" fill="#ffffff"/>
        </g>
        <!-- Nose & Energetic Grin -->
        <polygon points="56,61 64,61 60,66" fill="#ea580c"/>
        <path d="M51 69 Q60 76 69 69" stroke="#7c2d12" stroke-width="2.5" fill="none" stroke-linecap="round"/>
        <ellipse cx="35" cy="64" rx="5" ry="3" fill="#fb7185" opacity="0.65"/>
        <ellipse cx="85" cy="64" rx="5" ry="3" fill="#fb7185" opacity="0.65"/>
        <!-- Feet -->
        <ellipse cx="45" cy="107" rx="9" ry="5" fill="#ea580c"/>
        <ellipse cx="75" cy="107" rx="9" ry="5" fill="#ea580c"/>
      </svg>`,

    fox: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <polygon points="22,52 24,12 52,34" fill="#ec4899"/>
        <polygon points="28,46 29,21 47,35" fill="#fbcfe8"/>
        <polygon points="98,52 96,12 68,34" fill="#ec4899"/>
        <polygon points="92,46 91,21 73,35" fill="#fbcfe8"/>
        <ellipse cx="60" cy="85" rx="31" ry="24" fill="#f472b6"/>
        <ellipse cx="60" cy="87" rx="19" ry="17" fill="#fdf2f8"/>
        <ellipse cx="60" cy="56" rx="36" ry="30" fill="#f472b6"/>
        <path d="M25 60 Q42 84 60 76 Q78 84 95 60 Q82 70 60 66 Q38 70 25 60 Z" fill="#fdf2f8"/>
        <polygon points="60,30 63,37 60,42 57,37" fill="#38bdf8"/>
        <g class="mascot-pupils">
          <circle cx="44" cy="54" r="5.8" fill="#1e1b4b"/>
          <circle cx="76" cy="54" r="5.8" fill="#1e1b4b"/>
          <circle cx="46" cy="52" r="2.2" fill="#ffffff"/>
          <circle cx="78" cy="52" r="2.2" fill="#ffffff"/>
        </g>
        <circle cx="60" cy="64" r="3.8" fill="#1e1b4b"/>
        <path d="M54 70 Q60 75 66 70" stroke="#1e1b4b" stroke-width="2.2" fill="none" stroke-linecap="round"/>
      </svg>`,

    dragon: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <!-- Horns & Wings -->
        <polygon points="36,28 28,10 46,24" fill="#fbbf24"/>
        <polygon points="84,28 92,10 74,24" fill="#fbbf24"/>
        <path d="M24 68 L8 54 L14 78 L26 82 Z" fill="#06b6d4" class="mascot-wing-left"/>
        <path d="M96 68 L112 54 L106 78 L94 82 Z" fill="#06b6d4" class="mascot-wing-right"/>
        <!-- Body & Head -->
        <ellipse cx="60" cy="84" rx="32" ry="25" fill="#10b981"/>
        <ellipse cx="60" cy="86" rx="19" ry="18" fill="#a7f3d0"/>
        <ellipse cx="60" cy="55" rx="35" ry="30" fill="#10b981"/>
        <!-- Eyes -->
        <g class="mascot-pup pupils">
          <circle cx="44" cy="52" r="6.2" fill="#064e3b"/>
          <circle cx="76" cy="52" r="6.2" fill="#064e3b"/>
          <circle cx="46" cy="50" r="2.4" fill="#ffffff"/>
          <circle cx="78" cy="50" r="2.4" fill="#ffffff"/>
        </g>
        <!-- Nostrils & Smile -->
        <circle cx="55" cy="62" r="2" fill="#065f46"/>
        <circle cx="65" cy="62" r="2" fill="#065f46"/>
        <path d="M50 69 Q60 77 70 69" stroke="#064e3b" stroke-width="2.6" fill="none" stroke-linecap="round"/>
        <ellipse cx="34" cy="62" rx="5.5" ry="3" fill="#fde047" opacity="0.65"/>
        <ellipse cx="86" cy="62" rx="5.5" ry="3" fill="#fde047" opacity="0.65"/>
      </svg>`,

    ghost: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <!-- Cute White 3TG Mochi Ghost -->
        <path d="M60 22 C36 22 28 42 28 72 C28 98 40 102 52 100 C58 99 62 102 68 102 C78 102 82 98 92 98 C96 98 92 85 92 72 C92 42 84 22 60 22 Z" fill="#ffffff" stroke="#1f2937" stroke-width="2.8" stroke-linejoin="round"/>
        <!-- Rosy cheeks -->
        <ellipse cx="38" cy="66" rx="6.5" ry="4" fill="#fb7185" opacity="0.75"/>
        <ellipse cx="82" cy="66" rx="6.5" ry="4" fill="#fb7185" opacity="0.75"/>
        <!-- Happy curved eyes -->
        <path d="M40 56 Q46 48 52 56" stroke="#1f2937" stroke-width="3.2" fill="none" stroke-linecap="round"/>
        <path d="M68 56 Q74 48 80 56" stroke="#1f2937" stroke-width="3.2" fill="none" stroke-linecap="round"/>
        <!-- Cute mouth -->
        <path d="M57 65 Q60 69 63 65" stroke="#1f2937" stroke-width="2.5" fill="none" stroke-linecap="round"/>
        <!-- Bowl and chopsticks -->
        <ellipse cx="60" cy="86" rx="17" ry="11" fill="#f43f5e" stroke="#1f2937" stroke-width="2.2"/>
        <path d="M43 86 Q60 102 77 86" fill="#e11d48" stroke="#1f2937" stroke-width="2.2"/>
        <!-- Golden food noodles inside bowl -->
        <ellipse cx="60" cy="84" rx="13" ry="6" fill="#fef08a"/>
        <!-- Left arm -->
        <ellipse cx="40" cy="83" rx="4.5" ry="5.5" fill="#ffffff" stroke="#1f2937" stroke-width="2.2"/>
        <!-- Right arm -->
        <ellipse cx="78" cy="80" rx="4.5" ry="5.5" fill="#ffffff" stroke="#1f2937" stroke-width="2.2"/>
        <!-- Chopsticks -->
        <line x1="74" y1="83" x2="52" y2="76" stroke="#92400e" stroke-width="2.6" stroke-linecap="round"/>
        <line x1="76" y1="87" x2="55" y2="80" stroke="#92400e" stroke-width="2.6" stroke-linecap="round"/>
      </svg>`
  };

  // Extra Avatar options (uses mascot SVGs + Shiba & Astronaut)
  const EXTRA_AVATAR_SVGS = {
    shiba: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <polygon points="26,48 28,16 52,34" fill="#f59e0b"/>
        <polygon points="94,48 92,16 68,34" fill="#f59e0b"/>
        <circle cx="60" cy="60" r="38" fill="#fbbf24"/>
        <ellipse cx="60" cy="70" rx="26" ry="22" fill="#fef3c7"/>
        <circle cx="45" cy="46" r="4" fill="#fef3c7"/>
        <circle cx="75" cy="46" r="4" fill="#fef3c7"/>
        <circle cx="45" cy="56" r="5.5" fill="#1e293b"/>
        <circle cx="75" cy="56" r="5.5" fill="#1e293b"/>
        <ellipse cx="60" cy="65" rx="5" ry="3.8" fill="#1e293b"/>
        <path d="M52 72 Q60 79 68 72" stroke="#1e293b" stroke-width="2.5" fill="none" stroke-linecap="round"/>
      </svg>`,
    astronaut: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <circle cx="60" cy="60" r="40" fill="#e2e8f0" stroke="#94a3b8" stroke-width="4"/>
        <rect x="30" y="38" width="60" height="42" rx="20" fill="#1e1b4b" stroke="#38bdf8" stroke-width="2.5"/>
        <path d="M38 46 Q52 40 66 44" stroke="#bae6fd" stroke-width="3" fill="none" stroke-linecap="round" opacity="0.8"/>
        <circle cx="47" cy="59" r="4" fill="#38bdf8"/>
        <circle cx="73" cy="59" r="4" fill="#38bdf8"/>
        <path d="M54 67 Q60 72 66 67" stroke="#38bdf8" stroke-width="2.2" fill="none" stroke-linecap="round"/>
        <line x1="60" y1="20" x2="60" y2="10" stroke="#94a3b8" stroke-width="3"/>
        <circle cx="60" cy="9" r="4" fill="#ec4899"/>
      </svg>`,
    ninja: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <path d="M85 35 Q110 30 115 45 Q105 48 90 42 Z" fill="#ef4444"/>
        <path d="M88 40 Q112 45 110 60 Q100 56 90 48 Z" fill="#dc2626"/>
        <circle cx="60" cy="62" r="38" fill="#1e293b" stroke="#0f172a" stroke-width="2.5"/>
        <rect x="23" y="32" width="74" height="15" rx="6" fill="#ef4444"/>
        <circle cx="60" cy="39" r="4.5" fill="#fef08a"/>
        <rect x="34" y="47" width="52" height="18" rx="7" fill="#fde68a"/>
        <ellipse cx="46" cy="56" rx="5.5" ry="4" fill="#0f172a"/>
        <ellipse cx="74" cy="56" rx="5.5" ry="4" fill="#0f172a"/>
        <circle cx="48" cy="54" r="2" fill="#ffffff"/>
        <circle cx="76" cy="54" r="2" fill="#ffffff"/>
        <path d="M40 50 L52 53" stroke="#0f172a" stroke-width="2.5" stroke-linecap="round"/>
        <path d="M80 50 L68 53" stroke="#0f172a" stroke-width="2.5" stroke-linecap="round"/>
        <path d="M30 78 Q60 88 90 78" stroke="#ef4444" stroke-width="3" fill="none"/>
      </svg>`,
    wizard: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <circle cx="60" cy="66" r="34" fill="#6366f1" stroke="#4f46e5" stroke-width="2"/>
        <path d="M38 68 Q60 112 82 68 Q72 82 60 84 Q48 82 38 68 Z" fill="#f8fafc" stroke="#cbd5e1" stroke-width="1.5"/>
        <circle cx="48" cy="58" r="5" fill="#1e1b4b"/>
        <circle cx="72" cy="58" r="5" fill="#1e1b4b"/>
        <circle cx="50" cy="56" r="2.2" fill="#ffffff"/>
        <circle cx="74" cy="56" r="2.2" fill="#ffffff"/>
        <ellipse cx="38" cy="64" rx="4.5" ry="3" fill="#fda4af" opacity="0.8"/>
        <ellipse cx="82" cy="64" rx="4.5" ry="3" fill="#fda4af" opacity="0.8"/>
        <polygon points="60,10 22,46 98,46" fill="#4338ca"/>
        <path d="M16 46 Q60 40 104 46 Q60 52 16 46 Z" fill="#3730a3"/>
        <circle cx="60" cy="30" r="3.5" fill="#fbbf24"/>
        <polygon points="46,38 50,42 45,44" fill="#fbbf24"/>
        <polygon points="98,22 102,12 106,22 116,26 106,30 102,40 98,30 88,26" fill="#fef08a"/>
      </svg>`,
    crown: `
      <svg viewBox="0 0 120 120" class="mascot-svg" aria-hidden="true">
        <path d="M60 26 C28 26 16 54 16 76 C16 98 34 112 60 112 C86 112 104 98 104 76 C104 54 92 26 60 26 Z" fill="#d97706" opacity="0.45"/>
        <circle cx="60" cy="68" r="36" fill="#f59e0b"/>
        <circle cx="32" cy="46" r="10" fill="#f59e0b"/>
        <circle cx="32" cy="46" r="5.5" fill="#fed7aa"/>
        <circle cx="88" cy="46" r="10" fill="#f59e0b"/>
        <circle cx="88" cy="46" r="5.5" fill="#fed7aa"/>
        <circle cx="46" cy="64" r="5.5" fill="#1e1b4b"/>
        <circle cx="74" cy="64" r="5.5" fill="#1e1b4b"/>
        <circle cx="48" cy="62" r="2.2" fill="#ffffff"/>
        <circle cx="76" cy="62" r="2.2" fill="#ffffff"/>
        <ellipse cx="60" cy="74" rx="14" ry="9" fill="#fef3c7"/>
        <polygon points="56,70 64,70 60,75" fill="#78350f"/>
        <path d="M54 78 Q60 83 66 78" stroke="#78350f" stroke-width="2.2" fill="none" stroke-linecap="round"/>
        <polygon points="34,36 44,18 60,26 76,18 86,36" fill="#fbbf24" stroke="#d97706" stroke-width="2"/>
        <circle cx="44" cy="18" r="3" fill="#ef4444"/>
        <circle cx="60" cy="26" r="3.5" fill="#3b82f6"/>
        <circle cx="76" cy="18" r="3" fill="#ef4444"/>
        <rect x="36" y="34" width="48" height="6" rx="2" fill="#f59e0b"/>
      </svg>`
  };

  const COMPANIONS = {
    owl: {
      key: 'owl',
      name: 'Giáo sư Oliver',
      role: 'Gia sư Tiếng Anh Oxford',
      accent: 'var(--accent-primary)',
      greetings: [
        'Hello! Cùng chinh phục từ mới hôm nay nhé! ✨',
        'Every word counts! Cố lên bạn ơi! 🦉',
        'Làm ngay một đề Trắc nghiệm + Tự luận để não bộ siêu nhạy nào!'
      ],
      cheers: [
        'Brilliant! Trí nhớ của bạn đỉnh thật đấy! 🌟',
        'Awesome! Thêm điểm XP vào túi ngay! 🚀',
        'Spot on! Chuẩn như người bản xứ luôn!'
      ],
      encourages: [
        'Don’t worry! Sai một lần là nhớ cả đời đó 💪',
        'Cứ bình tĩnh xem gợi ý và lời giải AI bên dưới sẽ thuộc ngay!',
        'Kiên trì là chìa khóa của mọi Polyglot!'
      ]
    },
    panda: {
      key: 'panda',
      name: 'Võ sư BaoBao',
      role: 'Bạn đồng hành HSK',
      accent: '#e11d48',
      greetings: [
        '你好 (Nǐ hǎo)! Luyện phản xạ tiếng Trung thôi nào! 🎋',
        '加油 (Jiāyóu)! Mỗi ngày tích lũy thêm chữ Hán nhé!',
        'Hôm nay mình cùng rinh trọn mục tiêu XP nha!'
      ],
      cheers: [
        '太棒了 (Tài bàng le)! Tuyệt vời ông mặt trời! 🐼',
        'Chuẩn thanh điệu, chuẩn nghĩa luôn! +XP liền tay!',
        'Công phu ghi nhớ của bạn thăng tiến vùn vụt!'
      ],
      encourages: [
        '没关系 (Méi guānxi)! Không sao đâu, thử lại nhé! 💚',
        'Chữ Hán cần nhìn nhiều lần mới quen nét, cố lên!',
        'BaoBao luôn ở đây cổ vũ bạn mỗi ngày!'
      ]
    },
    neko: {
      key: 'neko',
      name: 'Mèo Mochi',
      role: 'Linh vật may mắn JLPT',
      accent: '#db2777',
      greetings: [
        'こんにちは! Mochi vẫy tay gọi may mắn cho bạn nè! 🐾',
        '頑張って (Ganbatte)! Cùng học tiếng Nhật thật vui nhé!',
        'Nhấp vào loa 🔊 để luyện nghe phát âm chuẩn nha!'
      ],
      cheers: [
        'すごい (Sugoi)! Đỉnh quá chừng luôn nya~ 🌸',
        'やった (Yatta)! Ghi điểm xuất sắc rồi!',
        'Chuông vàng rung vang chúc mừng bạn đó! 🔔'
      ],
      encourages: [
        '大丈夫 (Daijōbu)! Đừng nản lòng nhé nya~ 💖',
        'Lặp lại ngắt quãng sẽ giúp bạn nhớ siêu lâu!',
        'Hít một hơi thật sâu rồi sang câu tiếp theo nào!'
      ]
    },
    tiger: {
      key: 'tiger',
      name: 'Hổ con Horangi',
      role: 'Chiến binh TOPIK',
      accent: '#ea580c',
      greetings: [
        '안녕하세요! Nạp năng lượng học tiếng Hàn thôi! 🔥',
        '파이팅 (Fighting)! Tinh thần chiến binh bất bại!',
        'Giữ vững chuỗi Streak rực lửa cùng Horangi nhé!'
      ],
      cheers: [
        '대박 (Daebak)! Quá xuất sắc luôn bạn ơi! 🐯',
        'Tốc độ phản xạ đáng nể thật sự! ⚡',
        'Cứ đà này TOPIK cao nằm chắc trong tầm tay!'
      ],
      encourages: [
        '괜찮아요 (Gwaenchanayo)! Không sao cả, làm lại nào! 💪',
        'Hổ con vẫn tin tưởng bạn 100%, tiếp tục nhé!',
        'Mỗi lần ôn lại là một lần khắc sâu vào trí nhớ!'
      ]
    },
    fox: {
      key: 'fox',
      name: 'Cáo Kitsune',
      role: 'Tinh anh Đa ngôn ngữ',
      accent: '#ec4899',
      greetings: [
        'Chào nhà thám hiểm ngôn ngữ! Sẵn sàng săn XP chưa? ✨',
        'Bí quyết nhớ lâu: đọc to thành tiếng mỗi từ vựng nhé!',
        'Kitsune đã chuẩn bị pháo hoa chờ bạn hoàn thành bài!'
      ],
      cheers: [
        'Lấp lánh quá! Phản xạ cực kỳ nhạy bén! 💎',
        'Tuyệt kỹ ghi nhớ đã được kích hoạt! +XP!',
        'Chuỗi đúng liên tiếp ấn tượng quá đi!'
      ],
      encourages: [
        'Chút thử thách nhỏ thôi, bạn làm được mà! 🌟',
        'Đừng lo, não bộ đang tạo liên kết mới đấy!',
        'Cùng xem kỹ nghĩa rồi chinh phục lại nhé!'
      ]
    },
    dragon: {
      key: 'dragon',
      name: 'Rồng ngọc Long Nhi',
      role: 'Hộ vệ Huyền Thoại Polyglot',
      accent: '#10b981',
      greetings: [
        'Chào chủ nhân! Hôm nay ta chinh phục đỉnh cao nào? 🐉',
        'Tri thức ngoại ngữ chính là kho báu quý giá nhất!',
        'Hãy thử thách cả Trắc nghiệm lẫn Đặt câu Tự luận để đạt cấp Huyền Thoại nhé!'
      ],
      cheers: [
        'Uy lực phi thường! Kiến thức tăng vọt! 🔥',
        'Long Nhi tự hào về sự chăm chỉ của bạn! 👑',
        'Một bước tiến gần hơn tới danh hiệu Huyền Thoại!'
      ],
      encourages: [
        'Ngọc càng mài càng sáng, ôn thêm chút nữa thôi! 💎',
        'Không ai thành tài mà chưa từng quên từ vựng cả!',
        'Giữ vững ý chí nhé, Long Nhi luôn bên bạn!'
      ]
    }
  };

  const AVATAR_CATALOG = [
    { key: 'owl', label: 'Cú Oliver', price: 0, rarity: 'free', desc: 'Mặc định • Cú thông thái Oxford' },
    { key: 'panda', label: 'Gấu BaoBao', price: 0, rarity: 'free', desc: 'Mặc định • Võ sư HSK chăm chỉ' },
    { key: 'ghost', label: 'Bé Ma Mochi', price: 50, rarity: 'rare', desc: 'Linh vật 3TG siêu ngộ nghĩnh' },
    { key: 'neko', label: 'Mèo Thần Tài', price: 60, rarity: 'rare', desc: 'Chiêu tài may mắn JLPT' },
    { key: 'tiger', label: 'Hổ Horangi', price: 80, rarity: 'rare', desc: 'Bùng nổ năng lượng TOPIK' },
    { key: 'fox', label: 'Cáo Kitsune', price: 100, rarity: 'epic', desc: 'Tinh anh lấp lánh sắc màu' },
    { key: 'shiba', label: 'Cún Shiba', price: 120, rarity: 'epic', desc: 'Vui tươi trung thành đáng yêu' },
    { key: 'astronaut', label: 'Phi Hành Gia', price: 150, rarity: 'epic', desc: 'Khám phá vũ trụ tri thức' },
    { key: 'ninja', label: 'Ninja Học Giả', price: 200, rarity: 'legendary', desc: 'Phản xạ thần tốc trong đêm' },
    { key: 'wizard', label: 'Phù Thủy Cổ Đại', price: 250, rarity: 'legendary', desc: 'Phép thuật ngữ pháp kỳ diệu' },
    { key: 'dragon', label: 'Rồng Huyền Thoại', price: 300, rarity: 'legendary', desc: 'Thần thú rồng ngọc tối thượng' },
    { key: 'crown', label: 'Vua Đa Ngôn Ngữ', price: 500, rarity: 'mythic', desc: 'Vương miện hoàng gia cao quý' }
  ];

  const COMPANION_CATALOG = [
    { key: 'auto', label: '🔄 Tự động theo ngôn ngữ', desc: 'Tự đổi Cú Anh / Gấu Trung / Mèo Nhật / Hổ Hàn' },
    { key: 'owl', label: '🦉 Giáo sư Oliver', desc: 'Điềm đạm, thông thái chuẩn Oxford' },
    { key: 'panda', label: '🐼 Võ sư BaoBao', desc: 'Đáng yêu, nhiệt huyết chinh phục HSK' },
    { key: 'neko', label: '🐱 Mèo thần tài Mochi', desc: 'Dễ thương, cổ vũ may mắn JLPT' },
    { key: 'tiger', label: '🐯 Hổ con Horangi', desc: 'Mạnh mẽ, bùng nổ năng lượng TOPIK' },
    { key: 'fox', label: '🦊 Cáo tuyết Kitsune', desc: 'Tinh anh, lấp lánh sắc màu' },
    { key: 'dragon', label: '🐉 Rồng ngọc Long Nhi', desc: 'Linh vật Huyền Thoại Polyglot' }
  ];

  // ---------- 2. Sound Effects (Synthesized Web Audio API) ----------
  let audioCtx = null;
  let lastSoundTimestamp = 0;

  function isSoundEnabled() {
    return localStorage.getItem('polylearn_sfx') !== 'off';
  }

  function toggleSound() {
    const next = isSoundEnabled() ? 'off' : 'on';
    localStorage.setItem('polylearn_sfx', next);
    if (next === 'on') playSound('pop');
    return next === 'on';
  }

  function playSound(type = 'correct') {
    lastSoundTimestamp = performance.now();
    if (!isSoundEnabled()) return;
    try {
      const Ctx = window.AudioContext || window.webkitAudioContext;
      if (!Ctx) return;
      if (!audioCtx) audioCtx = new Ctx();
      if (audioCtx.state === 'suspended') audioCtx.resume();

      const now = audioCtx.currentTime;
      const playTone = (freq, startOffset, duration, wave = 'sine', gainVal = 0.08) => {
        const osc = audioCtx.createOscillator();
        const gain = audioCtx.createGain();
        osc.type = wave;
        osc.frequency.setValueAtTime(freq, now + startOffset);
        gain.gain.setValueAtTime(gainVal, now + startOffset);
        gain.gain.exponentialRampToValueAtTime(0.001, now + startOffset + duration);
        osc.connect(gain);
        gain.connect(audioCtx.destination);
        osc.start(now + startOffset);
        osc.stop(now + startOffset + duration);
      };

      if (type === 'correct') {
        playTone(587.33, 0, 0.14, 'triangle', 0.1);   // D5
        playTone(880.00, 0.09, 0.24, 'triangle', 0.1); // A5
      } else if (type === 'fanfare') {
        playTone(523.25, 0, 0.12, 'triangle', 0.11);    // C5
        playTone(659.25, 0.1, 0.12, 'triangle', 0.11);  // E5
        playTone(783.99, 0.2, 0.14, 'triangle', 0.11);  // G5
        playTone(1046.50, 0.32, 0.38, 'triangle', 0.13); // C6
      } else if (type === 'wrong') {
        playTone(320, 0, 0.15, 'sine', 0.08);
        playTone(250, 0.12, 0.22, 'sine', 0.08);
      } else if (type === 'pop') {
        playTone(720, 0, 0.09, 'sine', 0.07);
      } else if (type === 'tap') {
        playTone(940, 0, 0.045, 'sine', 0.04);
        playTone(1240, 0.018, 0.045, 'triangle', 0.028);
      } else if (type === 'whoosh') {
        playTone(460, 0, 0.07, 'sine', 0.045);
        playTone(690, 0.045, 0.09, 'triangle', 0.045);
      }
    } catch {
      // Ignore audio errors if blocked by browser policy
    }
  }

  // ---------- 3. Visual Effects: Confetti, Floating +XP & Level-Up ----------
  function launchConfetti(particleCount = 55) {
    const canvas = document.createElement('canvas');
    canvas.style.cssText = 'position:fixed;inset:0;width:100vw;height:100vh;pointer-events:none;z-index:3500;';
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    document.body.appendChild(canvas);

    const ctx = canvas.getContext('2d');
    const colors = ['#818cf8', '#f472b6', '#34d399', '#fbbf24', '#38bdf8', '#c084fc'];
    const particles = Array.from({ length: particleCount }, () => ({
      x: canvas.width * (0.25 + Math.random() * 0.5),
      y: canvas.height * 0.45,
      vx: (Math.random() - 0.5) * 14,
      vy: -(Math.random() * 11 + 6),
      size: Math.random() * 7 + 5,
      color: colors[Math.floor(Math.random() * colors.length)],
      rot: Math.random() * Math.PI * 2,
      vRot: (Math.random() - 0.5) * 0.25,
      alpha: 1
    }));

    let frame = 0;
    function tick() {
      frame++;
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      let alive = false;
      particles.forEach(p => {
        p.x += p.vx;
        p.y += p.vy;
        p.vy += 0.34; // gravity
        p.vx *= 0.99;
        p.rot += p.vRot;
        if (frame > 45) p.alpha -= 0.022;
        if (p.alpha > 0 && p.y < canvas.height + 30) {
          alive = true;
          ctx.save();
          ctx.globalAlpha = Math.max(0, p.alpha);
          ctx.translate(p.x, p.y);
          ctx.rotate(p.rot);
          ctx.fillStyle = p.color;
          ctx.fillRect(-p.size / 2, -p.size / 2, p.size, p.size * 0.65);
          ctx.restore();
        }
      });

      if (alive && frame < 130) {
        requestAnimationFrame(tick);
      } else {
        canvas.remove();
      }
    }
    requestAnimationFrame(tick);
  }

  function spawnFloatingXp(xpAmount, targetEl = null) {
    if (!xpAmount || xpAmount <= 0) return;
    const pill = document.createElement('div');
    pill.className = 'floating-xp-pill';
    pill.innerHTML = `✨ +${xpAmount} XP`;

    let left = window.innerWidth / 2 - 48;
    let top = window.innerHeight * 0.42;

    if (targetEl && targetEl.getBoundingClientRect) {
      const rect = targetEl.getBoundingClientRect();
      if (rect.width > 0 || rect.height > 0) {
        left = rect.left + rect.width / 2 - 48;
        top = Math.max(70, rect.top - 12);
      }
    }

    pill.style.left = `${left}px`;
    pill.style.top = `${top}px`;
    document.body.appendChild(pill);
    setTimeout(() => pill.remove(), 1450);
  }

  function showLevelUpCelebration(level) {
    playSound('fanfare');
    launchConfetti(95);

    const banner = document.createElement('div');
    banner.className = 'levelup-celebration-overlay';
    banner.innerHTML = `
      <div class="levelup-card">
        <div style="font-size: 3.4rem; margin-bottom: 6px;">🏆</div>
        <div style="font-size: 0.9rem; text-transform: uppercase; letter-spacing: 0.12em; color: var(--warning-text); font-weight: 800;">Thăng cấp xuất sắc!</div>
        <h2 style="font-family: 'Outfit'; font-size: 2.3rem; margin: 6px 0 8px;">Đạt Cấp Độ ${level}</h2>
        <p style="color: var(--text-secondary); font-size: 0.95rem; margin-bottom: 18px;">Bạn vừa tích lũy đủ XP để vươn lên tầm cao mới. Tiếp tục giữ lửa nhé!</p>
        <button class="btn btn-primary" id="btn-dismiss-levelup">Tuyệt vời! 🚀</button>
      </div>
    `;
    document.body.appendChild(banner);

    const remove = () => banner.remove();
    banner.querySelector('#btn-dismiss-levelup').addEventListener('click', remove);
    banner.addEventListener('click', (e) => { if (e.target === banner) remove(); });
    setTimeout(remove, 5500);
  }

  // ---------- 4. Helpers for SVG & Companion Resolution ----------
  function getAvatarSvg(key) {
    if (!key) return MASCOT_SVGS.owl;
    if (key === 'custom') {
      const u = typeof API !== 'undefined' ? API.getUser() : null;
      const customUrl = localStorage.getItem('polylearn_custom_avatar_' + (u?.id || 'guest'));
      if (customUrl) {
        return `<img src="${customUrl}" class="custom-avatar-img" alt="Avatar" style="width: 100%; height: 100%; object-fit: cover; border-radius: 50%;">`;
      }
    }
    if (key.startsWith('data:image/') || key.startsWith('http')) {
      return `<img src="${key}" class="custom-avatar-img" alt="Avatar" style="width: 100%; height: 100%; object-fit: cover; border-radius: 50%;">`;
    }
    return MASCOT_SVGS[key] || EXTRA_AVATAR_SVGS[key] || MASCOT_SVGS.owl;
  }

  function resolveCompanionKey(companionPreference, activeLanguageId) {
    if (companionPreference && companionPreference !== 'auto' && COMPANIONS[companionPreference]) {
      return companionPreference;
    }
    switch (Number(activeLanguageId)) {
      case 2: return 'panda';
      case 3: return 'neko';
      case 4: return 'tiger';
      default: return 'owl';
    }
  }

  function getCompanion(companionPreference, activeLanguageId) {
    const key = resolveCompanionKey(companionPreference, activeLanguageId);
    return {
      ...COMPANIONS[key],
      svg: MASCOT_SVGS[key]
    };
  }

  function pickRandom(arr) {
    return arr[Math.floor(Math.random() * arr.length)];
  }

  // ---------- 5. Floating Companion Dock Controller ----------
  let comboCount = 0;
  let moodTimer = null;

  function updateCompanionDock({ companionPref = 'auto', langId = 1, mood = 'idle', customText = null } = {}) {
    const dock = document.getElementById('companion-dock');
    if (!dock) return;

    const comp = getCompanion(companionPref, langId);
    const stage = document.getElementById('companion-dock-avatar');
    const nameEl = document.getElementById('companion-dock-name');
    const speechEl = document.getElementById('companion-dock-speech');
    const comboBadge = document.getElementById('companion-combo-badge');

    if (stage) {
      stage.innerHTML = comp.svg;
      stage.className = `companion-avatar-stage mood-${mood}`;
    }
    if (nameEl) {
      nameEl.textContent = comp.name;
      nameEl.style.color = comp.accent;
    }

    let message = customText;
    if (!message) {
      if (mood === 'happy' || mood === 'party') message = pickRandom(comp.cheers);
      else if (mood === 'encourage') message = pickRandom(comp.encourages);
      else message = pickRandom(comp.greetings);
    }

    if (speechEl) {
      speechEl.textContent = message;
      speechEl.classList.remove('bubble-pop');
      void speechEl.offsetWidth; // trigger reflow
      speechEl.classList.add('bubble-pop');
    }

    if (comboBadge) {
      if (comboCount >= 2) {
        comboBadge.style.display = 'inline-flex';
        comboBadge.textContent = `🔥 Combo x${comboCount}`;
      } else {
        comboBadge.style.display = 'none';
      }
    }

    clearTimeout(moodTimer);
    if (mood !== 'idle') {
      moodTimer = setTimeout(() => {
        if (stage) stage.className = 'companion-avatar-stage mood-idle';
      }, 2800);
    }
  }

  function recordCombo(isSuccess) {
    if (isSuccess) {
      comboCount++;
    } else {
      comboCount = 0;
    }
    return comboCount;
  }

  function resetCombo() {
    comboCount = 0;
  }

  function spawnHeartBurst(anchorEl) {
    if (!anchorEl) return;
    const rect = anchorEl.getBoundingClientRect();
    const icons = ['❤️', '✨', '🌟', '💫'];
    for (let i = 0; i < 4; i++) {
      const span = document.createElement('span');
      span.className = 'companion-heart-particle';
      span.textContent = icons[i % icons.length];
      span.style.left = `${rect.left + rect.width * (0.2 + Math.random() * 0.6)}px`;
      span.style.top = `${rect.top + 10}px`;
      span.style.animationDelay = `${i * 0.08}s`;
      document.body.appendChild(span);
      setTimeout(() => span.remove(), 1100);
    }
  }

  // ---------- 6. Entrance Cascade, Ambient Glyphs & Interactive Click FX ----------
  const LANGUAGE_AMBIENT_GLYPHS = {
    1: ['A', 'B', 'C', '✦', '★', '♪', 'W', '✧', 'E', '✦', 'Z', '★'],
    2: ['文', '字', '好', '✦', '学', '龙', '★', '道', '✧', '♪', '梦', '✦'],
    3: ['あ', 'い', 'う', '✦', '桜', '夢', '★', '語', '✧', '♪', '光', '✦'],
    4: ['한', '글', '가', '✦', '별', '꿈', '★', '나', '✧', '♪', '빛', '✦']
  };

  const LANGUAGE_SPARK_COLORS = {
    1: ['#6366f1', '#38bdf8', '#a855f7', '#fbbf24', '#f472b6'],
    2: ['#f43f5e', '#fbbf24', '#fb923c', '#e11d48', '#fde047'],
    3: ['#ec4899', '#c084fc', '#f472b6', '#8b5cf6', '#38bdf8'],
    4: ['#14b8a6', '#fb923c', '#34d399', '#fbbf24', '#38bdf8']
  };

  let effectsInitialized = false;
  let activeFxLangId = 1;

  function prefersReducedMotion() {
    return window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
  }

  function updateAmbientLanguage(langId = 1) {
    activeFxLangId = Number(langId) || 1;
    if (prefersReducedMotion()) return;

    let layer = document.getElementById('ambient-fx-layer');
    if (!layer) {
      layer = document.createElement('div');
      layer.id = 'ambient-fx-layer';
      layer.className = 'ambient-fx-layer';
      layer.setAttribute('aria-hidden', 'true');
      document.body.prepend(layer);
    }

    const glyphs = LANGUAGE_AMBIENT_GLYPHS[activeFxLangId] || LANGUAGE_AMBIENT_GLYPHS[1];
    layer.innerHTML = '';

    for (let i = 0; i < 14; i++) {
      const span = document.createElement('span');
      span.className = 'ambient-glyph';
      span.textContent = glyphs[i % glyphs.length];

      const leftPct = Math.round((i / 14) * 94 + (Math.random() * 5));
      const sizePx = Math.round(14 + Math.random() * 15);
      const durSec = (16 + Math.random() * 14).toFixed(1);
      const delaySec = (-Math.random() * 24).toFixed(1);
      const driftPx = Math.round((Math.random() - 0.5) * 90);
      const rotDeg = Math.round((Math.random() - 0.5) * 70);
      const peakOp = (0.07 + Math.random() * 0.08).toFixed(2);

      span.style.left = `${leftPct}%`;
      span.style.fontSize = `${sizePx}px`;
      span.style.setProperty('--glyph-dur', `${durSec}s`);
      span.style.setProperty('--glyph-delay', `${delaySec}s`);
      span.style.setProperty('--glyph-drift', `${driftPx}px`);
      span.style.setProperty('--glyph-rot', `${rotDeg}deg`);
      span.style.setProperty('--glyph-op', peakOp);
      layer.appendChild(span);
    }
  }

  function spawnClickBurst(clientX, clientY, isInteractive = false) {
    if (prefersReducedMotion()) return;

    const halo = document.createElement('div');
    halo.className = 'click-fx-halo';
    halo.style.left = `${clientX}px`;
    halo.style.top = `${clientY}px`;
    if (!isInteractive) {
      halo.style.width = '28px';
      halo.style.height = '28px';
    }
    document.body.appendChild(halo);
    setTimeout(() => halo.remove(), 540);

    const palette = LANGUAGE_SPARK_COLORS[activeFxLangId] || LANGUAGE_SPARK_COLORS[1];
    const symbols = isInteractive ? ['✦', '★', '✧', '•', '✦'] : ['✦', '•', '✧'];
    const count = isInteractive ? 9 : 5;

    for (let i = 0; i < count; i++) {
      const spark = document.createElement('span');
      spark.className = 'click-fx-spark';
      spark.textContent = symbols[i % symbols.length];

      const angle = (Math.PI * 2 * i) / count + (Math.random() - 0.5) * 0.45;
      const dist = (isInteractive ? 28 : 18) + Math.random() * (isInteractive ? 34 : 18);
      const dx = Math.round(Math.cos(angle) * dist);
      const dy = Math.round(Math.sin(angle) * dist);
      const size = Math.round((isInteractive ? 11 : 9) + Math.random() * 6);
      const dur = Math.round(460 + Math.random() * 190);
      const rot = Math.round((Math.random() - 0.5) * 200);
      const color = palette[i % palette.length];

      spark.style.setProperty('--sx', `${Math.round(clientX - size / 2)}px`);
      spark.style.setProperty('--sy', `${Math.round(clientY - size / 2)}px`);
      spark.style.setProperty('--dx', `${dx}px`);
      spark.style.setProperty('--dy', `${dy}px`);
      spark.style.setProperty('--spark-size', `${size}px`);
      spark.style.setProperty('--spark-dur', `${dur}ms`);
      spark.style.setProperty('--rot', `${rot}deg`);
      spark.style.setProperty('--spark-color', color);

      document.body.appendChild(spark);
      setTimeout(() => spark.remove(), dur + 40);
    }
  }

  function spawnRipple(el, clientX, clientY) {
    if (!el || prefersReducedMotion()) return;
    const rect = el.getBoundingClientRect();
    if (rect.width === 0 || rect.height === 0) return;

    const size = Math.round(Math.max(rect.width, rect.height) * 1.9);
    const wave = document.createElement('span');
    wave.className = 'click-ripple-wave';
    wave.style.width = `${size}px`;
    wave.style.height = `${size}px`;
    wave.style.left = `${Math.round(clientX - rect.left)}px`;
    wave.style.top = `${Math.round(clientY - rect.top)}px`;
    el.appendChild(wave);
    setTimeout(() => wave.remove(), 650);
  }

  function triggerEntranceCascade(containerEl = null) {
    if (prefersReducedMotion()) return;
    const scope = containerEl || document.querySelector('.view-section.active');
    if (!scope) return;

    const targets = scope.querySelectorAll(
      '.dashboard-hero, .glass-card, .dash-exam-mode-card, .lesson-card, .word-card-selectable, .badge-card'
    );

    Array.from(targets).slice(0, 18).forEach((el, idx) => {
      el.classList.remove('stagger-card-enter');
      el.style.setProperty('--stagger-idx', String(idx));
      void el.offsetWidth;
      el.classList.add('stagger-card-enter');
      el.addEventListener('animationend', () => {
        el.classList.remove('stagger-card-enter');
      }, { once: true });
    });
  }

  function initInteractiveEffects(initialLangId = 1) {
    if (effectsInitialized) return;
    effectsInitialized = true;

    updateAmbientLanguage(initialLangId);
    triggerEntranceCascade();

    // Gentle celebratory welcome burst when entering the page
    if (!prefersReducedMotion()) {
      setTimeout(() => {
        const heroEl = document.querySelector('.dashboard-hero') || document.querySelector('header');
        if (heroEl) {
          const r = heroEl.getBoundingClientRect();
          spawnClickBurst(r.left + r.width * 0.5, Math.max(90, r.top + r.height * 0.45), true);
        }
        launchConfetti(28);
      }, 360);
    }

    // Global click & ripple & subtle tap audio
    document.addEventListener('pointerdown', (e) => {
      if (e.button !== undefined && e.button !== 0) return;

      const interactiveEl = e.target.closest(
        'button, .btn, .nav-btn, .quiz-option, .exam-format-card, .dash-exam-mode-card, .preset-quiz-item, .roadmap-card, .palette-cell, .sample-chip, .speaker-btn, .word-card-selectable, .lesson-card, .companion-dock, a, select, label'
      );

      spawnClickBurst(e.clientX, e.clientY, Boolean(interactiveEl));

      const rippleTarget = e.target.closest(
        '.btn, .nav-btn, .quiz-option, .exam-format-card, .dash-exam-mode-card, .preset-quiz-item, .roadmap-card, .palette-cell, .sample-chip, .speaker-btn, .quick-ribbon-action'
      );
      if (rippleTarget) {
        spawnRipple(rippleTarget, e.clientX, e.clientY);
      }

      if (interactiveEl && interactiveEl.id !== 'btn-sound-toggle') {
        const downAt = performance.now();
        setTimeout(() => {
          if (lastSoundTimestamp < downAt) {
            playSound('tap');
          }
        }, 35);
      }
    }, { passive: true });

    // Cursor spotlight on Glass Cards + Mascot Pupil Eye-Tracking
    let rafPending = false;
    let pointerX = window.innerWidth / 2;
    let pointerY = window.innerHeight / 2;
    let pointerTarget = null;

    document.addEventListener('pointermove', (e) => {
      pointerX = e.clientX;
      pointerY = e.clientY;
      pointerTarget = e.target;
      if (rafPending || prefersReducedMotion()) return;
      rafPending = true;

      requestAnimationFrame(() => {
        rafPending = false;

        const card = pointerTarget && pointerTarget.closest ? pointerTarget.closest('.glass-card') : null;
        if (card) {
          const rect = card.getBoundingClientRect();
          card.style.setProperty('--spot-x', `${Math.round(pointerX - rect.left)}px`);
          card.style.setProperty('--spot-y', `${Math.round(pointerY - rect.top)}px`);
        }

        document.querySelectorAll('.mascot-pupils').forEach(pupilGroup => {
          const svg = pupilGroup.ownerSVGElement;
          if (!svg) return;
          const rect = svg.getBoundingClientRect();
          if (rect.width === 0 || rect.height === 0) return;
          const cx = rect.left + rect.width / 2;
          const cy = rect.top + rect.height / 2;
          const angle = Math.atan2(pointerY - cy, pointerX - cx);
          const dist = Math.min(2.6, Math.hypot(pointerX - cx, pointerY - cy) / 95);
          const dx = (Math.cos(angle) * dist).toFixed(2);
          const dy = (Math.sin(angle) * dist).toFixed(2);
          pupilGroup.style.transform = `translate(${dx}px, ${dy}px)`;
        });
      });
    }, { passive: true });
  }

  return {
    AVATAR_CATALOG,
    COMPANION_CATALOG,
    COMPANIONS,
    getAvatarSvg,
    getCompanion,
    resolveCompanionKey,
    isSoundEnabled,
    toggleSound,
    playSound,
    launchConfetti,
    spawnFloatingXp,
    showLevelUpCelebration,
    updateCompanionDock,
    recordCombo,
    resetCombo,
    spawnHeartBurst,
    updateAmbientLanguage,
    spawnClickBurst,
    triggerEntranceCascade,
    initInteractiveEffects
  };
})();

