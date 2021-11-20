using BlueprintCore.Actions.Builder;
using BlueprintCore.Blueprints.Configurators;
using BlueprintCore.Conditions.Builder;
using BlueprintCore.Utils;
using Kingmaker.AreaLogic.Etudes;
using Kingmaker.Armies.Blueprints;
using Kingmaker.Armies.Components;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Prerequisites;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Validation;
using Kingmaker.Controllers.Rest;
using Kingmaker.Corruption;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.EventConditionActionSystem.Events;
using Kingmaker.Designers.Mechanics.Enums;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.DLC;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Enums;
using Kingmaker.Enums.Damage;
using Kingmaker.Globalmap.Blueprints;
using Kingmaker.Kingdom.Armies;
using Kingmaker.Kingdom.Blueprints;
using Kingmaker.Kingdom.Buffs;
using Kingmaker.Kingdom.Flags;
using Kingmaker.PubSubSystem;
using Kingmaker.RandomEncounters.Settings;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.Settings;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Abilities.Components.TargetCheckers;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.UnitLogic.Buffs.Components;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.UnitLogic.Mechanics.Properties;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlueprintCore.Blueprints.Configurators
{
  /// <summary>Fluent API for creating and modifying blueprints.</summary>
  /// 
  /// <remarks>
  /// <para>
  /// Implementation is done using the
  /// <see href="https://en.wikipedia.org/wiki/Curiously_recurring_template_pattern">Curiously Recurring Template Pattern</see>.
  /// </para>
  /// 
  /// <list type="table">
  /// <listheader>Key Features</listheader>
  /// <item>
  ///   <term>Blueprint Creation</term>
  ///   <description>
  ///     Each configurator provides a function to create a new blueprint and register it in the game library.
  ///   </description>
  /// </item>
  /// <item>
  ///   <term>Component Type Safety</term>
  ///   <description>
  ///     <para>
  ///     Blueprints are very permissive; any <see cref="BlueprintComponent"/> can be added to any blueprint type. In
  ///     reality many component types are only functional on certain types of blueprints, defined using attributes.
  ///     </para>
  ///     <para>
  ///     The configurator API mimics the inheritance structure of blueprint types in the game to restrict the available
  ///     types of components. The API does not perfectly implement these restrictions because inheritance cannot
  ///     represent the restrictions completely. In those cases type safety is provided through validation.
  ///     </para>
  ///     <para>
  ///     See <see href="https://github.com/TylerGoeringer/OwlcatModdingWiki/wiki/%5BWrath%5D-Blueprints">Wrath Blueprints</see>
  ///     for more information on component and blueprint type safety.
  ///     </para>
  ///   </description>
  /// </item>
  /// <item>
  ///   <term>Validation</term>
  ///   <description>
  ///     <para>
  ///     When <see cref="Configure"/> is called a combination of Owlcat provided and custom validation logic checks the
  ///     blueprint for errors. All errors are then logged. This validates the blueprint only contains supported
  ///     component types as well as checking for some implicit usage errors, such as
  ///     <see href="https://github.com/TylerGoeringer/OwlcatModdingWiki/wiki/[Wrath]-Abilities">AbilityEffects</see>
  ///     usage.
  ///     </para>
  ///     <para>See <see cref="Validator"/> for more details on how validation works.</para>
  ///   </description>
  /// </item>
  /// <item>
  ///   <term>Fluent API</term>
  ///   <description>
  ///     <para>
  ///     The API is designed to minimize boilerplate required to modify blueprints and create components. Configurators
  ///     work with the <see cref="ActionsBuilder"/> and
  ///     <see cref="Conditions.Builder.ConditionsBuilder">ConditionsBuilder</see> APIs as well.
  ///     </para>
  ///     <para>
  ///     Complicated components such as
  ///     <see cref="Kingmaker.UnitLogic.Mechanics.Components.ContextRankConfig">ContextRankConfig</see> which do not
  ///     work well with the configurator API have their own helper classes.
  ///     e.g. <see cref="Components.ContextRankConfigs">ContextRankConfigs</see>
  ///     </para>
  ///   </description>
  /// </item>
  /// </list>
  /// 
  /// <example>
  /// Add the Skald's Vigor and Greater Skald's Vigor feats (minus UI icons):
  /// <code>
  /// var skaldClass = "WW-SkaldClass";
  /// var inspiredRageFeature = "WW-InspiredRageFeature";
  /// var inspiredRageBuff = "WW-InspiredRageBuff";
  /// var skaldsVigorBuff = "WW-SkaldsVigorBuff";
  /// var skaldsVigorFeat = "WW-SkaldsVigorFeat";
  /// var greaterSkaldsVigorFeat = "WW-GreaterSkaldsVigorFeat";
  ///   
  /// // Register the names
  /// BlueprintTool.AddGuidsByName(
  ///     (skaldClass, "6afa347d804838b48bda16acb0573dc0"),
  ///     (inspiredRageFeature, "1a639eadc2c3ed546bc4bb236864cd0c"),
  ///     (inspiredRageBuff, "75b3978757908d24aaaecaf2dc209b89"),
  ///     // New blueprints and guids
  ///     (skaldsVigorBuff, "35fa838eb545491fbe73d593a3c456ed"),
  ///     (skaldsVigorFeat, "59f825ec85744ac29e7d49201561638d"),
  ///     (greaterSkaldsVigorFeat, "b97fa348973a4c5a916d78e9ed029e1f"));
  ///  
  /// // Load the icons and strings (not provided by library)
  /// var skaldsVigorIcon = LoadSkaldsVigorIcon();
  /// var greaterSkaldsVigorIcon = LoadGreaterSkaldsVigorIcon();
  /// var skaldsVigorName = LoadSkaldsVigorName();
  /// var greaterSkaldsVigorName = LoadGreaterSkaldsVigorName();
  /// var skaldsVigorDescription = LoadSkaldsVigorDescription();
  /// var greaterSkaldsVigorDescription = LoadGreaterSkaldsVigorDescription();
  /// 
  /// // Create the buff
  /// BuffConfigurator.New(skaldsVigorBuff)
  ///     .ContextRankConfig(
  ///         // Sets a context rank value to 1 + 2 * (SkaldLevels / 8).
  ///         ContextRankConfigs.ClassLevel(new string[] { skaldClass }).DivideByThenDoubleThenAdd1(8))
  ///     // Adds fast healing to the buff. The base value is 1 and the context rank value is added. Before level 8
  ///     // it provides 2; at level 8 it increases to 4; at level 16 it increases to 6.
  ///     .FastHealing(1, bonusValue: ContextValues.Rank())
  ///     .Configure();
  ///   
  /// // Creates an action to apply the buff. Permanent duration is used because it stays active as long as Inspired
  /// // Rage is active.
  /// var applyBuff = ActionsBuilder.New().ApplyBuff(skaldsVigorBuff, permanent: true, dispellable: false);
  /// BuffConfigurator.For(inspiredRageBuff)
  ///     .FactContextActions(
  ///         onActivated:
  ///             ActionsBuilder.New()
  ///                 // When the Inspired Rage buff is applied to the caster, Skald's Vigor is applied if they have
  ///                 // the feat.
  ///                 .Conditional(
  ///                     ConditionsBuilder.New().TargetIsYourself().HasFact(skaldsVigorFeat),
  ///                     ifTrue: applyBuff)
  ///                 // For characters other than the caster, Skald's Vigor is only applied if the caster has the
  ///                 // greater feat. Note: Technically this will apply the buff to the caster twice, but by default
  ///                 // buffs do not stack so it has no effect.
  ///                 .Conditional(
  ///                     ConditionsBuilder.New().CasterHasFact(greaterSkaldsVigorFeat), ifTrue: applyBuff),
  ///         onDeactivated:
  ///             // Removes Skald's Vigor when Inspired Rage ends.
  ///             // There is actually a bug with this implementation; Lingering Song will extend the duration of
  ///             // Skald's Vigor when it should not. The fix for this is beyond the scope of this example.
  ///             ActionsBuilder.New().RemoveBuff(skaldsVigorBuff))
  ///     .Configure();
  /// </code>
  /// </example>
  /// </remarks>
  [Configures(typeof(BlueprintScriptableObject))]
  public abstract class BaseBlueprintConfigurator<T, TBuilder>
      where T : BlueprintScriptableObject
      where TBuilder : BaseBlueprintConfigurator<T, TBuilder>
  {
    /// <summary>Describes how to resolve conflicts when multiple unique components are added to a blueprint.</summary>
    /// 
    /// <remarks>
    /// When adding a component that is unique, the function accepts a <see cref="ComponentMerge"/> and
    /// <see cref="Action"/> argument to resolve the conflict. Whenever possible, a reasonable default behavior is
    /// provided. Usually this is in the form of concatenating two components that represent lists or combining flags.
    /// </remarks>
    public enum ComponentMerge
    {
      /// <summary>Default. Throws an exception.</summary>
      Fail = 0,
      /// <summary>Skips the new component. Useful for components without per-instance behavior.</summary>
      Skip,
      /// <summary>The new component is used and the existing component is removed.</summary>
      Replace,
      /// <summary>The two components are merged into one. Requires an <see cref="Action"/> to define merge behavior.</summary>
      Merge
    }

    protected static readonly LogWrapper Logger = LogWrapper.GetInternal("BlueprintConfigurator");

    private readonly List<BlueprintComponent> Components = new();
    private readonly HashSet<UniqueComponent> UniqueComponents = new();
    private readonly List<BlueprintComponent> ComponentsToRemove = new();
    private readonly List<Action<T>> InternalOnConfigure = new();
    private readonly List<Action<T>> ExternalOnConfigure = new();
    private readonly ValidationContext Context = new();

    private bool Configured = false;
    private readonly StringBuilder ValidationWarnings = new();

    protected long EnableSpellDescriptors;
    protected long DisableSpellDescriptors;

    protected AlignmentMaskType EnablePrerequisiteAlignment;
    protected AlignmentMaskType DisablePrerequisiteAlignment;

    protected readonly TBuilder Self = null;
    protected readonly string Name;
    protected readonly T Blueprint;

    protected BaseBlueprintConfigurator(string name)
    {
      Self = (TBuilder)this;
      Name = name;
      Blueprint = BlueprintTool.Get<T>(name);
    }

    /// <summary>Commits the configuration changes to the blueprint.</summary>
    /// 
    /// <remarks>
    /// <para>
    /// After commiting the changes the blueprint is validated and any errors are logged as a warning.
    /// </para>
    /// Throws <see cref="InvalidOperationException"/> if called twice on the same configurator.
    /// </remarks>
    public T Configure()
    {
      if (Configured)
      {
        throw new InvalidOperationException($"{Name} has already been configured.");
      }

      Configured = true;
      Logger.Verbose($"Configuring {Name}.");
      ConfigureBase();
      ConfigureInternal();

      AddComponents();
      OnConfigure();
      Blueprint.OnEnable();

      Logger.Verbose($"Validating configuration for {Name}.");
      ValidateBase();
      ValidateInternal();

      if (ValidationWarnings.Length > 0)
      {
        ValidationWarnings.Insert(
            /* index= */ 0,
            $"{Name} - {typeof(T)} has warnings:{Environment.NewLine}");
        Logger.Warn(ValidationWarnings.ToString());
      }
      return Blueprint;
    }

    /// <summary>Adds the specified <see cref="BlueprintComponent"/> to the blueprint.</summary>
    /// 
    /// <remarks>
    /// It is recommended to only call this from within a configurator class or when adding a component type not
    /// supported by the configurator.
    /// </remarks>
    public TBuilder AddComponent(BlueprintComponent component)
    {
      Components.Add(component);
      return Self;
    }

    /// <summary>
    /// Adds a <see cref="BlueprintComponent"/> of the specified type to the blueprint.
    /// </summary>
    /// 
    /// <remarks>
    /// It is recommended to only call this from within a configurator class or when adding a component type not
    /// supported by the configurator.
    /// </remarks>
    /// 
    /// <param name="init">Optional initialization <see cref="Action"/> run on the component.</param>
    public TBuilder AddComponent<C>(Action<C> init) where C : BlueprintComponent, new()
    {
      var component = new C();
      init?.Invoke(component);
      return AddComponent(component);
    }

    /// <summary>
    /// Edits the first <see cref="BlueprintComponent"/> of the specified type in the blueprint.
    /// </summary>
    /// 
    /// <param name="edit">Action invoked with the component as an input argument. Run when <see cref="Configure"/> is called.</param>
    public TBuilder EditComponent<C>(Action<C> edit) where C : BlueprintComponent
    {
      return OnConfigureInternal(
          bp =>
          {
            var component = bp.GetComponent<C>();
            if (component is not null) { edit.Invoke(component); }
          });
    }

    /// <summary>Executes the specified actions when <see cref="Configure"/> is called.</summary>
    /// 
    /// <remarks>
    /// Runs as the last step of configuration, after all components are added and all other changes are applied.
    /// </remarks>
    public TBuilder OnConfigure(params Action<T>[] actions)
    {
      ExternalOnConfigure.AddRange(actions);
      return Self;
    }

    /// <summary>Internal function comparable to <see cref="OnConfigure(Action{T}[])"/>.</summary>
    /// 
    /// <remarks>
    /// Runs after all configuration is done but before <see cref="OnConfigure(Action{T}[])"/>. Configurator functions
    /// should use this to ensure the blueprint is configured before user configuration actions are run.
    /// </remarks>
    protected TBuilder OnConfigureInternal(params Action<T>[] actions)
    {
      InternalOnConfigure.AddRange(actions);
      return Self;
    }

    /// <summary>Adds the specified <see cref="BlueprintComponent"/> to the blueprint with merge handling.</summary>
    /// 
    /// <remarks>
    /// <para>
    /// It is recommended to only call this from within a configurator class or when adding a component type not
    /// supported by the configurator.
    /// </para>
    /// <para>
    /// Use this for types without the <see cref="AllowMultipleComponentsAttribute"/>.
    /// </para>
    /// </remarks>
    public TBuilder AddUniqueComponent(
        BlueprintComponent component,
        ComponentMerge behavior = ComponentMerge.Fail,
        Action<BlueprintComponent, BlueprintComponent> merge = null)
    {
      UniqueComponents.Add(new UniqueComponent(component, behavior, merge));
      return Self;
    }

    /// <summary>Removed components from the blueprint matching the specified predicate.</summary>
    /// 
    /// <remarks>Has no effect on components added with the configurator.</remarks>
    public TBuilder RemoveComponents(Func<BlueprintComponent, bool> predicate)
    {
      ComponentsToRemove.AddRange(Blueprint.Components.Where(predicate));
      return Self;
    }


        /// <summary>
        /// Adds <see cref="Kingmaker.UnitLogic.FactLogic.AddFacts">AddFacts</see>
        /// </summary>
        /// 
        /// <param name="facts"><see cref="BlueprintUnitFact"/></param>
        public TBuilder AddFacts(
            string[] facts,
            int casterLevel = 0,
            bool hasDifficultyRequirements = false,
            bool invertDifficultyRequirements = false,
            GameDifficultyOption minDifficulty = GameDifficultyOption.Story)
        {
          var addFacts = new AddFacts
          {
            m_Facts =
                facts.Select(fact => BlueprintTool.GetRef<BlueprintUnitFactReference>(fact)).ToArray(),
            CasterLevel = casterLevel,
            HasDifficultyRequirements = hasDifficultyRequirements,
            InvertDifficultyRequirements = invertDifficultyRequirements,
            MinDifficulty = minDifficulty
          };
          return AddComponent(addFacts);
        }
    
        /// <summary>
        /// Adds <see cref="AddInitiatorSkillRollTrigger"/>
        /// </summary>
        public TBuilder OnSkillCheck(
            StatType skill, ActionsBuilder actions, bool onlySuccess = true)
        {
          var trigger = new AddInitiatorSkillRollTrigger
          {
            OnlySuccess = onlySuccess,
            Skill = skill,
            Action = actions.Build()
          };
          return AddComponent(trigger);
        }
    
        /// <summary>
        /// Adds <see cref="AddFactContextActions"/>
        /// </summary>
        /// 
        /// <remarks>Default Merge: Appends the given <see cref="Kingmaker.ElementsSystem.ActionList">ActionLists</see></remarks>
        [Implements(typeof(AddFactContextActions))]
        public TBuilder FactContextActions(
            ActionsBuilder onActivated = null,
            ActionsBuilder onDeactivated = null,
            ActionsBuilder onNewRound = null,
            ComponentMerge behavior = ComponentMerge.Merge,
            Action<BlueprintComponent, BlueprintComponent> merge = null)
        {
          if (onActivated == null && onDeactivated == null && onNewRound == null)
          {
            throw new InvalidOperationException("No actions provided.");
          }
          var contextActions = new AddFactContextActions
          {
            Activated = onActivated?.Build() ?? Constants.Empty.Actions,
            Deactivated = onDeactivated?.Build() ?? Constants.Empty.Actions,
            NewRound = onNewRound?.Build() ?? Constants.Empty.Actions
          };
          return AddUniqueComponent(contextActions, behavior, merge ?? MergeFactContextActions);
        }

        [Implements(typeof(AddFactContextActions))]
        private static void MergeFactContextActions(
            BlueprintComponent current, BlueprintComponent other)
        {
          var source = current as AddFactContextActions;
          var target = other as AddFactContextActions;
          source.Activated.Actions = CommonTool.Append(source.Activated.Actions, target.Activated.Actions);
          source.Deactivated.Actions = CommonTool.Append(source.Deactivated.Actions, target.Deactivated.Actions);
          source.NewRound.Actions = CommonTool.Append(source.NewRound.Actions, target.NewRound.Actions);
        }

    /// <summary>
    /// Adds <see cref="AllowOnZoneSettings"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AllowOnZoneSettings))]
    public TBuilder AddAllowOnZoneSettings(
        GlobalMapZone[] allowedNaturalSettings = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AllowOnZoneSettings();
      component.m_AllowedNaturalSettings = allowedNaturalSettings;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DlcCondition"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="dlcReward"><see cref="BlueprintDlcReward"/></param>
    [Generated]
    [Implements(typeof(DlcCondition))]
    public TBuilder AddDlcCondition(
        string dlcReward = null,
        bool hideInstead = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new DlcCondition();
      component.m_DlcReward = BlueprintTool.GetRef<BlueprintDlcRewardReference>(dlcReward);
      component.m_HideInstead = hideInstead;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DlcStoreCheat"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(DlcStoreCheat))]
    public TBuilder AddDlcStoreCheat(
        bool isAvailableInEditor = default,
        bool isAvailableInDevBuild = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new DlcStoreCheat();
      component.m_IsAvailableInEditor = isAvailableInEditor;
      component.m_IsAvailableInDevBuild = isAvailableInDevBuild;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DlcStoreEpic"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(DlcStoreEpic))]
    public TBuilder AddDlcStoreEpic(
        string epicId,
        BlueprintComponent.Flags flags = default)
    {
      var component = new DlcStoreEpic();
      component.m_EpicId = epicId;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DlcStoreGog"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(DlcStoreGog))]
    public TBuilder AddDlcStoreGog(
        ulong gogId = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new DlcStoreGog();
      component.m_GogId = gogId;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DlcStoreSteam"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(DlcStoreSteam))]
    public TBuilder AddDlcStoreSteam(
        uint steamId = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new DlcStoreSteam();
      component.m_SteamId = steamId;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddBuffOnCorruptionClear"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="buff"><see cref="BlueprintBuff"/></param>
    [Generated]
    [Implements(typeof(AddBuffOnCorruptionClear))]
    public TBuilder AddBuffOnCorruptionClear(
        string buff = null,
        int targetBuffRank = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddBuffOnCorruptionClear();
      component.m_Buff = BlueprintTool.GetRef<BlueprintBuffReference>(buff);
      component.m_TargetBuffRank = targetBuffRank;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ComponentsList"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="list"><see cref="BlueprintComponentList"/></param>
    [Generated]
    [Implements(typeof(ComponentsList))]
    public TBuilder AddComponentsList(
        string list = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new ComponentsList();
      component.m_List = BlueprintTool.GetRef<BlueprintComponentListReference>(list);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="UnlockableFlagComponent"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="blueprintKingdomProject"><see cref="BlueprintKingdomProject"/></param>
    [Generated]
    [Implements(typeof(UnlockableFlagComponent))]
    public TBuilder AddUnlockableFlagComponent(
        string blueprintKingdomProject = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new UnlockableFlagComponent();
      component.m_BlueprintKingdomProject = BlueprintTool.GetRef<BlueprintKingdomProjectReference>(blueprintKingdomProject);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddClassLevelsToPets"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="blueprintPet"><see cref="BlueprintPet"/></param>
    [Generated]
    [Implements(typeof(AddClassLevelsToPets))]
    public TBuilder AddClassLevelsToPets(
        string blueprintPet = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddClassLevelsToPets();
      component.m_BlueprintPet = BlueprintTool.GetRef<BlueprintPet.Reference>(blueprintPet);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddPlayerLeaveCombatTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AddPlayerLeaveCombatTrigger))]
    public TBuilder AddPlayerLeaveCombatTrigger(
        ActionList actions,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new AddPlayerLeaveCombatTrigger();
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ChangeVendorPrices"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(ChangeVendorPrices))]
    public TBuilder AddChangeVendorPrices(
        Dictionary<BlueprintItem,long> itemsToCosts,
        ChangeVendorPrices.Entry[] priceOverrides = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(priceOverrides);
      ValidateParam(itemsToCosts);
    
      var component = new ChangeVendorPrices();
      component.m_PriceOverrides = priceOverrides;
      component.m_ItemsToCosts = itemsToCosts;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ReplaceDamageDice"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="weaponType"><see cref="BlueprintWeaponType"/></param>
    [Generated]
    [Implements(typeof(ReplaceDamageDice))]
    public TBuilder AddReplaceDamageDice(
        string weaponType = null,
        ReplaceDamageDice.Progression[] progressions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(progressions);
    
      var component = new ReplaceDamageDice();
      component.m_WeaponType = BlueprintTool.GetRef<BlueprintWeaponTypeReference>(weaponType);
      component.Progressions = progressions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityAcceptBurnOnCast"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityAcceptBurnOnCast))]
    public TBuilder AddAbilityAcceptBurnOnCast(
        int burnValue = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityAcceptBurnOnCast();
      component.BurnValue = burnValue;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddBuffActions"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AddBuffActions))]
    public TBuilder AddBuffActions(
        ActionList activated,
        ActionList deactivated,
        ActionList newRound,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(activated);
      ValidateParam(deactivated);
      ValidateParam(newRound);
    
      var component = new AddBuffActions();
      component.Activated = activated;
      component.Deactivated = deactivated;
      component.NewRound = newRound;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="UnitPropertyComponent"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(UnitPropertyComponent))]
    public TBuilder AddUnitPropertyComponent(
        string name,
        PropertySettings settings,
        int baseValue = default,
        UnitPropertyComponent.ExternalProperty[] addExternalProperties = null,
        string[] addLocalProperties = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(settings);
      ValidateParam(addExternalProperties);
    
      var component = new UnitPropertyComponent();
      component.Name = name;
      component.m_Settings = settings;
      component.m_BaseValue = baseValue;
      component.m_AddExternalProperties = addExternalProperties;
      component.m_AddLocalProperties = addLocalProperties;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddInitiatorAttackRollTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AddInitiatorAttackRollTrigger))]
    public TBuilder AddInitiatorAttackRollTrigger(
        ActionList action,
        bool onlyHit = default,
        bool criticalHit = default,
        bool sneakAttack = default,
        bool onOwner = default,
        bool checkWeapon = default,
        WeaponCategory weaponCategory = default,
        bool affectFriendlyTouchSpells = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(action);
    
      var component = new AddInitiatorAttackRollTrigger();
      component.OnlyHit = onlyHit;
      component.CriticalHit = criticalHit;
      component.SneakAttack = sneakAttack;
      component.OnOwner = onOwner;
      component.CheckWeapon = checkWeapon;
      component.WeaponCategory = weaponCategory;
      component.AffectFriendlyTouchSpells = affectFriendlyTouchSpells;
      component.Action = action;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddInitiatorAttackWithWeaponTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="weaponType"><see cref="BlueprintWeaponType"/></param>
    [Generated]
    [Implements(typeof(AddInitiatorAttackWithWeaponTrigger))]
    public TBuilder AddInitiatorAttackWithWeaponTrigger(
        Feet distanceLessEqual,
        ActionList action,
        bool waitForAttackResolve = default,
        bool onlyHit = default,
        bool onMiss = default,
        bool onlyOnFullAttack = default,
        bool onlyOnFirstAttack = default,
        bool onlyOnFirstHit = default,
        bool criticalHit = default,
        bool onAttackOfOpportunity = default,
        bool notCriticalHit = default,
        bool onlySneakAttack = default,
        bool notSneakAttack = default,
        string weaponType = null,
        bool checkWeaponCategory = default,
        WeaponCategory category = default,
        bool checkWeaponGroup = default,
        WeaponFighterGroup group = default,
        bool checkWeaponRangeType = default,
        WeaponRangeType rangeType = default,
        bool actionsOnInitiator = default,
        bool reduceHPToZero = default,
        bool damageMoreTargetMaxHP = default,
        bool checkDistance = default,
        bool allNaturalAndUnarmed = default,
        bool duelistWeapon = default,
        bool notExtraAttack = default,
        bool onCharge = default,
        bool ignoreAutoHit = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(action);
    
      var component = new AddInitiatorAttackWithWeaponTrigger();
      component.WaitForAttackResolve = waitForAttackResolve;
      component.OnlyHit = onlyHit;
      component.OnMiss = onMiss;
      component.OnlyOnFullAttack = onlyOnFullAttack;
      component.OnlyOnFirstAttack = onlyOnFirstAttack;
      component.OnlyOnFirstHit = onlyOnFirstHit;
      component.CriticalHit = criticalHit;
      component.OnAttackOfOpportunity = onAttackOfOpportunity;
      component.NotCriticalHit = notCriticalHit;
      component.OnlySneakAttack = onlySneakAttack;
      component.NotSneakAttack = notSneakAttack;
      component.m_WeaponType = BlueprintTool.GetRef<BlueprintWeaponTypeReference>(weaponType);
      component.CheckWeaponCategory = checkWeaponCategory;
      component.Category = category;
      component.CheckWeaponGroup = checkWeaponGroup;
      component.Group = group;
      component.CheckWeaponRangeType = checkWeaponRangeType;
      component.RangeType = rangeType;
      component.ActionsOnInitiator = actionsOnInitiator;
      component.ReduceHPToZero = reduceHPToZero;
      component.DamageMoreTargetMaxHP = damageMoreTargetMaxHP;
      component.CheckDistance = checkDistance;
      component.DistanceLessEqual = distanceLessEqual;
      component.AllNaturalAndUnarmed = allNaturalAndUnarmed;
      component.DuelistWeapon = duelistWeapon;
      component.NotExtraAttack = notExtraAttack;
      component.OnCharge = onCharge;
      component.IgnoreAutoHit = ignoreAutoHit;
      component.Action = action;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddTargetAttackRollTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AddTargetAttackRollTrigger))]
    public TBuilder AddTargetAttackRollTrigger(
        ActionList actionsOnAttacker,
        ActionList actionOnSelf,
        bool onlyHit = default,
        bool criticalHit = default,
        bool onlyMelee = default,
        bool notReach = default,
        bool checkCategory = default,
        bool not = default,
        WeaponCategory[] categories = null,
        bool affectFriendlyTouchSpells = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actionsOnAttacker);
      ValidateParam(actionOnSelf);
    
      var component = new AddTargetAttackRollTrigger();
      component.OnlyHit = onlyHit;
      component.CriticalHit = criticalHit;
      component.OnlyMelee = onlyMelee;
      component.NotReach = notReach;
      component.CheckCategory = checkCategory;
      component.Not = not;
      component.Categories = categories;
      component.AffectFriendlyTouchSpells = affectFriendlyTouchSpells;
      component.ActionsOnAttacker = actionsOnAttacker;
      component.ActionOnSelf = actionOnSelf;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddTargetBeforeAttackRollTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AddTargetBeforeAttackRollTrigger))]
    public TBuilder AddTargetBeforeAttackRollTrigger(
        SpellDescriptorWrapper spellDescriptors,
        ActionList actionsOnAttacker,
        ActionList actionOnSelf,
        bool onlyMelee = default,
        bool notReach = default,
        bool checkDescriptor = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actionsOnAttacker);
      ValidateParam(actionOnSelf);
    
      var component = new AddTargetBeforeAttackRollTrigger();
      component.OnlyMelee = onlyMelee;
      component.NotReach = notReach;
      component.CheckDescriptor = checkDescriptor;
      component.SpellDescriptors = spellDescriptors;
      component.ActionsOnAttacker = actionsOnAttacker;
      component.ActionOnSelf = actionOnSelf;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AdditionalDiceOnAttack"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="weaponType"><see cref="BlueprintWeaponType"/></param>
    [Generated]
    [Implements(typeof(AdditionalDiceOnAttack))]
    public TBuilder AdditionalDiceOnAttack(
        Feet distanceLessEqual,
        ContextDiceValue value,
        DamageTypeDescription damageType,
        bool onlyOnFullAttack = default,
        bool onlyOnFirstAttack = default,
        bool onHit = default,
        bool onlyOnFirstHit = default,
        bool criticalHit = default,
        bool onAttackOfOpportunity = default,
        bool notCriticalHit = default,
        bool onlySneakAttack = default,
        bool notSneakAttack = default,
        string weaponType = null,
        bool checkWeaponCategory = default,
        WeaponCategory category = default,
        bool checkWeaponGroup = default,
        WeaponFighterGroup group = default,
        bool checkWeaponRangeType = default,
        WeaponRangeType rangeType = default,
        bool reduceHPToZero = default,
        bool checkDistance = default,
        bool allNaturalAndUnarmed = default,
        bool duelistWeapon = default,
        bool notExtraAttack = default,
        bool onCharge = default,
        ConditionsBuilder initiatorConditions = null,
        ConditionsBuilder targetConditions = null,
        bool randomizeDamage = default,
        List<AdditionalDiceOnAttack.DamageEntry> damageEntries = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(value);
      ValidateParam(damageType);
      ValidateParam(damageEntries);
    
      var component = new AdditionalDiceOnAttack();
      component.OnlyOnFullAttack = onlyOnFullAttack;
      component.OnlyOnFirstAttack = onlyOnFirstAttack;
      component.OnHit = onHit;
      component.OnlyOnFirstHit = onlyOnFirstHit;
      component.CriticalHit = criticalHit;
      component.OnAttackOfOpportunity = onAttackOfOpportunity;
      component.NotCriticalHit = notCriticalHit;
      component.OnlySneakAttack = onlySneakAttack;
      component.NotSneakAttack = notSneakAttack;
      component.m_WeaponType = BlueprintTool.GetRef<BlueprintWeaponTypeReference>(weaponType);
      component.CheckWeaponCategory = checkWeaponCategory;
      component.Category = category;
      component.CheckWeaponGroup = checkWeaponGroup;
      component.Group = group;
      component.CheckWeaponRangeType = checkWeaponRangeType;
      component.RangeType = rangeType;
      component.ReduceHPToZero = reduceHPToZero;
      component.CheckDistance = checkDistance;
      component.DistanceLessEqual = distanceLessEqual;
      component.AllNaturalAndUnarmed = allNaturalAndUnarmed;
      component.DuelistWeapon = duelistWeapon;
      component.NotExtraAttack = notExtraAttack;
      component.OnCharge = onCharge;
      component.InitiatorConditions = initiatorConditions?.Build() ?? Constants.Empty.Conditions;
      component.TargetConditions = targetConditions?.Build() ?? Constants.Empty.Conditions;
      component.m_RandomizeDamage = randomizeDamage;
      component.Value = value;
      component.DamageType = damageType;
      component.m_DamageEntries = damageEntries;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AdditionalStatBonusOnAttackDamage"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AdditionalStatBonusOnAttackDamage))]
    public TBuilder AdditionalStatBonusOnAttackDamage(
        ConditionEnum fullAttack = default,
        ConditionEnum firstAttack = default,
        bool checkCategory = default,
        WeaponCategory category = default,
        bool checkTwoHanded = default,
        float bonus = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AdditionalStatBonusOnAttackDamage();
      component.FullAttack = fullAttack;
      component.FirstAttack = firstAttack;
      component.CheckCategory = checkCategory;
      component.Category = category;
      component.CheckTwoHanded = checkTwoHanded;
      component.Bonus = bonus;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AllAttacksEnhancement"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AllAttacksEnhancement))]
    public TBuilder AddAllAttacksEnhancement(
        int bonus = default,
        ModifierDescriptor descriptor = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AllAttacksEnhancement();
      component.Bonus = bonus;
      component.Descriptor = descriptor;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="BashingFinish"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(BashingFinish))]
    public TBuilder AddBashingFinish(
        BlueprintComponent.Flags flags = default)
    {
      var component = new BashingFinish();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DestructiveShockwave"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(DestructiveShockwave))]
    public TBuilder AddDestructiveShockwave(
        BlueprintComponent.Flags flags = default)
    {
      var component = new DestructiveShockwave();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ShieldMaster"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(ShieldMaster))]
    public TBuilder AddShieldMaster(
        BlueprintComponent.Flags flags = default)
    {
      var component = new ShieldMaster();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityAoERadius"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityAoERadius))]
    public TBuilder AddAbilityAoERadius(
        Feet radius,
        Kingmaker.UnitLogic.Abilities.Components.TargetType targetType = default,
        bool canBeUsedInTacticalCombat = default,
        int diameterInCells = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityAoERadius();
      component.m_Radius = radius;
      component.m_TargetType = targetType;
      component.m_CanBeUsedInTacticalCombat = canBeUsedInTacticalCombat;
      component.m_DiameterInCells = diameterInCells;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityMagusSpellRecallCostCalculator"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="improvedFeature"><see cref="BlueprintFeature"/></param>
    [Generated]
    [Implements(typeof(AbilityMagusSpellRecallCostCalculator))]
    public TBuilder AddAbilityMagusSpellRecallCostCalculator(
        string improvedFeature = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityMagusSpellRecallCostCalculator();
      component.m_ImprovedFeature = BlueprintTool.GetRef<BlueprintFeatureReference>(improvedFeature);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityUseOnRest"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityUseOnRest))]
    public TBuilder AddAbilityUseOnRest(
        AbilityUseOnRestType type = default,
        int baseValue = default,
        bool addCasterLevel = default,
        bool multiplyByCasterLevel = default,
        int maxCasterLevel = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityUseOnRest();
      component.Type = type;
      component.BaseValue = baseValue;
      component.AddCasterLevel = addCasterLevel;
      component.MultiplyByCasterLevel = multiplyByCasterLevel;
      component.MaxCasterLevel = maxCasterLevel;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityTargetCellsRestriction"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityTargetCellsRestriction))]
    public TBuilder AddAbilityTargetCellsRestriction(
        List<int> allowedColumns = null,
        bool factionDependent = default,
        bool onlyEmptyCells = default,
        int diameter = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityTargetCellsRestriction();
      component.m_AllowedColumns = allowedColumns;
      component.m_FactionDependent = factionDependent;
      component.m_OnlyEmptyCells = onlyEmptyCells;
      component.m_Diameter = diameter;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityTargetHasCondition"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityTargetHasCondition))]
    public TBuilder AddAbilityTargetHasCondition(
        UnitCondition condition = default,
        bool not = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityTargetHasCondition();
      component.Condition = condition;
      component.Not = not;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityTargetHasConditionOrBuff"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="buffs"><see cref="BlueprintBuff"/></param>
    [Generated]
    [Implements(typeof(AbilityTargetHasConditionOrBuff))]
    public TBuilder AddAbilityTargetHasConditionOrBuff(
        bool not = default,
        UnitCondition condition = default,
        string[] buffs = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityTargetHasConditionOrBuff();
      component.Not = not;
      component.Condition = condition;
      component.m_Buffs = buffs.Select(name => BlueprintTool.GetRef<BlueprintBuffReference>(name)).ToArray();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityTargetHasOneOfConditionsOrHP"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityTargetHasOneOfConditionsOrHP))]
    public TBuilder AddAbilityTargetHasOneOfConditionsOrHP(
        UnitCondition[] condition = null,
        bool needHPCondition = default,
        int currentHPLessThan = default,
        bool invertedHP = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityTargetHasOneOfConditionsOrHP();
      component.Condition = condition;
      component.NeedHPCondition = needHPCondition;
      component.CurrentHPLessThan = currentHPLessThan;
      component.InvertedHP = invertedHP;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityTargetIsAnimalCompanion"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityTargetIsAnimalCompanion))]
    public TBuilder AddAbilityTargetIsAnimalCompanion(
        bool not = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityTargetIsAnimalCompanion();
      component.Not = not;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityTargetIsSuitableMount"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityTargetIsSuitableMount))]
    public TBuilder AddAbilityTargetIsSuitableMount(
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityTargetIsSuitableMount();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityTargetIsSuitableMountSize"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityTargetIsSuitableMountSize))]
    public TBuilder AddAbilityTargetIsSuitableMountSize(
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityTargetIsSuitableMountSize();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AbilityTargetRangeRestriction"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AbilityTargetRangeRestriction))]
    public TBuilder AddAbilityTargetRangeRestriction(
        Feet distance,
        CompareOperation.Type compareType = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AbilityTargetRangeRestriction();
      component.Distance = distance;
      component.CompareType = compareType;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ArmyBattleResultsTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="demonArmies"><see cref="BlueprintArmyPreset"/></param>
    /// <param name="crusadeLeader"><see cref="BlueprintArmyLeader"/></param>
    [Generated]
    [Implements(typeof(ArmyBattleResultsTrigger))]
    public TBuilder AddArmyBattleResultsTrigger(
        ActionList onCrusadersVictory,
        ActionList onDemonsVictory,
        RegionId assignedRegion = default,
        bool demonsFromList = default,
        string[] demonArmies = null,
        ArmyType demonsArmyType = default,
        bool specificCrusadeLeader = default,
        string crusadeLeader = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(onCrusadersVictory);
      ValidateParam(onDemonsVictory);
    
      var component = new ArmyBattleResultsTrigger();
      component.OnCrusadersVictory = onCrusadersVictory;
      component.OnDemonsVictory = onDemonsVictory;
      component.m_AssignedRegion = assignedRegion;
      component.m_DemonsFromList = demonsFromList;
      component.m_DemonArmies = demonArmies.Select(name => BlueprintTool.GetRef<BlueprintArmyPresetReference>(name)).ToList();
      component.m_DemonsArmyType = demonsArmyType;
      component.m_SpecificCrusadeLeader = specificCrusadeLeader;
      component.m_CrusadeLeader = BlueprintTool.GetRef<BlueprintArmyLeaderReference>(crusadeLeader);
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="KingdomRegionClaimedTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="regions"><see cref="BlueprintRegion"/></param>
    [Generated]
    [Implements(typeof(KingdomRegionClaimedTrigger))]
    public TBuilder AddKingdomRegionClaimedTrigger(
        ActionList actions,
        string[] regions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new KingdomRegionClaimedTrigger();
      component.m_Regions = regions.Select(name => BlueprintTool.GetRef<BlueprintRegionReference>(name)).ToArray();
      component.m_Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="SettlementSiegeTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="settlementLocation"><see cref="BlueprintGlobalMapPoint"/></param>
    [Generated]
    [Implements(typeof(SettlementSiegeTrigger))]
    public TBuilder AddSettlementSiegeTrigger(
        ActionList onSiegeStart,
        ActionList onSiegeEnd,
        ActionList onSettlementDestroyed,
        bool specificLocation = default,
        string settlementLocation = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(onSiegeStart);
      ValidateParam(onSiegeEnd);
      ValidateParam(onSettlementDestroyed);
    
      var component = new SettlementSiegeTrigger();
      component.m_SpecificLocation = specificLocation;
      component.m_SettlementLocation = BlueprintTool.GetRef<BlueprintGlobalMapPointReference>(settlementLocation);
      component.m_OnSiegeStart = onSiegeStart;
      component.m_OnSiegeEnd = onSiegeEnd;
      component.m_OnSettlementDestroyed = onSettlementDestroyed;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ArmyUnitRecruitedTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="armyUnits"><see cref="BlueprintUnit"/></param>
    [Generated]
    [Implements(typeof(ArmyUnitRecruitedTrigger))]
    public TBuilder AddArmyUnitRecruitedTrigger(
        ActionList action,
        MercenariesIncludeOption mercenariesFilter = default,
        bool byTag = default,
        ArmyProperties armyTag = default,
        bool byUnits = default,
        string[] armyUnits = null,
        int minCount = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(action);
    
      var component = new ArmyUnitRecruitedTrigger();
      component.m_MercenariesFilter = mercenariesFilter;
      component.m_ByTag = byTag;
      component.m_ArmyTag = armyTag;
      component.m_ByUnits = byUnits;
      component.m_ArmyUnits = armyUnits.Select(name => BlueprintTool.GetRef<BlueprintUnitReference>(name)).ToArray();
      component.m_MinCount = minCount;
      component.m_Action = action;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="LeaderRecruitedTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(LeaderRecruitedTrigger))]
    public TBuilder AddLeaderRecruitedTrigger(
        ActionList action,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(action);
    
      var component = new LeaderRecruitedTrigger();
      component.m_Action = action;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="SummonUnitsAfterArmyBattle"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(SummonUnitsAfterArmyBattle))]
    public TBuilder AddSummonUnitsAfterArmyBattle(
        SummonUnitsAfterArmyBattle.SummonGroup[] groups = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(groups);
    
      var component = new SummonUnitsAfterArmyBattle();
      component.m_Groups = groups;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ArmyAbilityTags"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(ArmyAbilityTags))]
    public TBuilder AddArmyAbilityTags(
        ArmyProperties properties = default,
        BlueprintComponent.Flags flags = default)
    {
      var component = new ArmyAbilityTags();
      component.Properties = properties;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="GarrisonDefeatedTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="location"><see cref="BlueprintGlobalMapPoint"/></param>
    [Generated]
    [Implements(typeof(GarrisonDefeatedTrigger))]
    public TBuilder AddGarrisonDefeatedTrigger(
        ActionList actions,
        string location = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new GarrisonDefeatedTrigger();
      component.m_Location = BlueprintTool.GetRef<BlueprintGlobalMapPoint.Reference>(location);
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="PlayerVisitGlobalMapLocationTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="location"><see cref="BlueprintGlobalMapPoint"/></param>
    [Generated]
    [Implements(typeof(PlayerVisitGlobalMapLocationTrigger))]
    public TBuilder AddPlayerVisitGlobalMapLocationTrigger(
        ActionList actions,
        string location = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new PlayerVisitGlobalMapLocationTrigger();
      component.m_Location = BlueprintTool.GetRef<BlueprintGlobalMapPoint.Reference>(location);
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AddEquipmentToPet"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="items"><see cref="BlueprintItem"/></param>
    [Generated]
    [Implements(typeof(AddEquipmentToPet))]
    public TBuilder AddEquipmentToPet(
        string[] items = null,
        BlueprintComponent.Flags flags = default)
    {
      var component = new AddEquipmentToPet();
      component.m_Items = items.Select(name => BlueprintTool.GetRef<BlueprintItemReference>(name)).ToArray();
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="OnIsleStateEnterTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(OnIsleStateEnterTrigger))]
    public TBuilder AddOnIsleStateEnterTrigger(
        IsleEvaluator isleEvaluator,
        string targetState,
        ActionList actions,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(isleEvaluator);
      ValidateParam(actions);
    
      var component = new OnIsleStateEnterTrigger();
      component.m_IsleEvaluator = isleEvaluator;
      component.m_TargetState = targetState;
      component.m_Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="OnIsleStateExitTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(OnIsleStateExitTrigger))]
    public TBuilder AddOnIsleStateExitTrigger(
        IsleEvaluator isleEvaluator,
        string targetState,
        ActionList actions,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(isleEvaluator);
      ValidateParam(actions);
    
      var component = new OnIsleStateExitTrigger();
      component.m_IsleEvaluator = isleEvaluator;
      component.m_TargetState = targetState;
      component.m_Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ActivateTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(ActivateTrigger))]
    public TBuilder AddActivateTrigger(
        ActionList actions,
        bool once = default,
        bool alsoOnAreaLoad = default,
        ConditionsBuilder conditions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new ActivateTrigger();
      component.m_Once = once;
      component.m_AlsoOnAreaLoad = alsoOnAreaLoad;
      component.Conditions = conditions?.Build() ?? Constants.Empty.Conditions;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="AreaDidLoadTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(AreaDidLoadTrigger))]
    public TBuilder AddAreaDidLoadTrigger(
        ActionList actions,
        ConditionsBuilder conditions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new AreaDidLoadTrigger();
      component.Actions = actions;
      component.Conditions = conditions?.Build() ?? Constants.Empty.Conditions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="CompanionRecruitTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="companionBlueprint"><see cref="BlueprintUnit"/></param>
    [Generated]
    [Implements(typeof(CompanionRecruitTrigger))]
    public TBuilder AddCompanionRecruitTrigger(
        ActionList actions,
        string companionBlueprint = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new CompanionRecruitTrigger();
      component.m_CompanionBlueprint = BlueprintTool.GetRef<BlueprintUnitReference>(companionBlueprint);
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="CompanionUnrecruitTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="companionBlueprint"><see cref="BlueprintUnit"/></param>
    [Generated]
    [Implements(typeof(CompanionUnrecruitTrigger))]
    public TBuilder AddCompanionUnrecruitTrigger(
        ActionList actions,
        string companionBlueprint = null,
        bool triggerOnDeath = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new CompanionUnrecruitTrigger();
      component.m_CompanionBlueprint = BlueprintTool.GetRef<BlueprintUnitReference>(companionBlueprint);
      component.TriggerOnDeath = triggerOnDeath;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="CustomEventTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(CustomEventTrigger))]
    public TBuilder AddCustomEventTrigger(
        string id,
        ActionList actions,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new CustomEventTrigger();
      component.Id = id;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DamageTypeTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(DamageTypeTrigger))]
    public TBuilder AddDamageTypeTrigger(
        UnitEvaluator unit,
        ActionList actions,
        bool anyDamageType = default,
        DamageEnergyType damageEType = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(unit);
      ValidateParam(actions);
    
      var component = new DamageTypeTrigger();
      component.Unit = unit;
      component.Actions = actions;
      component.AnyDamageType = anyDamageType;
      component.DamageEType = damageEType;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DeactivateTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(DeactivateTrigger))]
    public TBuilder AddDeactivateTrigger(
        ActionList actions,
        ConditionsBuilder conditions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new DeactivateTrigger();
      component.Conditions = conditions?.Build() ?? Constants.Empty.Conditions;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="DeviceInteractionTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(DeviceInteractionTrigger))]
    public TBuilder AddDeviceInteractionTrigger(
        ActionList actions,
        ActionList restrictedActions,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
      ValidateParam(restrictedActions);
    
      var component = new DeviceInteractionTrigger();
      component.Actions = actions;
      component.RestrictedActions = restrictedActions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="EvaluatedUnitCombatTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(EvaluatedUnitCombatTrigger))]
    public TBuilder AddEvaluatedUnitCombatTrigger(
        UnitEvaluator unit,
        ActionList actions,
        bool triggerOnExit = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(unit);
      ValidateParam(actions);
    
      var component = new EvaluatedUnitCombatTrigger();
      component.Unit = unit;
      component.Actions = actions;
      component.TriggerOnExit = triggerOnExit;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="EvaluatedUnitDeathTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(EvaluatedUnitDeathTrigger))]
    public TBuilder AddEvaluatedUnitDeathTrigger(
        UnitEvaluator unit,
        ActionList actions,
        bool anyUnit = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(unit);
      ValidateParam(actions);
    
      var component = new EvaluatedUnitDeathTrigger();
      component.AnyUnit = anyUnit;
      component.Unit = unit;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="EvaluatedUnitHealthTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(EvaluatedUnitHealthTrigger))]
    public TBuilder AddEvaluatedUnitHealthTrigger(
        UnitEvaluator unit,
        ActionList actions,
        bool once = default,
        int percentage = default,
        bool triggerOnAlreadyLowerHeath = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(unit);
      ValidateParam(actions);
    
      var component = new EvaluatedUnitHealthTrigger();
      component.Unit = unit;
      component.Once = once;
      component.Percentage = percentage;
      component.TriggerOnAlreadyLowerHeath = triggerOnAlreadyLowerHeath;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ExperienceTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(ExperienceTrigger))]
    public TBuilder AddExperienceTrigger(
        ActionList actions,
        int experience = default,
        ConditionsBuilder conditions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new ExperienceTrigger();
      component.Experience = experience;
      component.Conditions = conditions?.Build() ?? Constants.Empty.Conditions;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="GenericInteractionTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(GenericInteractionTrigger))]
    public TBuilder AddGenericInteractionTrigger(
        EntityReference mapObject,
        ActionList actions,
        ActionList restrictedActions,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(mapObject);
      ValidateParam(actions);
      ValidateParam(restrictedActions);
    
      var component = new GenericInteractionTrigger();
      component.MapObject = mapObject;
      component.Actions = actions;
      component.RestrictedActions = restrictedActions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ItemInContainerTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="itemToCheck"><see cref="BlueprintItem"/></param>
    [Generated]
    [Implements(typeof(ItemInContainerTrigger))]
    public TBuilder AddItemInContainerTrigger(
        MapObjectEvaluator mapObject,
        ActionList onAddActions,
        ActionList onRemoveActions,
        string itemToCheck = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(mapObject);
      ValidateParam(onAddActions);
      ValidateParam(onRemoveActions);
    
      var component = new ItemInContainerTrigger();
      component.m_ItemToCheck = BlueprintTool.GetRef<BlueprintItemReference>(itemToCheck);
      component.MapObject = mapObject;
      component.OnAddActions = onAddActions;
      component.OnRemoveActions = onRemoveActions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="MapObjectDestroyTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(MapObjectDestroyTrigger))]
    public TBuilder AddMapObjectDestroyTrigger(
        ActionList destroyedActions,
        ActionList destructionFailedActions,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(destroyedActions);
      ValidateParam(destructionFailedActions);
    
      var component = new MapObjectDestroyTrigger();
      component.DestroyedActions = destroyedActions;
      component.DestructionFailedActions = destructionFailedActions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="MapObjectPerceptionTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(MapObjectPerceptionTrigger))]
    public TBuilder AddMapObjectPerceptionTrigger(
        ActionList actions,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new MapObjectPerceptionTrigger();
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="PartyInventoryTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="item"><see cref="BlueprintItem"/></param>
    [Generated]
    [Implements(typeof(PartyInventoryTrigger))]
    public TBuilder AddPartyInventoryTrigger(
        ActionList onAddActions,
        ActionList onRemoveActions,
        string item = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(onAddActions);
      ValidateParam(onRemoveActions);
    
      var component = new PartyInventoryTrigger();
      component.m_Item = BlueprintTool.GetRef<BlueprintItemReference>(item);
      component.OnAddActions = onAddActions;
      component.OnRemoveActions = onRemoveActions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="PerceptionTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(PerceptionTrigger))]
    public TBuilder AddPerceptionTrigger(
        UnitEvaluator unit,
        MapObjectEvaluator mapObject,
        ActionList onSpotted,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(unit);
      ValidateParam(mapObject);
      ValidateParam(onSpotted);
    
      var component = new PerceptionTrigger();
      component.Unit = unit;
      component.MapObject = mapObject;
      component.OnSpotted = onSpotted;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="PlayerOpenItemDescriptionFirstTimeTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="item"><see cref="BlueprintItem"/></param>
    [Generated]
    [Implements(typeof(PlayerOpenItemDescriptionFirstTimeTrigger))]
    public TBuilder AddPlayerOpenItemDescriptionFirstTimeTrigger(
        ActionList action,
        string item = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(action);
    
      var component = new PlayerOpenItemDescriptionFirstTimeTrigger();
      component.m_Item = BlueprintTool.GetRef<BlueprintItemReference>(item);
      component.Action = action;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="RestTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(RestTrigger))]
    public TBuilder AddRestTrigger(
        ActionList actions,
        bool once = default,
        RestResult restResults = default,
        ConditionsBuilder conditions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new RestTrigger();
      component.Once = once;
      component.RestResults = restResults;
      component.Conditions = conditions?.Build() ?? Constants.Empty.Conditions;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="ScriptZoneTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(ScriptZoneTrigger))]
    public TBuilder AddScriptZoneTrigger(
        EntityReference scriptZone,
        string unitRef,
        ActionList onEnterActions,
        ActionList onExitActions,
        ConditionsBuilder onEnterConditions = null,
        ConditionsBuilder onExitConditions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(scriptZone);
      ValidateParam(onEnterActions);
      ValidateParam(onExitActions);
    
      var component = new ScriptZoneTrigger();
      component.ScriptZone = scriptZone;
      component.UnitRef = unitRef;
      component.OnEnterConditions = onEnterConditions?.Build() ?? Constants.Empty.Conditions;
      component.OnEnterActions = onEnterActions;
      component.OnExitConditions = onExitConditions?.Build() ?? Constants.Empty.Conditions;
      component.OnExitActions = onExitActions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="SkillCheckInteractionTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(SkillCheckInteractionTrigger))]
    public TBuilder AddSkillCheckInteractionTrigger(
        ActionList onSuccess,
        ActionList onFailure,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(onSuccess);
      ValidateParam(onFailure);
    
      var component = new SkillCheckInteractionTrigger();
      component.OnSuccess = onSuccess;
      component.OnFailure = onFailure;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="SpawnUnitTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="targetUnit"><see cref="BlueprintUnit"/></param>
    [Generated]
    [Implements(typeof(SpawnUnitTrigger))]
    public TBuilder AddSpawnUnitTrigger(
        ActionList actions,
        string targetUnit = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new SpawnUnitTrigger();
      component.m_TargetUnit = BlueprintTool.GetRef<BlueprintUnitReference>(targetUnit);
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="SpellCastTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="spells"><see cref="BlueprintAbility"/></param>
    [Generated]
    [Implements(typeof(SpellCastTrigger))]
    public TBuilder AddSpellCastTrigger(
        EntityReference scriptZone,
        ActionList actions,
        string[] spells = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(scriptZone);
      ValidateParam(actions);
    
      var component = new SpellCastTrigger();
      component.ScriptZone = scriptZone;
      component.m_Spells = spells.Select(name => BlueprintTool.GetRef<BlueprintAbilityReference>(name)).ToArray();
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="SummonPoolTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="summonPool"><see cref="BlueprintSummonPool"/></param>
    [Generated]
    [Implements(typeof(SummonPoolTrigger))]
    public TBuilder AddSummonPoolTrigger(
        ActionList actions,
        int count = default,
        SummonPoolTrigger.ChangeTypes changeType = default,
        string summonPool = null,
        ConditionsBuilder conditions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new SummonPoolTrigger();
      component.Count = count;
      component.ChangeType = changeType;
      component.m_SummonPool = BlueprintTool.GetRef<BlueprintSummonPoolReference>(summonPool);
      component.Conditions = conditions?.Build() ?? Constants.Empty.Conditions;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="TimeOfDayChangedTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(TimeOfDayChangedTrigger))]
    public TBuilder AddTimeOfDayChangedTrigger(
        ActionList actions,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new TimeOfDayChangedTrigger();
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="UIEventTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(UIEventTrigger))]
    public TBuilder AddUIEventTrigger(
        ActionList actions,
        UIEventType eventType = default,
        ConditionsBuilder conditions = null,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new UIEventTrigger();
      component.EventType = eventType;
      component.Conditions = conditions?.Build() ?? Constants.Empty.Conditions;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="UnitHealthTrigger"/> (Auto Generated)
    /// </summary>
    ///
    /// <param name="unit"><see cref="BlueprintUnit"/></param>
    [Generated]
    [Implements(typeof(UnitHealthTrigger))]
    public TBuilder AddUnitHealthTrigger(
        ActionList actions,
        string unit = null,
        int percentage = default,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(actions);
    
      var component = new UnitHealthTrigger();
      component.m_Unit = BlueprintTool.GetRef<BlueprintUnitReference>(unit);
      component.Percentage = percentage;
      component.Actions = actions;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    /// <summary>
    /// Adds <see cref="TrapTrigger"/> (Auto Generated)
    /// </summary>
    [Generated]
    [Implements(typeof(TrapTrigger))]
    public TBuilder AddTrapTrigger(
        MapObjectEvaluator trap,
        ActionList onActivation,
        ActionList onDisarm,
        BlueprintComponent.Flags flags = default)
    {
      ValidateParam(trap);
      ValidateParam(onActivation);
      ValidateParam(onDisarm);
    
      var component = new TrapTrigger();
      component.Trap = trap;
      component.OnActivation = onActivation;
      component.OnDisarm = onDisarm;
      component.m_Flags = flags;
      return AddComponent(component);
    }

    //----- Start: Configure & Validate

    /// <summary>Type specific configuration implemented in child classes.</summary>
    /// 
    /// <remarks>Components are added to the blueprint after this step.</remarks>
    protected virtual void ConfigureInternal() { }

    /// <summary>Type specific validation implemented in child classes.</summary>
    /// 
    /// <remarks>Implementations should report errors using <see cref="AddValidationWarning(string)"/>.</remarks>
    protected virtual void ValidateInternal() { }

    protected void AddValidationWarning(string msg) { ValidationWarnings.AppendLine(msg); }

    protected void ValidateParam(object obj) { Validator.Check(obj).ForEach(AddValidationWarning); }

    protected void ValidateParam<T>(IEnumerable<T> objects)
    {
      if (objects is null) { return; }
      foreach (var obj in objects) { ValidateParam(obj); }
    }

    private void ConfigureBase()
    {
      ConfigurePrerequisiteAlignment();
      ConfigureSpellDescriptors();
    }

    private void OnConfigure()
    {
      InternalOnConfigure.ForEach(action => action.Invoke(Blueprint));
      ExternalOnConfigure.ForEach(action => action.Invoke(Blueprint));
    }

    private void AddComponents()
    {
      foreach (UniqueComponent component in UniqueComponents)
      {
        var current = Blueprint.GetComponentMatchingType(component.Component);
        if (current == null)
        {
          Components.Add(component.Component);
          continue;
        }
        switch (component.Behavior)
        {
          case ComponentMerge.Skip:
            break;
          case ComponentMerge.Replace:
            ComponentsToRemove.Add(current);
            Components.Add(component.Component);
            break;
          case ComponentMerge.Merge:
            component.Merge(current, component.Component);
            break;
          case ComponentMerge.Fail:
          default:
            throw new InvalidOperationException($"Failed merging components of type {current.GetType()}");
        }
      }

      Blueprint.Components = Blueprint.Components.Except(ComponentsToRemove).ToArray();
      Blueprint.AddComponents(Components.ToArray());
    }

    private void ValidateBase()
    {
      var validationContext = new ValidationContext();
      Blueprint.Validate(validationContext);
      foreach (var error in validationContext.Errors) { AddValidationWarning(error); }

      ValidateComponents();
    }

    private void ConfigurePrerequisiteAlignment()
    {
      var component = Blueprint.GetComponent<PrerequisiteAlignment>();
      if (component == null)
      {
        // Don't create a component to remove prerequisite alignments
        if (EnablePrerequisiteAlignment == 0) { return; }

        component = new PrerequisiteAlignment();
        AddComponent(component);
      }
      component.Alignment |= EnablePrerequisiteAlignment;
      component.Alignment &= ~DisablePrerequisiteAlignment;
    }

    private void ConfigureSpellDescriptors()
    {
      var component = Blueprint.GetComponent<SpellDescriptorComponent>();
      if (component == null)
      {
        // Don't create a component to remove descriptors
        if (EnableSpellDescriptors == 0) { return; }

        component = new SpellDescriptorComponent();
        AddComponent(component);
      }
      component.Descriptor.m_IntValue |= EnableSpellDescriptors;
      component.Descriptor.m_IntValue &= ~DisableSpellDescriptors;
    }

    // TODO: Refactor validation to rely on Validator. That way it can be used externally.
    /// <summary>
    /// Validates each <see cref="BlueprintComponent"/> using its own validation, attributes, and custom logic.
    /// </summary>
    private void ValidateComponents()
    {
      if (Blueprint.Components == null) { return; }
      var componentTypes = new HashSet<Type>();
      foreach (BlueprintComponent component in Blueprint.Components)
      {
        component.ApplyValidation(Context);

        var componentType = component.GetType();
        Attribute[] attrs = Attribute.GetCustomAttributes(componentType);

        if (componentTypes.Contains(componentType)
            && !attrs.Where(attr => attr is AllowMultipleComponentsAttribute).Any())
        {
          AddValidationWarning($"Multiple {componentType} present but only one is allowed.");
        }
        else { componentTypes.Add(componentType); }

        List<AllowedOnAttribute> allowedOn =
            attrs
                .Where(attr => attr is AllowedOnAttribute)
                .Select(attr => attr as AllowedOnAttribute)
                .ToList();
        bool componentAllowed = false;
        var blueprintType = Blueprint.GetType();
        foreach (AllowedOnAttribute attr in allowedOn)
        {
          // Need .NET 5.0 for IsAssignableTo()
          if (IsAssignableTo(blueprintType, attr.Type))
          {
            componentAllowed = true;
            break;
          }
        }

        if (allowedOn.Count > 0 && !componentAllowed)
        {
          AddValidationWarning($"Component of {componentType} not allowed on {blueprintType}");
        }
      }

      // TODO: Unit test this
      // Make sure there are no conflicting ContextRankConfigs
      var duplicateRankTypes =
          Blueprint.GetComponents<ContextRankConfig>()
              .Select(config => config.m_Type)
              .GroupBy(type => type)
              .Where(group => group.Count() > 1)
              .Select(group => group.Key);
      if (duplicateRankTypes.Any())
      {
        AddValidationWarning(
            $"Duplicate ContextRankConfig.m_Type values found. Only one of each type is used: {string.Join(",", duplicateRankTypes)}");
      }

      Context.Errors.ToList().ForEach(str => AddValidationWarning(str));
    }

    private static bool IsAssignableTo(Type child, Type parent)
    {
      return child == parent || child.IsSubclassOf(parent);
    }

    private struct UniqueComponent
    {
      public BlueprintComponent Component { get; }
      public ComponentMerge Behavior { get; }
      public Action<BlueprintComponent, BlueprintComponent> Merge { get; }

      public UniqueComponent(
          BlueprintComponent component,
          ComponentMerge behavior,
          Action<BlueprintComponent, BlueprintComponent> merge)
      {
        Component = component;
        Behavior = behavior;
        Merge = merge;
      }
    }
  }

  /// <summary>Configurator for any blueprint inheriting from <see cref="BlueprintScriptableObject"/>.</summary>
  /// 
  /// <remarks>
  /// <para>
  /// Prefer using the explicit configurator implementations wherever available.
  /// </para>
  /// 
  /// <para>
  /// BlueprintConfigurator is useful for types not supported by the library. Because it is generically typed it will
  /// not expose functions for all supported component types or functions for fields. Instead you can use
  /// <see cref="BaseBlueprintConfigurator{T, TBuilder}.AddComponent">AddComponent</see>,
  /// <see cref="BaseBlueprintConfigurator{T, TBuilder}.AddUniqueComponent">AddUniqueComponent</see>,
  /// and <see cref="BaseBlueprintConfigurator{T, TBuilder}.OnConfigure">OnConfigure</see>. This enables the
  /// configurator API without a specific type implementation and ensures your blueprints are validated.
  /// </para>
  /// 
  /// <example>
  /// <code>
  /// BlueprintConfigurator&lt;BlueprintDlc>.New(DlcGuid)
  ///     .OnConfigure(dlc => dlc.Description = LocalizedDlcDescription)
  ///     .Configure();
  /// </code>
  /// </example>
  /// </remarks>
  [Configures(typeof(BlueprintScriptableObject))]
  public class BlueprintConfigurator<T> : BaseBlueprintConfigurator<T, BlueprintConfigurator<T>>
      where T : BlueprintScriptableObject, new()
  {
    private BlueprintConfigurator(string name) : base(name) { }

    /// <inheritdoc cref="Buffs.BuffConfigurator.For(string)"/>
    public static BlueprintConfigurator<T> For(string name)
    {
      return new BlueprintConfigurator<T>(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string)"/>
    public static BlueprintConfigurator<T> New(string name)
    {
      BlueprintTool.Create<T>(name);
      return For(name);
    }

    /// <inheritdoc cref="Buffs.BuffConfigurator.New(string, string)"/>
    public static BlueprintConfigurator<T> New(string name, string assetId)
    {
      BlueprintTool.Create<T>(name, assetId);
      return For(name);
    }
  }
}
