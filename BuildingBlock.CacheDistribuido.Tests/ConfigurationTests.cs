using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using Xunit;

namespace BuildingBlock.CacheDistribuido.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void REDIS_INSTANCE_NAME_IS_EMPTY()
        {
            const string msg = "O parametro REDIS_INSTANCE_NAME está null ou empty";

            var ex = Assert.Throws<ArgumentException>(() => Configurations.Configurations.AddCache(It.IsAny<IServiceCollection>(), It.IsAny<IConfiguration>()));
            Assert.Equal(msg, ex.Message);
        }
    }
}
