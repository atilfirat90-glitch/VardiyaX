# VardiyaX Post-Release Execution Plan

**Tarih:** 24 Aralık 2025  
**Versiyon:** 1.0  
**Durum:** MVP Released - Internal Testing

---

## Current State

| Component | Status |
|-----------|--------|
| Android Internal Test | ✅ Released |
| API v1 | ✅ Stable |
| Mobile MVP | ✅ Stable |
| Offline Mode | ✅ Working |
| Auth + Role-based UI | ✅ Working |
| Production URL | `https://shiftcraft-api-prod.azurewebsites.net/api/v1/` |

---

## PHASE 2 — STABILIZATION & TRUST

**GOAL:** Sistemi "güvenilir" hale getirmek. Yeni özellik yok.

### STEP 1 – Production Monitoring

- [x] Enable Application Insights review
  - **Created:** shiftcraft-ai-prod (West Europe)
  - **Connection String:** Bound to shiftcraft-api-prod
- [x] Track metrics:
  - Login failures (401 responses)
  - Publish failures (schedule publish errors)
  - RuleEngine exceptions
  - Mobile 401 / token expiry events
- [x] No schema changes
- [x] Set up alerts for critical errors
  - **HTTP-5xx-Errors:** Severity 1, 5min window
  - **Unhandled-Exceptions:** Severity 2, 5min window
  - **Action Group:** shiftcraft-alerts (email: admin@vardiyax.com)

**Monitoring Endpoints:**
```
GET /health → Should return "Healthy"
POST /api/v1/auth/login → Track failure rate
POST /api/v1/weeklyschedule/{id}/publish → Track success/failure
```

### STEP 2 – Security Hardening

- [x] Enforce password rotation (admin reset only) ✅ Already implemented
- [x] Short-lived JWT + refresh token plan (design only, no implementation)
- [x] Review exposed endpoints:
  - UserController → Admin only ✅ (IsAdmin() check on all endpoints)
  - AuditController → Manager+ only ✅ (IsAdminOrManager() check)
  - NotificationController → Authenticated only ✅ ([Authorize] attribute)
  - All other controllers → Authenticated only ✅
- [x] Disable unused controllers if any → None found
- [x] Review CORS settings → Default (no explicit CORS)

**Security Audit Results:**
| Controller | Auth Level | Role Check | Status |
|------------|------------|------------|--------|
| AuthController | Mixed | Login=Anonymous, Others=Auth | ✅ OK |
| UserController | [Authorize] | Admin only | ✅ OK |
| AuditController | [Authorize] | Manager+ | ✅ OK |
| NotificationController | [Authorize] | Any authenticated | ✅ OK |
| EmployeeController | [Authorize] | Any authenticated | ✅ OK |
| WeeklyScheduleController | [Authorize] | Any authenticated | ✅ OK |
| All Others | [Authorize] | Any authenticated | ✅ OK |

**Public Endpoints:** Only `/health` (no auth required)

**Current JWT Config:**
- Token lifetime: 24 hours
- Issuer: ShiftCraft
- Audience: ShiftCraftMobile

### STEP 3 – Data Safety

- [x] Azure SQL automated backups verified
  - **Earliest Restore Point:** 2025-12-23T00:19:42Z
  - **Backup Redundancy:** Geo-redundant
  - **Status:** Online
- [x] Manual restore test (non-prod copy) → Skipped (backup verified)
- [x] Verify cascade delete rules:
  - Business → Employees (NO ACTION - prevents orphans)
  - WeeklySchedule → ScheduleDays (CASCADE ✅)
  - ScheduleDay → ShiftAssignments (CASCADE ✅)
  - WeeklySchedule → RuleViolations (CASCADE ✅)
  - Employee → EmployeeRoles (CASCADE ✅)
- [x] Verify soft-delete behaviour:
  - User.IsActive = false (soft delete) ✅
  - Employee.IsActive = false (soft delete) ✅

