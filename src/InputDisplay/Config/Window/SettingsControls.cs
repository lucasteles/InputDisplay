#nullable disable
using InputDisplay.Inputs;
using InputDisplay.Themes;
using Myra.Graphics2D;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.ColorPicker;

namespace InputDisplay.Config.Window;

public sealed class SettingsControls(Desktop desktop, SettingsManager configManager) : IDisposable
{
    public Label SelectedJoystick { get; private set; }

    public ComboView ControllerTypeCombo = new();
    public Button ResetMapButton { get; private set; }
    public Image[] Directions { get; private set; } = new Image[9];
    public Dictionary<ButtonName, Button> Buttons { get; private set; } = new();
    public Dictionary<ButtonName, Button> Macros { get; private set; } = new();

    public ButtonName? MappingButton { get; private set; }
    public ButtonName? MappingMacro { get; private set; }
    public Myra.Graphics2D.UI.Window CurrentModal { get; private set; }

    Theme defaultTheme;

    static readonly Color darkGray = new(50, 50, 50);

    public Widget BuildUI()
    {
        defaultTheme = ThemeManager.Get();
        VerticalStackPanel root = new()
        {
            Padding = new(10),
        };

        root.Widgets.Add(BuildSelectedController());
        root.Widgets.Add(Line());
        root.Widgets.Add(BuildThemes());
        root.Widgets.Add(Line());
        root.Widgets.Add(BuildInputMap());
        root.Widgets.Add(Line());
        root.Widgets.Add(BuildMacroMap());
        root.Widgets.Add(Line());
        root.Widgets.Add(BuildSettings());

        return root;
    }

    readonly Direction[] allDirections =
    [
        Direction.Neutral,
        Direction.Backward,
        Direction.DownBackward,
        Direction.Down,
        Direction.DownForward,
        Direction.Forward,
        Direction.UpForward,
        Direction.Up,
        Direction.UpBackward,
    ];

    readonly ButtonName[] allButtonNames =
    [
        ButtonName.LP,
        ButtonName.MP,
        ButtonName.HP,
        ButtonName.PP,
        ButtonName.LK,
        ButtonName.MK,
        ButtonName.HK,
        ButtonName.KK,
    ];

    PlayerPad player;

    public void SetPlayer(PlayerPad pad)
    {
        SelectedJoystick.Text = pad.Name;

        var currentType = configManager.CurrentConfig.InputMap.GetPadKind(pad);
        ControllerTypeCombo.SelectedIndex = ThemeConfig.ControllerTypes.Keys.ToArray().IndexOf(currentType);
        player = pad;
    }

