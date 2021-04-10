﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

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

  // Obtiene el item el identificador del item seleccionado, de no haber ninguno retorna 0
  SelectedId()
    {
    var i = this.selItem;
    if( i <= 0 || i >= this.Items.length ) return 0;

    return this.Items[i].Id;
    }

  // Obtiene el item el nombre del item seleccionado, de no haber ninguno retorna ""
  SelectedName()
    {
    var i = this.selItem;
    if( i <= 0 || i >= this.Items.length ) return 0;

    return this.Items[i].Name;
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
    if( this.Datos.Fabric )
      for( let item of this.Datos.Fabric ) this.FabNames.set( item.Id, item.Name );

    this.CatNames = new Map();                                                          // Crea un nomenclador para categorias
    if( this.Datos.Categor )
      for( let item of this.Datos.Categor ) this.CatNames.set( item.Id, item.Name );

    this.CreateCbMarca( IDs.Marca );                                                    // Crea todos los objetos para manejar los comboboxs
    this.CreateCbModelo( IDs.Modelo );
    this.CreateCbMotores( IDs.Motor );

    this.CreateCbCategorias( IDs.Categor );
    this.CreateCbSubCategorias( IDs.SubCate );
    this.CreateCbFabricantes( IDs.Fabric );

    if( this.cbMotor  ) this.cbMotor.SelectById ( localStorage[ "lastSelMotor"  ] || 0 );                   // Restaura el último valor usado para cada uno de los combos
    if( this.cbModelo ) this.cbModelo.SelectById( localStorage[ "lastSelModelo" ] || 0 );
    if( this.cbMarca  ) this.cbMarca.SelectById ( localStorage[ "lastSelMarca"  ] || 0 );

    if( this.cbSubCategoria ) this.cbSubCategoria.SelectById ( localStorage[ "lastSelSubCategor" ] || 0 );
    this.SelCategoria();
    this.SelFabricante();
    }

  // Selecciona la categoria con el Id dado o si es null la ultima categoria swlwccionada
  SelCategoria( ID )
    {
    if( ID==null ) ID = localStorage["lastSelCategor"] || 0;

    if( this.cbCategoria ) this.cbCategoria.SelectById ( ID );
    }

  // Selecciona el fabricante con el Id dado o si es null selecciona el último utilizado
  SelFabricante( ID )
    {
    if( ID==null ) ID = localStorage["lastSelFabric"] || 0;

    if( this.cbFabricante ) this.cbFabricante.SelectById ( ID );
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
    if( !ID ) return;
    this.cbCategoria = new ComboBox( ID, (id) => { 
                                                 localStorage[ "lastSelCategor" ] = id; 
                                                 if( this.cbSubCategoria ) this.FillCbSubCategoria();
                                                 } );

    this.cbCategoria.AddItem( 0, "Todas las Categorias", 1 );

    for( let item of this.Datos.Categor )
      {
      if( item.Id>0 ) this.cbCategoria.AddItem( item.Id, item.Name );
      else            this.cbCategoria.AddItem( item.id, item.nombre );
      }
    }

  // Crea un objeto para manejar el combo con las sub-categorias de los recambios
  CreateCbSubCategorias( ID )
    {
    if( !ID ) return;
    this.cbSubCategoria = new ComboBox( ID, (id) => { localStorage[ "lastSelSubCategor" ] = id; } );

    this.FillCbSubCategoria( true );
    }

  // Llena el combo de subcategorias de acuerdo a la categoria que este seleccionada
  FillCbSubCategoria( noSel )
    {
    var nowCat    = this.cbCategoria.SelectedId();
    var nowSubCat = this.cbSubCategoria.SelectedId();

    this.cbSubCategoria.Clear();
    this.cbSubCategoria.AddItem( 0, "Todas las Subcategorias", 1 );

    var nowSel = 0;
    for( let item of this.Datos.SubCate )
      {
      if( nowCat==0 || parseInt(nowCat/10000)==parseInt(item.Id/10000) )
        {
        this.cbSubCategoria.AddItem( item.Id, item.Name );
  
        if( item.Id==nowSubCat ) nowSel=item.Id;
        }
      }

    if( !noSel ) this.cbSubCategoria.SelectById( nowSel );
    }

  // Crea un objeto para manejar el combo con los fabricantes de los recambios
  CreateCbFabricantes( ID )
    {
    if( !ID ) return;
    this.cbFabricante = new ComboBox( ID, (id) => { localStorage[ "lastSelFabric" ] = id; } );

    this.cbFabricante.AddItem( 0, "Todas los Fabricantes", 1 );

    for( let item of this.Datos.Fabric )
      {
      if( item.Id>0 ) this.cbFabricante.AddItem( item.Id, item.Name );
      else            this.cbFabricante.AddItem( item.id, item.nombre );
      }
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
    var marca  = this.GetSelMarca();
    var modelo = this.GetSelModelo();
    var motor  = this.GetSelMotor();
    var fabric = this.GetSelFrabricante();
    var categ  = this.GetSelCategoria();

    return marca + "/" + modelo + "/" + motor + "/" + categ + "/" + fabric;
    }

  GetSelMarca()   { return this.cbMarca?  this.cbMarca.SelectedId()  : "0"};
  GetSelModelo()  { return this.cbModelo? this.cbModelo.SelectedId() : "0"};
  GetSelMotor()   { return this.cbMotor?  this.cbMotor.SelectedId()  : "0"};

  GetSelMarcaName()   { return this.cbMarca?  this.cbMarca.SelectedName()  : ""};
  GetSelModeloName()  { return this.cbModelo? this.cbModelo.SelectedName() : ""};
  GetSelMotorName()   { return this.cbMotor?  this.cbMotor.SelectedName()  : ""};

  GetSelFrabricante()  { return this.cbFabricante?   this.cbFabricante.SelectedId()   : "0"};
  GetSelCategoria()    { return this.cbCategoria?    this.cbCategoria.SelectedId()    : "0"};
  GetSelSubCategoria() { return this.cbSubCategoria? this.cbSubCategoria.SelectedId() : "0"};
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
    this.data   = null;                           // Información que se manda en el cuerpo de la 
    }

  // Envia la solicitud al servidor a traves de la URL dada y devueleve los resultados a traves de la función 'funResult( jsonResult )'
  Send( Url, funResult, curElm )
    {
    var cur = curElm? new WaitCursor(curElm) : null;
    $.ajax( {
      url: Url, data:this.data, method:this.metodo, 
      complete: (xhr) =>
        {
        //setTimeout( ()=>                        // Para emular una demora en la carga de los datos
        //  {
          if( cur ) cur.Hide();

          var json = this.checkReturn( xhr );

          if( json.Error == 0 ) funResult( json );
          else                  this.fError( json );
          //}, 3000 );
        }
      } );
    }

  SendForm( form, Url, funResult )
    {
    var cur = new WaitCursor( form );
    try 
      { 
      var oReq = new XMLHttpRequest();
      oReq.open( this.metodo, Url, true );
      oReq.onload = (oEven)=>
        {
        cur.Hide();

        var json = this.checkReturn( oReq );

        if( json.Error == 0 ) funResult( json );
        else                  this.fError( json );
        };

      oReq.send( new FormData( form ) ); 
      }
    catch( e )                                                  
      { 
      cur.Hide();
      this.fError( { Error: -1, sError: "Error al conectarse con el servidor" } ); 
      }   
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
        case 204: sErr = "La solicitud se realizo correctamente, pero no se devolvio ningún contenido"; break;
        case 400: sErr = "La solicitud realizada es errorea"; break;
        case 401: sErr = "El recurda solicitado requiere autorizacion"; break;
        case 402: sErr = "Acceso denegado"; break;
        case 404: sErr = "La Url solicitada no fue encontrada"; break;
        case 405: sErr = "El método empleado en la petición no esta permitido"; break;
        case 406: sErr = "El servidor no puede dar una de las respuestas aceptadas"; break;
        case 408: sErr = "Se agotó el tiempo para responder a la solicitud"; break;
        case 410: sErr = "El recurso ya no esta disponible en el servidor"; break;
        case 415: sErr = "Tipo de medio no disponible en el servidor"; break;
        case 500: sErr = "Ocurrio un error interno en el servidor"; break;
        case 501: sErr = "Al menos hay una funcionalidad que no esta implementada en el servidor"; break;
        case 505: sErr = "Versión de HTTP no soportada"; break;
        case 507: sErr = "No hay espacio sificiente en el servidor"; break;
        case 509: sErr = "Se ha excesido en ancho de banda disponible"; break;
        case 511: sErr = "Se requiere la atenticación del navegador"; break;
        default: 
                if( Code>=500 ) sErr = "El servidor fallo al completar la solicitud";
          else if( Code>=400 ) sErr = "La solicitud tiene una sintaxis incorrecta o no puede procesarse";
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

// Muestra los errores producidos durante la solicitud de datos via AJAX.
// json = Respuesta del servidor en el formato {Error:..., sError:... }
//        donde: Error -> Número del error y sError -> Descripción del error.
// msg  = Objeto para mostrar errores dentro de la página del tipo MsgAlert
function ConnError( json, msg )
  {
  var sMsg = "";
  if( json.Error>0 ) sMsg = "<b> Error:" + json.Error + "</b> ";

  msg.Show( sMsg + json.sError, "danger");
  }

// Se usa en el evento OnLoad de un tag img para hacer que la imagen se centre en su contenedor, 
// verticalmente o horizontalmente de acuerdo a su relacion de aspecto
function CenterImg(e)
  {
  var img = e.currentTarget;

  if( img.naturalWidth > img.naturalHeight )
    { img.style.width = "100%"; img.style.height = "auto"; }
  else
    { img.style.width = "auto"; img.style.height = "100%"; }
  };

// Chequea que le entrada Inp tenga un valor entero entre min y max
function ValidateNum( Inp, name, min, max, int=false )
  {
  var num = +Inp.val();

  var sErr = "";
       if( isNaN(num) ) sErr = ", debe ser un valor númerico";
  else if( num <  min ) sErr = ", debe ser mayor o igual que " + min;
  else if( num >  max ) sErr = ", debe tener un valor menor que " + max;
  else if( int && num % 1!=0 ) sErr = ", debe ser un número entero";
  else return true;

  alert( name + sErr );
  Inp.focus();
  return false;
  }

// Verifica que la entrada inp tenga una cadena entre min y max caracteres
function ValidateStr( Inp, name, min, max )
  {
  var sVal = Inp.val();

  var sErr = "";
       if( sVal.length < min ) sErr = ", debe de tener al menos " + min + " caracteres.";
  else if( sVal.length > max ) sErr = ", debe de tener menos de " + max + " caracteres.";
  else return true;

  alert( name + sErr );
  Inp.focus();
  return false;
  }

// Verifica que la entrada inp tenga un valor seleccionado
function ValidateSel( Inp, name, def=-1 )
  {
  var val = Inp.val();

  var sErr = "";
       if( val == null ) sErr = "Debe seleccionar ";
  else if( val == def  ) sErr = "Debe cambiar ";
  else return true;

  alert( sErr + name );
  Inp.focus();
  return false;
  }

// Selecciona en la tabla 'idTable' la fila con identificador 'idRow'
function SelectTableRow( idRow, Table, tbFrm )
  {
  var tbBody = $(Table).children().eq(0);
  tbBody.find( ".row-selected" ).removeClass( "row-selected" );   // Quita la seleccion todas la filas

  if( idRow==-1 ) return -1;                                      // idRow = -1, solo quita la selección 

  var rows = tbBody.children();                                   // Toma todas las filas
  for( let i=0; i<rows.length; i++ )                              // Recorre todas las filas
    {
    var row = rows.eq(i);
    if( row.data("id") == idRow )                                // Id de la fila coincide con el buscado
      {
      row.addClass( "row-selected" );                            // Selecciona la fila
      setRowVisible( row, tbFrm );                               // Asegura que la fila sea visible dentro de 'tbFrm'

      var nextIdx = i<rows.length-1? i+1 : i-1;                   // Busca un id, cercano
      if( nextIdx < 0 ) return -1;

      return rows.eq(nextIdx).data("id");                         // Retorna ID cercano al seleccionado
      }
    }

  return -1;
  }

// Garantiza que la fila 'rowItem' sea visible dentro del frame que cotiene la tabla 'tbFrm'
function setRowVisible( rowItem, tbFrm )
  {
  var view = $(tbFrm)[0];
  var elm = rowItem[0];

  var Y = view.scrollTop;
  var H = view.clientHeight;

  var y = elm.offsetTop;
  var h = elm.clientHeight;

  if( y > Y && y+h < Y+H ) return;

  view.scrollTop = y - (H-h)/2;
  }

// Muestra/Oculta el panel izquierdo para seleccionar el recambio a editar
function AnimatePanel( hide=false )
  {
  var bnt = $("#show-lst-panel" )[0];
  if( !bnt || bnt.style.display=="none" ) return;           // No hace nada si el boton de cambio no esta visible

  var pnl  = $("#panelList");
  var left = pnl.position().left;
      
  pnl[0].style.left= (hide||left>=0)? "-310px" : "0px";

  HidePopUp();
  }

// Esta funcion chequea si el click se produjo dentro de una ventana popup
function CheckClickOnPopup( e )
  {
  var popups = $(".filters-popup");
  for( var i=0; i < popups.length; i++ )
    {
    var rect = $(".filters-popup")[i].getBoundingClientRect();

    if( e.clientX >= rect.left && e.clientX <= rect.right && e.clientY >= rect.top && e.clientY <= rect.bottom )
      return;
    }

  HidePopUp();
  }

// Oculta todas las ventana para filtos que esten abiertas
function HidePopUp( e )
  {
  if( e )
    {
    $(e.currentTarget).parent().parent().removeClass("open"); 
    e.stopPropagation();
    }
  else
    $(".filters-btn").removeClass("open");
  }

var rz;

// Muetra una ventana en el medio de la pantalla con la url dada
function ShowWidget( url, fnReturn )
  {
  var html ='<div id="NewRec">'+
              '<iframe src="'+ url +'"></iframe>'+
            '</div>';

  $("body").append(html);

  $("#NewRec iframe").on( "load" , e=>{ InitWidget( e ); } );       // Pone función de notificaón del Widget
  $("#NewRec"       ).on( "click", e=>{ Notify("close"); } );       // Cierra cuando click fuera del frame


  // Se llama después de cargarse la página en el Widget
  function InitWidget( e )
    { 
    var win = e.currentTarget.contentWindow;

    win.WidgetNotify = Notify;
    }

  // Atiende las notificaciones recibidas desde el Widget
  function Notify( event, data )
    { 
    if( event=="resize" )
      {
      $("#NewRec iframe").height( data );
      return;
      }

    document.getElementById("NewRec").remove();
    fnReturn( event, data );
    }
  }

// Configura el modo Widget cuando la pagina est lista para usar
// ObserverElem - Elemento que al cambiar de tamaño debe cambiar el tamño del Widget
function ModoWidgetConfig( ObserverElem )
  {
  if( !ModoWidget ) return;                                       // Si no esta en modo Widget no hace nada

  if( !WidgetNotify )                                             // Si no hay una función de notificación definida
    WidgetNotify = function(e){ if( e=="add" || e=="modify" || e=="delete") ClearItemDatos(); };    // Pone una por defecto

  WidgetNotify( "resize", $("html").height() );                   // Noticica para que cambie de tamaño

  if( typeof(ResizeObserver) == undefined ) return;               // Si no se soporta ResizeObserver termina

  var rz = new ResizeObserver( el=>{                              // Cada vez que 'ObserverElem' cambia de tamaño
    var elm = document.getElementsByTagName("html")[0];           // Obtiene la pagina completa

    WidgetNotify( "resize", $("html").height() );                 // Ajusta la altura del Widget a la altura de la página
    });

  rz.observe( ObserverElem );                                     // Observa cuando 'ObserverElem' cambia de tamaño
  }


// Pone los estilos para que la para la zona de edición en el modo Widget
function setWidgetStyles()
  {
  $("#btnClose").css( "display", "block" );                                         // Maneja boton cerrar del Widget
  $("#btnClose").on( 'click', ()=>{WidgetNotify( "close");} );   

  $(".columns-for-edit").css( "display", "block" )                                  // Quita el layout para las 2 columnas

  $(".main-frame").css( {"min-height":"initial", "margin":"1", "padding":"0"} );    // Ajusta tamaño y margenes del marco principal de la pagina
  $("#edit-rec").css( {"height":"initial"} );                                       // Quita el tamño fijo del pane de datos
  }



// ===================================================================================================================================================
var _PopUp;
var _NoHide;
// Clase para manejar los recuadros flotantes
function PopUp( clickElem, opciones )
  {
  "use strict";  
  var box;                                       // Cuadro que se va a mostrar
  var clkElem = $(clickElem);                    // Elemento que el hacer click sale el cuadro de dialogo
  var posElem = clkElem;
  var opt = opciones || {};
  var $this = this;
  var frm = $("body");  
  var Slide = opt.Slide;                         // Define si el cuadro se va a mover hacia los lados
  var mvBox = true;                              // El recuadro al cerrarse se mueve hacia el grupo #HidedBoxs
  var isCb = false;                              // El popup es del tipo combo con una lista de items
  var callFn = null;
  
  var pos = clkElem.css( "position" );
  if( pos!=="absolute" && pos!=="relative" )
    {clkElem.css( "position", "relative" );}
    
  {clkElem.css( "cursor", "pointer" );}
  
  if( opt.NoClick!==1 ) {clkElem.click( ShowPopup );}
  if( opt.PosElem ) {posElem = $(opt.PosElem);}

  this.idxSel = -1;
  this.OnShowPopUp = null;
  this.OnClosePopUp = null;
  this.OnVisiblePopUp = null;
  this.SetCallBack = function(fn) {callFn=fn;};
  this.GetBox = function(){return box;}; 
  this.Show   = function(){ ShowPopup(); }; 
    
  this.BoxFromList = function( list, cb, CallBack  )
    {
    if( box )  
      {box.removeAttr("style");
       box.remove();}
       
    callFn = CallBack;
    isCb   = cb;
      
    var s = ""; 
    if( opt.w    ) { s = 'width:'+ opt.w +'px; ';}
    if( opt.hMax ) { s+= 'max-height:'+ opt.hMax +'px; overflow-y: auto;';} 
    if( s !== "" ) { s = 'style="'+ s + '"';} 
    
    var html = '<div class="popup" '+ s + '>';
    
    for( var i=0; i<list.length; ++i )
      {
      var str = list[i];  
      html += '<div class="item-pu" item="'+ i +'">'+ str +'</div>';
      }
      
    html += '</div>';
    box = $(html);
    
    box.find(".item-pu").click( SelectedItem );
    };
    
  this.SetBox = function( Box, ClkClose ) 
    {
    box = $(Box);
    if( !ClkClose ) { box.click( function(e){ e.stopPropagation();} ); }
    if( mvBox ) { box.css( {display:'none', position:'absolute'} ); }
    };
    
  this.UseBox = function( Box, Click ) {mvBox=false; this.SetBox(Box,Click);};

  this.SetSelItem = function( iSel )
    { 
    if( !isCb ) {return;} 
    
    var Items = box.find(".item-pu");
    if( iSel<0 || iSel>=Items.length )
      { 
      clkElem.text("");
      this.idxSel = -1;
      }
    else
      { 
      clkElem.text( Items.eq(iSel).text() );
      this.idxSel = iSel;
      }
    };
   
  function ShowPopup( e )
    {
    if( _PopUp )
      {
      if( _PopUp===opt.NoHide ) { _NoHide = _PopUp; }
      else
        {  
        var ret = (_PopUp === $this)? null : ShowPopup;
        return _PopUp.ClosePopUp( e, ret );
        }
      }
 
    if( _NoHide && _NoHide!==opt.NoHide ) { return _NoHide.ClosePopUp( e, ShowPopup ); }
      
    if( !box ) {console.log("No box set"); return;}
    if( !mvBox && !Slide ) {console.log("Only for Slide"); return;}
        
    if( $this.OnShowPopUp ) {$this.OnShowPopUp();}   
    
    if( Slide ) { SlideLeftBox(); }
    else        { PushDownBox(); }
    
    $("html").one("click",$this.ClosePopUp );

    _PopUp = $this;
    
    if(e) {e.stopPropagation();}
    }
     
  this.ClosePopUp = function( e, fn, par )
    {
    if( Slide ) { SlideRightBox(fn, par); }
    else        { PushUpBox(fn, par); }

    if(e) {e.stopPropagation();}
    
    if( $this === _PopUp  ) { _PopUp  = undefined;}
    if( $this === _NoHide ) { _NoHide = undefined;}
    
    if( $this.OnClosePopUp ) {$this.OnClosePopUp();}
    };
    
  function SelectedItem()
    { 
    if( isCb ) 
      {
      clkElem.text( $(this).text() );
      box.find(".item-pu").click( SelectedItem );
      }
      
    $this.idxSel = +$(this).attr("item");

    $this.ClosePopUp( null, callFn, $this.idxSel ); 
    return false;
    }
    
  function PushDownBox()
    {
    clkElem.append( box );
    
    if( opt.wBtn  ) { box.outerWidth( clkElem.outerWidth() ); }
    
    var x = 0;
    var ye = clkElem.outerHeight();
    var wb = box.outerWidth(); 
    var mg = opt.mg || 10;
    var xe = clkElem.offset().left;
    if( opt.right )
      {
      var we = clkElem.outerWidth();
      var xi = xe - (wb - we) - mg;
      if( xi<0 ) {x = (xe+we)-(mg+wb);}
      
      box.css( "right", x );
      }
    else
      {
      var W = $("body").outerWidth();
      var xf = xe + wb + mg;
      
      if( xf>W ) { x = -(xf-W); }
      box.css( "left", x );
      }
    
    box.css( "top", ye );
  
    box.slideDown( 200, function()
      { 
      if( $this.OnVisiblePopUp ) {$this.OnVisiblePopUp();}
      });
    }    

  function PushUpBox( fn, par )
    {
    if( !box ) {return;}  
    box.slideUp(200, function()
      { 
      if( mvBox ) {$("#HidedBoxs").append( box );}
      else        {box.removeAttr("style");}
      
      if( fn ) {fn(par);}
      });
    }  

  var xi;   
  function SlideLeftBox()
    {
    if( mvBox ) {frm.append( box );} 
    
    var pos = posElem.offset();
    if( opt.Up ) { window.scroll({top:pos.top}); }
    
    pos.top += posElem.height();
    pos.left  = frm.width() + 10;
    
    box.css( {display:"block"} );
    frm.css( "overflow-x","hidden" );
    box.offset( pos );  
    
    xi = box.position().left;
    var mg = opt.mg || 15;
    var w = box.outerWidth(); 
    var xf = xi - ( 10 + w + mg );
    
    box.animate( {left: xf}, 500, function(){frm.css( "overflow-x","visible" );} );
    }
      
  function SlideRightBox( fn, par )
    {
    if( !box ) {return;}  
    frm.css( "overflow-x","hidden" );
    box.animate( {left: xi}, 500, function()
      { 
      if( mvBox ) {$("#HidedBoxs").append( box );} 
      else        {box.removeAttr("style");}
      
      frm.css( "overflow-x","visible" );
      
      if( fn ) {fn(par);}
      });
    }  

  }

// Obtine los datos del usuario logueado, u objeto vacio
function GetLogUser()
  {
  "use strict";  
  if( sessionStorage.User ) 
    return JSON.parse(sessionStorage.User);  
    
  return {};
  }
  
// Pone el usuario logueado
function SetLogUser()
  {
  var txt = "Entrar";
  if( sessionStorage.User ) 
    txt = JSON.parse(sessionStorage.User).nombre;  

  $("#btnLog span").text(txt) 
  }
  

// Maneja todas la opciones del menú principal
function UsersMenu()
  {
  "use strict";  
  var $this = this;
  this.ChangeUser = SetLogUser;

  var reCode = /^[0-9]{6,6}$/;
  var reTelf = /^\+?([0-9 \-]{7,}[,; ]*)+$/;
  var reMail = /^([a-z0-9\+_\-]+)(\.[a-z0-9\+_\-]+)*@([a-z0-9\-]+\.)+[a-z]{2,6}$/i;
      
  var bntLog = $("#btnLog");
   
  var mnuLogIn = new PopUp( bntLog );
  mnuLogIn.SetBox("#BoxUser");
  
  SetLogUser();

  $("#LogIn input").eq(1).val("");
  
  // Llama cada vez que sale el dialogo de usuario 
  mnuLogIn.OnShowPopUp = function()
    { 
    var User = GetLogUser();
    
    if( (User.id) ) showUserMnu();
    else            showLogIn();
    };
  
  // Se llama cuando se va ha loguear un usuario y contrasña
  $("#LogIn .btnRight").click(function() 
    {
    var Msg = $("#LogIn .MsgError");
    Msg.hide();
    
    var UserIn = $("#LogIn input");

    var pwd  = UserIn.eq(1).val();  
    var Data = 'Password='+encodeURIComponent(pwd);
    
    var name = UserIn.eq(0).val();
    Data += '&Nombre='+encodeURIComponent( name );  
    
    var Conn = new ServerConnection( "POST", (r)=>{ Msg.html(r.sError); Msg.show(); } );
    Conn.data = Data;

    Conn.Send( "/api/login", (user) => 
      {
      if( user.id > 0 )
        {
        if( user.confirmado ) LoginOk( user );
        else                  ConfirmCode( user.id );
        }
      else
        {
        var sErr = "Usuario y/o Contraseñas incorrecto." +
                   "<br/><br/><span style='color:#555'>Si olvido la contraseñas, oprima <a href=''style='font-weight: bold;'>aqui</a> para enviarsela por correo</span>";
                    
        Msg.html( sErr );
        Msg.find("a").click(function() {return EnviaPassWord(Data);} );
                  
        Msg.show();
        }  

      }, "#BoxUser" );
    });    

  // Termina el proceso de registro para el usuario dado
  function LoginOk( user )
    {
    sessionStorage.User = JSON.stringify(user);         // Guarda datos del usuario en la sección
                
    mnuLogIn.ClosePopUp(null, function()                // Cierra la ventana flotante
      {
      showUserMnu();                                    // Para la proxima utilización
      $this.ChangeUser();                               // Notifica a la página 
      });
    }

  // Envia la contraseñas al correo del usuario
  function EnviaPassWord( Data )
    {
    var Msg = $("#LogIn .MsgError");
    Msg.hide();
    
    var Conn = new ServerConnection( "POST", (r)=>{ Msg.html(r.sError); Msg.show(); } );
    Conn.data = Data;

    Conn.Send( "/api/send-password", () => { showMsgPassWord(); }, "#BoxUser" );
      
    return false;  
    }
  
  // Muestra dialogo para validar el código de confirmacion del correo
  function ConfirmCode( userId )
    {
    var Msg = $("#ConfirmCode .MsgError");                          // Para poner mensajes de errores
    Msg.hide();

    $("#ConfirmCode input").eq(1).val( userId );                    // Pone el identificador en un campo oculto
    
    showConfirmCode();                                              // Muestra pantalla para pone código de confirmación

    $("#ConfirmCode span").click( ()=>                              // Click para re-enviar el código
      {
      var Conn = new ServerConnection( "POST", (r)=>{ Msg.html(r.sError); Msg.show(); } );
      Conn.data = 'Id=' + userId;

      Conn.Send( "/api/send-code", (user) =>                        
        { 
        var msg = $("<div>Código enviado al correo ...</div>");

        $("#ConfirmCode .link-bottom").append(msg);                 // Pone un mensaje
        $("#ConfirmCode span").hide();                              // Quita el link para re-enviar

        setTimeout( ()=>                                            // Pone un intervalo de tiempo de 3 seg
          {
          msg.remove();                                             // Quita el mensaje
          $("#ConfirmCode span").show();                            // Muestra el link de re-enviar otra vez
          }, 5000 );
        
        }, "#BoxUser" );
      });
    }
  
  // Se llama para re-enviar el código de verificación
  $("#ConfirmCode .btnRight").click(function() 
    {
    var Msg = $("#ConfirmCode .MsgError");
    Msg.hide();
    
    var entries = $("#ConfirmCode input"); 
    
    var code   = entries.eq(0).val();
    var userId = entries.eq(1).val();
    
    if( !reCode.test(code) ) 
      { 
      Msg.text( "El código de validación son 6 digitos numéricos");
      Msg.show();
      entries.eq(0).focus();
      return;
      }
      
    var Data ='Code='+code+'&Id='+userId;

    var Conn = new ServerConnection( "POST", (r)=>{ Msg.html(r.sError); Msg.show(); } );
    Conn.data = Data;

    Conn.Send( "/api/veryfy-code", (user) => 
      { 
      if( user.confirmado ) LoginOk( user );
      else
        {
        Msg.text( "El código de validación es incorrecto");
        Msg.show();
        entries.eq(0).focus();
        }
      }, "#BoxUser" );
    });    

  // Desloguea al usuario actual      
  function OnLogOut()
    {
    var Msg = $("#mnuUser .MsgError");
    Msg.hide();

    var Conn = new ServerConnection( "POST", (r)=>{ Msg.html(r.sError); Msg.show(); } );

    Conn.Send( "/api/logout", (user) => 
      {
      mnuLogIn.ClosePopUp( null, function() 
        {
        delete sessionStorage.User;
        $this.ChangeUser();
        });
      }, "#BoxUser" );
    }
   
  // Llena formulario con datos de usuario actual 
  function FillDatos( User )
    {
    var Ctrs = $("#EditUser input");
    
    Ctrs.eq(0).val( User.correo );
    Ctrs.eq(1).val( User.nombre);
    
    Ctrs.eq(2).val(User.passWord);
    Ctrs.eq(3).val(User.passWord);
    
    Ctrs.eq(4).val( User.telefonos );
    }
   
  // Obtiene y valida los datos del usuario
  function GetUserData( Msg, box ) 
    {
    Msg.hide();
    
    var mail   = box.eq(0).val();
    var name   = box.eq(1).val();
    var pwd1   = box.eq(2).val();
    var pwd2   = box.eq(3).val();
    var telef  = box.eq(4).val();
    
    var txt="", foco;
         if( !reMail.test(mail)  ) { txt="El correo es erroneo";            foco = box.eq(0); }
    else if( name.length<3       ) { txt="Falta el nombre del usuario";     foco = box.eq(1); }
    else if( pwd1.length<1       ) { txt="Tiene que poner una contraseñas"; foco = box.eq(2); }
    else if( pwd1 !== pwd2       ) { txt="Las contraseñas no coinciden";    foco = box.eq(3); }
    else if( !reTelf.test(telef) ) { txt="El teléfono es erroneo";          foco = box.eq(4); }
      
    if( txt.length>0 ) { Msg.text(txt); Msg.show();  foco.focus(); return false; }
      
    return 'Correo='+encodeURIComponent(mail)+'&Nombre='+encodeURIComponent(name)+'&PassWord='+encodeURIComponent(pwd1)+'&Telefonos='+encodeURIComponent(telef);
    }
    
  // Se llama para crear un usuario nuevo
  $("#NewUser .btnRight").click(function() 
    {
    var Msg = $("#NewUser .MsgError");
    
    var Data = GetUserData( Msg, $("#NewUser input") );
    if( !Data ) {return;}

    Data +='&Id=0';
    
    var Conn = new ServerConnection( "POST", (r)=>{ Msg.html(r.sError); Msg.show(); } );
    Conn.data = Data;

    Conn.Send( "/api/add-usuario", (r) => { ConfirmCode( r.Id ); }, "#BoxUser"  );
    });    

  // Se llama para editar los datos del usuario actual
  $("#EditUser .btnRight").click(function() 
    {
    var Msg = $("#EditUser .MsgError");
    
    var Data = GetUserData( Msg, $("#EditUser input") );
    if( !Data ) {return;}

    var User = GetLogUser();
    if( !User.id ) 
      {
      Msg.text("Para modificar sus datos, el usuario debe estar logueado."); 
      Msg.show();  
      return false;
      }

    Data +='&Id='+User.id;
    
    var Conn = new ServerConnection( "POST", (r)=>{ Msg.html(r.sError); Msg.show(); } );
    Conn.data = Data;

    Conn.Send( "/api/modify-usuario", (r) => 
      { 
      if( r.confirm ) showMsgUpdate(); 
      else            ConfirmCode( r.Id ); 
      }, "#BoxUser"  );
    });    

  // Atiende los comados para los dialogos de usuario
  $("#BoxUser .item-pu").click(function() 
    {
    var cmd = +$(this).attr("cmd");
    
    switch( cmd )
      {
      case 1: OnLogOut(0);    break;  // Salir
      case 2: showLogIn();    break;  // Otro usuario
      case 3: showNewUser();  break;  // Usuario nuevo
      case 4: showEditUser(); break;  // Modificar usuario
      }
    });  
  
  // Pone el menú de inicio
  $("#BoxUser input").keyup( function(){$("#BoxUser .MsgError").hide();} );

  // Pone el menú de inicio
  $("#BoxUser .btnLeft").click(function() { return showUserMnu(); });  

  // Oculta todos los paneles
  function hideAll()
    {
    $("#LogIn"      ).hide();
    $("#NewUser"    ).hide();
    $("#ConfirmCode").hide();
    $("#mnuUser"    ).hide();
    $("#EditUser"   ).hide();
    $("#MsgPassWord").hide();
    $("#MsgUpdate"  ).hide();
    }

  // Muestra el panel para registrase como un usuario
  function showLogIn()      // OnSwUser(0)
    {
    hideAll();
    $("#LogIn").css("display", "block" );           // Muestra el panel de registro de usuario
    $("#LogIn input").eq(0).focus();                // Pone el foco en la primera entrada
    }

  // Muestra el panel para registrase como un usuario
  function showNewUser()      // OnSwUser(1)
    {
    hideAll();
    $("#NewUser").css("display", "block" );         // Muestra el panel con datos de nuevo usuario
    $("#NewUser input").eq(0).focus();              // Pone el foco en la primera entrada
    }

  // Muestra el panel para poner el código de activación
  function showConfirmCode()      // OnSwUser(2)
    {
    hideAll();
    $("#ConfirmCode").css("display", "block" );     // Muestra el panel de verificar el código
    $("#ConfirmCode input").eq(0).focus();          // Pone el foco en la primera entrada
    }

  // Muestra el menú con las opciones a realizar con los usuarios
  function showUserMnu()    // OnSwUser(3)
    {
    hideAll();

    var mnu = $("#mnuUser .item-pu");                                 // Items del menú de usuarios
    if( GetLogUser().id ) { mnu.eq(0).show(); mnu.eq(3).show(); }     // Si logueado, muestra Salir y Modificar
    else                  { mnu.eq(0).hide(); mnu.eq(3).hide(); }     // Oculta Salir y Modificar

    $("#mnuUser").css("display", "block" );           // Muestra el panel con el menú de usuario
    }

  // Muestra el panel para editar los datos del usuario
  function showEditUser()      // OnSwUser(4)
    {
    hideAll();
    $("#EditUser").css("display", "block" );          // Muestra el panel de registro de usuario
    $("#EditUser input").eq(0).focus();               // Pone el foco en la primera entrada

    FillDatos( GetLogUser() );                        // Llena los datos del usuario
    }

  // Muestra un mensaje del envio de la contraseñas por correo
  function showMsgPassWord()      // OnSwUser(5)
    {
    hideAll();
    $("#MsgPassWord").css("display", "block" );       // Muestra el panel con el mensaje
    }

  // Muestra un mensaje que para actualizar los datos hay que salir
  function showMsgUpdate()      // OnSwUser(6)
    {
    hideAll();
    $("#MsgUpdate").css("display", "block" );         // Muestra el panel con el mensaje
    }

  } // Fin de la clase UsersMenu


