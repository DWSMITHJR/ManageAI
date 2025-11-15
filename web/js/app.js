// Management Dashboard - Main Application

// Application State
let appState = {
    isAuthenticated: false,
    currentUser: null,
    dashboardData: {
        unreadEmails: 0,
        todayEvents: 0,
        pendingTodos: 0,
        services: 0
    },
    emails: [],
    events: [],
    todos: [],
    services: [],
    testResults: []
};

// DOM Elements (will be initialized after DOM is ready)
let mainContent, dashboardView, emailView, servicesView, todosView, testsView;
let navDashboard, navEmail, navServices, navTodos, navTests;
let authStatus, authText;

// Initialize DOM elements
function initializeDOMElements() {
    // Main View Elements
    mainContent = document.getElementById('main-content');
    dashboardView = document.getElementById('dashboard-view');
    emailView = document.getElementById('email-view');
    servicesView = document.getElementById('services-view');
    todosView = document.getElementById('todos-view');
    testsView = document.getElementById('tests-view');

    // Navigation Elements
    navDashboard = document.getElementById('nav-dashboard');
    navEmail = document.getElementById('nav-email');
    navServices = document.getElementById('nav-services');
    navTodos = document.getElementById('nav-todos');
    navTests = document.getElementById('nav-tests');

    // Status Elements
    authStatus = document.getElementById('auth-status');
    authText = document.getElementById('auth-text');

    // Debug: Log element availability
    console.log('DOM Elements Check:', {
        mainContent: !!mainContent,
        dashboardView: !!dashboardView,
        emailView: !!emailView,
        servicesView: !!servicesView,
        todosView: !!todosView,
        testsView: !!testsView,
        navDashboard: !!navDashboard,
        navEmail: !!navEmail,
        navServices: !!navServices,
        navTodos: !!navTodos,
        navTests: !!navTests,
        authStatus: !!authStatus,
        authText: !!authText
    });
}

// Theme System
class ThemeManager {
    constructor() {
        this.currentTheme = this.getStoredTheme() || 'light';
        this.themes = ['light', 'dark', 'ocean'];
        this.init();
    }

    init() {
        this.applyTheme(this.currentTheme);
        this.setupEventListeners();
        this.updateThemeButtons();
    }

    getStoredTheme() {
        return localStorage.getItem('themer-theme');
    }

    setStoredTheme(theme) {
        localStorage.setItem('themer-theme', theme);
    }

    applyTheme(theme) {
        if (!this.themes.includes(theme)) {
            console.warn(`Invalid theme: ${theme}. Using light theme.`);
            theme = 'light';
        }

        // Remove existing theme attribute
        document.documentElement.removeAttribute('data-theme');
        
        // Apply new theme
        if (theme !== 'light') {
            document.documentElement.setAttribute('data-theme', theme);
        }

        // Update body class for legacy support
        document.body.className = document.body.className.replace(/cyber-theme|light-theme|dark-theme|ocean-theme/g, '');
        
        if (theme === 'dark') {
            document.body.classList.add('dark-theme');
        } else if (theme === 'ocean') {
            document.body.classList.add('ocean-theme');
        } else {
            document.body.classList.add('cyber-theme');
        }

        this.currentTheme = theme;
        this.setStoredTheme(theme);
        this.updateThemeButtons();

        // Dispatch theme change event
        document.dispatchEvent(new CustomEvent('themechange', { 
            detail: { theme: theme } 
        }));

        console.log(`🎨 Theme changed to: ${theme}`);
    }

    setupEventListeners() {
        // Theme switcher buttons
        document.querySelectorAll('.theme-btn').forEach(btn => {
            btn.addEventListener('click', (e) => {
                const theme = e.currentTarget.getAttribute('data-theme');
                this.applyTheme(theme);
            });
        });

        // Keyboard shortcuts
        document.addEventListener('keydown', (e) => {
            // Ctrl/Cmd + Shift + T to cycle themes
            if ((e.ctrlKey || e.metaKey) && e.shiftKey && e.key === 'T') {
                e.preventDefault();
                this.cycleTheme();
            }
        });
    }

    cycleTheme() {
        const currentIndex = this.themes.indexOf(this.currentTheme);
        const nextIndex = (currentIndex + 1) % this.themes.length;
        this.applyTheme(this.themes[nextIndex]);
    }

    updateThemeButtons() {
        document.querySelectorAll('.theme-btn').forEach(btn => {
            const btnTheme = btn.getAttribute('data-theme');
            if (btnTheme === this.currentTheme) {
                btn.classList.add('active');
            } else {
                btn.classList.remove('active');
            }
        });
    }

    getCurrentTheme() {
        return this.currentTheme;
    }

    // Get CSS variable value
    getCSSVariable(variable) {
        return getComputedStyle(document.documentElement)
            .getPropertyValue(variable).trim();
    }

    // Generate theme preview
    generateThemePreview(theme) {
        const preview = {
            primary: this.getCSSVariable('--themer-primary-500'),
            secondary: this.getCSSVariable('--themer-secondary-500'),
            background: this.getCSSVariable('--themer-gray-50'),
            surface: this.getCSSVariable('--themer-gray-100'),
            text: this.getCSSVariable('--themer-gray-900')
        };

        if (theme === 'dark') {
            preview.background = this.getCSSVariable('--themer-gray-50');
            preview.surface = this.getCSSVariable('--themer-gray-100');
            preview.text = this.getCSSVariable('--themer-gray-900');
        }

        return preview;
    }
}

// Initialize Theme System
function initializeThemeSystem() {
    try {
        console.log('🎨 Initializing Theme System...');
        window.themeManager = new ThemeManager();
        console.log('✅ Theme System initialized successfully');
    } catch (error) {
        console.error('❌ Theme System initialization failed:', error);
        // Fallback to light theme
        document.documentElement.setAttribute('data-theme', 'light');
    }
}

// Global theme utilities
window.switchTheme = function(theme) {
    if (window.themeManager) {
        window.themeManager.applyTheme(theme);
    }
};

window.getCurrentTheme = function() {
    return window.themeManager ? window.themeManager.getCurrentTheme() : 'light';
};

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    console.log('🚀 ManagementAI Application Initializing...');
    
    // Initialize Theme System
    initializeThemeSystem();
    
    // Test toast notification system
    try {
        console.log('🔔 Testing toast notification system...');
        showInfoToast('Application loaded successfully', 'System Ready', {
            details: `
                <div class="small text-muted mt-2">
                    <div class="d-flex justify-content-between mb-1">
                        <span>🔧 Error Handler:</span><span class="fw-bold">Active</span>
                    </div>
                    <div class="d-flex justify-content-between">
                        <span>📊 Toast System:</span><span class="fw-bold">Operational</span>
                    </div>
                    <div class="d-flex justify-content-between">
                        <span>🎨 Theme System:</span><span class="fw-bold">Loaded</span>
                    </div>
                </div>
            `
        });
        console.log('✅ Toast system test completed');
    } catch (testError) {
        console.error('❌ Toast system test failed:', testError);
    }
    
    initializeApp();
});

async function initializeApp() {
    // Initialize DOM elements first
    initializeDOMElements();
    
    // Initialize Secrets Manager first
    initializeSecretsManager();
    
    // Auto-authenticate for local development
    if (window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1') {
        appState.isAuthenticated = true;
        appState.currentUser = 'Local User';
        updateAuthStatus();
        if (window.showStatusToast) {
            showStatusToast('Auto-authenticated for local development', 'info');
        }
    }
    
    // Initialize UI
    updateAuthStatus();
    await loadDashboardData();
    
    // Setup event listeners
    setupNavigation();
    setupEventListeners();
    
    // Show dashboard by default
    showView('dashboard');
}

// Navigation
function setupNavigation() {
    // Check if navigation elements exist
    if (!navDashboard || !navEmail || !navServices || !navTodos || !navTests) {
        console.error('Navigation elements not found:', {
            navDashboard: !!navDashboard,
            navEmail: !!navEmail,
            navServices: !!navServices,
            navTodos: !!navTodos,
            navTests: !!navTests
        });
        return;
    }
    
    navDashboard.addEventListener('click', (e) => {
        e.preventDefault();
        showView('dashboard');
        showToast('Dashboard view loaded', 'info');
    });
    
    navEmail.addEventListener('click', (e) => {
        e.preventDefault();
        showView('email');
        showToast('Email view loaded', 'info');
    });
    
    navServices.addEventListener('click', (e) => {
        e.preventDefault();
        showView('services');
        showToast('Services view loaded', 'info');
    });
    
    navTodos.addEventListener('click', (e) => {
        e.preventDefault();
        showView('todos');
        showToast('Tasks view loaded', 'info');
    });
    
    navTests.addEventListener('click', (e) => {
        e.preventDefault();
        showView('tests');
        showToast('Tests view loaded', 'info');
    });
}

