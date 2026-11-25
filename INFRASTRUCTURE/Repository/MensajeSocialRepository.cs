using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;
using Microsoft.Data.SqlClient;

namespace INFRASTRUCTURE.Repository
{
    public class MensajeSocialRepository : IMensajeSocialRepository
    {
        private readonly DBconnecctionFactory _factory;

        public MensajeSocialRepository(DBconnecctionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<MensajeSocial>> ListarAsync()
        {
            var list = new List<MensajeSocial>();
            using var con = _factory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT ID_MENSAJE, CANAL, REMITENTE, TEXTO, FECHA_RECEPCION, ESTADO, ID_CITA FROM dbo.MENSAJE_SOCIAL ORDER BY FECHA_RECEPCION DESC",
                con);
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                list.Add(new MensajeSocial
                {
                    ID_MENSAJE = Convert.ToInt32(dr["ID_MENSAJE"]),
                    CANAL = dr["CANAL"].ToString() ?? "",
                    REMITENTE = dr["REMITENTE"].ToString() ?? "",
                    TEXTO = dr["TEXTO"].ToString() ?? "",
                    FECHA_RECEPCION = Convert.ToDateTime(dr["FECHA_RECEPCION"]),
                    ESTADO = dr["ESTADO"].ToString() ?? "",
                    ID_CITA = dr["ID_CITA"] == DBNull.Value ? null : Convert.ToInt32(dr["ID_CITA"])
                });
            }
            return list;
        }

        public async Task NuevoAsync(MensajeSocial mensaje)
        {
            using var con = _factory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand(
                "INSERT INTO dbo.MENSAJE_SOCIAL (CANAL, REMITENTE, TEXTO, FECHA_RECEPCION, ESTADO, ID_CITA) " +
                "VALUES (@CANAL, @REMITENTE, @TEXTO, @FECHA, @ESTADO, @ID_CITA)", con);
            cmd.Parameters.Add(new SqlParameter("@CANAL", mensaje.CANAL));
            cmd.Parameters.Add(new SqlParameter("@REMITENTE", mensaje.REMITENTE));
            cmd.Parameters.Add(new SqlParameter("@TEXTO", mensaje.TEXTO));
            cmd.Parameters.Add(new SqlParameter("@FECHA", mensaje.FECHA_RECEPCION));
            cmd.Parameters.Add(new SqlParameter("@ESTADO", mensaje.ESTADO));
            if (mensaje.ID_CITA.HasValue)
                cmd.Parameters.Add(new SqlParameter("@ID_CITA", mensaje.ID_CITA.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@ID_CITA", DBNull.Value));
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task MarcarAtendidoAsync(int id, int? idCita)
        {
            using var con = _factory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand(
                "UPDATE dbo.MENSAJE_SOCIAL SET ESTADO = N'Atendido', ID_CITA = @ID_CITA WHERE ID_MENSAJE = @ID", con);
            cmd.Parameters.Add(new SqlParameter("@ID", id));
            if (idCita.HasValue)
                cmd.Parameters.Add(new SqlParameter("@ID_CITA", idCita.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@ID_CITA", DBNull.Value));
            await cmd.ExecuteNonQueryAsync();
        }
    }
}