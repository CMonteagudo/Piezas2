/*
import { isNumeric } from "jquery";

paypal.Buttons( {
  createOrder: function( data, actions )
  {
    return actions.order.create( {
      purchase_units: [
        {
          reference_id: "PUHF",
          description: "Sporting Goods",

          custom_id: "CUST-HighFashions",
          soft_descriptor: "HighFashions",
          amount:
          {
            currency_code: "USD",
            value: "230.00",
            breakdown:
            {
              item_total:
              {
                currency_code: "USD",
                value: "180.00"
              },
              shipping: {
                currency_code: "USD",
                value: "30.00"
              },
              handling: {
                currency_code: "USD",
                value: "10.00"
              },
              tax_total: {
                currency_code: "USD",
                value: "20.00"
              },
              shipping_discount: {
                currency_code: "USD",
                value: "10"
              }
            }
          },
          items: [
            {
              name: "T-Shirt",
              description: "Green XL",
              sku: "sku01",
              unit_amount: {
                currency_code: "USD",
                value: "90.00"
              },
              tax: {
                currency_code: "USD",
                value: "10.00"
              },
              quantity: "1",
              category: "PHYSICAL_GOODS"
            },
            {
              name: "Shoes",
              description: "Running, Size 10.5",
              sku: "sku02",
              unit_amount: {
                currency_code: "USD",
                value: "45.00"
              },
              tax: {
                currency_code: "USD",
                value: "5.00"
              },
              quantity: "2",
              category: "PHYSICAL_GOODS"
            }
          ],
          shipping: {
            method: "United States Postal Service",
            address: {
              name: {
                full_name: "John",
                surname: "Doe"
              },
              address_line_1: "123 Townsend St",
              address_line_2: "Floor 6",
              admin_area_2: "San Francisco",
              admin_area_1: "CA",
              postal_code: "94107",
              country_code: "US"
            }
          }
        } ]
    } )
  },
  onApprove: function( data, actions )
  {
    return actions.order.capture().then( function( details )
    {
      alert( 'Transaction completed by ' + details.payer.name.given_name )
      // Call your server to save the transaction
      return fetch( '/api/paypal-transaction-complete', {
        method: 'post',
        headers: {
          'content-type': 'application/json'
        },
        body: JSON.stringify( {
          orderID: data.orderID
        } )
      } )
    } )
  }
} );
*/

