/// <summary>
/// Executes when the DOM is fully loaded.
/// Attaches a submit event handler to the Edit Profile form.
/// </summary>

$(document).ready(function () {

    /// <summary>
    /// Form submit event for Edit Profile form.
    /// Validates fields before allowing form submission.
    /// </summary>
    $("#editProfileForm").submit(function (event) {
        let isValid = true;

        /// <summary>
        /// Validate Full Name: Field is required.
        /// </summary>
       
        let name = $("#Name").val().trim();
        if (name === "") {
            $("#nameError").text("Full Name is required.");
            isValid = false;
        } else {
            $("#nameError").text("");
        }

        /// <summary>
        /// Validate Email: Required and must be in a valid email format.
        /// </summary>
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

        /// <summary>
        /// Validate Phone Number: Required and must be a valid 10-digit number.
        /// </summary>
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

        /// <summary>
        /// Validate Address: Field is required.
        /// </summary>
        let address = $("#Address").val().trim();
        if (address === "") {
            $("#addressError").text("Address is required.");
            isValid = false;
        } else {
            $("#addressError").text("");
        }

        /// <summary>
        /// Prevents form submission if any validation fails.
        /// </summary>
        if (!isValid) {
            event.preventDefault(); 
        }
    });
});
