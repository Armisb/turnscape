using NUnit.Framework;
using UnityEngine;


public class LoginServiceTests
{
    private LoginService _service;

    [SetUp]
    public void SetUp()
    {
        _service = new LoginService();
    }

    [Test]
    public void ValidateFields_BothFilled_ReturnsTrue()
    {
        Assert.IsTrue(_service.ValidateFields("user123", "pass123"));
    }

    [Test]
    public void ValidateFields_EmptyUsername_ReturnsFalse()
    {
        Assert.IsFalse(_service.ValidateFields("", "pass123"));
    }

    [Test]
    public void ValidateFields_WhitespacePassword_ReturnsFalse()
    {
        Assert.IsFalse(_service.ValidateFields("user123", "   "));
    }

    [Test]
    public void FormatError_400_ReturnsUsernameTaken()
    {
        Assert.AreEqual("Username already in use.", _service.FormatErrorMessage("400 Bad Request"));
    }

    [Test]
    public void FormatError_401_ReturnsInvalidCredentials()
    {
        Assert.AreEqual("Invalid Username or Password", _service.FormatErrorMessage("401 Unauthorized"));
    }

    [Test]
    public void FormatError_NoStatusCode_ReturnsServerUnreachable()
    {
        Assert.AreEqual("Cannot reach the server.", _service.FormatErrorMessage("Network error"));
    }

    [Test]
    public void ParseLoginResponse_ValidJwt_ReturnsCorrectUserId()
    {
        var fakeResponse = JsonUtility.ToJson(new LoginResponse
        {
            accessToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoidGVzdDEyMyIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiOWEyOGNiMzEtMjE2Yi00N2EwLWE0NDYtZDdmMjVmMmQ2ZDE4IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiR2FtZVVzZXIiLCJleHAiOjE3NzQyMDExNzMsImlzcyI6IlZlcnlHb29kR2FtZTMwMDAiLCJhdWQiOiJTb0Nvb2xIb21lbGVzc1BMYXllcnMifQ.WztxhS7teC7WQipUj5wCvbVZuD8DLiepG_WikNy8zPE3mP1FUUsKiIqzD-OU9s2tvjqXR5ZtSYUlXWyal915Vg",
            refreshToken = "dummy"
        });

        var (userId, username) = _service.ParseLoginResponse(fakeResponse);

        Assert.AreEqual("9a28cb31-216b-47a0-a446-d7f25f2d6d18", userId);
        Assert.AreEqual("test123", username);
    }
    
    
    
    [Test]
public void ValidateFields_NullUsername_ReturnsFalse()
{
    Assert.IsFalse(_service.ValidateFields(null, "pass123"));
}

[Test]
public void ValidateFields_NullPassword_ReturnsFalse()
{
    Assert.IsFalse(_service.ValidateFields("user123", null));
}

[Test]
public void ValidateFields_BothNull_ReturnsFalse()
{
    Assert.IsFalse(_service.ValidateFields(null, null));
}

[Test]
public void ValidateFields_BothEmpty_ReturnsFalse()
{
    Assert.IsFalse(_service.ValidateFields("", ""));
}

[Test]
public void ValidateFields_WhitespaceUsername_ReturnsFalse()
{
    Assert.IsFalse(_service.ValidateFields("   ", "pass123"));
}

[Test]
public void ValidateFields_BothWhitespace_ReturnsFalse()
{
    Assert.IsFalse(_service.ValidateFields("   ", "   "));
}

[Test]
public void ValidateFields_SpecialCharacters_ReturnsTrue()
{
    Assert.IsTrue(_service.ValidateFields("user@123!", "p@$$w0rd!"));
}

[Test]
public void ValidateFields_VeryLongUsername_ReturnsTrue()
{
    Assert.IsTrue(_service.ValidateFields(new string('a', 1000), "pass123"));
}

[Test]
public void ValidateFields_VeryLongPassword_ReturnsTrue()
{
    Assert.IsTrue(_service.ValidateFields("user123", new string('a', 1000)));
}

[Test]
public void FormatError_404_ReturnsCannotConnect()
{
    Assert.AreEqual("Cannot connect to the server", _service.FormatErrorMessage("404 Not Found"));
}

[Test]
public void FormatError_EmptyString_ReturnsServerUnreachable()
{
    Assert.AreEqual("Cannot reach the server.", _service.FormatErrorMessage(""));
}


[Test]
public void ParseLoginResponse_InvalidToken_ReturnsNull()
{
    var fakeResponse = JsonUtility.ToJson(new LoginResponse
    {
        accessToken = "notavalidtoken",
        refreshToken = "dummy"
    });

    var (userId, username) = _service.ParseLoginResponse(fakeResponse);

    Assert.IsNull(userId);
    Assert.IsNull(username);
}

[Test]
public void ParseLoginResponse_EmptyToken_ReturnsNull()
{
    var fakeResponse = JsonUtility.ToJson(new LoginResponse
    {
        accessToken = "",
        refreshToken = ""
    });

    var (userId, username) = _service.ParseLoginResponse(fakeResponse);

    Assert.IsNull(userId);
    Assert.IsNull(username);
}
    
}