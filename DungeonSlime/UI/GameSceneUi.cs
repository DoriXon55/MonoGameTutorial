using System;
using Gum.DataTypes;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime.UI;

public class GameSceneUI : ContainerRuntime
{
    private static readonly string s_scoreFOrmat = "SCORE: {0:D6}";
    private SoundEffect _uiSoundEffect;
    private Panel _pausePanel;
    private AnimatedButton _resumeButton;
    private Panel _gameOverPanel;
    private AnimatedButton _retryButton;
    private TextRuntime _scoreText;

    public event EventHandler ResumeButtonClick;

    public event EventHandler QuitButtonClick;

    public event EventHandler RetryButtonClick;

    public GameSceneUI(){
        Dock(Gum.Wireframe.Dock.Fill);
        this.AddToRoot();

        ContentManager content = GumService.Default.ContentLoader.XnaContentManager;

        _uiSoundEffect = content.Load<SoundEffect>("audio/ui");
        TextureAtlas atlas = TextureAtlas.FromFile(content, "images/atlas-definition.xml");

        _scoreText = CreateScoreText();
        AddChild(_scoreText);

        _pausePanel = CreatePausePanel(atlas);
        AddChild(_pausePanel.Visual);

        _gameOverPanel = CreateGameOverPanel(atlas);
        AddChild(_gameOverPanel.Visual);
    }

    private TextRuntime CreateScoreText(){
        TextRuntime text = new TextRuntime();
        text.Anchor(Gum.Wireframe.Anchor.TopLeft);
        text.WidthUnits = DimensionUnitType.RelativeToChildren;
        text.X = 20.0f;
        text.Y = 5.0f;
        text.UseCustomFont = true;
        text.CustomFontFile = @"fonts/04b_30.fnt";
        text.FontScale = 0.25f;
        text.Text = string.Format(s_scoreFormat, 0);
        
        return text;
    }

    private Panel CreatePausePanel(TextureAtlas atlas)
    {
        Panel panel = new Panel();
        panel.Anchor(Gum.Wireframe.Anchor.Center);
        panel.Visual.WidthUnits = DimensionUnitType.Absolute;
        panel.Visual.HeightUnits = DimensionUnitType.Absolute;
        panel.Visual.Width = 264.0f;
        panel.Visual.Height = 70.0f;
        panel.IsVisible = false;

        TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

        NineSliceRuntime background = new NineSliceRuntime();
        background.Dock(Gum.Wireframe.Dock.Fill);
        background.Texture = backgroundRegion.Texture;
        background.TextureAddress = TextureAddress.Custom;
        background.TextureHeight = backgroundRegion.Height;
        background.TextureWidth = backgroundRegion.Width;
        background.TextureTop = backgroundRegion.SourceRectangle.Top;
        background.TextureLeft = backgroundRegion.SourceRectangle.Left;
        panel.AddChild(background);

        TextRuntime text = new TextRuntime();
        text.Text = "PAUSED";
        text.UseCustomFont = true;
        text.CustomFontFile = "fonts/04b_30.fnt";
        text.FontScale = 0.5f;
        text.X = 10.0f;
        text.Y = 10.0f;
        panel.AddChild(text);

        _resumeButton = new AnimatedButton(atlas);
        _resumeButton.Text = "RESUME";
        _resumeButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        _resumeButton.Visual.X = 9.0f;
        _resumeButton.Visual.Y = -9.0f;

        _resumeButton.Click += OnResumeButtonClicked;
        _resumeButton.GotFocus += OnElementGotFocus;

        panel.AddChild(_resumeButton);

        AnimatedButton quitButton = new AnimatedButton(atlas);
        quitButton.Text = "QUIT";
        quitButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        quitButton.Visual.X = -9.0f;
        quitButton.Visual.Y = -9.0f;

        quitButton.Click += OnQuitButtonClicked;
        quitButton.GotFocus += OnElementGotFocus;

        panel.AddChild(quitButton);

        return panel;
    }



