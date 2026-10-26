using APLICATION.Interfaces;
using DOMAIN.Entities;
using INFRASTRUCTURE.DataBase;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace INFRASTRUCTURE.Repository
{
    public class ServicioRepository : IServicioRepository
    {

        private DBconnecctionFactory _dBconnecttionFactory;

        public ServicioRepository(DBconnecctionFactory dbconnecttionFactory)
        {
            _dBconnecttionFactory= dbconnecttionFactory;
        }

        public async Task<IEnumerable<Servicio>> ListarServicioAsync()
        {
            var olist = new List<Servicio>();
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_LISTAR_SERVICIOS", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        olist.Add(new Servicio
                        {
                            ID_SERVICIO = Convert.ToInt32(dr["ID_SERVICIO"]),
                            NOMBRE_SERVICIO = dr["NOMBRE_SERVICIO"].ToString(),
                            CATEGORIA = dr["CATEGORIA"].ToString(),
                            DESCRIPCION_DE_SERVICIO = dr["DESCRIPCION"].ToString(),
                            PRECIO = Convert.ToInt32(dr["PRECIO"]),
                            ESTADO = dr["ESTADO"].ToString()
                        });
                    }
                }
                return olist;
            }
        }

        public async Task NuevoServicio(Servicio oServicio)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_NUEVO_SERVICIO", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@NOMBRE", oServicio.NOMBRE_SERVICIO));
                cmd.Parameters.Add(new SqlParameter("@CATEGORIA", oServicio.CATEGORIA));
                cmd.Parameters.Add(new SqlParameter("@DESCRIPCION", oServicio.DESCRIPCION_DE_SERVICIO));
                cmd.Parameters.Add(new SqlParameter("@PRECIO", oServicio.PRECIO));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task EditarServicio(Servicio oServicio)
        {
            using var con = _dBconnecttionFactory.CreateConnection();
            await con.OpenAsync();

            using (SqlCommand cmd = new SqlCommand("SP_EDITAR_SERVICIO", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@ID", oServicio.ID_SERVICIO));
                cmd.Parameters.Add(new SqlParameter("@NOMBRE", oServicio.NOMBRE_SERVICIO));
                cmd.Parameters.Add(new SqlParameter("@CATEGORIA", oServicio.CATEGORIA));
                cmd.Parameters.Add(new SqlParameter("@DESCRIPCION", oServicio.DESCRIPCION_DE_SERVICIO));
                cmd.Parameters.Add(new SqlParameter("@PRECIO", oServicio.PRECIO));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public Task EliminaServicio(int id)
        {
            throw new NotImplementedException();
        }

       
    }
}
