using System;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class ConsultaId
    {
        public class Ejecuta: IRequest<InstructorModel>
        {
            public Guid InstructorId {get; set;}
        }

        public class Manejador : IRequestHandler<Ejecuta, InstructorModel>
        {
            private readonly IInstructor _instructorRepositorio;
            public Manejador(IInstructor instructorRepositorio)
            {
                _instructorRepositorio = instructorRepositorio;
            }

            public async Task<InstructorModel> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var instructor = await _instructorRepositorio.ObtenerPorId(request.InstructorId);

                if(instructor == null)
                    throw new ManejadorExcepcion(System.Net.HttpStatusCode.NotFound, new {mensaje = "No se encontro el instructor"});
            
                return instructor;
            }
        }
    }
}