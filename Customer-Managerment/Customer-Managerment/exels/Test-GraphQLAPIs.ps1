# GraphQL API Test Script
$baseUrl = "http://localhost:5114/graphql"
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm"

# Test connection first
try {
    $testConn = Invoke-RestMethod -Uri $baseUrl -Method Post -ContentType "application/json" -Body '{"query":"{ __typename }"}' -TimeoutSec 5
    Write-Host "[OK] Server connection successful" -ForegroundColor Green
} catch {
    Write-Host "[ERROR] Cannot connect to server. Please start the server first." -ForegroundColor Red
    exit 1
}

# Function to test API
function Test-API {
    param($name, $query, $description)
    try {
        $body = @{"query" = $query} | ConvertTo-Json -Compress
        $resp = Invoke-RestMethod -Uri $baseUrl -Method Post -ContentType "application/json" -Body $body -TimeoutSec 10
        if ($resp.errors) {
            return @{Name=$name; Status="FAIL"; Message=$resp.errors[0].message; Description=$description}
        }
        return @{Name=$name; Status="PASS"; Message="Success"; Description=$description}
    } catch {
        return @{Name=$name; Status="ERROR"; Message=$_.Exception.Message; Description=$description}
    }
}

# Run all tests
$tests = @()

$tests += Test-API -name "login" -query 'mutation { login(authenticationRequest: { username: "admin", password: "Admin123!" }) { accessToken staff { id fullname role } } }' -description "Login to system"
$tests += Test-API -name "register" -query 'mutation { register(registerRequest: { username: "testuser", email: "test@example.com", password: "Test123!", confirmPassword: "Test123!" }) }' -description "Register new account"
$tests += Test-API -name "sendOTPForgotPassword" -query 'mutation { sendOTPForgotPassword(forgotPasswordRequest: { email: "admin@example.com" }) }' -description "Send OTP for forgot password"
$tests += Test-API -name "staffs" -query 'query { staffs { id username role person { fullname email phone } } }' -description "Get list of staff"
$tests += Test-API -name "staffById" -query 'query { staffById(idStaff: "00000000-0000-0000-0000-000000000001") { id username role } }' -description "Get staff by ID"
$tests += Test-API -name "leads" -query 'query { leads { id resource person { fullname email phone location } } }' -description "Get list of leads"
$tests += Test-API -name "leadById" -query 'query { leadById(idLead: "00000000-0000-0000-0000-000000000001") { id resource } }' -description "Get lead by ID"
$tests += Test-API -name "createLead" -query 'mutation { createLead(leadCreationRequest: { fullname: "Test Lead", email: "test@lead.com", phone: "0909123456", location: "HCM", resource: "Website" }) { id resource } }' -description "Create new lead"
$tests += Test-API -name "updateLead" -query 'mutation { updateLead(leadUpdateRequest: { fullname: "Updated Lead" }, idLead: "00000000-0000-0000-0000-000000000001") { id resource } }' -description "Update lead"
$tests += Test-API -name "deleteLead" -query 'mutation { deleteLead(idLead: "00000000-0000-0000-0000-000000000001") }' -description "Delete lead"
$tests += Test-API -name "customers" -query 'query { customers { id person { fullname email phone location } } }' -description "Get list of customers"
$tests += Test-API -name "customerById" -query 'query { customerById(idCustomer: "00000000-0000-0000-0000-000000000001") { id } }' -description "Get customer by ID"
$tests += Test-API -name "createCustomer" -query 'mutation { createCustomer(customerCreationRequest: { fullname: "Test Customer", email: "test@cust.com", phone: "0909123456", location: "HCM" }) { id } }' -description "Create new customer"
$tests += Test-API -name "updateCustomer" -query 'mutation { updateCustomer(customerUpdateRequest: { fullname: "Updated Customer" }, idCustomer: "00000000-0000-0000-0000-000000000001") { id } }' -description "Update customer"
$tests += Test-API -name "deleteCustomer" -query 'mutation { deleteCustomer(idCustomer: "00000000-0000-0000-0000-000000000001") }' -description "Delete customer"
$tests += Test-API -name "contacts" -query 'query { contacts { idContact type title content status lead { id } staff { id } } }' -description "Get list of contacts"
$tests += Test-API -name "contactById" -query 'query { contactById(idContact: "00000000-0000-0000-0000-000000000001") { idContact type } }' -description "Get contact by ID"
$tests += Test-API -name "createContact" -query 'mutation { createContact(contactCreationRequest: { type: "CALL", title: "Test Contact", content: "Test content", status: NEW }) { idContact } }' -description "Create new contact"
$tests += Test-API -name "updateContact" -query 'mutation { updateContact(contactUpdateRequest: { status: RESOLVED }, idContact: "00000000-0000-0000-0000-000000000001") { idContact } }' -description "Update contact"
$tests += Test-API -name "deleteContact" -query 'mutation { deleteContact(idContact: "00000000-0000-0000-0000-000000000001") }' -description "Delete contact"
$tests += Test-API -name "deals" -query 'query { deals { idDeal title price status customer { id } staff { id } } }' -description "Get list of deals"
$tests += Test-API -name "dealById" -query 'query { dealById(idDeal: "00000000-0000-0000-0000-000000000001") { idDeal title } }' -description "Get deal by ID"
$tests += Test-API -name "createDeal" -query 'mutation { createDeal(dealCreationRequest: { title: "Test Deal", content: "Test", price: 1000000, status: OPEN }) { idDeal } }' -description "Create new deal"
$tests += Test-API -name "updateDeal" -query 'mutation { updateDeal(dealUpdateRequest: { status: WON }, idDeal: "00000000-0000-0000-0000-000000000001") { idDeal } }' -description "Update deal"
$tests += Test-API -name "deleteDeal" -query 'mutation { deleteDeal(idDeal: "00000000-0000-0000-0000-000000000001") }' -description "Delete deal"
$tests += Test-API -name "statistics" -query 'query { statistics { totalDeals successfulDeals failedDeals totalCustomers totalLeads totalContacts totalStaffs totalProfit } }' -description "Get dashboard statistics"
$tests += Test-API -name "chartDeal" -query 'query { chartDeal { totalSuccessDeal totalFailedDeal totalSuccessPrice totalFailedPrice listSuccessfullDeal { idDeal title price } listFailedDeal { idDeal title price } } }' -description "Get chart deal data"
$tests += Test-API -name "searchLeads" -query 'query { searchLeads(keyword: "test") { id resource person { fullname email } } }' -description "Search leads"
$tests += Test-API -name "searchCustomers" -query 'query { searchCustomers(keyword: "test") { id person { fullname email } } }' -description "Search customers"
$tests += Test-API -name "restoreStaff" -query 'mutation { restoreStaff(idStaff: "00000000-0000-0000-0000-000000000001") { id username } }' -description "Restore deleted staff"
$tests += Test-API -name "restoreLead" -query 'mutation { restoreLead(idLead: "00000000-0000-0000-0000-000000000001") { id resource } }' -description "Restore deleted lead"
$tests += Test-API -name "restoreCustomer" -query 'mutation { restoreCustomer(idCustomer: "00000000-0000-0000-0000-000000000001") { id } }' -description "Restore deleted customer"
$tests += Test-API -name "getChatWelcomeMessage" -query 'query { getChatWelcomeMessage }' -description "Get chat welcome message"
$tests += Test-API -name "historyMessage" -query 'query { historyMessage(idStaff: "00000000-0000-0000-0000-000000000001") { idMessage content isBot } }' -description "Get chat history"
$tests += Test-API -name "sendChatMessage" -query 'mutation { sendChatMessage(chatRequest: { content: "Hello", idStaff: "00000000-0000-0000-0000-000000000001" }) { content isBot } }' -description "Send chat message"

