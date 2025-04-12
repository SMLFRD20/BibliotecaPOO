namespace BibliotecaPOO.Models
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Autor { get; set; } = string.Empty;
        public int Ano { get; set; }

        public string MostrarInfo => $"{Titulo} - {Autor} ({Ano})";
    }
}