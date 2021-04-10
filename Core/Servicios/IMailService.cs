using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piezas2.Core.Servicios
  {
  public interface IMailService
    {
    bool Send( string ToEmail, string Subject, string Body );
    }
  }
