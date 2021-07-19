using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API.Models.Catalogos;
using API.Models.Metodos;
using API.Models.Entidades;

namespace API.Controllers
{
    [Authorize]
    public class ConfigurarMatrizController : ApiController
    {
        CatalogoRespuestasHTTP _objCatalogoRespuestasHTTP = new CatalogoRespuestasHTTP();
        CatalogoOpcionUnoMatriz _objCatalogoOpcionUnoMatriz = new CatalogoOpcionUnoMatriz();
        CatalogoOpcionDosMatriz _objCatalogoOpcionDosMatriz = new CatalogoOpcionDosMatriz();
        CatalogoConfigurarMatriz _objCatalogoConfigurarMatriz = new CatalogoConfigurarMatriz();
        CatalogoAsignarEncuestado _objCatalogoAsignarEncuestado = new CatalogoAsignarEncuestado();
        CatalogoPregunta _objCatalogoPregunta = new CatalogoPregunta();
        Seguridad _seguridad = new Seguridad();

        [HttpPost]
        [Route("api/configurarmatriz_insertar")]
        public object configurarmatriz_insertar(ConfigurarMatriz _objConfigurarMatriz)
        {
            object _respuesta = new object();
            RespuestaHTTP _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
            try
            {
                if (_objConfigurarMatriz == null)
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
                    _http.mensaje = "Ingrese el objeto opción dos matriz";
                }
                else if (_objConfigurarMatriz.OpcionUnoMatriz.Pregunta.IdPreguntaEncriptado == null || string.IsNullOrEmpty(_objConfigurarMatriz.OpcionUnoMatriz.Pregunta.IdPreguntaEncriptado))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
                    _http.mensaje = "Ingrese el identificador de la pregunta";
                }
                else if (_objConfigurarMatriz.OpcionDosMatriz.Descripcion == null || string.IsNullOrEmpty(_objConfigurarMatriz.OpcionDosMatriz.Descripcion))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
                    _http.mensaje = "Ingrese la descripción de la opción dos matriz";
                }
                else
                {
                    int _idPregunta = Convert.ToInt32(_seguridad.DesEncriptar(_objConfigurarMatriz.OpcionUnoMatriz.Pregunta.IdPreguntaEncriptado));
                    var _objPregunta = _objCatalogoPregunta.ConsultarPreguntaPorId(_idPregunta).Where(c => c.Estado == true).FirstOrDefault();
                    if (_objPregunta == null)
                    {
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "404").FirstOrDefault();
                        _http.mensaje = "No se encontró la pregunta en el sistema";
                    }
                    else
                    {
                        _objConfigurarMatriz.OpcionUnoMatriz.Pregunta.IdPregunta = _idPregunta;
                        var _listaOpcionUnoMatrizPorPregunta = _objCatalogoOpcionUnoMatriz.ConsultarOpcionUnoMatrizPorIdPregunta(_idPregunta).Where(c => c.Estado == true).ToList();
                        if (_listaOpcionUnoMatrizPorPregunta.Count == 0)
                        {
                            _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                            _http.mensaje = "Debe ingresar primero las opciones uno de la matriz";
                        }
                        else
                        {
                            _objConfigurarMatriz.Estado = true;
                            _objConfigurarMatriz.OpcionDosMatriz.Estado = true;
                            int _idOpcionDosMatriz = _objCatalogoOpcionDosMatriz.InsertarOpcionDosMatriz(_objConfigurarMatriz.OpcionDosMatriz);
                            if (_idOpcionDosMatriz == 0)
                            {
                                _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                                _http.mensaje = "Ocurrió un error al tratar de ingresar la opción dos matriz";
                            }
                            else
                            {
                                _objConfigurarMatriz.OpcionDosMatriz.IdOpcionDosMatriz = _idOpcionDosMatriz;
                                foreach (var itemOpcionUno in _listaOpcionUnoMatrizPorPregunta)
                                {
                                    _objConfigurarMatriz.OpcionUnoMatriz.IdOpcionUnoMatriz = itemOpcionUno.IdOpcionUnoMatriz;
                                    int _idConfigurarMatriz = _objCatalogoConfigurarMatriz.InsertarConfigurarMatriz(_objConfigurarMatriz);
                                }
                                var _listaConfigurarMatriz = _objCatalogoConfigurarMatriz.ConsultarConfigurarMatrizPorIdPregunta(_objConfigurarMatriz.OpcionUnoMatriz.Pregunta.IdPregunta).Where(c => c.Estado == true).ToList();

                                foreach (var _objConfMatriz in _listaConfigurarMatriz)
                                {
                                    _objConfMatriz.IdConfigurarMatriz = 0;
                                    _objConfMatriz.OpcionDosMatriz.IdOpcionDosMatriz = 0;
                                    _objConfMatriz.OpcionUnoMatriz.IdOpcionUnoMatriz = 0;
                                    _objConfMatriz.OpcionUnoMatriz.Pregunta.IdPregunta = 0;
                                    _objConfMatriz.OpcionUnoMatriz.Pregunta.TipoPregunta.IdTipoPregunta = 0;
                                    _objConfMatriz.OpcionUnoMatriz.Pregunta.Seccion.IdSeccion = 0;
                                    _objConfMatriz.OpcionUnoMatriz.Pregunta.Seccion.Componente.IdComponente = 0;
                                    _objConfMatriz.OpcionUnoMatriz.Pregunta.Seccion.Componente.CuestionarioGenerico.IdCuestionarioGenerico = 0;
                                }

                                _respuesta = _listaConfigurarMatriz;
                                _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "200").FirstOrDefault();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _http.mensaje = _http.mensaje + " " + ex.Message.ToString();
            }
            return new { respuesta = _respuesta, http = _http };
        }


