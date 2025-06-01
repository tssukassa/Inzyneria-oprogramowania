document.getElementById('loginForm').addEventListener('submit', async (e) => {
  e.preventDefault();

  const username = document.getElementById('username').value;
  const password = document.getElementById('password').value;

  try {
    const response = await fetch('http://localhost:5077/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, password })
    });

    const result = document.getElementById('result');

    if (response.ok) {
      const data = await response.json();
      result.textContent = 'Zalogowany! Token:' + data.token;
      result.style.color = 'green';
    } else {
      result.textContent = 'Błąd logowania. Sprawdź login i hasło.';
      result.style.color = 'red';
    }
  } catch (error) {
    console.error("Błąd żądania:", error);
    document.getElementById('result').textContent = 'Błąd połączenia z serwerem';
    document.getElementById('result').style.color = 'red';
  }
});

// Zamknięncie formy logowania na "x"
document.querySelector(".close-button").addEventListener("click", () => {
  document.querySelector(".login-box").style.display = "none";
});
