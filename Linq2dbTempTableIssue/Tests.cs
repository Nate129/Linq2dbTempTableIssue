using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Xunit;

namespace Linq2dbTempTableIssue
{
    public class Tests
    {
        [Fact]
        public void BulkCopy_Inserts_Correctly_When_Id_Exists_In_Permanent_Table_With_Same_Model_Type()
        {
            var factory = new EFCoreSqliteInMemoryDbFactory();
            var context = factory.CreateDbContext<MainContext>();

            var person = new Person
            {
                Name = "John Doe"
            };

            context.Add(person);
            context.SaveChanges();

            var personCopy = new Person
            {
                Id = person.Id,
                Version = BitConverter.GetBytes(1),
                Name = "Jane Doe"
            };

            var connection = context.CreateLinqToDbConnection();

            var transaction = connection.BeginTransaction();

            var tempTable = connection.CreateTempTable(new List<Person> {personCopy},
                new BulkCopyOptions {KeepIdentity = true}, "PersonUpdate", tableOptions: TableOptions.IsTemporary);

            transaction.Commit();

            var firstPerson = tempTable.First();

            firstPerson.Name.Should().Be(personCopy.Name);
            firstPerson.Version.Should().BeEquivalentTo(personCopy.Version, options => options.WithStrictOrdering());
        }

        [Fact]
        public void BulkCopy_Inserts_Correctly_When_Id_Does_Not_Exist_In_Permanent_Table_With_Same_Model_Type()
        {
            var factory = new EFCoreSqliteInMemoryDbFactory();
            var context = factory.CreateDbContext<MainContext>(false);

            var person = new Person
            {
                Name = "John Doe"
            };

            context.Add(person);
            context.SaveChanges();

            var personCopy = new Person
            {
                Id = Guid.NewGuid(),
                Version = BitConverter.GetBytes(1),
                Name = "Jane Doe"
            };

            var connection = context.CreateLinqToDbConnection();

            var transaction = connection.BeginTransaction();

            var tempTable = connection.CreateTempTable(new List<Person> {personCopy},
                new BulkCopyOptions {KeepIdentity = true}, "PersonUpdate", tableOptions: TableOptions.IsTemporary);

            transaction.Commit();

            var firstPerson = tempTable.First();

            firstPerson.Name.Should().Be(personCopy.Name);
            firstPerson.Version.Should().BeEquivalentTo(personCopy.Version, options => options.WithStrictOrdering());
        }

        [Fact]
        public void
            BulkCopy_Inserts_Correctly_When_Id_Exists_In_Permanent_Table_With_Same_Model_Type_With_Extra_Entity()
        {
            var factory = new EFCoreSqliteInMemoryDbFactory();
            var context = factory.CreateDbContext<MainContext>();

            var person = new Person
            {
                Name = "John Doe"
            };

            var person2 = new Person
            {
                Name = "Jean Doe"
            };

            context.AddRange(person, person2);
            context.SaveChanges();

            var personCopy = new Person
            {
                Id = person.Id,
                Version = BitConverter.GetBytes(1),
                Name = "Jane Doe"
            };

            var connection = context.CreateLinqToDbConnection();

            var transaction = connection.BeginTransaction();

            var tempTable = connection.CreateTempTable(new List<Person> {personCopy},
                new BulkCopyOptions {KeepIdentity = true}, "PersonUpdate");

            transaction.Commit();

            var firstPerson = tempTable.First();

            tempTable.Count().Should().Be(1);
            firstPerson.Name.Should().Be(personCopy.Name);
            firstPerson.Version.Should().BeEquivalentTo(personCopy.Version, options => options.WithStrictOrdering());
        }
    }
}
