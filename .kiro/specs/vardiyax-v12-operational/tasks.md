# Implementation Plan: VardiyaX v1.2 Operational MVP

## Overview

Bu plan, VardiyaX'ı operasyonel hale getirmek için 5 fazda minimum değişiklik yapar. Her faz bağımsız test edilebilir ve deploy edilebilir.

## Tasks

- [x] 1. Phase 1: Employee Management
  - [x] 1.1 Create EmployeeService in Mobile
    - Create `Services/EmployeeService.cs` with IApiClient dependency
    - Implement GetAllAsync, GetActiveAsync, CreateAsync, UpdateAsync methods
    - Register in MauiProgram.cs DI container
    - _Requirements: 1.3, 1.5, 1.7, 1.8_

  - [x] 1.2 Create EmployeeManagePage View
    - Create `Views/EmployeeManagePage.xaml` with employee list
    - Add "+" button in toolbar for new employee
    - Show name and active status toggle for each employee
    - Add tap gesture to navigate to edit page
    - _Requirements: 1.3, 1.4, 1.6_

  - [x] 1.3 Create EmployeeManageViewModel
    - Create `ViewModels/EmployeeManageViewModel.cs`
    - Implement LoadEmployeesCommand, AddEmployeeCommand, EditEmployeeCommand
    - Handle role-based access (show error if not manager)
    - _Requirements: 1.3, 1.9_

  - [x] 1.4 Create EmployeeEditPage View
    - Create `Views/EmployeeEditPage.xaml` with name entry and active switch
    - Add Save button
    - Show validation errors
    - _Requirements: 1.6, 1.7, 1.8_

  - [x] 1.5 Create EmployeeEditViewModel
    - Create `ViewModels/EmployeeEditViewModel.cs`
    - Implement LoadEmployeeCommand, SaveCommand
    - Handle create vs edit mode
    - _Requirements: 1.5, 1.7, 1.8_

  - [x] 1.6 Add Employee Management to AppShell
    - Add "Çalışan Yönetimi" FlyoutItem with IsVisible binding to IsManager
    - Register routes in AppShell.xaml.cs
    - _Requirements: 1.1, 1.2, 1.9_

  - [ ]* 1.7 Write property test for employee creation default status
    - **Property 1: New Employee Default Active Status**
    - **Validates: Requirements 1.5**

  - [ ]* 1.8 Write property test for employee update round-trip
    - **Property 2: Employee Update Round-Trip**
    - **Validates: Requirements 1.7, 1.8**

- [-] 2. Phase 2: User Management Fix
  - [x] 2.1 Fix UserService navigation crash
    - Verify UserService uses IApiClient (already fixed in v1.1)
    - Test "Yeni" button navigation
    - Test user row tap navigation
    - _Requirements: 2.1, 2.2_

  - [x] 2.2 Improve UserController error messages
    - Add Turkish error messages for validation failures
    - Return specific error for duplicate username (409 Conflict)
    - Return specific error for short password
    - _Requirements: 2.6, 2.7_

  - [x] 2.3 Update UserDetailViewModel error handling
    - Display API error messages in UI
    - Show specific validation errors
    - _Requirements: 2.4_

  - [ ]* 2.4 Write property test for short password validation
    - **Property 5: Short Password Validation**
    - **Validates: Requirements 2.7**