        [HttpGet]
        [Route("api/configurarmatriz_consultarporidpregunta")]
        public object configurarmatriz_consultarporidpregunta(string _idPreguntaEncriptado, string _IdAsignarEncuestado)
        {
            object _respuesta = new object();
            object _respuesta1 = new object();
            int IdAsignarEncuestado = 0;
            RespuestaHTTP _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
            try
            {
                if (_IdAsignarEncuestado == null || string.IsNullOrEmpty(_IdAsignarEncuestado))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Algunos campos están vacíos";
                    return new { http = _http };
                }
                if (_IdAsignarEncuestado != "null")
                {
                    IdAsignarEncuestado = int.Parse(_seguridad.DesEncriptar(_IdAsignarEncuestado));
                    var objCatalogoAsignarEncuestado = _objCatalogoAsignarEncuestado.ConsultarAsignarEncuestadoPorId(IdAsignarEncuestado).Where(c => c.Estado == true).FirstOrDefault();
                    if (objCatalogoAsignarEncuestado == null)
                    {
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                        _http.mensaje = "El asignar encuestado no existe";
                        return new { http = _http };
                    }

                }
                else
                {
                    _IdAsignarEncuestado = Convert.ToString(0);
                }


                if (_idPreguntaEncriptado == null || string.IsNullOrEmpty(_idPreguntaEncriptado))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
                    _http.mensaje = "Ingrese el identificador de la pregunta";
                }
                else
                {
                    int _idPregunta = Convert.ToInt32(_seguridad.DesEncriptar(_idPreguntaEncriptado));
                    var _objPregunta = _objCatalogoPregunta.ConsultarPreguntaPorId(_idPregunta).Where(c => c.Estado == true).FirstOrDefault();
                    if (_objPregunta == null)
                    {
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "404").FirstOrDefault();
                        _http.mensaje = "No se encontró la pregunta en el sistema";
                    }
                    else
                    {
                        var _listaConfigurarMatriz = _objCatalogoConfigurarMatriz.ConsultarConfigurarMatrizPorIdPregunta(_idPregunta).Where(c => c.Estado == true).ToList();

                        if (_IdAsignarEncuestado != null)
                        {

                            _listaConfigurarMatriz = null;
                            _listaConfigurarMatriz = _objCatalogoConfigurarMatriz.ConsultarConfigurarMatrizPorIdPregunta2(_idPregunta, IdAsignarEncuestado).Where(c => c.Estado == true).ToList();
                        }
                        

                        foreach (var _objConfMatriz in _listaConfigurarMatriz)
                        {
                            _objConfMatriz.IdConfigurarMatriz = 0;
                            _objConfMatriz.OpcionDosMatriz.IdOpcionDosMatriz = 0;
                            _objConfMatriz.OpcionUnoMatriz.IdOpcionUnoMatriz = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.IdPregunta = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.TipoPregunta.IdTipoPregunta = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.Seccion.IdSeccion = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.Seccion.Componente.IdComponente = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.Seccion.Componente.CuestionarioGenerico.IdCuestionarioGenerico = 0;
                        }

                        _respuesta = _listaConfigurarMatriz;
                        _respuesta1 = _objPregunta;
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "200").FirstOrDefault();


                    }
                }
            }
            catch (Exception ex)
            {
                _http.mensaje = _http.mensaje + " " + ex.Message.ToString();
            }
            return new { respuesta = _respuesta, http = _http, respuesta1 = _respuesta1 };
        }
        [HttpGet]
        [Route("api/configurarmatriz_seleccionrespuesta")]
        public object configurarmatriz_seleccionrespuesta(string _idPreguntaEncriptado,  string _idcomunidad , string _idversioncuestionario)
        {
            object _respuesta = new object();
            object _respuesta1 = new object();
            
            RespuestaHTTP _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
            try
            {
              

                if (_idPreguntaEncriptado == null || string.IsNullOrEmpty(_idPreguntaEncriptado))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
                    _http.mensaje = "Ingrese el identificador de la pregunta";
                }
                else
                {
                    int _idPregunta = Convert.ToInt32(_seguridad.DesEncriptar(_idPreguntaEncriptado));
                    var _objPregunta = _objCatalogoPregunta.ConsultarPreguntaPorId(_idPregunta).Where(c => c.Estado == true).FirstOrDefault();
                    if (_objPregunta == null)
                    {
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "404").FirstOrDefault();
                        _http.mensaje = "No se encontró la pregunta en el sistema";
                    }
                    else
                    {
                        var _listaConfigurarMatriz = _objCatalogoConfigurarMatriz.ConsultarConfigurarMatrizPorIdPregunta(_idPregunta).Where(c => c.Estado == true).ToList();
                        
                        _listaConfigurarMatriz = null;
                        var idcomunidad = Convert.ToInt32(_seguridad.DesEncriptar(_idcomunidad));
                        var idversioncuestionario = Convert.ToInt32(_seguridad.DesEncriptar(_idversioncuestionario));
                        _listaConfigurarMatriz = _objCatalogoConfigurarMatriz.ConsultarConfigurarMatrizSeleccionRepsuesta(_idPregunta, idcomunidad, idversioncuestionario).Where(c => c.Estado == true).ToList();
                   



                        foreach (var _objConfMatriz in _listaConfigurarMatriz)
                        {
                            _objConfMatriz.IdConfigurarMatriz = 0;
                            _objConfMatriz.OpcionDosMatriz.IdOpcionDosMatriz = 0;
                            _objConfMatriz.OpcionUnoMatriz.IdOpcionUnoMatriz = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.IdPregunta = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.TipoPregunta.IdTipoPregunta = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.Seccion.IdSeccion = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.Seccion.Componente.IdComponente = 0;
                            _objConfMatriz.OpcionUnoMatriz.Pregunta.Seccion.Componente.CuestionarioGenerico.IdCuestionarioGenerico = 0;
                        }

                        _respuesta = _listaConfigurarMatriz;
                        _respuesta1 = _objPregunta;
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "200").FirstOrDefault();


                    }
                }
            }
            catch (Exception ex)
            {
                _http.mensaje = _http.mensaje + " " + ex.Message.ToString();
            }
            return new { respuesta = _respuesta, http = _http, respuesta1 = _respuesta1 };
        }
        [HttpGet]
        [Route("api/ObservacionesRespuestaMatrizSeleccion")]
        public object ObservacionesRespuestaMatrizSeleccion(string _idrespuestalogica, string _idcomunidad, string _idversioncuestionario)
        {
            object _respuesta = new object();
       

            RespuestaHTTP _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
            try
            {

                if (_idrespuestalogica == null || string.IsNullOrEmpty(_idrespuestalogica))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
                    _http.mensaje = "Ingrese el identificador de la respuesta lógica";
                }
                if (_idcomunidad == null || string.IsNullOrEmpty(_idcomunidad))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
                    _http.mensaje = "Ingrese el identificador de la comunidad";
                }
                if (_idversioncuestionario == null || string.IsNullOrEmpty(_idversioncuestionario))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
                    _http.mensaje = "Ingrese el identificador de la versión del cuestionario";
                }

                 int idrespuestalogica = Convert.ToInt32(_seguridad.DesEncriptar(_idrespuestalogica));
                 int idcomunidad = Convert.ToInt32(_seguridad.DesEncriptar(_idcomunidad));
                 int idversioncuestionario = Convert.ToInt32(_seguridad.DesEncriptar(_idversioncuestionario));
                 var _listaConfigurarMatriz = _objCatalogoConfigurarMatriz.ObservacionesRespuestaMatrizSeleccion(idrespuestalogica, idcomunidad, idversioncuestionario).ToList();


                foreach (var _objConfMatriz in _listaConfigurarMatriz)
                {
                    _objConfMatriz.IdPreguntas = 0;
                    _objConfMatriz.IdRespuestaLogica = 0;
      
                }

                _respuesta = _listaConfigurarMatriz;
                 _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "200").FirstOrDefault();
                    
                
            }
            catch (Exception ex)
            {
                _http.mensaje = _http.mensaje + " " + ex.Message.ToString();
            }
            return new { respuesta = _respuesta, http = _http };
        }
    }
}