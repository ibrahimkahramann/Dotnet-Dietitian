/**
 * Patient navigation module to ensure correct URLs on patient pages
 */
document.addEventListener('DOMContentLoaded', function() {
    'use strict';
    
    console.log('Patient navigation module loaded');
    
    // Verify user is authorized to access this page
    function verifyPatientAuth() {
        const userType = sessionStorage.getItem('userType') || localStorage.getItem('userType');
        
        // If no user type found or not patient, redirect to login
        if (!userType) {
            console.log('No user authentication found, redirecting to login');
            window.location.href = '/Account/Login?type=patient';
            return false;
        } else if (userType !== 'Hasta') {
            console.log('Non-patient user tried to access patient page, redirecting to login');
            window.location.href = '/Account/Login?authError=patient';
            return false;
        }
        
        return true;
    }
    
    // Only proceed if user is authorized
    if (!verifyPatientAuth()) return;
    
    // Mark body as patient page for additional script detection
    document.body.classList.add('patient-page');
    
    // Explicit patient page URL mappings
    const correctPatientURLs = {
        'dashboard-link': '/Patient/Dashboard',
        'diet-plans-link': '/Patient/DietProgram',
        'appointments-link': '/Patient/Appointments',
        'progress-link': '/Patient/ProgressTracking',
        'messages-link': '/Patient/Messages',
        'profile-link': '/Patient/Profile',
        'settings-link': '/Patient/Settings'
    };
    
    // Fix all patient links with the correct URLs
    function fixAllPatientLinks() {
        console.log('Ensuring all patient links are correct');
        
        // Fix sidebar links
        const sidebarLinks = document.querySelectorAll('#sidebar-wrapper .list-group-item');
        if (sidebarLinks.length > 0) {
            sidebarLinks.forEach(link => {
                const linkId = link.id;
                if (linkId && correctPatientURLs[linkId]) {
                    link.setAttribute('href', correctPatientURLs[linkId]);
                }
            });
        }
        
        // Fix dropdown menu links
        const dropdownLinks = document.querySelectorAll('.dropdown-menu .dropdown-item');
        if (dropdownLinks.length > 0) {
            dropdownLinks.forEach(link => {
                const linkText = link.textContent.trim();
                
                if (linkText.includes('Profilim')) {
                    link.setAttribute('href', '/Patient/Profile');
                } else if (linkText.includes('Ayarlar')) {
                    link.setAttribute('href', '/Patient/Settings');
                } else if (linkText.includes('Mesaj')) {
                    link.setAttribute('href', '/Patient/Messages');
                } else if (linkText.includes('Çıkış')) {
                    link.setAttribute('href', '/Account/Logout');
                }
            });
        }
    }
    
    // Execute immediately
    fixAllPatientLinks();
    
    // Also run after a slight delay to catch any dynamic changes
    setTimeout(fixAllPatientLinks, 200);
}); 