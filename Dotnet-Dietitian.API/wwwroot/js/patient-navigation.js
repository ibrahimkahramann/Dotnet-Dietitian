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
            window.location.href = 'login.html?type=patient';
            return false;
        } else if (userType !== 'patient') {
            console.log('Non-patient user tried to access patient page, redirecting to login');
            window.location.href = 'login.html?authError=patient';
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
        'dashboard-link': 'patient-dashboard.html',
        'diet-plans-link': 'patient-diet-plan.html',
        'appointments-link': 'patient-appointments.html',
        'progress-link': 'progress-tracking.html',
        'messages-link': 'patient-messages.html',
        'profile-link': 'patient-profile.html',
        'settings-link': 'patient-settings.html'
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
                    link.setAttribute('href', 'patient-profile.html');
                } else if (linkText.includes('Ayarlar')) {
                    link.setAttribute('href', 'patient-settings.html');
                } else if (linkText.includes('Mesaj')) {
                    link.setAttribute('href', 'patient-messages.html');
                }
            });
        }
    }
    
    // Execute immediately
    fixAllPatientLinks();
    
    // Also run after a slight delay to catch any dynamic changes
    setTimeout(fixAllPatientLinks, 200);
}); 