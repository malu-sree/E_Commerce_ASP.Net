$(document).ready(function () {
    $("#addProductForm").submit(function (event) {
        let isValid = true;

      
        let name = $("#Name").val().trim();
        if (name === "") {
            $("#nameError").text("Product name is required.");
            isValid = false;
        } else {
            $("#nameError").text("");
        }

      
        let description = $("#Description").val().trim();
        if (description === "") {
            $("#descriptionError").text("Description is required.");
            isValid = false;
        } else {
            $("#descriptionError").text("");
        }

        let price = $("#Price").val().trim();
        if (price === "" || isNaN(price) || price <= 0) {
            $("#priceError").text("Enter a valid price.");
            isValid = false;
        } else {
            $("#priceError").text("");
        }

     
        let quantity = $("#Quantity").val().trim();
        if (quantity === "" || isNaN(quantity) || parseInt(quantity) <= 0) {
            $("#quantityError").text("Enter a valid quantity.");
            isValid = false;
        } else {
            $("#quantityError").text("");
        }

     
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

        if (!isValid) {
            event.preventDefault(); 
        }
    });

    
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
