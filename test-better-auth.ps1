# Better-Auth EdDSA Integration Test

Write-Host "Testing Better-Auth EdDSA Integration" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$betterAuthUrl = "http://localhost:3000"
$apiUrl = "http://localhost:5037"

# Test 1: Check JWKS endpoint
Write-Host "Test 1: Checking JWKS endpoint..." -ForegroundColor Yellow
try {
    $jwks = Invoke-RestMethod -Uri "$betterAuthUrl/api/auth/jwks" -Method Get
    Write-Host "? JWKS endpoint accessible" -ForegroundColor Green
    Write-Host "Keys found: $($jwks.keys.Count)" -ForegroundColor Green
    foreach ($key in $jwks.keys) {
        Write-Host "  - Algorithm: $($key.alg), Key ID: $($key.kid)" -ForegroundColor Gray
    }
} catch {
    Write-Host "? JWKS endpoint not accessible: $_" -ForegroundColor Red
    Write-Host "Make sure better-auth is running on http://localhost:3000" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# Test 2: Check API ping
Write-Host "Test 2: Checking API health..." -ForegroundColor Yellow
try {
    $ping = Invoke-RestMethod -Uri "$apiUrl/api/users/ping" -Method Get
    Write-Host "? API is running: $($ping.message)" -ForegroundColor Green
} catch {
    Write-Host "? API is not running: $_" -ForegroundColor Red
    Write-Host "Make sure API is running on http://localhost:5037" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# Test 3: Request JWT token
Write-Host "Test 3: Please provide your JWT token from better-auth" -ForegroundColor Yellow
Write-Host "You can find it in:" -ForegroundColor Gray
Write-Host "  - Browser DevTools > Application > Local Storage" -ForegroundColor Gray
Write-Host "  - Or in cookies (look for 'better_auth.session_token' or similar)" -ForegroundColor Gray
Write-Host ""
Write-Host "IMPORTANT: Make sure you're logged in first at $betterAuthUrl" -ForegroundColor Yellow
Write-Host ""
$token = Read-Host "Enter your JWT token"

if ([string]::IsNullOrWhiteSpace($token)) {
    Write-Host "? No token provided" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Test 4: Debug token (decode without validation)
Write-Host "Test 4: Decoding token..." -ForegroundColor Yellow
try {
    $headers = @{
        "Authorization" = "Bearer $token"
    }
    $debugInfo = Invoke-RestMethod -Uri "$apiUrl/api/users/debug-token" -Method Get -Headers $headers
    Write-Host "? Token decoded successfully" -ForegroundColor Green
    Write-Host "Algorithm: $($debugInfo.algorithm)" -ForegroundColor Gray
    Write-Host "Expires: $($debugInfo.expires)" -ForegroundColor Gray
    Write-Host "Is Expired: $($debugInfo.isExpired)" -ForegroundColor Gray
    
    if ($debugInfo.isExpired) {
        Write-Host ""
        Write-Host "??  WARNING: Token has expired!" -ForegroundColor Red
        Write-Host "Please login again at $betterAuthUrl to get a fresh token" -ForegroundColor Yellow
        Write-Host ""
        $continue = Read-Host "Continue anyway? (y/N)"
        if ($continue -ne "y" -and $continue -ne "Y") {
            exit 0
        }
    }
    
    # Show user info from token
    Write-Host ""
    Write-Host "Token Claims:" -ForegroundColor Gray
    foreach ($claim in $debugInfo.payload) {
        if ($claim.type -in @("id", "sub", "name", "email", "emailVerified")) {
            Write-Host "  - $($claim.type): $($claim.value)" -ForegroundColor Gray
        }
    }
} catch {
    Write-Host "? Failed to decode token: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Test 5: Test authentication (validate token)
Write-Host "Test 5: Validating token with API..." -ForegroundColor Yellow
try {
    $authTest = Invoke-RestMethod -Uri "$apiUrl/api/users/test-auth" -Method Get -Headers $headers
    Write-Host "? Token validated successfully!" -ForegroundColor Green
    Write-Host "User ID (string): $($authTest.userIdString)" -ForegroundColor Gray
    Write-Host "Username: $($authTest.username)" -ForegroundColor Gray
    Write-Host "Email: $($authTest.email)" -ForegroundColor Gray
    Write-Host "Email Verified: $($authTest.emailVerified)" -ForegroundColor Gray
    
    if ($authTest.note) {
        Write-Host ""
        Write-Host "??  $($authTest.note)" -ForegroundColor Yellow
    }
    
    if ($authTest.conversionNote) {
        Write-Host ""
        Write-Host "ID Conversion Issue:" -ForegroundColor Yellow
        Write-Host $authTest.conversionNote -ForegroundColor Gray
    }
} catch {
    Write-Host "? Token validation failed: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.Value__
        Write-Host "Status Code: $statusCode" -ForegroundColor Red
        
        if ($statusCode -eq 401) {
            Write-Host ""
            Write-Host "Common causes of 401 Unauthorized:" -ForegroundColor Yellow
            Write-Host "  1. Token has expired (check step 4 output)" -ForegroundColor Gray
            Write-Host "  2. Token signature validation failed" -ForegroundColor Gray
            Write-Host "  3. JWKS keys don't match between better-auth and API" -ForegroundColor Gray
        }
    }
    exit 1
}
Write-Host ""

# Test 6: Get current user
Write-Host "Test 6: Fetching current user profile..." -ForegroundColor Yellow
Write-Host "(This will fail if user doesn't exist in database)" -ForegroundColor Gray
try {
    $user = Invoke-RestMethod -Uri "$apiUrl/api/users/me" -Method Get -Headers $headers
    Write-Host "? User profile retrieved successfully!" -ForegroundColor Green
    Write-Host "Display Name: $($user.displayName)" -ForegroundColor Gray
    Write-Host "Email: $($user.email)" -ForegroundColor Gray
    Write-Host "Subscription Tier: $($user.subscriptionTier)" -ForegroundColor Gray
} catch {
    Write-Host "? Failed to fetch user profile: $_" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.Value__
        if ($statusCode -eq 404) {
            Write-Host ""
            Write-Host "User not found in database. This is expected if:" -ForegroundColor Yellow
            Write-Host "  - This is a new better-auth user" -ForegroundColor Gray
            Write-Host "  - You haven't synced better-auth users to your database" -ForegroundColor Gray
            Write-Host "  - User IDs don't match (string vs int issue)" -ForegroundColor Gray
        }
    }
}
Write-Host ""

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Testing complete!" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. If token is expired: Login again to get fresh token" -ForegroundColor Gray
Write-Host "2. If user ID mismatch: Implement ID mapping or update database schema" -ForegroundColor Gray
Write-Host "3. If all works: Integrate better-auth with your frontend!" -ForegroundColor Gray
