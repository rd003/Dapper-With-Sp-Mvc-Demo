using DapperMvcDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperMvcDemo.Test
{
    public class CalculatorTest
    {
        [Theory]
        [InlineData(3,4,7)]
        [InlineData(7,4,11)]
        [InlineData(6,4,10)]
        public async Task Add_ShouldReturnValue(int a, int b, int expected)
        {
            // Arrange
           // int a = 5, b = 6, expected = 11;

            // Act
            int sum = Calculator.Add(a, b);

            // Assert
            Assert.Equal(expected, sum);
        }
    }
}
