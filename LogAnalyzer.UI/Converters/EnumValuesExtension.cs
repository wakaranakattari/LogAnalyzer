using System;
using System.Linq;
using System.Windows.Markup;

namespace LogAnalyzer.UI.Converters;

/// <summary>
/// Markup extension that returns all values of an enum for XAML binding.
/// </summary>
public class EnumValuesExtension : MarkupExtension
{
    private Type _enumType;

    /// <summary>
    /// Initializes a new instance of the EnumValuesExtension class.
    /// </summary>
    /// <param name="enumType">The enum type to get values from.</param>
    public EnumValuesExtension(Type enumType)
    {
        _enumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
    }

    /// <inheritdoc/>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (!_enumType.IsEnum)
            throw new InvalidOperationException($"Type {_enumType} is not an enum");

        return Enum.GetValues(_enumType).Cast<Enum>().ToList();
    }
}