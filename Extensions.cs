using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Piezas2
  {
  public static class Extensions
    {
    public static void AddSwagger( this IServiceCollection services )
      {
      services.AddSwaggerGen( c =>
        {
        c.SwaggerDoc( "v1", new OpenApiInfo
          {
          Version = "v1",
          Title = "API para recambios de coches",
          Description = "API Resful en ASP.NET para manejar los datos de los recambios",
//          TermsOfService = new System.Uri( "https://www.talkingdotnet.com" ),
//          Contact = new OpenApiContact() { Name = "Talking Dotnet", Email = "contact@talkingdotnet.com" }
          } );

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine( AppContext.BaseDirectory, xmlFile );
        c.IncludeXmlComments(xmlPath);
        });
      }

    public static void UseCustomSwagger( this IApplicationBuilder app )
      {
      app.UseSwagger();
      app.UseSwaggerUI( c =>
      {
        c.SwaggerEndpoint( "/swagger/v1/swagger.json", "API v1" );
      } );
      }
    }
  }
