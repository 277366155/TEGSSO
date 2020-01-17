using System;

namespace ConsoleTest.Filter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TestAttribute:Attribute
    {
        public string Name;
        public int Age;
        public TestAttribute( )
        {
           
        }
    }

    public class Test2Attribute : Attribute
    {
        public string Name;
        public Test2Attribute()
        {

        }
    }
}
