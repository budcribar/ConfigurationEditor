using System;
using Xunit;
using Bunit;
using PeakSWC.Configuration;
using static Bunit.ComponentParameterFactory;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    /// <summary>
    /// These tests are written entirely in C#.
    /// Learn more at https://bunit.egilhansen.com/docs/
    /// </summary>
    public class SerializationTest : TestContext
    {
        [Fact]
        public void TryWriteWithoutData()
        {
            // Arrange
            JSONConfigurationSerializer cs = new JSONConfigurationSerializer();
            cs.Write();

            // Assert 
            Assert.Equal("null", File.ReadAllText(cs.Path));
        }


        [Fact]
        public async void TryWriteWithoutRoot()
        {
            // Arrange
            JSONConfigurationSerializer cs = new JSONConfigurationSerializer();
            cs.Roots.Add(new RootComponent());
            await cs.Write();
            var ids = await cs.ReadIds();

            // Assert 
            Assert.Equal(cs.Roots.First().Id, ids.First());
        }

    }
}
