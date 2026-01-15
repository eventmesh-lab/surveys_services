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
    public class AnswerConfiguration : IEntityTypeConfiguration<AnswerPostgres>
    {
        public void Configure(EntityTypeBuilder<AnswerPostgres> builder)
        {
            builder.HasKey(u => u.Id); // Clave primaria

            builder.Property(u => u.PreguntaId)
                .IsRequired();

            builder.Property(u => u.UsuarioId)
                .IsRequired();

            builder.Property(u => u.FechaRespuesta)
                .IsRequired();

            builder.Property(u => u.Valor)
                .IsRequired();

        }
    }
}
