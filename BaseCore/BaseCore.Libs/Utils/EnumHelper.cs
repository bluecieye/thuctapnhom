

using System;
using System.ComponentModel;                        
using System.ComponentModel.DataAnnotations;        
using System.Linq;

namespace BaseCore.Libs.Utils
{

    // ════════════════════════════════════════════════════════════
    // HELPER ENUM (EXTENSION ATTRIBUTE)
    // ════════════════════════════════════════════════════════════

    public static class EnumHelper
    {
        #region Public Method

        

        

        

        
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();                                  
            var memberInfo = type.GetMember(value.ToString());            
            var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);  
            return (T)attributes.FirstOrDefault();                        

        }

        

        
        
        public static string ToDisplayName(this Enum value)
        {
            var attribute = value.GetAttribute<DisplayAttribute>();
            return attribute == null ? value.ToString() : attribute.Name;
        }

        

        
        
        public static string ToDescription(this Enum value)
        {
            var attribute = value.GetAttribute<DescriptionAttribute>();
            return attribute == null ? value.ToString() : attribute.Description;
        }

        #endregion
    }
}
