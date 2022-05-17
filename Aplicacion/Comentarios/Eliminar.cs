using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Comentarios
{
    public class Eliminar
    {
        public class Ejecuta: IRequest
        {
            public Guid Id {get; set;}
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
                var comentario = _context.Comentario.Where(x => x.ComentarioId == request.Id).FirstOrDefault();

                if(comentario == null)
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {errores = "No se encontro el comentario"});

                _context.Remove(comentario);
                var resultado = await _context.SaveChangesAsync();

                if(resultado > 0){
                    return Unit.Value;
                }

                throw new Exception("No se pudo eliminar el comentario");
            }
        }
    }
}