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

        public Task<IEnumerable<Enmienda>> ListarEnmiendaAsync()
        {
            throw new NotImplementedException();
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
