using System;
using System.Collections.Generic;
using System.Text;
using DOMAIN.Entities;

namespace APLICATION.Interfaces
{
    public interface IEnmiendaRepository
    {
        Task<IEnumerable<Enmienda>> ListarEnmiendaAsync();
        Task NuevoEnmiendaAsync(Enmienda oEnmienda);
        Task EditarEnmiendaAsync(Enmienda oEnmienda);
        Task EliminarEnmienda(int id);

        /// <summary>
        /// Interfaces: Definen los contratos que deben cumplir las demás capas (Inversión de Dependencias). 
        /// Aquí se declaran los métodos que los Repositorios y Servicios deben implementar, garantizando que la capa de Aplicación no dependa de implementaciones concretas.
        /// </summary>
    }
}
