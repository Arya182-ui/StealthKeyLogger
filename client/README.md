# Client Component - Interaction Data Collection Agent

[![.NET 9.0](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![Windows](https://img.shields.io/badge/Platform-Windows-blue.svg)](https://www.microsoft.com/windows)
[![Educational Use](https://img.shields.io/badge/Purpose-Educational-green.svg)](#educational-purpose)

> **⚠️ EDUCATIONAL PURPOSE ONLY**: This component is designed exclusively for cybersecurity education, authorized security research, and ethical penetration testing in controlled environments.


## Overview

The Client component is a research-grade data collection agent designed for authorized usability studies and user experience research in controlled environments.

## Prerequisites

- Windows 10/11 (x64)
- .NET 9.0 Runtime
- Administrator privileges for system-level telemetry (optional)
- Valid research authorization and user consent documentation

## High-Level Responsibilities

### Core Functions
- **Consent-Based Telemetry**: Collects interaction metadata only with explicit user consent
- **Usability Metrics**: Records application usage patterns for UX research
- **Performance Monitoring**: Tracks system performance during research sessions
- **Secure Transmission**: Encrypts and transmits data to research servers

### Architecture Components
- **Consent Manager**: Handles consent verification and user notification
- **Data Collector**: Gathers anonymized interaction metrics
- **Encryption Module**: Secures all data before transmission
- **Transmission Handler**: Manages secure data upload to research servers

## Configuration

### Environment Variables (Required)
```
API_URL=https://research-server.example.com/receive
API_KEY=your_research_api_key
AES_KEY_B64=your_base64_encoded_encryption_key
Telegram_Bot_Token=optional_notification_bot_token
Chat_ID=optional_admin_notification_id
```

### Consent Configuration
```
CONSENT_REQUIRED=true
CONSENT_DISPLAY_INTERVAL_MINUTES=60
CONSENT_WITHDRAWAL_ENDPOINT=https://research-server.example.com/withdraw
RESEARCH_CONTACT_EMAIL=research-admin@example.com
```

## Deployment & Run

### Development Setup
1. **Clone Repository**: `git clone [repository]`
2. **Install Dependencies**: `dotnet restore`
3. **Configure Environment**: Set required environment variables
4. **Build Application**: `dotnet build --configuration Release`
5. **Run with Consent UI**: `dotnet run --configuration Debug`

### Production Deployment
1. **Legal Review**: Complete legal and ethics approval process
2. **Consent Integration**: Verify consent mechanisms are active
3. **Build Release**: `dotnet publish --configuration Release`
4. **Deploy with Monitoring**: Deploy with active consent indicators
5. **Audit Logging**: Enable comprehensive audit trails

### Container Deployment
```bash
# Build container with consent UI
docker build -t research-client:latest .

# Run with consent environment
docker run -e CONSENT_REQUIRED=true \
           -e API_URL=https://research-server.example.com \
           research-client:latest
```

## Testing

### Synthetic Data Testing
```bash
# Run with test data generation
dotnet test --logger "console;verbosity=detailed"

# Consent flow testing
dotnet run --configuration Test --args "--synthetic-mode"

# Data encryption verification
dotnet run --args "--test-encryption"
```

### Consent Flow Testing
- Test consent presentation and user interaction
- Verify consent withdrawal mechanisms
- Validate data collection cessation upon withdrawal
- Confirm audit trail generation

## Security & Privacy

### Mandatory Consent Features
- **Pre-Collection Consent**: Explicit consent required before any data collection
- **Visible Indicators**: Active consent status displayed to user at all times
- **Withdrawal Mechanism**: One-click consent withdrawal and data cessation
- **Purpose Transparency**: Clear explanation of research objectives and data usage

### Data Protection
- **Encryption-at-Rest**: All collected data encrypted locally before transmission
- **Minimal Collection**: Only necessary interaction metadata collected
- **Anonymization**: Personal identifiers removed before storage
- **Retention Limits**: Automatic data purging per defined retention policies

### Access Controls
- **Role-Based Permissions**: Researcher access limited to aggregated data only
- **Audit Logging**: All data access and operations logged with timestamps
- **Secure Communication**: TLS 1.3 encryption for all data transmission
- **Credential Management**: Integration with enterprise secrets management

## Troubleshooting

### Common Issues

**Consent UI Not Displaying**
- Verify `CONSENT_REQUIRED=true` in environment
- Check consent display interval configuration
- Ensure UI dependencies are properly installed

**Data Transmission Failures**
- Verify network connectivity to research servers
- Check API key configuration and validity
- Confirm TLS certificate validation

**Permission Errors**
- Run with appropriate user privileges for telemetry collection
- Verify Windows permissions for temporary file creation
- Check firewall settings for outbound HTTPS connections

**High Resource Usage**
- Adjust collection interval in configuration
- Enable resource-aware collection modes
- Monitor system resources during collection sessions

## File Structure

### Core Implementation Files
- `Program.cs` - Main application entry and consent management
- `ConsentManager.cs` - User consent handling and validation
- `DataCollector.cs` - Interaction metadata collection
- `EncryptionService.cs` - Data encryption and secure transmission
- `AuditLogger.cs` - Comprehensive audit trail generation

### Configuration Files
- `LoggerApp.csproj` - Project dependencies and build configuration
- `appsettings.json` - Application configuration and consent settings
- `consent-config.json` - Consent flow configuration and messaging

### Support Files
- `install-consent.ps1` - Consent-aware installation script
- `test-synthetic.ps1` - Synthetic data generation for testing
- `validate-ethics.ps1` - Ethics compliance verification script

## Important Security Notes

⚠️ **This component must never be deployed without:**
- Active consent mechanisms and user notification systems
- Legal and ethics review approval documentation  
- Comprehensive audit logging and monitoring
- Clear data retention and deletion policies
- Incident response and breach notification procedures

📋 **Before any deployment, ensure:**
- All hard-coded credentials are replaced with secure secret management
- Consent withdrawal mechanisms are tested and functional
- Data collection scope is limited to research objectives only
- All transmission endpoints use authenticated, encrypted connections
