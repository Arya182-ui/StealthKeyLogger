# 🛡️ StealthKeyLogger

![Safe Version](https://img.shields.io/badge/status-sanitized-green?style=flat-square)

A professional, stealthy, and encrypted keylogging system designed for **authorized research**, **cybersecurity training**, and **red team simulations**.

> ⚠️ **Warning:** This public release is **for educational and authorized testing only**. Misuse is illegal and strictly prohibited.

---

## 🧪 Stripped-Down Public Version

> This repository contains a **sanitized and non-malicious** version of the original `StealthKeyLogger`.

✅ **What's Included**:
- AES-encrypted keystroke and screenshot logging logic (client)
- Secure Go-based server to decrypt and store logs in Firebase
- React-based admin dashboard to view logs in real time

🚫 **What's Removed**:
- DLL injection into `explorer.exe`
- Silent startup persistence and file dropping
- Self-deletion and anti-debugging measures
- Anything harmful or unauthorized in nature

> 🧠 **Why this version exists**: To educate and raise awareness of how such tools work under controlled and ethical environments (labs, sandboxes, academic research).

> 🔒 The original full version is **not public** and contains red-team-only techniques. For access, you must be a certified cybersecurity professional or researcher under an NDA.

---

## 📦 Project Overview

StealthKeyLogger is composed of three modular parts:

| Component | Description |
|----------|-------------|
| 🖥️ `client/` | C# keylogger that encrypts logs and screenshots |
| 🌐 `server/` | Go HTTP server that decrypts and stores logs securely |
| 📊 `dashboard/` | React admin panel to review logs with IP/timestamp context |

---

## 🚀 Features

- 🔐 AES-256 encryption for all logged data
- ☁️ Firebase Firestore for structured log storage
- 🖼 Firebase Storage for screenshots
- 🌍 React dashboard for easy log analysis
- ⚙️ Modular server-client-dashboard design

---

## 📂 Project Structure

StealthKeyLogger/
│
├── client/                   # C# keylogger DLL, injection logic
│   ├── yourlogger.dll
│   ├── Program.cs
│   └── Startup.cs
│
├── server/                   # Go server for decrypting & storing to Firebase
│   ├── main.go
│   ├── .env.example
│   └── README.md
│
├── dashboard/                # React dashboard to view logs
│   ├── src/
│   ├── public/
│   └── README.md
│
├── LICENSE
├── README.md                 # Main documentation
└── DISCLAIMER.md             # Ethical and legal use notice

---

## 🔐 Environment Setup

- **AES_KEY_B64** = base64 AES key
- **FIREBASE_STORAGE_BUCKET**
- **Firebase Service Account JSON (.env)**


---

## ⚙️ Setup Instructions

### 🔐 1. Firebase Configuration
- Create a Firebase project
- Enable **Firestore** and **Firebase Storage**
- Generate a **Service Account JSON**
- Save required values in `.env` or `.env.local`

### 🌍 2. Go Server Setup

```bash
cd server/
go run main.go
# Running at http://localhost:8080
```

### 💻 3. React Dashboard
```bash
cd dashboard/
npm install
npm run dev
# Open: http://localhost:5173
```

### 🧬 4. Inject Client DLL (C#)

Inject yourlogger.dll into explorer.exe

**It will** :
    Run on boot
    Log keys and screenshots
    Send encrypted data to your Go server

---

# ⚠️ DISCLAIMER

This project is strictly for:
- Cybersecurity research
- Red teaming (with permission)
- Academic use

Any misuse (stealing data, unauthorized installs) is **illegal** and **against the terms of this tool**.

By using this project, you agree:
- You're legally allowed to test such tools
- You won't deploy in real-world without **informed consent**


# 🧠 Why Open Source This?
- We believe in transparency and awareness. By exposing how stealthy keylogging can work:
- Security researchers can build defenses
- Students can learn offensive and defensive techniques
- Red teamers can simulate realistic attacks in training

### 🧠 Want the Full Version?

For **collaborations**, **research** **access to the full version**, or custom **red team tooling , responsible disclosure, security audits, or academic research**, 

- [GitHub](https://github.com/Arya182-ui)
- [Email](arya119000@gmail.com)
- [LinkedIn](https://www.linkedin.com/in/ayush-gangwar-cyber/)


## ☕ Support Me

Do you like My projects? You can show your support by buying me a coffee! Your contributions motivate me to keep improving and building more awesome projects. 💻❤  
[![Buy Me a Coffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](http://buymeacoffee.com/Arya182)





