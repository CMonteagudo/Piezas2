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
  //=======================================================================================================================================
  /// <summary> Obtiene todos los datos necesarios para el cuadro de busqueda de recambios </summary>
  public class RecambioUsoModel
    {
    public int Id { set; get; }
    public Fabricantes Fabricantes{ set; get; }
    public List<IdName> Categorias { set; get; }
    public List<IdName> SubCategorias { set; get; }

    public MarcaCoches Marcas { set; get; }
    public ModelosMarca Modelos { set; get; }
    public Motores Motores { set; get; }

    public List<CocheDesc> Coches { set; get; }
    public List<Item> Items { set; get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public RecambioUsoModel( int id, HttpContext HttpCtx )
      {
      this.Id = id;

      Fabricantes = new Fabricantes( HttpCtx );

      var cat = new Categorias( HttpCtx );

      Categorias = cat.getCategorias();
      SubCategorias = cat.getSubCategorias();

      Marcas = new MarcaCoches( HttpCtx );
      Modelos = new ModelosMarca( null, HttpCtx );
      Motores = new Motores( "en uso", null, HttpCtx );

      Coches = new RecambioUsos( 0, HttpCtx ).UsoInCoches().Coches;

      var rec = new Recambios( HttpCtx );
      rec.FindByDatos( "0/0/0/0/0/Order-4/Range-0-10000" );

      Items = rec.Items;
      }

    public HtmlString FabricantesToJson() => new HtmlString( JsonSerializer.Serialize( Fabricantes.Items ) );
    public HtmlString CategoriasToJson() => new HtmlString( JsonSerializer.Serialize( Categorias ) );
    public HtmlString SubCategoriasToJson() => new HtmlString( JsonSerializer.Serialize( SubCategorias ) );

    public HtmlString MarcasToJson() => new HtmlString( JsonSerializer.Serialize( Marcas.Items ) );
    public HtmlString ModelosToJson() => new HtmlString( JsonSerializer.Serialize( Modelos.Items ) );
    public HtmlString MotoresToJson() => new HtmlString( JsonSerializer.Serialize( Motores.Items ) );

    public HtmlString CochesToJson() => new HtmlString( JsonSerializer.Serialize( Coches ) );
    public HtmlString ItemsToJson() => new HtmlString( JsonSerializer.Serialize( Items ) );

    }
  }
  