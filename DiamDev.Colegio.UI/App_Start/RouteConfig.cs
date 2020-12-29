using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DiamDev.Colegio.UI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

           // routes.MapRoute(
           //    "NotasxGrado",
           //    "Notas/{id}/{seccionId}",
           //    new { controller = "Grado", action = "Notas" }
           //);

           // routes.MapRoute(
           //     "NotaxCurso",                                           
           //     "Nota_x_Curso/{id}/{seccionId}",
           //     new { controller = "Grado", action = "Nota_x_Curso" }
           // );

           // routes.MapRoute(
           //    "AsistenciaxCurso",
           //    "Asistencia_x_Curso/{id}/{seccionId}",
           //    new { controller = "Grado", action = "Asistencia_x_Curso" }
           //);         

           //routes.MapRoute(
           //      "CuadroxAsistencia",
           //      "Cuadro_x_Asistencia/{id}/{seccionId}/{cursoId}/{fecha}",
           //      new { controller = "Grado", action = "Cuadro_x_Asistencia", id = UrlParameter.Optional, seccionId = UrlParameter.Optional, cursoId = UrlParameter.Optional, fecha = UrlParameter.Optional }
           //);

          // routes.MapRoute(
          //      "CuadroxCurso",
          //      "Cuadro_x_Curso/{id}/{seccionId}/{cursoId}",
          //      new { controller = "Grado", action = "Cuadro_x_Curso" }
          // );

          // routes.MapRoute(
          //      "EncargadoCuadroxCurso",
          //      "Cuadro_x_Curso/{id}/{seccionId}/{cursoId}/{alumnoId}",
          //      new { controller = "Encargado", action = "Cuadro_x_Curso" }
          // );

          // routes.MapRoute(
          //      "ReporteEncargadoAsistenciaxCurso",
          //      "Boleta_x_Asistencia/{id}/{seccionId}/{cursoId}/{alumnoId}/{fechaInicial}/{fechaFinal}",
          //      new { controller = "Encargado", action = "Boleta_x_Asistencia" }
          // );

          // routes.MapRoute(
          //      "ReporteMaestroAsistenciaxCurso",
          //      "Boleta_x_Asistencia/{id}/{seccionId}/{cursoId}/{alumnoId}/{fechaInicial}/{fechaFinal}",
          //      new { controller = "Maestro", action = "Boleta_x_Asistencia" }
          // );

          // routes.MapRoute(
          //         "ReporteGradoAsistenciaxCurso",
          //         "Boleta_x_Asistencia/{id}/{seccionId}/{cursoId}/{alumnoId}/{fechaInicial}/{fechaFinal}",
          //         new { controller = "Grado", action = "Boleta_x_Asistencia" }
          // );

          // routes.MapRoute(
          //  "AdministradorNotasxAlumno",
          //  "Nota_x_Alumno/{id}/{seccionId}",
          //  new { controller = "Grado", action = "Nota_x_Alumno" }
          //);

          //routes.MapRoute(
          //      "AdministradorReporteNotaxAlumno",
          //      "Boleta/{id}/{seccionId}/{alumnoId}",
          //      new { controller = "Grado", action = "Boleta" }
          //);

          //routes.MapRoute(
          //  "MaestroNotasxAlumno",
          //  "Nota_x_Alumno/{id}/{seccionId}",
          //  new { controller = "Maestro", action = "Nota_x_Alumno" }
          //);

          //routes.MapRoute(
          //  "MaestroReporteNotaxAlumno",
          //  "Boleta/{id}/{seccionId}/{alumnoId}",
          //  new { controller = "Maestro", action = "Boleta" }
          //);

          //routes.MapRoute(
          //  "EncargadoReporteNotaxAlumno",
          //  "Nota/{id}/{seccionId}/{alumnoId}",
          //   new { controller = "Encargado", action = "Nota" }
          //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                 defaults: new { controller = "Seguridad", action = "Login", id = UrlParameter.Optional }
           );
        }
    }
}
