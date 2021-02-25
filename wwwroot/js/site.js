// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Maneja una combinacion de 'btn-group' y 'dropdown-menu', con funcionalidad similar al ComboBox de windows
class ComboBox
  {
  // Contructor con el Id del grupo
  constructor( elem, selFun  )
    {
    this.frame = $( elem );
    this.text = this.frame.children().eq(0);
    this.menu = this.frame.children().eq(2);
    this.selFun = selFun;

    this.Clear();
    }

  // Elimina todos los items que ha dentro del menu
  Clear()
    {
    this.menu.empty();
    this.text.html("&emsp;"); 

    this.Items = [];
    this.selItem = -1;
    }

  // Obtiene el # de items que hay en el combo
  Count()
    {
    return this.Items.length;
    }

  // Adiciona un elemento al menú y la función que debe ser llamada al seleccionar el item
  AddItem( id, Name )
    {
    var isTenue = id<=0 ? "class='tenue'" : "";
    var item = $( "<li " + isTenue + "><a href='#'>" + Name + "</a></li>" ) ;

    this.menu.append( item );
    this.Items.push( {"Id":id, "Name":Name } );

    var self = this;
    item.on( 'click', function( ev )
      {
      self.SelectById( id );

      self.frame.removeClass( "open" );
      ev.stopPropagation();
      } );
    }

  // Selecciona el item del menú que tenga el id indicado
  SelectById( id )
    {
    for( var i=0; i<this.Items.length; ++i )
      if( this.Items[i].Id == id )
        return this.ChangeSelection(i);

    return false;
    }

  // Selecciona el item del menú que tenga el nombre indicado
  SelectByName( Name )
    {
    for( var i = 0; i < this.Items.length; ++i )
      if( this.Items[i].Name == Name )
        return this.ChangeSelection( i );

    return false;
    }

  // Cambia el texto a del item seleccionado
  ChangeSelection( idx )
    {
    var itm = this.Items[ idx ];

    this.text.text( itm.Name );
    this.selItem = idx;

    if( itm.Id <= 0 ) this.text.addClass("tenue");
    else              this.text.removeClass( "tenue" );

    if( this.selFun ) this.selFun( itm.Id, itm.Name );

    return true;
    }

  // Obtiene el item seleccionado, de no haber ninguno retorna 0
  SelectedId()
    {
    var i = this.selItem;
    if( i <= 0 || i >= this.Items.length ) return 0;

    return this.Items[i].Id;
    }


}

// Muestra un cursor de espera sobre el elemento indicado
class WaitCursor
  {
  // Construye el objeto y muestra el cursor
  constructor( elem  )
    {
    var frm = $(elem);                        // Elemento que sive de marco para poner el cursor
    var old = frm.find("#wCursor");           // Busca si el elemeneto ya tiene un cursor
    if( old ) old.remove();                   // Lo quita

    // Cuadro que cubre el elemento y muestra el cursor en el medio
    var htm = '<div id="wCursor" style="position: absolute; width: 100%; height: 100%; top: 0; left: 0; display: flex; justify-content: center; background-color: #e9e9e944; align-items: center; ">'+
                '<img src="/images/wait-cur.gif"/>'+
              '</div>';

    this.cursor  = $(htm);                    // Crea el marco con el cursor
    this.frm     = frm;
    this.lastPos = frm.css("position");       // Si el elemento no tiene el atributo de posición correcto

    if( this.lastPos != "absolute" )
      frm.css( "position", "relative" );      // Lo arregla

    frm.append( this.cursor );                // Adiciona el cursor sobre el elemento
    }

  // Oculta el cursor de espera
  Hide()
    {
    this.frm.css( "position", this.lastPos ); // Retorna la posicion anterior
    this.cursor.remove();                     // Borra el elemento que ya habia puesto
    }
  }