    private Panel CreateGameOverPanel(TextureAtlas atlas)
    {
        Panel panel = new Panel();
        panel.Anchor(Gum.Wireframe.Anchor.Center);
        panel.Visual.WidthUnits = DimensionUnitType.Absolute;
        panel.Visual.HeightUnits = DimensionUnitType.Absolute;
        panel.Visual.Width = 264.0f;
        panel.Visual.Height = 70.0f;
        panel.IsVisible = false;

        TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

        NineSliceRuntime background = new NineSliceRuntime();
        background.Dock(Gum.Wireframe.Dock.Fill);
        background.Texture = backgroundRegion.Texture;
        background.TextureAddress = TextureAddress.Custom;
        background.TextureHeight = backgroundRegion.Height;
        background.TextureWidth = backgroundRegion.Width;
        background.TextureTop = backgroundRegion.SourceRectangle.Top;
        background.TextureLeft = backgroundRegion.SourceRectangle.Left;
        panel.AddChild(background);

        TextRuntime text = new TextRuntime();
        text.Text = "GAME OVER";
        text.WidthUnits = DimensionUnitType.RelativeToChildren;
        text.UseCustomFont = true;
        text.CustomFontFile = "fonts/04b_30.fnt";
        text.FontScale = 0.5f;
        text.X = 10.0f;
        text.Y = 10.0f;
        panel.AddChild(text);

        _retryButton = new AnimatedButton(atlas);
        _retryButton.Text = "RETRY";
        _retryButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
        _retryButton.Visual.X = 9.0f;
        _retryButton.Visual.Y = -9.0f;

        _retryButton.Click += OnRetryButtonClicked;
        _retryButton.GotFocus += OnElementGotFocus;

        panel.AddChild(_retryButton);

        AnimatedButton quitButton = new AnimatedButton(atlas);
        quitButton.Text = "QUIT";
        quitButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
        quitButton.Visual.X = -9.0f;
        quitButton.Visual.Y = -9.0f;

        quitButton.Click += OnQuitButtonClicked;
        quitButton.GotFocus += OnElementGotFocus;

        panel.AddChild(quitButton);

        return panel;
    }


    private void OnResumeButtonClicked(object sender, EventArgs args)
    {
        // Button was clicked, play the ui sound effect for auditory feedback.
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Since the resume button was clicked, we need to hide the pause panel.
        HidePausePanel();

        // Invoke the ResumeButtonClick event
        if(ResumeButtonClick != null)
        {
            ResumeButtonClick(sender, args);
        }
    }

    private void OnRetryButtonClicked(object sender, EventArgs args)
    {
        // Button was clicked, play the ui sound effect for auditory feedback.
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Since the retry button was clicked, we need to hide the game over panel.
        HideGameOverPanel();

        // Invoke the RetryButtonClick event.
        if(RetryButtonClick != null)
        {
            RetryButtonClick(sender, args);
        }
    }

    private void OnQuitButtonClicked(object sender, EventArgs args)
    {
        // Button was clicked, play the ui sound effect for auditory feedback.
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Both panels have a quit button, so hide both panels
        HidePausePanel();
        HideGameOverPanel();

        // Invoke the QuitButtonClick event.
        if(QuitButtonClick != null)
        {
            QuitButtonClick(sender, args);
        }
    }

    private void OnElementGotFocus(object sender, EventArgs args)
    {
        // A ui element that can receive focus has received focus, play the
        // ui sound effect for auditory feedback.
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
    }

    /// <summary>
    /// Updates the text on the score display.
    /// </summary>
    /// <param name="score">The score to display.</param>
    public void UpdateScoreText(int score)
    {
        _scoreText.Text = string.Format(s_scoreFormat, score);
    }

    /// <summary>
    /// Tells the game scene ui to show the pause panel.
    /// </summary>
    public void ShowPausePanel()
    {
        _pausePanel.IsVisible = true;

        // Give the resume button focus for keyboard/gamepad input.
        _resumeButton.IsFocused = true;

        // Ensure the game over panel isn't visible.
        _gameOverPanel.IsVisible = false;
    }

    /// <summary>
    /// Tells the game scene ui to hide the pause panel.
    /// </summary>
    public void HidePausePanel()
    {
        _pausePanel.IsVisible = false;
    }

    /// <summary>
    /// Tells the game scene ui to show the game over panel.
    /// </summary>
    public void ShowGameOverPanel()
    {
        _gameOverPanel.IsVisible = true;

        // Give the retry button focus for keyboard/gamepad input.
        _retryButton.IsFocused =true;

        // Ensure the pause panel isn't visible.
        _pausePanel.IsVisible = false;
    }

    /// <summary>
    /// Tells the game scene ui to hide the game over panel.
    /// </summary>
    public void HideGameOverPanel()
    {
        _gameOverPanel.IsVisible = false;
    }

    /// <summary>
    /// Updates the game scene ui.
    /// </summary>
    /// <param name="gameTime">A snapshot of the timing values for the current update cycle.</param>
    public void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);
    }

    /// <summary>
    /// Draws the game scene ui.
    /// </summary>
    public void Draw()
    {
        GumService.Default.Draw();
    }


}
