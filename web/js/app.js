// Bot Management System - Main Application

// Mock data - In a real app, this would come from an API
let bots = [
    { 
        id: 1, 
        name: 'Support Bot', 
        type: 'Chat', 
        description: 'Handles customer support queries', 
        isActive: true, 
        lastActive: new Date('2023-09-14T10:30:00'),
        performance: '98%',
        timeRunning: '2d 4h 15m',
        startTime: new Date(Date.now() - (2 * 24 * 60 * 60 * 1000) - (4 * 60 * 60 * 1000) - (15 * 60 * 1000))
    },
    { 
        id: 2, 
        name: 'Data Scraper', 
        type: 'Automation', 
        description: 'Scrapes data from various sources', 
        isActive: false, 
        lastActive: new Date('2023-09-13T15:45:00'),
        performance: '87%',
        timeRunning: '1d 7h 30m',
        startTime: new Date(Date.now() - (1 * 24 * 60 * 60 * 1000) - (7 * 60 * 60 * 1000) - (30 * 60 * 1000))
    },
    { 
        id: 3, 
        name: 'Analytics Bot', 
        type: 'Analytics', 
        description: 'Processes and analyzes data', 
        isActive: true, 
        lastActive: new Date('2023-09-15T09:15:00'),
        performance: '92%',
        timeRunning: '3d 12h 45m',
        startTime: new Date(Date.now() - (3 * 24 * 60 * 60 * 1000) - (12 * 60 * 60 * 1000) - (45 * 60 * 1000))
    },
    { 
        id: 4, 
        name: 'Monitoring Bot', 
        type: 'Monitoring', 
        description: 'Monitors system health', 
        isActive: true, 
        lastActive: new Date('2023-09-15T08:20:00'),
        performance: '99.9%',
        timeRunning: '5d 2h 10m',
        startTime: new Date(Date.now() - (5 * 24 * 60 * 60 * 1000) - (2 * 60 * 60 * 1000) - (10 * 60 * 1000))
    },
];

// DOM Elements
const mainContent = document.getElementById('main-content');
const dashboardView = document.getElementById('dashboard-view');
const botsView = document.getElementById('bots-view');
const statisticsView = document.getElementById('statistics-view');
const botsTable = document.getElementById('bots-table').querySelector('tbody');
const recentActivityTable = document.getElementById('recent-activity').querySelector('tbody');
const activityTimeline = document.getElementById('activity-timeline');
const botModal = new bootstrap.Modal(document.getElementById('botModal'));
const confirmModal = new bootstrap.Modal(document.getElementById('confirmModal'));
const botForm = document.getElementById('botForm');

// Chart instances
let statusChart, typeChart;

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    // Initialize UI
    updateDashboard();
    updateBotsTable();
    updateActivityTimeline();
    initializeCharts();
    
    // Navigation event listeners
    document.getElementById('nav-dashboard').addEventListener('click', (e) => {
        e.preventDefault();
        showView('dashboard');
    });
    
    document.getElementById('nav-bots').addEventListener('click', (e) => {
        e.preventDefault();
        showView('bots');
    });
    
    document.getElementById('nav-statistics').addEventListener('click', (e) => {
        e.preventDefault();
        showView('statistics');
    });
    
    // Button event listeners
    document.getElementById('add-bot-btn').addEventListener('click', () => showBotModal());
    document.getElementById('add-bot-btn-2').addEventListener('click', () => showBotModal());
    document.getElementById('saveBot').addEventListener('click', saveBot);
    
    // Form submission
    botForm.addEventListener('submit', (e) => {
        e.preventDefault();
        saveBot();
    });
    
    // Initialize with dashboard view
    showView('dashboard');
});

// Show the specified view
function showView(view) {
    // Hide all views
    dashboardView.style.display = 'none';
    botsView.style.display = 'none';
    statisticsView.style.display = 'none';
    
    // Show the selected view
    switch(view) {
        case 'dashboard':
            dashboardView.style.display = 'block';
            updateDashboard();
            break;
        case 'bots':
            botsView.style.display = 'block';
            updateBotsTable();
            break;
        case 'statistics':
            statisticsView.style.display = 'block';
            updateCharts();
            updateActivityTimeline();
            break;
    }
    
    // Update active nav link
    document.querySelectorAll('.nav-link').forEach(link => link.classList.remove('active'));
    document.getElementById(`nav-${view}`).classList.add('active');
}

