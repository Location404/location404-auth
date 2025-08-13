namespace UserIdentityService.Domain.ValueObjects;

public readonly record struct Level
{
    private const int PointsMultiplier = 80;

    public int ExperiencePoints { get; }
    public int CurrentLevel { get; }
    public int ExperienceInCurrentLevel => ExperiencePoints - GetTotalExperienceForLevel(CurrentLevel);
    public int ExperienceNeededForNextLevel => GetExperienceRequiredForLevel(CurrentLevel + 1);

    /// <summary>
    /// Initializes a new Level value object with the specified total experience points.
    /// </summary>
    /// <param name="experiencePoints">Total experience points (must be non-negative).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="experiencePoints"/> is negative.</exception>
    /// <remarks>The constructor sets ExperiencePoints and computes CurrentLevel from the provided experience total.</remarks>
    private Level(int experiencePoints)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(experiencePoints);
        ExperiencePoints = experiencePoints;
        
        CurrentLevel = CalculateLevelForExperience(experiencePoints);
    }
        
    public static Level Initial => new(0);

    /// <summary>
    /// Retorna um NOVO objeto Level com a experiência adicionada.
    /// Este método garante a imutabilidade do Value Object.
    /// </summary>
    /// <param name="points">Pontos de experiência a serem adicionados (deve ser positivo).</param>
    /// <summary>
    /// Returns a new Level with the specified positive experience points added.
    /// </summary>
    /// <param name="points">Positive experience points to add.</param>
    /// <returns>A new <see cref="Level"/> whose ExperiencePoints equals the current ExperiencePoints plus <paramref name="points"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="points"/> is less than or equal to zero.</exception>
    public Level AddExperience(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(points, nameof(points));
        return new Level(ExperiencePoints + points);
    }

    /// <summary>
    /// Calcula o nível correspondente a uma quantidade total de experiência.
    /// <summary>
    /// Calculates the player level corresponding to a total accumulated experience point value.
    /// </summary>
    /// <param name="totalExperience">Total accumulated experience points (non-negative).</param>
    /// <returns>The computed level (0-based). Returns 0 when <paramref name="totalExperience"/> is less than <c>PointsMultiplier</c>.</returns>
    private static int CalculateLevelForExperience(int totalExperience)
    {
        if (totalExperience < PointsMultiplier)
        {
            return 0;
        }

        var level = 0;
        var cumulativeXp = 0;
        while (true)
        {
            var xpForNextLevel = GetExperienceRequiredForLevel(level + 1);
            if (cumulativeXp + xpForNextLevel > totalExperience)
            {
                return level;
            }
            
            cumulativeXp += xpForNextLevel;
            level++;
        }
    }

    /// <summary>
    /// Calcula o total de XP necessário para atingir um determinado nível a partir do zero.
    /// <summary>
/// Calculates the total cumulative experience required to reach the specified level.
/// </summary>
/// <param name="level">Target level (levels <= 0 yield 0).</param>
/// <returns>The total experience points required to reach <paramref name="level"/>, computed as the triangular sum of per-level requirements scaled by <c>PointsMultiplier</c>.</returns>
    private static int GetTotalExperienceForLevel(int level) => level <= 0 ? 0 : level * (level + 1) / 2 * PointsMultiplier;

    /// <summary>
    /// Retorna a quantidade de XP necessária para completar um nível específico.
    /// <summary>
/// Gets the experience points required to advance into the specified level from the previous level.
/// </summary>
/// <param name="level">Target level (expected non-negative).</param>
/// <returns>The experience points required for that level (calculated as <c>level * PointsMultiplier</c>).</returns>
    private static int GetExperienceRequiredForLevel(int level) => level * PointsMultiplier;
}