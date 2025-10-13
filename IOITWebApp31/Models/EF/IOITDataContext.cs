using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace IOITWebApp31.Models.EF
{
    public partial class IOITDataContext : DbContext
    {
        public IOITDataContext()
        {
        }

        public IOITDataContext(DbContextOptions<IOITDataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Action> Action { get; set; }
        public virtual DbSet<Attactment> Attactment { get; set; }
        public virtual DbSet<Attribute> Attribute { get; set; }
        public virtual DbSet<AttributeMapping> AttributeMapping { get; set; }
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<BackLink> BackLink { get; set; }
        public virtual DbSet<Bank> Bank { get; set; }
        public virtual DbSet<Block> Block { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryMapping> CategoryMapping { get; set; }
        public virtual DbSet<CategoryRank> CategoryRank { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Config> Config { get; set; }
        public virtual DbSet<ConfigStar> ConfigStar { get; set; }
        public virtual DbSet<ConfigTable> ConfigTable { get; set; }
        public virtual DbSet<ConfigTableItem> ConfigTableItem { get; set; }
        public virtual DbSet<ConfigThumb> ConfigThumb { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerAddress> CustomerAddress { get; set; }
        public virtual DbSet<CustomerMapping> CustomerMapping { get; set; }
        public virtual DbSet<DataSet> DataSet { get; set; }
        public virtual DbSet<DataSetApproved> DataSetApproved { get; set; }
        public virtual DbSet<DataSetDown> DataSetDown { get; set; }
        public virtual DbSet<DataSetMapping> DataSetMapping { get; set; }
        public virtual DbSet<DataSetView> DataSetView { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Dictionary> Dictionary { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<FolderCeph> FolderCeph { get; set; }
        public virtual DbSet<Function> Function { get; set; }
        public virtual DbSet<FunctionRole> FunctionRole { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<LanguageMapping> LanguageMapping { get; set; }
        public virtual DbSet<LegalDoc> LegalDoc { get; set; }
        public virtual DbSet<Manufacturer> Manufacturer { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<MenuItem> MenuItem { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsApproved> NewsApproved { get; set; }
        public virtual DbSet<NewsNote> NewsNote { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderItem> OrderItem { get; set; }
        public virtual DbSet<PaymentHistory> PaymentHistory { get; set; }
        public virtual DbSet<PermaLink> PermaLink { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductAttribuite> ProductAttribuite { get; set; }
        public virtual DbSet<ProductCustomer> ProductCustomer { get; set; }
        public virtual DbSet<ProductImage> ProductImage { get; set; }
        public virtual DbSet<ProductReview> ProductReview { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<Publication> Publication { get; set; }
        public virtual DbSet<Ratify> Ratify { get; set; }
        public virtual DbSet<Related> Related { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Slide> Slide { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<TagMapping> TagMapping { get; set; }
        public virtual DbSet<TypeAttribute> TypeAttribute { get; set; }
        public virtual DbSet<TypeAttributeItem> TypeAttributeItem { get; set; }
        public virtual DbSet<TypeSlide> TypeSlide { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserMapping> UserMapping { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Wards> Wards { get; set; }
        public virtual DbSet<Website> Website { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");

                var configuration = builder.Build();
                string con = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(con);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Action>(entity =>
            {
                entity.Property(e => e.ActionName).HasMaxLength(200);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Ipaddress)
                    .HasColumnName("IPAddress")
                    .HasMaxLength(50);

                entity.Property(e => e.LastLogs).HasColumnType("ntext");

                entity.Property(e => e.Logs).HasColumnType("ntext");

                entity.Property(e => e.TargetId).HasMaxLength(50);

                entity.Property(e => e.TargetName).HasMaxLength(500);
            });

            modelBuilder.Entity<Attactment>(entity =>
            {
                entity.Property(e => e.AttactmentId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExtensionName).HasMaxLength(10);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(200);

                entity.Property(e => e.Thumb).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasColumnType("ntext");
            });

            modelBuilder.Entity<Attribute>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<AttributeMapping>(entity =>
            {
                entity.Property(e => e.AttributeMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Author>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Avatar).HasMaxLength(1000);

                entity.Property(e => e.Cccd).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.Property(e => e.NumberPhone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<BackLink>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.LinkIn).HasMaxLength(500);

                entity.Property(e => e.LinkOut).HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Bank>(entity =>
            {
                entity.Property(e => e.AccountId).HasMaxLength(100);

                entity.Property(e => e.AccountName).HasMaxLength(500);

                entity.Property(e => e.BranchName).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Block>(entity =>
            {
                entity.Property(e => e.Code).IsRequired();

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.IconFa).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.Avatar).HasMaxLength(500);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Lat)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Long)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Icon).HasMaxLength(2000);

                entity.Property(e => e.IconFa).HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(2000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<CategoryMapping>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<CategoryRank>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdateAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Address).HasColumnType("ntext");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.ContactEmail).HasMaxLength(500);

                entity.Property(e => e.ContactName).HasMaxLength(200);

                entity.Property(e => e.ContactPhone).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Fax).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Representative).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Config>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.BodyScript).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CustomCss).HasMaxLength(500);

                entity.Property(e => e.EmailColorBody).HasMaxLength(200);

                entity.Property(e => e.EmailColorFooter).HasMaxLength(200);

                entity.Property(e => e.EmailColorHeader).HasMaxLength(200);

                entity.Property(e => e.EmailDisplayName).HasMaxLength(200);

                entity.Property(e => e.EmailEnableSsl).HasColumnName("EmailEnableSSL");

                entity.Property(e => e.EmailHost).HasMaxLength(500);

                entity.Property(e => e.EmailLogo).HasMaxLength(500);

                entity.Property(e => e.EmailPasswordHash).HasMaxLength(500);

                entity.Property(e => e.EmailReceive).HasMaxLength(500);

                entity.Property(e => e.EmailSender).HasMaxLength(500);

                entity.Property(e => e.EmailUserName).HasMaxLength(500);

                entity.Property(e => e.ExchangRate).HasColumnType("money");

                entity.Property(e => e.FooterScript).HasMaxLength(500);

                entity.Property(e => e.HeaderScript).HasMaxLength(500);

                entity.Property(e => e.OpAccessCode).HasMaxLength(50);

                entity.Property(e => e.OpKey).HasMaxLength(100);

                entity.Property(e => e.OpMerchant).HasMaxLength(50);

                entity.Property(e => e.OpPassword).HasMaxLength(50);

                entity.Property(e => e.OpUser).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Website).HasMaxLength(500);
            });

            modelBuilder.Entity<ConfigStar>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.StarColor).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConfigTable>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConfigTableItem>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DataType).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ConfigThumb>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.Address)
                    .HasMaxLength(1000)
                    .IsFixedLength();

                entity.Property(e => e.Attactment).HasMaxLength(1000);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.FullName).HasMaxLength(500);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Flag).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Address).HasColumnType("ntext");

                entity.Property(e => e.AddressNumber).HasMaxLength(500);

                entity.Property(e => e.Avata).HasColumnType("ntext");

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateNumber).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.FullName).HasMaxLength(500);

                entity.Property(e => e.IdNumber).HasMaxLength(50);

                entity.Property(e => e.KeyRandom)
                    .HasMaxLength(8)
                    .IsFixedLength();

                entity.Property(e => e.LastLoginAt).HasColumnType("datetime");

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Username).HasMaxLength(500);
                // haohv
                entity.Property(e => e.StudentCode).HasMaxLength(50);
                entity.Property(e => e.StudentYear).HasMaxLength(50);
                entity.Property(e => e.StudentClass).HasMaxLength(50);
                entity.Property(e => e.AchievementNote).HasMaxLength(1000);
                entity.Property(e => e.HobbyNote).HasMaxLength(1000);
                entity.Property(e => e.PersonSummary).HasColumnType("ntext");
            });

            modelBuilder.Entity<CustomerAddress>(entity =>
            {
                entity.Property(e => e.CustomerAddressId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.Phone).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<CustomerMapping>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<DataSet>(entity =>
            {
                entity.Property(e => e.ApprovedAt).HasColumnType("datetime");

                entity.Property(e => e.ApprovingAt).HasColumnType("datetime");

                entity.Property(e => e.AuthorEmail).HasMaxLength(1000);

                entity.Property(e => e.AuthorName).HasMaxLength(1000);

                entity.Property(e => e.AuthorPhone).HasMaxLength(500);

                entity.Property(e => e.ConfirmsPrivate).HasMaxLength(1000);

                entity.Property(e => e.ConfirmsPublish).HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateEndOn).HasColumnType("datetime");

                entity.Property(e => e.DateStartActive).HasColumnType("datetime");

                entity.Property(e => e.DateStartOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.EditedAt).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(2000);

                entity.Property(e => e.LinkVideo).HasMaxLength(1000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.PublishedAt).HasColumnType("datetime");

                entity.Property(e => e.PublishingAt).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);

                entity.Property(e => e.Version).HasMaxLength(200);
            });

            modelBuilder.Entity<DataSetApproved>(entity =>
            {
                entity.Property(e => e.DataSetApprovedId).ValueGeneratedNever();

                entity.Property(e => e.Confirms).HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<DataSetDown>(entity =>
            {
                entity.Property(e => e.DataSetDownId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.IpAddress).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<DataSetMapping>(entity =>
            {
                entity.Property(e => e.DataSetMappingId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<DataSetView>(entity =>
            {
                entity.Property(e => e.DataSetViewId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.IpAddress).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Dictionary>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.StringEn).HasColumnType("ntext");

                entity.Property(e => e.StringVn).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<FolderCeph>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.Link).HasMaxLength(200);
            });

            modelBuilder.Entity<Function>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Icon).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(200);
            });

            modelBuilder.Entity<FunctionRole>(entity =>
            {
                entity.Property(e => e.ActiveKey).HasMaxLength(20);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Function)
                    .WithMany(p => p.FunctionRole)
                    .HasForeignKey(d => d.FunctionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FunctionRole_Function");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(5);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Flag).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<LanguageMapping>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<LegalDoc>(entity =>
            {
                entity.Property(e => e.Attactment).HasMaxLength(1000);

                entity.Property(e => e.Code).HasMaxLength(200);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateEffect).HasColumnType("datetime");

                entity.Property(e => e.DateIssue).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Extension)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(4000);

                entity.Property(e => e.Signer).HasMaxLength(1000);

                entity.Property(e => e.TichYeu).HasMaxLength(4000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.Property(e => e.Address).HasColumnType("ntext");

                entity.Property(e => e.AvatarOwner).HasMaxLength(500);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.Country).HasMaxLength(500);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Fax).HasMaxLength(50);

                entity.Property(e => e.Logo).HasMaxLength(1000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeywords).HasMaxLength(200);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Mobile).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.NickName).HasMaxLength(500);

                entity.Property(e => e.Owner).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(500);

                entity.Property(e => e.Website).HasMaxLength(500);
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.ApprovedAt).HasColumnType("datetime");

                entity.Property(e => e.ApprovingAt).HasColumnType("datetime");

                entity.Property(e => e.Author).HasMaxLength(100);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateEndOn).HasColumnType("datetime");

                entity.Property(e => e.DateStartActive).HasColumnType("datetime");

                entity.Property(e => e.DateStartOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.EditedAt).HasColumnType("datetime");

                entity.Property(e => e.EditingAt).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(2000);

                entity.Property(e => e.Introduce).HasColumnType("ntext");

                entity.Property(e => e.LinkVideo).HasMaxLength(1000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Note).HasMaxLength(2000);

                entity.Property(e => e.PublishedAt).HasColumnType("datetime");

                entity.Property(e => e.PublishingAt).HasColumnType("datetime");

                entity.Property(e => e.SystemDiagram).HasColumnType("ntext");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.TotalPrice).HasColumnType("money");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);

                entity.Property(e => e.ValuePrice).HasColumnType("money");
            });

            modelBuilder.Entity<NewsApproved>(entity =>
            {
                entity.Property(e => e.NewsApprovedId).ValueGeneratedNever();

                entity.Property(e => e.Confirms).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<NewsNote>(entity =>
            {
                entity.Property(e => e.NewsNoteId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Note).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.TargetId).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(1000);

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UrlLink).HasMaxLength(500);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CustomerNote).HasColumnType("ntext");

                entity.Property(e => e.OrderDelivery).HasColumnType("money");

                entity.Property(e => e.OrderDiscount).HasColumnType("money");

                entity.Property(e => e.OrderPaid).HasColumnType("money");

                entity.Property(e => e.OrderTax).HasColumnType("money");

                entity.Property(e => e.OrderTotal).HasColumnType("money");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('2015-09-20T16:17:38.112Z')");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.PriceDiscount).HasColumnType("money");

                entity.Property(e => e.PriceTax).HasColumnType("money");

                entity.Property(e => e.PriceTotal).HasColumnType("money");
            });

            modelBuilder.Entity<PaymentHistory>(entity =>
            {
                entity.Property(e => e.PaymentHistoryId).ValueGeneratedNever();

                entity.Property(e => e.AccessCode).HasMaxLength(10);

                entity.Property(e => e.AgainLink).HasMaxLength(500);

                entity.Property(e => e.Amount).HasMaxLength(50);

                entity.Property(e => e.Card).HasMaxLength(20);

                entity.Property(e => e.CardList).HasMaxLength(300);

                entity.Property(e => e.CardUid).HasMaxLength(100);

                entity.Property(e => e.Command).HasMaxLength(20);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Currency).HasMaxLength(5);

                entity.Property(e => e.CustomerEmail).HasMaxLength(200);

                entity.Property(e => e.CustomerId).HasMaxLength(50);

                entity.Property(e => e.CustomerPhone).HasMaxLength(20);

                entity.Property(e => e.Locale).HasMaxLength(5);

                entity.Property(e => e.MerchTxnRef).HasMaxLength(50);

                entity.Property(e => e.Merchant).HasMaxLength(50);

                entity.Property(e => e.Message).HasMaxLength(500);

                entity.Property(e => e.OrderInfo).HasMaxLength(50);

                entity.Property(e => e.PayChannel).HasMaxLength(50);

                entity.Property(e => e.ReturnUrl)
                    .HasColumnName("ReturnURL")
                    .HasMaxLength(500);

                entity.Property(e => e.SecureHash).HasMaxLength(100);

                entity.Property(e => e.TicketNo).HasMaxLength(20);

                entity.Property(e => e.TransactionNo).HasMaxLength(100);

                entity.Property(e => e.TxnResponseCode).HasMaxLength(10);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<PermaLink>(entity =>
            {
                entity.Property(e => e.PermaLinkId).ValueGeneratedNever();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Slug).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Configuration).HasColumnType("ntext");

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateEndOn).HasColumnType("datetime");

                entity.Property(e => e.DateStartActive).HasColumnType("datetime");

                entity.Property(e => e.DateStartOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Feature).HasColumnType("ntext");

                entity.Property(e => e.GuaranteeProduct)
                    .HasMaxLength(1000)
                    .IsFixedLength();

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.Introduce).HasColumnType("ntext");

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.NotePromotion).HasColumnType("ntext");

                entity.Property(e => e.NoteTech).HasColumnType("ntext");

                entity.Property(e => e.OriginProduct)
                    .HasMaxLength(1000)
                    .IsFixedLength();

                entity.Property(e => e.PriceImport).HasColumnType("money");

                entity.Property(e => e.PriceOther).HasColumnType("money");

                entity.Property(e => e.PriceSale).HasColumnType("money");

                entity.Property(e => e.PriceSpecial).HasColumnType("money");

                entity.Property(e => e.ProductAttributes).HasColumnType("ntext");

                entity.Property(e => e.ProductNote).HasColumnType("ntext");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<ProductAttribuite>(entity =>
            {
                entity.HasKey(e => e.ProductAttributeId);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnType("ntext");

                entity.Property(e => e.Height).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Image).HasMaxLength(1000);

                entity.Property(e => e.Length).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.PriceSpecial).HasColumnType("money");

                entity.Property(e => e.PriceSpecialEnd).HasColumnType("datetime");

                entity.Property(e => e.PriceSpecialStart).HasColumnType("datetime");

                entity.Property(e => e.Weight).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.Width).HasColumnType("decimal(18, 0)");
            });

            modelBuilder.Entity<ProductCustomer>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<ProductReview>(entity =>
            {
                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.Property(e => e.Code)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.Lang).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Publication>(entity =>
            {
                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.ContentsEn).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateEndOn).HasColumnType("datetime");

                entity.Property(e => e.DatePublic).HasMaxLength(50);

                entity.Property(e => e.DateStartActive).HasColumnType("datetime");

                entity.Property(e => e.DateStartOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.DescriptionEn).HasMaxLength(2000);

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.NumberOfTopic).HasMaxLength(100);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.TitleEn).HasMaxLength(1000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<Ratify>(entity =>
            {
                entity.Property(e => e.Author).HasMaxLength(100);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateEndOn).HasColumnType("datetime");

                entity.Property(e => e.DateStartActive).HasColumnType("datetime");

                entity.Property(e => e.DateStartOn).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.Introduce).HasColumnType("ntext");

                entity.Property(e => e.LinkVideo).HasMaxLength(1000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.SystemDiagram).HasColumnType("ntext");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<Related>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Slide>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.Image).HasColumnType("ntext");

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Title).HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);

                entity.Property(e => e.UrlYoutube).HasMaxLength(1000);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(1000);

                entity.Property(e => e.Url).HasMaxLength(1000);
            });

            modelBuilder.Entity<TagMapping>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<TypeAttribute>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Image).HasMaxLength(50);

                entity.Property(e => e.Location).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Size).HasMaxLength(50);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<TypeAttributeItem>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Image).HasMaxLength(1000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<TypeSlide>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(500);

                entity.Property(e => e.AddressNumber).HasMaxLength(500);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Contents).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.DateNumber).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(2000);

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.EmailAdmin).HasMaxLength(500);

                entity.Property(e => e.Fax).HasMaxLength(50);

                entity.Property(e => e.Icon).HasMaxLength(50);

                entity.Property(e => e.IconFa).HasMaxLength(50);

                entity.Property(e => e.IdNumber).HasMaxLength(50);

                entity.Property(e => e.Image).HasMaxLength(2000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.NameAdmin).HasMaxLength(100);

                entity.Property(e => e.NameEn).HasMaxLength(500);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.ShortName).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(1000);

                entity.Property(e => e.Website).HasMaxLength(100);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(200);

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(100);

                entity.Property(e => e.KeyLock).HasMaxLength(20);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.RegEmail).HasMaxLength(50);

                entity.Property(e => e.TokenSince).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<UserMapping>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Wards>(entity =>
            {
                entity.HasKey(e => e.WardId)
                    .HasName("PK__Wards__C6BD9BCAC8A935B5");

                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Name).HasMaxLength(500);
            });

            modelBuilder.Entity<Website>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.Address2En).HasMaxLength(1000);

                entity.Property(e => e.AddressEn).HasMaxLength(1000);

                entity.Property(e => e.Banner).HasMaxLength(1000);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Fax).HasMaxLength(50);

                entity.Property(e => e.GoogleAnalitics).HasMaxLength(50);

                entity.Property(e => e.GuaRanTeePhone).HasMaxLength(50);

                entity.Property(e => e.Hotline).HasMaxLength(50);

                entity.Property(e => e.Hotmail).HasMaxLength(500);

                entity.Property(e => e.Icon1).HasMaxLength(1000);

                entity.Property(e => e.Icon2).HasMaxLength(1000);

                entity.Property(e => e.Icon3).HasMaxLength(1000);

                entity.Property(e => e.Icon4).HasMaxLength(1000);

                entity.Property(e => e.Icon5).HasMaxLength(1000);

                entity.Property(e => e.Icon6).HasMaxLength(1000);

                entity.Property(e => e.IconBct).HasMaxLength(1000);

                entity.Property(e => e.Link1).HasMaxLength(1000);

                entity.Property(e => e.Link2).HasMaxLength(1000);

                entity.Property(e => e.Link3).HasMaxLength(1000);

                entity.Property(e => e.Link4).HasMaxLength(1000);

                entity.Property(e => e.Link5).HasMaxLength(1000);

                entity.Property(e => e.Link6).HasMaxLength(1000);

                entity.Property(e => e.LinkMap).HasMaxLength(1000);

                entity.Property(e => e.LinkOther1).HasMaxLength(1000);

                entity.Property(e => e.LinkOther2).HasMaxLength(1000);

                entity.Property(e => e.LinkOther3).HasMaxLength(1000);

                entity.Property(e => e.LogoFooter).HasMaxLength(1000);

                entity.Property(e => e.LogoHeader).HasMaxLength(1000);

                entity.Property(e => e.MetaDescription).HasMaxLength(500);

                entity.Property(e => e.MetaKeyword).HasMaxLength(300);

                entity.Property(e => e.MetaTitle).HasMaxLength(500);

                entity.Property(e => e.Name).HasMaxLength(500);

                entity.Property(e => e.Organizations).HasMaxLength(200);

                entity.Property(e => e.OrganizationsUp).HasMaxLength(200);

                entity.Property(e => e.SystemName).HasMaxLength(200);

                entity.Property(e => e.TechNiQuePhone).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.Property(e => e.UnitName).HasMaxLength(200);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Url).HasMaxLength(300);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
