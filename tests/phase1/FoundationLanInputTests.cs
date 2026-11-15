using KingdomZombies.Foundation;
using Shouldly;
using Xunit;

namespace KingdomZombies.Phase1.Tests;

/// <summary>Exercises screen-owned physical holds and local projection selection.</summary>
public sealed class FoundationLanInputTests
{
    /// <summary>Menu and text keys cannot become gameplay holds when a screen opens.</summary>
    [Fact]
    public void ScreenFocusAndLocalProjectionStayIndependent()
    {
        var input = new FoundationInput();
        input.SetContext(gameplay: false);
        input.Set(key: 68, pressed: true);
        input.Direction.ShouldBe(expected: 0);
        input.SetContext(gameplay: true);
        input.Direction.ShouldBe(expected: 0);
        input.Set(key: 68, pressed: false);
        input.Set(key: 68, pressed: true);
        input.Direction.ShouldBe(expected: 1);
    }
}
