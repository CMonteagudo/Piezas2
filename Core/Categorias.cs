using Microsoft.AspNetCore.Http;
using Piezas2.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piezas2.Core
  {
  //=======================================================================================================================================
  /// <summary> Obtiene la lista de todas las categorias de recambio disponibles en la base de datos </summary>
  public class Categorias
    {
    private readonly DbPiezasContext DbCtx;
    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto obtener categorias y subcategorias de la base de datos </summary>
    public Categorias( HttpContext HttpCtx )
      {
      DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                        // Obtiene contexto a la BD
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todas las categorias que hay en la base de dataos </summary>
    public List<IdName> ListIdName()
      {
      return DbCtx.Categorias.OrderBy( x => x.Nombre ).Select( x => new IdName( x.Id, x.Nombre ) ).ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todas las categorias, donde se obtienen todos los datos para cada categoria </summary>
    public List<Categorium> ListCategorias()
      {
      return DbCtx.Categorias.ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra la categoria con el Id dado y retorna sus datos, sino un objeto categoria vacio </summary>
    public Categorium Find( int id )
      {
      var cat = DbCtx.Categorias.Find( id );
      if( cat == null ) cat = new Categorium { Id = 0 };

      return cat;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra la categoria con una cadeda se caracteres que puede ser su nombre o su id, sino lo puede encontrat retorna un objeto categoria vacio </summary>
    public Categorium Find( string sCat )
      {
      if( int.TryParse( sCat, out int idCat ) )
        return Find( idCat );

      var cat = DbCtx.Categorias.FirstOrDefault( x => x.Nombre == sCat );
      if( cat == null ) cat = new Categorium { Id = 0 };

      return cat;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el nombre de una categoria, conociendo su Id </summary>
    public string findNombre( int id)
      {
      return DbCtx.Categorias.Find(id)?.Nombre ?? "";
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el listado de todas las sub-categorias que hay en la base de dataos </summary>
    public List<IdName> getSubCategorias()
      {
      var Items = new  List<IdName>();

      foreach( var cat in DbCtx.SubCategorias )
        if( cat.Id % 10000 != 0 )
          Items.Add( new IdName( cat.Id, cat.Nombre )  );

      return Items;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Cambia los datos de una categoria existente o inserta una nueva </summary>
    public int Change( int edId,  Categorium cat )
      {
      if( edId == 0 )
        { 
        DbCtx.Categorias.Add( cat );
        }
      else
        {
        var edtCat =  DbCtx.Categorias.Find( edId );
        if( edtCat == null ) return 0;

        edtCat.Id          = cat.Id;
        edtCat.Nombre      = cat.Nombre;
        edtCat.Logo        = cat.Logo;
        edtCat.Descripcion = cat.Descripcion;
        }

      DbCtx.SaveChanges();
      return cat.Id;
      }

//---------------------------------------------------------------------------------------------------------------------------------------
/// <summary> Borra la categoria con identificador 'id' </summary>
    public void Delete( int id )
      {
      var cat = DbCtx.Categorias.Find( id );
      if( cat == null ) return;

      DbCtx.Categorias.Remove( cat );

      DbCtx.SaveChanges();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Cambia el logo de la categoria con identificador 'Id', por el dado en 'fName' </summary>
    public void ChangeLogo( int id, string fName )
      {
      DbCtx.Categorias.Find( id ).Logo = fName;

      DbCtx.SaveChanges();
      }

    }

  }
