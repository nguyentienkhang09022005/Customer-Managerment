using Customer_Managerment.CustomerManagement.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Customer_Managerment.CustomerManagement.Infrastructure.Data;

public partial class CustomerManagementDbContext : DbContext
{
    public CustomerManagementDbContext(DbContextOptions<CustomerManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<Case> Cases { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Lead> Leads { get; set; }

    public virtual DbSet<Opportunity> Opportunities { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Quote> Quotes { get; set; }

    public virtual DbSet<QuoteDetail> QuoteDetails { get; set; }

    public virtual DbSet<Tasks> Tasks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.IdActivity).HasName("activity_pkey");

            entity.ToTable("activity");

            entity.Property(e => e.IdActivity)
                .ValueGeneratedNever()
                .HasColumnName("id_activity");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IdCustomer).HasColumnName("id_customer");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Subject)
                .HasMaxLength(100)
                .HasColumnName("subject");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Activities)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_activity_user");
        });

        modelBuilder.Entity<Campaign>(entity =>
        {
            entity.HasKey(e => e.IdCampaign).HasName("campaign_pkey");

            entity.ToTable("campaign");

            entity.Property(e => e.IdCampaign)
                .ValueGeneratedNever()
                .HasColumnName("id_campaign");
            entity.Property(e => e.Budget)
                .HasPrecision(15, 2)
                .HasColumnName("budget");
            entity.Property(e => e.CampaignName)
                .HasMaxLength(100)
                .HasColumnName("campaign_name");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.IdCompany).HasColumnName("id_company");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.IdCompanyNavigation).WithMany(p => p.Campaigns)
                .HasForeignKey(d => d.IdCompany)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_campaign_company");
        });

        modelBuilder.Entity<Case>(entity =>
        {
            entity.HasKey(e => e.IdCase).HasName("cases_pkey");

            entity.ToTable("cases");

            entity.Property(e => e.IdCase)
                .ValueGeneratedNever()
                .HasColumnName("id_case");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IdCustomer).HasColumnName("id_customer");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.ResolveAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("resolve_at");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.Cases)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_case_order");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Cases)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_case_user");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.IdCompany).HasName("company_pkey");

            entity.ToTable("company");

            entity.Property(e => e.IdCompany)
                .ValueGeneratedNever()
                .HasColumnName("id_company");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(100)
                .HasColumnName("company_name");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.EstablishmentDate).HasColumnName("establishment_date");
            entity.Property(e => e.Industry)
                .HasMaxLength(100)
                .HasColumnName("industry");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(50)
                .HasColumnName("tax_code");
        });

        modelBuilder.Entity<Lead>(entity =>
        {
            entity.HasKey(e => e.IdLead).HasName("lead_pkey");

            entity.ToTable("lead");

            entity.Property(e => e.IdLead)
                .ValueGeneratedNever()
                .HasColumnName("id_lead");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IdCampaign).HasColumnName("id_campaign");
            entity.Property(e => e.LeadEmail)
                .HasMaxLength(100)
                .HasColumnName("lead_email");
            entity.Property(e => e.LeadName)
                .HasMaxLength(100)
                .HasColumnName("lead_name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.IdCampaignNavigation).WithMany(p => p.Leads)
                .HasForeignKey(d => d.IdCampaign)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_lead_campaign");
        });

        modelBuilder.Entity<Opportunity>(entity =>
        {
            entity.HasKey(e => e.IdOpportunity).HasName("opportunity_pkey");

            entity.ToTable("opportunity");

            entity.Property(e => e.IdOpportunity)
                .ValueGeneratedNever()
                .HasColumnName("id_opportunity");
            entity.Property(e => e.Amount)
                .HasPrecision(15, 2)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpectedClosedDate).HasColumnName("expected_closed_date");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.OpportunityName)
                .HasMaxLength(100)
                .HasColumnName("opportunity_name");
            entity.Property(e => e.Stage)
                .HasMaxLength(50)
                .HasColumnName("stage");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Opportunities)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_opportunity_user");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrder).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.Property(e => e.IdOrder)
                .ValueGeneratedNever()
                .HasColumnName("id_order");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IdCustomer).HasColumnName("id_customer");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(15, 2)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_order_user");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.IdOrderDetail).HasName("order_detail_pkey");

            entity.ToTable("order_detail");

            entity.Property(e => e.IdOrderDetail)
                .ValueGeneratedNever()
                .HasColumnName("id_order_detail");
            entity.Property(e => e.IdOrder).HasColumnName("id_order");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(15, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(15, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.IdOrderNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.IdOrder)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_order_detail_order");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_order_detail_product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProduct).HasName("product_pkey");

            entity.ToTable("product");

            entity.Property(e => e.IdProduct)
                .ValueGeneratedNever()
                .HasColumnName("id_product");
            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasColumnName("category");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Price)
                .HasPrecision(15, 2)
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("product_name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(e => e.IdQuote).HasName("quote_pkey");

            entity.ToTable("quote");

            entity.Property(e => e.IdQuote)
                .ValueGeneratedNever()
                .HasColumnName("id_quote");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiryTime).HasColumnName("expiry_time");
            entity.Property(e => e.IdOpportunity).HasColumnName("id_opportunity");
            entity.Property(e => e.QuoteDate).HasColumnName("quote_date");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(15, 2)
                .HasColumnName("total_amount");

            entity.HasOne(d => d.IdOpportunityNavigation).WithMany(p => p.Quotes)
                .HasForeignKey(d => d.IdOpportunity)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_quote_opportunity");
        });

        modelBuilder.Entity<QuoteDetail>(entity =>
        {
            entity.HasKey(e => e.IdQuoteDetail).HasName("quote_detail_pkey");

            entity.ToTable("quote_detail");

            entity.Property(e => e.IdQuoteDetail)
                .ValueGeneratedNever()
                .HasColumnName("id_quote_detail");
            entity.Property(e => e.IdProduct).HasColumnName("id_product");
            entity.Property(e => e.IdQuote).HasColumnName("id_quote");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(15, 2)
                .HasColumnName("total_price");
            entity.Property(e => e.UnitPrice)
                .HasPrecision(15, 2)
                .HasColumnName("unit_price");

            entity.HasOne(d => d.IdProductNavigation).WithMany(p => p.QuoteDetails)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_quote_detail_product");

            entity.HasOne(d => d.IdQuoteNavigation).WithMany(p => p.QuoteDetails)
                .HasForeignKey(d => d.IdQuote)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_quote_detail_quote");
        });

        modelBuilder.Entity<Tasks>(entity =>
        {
            entity.HasKey(e => e.IdTask).HasName("task_pkey");

            entity.ToTable("task");

            entity.Property(e => e.IdTask)
                .ValueGeneratedNever()
                .HasColumnName("id_task");
            entity.Property(e => e.DueDate).HasColumnName("due_date");
            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .HasColumnName("title");

            entity.HasOne(d => d.IdUserNavigation).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.IdUser)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_task_user");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("id_user");
            entity.Property(e => e.Address)
                .HasMaxLength(200)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(70)
                .HasColumnName("fullname");
            entity.Property(e => e.IdCompany).HasColumnName("id_company");
            entity.Property(e => e.Password)
                .HasMaxLength(200)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.IdCompanyNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdCompany)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_user_company");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
