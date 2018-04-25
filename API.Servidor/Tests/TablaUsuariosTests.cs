using System;
using System.IO;
using API.Servidor.Clases;
using NUnit.Framework;

namespace API.Servidor.Tests {
    [TestFixture]
    public class TablaUsuariosTests {
        TablaUsuarios TablaTest;

        [SetUp]
        public void SetUp() {
            var nombre = "TablaTest";
            var directorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BD");
            Directory.CreateDirectory(directorio);
            var ruta = Path.Combine(directorio, nombre + ".txt");
            var texto = "ManoloManabus|123,Fanali|456,Torter|789";

            File.WriteAllText(ruta, texto);
            TablaTest = new TablaUsuarios(nombre, directorio);
        }

        [Test]
        public void AgregarUsuario() {
            var nombre = "Juana";
            var clave = "laloca";
            var esperado = "ManoloManabus|123,Fanali|456,Torter|789,Juana|laloca";

            TablaTest.AgregarUsuario(nombre, clave);

            var resultado = File.ReadAllText(TablaTest.Ruta);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        [TestCase("ManoloManabus", "Fanali|456,Torter|789")]
        [TestCase("Fanali", "ManoloManabus|123,Torter|789")]
        [TestCase("Torter", "ManoloManabus|123,Fanali|456")]
        public void EliminarUsuario(string nombre, string esperado) {
            TablaTest.EliminarUsuario(nombre);

            var resultado = File.ReadAllText(TablaTest.Ruta);

            Assert.AreEqual(esperado, resultado);
        }
    }
}
