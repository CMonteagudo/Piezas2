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
  public class FindRecambioModel
    {
    public MarcaCoches Marcas { set; get; }
    public ModelosMarca Modelos { set; get; }
    public Motores Motores { set; get; }
    public Categorias Categorias { set; get; }
    public Fabricantes Fabricantes{ set; get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public FindRecambioModel( HttpContext HttpCtx )
      {
      Marcas  = new MarcaCoches( HttpCtx );
      Modelos = new ModelosMarca( null, HttpCtx );
      Motores = new Motores( "en uso", null, HttpCtx );

      Categorias  = new Categorias( HttpCtx );
      Fabricantes = new Fabricantes( HttpCtx );
      }

    public HtmlString MarcasToJson( )     => new HtmlString( JsonSerializer.Serialize( Marcas.Items) );
    public HtmlString ModelosToJson()     => new HtmlString( JsonSerializer.Serialize( Modelos.Items ) );
    public HtmlString MotoresToJson()     => new HtmlString( JsonSerializer.Serialize( Motores.Items ) );
    public HtmlString CategoriasToJson()  => new HtmlString( JsonSerializer.Serialize( Categorias.Items ) );
    public HtmlString FabricantesToJson() => new HtmlString( JsonSerializer.Serialize( Fabricantes.Items ) );
    }
  }
  