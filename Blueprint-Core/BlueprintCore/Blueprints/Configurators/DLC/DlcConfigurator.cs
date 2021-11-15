using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.DLC;
using Kingmaker.Localization;
using System.Linq;

namespace BlueprintCore.Blueprints.Configurators.DLC
{
  /// <summary>Configurator for <see cref="BlueprintDlc"/>.</summary>
  /// <inheritdoc/>
  [Configures(typeof(BlueprintDlc))]
  public class DlcConfigurator : BaseBlueprintConfigurator<BlueprintDlc, DlcConfigurator>
  {
     private DlcConfigurator(string name) : base(name) { }

    /// <inheritdoc cref="Buffs.BuffConfigurator.For(string)"/>
    public static DlcConfigurator For(string name)
    {
      return new DlcConfigurator(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string)"/>
    public static DlcConfigurator New(string name)
    {
      BlueprintTool.Create<BlueprintDlc>(name);
      return For(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string, string)"/>
    public static DlcConfigurator New(string name, string assetId)
    {
      BlueprintTool.Create<BlueprintDlc>(name, assetId);
      return For(name);
    }

    /// <summary>
    /// Sets <see cref="BlueprintDlc.Description"/> (Auto Generated)
    /// </summary>
    [Generated]
    public DlcConfigurator SetDescription(LocalizedString value)
    {
      ValidateParam(value);
      return OnConfigureInternal(bp => bp.Description = value);
    }

    /// <summary>
    /// Modifies <see cref="BlueprintDlc.RewardReferences"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="values"><see cref="BlueprintDlcReward"/></param>
    [Generated]
    public DlcConfigurator AddToRewardReferences(params string[] values)
    {
      return OnConfigureInternal(bp => bp.RewardReferences = CommonTool.Append(bp.RewardReferences, values.Select(name => BlueprintTool.GetRef<BlueprintDlcRewardReference>(name)).ToArray()));
    }

    /// <summary>
    /// Modifies <see cref="BlueprintDlc.RewardReferences"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="values"><see cref="BlueprintDlcReward"/></param>
    [Generated]
    public DlcConfigurator RemoveFromRewardReferences(params string[] values)
    {
      return OnConfigureInternal(
          bp =>
          {
            var excludeRefs = values.Select(name => BlueprintTool.GetRef<BlueprintDlcRewardReference>(name));
            bp.RewardReferences =
                bp.RewardReferences
                    .Where(
                        bpRef => !excludeRefs.ToList().Exists(exclude => bpRef.deserializedGuid == exclude.deserializedGuid))
                    .ToArray();
          });
    }
  }
}
