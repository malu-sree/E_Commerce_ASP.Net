$(document).ready(function () {
    $("#registerForm").submit(function (event) {
        let isValid = true;


        let name = $("#Name").val().trim();
        let nameRegex = /^[A-Za-z\s]+$/;
        if (name === "") {
            $("#nameError").text("Name is required.");
            isValid = false;
        } else if (!nameRegex.test(name)) {
            $("#nameError").text("Name should only contain letters.");
            isValid = false;
        } else {
            $("#nameError").text("");
        }


        let email = $("#Email").val().trim();
        let emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (email === "") {
            $("#emailError").text("Email is required.");
            isValid = false;
        } else if (!emailRegex.test(email)) {
            $("#emailError").text("Enter a valid email.");
            isValid = false;
        } else {
            $("#emailError").text("");
        }

        let phone = $("#PhoneNumber").val().trim();
        let phoneRegex = /^[6-9]\d{9}$/;
        if (phone === "") {
            $("#phoneError").text("Phone number is required.");
            isValid = false;
        } else if (!phoneRegex.test(phone)) {
            $("#phoneError").text("Enter a valid 10-digit phone number.");
            isValid = false;
        } else {
            $("#phoneError").text("");
        }


        let address = $("#Address").val().trim();
        if (address === "") {
            $("#addressError").text("Address is required.");
            isValid = false;
        } else {
            $("#addressError").text("");
        }


        let password = $("#Password").val();
        if (password === "") {
            $("#passwordError").text("Password is required.");
            isValid = false;
        } else if (password.length < 6) {
            $("#passwordError").text("Password must be at least 6 characters.");
            isValid = false;
        } else {
            $("#passwordError").text("");
        }


        let confirmPassword = $("#ConfirmPassword").val();
        if (confirmPassword === "") {
            $("#confirmPasswordError").text("Confirm Password is required.");
            isValid = false;
        } else if (password !== confirmPassword) {
            $("#confirmPasswordError").text("Passwords do not match.");
            isValid = false;
        } else {
            $("#confirmPasswordError").text("");
        }

        let role = $("#Role").val();
        if (role === "") {
            $("#roleError").text("Please select a role.");
            isValid = false;
        } else {
            $("#roleError").text("");
        }

        if (!isValid) {
            event.preventDefault();
        }
    });

   
});

