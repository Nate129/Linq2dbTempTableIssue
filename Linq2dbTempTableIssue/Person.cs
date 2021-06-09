using System;

namespace Linq2dbTempTableIssue
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] Version { get; set; }
    }
}
