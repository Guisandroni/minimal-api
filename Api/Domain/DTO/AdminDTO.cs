

namespace minimal_api.Domain.DTO
{
    using minimal_api.Domain.Enum;

    public class AdminDTO
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public Perfil? Perfil { get; set; } = default!;
    }
}