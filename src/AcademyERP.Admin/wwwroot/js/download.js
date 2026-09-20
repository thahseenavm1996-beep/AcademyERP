window.downloadFile = (fileName, contentType, base64Data) => {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }

    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: contentType });
    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    link.style.display = "none";

    document.body.appendChild(link);
    link.click();
    link.remove();

    URL.revokeObjectURL(url);
};

window.shareReportImage = async (fileName, base64Data) => {
    const byteCharacters = atob(base64Data);
    const byteNumbers = new Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }

    const byteArray = new Uint8Array(byteNumbers);

    const blob = new Blob(
        [byteArray],
        { type: "image/png" }
    );

    const file = new File(
        [blob],
        fileName,
        { type: "image/png" }
    );

    if (
        navigator.share &&
        navigator.canShare &&
        navigator.canShare({ files: [file] })
    ) {
        await navigator.share({
            title: "Class Report",
            text: "Class Report",
            files: [file]
        });

        return true;
    }

    const url = URL.createObjectURL(blob);

    const link = document.createElement("a");
    link.href = url;
    link.download = fileName;
    link.style.display = "none";

    document.body.appendChild(link);
    link.click();
    link.remove();

    URL.revokeObjectURL(url);

    return false;
};
