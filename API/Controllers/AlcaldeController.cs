﻿using API.Models.Catalogos;
using API.Models.Metodos;
using API.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace API.Controllers
{
    public class AlcaldeController : ApiController
    {
        CatalogoRespuestasHTTP _objCatalogoRespuestasHTTP = new CatalogoRespuestasHTTP();
        CatalogoAlcalde _objCatalogoAlcalde = new CatalogoAlcalde();
        Seguridad _seguridad = new Seguridad();

        [HttpPost]
        [Route("api/alcalde_consultar")]
        public object alcalde_consultar()
        {
            object _respuesta = new object();
            RespuestaHTTP _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
            try
            {
                var _listaAlcalde = _objCatalogoAlcalde.ConsultarAlcalde().Where(c => c.Estado == true).ToList();
                foreach (var item in _listaAlcalde)
                {
                    item.IdAlcalde = 0;
                    item.Canton.IdCanton = 0;
                    item.Canton.Provincia.IdProvincia = 0;
                }
                _respuesta = _listaAlcalde;
                _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "200").FirstOrDefault();
            }
            catch (Exception ex)
            {
                _http.mensaje = _http.mensaje + " " + ex.Message.ToString();
            }
            return new { respuesta = _respuesta, http = _http };
        }

        [HttpPost]
        [Route("api/alcalde_consultar")]
        public object alcalde_consultar(string _idAlcaldeEncriptado)
        {
            object _respuesta = new object();
            RespuestaHTTP _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
            try
            {
                if (_idAlcaldeEncriptado == null || string.IsNullOrEmpty(_idAlcaldeEncriptado))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Ingrese el identificador del alcalde";
                }
                else
                {
                    int _idAlcalde = Convert.ToInt32(_seguridad.DesEncriptar(_idAlcaldeEncriptado));
                    var _objAlcalde = _objCatalogoAlcalde.ConsultarAlcaldePorId(_idAlcalde).Where(c => c.Estado == true).FirstOrDefault();
                    if (_objAlcalde == null)
                    {
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "404").FirstOrDefault();
                        _http.mensaje = "No se encontró el alcalde registrado.";
                    }
                    else
                    {
                        _objAlcalde.IdAlcalde = 0;
                        _objAlcalde.Canton.IdCanton = 0;
                        _objAlcalde.Canton.Provincia.IdProvincia = 0;
                        _respuesta = _objAlcalde;
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "200").FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _http.mensaje = _http.mensaje + " " + ex.Message.ToString();
            }
            return new { respuesta = _respuesta, http = _http };
        }

        [HttpPost]
        [Route("api/alcalde_insertar")]
        public object alcalde_insertar(Alcalde _objAlcalde)
        {
            object _respuesta = new object();
            RespuestaHTTP _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
            try
            {
                if (_objAlcalde == null)
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "No se encontró el objeto alcalde";
                }
                else if (_objAlcalde.Canton.IdCantonEncriptado == null || string.IsNullOrEmpty(_objAlcalde.Canton.IdCantonEncriptado))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Ingrese el cantón";
                }
                else if (_objAlcalde.Representante == null || string.IsNullOrEmpty(_objAlcalde.Representante))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Ingrese el nombre del representante";
                }
                else if (_objAlcalde.FechaIngreso.ToShortDateString() == "01/01/0001")
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Ingrese la fecha de ingreso";
                }
                else if (_objAlcalde.FechaSalida != null && (DateTime.Compare(_objAlcalde.FechaIngreso, Convert.ToDateTime(_objAlcalde.FechaSalida)) == 1 || DateTime.Compare(_objAlcalde.FechaIngreso, Convert.ToDateTime(_objAlcalde.FechaSalida)) == 0))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "La fecha de ingreso debe ser menor a la fecha de salida";
                }
                else
                {
                    _objAlcalde.Estado = true;
                    _objAlcalde.Canton.IdCanton = Convert.ToInt32(_seguridad.DesEncriptar(_objAlcalde.Canton.IdCantonEncriptado));
                    int _idAlcalde = _objCatalogoAlcalde.InsertarAlcalde(_objAlcalde);
                    if (_idAlcalde == 0)
                    {
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                        _http.mensaje = "Ocurrió un error al tratar de ingresar al alcalde";
                    }
                    else
                    {
                        var _objAlcaldeIngresado = _objCatalogoAlcalde.ConsultarAlcaldePorId(_idAlcalde).FirstOrDefault();
                        _objAlcaldeIngresado.IdAlcalde = 0;
                        _objAlcaldeIngresado.Canton.IdCanton = 0;
                        _objAlcaldeIngresado.Canton.Provincia.IdProvincia = 0;
                        _respuesta = _objAlcaldeIngresado;
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "200").FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _http.mensaje = _http.mensaje + " " + ex.Message.ToString();
            }
            return new { respuesta = _respuesta, http = _http };
        }
        [HttpPost]
        [Route("api/alcalde_modificar")]
        public object alcalde_modificar(Alcalde _objAlcalde)
        {
            object _respuesta = new object();
            RespuestaHTTP _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
            try
            {
                if (_objAlcalde == null)
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "No se encontró el objeto alcalde";
                }
                else if (_objAlcalde.IdAlcaldeEncriptado == null || string.IsNullOrEmpty(_objAlcalde.IdAlcaldeEncriptado))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Ingrese el cantón";
                }
                else if (_objAlcalde.Canton.IdCantonEncriptado == null || string.IsNullOrEmpty(_objAlcalde.Canton.IdCantonEncriptado))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Ingrese el cantón";
                }
                else if (_objAlcalde.Representante == null || string.IsNullOrEmpty(_objAlcalde.Representante))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Ingrese el nombre del representante";
                }
                else if (_objAlcalde.FechaIngreso.ToShortDateString() == "01/01/0001")
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Ingrese la fecha de ingreso";
                }
                else if (_objAlcalde.FechaSalida != null && (DateTime.Compare(_objAlcalde.FechaIngreso, Convert.ToDateTime(_objAlcalde.FechaSalida)) == 1 || DateTime.Compare(_objAlcalde.FechaIngreso, Convert.ToDateTime(_objAlcalde.FechaSalida)) == 0))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "La fecha de ingreso debe ser menor a la fecha de salida";
                }
                else
                {
                    _objAlcalde.Estado = true;
                    _objAlcalde.IdAlcalde= Convert.ToInt32(_seguridad.DesEncriptar(_objAlcalde.IdAlcaldeEncriptado));
                    _objAlcalde.Canton.IdCanton = Convert.ToInt32(_seguridad.DesEncriptar(_objAlcalde.Canton.IdCantonEncriptado));
                    int _idAlcalde = _objCatalogoAlcalde.ModificarAlcalde(_objAlcalde);
                    if (_idAlcalde == 0)
                    {
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                        _http.mensaje = "Ocurrió un error al tratar de modificar al alcalde";
                    }
                    else
                    {
                        var _objAlcaldeModificado = _objCatalogoAlcalde.ConsultarAlcaldePorId(_idAlcalde).FirstOrDefault();
                        _objAlcaldeModificado.IdAlcalde = 0;
                        _objAlcaldeModificado.Canton.IdCanton = 0;
                        _objAlcaldeModificado.Canton.Provincia.IdProvincia = 0;
                        _respuesta = _objAlcaldeModificado;
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "200").FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _http.mensaje = _http.mensaje + " " + ex.Message.ToString();
            }
            return new { respuesta = _respuesta, http = _http };
        }

        [HttpPost]
        [Route("api/alcalde_eliminar")]
        public object alcalde_eliminar(string _idAlcaldeEncriptado)
        {
            object _respuesta = new object();
            RespuestaHTTP _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "500").FirstOrDefault();
            try
            {
                if (_idAlcaldeEncriptado == null || string.IsNullOrEmpty(_idAlcaldeEncriptado))
                {
                    _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                    _http.mensaje = "Ingrese el identificador del alcalde";
                }
                else
                {
                    int _idAlcalde = Convert.ToInt32(_seguridad.DesEncriptar(_idAlcaldeEncriptado));
                    var _objPrefecto = _objCatalogoAlcalde.ConsultarAlcaldePorId(_idAlcalde).FirstOrDefault();
                    if (_objPrefecto.Utilizado == "1")
                    {
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "400").FirstOrDefault();
                        _http.mensaje = "Este alcalde ya ha sido utilizado, por lo tanto no lo puede eliminar.";
                    }
                    else
                    {
                        _objCatalogoAlcalde.EliminarAlcalde(_idAlcalde);
                        _http = _objCatalogoRespuestasHTTP.consultar().Where(x => x.codigo == "200").FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                _http.mensaje = _http.mensaje + " " + ex.Message.ToString();
            }
            return new { respuesta = _respuesta, http = _http };
        }
    }
}
