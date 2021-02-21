using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Piezas2.Core;
using System.Text.Json;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
// Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\01_Proyectos\_Web\Piezas2\DataBase\Piezas.mdf;Integrated Security=True;Connect Timeout=30
// dotnet ef dbcontext scaffold "Server=(LocalDB)\MSSQLLocalDB;Database=D:\01_Proyectos\_Web\Piezas2\Core\DataBase\Piezas.mdf;Trusted_Connection=True" Microsoft.EntityFrameWorkCore.SqlServer -o Core\Models

namespace Piezas2
  {
  [Route( "api/[controller]" )]
  [ApiController]
  public class Prueba2Controller : ControllerBase
    {
    // GET: api/<Prueba2Controller>
    [HttpGet]
    public IEnumerable<string> Get()
      {
      return new string[] { "value1", "value2" };
      }

    // GET api/<Prueba2Controller>/5
    [HttpGet( "{id}" )]
    public string Get( int id )
      {
      return $"value={id}";
      }

    /*
     * C- Coche, M- Modelo, m- Motor, c- Categoria, F- Fabricante
    */
    [HttpGet( "/Coche/{*Datos}" )]
    public ActionResult<ItemsFilters> GetCoche( string datos )
      {
      var Filters = new ItemsFilters( datos, HttpContext );
            
      return Filters;
      }

    // POST api/<Prueba2Controller>
    [HttpPost]
    public void Post( [FromBody] string value )
      {
      }

    // PUT api/<Prueba2Controller>/5
    [HttpPut( "{id}" )]
    public void Put( int id, [FromBody] string value )
      {
      }

    // DELETE api/<Prueba2Controller>/5
    [HttpDelete( "{id}" )]
    public void Delete( int id )
      {
      }
    }
  }
