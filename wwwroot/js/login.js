// Mã xử lý hiệu ứng vuốt qua cho login/register
document.addEventListener('DOMContentLoaded', () => {
    const container = document.querySelector('.container');
    const registerBtn = document.querySelectorAll('.register-btn');
    const loginBtn = document.querySelectorAll('.login-btn');

    if (container && registerBtn && loginBtn) {
        registerBtn.forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.preventDefault();
                container.classList.add('active');
            });
        });

        loginBtn.forEach(btn => {
            btn.addEventListener('click', (e) => {
                e.preventDefault();
                container.classList.remove('active');
            });
        });
    }
});