using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticaWebApi.Models;

namespace PracticaWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class autorController : Controller
    {
        private readonly bibliotecaContext _bibliotecaContexto;

        public autorController(bibliotecaContext bibliotecaContexto)
        {
            _bibliotecaContexto = bibliotecaContexto;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult Get()
        {
            List<autor> listadoAutores = (from e in _bibliotecaContexto.autor select e).ToList();

            if(listadoAutores.Count == 0)
            {
                return NotFound();
            }

            return Ok(listadoAutores);
        }

        [HttpGet]
        [Route("GetById/{id}")]
        public IActionResult Get(int id)
        {
            var autorConLibro = (from a in _bibliotecaContexto.autor
                                 join l in _bibliotecaContexto.libro
                                    on a.id_autor equals l.id_autor
                                where a.id_autor == id
                                 select new
                                 {
                                     a.nombre,
                                     a.nacionalidad,
                                     l.titulo
                                 }).ToList();

            if (autorConLibro == null)
            {
                return NotFound();
            }
            return Ok(autorConLibro);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult GuardarAutor([FromBody] autor _autor)
        {
            try
            {
                _bibliotecaContexto.autor.Add(_autor);
                _bibliotecaContexto.SaveChanges();
                return Ok(_autor);
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
        public IActionResult ActualizarAutor(int id, [FromBody] autor autorModificar)
        {
            autor? autorActual = (from e in _bibliotecaContexto.autor where e.id_autor == id select e).FirstOrDefault();
            if (autorActual == null)
            {
                return NotFound();
            }

            autorActual.nombre = autorModificar.nombre;
            autorActual.nacionalidad = autorModificar.nacionalidad;

            _bibliotecaContexto.Entry(autorActual).State = EntityState.Modified;
            _bibliotecaContexto.SaveChanges();

            return Ok(autorModificar);
        }

        [HttpDelete]
        [Route("eliminar/{id}")]
        public IActionResult EliminarAutor(int id)
        {
            autor? autor = (from e in _bibliotecaContexto.autor where e.id_autor == id select e).FirstOrDefault();

            if (autor == null) { return NotFound(); }

            _bibliotecaContexto.autor.Attach(autor);
            _bibliotecaContexto.autor.Remove(autor);
            _bibliotecaContexto.SaveChanges();

            return Ok(autor);
        }
    }
}
