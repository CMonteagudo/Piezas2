using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Piezas2.Core.Model;

namespace Piezas2
  {
  [Route( "api/[controller]" )]
  [ApiController]
  public class MarcasController : ControllerBase
    {
    private readonly DbPiezasContext _context;

    public MarcasController( DbPiezasContext context )
      {
      
      _context = context;
      }

    // GET: api/Marcas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Marca>>> GetMarcas()
      {
      return await _context.Marcas.ToListAsync();
      }

    // GET: api/Marcas/5
    [HttpGet( "{id}" )]
    public async Task<ActionResult<Marca>> GetMarca( int id )
      {
      var marca = await _context.Marcas.FindAsync(id);

      if( marca == null )
        {
        return NotFound();
        }

      return marca;
      }

    [HttpGet( "/api/MarcaId2/{name}" )]
    public ActionResult<int> MarcaId2( string Name )
      {
      var marca = _context.Marcas.FirstOrDefault( x=>x.Nombre==Name );
      return marca?.Id ?? -1;
      }


    [HttpGet( "/api/MarcaId/{name}" )]
    public ActionResult<int> MarcaId( string Name, [FromServices] DbPiezasContext DbCtx )
      {
      var marca = DbCtx.Marcas.FromSqlRaw("SELECT * FROM Marca WHERE Nombre={0}", Name).FirstOrDefault();
      return marca?.Id ?? -1;
      }

    // PUT: api/Marcas/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPut( "{id}" )]
    public async Task<IActionResult> PutMarca( int id, Marca marca )
      {
      if( id != marca.Id )
        {
        return BadRequest();
        }

      _context.Entry( marca ).State = EntityState.Modified;

      try
        {
        await _context.SaveChangesAsync();
        }
      catch( DbUpdateConcurrencyException )
        {
        if( !MarcaExists( id ) )
          {
          return NotFound();
          }
        else
          {
          throw;
          }
        }

      return NoContent();
      }

    // POST: api/Marcas
    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
    [HttpPost]
    public async Task<ActionResult<Marca>> PostMarca( Marca marca )
      {
      _context.Marcas.Add( marca );
      await _context.SaveChangesAsync();

      return CreatedAtAction( "GetMarca", new { id = marca.Id }, marca );
      }

    // DELETE: api/Marcas/5
    [HttpDelete( "{id}" )]
    public async Task<ActionResult<Marca>> DeleteMarca( int id )
      {
      var marca = await _context.Marcas.FindAsync(id);
      if( marca == null )
        {
        return NotFound();
        }

      _context.Marcas.Remove( marca );
      await _context.SaveChangesAsync();

      return marca;
      }

    private bool MarcaExists( int id )
      {
      return _context.Marcas.Any( e => e.Id == id );
      }
    }
  }
