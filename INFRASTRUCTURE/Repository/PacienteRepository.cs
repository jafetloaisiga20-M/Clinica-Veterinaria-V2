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
    public class PacienteRepository:IPacienteRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>


        private DBconnecctionFactory _dBconnecttionFactory;

        public PacienteRepository(DBconnecctionFactory bconnecttionFactory)
        {
            _dBconnecttionFactory = bconnecttionFactory;
        }

        public async Task<IEnumerable<Paciente>> ListarPacienteAsync()
        {
            var olist = new List<Paciente>();
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_PACIENTE", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Paciente
                        {
                            ID_PACIENTE = Convert.ToInt32(dr["ID_PACIENTE"]),
                            ID_CLIENTE = Convert.ToInt32(dr["ID_CLIENTE"]),
                            NOMBRE = dr["NOMBRE"].ToString(),
                            ESPECIE = dr["ESPECIE"].ToString(),
                            RAZA = dr["RAZA"].ToString(),
                            SEXO = dr["SEXO"].ToString(),
                            FECHA_NACIMIENTO = Convert.ToDateTime(dr["ID_CLIENTE"]),
                            PESO_ACTUAL = Convert.ToDecimal(dr["ID_CLIENTE"]),
                            ESTADO = dr["ESTADO"].ToString()
                        });
                    }
                }
                return olist;
            }
        }

        public async Task NuevoPaciente(Paciente oPaciente)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVO_PACIENTE", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID_CLIENTE", oPaciente.ID_CLIENTE));
                cmd.Parameters.Add(new SqlParameter("@NOMBRE", oPaciente.NOMBRE));
                cmd.Parameters.Add(new SqlParameter("@ESPECIE", oPaciente.ESPECIE));
                cmd.Parameters.Add(new SqlParameter("@RAZA", oPaciente.RAZA));
                cmd.Parameters.Add(new SqlParameter("@SEXO", oPaciente.SEXO));
                cmd.Parameters.Add(new SqlParameter("@FECHA_NACIMIENTO", oPaciente.FECHA_NACIMIENTO));
                cmd.Parameters.Add(new SqlParameter("@PESO_ACTUAL", oPaciente.PESO_ACTUAL));

                await cmd.ExecuteNonQueryAsync();
            }
        }


        public async Task EditarPaciente(Paciente oPaciente)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_EDITAR_PACIENTE", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", oPaciente.ID_PACIENTE));
                cmd.Parameters.Add(new SqlParameter("@ID_CLIENTE", oPaciente.ID_CLIENTE));
                cmd.Parameters.Add(new SqlParameter("@NOMBRE", oPaciente.NOMBRE));
                cmd.Parameters.Add(new SqlParameter("@ESPECIE", oPaciente.ESPECIE));
                cmd.Parameters.Add(new SqlParameter("@RAZA", oPaciente.RAZA));
                cmd.Parameters.Add(new SqlParameter("@SEXO", oPaciente.SEXO));
                cmd.Parameters.Add(new SqlParameter("@FECHA_NACIMIENTO", oPaciente.FECHA_NACIMIENTO));
                cmd.Parameters.Add(new SqlParameter("@PESO_ACTUAL", oPaciente.PESO_ACTUAL));


                await cmd.ExecuteNonQueryAsync();
            }
        }

        public Task EliminarPaciente(int id)
        {
            throw new NotImplementedException();
        }

       
    }
}
