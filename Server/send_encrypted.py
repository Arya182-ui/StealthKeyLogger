import base64
import os
import requests
from Crypto.Cipher import AES
from Crypto.Random import get_random_bytes
from Crypto.Util.Padding import pad
import dotenv

dotenv.load_dotenv()

# === 🔐 CONFIG ===
API_URL = "http://localhost:8080/receive"  # Server URL
API_KEY = os.getenv("API_KEY")
AES_KEY_B64 = os.getenv("AES_KEY_B64")  

# === 📸 Load PNG image as bytes (use a real screenshot or any PNG)
with open("logo.png", "rb") as f:  # replace with screenshot file
    image_bytes = f.read()

# === 🔒 Encrypt using AES-CBC ===
key = base64.b64decode(AES_KEY_B64)
iv = get_random_bytes(16)
cipher = AES.new(key, AES.MODE_CBC, iv)
ciphertext = cipher.encrypt(pad(image_bytes, AES.block_size))

# Combine IV + ciphertext, then Base64 encode
payload = base64.b64encode(iv + ciphertext).decode()

# === 📤 Send to server ===
headers = {
    "Content-Type": "text/plain",
    "X-API-KEY": API_KEY
}

response = requests.post(API_URL, data=payload, headers=headers)

# === 📄 Output ===
print(f"Status: {response.status_code}")
print("Response:", response.text)
