using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace Piezas2.Models
  {
  public class ErrorViewModel
    {
    public string LocalIp { get; private set; }
    public string RemoteIp { get; private set; }
    public string ErrorCode { get; private set; }

    public ErrorViewModel( string errorCode, HttpContext httpContext )
      {
      LocalIp  = httpContext.Connection.LocalIpAddress.ToString();
      RemoteIp = httpContext.Connection.LocalIpAddress.ToString();

      ErrorCode = errorCode;
      }
    }
  }
