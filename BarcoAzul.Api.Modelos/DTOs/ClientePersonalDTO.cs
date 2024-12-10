namespace BarcoAzul.Api.Modelos.DTOs
{
    public class ClientePersonalDTO
    {
        public string Id { get => $"{ClienteId}{PersonalId}"; }
        public string ClienteId { get; set; }
        public string PersonalId { get; set; }
        public bool Default { get; set; }
    }
}