// Update dashboard with current data
function updateDashboard() {
    const activeBots = bots.filter(bot => bot.isActive).length;
    const inactiveBots = bots.length - activeBots;
    
    document.getElementById('total-bots').textContent = bots.length;
    document.getElementById('active-bots').textContent = activeBots;
    document.getElementById('inactive-bots').textContent = inactiveBots;
    
    // Update recent activity
    updateRecentActivity();
}

// Update the bots table
function updateBotsTable() {
    botsTable.innerHTML = '';
    
    if (bots.length === 0) {
        const row = document.createElement('tr');
        row.innerHTML = `
            <td colspan="5" class="text-center py-4">
                <div class="empty-state">
                    <i class="bi bi-robot"></i>
                    <h5>No bots found</h5>
                    <p>Click the "Add Bot" button to create your first bot.</p>
                </div>
            </td>
        `;
        botsTable.appendChild(row);
        return;
    }
    
    // Sort by last active (newest first)
    const sortedBots = [...bots].sort((a, b) => 
        new Date(b.lastActive || 0) - new Date(a.lastActive || 0)
    );
    
    sortedBots.forEach(bot => {
        const row = document.createElement('tr');
        const statusClass = bot.isActive ? 'status-active' : 'status-inactive';
        const statusText = bot.isActive ? 'Active' : 'Inactive';
        const lastActive = bot.lastActive 
            ? new Date(bot.lastActive).toLocaleString() 
            : 'Never';
            
        row.innerHTML = `
            <td>
                <div class="d-flex align-items-center">
                    <span class="badge bg-secondary me-2">#${bot.id}</span>
                    <span>${bot.name}</span>
                </div>
            </td>
            <td>${bot.type}</td>
            <td><span class="${statusClass}">${statusText}</span></td>
            <td>${bot.performance || 'N/A'}</td>
            <td>${bot.timeRunning || 'N/A'}</td>
            <td>${lastActive}</td>
            <td>
                <button class="btn btn-sm btn-outline-primary me-1" onclick="editBot(${bot.id})" title="Edit Bot">
                    <i class="bi bi-pencil"></i>
                </button>
                <button class="btn btn-sm btn-outline-danger me-1" onclick="confirmDeleteBot(${bot.id})" title="Delete Bot">
                    <i class="bi bi-trash"></i>
                </button>
                <button class="btn btn-sm ${bot.isActive ? 'btn-outline-warning' : 'btn-outline-success'}" 
                        onclick="toggleBotStatus(${bot.id})"
                        title="${bot.isActive ? 'Pause Bot' : 'Start Bot'}">
                    <i class="bi ${bot.isActive ? 'bi-pause' : 'bi-play'}"></i>
                </button>
            </td>
        `;
        
        botsTable.appendChild(row);
    });
}

// Update recent activity table
function updateRecentActivity() {
    recentActivityTable.innerHTML = '';
    
    // Sort by last active (newest first) and limit to 5
    const recentBots = [...bots]
        .sort((a, b) => new Date(b.lastActive || 0) - new Date(a.lastActive || 0))
        .slice(0, 5);
    
    if (recentBots.length === 0) {
        const row = document.createElement('tr');
        row.innerHTML = '<td colspan="5" class="text-center py-4">No recent activity</td>';
        recentActivityTable.appendChild(row);
        return;
    }
    
    recentBots.forEach(bot => {
        const row = document.createElement('tr');
        const statusClass = bot.isActive ? 'status-active' : 'status-inactive';
        const statusText = bot.isActive ? 'Active' : 'Inactive';
        const lastActive = bot.lastActive 
            ? new Date(bot.lastActive).toLocaleString() 
            : 'Never';
            
        row.innerHTML = `
            <td>
                <div class="d-flex align-items-center">
                    <span class="badge bg-secondary me-2">#${bot.id}</span>
                    <span>${bot.name}</span>
                </div>
            </td>
            <td>${bot.type}</td>
            <td><span class="${statusClass}">${statusText}</span></td>
            <td>${bot.performance || 'N/A'}</td>
            <td>${bot.timeRunning || 'N/A'}</td>
            <td>${lastActive}</td>
            <td>
                <button class="btn btn-sm btn-outline-primary" onclick="viewBotDetails(${bot.id})" title="View Details">
                    <i class="bi bi-eye"></i> View
                </button>
            </td>
        `;
        
        recentActivityTable.appendChild(row);
    });
}

