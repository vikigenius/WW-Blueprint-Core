using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils;
using Kingmaker.Globalmap.Blueprints;

namespace BlueprintCore.Blueprints.Configurators.Globalmap
{
  /// <summary>Configurator for <see cref="BlueprintGlobalMapEdge"/>.</summary>
  /// <inheritdoc/>
  [Configures(typeof(BlueprintGlobalMapEdge))]
  public class GlobalMapEdgeConfigurator : BaseBlueprintConfigurator<BlueprintGlobalMapEdge, GlobalMapEdgeConfigurator>
  {
     private GlobalMapEdgeConfigurator(string name) : base(name) { }

    /// <inheritdoc cref="Buffs.BuffConfigurator.For(string)"/>
    public static GlobalMapEdgeConfigurator For(string name)
    {
      return new GlobalMapEdgeConfigurator(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string)"/>
    public static GlobalMapEdgeConfigurator New(string name)
    {
      BlueprintTool.Create<BlueprintGlobalMapEdge>(name);
      return For(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string, string)"/>
    public static GlobalMapEdgeConfigurator New(string name, string assetId)
    {
      BlueprintTool.Create<BlueprintGlobalMapEdge>(name, assetId);
      return For(name);
    }

    /// <summary>
    /// Sets <see cref="BlueprintGlobalMapEdge.Type"/> (Auto Generated)
    /// </summary>
    [Generated]
    public GlobalMapEdgeConfigurator SetType(GlobalMapEdgeType value)
    {
      ValidateParam(value);
      return OnConfigureInternal(bp => bp.Type = value);
    }

    /// <summary>
    /// Sets <see cref="BlueprintGlobalMapEdge.Priority"/> (Auto Generated)
    /// </summary>
    [Generated]
    public GlobalMapEdgeConfigurator SetPriority(GlobalMapEdgePriority value)
    {
      ValidateParam(value);
      return OnConfigureInternal(bp => bp.Priority = value);
    }

    /// <summary>
    /// Sets <see cref="BlueprintGlobalMapEdge.m_Point1"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="value"><see cref="BlueprintGlobalMapPoint"/></param>
    [Generated]
    public GlobalMapEdgeConfigurator SetPoint1(string value)
    {
      return OnConfigureInternal(bp => bp.m_Point1 = BlueprintTool.GetRef<BlueprintGlobalMapPoint.Reference>(value));
    }

    /// <summary>
    /// Sets <see cref="BlueprintGlobalMapEdge.m_Point2"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="value"><see cref="BlueprintGlobalMapPoint"/></param>
    [Generated]
    public GlobalMapEdgeConfigurator SetPoint2(string value)
    {
      return OnConfigureInternal(bp => bp.m_Point2 = BlueprintTool.GetRef<BlueprintGlobalMapPoint.Reference>(value));
    }

    /// <summary>
    /// Sets <see cref="BlueprintGlobalMapEdge.LockCondition"/> (Auto Generated)
    /// </summary>
    [Generated]
    public GlobalMapEdgeConfigurator SetLockCondition(ConditionsBuilder value)
    {
      return OnConfigureInternal(bp => bp.LockCondition = value.Build());
    }

    /// <summary>
    /// Sets <see cref="BlueprintGlobalMapEdge.Length"/> (Auto Generated)
    /// </summary>
    [Generated]
    public GlobalMapEdgeConfigurator SetLength(float value)
    {
      return OnConfigureInternal(bp => bp.Length = value);
    }
  }
}
