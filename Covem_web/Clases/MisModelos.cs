using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Covem_web
{
    public class Vista_Paquete
    {
        public GrupoServicio  grupo{ get; set; }
        public List<servicios> listaCompleta { get; set; }
        public List<servicios> listaAgregar { get; set; }
        public List<Grupo_Relacion_Servicio> listaRelacionada { get; set; }
    }
    public class Vista_Cheking
    {
        public Auto auto { get; set; }
        public Marca Marca { get; set; }
        public Modelo modelo { get; set; }
        public Cheking cheking { get; set; }
        public Persona persona { get; set; }
        public List<Persona_Archivo> lista { get; set; }
    }
    public class Vista_orden
    {
        public Automoviles automovil { get; set; }
    }
    public class Vista_Principal
    {
        public Auto auto { get; set; }

        public Marca marca { get; set; }
        public Persona persona { get; set; }
        public Modelo modelo { get; set; }
        public Cheking cheking { get; set; }
    }
    public class Vista_Servicio
    {
        public serviciocategoria categoria { get; set; }
        public servicios servicio { get; set; }
    }
    public class modelo_Completa
    {
        public List<Marca> listamarcas { get; set; }
        public Marca Marca { get; set; }
        public Modelo modelo { get; set; }
        public int autos { get; set; }
    }
    public class Marca_Completa
    {
        public Marca Marca { get; set; }
        public int modelos { get; set; }        
    }
    public class Servicios_Taller
    {
        public ServicioTaller Taller { get; set; }
        public serviciocategoria categoria { get; set; }
        public servicios servicio { get; set; }
    }
    public class Persona_Archivos
    {
        public Persona persona { get; set; }
        public List<Persona_Archivo> archivos { get; set; }
    }
    public class Automoviles
    {
        public Auto auto { get; set; }

        public Marca marca { get; set; }
        public Persona persona { get; set; }
        public Modelo modelo { get; set; }
        public List<auto_Fotos> Fotos { get; set; }
        public List<Persona> P_disponibles { get; set; }
        public List<Auto_Archivo> Larchivo { get; set; }
        public Auto_Archivo archivo { get; set; }
        public EntregaRecepcion_Vehiculo EntregaRecepcion { get; set; }

        public int bandera { get; set; }
        public List<tallerMecanico> Talleres_Disponibles = new List<tallerMecanico>();
        public tallerMecanico Taller = new tallerMecanico();
        public List<servicios> Lservicios = new List<servicios>();
        public List<servicios> LserviciosA = new List<servicios>();
        public List<Entrega_Sevicios> LEServicios = new List<Entrega_Sevicios>();
    }


    public class CascadingModel
    {

        public Auto auto { get; set; }
        public List<Marca> Marcas { get; set; }
        public List<Modelo> Modelos { get; set; }


        public int MarcaId { get; set; }
        public int ModeloId { get; set; }

    }

}