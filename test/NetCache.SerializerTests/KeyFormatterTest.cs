using System;
using Xunit;

namespace NetCache.SerializerTests
{
    public class KeyFormatterTest
    {
        [Fact]
        public void Test()
        {
            var formatter = new KeyFormatter();

            Assert.Throws<ArgumentNullException>(() => formatter.Format<string?>(null));

            var key = Guid.NewGuid();

            Assert.Equal(key.ToString(), formatter.Format(key.ToString()));
            Assert.Equal(key.ToString(), formatter.Format(key));
        }
    }
}
