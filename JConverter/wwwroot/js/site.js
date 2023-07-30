// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {
    const uploadForm = document.getElementById("uploadForm");
    const convertButton = document.getElementById("convertButton");

    // Enable the "Convert" button when both file and checkbox are selected
    document.getElementById("fileInput").addEventListener("change", updateButtonStatus);
    document.getElementById("isPublicCheckbox").addEventListener("change", updateButtonStatus);

    function updateButtonStatus() {
        const fileInput = document.getElementById("fileInput");
        const checkbox = document.getElementById("isPublicCheckbox");

        if (fileInput.files.length > 0 && checkbox.checked) {
            convertButton.disabled = false;
        } else {
            convertButton.disabled = true;
        }
    }

    // Handle the form submission using Axios
    uploadForm.addEventListener("submit", function (event) {
        event.preventDefault(); // Prevent the default form submission

        // Create a new FormData object
        const formData = new FormData(uploadForm);
        console.log("test1");

        axios.post('/api/ConverterApi', formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
            .then(response => {

                Swal.fire({
                    icon: 'success',
                    title: 'Success',
                    text: 'File converted and uploaded successfully!',
                    timer: 2000
                });
                // Optional: Update the view with the converted JSON data or any other information
                // ...
            })
            .catch(error => {

                Swal.fire({
                    icon: 'error',
                    title: 'Message',
                    text: error.response.data.title ?? error.response.data.errorMsg
                });
            });
    });
});

function updateFileLabel() {
    const fileInput = document.getElementById("fileInput");
    const fileLabel = document.getElementById("fileLabel");
    const selectedFileName = document.getElementById("selectedFileName");
    const filenameInput = document.getElementById("filenameInput");

    if (fileInput.files.length > 0) {
        fileLabel.textContent = "Change File";
        selectedFileName.textContent = fileInput.files[0].name;
        filenameInput.value = fileInput.files[0].name;
    } else {
        fileLabel.textContent = "Upload File";
        selectedFileName.textContent = "";
        filenameInput.value = "";
    }
}
document.getElementById("fileInput").addEventListener("change", updateFileLabel);