using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piezas2.Core;
using Piezas2.Core.Model;

// Comando para obtener las clases que mapean a la base de datos
// dotnet ef dbcontext scaffold "Server=(LocalDB)\MSSQLLocalDB;Database=D:\01_Proyectos\_Web\Piezas2\Core\DataBase\Piezas.mdf;Trusted_Connection=True" Microsoft.EntityFrameWorkCore.SqlServer -o Core\Models

namespace Piezas2
  {
  //=========================================================================================================================================
  /// <summary> Esta parte del API contiene todas la funciones para modificar la base de datos, se supone que solo sea usada por la interfaces
  /// que se hagan para la administracion de los datos, debe haber un nivel de seguridad alto y va a ser utilizada por un reducido número de
  /// personas </summary>
  [ApiController]
  public class DbAtions : ControllerBase
    {
    //private readonly DbPiezasContext _context;
    private readonly IWebHostEnvironment Env;
    private static readonly string dirSep = Path.DirectorySeparatorChar.ToString();

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Contruye el objeto y guarda el contexto a la base de datos para su uso posterior</summary>
    public DbAtions( /*DbPiezasContext context,*/ IWebHostEnvironment env )
      {
      //_context = context;
      Env = env;
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Quita el uso del item 'ItemId' por los coches dados en la lista 'CochesList' </summary>
    [HttpGet( "/api/del-recambio-usos/{ItemId:int}/{CochesList}" )]
    public ActionResult<string> DelRecambioUsos( int ItemId, string CochesList )
      {
      var uso    = new RecambioUsos( ItemId, HttpContext );
      var numDel = uso.DeleteCoches( CochesList );

      return $"{{\"Count\":{numDel}}}";
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona el uso del item 'ItemId' por los coches dados en la lista 'CochesList' </summary>
    [HttpGet( "/api/add-recambio-usos/{ItemId:int}/{CochesList}" )]
    public ActionResult<string> AddRecambioUsos( int ItemId, string CochesList )
      {
      var uso = new RecambioUsos( ItemId, HttpContext );
      var numAdd = uso.AddCoches( CochesList );

      return $"{{\"Count\":{numAdd}}}";
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona un nuev fabricante de coche </summary>
    [HttpPost( "/api/add-fabricante" )]
    public ActionResult<string> AddMaker( IFormFile FileFoto/*, Fabricante maker*/ )
      {
      Fabricante maker = GetMaker();
      try
        {
        maker.Id = 0;
        int Id = AddModifyMaker( FileFoto, maker );
        return OkJsn( Id );
        }
      catch( Exception ){ return ErrModify( maker.Id, maker.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Modifica los datos de un fabricante ya existente </summary>
    [HttpPost( "/api/modify-fabricante" )]
    public ActionResult<string> ModifyMaker( IFormFile FileFoto/*, Fabricante maker*/ )
      {
      Fabricante maker = GetMaker();
      try
        {
        int Id = AddModifyMaker( FileFoto, maker );
        if( Id <= 0 ) return ErrNoExist( maker.Id, maker.Nombre );

        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( maker.Id, maker.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra el fabricante de recambio especificado </summary>
    [HttpDelete( "/api/delete-fabricante" )]
    public ActionResult<string> DeleteMaker( /*Fabricante maker*/ )
      {
      var maker = GetMaker();
      int Id = maker.Id;

      try { new Fabricantes( HttpContext ).Delete( Id ); }
      catch( Exception e ) { return ErrOnDel( e, Id, maker.Nombre ); }

      DeleteFile( maker.Logo );
      return OkJsn( Id );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona una nueva marca de coche </summary>
    [HttpPost( "/api/add-marca" )]
    public ActionResult<string> AddMarca( IFormFile FileFoto/*, Marca marca*/ )
      {
      Marca marca = GetMarca();
      try
        {
        marca.Id = 0;
        int Id = AddModifyMarca( FileFoto, marca );
        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( marca.Id, marca.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Modifica los datos de una marca ya existente </summary>
    [HttpPost( "/api/modify-marca" )]
    public ActionResult<string> ModifyMarca( IFormFile FileFoto/*, Marca marca*/ )
      {
      Marca marca = GetMarca();
      try
        {
        int Id = AddModifyMarca( FileFoto, marca );
        if( Id <= 0 ) return ErrNoExist( marca.Id, marca.Nombre );

        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( marca.Id, marca.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra la marca de coche especificada </summary>
    [HttpDelete( "/api/delete-marca" )]
    public ActionResult<string> DeleteMarca( /*Marca marca*/ )
      {
      var marca = GetMarca();
      int Id = marca.Id;

      try { new Marcas( HttpContext ).Delete( Id ); }
      catch( Exception e ) { return ErrOnDel( e, Id, marca.Nombre ); }

      DeleteFile( marca.Logo );
      return OkJsn( Id );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona una nueva categoria de recambio de coche </summary>
    [HttpPost( "/api/add-categoria" )]
    public ActionResult<string> AddCategoria( IFormFile FileFoto/*, Categorium cat*/ )
      {
      var (_, cat) = GetCategoria();                                 // Obtiene los datos de la solicitud  

      return AddModifyCategoria( FileFoto, 0, cat );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Modifica los datos de una categoria ya existente </summary>
    [HttpPost( "/api/modify-categoria" )]
    public ActionResult<string> ModifyCategoria( IFormFile FileFoto/*, Categorium cat*/ )
      {
      var (edId, cat) = GetCategoria();                                 // Obtiene los datos de la solicitud  

      return AddModifyCategoria( FileFoto, edId, cat );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra la categoria de recambios de coche especificada </summary>
    [HttpDelete( "/api/delete-categoria" )]
    public ActionResult<string> DeleteCategoria( /*Categorium cat*/ )
      {
      var (edId, cat) = GetCategoria();

      try { new Categorias( HttpContext ).Delete( edId ); }
      catch( Exception e ) { return ErrOnDel( e, edId, cat.Nombre ); }

      DeleteFile( cat.Logo );
      return OkJsn( edId );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona un nuevo coche a la Base de Datos </summary>
    [HttpPost( "/api/add-coche" )]
    public ActionResult<string> AddCoche( IFormFile FileFoto/*, Coche coche*/ )
      {
      var  coches = new Coches(HttpContext);                              // Obtiene objeto para operaciones con los coches
      Coche coche = GetCoche();
      try
        {
        coche.Id = 0;                                                     // Fuerza a que se cree un registro nuevo
        int Id = AddModifyCoche( FileFoto, coche, coches );
        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( coche.Id, coches.CocheName( coche ) ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Modifica los datos de un coche ya existente </summary>
    [HttpPost( "/api/modify-coche" )]
    public ActionResult<string> ModifyCoche( IFormFile FileFoto/*, Coche coche*/ )
      {
      var  coches = new Coches(HttpContext);                             // Obtiene objeto para operaciones con los coches
      Coche coche = GetCoche();
      try
        {
        int Id = AddModifyCoche( FileFoto, coche, coches );
        if( Id <= 0 ) return ErrNoExist( coche.Id, coches.CocheName( coche ) );

        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( coche.Id, coches.CocheName(coche) ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra la marca de coche especificada </summary>
    [HttpDelete( "/api/delete-coche" )]
    public ActionResult<string> DeleteCoche( /*Coche coche*/ )
      {
      var coche  = GetCoche();
      int Id     = coche.Id;
      var coches = new Coches(HttpContext);                             // Obtiene objeto para operaciones con los coches

      try { coches.Delete( Id ); }
      catch( Exception e ) { return ErrOnDel( e, Id, coches.CocheName( coche ) ); }

      DeleteFile( coche.Foto );
      return OkJsn( Id );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona un nuevo modelo a la Base de Datos </summary>
    [HttpPost( "/api/add-modelo" )]
    public ActionResult<string> AddModelo( /* Modelo modelo*/ )
      {
      Modelo modelo = GetModelo();
      try
        {
        modelo.Id = 0;                                                     // Fuerza a que se cree un registro nuevo
        int Id = new Modelos(HttpContext).Change( modelo );
        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( modelo.Id, modelo.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Modifica los datos de un modelo ya existente </summary>
    [HttpPost( "/api/modify-modelo" )]
    public ActionResult<string> ModifyModelo( /*Modelo modelo*/ )
      {
      Modelo modelo = GetModelo();
      try
        {
        int Id = new Modelos(HttpContext).Change( modelo );
        if( Id <= 0 ) return ErrNoExist( modelo.Id, modelo.Nombre );

        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( modelo.Id, modelo.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra el modelo de coche especificado </summary>
    [HttpDelete( "/api/delete-modelo" )]
    public ActionResult<string> DeleteModelo( /*Modelo modelo*/ )
      {
      Modelo modelo = GetModelo();

      try                  { new Modelos(HttpContext).Delete( modelo.Id );   }
      catch( Exception e ) { return ErrOnDel( e, modelo.Id, modelo.Nombre ); }

      return OkJsn( modelo.Id );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona un nuevo motor a la Base de Datos </summary>
    [HttpPost( "/api/add-motor" )]
    public ActionResult<string> AddMotor( /* Motor motor*/ )
      {
      Motor motor = GetMotor();
      try
        {
        motor.Id = 0;                                                       // Fuerza a que se cree un registro nuevo
        int Id = new Motores(HttpContext).Change( motor );
        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( motor.Id, motor.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Modifica los datos de un motor ya existente </summary>
    [HttpPost( "/api/modify-motor" )]
    public ActionResult<string> ModifyMotor( /*Motor motor*/ )
      {
      Motor motor = GetMotor();
      try
        {
        int Id =  new Motores(HttpContext).Change( motor );
        if( Id <= 0 ) return ErrNoExist( motor.Id, motor.Nombre );

        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( motor.Id, motor.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra el motor de coche especificado </summary>
    [HttpDelete( "/api/delete-motor" )]
    public ActionResult<string> DeleteMotor( /*Motor motor*/ )
      {
      Motor motor = GetMotor();

      try                  { new Motores( HttpContext ).Delete( motor.Id ); }
      catch( Exception e ) { return ErrOnDel( e, motor.Id, motor.Nombre );  } 

      return OkJsn( motor.Id );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Adiciona un nuevo recambio a la Base de Datos </summary>
    [HttpPost( "/api/add-recambio" )]
    public ActionResult<string> AddRecambio( IFormFile FileFoto/*, Item pza*/ )
      {
      var piezas = new Recambios(HttpContext);                          // Obtiene objeto para operaciones con los coches
      var pza    = GetRecambio();
      try
        {
        pza.Id = 0;                                                     // Fuerza a que se cree un registro nuevo
        int Id = AddModifyRecambio( FileFoto, pza, piezas );
        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( pza.Id, pza.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Modifica los datos de un recambio ya existente </summary>
    [HttpPost( "/api/modify-recambio" )]
    public ActionResult<string> ModifyRecambio( IFormFile FileFoto/*, Item pza*/ )
      {
      var piezas = new Recambios(HttpContext);                             // Obtiene objeto para operaciones con los coches
      var pza    = GetRecambio();
      try
        {
        int Id = AddModifyRecambio( FileFoto, pza, piezas );
        if( Id <= 0 ) return ErrNoExist( pza.Id, pza.Nombre );

        return OkJsn( Id );
        }
      catch( Exception ) { return ErrModify( pza.Id, pza.Nombre ); }
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Borra el recambio espcecificado </summary>
    [HttpDelete( "/api/delete-recambio" )]
    public ActionResult<string> DeleteRecambio( /*Item pza*/ )
      {
      var pza = GetRecambio();
      int Id = pza.Id;

      try  { new Recambios(HttpContext).Delete( Id ); }
      catch( Exception e ) { return ErrOnDel( e, Id, pza.Nombre ); }

      DeleteFile( pza.Foto );
      return OkJsn( Id );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON que indica que todo estuvo bien para el elemento identifcado por 'id'  </summary>
    private string OkJsn( int id )
      {
      return $"{{\"Error\":0, \"Id\":{id} }}";
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON con el número y la descripción del error  </summary>
    private string ErrJsn( int err, string sErr )
      {
      return $"{{\"Error\":{err}, \"sError\":\"{sErr}\" }}";
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON con el código y el mensaje de cuando un record no se encuentra en la base de datos  </summary>
    private string ErrNoExist( int id, string name )
      {
      return ErrJsn( 1003, $"No se encontro en la base de datos, el registro con Id={id} y nombre '{name}'." );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON con el código y el mensaje de cuando un record no se puede modificar o adicionar  </summary>
    private string ErrModify( int id, string name )
      {
      return ErrJsn( 1001, $"Error al adicionar o modificar el regístro con Id={id} y nombe '{name}'" );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Retorna una cadena JSON con el código y el mensaje de cuando un record no se puede borrar  </summary>
    private string ErrOnDel( Exception e, int id, string name )
      {
      var sErr = $"El regístro con ID={id} y nombre '{name}' no se puede borrar";

      if( e.GetType().Name == "DbUpdateException" ) sErr += $", porque se esta utlizando.";

      return ErrJsn( 1002, sErr );
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obteine un objeto Marca con los datos obtenidos del formulario </summary>
    private Marca GetMarca()
      {
      var frm = HttpContext.Request.Form;

      return new Marca { Id = int.Parse( frm["Id"] ), Nombre = frm["Nombre"], Logo = frm["Logo"], Descripcion = frm["Descripcion"] };
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obteine un objeto Categoria con los datos obtenidos del formulario </summary>
    private (int, Categorium) GetCategoria()
      {
      var frm = HttpContext.Request.Form;

      var cat = new Categorium { Id = int.Parse( frm["Id"] ), Nombre = frm["Nombre"], Logo = frm["Logo"], Descripcion = frm["Descripcion"] };
      var edId = int.Parse( frm["edId"] );

      return( edId, cat);
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obteine un objeto Fabricante con los datos obtenidos del formulario </summary>
    private Fabricante GetMaker()
      {
      var frm = HttpContext.Request.Form;

      return new Fabricante { Id = int.Parse( frm["Id"] ), Nombre = frm["Nombre"], Logo = frm["Logo"], Descripcion = frm["Descripcion"] };
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obteine un objeto Modelo de coche con los datos obtenidos del formulario </summary>
    private Modelo GetModelo()
      {
      var frm = HttpContext.Request.Form;

      return new Modelo { Id = int.Parse( frm["Id"] ), Marca = int.Parse( frm["Marca"] ), Nombre = frm["Nombre"], Descripcion = frm["Descripcion"] };
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un objeto Coche con los datos obtenidos del formulario </summary>
    private Coche GetCoche()
      {
      var frm = HttpContext.Request.Form;

      return new Coche { Id          = int.Parse( frm["Id"         ] ), 
                         Marca       = int.Parse( frm["Marca"      ] ),
                         Modelo      = int.Parse( frm["Modelo"     ] ),
                         Motor       = int.Parse( frm["Motor"      ] ),
                         Caja        =            frm["Caja"       ],
                         Carroceria  =            frm["Carroceria" ],
                         Foto        =            frm["Foto"       ],
                         Descripcion =            frm["Descripcion"]
                       };
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un objeto Motor con los datos obtenidos del formulario </summary>
    private Motor GetMotor()
      {
      var frm = HttpContext.Request.Form;

      return new Motor
        {
        Id          = int.Parse( frm["Id"         ] ),
        Nombre      =            frm["Nombre"     ],
        Combustible = int.Parse( frm["Combustible"] ),
        Capacidad   = int.Parse( frm["Capacidad"  ] ),
        Potencia    = int.Parse( frm["Potencia"   ] ),
        Descripcion =            frm["Descripcion"]
        };
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Obtiene un objeto Items con los datos obtenidos del formulario </summary>
    private Item GetRecambio()
      {
      var frm = HttpContext.Request.Form;

      decimal.TryParse( frm["Precio"], out decimal precio );

      return new Item { Id          = int.Parse( frm["Id"         ] ),
                        Nombre      =            frm["Nombre"     ],
                        Categoria   = int.Parse( frm["Categoria"  ] ),
                        Fabricante  = int.Parse( frm["Fabricante" ] ),
                        Codigo      =            frm["Codigo"     ],
                        Foto        =            frm["Foto"       ],
                        Precio      =            precio,    
                        Descripcion =            frm["Descripcion"]
                      };
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Adiciona una nueva marca o modifica una ya existente (si marca.Id==0 crea una nueva)  </summary>
    private int AddModifyMarca( IFormFile fileFoto, Marca marca )
      {
      var nuevo = marca.Id==0;                                          // Indica que es un registro nuevo (no una modificación)
      var marcas = new Marcas(HttpContext);                             // Obtiene objetos para operaciones con las marcas

      var Id = marcas.Change( marca );                                  // Si marca.Id == 0, se interpreta como una marca nueva
      if( Id <= 0 ) return Id;

      try
        {
        string fName = SaveFile( "Logos", marca.Nombre, fileFoto );       // Guarda al fichero del logo en el directorio 'Logos'

        if( fName == null && nuevo )                                      // Si no se cargo un Logo y si es un registro nuevo
          fName = CopyFile( marca.Logo, "Logos", marca.Nombre );          // Hace un duplicado del fichero del logo

        if( fName != null )                                               // Si se cambio el logo
          marcas.ChangeLogo( Id, fName );                                 // Modifica localizacion y nombre del logo
        }
      catch( Exception ){}                                                // Ignora cualquier error guardando la imagen

      return Id;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Adiciona/Modifica una nueva categoria de acuerdo a los datos del cuerpo de la solicitud (si edId crea una nueva)  </summary>
    private string AddModifyCategoria( IFormFile fileFoto, int edId, Categorium cat )
      {
      var categorias = new Categorias(HttpContext);                     // Obtiene objetos para operaciones con las categorias

      int Id;
      try                { Id = categorias.Change( edId, cat );  }      // Si edId = 0, adiciona, si no modifica una categoria
      catch( Exception ) { return ErrModify( edId, cat.Nombre ); }      // Hubo un error modificando o borrando la categoria

      if( Id <= 0 ) return ErrNoExist( edId, cat.Nombre );              // No se encontro la categoria a borrar
                                                              
      try                                                               // Intenta guardar el logotipo de la categoria
        {
        var name = "Categoria" + cat.Id;                                // Nombre que va ha tener el fichero
        var fName = SaveFile( "Logos", name, fileFoto );                // Trata de guardar el logo, subido con la solicitud

        if( fName == null && edId == 0 )                                // Si no se subio un Logo y si es un registro nuevo
          fName = CopyFile( cat.Logo, "Logos", name );                  // Hace un duplicado del fichero del logo

        if( fName != null )                                             // Si se cambio el logo
          categorias.ChangeLogo( Id, fName );                           // Modifica localizacion y nombre del logo en la BD
        }
      catch( Exception ) { }                                            // Ignora cualquier error guardando el logo

      return OkJsn( Id );
      }

  //---------------------------------------------------------------------------------------------------------------------------------------
  ///<summary> Adiciona un nuevo fabricante o modifica uno ya existente (si maker.Id==0 crea uno nuevo)  </summary>
  private int AddModifyMaker( IFormFile fileFoto, Fabricante maker )
      {
      var nuevo = maker.Id==0;                                          // Indica que es un registro nuevo (no una modificación)
      var makers = new Fabricantes(HttpContext);                        // Obtiene objetos para operaciones con los fabricantes

      var Id = makers.Change( maker );                                  // Si maker.Id == 0, se interpreta como una marca nueva
      if( Id <= 0 ) return Id;

      try
        {
        var fName = SaveFile( "Logos", maker.Nombre, fileFoto );        // Guarda al fichero del logo en el directorio 'Logos'

        if( fName == null && nuevo )                                    // Si no se cargo un Logo y si es un registro nuevo
          fName = CopyFile( maker.Logo, "Logos", maker.Nombre );        // Hace un duplicado del fichero del logo

        if( fName != null )                                             // Si se cambio el logo
          makers.ChangeLogo( Id, fName );                               // Modifica localizacion y nombre del logo
        }
      catch( Exception ) { }                                            // Ignora cualquier error guardando la imagen

      return Id;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Adiciona un nuevo coche o modifica uno ya existente (si coche.Id==0 crea uno nueva)  </summary>
    private int AddModifyCoche( IFormFile fileFoto, Coche coche, Coches coches )
      {
      var nuevo = coche.Id==0;                                          // Indica que es un registro nuevo (no una modificación)

      var Id = coches.Change( coche );                                  // Si coche.Id == 0, se interpreta como un coche nueva
      if( Id <= 0 ) return Id;

      try
        {
        var srvName = $"{Id}-{coches.CocheName(coche)}";                // Nombre del fichero de la foto en el servidor
        var fName   = SaveFile( "Coches", srvName, fileFoto );          // Guarda al fichero de imagen

        if( fName == null && nuevo )                                    // Si no se cargo un Logo y si es un registro nuevo
          fName = CopyFile( coche.Foto, "Coches", srvName );            // Hace un duplicado del fichero del logo

        if( fName != null )                                             // Si se cambio el logo
          coches.ChangeFoto( Id, fName );                               // Modifica localizacion y nombre del logo
        }
      catch( Exception ) { }                                            // Ignora cualquier error guardando la imagen

      return Id;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Adiciona un nuevo coche o modifica uno ya existente (si coche.Id==0 crea uno nueva)  </summary>
    private int AddModifyRecambio( IFormFile fileFoto, Item pza, Recambios piezas )
      {
      var nuevo = (pza.Id==0);                                          // Indica que es un registro nuevo (no una modificación)

      var Id = piezas.Change(pza);                                      // Realiza los cambios de la BD
      if( Id <= 0 ) return Id;                                          // Si hubo un error, retorna sin hacer nada

      try
        {
        var name  = $"Pza {pza.Codigo.Trim()}-{Id}";                    // Nombre de la foto del recambio en el servidor
        var fName = SaveFile( "Items", name, fileFoto );                // Guarda la foto, si fue modificada

        if( fName == null && nuevo )                                    // Si no se cargo la foto y es un registro nuevo
          fName = CopyFile( pza.Foto, "Items", name );                  // Hace un duplicado de la con el nombre nuevo

        if( fName != null )                                             // Si se cambio el logo
          piezas.ChangeFoto( Id, fName );                               // Modifica localizacion y nombre del logo
        }
      catch( Exception ) { }                                            // Ignora cualquier error guardando la imagen

      return Id;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Guarda la foto cargada desde el cliente en el servidor  </summary>
    private string SaveFile( string dir, string name, IFormFile fileFoto )
      {
      if( fileFoto == null || fileFoto.Length == 0 ) return null;

      var ext      = Path.GetExtension( fileFoto.FileName );
      var fotoName = Path.Combine( "images", dir, $"{name}{ext}" );
      var wwwRoot  = Env.WebRootPath;
      var path     = Path.Combine( wwwRoot, fotoName );

      using( FileStream stream = new FileStream( path, FileMode.Create ) )
        {
        fileFoto.CopyTo( stream );
        }

      return dirSep + fotoName;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    // Hace una copia del fichero 'nowFile' hacia el fichero nombrado 'Name' del directorio 'Dir'
    private string CopyFile( string nowFile, string Dir, string Name )
      {
      if( string.IsNullOrWhiteSpace( nowFile ) ) return null;

      var root    = Env.WebRootPath;
      var oldPath = Path.Combine( root, nowFile.TrimStart(dirSep[0]) );

      var ext     = Path.GetExtension( nowFile );
      var newFile = Path.Combine( "images", Dir, Name + ext );
      var newPath = Path.Combine( root, newFile );

      System.IO.File.Copy( oldPath, newPath, true );
      return dirSep + newFile;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    ///<summary> Borra un fichero del servidor </summary>
    private bool DeleteFile( string fName )
      {
      try
        {
        if( string.IsNullOrWhiteSpace( fName ) ) return false;

        var path = Path.Combine( Env.WebRootPath, fName.TrimStart( dirSep[0] ) );
        System.IO.File.Delete( path );

        return true;
        }
      catch( Exception ) { return false; }
      }

    }//=========================================================================================================================================
  }
