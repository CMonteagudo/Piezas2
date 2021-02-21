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

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene la lista de Items que satiface los parametros especificados </summary>
    public Recambios( string datos, HttpContext HttpCtx )
      {
      Filters = new ItemsFilters( datos, HttpCtx );

      if( !ValidateFilters() ) return;              // Si hay algún filtro no valido, no retorna ningún registro

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

      var DbCtx = (DbPiezasContext) HttpCtx.RequestServices.GetService(typeof(DbPiezasContext));                                // Obtiene contexto a la BD

      Items = DbCtx.Items.FromSqlRaw( $"{sSelect} FROM {sTable} {sWhere} {sOrder}" ).ToList();                                  // Ejecuta la secuencia SQL

      if( Filters.RegFirst == 0 )                                                                                               // Si la primera página
        Count = DbCtx.Items.FromSqlRaw( $"{sSelect} FROM {sTable} {sWhere}" ).Count();                                          // Calcula la cantidad de registros
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

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Toma la secuencia 'items' y la ordena según lo indicado en los filtros </summary>
    private IQueryable<Item> OrderBy( IQueryable<Item> items )
      {
      return Filters.Orden switch       // OrdenFields = { "codigo", "categoria", "fabricante", "nombre" }
          {
           1 => items.OrderBy( x => x.Codigo     ),
           2 => items.OrderBy( x => x.Categoria  ),
           3 => items.OrderBy( x => x.Fabricante ),
           4 => items.OrderBy( x => x.Nombre     ),
          -1 => items.OrderByDescending( x => x.Codigo     ),
          -2 => items.OrderByDescending( x => x.Categoria  ),
          -3 => items.OrderByDescending( x => x.Fabricante ),
          -4 => items.OrderByDescending( x => x.Nombre     ),
          _ => items
          };
      }

    } //=======================================================================================================================================
  }