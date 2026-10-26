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
    public class VeterinarioRepository : IVeterinarioRepository
    {
        private readonly DBconnecctionFactory _conncetionFactory;

        public VeterinarioRepository(DBconnecctionFactory conncetionFactory)
        {
            _conncetionFactory = conncetionFactory;
        }

        //listar
        public async Task<IEnumerable<Veterinario>> ListarVeterinariosAsync()
        {
            var olist = new List<Veterinario>();
            using var con = _conncetionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_VETERINARIO", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Veterinario
                        {
                            ID_VETERINARIO = Convert.ToInt32(dr["ID_VETERINARIO"]),
                            ID_SUCURSAL = Convert.ToInt32(dr["ID_SUCURSAL"]),
                            NOMBRE_COMPLETO = dr["NOMBRE_COMPLETO"].ToString(),
                            ESPECIALIDAD = dr["ESPECIALIDAD"].ToString(),
                            CV_DESCRIPCION = dr["CV_DESCRIPCION"].ToString(),
                            FOTO_URL = dr["FOTO_URL"].ToString(),
                            TELEFONO = dr["TELEFONO"].ToString(),
                            ESTADO = dr["ESTADO"].ToString()
                        });
                    }
                }
                return olist;
            }
        }

        //Insertar
        public Task NuevoVeterinario(Veterinario oVeterinario)
        {
            throw new NotImplementedException();
        }

        //editar
        public async Task EditarVeterinario(Veterinario oVeterinario)
        {
            using var con = _conncetionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_EDITAR_VETERINARIO", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", oVeterinario.ID_VETERINARIO));
                cmd.Parameters.Add(new SqlParameter("@NOMBRE_COMPLETO", oVeterinario.NOMBRE_COMPLETO));
                cmd.Parameters.Add(new SqlParameter("@ESPECIALIDAD", oVeterinario.ESPECIALIDAD));
                cmd.Parameters.Add(new SqlParameter("@CV_DESCRIPCION", oVeterinario.CV_DESCRIPCION));
                cmd.Parameters.Add(new SqlParameter("@FOTO_URL", oVeterinario.FOTO_URL));
                cmd.Parameters.Add(new SqlParameter("@TELEFONO", oVeterinario.TELEFONO));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        //ELiminar
        public async Task EliminarVeterinario(int id)
        {
            using var con = _conncetionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_ELIMINAR_VETERINARIO", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", id));

                await cmd.ExecuteNonQueryAsync();
            }
        }

      
    }
}

