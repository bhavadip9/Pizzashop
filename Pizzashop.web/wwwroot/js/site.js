// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// const passwordIcon = document.querySelector('.eye-icon');
// const input = document.querySelector('.password-box input');

// var password = true;

// passwordIcon.addEventListener('click', function () {
//     if (password) {
//         // using the javascript dynamically changed the INPUT element type attribute from password to text
//         input.setAttribute('type', 'text');
//         passwordIcon.classList.remove('bi-eye-fill');
//         passwordIcon.classList.add('bi-eye-slash-fill');


//     } else {
//         // using the javascript dynamically changed the INPUT element type attribute from text to password
//         input.setAttribute('type', 'password');
//         passwordIcon.classList.remove('bi-eye-slash-fill');
//         passwordIcon.classList.add('bi-eye-fill');

//     }
//     password = !password;
// });

// Define the togglePassword function
function togglePassword() {
    // Select the password input field and eye icon
    const passwordIcon = document.querySelector('.eye-icon');
    const input = document.querySelector('.password-box input');
    
    // Check the current type of the input field and toggle
    if (input.getAttribute('type') === 'password') {
        // Change to text to show the password
        input.setAttribute('type', 'text');
        // Change the icon to a "slash" indicating visible password
        passwordIcon.classList.remove('bi-eye-fill');
        passwordIcon.classList.add('bi-eye-slash-fill');
    } else {
        // Change back to password to hide it
        input.setAttribute('type', 'password');
        // Change the icon back to the "eye" indicating hidden password
        passwordIcon.classList.remove('bi-eye-slash-fill');
        passwordIcon.classList.add('bi-eye-fill');
    }
}
