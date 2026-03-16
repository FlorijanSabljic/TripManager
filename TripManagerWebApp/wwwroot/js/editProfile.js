document.addEventListener('DOMContentLoaded', () => {

    const saveProfileBtn = document.getElementById('saveProfile');
    if (!saveProfileBtn) return;

    saveProfileBtn.addEventListener('click', (event) => {
        event.preventDefault();

        const form = saveProfileBtn.closest('form');
        const formData = new FormData(form);
        const url = form.action;
        const body = new URLSearchParams(formData);

        fetch(url, {
            method: 'POST',
            body: body,
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    showProfileMessage('Profile updated successfully!', 'success');
                } else {
                    showProfileMessage('Error updating profile: ' + data.error, 'danger');
                }
            })
        function showProfileMessage(message, type) {
            let popupElement = document.getElementById('profilePopup');
            let messageElement = document.getElementById('profilePopupMsg');
            if (!popupElement || !messageElement) return;
            popupElement.className = 'profile-popup ' + type;
            messageElement.textContent = message;
            popupElement.style.display = 'block';
            setTimeout(() => {
                popupElement.style.display = 'none';
            }, 2500);
        }
    });
}); 
