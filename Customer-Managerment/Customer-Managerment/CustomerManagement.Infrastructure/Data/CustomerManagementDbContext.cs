using System;
using System.Collections.Generic;
using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data;

public partial class CustomerManagementDbContext : DbContext
{
    public CustomerManagementDbContext(DbContextOptions<CustomerManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Deal> Deals { get; set; }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.IdActivity).HasName("contact_pkey");

            entity.ToTable("contact");

            entity.Property(e => e.IdActivity)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_activity");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IdLead).HasColumnName("id_lead");
            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.IdLeadNavigation).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.IdLead)
                .HasConstraintName("fk_contact_lead");

            entity.HasOne(d => d.IdStaffNavigation).WithMany(p => p.Contacts)
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_contact_staff");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.IdLead).HasName("customer_pkey");

            entity.ToTable("customer");

            entity.Property(e => e.IdLead)
                .ValueGeneratedNever()
                .HasColumnName("id_lead");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");

            entity.HasOne(d => d.IdLeadNavigation).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.IdLead)
                .HasConstraintName("fk_customer_person");
        });

        modelBuilder.Entity<Deal>(entity =>
        {
            entity.HasKey(e => e.IdDeal).HasName("deal_pkey");

            entity.ToTable("deal");

            entity.Property(e => e.IdDeal)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_deal");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IdCustomer).HasColumnName("id_customer");
            entity.Property(e => e.IdStaff).HasColumnName("id_staff");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasColumnName("price");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValueSql("'Open'::character varying")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.IdCustomerNavigation).WithMany(p => p.Deals)
                .HasForeignKey(d => d.IdCustomer)
                .HasConstraintName("fk_deal_customer");

            entity.HasOne(d => d.IdStaffNavigation).WithMany(p => p.Deals)
                .HasForeignKey(d => d.IdStaff)
                .HasConstraintName("fk_deal_staff");
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.IdLead).HasName("lead_pkey");

            entity.ToTable("lead");

            entity.Property(e => e.IdLead)
                .ValueGeneratedNever()
                .HasColumnName("id_lead");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Resource)
                .HasMaxLength(100)
                .HasColumnName("resource");

            entity.HasOne(d => d.IdLeadNavigation).WithOne(p => p.Lead)
                .HasForeignKey<Lead>(d => d.IdLead)
                .HasConstraintName("fk_lead_person");
        });

        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.IdLead).HasName("person_pkey");

            entity.ToTable("person");

            entity.HasIndex(e => e.Email, "person_email_key").IsUnique();

            entity.Property(e => e.IdLead)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_lead");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Location)
                .HasMaxLength(100)
                .HasColumnName("location");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Salary)
                .HasPrecision(15, 2)
                .HasColumnName("salary");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.IdStaff).HasName("staff_pkey");

            entity.ToTable("staff");

            entity.HasIndex(e => e.Email, "staff_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "staff_username_key").IsUnique();

            entity.Property(e => e.IdStaff)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id_staff");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
