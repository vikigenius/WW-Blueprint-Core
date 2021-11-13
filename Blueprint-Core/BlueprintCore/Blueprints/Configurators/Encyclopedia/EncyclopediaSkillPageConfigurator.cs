using BlueprintCore.Blueprints.Configurators.Encyclopedia;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Encyclopedia;
namespace BlueprintCore.Blueprints.Configurators.Encyclopedia
{
  /// <summary>Configurator for <see cref="BlueprintEncyclopediaSkillPage"/>.</summary>
  /// <inheritdoc/>
  [Configures(typeof(BlueprintEncyclopediaSkillPage))]
  public class EncyclopediaSkillPageConfigurator : BaseEncyclopediaPageConfigurator<BlueprintEncyclopediaSkillPage, EncyclopediaSkillPageConfigurator>
  {
     private EncyclopediaSkillPageConfigurator(string name) : base(name) { }

    /// <inheritdoc cref="Buffs.BuffConfigurator.For(string)"/>
    public static EncyclopediaSkillPageConfigurator For(string name)
    {
      return new EncyclopediaSkillPageConfigurator(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string)"/>
    public static EncyclopediaSkillPageConfigurator New(string name)
    {
      BlueprintTool.Create<BlueprintEncyclopediaSkillPage>(name);
      return For(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string, string)"/>
    public static EncyclopediaSkillPageConfigurator New(string name, string assetId)
    {
      BlueprintTool.Create<BlueprintEncyclopediaSkillPage>(name, assetId);
      return For(name);
    }

  }
}
