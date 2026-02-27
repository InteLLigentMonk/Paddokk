using MediatR;

namespace Paddokk.Core.Interfaces;

public interface IQuery<TResponse> : IRequest<TResponse> { }
