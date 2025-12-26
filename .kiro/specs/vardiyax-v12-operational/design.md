# Design Document: VardiyaX v1.2 Operational MVP

## Overview

Bu tasarım, VardiyaX mobil uygulamasını READ-ONLY durumundan operasyonel bir kafe yönetim aracına dönüştürür. Mevcut Clean Architecture yapısı korunarak minimum değişiklikle günlük vardiya operasyonları sağlanır.

### Design Principles
- Mevcut mimariyi koruma (Clean Architecture)
- Minimum kod değişikliği
- Form tabanlı UI (drag & drop yok)
- Liste tabanlı görünümler (calendar grid yok)
- Mevcut API endpoint'lerini kullanma

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Mobile App (MAUI)                        │
├─────────────────────────────────────────────────────────────┤
│  Views (XAML)           │  ViewModels (C#)                  │
│  ├─ EmployeeManagePage  │  ├─ EmployeeManageViewModel       │
│  ├─ EmployeeEditPage    │  ├─ EmployeeEditViewModel         │
│  ├─ ShiftCreatePage     │  ├─ ShiftCreateViewModel          │
│  ├─ ScheduleViewPage    │  ├─ ScheduleViewViewModel         │
│  └─ (Existing pages)    │  └─ (Existing ViewModels)         │
├─────────────────────────────────────────────────────────────┤
│  Services                                                   │
│  ├─ EmployeeService (NEW)                                   │
│  ├─ ShiftService (NEW)                                      │
│  └─ IApiClient (EXISTING)                                   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    API (ASP.NET Core)                       │
├─────────────────────────────────────────────────────────────┤
│  Controllers (EXISTING)                                     │
│  ├─ EmployeeController    - Add role validation             │
│  ├─ ShiftAssignmentController - Add validation              │
│  ├─ WeeklyScheduleController - Already has publish          │
│  └─ UserController        - Fix error messages              │
├─────────────────────────────────────────────────────────────┤
│  Services                                                   │
│  ├─ ShiftValidationService (NEW)                            │
│  └─ ScheduleValidationService (EXISTING)                    │
└─────────────────────────────────────────────────────────────┘
```

## Components and Interfaces

### Mobile App - New Views

#### 1. EmployeeManagePage.xaml
```
┌────────────────────────────────┐
│ ← Çalışan Yönetimi         [+] │
├────────────────────────────────┤
│ ┌────────────────────────────┐ │
│ │ 👤 Ahmet Yılmaz      [✓]  │ │
│ │    Aktif                   │ │
│ └────────────────────────────┘ │
│ ┌────────────────────────────┐ │
│ │ 👤 Mehmet Demir      [✗]  │ │
│ │    Pasif                   │ │
│ └────────────────────────────┘ │
│ ...                            │
└────────────────────────────────┘
```

#### 2. EmployeeEditPage.xaml
```
┌────────────────────────────────┐
│ ← Çalışan Düzenle              │
├────────────────────────────────┤
│                                │
│ Ad Soyad:                      │
│ ┌────────────────────────────┐ │
│ │ Ahmet Yılmaz               │ │
│ └────────────────────────────┘ │
│                                │
│ Durum:                         │
│ ┌────────────────────────────┐ │
│ │ Aktif              [═══○] │ │
│ └────────────────────────────┘ │
│                                │
│ ┌────────────────────────────┐ │
│ │         KAYDET             │ │
│ └────────────────────────────┘ │
└────────────────────────────────┘
```

#### 3. ShiftCreatePage.xaml
```
┌────────────────────────────────┐
│ ← Vardiya Oluştur              │
├────────────────────────────────┤
│                                │
│ Çalışan:                       │
│ ┌────────────────────────────┐ │
│ │ Ahmet Yılmaz           ▼  │ │
│ └────────────────────────────┘ │
│                                │
│ Tarih:                         │
│ ┌────────────────────────────┐ │
│ │ 26 Aralık 2025         📅 │ │
│ └────────────────────────────┘ │
│                                │
│ Başlangıç:        Bitiş:       │
│ ┌──────────┐     ┌──────────┐  │
│ │ 09:00 ▼ │     │ 17:00 ▼ │  │
│ └──────────┘     └──────────┘  │
│                                │
│ ┌────────────────────────────┐ │
│ │         KAYDET             │ │
│ └────────────────────────────┘ │
│                                │
│ ┌────────────────────────────┐ │
│ │        YAYINLA             │ │
│ └────────────────────────────┘ │
└────────────────────────────────┘
```

#### 4. ScheduleViewPage.xaml
```
┌────────────────────────────────┐
│ ← Program                      │
├────────────────────────────────┤
│ [Günlük] [Haftalık]            │
├────────────────────────────────┤
│ 📅 26 Aralık 2025 - Perşembe   │
├────────────────────────────────┤
│ ┌────────────────────────────┐ │
│ │ Ahmet Yılmaz               │ │
│ │ 09:00 - 17:00  [Yayınlandı]│ │
│ └────────────────────────────┘ │
│ ┌────────────────────────────┐ │
│ │ Mehmet Demir               │ │
│ │ 14:00 - 22:00  [Taslak]    │ │
│ └────────────────────────────┘ │
├────────────────────────────────┤
│ 📅 27 Aralık 2025 - Cuma       │
├────────────────────────────────┤
│ ...                            │
└────────────────────────────────┘
```

### Mobile App - New Services

#### IEmployeeService.cs
```csharp
public interface IEmployeeService
{
    Task<List<Employee>> GetAllAsync();
    Task<List<Employee>> GetActiveAsync();
    Task<Employee> GetByIdAsync(int id);
    Task<Employee> CreateAsync(string name);
    Task UpdateAsync(int id, string name, bool isActive);
}
```

#### IShiftService.cs
```csharp
public interface IShiftService
{
    Task<List<ShiftView>> GetByDateRangeAsync(DateTime start, DateTime end);
    Task<ShiftAssignment> CreateAsync(int employeeId, DateTime date, TimeSpan start, TimeSpan end);
    Task PublishAsync(int scheduleId);
}
```

### API - Validation Service

#### IShiftValidationService.cs
```csharp
public interface IShiftValidationService
{
    Task<ValidationResult> ValidateShiftAsync(int employeeId, DateTime date, TimeSpan start, TimeSpan end);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
}
```

### AppShell Menu Updates

```xml
<!-- Existing items -->
<FlyoutItem Title="Çalışanlar" ... />

<!-- New items for Manager/Admin -->
<FlyoutItem Title="Çalışan Yönetimi" 
            IsVisible="{Binding IsManager}"
            Route="employeemanage" />
            
<FlyoutItem Title="Vardiya Oluştur" 
            IsVisible="{Binding IsManager}"
            Route="shiftcreate" />
            
<FlyoutItem Title="Program Görüntüle" 
            Route="scheduleview" />
```

## Data Models

### Mobile Models (Existing - No Changes)

```csharp
public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public int BusinessId { get; set; }
}
```

### New Mobile Models

```csharp
public class ShiftView
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Status { get; set; } // "Draft" or "Published"
}

