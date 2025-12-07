# Global Exception Handling Guide

## Quick Start for Team Members

### Step 1: Add the using statement to your Controller and Manager
```csharp
using Mos3ef.Api.Exceptions;
```

### Step 2: In your **Manager**, throw exceptions instead of returning null/false
```csharp
// ❌ OLD WAY - Don't do this
public async Task<HospitalReadDto?> GetAsync(string userId)
{
    var hospital = await _repository.GetByUserIdAsync(userId);
    return hospital == null ? null : _mapper.Map<HospitalReadDto>(hospital);
}

// ✅ NEW WAY - Do this
public async Task<HospitalReadDto> GetAsync(string userId)
{
    var hospital = await _repository.GetByUserIdAsync(userId);
    if (hospital == null)
        throw new NotFoundException("Hospital not found.");
    
    return _mapper.Map<HospitalReadDto>(hospital);
}
```

### Step 3: In your **Controller**, just call the Manager - no error handling needed
```csharp
// ❌ OLD WAY - Don't do this
[HttpGet("Get-Profile")]
public async Task<IActionResult> GetAsync()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var hospital = await _hospitalManager.GetAsync(userId);
    if (hospital == null)
        return Ok(Response<Hospital>.Fail("No Hospital found"));  // Returns 200 with fail!

    return Ok(Response<HospitalReadDto>.Success(hospital, "Hospital fetched"));
}

// ✅ NEW WAY - Do this
[HttpGet("Get-Profile")]
public async Task<IActionResult> GetAsync()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (string.IsNullOrEmpty(userId))
        throw new UnauthorizedException("User not authenticated.");

    // Manager throws NotFoundException if not found
    var hospital = await _hospitalManager.GetAsync(userId);
    return Ok(Response<HospitalReadDto>.Success(hospital, "Hospital fetched"));
}
```

---

## Available Exception Classes

| Exception | HTTP Status | When to Use |
|-----------|-------------|-------------|
| `BadRequestException` | 400 | Invalid input, business rule violation |
| `ValidationException` | 400 | Model validation errors (pass list of errors) |
| `UnauthorizedException` | 401 | User not authenticated |
| `ForbiddenException` | 403 | User authenticated but not authorized |
| `NotFoundException` | 404 | Resource not found |
| `InvalidOperationException` | 400 | Business logic errors (built-in C# exception) |

---

## Usage Examples

### BadRequestException
```csharp
if (id <= 0)
    throw new BadRequestException("ID must be greater than 0.");
```

### ValidationException (for model validation)
```csharp
if (!ModelState.IsValid)
{
    var errors = ModelState.Values
        .SelectMany(v => v.Errors)
        .Select(e => e.ErrorMessage)
        .ToList();
    throw new ValidationException(errors);
}
```

### NotFoundException
```csharp
var service = await _repository.GetByIdAsync(id);
if (service == null)
    throw new NotFoundException($"Service with ID {id} not found.");
```

### UnauthorizedException
```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (string.IsNullOrEmpty(userId))
    throw new UnauthorizedException("User not authenticated.");
```

### ForbiddenException
```csharp
if (!isAdmin && myId != requestedId)
    throw new ForbiddenException("You are not authorized to access this resource.");
```

---

## Response Format

All exceptions are caught by `GlobalExceptionMiddleware` and return this JSON format:

```json
{
    "message": "Error message here",
    "data": null,
    "isSucceded": false,
    "dateTime": "2025-12-07T15:54:15"
}
```

This matches the `Response<T>` wrapper format exactly!

---

## Benefits

1. **Consistent HTTP Status Codes** - Frontend can rely on status codes for error handling
2. **Clean Controllers** - No manual error handling logic
3. **Reusable Managers** - Business logic in one place, throws meaningful exceptions
4. **Consistent Response Format** - Same format for success and error responses
