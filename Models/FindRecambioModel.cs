using Microsoft.AspNetCore.Http;
using Piezas2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Piezas2.Core.Model;

namespace Piezas2.Models
  {
  //=======================================================================================================================================
  /// <summary> Obtiene todos los datos necesarios para el cuadro de busqueda de recambios </summary>
  public class FindRecambioModel
    {
    public List<IdName> Marcas { set; get; }
    public List<Modelo> Modelos { set; get; }
    public List<MotorCoche> Motores { set; get; }
    public List<IdName> Fabricantes { set; get; }
    public List<IdName> Categorias { set; get; }
    public List<IdName> SubCategorias { set; get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public FindRecambioModel( HttpContext HttpCtx )
      {
      Marcas  = new Marcas( HttpCtx ).ListIdName();
      Modelos = new Modelos( HttpCtx ).ListModelos();
      Motores = new Motores( HttpCtx ).FindForCoche( "en uso", null );

      Fabricantes = new Fabricantes( HttpCtx ).ListIdName();

      var cat = new Categorias( HttpCtx );

      Categorias    = cat.ListIdName();
      SubCategorias = cat.getSubCategorias();
      }

    public HtmlString MarcasToJson( )       => new HtmlString( JsonSerializer.Serialize( Marcas) );
    public HtmlString ModelosToJson()       => new HtmlString( JsonSerializer.Serialize( Modelos ) );
    public HtmlString MotoresToJson()       => new HtmlString( JsonSerializer.Serialize( Motores ) );
    public HtmlString FabricantesToJson()   => new HtmlString( JsonSerializer.Serialize( Fabricantes ) );
    public HtmlString CategoriasToJson()    => new HtmlString( JsonSerializer.Serialize( Categorias ) );
    public HtmlString SubCategoriasToJson() => new HtmlString( JsonSerializer.Serialize( SubCategorias ) );
    }
  }
  