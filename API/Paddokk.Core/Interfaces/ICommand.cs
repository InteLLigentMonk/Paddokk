using MediatR;

namespace Paddokk.Core.Interfaces;

public interface ICommand<TResponse> : IRequest<TResponse> { }
