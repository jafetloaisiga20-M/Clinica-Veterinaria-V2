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
    public class ExpedienteMedicoRepository : IExpedienteMedicoRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>

        private DBconnecctionFactory _dBconnecttionFactory;

        public ExpedienteMedicoRepository(DBconnecctionFactory bconnecttionFactory)
        {
            _dBconnecttionFactory = bconnecttionFactory;
        }


        public async Task<IEnumerable<ExpedienteMedico>> ListarExpedienteAsync()
        {
            var olist = new List<ExpedienteMedico>();
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_EXPEDIENTE_MEDICO", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new ExpedienteMedico
                        {
                            ID_EXPEDIENTE = Convert.ToInt32(dr["ID_EXPEDIENTE"]),
                            ID_PACIENTE = Convert.ToInt32(dr["ID_PACIENTE"]),
                            ID_CITAS = Convert.ToInt32(dr["ID_CITAS"]),
                            MOTIVO_VISITA = dr["MOTIVO_VISITA"].ToString(),
                            SINTOMAS = dr["SINTOMAS"].ToString(),
                            DIAGNOSTICO = dr["DIAGNOSTICO"].ToString(),
                            TRATAMIENTOS = dr["TRATAMIENTOS"].ToString(),
                            RECETA_MEDICA = dr["RECETA_MEDICA"].ToString(),
                            FECHA_REGISTRO = Convert.ToDateTime(dr["FECHA_REGISTRO"]),
                        });
                    }
                }
                return olist;
            }
        }

        public async Task NuevoExpediente(ExpedienteMedico oExpediente)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVO_EXPEDIENTE", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID_PACIENTE", oExpediente.ID_PACIENTE));
                cmd.Parameters.Add(new SqlParameter("@ID_CITAS", oExpediente.ID_CITAS));
                cmd.Parameters.Add(new SqlParameter("@MOTIVO_VISITA", oExpediente.MOTIVO_VISITA));
                cmd.Parameters.Add(new SqlParameter("@SINTOMAS", oExpediente.SINTOMAS));
                cmd.Parameters.Add(new SqlParameter("@DIAGNOSTICO", oExpediente.DIAGNOSTICO));
                cmd.Parameters.Add(new SqlParameter("@TRATAMIENTOS", oExpediente.TRATAMIENTOS));
                cmd.Parameters.Add(new SqlParameter("@RECETA_MEDICA", oExpediente.RECETA_MEDICA));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public Task EditarExpediente(ExpedienteMedico oExpediente)
        {
            throw new NotImplementedException();
        }

        public Task EliminarExpediente(int id)
        {
            throw new NotImplementedException();
        }

      
    }
}
