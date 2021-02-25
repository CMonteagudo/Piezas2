using Microsoft.AspNetCore.Http;
using Piezas2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Html;

namespace Piezas2.Models
  {
  //=======================================================================================================================================
  /// <summary> Obtiene todos los datos necesarios para el cuadro de busqueda de recambios </summary>
  public class RecambioUsoModel
    {
    public int Id { set; get; }
    public Categorias Categorias { set; get; }
    public Fabricantes Fabricantes{ set; get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public RecambioUsoModel( int id, HttpContext HttpCtx )
      {
      this.Id = id;

      Categorias  = new Categorias( HttpCtx );
      Fabricantes = new Fabricantes( HttpCtx );
      }

    public HtmlString CategoriasToJson()  => new HtmlString( JsonSerializer.Serialize( Categorias.Items ) );
    public HtmlString FabricantesToJson() => new HtmlString( JsonSerializer.Serialize( Fabricantes.Items ) );
    }
  }
  