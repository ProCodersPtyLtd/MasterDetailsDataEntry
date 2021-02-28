using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Platz.ObjectBuilder.Tests
{
    public class JsonTests
    {
        [Fact]
        public void Test1()
        {
            var json = "{\"Id\":1,\"Name\":\"q\",\"Surname\":\"qq\",\"Phone\":\"qqq\",\"Dob\":null}";
            var e = JsonSerializer.Deserialize(json, typeof(Default.Person));
        }
    }
}
