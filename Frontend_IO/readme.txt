# Frontend_IO – Interfejs użytkownika systemu rezerwacji lotów ✈️

To jest część frontendowa projektu realizowanego w ramach przedmiotu *Inżynieria Oprogramowania*. Aplikacja umożliwia klientom, pracownikom i partnerom zarządzać lotami, rezerwacjami i użytkownikami za pośrednictwem prostego interfejsu webowego.

## 📁 Struktura projektu

- `index.html` – Strona logowania
- `profile.html` – Profil użytkownika (różne widoki dla Client / Employee / Partner)
- `bookings.html` – Rezerwacje zalogowanego użytkownika
- `search-user.html` – Wyszukiwanie użytkownika (Employee)
- `search-booking.html` – Wyszukiwanie rezerwacji (Employee)
- `create-flight.html` – Tworzenie nowego lotu (Employee / Partner)
- `delete-flight.html` – Wyszukiwanie i usuwanie lotów (Employee / Partner)
- `booking-stats.html` – Statystyki rezerwacji (Partner)

## 🛡️ Autoryzacja

- Autoryzacja odbywa się przy użyciu tokenów JWT.
- Token zapisywany jest w `localStorage` jako `token`.
- W pliku `token-utils.js` znajdują się funkcje do:
  - odczytu roli (`getRoleFromToken`)
  - odczytu nazwy użytkownika (`getUsernameFromToken`)

## 👥 Role i dostęp

| Rola     | Uprawnienia                                                           |
|----------|------------------------------------------------------------------------|
| Client   | Przeglądanie i anulowanie własnych rezerwacji                         |
| Employee | Wyszukiwanie użytkowników i rezerwacji, tworzenie i usuwanie lotów    |
| Partner  | Tworzenie/usuwanie lotów, przeglądanie statystyk rezerwacji           |

## 📦 Technologie

- HTML5 + CSS3 (custom)
- JavaScript (Vanilla)
- REST API – komunikacja z backendem (.NET)
- Obsługa żądań `GET`, `POST`, `DELETE` z autoryzacją Bearer Token
- Dynamiczne elementy UI (tworzone w JS) oraz modale potwierdzeń

## ✅ Wymagania

- Backend uruchomiony lokalnie na `http://localhost:5077`
- Token JWT zapisany w `localStorage`
- Przeglądarka z obsługą ES6+

## 🧪 Testowanie

- Testowanie manualne przez otwarcie plików `.html` w przeglądarce
- Każdy moduł sprawdza uprawnienia użytkownika
- Przy próbie dostępu bez uprawnień pojawia się komunikat błędu

## 🧭 Nawigacja

Dostęp do poszczególnych funkcji zależy od roli użytkownika. Interfejs dynamicznie pokazuje dostępne opcje (np. przyciski tylko dla Employee/Partner).

---

## 👨‍💻 Autor

Projekt realizowany przez [@tssukassa](https://github.com/tssukassa)  
Repozytorium: [Inzyneria-oprogramowania](https://github.com/tssukassa/Inzyneria-oprogramowania)

