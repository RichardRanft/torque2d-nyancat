//-----------------------------------------------------------------------------
// Copyright (c) 2013 Roostertail Games
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to
// deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
// sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//-----------------------------------------------------------------------------

function NyanCat::create( %this )
{    
    // Load the preferences.
    %this.loadPreferences();
    
    // Load NyanCat scripts.
    exec( "./scripts/console.cs" );
    exec( "./scripts/toolbox.cs" );    
    exec( "./scripts/customToolboxGui.cs" );
    exec( "./scripts/manipulation.cs" );
    exec( "./scripts/scene.cs" );
    exec( "./scripts/toys.cs" );
    exec( "./scripts/gui/mainMenuGui.cs" );
    exec( "./scripts/behaviors/nyanCatBehavior.cs" );
    exec( "./scripts/worldLimits.cs" );
    exec( "./scripts/playerSprite.cs" );
    exec( "./scripts/AssetGen.cs" );

    // Load GUI profiles.
    exec("./gui/guiProfiles.cs");

    // Create the NyanCat window.
    CreateNyanCatWindow();
    
    // Load and configure the console.
    NyanCat.add( TamlRead("./gui/ConsoleDialog.gui.taml") );
    GlobalActionMap.bind( keyboard, "ctrl tilde", toggleConsole );
    GlobalActionMap.bind( keyboard, "escape", showMainMenu );
    //GlobalActionMap.bind( keyboard, "m", toggleMainMenu );
    
    // Load and configure the toolbox.
    NyanCat.add( TamlRead("./gui/ToolboxDialog.gui.taml") );

    // Load and configure the main overlay.
    NyanCat.add( TamlRead("./gui/MainOverlay.gui.taml") );
    
    // Load and configure the main overlay.
    NyanCat.add( TamlRead("./gui/MainMenu.gui.taml") );
    
    // Initialize the "cannot render" proxy.
    new RenderProxy(CannotRenderProxy)
    {
        Image = "NyanCat:CannotRender";
    };
    NyanCat.add( CannotRenderProxy );
    showMainMenu();
    //makeAssetFiles("^NyanCatAssets/assets/images/", "image");
    //makeAssetFiles("^NyanCatAssets/assets/sprites/", "image");
    //makeAssetFiles("^NyanCatAssets/assets/audio/", "sound");
}

//-----------------------------------------------------------------------------

function NyanCat::destroy( %this )
{
	// stop all sounds
	alxStopAll();
    // Save NyanCat preferences.
    %this.savePreferences();    
    
    // Destroy the NyanCat window.
    destroyNyanCatWindow();
    
    // Destroy the NyanCat scene.
    destroyNyanCatScene();
}

//-----------------------------------------------------------------------------

function NyanCat::loadPreferences( %this )
{
    // Load the default preferences.
    exec( "./scripts/NyanCatPreferences.cs" );
    
    // Load the last session preferences if available.
    if ( isFile("preferences.cs") )
        exec( "preferences.cs" );   
}

//-----------------------------------------------------------------------------

function NyanCat::savePreferences( %this )
{
    // Export only the NyanCat preferences.
    export("$pref::NyanCat::*", "preferences.cs", false );        
    export("$pref::Video::*", "preferences.cs", true );
}

//-----------------------------------------------------------------------------

function NyanCat::reset( %this )
{
    setRandomSeed();
    %this.score = 0;
    // Clear the scene.
    NyanCatScene.clear();    
    
    // Set zero gravity.
    NyanCatScene.setGravity( 0, 0 );
    
    NyanCatScene.setScenePause(false);  

    // Camera Configuration
    NyanCatWindow.setCameraPosition( 0, 0 );
    NyanCatWindow.setCameraAngle( 0 );
    NyanCatWindow.setCameraSize( NyanCat.CameraWidth, NyanCat.CameraHeight );
    %right = NyanCat.WorldWidth / 2;
    %left = -1 * %right;
    NyanCatWindow.setViewLimitOn( (%left), (NyanCat.CameraHeight/-2), (%right), (NyanCat.CameraHeight/2) );
    if( !isObject(NyanCatScore) )
    {
        %obj = new GuiTextCtrl(NyanCatScore){
            Profile = "GuiLargeTextProfile";
            Extent = "50 30";
        };
        NyanCatWindow.addGuiControl(%obj);
        %obj.position = (NyanCatWindow.Extent.x / 2) - (NyanCatScore.Extent.x / 2) SPC 5;
    }
    
    NyanCatScore.text = "0";

    // Background.
    %this.createBackground();
    
    %this.createStars(0, 0, 2, NyanCat.BackgroundDomain);
    
    %this.createWorldLimits();
    
    // Nyan Cat    
    %this.createNyanCat();
}

//-----------------------------------------------------------------------------

