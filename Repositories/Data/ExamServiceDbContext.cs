using BO.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Data
{
    public class ExamServiceDbContext : DbContext
    {
        public ExamServiceDbContext(DbContextOptions<ExamServiceDbContext> options) : base(options)
        {
        }

        // Khai báo các DbSet (tương ứng với các bảng)
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionBank> QuestionBanks { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<ExamSet> ExamSets { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<SubmissionAnswer> SubmissionAnswers { get; set; }

        //insert new dbset here

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Cấu hình các mối quan hệ (Relationships) ---

            // Cấu hình khóa chính phức hợp (Composite Key) cho bảng N-N ExamQuestion
            modelBuilder.Entity<ExamQuestion>()
                .HasKey(eq => new { eq.ExamId, eq.QuestionId });

            // Quan hệ 1-N: QuestionBank -> Question
            modelBuilder.Entity<Question>()
                .HasOne(q => q.QuestionBank)
                .WithMany(qb => qb.Questions)
                .HasForeignKey(q => q.QuestionBankId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa QuestionBank thì xóa Question

            // Quan hệ 1-N: Exam -> Submission
            modelBuilder.Entity<Submission>()
                .HasOne(s => s.Exam)
                .WithMany(e => e.Submissions)
                .HasForeignKey(s => s.ExamId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Exam nếu đã có Submission

            // Quan hệ N-N: Exam <-> Question (thông qua ExamQuestion)
            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Exam)
                .WithMany(e => e.ExamQuestions)
                .HasForeignKey(eq => eq.ExamId);

            modelBuilder.Entity<ExamQuestion>()
                .HasOne(eq => eq.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(eq => eq.QuestionId);

            // --- Cấu hình Index cho các Khóa ngoại (FK) từ service khác ---
            // Giúp tăng tốc độ truy vấn trên các ID này
            modelBuilder.Entity<Submission>()
                .HasIndex(s => s.StudentId);

            modelBuilder.Entity<ExamSet>()
                .HasIndex(es => es.FlashcardId);

            // Cấu hình cho SubmissionAnswer
            modelBuilder.Entity<SubmissionAnswer>()
                .HasOne(sa => sa.Submission)
                .WithMany(s => s.Answers) // <-- Trỏ đến thuộc tính 'Answers' trong Submission
                .HasForeignKey(sa => sa.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Submission thì xóa Answer

            modelBuilder.Entity<SubmissionAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany() // Question không cần biết về SubmissionAnswer
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa Question nếu đã có câu trả lời
        }
    }
}



