using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class ServiciosServices
    {
        private readonly IServicioRepository _repository;

        public ServiciosServices(IServicioRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<ServicioDTOs>> ListarServicios()
        {
            var lista = await _repository.ListarServicioAsync();
            return lista.Select(p => new ServicioDTOs
            {
               ID_SERVICIO = p.ID_SERVICIO,
               NOMBRE_SERVICIO = p.NOMBRE_SERVICIO,
               CATEGORIA = p.CATEGORIA,
               DESCRIPCION_DE_SERVICIO = p.DESCRIPCION_DE_SERVICIO,
               PRECIO = p.PRECIO,
               ESTADO = p.ESTADO
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevaServicio(ServicioDTOs Ser)
        {
            var oServicio = new Servicio
            {
                NOMBRE_SERVICIO = Ser.NOMBRE_SERVICIO,
                CATEGORIA = Ser.CATEGORIA,
                DESCRIPCION_DE_SERVICIO = Ser.DESCRIPCION_DE_SERVICIO,
                PRECIO = Ser.PRECIO,
                ESTADO = Ser.ESTADO
            };
            await _repository.NuevoServicio(oServicio);
        }

        //Editar ---------------------------------------------------------------------
        public async Task EditarServicio(ServicioDTOs Ser)
        {
            var oServicio = new Servicio
            {
                ID_SERVICIO = Ser.ID_SERVICIO,
                NOMBRE_SERVICIO = Ser.NOMBRE_SERVICIO,
                CATEGORIA = Ser.CATEGORIA,
                DESCRIPCION_DE_SERVICIO = Ser.DESCRIPCION_DE_SERVICIO,
                PRECIO = Ser.PRECIO,
                ESTADO = Ser.ESTADO
            };
            await _repository.EditarServicio(oServicio);
        }

        //Eliminar ---------------------------------------------------------------------
        public async Task ELiminarServicio(int id)
        {
            await _repository.EliminaServicio(id);
        }
    }
}
