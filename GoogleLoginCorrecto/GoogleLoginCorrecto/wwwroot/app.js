
async function fetchApi(url) {
    try {
        // Hacemos la petición fetch. 'credentials: "include"' es CRUCIAL.
        // Le dice al navegador que envíe las cookies de sesión con la petición.
        const response = await fetch(url, {
            credentials: 'include'
        });

        if (!response.ok) {
            // Si la respuesta no es un 2xx, lanzamos un error con el status.
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        // Devolvemos el cuerpo de la respuesta como texto JSON.
        const data = await response.json();
        return data;
    } catch (e) {
        // Si hay un error de red o de otro tipo, lo relanzamos para que C# lo capture.
        console.error("Fetch API error:", e);
        throw e;
    }
}