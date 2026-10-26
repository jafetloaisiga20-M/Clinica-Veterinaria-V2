using APLICATION.Interfaces;
using APLICATION.Servicios;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace INFRASTRUCTURE.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {

        private DBconnecctionFactory _dbConnectionFactory;

        public UsuarioRepository(DBconnecctionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Usuario>> ListarUsuarioAsync()
        {
            var olist = new List<Usuario>();
            using var con = _dbConnectionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_USUARIOS", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Usuario
                        {
                            ID_USUARIO = Convert.ToInt32(dr["ID_USUARIO"]),
                            ID_ROL = Convert.ToInt32(dr["ID_ROL"]),
                            ID_SUCURSAL = Convert.ToInt32(dr["ID_SUCURSAL"]),
                            NOMBRE_USUARIO = dr["NOMBRE_USUARIO"].ToString(),
                            PASSWORD_HASH = dr["PASSWORD_HASH"].ToString(),
                            ESTADO = dr["ESTADO"].ToString()
                        });
                    }
                }
                return olist;
            }
        }

        public async Task NuevoUsuario(Usuario oUsuario)
        {
            using var con = _dbConnectionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVO_USUARIO", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID_ROL", oUsuario.ID_ROL));
                cmd.Parameters.Add(new SqlParameter("@ID_SUCURSAL", oUsuario.ID_SUCURSAL));
                cmd.Parameters.Add(new SqlParameter("@NOMBRE_USUARIO", oUsuario.NOMBRE_USUARIO));
                cmd.Parameters.Add(new SqlParameter("@PASSWORD_HASH", oUsuario.PASSWORD_HASH));


                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarUsuario(Usuario oUsuario)
        {
            using var con = _dbConnectionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_EDITAR_USUARIO", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", oUsuario.ID_USUARIO));
                cmd.Parameters.Add(new SqlParameter("@ID_ROL", oUsuario.ID_ROL));
                cmd.Parameters.Add(new SqlParameter("@ID_SUCURSAL", oUsuario.ID_SUCURSAL));
                cmd.Parameters.Add(new SqlParameter("@NOMBRE_USUARIO", oUsuario.NOMBRE_USUARIO));
                cmd.Parameters.Add(new SqlParameter("@PASSWORD_HASH", oUsuario.PASSWORD_HASH));


                await cmd.ExecuteNonQueryAsync();
            }
        }

        public Task EliminarUsuario(int id)
        {
            throw new NotImplementedException();
        }

      
    }
}
