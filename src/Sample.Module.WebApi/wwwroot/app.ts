// The following sample code uses TypeScript and must be compiled to JavaScript
// before a browser can execute it.
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notifications")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("message", time => {
    var li = document.createElement("li");
    li.textContent = time;
    document.getElementById("messagesList").appendChild(li);
});

// We need an async function in order to use await, but we want this code to run immediately,
// so we use an "immediately-executed async function"
(async () => {
    try {
        await connection.start();
    } catch (e) {
        console.error(e.toString());
    }
})();