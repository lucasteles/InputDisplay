#nullable disable
using InputDisplay.Inputs;
using InputDisplay.Themes;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace InputDisplay.Config.Screen;

public sealed class SettingsControls(Desktop desktop, SettingsManager configManager) : IDisposable
{
    public Label SelectedJoystick { get; private set; }

    public Button ResetMapButton { get; private set; }
    public Image[] Directions { get; private set; } = new Image[9];
    public Dictionary<ButtonName, Button> Buttons { get; private set; } = new();

    public ButtonName? MappingButton { get; private set; }
    public Window CurrentModal { get; private set; }

    Theme theme;

    static readonly Color darkGray = new(50, 50, 50);

    public Widget BuildUI()
    {
        theme = ThemeManager.Get();
        VerticalStackPanel root = new()
        {
            Padding = new(20),
        };

        root.Widgets.Add(BuildSelectedController());
        root.Widgets.Add(Line());
        root.Widgets.Add(BuildInputMap());
        root.Widgets.Add(Line());

        root.Widgets.Add(BuildSettings());

        return root;
    }

    Widget BuildSettings()
    {
        Grid grid = new();

        return grid;
    }

    Widget BuildInputMap()
    {
        VerticalStackPanel root = new()
        {
            Margin = new(0, 20),
        };


        Label title = new()
        {
            Text = "Input Mapping:",
            TextColor = Color.White,
            Padding = new(4),
        };
        root.Widgets.Add(title);


        HorizontalStackPanel mappings = new()
        {
            Margin = new(0, 5),
        };
        root.Widgets.Add(mappings);

        var dirGrid = new Grid
        {
            RowSpacing = 8,
            ColumnSpacing = 8,
            Margin = new(30),
            VerticalAlignment = VerticalAlignment.Center,
        };
        mappings.Widgets.Add(dirGrid);

        dirGrid.ColumnsProportions.Add(new(ProportionType.Auto));
        dirGrid.ColumnsProportions.Add(new(ProportionType.Auto));
        dirGrid.ColumnsProportions.Add(new(ProportionType.Auto));
        dirGrid.RowsProportions.Add(new(ProportionType.Auto));
        dirGrid.RowsProportions.Add(new(ProportionType.Auto));
        dirGrid.RowsProportions.Add(new(ProportionType.Auto));

        InitNumpadDirection(1, (3, 1), dirGrid);
        InitNumpadDirection(2, (3, 2), dirGrid);
        InitNumpadDirection(3, (3, 3), dirGrid);
        InitNumpadDirection(4, (2, 1), dirGrid);
        InitNumpadDirection(5, (2, 2), dirGrid);
        InitNumpadDirection(6, (2, 3), dirGrid);
        InitNumpadDirection(7, (1, 1), dirGrid);
        InitNumpadDirection(8, (1, 2), dirGrid);
        InitNumpadDirection(9, (1, 3), dirGrid);

        var buttonsGrid = new Grid
        {
            RowSpacing = 12,
            ColumnSpacing = 12,
            Margin = new(30),
            VerticalAlignment = VerticalAlignment.Center,
        };
        mappings.Widgets.Add(buttonsGrid);
        buttonsGrid.ColumnsProportions.Add(new(ProportionType.Auto));
        buttonsGrid.ColumnsProportions.Add(new(ProportionType.Auto));
        buttonsGrid.RowsProportions.Add(new(ProportionType.Auto));
        buttonsGrid.RowsProportions.Add(new(ProportionType.Auto));

        InitButton(ButtonName.LP, (1, 1), buttonsGrid);
        InitButton(ButtonName.MP, (1, 2), buttonsGrid);
        InitButton(ButtonName.HP, (1, 3), buttonsGrid);
        InitButton(ButtonName.PP, (1, 4), buttonsGrid);

        InitButton(ButtonName.LK, (2, 1), buttonsGrid);
        InitButton(ButtonName.MK, (2, 2), buttonsGrid);
        InitButton(ButtonName.HK, (2, 3), buttonsGrid);
        InitButton(ButtonName.KK, (2, 4), buttonsGrid);

        ResetMapButton = new()
        {
            Padding = new(10),
            Margin = new(30, 10),
            Content = new Label
            {
                Text = "Reset",
            },
        };

        root.Widgets.Add(ResetMapButton);

        return root;
    }

    void InitButton(ButtonName buttonName, (int Row, int Collumn) pos, Grid grid)
    {
        Button button = new()
        {
            Width = 55,
            Height = 55,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Tag = buttonName,
            Content =
                new Label
                {
                    Padding = new(10),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextColor = Color.White,
                    Text = buttonName.ToString(),
                }
        };

        button.Click += OnButtonMapClick;

        Buttons.Add(buttonName, button);

        Grid.SetColumn(button, pos.Collumn - 1);
        Grid.SetRow(button, pos.Row - 1);
        grid.Widgets.Add(button);
    }

    void OnButtonMapClick(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var name = (ButtonName)button.Tag;

        MappingButton = name;
        CurrentModal = BuildButtonMapModal($"Mapping {name}");
        CurrentModal.ShowModal(desktop);
    }

    public void ButtonMapped()
    {
        CurrentModal?.Close();
        MappingButton = null;
        CurrentModal = null;
    }

    void InitNumpadDirection(int numpad, (int Row, int Collumn) pos, Grid grid)
    {
        var dir = NumpadNotation.ToDirection(numpad);
        var texture = theme.GetTexture(dir);
        var index = numpad - 1;

        Directions[index] = new()
        {
            Renderable = new TextureRegion(texture),
            Width = 40,
            Height = 40,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Color = darkGray,
        };

        Grid.SetColumn(Directions[index], pos.Collumn - 1);
        Grid.SetRow(Directions[index], pos.Row - 1);
        grid.Widgets.Add(Directions[index]);
    }

    Widget Line() =>
        new Panel
        {
            BorderThickness = new(0, 1),
            Border = new SolidBrush(Color.White),
            Margin = new(0, 5),
        };

    Widget BuildSelectedController()
    {
        HorizontalStackPanel root = new();

        Label labelJoyStick = new()
        {
            Text = "Selected Controller: ",
            TextColor = Color.White,
            Padding = new(2),
        };

        SelectedJoystick = new()
        {
            Background = new SolidBrush(Color.DarkSlateGray),
            Padding = new(2),
            MinWidth = 200,
        };

        root.Widgets.Add(labelJoyStick);
        root.Widgets.Add(SelectedJoystick);

        return root;
    }

    public void HighLightDirection(Direction dir)
    {
        var index = NumpadNotation.From(dir) - 1;
        for (var i = 0; i < Directions.Length; i++)
            Directions[i].Color = index == i ? Color.White : darkGray;
    }

    public void HighLightButtons(ButtonName buttons)
    {
        foreach (var (name, button) in Buttons)
            if (button.Content is Label label)
                label.Background = buttons.HasFlag(name)
                    ? new SolidBrush(Color.Green)
                    : (IBrush)null;
    }

    public void Dispose()
    {
        foreach (var btn in Buttons.Values)
            btn.Click -= OnButtonMapClick;
    }

    Window BuildButtonMapModal(string title)
    {
        Label label = new()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Text = "Press any button...",
            Margin = new(20),
        };

        var buttonMapModal = Dialog.CreateMessageBox(title, label);
        buttonMapModal.ButtonOk.Visible = false;
        buttonMapModal.Padding = new(10);
        return buttonMapModal;
    }
}
