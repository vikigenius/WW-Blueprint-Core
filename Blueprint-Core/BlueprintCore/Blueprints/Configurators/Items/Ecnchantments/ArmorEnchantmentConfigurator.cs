using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Designers.Mechanics.EquipmentEnchants;
using Kingmaker.Designers.Mechanics.Facts;

namespace BlueprintCore.Blueprints.Configurators.Items.Ecnchantments
{
  /// <summary>Configurator for <see cref="BlueprintArmorEnchantment"/>.</summary>
  /// <inheritdoc/>
  [Configures(typeof(BlueprintArmorEnchantment))]
  public class ArmorEnchantmentConfigurator : BaseEquipmentEnchantmentConfigurator<BlueprintArmorEnchantment, ArmorEnchantmentConfigurator>
  {
     private ArmorEnchantmentConfigurator(string name) : base(name) { }

    /// <inheritdoc cref="Buffs.BuffConfigurator.For(string)"/>
    public static ArmorEnchantmentConfigurator For(string name)
    {
      return new ArmorEnchantmentConfigurator(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string)"/>
    public static ArmorEnchantmentConfigurator New(string name)
    {
      BlueprintTool.Create<BlueprintArmorEnchantment>(name);
      return For(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string, string)"/>
    public static ArmorEnchantmentConfigurator New(string name, string assetId)
    {
      BlueprintTool.Create<BlueprintArmorEnchantment>(name, assetId);
      return For(name);
    }

    /// <summary>
    /// Adds <see cref="ArmorEnhancementBonus"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(ArmorEnhancementBonus))]
    public ArmorEnchantmentConfigurator AddArmorEnhancementBonus(
        int EnhancementValue)
    {
      
      var component =  new ArmorEnhancementBonus();
      component.EnhancementValue = EnhancementValue;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddSavesFixerArmorRecalculator"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="m_Feature"><see cref="BlueprintFeature"/></param>
    [Generated]
    [Implements(typeof(AddSavesFixerArmorRecalculator))]
    public ArmorEnchantmentConfigurator AddAddSavesFixerArmorRecalculator(
        string m_Feature)
    {
      
      var component =  new AddSavesFixerArmorRecalculator();
      component.m_Feature = BlueprintTool.GetRef<BlueprintFeatureReference>(m_Feature);
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AdvanceArmorStats"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AdvanceArmorStats))]
    public ArmorEnchantmentConfigurator AddAdvanceArmorStats(
        int MaxDexBonusShift,
        int ArmorCheckPenaltyShift,
        int ArcaneSpellFailureShift)
    {
      
      var component =  new AdvanceArmorStats();
      component.MaxDexBonusShift = MaxDexBonusShift;
      component.ArmorCheckPenaltyShift = ArmorCheckPenaltyShift;
      component.ArcaneSpellFailureShift = ArcaneSpellFailureShift;
      return AddComponent(component);
    }
  }
}