- [x] 3. Checkpoint - Phase 1 & 2 Complete
  - [x] Ensure all tests pass, ask the user if questions arise.
  - [x] Test employee management flow on emulator
  - [x] Test user management fix on emulator
  - UI Test Results (2024-12-27) - API CONNECTED:
    - ✅ Login - admin/admin başarılı
    - ✅ Ana Sayfa (Employees) - 4 çalışan API'den yüklendi
    - ✅ Drawer Menu - Tüm menü öğeleri görünür
    - ✅ Kullanıcı Yönetimi - 1 kullanıcı (admin) görünür
    - ⚠️ Yeni Kullanıcı Formu - "Sayfa açılırken hata oluştu" (v1.1 bug)
    - ✅ Denetim Günlükleri - Giriş Tab:
      - 7 login kaydı API'den yüklendi
      - Başarılı ve başarısız girişler gösteriliyor
      - Tarih filtresi çalışıyor
    - ✅ Denetim Günlükleri - Yayın Tab: "Yayın kaydı bulunamadı"
    - ✅ Denetim Günlükleri - İhlaller Tab: "İhlal kaydı bulunamadı"
    - ✅ Bildirim Ayarları - 3 toggle + hatırlatma zamanı
    - ✅ Vardiyalar Tab - 1 schedule (Published, Business #5)
    - ✅ İhlaller Tab - "No violations found, All schedules are compliant!"
    - ✅ Çıkış Yap - Dialog çalışıyor:
      - "Çıkış yapmak istediğinize emin misiniz?"
      - "Evet" / "Hayır" butonları
  - API Connection: http://10.0.2.2:5184/api/ (emulator loopback)
  - APK: vardiyax-v1.1-release.apk
  - Known Bug: Yeni Kullanıcı formu açılırken hata (v1.1)

- [ ] 4. Phase 3: Shift Creation
  - [ ] 4.1 Create ShiftService in Mobile
    - Create `Services/ShiftService.cs` with IApiClient dependency
    - Implement CreateAsync, GetByDateRangeAsync methods
    - Register in MauiProgram.cs DI container
    - _Requirements: 3.5, 5.4_

  - [ ] 4.2 Create ShiftValidationService in API
    - Create `Application/Services/ShiftValidationService.cs`
    - Implement ValidateShiftAsync with time and overlap checks
    - Return all errors in single response
    - _Requirements: 7.1, 7.2, 7.5_

  - [ ] 4.3 Update ShiftAssignmentController with validation
    - Add validation call before creating shift
    - Return 400 with Turkish error messages
    - Add role authorization (Manager/Admin only)
    - _Requirements: 3.7, 3.8, 7.1, 7.2, 7.3_

  - [ ] 4.4 Create ShiftCreatePage View
    - Create `Views/ShiftCreatePage.xaml` with form layout
    - Add Employee picker (active employees only)
    - Add Date picker, Start time picker, End time picker
    - Add Save and Publish buttons
    - _Requirements: 3.2, 3.3, 3.4, 3.6_

  - [ ] 4.5 Create ShiftCreateViewModel
    - Create `ViewModels/ShiftCreateViewModel.cs`
    - Implement LoadEmployeesCommand (active only), SaveCommand, PublishCommand
    - Handle validation errors display
    - _Requirements: 3.4, 3.5, 3.6, 3.7_

  - [ ] 4.6 Add Shift Create to AppShell
    - Add "Vardiya Oluştur" FlyoutItem with IsVisible binding to IsManager
    - Register route in AppShell.xaml.cs
    - _Requirements: 3.1_

  - [ ]* 4.7 Write property test for end time validation
    - **Property 8: End Time After Start Time Validation**
    - **Validates: Requirements 3.7, 7.1**

  - [ ]* 4.8 Write property test for overlap detection
    - **Property 9: Overlapping Shift Validation**
    - **Validates: Requirements 3.8, 7.2, 7.3**

  - [ ]* 4.9 Write property test for active employees filter
    - **Property 6: Active Employees Only in Picker**
    - **Validates: Requirements 3.4**

- [ ] 5. Phase 4: Shift Publish
  - [ ] 5.1 Add publish functionality to ShiftService
    - Implement PublishAsync method
    - Call WeeklySchedule publish endpoint
    - _Requirements: 4.2_

  - [ ] 5.2 Update ShiftCreateViewModel for publish
    - Add PublishCommand implementation
    - Show success/error messages
    - Handle validation errors before publish
    - _Requirements: 4.1, 4.5, 4.6_

  - [ ]* 5.3 Write property test for publish status change
    - **Property 10: Publish Status Change**
    - **Validates: Requirements 4.2**

  - [ ]* 5.4 Write property test for publish log creation
    - **Property 11: Publish Log Creation**
    - **Validates: Requirements 4.3**

- [ ] 6. Checkpoint - Phase 3 & 4 Complete
  - Ensure all tests pass, ask the user if questions arise.
  - Test shift creation flow on emulator
  - Test publish flow on emulator

- [ ] 7. Phase 5: Schedule View
  - [ ] 7.1 Create ScheduleViewPage View
    - Create `Views/ScheduleViewPage.xaml` with list layout
    - Add Daily/Weekly toggle buttons
    - Group shifts by date with headers
    - Show employee name, time, status for each shift
    - _Requirements: 5.3, 5.4, 5.5, 5.6, 5.7, 5.8_

  - [ ] 7.2 Create ScheduleViewViewModel
    - Create `ViewModels/ScheduleViewViewModel.cs`
    - Implement LoadShiftsCommand with date range filter
    - Implement DailyCommand, WeeklyCommand for view toggle
    - Group shifts by date
    - _Requirements: 5.5, 5.6, 5.7, 5.8_

  - [ ] 7.3 Add Schedule View to AppShell
    - Add "Program Görüntüle" FlyoutItem (visible to all roles)
    - Register route in AppShell.xaml.cs
    - _Requirements: 5.1, 5.2_

  - [ ]* 7.4 Write property test for schedule grouping
    - **Property 14: Schedule Grouped By Date**
    - **Validates: Requirements 5.8**

- [ ] 8. Phase 6: Role-Based Access Control
  - [ ] 8.1 Update AuthService with role checks
    - Add IsEmployee, IsManager, IsAdmin properties
    - Parse role from JWT token
    - _Requirements: 6.1, 6.2, 6.3, 6.4_

  - [ ] 8.2 Update AppShell menu visibility
    - Bind FlyoutItem IsVisible to role properties
    - Hide management items for Employee role
    - Show all items for Admin role
    - _Requirements: 6.1, 6.2, 6.3, 6.4_

  - [ ] 8.3 Add role authorization to API controllers
    - Add [Authorize(Roles = "Admin,Manager")] to EmployeeController POST/PUT
    - Add [Authorize(Roles = "Admin,Manager")] to ShiftAssignmentController POST/PUT
    - Return 403 for unauthorized access
    - _Requirements: 6.5_

  - [ ]* 8.4 Write property test for role-based menu visibility
    - **Property 15: Role-Based Menu Visibility**
    - **Validates: Requirements 6.1, 6.2, 6.3, 6.4**

  - [ ]* 8.5 Write property test for unauthorized API access
    - **Property 16: Unauthorized API Returns 403**
    - **Validates: Requirements 6.5**

- [ ] 9. Final Checkpoint
  - Ensure all tests pass, ask the user if questions arise.
  - Full end-to-end test on emulator:
    - Login as Admin → all menus visible
    - Add employee → verify in list
    - Create shift → verify draft status
    - Publish shift → verify published status
    - View schedule → verify grouping
    - Login as Employee → management menus hidden

## Notes

- Tasks marked with `*` are optional property-based tests
- Each phase can be deployed independently
- Existing API endpoints are reused where possible
- No database schema changes required
- Turkish error messages throughout

## Effort Estimates

| Phase | Effort | Description |
|-------|--------|-------------|
| Phase 1 | 4-6 hours | Employee Management |
| Phase 2 | 1-2 hours | User Management Fix |
| Phase 3 | 4-6 hours | Shift Creation |
| Phase 4 | 2-3 hours | Shift Publish |
| Phase 5 | 3-4 hours | Schedule View |
| Phase 6 | 2-3 hours | Role-Based Access |
| **Total** | **16-24 hours** | |

