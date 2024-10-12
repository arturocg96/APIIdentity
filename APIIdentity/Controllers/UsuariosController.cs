﻿using APIIdentity.Modelos;
using APIIdentity.Modelos.Dtos;
using APIIdentity.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace APIIdentity.Controllers
{    
        [Route("api/usuarios")]
        [ApiController]
        public class UsuariosController : ControllerBase
        {
            private readonly IUsuarioRepositorio _usRepo;
            protected RespuestaAPI _respuestaApi;
            private readonly IMapper _mapper;

            public UsuariosController(IUsuarioRepositorio usRepo, IMapper mapper)
            {
                _usRepo = usRepo;
                _mapper = mapper;
                _respuestaApi = new();
            }

            [Authorize(Roles = "Admin")]
            [HttpGet]
            [ResponseCache(CacheProfileName = "PorDefecto30Segundos")]
            [ProducesResponseType(StatusCodes.Status403Forbidden)]
            [ProducesResponseType(StatusCodes.Status200OK)]
            public IActionResult GetUsuarios()
            {
                var listaUsuarios = _usRepo.GetUsuarios();

                var listaUsuariosDto = new List<UsuarioDto>();

                foreach (var lista in listaUsuarios)
                {
                    listaUsuariosDto.Add(_mapper.Map<UsuarioDto>(lista));
                }
                return Ok(listaUsuariosDto);
            }

            [Authorize(Roles = "Admin")]
            [HttpGet("{usuarioId}", Name = "GetUsuario")]
            [ResponseCache(CacheProfileName = "PorDefecto30Segundos")]
            [ProducesResponseType(StatusCodes.Status403Forbidden)]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public IActionResult GetUsuario(string usuarioId)
            {
                var itemUsuario = _usRepo.GetUsuario(usuarioId);

                if (itemUsuario == null)
                {
                    return NotFound();
                }

                var itemUsuarioDto = _mapper.Map<UsuarioDto>(itemUsuario);

                return Ok(itemUsuarioDto);
            }

            [AllowAnonymous]
            [HttpPost("registro")]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
            {
                bool validarNombreUsuarioUnico = _usRepo.IsUniqueUser(usuarioRegistroDto.NombreUsuario);
                if (!validarNombreUsuarioUnico)
                {
                    _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                    _respuestaApi.IsSuccess = false;
                    _respuestaApi.ErrorMessages.Add("El nombre de usuario ya existe");
                    return BadRequest(_respuestaApi);
                }

                var usuario = await _usRepo.Registro(usuarioRegistroDto);
                if (usuario == null)
                {
                    _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                    _respuestaApi.IsSuccess = false;
                    _respuestaApi.ErrorMessages.Add("Error en el registro");
                    return BadRequest(_respuestaApi);
                }

                _respuestaApi.StatusCode = HttpStatusCode.OK;
                _respuestaApi.IsSuccess = true;
                return Ok(_respuestaApi);

            }

            [AllowAnonymous]
            [HttpPost("login")]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto)
            {

                var respuestaLogin = await _usRepo.Login(usuarioLoginDto);

                if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
                {
                    _respuestaApi.StatusCode = HttpStatusCode.BadRequest;
                    _respuestaApi.IsSuccess = false;
                    _respuestaApi.ErrorMessages.Add("El nombre de usuario o password son incorrectos");
                    return BadRequest(_respuestaApi);
                }

                _respuestaApi.StatusCode = HttpStatusCode.OK;
                _respuestaApi.IsSuccess = true;
                _respuestaApi.Result = respuestaLogin;
                return Ok(_respuestaApi);
            }
        }
    }