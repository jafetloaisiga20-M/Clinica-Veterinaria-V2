using System;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class SucursalServices
    {
        private readonly ISucursalRepository _repository;

        public SucursalServices(ISucursalRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<SucursalDTOs>> ListarSucursal()
        {
            var lista = await _repository.ListarSucursalAsync();
            return lista.Select(p => new SucursalDTOs
            {
              ID_SUCURSAL = p.ID_SUCURSAL,
              NOMBRE_SUCURSAL = p.NOMBRE_SUCURSAL,
              DIRECCION = p.DIRECCION,
              CIUDAD = p.CIUDAD,
              TELEFONO = p.TELEFONO,
              ESTADO = p.ESTADO
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevaSucursal(SucursalDTOs Suc)
        {
            var osucursal = new Sucursal
            {
                NOMBRE_SUCURSAL = Suc.NOMBRE_SUCURSAL,
                DIRECCION = Suc.DIRECCION,
                CIUDAD = Suc.CIUDAD,
                TELEFONO = Suc.TELEFONO,
                ESTADO = Suc.ESTADO
            };
            await _repository.NuevaSucursal(osucursal);
        }

        //Editar ---------------------------------------------------------------------
        public async Task EditarSucursal(SucursalDTOs Suc)
        {
            var osucursal = new Sucursal
            {
                ID_SUCURSAL = Suc.ID_SUCURSAL,
                NOMBRE_SUCURSAL = Suc.NOMBRE_SUCURSAL,
                DIRECCION = Suc.DIRECCION,
                CIUDAD = Suc.CIUDAD,
                TELEFONO = Suc.TELEFONO,
                ESTADO = Suc.ESTADO
            };
            await _repository.EditarSucursal(osucursal);
        }

        //Eliminar ---------------------------------------------------------------------
        //public async Task ELiminarSucursal(int id)
        //{
        //    await _repository.EliminarSucursal(id);
        //}
    }
}