// Update activity timeline
function updateActivityTimeline() {
    activityTimeline.innerHTML = '';
    
    // Sort by last active (newest first) and limit to 10
    const recentActivity = [...bots]
        .filter(bot => bot.lastActive)
        .sort((a, b) => new Date(b.lastActive) - new Date(a.lastActive))
        .slice(0, 10);
    
    if (recentActivity.length === 0) {
        activityTimeline.innerHTML = `
            <div class="text-center py-4 text-muted">
                <i class="bi bi-activity"></i>
                <p class="mt-2">No recent activity</p>
            </div>
        `;
        return;
    }
    
    recentActivity.forEach(bot => {
        const activityItem = document.createElement('div');
        activityItem.className = 'timeline-item';
        
        const timeAgo = formatTimeAgo(bot.lastActive);
        const statusText = bot.isActive ? 'was active' : 'was inactive';
        
        activityItem.innerHTML = `
            <div class="timeline-time">${timeAgo} ago</div>
            <div class="timeline-content">
                <strong>${bot.name}</strong> ${statusText}
            </div>
        `;
        
        activityTimeline.appendChild(activityItem);
    });
}

// Format time ago
function formatTimeAgo(dateString) {
    const date = new Date(dateString);
    const now = new Date();
    const seconds = Math.floor((now - date) / 1000);
    
    let interval = Math.floor(seconds / 31536000);
    if (interval >= 1) return interval + ' year' + (interval === 1 ? '' : 's');
    
    interval = Math.floor(seconds / 2592000);
    if (interval >= 1) return interval + ' month' + (interval === 1 ? '' : 's');
    
    interval = Math.floor(seconds / 86400);
    if (interval >= 1) return interval + ' day' + (interval === 1 ? '' : 's');
    
    interval = Math.floor(seconds / 3600);
    if (interval >= 1) return interval + ' hour' + (interval === 1 ? '' : 's');
    
    interval = Math.floor(seconds / 60);
    if (interval >= 1) return interval + ' minute' + (interval === 1 ? '' : 's');
    
    return 'a few seconds';
}