function showView(viewName) {
    console.log('showView called with:', viewName);
    
    // Check if view elements exist
    if (!dashboardView || !emailView || !servicesView || !todosView || !testsView) {
        console.error('View elements not found:', {
            dashboardView: !!dashboardView,
            emailView: !!emailView,
            servicesView: !!servicesView,
            todosView: !!todosView,
            testsView: !!testsView
        });
        return;
    }
    
    // Hide all views
    dashboardView.style.display = 'none';
    emailView.style.display = 'none';
    servicesView.style.display = 'none';
    todosView.style.display = 'none';
    testsView.style.display = 'none';
    
    // Remove active class from all nav items
    const navLinks = document.querySelectorAll('.nav-link');
    navLinks.forEach(link => link.classList.remove('active'));
    
    console.log('Showing view:', viewName);
    
    // Show selected view and update nav
    switch(viewName) {
        case 'dashboard':
            dashboardView.style.display = 'block';
            navDashboard.classList.add('active');
            refreshDashboard();
            break;
        case 'email':
            emailView.style.display = 'block';
            navEmail.classList.add('active');
            loadEmailData();
            break;
        case 'services':
            servicesView.style.display = 'block';
            navServices.classList.add('active');
            loadServices();
            break;
        case 'todos':
            todosView.style.display = 'block';
            navTodos.classList.add('active');
            loadTodos();
            break;
        case 'tests':
            testsView.style.display = 'block';
            navTests.classList.add('active');
            loadTestConfiguration();
            break;
        default:
            console.error('Unknown view:', viewName);
    }
}

// Event Listeners
function setupEventListeners() {
    // Dashboard actions
    const refreshDashboardBtn = document.getElementById('refresh-dashboard-btn');
    if (refreshDashboardBtn) refreshDashboardBtn.addEventListener('click', refreshDashboard);
    
    const runTestsBtn = document.getElementById('run-tests-btn');
    if (runTestsBtn) runTestsBtn.addEventListener('click', runAllTests);
    
    const composeEmailBtn = document.getElementById('compose-email-btn');
    if (composeEmailBtn) composeEmailBtn.addEventListener('click', showEmailModal);
    
    const addTaskBtn = document.getElementById('add-task-btn');
    if (addTaskBtn) addTaskBtn.addEventListener('click', showTodoModal);

    // Email actions
    const refreshEmailsBtn = document.getElementById('refresh-emails-btn');
    if (refreshEmailsBtn) refreshEmailsBtn.addEventListener('click', loadEmails);
    
    const refreshCalendarBtn = document.getElementById('refresh-calendar-btn');
    if (refreshCalendarBtn) refreshCalendarBtn.addEventListener('click', loadEvents);
    
    const sendEmailBtn = document.getElementById('send-email-btn');
    if (sendEmailBtn) sendEmailBtn.addEventListener('click', sendEmail);
    
    // Services actions - add null checks
    const testAllServicesBtn = document.getElementById('test-all-services-btn');
    if (testAllServicesBtn) testAllServicesBtn.addEventListener('click', testAllServices);
    
    const checkCreditsBtn = document.getElementById('check-credits-btn');
    if (checkCreditsBtn) checkCreditsBtn.addEventListener('click', checkAllCredits);
    
    // Todos actions - add null checks
    const addTodoBtn = document.getElementById('add-todo-btn');
    if (addTodoBtn) addTodoBtn.addEventListener('click', showTodoModal);
    
    const saveTodoBtn = document.getElementById('save-todo-btn');
    if (saveTodoBtn) saveTodoBtn.addEventListener('click', saveTodo);
    
    // Tests actions - add null checks
    const runAllTestsBtn = document.getElementById('run-all-tests-btn');
    if (runAllTestsBtn) runAllTestsBtn.addEventListener('click', runAllTests);
    
    const exportResultsBtn = document.getElementById('export-results-btn');
    if (exportResultsBtn) exportResultsBtn.addEventListener('click', exportTestResults);
    
    // Form submit handlers
    const todoForm = document.getElementById('todoForm');
    if (todoForm) {
        todoForm.addEventListener('submit', (e) => {
            e.preventDefault();
            saveTodo();
        });
    }
    
    const emailForm = document.getElementById('emailForm');
    if (emailForm) {
        emailForm.addEventListener('submit', (e) => {
            e.preventDefault();
            sendEmail();
        });
    }
    
    // Authentication - add null check
    const logoutBtn = document.getElementById('logout-btn');
    if (logoutBtn) logoutBtn.addEventListener('click', logout);
}

// Authentication
function updateAuthStatus() {
    if (appState.isAuthenticated) {
        authText.textContent = `Authenticated as ${appState.currentUser}`;
        authStatus.classList.add('text-success');
    } else {
        authText.textContent = 'Not Authenticated';
        authStatus.classList.remove('text-success');
    }
}

async function authenticate() {
    try {
        // In a real app, this would call the authentication API
        showLoadingToast('Authenticating...');
        
        try {
            // Simulate authentication
            await new Promise(resolve => setTimeout(resolve, 1000));
            
            appState.isAuthenticated = true;
            appState.currentUser = '[DEMO] Demo User';
            updateAuthStatus();
            
            showSuccessToast('Authentication successful!', 'Login Success');
            
            // Load dashboard data after successful auth
            await loadDashboardData();
            
        } catch (error) {
            console.error('Authentication failed:', error);
            showErrorToast('Authentication failed: ' + error.message, 'Login Failed');
        }
    } catch (error) {
        console.error('Authentication failed:', error);
        showErrorToast('Authentication failed: ' + error.message, 'Login Failed');
    }
}

function logout() {
    appState.isAuthenticated = false;
    appState.currentUser = null;
    updateAuthStatus();
    showStatusToast('Logged out successfully', 'info');
    showView('dashboard');
}

// Dashboard Functions
async function loadDashboardData() {
    try {
        // In a real app, this would fetch data from the API
        await Promise.all([
            loadEmails(),
            loadEvents(),
            loadTodos(),
            loadServices()
        ]);
        
        updateDashboardSummary();
    } catch (error) {
        showToast('Failed to load dashboard data: ' + error.message, 'error');
    }
}

function updateDashboardSummary() {
    const unreadElement = document.getElementById('unread-count');
    const eventsElement = document.getElementById('events-count');
    const pendingElement = document.getElementById('pending-count');
    const servicesElement = document.getElementById('services-count');
    
    if (unreadElement) unreadElement.textContent = appState.dashboardData.unreadEmails;
    if (eventsElement) eventsElement.textContent = appState.dashboardData.todayEvents;
    if (pendingElement) pendingElement.textContent = appState.dashboardData.pendingTodos;
    if (servicesElement) servicesElement.textContent = appState.dashboardData.services;
}

