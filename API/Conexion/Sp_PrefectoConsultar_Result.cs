//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace API.Conexion
{
    using System;
    
    public partial class Sp_PrefectoConsultar_Result
    {
        public int IdPrefecto { get; set; }
        public int IdProvincia { get; set; }
        public string Representante { get; set; }
        public System.DateTime FechaIngreso { get; set; }
        public Nullable<System.DateTime> FechaSalida { get; set; }
        public bool EstadoPrefecto { get; set; }
        public string UtilizadoPrefecto { get; set; }
        public string CodigoProvincia { get; set; }
        public string NombreProvincia { get; set; }
        public string DescripcionProvincia { get; set; }
        public string RutaLogoProvincia { get; set; }
        public bool EstadoProvincia { get; set; }
    }
}
