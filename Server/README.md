# 🌐 Server Component - Educational Security Research Backend

[![Go 1.21+](https://img.shields.io/badge/Go-1.21+-blue.svg)](https://golang.org/)
[![Firebase](https://img.shields.io/badge/Backend-Firebase-orange.svg)](https://firebase.google.com/)
[![Security First](https://img.shields.io/badge/Security-First-red.svg)](#security-features)
[![Educational](https://img.shields.io/badge/Purpose-Educational-green.svg)](#educational-objectives)

> **⚠️ EDUCATIONAL PURPOSE**: This server component demonstrates secure backend development, data protection, and compliance frameworks for cybersecurity education and authorized research.

## 🎓 Educational Objectives

**Learn Advanced Security Concepts:**
- **🔐 Backend Security Architecture**: Secure API design and implementation
- **📄 Data Protection Frameworks**: GDPR/CCPA compliant data handling
- **🔑 Authentication Systems**: Multi-factor auth and API security
- **📊 Audit & Compliance**: Enterprise-grade logging and monitoring
- **☁️ Cloud Security**: Firebase security rules and cloud best practices

**Target Learning Audience:**
- 🎓 Backend security developers
- 🔍 Cloud security engineers
- 📈 Compliance professionals
- 🏛️ Academic security researchers

## 📊 Prerequisites & Learning Setup

### 🔧 **Technical Learning Requirements**
- **Runtime**: Go 1.21+ (learn modern Go security practices)
- **Cloud Platform**: Google Cloud Platform or Firebase (cloud security)
- **Database**: Firestore (NoSQL security patterns)
- **Storage**: Cloud Storage (secure file handling)
- **Monitoring**: Cloud Logging (security audit trails)

### 📜 **Educational Prerequisites**
- **✅ Basic Go Knowledge**: Understand Go syntax and concurrency
- **✅ Cloud Security Basics**: Familiarity with cloud security concepts
- **✅ API Security**: Understanding of REST API security principles
- **✅ Compliance Awareness**: Basic knowledge of GDPR/CCPA requirements

## 🏗️ Educational Architecture

### 🔒 **Security-First Backend Design**
```
                    🌐 Client Requests (TLS 1.3)
                              │
                    ┌─────────────────┐
                    │  Load Balancer    │ ← Learn: Traffic distribution
                    │  (Rate Limiting)  │
                    └─────────┬─────────┘
                             │
              ┌───────────────────────┐
              │    Auth Middleware     │ ← Learn: API key validation
              │  (API Key + JWT)     │
              └───────────┬───────────┘
                       │
    ┌─────────────────────────────────┐
    │         Go Server Core          │ ← Learn: Secure server design
    │   (Encryption + Processing)     │
    └───────────┬───────────┬───────────┘
             │               │
    ┌────────────┐    ┌───────────────┐
    │  Firestore   │    │ Cloud Storage │ ← Learn: Cloud data security
    │ (Documents) │    │  (Files)      │
    └────────────┘    └───────────────┘
```

### 📚 **Learning Components**
- **🔑 Authentication Layer**: Learn API security and access control
- **🔐 Encryption Handler**: Understand data protection in transit and at rest
- **📊 Data Processor**: Study secure data transformation and anonymization
- **📝 Audit System**: Explore compliance logging and monitoring
- **☁️ Cloud Integration**: Master Firebase security rules and patterns

## Configuration

### Environment Variables (Production)
```bash
# Firebase Configuration
FIREBASE_PROJECT_ID=your-research-project-id
FIREBASE_STORAGE_BUCKET=your-research-project.appspot.com
GOOGLE_APPLICATION_CREDENTIALS=/path/to/service-account.json

# API Security
API_KEY=your-secure-api-key-from-secrets-manager
AES_KEY_B64=your-encryption-key-from-secrets-manager
ALLOWED_ORIGINS=https://research-dashboard.example.com

# Data Retention
DATA_RETENTION_DAYS=90
AUTO_DELETION_ENABLED=true
ANONYMIZATION_DELAY_HOURS=24

# Monitoring
LOG_LEVEL=INFO
AUDIT_LOG_RETENTION_DAYS=2555  # 7 years for compliance
METRICS_ENDPOINT=https://monitoring.example.com
```

### Firebase Setup
```bash
# Initialize Firebase project
firebase init firestore
firebase init storage

# Configure security rules
firebase deploy --only firestore:rules,storage
```

## Deployment & Run

### Development Setup
```bash
# Clone and setup
git clone [repository]
cd Server
go mod tidy

# Configure environment
cp .env.example .env
# Edit .env with your configuration

# Run development server
go run main.go
```

### Production Deployment
```bash
# Build optimized binary
CGO_ENABLED=0 GOOS=linux go build -a -ldflags '-extldflags "-static"' -o research-server main.go

# Container deployment
docker build -t research-server:latest .
docker run -p 8080:8080 \
  -e FIREBASE_PROJECT_ID=your-project \
  -e GOOGLE_APPLICATION_CREDENTIALS=/app/creds.json \
  research-server:latest
```

### Cloud Deployment (Google Cloud Run)
```bash
# Deploy to Cloud Run with secrets
gcloud run deploy research-server \
  --image gcr.io/your-project/research-server:latest \
  --platform managed \
  --region us-central1 \
  --set-env-vars FIREBASE_PROJECT_ID=your-project \
  --set-secrets API_KEY=api-key-secret:latest \
  --set-secrets AES_KEY_B64=encryption-key-secret:latest
```

## Testing

### Unit Testing
```bash
# Run all tests with coverage
go test -v -race -coverprofile=coverage.out ./...

# Generate coverage report
go tool cover -html=coverage.out -o coverage.html
```

### Integration Testing
```bash
# Test with synthetic data
go test -tags=integration -v ./tests/integration/...

# Test encryption/decryption pipeline
curl -X POST http://localhost:8080/receive \
  -H "X-API-KEY: test-key" \
  -H "Content-Type: text/plain" \
  --data-binary @testdata/sample-encrypted.txt

# Test consent withdrawal endpoint
curl -X DELETE http://localhost:8080/data/user-123 \
  -H "Authorization: Bearer admin-token"
```

### Load Testing
```bash
# Performance testing with hey
hey -n 1000 -c 10 -H "X-API-KEY: test-key" \
  -m POST -d @testdata/sample.txt \
  http://localhost:8080/receive
```

## Security & Privacy

### Authentication & Authorization
- **API Key Validation**: All requests require valid API keys stored in secrets manager
- **Rate Limiting**: Configurable request rate limits per client/IP address
- **CORS Protection**: Strict cross-origin resource sharing policies
- **TLS Enforcement**: HTTPS-only communication with TLS 1.3 minimum

### Data Protection
- **Encryption-in-Transit**: All API communications use TLS encryption
- **Encryption-at-Rest**: Firebase data encrypted with Google-managed keys
- **Data Anonymization**: Automatic removal of personal identifiers after processing
- **Retention Policies**: Automatic data deletion per configured retention periods

### Access Controls
- **Role-Based Access**: Researcher vs. administrator permission levels
- **Audit Logging**: All data operations logged with user attribution
- **Consent Tracking**: Links data records to consent artifacts
- **Deletion Enforcement**: Automated data purging upon consent withdrawal

### Compliance Features
- **GDPR Article 17**: Automated right-to-deletion implementation
- **Data Minimization**: Collection limited to research-necessary metadata only
- **Purpose Limitation**: Data usage restricted to documented research objectives
- **Breach Detection**: Automated monitoring for unauthorized access attempts

## API Endpoints

### Data Collection
```
POST /receive
Headers: X-API-KEY, Content-Type: text/plain
Body: Encrypted research data
Response: 200 OK | 401 Unauthorized | 400 Bad Request
```

### Consent Management
```
DELETE /data/{user-id}
Headers: Authorization: Bearer {admin-token}
Response: 200 OK | 404 Not Found | 403 Forbidden

GET /consent/{user-id}
Headers: Authorization: Bearer {token}
Response: Consent status and metadata
```

### Administration
```
GET /health
Response: Service health and status information

GET /metrics
Headers: Authorization: Bearer {metrics-token}
Response: Prometheus-compatible metrics

POST /admin/purge-expired
Headers: Authorization: Bearer {admin-token}
Response: Purged data count and status
```

## Troubleshooting

### Common Issues

**Firebase Connection Errors**
- Verify service account credentials are correctly configured
- Check Firebase project ID and IAM permissions
- Ensure Firestore and Storage APIs are enabled

**Authentication Failures**
- Verify API keys are stored securely and accessible
- Check rate limiting configurations and client IP allowlists
- Confirm CORS settings match client domains

**Data Processing Errors**
- Validate AES encryption keys match client configuration
- Check data format compliance with expected schema
- Monitor decryption error rates in application logs

**Storage Issues**
- Verify Firebase storage bucket permissions and quotas
- Check data retention policies and automated cleanup jobs
- Monitor storage costs and usage patterns

## File Structure

### Core Implementation Files
- `main.go` - HTTP server setup and routing configuration
- `handlers/` - HTTP request handlers and middleware
- `auth/` - Authentication and authorization logic
- `encryption/` - Data encryption and decryption services
- `storage/` - Firebase integration and data persistence
- `audit/` - Comprehensive audit logging system

### Configuration Files
- `go.mod` - Go module dependencies and versions
- `Dockerfile` - Container build configuration
- `firebase-config/` - Firebase security rules and configuration
- `.env.example` - Environment variable template

### Infrastructure Files
- `deploy/` - Deployment scripts and configurations
- `monitoring/` - Prometheus metrics and alerting rules
- `scripts/` - Database migration and maintenance scripts

## Important Security Notes

⚠️ **Critical Security Requirements:**
- Never deploy without proper secrets management integration
- All API keys and encryption keys must be rotated regularly
- Enable comprehensive audit logging and monitoring
- Implement automated backup and disaster recovery procedures
- Conduct regular security assessments and penetration testing

📋 **Pre-Deployment Checklist:**
- [ ] Service account credentials secured in secrets manager
- [ ] API rate limiting and DDoS protection configured
- [ ] Data retention and automated deletion policies active
- [ ] Audit logging and monitoring systems operational
- [ ] Incident response and breach notification procedures tested
- [ ] Firebase security rules reviewed and restrictive
- [ ] TLS certificates valid and automatically renewed