//==================================================================================================================================================================
// Maneja el proceso se creacion de la orden para el botón de compras de Paypal
class PayOrden
  {
  // Se una orden para el pago en Paypal
  constructor( factura, monto, moneda )
    {
    this.datos = {};

    if( typeof(factura ) !="string" ) return this._Error( "Debe de especificarse un identificador de factura" );
      
    this.datos.invoice_id = factura;
    this.datos.amount     =  this._PayVal( monto, moneda );
    }

  // Establece una descripción para la orden (opcional)
  Descripcion( Desc )
    {
    if( typeof(Desc) != "string" ) return this._Error( "La descripción es incorrecta" );

    this.datos.description = Desc;
    }

  // Establece otros datos para la orden (opcionales)
  OtrosDatos( RefId, CustomId, SoftDesc )
    {
    if( typeof( RefId    )=="string" ) this.datos.reference_id    = RefId;
    if( typeof( CustomId )=="string" ) this.datos.custom_id       = CustomId;
    if( typeof( SoftDesc )=="string" ) this.datos.soft_descriptor = SoftDesc;
    }

  // Adiciona un articulo a la orden y retorna el objeto para cambiar sus propiedades
  AddItem( nombre, cantidad, precio, moneda )
    {
    if( typeof( nombre ) != "string" ) return this._Error("Debe de especificarse el nombre del item");

    var cantVal = Number(cantidad);
    if( !cantVal>0 )  return this._Error("Debe de especificarse una cantidad de item mayor a 0");

    var precVal = Number(precio);
    if( !precVal>0 )  return this._Error("Debe de especificarse un precio por item mayor a 0");

    var item = {};
     
    item.name        = nombre;
    item.quantity    = String(cantVal);
    item.unit_amount = this._PayVal( precio, moneda );

    var body = this.datos;
    if( body.items == undefined ) body.items = [];

    body.items.push( item );
    return item
    }

  // Estable que el Item se puede entregar de manera digital o es un item fisico (opcional)
  ItemCategoria( item, digital )
    {
    var Itm = this._GetItem( item );
    if( Itm ) Itm.category = digital? "DIGITAL_GOODS" : "PHISICAL_GOODS";
    }

  // Estable una descripción detallada para el item (opcional)
  ItemDescripcion( item, desc )
    {
    if( typeof( desc )!="string" )  return this._Error("El valor proporcionado para la descripción del Item es incorrecto"); 
      
    var Itm = this._GetItem( item );
    if( Itm ) Itm.description = desc;
    }

  // Estable el código de almacenamiento del item (SKU) (opcional)
  ItemSku( item, sku )
    {
    if( typeof( sku )!="string" )  return this._Error("El valor proporcionado para el SKU del Item es incorrecto"); 
      
    var Itm = this._GetItem( item );
    if( Itm ) Itm.sku = desc;
    }

  // Pone el impuesto asociado a un item (opcional)
  ItemImpuesto( item, valor, moneda )
    {
    var Itm = this._GetItem( item );
    if( Itm ) Itm.tax = this._PayVal( valor, moneda );
    }

  // Parte del desglose del monto de la orden perteneciente a los items (opcional)
  DesgloseItems( valor, moneda )
    {
    this._GetDesglose().item_total = this._PayVal( valor, moneda );
    }

  // Parte del desglose del monto de la orden perteneciente al envio (opcional)
  DesgloseEnvio( valor, moneda )
    {
    this._GetDesglose().shipping = this._PayVal( valor, moneda );
    }

  // Parte del desglose del monto de la orden perteneciente la manipulación (opcional)
  DesgloseManipulacion( valor, moneda )
    {
    this._GetDesglose().handling = this._PayVal( valor, moneda );
    }

  // Parte del desglose del monto de la orden perteneciente los impuestos (opcional)
  DesgloseImpuesto( valor, moneda )
    {
    this._GetDesglose().tax_total = this._PayVal( valor, moneda );
    }

  // Parte del desglose del monto de la orden perteneciente los descuentos (opcional)
  MotoDescuento( valor, moneda )
    {
    this._GetDesglose().shipping_discount = this._PayVal( valor, moneda );
    }

  // Pone los datos principales del envio  (opcional)
  Envio( metodo, nombre, apellido, codPostal, codPais )
    {
    var body = this.datos;
    if( body.shipping == undefined ) body.shipping = {};

    if( typeof(metodo)=="string" ) body.shipping.method = metodo;

    body.shipping.address = {};
    body.shipping.address.name = {};

    if( typeof( nombre   )=="string" ) body.shipping.address.name.full_name = nombre;
    if( typeof( apellido )=="string" ) body.shipping.address.name.surname   = apellido;

    if( typeof( codPostal )=="string" ) body.shipping.address.postal_code  = codPostal;
    if( typeof( codPais   )=="string" ) body.shipping.address.country_code = codPais;
    }

  // Pone los datos para la primera dirección de envio (opcional)
  EnvioDirección1( direccion, area )
    {
    if( typeof( direccion )=="string" ) body.shipping.address.address_line_1 = direccion;
    if( typeof( area      )=="string" ) body.shipping.address.admin_area_1   = area;
    }

  // Pone los datos para la primera dirección de envio (opcional)
  EnvioDirección1( direccion, area )
    {
    if( typeof( direccion )=="string" ) body.shipping.address.address_line_2 = direccion;
    if( typeof( area      )=="string" ) body.shipping.address.admin_area_2   = area;
    }

  // Retorna un objeto javascript con todos los datos de la oreden
  GetDatos()
    {
    var obj = {};
    obj.purchase_units = [];

    obj.purchase_units.push( this.datos );
    return obj;
    }

  // Retorna un objeto JSON con todos los datos de la oreden
  GetJson()
    {
    return JSON.stringify( this.GetDatos() );
    }

  // Función interna, trata un error producido con el mensaje 'msg'
  _Error( msg )
    {
    console.log( "** ERROR *** : " + msg );
    return null;
    }

  // Función interna, crea un objeto que representa un valor a pagar
  _PayVal( val, moneda )
    {
    var obj = {};

    var numVal = Number(val);
    if( !numVal>0 ) return this._Error("El valor proporcionado no es un número válido"); 
    obj.value = String( numVal );

    if( typeof( moneda ) !="string" ) return obj;               // Si no es una cadena ignora a la moneda
      
    moneda = moneda.toUpperCase();
    switch( moneda )
      {
      case "AUD": break;     // Australian dollar
      case "BRL": break;     // Brazilian real	      
      case "CAD": break;     // Canadian dollar	      
      case "CZK": break;     // Czech koruna	        
      case "DKK": break;     // Danish krone	        
      case "EUR": break;     // Euro	                
      case "HKD": break;     // Hong Kong dollar	    
      case "HUF": break;     // Hungarian forint	    
      case "ILS": break;     // Israeli new shekel	  
      case "JPY": break;     // Japanese yen	        
      case "MYR": break;     // Malaysian ringgit	    
      case "MXN": break;     // Mexican peso	        
      case "TWD": break;     // New Taiwan dollar	    
      case "NZD": break;     // New Zealand dollar	  
      case "NOK": break;     // Norwegian krone	      
      case "PHP": break;     // Philippine peso	      
      case "PLN": break;     // Polish złoty	        
      case "GBP": break;     // Pound sterling	      
      case "RUB": break;     // Russian ruble	        
      case "SGD": break;     // Singapore dollar	    
      case "SEK": break;     // Swedish krona	        
      case "CHF": break;     // Swiss franc	          
      case "THB": break;     // Thai baht	            
      case "USD": break;     // United States dollar	

      case "$"    : moneda ="USD"; break;     // United States dollar	
      case "DOLAR": moneda ="USD"; break;     // United States dollar	

      case "€"    : moneda ="EUR"; break;     // Euro
      case "EURO" : moneda ="EUR"; break;     // Euro
      case "EUROS": moneda ="EUR"; break;     // Euro

      default:
          {
          this._Error("La moneda '"+ moneda +"' suministrada no es soportada por Paypal");       // Ignora la moneda, pero acepta el valor (la moneda es opcional )
          return obj;
          }
      }

    obj.currency_code = moneda;

    return obj;
    }

  // Función interna, obtiene el objeto desglose dentro de los datos de la orden
  _GetDesglose()
    {
    var body = this.datos;
    if( body.amount.breakdown == undefined ) body.amount.breakdown = {};

    return body.amount.breakdown;
    }

  // Verifica el valor del Item, si en un entero lo interpreta como un indice, si hay error retorna null
  _GetItem( item )
    {
    var items = this.datos.items;
    if( items == undefined ) return this._Error("No se ha adicionado ningún item a la orden");

    if( typeof(item) == "number" )
      {
      var idx = Math.trunc(item);    
      if( idx<0 || idx >= items.length )
        return this._Error( "Indice del item fuera de rango" );

      return items[idx]; 
      }

    if( typeof(item) != "object" )
      return this._Error("El item suministrado es incorrecto");

    return item;
    }

}


