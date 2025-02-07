using Grpc.Core;

namespace ThisIsAnAttack.Remoting.Grpc;

public interface IGrpcClientFactory
{
    TClient CreateClient<TClient>(
        CancellationToken cancellationToken)
        where TClient : ClientBase<TClient>;
}
