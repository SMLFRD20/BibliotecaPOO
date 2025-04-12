using BibliotecaPOO.Data;
using BibliotecaPOO.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BibliotecaPOO
{
    public partial class MainWindow : Window
    {
        private readonly BibliotecaContext _context = new BibliotecaContext();
        private Libro? _libroSeleccionado;
        private bool _modoEdicion = false;

        public MainWindow()
        {
            InitializeComponent();
            _context.Database.EnsureCreated();
            CargarLibros();
        }

        private void CargarLibros()
        {
            var libros = _context.Libros.ToList();
            lstLibros.ItemsSource = libros;
            AplicarOrden();
        }

        private void cmbOrdenar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AplicarOrden();
        }

        private void AplicarOrden()
        {
            if (lstLibros.ItemsSource == null) return;

            var libros = _context.Libros.ToList();
            IOrderedEnumerable<Libro>? librosOrdenados = null;

            switch (cmbOrdenar.SelectedIndex)
            {
                case 0:
                    librosOrdenados = libros.OrderBy(l => l.Titulo);
                    break;
                case 1:
                    librosOrdenados = libros.OrderByDescending(l => l.Titulo);
                    break;
                case 2:
                    librosOrdenados = libros.OrderBy(l => l.Autor);
                    break;
                case 3:
                    librosOrdenados = libros.OrderByDescending(l => l.Autor);
                    break;
                case 4:
                    librosOrdenados = libros.OrderBy(l => l.Ano);
                    break;
                case 5:
                    librosOrdenados = libros.OrderByDescending(l => l.Ano);
                    break;
                default:
                    librosOrdenados = libros.OrderBy(l => l.Titulo);
                    break;
            }

            lstLibros.ItemsSource = librosOrdenados.ToList();
        }

        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = txtBuscar.Text.ToLower();
            var librosFiltrados = _context.Libros
                .Where(l => l.Titulo.ToLower().Contains(texto) || l.Autor.ToLower().Contains(texto))
                .ToList();

            lstLibros.ItemsSource = librosFiltrados;
            AplicarOrden();
        }

        private void lstLibros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEditar.IsEnabled = lstLibros.SelectedItem != null;
            btnEliminar.IsEnabled = lstLibros.SelectedItem != null;
        }

        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtAno.Text, out int ano) && !string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                if (_modoEdicion && _libroSeleccionado != null)
                {
                    _libroSeleccionado.Titulo = txtTitulo.Text;
                    _libroSeleccionado.Autor = txtAutor.Text;
                    _libroSeleccionado.Ano = ano;
                    _context.SaveChanges();
                    ResetearEstado();
                }
                else
                {
                    _context.Libros.Add(new Libro
                    {
                        Titulo = txtTitulo.Text,
                        Autor = txtAutor.Text,
                        Ano = ano
                    });
                    _context.SaveChanges();
                }

                CargarLibros();
                LimpiarCampos();
            }
            else
            {
                MessageBox.Show("Ingrese un título y año válidos.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (lstLibros.SelectedItem is Libro libro)
            {
                _modoEdicion = true;
                _libroSeleccionado = libro;

                txtTitulo.Text = libro.Titulo;
                txtAutor.Text = libro.Autor;
                txtAno.Text = libro.Ano.ToString();

                btnAgregar.Content = "Guardar";
                btnEditar.IsEnabled = false;
                btnCancelar.IsEnabled = true;
                btnEliminar.IsEnabled = false;
            }
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            ResetearEstado();
            LimpiarCampos();
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lstLibros.SelectedItem is Libro libro)
            {
                var respuesta = MessageBox.Show($"¿Eliminar '{libro.Titulo}' permanentemente?",
                    "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (respuesta == MessageBoxResult.Yes)
                {
                    _context.Libros.Remove(libro);
                    _context.SaveChanges();
                    CargarLibros();
                    ResetearEstado();
                }
            }
        }

        private void LimpiarCampos()
        {
            txtTitulo.Clear();
            txtAutor.Clear();
            txtAno.Clear();
        }

        private void ResetearEstado()
        {
            _modoEdicion = false;
            _libroSeleccionado = null;
            btnAgregar.Content = "Agregar Libro";
            btnEditar.IsEnabled = lstLibros.SelectedItem != null;
            btnCancelar.IsEnabled = false;
            btnEliminar.IsEnabled = lstLibros.SelectedItem != null;
        }
    }
}