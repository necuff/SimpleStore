using Zaykov.SimpleStore;

namespace Zaykov.SimpleStore.Tests
{
    public class CommandParserTests
    {
        #region SET command tests
        [Fact]
        public void SetCommand_Valid()
        {
            //Arrange
            string command = "SET KEY VALUE";
            
            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Equal("SET", result.command.ToString());
            Assert.Equal("KEY", result.key.ToString());
            Assert.Equal("VALUE", result.value.ToString());
        }

        [Fact]
        public void SetCommand_Invalid_1() 
        {
            //Arrange
            string command = "SET KEY";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Empty(result.command.ToString());
            Assert.Empty(result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        [Fact]
        public void SetCommand_Invalid_2()
        {
            //Arrange
            string command = "SET KEY value1 value2";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Empty(result.command.ToString());
            Assert.Empty(result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        [Fact]
        public void SetCommand_Invalid_3()
        {
            //Arrange
            string command = "SET";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Empty(result.command.ToString());
            Assert.Empty(result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        #endregion

        #region GET command tests

        [Fact]
        public void GetCommand_Valid()
        {
            //Arrange
            string command = "GET KEY";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Equal("GET", result.command.ToString());
            Assert.Equal("KEY", result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        [Fact]
        public void GetCommand_Invalid_1()
        {
            //Arrange
            string command = "GET KEY VALUE";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Empty(result.command.ToString());
            Assert.Empty(result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        [Fact]
        public void GetCommand_Invalid_2()
        {
            //Arrange
            string command = "GET";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Empty(result.command.ToString());
            Assert.Empty(result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        [Fact]
        public void GetCommand_Invalid_3()
        {
            //Arrange
            string command = "GET KEY VALUE VALUE2";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Empty(result.command.ToString());
            Assert.Empty(result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        #endregion

        #region DELETE command tests
        [Fact]
        public void DeleteCommand_Valid()
        {
            //Arrange
            string command = "DELETE KEY";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Equal("DELETE", result.command.ToString());
            Assert.Equal("KEY", result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        [Fact]
        public void DeleteCommand_Invalid_1()
        {
            //Arrange
            string command = "DELETE KEY VALUE";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Empty(result.command.ToString());
            Assert.Empty(result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        [Fact]
        public void DeleteCommand_Invalid_2()
        {
            //Arrange
            string command = "DELETE";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Empty(result.command.ToString());
            Assert.Empty(result.key.ToString());
            Assert.Empty(result.value.ToString());
        }

        [Fact]
        public void DeleteCommand_Invalid_3()
        {
            //Arrange
            string command = "DELETE KEY VALUE VALUE2";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Empty(result.command.ToString());
            Assert.Empty(result.key.ToString());
            Assert.Empty(result.value.ToString());
        }
        #endregion

        #region common tests
        [Fact]
        public void EmptyCommandParse()
        {
            string command = "";

            var result = CommandParser.Parse(command);

            Assert.Equal("", result.command.ToString());
            Assert.Equal("", result.key.ToString());
            Assert.Equal("", result.value.ToString());
        } 
        [Fact]
        public void CommandWithSpacesParse()
        {
            //Arrange
            string command = "  SET    KEY    VALUE     ";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Equal("SET", result.command.ToString());
            Assert.Equal("KEY", result.key.ToString());
            Assert.Equal("VALUE", result.value.ToString());
        }
        [Fact]
        public void CommandWithRandomTextCase()
        {
            //Arrange
            string command = "Set KeY value";

            //Act
            var result = CommandParser.Parse(command);

            //Assert
            Assert.Equal("SET", result.command.ToString().ToUpper());
            Assert.Equal("KEY", result.key.ToString().ToUpper());
            Assert.Equal("VALUE", result.value.ToString().ToUpper());
        }

        #endregion
    }
}