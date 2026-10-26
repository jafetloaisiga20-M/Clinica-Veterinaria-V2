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
    public class ClienteRepository : IClienteRepository
    {
        /// <summary>
        /// Repositorio: Implementación concreta de las interfaces de acceso a datos definidas en la capa de Aplicación o Dominio. 
        /// Aquí reside el código directo de ADO.NET (consultas SQL, ejecución de procedimientos almacenados y mapeo de SqlDataReader a Entidades).
        /// </summary>

        private DBconnecctionFactory _dBconnecttionFactory;

        public ClienteRepository(DBconnecctionFactory dbconnecttionFactory)
        {
            _dBconnecttionFactory = dbconnecttionFactory;
        }

        public async Task EditarClienteAsync(Cliente oCliente)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_EDITAR_CLIENTE", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", oCliente.ID_CLIENTE));
                cmd.Parameters.Add(new SqlParameter("@NOMBRE_COMPLETO", oCliente.NOMBRE_COMPLETO));
                cmd.Parameters.Add(new SqlParameter("@TELEFONO", oCliente.TELEFONO));
                cmd.Parameters.Add(new SqlParameter("@WHATSAPP", oCliente.WHATSAPP));
                cmd.Parameters.Add(new SqlParameter("@EMAIL", oCliente.EMAIL));
                cmd.Parameters.Add(new SqlParameter("@DIRECCION", oCliente.DIRECCION));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EliminarClienteAsync(int id)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_ELIMINAR_CLIENTE", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID",id));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<Cliente>> ListarClientesAsync()
        {
            var olist = new List<Cliente>();
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_CLIENTES", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while(await dr.ReadAsync())
                    {
                        olist.Add(new Cliente
                        {
                            ID_CLIENTE = Convert.ToInt32(dr["ID_CLIENTE"]),
                            NOMBRE_COMPLETO = dr["NOMBRE_COMPLETO"].ToString(),
                            TELEFONO = dr["TELEFONO"].ToString(),
                            WHATSAPP = dr["WHATSAPP"].ToString(),
                            EMAIL = dr["EMAIL"].ToString(),
                            DIRECCION = dr["DIRECCION"].ToString(),
                            ESTADO = dr["ESTADO"].ToString()
                        });
                    }
                }
                return olist;
            }
        }

        public async Task NuevoClienteAsync(Cliente oCliente)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using(SqlCommand cmd = new SqlCommand("SP_NUEVO_CLIENTE",con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NOMBRE_COMPLETO", oCliente.NOMBRE_COMPLETO));
                cmd.Parameters.Add(new SqlParameter("@TELEFONO", oCliente.TELEFONO));
                cmd.Parameters.Add(new SqlParameter("@WHATSAPP", oCliente.WHATSAPP));
                cmd.Parameters.Add(new SqlParameter("@EMAIL", oCliente.EMAIL));
                cmd.Parameters.Add(new SqlParameter("@DIRECCION", oCliente.DIRECCION));

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
