# GraphQL API Test Results - Excel Export
# This script creates an Excel file with test results

$csvPath = "D:\ProjectASP.NET\Customer-Managerment\Customer-Managerment\Customer-Managerment\exels\GraphQL_API_Test_Results.csv"
$xlsxPath = "D:\ProjectASP.NET\Customer-Managerment\Customer-Managerment\Customer-Managerment\exels\GraphQL_API_Test_Results.xlsx"

# Read CSV content
$csvContent = Get-Content $csvPath -Raw -Encoding UTF8

# Try to create Excel using COM object
try {
    $excel = New-Object -ComObject Excel.Application
    $excel.Visible = $false
    $excel.DisplayAlerts = $false

    $workbook = $excel.Workbooks.Add()
    $worksheet = $workbook.Worksheets.Item(1)
    $worksheet.Name = "API Test Results"

    # Read CSV and populate Excel
    $lines = $csvContent -split "`n"
    $row = 1

    foreach ($line in $lines) {
        if ($line.Trim() -eq "") { continue }
        $columns = $line -split ","
        $col = 1
        foreach ($cell in $columns) {
            $cleanCell = $cell.Trim('"')
            $worksheet.Cells.Item($row, $col) = $cleanCell
            $col++
        }
        $row++
    }

    # Format header row
    $worksheet.Range("A1:E1").Font.Bold = $true
    $worksheet.Range("A1:E1").Interior.Color = 52896

    # Auto fit columns
    $worksheet.UsedRange.EntireColumn.AutoFit() | Out-Null

    # Save as xlsx
    $workbook.SaveAs($xlsxPath, 51)  # 51 = xlOpenXMLWorkbook
    $workbook.Close()
    $excel.Quit()

    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null

    Write-Host "Excel file created successfully: $xlsxPath" -ForegroundColor Green
} catch {
    Write-Host "Could not create Excel file: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "CSV file is available at: $csvPath" -ForegroundColor Cyan
}