// Initialize charts
function initializeCharts() {
    // Status Distribution Chart
    const statusCtx = document.getElementById('statusChart').getContext('2d');
    statusChart = new Chart(statusCtx, {
        type: 'doughnut',
        data: {
            labels: ['Active', 'Inactive'],
            datasets: [{
                data: [0, 0],
                backgroundColor: ['#198754', '#dc3545'],
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                }
            }
        }
    });
    
    // Bot Types Chart
    const typeCtx = document.getElementById('typeChart').getContext('2d');
    typeChart = new Chart(typeCtx, {
        type: 'bar',
        data: {
            labels: [],
            datasets: [{
                label: 'Number of Bots',
                data: [],
                backgroundColor: '#0d6efd',
                borderWidth: 0,
                borderRadius: 4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
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
    
    updateCharts();
}

// Update charts with current data
function updateCharts() {
    // Update status chart
    const activeBots = bots.filter(bot => bot.isActive).length;
    const inactiveBots = bots.length - activeBots;
    
    statusChart.data.datasets[0].data = [activeBots, inactiveBots];
    statusChart.update();
    
    // Update types chart
    const types = {};
    bots.forEach(bot => {
        types[bot.type] = (types[bot.type] || 0) + 1;
    });
    
    typeChart.data.labels = Object.keys(types);
    typeChart.data.datasets[0].data = Object.values(types);
    typeChart.update();
}

// Show bot modal for adding/editing
function showBotModal(botId = null) {
    const modalTitle = document.getElementById('botModalTitle');
    const form = document.getElementById('botForm');
    
    if (botId) {
        // Edit mode
        const bot = bots.find(b => b.id === botId);
        if (!bot) return;
        
        modalTitle.textContent = 'Edit Bot';
        document.getElementById('botId').value = bot.id;
        document.getElementById('botName').value = bot.name;
        document.getElementById('botType').value = bot.type;
        document.getElementById('botDescription').value = bot.description || '';
        document.getElementById('botIsActive').checked = bot.isActive;
    } else {
        // Add mode
        modalTitle.textContent = 'Add New Bot';
        form.reset();
        document.getElementById('botId').value = '';
        document.getElementById('botIsActive').checked = true;
    }
    
    botModal.show();
}

// Save bot (add or update)
function saveBot() {
    const id = document.getElementById('botId').value;
    const name = document.getElementById('botName').value.trim();
    const type = document.getElementById('botType').value;
    const description = document.getElementById('botDescription').value.trim();
    const isActive = document.getElementById('botIsActive').checked;
    
    if (!name) {
        alert('Please enter a bot name');
        return;
    }
    
    if (id) {
        // Update existing bot
        const index = bots.findIndex(b => b.id === parseInt(id));
        if (index !== -1) {
            bots[index] = {
                ...bots[index],
                name,
                type,
                description,
                isActive,
                lastActive: isActive ? new Date() : bots[index].lastActive
            };
            showToast('Bot updated successfully', 'success');
        }
    } else {
        // Add new bot
        const newBot = {
            id: bots.length > 0 ? Math.max(...bots.map(b => b.id)) + 1 : 1,
            name,
            type,
            description,
            isActive,
            lastActive: isActive ? new Date() : null
        };
        bots.push(newBot);
        showToast('Bot added successfully', 'success');
    }
    
    // Close modal and refresh UI
    botModal.hide();
    updateDashboard();
    updateBotsTable();
    updateCharts();
    updateActivityTimeline();
}

// Toggle bot status
function toggleBotStatus(id) {
    const bot = bots.find(b => b.id === id);
    if (!bot) return;
    
    bot.isActive = !bot.isActive;
    if (bot.isActive) {
        bot.lastActive = new Date();
    }
    
    updateDashboard();
    updateBotsTable();
    updateCharts();
    updateActivityTimeline();
    
    showToast(`Bot ${bot.isActive ? 'activated' : 'deactivated'} successfully`, 'success');
}

// Confirm bot deletion
function confirmDeleteBot(id) {
    const bot = bots.find(b => b.id === id);
    if (!bot) return;
    
    document.getElementById('confirmMessage').innerHTML = `
        Are you sure you want to delete <strong>${bot.name}</strong>? This action cannot be undone.
    `;
    
    const confirmBtn = document.getElementById('confirmAction');
    const oldHandler = confirmBtn.onclick;
    
    confirmBtn.onclick = function() {
        deleteBot(id);
        confirmModal.hide();
    };
    
    confirmModal.show();
    
    // Clean up event listener when modal is hidden
    confirmModal._element.addEventListener('hidden.bs.modal', function() {
        confirmBtn.onclick = oldHandler;
    }, { once: true });
}

// Delete bot
function deleteBot(id) {
    const index = bots.findIndex(b => b.id === id);
    if (index === -1) return;
    
    bots.splice(index, 1);
    
    updateDashboard();
    updateBotsTable();
    updateCharts();
    updateActivityTimeline();
    
    showToast('Bot deleted successfully', 'success');
}

// View bot details
function viewBotDetails(id) {
    const bot = bots.find(b => b.id === id);
    if (!bot) return;
    
    // In a real app, you might show a detailed view modal
    showBotModal(id);
}

// Edit bot
function editBot(id) {
    showBotModal(id);
}

// Show toast notification
function showToast(message, type = 'info') {
    // In a real app, you might use a toast library or implement a custom toast
    alert(`${type.toUpperCase()}: ${message}`);
}

// Helper function to format date
function formatDate(dateString) {
    if (!dateString) return 'Never';
    return new Date(dateString).toLocaleString();
}

// Expose functions to global scope for HTML event handlers
window.viewBotDetails = viewBotDetails;
window.editBot = editBot;
window.confirmDeleteBot = confirmDeleteBot;
window.toggleBotStatus = toggleBotStatus;