# Summary
$passCount = ($tests | Where-Object { $_.Status -eq "PASS" }).Count
$failCount = ($tests | Where-Object { $_.Status -eq "FAIL" }).Count
$errorCount = ($tests | Where-Object { $_.Status -eq "ERROR" }).Count

Write-Host ""
Write-Host "=== TEST SUMMARY ===" -ForegroundColor Cyan
Write-Host "Total APIs tested: $($tests.Count)"
Write-Host "  PASS: $passCount" -ForegroundColor Green
Write-Host "  FAIL: $failCount" -ForegroundColor Red
Write-Host "  ERROR: $errorCount" -ForegroundColor Yellow

Write-Host ""
Write-Host "=== DETAILED RESULTS ===" -ForegroundColor Cyan
$tests | Format-Table Name, Status, Message -AutoSize

# Save to CSV
$csvContent = "STT,Name,Description,Status,Message`n"
$i = 1
foreach ($t in $tests) {
    $csvContent += "$i,$($t.Name),$($t.Description),$($t.Status),$($t.Message)`n"
    $i++
}

$csvPath = "D:\ProjectASP.NET\Customer-Managerment\Customer-Managerment\Customer-Managerment\exels\GraphQL_API_Test_Results.csv"
[System.IO.File]::WriteAllText($csvPath, $csvContent, [System.Text.Encoding]::UTF8)
Write-Host ""
Write-Host "Results saved to: $csvPath" -ForegroundColor Green
