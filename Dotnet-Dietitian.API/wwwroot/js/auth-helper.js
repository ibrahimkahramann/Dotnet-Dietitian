/**
 * Authentication Helper
 * This script provides user authentication and session management 
 * to ensure users stay in their correct sections (patient/dietitian)
 * 
 * NOT: TEST MODU - GİRİŞ KONTROLÜ DEVRE DIŞI
 */

(function() {
    'use strict';
    
    // TEST MODU: Oturum kontrol sistemi devre dışı bırakıldı
    // Bu kod sadece oturum kayıtları için çalışıyor
    
    console.log('Auth Helper: TEST MODU - Giriş kontrolü devre dışı bırakıldı');
    
    // İlerleme Takibi sayfası için özel muafiyet
    const currentPage = window.location.pathname.split('/').pop() || 'index.html';
    if (currentPage === 'patient-progress-tracking.html') {
        console.log('Auth Helper: İlerleme Takibi sayfası için oturum kontrolü devre dışı');
        sessionStorage.setItem('userType', 'patient');
        localStorage.setItem('userType', 'patient');
        document.body.classList.add('patient-user');
        return; // Auth helper işlemlerini burada sonlandır
    }
    
    // Sadece logout işlemini yönet
    document.addEventListener('DOMContentLoaded', function() {
        // Eğer sayfamız patient- ile başlıyorsa ve oturum yoksa, otomatik oturum oluştur
        const currentPage = window.location.pathname.split('/').pop() || 'index.html';
        
        if (currentPage.startsWith('patient-') && !sessionStorage.getItem('userType')) {
            console.log('Auth Helper: Hasta sayfası için otomatik oturum açılıyor');
            sessionStorage.setItem('userType', 'patient');
        } else if (currentPage.startsWith('dietitian-') && !sessionStorage.getItem('userType')) {
            console.log('Auth Helper: Diyetisyen sayfası için otomatik oturum açılıyor');
            sessionStorage.setItem('userType', 'dietitian');
        }
        
        // Çıkış linkleri için işleyici ekle
        setupLogoutHandler();
        
        // CSS sınıfları ekle
        if (currentPage.startsWith('patient-')) {
            document.body.classList.add('patient-user');
        } else if (currentPage.startsWith('dietitian-')) {
            document.body.classList.add('dietitian-user');
        }
    });
    
    // Setup logout handler to clear session when user logs out
    function setupLogoutHandler() {
        // Target all logout links (typically with href="index.html")
        const logoutLinks = document.querySelectorAll('a[href="index.html"]');
        
        logoutLinks.forEach(link => {
            link.addEventListener('click', function() {
                // Clear all authentication data
                sessionStorage.removeItem('userType');
                localStorage.removeItem('userType');
                localStorage.removeItem('userEmail');
                console.log('Auth Helper: User logged out, session cleared');
            });
        });
    }
})(); 