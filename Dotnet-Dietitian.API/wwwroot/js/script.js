// Form Validation
document.addEventListener('DOMContentLoaded', function() {
    'use strict';
    
    // Form validation
    const forms = document.querySelectorAll('.needs-validation');
    
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            } else if (form.id === 'contactForm') {
                event.preventDefault();
                submitContactForm(form);
            }
            
            form.classList.add('was-validated');
        }, false);
    });
    
    // Handle contact form submission
    function submitContactForm(form) {
        const submitBtn = form.querySelector('button[type="submit"]');
        const originalText = submitBtn.innerHTML;
        
        submitBtn.disabled = true;
        submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Gönderiliyor...';
        
        // Simulate form submission
        setTimeout(() => {
            form.reset();
            form.classList.remove('was-validated');
            submitBtn.disabled = false;
            submitBtn.innerHTML = originalText;
            
            // Show success message
            showAlert('Mesajınız başarıyla gönderildi. En kısa sürede size dönüş yapacağız.', 'success');
        }, 1500);
    }
    
    // Alert function
    function showAlert(message, type = 'info') {
        const alertPlaceholder = document.createElement('div');
        alertPlaceholder.className = `alert-container position-fixed top-0 start-50 translate-middle-x p-3`;
        alertPlaceholder.style.zIndex = '9999';
        document.body.appendChild(alertPlaceholder);
        
        const wrapper = document.createElement('div');
        wrapper.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;
        
        alertPlaceholder.append(wrapper);
        
        // Auto remove after 5 seconds
        setTimeout(() => {
            const alert = new bootstrap.Alert(wrapper.querySelector('.alert'));
            alert.close();
        }, 5000);
    }
    
    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;
            
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                window.scrollTo({
                    top: targetElement.offsetTop - 80,
                    behavior: 'smooth'
                });
            }
        });
    });
    
    // Add scroll animation for elements
    const animateElements = document.querySelectorAll('.card, .testimonial-item');
    
    function checkVisibility() {
        animateElements.forEach(element => {
            const elementTop = element.getBoundingClientRect().top;
            const elementVisible = 150;
            
            if (elementTop < window.innerHeight - elementVisible) {
                element.classList.add('animate-fade-in');
            }
        });
    }
    
    // Initial check on load
    checkVisibility();
    
    // Check on scroll
    window.addEventListener('scroll', checkVisibility);
    
    // Toggle sidebar functionality
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebarToggleTop = document.getElementById('sidebarToggleTop');
    
    function toggleSidebar() {
        document.body.classList.toggle('sidebar-toggled');
        const sidebar = document.querySelector('.sidebar');
        if (sidebar) {
            sidebar.classList.toggle('toggled');
        }
    }
    
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', toggleSidebar);
    }
    
    if (sidebarToggleTop) {
        sidebarToggleTop.addEventListener('click', toggleSidebar);
    }
    
    // Close sidebar on small screens when clicking away
    window.addEventListener('resize', function() {
        if (window.innerWidth < 768) {
            document.body.classList.add('sidebar-toggled');
            const sidebar = document.querySelector('.sidebar');
            if (sidebar) {
                sidebar.classList.add('toggled');
            }
        } else {
            document.body.classList.remove('sidebar-toggled');
            const sidebar = document.querySelector('.sidebar');
            if (sidebar) {
                sidebar.classList.remove('toggled');
            }
        }
    });
    
    // Scroll to top button
    const scrollToTopButton = document.getElementById('scrollToTop');
    if (scrollToTopButton) {
        window.addEventListener('scroll', function() {
            if (window.pageYOffset > 300) {
                scrollToTopButton.style.display = 'block';
            } else {
                scrollToTopButton.style.display = 'none';
            }
        });
        
        scrollToTopButton.addEventListener('click', function() {
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }
    
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Handle user logout
    const logoutButton = document.getElementById('logoutButton');
    if (logoutButton) {
        logoutButton.addEventListener('click', function(e) {
            e.preventDefault();
            
            // In a real app, this would be an API call to log out
            console.log('User logout requested');
            
            // Redirect to login page
            setTimeout(function() {
                window.location.href = 'login.html';
            }, 500);
        });
    }
    
    // Demo alert dismiss
    const demoAlerts = document.querySelectorAll('.demo-alert .btn-close');
    demoAlerts.forEach(alert => {
        alert.addEventListener('click', function() {
            this.parentElement.classList.add('d-none');
        });
    });
    
    // API connection test
    function testApiConnection() {
        // This is a placeholder for actual API connection test
        console.log('Testing API connection...');
        
        // In a real application, this would be:
        /*
        fetch('https://api.example.com/health')
            .then(response => response.json())
            .then(data => {
                console.log('API connection successful:', data);
            })
            .catch(error => {
                console.error('API connection failed:', error);
                showApiError();
            });
        */
    }
    
    // Only test API on dashboard pages
    if (document.querySelector('.dashboard-content')) {
        testApiConnection();
    }
    
    // Toggle sidebar
    const sidebarToggleNew = document.getElementById('sidebarToggle');
    if (sidebarToggleNew) {
        sidebarToggleNew.addEventListener('click', event => {
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
    
    // Add active class to current page link in sidebar
    const currentPage = window.location.pathname.split('/').pop();
    const sidebarLinks = document.querySelectorAll('#sidebar-wrapper .list-group-item');
    
    sidebarLinks.forEach(link => {
        const linkHref = link.getAttribute('href');
        if (linkHref === currentPage) {
            // Remove active class from all links
            sidebarLinks.forEach(item => item.classList.remove('active'));
            // Add active class to current link
            link.classList.add('active');
        }
    });
});

// Add active class to nav items on scroll
window.addEventListener('scroll', function() {
    const sections = document.querySelectorAll('section');
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    
    let currentSection = '';
    
    sections.forEach(section => {
        const sectionTop = section.offsetTop - 100;
        const sectionHeight = section.clientHeight;
        if (window.pageYOffset >= sectionTop && window.pageYOffset < sectionTop + sectionHeight) {
            currentSection = section.getAttribute('id');
        }
    });
    
    navLinks.forEach(link => {
        link.classList.remove('active');
        if (link.getAttribute('href') === '#' + currentSection) {
            link.classList.add('active');
        }
    });
});

// Handle navigation to dashboard based on user type
const dashboardLinks = document.querySelectorAll('a[href="dashboard.html"], a[href="#dashboard"]');
dashboardLinks.forEach(link => {
    link.addEventListener('click', function(e) {
        // Check if user type is stored
        const userType = sessionStorage.getItem('userType') || localStorage.getItem('userType');
        
        if (userType) {
            e.preventDefault();
            // Redirect to the appropriate dashboard based on user type
            if (userType === 'patient') {
                window.location.href = 'patient-dashboard.html';
            } else if (userType === 'dietitian') {
                window.location.href = 'dietitian-dashboard.html';
            }
        }
        // If no user type is stored, let the default link work (usually to login page)
    });
}); 