// Maneja la interface para filtrar las los recambios
class UIFilters
  {
  // Construye el objeto y muestra el cursor
  constructor( Datos, IDs  )
    {
    this.Datos = Datos;

    this.FabNames = new Map();                                                          // Crea un nomenclador para fabricantes
    for( let item of this.Datos.Fabric ) this.FabNames.set( item.Id, item.Name );

    this.CatNames = new Map();                                                          // Crea un nomenclador para categorias
    for( let item of this.Datos.Categor ) this.CatNames.set( item.Id, item.Name );

    this.CreateCbMarca( IDs.Marca );                                                    // Crea todos los objetos para manejar los comboboxs
    this.CreateCbModelo( IDs.Modelo );
    this.CreateCbMotores( IDs.Motor );

    this.CreateCbCategorias( IDs.Categor );
    this.CreateCbFabricantes( IDs.Fabric );

    if( this.cbMotor  ) this.cbMotor.SelectById ( localStorage[ "lastSelMotor"  ] || 0 );                   // Restaura el último valor usado para cada uno de los combos
    if( this.cbModelo ) this.cbModelo.SelectById( localStorage[ "lastSelModelo" ] || 0 );
    if( this.cbMarca  ) this.cbMarca.SelectById ( localStorage[ "lastSelMarca"  ] || 0 );

    this.cbCategoria.SelectById ( localStorage[ "lastSelCategor" ] || 0 );
    this.cbFabricante.SelectById( localStorage[ "lastSelFabric"  ] || 0 );
    }

  // Obtiene un nomenclador para los fabricantes
  FabName( ID )
    {
    return this.FabNames.get(ID) || "";
    }

  // Obtiene un nomenclador para las categorias
  CatName( ID )
    {
    return this.CatNames.get(ID) || "";
    }

  // Crea un objeto para manejar el combo con las marcas de los coches
  CreateCbMarca( ID )
    {
    if( !ID ) return;
    this.cbMarca = new ComboBox( ID, (id) =>{
                                            localStorage[ "lastSelMarca" ] = id;
                                            this.FillCbModelos();
                                            } );

    this.cbMarca.AddItem( 0, "Todas las Marcas", 1 );

    for( let item of this.Datos.Marcas )
      this.cbMarca.AddItem( item.Id, item.Name );
    }

  // Crea un objeto para manejar el combo con los modelos de los coches
  CreateCbModelo( ID )
    {
    if( !ID ) return;
    this.cbModelo = new ComboBox( ID, (id) => {
                                              localStorage[ "lastSelModelo" ] = id;
                                              this.FillCbMotores();
                                              } );

    this.FillCbModelos( true );
    }

  // Crea un objeto para manejar el combo con los Motores de los coches
  CreateCbMotores( ID )
    {
    if( !ID ) return;
    this.cbMotor = new ComboBox( ID, (id) => {localStorage["lastSelMotor"]=id;}  );

    this.FillCbMotores( true );
    }

  // Crea un objeto para manejar el combo con las categorias de los recambios
  CreateCbCategorias( ID )
    {
    this.cbCategoria = new ComboBox( ID, (id) => { localStorage[ "lastSelCategor" ] = id; } );

    this.cbCategoria.AddItem( 0, "Todas las Categorias", 1 );

    for( let item of this.Datos.Categor )
      this.cbCategoria.AddItem( item.Id, item.Name );
    }

  // Crea un objeto para manejar el combo con los fabricantes de los recambios
  CreateCbFabricantes( ID )
    {
    this.cbFabricante = new ComboBox( ID, (id) => { localStorage[ "lastSelFabric" ] = id; } );

    this.cbFabricante.AddItem( 0, "Todas los Fabricantes", 1 );

    for( let item of this.Datos.Fabric )
      this.cbFabricante.AddItem( item.Id, item.Name );
    }

  // Llena el combo modelos de acuerdo a la marca que este seleccionada
  FillCbModelos( noSel )
    {
    var lastName  = "";
    var nowModelo = this.cbModelo.SelectedId();
    var nowMarca  = this.cbMarca.SelectedId();

    this.cbModelo.Clear();
    this.cbModelo.AddItem( 0, "Todos los Modelos", 1 );

    var nowSel = 0;
    for( let mod of this.Datos.Modelos )
      {
      if( mod.Nombre != lastName && ( nowMarca==0 || nowMarca==mod.Marca)  )
        {
        this.cbModelo.AddItem( mod.Id, mod.Nombre );

        if( mod.Id==nowModelo ) nowSel=mod.Id;
        lastName = mod.Nombre;
        }
      }

    if( !noSel ) this.cbModelo.SelectById( nowSel );
    }

  // Llena el combo motores de acuerdo a la marca y al modelo seleccionado
  FillCbMotores( noSel )
    {
    var lastName  = "";
    var nowModelo = this.cbModelo.SelectedId();
    var nowMarca  = this.cbMarca.SelectedId();
    var nowMotor  = this.cbMotor.SelectedId();

    this.cbMotor.Clear();
    this.cbMotor.AddItem( 0, "Todos los Motores", 1 );

    var nowSel = 0;
    for( let mot of this.Datos.Motores )
      {
      if( mot.Nombre != lastName && ( nowMarca == 0 || nowMarca == mot.Marca ) && ( nowModelo == 0 || nowModelo == mot.Modelo ) )
        {
        this.cbMotor.AddItem( mot.Id, mot.Nombre );

        if( mot.Id == nowMotor ) nowSel = mot.Id;
        lastName = mot.Nombre;
        }
      }

    if( !noSel ) this.cbMotor.SelectById( nowSel );
    }

  // Obtiene los segmentos de Url que representan los datos seleccionados
  GetUrlSegments()
    {
    var marca  = this.cbMarca?  this.cbMarca.SelectedId() : "0";
    var modelo = this.cbModelo? this.cbModelo.SelectedId() : "0";
    var motor  = this.cbMotor?  this.cbMotor.SelectedId() : "0";
    var fabric = this.cbFabricante.SelectedId();
    var categ  = this.cbCategoria.SelectedId();

    return marca + "/" + modelo + "/" + motor + "/" + categ + "/" + fabric;
    }

  }

