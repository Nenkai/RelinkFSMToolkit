using RelinkFSMToolkit;
using RelinkFSMToolkit.ViewModels;

// Setup DI
var builder = WpfApplication<App, MainWindow>.CreateBuilder(args);

// Register services
builder.Services
    // Window VMs
    .AddSingleton<ApplicationViewModel>()
    // Control VMs
    .AddSingleton<EditorViewModel>()
    .AddSingleton<PropertyGridViewModel>()
    .AddSingleton<ExplorerViewModel>()
    .AddSingleton<ToolboxViewModel>()
    .AddSingleton<TopMenuViewModel>()
    .AddSingleton<ConnectionEditorViewModel>()
    // Future services goes here
    ;

// Run.
var app = builder.Build();
await app.RunAsync();