function getUsernameFromToken() {
  const token = localStorage.getItem("token");
  if (!token) return null;

  try {
    const payloadBase64 = token.split('.')[1];
    const decodedPayload = JSON.parse(atob(payloadBase64));
    return decodedPayload.unique_name || null;
  } catch (error) {
    console.error("Blad :", error);
    return null;
  }
}
