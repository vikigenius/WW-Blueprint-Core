using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Blueprints.Configurators.Facts;
using BlueprintCore.Utils;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Designers.Mechanics.EquipmentEnchants;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Localization;
using Kingmaker.Settings;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BlueprintCore.Blueprints.Configurators.Items.Ecnchantments
{
  /// <summary>
  /// Implements common fields and components for blueprints inheriting from <see cref="BlueprintItemEnchantment"/>.
  /// </summary>
  /// <inheritdoc/>
  [Configures(typeof(BlueprintItemEnchantment))]
  public abstract class BaseItemEnchantmentConfigurator<T, TBuilder>
      : BaseFactConfigurator<T, TBuilder>
      where T : BlueprintItemEnchantment
      where TBuilder : BaseBlueprintConfigurator<T, TBuilder>
  {
    protected BaseItemEnchantmentConfigurator(string name) : base(name) { }

    /// <summary>
    /// Sets <see cref="BlueprintItemEnchantment.m_AllowNonContextActions"/> (Auto Generated)
    /// </summary>
    [Generated]
    public TBuilder SetAllowNonContextActions(bool allowNonContextActions)
    {
      return OnConfigureInternal(
          bp =>
          {
            bp.m_AllowNonContextActions = allowNonContextActions;
          });
    }

    /// <summary>
    /// Sets <see cref="BlueprintItemEnchantment.m_EnchantName"/> (Auto Generated)
    /// </summary>
    [Generated]
    public TBuilder SetEnchantName(LocalizedString enchantName)
    {
      ValidateParam(enchantName);
    
      return OnConfigureInternal(
          bp =>
          {
            bp.m_EnchantName = enchantName ?? Constants.Empty.String;
          });
    }

    /// <summary>
    /// Sets <see cref="BlueprintItemEnchantment.m_Description"/> (Auto Generated)
    /// </summary>
    [Generated]
    public TBuilder SetDescription(LocalizedString description)
    {
      ValidateParam(description);
    
      return OnConfigureInternal(
          bp =>
          {
            bp.m_Description = description ?? Constants.Empty.String;
          });
    }

    /// <summary>
    /// Sets <see cref="BlueprintItemEnchantment.m_Prefix"/> (Auto Generated)
    /// </summary>
    [Generated]
    public TBuilder SetPrefix(LocalizedString prefix)
    {
      ValidateParam(prefix);
    
      return OnConfigureInternal(
          bp =>
          {
            bp.m_Prefix = prefix ?? Constants.Empty.String;
          });
    }

    /// <summary>
    /// Sets <see cref="BlueprintItemEnchantment.m_Suffix"/> (Auto Generated)
    /// </summary>
    [Generated]
    public TBuilder SetSuffix(LocalizedString suffix)
    {
      ValidateParam(suffix);
    
      return OnConfigureInternal(
          bp =>
          {
            bp.m_Suffix = suffix ?? Constants.Empty.String;
          });
    }

    /// <summary>
    /// Sets <see cref="BlueprintItemEnchantment.m_EnchantmentCost"/> (Auto Generated)
    /// </summary>
    [Generated]
    public TBuilder SetEnchantmentCost(int enchantmentCost)
    {
      return OnConfigureInternal(
          bp =>
          {
            bp.m_EnchantmentCost = enchantmentCost;
          });
    }

    /// <summary>
    /// Sets <see cref="BlueprintItemEnchantment.m_IdentifyDC"/> (Auto Generated)
    /// </summary>
    [Generated]
    public TBuilder SetIdentifyDC(int identifyDC)
    {
      return OnConfigureInternal(
          bp =>
          {
            bp.m_IdentifyDC = identifyDC;
          });
    }

    /// <summary>
    /// Adds <see cref="Kingmaker.UnitLogic.Mechanics.Components.ContextRankConfig">ContextRankConfig</see>
    /// </summary>
    /// 
    /// <remarks>Use <see cref="Components.ContextRankConfigs">ContextRankConfigs</see> to create the config</remarks>
    [Implements(typeof(ContextRankConfig))]
    public TBuilder AddContextRankConfig(ContextRankConfig rankConfig)
    {
      return AddComponent(rankConfig);
    }

    /// <summary>
    /// Adds <see cref="ItemEnchantmentEnableWhileEtudePlaying"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="etude"><see cref="BlueprintEtude"/></param>
    [Generated]
    [Implements(typeof(ItemEnchantmentEnableWhileEtudePlaying))]
    public TBuilder AddItemEnchantmentEnableWhileEtudePlaying(
        string etude = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new ItemEnchantmentEnableWhileEtudePlaying();
      component.m_Etude = BlueprintTool.GetRef<BlueprintEtudeReference>(etude);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ContextCalculateAbilityParams"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="customProperty"><see cref="BlueprintUnitProperty"/></param>
    [Generated]
    [Implements(typeof(ContextCalculateAbilityParams))]
    public TBuilder AddContextCalculateAbilityParams(
        ContextValue casterLevel,
        ContextValue spellLevel,
        bool useKineticistMainStat = default,
        StatType statType = default,
        bool statTypeFromCustomProperty = default,
        string customProperty = null,
        bool replaceCasterLevel = default,
        bool replaceSpellLevel = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(casterLevel);
      ValidateParam(spellLevel);
    
      var component = new ContextCalculateAbilityParams();
      component.UseKineticistMainStat = useKineticistMainStat;
      component.StatType = statType;
      component.StatTypeFromCustomProperty = statTypeFromCustomProperty;
      component.m_CustomProperty = BlueprintTool.GetRef<BlueprintUnitPropertyReference>(customProperty);
      component.ReplaceCasterLevel = replaceCasterLevel;
      component.CasterLevel = casterLevel;
      component.ReplaceSpellLevel = replaceSpellLevel;
      component.SpellLevel = spellLevel;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ContextCalculateAbilityParamsBasedOnClass"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="characterClass"><see cref="BlueprintCharacterClass"/></param>
    [Generated]
    [Implements(typeof(ContextCalculateAbilityParamsBasedOnClass))]
    public TBuilder AddContextCalculateAbilityParamsBasedOnClass(
        bool useKineticistMainStat = default,
        StatType statType = default,
        string characterClass = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new ContextCalculateAbilityParamsBasedOnClass();
      component.UseKineticistMainStat = useKineticistMainStat;
      component.StatType = statType;
      component.m_CharacterClass = BlueprintTool.GetRef<BlueprintCharacterClassReference>(characterClass);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ContextCalculateSharedValue"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(ContextCalculateSharedValue))]
    public TBuilder AddContextCalculateSharedValue(
        ContextDiceValue value,
        AbilitySharedValue valueType = default,
        double modifier = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(value);
    
      var component = new ContextCalculateSharedValue();
      component.ValueType = valueType;
      component.Value = value;
      component.Modifier = modifier;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ContextSetAbilityParams"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(ContextSetAbilityParams))]
    public TBuilder AddContextSetAbilityParams(
        ContextValue dC,
        ContextValue casterLevel,
        ContextValue concentration,
        ContextValue spellLevel,
        bool add10ToDC = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(dC);
      ValidateParam(casterLevel);
      ValidateParam(concentration);
      ValidateParam(spellLevel);
    
      var component = new ContextSetAbilityParams();
      component.Add10ToDC = add10ToDC;
      component.DC = dC;
      component.CasterLevel = casterLevel;
      component.Concentration = concentration;
      component.SpellLevel = spellLevel;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityDifficultyLimitDC"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityDifficultyLimitDC))]
    public TBuilder AddAbilityDifficultyLimitDC(
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityDifficultyLimitDC();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ACBonusAgainstFactOwnerEquipment"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="checkedFact"><see cref="BlueprintFeature"/></param>
    [Generated]
    [Implements(typeof(ACBonusAgainstFactOwnerEquipment))]
    public TBuilder AddACBonusAgainstFactOwnerEquipment(
        string checkedFact = null,
        int bonus = default,
        ModifierDescriptor descriptor = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new ACBonusAgainstFactOwnerEquipment();
      component.m_CheckedFact = BlueprintTool.GetRef<BlueprintFeatureReference>(checkedFact);
      component.Bonus = bonus;
      component.Descriptor = descriptor;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddCasterLevelEquipment"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="spell"><see cref="BlueprintAbility"/></param>
    [Generated]
    [Implements(typeof(AddCasterLevelEquipment))]
    public TBuilder AddCasterLevelEquipment(
        string spell = null,
        int bonus = default,
        ModifierDescriptor descriptor = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddCasterLevelEquipment();
      component.m_Spell = BlueprintTool.GetRef<BlueprintAbilityReference>(spell);
      component.Bonus = bonus;
      component.Descriptor = descriptor;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddConditionImmunityEquipment"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AddConditionImmunityEquipment))]
    public TBuilder AddConditionImmunityEquipment(
        UnitCondition condition = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddConditionImmunityEquipment();
      component.Condition = condition;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddSpellbookEquipment"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="spellbook"><see cref="BlueprintSpellbook"/></param>
    [Generated]
    [Implements(typeof(AddSpellbookEquipment))]
    public TBuilder AddSpellbookEquipment(
        string spellbook = null,
        int casterLevel = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddSpellbookEquipment();
      component.m_Spellbook = BlueprintTool.GetRef<BlueprintSpellbookReference>(spellbook);
      component.CasterLevel = casterLevel;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddStatBonusEquipment"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AddStatBonusEquipment))]
    public TBuilder AddStatBonusEquipment(
        ModifierDescriptor descriptor = default,
        StatType stat = default,
        int value = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddStatBonusEquipment();
      component.Descriptor = descriptor;
      component.Stat = stat;
      component.Value = value;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddStatBonusEquipmentUnlessEnchant"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="checkedEnchantment"><see cref="BlueprintItemEnchantment"/></param>
    [Generated]
    [Implements(typeof(AddStatBonusEquipmentUnlessEnchant))]
    public TBuilder AddStatBonusEquipmentUnlessEnchant(
        ModifierDescriptor descriptor = default,
        StatType stat = default,
        int value = default,
        string checkedEnchantment = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddStatBonusEquipmentUnlessEnchant();
      component.Descriptor = descriptor;
      component.Stat = stat;
      component.Value = value;
      component.m_CheckedEnchantment = BlueprintTool.GetRef<BlueprintItemEnchantmentReference>(checkedEnchantment);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddUnitFactEquipment"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="blueprint"><see cref="BlueprintUnitFact"/></param>
    [Generated]
    [Implements(typeof(AddUnitFactEquipment))]
    public TBuilder AddUnitFactEquipment(
        string blueprint = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddUnitFactEquipment();
      component.m_Blueprint = BlueprintTool.GetRef<BlueprintUnitFactReference>(blueprint);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddUnitFeatureEquipment"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="feature"><see cref="BlueprintFeature"/></param>
    [Generated]
    [Implements(typeof(AddUnitFeatureEquipment))]
    public TBuilder AddUnitFeatureEquipment(
        string feature = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddUnitFeatureEquipment();
      component.m_Feature = BlueprintTool.GetRef<BlueprintFeatureReference>(feature);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AttackBonusAgainstFactOwnerEquipment"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="checkedFact"><see cref="BlueprintFeature"/></param>
    [Generated]
    [Implements(typeof(AttackBonusAgainstFactOwnerEquipment))]
    public TBuilder AddAttackBonusAgainstFactOwnerEquipment(
        string checkedFact = null,
        int attackBonus = default,
        ModifierDescriptor descriptor = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AttackBonusAgainstFactOwnerEquipment();
      component.m_CheckedFact = BlueprintTool.GetRef<BlueprintFeatureReference>(checkedFact);
      component.AttackBonus = attackBonus;
      component.Descriptor = descriptor;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DamageBonusAgainstFactOwnerEquipment"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="checkedFact"><see cref="BlueprintUnitFact"/></param>
    [Generated]
    [Implements(typeof(DamageBonusAgainstFactOwnerEquipment))]
    public TBuilder AddDamageBonusAgainstFactOwnerEquipment(
        string checkedFact = null,
        int damageBonus = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new DamageBonusAgainstFactOwnerEquipment();
      component.m_CheckedFact = BlueprintTool.GetRef<BlueprintUnitFactReference>(checkedFact);
      component.DamageBonus = damageBonus;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="EnchantmentAddBuffWhileInStealth"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(EnchantmentAddBuffWhileInStealth))]
    public TBuilder AddEnchantmentAddBuffWhileInStealth(
        EnchantmentAddBuffWhileInStealth.BuffAndDeactivateDuration[] buffs = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(buffs);
    
      var component = new EnchantmentAddBuffWhileInStealth();
      component.Buffs = buffs;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="IgnoreResistanceForDamageFromEnchantment"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(IgnoreResistanceForDamageFromEnchantment))]
    public TBuilder AddIgnoreResistanceForDamageFromEnchantment(
        IgnoreResistanceForDamageFromEnchantment.IgnoreType type = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new IgnoreResistanceForDamageFromEnchantment();
      component.m_Type = type;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="IncreaseMaxStatEnchantment"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(IncreaseMaxStatEnchantment))]
    public TBuilder AddIncreaseMaxStatEnchantment(
        int value = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new IncreaseMaxStatEnchantment();
      component.Value = value;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="IncreaseStatEquipment"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(IncreaseStatEquipment))]
    public TBuilder AddIncreaseStatEquipment(
        ModifierDescriptor descriptor = default,
        StatType stat = default,
        int value = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new IncreaseStatEquipment();
      component.Descriptor = descriptor;
      component.Stat = stat;
      component.Value = value;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="MithralEnchantment"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(MithralEnchantment))]
    public TBuilder AddMithralEnchantment(
        BlueprintComponent.Flags flags = default)
    {
      var component = new MithralEnchantment();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="PreventAbilityInterruption"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="abilities"><see cref="BlueprintActivatableAbility"/></param>
    [Generated]
    [Implements(typeof(PreventAbilityInterruption))]
    public TBuilder AddPreventAbilityInterruption(
        string[] abilities = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new PreventAbilityInterruption();
      component.m_Abilities = abilities.Select(name => BlueprintTool.GetRef<BlueprintActivatableAbilityReference>(name)).ToList();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="WeaponTypeAttackEnchant"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="type"><see cref="BlueprintWeaponType"/></param>
    [Generated]
    [Implements(typeof(WeaponTypeAttackEnchant))]
    public TBuilder AddWeaponTypeAttackEnchant(
        string type = null,
        int bonus = default,
        ModifierDescriptor descriptor = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new WeaponTypeAttackEnchant();
      component.m_Type = BlueprintTool.GetRef<BlueprintWeaponTypeReference>(type);
      component.Bonus = bonus;
      component.Descriptor = descriptor;
      component.m_Flags = flags;
      return AddComponent(component);
    }
  }
}