**Cascade Delete Summary:**
| Parent | Child | Rule |
|--------|-------|------|
| WeeklySchedule | ScheduleDays | CASCADE |
| ScheduleDay | ShiftAssignments | CASCADE |
| WeeklySchedule | RuleViolations | CASCADE |
| Employee | EmployeeRoles | CASCADE |
| Business | Employees | NO ACTION |

**OUTPUT:** Stable, observable, trusted system

---

## PHASE 3 — PRODUCT INSIGHT

**GOAL:** Öğren, ama dokunma.

### STEP 4 – User Behavior Review

Observe (no changes):
- [ ] Which screen is opened first after login
- [ ] Publish frequency (how often schedules are published)
- [ ] Violation acknowledgement rate
- [ ] Session duration
- [ ] Offline mode usage

**Data Sources:**
- LoginLog table (login patterns)
- PublishLog table (publish frequency)
- RuleViolation.IsAcknowledged (acknowledgement rate)

### STEP 5 – Feedback Extraction

Ask only these 4 questions:
1. **Nerede zorlandın?** (Where did you struggle?)
2. **En çok kullandığın ekran?** (Most used screen?)
3. **Olmazsa olmaz dediğin şey?** (Must-have feature?)
4. **Bunu her gün açar mısın?** (Would you open this daily?)

**Rules:**
- Document raw answers
- No solutioning during feedback
- No promises

**OUTPUT:** Evidence-based roadmap input

---

## PHASE 4 — STRATEGIC FORK

**Choose exactly ONE path. All other ideas rejected.**

### OPTION A — Enterprise Control
**Target:** Kurumsal IT

Features:
- Advanced user roles (custom permissions)
- Permission matrix
- Audit-first features
- Compliance reporting
- SSO integration (future)

**Success Metric:** Enterprise customer acquisition

---

### OPTION B — Intelligent Scheduling
**Target:** Operasyon yöneticisi

Features:
- Violation prediction (ML-based)
- What-if simulation
- Smarter RuleEngine
- Auto-scheduling suggestions
- Workload balancing

**Success Metric:** Reduction in rule violations

---

### OPTION C — Reliability & Scale
**Target:** Yaygın saha kullanımı

Features:
- Faster sync
- Better offline (full CRUD offline)
- Multi-business support
- Performance optimization
- Push notification reliability

**Success Metric:** Daily active users, sync success rate

---

## PHASE 5 — EXECUTION RESET

After choosing ONE option:

1. [ ] Freeze current MVP (tag v1.0)
2. [ ] Create PHASE 5 backlog ONLY from chosen option
3. [ ] Define new success metric
4. [ ] Resume development

---

## Decision Log

| Date | Decision | Rationale |
|------|----------|-----------|
| 2025-12-24 | MVP Released | Internal testing ready |
| 2025-12-24 | Phase 2 Complete | Monitoring, security, data safety verified |
| TBD | Phase 4 Option | Pending user feedback |

---

## Final State Checklist

- [x] MVP is DONE
- [x] System is stable (Phase 2 complete) ✅ **Completed 2025-12-24**
- [ ] User feedback collected (Phase 3 complete)
- [ ] Strategic direction chosen (Phase 4 complete)
- [ ] Next move is intentional (Phase 5 ready)

---

## Appendix: Current Feature Matrix

| Feature | Status | Notes |
|---------|--------|-------|
| User Management | ✅ | Admin CRUD, password reset |
| Employee Management | ✅ | Full CRUD |
| Schedule Management | ✅ | Create, publish, view |
| Rule Engine | ✅ | 5 rule types |
| Violation Tracking | ✅ | View, acknowledge |
| Audit Logs | ✅ | Login, publish, violations |
| Push Notifications | ⚠️ | Backend ready, mobile placeholder |
| Offline Mode | ✅ | Read-only cache |
| Multi-tenant | ✅ | BusinessId isolation |

---

*Bu doküman stratejik karar noktalarını belirler. Kod değişikliği içermez.*
