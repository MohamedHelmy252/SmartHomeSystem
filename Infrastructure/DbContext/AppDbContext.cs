using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Home> Homes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<ESP32Device> ESP32Devices { get; set; }
        public DbSet<SmartDevice> SmartDevices { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        public DbSet<Camera> Cameras { get; set; }
        public DbSet<KnownFace> KnownFaces { get; set; }
        public DbSet<Alert> Alerts { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<AutomationRule> AutomationRules { get; set; }
        public DbSet<AutomationAction> AutomationActions { get; set; }
        public DbSet<AutomationExecution> AutomationExecutions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Primary Keys
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<Home>().HasKey(h => h.HomeId);
            modelBuilder.Entity<Room>().HasKey(r => r.RoomId);
            modelBuilder.Entity<ESP32Device>().HasKey(e => e.ESP32DeviceId);
            modelBuilder.Entity<SmartDevice>().HasKey(s => s.SmartDeviceId);
            modelBuilder.Entity<Sensor>().HasKey(s => s.SensorId);
            modelBuilder.Entity<Camera>().HasKey(c => c.CameraId);
            modelBuilder.Entity<KnownFace>().HasKey(k => k.FaceId);
            modelBuilder.Entity<Alert>().HasKey(a => a.AlertId);
            modelBuilder.Entity<Notification>().HasKey(n => n.NotificationId);
            modelBuilder.Entity<Log>().HasKey(l => l.LogId);
            modelBuilder.Entity<AutomationRule>().HasKey(r => r.RuleId);
            modelBuilder.Entity<AutomationAction>().HasKey(a => a.ActionId);
            modelBuilder.Entity<AutomationExecution>().HasKey(e => e.ExecutionId);

            // User - Home
            modelBuilder.Entity<Home>()
                .HasOne(h => h.Owner)
                .WithOne(u => u.Home)
                .HasForeignKey<Home>(h => h.OwnerUserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Home - Rooms
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Home)
                .WithMany(h => h.Rooms)
                .HasForeignKey(r => r.HomeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Room - ESP32Devices
            modelBuilder.Entity<ESP32Device>()
                .HasOne(e => e.Room)
                .WithMany(r => r.ESP32Devices)
                .HasForeignKey(e => e.RoomId)
                .OnDelete(DeleteBehavior.NoAction);

            // ESP32Device - SmartDevices
            modelBuilder.Entity<SmartDevice>()
                .HasOne(s => s.ESP32Device)
                .WithMany(e => e.SmartDevices)
                .HasForeignKey(s => s.ESP32DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            // ESP32Device - Sensors
            modelBuilder.Entity<Sensor>()
                .HasOne(s => s.ESP32Device)
                .WithMany(e => e.Sensors)
                .HasForeignKey(s => s.ESP32DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            // Home - Cameras
            modelBuilder.Entity<Camera>()
                .HasOne(c => c.Home)
                .WithMany(h => h.Cameras)
                .HasForeignKey(c => c.HomeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Home - KnownFaces
            modelBuilder.Entity<KnownFace>()
                .HasOne(k => k.Home)
                .WithMany(h => h.KnownFaces)
                .HasForeignKey(k => k.HomeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Home - Alerts
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Home)
                .WithMany(h => h.Alerts)
                .HasForeignKey(a => a.HomeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Alert - Notifications
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Alert)
                .WithMany(a => a.Notifications)
                .HasForeignKey(n => n.AlertId)
                .OnDelete(DeleteBehavior.NoAction);

            // User - Logs
            modelBuilder.Entity<Log>()
                .HasOne(l => l.User)
                .WithMany(u => u.Logs)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Home - Logs
            modelBuilder.Entity<Log>()
                .HasOne(l => l.Home)
                .WithMany(h => h.Logs)
                .HasForeignKey(l => l.HomeId)
                .OnDelete(DeleteBehavior.NoAction);

            // Home - AutomationRules
            modelBuilder.Entity<AutomationRule>()
                .HasOne(a => a.Home)
                .WithMany(h => h.AutomationRules)
                .HasForeignKey(a => a.HomeId)
                .OnDelete(DeleteBehavior.NoAction);

            // User - AutomationRules
            modelBuilder.Entity<AutomationRule>()
                .HasOne(a => a.User)
                .WithMany(u => u.AutomationRules)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Sensor - AutomationRules
            modelBuilder.Entity<AutomationRule>()
                .HasOne(a => a.Sensor)
                .WithMany(s => s.AutomationRules)
                .HasForeignKey(a => a.SensorId)
                .OnDelete(DeleteBehavior.NoAction);

            // AutomationRule - AutomationActions
            modelBuilder.Entity<AutomationAction>()
                .HasOne(a => a.Rule)
                .WithMany(r => r.Actions)
                .HasForeignKey(a => a.RuleId)
                .OnDelete(DeleteBehavior.NoAction);

            // SmartDevice - AutomationActions
            modelBuilder.Entity<AutomationAction>()
                .HasOne(a => a.TargetDevice)
                .WithMany()
                .HasForeignKey(a => a.TargetDeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            // AutomationRule - AutomationExecutions
            modelBuilder.Entity<AutomationExecution>()
                .HasOne(e => e.Rule)
                .WithMany(r => r.Executions)
                .HasForeignKey(e => e.RuleId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}