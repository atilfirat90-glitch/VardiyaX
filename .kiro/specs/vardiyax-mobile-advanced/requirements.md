# Requirements Document

## Introduction

VardiyaX mobil uygulaması için gelişmiş özellikler: kullanıcı yönetimi, denetim günlükleri, push bildirimleri ve çevrimdışı okuma modu. Bu özellikler, mobil uygulamanın işlevselliğini genişleterek yöneticilere tam kontrol ve kullanıcılara kesintisiz deneyim sağlar.

## Glossary

- **System**: VardiyaX mobil uygulaması
- **Admin**: Sistem yöneticisi rolündeki kullanıcı
- **Manager**: Vardiya yöneticisi rolündeki kullanıcı
- **Worker**: Çalışan rolündeki kullanıcı
- **User**: Sisteme giriş yapabilen herhangi bir kullanıcı
- **Audit_Log**: Sistem aktivitelerinin kaydı
- **Push_Notification**: Mobil cihaza gönderilen bildirim
- **Offline_Cache**: Çevrimdışı erişim için yerel depolama

## Requirements

### Requirement 1: Kullanıcı Yönetimi

**User Story:** Admin olarak, kullanıcıları yönetmek istiyorum, böylece sisteme erişimi kontrol edebilirim.

#### Acceptance Criteria

1. WHEN an admin accesses user management, THE System SHALL display a list of all users with username, role, and status
2. WHEN an admin creates a new user, THE System SHALL require username, password, and role selection
3. WHEN an admin creates a user, THE System SHALL validate username uniqueness
4. WHEN an admin updates a user role, THE System SHALL immediately apply the new permissions
5. WHEN an admin deactivates a user, THE System SHALL prevent that user from logging in
6. WHEN a non-admin user attempts to access user management, THE System SHALL deny access and show an error message
7. THE System SHALL support roles: Admin, Manager, Worker, Trainee

### Requirement 2: Şifre Sıfırlama

**User Story:** Admin olarak, kullanıcı şifrelerini sıfırlamak istiyorum, böylece şifresini unutan kullanıcılara yardımcı olabilirim.

#### Acceptance Criteria

1. WHEN an admin initiates password reset, THE System SHALL generate a temporary password
2. WHEN a password is reset, THE System SHALL require the user to change password on next login
3. WHEN setting a new password, THE System SHALL enforce minimum 8 characters with at least one number
4. IF a password reset fails, THEN THE System SHALL display an appropriate error message
5. THE System SHALL log all password reset actions

### Requirement 3: Giriş/Çıkış Günlükleri

**User Story:** Admin olarak, kullanıcı giriş/çıkış aktivitelerini görmek istiyorum, böylece güvenlik denetimi yapabilirim.

#### Acceptance Criteria

1. WHEN a user logs in successfully, THE System SHALL record timestamp, username, and device info
2. WHEN a user logs out, THE System SHALL record the logout timestamp
3. WHEN a login attempt fails, THE System SHALL record the failed attempt with reason
4. WHEN an admin views login logs, THE System SHALL display logs sorted by timestamp descending
5. THE System SHALL retain login logs for at least 90 days
6. THE System SHALL support filtering logs by username and date range

### Requirement 4: Program Yayınlama Günlükleri

**User Story:** Manager olarak, program yayınlama geçmişini görmek istiyorum, böylece değişiklikleri takip edebilirim.

#### Acceptance Criteria

1. WHEN a schedule is published, THE System SHALL record publisher username, timestamp, and schedule week
2. WHEN viewing publish logs, THE System SHALL display the affected employee count
3. THE System SHALL support filtering publish logs by date range and publisher
4. WHEN a schedule is republished, THE System SHALL record it as a separate entry

### Requirement 5: Kural İhlali Geçmişi

**User Story:** Manager olarak, kural ihlali geçmişini görmek istiyorum, böylece tekrarlayan sorunları tespit edebilirim.

