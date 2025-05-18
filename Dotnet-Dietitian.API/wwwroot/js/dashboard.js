document.addEventListener('DOMContentLoaded', function() {
    'use strict';
    
    // Toggle sidebar
    const sidebarToggle = document.getElementById('sidebarToggle');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', event => {
            event.preventDefault();
            document.body.classList.toggle('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', document.body.classList.contains('sb-sidenav-toggled'));
        });
    }
    
    // Set current date in the dashboard
    const currentDateElement = document.getElementById('current-date');
    if (currentDateElement) {
        const options = { year: 'numeric', month: 'long', day: 'numeric' };
        const today = new Date();
        currentDateElement.textContent = today.toLocaleDateString('tr-TR', options);
    }
    
    // Auth and Navigation System
    function handleNavigation() {
        // Check user authentication from session storage
        const userType = sessionStorage.getItem('userType') || localStorage.getItem('userType');
        
        // If no user type stored, redirect to login
        if (!userType) {
            console.log('No user type found in session, redirecting to login');
            window.location.href = 'login.html';
            return;
        }
        
        // Get current page from URL
        const currentPage = window.location.pathname.split('/').pop() || 'index.html';
        const isPatientPage = currentPage.startsWith('patient-');
        
        // Verify user is on the correct page type
        if (userType === 'patient' && !isPatientPage) {
            // Patient trying to access dietitian page, redirect to patient dashboard
            console.log('Patient tried to access dietitian page, redirecting');
            window.location.href = 'patient-dashboard.html';
            return;
        } else if (userType === 'dietitian' && isPatientPage) {
            // Dietitian trying to access patient page, redirect to dietitian dashboard
            console.log('Dietitian tried to access patient page, redirecting');
            window.location.href = 'dietitian-dashboard.html';
            return;
        }
        
        // Add user type class to body for CSS targeting
        document.body.classList.add(`${userType}-user`);
        
        // Fix sidebar links URLS based on user type
        fixSidebarLinks(userType);
        
        // Set active sidebar link based on current page
        setActiveSidebarLink(currentPage, userType);
        
        // For pages with multiple content sections in one file (like in older versions)
        handleContentSections(currentPage, userType);
        
        // Log for debugging
        console.log('Navigation handled by dashboard.js');
        console.log('User type:', userType);
        console.log('Current page:', currentPage);
        console.log('Active link:', document.querySelector('#sidebar-wrapper .list-group-item.active')?.id);
    }
    
    // Fix sidebar links to ensure they point to the correct patient/dietitian pages
    function fixSidebarLinks(userType) {
        const sidebarLinks = document.querySelectorAll('#sidebar-wrapper .list-group-item');
        
        // Define correct URLs for patient and dietitian pages
        const patientPageURLs = {
            'dashboard-link': 'patient-dashboard.html',
            'diet-plans-link': 'patient-diet-plan.html',
            'appointments-link': 'patient-appointments.html',
            'progress-link': 'progress-tracking.html',
            'messages-link': 'patient-messages.html',
            'profile-link': 'patient-profile.html',
            'settings-link': 'patient-settings.html'
        };
        
        const dietitianPageURLs = {
            'dashboard-link': 'dietitian-dashboard.html',
            'patients-link': 'patients-view.html',
            'appointments-link': 'appointments.html',
            'diet-plans-link': 'diet-plans.html',
            'messages-link': 'messages.html',
            'profile-link': 'profile.html',
            'settings-link': 'settings.html'
        };
        
        // Apply correct URLs based on user type
        sidebarLinks.forEach(link => {
            const linkId = link.id;
            if (linkId) {
                if (userType === 'patient' && patientPageURLs[linkId]) {
                    link.setAttribute('href', patientPageURLs[linkId]);
                } else if (userType === 'dietitian' && dietitianPageURLs[linkId]) {
                    link.setAttribute('href', dietitianPageURLs[linkId]);
                }
            }
        });
        
        // Also fix dropdown menu profile/settings links
        const dropdownLinks = document.querySelectorAll('.dropdown-menu .dropdown-item');
        dropdownLinks.forEach(link => {
            const linkText = link.textContent.trim();
            
            if (userType === 'patient') {
                // On patient pages, make sure links go to patient pages
                if (linkText.includes('Profilim')) {
                    link.setAttribute('href', 'patient-profile.html');
                } else if (linkText.includes('Ayarlar')) {
                    link.setAttribute('href', 'patient-settings.html');
                } else if (linkText.includes('Mesaj')) {
                    link.setAttribute('href', 'patient-messages.html');
                }
            } else if (userType === 'dietitian') {
                // On dietitian pages, make sure links go to dietitian pages
                if (linkText.includes('Profilim')) {
                    link.setAttribute('href', 'profile.html');
                } else if (linkText.includes('Ayarlar')) {
                    link.setAttribute('href', 'settings.html');
                } else if (linkText.includes('Mesaj')) {
                    link.setAttribute('href', 'messages.html');
                }
            }
        });
    }
    
    // Set active sidebar link based on current page
    function setActiveSidebarLink(currentPage, userType) {
        const sidebarLinks = document.querySelectorAll('#sidebar-wrapper .list-group-item');
        
        // Reset all active states
        sidebarLinks.forEach(item => item.classList.remove('active'));
        
        // Key mappings for page types
        const patientPageMappings = {
            'patient-dashboard.html': 'dashboard-link',
            'patient-diet-plan.html': 'diet-plans-link',
            'patient-appointments.html': 'appointments-link',
            'progress-tracking.html': 'progress-link',
            'patient-messages.html': 'messages-link',
            'patient-profile.html': 'profile-link',
            'patient-settings.html': 'settings-link'
        };
        
        const dietitianPageMappings = {
            'dietitian-dashboard.html': 'dashboard-link',
            'patients-view.html': 'patients-link',
            'appointments.html': 'appointments-link',
            'diet-plans.html': 'diet-plans-link',
            'messages.html': 'messages-link',
            'profile.html': 'profile-link',
            'settings.html': 'settings-link'
        };
        
        // Try to find a direct mapping first
        let targetLinkId = null;
        if (userType === 'patient') {
            targetLinkId = patientPageMappings[currentPage];
        } else if (userType === 'dietitian') {
            targetLinkId = dietitianPageMappings[currentPage];
        }
        
        // Apply active class if mapping found
        if (targetLinkId) {
            const targetLink = document.getElementById(targetLinkId);
            if (targetLink) {
                targetLink.classList.add('active');
                return;
            }
        }
        
        // Fallback to direct href match if no mapping
        let matchFound = false;
        sidebarLinks.forEach(link => {
            const linkHref = link.getAttribute('href');
            if (linkHref === currentPage) {
                link.classList.add('active');
                matchFound = true;
            }
        });
        
        // Last resort: try to match by page name section
        if (!matchFound && currentPage.includes('-')) {
            const pageParts = currentPage.split('-');
            if (pageParts.length > 1) {
                const pageSection = pageParts[1].replace('.html', '');
                
                sidebarLinks.forEach(link => {
                    const linkId = link.id;
                    if (linkId && linkId.startsWith(pageSection)) {
                        link.classList.add('active');
                    }
                });
            }
        }
    }
    
    // Handle content sections for pages with multiple sections
    function handleContentSections(currentPage, userType) {
        const contentSections = document.querySelectorAll('.content-section');
        if (contentSections.length === 0) return;
        
        // Extract section name from URL
        let activeSection = 'dashboard';
        
        if (currentPage.includes('-')) {
            // Get the section name (e.g., patient-diet-plan.html -> diet-plan)
            const parts = currentPage.split('-');
            if (parts.length > 1) {
                activeSection = parts[1].replace('.html', '');
                
                // Handle special cases
                if (activeSection === 'diet') activeSection = 'diet-plans';
                if (activeSection === 'diet-plan') activeSection = 'diet-plans';
            }
        }
        
        // Hide all sections first
        contentSections.forEach(section => {
            section.classList.add('d-none');
        });
        
        // Show the active section
        const activeContentSection = document.getElementById(`${activeSection}-content`);
        if (activeContentSection) {
            activeContentSection.classList.remove('d-none');
        } else {
            // Default to dashboard content if specified section not found
            const dashboardContent = document.getElementById('dashboard-content');
            if (dashboardContent) {
                dashboardContent.classList.remove('d-none');
            }
        }
    }
    
    // Setup logout handler to clear session storage
    const logoutButtons = document.querySelectorAll('a[href="index.html"]');
    logoutButtons.forEach(button => {
        button.addEventListener('click', function(e) {
            // Clear user session data on logout
            sessionStorage.removeItem('userType');
            localStorage.removeItem('userType');
            localStorage.removeItem('userEmail');
            console.log('User logged out, session cleared');
        });
    });
    
    // Initialize auth and navigation on page load
    handleNavigation();
    
    // Add event listeners to all links that point to other dashboard pages
    document.querySelectorAll('a[href$=".html"]').forEach(link => {
        link.addEventListener('click', function(e) {
            const href = this.getAttribute('href');
            const userType = sessionStorage.getItem('userType') || localStorage.getItem('userType');
            
            // Skip non-dashboard pages like index.html, login.html, etc.
            if (['index.html', 'login.html', 'register.html', 'forgot-password.html'].includes(href)) {
                return;
            }
            
            // Skip if it's already the correct page type for this user
            const isPatientPage = href.startsWith('patient-');
            if ((userType === 'patient' && isPatientPage) || (userType === 'dietitian' && !isPatientPage)) {
                return;
            }
            
            // If user type doesn't match the page they're trying to access, prevent navigation
            e.preventDefault();
            
            // Redirect them to login page with error message
            window.location.href = 'login.html?authError=' + userType;
        });
    });
    
    // Check if we're on a dashboard page
    if (!document.querySelector('.dashboard-content')) return;
    
    // Initialize charts if Chart.js is available
    if (typeof Chart !== 'undefined') {
        initializeCharts();
    }
    
    // Handle notification dropdown
    const notificationDropdown = document.getElementById('notificationDropdown');
    if (notificationDropdown) {
        notificationDropdown.addEventListener('click', function() {
            // In a real app, this would mark notifications as read via API
            const unreadCount = document.getElementById('unreadNotificationCount');
            if (unreadCount) {
                unreadCount.textContent = '0';
                unreadCount.classList.add('d-none');
            }
        });
    }
    
    // Populate dashboard summary cards with data
    populateDashboardSummary();
    
    // Initialize the calendar if FullCalendar is available
    if (typeof FullCalendar !== 'undefined') {
        initializeCalendar();
    }
    
    // Setup the profile update form submit handler
    const profileForm = document.getElementById('profileUpdateForm');
    if (profileForm) {
        profileForm.addEventListener('submit', function(e) {
            e.preventDefault();
            updateProfile(this);
        });
    }
});

