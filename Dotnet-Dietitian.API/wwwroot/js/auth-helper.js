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
        const cookieUserType = getUserTypeFromCookie();
        if (cookieUserType) return cookieUserType;
        
        const sessionUserType = sessionStorage.getItem('userType');
        if (sessionUserType) return sessionUserType;
        
        return localStorage.getItem('userType');
    }
    
    document.addEventListener('DOMContentLoaded', function() {
        console.log("Auth Helper: Started - checking authentication");
        const currentPath = window.location.pathname;
        const userType = getUserType();
        
        console.log("Auth Helper: Current path:", currentPath, "User Type:", userType);

        // Muaf tutulacak sayfalar (örneğin login, register, index vb.)
        const publicPages = ['/', '/Home/Index', '/Account/Login', '/Account/Register', '/Account/ForgotPassword'];
        if (publicPages.some(page => currentPath.toLowerCase().endsWith(page.toLowerCase())) || currentPath === "/" ) {
            console.log('Auth Helper: Public page, no auth check needed.');
            setupLogoutHandler();
            return;
        }
        
        if (userType) {
            sessionStorage.setItem('userType', userType);
            document.body.classList.add(userType.toLowerCase() + '-user');

            const isPatientPath = currentPath.toLowerCase().includes('/patient');
            const isDietitianPath = currentPath.toLowerCase().includes('/dietitian');

            console.log("Auth Helper: Path analysis - Patient:", isPatientPath, "Dietitian:", isDietitianPath);

            if (isPatientPath && userType !== 'Hasta') {
                console.log('Auth Helper: Non-patient user trying to access patient page. Redirecting...');
                window.location.href = '/Account/Login?type=patient&authError=patient';
                return;
            }
            
            if (isDietitianPath && userType !== 'Diyetisyen') {
                console.log('Auth Helper: Non-dietitian user trying to access dietitian page. Redirecting...');
                window.location.href = '/Account/Login?type=dietitian&authError=dietitian';
                return;
            }
        } else {
            const isPatientPath = currentPath.toLowerCase().includes('/patient');
            const isDietitianPath = currentPath.toLowerCase().includes('/dietitian');

            if (isPatientPath) {
                console.log('Auth Helper: No session, trying to access patient page. Redirecting...');
                window.location.href = '/Account/Login?type=patient';
                return;
            }
            if (isDietitianPath) {
                console.log('Auth Helper: No session, trying to access dietitian page. Redirecting...');
                window.location.href = '/Account/Login?type=dietitian';
                return;
            }
        }
        
        setupLogoutHandler();
    });
    
    // Setup logout handler to clear session when user logs out
    function setupLogoutHandler() {
        const logoutLinks = document.querySelectorAll('a[href="index.html"], a[href="/Account/Logout"], a[href="../Account/Logout"], a[href*="Logout"]');
        
        logoutLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                // Update any "index.html" hrefs to point to /Account/Logout
                if (link.getAttribute('href') === 'index.html') {
                    link.setAttribute('href', '/Account/Logout');
                }
                
                // Eğer MVC linkiyse ve Logout action'ına gidiyorsa, JS ile session temizleme işlemini backend'e bırak.
                const href = link.getAttribute('href');
                if (!(href && href.toLowerCase().includes('/account/logout'))) {
                    sessionStorage.removeItem('userType');
                    localStorage.removeItem('userType');
                    localStorage.removeItem('userEmail');
                    document.cookie = "userType=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
                    document.cookie = "jwt_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/; secure; HttpOnly; SameSite=Strict";
                    console.log('Auth Helper: User logged out via non-MVC link, session cleared from client-side.');
                } else {
                    console.log('Auth Helper: Logout via MVC link, client-side session clearing skipped.');
                }
                // Yönlendirmeyi durdurmamak için e.preventDefault() KULLANILMADI
            });
        });
    }
})(); 