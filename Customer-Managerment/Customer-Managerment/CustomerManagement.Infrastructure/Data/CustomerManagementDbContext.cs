using System;
using System.Collections.Generic;
using Customer_Managerment.CustomerManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data;

public class CustomerManagementDbContext : DbContext
{
    public CustomerManagementDbContext(DbContextOptions<CustomerManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Person> Persons => Set<Person>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Deal> Deals => Set<Deal>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<NoteMention> NoteMentions => Set<NoteMention>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<StaffActivityLog> StaffActivityLogs => Set<StaffActivityLog>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<CalendarEvent> CalendarEvents => Set<CalendarEvent>();
    public DbSet<EventParticipant> EventParticipants => Set<EventParticipant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Person entity with TPH (Table Per Hierarchy)
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("person_pkey");

            entity.ToTable("persons");

            entity.HasIndex(e => e.Email, "persons_email_key").IsUnique();
            entity.HasIndex(e => e.Username, "persons_username_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Fullname)
                .HasMaxLength(200)
                .HasColumnName("fullname");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Location)
                .HasMaxLength(500)
                .HasColumnName("location");
            entity.Property(e => e.Discriminator)
                .HasMaxLength(50)
                .HasColumnName("discriminator");

            // Staff-specific properties
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(500)
                .HasColumnName("password_hash");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Salary)
                .HasPrecision(15, 2)
                .HasColumnName("salary");
            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");
            entity.Property(e => e.LastActiveAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_active_at");

            // Lead-specific properties
            entity.Property(e => e.Resource)
                .HasMaxLength(500)
                .HasColumnName("resource");

            // Configure query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure Contact entity
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.IdContact).HasName("contact_pkey");

            entity.ToTable("contacts");

