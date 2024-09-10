using Microsoft.AspNetCore.Identity;
using NikeStore.Models;

namespace NikeStore
{
    public class UserSeeder
    {
        private readonly UserManager<User> _userManager;

        public UserSeeder(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task SeedUsersAsync()
        {
            // Crea un nuovo utente admin
            User adminUser = new User
            {
                UserName = "stantill",
                FirstName = "Samuele",
                LastName = "Tantillo"
            };

            // Verifica se l'utente admin esiste già
            if (await _userManager.FindByNameAsync(adminUser.UserName) == null)
            {
                // Crea l'utente admin con una password
                var result = await _userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    // Assegna il ruolo "Admin" all'utente creato
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    // Log o gestione degli errori in base ai risultati
                    throw new InvalidOperationException("Non è stato possibile creare l'utente admin.");
                }
            }
        }
    }
}
