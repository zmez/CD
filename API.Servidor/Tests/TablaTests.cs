using System;
using System.IO;
using API.Servidor.Clases;
using NUnit.Framework;

namespace API.Servidor.Tests {
    [TestFixture]
    public class TablaTests {
        Tabla TablaTest;

        [SetUp]
        public void SetUp() {
            var nombre = "TablaTest";
            var directorio = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BD");
            Directory.CreateDirectory(directorio);
            var ruta = Path.Combine(directorio, nombre + ".txt");
            var texto = "L1,L2,L3";

            File.WriteAllText(ruta, texto);
            TablaTest = new Tabla(nombre, directorio);
        }

        [Test]
        public void AgregarRegistro() {
            TablaTest.AgregarRegistro("L4");
            var resultado = File.ReadAllText(TablaTest.Ruta);

            Assert.AreEqual("L1,L2,L3,L4", resultado);
        }

        [Test]
        [TestCase("L1", "L2,L3")]
        [TestCase("L2", "L1,L3")]
        [TestCase("L3", "L1,L2")]
        public void EliminarRegistro(string registro, string esperado) {
            TablaTest.EliminarRegistro(registro);
            var resultado = File.ReadAllText(TablaTest.Ruta);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        [TestCase("L1", true)]
        [TestCase("L2", true)]
        [TestCase("L3", true)]
        [TestCase("L4", false)]
        public void RegistroExiste(string registro, bool esperado) {
            var resultado = TablaTest.RegistroExiste(registro);

            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        [TestCase("L1", "L0", "L2,L3,L0")]
        [TestCase("L2", "L0", "L1,L3,L0")]
        [TestCase("L3", "L0", "L1,L2,L0")]
        public void ActualizarRegistro(string registroActual, string registroNuevo, string esperado) {
            TablaTest.ActualizarRegistro(registroActual, registroNuevo);
            var resultado = File.ReadAllText(TablaTest.Ruta);
            Assert.AreEqual(esperado, resultado);
        }

        [Test]
        public void CreacionRuta() {
            var esperado = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"BD\TablaTest.txt");
            Assert.AreEqual(esperado, TablaTest.Ruta);
        }
    }
}
