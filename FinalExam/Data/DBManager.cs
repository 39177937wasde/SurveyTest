using Microsoft.EntityFrameworkCore;
using FinalExam.Models.Entities;
using System.Net.Mail;
using System.Net;
using Microsoft.CodeAnalysis.Options;
namespace FinalExam.Data
{
    public class DBManager : DbContext
    {

        public DBManager(DbContextOptions options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Questionnaire> Questionnaires { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<QuestionnaireRespond> QuestionnaireResponds { get; set; }
        public async Task<List<Questionnaire>> GetUserQuestionnaires(int userId)
        {
            return await Questionnaires
                .Where(q => q.OwnerID == userId)
                .ToListAsync();
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>(entity =>
        //    {
        //        entity.HasKey(e => e.UserID);
        //        entity.Property(e => e.UserAccount).IsRequired();
        //    });

        //    modelBuilder.Entity<Questionnaire>(entity =>
        //    {
        //        entity.HasKey(e => e.QuestionnaireID);
        //        entity.Property(e => e.OwnerID).IsRequired();
        //        // 其他屬性配置
        //    });

        //modelBuilder.Entity<Questionnaire>(entity =>
        //{
        //    entity.HasKey(e => e.QuestionnaireID);
        //    entity.Property(e => e.OwnerID).IsRequired();
        //    // 確保這裡沒有配置 UserId 或 UserId1
        //});

        //modelBuilder.Entity<Question>(entity =>
        //{
        //    entity.HasKey(e => e.QuestionID);
        //    entity.Property(e => e.QuestionnaireID).IsRequired();
        //    entity.Property(e => e.QuestionText).IsRequired();
        //});

        //modelBuilder.Entity<Option>(entity =>
        //{
        //    entity.HasKey(e => e.OptionID);
        //    entity.Property(e => e.QuestionID).IsRequired();
        //    entity.Property(e => e.OptionText).IsRequired();
        //});

        //modelBuilder.Entity<QuestionnaireRespond>(entity =>
        //{
        //    entity.HasKey(e => e.RespondID);
        //    entity.Property(e => e.QuestionnaireID).IsRequired();
        //    entity.Property(e => e.Answer).IsRequired();
        //});
        //modelBuilder.Entity<Questionnaire>(entity =>
        //{
        //    entity.HasKey(e => e.QuestionnaireID);
        //    entity.Property(e => e.OwnerID).IsRequired();
        //    entity.Property(e => e.Tag).IsRequired();
        //    entity.Property(e => e.EndTime).IsRequired();
        //    entity.Property(e => e.Copies).IsRequired();
        //    entity.Property(e => e.UseCopies).IsRequired();
        //    entity.Property(e => e.State).IsRequired();
        //});
        //}

        //public void SendVerificationEmail(string email)
        //{
        //    var fromEmail = new MailAddress("s1091785@gm.pu.edu.tw", "nxgz safc uwjx lopv");
        //    var toEmail = new MailAddress(email);
        //    var fromEmailPassword = "Daniel39177937";
        //    var existingUser = 
        //    string subject = "Please confirm your email address";
        //    string body = $"Your verification code is: {}";

        //    using (var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com", // Replace with actual SMTP server
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
        //    })
        //    {
        //        using (var message = new MailMessage(fromEmail, toEmail)
        //        {
        //            Subject = subject,
        //            Body = body,
        //            IsBodyHtml = true
        //        })
        //        {
        //            smtp.Send(message);
        //        }
        //    }
        //}
    }
}
