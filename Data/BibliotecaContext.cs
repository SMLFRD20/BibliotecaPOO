using BibliotecaPOO.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaPOO.Data
{
    public class BibliotecaContext : DbContext
    {
        public DbSet<Libro> Libros { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=Biblioteca.db");
        }
    }
}