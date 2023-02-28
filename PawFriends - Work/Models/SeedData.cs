using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PawFriends___Work.Data;
using static System.Net.WebRequestMethods;
using System.Collections.Generic;

namespace PawFriends___Work.Models
{
    //PASUL 4 - useri si roluri
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                //Se verifica daca baza de date are deja roluri adaugate
                if (context.Roles.Any())
                {
                    //Daca da, nu mai adaugam.
                    return;
                }

                //Daca nu, se vor creea la pasul urmator:
                context.Roles.AddRange(
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7210", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7211", Name = "Editor", NormalizedName = "Editor".ToUpper() },
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af483d56fd7212", Name = "User", NormalizedName = "User".ToUpper() }
                    );

                //Instanta pentru parolele utilizatorilor, care sunt de tip hash.
                var hasher = new PasswordHasher<ApplicationUser>();


                //Se creeaza userii in baza de date.
                context.Users.AddRange(
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                    // primary key
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },
               new ApplicationUser
               {
                   Id = "8e445865-a24d-4543-a6c6-9443d048cdb1",
                   UserName = "editor@test.com",
                   EmailConfirmed = true,
                   NormalizedEmail = "EDITOR@TEST.COM",
                   Email = "editor@test.com",
                   NormalizedUserName = "EDITOR@TEST.COM",
                   PasswordHash = hasher.HashPassword(null, "Editor1!")
               },
               new ApplicationUser
               {
                   Id = "8e445865-a24d-4543-a6c6-9443d048cdb2",
                   UserName = "user@test.com",
                   EmailConfirmed = true,
                   NormalizedEmail = "USER@TEST.COM",
                   Email = "user@test.com",
                   NormalizedUserName = "USER@TEST.COM",
                   PasswordHash = hasher.HashPassword(null, "User1!")
               }
               );

                //Asociem fiecarui user cate un rol

                context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af483d56fd7210",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af483d56fd7211",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af483d56fd7212",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
                }
                );

                //Salvam modificarile.
                context.SaveChanges();
            }
        }
    }
}