using LighthouseSocial.Infrastructure.Auditors;
using Microsoft.Extensions.Logging.Abstractions;

namespace LighthouseSocial.Tests.Auditors;

public class DefaultCommentAuditorTests
{
    private readonly DefaultCommentAuditor _auditor;

    public DefaultCommentAuditorTests()
    {
        var logger = NullLogger<DefaultCommentAuditor>.Instance;
        _auditor = new DefaultCommentAuditor(logger);
    }

    [Fact]
    public async Task IsTextCleanAsync_ShouldReturnTrue_WhenTextValid()
    {
        // Arrange
        var comment = "It's a lovely lighthouse photo. I love it!";

        // Act
        var result = await _auditor.IsTextCleanAsync(comment);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task IsTextCleanAsync_ShouldReturnFails_WhenTextContainsBadWord()
    {
        // Arrange
        var comment = "This text contains a badword.";

        // Act
        var result = await _auditor.IsTextCleanAsync(comment);

        // Assert
        Assert.True(result.Success);
        Assert.False(result.Data);
    }
}