async function refreshDashboard() {
    return await errorHandler.executeWithRetry(
        async () => {
            const loadingToastId = showLoadingToast('Refreshing dashboard data...');
            
            try {
                const startTime = Date.now();
                await loadDashboardData();
                const duration = ((Date.now() - startTime) / 1000).toFixed(1);
                
                hideToast(loadingToastId);
                
                // Detailed success notification
                const successDetails = {
                    servicesUpdated: appState.services.length,
                    emailsLoaded: appState.emails.length,
                    todosCount: appState.todos.length,
                    eventsToday: appState.events.filter(e => {
                        const eventDate = new Date(e.date);
                        const today = new Date();
                        return eventDate.toDateString() === today.toDateString();
                    }).length
                };
                
                showSuccessToast(
                    `Dashboard refreshed successfully in ${duration}s`,
                    'Dashboard Updated',
                    {
                        details: `
                            <div class="small text-muted mt-2">
                                <div class="d-flex justify-content-between mb-1">
                                    <span>📊 Services:</span><span class="fw-bold">${successDetails.servicesUpdated}</span>
                                </div>
                                <div class="d-flex justify-content-between mb-1">
                                    <span>📧 Emails:</span><span class="fw-bold">${successDetails.emailsLoaded}</span>
                                </div>
                                <div class="d-flex justify-content-between mb-1">
                                    <span>✅ Tasks:</span><span class="fw-bold">${successDetails.todosCount}</span>
                                </div>
                                <div class="d-flex justify-content-between">
                                    <span>📅 Events Today:</span><span class="fw-bold">${successDetails.eventsToday}</span>
                                </div>
                            </div>
                        `
                    }
                );
                
                return true;
            } catch (error) {
                hideToast(loadingToastId);
                throw error;
            }
        },
        { 
            operation: 'refresh-dashboard',
            component: 'dashboard'
        },
        { 
            maxRetries: 2,
            delay: 500,
            backoffMultiplier: 1.5
        }
    ).catch(error => {
        errorHandler.log(error, { 
            operation: 'refresh-dashboard',
            component: 'dashboard',
            userAction: 'click refresh button'
        });
        
        // Detailed failure notification
        showErrorToast(
            `Dashboard refresh failed: ${error.message}`,
            'Refresh Error',
            {
                details: `
                    <div class="small text-muted mt-2">
                        <div class="mb-2">⚠️ Unable to load latest data</div>
                        <div class="d-flex justify-content-between mb-1">
                            <span>Attempt:</span><span class="fw-bold">1/3</span>
                        </div>
                        <div class="d-flex justify-content-between mb-1">
                            <span>Error:</span><span class="fw-bold text-danger">${error.name}</span>
                        </div>
                        <div class="mt-2">
                            <button class="btn btn-sm btn-outline-light" onclick="refreshDashboard()">
                                🔄 Retry Now
                            </button>
                        </div>
                    </div>
                `
            }
        );
        
        // Fallback: try to load cached data
        try {
            if (appState.services.length === 0) {
                // Load demo data as fallback
                appState.services = [
                    { name: '[DEMO] Anthropic Claude', type: 'AI Service', status: 'Active', lastTest: '2 hours ago', credits: '85%', testResult: 'Passed' },
                    { name: '[DEMO] OpenAI GPT-4', type: 'AI Service', status: 'Active', lastTest: '1 hour ago', credits: '92%', testResult: 'Passed' },
                    { name: '[DEMO] Google Gemini', type: 'AI Service', status: 'Active', lastTest: '30 mins ago', credits: '78%', testResult: 'Passed' },
                    { name: '[DEMO] Hugging Face', type: 'ML Platform', status: 'Active', lastTest: '45 mins ago', credits: '88%', testResult: 'Passed' }
                ];
                renderServices();
                
                showInfoToast(
                    'Showing cached demo data. Some features may be limited.',
                    'Limited Mode',
                    {
                        details: `
                            <div class="small text-muted mt-2">
                                <div class="mb-2">📦 Using cached data</div>
                                <div class="d-flex justify-content-between">
                                    <span>Data Source:</span><span class="fw-bold">Demo Cache</span>
                                </div>
                            </div>
                        `
                    }
                );
            }
        } catch (fallbackError) {
            errorHandler.log(fallbackError, { 
                operation: 'refresh-dashboard-fallback',
                component: 'dashboard'
            });
        }
    });
}

// Email Functions
async function loadEmailData() {
    await Promise.all([
        loadEmails(),
        loadEvents()
    ]);
}

async function loadEmails() {
    try {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500));
        
        appState.emails = [
            {
                id: '1',
                subject: '[DEMO] Welcome to Management Dashboard',
                sender: '[DEMO] system@management.com',
                preview: '[DEMO] Thank you for using Management Dashboard...',
                time: '2 hours ago',
                unread: true
            },
            {
                id: '2',
                subject: '[DEMO] Service Test Results',
                sender: '[DEMO] tests@management.com',
                preview: '[DEMO] Your scheduled service tests have completed...',
                time: '5 hours ago',
                unread: true
            }
        ];
        
        appState.dashboardData.unreadEmails = appState.emails.filter(e => e.unread).length;
        renderEmails();
    } catch (error) {
        showToast('Failed to load emails: ' + error.message, 'error');
    }
}

function renderEmails() {
    try {
        const container = document.getElementById('emails-container');
        const recentContainer = document.getElementById('recent-emails');
        
        const emailHtml = appState.emails.map(email => `
        <div class="email-item p-3 border-bottom ${email.unread ? 'bg-light' : ''}">
            <div class="d-flex justify-content-between align-items-start">
                <div class="flex-grow-1">
                    <h6 class="mb-1 ${email.unread ? 'fw-bold' : ''}">${email.subject}</h6>
                    <p class="mb-1 text-muted small">${email.sender}</p>
                    <p class="mb-0 text-truncate">${email.preview}</p>
                </div>
                <small class="text-muted ms-2">${email.time}</small>
            </div>
        </div>
    `).join('');
    
        if (container) container.innerHTML = emailHtml || '<p class="text-muted text-center">No emails found</p>';
        if (recentContainer) recentContainer.innerHTML = emailHtml || '<p class="text-muted text-center">No recent emails</p>';
    } catch (error) {
        showToast('Error rendering emails: ' + error.message, 'error');
    }
}

async function loadEvents() {
    try {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500));
        
        const today = new Date();
        appState.events = [
            {
                id: '1',
                title: '[DEMO] Team Standup',
                start: new Date(today.setHours(9, 0, 0, 0)),
                end: new Date(today.setHours(9, 30, 0, 0)),
                description: '[DEMO] Daily team synchronization meeting'
            },
            {
                id: '2',
                title: '[DEMO] Service Review',
                start: new Date(today.setHours(14, 0, 0, 0)),
                end: new Date(today.setHours(15, 0, 0, 0)),
                description: '[DEMO] Review service performance and metrics'
            }
        ];
        
        appState.dashboardData.todayEvents = appState.events.length;
        renderEvents();
    } catch (error) {
        showToast('Failed to load events: ' + error.message, 'error');
    }
}

function renderEvents() {
    try {
        const container = document.getElementById('events-container');
        const upcomingContainer = document.getElementById('upcoming-events');
        
        const eventHtml = appState.events.map(event => `
        <div class="event-item p-3 border-bottom">
            <div class="d-flex justify-content-between align-items-start">
                <div class="flex-grow-1">
                    <h6 class="mb-1">${event.title}</h6>
                    <p class="mb-1 text-muted small">${event.description}</p>
                    <p class="mb-0">
                        <small class="text-muted">
                            ${new Date(event.start).toLocaleString()} - ${new Date(event.end).toLocaleTimeString()}
                        </small>
                    </p>
                </div>
            </div>
        </div>
    `).join('');
    
        if (container) container.innerHTML = eventHtml || '<p class="text-muted text-center">No events today</p>';
        if (upcomingContainer) upcomingContainer.innerHTML = eventHtml || '<p class="text-muted text-center">No upcoming events</p>';
    } catch (error) {
        showToast('Error rendering events: ' + error.message, 'error');
    }
}

function showEmailModal() {
    return errorHandler.executeWithRetry(
        async () => {
            const modalElement = document.getElementById('emailModal');
            if (!modalElement) {
                throw new Error('Email modal element not found in DOM');
            }
            
            // Check if Bootstrap is loaded
            if (typeof bootstrap === 'undefined') {
                throw new Error('Bootstrap library not loaded');
            }
            
            const modal = new bootstrap.Modal(modalElement);
            
            // Add event listeners for error handling
            modalElement.addEventListener('hidden.bs.modal', function handler() {
                // Clean up event listener
                modalElement.removeEventListener('hidden.bs.modal', handler);
            }, { once: true });
            
            modal.show();
            
            // Detailed success notification
            showSuccessToast(
                'Email composer opened successfully',
                'Compose Email',
                {
                    details: `
                        <div class="small text-muted mt-2">
                            <div class="mb-2">📧 Ready to compose new email</div>
                            <div class="d-flex justify-content-between mb-1">
                                <span>Mode:</span><span class="fw-bold">New Message</span>
                            </div>
                            <div class="d-flex justify-content-between">
                                <span>Templates:</span><span class="fw-bold">4 Available</span>
                            </div>
                        </div>
                    `
                }
            );
            
            return true;
        },
        { 
            operation: 'show-email-modal',
            component: 'email'
        },
        { 
            maxRetries: 1,
            delay: 200
        }
    ).catch(error => {
        errorHandler.log(error, { 
            operation: 'show-email-modal',
            component: 'email',
            userAction: 'click compose email button'
        });
        
        // Detailed failure notification
        showErrorToast(
            `Email composer failed: ${error.message}`,
            'Modal Error',
            {
                details: `
                    <div class="small text-muted mt-2">
                        <div class="mb-2">⚠️ Unable to open email composer</div>
                        <div class="d-flex justify-content-between mb-1">
                            <span>Error Type:</span><span class="fw-bold text-danger">UI Component</span>
                        </div>
                        <div class="d-flex justify-content-between mb-2">
                            <span>Bootstrap:</span><span class="fw-bold">${typeof bootstrap !== 'undefined' ? 'Loaded' : 'Missing'}</span>
                        </div>
                        <div class="mt-2">
                            <button class="btn btn-sm btn-outline-light" onclick="showView('email')">
                                📧 Go to Email Section
                            </button>
                        </div>
                    </div>
                `
            }
        );
        
        // Fallback: try to navigate to email section
        try {
            showView('email');
            showInfoToast(
                'Navigated to email section instead.',
                'Fallback Navigation',
                {
                    details: `
                        <div class="small text-muted mt-2">
                            <div class="mb-2">🔄 Alternative navigation used</div>
                            <div class="d-flex justify-content-between">
                                <span>Section:</span><span class="fw-bold">Email Inbox</span>
                            </div>
                        </div>
                    `
                }
            );
        } catch (fallbackError) {
            errorHandler.log(fallbackError, { 
                operation: 'show-email-modal-fallback',
                component: 'email'
            });
        }
    });
}

