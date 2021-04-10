//using MailKit.Net.Smtp;
//using MailKit.Security;
using Microsoft.Extensions.Options;
//using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Piezas2.Core.Servicios
  {
  //=========================================================================================================================================
  /// <summary> Servicio para enviar correos desde cualquier lugar de la aplicación </summary>
  public class MailService : IMailService
    {
    private readonly MailSettings _mailSettings;
    public MailService( IOptions<MailSettings> mailSettings )
      {
      _mailSettings = mailSettings.Value;
      }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Usa los componentes MailKit y MimeKit que dan más posibilidades pero es más dificil probar localmente </summary>
    //public bool Send( string ToEmail, string Subject, string Body )
    //  {
    //  var builder = new BodyBuilder();
    //  builder.HtmlBody = Body;

    //  var email = new MimeMessage()
    //    {
    //    Sender = MailboxAddress.Parse( _mailSettings.Mail ),
    //    Body = builder.ToMessageBody(),
    //    Subject = Subject
    //    };

    //  email.To.Add( MailboxAddress.Parse( ToEmail ) );

    //  using( var smtp = new MailKit.Net.Smtp.SmtpClient() )
    //    { 
    //    smtp.Connect( _mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls );
    //    smtp.Authenticate( _mailSettings.Mail, _mailSettings.Password );

    //    smtp.Send( email );
    //    smtp.Disconnect( true );
    //    }

    //  return true;
    //  }

    //-----------------------------------------------------------------------------------------------------------------------------------------
    /// <summary> Usa las clases de .Net, es para ecenarios más sencillo y permite probar fácilmente </summary>
    public bool Send( string ToEmail, string Subject, string Body )
      {
      try
        {
        var fromAddress = new MailAddress( _mailSettings.Mail, _mailSettings.DisplayName );
        var toAddress   = new MailAddress( ToEmail   );

        var mailMsg = new MailMessage()
          {
          From       = fromAddress,
          Body       = Body,
          IsBodyHtml = true,
          Subject    = Subject
          };

        mailMsg.To.Add( toAddress );

        var smtp = new System.Net.Mail.SmtpClient();

        if( string.IsNullOrWhiteSpace( _mailSettings.PickupDirectory ) )
          {
          smtp.Host        = _mailSettings.Host;
          smtp.Port        = _mailSettings.Port;
          smtp.Credentials = new NetworkCredential( _mailSettings.Mail, _mailSettings.Password );
          smtp.EnableSsl   = true;
          }
        else
          {
          smtp.DeliveryMethod          = SmtpDeliveryMethod.SpecifiedPickupDirectory;
          smtp.PickupDirectoryLocation = _mailSettings.PickupDirectory;
          };

        smtp.Send( mailMsg );

        return true;
        }
      catch( Exception ) { return false; }
      }

    }
  }
