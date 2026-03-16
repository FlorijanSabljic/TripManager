document.getElementById('loginForm').onsubmit = async function (e) {
    e.preventDefault();
    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const res = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password })
    });
    if (res.ok) {
        const data = await res.json();
        localStorage.setItem('jwt', data.token);
        window.location.href = 'logs.html';
    } else {
        document.getElementById('error').innerText = 'Neispravni podaci!';
    }
};