function NyanCat::createScene( %this )
{
    NyanCat.CameraWidth = 20;
    NyanCat.CameraHeight = 15;
    NyanCat.WorldWidth = NyanCat.CameraWidth;
    NyanCat.WorldLeft = NyanCat.WorldWidth * -0.5;
    NyanCat.WorldRight = NyanCat.WorldWidth * 0.5;
    NyanCat.BackdropDomain = 31;
    NyanCat.BackgroundDomain = 25;
    NyanCat.ForegroundDomain = 10;    

    NyanCat.RotateCamera = false;

    %this.reset();

    Canvas.popDialog(MainMenu);
}

//-----------------------------------------------------------------------------

function NyanCat::createBackground(%this)
{
    // Atmosphere
    %obj = new Sprite();
    %obj.setBodyType( "static" );
    %obj.setImage( "NyanCatAssets:NightSkyBG" );
    %obj.setSize( NyanCat.WorldWidth * (NyanCat.CameraWidth*2), 75 );
    %obj.setSceneLayer( NyanCat.BackdropDomain );
    %obj.setSceneGroup( NyanCat.BackdropDomain );
    %obj.setCollisionSuppress();
    %obj.setAwake( false );
    %obj.setActive( false );
    NyanCatScene.add( %obj );  
}

// -----------------------------------------------------------------------------

function NyanCat::createStars(%this, %x, %y, %scale, %layer)
{
    // Create an impact explosion at the projectiles position.
    %particlePlayer = new ParticlePlayer();
    %particlePlayer.BodyType = static;
    %particlePlayer.SetPosition( %x, %y );
    %particlePlayer.SetSize(NyanCat.CameraWidth, NyanCat.CameraHeight);
    %particlePlayer.SceneLayer = %layer;
    %particlePlayer.ParticleInterpolation = true;
    %particlePlayer.Particle = "NyanCatAssets:NightStars";
    %particlePlayer.SizeScale = %scale;
    %particlePlayer.EmissionRateScale = "1.0";
    %particlePlayer.ForceScale = "0.0";
    %particlePlayer.CameraIdleDistance = NyanCat.CameraWidth * 1.0;
    NyanCatScene.add( %particlePlayer ); 
    return %particlePlayer;    
}

// -----------------------------------------------------------------------------

function NyanCat::createWorldLimits(%this)
{
    // Create a trigger object to act as the world bounds.
    if( isObject(NyanCatWorldLimits) )
        NyanCatWorldLimits.safeDelete();

    %worldLimits = new Trigger(NyanCatWorldLimits);
    %worldLimits.SetPosition( 0, 0 );
    %worldLimits.SetSize(NyanCat.WorldWidth, NyanCat.CameraHeight);
    %worldLimits.setEnterCallback(false);
    %worldLimits.setStayCallback(false);
    %worldLimits.createPolygonBoxCollisionShape(NyanCat.CameraWidth + 2 SPC NyanCat.CameraHeight);
    %worldLimits.setCollisionShapeIsSensor(0, true);

    if( isObject(NyanCatWorldLimits) )
    {
        //echo(" --- NyanCatWorldLimits: " @ %worldLimits @ " created");
        NyanCatScene.add( %worldLimits ); 
        return %worldLimits;
    }
    return -1;
}

//-----------------------------------------------------------------------------

function NyanCat::createNyanCat(%this)
{
    // Nyan Cat Sprite
    %obj = new Sprite(){
        class = "PlayerSprite";
    };
    %obj.setBodyType( "dynamic" );
    %obj.Animation = "NyanCatAssets:NyanCatAnimation01";
    %obj.setSize( 2, 2 );
    %obj.setPosition( 0, 0 );
    %obj.setSceneLayer( NyanCat.ForegroundDomain );
    %obj.setSceneGroup( NyanCat.ForegroundDomain );
    %obj.setCollisionSuppress(false);
    %obj.setCollisionCallback(true);
    %obj.setAwake( true );
    %obj.setActive( true );
    %obj.createPolygonBoxCollisionShape("1 1");
    %obj.setCollisionShapeFriction(0, 0.0);
    %obj.setCollisionShapeDensity(0, 0.0);
    %obj.setCollisionShapeRestitution(0, 1.0);
    %obj.setLinearDamping(0.0);
    %obj.setUseInputEvents(true);

    %Beh = NyanCatBehavior.createInstance();
    %Beh.sceneWindowObject = NyanCatWindow;

    %obj.addBehavior(%Beh);
    
    NyanCatScene.add( %obj );
    %obj.FixedAngle = true;
    
    %this.NyanCatSprite = %obj;
}

//-----------------------------------------------------------------------------

function NyanCat::loadGame( %this )
{
	%this.gameList = TamlRead("^NyanCat/data/saveData.taml");
}

//-----------------------------------------------------------------------------

function NyanCat::saveGame( %this )
{
	TamlWrite(%this.gameList, "^NyanCat/data/saveData.taml");
}

function exitGame()
{
	GameStateEventManager.postEvent("_QuitRequest", "");
}
