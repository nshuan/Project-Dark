using System;

namespace Dark.Tools.GoogleSheetTool
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ConfigNodeLogicTypeAttribute : Attribute
    {
        public LogicType LogicType { get; } 
        public ConfigNodeLogicTypeAttribute(LogicType value) => LogicType = value;
    }
}