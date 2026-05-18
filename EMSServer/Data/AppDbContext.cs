using EMSBaseLibrary.DTOs;
using EMSBaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMSServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        #region DbSets
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<EmployeePosition> Positions => Set<EmployeePosition>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
        public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
        public DbSet<PerformanceReview> PerformanceReviews => Set<PerformanceReview>();
        public DbSet<EmployeeDocument> EmployeeDocuments => Set<EmployeeDocument>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<EmployeeNotification> EmployeeNotifications => Set<EmployeeNotification>();
        public DbSet<Bonus> Bonuses => Set<Bonus>();
        public DbSet<Deduction> Deductions => Set<Deduction>();
        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<EmployeeAsset> EmployeeAssets => Set<EmployeeAsset>();
        public DbSet<User> Users { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<RefreshTokenInfo> RefreshTokenInfos { get; set; }
        #endregion

        #region Configurations

        private void ConfigureEmployee(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Employee>();

            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.Salary)
                  .HasPrecision(18, 2);

            entity.HasOne(e => e.Manager)
             .WithMany(e => e.Subordinates)
             .HasForeignKey(e => e.ManagerId)
             .OnDelete(DeleteBehavior.Restrict);

        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Employee → Department
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee → Role
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany(r => r.Employees)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.SetNull);

            // Employee → Position
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId)
                .OnDelete(DeleteBehavior.SetNull);

            // Attendance
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeId);

            // Leave Requests
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(l => l.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(l => l.Approver)
                .WithMany()
                .HasForeignKey(l => l.ApprovedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Performance Reviews
            modelBuilder.Entity<PerformanceReview>()
                .HasOne(r => r.Employee)
                .WithMany()
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PerformanceReview>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Documents
            modelBuilder.Entity<EmployeeDocument>()
                .HasOne(d => d.Employee)
                .WithMany(e => e.Documents)
                .HasForeignKey(d => d.EmployeeId);

            modelBuilder.Entity<EmployeeDocument>()
                .HasOne(d => d.Uploader)
                .WithMany()
                .HasForeignKey(d => d.UploadedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Notifications (many-to-many via join)
            modelBuilder.Entity<EmployeeNotification>()
                .HasOne(en => en.Employee)
                .WithMany(e => e.EmployeeNotifications)
                .HasForeignKey(en => en.EmployeeId);

            modelBuilder.Entity<EmployeeNotification>()
                .HasOne(en => en.Notification)
                .WithMany(n => n.EmployeeNotifications)
                .HasForeignKey(en => en.NotificationId);

            // Bonuses
            modelBuilder.Entity<Bonus>()
                .HasOne(b => b.Employee)
                .WithMany(e => e.Bonuses)
                .HasForeignKey(b => b.EmployeeId);

            // Deductions
            modelBuilder.Entity<Deduction>()
                .HasOne(d => d.Employee)
                .WithMany(e => e.Deductions)
                .HasForeignKey(d => d.EmployeeId);

            // Assets
            modelBuilder.Entity<EmployeeAsset>()
                .HasOne(ea => ea.Employee)
                .WithMany(e => e.EmployeeAssets)
                .HasForeignKey(ea => ea.EmployeeId);

            modelBuilder.Entity<EmployeeAsset>()
                .HasOne(ea => ea.Asset)
                .WithMany(a => a.EmployeeAssets)
                .HasForeignKey(ea => ea.AssetId);
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EmployeeNotification>()
                .HasIndex(en => new { en.EmployeeId, en.IsRead });

            modelBuilder.Entity<Asset>()
                .HasIndex(a => a.AssetTag)
                .IsUnique();

            modelBuilder.Entity<EmployeeDocument>()
                .HasIndex(d => d.EmployeeId);

            modelBuilder.Entity<Bonus>()
                .HasIndex(b => b.EmployeeId);

            modelBuilder.Entity<Deduction>()
                .HasIndex(d => d.EmployeeId);
        }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureEmployee(modelBuilder);
            ConfigureRelationships(modelBuilder);
            ConfigureIndexes(modelBuilder);

        }

    }
}
