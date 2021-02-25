using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Obtiene los datos de un recammbio especifico desde la base de datos </summary>
  public class Recambio
    {
    public int IdItem { get; set; }
    public int IdFabricante { get; set; }
    public string Fabricante { get; set; }
    public int IdCategoria { get; set; }
    public string Categoria { get; set; }
    public string Nombre { get; set; }
    public string Codigo { get; set; }
    public string Foto { get; set; }
    public string Precio { get; set; }
    public string Descripcion { get; set; }
    public List<CocheDesc> Coches { get; set; } = new List<CocheDesc>();

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public Recambio( int id, HttpContext HttpCtx )
      {
      var DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD

      var Recamb = DbCtx.Items.Find( id );
      if( Recamb==null ) return;

      IdItem       = Recamb.Id;
      IdFabricante = Recamb.Fabricante;
      IdCategoria  = Recamb.Categoria;
      Nombre       = Recamb.Nombre;
      Codigo       = Recamb.Codigo;
      Foto         = Recamb.Foto;
      Precio       = Recamb.Precio.HasValue? Recamb.Precio.ToString() : "";
      Descripcion  = Recamb.Descripcion;

      if( string.IsNullOrWhiteSpace(Foto)        ) Foto = "";
      if( string.IsNullOrWhiteSpace(Descripcion) ) Descripcion = "";

      Fabricante = DbCtx.Fabricantes.Find( IdFabricante )?.Nombre;
      Categoria    = DbCtx.Categoria.Find( IdCategoria )?.Nombre;

      var sql = "SELECT c.* FROM Coche AS c INNER JOIN  ItemCoche AS ic ON c.Id = ic.IdCoche WHERE ic.IdItem = {0}";
      var coches  = DbCtx.Coches.FromSqlRaw(sql,id).Include( c => c.MarcaNavigation ).Include( c => c.ModeloNavigation ).Include( c => c.MotorNavigation ).ToList();

      foreach( var coche in coches )
        Coches.Add( new CocheDesc(coche) );
      }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Mantiene los datos un coche, de una manera resumida y mas descriptiva  </summary>
    public class CocheDesc
      {
      public CocheDesc( Coche coche )
        {
        Id      = coche.Id;
        Marca   = coche.MarcaNavigation.Nombre;
        Modelo  = coche.ModeloNavigation.Nombre;
        Motor   = coche.MotorNavigation.Nombre;
        UseItem = coche.ItemCoches.Count;
        }

      public int Id { get; set; }
      public string Marca { get; set; }
      public string Modelo { get; set; }
      public string Motor { get; set; }
      public int UseItem { get; set; }
      }
  }