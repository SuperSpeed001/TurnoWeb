using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Turnos.Common.Models.DB
{
    public partial class dbTurnosContext : DbContext
    {
        public dbTurnosContext()
        {
        }

        public dbTurnosContext(DbContextOptions<dbTurnosContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dias> Dias { get; set; }
        public virtual DbSet<Generar> Generar { get; set; }
        public virtual DbSet<Horarios> Horarios { get; set; }
        public virtual DbSet<Personas> Personas { get; set; }
        public virtual DbSet<Turnos> Turnos { get; set; }

       /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=199.5.83.228;Database=dbTurnos;user id=usrTurnos;password=usrTurnos##;Integrated Security=False;Trusted_Connection=False;");
            }
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Dias>(entity =>
            {
                entity.HasKey(e => e.DiaId);

                entity.Property(e => e.DiaId).HasColumnName("dia_id");

                entity.Property(e => e.DiaEstado)
                    .IsRequired()
                    .HasColumnName("dia_estado")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.DiaFalta)
                    .HasColumnName("dia_falta")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.DiaNombre)
                    .IsRequired()
                    .HasColumnName("dia_nombre")
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<Generar>(entity =>
            {
                entity.HasKey(e => e.GenId);

                entity.Property(e => e.GenId).HasColumnName("gen_id");

                entity.Property(e => e.BoxId).HasColumnName("box_id");

                entity.Property(e => e.DiaId).HasColumnName("dia_id");

                entity.Property(e => e.GenEstado)
                    .IsRequired()
                    .HasColumnName("gen_estado")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.GenFalta)
                    .HasColumnName("gen_falta")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.GenHorafin)
                    .IsRequired()
                    .HasColumnName("gen_horafin")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.GenHoraini)
                    .IsRequired()
                    .HasColumnName("gen_horaini")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.GenInterva).HasColumnName("gen_interva");

                entity.Property(e => e.TurId).HasColumnName("tur_id");
            });

            modelBuilder.Entity<Horarios>(entity =>
            {
                entity.HasKey(e => e.HorId);

                entity.Property(e => e.HorId).HasColumnName("hor_id");

                entity.Property(e => e.BoxId).HasColumnName("box_id");

                entity.Property(e => e.HorDia)
                    .IsRequired()
                    .HasColumnName("hor_dia")
                    .HasMaxLength(10);

                entity.Property(e => e.HorEstado)
                    .IsRequired()
                    .HasColumnName("hor_estado")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.HorFalta)
                    .HasColumnName("hor_falta")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.HorFecha)
                    .HasColumnName("hor_fecha")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.HorHora)
                    .IsRequired()
                    .HasColumnName("hor_hora")
                    .HasMaxLength(10);

                entity.Property(e => e.HorReserv).HasColumnName("hor_reserv");

                entity.Property(e => e.PerId).HasColumnName("per_id");

                entity.Property(e => e.TurId).HasColumnName("tur_id");
            });

            modelBuilder.Entity<Personas>(entity =>
            {
                entity.HasKey(e => e.PerId);

                entity.Property(e => e.PerId).HasColumnName("per_id");

                entity.Property(e => e.PerDni)
                    .IsRequired()
                    .HasColumnName("per_dni")
                    .HasMaxLength(11);

                entity.Property(e => e.PerEmail)
                    .IsRequired()
                    .HasColumnName("per_email")
                    .HasMaxLength(40);

                entity.Property(e => e.PerEstado)
                    .IsRequired()
                    .HasColumnName("per_estado")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.PerFalta)
                    .HasColumnName("per_falta")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.PerNombre)
                    .IsRequired()
                    .HasColumnName("per_nombre")
                    .HasMaxLength(40);

                entity.Property(e => e.PerTelef)
                    .IsRequired()
                    .HasColumnName("per_telef")
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Turnos>(entity =>
            {
                entity.HasKey(e => e.TurId);

                entity.Property(e => e.TurId).HasColumnName("tur_id");

                entity.Property(e => e.TurEstado)
                    .IsRequired()
                    .HasColumnName("tur_estado")
                    .HasMaxLength(1)
                    .IsUnicode(false);

                entity.Property(e => e.TurFalta)
                    .HasColumnName("tur_falta")
                    .HasColumnType("smalldatetime");

                entity.Property(e => e.TurNombre)
                    .IsRequired()
                    .HasColumnName("tur_nombre")
                    .HasMaxLength(50);
            });

            modelBuilder.Query<sp_ListarTurnos>();
            modelBuilder.Query<sp_ListarHorarioPorFechaHora>();

        }

        #region procedimientos almacenados

        public async Task<IEnumerable<sp_ListarTurnos>> ListarTurnos()
        {
            List<sp_ListarTurnos> lista = new List<sp_ListarTurnos>();

            try
            {
                string sqlQuery = "EXEC [dbo].[sp_listarTurnos] ";
                lista = await this.Query<sp_ListarTurnos>().FromSql(sqlQuery).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
            return lista;
        }

        /// <summary>
        /// Listar horarios por fecha, para fecha del dia actual va con hora
        /// </summary>
        /// <param name="turnoId"></param>
        /// <param name="fecha"> aaaammdd</param>
        /// <param name="hora">hh:mm:ss</param>
        /// <returns></returns>
        public async Task<IEnumerable<sp_ListarHorarioPorFechaHora>> ListarHorariosPorFecha(int turnoId, string fecha, string hora = null)
        {
            List<sp_ListarHorarioPorFechaHora> lista = new List<sp_ListarHorarioPorFechaHora>();

            try
            {
                string sqlQuery = string.Empty;
                SqlParameter turnoIdParam = new SqlParameter("@tur_id", turnoId);
                SqlParameter fechaParam = new SqlParameter("@hor_fecha", fecha);

                if (hora == null)
                {

                    sqlQuery = "EXEC [dbo].[sp_ListarHorarioPorFecha] " + "@tur_id, " + "@hor_fecha";

                    lista = await this.Query<sp_ListarHorarioPorFechaHora>().FromSql(sqlQuery, turnoIdParam, fechaParam).ToListAsync();
                }
                else
                {
                    SqlParameter horaParam = new SqlParameter("@hor_hora", hora);
                    sqlQuery = "EXEC [dbo].[sp_ListarHorarioPorFechaHora] " + "@tur_id, " + "@hor_fecha, " + "@hor_hora";
                    lista = await this.Query<sp_ListarHorarioPorFechaHora>().FromSql(sqlQuery, turnoIdParam, fechaParam, horaParam).ToListAsync();
                }


            }
            catch (Exception)
            {

                throw;
            }
            return lista;
        }
        #endregion
    }
}
