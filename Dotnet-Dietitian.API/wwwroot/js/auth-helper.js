/**
 * Authentication Helper
 * This script provides user authentication and session management 
 * to ensure users stay in their correct sections (patient/dietitian)
 * 
 * Entegre mod - Razor sayfaları ve backend ile uyumlu çalışır
 */

(function() {
    'use strict';
    
    // Kullanıcı tipini cookie'den alma
    function getUserTypeFromCookie() {
        const cookies = document.cookie.split(';');
        for (let i = 0; i < cookies.length; i++) {
            const cookie = cookies[i].trim();
            if (cookie.startsWith('userType=')) {
                return cookie.substring('userType='.length, cookie.length);
            }
        }
        return null;
    }
    
    // Kullanıcı tipini alma fonksiyonu - önce cookie, sonra session ve local storage
    function getUserType() {
        // İlk önce cookie'den kontrol et (backend'den gelir)
        const cookieUserType = getUserTypeFromCookie();
        if (cookieUserType) {
            return cookieUserType;
        }
        
        // Sonra sessionStorage'dan kontrol et
        const sessionUserType = sessionStorage.getItem('userType');
        if (sessionUserType) {
            return sessionUserType;
        }
        
        // Son olarak localStorage'dan kontrol et (hatırla seçeneği varsa)
        return localStorage.getItem('userType');
    }
    
    document.addEventListener('DOMContentLoaded', function() {
        // Mevcut sayfa bilgisi
        const currentPage = window.location.pathname.split('/').pop() || 'index.html';
        
        // İlerleme Takibi sayfası için özel muafiyet
        if (currentPage === 'patient-progress-tracking.html') {
            console.log('Auth Helper: İlerleme Takibi sayfası için oturum kontrolü devre dışı');
            sessionStorage.setItem('userType', 'Hasta');
            document.body.classList.add('patient-user');
            return; // Auth helper işlemlerini burada sonlandır
        }
        
        // Kullanıcı tipini al
        const userType = getUserType();
        
        // Oturum bilgisini senkronize et (cookie, session ve local storage arasında)
        if (userType) {
            sessionStorage.setItem('userType', userType);
            
            // CSS sınıflarını ekle
            if (userType === 'Hasta') {
                document.body.classList.add('patient-user');
            } else if (userType === 'Diyetisyen') {
                document.body.classList.add('dietitian-user');
            }
            
            // Hasta sayfalarına erişim kontrolü
            if (currentPage.startsWith('patient-') && userType !== 'Hasta') {
                console.log('Auth Helper: Hasta sayfasına yetkisiz erişim, yönlendiriliyor...');
                window.location.href = '/Account/Login?type=patient&authError=patient';
                return;
            }
            
            // Diyetisyen sayfalarına erişim kontrolü
            if (currentPage.startsWith('dietitian-') && userType !== 'Diyetisyen') {
                console.log('Auth Helper: Diyetisyen sayfasına yetkisiz erişim, yönlendiriliyor...');
                window.location.href = '/Account/Login?type=dietitian&authError=dietitian';
                return;
            }
        } else {
            // Oturum yoksa ve korumalı sayfaya erişim varsa yönlendir
            if (currentPage.startsWith('patient-')) {
                console.log('Auth Helper: Oturumsuz hasta sayfası erişimi, yönlendiriliyor...');
                window.location.href = '/Account/Login?type=patient';
                return;
            } else if (currentPage.startsWith('dietitian-')) {
                console.log('Auth Helper: Oturumsuz diyetisyen sayfası erişimi, yönlendiriliyor...');
                window.location.href = '/Account/Login?type=dietitian';
                return;
            }
        }
        
        // Çıkış linkleri için işleyici ekle
        setupLogoutHandler();
    });
    
    // Setup logout handler to clear session when user logs out
    function setupLogoutHandler() {
        // Target all logout links
        const logoutLinks = document.querySelectorAll('a[href="index.html"], a[href="/Account/Logout"], a[href="../Account/Logout"]');
        
        logoutLinks.forEach(link => {
            link.addEventListener('click', function() {
                // Clear all authentication data
                sessionStorage.removeItem('userType');
                localStorage.removeItem('userType');
                localStorage.removeItem('userEmail');
                
                // Try to expire the auth cookie by setting it to past date
                document.cookie = "userType=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
                document.cookie = "jwt_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; secure; HttpOnly; SameSite=Strict";
                
                console.log('Auth Helper: User logged out, session cleared');
            });
        });
    }
})(); 