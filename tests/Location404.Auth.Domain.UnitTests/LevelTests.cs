using FluentAssertions;
using Location404.Auth.Domain.ValueObjects;

namespace Location404.Auth.Domain.UnitTests;

public class LevelTests
{
    [Fact]
    public void Initial_ShouldReturnLevelZeroWithZeroXP()
    {
        var level = Level.Initial;

        level.ExperiencePoints.Should().Be(0);
        level.CurrentLevel.Should().Be(0);
        level.ExperienceInCurrentLevel.Should().Be(0);
        level.ExperienceNeededForNextLevel.Should().Be(80);
    }

    [Fact]
    public void AddExperience_WithPositivePoints_ShouldIncreaseExperiencePoints()
    {
        var level = Level.Initial;

        var newLevel = level.AddExperience(50);

        newLevel.ExperiencePoints.Should().Be(50);
        newLevel.CurrentLevel.Should().Be(0);
    }

    [Fact]
    public void AddExperience_WithNegativePoints_ShouldThrowArgumentOutOfRangeException()
    {
        var level = Level.Initial;

        var act = () => level.AddExperience(-10);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void AddExperience_WithZeroPoints_ShouldThrowArgumentOutOfRangeException()
    {
        var level = Level.Initial;

        var act = () => level.AddExperience(0);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void CurrentLevel_With80XP_ShouldReturnLevel1()
    {
        var level = Level.Initial.AddExperience(80);

        level.CurrentLevel.Should().Be(1);
        level.ExperiencePoints.Should().Be(80);
        level.ExperienceInCurrentLevel.Should().Be(0);
        level.ExperienceNeededForNextLevel.Should().Be(160);
    }

    [Fact]
    public void CurrentLevel_With240XP_ShouldReturnLevel2()
    {
        var level = Level.Initial
            .AddExperience(80)
            .AddExperience(160);

        level.CurrentLevel.Should().Be(2);
        level.ExperiencePoints.Should().Be(240);
        level.ExperienceInCurrentLevel.Should().Be(0);
        level.ExperienceNeededForNextLevel.Should().Be(240);
    }

    [Fact]
    public void CurrentLevel_With150XP_ShouldReturnLevel1With70InCurrentLevel()
    {
        var level = Level.Initial.AddExperience(150);

        level.CurrentLevel.Should().Be(1);
        level.ExperiencePoints.Should().Be(150);
        level.ExperienceInCurrentLevel.Should().Be(70);
        level.ExperienceNeededForNextLevel.Should().Be(160);
    }

    [Fact]
    public void AddExperience_MultipleTimes_ShouldAccumulateCorrectly()
    {
        var level = Level.Initial
            .AddExperience(50)
            .AddExperience(30)
            .AddExperience(20);

        level.ExperiencePoints.Should().Be(100);
        level.CurrentLevel.Should().Be(1);
        level.ExperienceInCurrentLevel.Should().Be(20);
    }

    [Fact]
    public void ExperienceNeededForNextLevel_ShouldIncrease_WithLevel()
    {
        var level0 = Level.Initial;
        var level1 = level0.AddExperience(80);
        var level2 = level1.AddExperience(160);

        level0.ExperienceNeededForNextLevel.Should().Be(80);
        level1.ExperienceNeededForNextLevel.Should().Be(160);
        level2.ExperienceNeededForNextLevel.Should().Be(240);
    }

    [Fact]
    public void CurrentLevel_WithExactlyEnoughXP_ShouldLevelUp()
    {
        var level = Level.Initial.AddExperience(80);

        level.CurrentLevel.Should().Be(1);
        level.ExperienceInCurrentLevel.Should().Be(0);
    }

    [Fact]
    public void AddExperience_ShouldBeImmutable_AndReturnNewInstance()
    {
        var level1 = Level.Initial;
        var level2 = level1.AddExperience(50);

        level1.ExperiencePoints.Should().Be(0);
        level2.ExperiencePoints.Should().Be(50);
        level1.Should().NotBeSameAs(level2);
    }
}
