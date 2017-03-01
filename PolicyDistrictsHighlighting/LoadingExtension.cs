using ColossalFramework.UI;
using ICities;

namespace PolicyDistrictsHighlighting
{
    public class LoadingExtension : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            PoliciesPanelDetour.Deploy();
            DistrictGroupPanelDetour.Deploy();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if (mode != LoadMode.NewGame && mode != LoadMode.LoadGame)
            {
                return;
            }
            var districtsButton = UIView.Find<UIButton>("Districts");
            districtsButton.eventClicked += (sender, p) => { DistrictManager.instance.HighlightPolicy = DistrictPolicies.Policies.None; };
            var policiesPanel = UIView.Find<UIPanel>("PoliciesPanel");
            policiesPanel.eventVisibilityChanged += (sender, p) => { if (p) { DistrictManager.instance.HighlightPolicy = DistrictPolicies.Policies.None; } };
        }

        public override void OnReleased()
        {
            DistrictGroupPanelDetour.Revert();
            PoliciesPanelDetour.Revert();
        }
    }
}