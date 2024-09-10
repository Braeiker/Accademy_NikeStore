using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NikeStore.Connection.Service;
using NikeStore.Models;
using NikeStore.Models.Dto;

namespace AdidasStore.Controller
{
    // Attributo che definisce la route base per questo controller e abilita il comportamento API
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Richiede che l'utente sia autenticato per accedere alle azioni del controller (escluse quelle con [AllowAnonymous])
    public class ApplicationController : ControllerBase
    {
        // Dichiarazione dei campi privati per le dipendenze necessarie
        private readonly UserManager<User> _userManager;   // Gestisce le operazioni relative agli utenti
        private readonly RoleManager<Role> _roleManager;   // Gestisce le operazioni relative ai ruoli
        private readonly SignInManager<User> _signInManager; // Gestisce le operazioni di autenticazione
        private readonly AuthService _authService;         // Servizio per la gestione dell'autenticazione e la generazione di token JWT

        // Costruttore con iniezione delle dipendenze tramite dependency injection
        public ApplicationController(UserManager<User> userManager,
                                     RoleManager<Role> roleManager,
                                     SignInManager<User> signInManager,
                                     AuthService authService)
        {
            _userManager = userManager;    // Assegna l'istanza di UserManager passata al costruttore
            _roleManager = roleManager;    // Assegna l'istanza di RoleManager passata al costruttore
            _signInManager = signInManager; // Assegna l'istanza di SignInManager passata al costruttore
            _authService = authService;    // Assegna l'istanza di AuthService passata al costruttore
        }

        // Metodo per la registrazione di un nuovo utente
        [HttpPost("register")]
        [AllowAnonymous] // Permette l'accesso a questa azione anche senza autenticazione
        public async Task<IActionResult> Register([FromBody] UserDto model)
        {
            // Controlla se il modello ricevuto è valido (tutti i campi richiesti sono presenti e conformi)
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Restituisce una risposta 400 con gli errori di validazione

            // Crea un nuovo oggetto User con i dati del modello ricevuto
            var user = new User
            {
                UserName = model.Username,      // Imposta il nome utente uguale all'email
                Email = model.Email,         // Imposta l'email dell'utente
                FirstName = model.FirstName,           // Imposta il nome dell'utente
                LastName = model.LastName,   // Imposta il cognome dell'utente
            };
            Console.WriteLine("User");
            return Ok();

        }

        // Metodo per il login dell'utente
        [HttpPost("login")]
        [AllowAnonymous] // Permette l'accesso a questa azione anche senza autenticazione
        public async Task<IActionResult> Login([FromBody] UserDto model)
        {
            // Controlla se il modello ricevuto è valido
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Restituisce una risposta 400 con gli errori di validazione

            // Se il login non è riuscito, restituisce una risposta 401 (non autorizzato) con un messaggio di errore
            return Unauthorized(new { Message = "Invalid login attempt." });
        }
    }
}

