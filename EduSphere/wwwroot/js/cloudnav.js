document.querySelectorAll('.cloud-efekt-link').forEach(link => {
    link.addEventListener('click', function (e) {
        const cloud = link.querySelector('.hover-cloud');
        if (cloud) {
            e.preventDefault();
            link.classList.add('clicked');
            setTimeout(() => {
                window.location = link.href;
            }, 370);
        }
    });
});