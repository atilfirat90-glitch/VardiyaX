# VardiyaX Production Monitoring Checklist

**Phase 2 - Stabilization & Trust**

---

## Daily Checks

### Health
- [ ] `GET /health` returns "Healthy"
- [ ] Response time < 500ms

### Authentication
- [ ] Login success rate > 95%
- [ ] No unusual 401 spikes
- [ ] Token expiry handling working

### Core Functions
- [ ] Schedule publish working
- [ ] Rule engine executing without errors
- [ ] Violation detection accurate

---

## Weekly Checks

### Database
- [ ] Azure SQL backup status: OK
- [ ] Database size within limits
- [ ] No slow queries (> 5s)

### Security
- [ ] No unauthorized access attempts
- [ ] Admin-only endpoints protected
- [ ] No exposed sensitive data in logs

### Performance
- [ ] API response times acceptable
- [ ] No memory leaks
- [ ] CPU usage normal

---

## Metrics to Track

| Metric | Target | Current |
|--------|--------|---------|
| Health endpoint uptime | 99.9% | TBD |
| Login success rate | > 95% | TBD |
| Publish success rate | > 99% | TBD |
| Average API response | < 500ms | TBD |
| Daily active users | Baseline | TBD |

---

## Alert Thresholds

| Event | Threshold | Action |
|-------|-----------|--------|
| Health check fail | 3 consecutive | Investigate immediately |
| Login failure spike | > 10 in 5 min | Check for attack |
| 500 errors | > 5 in 1 hour | Review logs |
| Database connection fail | Any | Critical alert |

---

## Log Queries

### Login Failures (Last 24h)
```sql
SELECT Username, Timestamp, FailureReason 
FROM LoginLogs 
WHERE Action = 'FailedLogin' 
AND Timestamp > DATEADD(hour, -24, GETUTCDATE())
ORDER BY Timestamp DESC
```

### Publish Activity (Last 7 days)
```sql
SELECT PublisherUsername, COUNT(*) as PublishCount
FROM PublishLogs
WHERE Timestamp > DATEADD(day, -7, GETUTCDATE())
GROUP BY PublisherUsername
ORDER BY PublishCount DESC
```

### Unacknowledged Violations
```sql
SELECT COUNT(*) as UnacknowledgedCount
FROM RuleViolations
WHERE IsAcknowledged = 0
```

---

## Incident Response

1. **Detect** - Alert triggered or user report
2. **Assess** - Check health, logs, metrics
3. **Contain** - Isolate if needed
4. **Fix** - Apply hotfix or rollback
5. **Document** - Record in incident log

---

*Bu checklist Phase 2 tamamlanana kadar günlük/haftalık takip edilir.*
