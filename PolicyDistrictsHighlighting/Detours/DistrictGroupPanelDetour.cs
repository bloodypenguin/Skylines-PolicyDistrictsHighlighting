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
            this.CreateGroupItem(new GeneratedGroupPanel.GroupInfo("DistrictSpecializationPaint", this.GetCategoryOrder(this.name), "District"), "DISTRICT_CATEGORY");
            this.CreateGroupItem((GeneratedGroupPanel.GroupInfo)new DistrictGroupPanel.PTGroupInfo("DistrictSpecializationIndustrial", this.GetCategoryOrder(this.name), UnlockManager.Feature.IndustrySpecializations, "District"), "DISTRICT_CATEGORY");
            if (Singleton<DistrictManager>.instance.IsPolicyLoaded(DistrictPolicies.Policies.Leisure))
                this.CreateGroupItem((GeneratedGroupPanel.GroupInfo)new DistrictGroupPanel.PTGroupInfo("DistrictSpecializationCommercial", this.GetCategoryOrder(this.name), UnlockManager.Feature.CommercialSpecialization, "District"), "DISTRICT_CATEGORY");
            //begin mod
            SetupEvents("Paint", DistrictPolicies.Policies.None);
            SetupEvents("Erase", DistrictPolicies.Policies.None);
            SetupEvents("SpecializationForest", DistrictPolicies.Policies.Forest);
            SetupEvents("SpecializationFarming", DistrictPolicies.Policies.Farming);
            SetupEvents("SpecializationOil", DistrictPolicies.Policies.Oil);
            SetupEvents("SpecializationOre", DistrictPolicies.Policies.Ore);
            SetupEvents("SpecializationNone", DistrictPolicies.Policies.None);
            SetupEvents("SpecializationTourist", DistrictPolicies.Policies.Tourist);
            SetupEvents("SpecializationLeisure", DistrictPolicies.Policies.Leisure);
            SetupEvents("SpecializationCommercialNone", DistrictPolicies.Policies.None);
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