using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Utils;
using Kingmaker.Blueprints.Facts;
namespace BlueprintCore.Blueprints.Configurators.Facts
{
  /// <summary>
  /// Implements common fields and components for blueprints inheriting from <see cref="BlueprintFact"/>.
  /// </summary>
  /// <inheritdoc/>
  [Configures(typeof(BlueprintFact))]
  public abstract class BaseFactConfigurator<T, TBuilder>
      : BaseBlueprintConfigurator<T, TBuilder>
      where T : BlueprintFact
      where TBuilder : BaseBlueprintConfigurator<T, TBuilder>
  {
    protected BaseFactConfigurator(string name) : base(name) { }
  }
}
