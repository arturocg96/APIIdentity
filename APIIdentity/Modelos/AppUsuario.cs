using Microsoft.AspNetCore.Identity;

namespace APIIdentity.Modelos
{
    public class AppUsuario : IdentityUser
    {
        public string Nombre { get; set; }
    }
}
