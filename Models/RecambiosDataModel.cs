using Microsoft.AspNetCore.Http;
using Piezas2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Html;
using Piezas2.Core.Model;
using static Piezas2.Core.Coches;

namespace Piezas2.Models
  {
  public enum DataInfo { All, Items, Coches };

  //=======================================================================================================================================
  /// <summary> Obtiene todos los datos necesarios para el cuadro de busqueda de recambios </summary>
  public class RecambiosDatosModel
    {
    public int Id { set; get; }
    public List<IdName> Fabricantes { set; get; }
    public List<IdName> Categorias { set; get; }
    public List<IdName> SubCategorias { set; get; }

    public List<IdName> Marcas { set; get; }
    public List<Modelo> Modelos { set; get; }
    public List<MotorCoche> Motores { set; get; }
    public List<Motor> Motores2 { set; get; }

    public List<CocheDesc> Coches { set; get; }
    public List<Item> Items { set; get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public RecambiosDatosModel( int id, HttpContext HttpCtx, DataInfo tipo )
      {
      this.Id = id;

      if( tipo == DataInfo.All || tipo == DataInfo.Items )                      // Obtiene datos sobre los Items
        {
        Fabricantes = new Fabricantes( HttpCtx ).ListIdName();

        var cat = new Categorias( HttpCtx );

        Categorias    = cat.ListIdName();
        SubCategorias = cat.getSubCategorias();

        Items = new Recambios( HttpCtx ).FindByDatos( "0/0/0/0/0/Order-4/Range-0-10000" ).Items;
        }

      if( tipo == DataInfo.All || tipo == DataInfo.Coches )                     // Obtiene datos sobre los coches
        { 
        Marcas = new Marcas( HttpCtx ).ListIdName();
        Modelos = new Modelos( HttpCtx ).ListModelos();

        if( tipo != DataInfo.Coches )
          Motores = new Motores( HttpCtx ).FindForCoche( "en uso", null );
        else
          Motores2 = new Motores( HttpCtx ).ListMotores();


        Coches = new RecambioUsos( 0, HttpCtx ).UsoInCoches().Coches;
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Funciones utilitarias que retornan los datos en formanto Json </summary>
    public HtmlString FabricantesToJson() => new HtmlString( JsonSerializer.Serialize( Fabricantes ) );
    public HtmlString CategoriasToJson() => new HtmlString( JsonSerializer.Serialize( Categorias ) );
    public HtmlString SubCategoriasToJson() => new HtmlString( JsonSerializer.Serialize( SubCategorias ) );

    public HtmlString MarcasToJson() => new HtmlString( JsonSerializer.Serialize( Marcas ) );
    public HtmlString ModelosToJson() => new HtmlString( JsonSerializer.Serialize( Modelos ) );
    public HtmlString MotoresToJson() => new HtmlString( JsonSerializer.Serialize( Motores ) );
    public HtmlString Motores2ToJson() => new HtmlString( JsonSerializer.Serialize( Motores2 ) );

    public HtmlString CochesToJson() => new HtmlString( JsonSerializer.Serialize( Coches ) );
    public HtmlString ItemsToJson() => new HtmlString( JsonSerializer.Serialize( Items ) );

    }
  }
  