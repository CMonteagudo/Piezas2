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
  /// <summary> Obtiene una lista de recambio según los datos suministrados </summary>
  public class Recambios
    {
    /// <summary> Lista de filtros y datos que se deben usar en la consulta </summary>
    public ItemsFilters Filters { set; get; }

    /// <summary> Lista Items encontrados después de aplicar los filtros </summary>
    public List<Item> Items { get; set; } = new List<Item>();

    /// <summary> Número de items totales sin aplicar la paginación, solo se calcula para la primera página </summary>
    public int Count { get; set; } = -1;      // -1 => No se calculo la cantidad de registros

    private readonly HttpContext HttpCtx;
    private readonly DbPiezasContext DbCtx;

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la lista de Items que satiface los parametros especificados </summary>
    public Recambios( HttpContext HttpCtx )
      {
      this.HttpCtx = HttpCtx;
      this.DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                 // Obtiene contexto a la BD
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Busca el recambio con el identificador dado y retorna sus datos o null si fue encontrado </summary>
    public Item Find( int Id )
      {
      return DbCtx.Items.Find( Id );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la lista de Items que satiface los parametros especificados </summary>
    public Recambios FindByDatos( string datos )
      {
      Filters = new ItemsFilters( datos, HttpCtx );

      if( !ValidateFilters() ) return this;              // Si hay algún filtro no valido, no retorna ningún registro

      string sSelect = $"SELECT DISTINCT I.Id, I.Fabricante, I.Categoria, I.Nombre, I.Codigo, I.Foto, I.Precio, I.Descripcion";

      var hasCoche = Filters.Marca.Id> 0 || Filters.Modelo.Id>0 || Filters.Motor.Id>0;      // Si hay que tener en cuenta el coche
      var sTable   = hasCoche? "Item AS I INNER JOIN ItemCoche AS IC ON I.Id = IC.IdItem INNER JOIN Coche AS C ON IC.IdCoche = C.Id" : "Item AS I";

      StringBuilder sWhere = new StringBuilder("");
      if( hasCoche || Filters.Categoria.Id> 0 || Filters.Fabricante.Id> 0 )                 // Si hay algun tipo de filtro
        {
        var sep = "";

        sWhere.Append("WHERE ");

        if( Filters.Marca.Id  > 0 ) { sWhere.Append(      $"C.Marca  = {Filters.Marca.Id}"  ); sep = " AND "; }                 // Filtra por la marca de coche
        if( Filters.Modelo.Id > 0 ) { sWhere.Append( $"{sep}C.Modelo = {Filters.Modelo.Id}" ); sep = " AND "; }                 // Filtra por el modelo del coche
        if( Filters.Motor.Id  > 0 ) { sWhere.Append( $"{sep}C.Motor  = {Filters.Motor.Id}"  ); sep = " AND "; }                 // Filtra por el motor del coche

        if( Filters.Categoria.Id  > 0 ) { sWhere.Append( $"{sep}I.Categoria  = {Filters.Categoria.Id}"   ); sep = " AND "; }    // Filtra por la categoria de la pieza
        if( Filters.Fabricante.Id > 0 ) { sWhere.Append( $"{sep}I.Fabricante = {Filters.Fabricante.Id}" ); }                    // Filtra por el fabricante de la pieza
        }

      string sOrder = $"ORDER BY {getOrderField()} OFFSET {Filters.RegFirst} ROWS FETCH NEXT {Filters.RegCount} ROWS ONLY";     // Ordena y limita los recambios devueltos

      Items = DbCtx.Items.FromSqlRaw( $"{sSelect} FROM {sTable} {sWhere} {sOrder}" ).ToList();                                  // Ejecuta la secuencia SQL

      if( Filters.RegFirst == 0 )                                                                                               // Si la primera página
        Count = DbCtx.Items.FromSqlRaw( $"{sSelect} FROM {sTable} {sWhere}" ).Count();                                          // Calcula la cantidad de registros

      return this;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Inserta o cambia un recambio, de acuerdo a los datos suministrados </summary>
    public int Change( Item pza )
      {
      if( pza.Id == 0 )
        {
        var newItem = DbCtx.Items.Add( pza );

        DbCtx.SaveChanges();
        return newItem.Entity.Id;
        }
      else
        {
        var edtItem =  DbCtx.Items.Find( pza.Id );
        if( edtItem == null) return 0;

        edtItem.Nombre      = pza.Nombre;
        edtItem.Codigo      = pza.Codigo;
        edtItem.Precio      = pza.Precio;
        edtItem.Foto        = pza.Foto;
        edtItem.Categoria   = pza.Categoria;
        edtItem.Fabricante  = pza.Fabricante;
        edtItem.Descripcion = pza.Descripcion;

        DbCtx.SaveChanges();
        return pza.Id;
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Borra el recambio, indicado por el id </summary>
    public void Delete( int id )
      {
      var item = DbCtx.Items.Find( id );
      if( item == null ) return;

      DbCtx.Items.Remove( item );

      DbCtx.SaveChanges();
      }

      //---------------------------------------------------------------------------------------------------------------------------------------
      /// <summary> Cambia la foto del recambio por la foto especificada </summary>
      internal void ChangeFoto( int id, string fotoName )
      {
      DbCtx.Items.Find( id ).Foto = fotoName;

      DbCtx.SaveChanges();
      }


    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la lista de Items que contengan la cadena 'name' dentro de su nombre </summary>
    internal Recambios FindByName( string name, string datos )
      {
      Filters = new ItemsFilters( datos, HttpCtx );

      string sSelect = $"SELECT DISTINCT * FROM Item WHERE (CHARINDEX('{name}', Nombre) > 0) ";

      string sOrder = $"ORDER BY {getOrderField()} OFFSET {Filters.RegFirst} ROWS FETCH NEXT {Filters.RegCount} ROWS ONLY";     // Ordena y limita los recambios devueltos

      Items = DbCtx.Items.FromSqlRaw( sSelect + sOrder ).ToList();                                  // Ejecuta la secuencia SQL

      if( Filters.RegFirst == 0 )                                                                   // Si la primera página
        Count = DbCtx.Items.FromSqlRaw( sSelect ).Count();                                          // Calcula la cantidad de registros

      return this;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la lista de Items que tengan el código especicicado </summary>
    internal Recambios FindByCode( int code )
      {
      var sCode = code.ToString();
      Items = DbCtx.Items.Where( i => i.Codigo == sCode ).ToList();          
      Count = Items.Count();

      return this;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Retorna la lista de items que no son usados por ningún coche </summary>
    internal List<IdName> FindNoUsed()
      {
      string sSelect = "SELECT I.* FROM Item AS I LEFT JOIN ItemCoche AS IC ON I.Id = IC.IdItem WHERE ISNULL(IC.IdItem,0)= 0";    // Ordena y limita los recambios devueltos

      return DbCtx.Items.FromSqlRaw( sSelect ).Select( i => new IdName( i.Id, i.Nombre ) ).ToList();                              // Ejecuta la secuencia SQL
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Retorna la lista de items son usados al menos por un coche </summary>
    internal List<IdName> FindUsed()
      {
      string sSelect = "SELECT I.Id, Nombre FROM Item AS I INNER JOIN ItemCoche AS IC ON I.Id = IC.IdItem GROUP BY I.Id, Nombre ORDER BY COUNT(I.Id) DESC OFFSET 0 ROWS";    // Ordena y limita los recambios devueltos

      return DbCtx.Items.FromSqlRaw( sSelect ).Select( i => new IdName( i.Id, i.Nombre ) ).ToList();                              // Ejecuta la secuencia SQL
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene el nombre del campo que se va a utilizar para ordenar</summary>
    private object getOrderField()
      {
      return Filters.Orden switch       // OrdenFields = { "codigo", "categoria", "fabricante", "nombre" }
          {
           1 => "Codigo",
           2 => "Categoria",
           3 => "Fabricante",
           4 => "Nombre",
          -1 => "Codigo DESC",
          -2 => "Categoria DESC",
          -3 => "Fabricante DESC",
          -4 => "Nombre DESC",
           _ => "Id"
          };
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Chequea que todos los filtros sean validos, en caso contrario retorna false</summary>
    private bool ValidateFilters()
      {
      // <0 => El fitro no se puede cumplir, 0 => No se filtra por ese campo, >0 => El campo debe cumplir con ese valor
      // <0 => Es cuando el formaro es incorrecto o el valor no esta en la base de datos
      var ok = Filters.Marca.Id      >=0 && 
               Filters.Modelo.Id     >=0 && 
               Filters.Motor.Id      >=0 && 
               Filters.Categoria.Id  >=0 && 
               Filters.Fabricante.Id >=0;

      if( ok ) return true;           // Todos los filtros OK

      Count = 0;                      // Pone contador de registros en 0
      return false;                   // No es necesario buscar, porque el menos un filtro no se va a cumplir
      }

    } //=======================================================================================================================================
  }