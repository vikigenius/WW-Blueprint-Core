using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Globalmap.Blueprints;
using Kingmaker.Localization;
using System.Linq;

namespace BlueprintCore.Blueprints.Configurators.Globalmap
{
  /// <summary>Configurator for <see cref="BlueprintMultiEntrance"/>.</summary>
  /// <inheritdoc/>
  [Configures(typeof(BlueprintMultiEntrance))]
  public class MultiEntranceConfigurator : BaseBlueprintConfigurator<BlueprintMultiEntrance, MultiEntranceConfigurator>
  {
     private MultiEntranceConfigurator(string name) : base(name) { }

    /// <inheritdoc cref="Buffs.BuffConfigurator.For(string)"/>
    public static MultiEntranceConfigurator For(string name)
    {
      return new MultiEntranceConfigurator(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string)"/>
    public static MultiEntranceConfigurator New(string name)
    {
      BlueprintTool.Create<BlueprintMultiEntrance>(name);
      return For(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string, string)"/>
    public static MultiEntranceConfigurator New(string name, string assetId)
    {
      BlueprintTool.Create<BlueprintMultiEntrance>(name, assetId);
      return For(name);
    }

    /// <summary>
    /// Sets <see cref="BlueprintMultiEntrance.Map"/> (Auto Generated)
    /// </summary>
    [Generated]
    public MultiEntranceConfigurator SetMap(BlueprintMultiEntrance.BlueprintMultiEntranceMap value)
    {
      ValidateParam(value);
      return OnConfigureInternal(bp => bp.Map = value);
    }

    /// <summary>
    /// Sets <see cref="BlueprintMultiEntrance.Name"/> (Auto Generated)
    /// </summary>
    [Generated]
    public MultiEntranceConfigurator SetName(LocalizedString value)
    {
      ValidateParam(value);
      return OnConfigureInternal(bp => bp.Name = value);
    }

    /// <summary>
    /// Modifies <see cref="BlueprintMultiEntrance.m_Entries"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="values"><see cref="BlueprintMultiEntranceEntry"/></param>
    [Generated]
    public MultiEntranceConfigurator AddToEntries(params string[] values)
    {
      return OnConfigureInternal(bp => bp.m_Entries = CommonTool.Append(bp.m_Entries, values.Select(name => BlueprintTool.GetRef<BlueprintMultiEntranceEntryReference>(name)).ToArray()));
    }

    /// <summary>
    /// Modifies <see cref="BlueprintMultiEntrance.m_Entries"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="values"><see cref="BlueprintMultiEntranceEntry"/></param>
    [Generated]
    public MultiEntranceConfigurator RemoveFromEntries(params string[] values)
    {
      return OnConfigureInternal(
          bp =>
          {
            var excludeRefs = values.Select(name => BlueprintTool.GetRef<BlueprintMultiEntranceEntryReference>(name));
            bp.m_Entries =
                bp.m_Entries
                    .Where(
                        bpRef => !excludeRefs.ToList().Exists(exclude => bpRef.deserializedGuid == exclude.deserializedGuid))
                    .ToArray();
          });
    }
  }
}
