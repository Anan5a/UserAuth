// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//$(".toast").toast("show")

document.addEventListener('click', function (event) {
    if (event.target.matches(".eye-password")) {
        const parentElement = event.target.parentElement;
        const eyeIcon = parentElement.querySelector('.eye-password');
        const passwordField = parentElement.querySelector('input.password');
        const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordField.setAttribute('type', type);
        eyeIcon.classList.toggle('fa-eye');
        eyeIcon.classList.toggle('fa-eye-slash');
        eyeIcon.style.right = '10px';
       
    }
});
