// SPDX-FileCopyrightText: 2024 Julian Giebel <juliangiebel@live.de>
//
// SPDX-License-Identifier: MIT

using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;

namespace Content.Client.MassMedia.Ui;

[GenerateTypedNameReferences]
public sealed partial class NewsArticleCard : Control
{
    private string? _authorMarkup;
    private TimeSpan? _publicationTime;

    public Action? OnDeletePressed;
    public int ArtcileNumber;

    public string? Title
    {
        get => TitleLabel.Text;
        set => TitleLabel.Text = value?.Length <= 30 ? value : $"{value?[..30]}...";
    }

    public string? Author
    {
        get => _authorMarkup;
        set
        {
            _authorMarkup = value;
            AuthorLabel.Text = _authorMarkup ?? "";
        }
    }

    public TimeSpan? PublicationTime
    {
        get => _publicationTime;
        set
        {
            _publicationTime = value;
            PublishTimeLabel.Text = value?.ToString(@"hh\:mm\:ss") ?? "";
        }
    }

    public NewsArticleCard()
    {
        RobustXamlLoader.Load(this);
        DeleteButton.OnPressed += _ => OnDeletePressed?.Invoke();
    }
}