using Microsoft.AspNetCore.Http;
using Piezas2.Core;
using Piezas2.Core.Model;
using System.Collections.Generic;

namespace Piezas2.Models
  {
  //=======================================================================================================================================
  /// <summary> Obtiene todos los datos necesarios para el cuadro de busqueda de recambios </summary>
  public class VentasModel : BaseModel
    {
    public List<Ventum> Ventas { set; get; } = new List<Ventum>();
    public int    Error { set; get; }
    public string sError { set; get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto y obtiene los datos de la base de datos </summary>
    public VentasModel( HttpContext HttpCtx ) : base( HttpCtx )
      {
      try
        {
        Ventas = new Ventas( HttpCtx ).ListVentas( _UserId, 0 );
        }
      catch( System.Exception e)
        {
        (Error, sError) = retJson.ErrorDatos( 1009, "No se puedo obtener el listado articulos pendientes de pago", e);
        }
      }
    }
  }
  