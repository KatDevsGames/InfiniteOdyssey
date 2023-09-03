using System.Collections.Generic;
using System.Linq;
using FmodForFoxes;
using InfiniteOdyssey.Extensions.Converters;
using InfiniteOdyssey.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace InfiniteOdyssey;

public class Game : Microsoft.Xna.Framework.Game
{
    private readonly INativeFmodLibrary m_nativeLibrary;
    private readonly GraphicsDeviceManager m_graphics;
    public SpriteBatch SpriteBatch { get; private set; }

    private InputMapper m_inputMapper = new();

    private readonly SceneManager m_sceneManager = new();

    public RenderTarget2D RenderTarget { get; private set; }
    private Rectangle m_renderDest;

    private readonly Point NATIVE_RESOLUTION = new(1280, 720);

#pragma warning disable CS8618 //nobody fucking cares, nerd - kat
    public Game(INativeFmodLibrary nativeLibrary)
#pragma warning restore CS8618
    {
        //FMOD
        m_nativeLibrary = nativeLibrary;

        //Graphics
        m_graphics = new GraphicsDeviceManager(this);
        int realX = Settings.DisplayWidth;
        int realY = Settings.DisplayHeight;
        m_graphics.PreferredBackBufferWidth = realX;
        m_graphics.PreferredBackBufferHeight = realY;
        m_renderDest = GetRenderTargetDestination(NATIVE_RESOLUTION, realX, realY);

        //Content Loader
        Content.RootDirectory = "Content";

        //Misc
        IsMouseVisible = true;

        //Scene Manager
        m_sceneManager.Add(new Title(this), "Title");
    }

    public IEnumerable<DisplayMode> GetValidResolutions() => GraphicsAdapter.DefaultAdapter.SupportedDisplayModes;

#if DESKTOP
    public void ChangeResolution(DisplayMode mode) => ChangeResolution(mode.Width, mode.Height, m_graphics.IsFullScreen);

    public void ChangeResolution(DisplayMode mode, bool fullScreen) => ChangeResolution(mode.Width, mode.Height, fullScreen);

    public void ChangeResolution(int width, int height)
    {
        bool fullScreen = m_graphics.IsFullScreen &&
                               GraphicsAdapter.DefaultAdapter.SupportedDisplayModes.Any(mode =>
                                   (mode.Width == width) && (mode.Height == height));
        ChangeResolution(width, height, fullScreen);
    }

    public void ChangeResolution(int width, int height, bool fullScreen)
    {
        m_graphics.PreferredBackBufferWidth = width;
        m_graphics.PreferredBackBufferHeight = height;
        m_graphics.IsFullScreen = fullScreen;
        m_graphics.ApplyChanges();
        m_renderDest = GetRenderTargetDestination(NATIVE_RESOLUTION, width, height);
    }
#endif

    protected override void Initialize()
    {
        JsonInitializer.Init();
        RenderTarget = new RenderTarget2D(m_graphics.GraphicsDevice, 1280, 720);

        m_sceneManager.Initalize();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        FmodManager.Init(m_nativeLibrary, FmodInitMode.CoreAndStudio, "Content");

        // load content here
        m_sceneManager.Load("Title");
    }

    protected override void UnloadContent()
    {
        FmodManager.Unload();
    }

    // I know Game.Components is a thing but I prefer the exact control of doing it like this - kat
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        FmodManager.Update();
        m_inputMapper.Update(gameTime);
        m_sceneManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(RenderTarget);
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin();
        // put drawing code here
        m_sceneManager.Draw(gameTime);
        SpriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin();
        SpriteBatch.Draw(RenderTarget, m_renderDest, Color.White);
        SpriteBatch.End();

        base.Draw(gameTime);
    }

    Rectangle GetRenderTargetDestination(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
    {
        float resolutionRatio = (float)resolution.X / resolution.Y;
        Point bounds = new(preferredBackBufferWidth, preferredBackBufferHeight);
        float screenRatio = (float)bounds.X / bounds.Y;
        float scale;
        Rectangle rectangle = new();

        if (resolutionRatio < screenRatio)
            scale = (float)bounds.Y / resolution.Y;
        else if (resolutionRatio > screenRatio)
            scale = (float)bounds.X / resolution.X;
        else
        {
            // Resolution and window/screen share aspect ratio
            rectangle.Size = bounds;
            return rectangle;
        }
        rectangle.Width = (int)(resolution.X * scale);
        rectangle.Height = (int)(resolution.Y * scale);
        return CenterRectangle(new Rectangle(Point.Zero, bounds), rectangle);
    }

    static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
    {
        Point delta = outerRectangle.Center - innerRectangle.Center;
        innerRectangle.Offset(delta);
        return innerRectangle;
    }
}