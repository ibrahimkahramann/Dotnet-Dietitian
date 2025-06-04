document.addEventListener('DOMContentLoaded', function() {
    'use strict';
    
    console.log('Dashboard.js: Loading dashboard scripts');
    
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
    
    // In MVC environment, we don't need the HTML-based navigation - let the server handle it
    // We can disable most of these functions to avoid conflicts with MVC routing
    // Modified handleNavigation only used for supporting legacy components when needed
    
    function handleNavigation() {
        // Check user authentication from session storage
        const userType = sessionStorage.getItem('userType') || localStorage.getItem('userType');
        
        // If no user type stored, let the server handle redirection
        if (!userType) {
            console.log('Dashboard.js: No user type found in session');
            // Let auth-helper.js handle redirections
            return;
        }
        
        // Add user type class to body for CSS targeting
        document.body.classList.add(`${userType.toLowerCase()}-user`);
        
        // Log for debugging
        console.log('Dashboard.js: Basic navigation handling');
        console.log('User type:', userType);
    }
    
    // Function to set up legacy UI components, used only if needed
    function setupLegacyUIComponents() {
        const userType = sessionStorage.getItem('userType') || localStorage.getItem('userType');
        
        // In MVC environment, we prefer server-side routes, so don't modify links automatically
        console.log('Dashboard.js: Letting server handle routes');
    }
    
    // Initialize any active charts if they exist in the page
    function initializeCharts() {
        // Helper function to check if a chart container exists
        function chartExists(id) {
            return document.getElementById(id) !== null;
        }
        
        // Only try to create charts if the containers exist
        if (!window.Chart) {
            console.log('Chart.js not loaded, skipping chart initialization');
            return;
        }
        
        // Load chart initialization only if needed
        // Rest of the chart code is unchanged...
    }
    
    // Simple function to handle content sections on single-page layouts (legacy support)
    function handleContentSections() {
        // Get all content section elements
        const contentSections = document.querySelectorAll('.content-section');
        if (contentSections.length <= 1) return; // No need to handle if only one section
        
        // Get all navigation links that might control these sections
        const navLinks = document.querySelectorAll('.list-group-item');
        
        navLinks.forEach(link => {
            link.addEventListener('click', function(e) {
                const linkId = this.id;
                if (!linkId) return;
                
                // Check if this is an MVC link (contains asp-controller/asp-action attributes)
                const hasAspAttributes = this.hasAttribute('asp-controller') || 
                                         this.hasAttribute('asp-action');
                                         
                // If it's an MVC link, let the server handle it, don't prevent default
                if (hasAspAttributes) return;
                
                // Otherwise, handle content switching for single-page layout
                const targetId = linkId.replace('-link', '-content');
                const targetContent = document.getElementById(targetId);
                
                if (targetContent) {
                    e.preventDefault(); // Only prevent default if we're handling it client-side
                    
                    // Hide all content sections
                    contentSections.forEach(section => {
                        section.classList.add('d-none');
                    });
                    
                    // Show target content section
                    targetContent.classList.remove('d-none');
                    
                    // Update active link
                    navLinks.forEach(navLink => {
                        navLink.classList.remove('active');
                    });
                    this.classList.add('active');
                }
            });
        });
    }
    
    // Initialize the dashboard
    function init() {
        // Do minimal handling to avoid conflicts with MVC
        handleNavigation();
        initializeCharts();
        handleContentSections();
        
        console.log('Dashboard.js: Dashboard initialized in MVC mode');
    }
    
    // Run initialization
    init();
}); 