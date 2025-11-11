using ChatClient.Configurations;
using Raylib_cs;
using System.Numerics;
using Xunit;

public class MouseInputTests
{
    [Fact]
    public void IsHovered_ReturnsTrue_WhenMouseInsideRectangle()
    {
        Rectangle rect = new Rectangle(100, 100, 200, 200);
        MouseInput.TestSetPosition(new Vector2(150, 150));

        Assert.True(MouseInput.IsHovered(rect));
    }

    [Fact]
    public void IsHovered_ReturnsTrue_WhenMouseOnEdgeOfRectangle()
    {
        Rectangle rect = new Rectangle(100, 100, 200, 200);
        MouseInput.TestSetPosition(new Vector2(100, 150));

        Assert.True(MouseInput.IsHovered(rect));
    }

    [Fact]
    public void IsHovered_ReturnsFalse_WhenMouseFarOutsideRectangle()
    {
        Rectangle rect = new Rectangle(100, 100, 200, 200);
        MouseInput.TestSetPosition(new Vector2(400, 400));

        Assert.False(MouseInput.IsHovered(rect));
    }
}