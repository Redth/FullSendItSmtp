# FullSendIt SMTP Relay

A lightweight .NET 10 SMTP relay/forwarder service that runs in Docker. This service listens for incoming SMTP connections and relays messages to a configured upstream SMTP server.

## Features

- 🚀 Simple SMTP relay/forwarder
- 🔐 Optional authentication for incoming connections
- 🐳 Docker-ready with multi-stage builds
- ⚙️ Configuration via environment variables
- 📧 Supports TLS for upstream relay
- 📝 Real-time logging of relayed messages

## Quick Start

### Using Docker Compose

1. Edit `docker-compose.yml` and configure your relay server settings:

```yaml
environment:
  - RELAY_HOST=smtp.gmail.com
  - RELAY_PORT=587
  - RELAY_USERNAME=your-email@gmail.com
  - RELAY_PASSWORD=your-app-password
  - RELAY_USE_TLS=true
```

2. Start the service:

```bash
docker-compose up -d
```

3. The SMTP relay will be available on `localhost:2525`

### Using Docker CLI

```bash
docker build -t fullsendit-smtp-relay .

docker run -d \
  -p 2525:25 \
  -e RELAY_HOST=smtp.gmail.com \
  -e RELAY_PORT=587 \
  -e RELAY_USERNAME=your-email@gmail.com \
  -e RELAY_PASSWORD=your-app-password \
  -e RELAY_USE_TLS=true \
  fullsendit-smtp-relay
```

## Configuration

All configuration is done via environment variables:

### Required Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `RELAY_HOST` | Upstream SMTP server hostname | `smtp.gmail.com` |

### Optional Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `SMTP_LISTEN_PORT` | `25` | Port to listen for incoming SMTP connections |
| `RELAY_PORT` | `587` | Port of the upstream SMTP server |
| `RELAY_USERNAME` | - | Username for upstream SMTP authentication |
| `RELAY_PASSWORD` | - | Password for upstream SMTP authentication |
| `RELAY_USE_TLS` | `true` | Use TLS/STARTTLS for upstream connection |
| `REQUIRE_AUTH` | `false` | Require authentication for incoming connections |
| `AUTH_USERNAME` | - | Username for incoming authentication (if `REQUIRE_AUTH=true`) |
| `AUTH_PASSWORD` | - | Password for incoming authentication (if `REQUIRE_AUTH=true`) |

## Usage Examples

### Gmail Relay

```yaml
environment:
  - RELAY_HOST=smtp.gmail.com
  - RELAY_PORT=587
  - RELAY_USERNAME=your-email@gmail.com
  - RELAY_PASSWORD=your-app-password
  - RELAY_USE_TLS=true
```

### SendGrid Relay

```yaml
environment:
  - RELAY_HOST=smtp.sendgrid.net
  - RELAY_PORT=587
  - RELAY_USERNAME=apikey
  - RELAY_PASSWORD=your-sendgrid-api-key
  - RELAY_USE_TLS=true
```

### With Authentication Required

```yaml
environment:
  - RELAY_HOST=smtp.example.com
  - RELAY_PORT=587
  - RELAY_USERNAME=relay-user
  - RELAY_PASSWORD=relay-pass
  - RELAY_USE_TLS=true
  - REQUIRE_AUTH=true
  - AUTH_USERNAME=client-user
  - AUTH_PASSWORD=client-pass
```

## Testing

Send a test email using `telnet` or `nc`:

```bash
telnet localhost 2525
```

```
EHLO localhost
MAIL FROM:<sender@example.com>
RCPT TO:<recipient@example.com>
DATA
Subject: Test Email

This is a test message.
.
QUIT
```

Or use a mail client configured with:
- SMTP Server: `localhost`
- Port: `2525`
- Authentication: As configured in your environment variables

## Development

### Prerequisites

- .NET 10 SDK
- Docker (optional)

### Build locally

```bash
dotnet restore
dotnet build
```

### Run locally

```bash
export RELAY_HOST=smtp.gmail.com
export RELAY_PORT=587
export RELAY_USERNAME=your-email@gmail.com
export RELAY_PASSWORD=your-app-password
export RELAY_USE_TLS=true

dotnet run
```

## License

See LICENSE file for details.