            entity.Property(e => e.IdContact)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_contact");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Content)
                .HasColumnName("content");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.IdLead).HasColumnName("id_lead");

            entity.HasOne(d => d.IdStaffNavigation)
                .WithMany(p => p.Contacts)
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_contact_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdLeadNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdLead)
                .HasConstraintName("fk_contact_lead")
                .OnDelete(DeleteBehavior.Restrict);

            // Query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure Deal entity
        modelBuilder.Entity<Deal>(entity =>
        {
            entity.HasKey(e => e.IdDeal).HasName("deal_pkey");

            entity.ToTable("deals");

            entity.Property(e => e.IdDeal)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_deal");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Content)
                .HasColumnName("content");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasColumnName("price");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.IdCustomer).HasColumnName("id_customer");

            entity.HasOne(d => d.IdStaffNavigation)
                .WithMany(p => p.Deals)
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_deal_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.IdCustomerNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdCustomer)
                .HasConstraintName("fk_deal_customer")
                .OnDelete(DeleteBehavior.Restrict);

            // Query filter for soft delete
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure TaskEntity
        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.HasKey(e => e.IdTask).HasName("task_pkey");

            entity.ToTable("tasks");

            entity.Property(e => e.IdTask)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_task");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.Description)
                .HasColumnName("description");
            entity.Property(e => e.DueDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("due_date");
            entity.Property(e => e.Priority)
                .HasColumnName("priority");
            entity.Property(e => e.Status)
                .HasColumnName("status");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            entity.Property(e => e.IdStaffAssigned).HasColumnName("id_staff_assigned");
            entity.Property(e => e.LinkedEntityType)
                .HasMaxLength(50)
                .HasColumnName("linked_entity_type");
            entity.Property(e => e.LinkedEntityId).HasColumnName("linked_entity_id");

            entity.HasOne(d => d.IdStaffAssignedNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdStaffAssigned)
                .HasConstraintName("fk_task_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IdStaffAssigned, "ix_tasks_id_staff_assigned");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure Note
        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.IdNote).HasName("note_pkey");

            entity.ToTable("notes");

            entity.Property(e => e.IdNote)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_note");
            entity.Property(e => e.Content)
                .HasColumnName("content");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.IsPinned)
                .HasDefaultValue(false)
                .HasColumnName("is_pinned");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.DeletedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("deleted_at");

            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.LinkedEntityType)
                .HasMaxLength(50)
                .HasColumnName("linked_entity_type");
            entity.Property(e => e.LinkedEntityId).HasColumnName("linked_entity_id");
            entity.Property(e => e.ParentNoteId).HasColumnName("parent_note_id");

            entity.HasOne(d => d.IdStaffNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_note_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.ParentNote)
                .WithMany(d => d.Replies)
                .HasForeignKey(d => d.ParentNoteId)
                .HasConstraintName("fk_note_parent")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IdStaff, "ix_notes_id_staff");
            entity.HasIndex(e => new { e.LinkedEntityType, e.LinkedEntityId }, "ix_notes_entity");

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure NoteMention
        modelBuilder.Entity<NoteMention>(entity =>
        {
            entity.HasKey(e => e.IdMention).HasName("note_mention_pkey");

            entity.ToTable("note_mentions");

            entity.Property(e => e.IdMention)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_mention");
            entity.Property(e => e.IdNote).HasColumnName("id_note");
            entity.Property(e => e.IdStaffMentioned).HasColumnName("id_staff_mentioned");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.HasOne(d => d.IdNoteNavigation)
                .WithMany(d => d.Mentions)
                .HasForeignKey(d => d.IdNote)
                .HasConstraintName("fk_note_mention_note")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdStaffMentionedNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdStaffMentioned)
                .HasConstraintName("fk_note_mention_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IdNote, "ix_note_mentions_id_note");
            entity.HasIndex(e => e.IdStaffMentioned, "ix_note_mentions_id_staff_mentioned");
        });

        // Configure Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.IdNotification).HasName("notification_pkey");

            entity.ToTable("notifications");

            entity.Property(e => e.IdNotification)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_notification");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.Message)
                .HasColumnName("message");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("is_read");
            entity.Property(e => e.IsPinned)
                .HasDefaultValue(false)
                .HasColumnName("is_pinned");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.RelatedEntityType)
                .HasMaxLength(50)
                .HasColumnName("related_entity_type");
            entity.Property(e => e.RelatedEntityId).HasColumnName("related_entity_id");

            entity.HasOne(d => d.IdStaffNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_notification_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IdStaff, "ix_notifications_id_staff");
            entity.HasIndex(e => new { e.IdStaff, e.IsRead }, "ix_notifications_id_staff_is_read");
        });

        // Configure StaffActivityLog
        modelBuilder.Entity<StaffActivityLog>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("staff_activity_log_pkey");

            entity.ToTable("staff_activity_logs");

            entity.Property(e => e.IdLog)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_log");
            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ip_address");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(500)
                .HasColumnName("user_agent");

            entity.HasOne(d => d.IdStaffNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_staff_activity_log_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IdStaff, "ix_staff_activity_logs_id_staff");
            entity.HasIndex(e => e.Timestamp, "ix_staff_activity_logs_timestamp");
        });

        // Configure TeamMember
        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("team_member_pkey");

            entity.ToTable("team_members");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("assigned_at");
            entity.Property(e => e.AssignedBy)
                .HasMaxLength(100)
                .HasColumnName("assigned_by");
            entity.Property(e => e.CanEdit)
                .HasDefaultValue(false)
                .HasColumnName("can_edit");
            entity.Property(e => e.CanDelete)
                .HasDefaultValue(false)
                .HasColumnName("can_delete");

            entity.HasOne(d => d.IdStaffNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_team_member_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.EntityType, e.EntityId }, "ix_team_members_entity");
            entity.HasIndex(e => e.IdStaff, "ix_team_members_id_staff");
        });

        // Configure AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.IdLog).HasName("audit_log_pkey");

            entity.ToTable("audit_logs");

            entity.Property(e => e.IdLog)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_log");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.EntityType)
                .HasMaxLength(50)
                .HasColumnName("entity_type");
            entity.Property(e => e.EntityId).HasColumnName("entity_id");
            entity.Property(e => e.OldValues)
                .HasColumnType("jsonb")
                .HasColumnName("old_values");
            entity.Property(e => e.NewValues)
                .HasColumnType("jsonb")
                .HasColumnName("new_values");
            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.StaffName)
                .HasMaxLength(200)
                .HasColumnName("staff_name");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ip_address");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(500)
                .HasColumnName("user_agent");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");

            entity.HasOne(d => d.IdStaffNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_audit_log_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.EntityType, "ix_audit_logs_entity_type");
            entity.HasIndex(e => e.EntityId, "ix_audit_logs_entity_id");
            entity.HasIndex(e => e.Timestamp, "ix_audit_logs_timestamp");
            entity.HasIndex(e => e.IdStaff, "ix_audit_logs_id_staff");
        });

        // Configure CalendarEvent
        modelBuilder.Entity<CalendarEvent>(entity =>
        {
            entity.HasKey(e => e.IdEvent).HasName("calendar_event_pkey");

            entity.ToTable("calendar_events");

            entity.Property(e => e.IdEvent)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_event");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");
            entity.Property(e => e.Description)
                .HasColumnName("description");
            entity.Property(e => e.EventType)
                .HasMaxLength(50)
                .HasColumnName("event_type");
            entity.Property(e => e.StartTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("start_time");
            entity.Property(e => e.EndTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("end_time");
            entity.Property(e => e.Location)
                .HasMaxLength(500)
                .HasColumnName("location");
            entity.Property(e => e.IsAllDay)
                .HasDefaultValue(false)
                .HasColumnName("is_all_day");
            entity.Property(e => e.ReminderMinutes)
                .HasColumnName("reminder_minutes");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.RelatedEntityType)
                .HasMaxLength(50)
                .HasColumnName("related_entity_type");
            entity.Property(e => e.RelatedEntityId).HasColumnName("related_entity_id");

            entity.HasOne(d => d.IdStaffNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_calendar_event_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IdStaff, "ix_calendar_events_id_staff");
            entity.HasIndex(e => e.StartTime, "ix_calendar_events_start_time");
            entity.HasIndex(e => new { e.IdStaff, e.StartTime }, "ix_calendar_events_staff_start");
        });

        // Configure EventParticipant
        modelBuilder.Entity<EventParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("event_participant_pkey");

            entity.ToTable("event_participants");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.IdEvent).HasColumnName("id_event");
            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.RespondedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("responded_at");

            entity.HasOne(d => d.IdEventNavigation)
                .WithMany(p => p.Participants)
                .HasForeignKey(d => d.IdEvent)
                .HasConstraintName("fk_event_participant_event")
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.IdStaffNavigation)
                .WithMany()
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_event_participant_staff")
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.IdEvent, "ix_event_participants_id_event");
            entity.HasIndex(e => e.IdStaff, "ix_event_participants_id_staff");
        });
    }
}