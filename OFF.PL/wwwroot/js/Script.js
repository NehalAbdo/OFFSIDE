{
    const button = document.getElementById("btn-message");
    const message = document.getElementById("review");

    button.addEventListener("click", function () {
        message.style.display = "block"; // Show the message

        setTimeout(function () {
            message.style.display = "none"; // Hide the message after 2 seconds
        }, 2000);
    });
}