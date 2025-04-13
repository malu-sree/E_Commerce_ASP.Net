/// <summary>
/// Executes when the DOM is fully loaded.
/// Binds validation and image preview logic to the Add Product form.
/// </summary>

$(document).ready(function () {


    /// <summary>
    /// Form submit event handler for validating product fields before submission.
    /// </summary>
    $("#addProductForm").submit(function (event) {


        let isValid = true;
        /// <summary>
        /// Validate Product Name: Field must not be empty.
        /// </summary>
      
        let name = $("#Name").val().trim();
        if (name === "") {
            $("#nameError").text("Product name is required.");
            isValid = false;
        } else {
            $("#nameError").text("");
        }

        /// <summary>
        /// Validate Product Description: Field must not be empty.
        /// </summary>
        let description = $("#Description").val().trim();
        if (description === "") {
            $("#descriptionError").text("Description is required.");
            isValid = false;
        } else {
            $("#descriptionError").text("");
        }

        /// <summary>
        /// Validate Price: Must be a number greater than 0.
        /// </summary>
        let price = $("#Price").val().trim();
        if (price === "" || isNaN(price) || price <= 0) {
            $("#priceError").text("Enter a valid price.");
            isValid = false;
        } else {
            $("#priceError").text("");
        }

        /// <summary>
        /// Validate Quantity: Must be a whole number greater than 0.
        /// </summary>
        let quantity = $("#Quantity").val().trim();
        if (quantity === "" || isNaN(quantity) || parseInt(quantity) <= 0) {
            $("#quantityError").text("Enter a valid quantity.");
            isValid = false;
        } else {
            $("#quantityError").text("");
        }

        /// <summary>
        /// Validate Image Upload:
        /// - File must be selected
        /// - File must be an image
        /// - File size must be under 2MB
        /// </summary>
        let image = $("#imageUpload")[0].files[0];
        if (!image) {
            $("#imageError").text("Please upload a product image.");
            isValid = false;
        } else if (!image.type.startsWith("image/")) {
            $("#imageError").text("Only image files are allowed.");
            isValid = false;
        } else if (image.size > 2 * 1024 * 1024) { 
            $("#imageError").text("File size must be less than 2MB.");
            isValid = false;
        } else {
            $("#imageError").text("");
        }

        /// <summary>
        /// Prevent form submission if validation fails.
        /// </summary>

        if (!isValid) {
            event.preventDefault(); 
        }
    });

    /// <summary>
    /// Image input change event:
    /// Converts selected image file to Base64 and stores it in a hidden input field.
    /// </summary>
    $("#imageUpload").change(function () {
        let file = this.files[0];
        if (file && file.type.startsWith("image/")) {
            let reader = new FileReader();
            reader.onload = function (e) {
                $("#imageBase64").val(e.target.result.split(",")[1]);
            };
            reader.readAsDataURL(file);
        }
    });
});
