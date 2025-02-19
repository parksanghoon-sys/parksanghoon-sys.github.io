window.measurePing = async function() {
    let start = performance.now();
    await fetch("https://www.google.com", { method: "HEAD", mode: "no-cors" });
    let end = performance.now();
    return (end - start).toFixed(2);
};

window.measureDownloadSpeed = async function () {
    let fileSize = 5 * 1024 * 1024; // 5MB
    let start = performance.now();

    let response = await fetch("https://speed.cloudflare.com/__down?bytes=5242880", {
        method: "GET",
        cache: "no-store",
        mode: "no-cors"
    });

    await response.arrayBuffer(); // 데이터를 실제로 다운로드

    let end = performance.now();
    let duration = (end - start) / 1000; // 초 단위
    let speedMbps = (fileSize * 8) / (duration * 1024 * 1024);
    return speedMbps.toFixed(2);
};

window.measureUploadSpeed = async function () {
    let fileSize = 2 * 1024 * 1024; // 2MB
    let data = new Uint8Array(fileSize); // 2MB 더미 데이터 생성

    let start = performance.now();

    try {
        await fetch("https://speed.cloudflare.com/__up", {
            method: "POST",
            body: data,
            mode: "no-cors"
        });
    } catch (e) {
        // 응답을 받을 수 없어도 업로드는 진행됨
    }

    let end = performance.now();

    let duration = (end - start) / 1000; // 초 단위
    let speedMbps = (fileSize * 8) / (duration * 1024 * 1024);
    return speedMbps.toFixed(2);
};

