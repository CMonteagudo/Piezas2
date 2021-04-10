using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Piezas2.Core.Model;
using Piezas2.Core.Servicios;

namespace Piezas2
  {
  //=======================================================================================================================================
  // Se llama al inicializar el sitio y su objetivo es establecer todas las coficuraciones necesarias para su funcionamiento
  public class Startup
    {
    /// <summary> Cadena que se usa para la conexion a la base de datos </summary>    
    public static string conneStr;

    public IConfiguration Configuration { get; }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Se llama al crear la aplicación y se le pasan todos los paramentros de configuración de la aplicación</summary>
    public Startup( IConfiguration configuration )
      {
      Configuration = configuration;

      conneStr = Configuration.GetConnectionString( "DefaultConnection" )
                                                  .Replace( "[ProjetDirectory]", Directory.GetCurrentDirectory() );
      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>Se llama para establecer todos los servicios que se van a utilizar</summary>
    public void ConfigureServices( IServiceCollection services )
      {
      services.AddDistributedMemoryCache();
      services.AddSession();

      services.Configure<MailSettings>( Configuration.GetSection( "MailSettings" ) );
      services.AddTransient<IMailService, MailService>();

      services.AddControllersWithViews();
      services.AddEntityFrameworkSqlServer().AddDbContext<DbPiezasContext>( 
        ( serviceProvider, options ) => options.UseSqlServer( conneStr ).UseInternalServiceProvider( serviceProvider ) );

      }

    //---------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>El llamado para establecer la configuración para las soliciudes recibidas</summary>
    public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
      {
      if( env.IsDevelopment() )
        {
        //app.UseStatusCodePages( "text/plain", "HTTP Error - Status Code:{0} " );
        //app.UseStatusCodePagesWithRedirects( "/error/{0}" );
        app.UseDeveloperExceptionPage();
        }
      else
        {
        //app.UseStatusCodePagesWithRedirects( "/Home/error/{0}" );
        app.UseExceptionHandler( "/Error" );
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
        }

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthorization();

      app.UseSession();
      app.UseEndpoints( endpoints =>
       {
       endpoints.MapControllers();

         //endpoints.MapControllerRoute(
         //         name: "default",
         //         pattern: "{controller=Home}/{action=Index}/{id?}" );
       } );
      }
    }

  //===================================================================================================================================================
  // Esta clase es para que las herramientas de linea de comando descubran sobre cual base de datos deben trabajar
  public class DbPiezasContextFactory : IDesignTimeDbContextFactory<DbPiezasContext>
    {
    public DbPiezasContext CreateDbContext( string[] args )
      {
      var optionsBuilder = new DbContextOptionsBuilder<DbPiezasContext>();
      optionsBuilder.UseSqlServer( Startup.conneStr );

      return new DbPiezasContext( optionsBuilder.Options );
      }
    }
  }
