using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Modelos.Otros;
using AutoMapper;
using BarcoAzul.Api.Modelos.DTOs;

namespace BarcoAzul.Api.Logica.Configuracion
{
    public static class Mapping
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg => {
                // This line ensures that internal properties are also mapped over.
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        });

        public static IMapper Mapper => Lazy.Value;
    }

    public class MappingProfile : Profile
    {
        //TODO: IR DESCOMENTANDO DE ACUERDO A LA CREACION DE LOS DEMAS MODELOS Y VISTAS
        public MappingProfile()
        {
            CreateMap<vEntidadBancaria, oEntidadBancaria>();
            CreateMap<CuentaCorrienteDTO, oCuentaCorriente>();
            CreateMap<ProvinciaDTO, oProvincia>();
            CreateMap<DistritoDTO, oDistrito>();
            CreateMap<PersonalDTO, oPersonal>();
            //CreateMap<TipoCobroPagoDTO, oTipoCobroPago>();
            CreateMap<ClienteDTO, oCliente>();
            CreateMap<ClientePersonalDTO, oClientePersonal>();
            CreateMap<ProveedorDTO, oProveedor>();
            CreateMap<EmpresaTransporteDTO, oEmpresaTransporte>();
            CreateMap<VehiculoDTO, oVehiculo>();
            CreateMap<ConductorDTO, oConductor>();
            //CreateMap<UsuarioRegistrarDTO, oUsuario>();
            //CreateMap<UsuarioModificarDTO, oUsuario>();
            CreateMap<ArticuloDTO, oArticulo>();
            //CreateMap<CuadreStockDTO, oCuadreStock>();
            //CreateMap<SalidaCilindrosDTO, oSalidaCilindros>();
            //CreateMap<EntradaCilindrosDTO, oEntradaCilindros>();
            //CreateMap<MovimientoBancarioDTO, oMovimientoBancario>();
            //CreateMap<OrdenCompraDTO, oOrdenCompra>();
            //CreateMap<DocumentoCompraDTO, oDocumentoCompra>();
            //CreateMap<FacturaNegociableDTO, oFacturaNegociable>();
            //CreateMap<LetraCambioCompraDTO, oLetraCambioCompra>();
            //CreateMap<CEFDTO, oCEF>();
            //CreateMap<ChequeDTO, oCheque>();
            //CreateMap<GuiaCompraDTO, oGuiaCompra>();
            //CreateMap<oCuentaPorPagarAbono, oAbonoCompra>().ReverseMap();
            //CreateMap<CotizacionDTO, oCotizacion>();
            //CreateMap<DocumentoVentaDTO, oDocumentoVenta>();
            //CreateMap<oCorrelativo, oTipoDocumentoSerie>();
            //CreateMap<RetencionDTO, oRetencion>();
            //CreateMap<GuiaRemisionDTO, oGuiaRemision>();
            MapConfiguracionGlobal();
            //CreateMap<PlanillaCobroDTO, oPlanillaCobro>();
            //CreateMap<LetraCambioVentaDTO, oLetraCambioVenta>();
            //CreateMap<EntradaAlmacenDTO, oEntradaAlmacen>();
            //CreateMap<oCuentaPorCobrarAbono, oAbonoVenta>().ReverseMap();
            //CreateMap<SalidaAlmacenDTO, oSalidaAlmacen>();
        }

        private void MapConfiguracionGlobal()
        {
            CreateMap<oConfiguracionGlobal, oConfiguracionSimplificado>()
                .ForMember(dest => dest.FechaInicio, opt => opt.MapFrom(src => src.FiltroFechaInicio))
                .ForMember(dest => dest.FechaFin, opt => opt.MapFrom(src => src.FiltroFechaFin))
                .ForMember(dest => dest.UsuarioId, opt => opt.MapFrom(src => src.DefaultUsuarioId))
                .ForMember(dest => dest.ClienteId, opt => opt.MapFrom(src => src.DefaultClienteId))
                .ForMember(dest => dest.ProveedorId, opt => opt.MapFrom(src => src.DefaultProveedorId))
                .ForMember(dest => dest.PersonalId, opt => opt.MapFrom(src => src.DefaultPersonalId))
                .ForMember(dest => dest.LineaId, opt => opt.MapFrom(src => src.DefaultLineaId))
                .ForMember(dest => dest.SubLineaId, opt => opt.MapFrom(src => src.DefaultSubLineaId))
                .ForMember(dest => dest.ArticuloId, opt => opt.MapFrom(src => src.DefaultLineaId + src.DefaultSubLineaId + src.DefaultArticuloId))
                .ForMember(dest => dest.MarcaId, opt => opt.MapFrom(src => src.DefaultMarcaId))
                .ForMember(dest => dest.ConductorId, opt => opt.MapFrom(src => src.DefaultConductorId))
                .ForMember(dest => dest.PorcentajeIGV, opt => opt.MapFrom(src => src.DefaultPorcentajeIgv));
        }
    }
}
