using DungeonSlime.Scenes;
using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;
using Gum.Forms;
using Gum.Forms.Controls;
using MonoGameGum;

namespace DungeonSlime;

public class Game1 : Core
{
    // The background theme song.
    private Song _themeSong;

    public Game1() : base("Dungeon Slime", 1280, 720, false)
    {

    }

    private void InitializeGum()
    {
        GumService.Default.Initialize(this, DefaultVisualsVersion.V2);

        // Tell the Gum servis which content manager to use.
        GumService.Default.ContentLoader.XnaContentManager = Core.Content;
        FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);
        FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);
        FrameworkElement.TabReverseKeyCombos.Add(new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Down });

        GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
        GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
        GumService.Default.Renderer.Camera.Zoom = 4.0f;
    }



    protected override void Initialize()
    {
        base.Initialize();

        // Start playing the background music.
        Audio.PlaySong(_themeSong);

        InitializeGum();

        // Start the game with the title scene.
        ChangeScene(new TitleScene());
    }

    protected override void LoadContent()
    {
        // Load the background theme music.
        _themeSong = Content.Load<Song>("audio/theme");
    }




}
