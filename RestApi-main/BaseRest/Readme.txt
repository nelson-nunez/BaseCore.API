INICIANDO PROYECTO

1- Crear proyecto web api con .net5 (definir nombre corto para no tener problemas de url. Ver soluciones url largas)
2- Configurar compatibilidad docker al crear el proyecto.
   MODELO:	
3- Crear biblioteca de clases.
4- Referenciar la biblioteca de clases en el proyecto ppal o de inicio
5- Crear el modelo y los dtos planos
6- Instalar sqlserver y mssms
7- Configurar el servidor sqlserver y copiar nombre
   https://www.exceleinfo.com/como-instalar-y-configurar-sql-server-paso-a-paso/
8- Crear string de conexion en el pproyecto ppal o de inicio. en appsettings.json (es mas seguro y ordenado q en el mismo modelcontext)
9- Crear DbModelContext en la biblioteca de clases
10- Se define las propiedades de string e inicializadoras en OnConfiguring. 
	Tambien definir el tipo de carga si es perezosa o din.
	Validar que se este tomando el string de conexion con una excepcion
11- Definir en onmodelcreating las clases y el tipo de borrado
12- Crear un seed para algunos objetos iniciales requeridos. 
13- Crear migraciones (Referenciar el proyecto en los comandos si son varios)
	https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/projects?tabs=vs
	Add-Migration NewMigration -Project BaseRest.Core.Model
	Update-Database -Project BaseRest.Core.Model

	CAPA DATA ACCESS (designpatter):
14 - Crear Capa UnitOf work (es un patron de diseño que abstrae el acceso a los datos de la la logica den)
15- Inyectar el dbcontext y contextaccesor en unitofwortk.
16- Crear repositorio generico con las ppales acciones.
17- crear un repositorio para la clase qque invoque el repogenerico. Se pueden agregar acciones no standar en este.
	
	CAPA BUSINESS:
18- Crear un busines de la clase a trabajar con las consultas basicas. invocando las acciones del unitofwork.
	Los datos tienen que trabajarse en base al modelo original, los dto se implementan recien del lado del controller.
	
	AUTOMAPPER:
19- Instalar paquete AutoMapper.Extensions.Microsoft.DependencyInjection
20- Añadir AutoMapperProfile y definir en el mismo las clases que se van a mapear.
21- Agregar en el startup de la api referencias los servicios agregados
    services.AddScoped<CustomerRepository, CustomerRepository>();
    services.AddScoped<UnitOfWork, UnitOfWork>();
    services.AddScoped<CustomerBusiness, CustomerBusiness>();

23- Instalar Microsoft.AspNetCore.Authentication.JwtBearer
24- Crear Controller y busines de autenticacion.
25- Crear repo y un seed de un usuario basico.
26- Configurar en el startup el autenticacion y token y dsps el swagger para que use bearer token.
27- Configurar en services para que lea los token del swagger y en startup ->Configure poner app.UseAuthentication(); app.UseAuthorization();
	
	Serilog (https://www.codeproject.com/Articles/5344667/Logging-with-Serilog-in-ASP-NET-Core-Web-API)
28- Se instala los paquetes Serilog.Sinks.MSSqlServer y Serilog.AspNetCore (este tiene mas compatibilidad con las versiones mas altas de .net)
29- En el Program.cs se agrega la referencia y en la misma de donde sacar la configuración
30- Agregar la escritura del log en la logica o usar su vinculación automatica con las excepciones y leerlo desde la db directamente




consultar si el delete del generic repository esta haciendo un delete hard o soft!!!!!!!!!!!!!!!!!!!
porqur usamos el action result??? porq no directo la clase resultante nomas???

PAQUETES REQUERIDOS
Microsoft.VisualStudio.Azure.Containers.Tools.Targets (PREINSTALADO)
Microsoft.VisualStudio.Web.CodeGeneration.Design (PREINSTALADO)
Swashbuckle.AspNetCore (PREINSTALADO)
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.Extensions.Configuration
Microsoft.Extensions.Configuration.Json
Newtonsoft.Json
AutoMapper.Extensions.Microsoft.DependencyInjection
Microsoft.AspNetCore.Http.Abstractions
Microsoft.EntityFrameworkCore.Proxies