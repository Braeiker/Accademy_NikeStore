using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Namespace per l'autenticazione JWT
using Microsoft.AspNetCore.Identity; // Namespace per Identity
using Microsoft.EntityFrameworkCore; // Namespace per Entity Framework
using Microsoft.IdentityModel.Tokens;
using NikeStore;
using NikeStore.Connection.Service;
using NikeStore.Models;
using NikeStore.Models.Settings; // Namespace per la gestione dei token JWT

// Crea un costruttore di WebApplication
var builder = WebApplication.CreateBuilder(args);

// Legge le configurazioni dal file appsettings.json
// e mappa le sezioni Identity e Jwt su oggetti della classe corrispondente
var identity = builder.Configuration.GetSection(nameof(Identity)).Get<Identity>();
var jwt = builder.Configuration.GetSection(nameof(Jwt)).Get<Jwt>();

// Aggiunge i servizi per i controller MVC e le API endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura il contesto di Entity Framework per utilizzare SQL Server
// Ottiene la stringa di connessione dal file di configurazione
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configura Identity per gestire l'autenticazione e l'autorizzazione
builder.Services.AddIdentity<User, Role>(options =>
{
    // Configura le opzioni di Identity basate sulle impostazioni lette dal file di configurazione
    options.SignIn.RequireConfirmedAccount = identity.SignInRequireConfirmedAccount;
    options.Password.RequiredLength = identity.RequiredLength;
    options.Password.RequireDigit = identity.RequireDigit;
    options.Password.RequireLowercase = identity.RequireLowercase;
    options.Password.RequireNonAlphanumeric = identity.RequireNonAlphanumeric;
    options.Password.RequireUppercase = identity.RequireUppercase;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()  // Specifica il contesto EF da usare
    .AddDefaultTokenProviders();                      // Aggiunge i provider per i token di default

// Configura l'autenticazione per utilizzare JWT (JSON Web Tokens)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // Default scheme per autenticazione
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;     // Default scheme per la sfida di autenticazione
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,                     // Verifica il mittente del token
        ValidateAudience = true,                   // Verifica il destinatario del token
        ValidateLifetime = true,                  // Verifica la scadenza del token
        ValidateIssuerSigningKey = true,          // Verifica la chiave di firma del token
        ValidAudience = jwt.Audience,              // Imposta il pubblico valido per il token
        ValidIssuer = jwt.Issuer,                 // Imposta il mittente valido per il token
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecurityKey)) // Imposta la chiave di firma
    };
});

// Configura i servizi per l'Identity e il JWT utilizzando le sezioni lette dalla configurazione
builder.Services.Configure<Identity>(builder.Configuration.GetSection(nameof(Identity)));
builder.Services.Configure<Jwt>(builder.Configuration.GetSection(nameof(Jwt)));

// Registra i servizi necessari per l'autenticazione e la gestione degli utenti
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<RoleManager<Role>>();
builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<UserSeeder>();

// Nota: hai aggiunto SignInManager<User> due volte. Una delle due righe può essere rimossa
builder.Services.AddScoped<AuthService>();  // Registra il servizio di autenticazione personalizzato

// Configura Swagger per la generazione della documentazione API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Costruisce l'applicazione
var app = builder.Build();

// Configura la pipeline delle richieste HTTP
if (app.Environment.IsDevelopment())
{
    // Abilita Swagger UI solo in ambiente di sviluppo
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // Reindirizza le richieste HTTP a HTTPS
app.UseAuthentication();
app.UseAuthorization();    // Abilita l'autorizzazione

app.MapControllers();     // Mappa i controller per le API

// Configura il middleware per la gestione CORS (Cross-Origin Resource Sharing)
// Permette richieste da qualsiasi origine, metodo e intestazione
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Blocca la migrazione del database e l'inizializzazione solo in fase di debug
#if DEBUG
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetService<UserManager<User>>();
    var roleManager = services.GetService<RoleManager<Role>>();
    var userSeeder = services.GetService<UserSeeder>();

    await applicationDbContext.Database.MigrateAsync();
    var dbInitializer = new DbInitializer(applicationDbContext, userManager, roleManager);
    dbInitializer.Initialize();
}
#endif

// Avvia l'applicazione
app.Run();
