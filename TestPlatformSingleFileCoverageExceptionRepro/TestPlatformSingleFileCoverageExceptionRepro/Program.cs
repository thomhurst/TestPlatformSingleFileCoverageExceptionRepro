using Microsoft.Testing.Extensions;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Capabilities;
using Microsoft.Testing.Platform.Capabilities.TestFramework;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.TestHost;

var builder = await TestApplication.CreateBuilderAsync(args);
builder.RegisterTestFramework(_ => new TestFrameworkCapabilities(), (_, _) => new DummyAdapter());
builder.AddCodeCoverageProvider();
var app = await builder.BuildAsync();
return await app.RunAsync();

internal class DummyAdapter() : ITestFramework, IDataProducer
{
    public string Uid => nameof(DummyAdapter);

    public string Version => string.Empty;

    public string DisplayName => string.Empty;

    public string Description => string.Empty;

    public Type[] DataTypesProduced => new[] { typeof(TestNodeUpdateMessage) };

    public Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context) => Task.FromResult(new CloseTestSessionResult { IsSuccess = true });

    public Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context) => Task.FromResult(new CreateTestSessionResult { IsSuccess = true });

    public Task ExecuteRequestAsync(ExecuteRequestContext context)
    {
        context.MessageBus.PublishAsync(this, new TestNodeUpdateMessage(new SessionUid("1"), new TestNode()
        {
            Uid = "2",
            DisplayName = "Blah",
            Properties = new PropertyBag(PassedTestNodeStateProperty.CachedInstance)
        }));
     
        context.Complete();
        
        return Task.CompletedTask;
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);
}

internal class Capabilities : ITestFrameworkCapabilities
{
    IReadOnlyCollection<ITestFrameworkCapability> ICapabilities<ITestFrameworkCapability>.Capabilities
        => Array.Empty<ITestFrameworkCapability>();
}