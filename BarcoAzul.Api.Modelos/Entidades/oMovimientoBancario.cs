using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades.Extensiones;
using BarcoAzul.Api.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oMovimientoBancario
    {
        public string Id { get; set; }
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [Required(ErrorMessage = "La cuenta corriente es requerida.")]
        public string CuentaCorrienteId { get; set; }
        [Required(ErrorMessage = "La fecha de emisión es requerida.")]
        public DateTime FechaEmision { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        public string TipoMovimientoId { get; set; }
        public string TipoOperacionId { get; set; }
        [Required(ErrorMessage = "El número de operación es requerido.")]
        public string NumeroOperacion { get; set; }
        public bool IsCierreCaja { get; set; }
        public string TipoBeneficiarioId { get; set; }
        public string ClienteProveedorId { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string ClienteProveedorNombre { get; set; }
        public string Concepto { get; set; }
        public string DocumentoReferencia { get; set; }
        public bool TieneDetraccion { get; set; }
        public decimal PorcentajeITF { get; set; }
        public decimal MontoITF { get; set; }
        public decimal MontoInteres { get; set; }
        public decimal Monto { get; set; }
        [Required(ErrorMessage = "El total es requerido.")]
        [Range(0.01, int.MaxValue, ErrorMessage = "El total no puede ser igual a cero (0.00)")]
        public decimal Total { get; set; }
        public bool TieneCuentaDestino { get; set; }
        public string CuentaDestinoId { get; set; }
        public string MonedaId { get; set; }
        public List<oMovimientoBancarioDetalle> Detalles { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string ClienteId { get; set; }
        [JsonIgnore]
        public string ProveedorId { get; set; }
        [JsonIgnore]
        public string DocumentoVentaCompraId { get; set; }
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oCuentaCorriente CuentaCorriente { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            ClienteProveedorNombre = ClienteProveedorNombre?.Trim();
            NumeroOperacion = NumeroOperacion?.Trim();
            Concepto = Concepto?.Trim();
        }

        public void CompletarDatosDetalles()
        {
            if (Detalles is not null)
            {
                foreach (var detalle in Detalles)
                {
                    detalle.TipoMovimientoId = TipoMovimientoId;
                    detalle.EmpresaId = EmpresaId;
                    detalle.TipoDocumentoId = TipoDocumentoId;
                    detalle.Serie = Id.Right(4);
                    detalle.Numero = Id;
                    detalle.MonedaId = MonedaId;
                    detalle.TipoCambio = TipoCambio;
                    detalle.ProveedorId = ProveedorId;

                    if (detalle.TipoMovimientoId == "IN")
                    {
                        var splitId = new oSplitDocumentoVentaId(detalle.DocumentoVentaCompraId);
                        detalle.DocumentoVentaCompraEmpresaId = splitId.EmpresaId;
                        detalle.DocumentoVentaCompraTipoDocumentoId = splitId.TipoDocumentoId;
                        detalle.DocumentoVentaCompraSerie = splitId.Serie;
                        detalle.DocumentoVentaCompraNumero = splitId.Numero;
                    }
                    else
                    {
                        var splitId = new oSplitDocumentoCompraId(detalle.DocumentoVentaCompraId);
                        detalle.DocumentoVentaCompraEmpresaId = splitId.EmpresaId;
                        detalle.DocumentoVentaCompraTipoDocumentoId = splitId.TipoDocumentoId;
                        detalle.DocumentoVentaCompraSerie = splitId.Serie;
                        detalle.DocumentoVentaCompraNumero = splitId.Numero;
                    }
                }
            }
        }
    }

    public class oMovimientoBancarioDetalle
    {
        [JsonIgnore]
        public string TipoMovimientoId { get; set; }
        [JsonIgnore]
        public string MovimientoBancarioId => TipoMovimientoId == "IN" ? $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}" : $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}";
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [JsonIgnore]
        public string Serie { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }
        [JsonIgnore]
        public string ProveedorId { get; set; }
        public int DetalleId { get; set; }
        public string DocumentoVentaCompraId { get; set; }
        [JsonIgnore]
        public string DocumentoVentaCompraEmpresaId { get; set; }
        [JsonIgnore]
        public string DocumentoVentaCompraTipoDocumentoId { get; set; }
        [JsonIgnore]
        public string DocumentoVentaCompraSerie { get; set; }
        [JsonIgnore]
        public string DocumentoVentaCompraNumero { get; set; }
        public DateTime DocumentoVentaCompraFechaEmision { get; set; }
        [JsonIgnore]
        public string DocumentoVentaCompraNumeroDocumento => TipoMovimientoId == "IN" ? Comun.VentaIdADocumento(DocumentoVentaCompraId) : Comun.CompraIdADocumento(DocumentoVentaCompraId);
        public string Concepto { get; set; }
        public decimal Abono { get; set; }
        public decimal Saldo { get; set; }
        public string DocumentoRelacionado { get; set; }
        [JsonIgnore]
        public int AbonoId { get; set; }
        [JsonIgnore]
        public string MonedaId { get; set; }
        [JsonIgnore]
        public decimal TipoCambio { get; set; }
    }
}
