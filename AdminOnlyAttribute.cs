using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piezas2
  {
  public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
    {
    public void OnAuthorization( AuthorizationFilterContext context )
      {
      var ssn = context.HttpContext.Session;

      if( ssn?.GetInt32( "Admin" ) != 1 )
        {
        context.Result = new StatusCodeResult( StatusCodes.Status404NotFound );
        }
      }
    }
  }
