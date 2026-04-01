Help & Documentation
====================

This page provides an overview of how the application works, how to configure it, and how to deploy it on a new server. It is intended for technicians, operators, and administrators responsible for installation, troubleshooting, and maintenance.

* * *

Overview
--------

This application runs as a background service and communicates with BMS (Building Management System) devices over HTTP/IP. It periodically polls each device, processes the returned data, and uploads telemetry securely to Azure IoT Hub using Device Provisioning Service (DPS).

A lightweight web interface is hosted using Kestrel and provides status information, logs, and configuration visibility.

* * *

How the System Works
--------------------

1.  The service loads configuration from `appsettings.json`.
2.  Each device listed under **NetworkSettings** is polled at a fixed interval.
3.  Requests are sent using device‑specific protocols (Danfoss, EmersonE2, EmersonE3).
4.  Telemetry is formatted and uploaded to Azure IoT Hub using DPS authentication.
5.  The web frontend is served via Kestrel on the configured port.

* * *

Accessing the Web Interface
---------------------------

### Kestrel Configuration

The web interface is hosted by Kestrel. The listening address and port are configured in `appsettings.json`:

```json
"Kestrel": {
  "Endpoints": {
    "Http": {
      "Url": "http://0.0.0.0:8080"
    }
  }
}
```

*   **0.0.0.0** means “listen on all network interfaces.” This allows access from other machines on the network.
*   **8080** is the port the web UI is served on.

### Firewall / Network Notes

*   Ensure the server firewall allows inbound TCP traffic on the configured port (default: 8080).
*   If running behind a reverse proxy (NGINX, Apache, IIS), forward traffic to the same port.
*   To change the port, update the `Url` field and restart the service.

Once running, access the web UI using:

http://<server-ip>:8080

Or on the local machine,
http://127.0.0.1:8080

* * *

Configuration Reference
-----------------------

All settings are stored in `appsettings.json`. Below is a detailed explanation of each section.

### LoggingSettings

*   **MinimumLevel** — Lowest log severity written to file.
*   **FileSizeLimitBytes** — Maximum size of each log file before rolling over.
*   **RetainedFileCountLimit** — Number of old log files kept before deletion.

### GeneralSettings

*   **http_request_delay_seconds** — Delay between requests to each device IP.
*   **http_timeout_delay_seconds** — Timeout for each HTTP request.
*   **http_retry_count** — Number of retry attempts for failed device requests.
*   **soft_reset_interval_hours** — Periodic cleanup operation.
*   **keep_alive** — Whether HTTP connections remain open between requests.

### AzureSettings

*   **tenant_id** — Azure AD tenant used for authentication.
*   **client_id** — Service principal used to access Azure Key Vault.
*   **device_id** — IoT Hub device identity name.
*   **scope_id** — DPS provisioning scope ID.
*   **secret_name** — Name of the DPS group key stored in Key Vault.
*   **vault_name** — Azure Key Vault containing the secret.
*   **certificate_subject** — Local certificate used to authenticate to Key Vault.
*   **sas_ttl_days** — Lifetime of generated SAS tokens.

### NetworkSettings

Defines the devices the service will poll.

*   **ip** — Device IP address.
*   **device_type** — Protocol type (case‑sensitive): `Danfoss`, `EmersonE2`, `EmersonE3`.

* * *

Configuration Example
---------------------

```json
{
  "LoggingSettings": {
    "MinimumLevel": "Information",
    "FileSizeLimitBytes": 1000000,
    "RetainedFileCountLimit": 7
  },
  "GeneralSettings": {
    "http_request_delay_seconds": 5,
    "http_timeout_delay_seconds": 10,
    "http_retry_count": 3,
    "soft_reset_interval_hours": 12,
    "keep_alive": false
  },
  "AzureSettings": {
    "tenant_id": "your-tenant-id",
    "client_id": "your-client-id",
    "device_id": "your-iot-device-name",
    "scope_id": "your-dps-scope-id",
    "secret_name": "your-keyvault-secret",
    "vault_name": "your-keyvault-name",
    "certificate_subject": "your-cert",
    "sas_ttl_days": 90
  },
  "NetworkSettings": {
    "bms\_devices": [
      {
        "ip": "10.0.0.10",
        "device_type": "Danfoss"
      }
    ]
  }
}
```

* * *

Troubleshooting
---------------

### Cannot Access Web Interface

*   Verify the service is running.
*   Check that Kestrel is listening on the expected port.
*   Ensure the firewall allows inbound traffic on that port.
*   Confirm you are using the correct server IP address.

### Device Communication Issues

*   Verify the device IP is reachable from the server.
*   Confirm the `device_type` is spelled correctly (case‑sensitive).
*   Increase `http_timeout_delay_seconds` or `http_retry_count` if needed.

### Azure Authentication Issues

*   Ensure the service principal has access to the Key Vault.
*   Verify the certificate is installed and matches `certificate_subject`.
*   Check that the DPS scope ID and device ID are correct.

* * *

For additional support or deployment guidance, consult your system administrator or the deployment documentation provided with this application.
