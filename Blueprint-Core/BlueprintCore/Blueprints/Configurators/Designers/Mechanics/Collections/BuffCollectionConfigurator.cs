using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Designers.Mechanics.Collections;
using System.Linq;

namespace BlueprintCore.Blueprints.Configurators.Designers.Mechanics.Collections
{
  /// <summary>Configurator for <see cref="BuffCollection"/>.</summary>
  /// <inheritdoc/>
  [Configures(typeof(BuffCollection))]
  public class BuffCollectionConfigurator : BaseBlueprintConfigurator<BuffCollection, BuffCollectionConfigurator>
  {
     private BuffCollectionConfigurator(string name) : base(name) { }

    /// <inheritdoc cref="Buffs.BuffConfigurator.For(string)"/>
    public static BuffCollectionConfigurator For(string name)
    {
      return new BuffCollectionConfigurator(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string)"/>
    public static BuffCollectionConfigurator New(string name)
    {
      BlueprintTool.Create<BuffCollection>(name);
      return For(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string, string)"/>
    public static BuffCollectionConfigurator New(string name, string assetId)
    {
      BlueprintTool.Create<BuffCollection>(name, assetId);
      return For(name);
    }

    /// <summary>
    /// Sets <see cref="BuffCollection.CheckHidden"/> (Auto Generated)
    /// </summary>
    [Generated]
    public BuffCollectionConfigurator SetCheckHidden(bool value)
    {
      return OnConfigureInternal(bp => bp.CheckHidden = value);
    }

    /// <summary>
    /// Modifies <see cref="BuffCollection.m_BuffList"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="values"><see cref="BlueprintBuff"/></param>
    [Generated]
    public BuffCollectionConfigurator AddToBuffList(params string[] values)
    {
      return OnConfigureInternal(bp => bp.m_BuffList = CommonTool.Append(bp.m_BuffList, values.Select(name => BlueprintTool.GetRef<BlueprintBuffReference>(name)).ToArray()));
    }

    /// <summary>
    /// Modifies <see cref="BuffCollection.m_BuffList"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="values"><see cref="BlueprintBuff"/></param>
    [Generated]
    public BuffCollectionConfigurator RemoveFromBuffList(params string[] values)
    {
      return OnConfigureInternal(
          bp =>
          {
            var excludeRefs = values.Select(name => BlueprintTool.GetRef<BlueprintBuffReference>(name));
            bp.m_BuffList =
                bp.m_BuffList
                    .Where(
                        bpRef => !excludeRefs.ToList().Exists(exclude => bpRef.deserializedGuid == exclude.deserializedGuid))
                    .ToArray();
          });
    }
  }
}
