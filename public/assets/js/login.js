/**
 * TaskFlow - Login Page JavaScript
 */

document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');
    const submitButton = document.querySelector('.btn-login');
    
    if (loginForm) {
        loginForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            // Get form data
            const email = document.getElementById('email').value.trim();
            const password = document.getElementById('password').value;
            const rememberMe = document.getElementById('rememberMe').checked;
            
            // Basic validation
            if (!email || !password) {
                showAlert('Please fill in all fields', 'error');
                return;
            }
            
            // Email validation
            if (!isValidEmail(email)) {
                showAlert('Please enter a valid email address', 'error');
                return;
            }
            
            // Add loading state
            submitButton.classList.add('loading');
            submitButton.disabled = true;
            
            // Submit form
            const formData = new FormData();
            formData.append('email', email);
            formData.append('password', password);
            formData.append('remember_me', rememberMe ? '1' : '0');
            
            fetch('login_process.php', {
                method: 'POST',
                body: formData
            })
            .then(response => response.json())
            .then(data => {
                submitButton.classList.remove('loading');
                submitButton.disabled = false;
                
                if (data.success) {
                    showAlert(data.message, 'success');
                    // Redirect to dashboard after short delay
                    setTimeout(() => {
                        window.location.href = 'dashboard.php';
                    }, 1000);
                } else {
                    showAlert(data.message, 'error');
                }
            })
            .catch(error => {
                submitButton.classList.remove('loading');
                submitButton.disabled = false;
                showAlert('An error occurred. Please try again.', 'error');
                console.error('Error:', error);
            });
        });
    }
    
    // Auto-hide alerts after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            fadeOut(alert);
        }, 5000);
    });
});

/**
 * Validate email format
 */
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

/**
 * Show alert message
 */
function showAlert(message, type) {
    // Remove existing alerts
    const existingAlerts = document.querySelectorAll('.alert');
    existingAlerts.forEach(alert => alert.remove());
    
    // Create new alert
    const alert = document.createElement('div');
    alert.className = `alert alert-${type}`;
    alert.textContent = message;
    
    // Insert before form
    const loginForm = document.getElementById('loginForm');
    loginForm.parentNode.insertBefore(alert, loginForm);
    
    // Auto-hide after 5 seconds
    setTimeout(() => {
        fadeOut(alert);
    }, 5000);
}

/**
 * Fade out element
 */
function fadeOut(element) {
    let opacity = 1;
    const timer = setInterval(() => {
        if (opacity <= 0.1) {
            clearInterval(timer);
            element.remove();
        }
        element.style.opacity = opacity;
        opacity -= 0.1;
    }, 50);
}

/**
 * Toggle password visibility (if needed in future)
 */
function togglePasswordVisibility(inputId) {
    const input = document.getElementById(inputId);
    if (input.type === 'password') {
        input.type = 'text';
    } else {
        input.type = 'password';
    }
}
