using Microsoft.Maui.Devices.Sensors;

namespace MyMauiApp;

public partial class MainPage : ContentPage
{
    private bool _isRunning;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        StopSensor();
    }

    private void OnToggleClicked(object sender, EventArgs e)
    {
        if (!Accelerometer.Default.IsSupported)
        {
            StatusLabel.Text = "Устройство не поддерживает акселерометр";
            ToggleButton.IsEnabled = false;
            return;
        }

        if (_isRunning)
        {
            StopSensor();
        }
        else
        {
            StartSensor();
        }
    }

    private void StartSensor()
    {
        Accelerometer.Default.ReadingChanged += OnReadingChanged;
        Accelerometer.Default.Start(SensorSpeed.UI);
        _isRunning = true;
        ToggleButton.Text = "Стоп";
        StatusLabel.Text = "Измерение...";
        HintLabel.Text = "Положите телефон на поверхность экраном вверх.";
    }

    private void StopSensor()
    {
        if (!_isRunning) return;

        Accelerometer.Default.Stop();
        Accelerometer.Default.ReadingChanged -= OnReadingChanged;
        _isRunning = false;
        ToggleButton.Text = "Старт";
        StatusLabel.Text = "Остановлено";
    }

    private void OnReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        var a = e.Reading.Acceleration;

        RawLabel.Text = $"X: {a.X:F2}   Y: {a.Y:F2}   Z: {a.Z:F2}";

        double angleX = Math.Atan2(a.X, Math.Sqrt(a.Y * a.Y + a.Z * a.Z)) * 180.0 / Math.PI;
        double angleY = Math.Atan2(a.Y, Math.Sqrt(a.X * a.X + a.Z * a.Z)) * 180.0 / Math.PI;

        AngleXLabel.Text = $"{angleX:F1}°";
        AngleYLabel.Text = $"{angleY:F1}°";

        double tilt = Math.Max(Math.Abs(angleX), Math.Abs(angleY));
        AngleLabel.Text = $"{tilt:F1}°";

        if (tilt < 2.0)
        {
            AngleLabel.TextColor = Colors.Green;
            StatusLabel.Text = "Горизонтально ✓";
        }
        else if (tilt < 5.0)
        {
            AngleLabel.TextColor = Colors.Orange;
            StatusLabel.Text = "Почти горизонтально";
        }
        else
        {
            AngleLabel.TextColor = Colors.Red;
            StatusLabel.Text = "Отклонение";
        }
    }
}