async function sendEmail() {
    const to = document.getElementById('email-to').value;
    const subject = document.getElementById('email-subject').value;
    const body = document.getElementById('email-body').value;
    
    try {
        const loadingToast = showLoadingToast('Sending email...');
        
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        const modal = bootstrap.Modal.getInstance(document.getElementById('emailModal'));
        modal.hide();
        
        // Clear form
        document.getElementById('emailForm').reset();
        
        hideLoadingToast(loadingToast);
        showActionToast('Email sent', `to ${to}`, true);
    } catch (error) {
        showErrorToast('Failed to send email: ' + error.message, 'Email Error');
    }
}

// Services Functions
async function loadServices() {
    try {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500));
        
        appState.services = [
            {
                name: '[DEMO] Service 1',
                type: '[DEMO] Type 1',
                status: 'Active',
                lastTest: '[DEMO] 2 hours ago',
                credits: '[DEMO] $45.67',
                testResult: '[DEMO] Passed'
            },
            {
                name: '[DEMO] Service 2',
                type: '[DEMO] Type 2',
                status: 'Active',
                lastTest: '[DEMO] 1 day ago',
                credits: '[DEMO] $123.45',
                testResult: '[DEMO] Passed'
            },
            {
                name: '[DEMO] Service 3',
                type: '[DEMO] Type 3',
                status: 'Active',
                lastTest: '[DEMO] 3 days ago',
                credits: '[DEMO] $78.90',
                testResult: '[DEMO] Passed'
            },
            {
                name: '[DEMO] Service 4',
                type: '[DEMO] Type 4',
                status: 'Inactive',
                lastTest: '[DEMO] 1 week ago',
                credits: '[DEMO] $12.34',
                testResult: '[DEMO] Failed'
            }
        ];
        
        appState.dashboardData.services = appState.services.length;
        renderServices();
    } catch (error) {
        showToast('Failed to load services: ' + error.message, 'error');
    }
}

function renderServices() {
    const tbody = document.querySelector('#services-table tbody');
    
    if (!tbody) {
        showToast('Services table not found', 'error');
        return;
    }
    
    try {
        const rows = appState.services.map(service => `
        <tr>
            <td>${service.name}</td>
            <td>${service.type}</td>
            <td>
                <span class="badge bg-${service.status === 'Active' ? 'success' : 'secondary'}">
                    ${service.status}
                </span>
            </td>
            <td>${service.lastTest || 'Never'}</td>
            <td>${service.credits || 'N/A'}</td>
            <td>
                <button class="btn btn-sm btn-outline-primary me-1" onclick="testService('${service.id}')">
                    Test
                </button>
                <button class="btn btn-sm btn-outline-secondary" onclick="checkCredits('${service.id}')">
                    Credits
                </button>
            </td>
        </tr>
    `).join('');
    
        tbody.innerHTML = rows || '<tr><td colspan="6" class="text-center text-muted">No services found</td></tr>';
    } catch (error) {
        showToast('Error rendering services: ' + error.message, 'error');
    }
}

async function testService(serviceName) {
    showToast(`Testing ${serviceName}...`, 'info');
    
    // Simulate test
    await new Promise(resolve => setTimeout(resolve, 2000));
    
    showToast(`${serviceName} test completed successfully`, 'success');
    loadServices(); // Refresh the services list
}

async function checkCredits(serviceName) {
    showToast(`Checking credits for ${serviceName}...`, 'info');
    
    // Simulate credit check
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    showToast(`${serviceName} credits: $${(Math.random() * 100).toFixed(2)}`, 'info');
}

async function testAllServices() {
    showToast('Testing all services...', 'info');
    
    for (const service of appState.services) {
        if (service.status === 'Active') {
            await testService(service.name);
        }
    }
}

async function checkAllCredits() {
    showToast('Checking credits for all services...', 'info');
    
    for (const service of appState.services) {
        await checkCredits(service.name);
    }
}

// Todos Functions
async function loadTodos() {
    try {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 500));
        
        appState.todos = [
            {
                id: '1',
                title: '[DEMO] Review service performance',
                description: '[DEMO] Analyze weekly performance metrics',
                priority: 'high',
                dueAt: new Date(Date.now() + 24 * 60 * 60 * 1000),
                isCompleted: false,
                createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000),
                tags: ['[DEMO] work', '[DEMO] service']
            },
            {
                id: '2',
                title: '[DEMO] Service Review',
                description: '[DEMO] Review service performance and metrics',
                priority: 'medium',
                dueAt: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000),
                isCompleted: false,
                createdAt: new Date(Date.now() - 1 * 24 * 60 * 60 * 1000),
                tags: ['[DEMO] documentation']
            },
            {
                id: '3',
                title: '[DEMO] Code review',
                description: '[DEMO] Review pull requests from team',
                priority: 'medium',
                dueAt: null,
                isCompleted: true,
                createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
                tags: ['[DEMO] work', '[DEMO] review']
            }
        ];
        
        updateTodoStatistics();
        renderTodos();
    } catch (error) {
        showToast('Failed to load todos: ' + error.message, 'error');
    }
}

function updateTodoStatistics() {
    const total = appState.todos.length;
    const completed = appState.todos.filter(t => t.isCompleted).length;
    const pending = total - completed;
    const completionRate = total > 0 ? Math.round((completed / total) * 100) : 0;
    
    const totalElement = document.getElementById('todo-total');
    const completedElement = document.getElementById('todo-completed');
    const pendingElement = document.getElementById('todo-pending');
    const rateElement = document.getElementById('todo-completion-rate');
    
    if (totalElement) totalElement.textContent = total;
    if (completedElement) completedElement.textContent = completed;
    if (pendingElement) pendingElement.textContent = pending;
    if (rateElement) rateElement.textContent = completionRate + '%';
}

function renderTodos() {
    try {
        const container = document.getElementById('todos-container');
        
        const todoHtml = appState.todos.map(todo => `
        <div class="todo-item p-3 border-bottom ${todo.isCompleted ? 'bg-light' : ''}">
            <div class="d-flex justify-content-between align-items-start">
                <div class="flex-grow-1">
                    <div class="form-check">
                        <input class="form-check-input" type="checkbox" 
                               ${todo.isCompleted ? 'checked' : ''} 
                               onchange="toggleTodo('${todo.id}')">
                        <label class="form-check-label ${todo.isCompleted ? 'text-decoration-line-through text-muted' : ''}">
                            <h6 class="mb-1">${todo.title}</h6>
                            <p class="mb-1 text-muted small">${todo.description}</p>
                        </label>
                    </div>
                    <div class="d-flex gap-2 mt-2">
                        <span class="badge bg-${getPriorityColor(todo.priority)}">${todo.priority}</span>
                        ${todo.dueAt ? `<small class="text-muted">Due: ${new Date(todo.dueAt).toLocaleDateString()}</small>` : ''}
                    </div>
                </div>
                <div class="btn-group btn-group-sm">
                    <button class="btn btn-outline-primary" onclick="editTodo('${todo.id}')">Edit</button>
                    <button class="btn btn-outline-danger" onclick="deleteTodo('${todo.id}')">Delete</button>
                </div>
            </div>
        </div>
    `).join('');
    
        if (container) container.innerHTML = todoHtml || '<p class="text-muted text-center">No tasks found</p>';
    } catch (error) {
        showToast('Error rendering tasks: ' + error.message, 'error');
    }
}

function getPriorityColor(priority) {
    switch(priority.toLowerCase()) {
        case 'high': return 'danger';
        case 'medium': return 'warning';
        case 'low': return 'success';
        default: return 'secondary';
    }
}

