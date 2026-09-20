using System.Threading.Channels;
using Portfolio.Api.Models;

namespace Portfolio.Api.Services;

public interface IContactRequestQueue
{
    ValueTask EnqueueAsync(QueuedContactRequest request, CancellationToken cancellationToken);
    IAsyncEnumerable<QueuedContactRequest> ReadAllAsync(CancellationToken cancellationToken);
}

public sealed class ContactRequestQueue : IContactRequestQueue
{
    private readonly Channel<QueuedContactRequest> _channel = Channel.CreateBounded<QueuedContactRequest>(
        new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask EnqueueAsync(QueuedContactRequest request, CancellationToken cancellationToken) =>
        _channel.Writer.WriteAsync(request, cancellationToken);

    public IAsyncEnumerable<QueuedContactRequest> ReadAllAsync(CancellationToken cancellationToken) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
