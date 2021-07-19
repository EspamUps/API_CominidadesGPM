using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Models.Entidades
{
    public class ObservacionesMatriz
    {
        public int IdPreguntas { get; set; }
        public int IdRespuestaLogica { get; set; }
        public string DescripcionRespuestaAbierta { get; set; }
        public int IdDatos { get; set; }
        public string Datos { get; set; }

        public ObservacionesMatriz(int idPreguntas, int idRespuestaLogica, string descripcionRespuestaAbierta, int idDatos, string datos)
        {
            IdPreguntas = idPreguntas;
            IdRespuestaLogica = idRespuestaLogica;
            DescripcionRespuestaAbierta = descripcionRespuestaAbierta;
            IdDatos = idDatos;
            Datos = datos;
        }
    }
}