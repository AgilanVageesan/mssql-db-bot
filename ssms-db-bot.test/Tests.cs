using System;
using Xunit;

namespace ssms_db_bot.test
{
    public class Tests
    {
        [Fact]
        public void GetRequestTest()
        {
            //Action
            var result = new string("test");

            //Assert
            _ = result.Equals("test");
        }

    }
}
