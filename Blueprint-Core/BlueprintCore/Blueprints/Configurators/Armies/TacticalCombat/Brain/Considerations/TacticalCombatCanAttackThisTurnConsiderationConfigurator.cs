using BlueprintCore.Blueprints.Configurators.AI.Considerations;
using BlueprintCore.Utils;
using Kingmaker.Armies.TacticalCombat.Brain.Considerations;

namespace BlueprintCore.Blueprints.Configurators.Armies.TacticalCombat.Brain.Considerations
{
  /// <summary>Configurator for <see cref="TacticalCombatCanAttackThisTurnConsideration"/>.</summary>
  /// <inheritdoc/>
  [Configures(typeof(TacticalCombatCanAttackThisTurnConsideration))]
  public class TacticalCombatCanAttackThisTurnConsiderationConfigurator : BaseConsiderationConfigurator<TacticalCombatCanAttackThisTurnConsideration, TacticalCombatCanAttackThisTurnConsiderationConfigurator>
  {
     private TacticalCombatCanAttackThisTurnConsiderationConfigurator(string name) : base(name) { }

    /// <inheritdoc cref="Buffs.BuffConfigurator.For(string)"/>
    public static TacticalCombatCanAttackThisTurnConsiderationConfigurator For(string name)
    {
      return new TacticalCombatCanAttackThisTurnConsiderationConfigurator(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string)"/>
    public static TacticalCombatCanAttackThisTurnConsiderationConfigurator New(string name)
    {
      BlueprintTool.Create<TacticalCombatCanAttackThisTurnConsideration>(name);
      return For(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string, string)"/>
    public static TacticalCombatCanAttackThisTurnConsiderationConfigurator New(string name, string assetId)
    {
      BlueprintTool.Create<TacticalCombatCanAttackThisTurnConsideration>(name, assetId);
      return For(name);
    }
  }
}