// Initialize dashboard charts
function initializeCharts() {
    // Patient progress chart
    const patientProgressCtx = document.getElementById('patientProgressChart');
    if (patientProgressCtx) {
        new Chart(patientProgressCtx, {
            type: 'line',
            data: {
                labels: ['Mart', 'Nisan', 'Mayıs', 'Haziran', 'Temmuz', 'Ağustos'],
                datasets: [{
                    label: 'Yeni Hasta',
                    backgroundColor: 'rgba(78, 115, 223, 0.05)',
                    borderColor: 'rgba(78, 115, 223, 1)',
                    pointBackgroundColor: 'rgba(78, 115, 223, 1)',
                    pointBorderColor: '#fff',
                    pointHoverBackgroundColor: '#fff',
                    pointHoverBorderColor: 'rgba(78, 115, 223, 1)',
                    data: [12, 19, 15, 26, 22, 28],
                    lineTension: 0.3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: {
                        grid: {
                            display: false
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)'
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    }
    
    // Weight progress chart for patient dashboard
    const weightChartCtx = document.getElementById('weightProgressChart');
    if (weightChartCtx) {
        new Chart(weightChartCtx, {
            type: 'line',
            data: {
                labels: ['Hafta 1', 'Hafta 2', 'Hafta 3', 'Hafta 4', 'Hafta 5', 'Hafta 6'],
                datasets: [{
                    label: 'Kilo',
                    backgroundColor: 'rgba(28, 200, 138, 0.05)',
                    borderColor: 'rgba(28, 200, 138, 1)',
                    pointBackgroundColor: 'rgba(28, 200, 138, 1)',
                    pointBorderColor: '#fff',
                    pointHoverBackgroundColor: '#fff',
                    pointHoverBorderColor: 'rgba(28, 200, 138, 1)',
                    data: [78, 76.5, 75.2, 74.1, 73.5, 72.8],
                    lineTension: 0.3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    x: {
                        grid: {
                            display: false
                        }
                    },
                    y: {
                        beginAtZero: false,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.05)'
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    }
                }
            }
        });
    }
    
    // Distribution chart (pie chart)
    const distributionChartCtx = document.getElementById('dietDistributionChart');
    if (distributionChartCtx) {
        new Chart(distributionChartCtx, {
            type: 'doughnut',
            data: {
                labels: ['Protein', 'Karbonhidrat', 'Yağ'],
                datasets: [{
                    data: [30, 50, 20],
                    backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc'],
                    hoverBackgroundColor: ['#2e59d9', '#17a673', '#2c9faf'],
                    hoverBorderColor: 'rgba(234, 236, 244, 1)',
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom'
                    }
                },
                cutout: '70%'
            }
        });
    }
}

// Populate dashboard summary cards
function populateDashboardSummary() {
    // In a real app, this data would come from an API
    const summaryData = {
        patientCount: 48,
        activePatientCount: 32,
        completedDietPlans: 124,
        upcomingAppointments: 8
    };
    
    // Update the DOM with this data
    const patientCounter = document.getElementById('patientCounter');
    if (patientCounter) patientCounter.textContent = summaryData.patientCount;
    
    const activePatientCounter = document.getElementById('activePatientCounter');
    if (activePatientCounter) activePatientCounter.textContent = summaryData.activePatientCount;
    
    const completedPlansCounter = document.getElementById('completedPlansCounter');
    if (completedPlansCounter) completedPlansCounter.textContent = summaryData.completedDietPlans;
    
    const appointmentsCounter = document.getElementById('appointmentsCounter');
    if (appointmentsCounter) appointmentsCounter.textContent = summaryData.upcomingAppointments;
}

// Initialize the appointment calendar
function initializeCalendar() {
    const calendarEl = document.getElementById('appointmentCalendar');
    if (calendarEl) {
        const calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'dayGridMonth',
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: 'dayGridMonth,timeGridWeek,timeGridDay'
            },
            events: [
                {
                    title: 'Zeynep Kaya - Kontrol',
                    start: '2025-05-10T10:30:00',
                    end: '2025-05-10T11:00:00',
                    backgroundColor: '#4e73df',
                    borderColor: '#4e73df'
                },
                {
                    title: 'Ahmet Yıldız - İlk Görüşme',
                    start: '2025-05-12T14:00:00',
                    end: '2025-05-12T15:00:00',
                    backgroundColor: '#1cc88a',
                    borderColor: '#1cc88a'
                },
                {
                    title: 'Selin Demir - Diyet Planı',
                    start: '2025-05-15T09:00:00',
                    end: '2025-05-15T09:30:00',
                    backgroundColor: '#36b9cc',
                    borderColor: '#36b9cc'
                }
            ],
            eventClick: function(info) {
                // Handle event click
                alert('Randevu: ' + info.event.title);
            }
        });
        calendar.render();
    }
}

// Handle profile update form submission
function updateProfile(form) {
    const submitBtn = form.querySelector('button[type="submit"]');
    const originalText = submitBtn.innerHTML;
    
    submitBtn.disabled = true;
    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Kaydediliyor...';
    
    // Simulate API call
    setTimeout(() => {
        submitBtn.disabled = false;
        submitBtn.innerHTML = originalText;
        
        // Show success message in a toast or alert
        const alertPlaceholder = document.getElementById('profileUpdateAlert');
        if (alertPlaceholder) {
            alertPlaceholder.innerHTML = `
                <div class="alert alert-success alert-dismissible fade show" role="alert">
                    Profil bilgileriniz başarıyla güncellendi.
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </div>
            `;
        }
    }, 1500);
}

// Dropdown menu behavior
document.querySelectorAll('.dropdown-toggle').forEach(dropdown => {
    dropdown.addEventListener('click', function(e) {
        e.stopPropagation();
    });
}); 