//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Covem_web
{
    using System;
    using System.Collections.Generic;
    
    public partial class EntregaRecepcion_Vehiculo
    {
        public int id { get; set; }
        public Nullable<int> idAuto { get; set; }
        public Nullable<int> idPersonaEntrega { get; set; }
        public Nullable<System.DateTime> fechaEntrega { get; set; }
        public string ObservecionesEntrega { get; set; }
        public Nullable<bool> activo { get; set; }
        public Nullable<decimal> kilometrajeEntrega { get; set; }
        public Nullable<int> idPersonallegada { get; set; }
        public Nullable<System.DateTime> fechallegada { get; set; }
        public string Observecionesllegada { get; set; }
        public Nullable<decimal> kilometrajellegada { get; set; }
        public Nullable<int> idtaller { get; set; }
        public string estatus { get; set; }
    }
}
