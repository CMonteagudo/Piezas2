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
    /// <summary> Obtiene la lista de Items que satiface los parametros especificados </summary>
    public bool FindByDatos( string datos )
      {
      Filters = new ItemsFilters( datos, HttpCtx );

      if( !ValidateFilters() ) return false;              // Si hay algún filtro no valido, no retorna ningún registro

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

      return true;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la lista de Items que contengan la cadena 'name' dentro de su nombre </summary>
    internal void FindByName( string name, string datos )
      {
      Filters = new ItemsFilters( datos, HttpCtx );

      string sSelect = $"SELECT DISTINCT * FROM Item WHERE (CHARINDEX('{name}', Nombre) > 0) ";

      string sOrder = $"ORDER BY {getOrderField()} OFFSET {Filters.RegFirst} ROWS FETCH NEXT {Filters.RegCount} ROWS ONLY";     // Ordena y limita los recambios devueltos

      Items = DbCtx.Items.FromSqlRaw( sSelect + sOrder ).ToList();                                  // Ejecuta la secuencia SQL

      if( Filters.RegFirst == 0 )                                                                   // Si la primera página
        Count = DbCtx.Items.FromSqlRaw( sSelect ).Count();                                          // Calcula la cantidad de registros
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la lista de Items que tengan el código especicicado </summary>
    internal void FindByCode( int code )
      {
      var sCode = code.ToString();
      Items = DbCtx.Items.Where( i => i.Codigo == sCode ).ToList();          
      Count = Items.Count();
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