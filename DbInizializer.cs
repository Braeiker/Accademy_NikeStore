using Microsoft.AspNetCore.Identity;
using NikeStore.Models;

namespace NikeStore
{
    public class DbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        // Costruttore per iniettare le dipendenze necessarie
        public DbInitializer(ApplicationDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Metodo per inizializzare il database
        public void Initialize()
        {
            // Verifica se il ruolo "Admin" esiste
            if (_roleManager.FindByNameAsync("Admin").Result == null)
            {
                // Crea i ruoli "Admin" e "User" se non esistono
                _roleManager.CreateAsync(new Role() { Id = Guid.NewGuid().ToString(), Name = "Admin" }).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new Role() { Id = Guid.NewGuid().ToString(), Name = "User" }).GetAwaiter().GetResult();
            }
            else
            {
                // Esci dal metodo se i ruoli già esistono
                return;
            }

            //    // Crea un nuovo utente admin
            //    User adminUser = new User()
            //    {
            //        UserName = "stantill",
            //        FirstName = "Samuele",
            //        LastName = "Tantillo"
            //    };

            //    // Crea l'utente admin con una password
            //    _userManager.CreateAsync(adminUser, "Admin123!").GetAwaiter().GetResult();

            //    // Assegna il ruolo "Admin" all'utente creato
            //    _userManager.AddToRoleAsync(adminUser, "Admin").GetAwaiter().GetResult();
            //}
        }

    }
}
