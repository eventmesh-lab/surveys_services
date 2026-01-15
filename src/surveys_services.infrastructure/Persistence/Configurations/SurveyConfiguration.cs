using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using surveys_services.infrastructure.Persistence.Models;

namespace surveys_services.infrastructure.Persistence.Configurations
{
    public class SurveyConfiguration : IEntityTypeConfiguration<SurveyPostgres>
    {
        public void Configure(EntityTypeBuilder<SurveyPostgres> builder)
        {
            builder.HasKey(u => u.Id); // Clave primaria

            builder.Property(u => u.EventoId)
                .IsRequired();

            builder.Property(u => u.Titulo)
                .IsRequired();

            builder.Property(u => u.FechaCreacion)
                .IsRequired();

        }
    }
}
