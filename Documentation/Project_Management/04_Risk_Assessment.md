# Risk Assessment

## Risk Matrix

| Risk | Mitigation Strategy |
| :--- | :--- |
| **Data privacy/security breaches** | JWT authentication and token revocation middleware. |
| **Inaccurate hospital location data** | Latitude/longitude fields for precise mapping. |
| **System performance under load** | In-memory caching for heavy read operations (e.g., reviews). |
