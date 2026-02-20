export const onRequestGet = async () => {
  try {
    const response = await fetch("https://api.chucknorris.io/jokes/random", {
      method: "GET",
      headers: {
        "Accept": "application/json",
      },
    });

    if (!response.ok) {
      return new Response("Failed to fetch joke", { status: 502 });
    }

    const data = await response.json();

    return Response.json(data);
  } catch (error) {
    console.error(error);
    return new Response("Internal server error", { status: 500 });
  }
};