public class CreateShiftRequest
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
```

### API DTOs (New)

```csharp
public class CreateEmployeeRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }
}

public class UpdateEmployeeRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }
    public bool IsActive { get; set; }
}

public class ShiftValidationError
{
    public string Code { get; set; }
    public string Message { get; set; }
}
```

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system-essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: New Employee Default Active Status
*For any* valid employee name submitted to the create endpoint, the created employee record SHALL have IsActive=true.
**Validates: Requirements 1.5**

### Property 2: Employee Update Round-Trip
*For any* employee and any valid new name/status, updating then fetching the employee SHALL return the updated values.
**Validates: Requirements 1.7, 1.8**

### Property 3: Non-Manager Access Denied
*For any* user without Manager or Admin role, accessing employee management endpoints SHALL return 403 Forbidden.
**Validates: Requirements 1.9**

### Property 4: API Error Message Propagation
*For any* API error response, the mobile app SHALL display the exact error message from the response body.
**Validates: Requirements 2.4**

### Property 5: Short Password Validation
*For any* password with fewer than 6 characters, the API SHALL return a validation error with message "Şifre en az 6 karakter olmalı".
**Validates: Requirements 2.7**

### Property 6: Active Employees Only in Picker
*For any* employee list displayed in the shift creation picker, all employees SHALL have IsActive=true.
**Validates: Requirements 3.4**

### Property 7: New Shift Draft Status
*For any* valid shift created through the API, the initial status SHALL be Draft.
**Validates: Requirements 3.5**

### Property 8: End Time After Start Time Validation
*For any* shift where EndTime <= StartTime, the API SHALL return a validation error.
**Validates: Requirements 3.7, 7.1**

### Property 9: Overlapping Shift Validation
*For any* shift that overlaps with an existing shift for the same employee on the same date, the API SHALL return an overlap error.
**Validates: Requirements 3.8, 7.2, 7.3**

### Property 10: Publish Status Change
*For any* draft shift that is published, the status SHALL change to Published.
**Validates: Requirements 4.2**

### Property 11: Publish Log Creation
*For any* successful publish operation, a PublishLog record SHALL be created with correct metadata.
**Validates: Requirements 4.3**

### Property 12: Validation Errors Before Publish
*For any* schedule with validation errors, publishing SHALL return all violations without changing status.
**Validates: Requirements 4.6**

### Property 13: Schedule View Required Fields
*For any* shift displayed in schedule view, the display SHALL include employee name, date, start time, end time, and status.
**Validates: Requirements 5.4**

### Property 14: Schedule Grouped By Date
*For any* list of shifts in schedule view, shifts SHALL be grouped by date with date headers.
**Validates: Requirements 5.8**

### Property 15: Role-Based Menu Visibility
*For any* user with Employee role, management menu items (Çalışan Yönetimi, Vardiya Oluştur) SHALL be hidden.
*For any* user with Manager role, management menu items SHALL be visible.
*For any* user with Admin role, all menu items including Kullanıcı Yönetimi SHALL be visible.
**Validates: Requirements 6.1, 6.2, 6.3, 6.4**

### Property 16: Unauthorized API Returns 403
*For any* protected API endpoint called without proper authorization, the response SHALL be 403 Forbidden.
**Validates: Requirements 6.5**

### Property 17: Multiple Validation Errors
*For any* request with multiple validation errors, the API SHALL return all errors in a single response.
**Validates: Requirements 7.5**

## Error Handling

### Mobile App Error Handling

```csharp
// Standard error handling pattern for all services
try
{
    var result = await _apiClient.PostAsync<T>(endpoint, data);
    return result;
}
catch (ApiException ex) when (ex.StatusCode == 400)
{
    // Validation error - show specific message
    await _toastService.ShowError(ex.Message);
    throw;
}
catch (ApiException ex) when (ex.StatusCode == 403)
{
    // Access denied
    await _toastService.ShowError("Bu işlem için yetkiniz yok");
    throw;
}
catch (HttpRequestException)
{
    await _toastService.ShowError("Bağlantı hatası");
    throw;
}
```

### API Error Responses

| Scenario | Status Code | Response Body |
|----------|-------------|---------------|
| Validation error | 400 | `{"errors": ["Bitiş saati başlangıçtan sonra olmalı"]}` |
| Unauthorized | 401 | `{"message": "Token geçersiz"}` |
| Forbidden | 403 | `{"message": "Bu işlem için yetkiniz yok"}` |
| Not found | 404 | `{"message": "Kayıt bulunamadı"}` |
| Duplicate | 409 | `{"message": "Kullanıcı adı zaten mevcut"}` |

## Testing Strategy

### Unit Tests
- ViewModel command execution
- Service method logic
- Validation logic
- Error handling paths

### Property-Based Tests (FsCheck)
- Shift time validation (Property 8)
- Overlap detection (Property 9)
- Role-based access (Property 15, 16)
- Multiple error aggregation (Property 17)

### Integration Tests
- API endpoint authorization
- Database round-trip operations
- Publish workflow

### UI Tests (Manual)
- Navigation flows
- Form submissions
- Error message display
- Menu visibility by role

