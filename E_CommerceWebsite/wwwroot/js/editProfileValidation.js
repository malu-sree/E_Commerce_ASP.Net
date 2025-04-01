$(document).ready(function () {
    $("#editProfileForm").submit(function (event) {
        let isValid = true;

       
        let name = $("#Name").val().trim();
        if (name === "") {
            $("#nameError").text("Full Name is required.");
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
        let phoneRegex = /^[0-9]{10}$/;
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

        if (!isValid) {
            event.preventDefault(); 
        }
    });
});
