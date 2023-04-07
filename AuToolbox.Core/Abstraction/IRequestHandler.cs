namespace AuToolbox.Core.Abstraction;

public interface IRequestHandler<TResult>
{
    Task<TResult> Send(string address, Stream contentStream);
}