#### Acceptance Criteria

1. WHEN viewing violation history, THE System SHALL display all violations with date, employee, and rule type
2. THE System SHALL support filtering violations by employee, rule type, and date range
3. THE System SHALL display violation trends (count per week/month)
4. WHEN a violation is resolved, THE System SHALL allow marking it as acknowledged

### Requirement 6: Program Yayınlama Bildirimi

**User Story:** Worker olarak, yeni program yayınlandığında bildirim almak istiyorum, böylece vardiyalarımı zamanında görebilirim.

#### Acceptance Criteria

1. WHEN a schedule is published, THE System SHALL send push notification to all affected employees
2. THE System SHALL include week start date in the notification
3. WHEN a user taps the notification, THE System SHALL navigate to the schedule view
4. THE System SHALL allow users to enable/disable schedule notifications

### Requirement 7: Kural İhlali Bildirimi

**User Story:** Manager olarak, kural ihlali tespit edildiğinde bildirim almak istiyorum, böylece hızlı aksiyon alabilirim.

#### Acceptance Criteria

1. WHEN a rule violation is detected, THE System SHALL send push notification to managers
2. THE System SHALL include violation type and affected employee in the notification
3. WHEN a user taps the notification, THE System SHALL navigate to the violations view
4. THE System SHALL allow managers to enable/disable violation notifications

### Requirement 8: Vardiya Hatırlatma Bildirimi

**User Story:** Worker olarak, yaklaşan vardiyam için hatırlatma almak istiyorum, böylece işe geç kalmam.

#### Acceptance Criteria

1. THE System SHALL send reminder notification 24 hours before shift start
2. THE System SHALL send reminder notification 1 hour before shift start
3. THE System SHALL include shift time and location in the notification
4. THE System SHALL allow users to configure reminder timing preferences
5. IF a shift is cancelled, THEN THE System SHALL not send reminder notifications

### Requirement 9: Çevrimdışı Program Önbelleği

**User Story:** Worker olarak, internet bağlantısı olmadan programımı görmek istiyorum, böylece her zaman vardiyalarımı kontrol edebilirim.

#### Acceptance Criteria

1. WHEN a user views schedules online, THE System SHALL cache the data locally
2. WHEN the device is offline, THE System SHALL display cached schedule data
3. THE System SHALL display a clear offline indicator when showing cached data
4. THE System SHALL cache at least the current and next week's schedules
5. WHEN the device comes online, THE System SHALL automatically refresh cached data

### Requirement 10: Çevrimdışı Okuma Modu

**User Story:** User olarak, çevrimdışıyken uygulamayı kullanmak istiyorum, böylece temel bilgilere erişebilirim.

#### Acceptance Criteria

1. WHILE offline, THE System SHALL allow viewing cached schedules in read-only mode
2. WHILE offline, THE System SHALL disable all write operations (create, update, delete)
3. WHILE offline, THE System SHALL display last sync timestamp
4. WHEN attempting a write operation offline, THE System SHALL disable action buttons and show non-blocking toast "Çevrimdışı - Bu işlem internet bağlantısı gerektirir"
5. WHILE offline, THE System SHALL NOT show modal alerts for write attempts
6. WHEN connectivity is restored, THE System SHALL automatically sync and enable write operations

### Requirement 11: Multi-Tenant Veri İzolasyonu

**User Story:** Sistem yöneticisi olarak, işletmeler arası veri izolasyonunu sağlamak istiyorum, böylece güvenlik ve gizlilik korunur.

#### Acceptance Criteria

1. THE System SHALL scope all user queries by BusinessId
2. THE System SHALL scope all schedule queries by BusinessId
3. THE System SHALL scope all audit log queries by BusinessId
4. THE System SHALL scope all violation queries by BusinessId
5. WHEN a user attempts to access data from another business, THE System SHALL return 403 Forbidden
6. THE System SHALL validate BusinessId on all create and update operations