function showTodoModal() {
    return errorHandler.executeWithRetry(
        async () => {
            const modalElement = document.getElementById('todoModal');
            if (!modalElement) {
                throw new Error('Task modal element not found in DOM');
            }
            
            // Check if Bootstrap is loaded
            if (typeof bootstrap === 'undefined') {
                throw new Error('Bootstrap library not loaded');
            }
            
            const modal = new bootstrap.Modal(modalElement);
            
            // Add event listeners for error handling
            modalElement.addEventListener('hidden.bs.modal', function handler() {
                // Clean up event listener
                modalElement.removeEventListener('hidden.bs.modal', handler);
            }, { once: true });
            
            modal.show();
            
            // Detailed success notification
            showSuccessToast(
                'Task composer opened successfully',
                'New Task',
                {
                    details: `
                        <div class="small text-muted mt-2">
                            <div class="mb-2">✅ Ready to create new task</div>
                            <div class="d-flex justify-content-between mb-1">
                                <span>Mode:</span><span class="fw-bold">Task Creation</span>
                            </div>
                            <div class="d-flex justify-content-between mb-1">
                                <span>Priority Levels:</span><span class="fw-bold">3 Available</span>
                            </div>
                            <div class="d-flex justify-content-between">
                                <span>Current Tasks:</span><span class="fw-bold">${appState.todos.length}</span>
                            </div>
                        </div>
                    `
                }
            );
            
            return true;
        },
        { 
            operation: 'show-todo-modal',
            component: 'todos'
        },
        { 
            maxRetries: 1,
            delay: 200
        }
    ).catch(error => {
        errorHandler.log(error, { 
            operation: 'show-todo-modal',
            component: 'todos',
            userAction: 'click add task button'
        });
        
        // Detailed failure notification
        showErrorToast(
            `Task composer failed: ${error.message}`,
            'Modal Error',
            {
                details: `
                    <div class="small text-muted mt-2">
                        <div class="mb-2">⚠️ Unable to open task composer</div>
                        <div class="d-flex justify-content-between mb-1">
                            <span>Error Type:</span><span class="fw-bold text-danger">UI Component</span>
                        </div>
                        <div class="d-flex justify-content-between mb-2">
                            <span>Bootstrap:</span><span class="fw-bold">${typeof bootstrap !== 'undefined' ? 'Loaded' : 'Missing'}</span>
                        </div>
                        <div class="mt-2">
                            <button class="btn btn-sm btn-outline-light" onclick="showView('todos')">
                                ✅ Go to Tasks Section
                            </button>
                        </div>
                    </div>
                `
            }
        );
        
        // Fallback: try to navigate to todos section
        try {
            showView('todos');
            showInfoToast(
                'Navigated to tasks section instead.',
                'Fallback Navigation',
                {
                    details: `
                        <div class="small text-muted mt-2">
                            <div class="mb-2">🔄 Alternative navigation used</div>
                            <div class="d-flex justify-content-between">
                                <span>Section:</span><span class="fw-bold">Task Management</span>
                            </div>
                        </div>
                    `
                }
            );
        } catch (fallbackError) {
            errorHandler.log(fallbackError, { 
                operation: 'show-todo-modal-fallback',
                component: 'todos'
            });
        }
    });
}

async function saveTodo() {
    const title = document.getElementById('todo-title').value.trim();
    const description = document.getElementById('todo-description').value.trim();
    const dueDate = document.getElementById('todo-due').value;
    const priority = document.getElementById('todo-priority').value;
    
    if (!title) {
        showToast('Please enter a task title', 'error');
        return;
    }
    
    const newTodo = {
        id: Date.now().toString(),
        title,
        description,
        dueAt: dueDate ? new Date(dueDate) : null,
        priority,
        isCompleted: false,
        createdAt: new Date()
    };
    
    appState.todos.unshift(newTodo);
    
    const modalElement = document.getElementById('todoModal');
    if (modalElement) {
        const modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) {
            modal.hide();
        }
    }
    
    // Clear form
    const formElement = document.getElementById('todoForm');
    if (formElement) {
        formElement.reset();
    }
    
    renderTodos();
    updateTodoStatistics();
    showToast('Task added', 'success', title, 2000);
}

function toggleTodo(id) {
    const todo = appState.todos.find(t => t.id === id);
    if (todo) {
        todo.isCompleted = !todo.isCompleted;
        updateTodoStatistics();
        renderTodos();
        const status = todo.isCompleted ? 'completed' : 'reopened';
        showActionToast(`Task ${status}`, todo.title, todo.isCompleted);
    } else {
        showToast('Task not found', 'error');
    }
}

function editTodo(id) {
    // Implementation for editing todo
    showToast('Edit functionality coming soon', 'info');
}

function deleteTodo(id) {
    const todo = appState.todos.find(t => t.id === id);
    if (confirm('Are you sure you want to delete this task?')) {
        appState.todos = appState.todos.filter(t => t.id !== id);
        updateTodoStatistics();
        renderTodos();
        showActionToast('Task deleted', todo?.title || 'Task', true);
    }
}

// Test Functions
function loadTestConfiguration() {
    showToast('Test configuration loaded', 'info');
}

async function runAllTests() {
    return errorHandler.executeWithRetry(
        async () => {
            const progressBar = document.getElementById('test-progress');
            const statusDiv = document.getElementById('test-status');
            const resultsContainer = document.getElementById('test-results-container');
            
            // Validate required DOM elements
            if (!progressBar || !statusDiv || !resultsContainer) {
                throw new Error('Required test elements not found in DOM');
            }
            
            // Get selected services with validation
            const serviceChecks = [
                { id: 'test-anthropic', name: 'Anthropic' },
                { id: 'test-openai', name: 'OpenAI' },
                { id: 'test-google', name: 'Google' },
                { id: 'test-huggingface', name: 'Hugging Face' }
            ];
            
            const selectedServices = [];
            const missingElements = [];
            
            for (const service of serviceChecks) {
                const element = document.getElementById(service.id);
                if (!element) {
                    missingElements.push(service.id);
                    continue;
                }
                if (element.checked) {
                    selectedServices.push(service.name);
                }
            }
            
            if (missingElements.length > 0) {
                errorHandler.log(new Error(`Missing service checkboxes: ${missingElements.join(', ')}`), {
                    operation: 'run-tests-validation',
                    component: 'tests',
                    missingElements
                });
            }
            
            if (selectedServices.length === 0) {
                showWarningToast('Please select at least one service to test', 'No Services Selected');
                return { success: false, reason: 'no_services_selected' };
            }
            
            const loadingToastId = showLoadingToast(`Running tests for ${selectedServices.length} services...`);
            
            try {
                // Show progress
                statusDiv.textContent = 'Running tests...';
                resultsContainer.innerHTML = '<div class="text-center"><i class="bi bi-gear fs-1 spin"></i><p>Running tests...</p></div>';
                
                // Simulate test progress with validation
                const maxProgress = 100;
                const progressStep = 10;
                
                for (let progress = 0; progress <= maxProgress; progress += progressStep) {
                    progressBar.style.width = progress + '%';
                    progressBar.textContent = progress + '%';
                    
                    // Validate progress bar still exists
                    if (!progressBar || !progressBar.style) {
                        throw new Error('Progress bar element lost during execution');
                    }
                    
                    await new Promise(resolve => setTimeout(resolve, 200));
                }
                
                // Generate test results with enterprise-grade validation
                const testResults = selectedServices.map(service => {
                    const baseResult = {
                        service,
                        timestamp: new Date().toISOString(),
                        testId: 'test_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9)
                    };
                    
                    // Simulate test with 80% success rate
                    const isSuccess = Math.random() > 0.2;
                    
                    return {
                        ...baseResult,
                        status: isSuccess ? 'Passed' : 'Failed',
                        duration: Math.floor(Math.random() * 5000) + 1000,
                        details: {
                            requests: Math.floor(Math.random() * 10) + 1,
                            errors: isSuccess ? 0 : Math.floor(Math.random() * 3) + 1,
                            latency: Math.floor(Math.random() * 1000) + 100,
                            throughput: Math.floor(Math.random() * 1000) + 500,
                            successRate: isSuccess ? Math.floor(Math.random() * 20) + 80 : Math.floor(Math.random() * 30) + 40
                        }
                    };
                });
                
                // Validate results
                if (testResults.length !== selectedServices.length) {
                    throw new Error('Test results count mismatch');
                }
                
                appState.testResults = testResults;
                renderTestResults();
                
                statusDiv.textContent = `Tests completed for ${selectedServices.length} services`;
                hideToast(loadingToastId);
                
                const passedCount = testResults.filter(r => r.status === 'Passed').length;
                const failedCount = testResults.filter(r => r.status === 'Failed').length;
                const avgDuration = (testResults.reduce((sum, r) => sum + r.duration, 0) / testResults.length / 1000).toFixed(1);
                
                showSuccessToast(
                    `All tests completed successfully in ${avgDuration}s`,
                    'Tests Complete',
                    {
                        details: `
                            <div class="small text-muted mt-2">
                                <div class="mb-2">🧪 Test execution finished</div>
                                <div class="d-flex justify-content-between mb-1">
                                    <span>✅ Passed:</span><span class="fw-bold text-success">${passedCount}</span>
                                </div>
                                <div class="d-flex justify-content-between mb-1">
                                    <span>❌ Failed:</span><span class="fw-bold text-danger">${failedCount}</span>
                                </div>
                                <div class="d-flex justify-content-between mb-1">
                                    <span>⏱️ Duration:</span><span class="fw-bold">${avgDuration}s</span>
                                </div>
                                <div class="d-flex justify-content-between">
                                    <span>📊 Success Rate:</span><span class="fw-bold">${Math.round((passedCount / testResults.length) * 100)}%</span>
                                </div>
                            </div>
                        `
                    }
                );
                
                return { 
                    success: true, 
                    results: testResults,
                    summary: {
                        total: testResults.length,
                        passed: testResults.filter(r => r.status === 'Passed').length,
                        failed: testResults.filter(r => r.status === 'Failed').length
                    }
                };
                
            } catch (testError) {
                hideToast(loadingToastId);
                throw testError;
            }
        },
        { 
            operation: 'run-tests',
            component: 'tests'
        },
        { 
            maxRetries: 1,
            delay: 1000,
            backoffMultiplier: 2
        }
    ).catch(error => {
        errorHandler.log(error, { 
            operation: 'run-tests',
            component: 'tests',
            userAction: 'click run tests button'
        });
        
        showErrorToast(
            `Test execution failed: ${error.message}`,
            'Test Error',
            {
                details: `
                    <div class="small text-muted mt-2">
                        <div class="mb-2">⚠️ Test execution interrupted</div>
                        <div class="d-flex justify-content-between mb-1">
                            <span>Services Selected:</span><span class="fw-bold">${selectedServices.length}</span>
                        </div>
                        <div class="d-flex justify-content-between mb-1">
                            <span>Error Type:</span><span class="fw-bold text-danger">${error.name}</span>
                        </div>
                        <div class="d-flex justify-content-between mb-2">
                            <span>Attempt:</span><span class="fw-bold">1/2</span>
                        </div>
                        <div class="mt-2">
                            <button class="btn btn-sm btn-outline-light me-2" onclick="runAllTests()">
                                🔄 Retry Tests
                            </button>
                            <button class="btn btn-sm btn-outline-light" onclick="showView('tests')">
                                📊 View Test Panel
                            </button>
                        </div>
                    </div>
                `
            }
        );
        
        // Reset UI state
        try {
            const statusDiv = document.getElementById('test-status');
            const progressBar = document.getElementById('test-progress');
            const resultsContainer = document.getElementById('test-results-container');
            
            if (statusDiv) statusDiv.textContent = 'Tests failed';
            if (progressBar) {
                progressBar.style.width = '0%';
                progressBar.textContent = '0%';
            }
            if (resultsContainer) {
                resultsContainer.innerHTML = '<div class="text-center text-muted"><i class="bi bi-exclamation-triangle"></i><p>Tests failed to complete</p></div>';
            }
        } catch (resetError) {
            errorHandler.log(resetError, { 
                operation: 'run-tests-reset',
                component: 'tests'
            });
        }
        
        return { success: false, error: error.message };
    });
}

