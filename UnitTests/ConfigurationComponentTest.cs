using System;
using Xunit;
using Bunit;
using PeakSWC;

using static Bunit.ComponentParameterFactory;
using PeakSWC.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace UnitTests
{
    /// <summary>
    /// These tests are written entirely in C#.
    /// Learn more at https://bunit.egilhansen.com/docs/
    /// </summary>
    public class ConfigurationComponentTest : TestContext
    {
        
        //[Fact]
        public void StartComponent()
        {
            using var ctx = new TestContext();

            // Register services
            ctx.Services.AddSingleton<IConfigurationSerializer>(new JSONConfigurationSerializer());
            //ctx.Services.AddScoped<NavigationManager>
            ctx.Services.AddScoped<DialogService>();
            ctx.Services.AddScoped<NotificationService>();


            // Arrange
            var cut = ctx.RenderComponent<PeakSWC.ConfigurationEditorComponent>();

            // Assert that content of the paragraph shows counter at zero
            cut.Find("p").MarkupMatches("<p>Current count: 0</p>"); 
        }

        [Fact]
        public void ClickingButtonIncrementsCounter()
        {
            // Arrange
            var cut = RenderComponent<Counter>();

            // Act - click button to increment counter
            cut.Find("button").Click();

            // Assert that the counter was incremented
            cut.Find("p").MarkupMatches("<p>Current count: 1</p>");
        }
    }
}
