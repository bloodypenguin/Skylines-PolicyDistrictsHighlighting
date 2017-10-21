using System;
using System.Reflection;
using ColossalFramework.DataBinding;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using PolicyDistrictsHighlighting.Redirection;
using UnityEngine;

namespace PolicyDistrictsHighlighting
{
    [TargetType(typeof(PoliciesPanel))]
    public class PoliciesPanelDetour : PoliciesPanel
    {
        private static readonly string kPolicyTemplate = "PolicyTemplate";
        private static FieldInfo _objectIndexField = typeof (PoliciesPanel).GetField("m_ObjectIndex",
            BindingFlags.NonPublic | BindingFlags.Instance);
        private static PropertyInfo _policiesTooltipPropertyInfo = typeof(PoliciesPanel).GetProperty("policiesTooltip",
            BindingFlags.NonPublic | BindingFlags.Instance);
        private static PropertyInfo _defaultTooltipPropertyInfo = typeof(PoliciesPanel).GetProperty("defaultTooltip",
            BindingFlags.NonPublic | BindingFlags.Instance);

        private static RedirectCallsState _state;
        private static MethodInfo _originalInfo;
        private static MethodInfo _detourInfo = typeof(PoliciesPanelDetour).GetMethod("SpawnPolicyEntry", BindingFlags.NonPublic | BindingFlags.Instance);
        private static bool _deployed;

        public static void Deploy()
        {
            if (_deployed)
            {
                return;
            }
            var tuple = RedirectionUtil.RedirectMethod(typeof(PoliciesPanel), _detourInfo);
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
        private void SpawnPolicyEntry(UIComponent container, string name, string unlockText, bool isEnabled)
        {
            UIPanel uiPanel;
            var objectIndex = (int)_objectIndexField.GetValue(this);
            if (container.childCount > objectIndex)
            {
                uiPanel = container.components[objectIndex] as UIPanel;
            }
            else
            {
                GameObject asGameObject = UITemplateManager.GetAsGameObject(kPolicyTemplate);
                asGameObject.name = name;
                uiPanel = container.AttachUIComponent(asGameObject) as UIPanel;
            }
            uiPanel.FitTo(uiPanel.parent, LayoutDirection.Horizontal);
            uiPanel.stringUserData = name;
            uiPanel.objectUserData = (object)this;
            uiPanel.isEnabled = isEnabled;
            UIButton uiButton = uiPanel.Find<UIButton>("PolicyButton");
            uiButton.text = ColossalFramework.Globalization.Locale.Get("POLICIES", name);
            string str = "IconPolicy" + name;
            uiButton.pivot = this.m_DockingPosition != PoliciesPanel.DockingPosition.Left ? UIPivotPoint.TopLeft : UIPivotPoint.TopRight;
            if (isEnabled)
            {
                uiButton.tooltipBox = (UIComponent)_policiesTooltipPropertyInfo.GetValue(this, null);
                uiButton.tooltip = TooltipHelper.Format("title", ColossalFramework.Globalization.Locale.Get("POLICIES", name), "text", ColossalFramework.Globalization.Locale.Get("POLICIES_DETAIL", name));
            }
            else
            {
                uiButton.tooltipBox = (UIComponent)_defaultTooltipPropertyInfo.GetValue(this, null);
                uiButton.tooltip = ColossalFramework.Globalization.Locale.Get("POLICIES", name) + " - " + unlockText;
            }
            //begin mod
            uiButton.eventTooltipEnter += (sender, p) =>
            {
                DistrictManager.instance.HighlightPolicy = (DistrictPolicies.Policies)Enum.Parse(typeof(DistrictPolicies.Policies), name);
            };
            uiButton.eventTooltipLeave += (sender, p) =>
            {
                DistrictManager.instance.HighlightPolicy = DistrictPolicies.Policies.None;
            };
            //end mod
            uiButton.normalFgSprite = str;
            uiButton.disabledFgSprite = str + "Disabled";
            _objectIndexField.SetValue(this, objectIndex + 1);
        }
    }
}