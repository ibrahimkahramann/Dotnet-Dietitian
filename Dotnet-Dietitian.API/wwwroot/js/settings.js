/**
 * Settings.js - Kullanıcı ayarlarını uygulamak için kullanılan yardımcı fonksiyonlar
 */

// Sayfa yüklendiğinde ayarları uygula
document.addEventListener('DOMContentLoaded', function() {
    // Mevcut ayarları uygula
    applyUserSettings();
    
    // Tema değişikliğinde işlem yap
    const themeInputs = document.querySelectorAll('input[name="theme"]');
    themeInputs.forEach(input => {
        input.addEventListener('change', function() {
            if (this.checked) {
                applyTheme(this.value);
                
                // Ayarları sunucuya kaydet - formun otomatik gönderilmesi
                const form = this.closest('form');
                if (form && form.id === 'appearanceSettingsForm') {
                    form.submit();
                }
            }
        });
    });
});

/**
 * Kullanıcı ayarlarını sayfaya uygula
 */
function applyUserSettings() {
    // Tema ayarlarını al ve uygula
    const savedTheme = localStorage.getItem('userTheme');
    if (savedTheme) {
        applyTheme(savedTheme);
        
        // Radyo butonlarını da güncelle
        const themeRadios = document.querySelectorAll('input[name="theme"]');
        themeRadios.forEach(radio => {
            if (radio.value === savedTheme) {
                radio.checked = true;
            }
        });
    } else {
        // Varsayılan temayı seç
        const currentTheme = getCurrentThemeSetting();
        applyTheme(currentTheme);
    }
}

/**
 * Mevcut tema ayarını al
 * @returns {string} light, dark veya system
 */
function getCurrentThemeSetting() {
    const themeRadios = document.querySelectorAll('input[name="theme"]');
    let selectedTheme = 'light'; // Varsayılan tema
    
    themeRadios.forEach(radio => {
        if (radio.checked) {
            selectedTheme = radio.value;
        }
    });
    
    // System seçiliyse, sistem tercihini kontrol et
    if (selectedTheme === 'system') {
        return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    
    return selectedTheme;
}

/**
 * Temayı sayfa üzerinde uygula
 * @param {string} theme - Uygulanacak tema (light, dark, system)
 */
function applyTheme(theme) {
    const htmlElement = document.documentElement;
    
    // Sistem tercihine göre belirle
    if (theme === 'system') {
        theme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    
    // Mevcut temaları temizle ve yeni temayı ekle
    htmlElement.classList.remove('theme-light', 'theme-dark');
    htmlElement.classList.add('theme-' + theme);
    
    // Bootstrap tema sınıflarını da uygula
    if (theme === 'dark') {
        htmlElement.setAttribute('data-bs-theme', 'dark');
    } else {
        htmlElement.setAttribute('data-bs-theme', 'light');
    }
    
    // Tema değişikliğini local storage'a kaydet
    localStorage.setItem('userTheme', theme);
} 