// Muesta un mensaje con una alerta para el usuario dentro de la misma página web
class MsgAlert
  {
  // Construye el objeto y configura el elemento para mostrar la alerta
  constructor( elem  )
    {
    this.msg = $(elem);                        // Elemento donde se va a mostrar el mensaje (generalmente un div)
    this.tipo = "alert-info";                  // Tipo de mensaje a mostrar

    this.msg.addClass("alert");
    this.msg.addClass("hidden");
    this.msg.addClass( this.tipo );
    }

  // Muestra el mensaje del tipo dado, 'tipo' acepta los mismo tipo que booptrap (info, warning, danger, success)
  Show( txt, tipo="info" )
    {
    this.msg.removeClass( this.tipo );
    this.tipo = "alert-" + tipo;
    this.msg.addClass( this.tipo );

    this.msg.html( txt );

    var bntClose = $("<div class='btn-"+ tipo +" btn-close-msg'>x</div>");    // Crea un botón para ocualtar el mensaje
    bntClose.on( 'click', () => this.Hide() );                                      // Pone el evento para el ocultamiento

    this.msg.append( bntClose );               // Adiciona botón de cerrar al mensaje

    this.msg.removeClass("hidden");
    }

  // Oculta el mensaje
  Hide()
    {
    this.msg.addClass("hidden");
    }
  }

// Maneja una conexion con el servidor para realizar una solicitud
class ServerConnection
  {
  // Construye el objeto y configura el elemento para mostrar la alerta
  constructor( metodo, funError  )
    {
    this.metodo = metodo || "GET";                // Metodo empleado para la conexion (GET(por defecto),POST,PUT,DELETE,PATCH)
    this.fError = funError || this.ShowError;     // Función que se llama cuando hay error, por defecto 'alert'
    this.body   = null;                           // Información que se manda en el cuerpo de la solicitud
    }

  // Envia la solicitud al servidor a traves de la URL dada y devueleve los resultados a traves de la función 'funResult( jsonResult )'
  Send( Url, funResult, curElm )
    {
    var cur = curElm? new WaitCursor(curElm) : null;
    $.ajax( {
      url: Url, body:this.body, method:this.metodo,
      complete: (xhr) =>
        {
        if( cur ) cur.Hide();

        var json = this.checkReturn( xhr );

        if( json.Error == 0 ) funResult( json );
        else                  this.fError( json );
        }
      } );
    }

  // Chequea que el retorno desde Ajax es corecto
  checkReturn( xhr )
    {
    if( xhr.readyState === 4 )
      {
      if( xhr.status === 200 )                                      // La conexión termino OK
        {
        try 
          { 
          var jSon = JSON.parse( xhr.responseText );                // Valida si la respuesta es un JSON ok
          if( !jSon.Error ) jSon.Error = 0;                         // Marca que no hay error, si no venia ninguno del servidor
          return jSon;
          }             
        catch( e )                                                  // La respuesta no se pudo convertir a JSON
          { 
          return { Error: -1, sError: "La respuesta del servidor fue erronea" }; 
          }   
        }

      var sErr;
      var Code = xhr.status;
      switch( Code )                                        // La conexión no termino
        {
        case 400: sErr = "La solicitud realizada es errorea"; break;
        case 401: sErr = "El recurda solicitado requiere autorizacion"; break;
        case 402: sErr = "Acceso denegado"; break;
        case 404: sErr = "La Url solicitada no fue encontrada"; break;
        case 405: sErr = "El método empleado en la petición no esta permitido"; break;
        case 406: sErr = "El servidor no puede dar una de las respuestas aceptadas"; break;
        case 408: sErr = "Se agotó el tiempo para responder a la solicitud"; break;
        case 410: sErr = "El recurso ya no esta disponible en el servidor"; break;
        case 400: sErr = "El recurso ya no esta disponible en el servidor"; break;
        case 500: sErr = "Ocurrio un error intero en el servidor"; break;
        case 501: sErr = "Al menos hay una funcionalidad que no esta implementada en el servidor"; break;
        case 505: sErr = "Versión de HTTP no soportada"; break;
        case 507: sErr = "No hay espacio sificiente en el servidor"; break;
        case 509: sErr = "Se ha excesido en ancho de banda disponible"; break;
        case 511: sErr = "Se requiere la atenticación del navegador"; break;
        default: 
                if( Code>=500 ) sErr = "El servidor fallo al completar la solicitud";
          else if( Code>=400 ) sErr = "La solicitud tiene una sintaxis incorrecta 0 no puede porcesarse";
          else if( Code>=300 ) sErr = "El cliente debe tomar una acción adicional para completar la solicitud";
          else if( Code>=200 ) sErr = "La petición fue atendida correctamete";
          else if( Code>=100 ) sErr = "La patición fue recibida y continua en proceso";
        }

      return { Error: Code, sError: sErr };
      }
    else return { Error: -1, sError: "No se puede conectar con el servidor" };       // La conexión no se realizo
    }

  // Muestra los errores producidos
  ShowError( json )
    {
    var sMsg = "";
    if( json.Error>0 ) sMsg = "Error " + json.Error + ": ";                                     // Si el error es negativo no pone el codigo

    alert( sMsg + json.sError );
    }
  }
