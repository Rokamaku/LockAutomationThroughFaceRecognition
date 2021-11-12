using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Para.LockAutomation.Models
{
    public class LockAutomationDbContext : DbContext
    {
        public LockAutomationDbContext(DbContextOptions<LockAutomationDbContext> options) : base(options)
        {

        }
        protected virtual DbSet<FaceEntity> face { get; set; }
        protected virtual DbSet<PersonEntity> person { get; set; }
        protected virtual DbSet<PersonGroupEntity> personGroup { get; set; }
        protected virtual DbSet<FaceLogEntity> faceLog { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PersonGroupEntity>(entity =>
            {
                entity.HasKey(pg => pg.Id);
            });

            builder.Entity<PersonEntity>(entity =>
            {
                entity.HasKey(ps => ps.Id);
                entity.HasOne(ps => ps.PersonGroup)
                    .WithMany(pg => pg.Persons)
                    .HasForeignKey(ps => ps.PersonGroupId)
                    .HasPrincipalKey(pg => pg.Id);

            });

            builder.Entity<FaceEntity>(entity =>
            {
                entity.HasKey(fc => fc.Id);
                entity.HasOne(fc => fc.Person)
                    .WithMany(ps => ps.Faces)
                    .HasForeignKey(fc => fc.PersonId)
                    .HasPrincipalKey(ps => ps.Id);
                entity.Property(fc => fc.File).HasConversion(
                    js => JsonConvert.SerializeObject(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    js => JsonConvert.DeserializeObject<AzureFile>(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                    );
            });

            builder.Entity<FaceLogEntity>(entity =>
            {
                entity.HasKey(fl => fl.Id);
                entity.HasOne(fl => fl.PersonGroup)
                    .WithMany()
                    .HasForeignKey(fl => fl.PersonGroupId)
                    .HasPrincipalKey(fl => fl.Id);
                entity.Property(fl => fl.Persons).HasConversion(
                    js => JsonConvert.SerializeObject(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    js => JsonConvert.DeserializeObject<List<Guid>>(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                    );
                entity.Property(fl => fl.FaceRectangles).HasConversion(
                    js => JsonConvert.SerializeObject(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    js => JsonConvert.DeserializeObject<List<FaceRectangle>>(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                    );
                entity.Property(fl => fl.File).HasConversion(
                    js => JsonConvert.SerializeObject(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    js => JsonConvert.DeserializeObject<AzureFile>(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                    );
                entity.Property(fl => fl.Confidences).HasConversion(
                    js => JsonConvert.SerializeObject(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    js => JsonConvert.DeserializeObject<List<double>>(js, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                    );
            });
        }

        public IQueryable<T> Entity<T>(bool isTracking = false) where T : BaseEntity
        {
            return isTracking ?
                Set<T>() :
                Set<T>().AsNoTracking();
        }

        public IQueryable<T> ExecProc<T>(string sqlQuery) where T : BaseEntity
        {
            return Set<T>().AsNoTracking().FromSql(sqlQuery);
        }

        [DbFunction(FunctionName = "fn_CountTotalFaceLogInGroup", Schema = "dbo")]
        public static int fn_CountTotalFaceLogInGroup(int personGroupId, DateTime? fromDate, DateTime? toDate)
        {
            return 0;
        }

        [DbFunction(FunctionName = "fn_CountTotalKnownFaceLog", Schema = "dbo")]
        public static int fn_CountTotalKnownFaceLog(int personGroupId, DateTime? fromDate, DateTime? toDate)
        {
            return 0;
        }

        [DbFunction(FunctionName = "fn_CountTotalUnknownFaceLog", Schema = "dbo")]
        public static int fn_CountTotalUnknownFaceLog(int personGroupId, DateTime? fromDate, DateTime? toDate)
        {
            return 0;
        }

        [DbFunction(FunctionName = "fn_CountTotalPersonFaceLog", Schema = "dbo")]
        public static int fn_CountTotalPersonFaceLog(int personGroupId, int personId, DateTime? fromDate, DateTime? toDate)
        {
            return 0;
        }

        [DbFunction(FunctionName = "fn_CountTotalUndetectedFaceLog", Schema = "dbo")]
        public static int fn_CountTotalUndetectedFaceLog(int personGroupId, DateTime? fromDate, DateTime? toDate)
        {
            return 0;
        }

    }
}