function renderTestResults() {
    try {
        const container = document.getElementById('test-results-container');
        
        const resultsHtml = appState.testResults.map(result => `
        <div class="test-result p-3 border-bottom">
            <div class="d-flex justify-content-between align-items-center">
                <div>
                    <h6 class="mb-1">${result.service}</h6>
                    <div class="d-flex gap-2">
                        <span class="badge bg-${result.status === 'Passed' ? 'success' : 'danger'}">
                            ${result.status}
                        </span>
                        <small class="text-muted">Duration: ${result.duration}ms</small>
                    </div>
                    <div class="mt-2">
                        <small class="text-muted">
                            Requests: ${result.details.requests} | 
                            Errors: ${result.details.errors} | 
                            Latency: ${result.details.latency}ms
                        </small>
                    </div>
                </div>
            </div>
        </div>
    `).join('');
    
        if (container) container.innerHTML = resultsHtml || '<p class="text-muted text-center">No test results</p>';
    } catch (error) {
        showToast('Error rendering test results: ' + error.message, 'error');
    }
}

function exportTestResults() {
    if (appState.testResults.length === 0) {
        showToast('No test results to export', 'warning');
        return;
    }
    
    const results = appState.testResults.map(r => 
        `${r.service}: ${r.status} (${r.duration}ms)`
    ).join('\n');
    
    const blob = new Blob([results], { type: 'text/plain' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `test-results-${new Date().toISOString().split('T')[0]}.txt`;
    a.click();
    URL.revokeObjectURL(url);
    
    showToast('Test results exported', 'success');
}

// Enterprise Error Handling System
class ErrorHandler {
    constructor() {
        this.errorLog = [];
        this.circuitBreakers = new Map();
        this.retryAttempts = new Map();
        this.maxRetries = 3;
        this.circuitBreakerThreshold = 5;
        this.circuitBreakerTimeout = 60000; // 1 minute
    }

    log(error, context, severity = 'error') {
        const errorEntry = {
            timestamp: new Date().toISOString(),
            message: error.message || error,
            stack: error.stack,
            context: context || {},
            severity: severity,
            userId: appState?.currentUser || 'anonymous',
            sessionId: this.getSessionId(),
            userAgent: navigator.userAgent,
            url: window.location.href
        };

        this.errorLog.push(errorEntry);
        console.error(`[${severity.toUpperCase()}] ${context.operation || 'Unknown'}:`, errorEntry);

        // Store in localStorage for debugging
        try {
            const existingLogs = JSON.parse(localStorage.getItem('error-logs') || '[]');
            existingLogs.push(errorEntry);
            // Keep only last 100 errors
            if (existingLogs.length > 100) {
                existingLogs.splice(0, existingLogs.length - 100);
            }
            localStorage.setItem('error-logs', JSON.stringify(existingLogs));
        } catch (logError) {
            console.warn('Failed to store error log:', logError);
        }

        return errorEntry;
    }

    getSessionId() {
        let sessionId = sessionStorage.getItem('session-id');
        if (!sessionId) {
            sessionId = 'session_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9);
            sessionStorage.setItem('session-id', sessionId);
        }
        return sessionId;
    }

    async executeWithRetry(operation, context, options = {}) {
        const operationKey = `${context.operation}_${context.component || 'unknown'}`;
        const maxRetries = options.maxRetries || this.maxRetries;
        const delay = options.delay || 1000;
        const backoffMultiplier = options.backoffMultiplier || 2;

        // Check circuit breaker
        if (this.isCircuitBreakerOpen(operationKey)) {
            throw new Error(`Circuit breaker is open for ${operationKey}`);
        }

        let lastError;
        for (let attempt = 1; attempt <= maxRetries; attempt++) {
            try {
                const result = await operation();
                // Reset circuit breaker on success
                this.resetCircuitBreaker(operationKey);
                return result;
            } catch (error) {
                lastError = error;
                this.log(error, { ...context, attempt, maxRetries }, 'warning');
                
                if (attempt < maxRetries) {
                    const waitTime = delay * Math.pow(backoffMultiplier, attempt - 1);
                    await this.delay(waitTime);
                }
            }
        }

        // Circuit breaker triggered
        this.incrementCircuitBreaker(operationKey);
        throw lastError;
    }

    isCircuitBreakerOpen(operationKey) {
        const breaker = this.circuitBreakers.get(operationKey);
        if (!breaker) return false;

        if (breaker.state === 'open') {
            if (Date.now() - breaker.lastFailure > this.circuitBreakerTimeout) {
                breaker.state = 'half-open';
                return false;
            }
            return true;
        }
        return false;
    }

    incrementCircuitBreaker(operationKey) {
        const breaker = this.circuitBreakers.get(operationKey) || {
            failures: 0,
            state: 'closed',
            lastFailure: 0
        };

        breaker.failures++;
        breaker.lastFailure = Date.now();

        if (breaker.failures >= this.circuitBreakerThreshold) {
            breaker.state = 'open';
            this.log(new Error(`Circuit breaker opened for ${operationKey}`), 
                    { operation: 'circuit-breaker', component: operationKey }, 'warning');
        }

        this.circuitBreakers.set(operationKey, breaker);
    }

    resetCircuitBreaker(operationKey) {
        this.circuitBreakers.delete(operationKey);
    }

    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    getErrorSummary() {
        const recent = this.errorLog.slice(-50);
        const bySeverity = recent.reduce((acc, error) => {
            acc[error.severity] = (acc[error.severity] || 0) + 1;
            return acc;
        }, {});

        return {
            total: this.errorLog.length,
            recent: recent.length,
            bySeverity,
            lastError: this.errorLog[this.errorLog.length - 1]
        };
    }

    clearLogs() {
        this.errorLog = [];
        localStorage.removeItem('error-logs');
    }
}

// Global error handler instance
const errorHandler = new ErrorHandler();

// Global error event listeners
window.addEventListener('error', (event) => {
    errorHandler.log(event.error, {
        operation: 'global-error',
        filename: event.filename,
        lineno: event.lineno,
        colno: event.colno
    });
});

window.addEventListener('unhandledrejection', (event) => {
    errorHandler.log(event.reason, {
        operation: 'unhandled-promise-rejection'
    });
});

// Enhanced toast with enterprise features
function showToast(message, type = 'info', options = {}) {
    try {
        const toastContainer = document.getElementById('toast-container');
        if (!toastContainer) {
            console.warn('Toast container not found');
            return;
        }

        const toastId = 'toast-' + Date.now() + '-' + Math.random().toString(36).substr(2, 9);
        const toastElement = document.createElement('div');
        toastElement.id = toastId;
        toastElement.className = 'toast align-items-center text-white bg-' + type + ' border-0';
        toastElement.setAttribute('role', 'alert');
        toastElement.setAttribute('aria-live', 'assertive');
        toastElement.setAttribute('aria-atomic', 'true');

        const toastContent = `
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                    ${options.details ? `<div class="mt-2">${options.details}</div>` : ''}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        `;

        toastElement.innerHTML = toastContent;
        toastContainer.appendChild(toastElement);

        const toast = new bootstrap.Toast(toastElement, {
            autohide: options.autohide !== false,
            delay: options.duration || 5000
        });

        toast.show();

        toastElement.addEventListener('hidden.bs.toast', () => {
            toastElement.remove();
        });

        return toastId;
    } catch (error) {
        errorHandler.log(error, { operation: 'show-toast', message, type });
        // Fallback to console
        console.log(`[${type.toUpperCase()}] ${message}`);
    }
}

function showLoadingToast(message) {
    return showToast(message, 'info', { autohide: false, duration: 0 });
}

function hideToast(toastId) {
    try {
        const toastElement = document.getElementById(toastId);
        if (toastElement) {
            const toast = bootstrap.Toast.getInstance(toastElement);
            if (toast) {
                toast.hide();
            }
        }
    } catch (error) {
        errorHandler.log(error, { operation: 'hide-toast', toastId });
    }
}

function showSuccessToast(message, title, options = {}) {
    if (title) message = `<strong>${title}</strong><br>${message}`;
    return showToast(message, 'success', options);
}

function showErrorToast(message, title, options = {}) {
    if (title) message = `<strong>${title}</strong><br>${message}`;
    return showToast(message, 'danger', options);
}

function showWarningToast(message, title, options = {}) {
    if (title) message = `<strong>${title}</strong><br>${message}`;
    return showToast(message, 'warning', options);
}

function showInfoToast(message, title, options = {}) {
    if (title) message = `<strong>${title}</strong><br>${message}`;
    return showToast(message, 'info', options);
}

// Add CSS for spinning animation
const style = document.createElement('style');
style.textContent = `
    .spin {
        animation: spin 1s linear infinite;
    }
    @keyframes spin {
        from { transform: rotate(0deg); }
        to { transform: rotate(360deg); }
    }
`;
document.head.appendChild(style);

// Secrets Management
class SecretsManager {
    constructor() {
        this.storageKey = 'bot-management-secrets';
        this.initializeEventListeners();
        this.migratePlainSecrets(); // Migrate any existing plain text secrets
        // Don't call loadSecrets here - let it be called when modal is shown
    }

    // Migrate existing plain text secrets to encrypted format
    migratePlainSecrets() {
        try {
            const plainSecrets = localStorage.getItem(this.storageKey);
            if (plainSecrets) {
                // Check if it's already encrypted (base64 encoded)
                try {
                    atob(plainSecrets); // Test if it's base64
                    // If no error, it's likely already encrypted
                    return;
                } catch (e) {
                    // Not base64, so it's plain text - migrate it
                    const secrets = JSON.parse(plainSecrets);
                    const secretsJson = JSON.stringify(secrets);
                    const encryptedSecrets = this.encrypt(secretsJson);
                    localStorage.setItem(this.storageKey, encryptedSecrets);
                    
                    if (window.showToast) {
                        window.showToast('Secrets have been encrypted for security', 'info');
                    }
                }
            }
        } catch (error) {
            console.error('Failed to migrate secrets:', error);
        }
    }

    // Simple encryption using XOR with a derived key
    encrypt(text) {
        if (!text) return text;
        
        // Derive key from browser fingerprint (not super secure but better than plain text)
        const key = this.deriveKey();
        let result = '';
        
        for (let i = 0; i < text.length; i++) {
            result += String.fromCharCode(text.charCodeAt(i) ^ key.charCodeAt(i % key.length));
        }
        
        // Convert to base64 for storage
        return btoa(result);
    }

    // Simple decryption using XOR with the same derived key
    decrypt(encryptedText) {
        if (!encryptedText) return encryptedText;
        
        try {
            // Convert from base64
            const text = atob(encryptedText);
            const key = this.deriveKey();
            let result = '';
            
            for (let i = 0; i < text.length; i++) {
                result += String.fromCharCode(text.charCodeAt(i) ^ key.charCodeAt(i % key.length));
            }
            
            return result;
        } catch (error) {
            console.error('Failed to decrypt secrets:', error);
            return '{}'; // Return empty object if decryption fails
        }
    }

    // Derive a key from browser characteristics
    deriveKey() {
        const fingerprint = [
            navigator.userAgent,
            navigator.language,
            screen.width + 'x' + screen.height,
            new Date().getTimezoneOffset(),
            localStorage.length
        ].join('|');
        
        // Create a simple hash
        let hash = 0;
        for (let i = 0; i < fingerprint.length; i++) {
            const char = fingerprint.charCodeAt(i);
            hash = ((hash << 5) - hash) + char;
            hash = hash & hash; // Convert to 32-bit integer
        }
        
        // Convert to a string key
        return Math.abs(hash).toString(16).padEnd(32, '0').substring(0, 32);
    }

    // Encrypt individual secret values
    encryptSecrets(secrets) {
        const encrypted = {};
        
        for (const [key, value] of Object.entries(secrets)) {
            if (typeof value === 'object' && value !== null) {
                encrypted[key] = this.encryptSecrets(value);
            } else if (typeof value === 'string') {
                encrypted[key] = this.encrypt(value);
            } else {
                encrypted[key] = value;
            }
        }
        
        return encrypted;
    }

    // Decrypt individual secret values
    decryptSecrets(encryptedSecrets) {
        try {
            const decrypted = {};
            
            for (const [key, value] of Object.entries(encryptedSecrets)) {
                if (typeof value === 'object' && value !== null) {
                    decrypted[key] = this.decryptSecrets(value);
                } else if (typeof value === 'string') {
                    decrypted[key] = this.decrypt(value);
                } else {
                    decrypted[key] = value;
                }
            }
            
            return decrypted;
        } catch (error) {
            console.error('Failed to decrypt secrets:', error);
            return {}; // Return empty object if decryption fails
        }
    }

    initializeEventListeners() {
        // Only initialize if DOM elements are available
        if (!document.getElementById('secretsModal')) {
            console.warn('Secrets modal not found, skipping event listener initialization');
            return;
        }

        // Save secrets button
        const saveBtn = document.getElementById('save-secrets-btn');
        if (saveBtn) {
            saveBtn.addEventListener('click', () => {
                this.saveSecrets();
            });
        }

        // Toggle password visibility buttons
        const toggleButtons = document.querySelectorAll('[id^="toggle-"]');
        toggleButtons.forEach(button => {
            button.addEventListener('click', (e) => {
                const targetId = e.currentTarget.id.replace('toggle-', '');
                this.togglePasswordVisibility(targetId);
            });
        });

        // Load secrets when modal is shown
        const secretsModal = document.getElementById('secretsModal');
        if (secretsModal) {
            secretsModal.addEventListener('show.bs.modal', () => {
                this.loadSecrets();
            });
        }
    }

    togglePasswordVisibility(fieldId) {
        const input = document.getElementById(fieldId);
        const button = document.getElementById(`toggle-${fieldId}`);
        
        if (!input || !button) {
            console.warn(`Toggle password visibility failed: input or button not found for field ${fieldId}`);
            return;
        }
        
        const icon = button.querySelector('i');
        if (!icon) {
            console.warn(`Toggle password visibility failed: icon not found in button for field ${fieldId}`);
            return;
        }

        if (input.type === 'password') {
            input.type = 'text';
            icon.classList.remove('bi-eye');
            icon.classList.add('bi-eye-slash');
        } else {
            input.type = 'password';
            icon.classList.remove('bi-eye-slash');
            icon.classList.add('bi-eye');
        }
    }

    getSecrets() {
        try {
            const encryptedSecrets = localStorage.getItem(this.storageKey);
            if (!encryptedSecrets) return {};

            const decryptedJson = this.decrypt(encryptedSecrets);
            
            // Validate that decrypted data is valid JSON string
            if (!decryptedJson || typeof decryptedJson !== 'string') {
                console.error('Decrypted secrets is not a valid string');
                return {};
            }
            
            // Try to parse JSON, with better error handling
            try {
                // Clean up common JSON formatting issues
                let cleanedJson = decryptedJson
                    .replace(/[\u0000-\u001F\u007F-\u009F]/g, '') // Remove control characters
                    .replace(/,\s*}/g, '}') // Remove trailing commas
                    .replace(/,\s*]/g, ']') // Remove trailing commas in arrays
                    .replace(/([{,]\s*)([a-zA-Z_][a-zA-Z0-9_]*)\s*:/g, '$1"$2":'); // Quote unquoted property names
                
                return JSON.parse(cleanedJson);
            } catch (parseError) {
                console.error('Failed to parse decrypted secrets as JSON:', parseError);
                console.error('Decrypted data:', decryptedJson);

                // If parsing fails, remove corrupted data from localStorage
                localStorage.removeItem(this.storageKey);
                return {};
            }
        } catch (error) {
            console.error('Failed to load secrets:', error);
            return {};
        }
    }

    saveSecrets() {
        // Check if required DOM elements exist
        if (!document.getElementById('protonmail-client-id') || 
            !document.getElementById('protonmail-client-secret')) {
            console.error('Secrets form elements not found');
            if (window.showToast) {
                window.showToast('Secrets form not available', 'error');
            }
            return;
        }

        const secrets = {
            protonmail: {
                clientId: document.getElementById('protonmail-client-id').value,
                clientSecret: document.getElementById('protonmail-client-secret').value
            },
            semanticKernel: {
                defaultProvider: document.getElementById('sk-default-provider').value,
                providers: {
                    openai: {
                        apiKey: document.getElementById('openai-api-key').value,
                        modelId: document.getElementById('openai-model-id').value
                    },
                    anthropic: {
                        apiKey: document.getElementById('anthropic-api-key').value,
                        modelId: document.getElementById('anthropic-model-id').value
                    },
                    google: {
                        apiKey: document.getElementById('google-api-key').value,
                        modelId: document.getElementById('google-model-id').value
                    },
                    huggingface: {
                        apiKey: document.getElementById('huggingface-api-key').value,
                        modelId: document.getElementById('huggingface-model-id').value
                    },
                    ollama: {
                        endpoint: document.getElementById('ollama-endpoint').value,
                        modelId: document.getElementById('ollama-model-id').value
                    }
                }
            },
            database: {
                connectionString: document.getElementById('db-connection-string').value
            }
        };

        try {
            // Encrypt the secrets before storing
            const secretsJson = JSON.stringify(secrets);
            const encryptedSecrets = this.encrypt(secretsJson);
            localStorage.setItem(this.storageKey, encryptedSecrets);
            
            // Show success message
            if (window.showToast) {
                window.showToast('Secrets saved and encrypted successfully!', 'success');
            }
        } catch (error) {
            console.error('Failed to save secrets:', error);
            if (window.showToast) {
                window.showToast('Failed to save secrets', 'error');
            }
            return;
        }
        
        // Close modal
        const modalElement = document.getElementById('secretsModal');
        if (modalElement) {
            const modal = bootstrap.Modal.getInstance(modalElement);
            if (modal) {
                modal.hide();
            }
        }
    }

    loadSecrets() {
        const secrets = this.getSecrets();
        
        // Check if form elements exist before trying to populate them
        if (!document.getElementById('protonmail-client-id')) {
            console.warn('Secrets form elements not found, skipping load');
            return;
        }
        
        // Load ProtonMail settings
        if (secrets.protonmail) {
            document.getElementById('protonmail-client-id').value = secrets.protonmail.clientId || '';
            document.getElementById('protonmail-client-secret').value = secrets.protonmail.clientSecret || '';
        }

        // Load Semantic Kernel settings
        if (secrets.semanticKernel) {
            const defaultProviderElement = document.getElementById('sk-default-provider');
            if (defaultProviderElement) {
                defaultProviderElement.value = secrets.semanticKernel.defaultProvider || 'OpenAI';
            }
            
            if (secrets.semanticKernel.providers) {
                const providers = secrets.semanticKernel.providers;
                
                // OpenAI
                if (providers.openai) {
                    const openaiKeyElement = document.getElementById('openai-api-key');
                    const openaiModelElement = document.getElementById('openai-model-id');
                    if (openaiKeyElement) openaiKeyElement.value = providers.openai.apiKey || '';
                    if (openaiModelElement) openaiModelElement.value = providers.openai.modelId || 'gpt-4';
                }
                
                // Anthropic
                if (providers.anthropic) {
                    const anthropicKeyElement = document.getElementById('anthropic-api-key');
                    const anthropicModelElement = document.getElementById('anthropic-model-id');
                    if (anthropicKeyElement) anthropicKeyElement.value = providers.anthropic.apiKey || '';
                    if (anthropicModelElement) anthropicModelElement.value = providers.anthropic.modelId || 'claude-3-sonnet';
                }
                
                // Google
                if (providers.google) {
                    const googleKeyElement = document.getElementById('google-api-key');
                    const googleModelElement = document.getElementById('google-model-id');
                    if (googleKeyElement) googleKeyElement.value = providers.google.apiKey || '';
                    if (googleModelElement) googleModelElement.value = providers.google.modelId || 'gemini-pro';
                }
                
                // HuggingFace
                if (providers.huggingface) {
                    const huggingfaceKeyElement = document.getElementById('huggingface-api-key');
                    const huggingfaceModelElement = document.getElementById('huggingface-model-id');
                    if (huggingfaceKeyElement) huggingfaceKeyElement.value = providers.huggingface.apiKey || '';
                    if (huggingfaceModelElement) huggingfaceModelElement.value = providers.huggingface.modelId || '';
                }
                
                // Ollama
                if (providers.ollama) {
                    const ollamaEndpointElement = document.getElementById('ollama-endpoint');
                    const ollamaModelElement = document.getElementById('ollama-model-id');
                    if (ollamaEndpointElement) ollamaEndpointElement.value = providers.ollama.endpoint || 'http://localhost:11434';
                    if (ollamaModelElement) ollamaModelElement.value = providers.ollama.modelId || '';
                }
            }
        }

        // Load Database settings
        if (secrets.database) {
            const dbElement = document.getElementById('db-connection-string');
            if (dbElement) dbElement.value = secrets.database.connectionString || '';
        }
    }

    getSecret(key) {
        const secrets = this.getSecrets();
        return this.getNestedValue(secrets, key);
    }

    getNestedValue(obj, path) {
        return path.split('.').reduce((current, key) => current && current[key], obj);
    }
}

// Initialize Secrets Manager - defer until DOM is ready
let secretsManager = null;

function initializeSecretsManager() {
    if (document.getElementById('secretsModal')) {
        secretsManager = new SecretsManager();
    }
}

// Helper function to get secrets for API calls
function getSecrets() {
    return secretsManager ? secretsManager.getSecrets() : {};
}

// Update latest coverage report links
function updateCoverageLinks() {
    // This would typically be updated by the backend after builds
    // For now, we'll use the latest known coverage report
    const latestCoveragePath = '../TestResults/2011d213-c5ab-42bd-9807-eec5933a7bc9/coverage.cobertura.xml';
    
    // Update dashboard test results link
    const dashboardLink = document.getElementById('view-latest-coverage-btn');
    if (dashboardLink) {
        dashboardLink.href = latestCoveragePath;
    }
    
    // Update test runner link
    const runnerLink = document.getElementById('view-latest-coverage-runner-btn');
    if (runnerLink) {
        runnerLink.href = latestCoveragePath;
    }
}

// Initialize coverage links on page load
document.addEventListener('DOMContentLoaded', () => {
    updateCoverageLinks();
});

// Function to fetch latest test results from backend
async function fetchLatestTestResults() {
    try {
        // This would be an API call to get the latest test results
        // const response = await fetch('/api/test-results/latest');
        // const data = await response.json();
        // updateCoverageLinks(data.latestCoveragePath);
        
        // For now, just update with known latest
        updateCoverageLinks();
    } catch (error) {
        console.error('Failed to fetch latest test results:', error);
    }
}

// Add periodic check for new test results (every 30 seconds)
setInterval(fetchLatestTestResults, 30000);
