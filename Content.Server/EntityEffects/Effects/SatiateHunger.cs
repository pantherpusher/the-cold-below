using Content.Server._Coyote.Needs;
using Content.Server.Nutrition.Components;
using Content.Shared._Coyote.Needs;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects;
using Content.Shared.Nutrition.Components;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Prototypes;

namespace Content.Server.EntityEffects.Effects
{
    /// <summary>
    /// Attempts to find a NeedsComponent that supports Hunger on the target,
    /// and to update it's hunger values.
    /// </summary>
    public sealed partial class SatiateHunger : EntityEffect
    {
        private const float DefaultNutritionFactor = 3.0f;

        /// <summary>
        ///     How much hunger is satiated.
        ///     Is multiplied by quantity if used with EntityEffectReagentArgs.
        /// </summary>
        [DataField("factor")] public float NutritionFactor { get; set; } = DefaultNutritionFactor;

        //Remove reagent at set rate, satiate hunger if a NeedsComponent that supports Hunger can be found
        public override void Effect(EntityEffectBaseArgs args)
        {
            var entman = args.EntityManager;
            if (!entman.TryGetComponent(args.TargetEntity, out NeedsComponent? needy))
                return;
            if (!entman.System<SharedNeedsSystem>().UsesHunger(args.TargetEntity, needy))
                return;
            if (args is EntityEffectReagentArgs reagentArgs)
            {
                entman.System<SharedNeedsSystem>()
                    .ModifyHunger(
                        reagentArgs.TargetEntity,
                        NutritionFactor * (float)reagentArgs.Quantity,
                        needy);
            }
            else
            {
                entman.System<SharedNeedsSystem>()
                    .ModifyHunger(
                        args.TargetEntity,
                        NutritionFactor,
                        needy);
            }
        }

        protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
            => Loc.GetString("reagent-effect-guidebook-satiate-hunger", ("chance", Probability), ("relative", NutritionFactor / DefaultNutritionFactor));
    }
}
