if (!localStorage.getItem('jwt')) {
    window.location.href = 'login.html';
}

async function loadLogs(count) {
    document.getElementById('error').innerText = '';
    const res = await fetch(`/api/logs/get/${count}`, {
        headers: { 'Authorization': 'Bearer ' + localStorage.getItem('jwt') }
    });
    const tbody = document.querySelector('#logsTable tbody');
    tbody.innerHTML = '';
    if (res.ok) {
        const logs = await res.json();
        logs.sort((a, b) => a.id - b.id);
        logs.forEach(log => {
            const row = `<tr>
                <td>${log.id}</td>
                <td>${log.message}</td>
                <td>${log.timestamp}</td>
            </tr>`;
            tbody.innerHTML += row;
        });
    } else if (res.status === 401) {
        localStorage.removeItem('jwt');
        window.location.href = 'login.html';
    } else {
        document.getElementById('error').innerText = 'Greška pri dohvaćanju zapisnika!';
    }
}

document.getElementById('count').onchange = function () {
    loadLogs(this.value);
};

document.getElementById('logout').onclick = function () {
    localStorage.removeItem('jwt');
    window.location.href = 'login.html';
};

// Učitaj početnih 10 zapisa
loadLogs(document.getElementById('count').value);