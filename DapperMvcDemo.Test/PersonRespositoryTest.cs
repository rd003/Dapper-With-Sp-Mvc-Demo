using DapperMvcDemo.Data.DataAccess;
using DapperMvcDemo.Data.Models.Domain;
using DapperMvcDemo.Data.Repository;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DapperMvcDemo.Test
{
    public class PersonRespositoryTest
    {
        private readonly Mock<ISqlDataAccess> _mockDataAccess;
        private readonly IPersonRepository _personRepository;

        public PersonRespositoryTest()
        {
            _mockDataAccess= new Mock<ISqlDataAccess>();
            _personRepository= new PersonRepository(_mockDataAccess.Object);
        }

        private List<Person> people = new List<Person>()
        {
            new Person{Name="john",Email="John@gmail.com",Address="add 1"},
            new Person{Name="john2",Email="John2@gmail.com",Address="add 2"},
            new Person{Name="john3",Email="John3@gmail.com",Address="add 3"}
        };

        [Fact]
        public async Task AddAsync_ShouldReturnTrueOnSuccess()
        {
            // arrange
            Person person = people[0];
            _mockDataAccess.Setup(x => x.SaveData(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>()));

            // act

            bool result = await _personRepository.AddAsync(person);

            // assert
            Assert.True(result);
        }


        [Fact]
        public async Task AddAsync_ShouldReturnFalseOnException()
        {
            // arrange
            Person person = people[0];
            _mockDataAccess.Setup(x => x.SaveData(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // act

            bool result = await _personRepository.AddAsync(person);

            // assert
            Assert.False(result);
        }


        [Fact]
        public async Task AddAsync_ShouldCallSaveDataWithValidaParameters()
        {
            // arrange
            var person = people[0];

            // act
            var result = await _personRepository.AddAsync(person);

            // assert
            _mockDataAccess.Verify(x => x.SaveData(
                 "sp_create_person",
                 It.Is<object>(
                       o=> o.GetType().GetProperty("Name").GetValue(o).ToString()== person.Name
                       && o.GetType().GetProperty("Email").GetValue(o).ToString() == person.Email
                       && o.GetType().GetProperty("Address").GetValue(o).ToString() == person.Address
                     ),
                   "conn"
                ),Times.Once);
        }

        // Tests for UpdateAsync

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenPersonIsUpdated()
        {
            // arrange
            var person = people[0];
            _mockDataAccess.Setup(x => x.SaveData(It.IsAny<string>(), It.IsAny<Object>, It.IsAny<string>()));

            //assert
            bool updateResult = await _personRepository.UpdateAsync(person);

            // assert
            Assert.True(updateResult);
        }


        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenDataIsNotSuccessfullySaved()
        {
            // arrange
            Person person = people[0];
            _mockDataAccess.Setup(x => x.SaveData(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // act
            bool result = await _personRepository.UpdateAsync(person);

            // assert
            Assert.False(result);
        }


        [Fact]
        public async Task UpdateAsync_SaveDataShouldCallOnceWithValidParameters()
        {
            //arrange
            Person person = people[0];

            //act
            bool result = await _personRepository.UpdateAsync(person);

            // assert
            _mockDataAccess.Verify(x =>
            x.SaveData(
                "sp_update_person",
                It.Is<object>(o =>
                 Convert.ToInt32(o.GetType().GetProperty("Id").GetValue(o)) == person.Id &&
                 o.GetType().GetProperty("Name").GetValue(o).ToString() == person.Name &&
                 o.GetType().GetProperty("Email").GetValue(o).ToString() == person.Email &&
                 o.GetType().GetProperty("Address").GetValue(o).ToString() == person.Address
              ),
                "conn"
           ), Times.Exactly(1));
        }

        // test cases for get by id async

        [Fact]
        public async Task GetByIdAsync_ReturnsPerson()
        {
            // arrange
            Person expectedPerson = people[0];
            _mockDataAccess
               .Setup(x => x.GetData<Person, dynamic>("sp_get_person", It.IsAny<object>(), "conn"))
               .ReturnsAsync(new List<Person> { expectedPerson });

            // act
            Person result = await _personRepository.GetByIdAsync(expectedPerson.Id);

            // assert
            Assert.NotNull(result);
            Assert.Equal(expectedPerson.Id, result.Id);
        }


        [Fact]
        public async Task GetByIdAsync_ShouldCallGetDataOnceWithValidParameters()
        {
            // arrange
            Person expectedPerson = people[0];


            // act
            Person result = await _personRepository.GetByIdAsync(expectedPerson.Id);

            // assert
            _mockDataAccess
              .Verify(x => x.GetData<Person, dynamic>(
                  "sp_get_person",
                  It.Is<object>(
                      o => Convert.ToInt32(o.GetType().GetProperty("Id").GetValue(o)) == expectedPerson.Id
                      ),
                  "conn"),
                  Times.Once);

        }

        // test cases for getAllAsync



        [Fact]
        public async Task GetAllAsync_ShouldReturnACollection()
        {
            // arrange
            _mockDataAccess.Setup(x => x.GetData<Person, dynamic>("sp_get_people", It.IsAny<object>(), "conn")).ReturnsAsync(people);

            // act
            IEnumerable<Person> result = await _personRepository.GetAllAsync();

            // assert
            Assert.Equal(people.Count, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldCallGetDataOnceWithValidParameters()
        {

            // arrange 

            // act
            var result = await _personRepository.GetAllAsync();

            //assert
            _mockDataAccess.Verify(x => x.GetData<Person, dynamic>(
             "sp_get_people",
              It.Is<object>(o => o != null && !o.GetType().GetProperties().Any()),
             "conn"),
            Times.Once);
        }


        // Test cases for delete async



        [Fact]
        public async Task DeleteAsync_ShouldReturnTrueWhenSuccessfullyDeleted()
        {
            // arrange 
            int id = 1;

            _mockDataAccess.Setup(x => x.SaveData("sp_delete_person", It.IsAny<object>(), "conn"));

            // act
            bool result = await _personRepository.DeleteAsync(id);

            // assert
            Assert.True(result);

        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalseWhenGetAnException()
        {
            // arrange 
            int id = 1;

            _mockDataAccess.Setup(x => x.SaveData(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // act
            bool result = await _personRepository.DeleteAsync(id);

            // assert
            Assert.False(result);

        }


    }
}
