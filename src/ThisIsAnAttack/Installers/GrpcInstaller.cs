using Grpc.Core;
using ThisIsAnAttack.Configuration;
using ThisIsAnAttack.Logging;
using ThisIsAnAttack.Remoting.Grpc;
using Zenject;

namespace ThisIsAnAttack.Installers;

/// <summary>
/// Installer for setting up gRPC services and dependencies.
/// </summary>
public class GrpcInstaller : Installer<GrpcInstaller>
{
    /// <summary>
    /// Gets the default server URL.
    /// </summary>
    public static string DefaultServerUrl { get; } = "http://14.13.64.130:56078";
    //public static string DefaultServerUrl { get; } = "http://localhost:5029";

    private IPluginLogger Logger { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrpcInstaller"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public GrpcInstaller(
        IPluginLogger logger)
    {
        this.Logger = logger;
        this.Logger.Debug($"{nameof(GrpcInstaller)} constructor called.");
    }

    /// <summary>
    /// Installs the bindings for gRPC services.
    /// </summary>
    public override void InstallBindings()
    {
        this.Logger.Debug($"{nameof(GrpcInstaller)}.{nameof(InstallBindings)}() called.");

        // Bind the gRPC channel
        this.Container
            .Bind<Channel>()
            .FromMethod(c =>
            {
                var config = c.Container.Resolve<PluginConfig>();

                var grpcServerAddress = config.GrpcServerAddress;

                if (string.IsNullOrWhiteSpace(grpcServerAddress))
                {
                    this.Logger.Warn($"gRPC server address is not set. Using default: {DefaultServerUrl}");

                    grpcServerAddress = DefaultServerUrl;
                }

                this.Logger.Info($"Connecting to gRPC server at: {grpcServerAddress}");


                var uriBuilder = new UriBuilder(grpcServerAddress);
                var (host, port, isSecure) = (uriBuilder.Host, uriBuilder.Port, uriBuilder.Scheme == "https");

                var channel = new Channel(
                    host, port,
                    isSecure ? ChannelCredentials.SecureSsl : ChannelCredentials.Insecure,
                    new[] {
                        // Set the maximum message sizes
                        //   - Purpose: To allow large messages to be sent
                        //   - Note: The default maximum message size is 4MB.
                        new ChannelOption(ChannelOptions.MaxSendMessageLength, 1024 * 1024 * 1024),

                        // Set the maximum receive message size
                        //   - Purpose: To allow large messages to be received
                        //   - Note: The default maximum message size is 4MB.
                        new ChannelOption(ChannelOptions.MaxReceiveMessageLength, 1024 * 1024 * 1024),

                        // Set the maximum number of concurrent streams
                        //   - Purpose: To allow multiple streams to be processed concurrently.
                        //   - Note: The default maximum number of concurrent streams is 100.
                        new ChannelOption(ChannelOptions.MaxConcurrentStreams, 1000),

                        // Set keep-alive options
                        //   - interval to 5 seconds
                        //   - timeout per request to 3 seconds
                         new ChannelOption("grpc.keepalive_time_ms", 5000),   // Interval between pings
                         new ChannelOption("grpc.keepalive_timeout_ms", 3000), // Timeout per ping

            });

                channel.ShutdownToken.Register(() =>
                {
                    this.Logger.Info($"Channel shutdown requested. State: {channel.State}");
                });

                this.Logger.Info($"Channel created. State: {channel.State}");
                return channel;
            })
            .AsSingle();

        this.Container
            .BindInterfacesAndSelfTo<GrpcRealtimeScoreSubmitterProxy>()
            .AsTransient();

        this.Container
            .BindInterfacesAndSelfTo<GrpcInitializer>()
            .AsSingle();
    }
}
