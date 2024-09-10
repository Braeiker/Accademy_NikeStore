using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NikeStore.Models;

namespace NikeStore.Controller
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        // Dichiarazione del campo privato _userManager che sarà utilizzato per interagire con il sistema di gestione degli utenti.
        private readonly UserManager<User> _userManager;

        // Costruttore del controller che riceve un'istanza di UserManager tramite dependency injection.
        public AdminController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // Metodo HTTP GET per recuperare tutti gli utenti.
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            // Utilizza UserManager per ottenere tutti gli utenti dal database in modo asincrono.
            var users = await _userManager.Users.ToListAsync();

            // Restituisce un risultato OK con la lista di utenti ottenuta.
            return Ok(users);
        }

        // Metodo HTTP GET per recuperare un utente specifico in base al nome.
        [HttpGet("{name}")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            // Utilizza UserManager per cercare un utente in base al nome, in modo asincrono.
            var user = await _userManager.FindByNameAsync(name);

            // Se l'utente non viene trovato, restituisce un risultato NotFound.
            if (user == null)
            {
                return NotFound();
            }

            // Se l'utente viene trovato, restituisce un risultato OK con l'utente.
            return Ok(user);
        }

        // Metodo HTTP PUT per aggiornare i ruoli di un utente in base al nome utente.
        [HttpPut("users/roles")]
        public async Task<IActionResult> UpdateUserRoles(string username, [FromBody] List<string> newRoles)
        {
            // Verifica se la lista dei nuovi ruoli è null o vuota, restituendo un BadRequest se lo è.
            if (newRoles == null || !newRoles.Any())
            {
                return BadRequest("Invalid User");
            }
            else
            {
                // Step 1: Recupera l'utente tramite Username.
                var user = await _userManager.FindByNameAsync(username);

                // Se l'utente non viene trovato, restituisce un risultato NotFound con un messaggio di errore.
                if (user == null)
                {
                    return NotFound($"User with username {username} not found.");
                }

                // Step 2: Rimuove tutti i ruoli esistenti dell'utente.
                var currentRoles = await _userManager.GetRolesAsync(user);  // Recupera i ruoli attuali dell'utente.
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);  // Rimuove i ruoli.

                // Se la rimozione dei ruoli non ha successo, raccoglie gli errori e restituisce un BadRequest con un messaggio di errore.
                if (!removeResult.Succeeded)
                {
                    var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                    return BadRequest($"Failed to remove user roles: {errors}");
                }

                // Step 3: Aggiunge i nuovi ruoli all'utente.
                var addResult = await _userManager.AddToRolesAsync(user, newRoles);

                if (addResult.Succeeded)
                {
                    return Ok("User roles updated successfully.");
                }
                else
                {

                    var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    return BadRequest($"Failed to add new roles: {errors}");
                }
            }
        }
    }
}


