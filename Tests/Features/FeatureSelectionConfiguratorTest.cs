using BlueprintCore.Features;
using BlueprintCore.Blueprints;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Selection;
using Xunit;

namespace BlueprintCore.Tests.Features
{
  public class FeatureSelectionConfiguratorTest
      : CommonFeatureConfiguratorTest<BlueprintFeatureSelection, FeatureSelectionConfigurator>
  {
    public FeatureSelectionConfiguratorTest() : base()
    {
      CreateBlueprint<BlueprintFeatureSelection>(Guid);
    }

    protected override FeatureSelectionConfigurator GetConfigurator(string guid)
    {
      return FeatureSelectionConfigurator.For(guid);
    }

    [Fact]
    public void PrerequisiteSelectionPossible()
    {
      GetConfigurator(Guid)
          .PrerequisiteSelectionPossible()
          .Configure();

      var blueprint = BlueprintTool.Get<BlueprintFeatureSelection>(Guid);
      var selectionPossible = blueprint.GetComponent<PrerequisiteSelectionPossible>();
      Assert.NotNull(selectionPossible);

      Assert.Equal(
          blueprint.ToReference<BlueprintFeatureSelectionReference>(),
          selectionPossible.m_ThisFeature);
    }
  }
}
