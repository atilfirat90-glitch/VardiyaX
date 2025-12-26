# Requirements Document

## Introduction

VardiyaX v1.2 Operational MVP - Kafe günlük operasyonlarını yönetmek için minimum gerekli özellikler. Bu sürüm mevcut READ-ONLY uygulamayı operasyonel hale getirir. Yeniden tasarım veya gelişmiş özellikler içermez.

## Glossary

- **Mobile_App**: VardiyaX .NET MAUI mobil uygulaması
- **API**: ShiftCraft.Api backend servisi
- **Employee**: Vardiya atanabilecek çalışan kaydı
- **User**: Sisteme giriş yapabilen kullanıcı hesabı
- **Shift**: Belirli bir çalışana atanan vardiya (tarih, başlangıç/bitiş saati)
- **Schedule**: Haftalık veya günlük vardiya planı
- **Draft**: Henüz yayınlanmamış taslak vardiya
- **Published**: Çalışanlara bildirilmiş yayınlanmış vardiya
- **Admin**: Tam yetkili sistem yöneticisi
- **Manager**: Vardiya ve çalışan yönetimi yetkili kullanıcı
- **Employee_User**: Sadece kendi vardiyalarını görebilen kullanıcı

## Requirements

### Requirement 1: Employee Management Screen

**User Story:** As a manager, I want to manage employees from the mobile app, so that I can add new staff and update their status without accessing the backend directly.

#### Acceptance Criteria

1. WHEN a manager opens the hamburger menu, THE Mobile_App SHALL display an "Çalışan Yönetimi" menu item
2. WHEN a manager taps "Çalışan Yönetimi", THE Mobile_App SHALL navigate to the Employee Management screen
3. WHEN the Employee Management screen loads, THE Mobile_App SHALL display a list of all employees with name and active status
4. WHEN a manager taps the "+" button, THE Mobile_App SHALL display an Add Employee form
5. WHEN a manager submits a valid employee name, THE API SHALL create a new employee record with IsActive=true
6. WHEN a manager taps an employee row, THE Mobile_App SHALL navigate to the Employee Edit screen
7. WHEN a manager updates employee name and saves, THE API SHALL update the employee record
8. WHEN a manager toggles the active switch, THE API SHALL update the employee IsActive status
9. IF a non-manager user accesses Employee Management, THEN THE Mobile_App SHALL display an access denied message
10. THE Mobile_App SHALL NOT provide a delete employee function (only deactivate)

### Requirement 2: User Management Fix

**User Story:** As an admin, I want the User Management screens to work correctly, so that I can create and manage user accounts.

#### Acceptance Criteria

1. WHEN an admin taps "Yeni" button on Users page, THE Mobile_App SHALL navigate to the New User form without crashing
2. WHEN an admin taps a user row, THE Mobile_App SHALL navigate to User Detail page without crashing
3. WHEN creating a new user, THE Mobile_App SHALL display fields for username, password, and role selection
4. WHEN a user creation fails, THE Mobile_App SHALL display the specific error message from API
5. WHEN an admin toggles user active status, THE API SHALL update the user IsActive field
6. IF username already exists, THEN THE API SHALL return a clear "Kullanıcı adı zaten mevcut" error
7. IF password is too short, THEN THE API SHALL return a clear "Şifre en az 6 karakter olmalı" error

### Requirement 3: Shift Creation Form

**User Story:** As a manager, I want to create shifts using a simple form, so that I can assign work hours to employees.

#### Acceptance Criteria

1. WHEN a manager opens the hamburger menu, THE Mobile_App SHALL display a "Vardiya Oluştur" menu item
2. WHEN a manager taps "Vardiya Oluştur", THE Mobile_App SHALL display a shift creation form
3. THE Shift_Form SHALL contain: Employee picker, Date picker, Start time picker, End time picker
4. WHEN a manager selects an employee, THE Mobile_App SHALL filter to show only active employees
5. WHEN a manager submits a valid shift, THE API SHALL create a ShiftAssignment with Draft status
6. WHEN a manager taps "Kaydet", THE Mobile_App SHALL save the shift and show success message
7. IF end time is before or equal to start time, THEN THE Mobile_App SHALL display "Bitiş saati başlangıçtan sonra olmalı" error
8. IF the employee already has a shift overlapping the selected time, THEN THE API SHALL return an overlap error

### Requirement 4: Shift Publish

**User Story:** As a manager, I want to publish shifts, so that employees are notified of their work schedule.

#### Acceptance Criteria

1. WHEN viewing draft shifts, THE Mobile_App SHALL display a "Yayınla" button
2. WHEN a manager taps "Yayınla", THE API SHALL update shift status to Published
3. WHEN a shift is published, THE API SHALL create a PublishLog record
4. WHEN a shift is published, THE API SHALL trigger push notifications to affected employees
5. WHEN publish succeeds, THE Mobile_App SHALL display "Vardiya yayınlandı" success message
6. IF validation errors exist, THEN THE API SHALL return the list of violations before publishing

### Requirement 5: Schedule View Screen

**User Story:** As a user, I want to view the schedule, so that I can see who is working when.

#### Acceptance Criteria

1. WHEN a user opens the hamburger menu, THE Mobile_App SHALL display a "Program Görüntüle" menu item
2. WHEN a user taps "Program Görüntüle", THE Mobile_App SHALL display the Schedule View screen
3. THE Schedule_View SHALL display shifts in a list format (not calendar grid)
4. THE Schedule_View SHALL show: Employee name, Date, Start time, End time, Status (Draft/Published)
5. WHEN the screen loads, THE Mobile_App SHALL default to showing today's shifts
6. WHEN a user taps "Haftalık", THE Mobile_App SHALL show the current week's shifts
7. WHEN a user taps "Günlük", THE Mobile_App SHALL show only today's shifts
8. THE Schedule_View SHALL group shifts by date with date headers

### Requirement 6: Role-Based Access Control

**User Story:** As a system administrator, I want role-based access enforced, so that users can only perform authorized actions.

#### Acceptance Criteria

1. WHEN an Employee_User logs in, THE Mobile_App SHALL hide "Çalışan Yönetimi" menu item
2. WHEN an Employee_User logs in, THE Mobile_App SHALL hide "Vardiya Oluştur" menu item
3. WHEN a Manager logs in, THE Mobile_App SHALL show all management menu items
4. WHEN an Admin logs in, THE Mobile_App SHALL show all menu items including "Kullanıcı Yönetimi"
5. IF an unauthorized user calls a protected API endpoint, THEN THE API SHALL return 403 Forbidden
6. THE API SHALL validate user role from JWT token on every protected request

### Requirement 7: Shift Validation

**User Story:** As a manager, I want shift validation, so that I don't create invalid or conflicting schedules.

#### Acceptance Criteria

1. WHEN creating a shift, THE API SHALL validate that end time is after start time
2. WHEN creating a shift, THE API SHALL check for overlapping shifts for the same employee
3. IF a shift overlaps with an existing shift, THEN THE API SHALL return "Bu çalışanın bu saatlerde başka vardiyası var" error
4. WHEN publishing shifts, THE API SHALL run all validation rules
5. THE API SHALL return all validation errors in a single response (not fail on first error)

