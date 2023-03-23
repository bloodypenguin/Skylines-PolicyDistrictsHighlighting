using System.Reflection;
using ColossalFramework;
using ColossalFramework.UI;
using PolicyDistrictsHighlighting.Redirection;

namespace PolicyDistrictsHighlighting
{
    [TargetType(typeof(DistrictGroupPanel))]
    public class DistrictGroupPanelDetour : GeneratedGroupPanel
    {
        private static RedirectCallsState _state;
        private static MethodInfo _originalInfo;
        private static MethodInfo _detourInfo = typeof(DistrictGroupPanelDetour).GetMethod("CustomRefreshPanel", BindingFlags.NonPublic | BindingFlags.Instance);
        private static bool _deployed;

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            var tuple = RedirectionUtil.RedirectMethod(typeof(DistrictGroupPanel), _detourInfo);
            _originalInfo = tuple.First;
            _state = tuple.Second;
            _deployed = true;
        }

        public static void Revert()
        {
            if (!_deployed) return;
            if (_originalInfo != null && _detourInfo != null)
            {
                RedirectionHelper.RevertRedirect(_originalInfo, _state);
            }
            _deployed = false;
        }

        [RedirectMethod]
        protected override bool CustomRefreshPanel()
        {
            bool flag1 = SteamHelper.IsDLCOwned(SteamHelper.DLC.AfterDarkDLC);
            bool flag2 = SteamHelper.IsDLCOwned(SteamHelper.DLC.GreenCitiesDLC);
            bool flag3 = SteamHelper.IsDLCOwned(SteamHelper.DLC.PlazasAndPromenadesDLC);
            bool flag4 = SteamHelper.IsDLCOwned(SteamHelper.DLC.FinancialDistrictsDLC);
            this.CreateGroupItem(new GeneratedGroupPanel.GroupInfo("DistrictSpecializationPaint", this.GetCategoryOrder("DistrictSpecializationPaint"), "District"), "DISTRICT_CATEGORY");
            this.CreateGroupItem((GeneratedGroupPanel.GroupInfo) new DistrictGroupPanel.PTGroupInfo("DistrictSpecializationIndustrial", this.GetCategoryOrder("DistrictSpecializationIndustrial"), UnlockManager.Feature.IndustrySpecializations, "District"), "DISTRICT_CATEGORY");
            if (flag2 || flag1 || flag3)
            {
              UnlockManager.Feature feature = !flag2 ? (!flag1 ? (!flag3 ? UnlockManager.Feature.CommercialSpecialization : UnlockManager.Feature.WallToWallSpecializations) : UnlockManager.Feature.CommercialSpecialization) : UnlockManager.Feature.CommercialSpecializationGC;
              this.CreateGroupItem((GeneratedGroupPanel.GroupInfo) new DistrictGroupPanel.PTGroupInfo("DistrictSpecializationCommercial", this.GetCategoryOrder("DistrictSpecializationCommercial"), feature, "District"), "DISTRICT_CATEGORY");
            }
            if (flag2 || flag3 || flag4)
              this.CreateGroupItem((GeneratedGroupPanel.GroupInfo) new DistrictGroupPanel.PTGroupInfo("DistrictSpecializationOffice", this.GetCategoryOrder("DistrictSpecializationOffice"), UnlockManager.Feature.OfficeSpecializations, "District"), "DISTRICT_CATEGORY");
            if (flag2 || flag3)
            {
              UnlockManager.Feature feature = !flag2 ? (!flag3 ? UnlockManager.Feature.ResidentialSpecializations : UnlockManager.Feature.WallToWallSpecializations) : UnlockManager.Feature.ResidentialSpecializations;
              this.CreateGroupItem((GeneratedGroupPanel.GroupInfo) new DistrictGroupPanel.PTGroupInfo("DistrictSpecializationResidential", this.GetCategoryOrder("DistrictSpecializationResidential"), feature, "District"), "DISTRICT_CATEGORY");
            }
            //begin mod
            SetupEvents("Paint", DistrictPolicies.Policies.None);
            SetupEvents("Erase", DistrictPolicies.Policies.None);
            SetupEvents("SpecializationForest", DistrictPolicies.Policies.Forest);
            SetupEvents("SpecializationFarming", DistrictPolicies.Policies.Farming);
            SetupEvents("SpecializationOil", DistrictPolicies.Policies.Oil);
            SetupEvents("SpecializationOre", DistrictPolicies.Policies.Ore);
            SetupEvents("SpecializationNone", DistrictPolicies.Policies.None);
            if (flag1)
            {
                SetupEvents("SpecializationTourist", DistrictPolicies.Policies.Tourist);
                SetupEvents("SpecializationLeisure", DistrictPolicies.Policies.Leisure);
            }
            if (flag1 || flag2)
            {
                SetupEvents("SpecializationCommercialNone", DistrictPolicies.Policies.None);
            }
            if (flag2)
            {
                SetupEvents("SpecializationOrganic", DistrictPolicies.Policies.Organic);
                SetupEvents("SpecializationSelfsufficient", DistrictPolicies.Policies.Selfsufficient);
                SetupEvents("SpecializationResidentialNone", DistrictPolicies.Policies.None);
                SetupEvents("SpecializationHightech", DistrictPolicies.Policies.Hightech);
                SetupEvents("SpecializationOfficeNone", DistrictPolicies.Policies.None);
            }
            //end mod
            return true;
        }

        private void SetupEvents(string buttonName, DistrictPolicies.Policies policy)
        {
            var uiButton = this.Find<UIButton>(buttonName);
            if (uiButton != null)
            {
                uiButton.eventClicked +=
                    (sender, p) => { DistrictManager.instance.HighlightPolicy = policy; };
            }
        }
    }
}