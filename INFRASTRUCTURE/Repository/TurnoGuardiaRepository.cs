using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace INFRASTRUCTURE.Repository
{
    public class TurnoGuardiaRepository : ITurnoGuardiaRepository
    {

        private DBconnecctionFactory _dBconnecttionFactory;
        public TurnoGuardiaRepository(DBconnecctionFactory dbconnecttionFactory)
        {
            _dBconnecttionFactory = dbconnecttionFactory;
        }

        public async Task<IEnumerable<TurnoGuardia>> ListarTurnoAsync()
        {
            var olist = new List<TurnoGuardia>();
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_GUARDIA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new TurnoGuardia
                        {
                            ID_TURNO = Convert.ToInt32(dr["ID_CLIENTE"]),
                            ID_VETERINARIO = Convert.ToInt32(dr["ID_VETERINARIO"]),
                            FECHA = Convert.ToDateTime(dr["FECHA"]),
                            HORA_INICIO = TimeOnly.FromTimeSpan((TimeSpan)dr["HORA_INICIO"]),
                            HORA_FIN = TimeOnly.FromTimeSpan((TimeSpan)dr["HORA_FIN"]),
                            Estado = dr["Estado"].ToString()
                        });
                    }
                }
                return olist;
            }
        }

        public async Task NuevoTurno(TurnoGuardia oTurno)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVA_GUARDIA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NOMBRE_COMPLETO", oTurno.ID_VETERINARIO));
                cmd.Parameters.Add(new SqlParameter("@TELEFONO", oTurno.FECHA));
                cmd.Parameters.Add(new SqlParameter("@WHATSAPP", oTurno.HORA_INICIO));
                cmd.Parameters.Add(new SqlParameter("@EMAIL", oTurno.HORA_FIN));
                

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarTurno(TurnoGuardia oTurno)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_EDITAR_GUARDIA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", oTurno.ID_TURNO));
                cmd.Parameters.Add(new SqlParameter("@ID_VETERINARIO", oTurno.ID_VETERINARIO));
                cmd.Parameters.Add(new SqlParameter("@FECHA", oTurno.FECHA));
                cmd.Parameters.Add(new SqlParameter("@HORA_INICIO", oTurno.HORA_INICIO));
                cmd.Parameters.Add(new SqlParameter("@HORA_FIN", oTurno.HORA_FIN));


                await cmd.ExecuteNonQueryAsync();
            }
        }

        public Task EliminarTurno(int id)
        {
            throw new NotImplementedException();
        }

      
    }
}
