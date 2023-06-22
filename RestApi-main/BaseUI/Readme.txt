
Lineamientos

1. Instalar Syncfusion y bootstrap (Version 19.0.2.55)
2. Agregarlo en Imports
3. Agregar licencia de syncfusion en Startup
4. Agregar las clases common
5. Configurar servicios y acceso en startup
6. Setear la cultura en Startup para evitar problemas de conversion con horas y monedas
7. Agregar extension AddInfraestructureServices para mejor organización de los servicios Blazor
8. Agregar ApplicationDbContext para poder scafoldear Identity
9. Scafoldear Identity

10.Agregar en Host los links de cdn o paquetes descargados de css js necesarios.

Recaptcha
11.@*
    1° Agregar en el startup services.Configure<reCAPTCHAVerificationOptions>(Configuration.GetSection("reCAPTCHA"));
    2° Agregar en Appsettings la key secreta
    3° Agregar manualmente archivo script en carpeta script en wwwroot
    4° Agregar referencia al archivo script desde el head del host
    5° Añadir componente en pagina, instanciar y agregar metodos para suceso o falla
*@

Authorize
12. Scafoldear, agregar referencias. Y crear el componente de chequeo LoginRedirect. 
13. IMPORTANTE agregar en Imports @attribute [Authorize] asi lo requiere en todas las paginas excepto las que yo ponga AllowAnnonimous