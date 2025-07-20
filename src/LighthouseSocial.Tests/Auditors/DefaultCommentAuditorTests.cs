using LighthouseSocial.Infrastructure.Auditors;

namespace LighthouseSocial.Tests.Auditors;

public class DefaultCommentAuditorTests
{
    private readonly DefaultCommentAuditor _auditor;

    public DefaultCommentAuditorTests()
    {
        _auditor = new DefaultCommentAuditor();
    }

    [Fact]
    public async Task IsTextCleanAsync_ShouldReturnTrue_WhenTextValid()
    {
        // Arrange
        var comment = "It's a lovely lighthouse photo. I love it!";

        // Act
        var result = await _auditor.IsTextCleanAsync(comment);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsTextCleanAsync_ShouldReturnFails_WhenTextContainsBadWord()
    {
        // Arrange
        var comment = "This text contains a badword.";

        // Act
        var result = await _auditor.IsTextCleanAsync(comment);

        // Assert
        Assert.False(result);
    }
}
