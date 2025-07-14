# 📊 StealthKeyLogger – React Dashboard

> A modern React-based dashboard to visualize logs captured by the StealthKeyLogger system in real time.

---

## 🌟 Overview

This dashboard fetches and displays:
- 🧾 **Keystrokes** with timestamps and IPs
- 🖼 **Screenshots** uploaded from the client
- 🔍 Filters, sorting, and live previews

It integrates with **Firebase Firestore** and **Firebase Storage** for secure, scalable log retrieval.

---

## 📁 Project Structure

dashboard/
├── src/ # App source code (React + TypeScript)
├── README.md # Dashboard-specific documentation
├── eslint.config.js # ESLint configuration
├── example.env # Sample environment variables
├── index.html # HTML entry point
├── package-lock.json # NPM lock file
├── package.json # Project dependencies and scripts
├── postcss.config.js # PostCSS config (used with TailwindCSS)
├── tailwind.config.js # TailwindCSS custom configuration
├── tsconfig.app.json # TypeScript config for app context
├── tsconfig.json # Base TypeScript configuration
├── tsconfig.node.json # TypeScript config for Node scripts
├── vite.config.ts # Vite build configuration


## 🔐 Environment Variables (`.env`)
**update in Example.env and change name to .env**


---

## 🚀 Getting Started

### 1. Install Dependencies

```bash
cd dashboard/
npm install
```

### 2. Start Development Server
```bash
npm run dev
# Visit http://localhost:5173
```


## 🖼 Features
- 📌 View & Delete real-time logs (Firestore)
- 📷 Auto-load encrypted screenshots (Firebase Storage)
- 🔐 Auth with Firebase (optional)
- 📅 Time-based sorting
- 🌐 IP tagging

## ✅ Tech Stack
- ⚛️ React + Vite
- 🔥 Firebase (Firestore + Storage)
- 💨 TailwindCSS
- 🧪 TypeScript

## 🧠 How It Works
The client sends encrypted logs to the server → Server decrypts and stores in Firestore/Storage → Dashboard fetches and renders them using Firebase SDK.


## 📜 Disclaimer
This dashboard is part of the StealthKeyLogger project and is meant only for ethical research and red teaming labs. You must not use it for unauthorized surveillance or activity.
