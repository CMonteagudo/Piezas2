using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piezas2.Models
  {
  public class PaypalSetting
    {
    public const string SecName = "Paypal";
    public string ApiAppName { get; set; }
    public string Account { get; set; }
    public string ClientID { get; set; }
    public string Secret { get; set; }
    }


  }
