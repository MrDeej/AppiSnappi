using FamilyApplication.AspireApp.Web.CosmosDb.User;
using FamilyApplication.AspireApp.Web.Databuffer;
using FamilyApplication.AspireApp.Web.Sessions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;

namespace FamilyApplication.AspireApp.Web.Tests
{

    public class SessionManagerTests
    {
        private readonly Mock<IHttpContextAccessor> _contextAccessorMock;
        private readonly GlobalVm _globalVm;
        private readonly SessionManager _sessionManager;

        public SessionManagerTests()
        {
            _contextAccessorMock = new Mock<IHttpContextAccessor>();
            _globalVm = new GlobalVm();
            _sessionManager = new SessionManager(_contextAccessorMock.Object, _globalVm);
        }

        [Fact]
        public void GetMyUserDto_AuthenticatedUserWithMatchingEmail_ShouldReturnUserDto()
        {
            // Arrange
            var userEmail = "test@example.com";
            var userDto = new UserDto { Username = userEmail };
            _globalVm.UserDtos.Add(userDto);

            var claims = new List<Claim> { new Claim("emails", userEmail) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.User).Returns(principal);
            _contextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

            // Act
            var result = _sessionManager.GetMyUserDto();

            // Assert
            Assert.Equal(userEmail, result.Username);
        }

        [Fact]
        public void GetMyUserDto_UnauthenticatedUser_ShouldReturnNewUserDto()
        {
            // Arrange
            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.User).Returns(new ClaimsPrincipal());
            _contextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

            // Act
            var result = _sessionManager.GetMyUserDto();

            // Assert
            Assert.NotNull(result); // Assuming new UserDto() has default properties
            Assert.Equal(string.Empty, result.Username); // Adjust based on UserDto defaults
        }

        [Fact]
        public void GetMyUserDto_AuthenticatedButNoMatchingUser_ShouldReturnNewUserDto()
        {
            // Arrange
            var userEmail = "test@example.com";

            var claims = new List<Claim> { new Claim("emails", userEmail) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.User).Returns(principal);
            _contextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

            // Act
            var result = _sessionManager.GetMyUserDto();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Username); // Adjust based on defaults
        }

        [Fact]
        public void StartSession_ShouldAddSessionAndRaiseEvent()
        {
            // Arrange
            var sessionId = "test-session-1";
            var userDto = new UserDto { Username = "testuser" };
            bool eventRaised = false;
            _sessionManager.SessionChanged += (sender, args) => eventRaised = true;

            // Act
            _sessionManager.StartSession(sessionId, userDto);

            // Assert
            Assert.True(_sessionManager.ActiveSessions.ContainsKey(sessionId));
            Assert.Equal(userDto, _sessionManager.ActiveSessions[sessionId]);
            Assert.True(eventRaised);
        }

        [Fact]
        public void EndSession_ExistingSession_ShouldRemoveSessionAndRaiseEvent()
        {
            // Arrange
            var sessionId = "test-session-2";
            var userDto = new UserDto();
            _sessionManager.StartSession(sessionId, userDto); // Pre-add
            bool eventRaised = false;
            _sessionManager.SessionChanged += (sender, args) => eventRaised = true;

            // Act
            _sessionManager.EndSession(sessionId);

            // Assert
            Assert.False(_sessionManager.ActiveSessions.ContainsKey(sessionId));
            Assert.True(eventRaised);
        }

        [Fact]
        public void EndSession_NonExistingSession_ShouldNotRaiseEvent()
        {
            // Arrange
            var sessionId = "non-existent";
            bool eventRaised = false;
            _sessionManager.SessionChanged += (sender, args) => eventRaised = true;

            // Act
            _sessionManager.EndSession(sessionId);

            // Assert
            Assert.False(eventRaised);
        }

        [Fact]
        public async Task RefreshSession_ExistingSession_ShouldUpdateLastActivityAndRaiseEvent()
        {
            // Arrange
            var sessionId = "test-session-3";
            var lastActivity = DateTimeOffset.UtcNow.AddMinutes(-3);
            var userDto = new UserDto { LastActivity = lastActivity };
            _sessionManager.StartSession(sessionId, userDto); // Pre-add
            bool eventRaised = false;
            _sessionManager.SessionChanged += (sender, args) => eventRaised = true;

            // Wait a bit to ensure timestamp changes
            await Task.Delay(100);

            // Act
            _sessionManager.RefreshSession(sessionId);

            // Assert
            Assert.True(_sessionManager.ActiveSessions[sessionId]?.LastActivity > lastActivity);
            Assert.True(eventRaised);
        }

        [Fact]
        public void RefreshSession_NonExistingSession_ShouldNotRaiseEvent()
        {
            // Arrange
            var sessionId = "non-existent";
            bool eventRaised = false;
            _sessionManager.SessionChanged += (sender, args) => eventRaised = true;

            // Act
            _sessionManager.RefreshSession(sessionId);

            // Assert
            Assert.False(eventRaised);
        }
    }
}
