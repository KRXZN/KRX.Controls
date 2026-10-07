using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Leaf.Controls.Converters
{
    /// <summary>
    /// 枚举显示转换器：按 "Enum_{类型名}_{成员名}" 键查应用资源字典（支持多语言热切换），
    /// 查不到时退回 <see cref="DescriptionAttribute"/>，再退回成员名。
    /// 语言切换后需重新触发绑定（重开窗口/重选条目）才会取到新语言文本。
    /// </summary>
    public class EnumDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            var type = value.GetType();
            if (!type.IsEnum)
                return string.Empty;
            var name = Enum.GetName(type, value);
            if (name == null)
                return string.Empty;

            var localized = FindLocalized(type, name);
            if (localized is not null)
                return localized;

            var field = type.GetField(name);
            if (field == null)
                return string.Empty;
            var attrs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
            if (attrs.Length > 0)
            {
                var descAttr = (System.ComponentModel.DescriptionAttribute)attrs[0];
                return descAttr.Description;
            }
            return name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !targetType.IsEnum)
                return Binding.DoNothing;

            var description = value.ToString();
            if (string.IsNullOrEmpty(description))
                return Binding.DoNothing;

            // 遍历枚举的所有字段，查找匹配的Description
            foreach (var field in targetType.GetFields())
            {
                if (field.IsSpecialName)
                    continue;

                // 当前语言下的资源文本
                if (FindLocalized(targetType, field.Name) == description)
                {
                    return Enum.Parse(targetType, field.Name);
                }

                var attrs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                {
                    var descAttr = (DescriptionAttribute)attrs[0];
                    if (descAttr.Description == description)
                    {
                        return Enum.Parse(targetType, field.Name);
                    }
                }
                // 如果没有Description特性，则直接比较名称
                else if (field.Name == description)
                {
                    return Enum.Parse(targetType, field.Name);
                }
            }

            return Binding.DoNothing;
        }

        /// <summary>按 "Enum_{类型名}_{成员名}" 查应用资源字典；未定义该键时返回 null。</summary>
        private static string? FindLocalized(Type enumType, string fieldName)
        {
            var key = $"Enum_{enumType.Name}_{fieldName}";
            return Application.Current?.TryFindResource(key) as string;
        }
    }
}
