using System;

namespace Dark.Tools.GoogleSheetTool
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConfigNodeLogicTypeV2Attribute : Attribute
    {
        public NodeBonusTypeV2 LogicType { get; } 
        public ConfigNodeLogicTypeV2Attribute(NodeBonusTypeV2 value) => LogicType = value;
    }
}