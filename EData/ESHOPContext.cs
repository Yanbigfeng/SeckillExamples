using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SeckillExamples.EData
{
    public partial class ESHOPContext : DbContext
    {
        public ESHOPContext()
        {
        }

        public ESHOPContext(DbContextOptions<ESHOPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TArticle> TArticle { get; set; }
        public virtual DbSet<TOrder> TOrder { get; set; }
        public virtual DbSet<TPayRecord> TPayRecord { get; set; }
        public virtual DbSet<TSeckillArticle> TSeckillArticle { get; set; }
        public virtual DbSet<TSeckillTimeModel> TSeckillTimeModel { get; set; }
        public virtual DbSet<TUser> TUser { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TArticle>(entity =>
            {
                entity.ToTable("T_Article");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.AddTime)
                    .HasColumnName("addTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.ArticleDesc)
                    .HasColumnName("articleDesc")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ArticlePrice)
                    .HasColumnName("articlePrice")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ArticleStatus).HasColumnName("articleStatus");

                entity.Property(e => e.ArticleStockNum).HasColumnName("articleStockNum");

                entity.Property(e => e.ArticleTitle)
                    .HasColumnName("articleTitle")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DelFlag).HasColumnName("delFlag");
            });

            modelBuilder.Entity<TOrder>(entity =>
            {
                entity.ToTable("T_Order");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.AddTime)
                    .HasColumnName("addTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.ArticleId)
                    .IsRequired()
                    .HasColumnName("articleId")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.DelFlag).HasColumnName("delFlag");

                entity.Property(e => e.DeliverTime)
                    .HasColumnName("deliverTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.FinishTime)
                    .HasColumnName("finishTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderAddress)
                    .HasColumnName("orderAddress")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.OrderNum)
                    .IsRequired()
                    .HasColumnName("orderNum")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrderPhone)
                    .HasColumnName("orderPhone")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrderRemark)
                    .HasColumnName("orderRemark")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.OrderStatus).HasColumnName("orderStatus");

                entity.Property(e => e.OrderTotalPrice)
                    .HasColumnName("orderTotalPrice")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PayTime)
                    .HasColumnName("payTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("updateTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("userId")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TPayRecord>(entity =>
            {
                entity.ToTable("T_PayRecord");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.AddTime)
                    .HasColumnName("addTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DelFlag).HasColumnName("delFlag");

                entity.Property(e => e.OrderId)
                    .IsRequired()
                    .HasColumnName("orderId")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.OrderNum)
                    .IsRequired()
                    .HasColumnName("orderNum")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PayPrice)
                    .HasColumnName("payPrice")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PayStatus).HasColumnName("payStatus");

                entity.Property(e => e.PayStyle).HasColumnName("payStyle");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("userId")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<TSeckillArticle>(entity =>
            {
                entity.ToTable("T_SeckillArticle");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.AddTime)
                    .HasColumnName("addTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.ArticleId)
                    .IsRequired()
                    .HasColumnName("articleId")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.ArticleName)
                    .HasColumnName("articleName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ArticlePercent)
                    .HasColumnName("articlePercent")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ArticlePrice)
                    .HasColumnName("articlePrice")
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ArticleStockNum).HasColumnName("articleStockNum");

                entity.Property(e => e.ArticleUrl)
                    .HasColumnName("articleUrl")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DelFlag).HasColumnName("delFlag");

                entity.Property(e => e.LimitNum).HasColumnName("limitNum");

                entity.Property(e => e.SeckillStatus).HasColumnName("seckillStatus");

                entity.Property(e => e.SeckillTimeModelId)
                    .IsRequired()
                    .HasColumnName("seckillTimeModelId")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.SeckillType).HasColumnName("seckillType");
            });

            modelBuilder.Entity<TSeckillTimeModel>(entity =>
            {
                entity.ToTable("T_SeckillTimeModel");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.AddTime)
                    .HasColumnName("addTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DelFlag).HasColumnName("delFlag");

                entity.Property(e => e.EndTime)
                    .HasColumnName("endTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.SeckillTime)
                    .IsRequired()
                    .HasColumnName("seckillTime")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.StartTime)
                    .HasColumnName("startTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.Status).HasColumnName("status");
            });

            modelBuilder.Entity<TUser>(entity =>
            {
                entity.ToTable("T_User");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.AddTime)
                    .HasColumnName("addTime")
                    .HasColumnType("datetime");

                entity.Property(e => e.DelFlag).HasColumnName("delFlag");

                entity.Property(e => e.UserAccount)
                    .IsRequired()
                    .HasColumnName("userAccount")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserNickName)
                    .HasColumnName("userNickName")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserPwd)
                    .IsRequired()
                    .HasColumnName("userPwd")
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
