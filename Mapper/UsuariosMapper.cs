using APIIdentity.Modelos.Dtos;
using APIIdentity.Modelos;
using AutoMapper;

namespace APIIdentity.Mapper
{
    public class UsuariosMapper : Profile
    {
        public UsuariosMapper() {
            CreateMap<AppUsuario, UsuarioDatosDto>().ReverseMap();
            CreateMap<AppUsuario, UsuarioDto>().ReverseMap();
        }
    }
}
