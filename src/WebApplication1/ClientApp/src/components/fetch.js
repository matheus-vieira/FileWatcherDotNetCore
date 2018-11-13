export fetch => (url, action) => {
    fetch(url)
        .then(response => response.json())
        .then(action)
}