using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Obtiene una lista los modelos para una marca determinada, o si no se suministra la marca, retorna todos los modelos </summary>
  public class ModelosMarca
    {
    /// <summary> Lista de modelos de coches según la marca o todos los existentes </summary>
    public List<Modelo> Items { get; set; } = new List<Modelo>();

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los modelos de la base de datos </summary>
    public ModelosMarca( string sMarca, HttpContext HttpCtx )
      {
      var DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD

      if( string.IsNullOrWhiteSpace(sMarca) )
        {
        Items = DbCtx.Modelos.OrderBy( x => x.Nombre ).ToList();

        //var Marcas = DbCtx.Marcas.ToDictionary( x => x.Id, x => x.Nombre );

        //Items.ForEach( x => AppendMarca( x, Marcas ) );
        }
      else
        { 
        if( !int.TryParse( sMarca, out int idMarca  ) )
          idMarca = DbCtx.Marcas.FirstOrDefault( x => x.Nombre == sMarca )?.Id ?? -1;

        Items = DbCtx.Modelos.Where( x => x.Marca== idMarca ).OrderBy( x => x.Nombre ).ToList();
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona el nombre de la marca delante del nombre del modelo </summary>
    private void AppendMarca( Modelo x, Dictionary<int, string> marcas )
      {
      if( !x.Marca.HasValue ) return;

      if( marcas.TryGetValue( x.Marca.Value, out string sMarca  ) )
        x.Nombre = sMarca + ' ' + x.Nombre;
      }
    }
  }