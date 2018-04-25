using System;
using System.IO;
using API.Servidor.Clases;
using NUnit.Framework;
using Utilidades;

namespace API.Servidor.Tests {
    [TestFixture]
    public class TablaLibroAutorTests {
        TablaLibroAutor TablaLibroAutor;
        Tabla TablaLibros;
        Tabla TablaAutores;

        [SetUp]
        public void SetUp() {
            // TablaLibros.
            var nombre = "TablaLibrosTest";
            var directorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BD");
            Directory.CreateDirectory(directorio);
            var ruta = Path.Combine(directorio, nombre + ".txt");
            var texto = "libro1,libro2,libro3,libro4";

            File.WriteAllText(ruta, texto);
            TablaLibros = new Tabla(nombre, directorio);

            // TablaAutores.
            nombre = "TablaAutoresTest";
            directorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BD");
            Directory.CreateDirectory(directorio);
            ruta = Path.Combine(directorio, nombre + ".txt");
            texto = "autor1,autor2,autor3,autor4";

            File.WriteAllText(ruta, texto);
            TablaAutores = new Tabla(nombre, directorio);

            // TablaLibroAutor.
            nombre = "TablaLibroAutorTest";
            directorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BD");
            Directory.CreateDirectory(directorio);
            ruta = Path.Combine(directorio, nombre + ".txt");
            texto = "libro1|autor1,libro2|autor1,libro2|autor4,libro2|autor3";

            File.WriteAllText(ruta, texto);
            TablaLibroAutor = new TablaLibroAutor(nombre, directorio, TablaLibros, TablaAutores);
        }


        [Test]
        public void Relacionar() {
            TablaLibroAutor.Relacionar("libro3", "autor3");
            var resultado = File.ReadAllText(TablaLibroAutor.Ruta);
            var esperado = "libro1|autor1,libro2|autor1,libro2|autor4,libro2|autor3,libro3|autor3";

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void ObtenerLibrosDeAutor() {
            var libros = TablaLibroAutor.ObtenerLibrosDeAutor("autor1");
            var resultado = Funciones.ListToSeparatedString(libros, ',');
            var esperado = "libro1,libro2";

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void ObtenerAutoresDeLibro() {
            var autores = TablaLibroAutor.ObtenerAutoresDeLibro("libro2");
            var resultado = Funciones.ListToSeparatedString(autores, ',');
            var esperado = "autor1,autor4,autor3";

            Assert.AreEqual(esperado, resultado);
        }
    }
}
