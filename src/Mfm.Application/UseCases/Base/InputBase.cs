using MediatR;

namespace Mfm.Application.UseCases.Base;

public abstract class InputBase<TOutput> : IRequest<TOutput>
    where TOutput : OutputBase
{
}
