using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaWebApi.Models;

namespace PracticaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class libroController : Controller
    {
        private readonly bibliotecaContext _bibliotecaContexto;
        public libroController(bibliotecaContext bibliotecaContexto)
        {
            _bibliotecaContexto = bibliotecaContexto;
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<libro> listadoLibros = (from e in _bibliotecaContexto.libro select e).ToList();

            if (listadoLibros.Count == 0)
            {
                return NotFound();
            }

            return Ok(listadoLibros);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var libroConAutor = (from l in _bibliotecaContexto.libro
                                 join a in _bibliotecaContexto.autor
                                    on l.id_autor equals a.id_autor
                                where l.id_libro == id
                                 select new
                                 {
                                     l.titulo,
                                     l.anioPublicacion,
                                     l.resumen,
                                     a.nombre
                                 }).ToList();

            if (libroConAutor == null)
            {
                return NotFound();
            }
            return Ok(libroConAutor);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult AgregarLibro([FromBody] libro _libro) {
            try
            {
                _bibliotecaContexto.libro.Add(_libro);
                _bibliotecaContexto.SaveChanges();
                return Ok(_libro);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Error de base de datos: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error general: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("actualizar/{id}")]
        public IActionResult actualizarLibro(int id, [FromBody] libro actualizarLibro) {
            libro? libroActual = (from e in _bibliotecaContexto.libro where e.id_libro == id select e).FirstOrDefault();
            if (libroActual == null)
            {
                return NotFound();
            }

            libroActual.titulo = actualizarLibro.titulo;
            libroActual.anioPublicacion = actualizarLibro.anioPublicacion.Date;
            libroActual.id_autor = actualizarLibro.id_autor;
            libroActual.id_categoria = actualizarLibro.id_categoria;
            libroActual.resumen = actualizarLibro.resumen;

            _bibliotecaContexto.Entry(libroActual).State = EntityState.Modified;
            _bibliotecaContexto.SaveChanges();

            return Ok(actualizarLibro);
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarLibro(int id)
        {
            libro? libro = (from l in _bibliotecaContexto.libro where l.id_libro == id select l).FirstOrDefault();

            if (libro == null) { return NotFound(); }

            _bibliotecaContexto.libro.Attach(libro);
            _bibliotecaContexto.libro.Remove(libro);
            _bibliotecaContexto.SaveChanges();

            return Ok(libro);
        }

        //Libros después del año 2000
        [HttpGet]
        [Route("GetAnio")]
        public IActionResult librosConAnio(int anio) { 
            List<libro> librosPublicadosAnio = (from l in _bibliotecaContexto.libro 
                                                where l.anioPublicacion.Year > anio
                                                select l).ToList();
            if (librosPublicadosAnio.Count == 0) { 
                return NotFound();
            }

            return Ok(librosPublicadosAnio);
        }

        //Libros de un solo autor
        [HttpGet]
        [Route("GetLibroAutor")]
        public IActionResult obtenerLibrosXAutor(int id) { 
            var listaLibrosXAutor = (from l in _bibliotecaContexto.libro
                                             join a in _bibliotecaContexto.autor
                                             on l.id_autor equals a.id_autor
                                             where a.id_autor == id
                                             group a by a.nombre into grupo
                                             select new
                                             {
                                                 nombre = grupo.Key,
                                                 cantidadLibros = grupo.Count()
                                             }).ToList();

            if (!listaLibrosXAutor.Any()) { return NotFound(); }
            return Ok(listaLibrosXAutor);
        }
        //Paginacion en libros
        [HttpGet]
        [Route("GetXPaginacion")]
        public IActionResult obtenerLibrosPaginacion()
        {
            var listadoLibros = (from l in _bibliotecaContexto.libro
                                 select new
                                 {
                                     l.titulo,
                                     l.anioPublicacion,
                                     l.resumen                  //Tiene que haber mas de 10 de cada tabla
                                 }).Skip(10).Take(10).ToList(); //si se usa join si no solo más de 10

            if (listadoLibros.Count == 0)
            {
                return NotFound();
            }

            return Ok(listadoLibros);
        }

        //Busca de libros por titulo
        [HttpGet]
        [Route("busquedaXTitulo")]
        public IActionResult busquedaPoTitulo(string titulo)
        {
            List<libro> listadoPorTitulos = (from l in _bibliotecaContexto.libro
                                             where l.titulo == titulo
                                             select l).ToList();

            if(listadoPorTitulos.Count == 0)
            {
                return NotFound();
            }
            return Ok(listadoPorTitulos);
        }
    }
}
