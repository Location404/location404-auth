namespace Location404.Auth.Domain.ValueObjects;

public readonly record struct Level
{
    private const int PointsMultiplier = 80;

    public int ExperiencePoints { get; }
    public int CurrentLevel { get; }
    public int ExperienceInCurrentLevel => ExperiencePoints - GetTotalExperienceForLevel(CurrentLevel);
    public int ExperienceNeededForNextLevel => GetExperienceRequiredForLevel(CurrentLevel + 1);

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
    /// <returns>Uma nova instância de Level com o XP atualizado.</returns>
    public Level AddExperience(int points)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(points, nameof(points));
        return new Level(ExperiencePoints + points);
    }

    /// <summary>
    /// Calcula o nível correspondente a uma quantidade total de experiência.
    /// </summary>
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
    /// </summary>
    private static int GetTotalExperienceForLevel(int level) => level <= 0 ? 0 : level * (level + 1) / 2 * PointsMultiplier;

    /// <summary>
    /// Retorna a quantidade de XP necessária para completar um nível específico.
    /// </summary>
    private static int GetExperienceRequiredForLevel(int level) => level * PointsMultiplier;
}