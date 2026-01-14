using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using surveys_services.infrastructure.Persistence.Configurations;
using surveys_services.infrastructure.Persistence.Models;

namespace surveys_services.infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        /// <summary>
        /// Atributo que corresponde a la tabla de HistorialPagos en la base de datos PostgreSQL.
        /// </summary>
        public DbSet<SurveyPostgres> Surveys { get; set; }
        public DbSet<QuestionPostgres> Questions { get; set; }
        public DbSet<AnswerPostgres> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SurveyConfiguration());
            modelBuilder.ApplyConfiguration(new QuestionConfiguration());
            modelBuilder.ApplyConfiguration(new AnswerConfiguration());
        }
    }
}
