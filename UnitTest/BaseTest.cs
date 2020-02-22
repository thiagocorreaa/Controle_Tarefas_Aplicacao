using System;

namespace UnitTest
{
    public class BaseTest
    {
        protected double RandomDouble() => new Random().NextDouble();

        protected int RandomInteger() => new Random().Next(1, 100);
    }
}