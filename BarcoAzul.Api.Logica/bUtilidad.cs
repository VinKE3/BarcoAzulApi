using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;


namespace BarcoAzul.Api.Logica
{
    public class bUtilidad
    {
        public static IEnumerable<DepartamentoViewDTO> ListarDepartamentosProvinciasDistritos(
            IEnumerable<oDepartamento> departamentos,
            IEnumerable<oProvincia> provincias,
            IEnumerable<oDistrito> distritos = null)
        {
            return departamentos.Select(x => new DepartamentoViewDTO
            {
                Id = x.Id,
                Nombre = x.Nombre,
                Provincias = provincias?.Where(y => y.DepartamentoId == x.Id).Select(y => new ProvinciaViewDTO
                {
                    Id = y.ProvinciaId,
                    Nombre = y.Nombre,
                    Distritos = distritos?.Where(z => z.DepartamentoId == y.DepartamentoId && z.ProvinciaId == y.ProvinciaId).Select(z => new DistritoViewDTO
                    {
                        Id = z.DistritoId,
                        Nombre = z.Nombre
                    })
                })
            });
        }
    }
}
