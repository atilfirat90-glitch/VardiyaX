# ShiftCraft API Contract v1 (Mobile MVP)

## Base URL
`https://shiftcraft-api-prod.azurewebsites.net/api`

## Authentication
Currently: None (to be implemented)

---

## Core Endpoints

### Business
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /business | List all businesses |
| GET | /business/{id} | Get business by ID |
| POST | /business | Create business |
| PUT | /business/{id} | Update business |
| DELETE | /business/{id} | Delete business |

**Request (POST/PUT):**
```json
{
  "name": "string",
  "timezone": "string"
}
```

**Response:**
```json
{
  "id": 1,
  "name": "string",
  "timezone": "string"
}
```

---

### Employee
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /employee | List all employees |
| GET | /employee/{id} | Get employee by ID |
| GET | /employee/business/{businessId} | Get employees by business |
| POST | /employee | Create employee |
| PUT | /employee/{id} | Update employee |
| DELETE | /employee/{id} | Delete employee |

**Request (POST/PUT):**
```json
{
  "businessId": 1,
  "name": "string",
  "isCoreStaff": true,
  "weeklyMaxMinutes": 2400,
  "isActive": true
}
```

---

### Role
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /role | List all roles |
| GET | /role/{id} | Get role by ID |

**Response:**
```json
{
  "id": 1,
  "name": "Manager|Supervisor|Worker|Trainee"
}
```

---

### EmployeeRole
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /employeerole | List all employee-role assignments |
| POST | /employeerole | Assign role to employee |
| DELETE | /employeerole/{employeeId}/{roleId} | Remove role from employee |

**Request (POST):**
```json
{
  "employeeId": 1,
  "roleId": 1
}
```

---

### ShiftTemplate
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /shifttemplate | List all shift templates |
| GET | /shifttemplate/{id} | Get template by ID |
| GET | /shifttemplate/business/{businessId} | Get templates by business |
| POST | /shifttemplate | Create template |
| PUT | /shifttemplate/{id} | Update template |
| DELETE | /shifttemplate/{id} | Delete template |

**Request (POST/PUT):**
```json
{
  "businessId": 1,
  "name": "Morning Shift",
  "startTime": "08:00:00",
  "endTime": "16:00:00",
  "durationMinutes": 480
}
```

---

### WeeklySchedule
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /weeklyschedule | List all schedules |
| GET | /weeklyschedule/{id} | Get schedule by ID |
| GET | /weeklyschedule/business/{businessId} | Get schedules by business |
| POST | /weeklyschedule | Create schedule |
| POST | /weeklyschedule/{id}/publish | Publish schedule (validates rules) |
| DELETE | /weeklyschedule/{id} | Delete schedule |

**Request (POST):**
```json
{
  "businessId": 1,
  "weekStartDate": "2024-12-30",
  "status": 0
}
```

**Status Enum:**
- 0 = Draft
- 1 = Published

**Publish Response:**
```json
[
  {
    "id": 1,
    "weeklyScheduleId": 1,
    "employeeId": 3,
    "violationDate": "2024-12-30",
    "ruleCode": 1
  }
]
```

---

### ScheduleDay
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /scheduleday | List all schedule days |
| GET | /scheduleday/{id} | Get day by ID |
| POST | /scheduleday | Create schedule day |
| DELETE | /scheduleday/{id} | Delete schedule day |

**Request (POST):**
```json
{
  "weeklyScheduleId": 1,
  "date": "2024-12-30",
  "dayTypeId": 1
}
```

**DayType Enum:**
- 1 = Weekday
- 2 = Weekend
- 3 = Holiday

---

### ShiftAssignment
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /shiftassignment | List all assignments |
| GET | /shiftassignment/{id} | Get assignment by ID |
| POST | /shiftassignment | Create assignment |
| DELETE | /shiftassignment/{id} | Delete assignment |

**Request (POST):**
```json
{
  "scheduleDayId": 1,
  "employeeId": 1,
  "roleId": 1,
  "shiftTemplateId": 1,
  "source": 0
}
```

**Source Enum:**
- 0 = Manual
- 1 = Auto

---

### WorkRule
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /workrule | List all work rules |
| GET | /workrule/{id} | Get rule by ID |
| POST | /workrule | Create work rule |
| PUT | /workrule/{id} | Update work rule |
| DELETE | /workrule/{id} | Delete work rule |

**Request (POST/PUT):**
```json
{
  "businessId": 1,
  "isSevenDaysOpen": false,
  "maxDailyMinutes": 480,
  "minWeeklyOffDays": 1
}
```

---

### RuleViolation
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /ruleviolation | List all violations |
| GET | /ruleviolation/schedule/{scheduleId} | Get violations by schedule |

**RuleCode Enum:**
- 1 = MinWeeklyOffDays violation
- 2 = MaxDailyMinutes violation
- 3 = WeeklyMaxMinutes violation

---

### Health Check
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /health | Health check endpoint |

**Response:** `Healthy` (200 OK)

---

## Version History
| Version | Date | Changes |
|---------|------|---------|
| v1.0.0 | 2024-12-23 | Initial release - Mobile MVP |

## Breaking Change Policy
- v1 endpoints are frozen for mobile compatibility
- New features will be added as new endpoints
- Deprecation notice: 3 months before removal
