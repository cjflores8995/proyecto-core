using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta : IRequest
        {
            public Guid CursoId { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                _context = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var instructoresDB = _context.CursoInstructor.Where(x => x.CursoId == request.CursoId).ToList();

                foreach (var instructorEliminar in instructoresDB)
                {
                    _context.CursoInstructor.Remove(instructorEliminar);
                }

                /* Eliminar comentarios */
                var comentariosDB = _context.Comentario.Where(x => x.CursoId == request.CursoId);
                foreach(var comentario in comentariosDB)
                {
                    _context.Comentario.Remove(comentario);
                }

                /* Eliminar precio */
                var precioDB = _context.Precio.Where(x => x.CursoId == request.CursoId).FirstOrDefault();
                if(precioDB != null)
                {
                    _context.Precio.Remove(precioDB);
                }


                var curso = await _context.Curso.FindAsync(request.CursoId);

                if (curso == null)
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje = "No se encontro el curso"});

                _context.Remove(curso);

                var resultado = await _context.SaveChangesAsync();

                if (resultado > 0)
                {
                    return Unit.Value;
                }

                throw new System.Exception("No se pudo eliminar el curso.");
            }
        }
    }
}