// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Maneja una combinacion de 'btn-group' y 'dropdown-menu', con funcionalidad similas al ComboBox de windows
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


