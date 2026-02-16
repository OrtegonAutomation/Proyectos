using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using FifoCleanup.Engine.Models;

namespace FifoCleanup.UI.Converters;

/// <summary>Convierte StorageLevel a un SolidColorBrush para indicadores</summary>
public class StorageLevelToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is StorageLevel level)
        {
            return level switch
            {
                StorageLevel.Green => new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                StorageLevel.Yellow => new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                StorageLevel.Red => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
                _ => new SolidColorBrush(Colors.Gray)
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Convierte porcentaje de uso a color</summary>
public class UsagePercentToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double percent)
        {
            return percent switch
            {
                < 70 => new SolidColorBrush(Color.FromRgb(39, 174, 96)),
                < 85 => new SolidColorBrush(Color.FromRgb(243, 156, 18)),
                _ => new SolidColorBrush(Color.FromRgb(231, 76, 60))
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Convierte bytes a formato legible</summary>
public class BytesToSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        long bytes = value switch
        {
            long l => l,
            double d => (long)d,
            int i => i,
            _ => 0
        };

        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Convierte bool a Visibility</summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool invert = parameter?.ToString() == "Invert";
        bool isVisible = value is bool b && b;
        if (invert) isVisible = !isVisible;
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility v && v == Visibility.Visible;
}

/// <summary>Convierte double? a texto o "N/A"</summary>
public class NullableDoubleToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            string format = parameter?.ToString() ?? "F1";
            return d.ToString(format, culture);
        }
        return "N/A";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>Convierte BitacoraEventType a texto en español</summary>
public class EventTypeToSpanishConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is BitacoraEventType type)
        {
            return type switch
            {
                BitacoraEventType.Inventory => "Inventario",
                BitacoraEventType.CleanupManual => "Limpieza Manual",
                BitacoraEventType.CleanupScheduled => "Limpieza Programada",
                BitacoraEventType.CleanupPreventive => "Limpieza Preventiva",
                BitacoraEventType.Simulation => "Simulación",
                BitacoraEventType.Configuration => "Configuración",
                BitacoraEventType.Alarm => "Alarma",
                BitacoraEventType.Error => "Error",
                BitacoraEventType.SystemStart => "Inicio del Sistema",
                BitacoraEventType.SystemStop => "Detención del Sistema",
                BitacoraEventType.FileDetected => "Archivo Detectado",
                _ => type.ToString()
            };
        }
        return value?.ToString() ?? "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
