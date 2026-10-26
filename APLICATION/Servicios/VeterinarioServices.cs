using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using APLICATION.DTOs;
using APLICATION.Interfaces;
using DOMAIN.Entities;

namespace APLICATION.Servicios
{
    public class VeterinarioServices
    {
        private readonly IVeterinarioRepository _repository;

        public VeterinarioServices(IVeterinarioRepository repository)
        {
            _repository = repository;
        }

        //Listar ---------------------------------------------------------------------
        public async Task<IEnumerable<VeterinarioDTOs>> ListarVeterinarios()
        {
            var lista = await _repository.ListarVeterinariosAsync();
            return lista.Select(p => new VeterinarioDTOs
            {
                ID_VETERINARIO = p.ID_VETERINARIO,
                ID_SUCURSAL = p.ID_SUCURSAL,
                NOMBRE_COMPLETO = p.NOMBRE_COMPLETO,
                ESPECIALIDAD = p.ESPECIALIDAD,
                CV_DESCRIPCION = p.CV_DESCRIPCION,
                FOTO_URL = p.FOTO_URL,
                TELEFONO = p.TELEFONO,
                ESTADO = p.ESTADO
            });
        }

        //insertar ---------------------------------------------------------------------
        public async Task NuevoVeterinario(VeterinarioDTOs vet)
        {
            var oVet = new Veterinario
            {
                ID_SUCURSAL = vet.ID_SUCURSAL,
                NOMBRE_COMPLETO = vet.NOMBRE_COMPLETO,
                ESPECIALIDAD = vet.ESPECIALIDAD,
                CV_DESCRIPCION = vet.CV_DESCRIPCION,
                FOTO_URL = vet.FOTO_URL,
                TELEFONO = vet.TELEFONO,
                ESTADO = vet.ESTADO
            };
            await _repository.NuevoVeterinario(oVet);
        }

        //Editar ---------------------------------------------------------------------
        public async Task EditarVeterinario(VeterinarioDTOs vet)
        {
            var oVet = new Veterinario
            {
                ID_VETERINARIO = vet.ID_VETERINARIO,
                ID_SUCURSAL = vet.ID_SUCURSAL,
                NOMBRE_COMPLETO = vet.NOMBRE_COMPLETO,
                ESPECIALIDAD = vet.ESPECIALIDAD,
                CV_DESCRIPCION = vet.CV_DESCRIPCION,
                FOTO_URL = vet.FOTO_URL,
                TELEFONO = vet.TELEFONO,
                ESTADO = vet.ESTADO
            };
            await _repository.EditarVeterinario(oVet);
        }

        //Eliminar ---------------------------------------------------------------------
        public async Task ELiminarVeterinario(int id)
        {
            await _repository.EliminarVeterinario(id);
        }

    }
}
