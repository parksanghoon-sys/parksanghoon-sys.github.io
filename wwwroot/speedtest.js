window.measurePing = async function() {
    let start = performance.now();
    await fetch("https://www.google.com", { method: "HEAD", mode: "no-cors" });
    let end = performance.now();
    return (end - start).toFixed(2);
};

window.measureDownloadSpeed = async function() {
    let fileSize = 5 * 1024 * 1024; // 5MB 가짜 데이터
    let start = performance.now();
    let response = await fetch(`https://speed.hetzner.de/5MB.bin`);
    await response.arrayBuffer();
    let end = performance.now();

    let duration = (end - start) / 1000; // 초 단위
    let speedMbps = (fileSize * 8) / (duration * 1024 * 1024);
    return speedMbps.toFixed(2);
};

window.measureUploadSpeed = async function() {
    let fileSize = 2 * 1024 * 1024; // 2MB 더미 데이터
    let data = new Uint8Array(fileSize);
    let start = performance.now();
    await fetch("https://postman-echo.com/post", {
        method: "POST",
        body: data
    });
    let end = performance.now();

    let duration = (end - start) / 1000;
    let speedMbps = (fileSize * 8) / (duration * 1024 * 1024);
    return speedMbps.toFixed(2);
};
