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
    public class EnmiendaMedicaRepository : IEnmiendaRepository
    {

        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>

        private DBconnecctionFactory _dBconnecttionFactory;

        public EnmiendaMedicaRepository(DBconnecctionFactory bconnecttionFactory)
        {
            _dBconnecttionFactory = bconnecttionFactory;
        }

        public async Task<IEnumerable<Enmienda>> ListarEnmiendaAsync()
        {
            var list = new List<Enmienda>();
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();
            using var cmd = new SqlCommand(
                "SELECT ID_ENMIENDA, ID_EXPEDIENTE, ID_VETERINARIO, NOTA_ADICIONAL, FECHA_ENMIENDA FROM dbo.ENMIENDA ORDER BY FECHA_ENMIENDA DESC", con);
            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                list.Add(new Enmienda
                {
                    ID_ENMIENDA = Convert.ToInt32(dr["ID_ENMIENDA"]),
                    ID_EXPEDIENTE = Convert.ToInt32(dr["ID_EXPEDIENTE"]),
                    ID_VETERINARIO = Convert.ToInt32(dr["ID_VETERINARIO"]),
                    NOTA_ADICIONAL = dr["NOTA_ADICIONAL"].ToString() ?? "",
                    FECHA_ENMIENDA = Convert.ToDateTime(dr["FECHA_ENMIENDA"])
                });
            }
            return list;
        }

        public async Task NuevoEnmiendaAsync(Enmienda oEnmienda)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVA_ENMIENDA", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID_EXPEDIENTE", oEnmienda.ID_EXPEDIENTE));
                cmd.Parameters.Add(new SqlParameter("@ID_VETERINARIO", oEnmienda.ID_VETERINARIO));
                cmd.Parameters.Add(new SqlParameter("@NOTA_ADICIONAL", oEnmienda.NOTA_ADICIONAL));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public Task EditarEnmiendaAsync(Enmienda oEnmienda)
        {
            throw new NotImplementedException();
        }

        public Task EliminarEnmienda(int id)
        {
            throw new NotImplementedException();
        }

     
    }
}
