using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Piezas2.Core.Model;
using Piezas2.Core.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Piezas2
  {
  //=======================================================================================================================================
  /// <summary> Maneja todas las acciones relacionadas con los usuarios del sistema </summary>
  public class Usuarios
    {
    readonly DbPiezasContext DbCtx;            // Conecto para acceder a la base de datos
    readonly IMailService    Mail;
    readonly ISession        Session;

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Construye el objeto con informacion se la conexion </summary>
    public Usuarios( HttpContext HttpCtx )
      {
      DbCtx = (DbPiezasContext)HttpCtx.RequestServices.GetService( typeof( DbPiezasContext ) );         // Obtiene contexto a la BD
      Mail  = (IMailService)HttpCtx.RequestServices.GetService( typeof( IMailService ) );               // Obtiene el servicio para enviar correos

      Session = HttpCtx.Session;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra si existe un usuario con el nombre y la contraseña dado </summary>
    internal Usuario LogIn( string user, string pass )
      {
      var logUser = DbCtx.Usuarios.Where( u=> u.Nombre==user && u.PassWord==pass ).FirstOrDefault();
      if( logUser == null ) return new Usuario { Id = 0 };

      if( logUser.Confirmado != 0 )  SetLogin( logUser );                 // Si ya esta confirmado, se loguea
      else                           SendCode( logUser.Id );              // Envia un código de confirmación al correo del usuario

      return logUser;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Si hay un usuario registrado lo desregitra </summary>
    internal void LogOut()
      {
      Session.Remove( "UserId" );
      Session.Remove( "Admin" );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Envia un correo con un código de verificación el usuario con identificador 'idUser' </summary>
    internal void SendCode( int idUser )
      {
      var user     = Find( idUser );
      string sCode = GetVerifyCode( user.Correo );

      var Asunto = $"Código le verificación {sCode}";
      var Body   = $"Para terminar el proceso de regístro del usuario {user.Nombre} ponga el código {sCode} cuando sea solicitado"; 

      Mail.Send( user.Correo, Asunto, Body );
      }
    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Envia la contraseñas al correo del usuario </summary>
    internal void SendPassword( string Name )
      {
      var Users = DbCtx.Usuarios.Where( u=> u.Nombre==Name );
      if( Users.Count()==0  ) throw new Exception( "UserNoExist" ); ;

      foreach( var user in Users )
        {
        var Asunto = $"Recordatorio de contraseñas";
        var Body   = $"Datos del usuario<br/><br/> Nombre: <b>{user.Nombre}</b><br/> Contraseñas: <b>{user.PassWord}</b>";

        Mail.Send( user.Correo, Asunto, Body );
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra el usuario con el Id dado y retorna sus datos, sino un objeto usuario vacio </summary>
    internal Usuario VeryfyCode( int idUser, string sCode )
      {
      var user = Find( idUser );
      var Code = GetVerifyCode( user.Correo );

      if( Code == sCode ) SetLogin( user );

      return user;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Encuentra el usuario con el Id dado y retorna sus datos, sino un objeto usuario vacio </summary>
    public Usuario Find( int id )
      {
      var user = DbCtx.Usuarios.Find( id );
      if( user == null ) user = new Usuario { Id = 0 };

      return user;
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Obtiene un lista de todos los usuarios, donde se obtienen todos los datos para cada usuario </summary>
    public List<Usuario> ListUsarios()
      {
      return DbCtx.Usuarios.ToList();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Cambia los datos de un usuario existente o inserta uno nuevo </summary>
    public int Change( Usuario user )
      {
      if( user.Id == 0 )
        {
        chkNameEmail( user );                                 // Chequea el nombre del usuario o el correo no se repitan

        user.Confirmado = 0;                                  // Fuerza verificar el correo 
        var newItem = DbCtx.Usuarios.Add( user );

        DbCtx.SaveChanges();

        SendCode( newItem.Entity.Id );                        // Envia un código de confirmación al correo del usuario
        return newItem.Entity.Id;
        }
      else
        {
        chkNameEmail( user );                                 // Chequea el nombre del usuario o el correo no se repitan

        var edtUser =  DbCtx.Usuarios.Find( user.Id );        // Busca al usuario en la
        if( edtUser == null ) return 0;

        var chgMail = edtUser.Correo != user.Correo;          // Si se cambia el correo
        if( chgMail ) edtUser.Confirmado = 0;                 // Obliga volver a confirmar el correo

        edtUser.Nombre     = user.Nombre;
        edtUser.Correo     = user.Correo;
        edtUser.Telefonos  = user.Telefonos;
        edtUser.PassWord   = user.PassWord;

        DbCtx.SaveChanges();

        if( chgMail ) SendCode( edtUser.Id );                 // Envia un código de confirmación al correo del usuario
        return user.Id;
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Borra el usuario con identificador 'id' </summary>
    public void Delete( int id )
      {
      var user = DbCtx.Usuarios.Find( id );
      if( user == null ) return;

      DbCtx.Usuarios.Remove( user );

      DbCtx.SaveChanges();
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Calcula un código de 6 digitos basado en el correo </summary>
    private string GetVerifyCode( string correo )
      {
      if( string.IsNullOrEmpty( correo ) ) return "000000";

      UInt32 code = 0;

      for( UInt32 j=1; ; ++j )                                // Repite el proceso hasta que el checksum sea de 6 digitos
        {
        for( int i=0; i < correo.Length; i++ )
          code += (UInt32)((i+1)*(j*1539)) + Convert.ToUInt32( correo[i] );

        if( code > 10_00_00 )                                 // Si es de 6 digitos o más
          {
          while( code > 99_99_99 ) code /= 10;                // Si son más de 6 dígitos, quita el último dígito

          return code.ToString();                             // Convierte el código a cadena
          }
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Chequea exite un usuario con el mismo nombre o correo, se ser cierto emite una excepción </summary>
    private void chkNameEmail( Usuario user )
      {
      if( user.Id == 0 )
        { 
        if( DbCtx.Usuarios.Where( u=>u.Nombre==user.Nombre).FirstOrDefault() != null )
          throw new Exception( "UserExist" );

        if( DbCtx.Usuarios.Where( u => u.Correo == user.Correo ).FirstOrDefault() != null )
          throw new Exception( "MailExist" );
        }
      else
        {
        if( DbCtx.Usuarios.Where( u => u.Nombre == user.Nombre && u.Id != user.Id ).FirstOrDefault() != null )
          throw new Exception( "UserExist" );

        if( DbCtx.Usuarios.Where( u => u.Correo == user.Correo && u.Id != user.Id ).FirstOrDefault() != null )
          throw new Exception( "MailExist" );
        }
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Establece al usuario 'user' como loguedo en el servidor </summary>
    private void SetLogin( Usuario user )
      {
      Session.SetInt32( "UserId", user.Id );                  // Inicia la sección para el usuario
      Session.SetInt32( "Admin", user.Admin );                // Pone la sección para el modo administrador

      user.NLogin += 1;                                      // Incrementa la cantidad de veces que el usuario se registra
      user.Confirmado = 1;
      DbCtx.SaveChanges();
      }



    }
  }