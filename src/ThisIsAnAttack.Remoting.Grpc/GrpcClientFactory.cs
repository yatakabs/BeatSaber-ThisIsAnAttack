using Grpc.Core;

namespace ThisIsAnAttack.Remoting.Grpc;

/// <summary>
/// Factory class for creating gRPC clients.
/// </summary>
public class GrpcClientFactory : IGrpcClientFactory
{
    private Channel Channel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcClientFactory"/> class.
    /// </summary>
    /// <param name="channel">The gRPC channel to use for client connections.</param>
    public GrpcClientFactory(Channel channel)
    {
        this.Channel = channel;
    }

    /// <summary>
    /// Creates a gRPC client of the specified type.
    /// </summary>
    /// <typeparam name="TClient">The type of gRPC client to create.</typeparam>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A new instance of the specified gRPC client type.</returns>
    /// <exception cref="Exception">Thrown when the client creation fails.</exception>
    public TClient CreateClient<TClient>(
        CancellationToken cancellationToken)
        where TClient : ClientBase<TClient>
    {
        return (TClient)Activator.CreateInstance(
            typeof(TClient),
            this.Channel);
    }
}
