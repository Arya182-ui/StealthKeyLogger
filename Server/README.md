## 🔐 Educational Keylog Server (Safe Version – Go)

This is a **safe demo server** that simulates receiving and logging data (e.g., keystrokes) over HTTP POST.

### 🚫 Disclaimer
This version **does not** connect to Firebase, store sensitive data, or log real screenshots.
It's designed for **educational or red-team simulation purposes only**.

---

### 🚀 Features:
- Accepts simulated keylog data via `POST /receive`
- Returns all received logs via `GET /logs`
- API key check via `.env` for basic protection
- CORS-enabled for frontend testing

---

### 🧠 For full version (with Firebase, decryption, screenshots), contact me for **responsible use**.

