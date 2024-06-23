// Arquivo: UserDbContext.cs
using Microsoft.EntityFrameworkCore;
using firstORM.models;
using Microsoft.AspNetCore.Mvc;

namespace firstORM.data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
        public DbSet<UserModel> Users { get; set; }
       

        public void AddUserModel(String nome, String email, String senha)
        {
            var newUser = new UserModel
            {
                nome = nome,
                email = email,
                senha = senha
            };
            Users.Add(newUser);
            SaveChanges();
        }
        
    }
}