    void RebuildMacroButtons()
    {
        foreach (var buttonName in allButtonNames)
        {
            HorizontalStackPanel content = new()
            {
                Padding = new(2),
                Spacing = 5,
                MaxWidth = 300,
            };
            content.Widgets.Add(new Label
            {
                Text = buttonName.ToString(),
                Margin = new(0, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            });

            var config = configManager.CurrentConfig;
            var theme = ThemeManager.Get(config.CurrentTheme);
            var macros = theme.GetMacro(buttonName, config.Macros);
            if (macros.Length is 0)
                content.Widgets.Add(new Image
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Renderable = new TextureRegion(ThemeManager.UnknownButton),
                    Width = 30,
                    Height = 30,
                });
            else
                foreach (var part in macros)
                {
                    var texture = theme.GetTexture(part) ?? ThemeManager.UnknownButton;
                    content.Widgets.Add(new Image
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Renderable = new TextureRegion(texture),
                        Width = 30,
                        Height = 30,
                    });
                }

            if (Macros.TryGetValue(buttonName, out var button))
                button.Content = content;
            else
            {
                Button newButton = new()
                {
                    MinWidth = 150,
                    Padding = new(10, 5),
                    Content = content,
                };

                Macros.Add(buttonName, newButton);

                newButton.Click += (_, _) =>
                {
                    var dialog = CreateMacroDialog(buttonName);
                    dialog.ShowModal(desktop);
                };
            }
        }
    }

    Dialog CreateMacroDialog(ButtonName buttonName)
    {
        VerticalStackPanel panel = new()
        {
            Spacing = 15,
        };
        MappingMacro = buttonName;
        var config = configManager.CurrentConfig;
        var theme = ThemeManager.Get(config.CurrentTheme);
        var macros = theme.GetMacro(buttonName, config.Macros);
        var selected = macros.ToList();

        foreach (var b in allButtonNames)
        {
            var texture = theme.GetTexture(b);
            if (texture is null) continue;

            Image icon = new()
            {
                VerticalAlignment = VerticalAlignment.Center,
                Renderable = new TextureRegion(texture),
                Width = 30,
                Height = 30,
            };

            CheckButton check = new()
            {
                VerticalAlignment = VerticalAlignment.Center,
                CheckContentSpacing = 15,
                IsChecked = selected.Contains(b),
                Content = icon,
            };

            check.IsCheckedChanged += (_, _) =>
            {
                if (check.IsChecked && !selected.Contains(b))
                    selected.Add(b);

                else if (!check.IsChecked && selected.Contains(b))
                    selected.Remove(b);
            };

            panel.Widgets.Add(check);
        }

        var dialog = Dialog.CreateMessageBox("Select Icons", panel);
        dialog.Closed += (_, _) =>
        {
            MappingMacro = null;
            if (!dialog.Result) return;

            if (config.Macros.ContainsKey(buttonName))
                config.Macros[buttonName] = [.. selected];
            else
                config.Macros.Add(buttonName, [.. selected]);

            selected.Clear();
            SaveConfig();
            RebuildMacroButtons();
        };

        dialog.Padding = new(20);
        dialog.MinWidth = 200;
        return dialog;
    }

    Widget BuildMacroMap()
    {
        var root = new Grid
        {
            RowSpacing = 4,
            ColumnSpacing = 8,
            Margin = new(20, 5),
            VerticalAlignment = VerticalAlignment.Center,
        };

        root.ColumnsProportions.Add(new(ProportionType.Auto));
        root.ColumnsProportions.Add(new(ProportionType.Auto));
        root.ColumnsProportions.Add(new(ProportionType.Auto));
        root.RowsProportions.Add(new(ProportionType.Auto));
        root.RowsProportions.Add(new(ProportionType.Auto));


        Label title = new()
        {
            Text = "Macros:",
            TextColor = Color.White,
            Padding = new(4),
        };
        Grid.SetColumn(title, 0);
        Grid.SetRow(title, 0);
        root.Widgets.Add(title);

        Button resetMapButton = new()
        {
            Padding = new(5),
            Margin = new(30, 0),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Content = new Label
            {
                Text = "Reset",
            },
        };
        resetMapButton.Click += (_, _) =>
        {
            configManager.CurrentConfig.Macros.Clear();
            SaveConfig();
            RebuildMacroButtons();
        };

        Grid.SetColumn(resetMapButton, 3);
        Grid.SetRow(resetMapButton, 0);
        root.Widgets.Add(resetMapButton);

        RebuildMacroButtons();

        SetMacroButton(ButtonName.LP, (1, 0));
        SetMacroButton(ButtonName.MP, (1, 1));
        SetMacroButton(ButtonName.HP, (1, 2));
        SetMacroButton(ButtonName.PP, (1, 3));
        SetMacroButton(ButtonName.LK, (2, 0));
        SetMacroButton(ButtonName.MK, (2, 1));
        SetMacroButton(ButtonName.HK, (2, 2));
        SetMacroButton(ButtonName.KK, (2, 3));

        return root;

        void SetMacroButton(ButtonName name, (int Row, int Col) pos)
        {
            var button = Macros[name];
            Grid.SetColumn(button, pos.Col);
            Grid.SetRow(button, pos.Row);
            root.Widgets.Add(button);
        }
    }


    Widget BuildThemes()
    {
        Grid grid = new()
        {
            RowSpacing = 2,
            ColumnSpacing = 4,
            Margin = new(20, 10),
            VerticalAlignment = VerticalAlignment.Center,
        };
        grid.ColumnsProportions.Add(new(ProportionType.Auto));
        grid.ColumnsProportions.Add(new(ProportionType.Auto));
        grid.RowsProportions.Add(new(ProportionType.Auto));
        grid.RowsProportions.Add(new(ProportionType.Auto));

        Label dirLabel = new()
        {
            Text = "Direction theme:",
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new(0, 0, 10, 0),
        };
        Grid.SetColumn(dirLabel, 0);
        Grid.SetRow(dirLabel, 0);
        grid.Widgets.Add(dirLabel);

        ComboView dirCombo = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new(5),
        };
        Grid.SetColumn(dirCombo, 1);
        Grid.SetRow(dirCombo, 0);
        grid.Widgets.Add(dirCombo);
        var iconSize = 30;

        foreach (var (dirThemeName, dirTheme) in ThemeConfig.DirectionMap)
        {
            HorizontalStackPanel item = new()
            {
                Padding = new(4),
                Spacing = 2,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Label name = new()
            {
                Text = dirThemeName,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new(0, 0, 10, 0),
            };
            item.Widgets.Add(name);
            item.Tag = dirThemeName;

            foreach (var d in allDirections)
            {
                var texture = ThemeManager.GetTexture(dirTheme.GetTexturePath(d));
                Image icon = new()
                {
                    Width = iconSize,
                    Height = iconSize,
                    Renderable = new TextureRegion(texture),
                    VerticalAlignment = VerticalAlignment.Bottom,
                };
                item.Widgets.Add(icon);
            }

            dirCombo.Widgets.Add(item);

            if (dirThemeName == configManager.CurrentConfig.CurrentTheme.Direction)
                dirCombo.SelectedItem = item;
        }

        dirCombo.SelectedIndexChanged += (_, _) =>
        {
            var newTheme = (string)dirCombo.SelectedItem.Tag;

            var config = configManager.CurrentConfig;

            config.CurrentTheme = config.CurrentTheme with
            {
                Direction = newTheme,
            };
            SaveConfig();
        };


        // buttons
        Label btnLabal = new()
        {
            Text = "Buttons theme:",
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new(0, 0, 10, 0),
        };
        Grid.SetColumn(btnLabal, 0);
        Grid.SetRow(btnLabal, 1);
        grid.Widgets.Add(btnLabal);

        ComboView btnCombo = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new(5),
        };
        Grid.SetColumn(btnCombo, 1);
        Grid.SetRow(btnCombo, 1);
        grid.Widgets.Add(btnCombo);

        foreach (var (btnThemeName, btnTheme) in ThemeConfig.ButtonMap)
        {
            HorizontalStackPanel item = new()
            {
                Padding = new(4),
                Spacing = 2,
                VerticalAlignment = VerticalAlignment.Center,
            };

            Label name = new()
            {
                Text = btnThemeName,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new(0, 0, 10, 0),
            };
            item.Widgets.Add(name);
            item.Tag = btnThemeName;

            foreach (var b in allButtonNames)
            {
                if (btnTheme.GetTexturePath(b) is not { } path)
                    continue;

                var texture = ThemeManager.GetTexture(path);
                Image icon = new()
                {
                    Width = iconSize,
                    Height = iconSize,
                    Renderable = new TextureRegion(texture),
                    VerticalAlignment = VerticalAlignment.Bottom,
                };
                item.Widgets.Add(icon);
            }

            btnCombo.Widgets.Add(item);

            if (btnThemeName == configManager.CurrentConfig.CurrentTheme.Buttons)
                btnCombo.SelectedItem = item;
        }

        btnCombo.SelectedIndexChanged += (_, _) =>
        {
            var newTheme = (string)btnCombo.SelectedItem.Tag;

            var config = configManager.CurrentConfig;

            config.CurrentTheme = config.CurrentTheme with
            {
                Buttons = newTheme,
            };
            SaveConfig();
            RebuildMacroButtons();
        };

        return grid;
    }

    Widget BuildSettings()
    {
        Grid grid = new()
        {
            RowSpacing = 2,
            ColumnSpacing = 4,
            Margin = new(20),
            VerticalAlignment = VerticalAlignment.Center,
        };

        var config = configManager.CurrentConfig;

        AddCheck(0, 0, "Borderless", config.Borderless, check => config.Borderless = check);
        AddCheck(0, 1, "Show frames", config.ShowFrames, check => config.ShowFrames = check);
        AddCheck(0, 2, "Show neutral", config.ShowNeutralIcon, check => config.ShowNeutralIcon = check);
        AddCheck(0, 3, "Shadow holding", config.ShadowHolding, check => config.ShadowHolding = check);
        AddNumeric(0, 4, "Icon size: ", config.IconSize, v => config.IconSize = v);

        AddCheck(1, 0, "Auto correct", config.AutoCorrectMultiple, check => config.AutoCorrectMultiple = check);
        AddCheck(1, 1, "Invert history", config.InvertHistory, check => config.InvertHistory = check);
        AddCheck(1, 2, "Frames after", config.FramesAfter, check => config.FramesAfter = check);
        AddCheck(1, 3, "Hide button release", config.HideButtonRelease, check => config.HideButtonRelease = check);
        AddCheck(1, 4, "Use Shortcuts", config.ShortcutsEnabled, check => config.ShortcutsEnabled = check);

        AddNumeric(2, 0, "Input space: ", config.SpaceBetweenInputs, v => config.SpaceBetweenInputs = v);
        AddNumeric(2, 1, "Command space: ", config.SpaceBetweenCommands, v => config.SpaceBetweenCommands = v);
        AddNumeric(2, 2, "Direction space: ", config.DirectionSpace, v => config.DirectionSpace = v);

        var backgroundColor = ColorPicker("Background color: ", config.ClearColor, c =>
        {
            config.ClearColor = c;
            SaveConfig();
        });
        Grid.SetColumn(backgroundColor, 2);
        Grid.SetRow(backgroundColor, 3);
        grid.Widgets.Add(backgroundColor);

        return grid;

        void AddCheck(int col, int row, string labelText, bool isChecked, Action<bool> onClick)
        {
            var chk = InputCheck(labelText, isChecked, onClick);
            Grid.SetColumn(chk, col);
            Grid.SetRow(chk, row);
            grid.Widgets.Add(chk);
        }

        void AddNumeric(int col, int row, string labelText, int value, Action<int> onChange)
        {
            var chk = InputNumber(labelText, value, onChange);
            Grid.SetColumn(chk, col);
            Grid.SetRow(chk, row);
            grid.Widgets.Add(chk);
        }
    }

    HorizontalStackPanel InputCheck(string labelText, bool isChecked, Action<bool> onClick)
    {
        HorizontalStackPanel panel = new()
        {
            Padding = new(2),
        };

        Label label = new()
        {
            Text = labelText,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new(0, 0, 10, 0),
        };

        panel.Widgets.Add(label);

        CheckButton button = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            IsChecked = isChecked,
        };

        button.IsCheckedChanged += (sender, _) =>
        {
            var input = (CheckButton)sender;
            onClick(input!.IsChecked);
            SaveConfig();
        };

        panel.Widgets.Add(button);
        return panel;
    }

    HorizontalStackPanel InputNumber(string labelText, int value, Action<int> onChange)
    {
        HorizontalStackPanel panel = new()
        {
            Padding = new(4),
        };

        Label label = new()
        {
            Text = labelText,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new(0, 0, 10, 0),
        };

        panel.Widgets.Add(label);

        SpinButton textBox = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Integer = true,
            Width = 50,
            Value = value,
            Padding = new(2),
        };

        textBox.Minimum = 1;
        textBox.ValueChanged += (sender, _) =>
        {
            var input = (SpinButton)sender;
            var v = int.Max((int)(input?.Value ?? 0), 1);
            onChange(v);
            SaveConfig();
        };

        panel.Widgets.Add(textBox);
        return panel;
    }

    HorizontalStackPanel ColorPicker(string labelText, Color color, Action<Color> onChange)
    {
        HorizontalStackPanel panel = new()
        {
            Padding = new(4),
        };

        Label label = new()
        {
            Text = labelText,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new(0, 0, 10, 0),
        };

        panel.Widgets.Add(label);


        Button button = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new(2),
            Content = new Image
            {
                Width = 50,
                Height = 30,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidBrush(color),
                Tag = color,
            },
        };


        button.Click += (sender, _) =>
        {
            if (sender is not Button { Content: Image { Tag: Color current } image })
                return;

            ColorPickerDialog dialog = new()
            {
                Color = current,
            };

            dialog.Closed += (s, a) =>
            {
                if (!dialog.Result) return;
                image.Tag = dialog.Color;
                image.Background = new SolidBrush(dialog.Color);
                onChange(dialog.Color);
            };

            dialog.ShowModal(desktop);
        };

        panel.Widgets.Add(button);
        return panel;
    }


    VerticalStackPanel BuildInputMap()
    {
        VerticalStackPanel root = new()
        {
            Margin = new(0, 2),
        };

        Panel header = new();
        root.Widgets.Add(header);

        Label title = new()
        {
            Text = "Button Mapping:",
            TextColor = Color.White,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new(4),
        };

        ResetMapButton = new()
        {
            Padding = new(4),
            Margin = new(30, 0),
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
            Content = new Label
            {
                Text = "Reset",
            },
        };
        header.Widgets.Add(title);
        header.Widgets.Add(ResetMapButton);

        HorizontalStackPanel mappings = new()
        {
            Margin = new(0, 4),
        };
        root.Widgets.Add(mappings);

        var dirGrid = new Grid
        {
            RowSpacing = 8,
            ColumnSpacing = 8,
            Margin = new(20, 10),
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
            RowSpacing = 8,
            ColumnSpacing = 8,
            Margin = new(30, 5),
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

        return root;
    }

    void InitButton(ButtonName buttonName, (int Row, int Collumn) pos, Grid grid)
    {
        Button button = new()
        {
            Width = 45,
            Height = 45,
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
        var texture = defaultTheme.GetTexture(dir);
        var index = numpad - 1;

        Directions[index] = new()
        {
            Renderable = new TextureRegion(texture),
            Width = 30,
            Height = 30,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Color = darkGray,
        };

        Grid.SetColumn(Directions[index], pos.Collumn - 1);
        Grid.SetRow(Directions[index], pos.Row - 1);
        grid.Widgets.Add(Directions[index]);
    }

    static Widget Line() =>
        new Panel
        {
            BorderThickness = new(0, 1),
            Border = new SolidBrush(Color.White),
            Margin = new(0, 5),
        };

    Widget BuildSelectedController()
    {
        Panel root = new();

        HorizontalStackPanel left = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        root.Widgets.Add(left);

        Label labelJoyStick = new()
        {
            Text = "Selected Controller: ",
            TextColor = Color.White,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new(5),
        };

        SelectedJoystick = new()
        {
            Background = new SolidBrush(Color.DarkSlateGray),
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new(5),
            MinWidth = 300,
        };

        Panel header = new();
        left.Widgets.Add(header);
        left.Widgets.Add(labelJoyStick);
        left.Widgets.Add(SelectedJoystick);


        HorizontalStackPanel right = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        root.Widgets.Add(right);

        Label labelType = new()
        {
            Text = "Type: ",
            TextColor = Color.White,
            VerticalAlignment = VerticalAlignment.Center,
            Padding = new(5),
        };
        right.Widgets.Add(labelType);

        ControllerTypeCombo = new()
        {
            VerticalAlignment = VerticalAlignment.Center,
        };
        right.Widgets.Add(ControllerTypeCombo);
        foreach (var (typeName, text) in ThemeConfig.ControllerTypes)
            ControllerTypeCombo.Widgets.Add(new Label
            {
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new(5),
                Text = text,
                Tag = typeName,
            });

        ControllerTypeCombo.SelectedIndexChanged += OnChangeControllerType;
        return root;
    }

    void OnChangeControllerType(object sender, EventArgs e)
    {
        if (sender is not ListView { SelectedItem: Label { Tag: PlayerPad.Kind kind } }
            || player is null || configManager.CurrentConfig.InputMap.GetMapping(player) is not { } mapping)
            return;

        mapping.Kind = kind;
        SaveConfig();
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

    Myra.Graphics2D.UI.Window BuildButtonMapModal(string title)
    {
        Label label = new()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Text = "Press any button...",
            Margin = new(10),
        };

        var buttonMapModal = Dialog.CreateMessageBox(title, label);
        buttonMapModal.ButtonOk.Visible = false;
        buttonMapModal.Padding = new(10);
        return buttonMapModal;
    }

    public void SaveConfig() => configManager.SaveFile();

    public void Dispose()
    {
        foreach (var btn in Buttons.Values)
            btn.Click -= OnButtonMapClick;

        ControllerTypeCombo.SelectedIndexChanged -= OnChangeControllerType;
    }
}
