using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;
using Microsoft.Data.SqlClient;
using System.Data;

namespace INFRASTRUCTURE.Repository
{
    public class CitasRepository : ICitasRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>

        private DBconnecctionFactory _dBconnecttionFactory;

        public CitasRepository(DBconnecctionFactory dbconnecttionFactory)
        {
            _dBconnecttionFactory = dbconnecttionFactory;
        }

        public async Task<IEnumerable<Citas>> ListarCitasAsync()
        {
            var olist = new List<Citas>();
            using var  con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using(SqlCommand cmd = new  SqlCommand("SP_LISTAR_CITAS", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while(await dr.ReadAsync())
                    {
                        olist.Add(new Citas
                        {
                            ID_CITAS = Convert.ToInt32(dr["ID_CITAS"]),
                            ID_PACIENTE = Convert.ToInt32(dr["ID_PACIENTE"]),
                            ID_SUCURSAL = Convert.ToInt32(dr["ID_SUCURSAL"]),
                            ID_VETERINARIO = Convert.ToInt32(dr["ID_VETERIANRIO"]),
                            FECHA_HORA = Convert.ToDateTime(dr["FECHA_HORA"]),
                            ORIGEN_CONTACTO = dr["ORIGEN_CONTACTO"].ToString(),
                            ESTADO_CITA = dr["ESTADP_CITA"].ToString()
                        });
                    }
                }
                return olist;
            }
            
        }

        public async Task NuevaCita(Citas oCitas)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

                using (SqlCommand cmd = new SqlCommand("SP_NUEVA_CITA", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add(new SqlParameter("@ID_PACIENTE", oCitas.ID_PACIENTE));
                    cmd.Parameters.Add(new SqlParameter("@ID_SUCURSAL",oCitas.ID_SUCURSAL));
                    cmd.Parameters.Add(new SqlParameter("@ID_VETERINARIO", oCitas.ID_VETERINARIO));
                    cmd.Parameters.Add(new SqlParameter("@FECHA_HORA", oCitas.FECHA_HORA));
                    cmd.Parameters.Add(new SqlParameter("@ORIGEN_CONTACTO", oCitas.ORIGEN_CONTACTO));

                    await cmd.ExecuteNonQueryAsync();
                }
        }
        public  async Task EditarCitas(Citas oCitas)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_EDITAR_CITA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", oCitas.ID_CITAS));
                cmd.Parameters.Add(new SqlParameter("@ID_PACIENTE", oCitas.ID_CITAS));
                cmd.Parameters.Add(new SqlParameter("@ID_SUCURSAL", oCitas.ID_CITAS));
                cmd.Parameters.Add(new SqlParameter("@ID_VETERIANRIO", oCitas.ID_CITAS));
                cmd.Parameters.Add(new SqlParameter("@FECHA_HORA", oCitas.ID_CITAS));
                cmd.Parameters.Add(new SqlParameter("@ORIGEN_CONTACTO", oCitas.ID_CITAS));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarCitas(int id)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_CANCELAR_CITA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", id));

                await cmd.ExecuteNonQueryAsync();
            }
        }

       
    }
}
